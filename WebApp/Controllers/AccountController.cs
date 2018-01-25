using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Threading.Tasks;
using WebApp.Dal;

namespace WebApp.Controllers
{
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly AzureAdOptions _adOptions;
        private readonly ITenantRepository _tenantRepository;
        private readonly IUserRepository _userRepository;

        public AccountController(IOptions<AzureAdOptions> options, ITenantRepository tenantRepository, IUserRepository userRepository)
        {
            _adOptions = options.Value;
            _tenantRepository = tenantRepository;
            _userRepository = userRepository;
        }

        [HttpGet]
        public IActionResult SignIn()
        {
            var redirectUrl = Url.Page("/Index");
            return Challenge(
                new AuthenticationProperties { RedirectUri = redirectUrl },
                OpenIdConnectDefaults.AuthenticationScheme
            );
        }

        [HttpGet]
        public IActionResult SignOut()
        {
            var callbackUrl = Url.Page("/Account/Welcome", pageHandler: null, values: null, protocol: Request.Scheme);
            return SignOut(
                new AuthenticationProperties { RedirectUri = callbackUrl },
                CookieAuthenticationDefaults.AuthenticationScheme, OpenIdConnectDefaults.AuthenticationScheme
            );
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> SignUp(Dal.Tenant tenant)
        {
            var state = Guid.NewGuid().ToString();
            var request = String.Format(_adOptions.SignUpRequestUri,
                Uri.EscapeDataString(_adOptions.ClientId),
                Uri.EscapeDataString(_adOptions.GraphApiUri),
                Uri.EscapeDataString($"{this.Request.Scheme}://{this.Request.Host}/Account/ProcessSignUpCode"), 
                state, 
                tenant.AdminConsented ? "admin_consent": "consent");

            tenant.StateMarker = state;

            await _tenantRepository.Add(tenant);
            return new RedirectResult(request);
        }

        public async Task<IActionResult> ProcessSignUpCode(string code, string error, string error_description, string resource, string state)
        {
            var tenant = await _tenantRepository.GetByStateMarker(state);
            if (null == tenant)
                return RedirectToPage("/Error");

            var clientCredentials = new ClientCredential(_adOptions.ClientId, _adOptions.ClientSecret);
            var authContext = new AuthenticationContext(_adOptions.Instance);
            var authResult = authContext.AcquireTokenByAuthorizationCodeAsync(code, new Uri($"{this.Request.Scheme}://{this.Request.Host}/Account/ProcessSignUpCode"), clientCredentials).Result;

            var tenantId = Guid.Parse(authResult.TenantId);
            var token = authResult.AccessToken;

            if (tenant.AdminConsented)
            {
                tenant.TenantId = tenantId;
                tenant.Issuer = $"https://sts.windows.net/{tenantId}/";
                await _tenantRepository.Update(tenant);
            }
            else
            {
                if (null == await _userRepository.GetByUpnAndTenantId(authResult.UserInfo.DisplayableId, tenantId))
                {
                    await _userRepository.Add(new User
                    {
                        PrincipalName = authResult.UserInfo.DisplayableId,
                        TenantId = tenantId
                    });
                }

                // Remove tenant entry
                await _tenantRepository.Remove(tenant);
            }

            return this.SignIn();
        }
    }
}

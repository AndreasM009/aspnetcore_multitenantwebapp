using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;

namespace WebApp.Controllers
{
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly AzureAdOptions _adOptions;

        public AccountController(IOptions<AzureAdOptions> options)
        {
            _adOptions = options.Value;
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

        public IActionResult SignUp()
        {
            var state = Guid.NewGuid().ToString();
            var request = String.Format(_adOptions.SignUpRequestUri,
                Uri.EscapeDataString(_adOptions.ClientId),
                Uri.EscapeDataString(_adOptions.GraphApiUri),
                Uri.EscapeDataString($"{this.Request.Scheme}://{this.Request.Host}/Account/ProcessSignUpCode"), 
                state, 
                "consent");
          
            return new RedirectResult(request);
        }

        public IActionResult ProcessSignUpCode(string code, string error, string error_description, string resource, string state)
        {            
            var clientCredentials = new ClientCredential(_adOptions.ClientId, _adOptions.ClientSecret);
            var authContext = new AuthenticationContext(_adOptions.Instance);
            var authResult = authContext.AcquireTokenByAuthorizationCodeAsync(code, new Uri($"{this.Request.Scheme}://{this.Request.Host}/Account/ProcessSignUpCode"), clientCredentials).Result;

            var tenantId = authResult.TenantId;
            var token = authResult.AccessToken;
            return this.SignIn();
        }
    }
}

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace WebApp.Controllers
{
    [Route("[controller]/[action]")]
    public class ApiController : Controller
    {
        private readonly AzureAdOptions _options;

        public ApiController(IOptions<AzureAdOptions> options)
        {
            _options = options.Value;
        }

        [HttpGet]
        public async Task<IActionResult> GetValue()
        {
            var userOid = User.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;
            var authContext = new AuthenticationContext(_options.Instance, new MemoryTokenCache(userOid));
            var cred = new ClientCredential(_options.ClientId, _options.ClientSecret);
            var authResult = await authContext.AcquireTokenSilentAsync(_options.WebApiUri, cred, new UserIdentifier(userOid, UserIdentifierType.UniqueId));

            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", authResult.AccessToken);
            var result = await httpClient.GetStringAsync("https://localhost:44355/api/Values");

            return Ok(result);
        }
    }
}

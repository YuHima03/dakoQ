using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace Dakoq.WebApp.Controllers
{
    [ApiController]
    [Route("/api/auth/traq")]
    public class TraqAuthController(IOptions<AppConfiguration> config, IHttpClientFactory httpClientFactory) : ControllerBase
    {
        readonly IOptions<AppConfiguration> _config = config;
        readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> CallbackAsync([FromQuery(Name = "code")] string? code)
        {
            var traqOAuthClient = _config.Value.TraqOAuthClientInfo;
            if (traqOAuthClient?.ClientId is null)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            Traq.Api.Oauth2Api traqOAuthApi = new(_httpClientFactory.CreateClient("traQ"));
            var res = await traqOAuthApi.PostOAuth2TokenWithHttpInfoAsync(
                grantType: "authorization_code",
                clientId: traqOAuthClient.ClientId,
                code: code
            );
            if (res.StatusCode != System.Net.HttpStatusCode.OK)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            var token = res.Data;
            ClaimsIdentity identity = new(
                claims: [
                    new Claim(ClaimTypes.UserData, token.AccessToken),
                    new Claim(ClaimTypes.Expiration, (DateTimeOffset.Now+TimeSpan.FromSeconds(token.ExpiresIn)).ToString())
                ],
                authenticationType: CookieAuthenticationDefaults.AuthenticationScheme
            );

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity)
            );
            return Redirect("/");
        }

        [Route("")]
        [HttpGet]
        public IActionResult IndexAsync()
        {
            var conf = _config.Value;

            if (conf.TraqApiBaseAddress is null || conf.TraqOAuthClientInfo?.ClientId is null)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            var query = QueryString.Create([
                KeyValuePair.Create<string, string?>("response_type", "code"),
                KeyValuePair.Create<string, string?>("client_id", conf.TraqOAuthClientInfo.ClientId),
                KeyValuePair.Create<string, string?>("scope", "read")
                ]);

            UriBuilder ub = new(conf.TraqApiBaseAddress);
            ub.Path = Path.Combine(ub.Path, "oauth2/authorize");
            ub.Query = query.ToString();

            return Redirect(ub.ToString());
        }
    }
}

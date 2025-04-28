using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace Dakoq.WebApp.Controllers.Authentication
{
    [ApiController]
    [Route("/api/auth/traq")]
    public class TraqAuthController(
        IOptions<AppConfiguration> config,
        IHttpClientFactory httpClientFactory,
        ILogger<TraqAuthController> logger
        ) : ControllerBase
    {
        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> CallbackAsync([FromQuery(Name = "code")] string? code)
        {
            var conf = config.Value;
            var traqOAuthClient = conf.TraqOAuthClientInfo;
            if (traqOAuthClient?.ClientId is null)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable);
            }

            Traq.Api.Oauth2Api traqOAuthApi = new(httpClientFactory.CreateClient("traQ"));
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
            if (token.TokenType != "Bearer")
            {
                logger.LogError("Invalid token type: {TokenType}", token.TokenType);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            Traq.ITraqApiClient traqClient = new Traq.TraqApiClient(Options.Create(new Traq.TraqApiClientOptions()
            {
                BaseAddress = conf.TraqApiBaseAddress?.ToString() ?? "",
                BearerAuthToken = token.AccessToken
            }));

            Traq.Model.MyUserDetail traqUser;
            try
            {
                traqUser = await traqClient.MeApi.GetMeAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to get user info.");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            if (token.AccessToken is null)
            {
                logger.LogError("Access token is null");
                return StatusCode(StatusCodes.Status500InternalServerError);
            }

            ClaimsIdentity identity = new(
                claims: [
                    new Claim(ClaimTypes.Name, traqUser.Name),
                    new Claim(ClaimTypes.NameIdentifier, traqUser.Id.ToString()),
                    new Claim(ClaimTypes.UserData, token.AccessToken),
                    new Claim(ClaimTypes.Expiration, (DateTimeOffset.Now+TimeSpan.FromSeconds(token.ExpiresIn)).ToString())
                ],
                authenticationType: CookieAuthenticationDefaults.AuthenticationScheme
            );

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(identity),
                new AuthenticationProperties
                {
                    IsPersistent = true,
                    ExpiresUtc = DateTimeOffset.UtcNow.AddSeconds(Math.Min(token.ExpiresIn, TimeSpan.SecondsPerDay * 7)), // Expires in 7 days at most.
                }
            );
            return Redirect("/");
        }

        [Route("")]
        [HttpGet]
        public IActionResult IndexAsync()
        {
            var conf = config.Value;

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

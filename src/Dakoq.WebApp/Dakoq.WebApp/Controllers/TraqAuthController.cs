using Microsoft.AspNetCore.Mvc;

namespace Dakoq.WebApp.Controllers
{
    [ApiController]
    [Route("/auth/traq")]
    public class TraqAuthController(IHttpClientFactory httpClientFactory) : ControllerBase
    {
        readonly IHttpClientFactory _httpClientFactory = httpClientFactory;

        [Route("[action]")]
        [HttpGet]
        public async Task<IActionResult> CallbackAsync([FromQuery(Name = "code")] string? code)
        {
            return Forbid();

            Traq.Api.Oauth2Api traqOAuthApi = new(_httpClientFactory.CreateClient("traQ"));
            var token = await traqOAuthApi.PostOAuth2TokenWithHttpInfoAsync(
                grantType: "authorization_code",
                clientId: "",
                code: code
            );

            return Ok();
        }
    }
}

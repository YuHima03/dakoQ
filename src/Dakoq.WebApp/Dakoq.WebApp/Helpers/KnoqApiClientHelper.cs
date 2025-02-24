using Microsoft.Extensions.Options;

namespace Dakoq.WebApp.Helpers
{
    internal static class KnoqApiClientHelper
    {
        public static async Task<Knoq.KnoqApiClient?> CreateClientAsync(AppConfiguration appConf)
        {
            if (appConf.KnoqAuthInfo is null)
            {
                return null;
            }

            HttpClientHandler clientHandler = new()
            {
                UseCookies = true,
                AllowAutoRedirect = false
            };
            HttpClient client = new(clientHandler);

            Traq.Client.Configuration traqApiConfig = new() { BasePath = appConf.TraqApiBaseAddress?.ToString() ?? "https://q.trap.jp/api/v3" };
            Knoq.Client.Configuration knoqApiConfig = new() { BasePath = appConf.KnoqApiBaseAddress?.ToString() ?? "https://knoq.trap.jp/api" };

            using CancellationTokenSource cts = new();

            {
                Traq.Api.AuthenticationApi authApi = new(client, traqApiConfig, clientHandler);
                Traq.Model.PostLoginRequest req = new(
                    name: appConf.KnoqAuthInfo.TraqUsername ?? "",
                    password: appConf.KnoqAuthInfo.TraqPassword ?? ""
                );
                await authApi.LoginAsync(null, req, cts.Token);
            }

            Uri? oauthUri = null; // Should be https://q.trap.jp/api/v3/oauth2/authorize
            {
                Knoq.Api.AuthenticationApi authApi = new(client, knoqApiConfig, clientHandler);
                var authParams = await authApi.GetAuthParamsAsync(cts.Token);
                oauthUri = new(authParams.Url);
            }

            {
                var res = await client.GetAsync(oauthUri, cts.Token); // Should be redirected to https://q.trap.jp/consent
                if (res.StatusCode == System.Net.HttpStatusCode.Found)
                {
                    if (res.Headers.Location is Uri location)
                    {
                        var redirectRes = await client.GetAsync(location.IsAbsoluteUri ? location : new Uri(new Uri($"{oauthUri.Scheme}://{oauthUri.Host}"), location.ToString()), cts.Token);
                        if (!redirectRes.IsSuccessStatusCode)
                        {
                            throw new Exception("Failed to access redirect url.");
                        }
                    }
                }
            }

            {
                Traq.Api.Oauth2Api oauth2Api = new(client, traqApiConfig, clientHandler);
                var res = await oauth2Api.PostOAuth2AuthorizeDecideWithHttpInfoAsync("approve", cts.Token);
                if (res.StatusCode == System.Net.HttpStatusCode.Found)
                {
                    if (Uri.TryCreate(res.Headers["Location"].FirstOrDefault(), UriKind.RelativeOrAbsolute, out var location))
                    {
                        var redirectRes = await client.GetAsync(location, cts.Token); // Should be redirected to https://knoq.trap.jp/api/callback
                        if (redirectRes.StatusCode != System.Net.HttpStatusCode.Found && !redirectRes.IsSuccessStatusCode)
                        {
                            throw new Exception("Http request failed.");
                        }
                    }
                }
            }

            UriBuilder cookieUri = new(knoqApiConfig.BasePath)
            {
                Fragment = "",
                Path = "",
                Query = ""
            };

            return new Knoq.KnoqApiClient(Options.Create(new Knoq.KnoqApiClientOptions()
            {
                BaseAddress = knoqApiConfig.BasePath,
                CookieAuthToken = clientHandler.CookieContainer.GetCookies(cookieUri.Uri).Where(c => c.Name == "session").First().Value
            }));
        }
    }
}

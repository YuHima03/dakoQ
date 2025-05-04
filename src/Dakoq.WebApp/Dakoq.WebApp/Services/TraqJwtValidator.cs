using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace Dakoq.WebApp.Services
{
    public sealed class TraqJwtValidator(
        ILogger<TraqJwtValidator> logger,
        IHttpClientFactory httpClientFactory,
        IOptions<TraqJwtValidatorOptions> options,
        IOptions<Traq.TraqApiClientOptions> traqOptions)
    {
        readonly TraqJwtValidatorOptions _options = options.Value;
        readonly Uri _traqOidcDiscoveryUri = new UriBuilder(traqOptions.Value.BaseAddress) { Fragment = "", Path = ".well-known/openid-configuration", Query = "" }.Uri;

        DateTime _jwksLastUpdatedAt = DateTime.MinValue;
        readonly SemaphoreSlim _sem = new(1, 1);
        readonly JsonWebTokenHandler _tokenHandler = new();
        readonly TokenValidationParameters _tokenValidationParams = new()
        {
            IssuerSigningKeys = [],
            ValidateAudience = false,
            ValidateActor = false,
            ValidateIssuer = false,
            ValidateIssuerSigningKey = false,
            ValidateLifetime = false,
            ValidateSignatureLast = false,
            ValidateTokenReplay = false,
            ValidateWithLKG = false,
            LogValidationExceptions = true
        };

        async ValueTask<JsonWebKeySet?> GetJwksAsync(CancellationToken ct)
        {
            using var client = httpClientFactory.CreateClient();
            var oidcConfig = await client.GetFromJsonAsync<OpenIdConnectConfiguration>(_traqOidcDiscoveryUri, ct);
            if (oidcConfig is null || oidcConfig.JwksUri is null)
            {
                return null;
            }
            return await client.GetFromJsonAsync<JsonWebKeySet>(oidcConfig.JwksUri, ct);
        }

        public async ValueTask<TraqJwtPayload?> ValidateAsync(string encodedToken, CancellationToken ct)
        {
            TokenValidationResult validationResult;

            await _sem.WaitAsync(ct);
            try
            {
                if (!_tokenValidationParams.IssuerSigningKeys.Any() || DateTime.UtcNow.Subtract(_jwksLastUpdatedAt) > _options.KeysValidPeriod)
                {
                    JsonWebKeySet? jwks;
                    try
                    {
                        jwks = await GetJwksAsync(ct);
                    }
                    catch (Exception e)
                    {
                        logger.LogError(e, "Failed to get JWKs.");
                        return null;
                    }
                    _tokenValidationParams.IssuerSigningKeys = jwks?.Keys ?? [];
                    _jwksLastUpdatedAt = DateTime.UtcNow;
                }

                validationResult = await _tokenHandler.ValidateTokenAsync(encodedToken, _tokenValidationParams);
                if (!validationResult.IsValid)
                {
                    logger.LogError(validationResult.Exception, "Failed to validate token.");
                    return null;
                }

                var utcNow = DateTime.UtcNow;
                var token = (JsonWebToken)validationResult.SecurityToken;
                if (utcNow < token.ValidFrom || token.ValidTo < utcNow)
                {
                    return null;
                }

                var claims = token.Claims.ToDictionary(c => c.Type, c => c.Value);
                return new TraqJwtPayload
                {
                    DisplayName = claims["displayName"],
                    Name = claims["name"],
                    UserId = Guid.Parse(claims["userId"]),
                };
            }
            catch (Exception e)
            {
                logger.LogError(e, "Failed to validate token.");
                return null;
            }
            finally
            {
                _sem.Release();
            }
        }
    }

    public sealed class TraqJwtValidatorOptions
    {
        public TimeSpan KeysValidPeriod { get; set; } = TimeSpan.FromMinutes(5);
    }

    public sealed class TraqJwtPayload
    {
        public required string DisplayName { get; init; }

        public required string Name { get; init; }

        public Guid UserId { get; init; }
    }
}

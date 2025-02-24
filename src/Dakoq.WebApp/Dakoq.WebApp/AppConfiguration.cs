namespace Dakoq.WebApp
{
    public class AppConfiguration
    {
        public Uri? DakoqBaseAddress { get; set; }

        public Uri? KnoqApiBaseAddress { get; set; }

        public Uri? TraqApiBaseAddress { get; set; }

        public KnoqAuthenticationInfo? KnoqAuthInfo { get; set; }

        public TraqOAuthClientInfo? TraqOAuthClientInfo { get; set; }

        public string? DbConnectionString { get; set; }

        Lazy<Uri> _traqOAuthRedirectUri;
        public Uri TraqOAuthRedirectUri => _traqOAuthRedirectUri.Value;

        public AppConfiguration()
        {
            _traqOAuthRedirectUri = new(() => new Uri(DakoqBaseAddress ?? new Uri("http://localhost"), "api/auth/traq/callback"));
        }
    }

    public sealed class KnoqAuthenticationInfo
    {
        public string? TraqUsername { get; set; }

        public string? TraqPassword { get; set; }
    }

    public sealed class TraqOAuthClientInfo
    {
        public string? ClientId { get; set; }

        public string? ClientSecret { get; set; }
    }
}

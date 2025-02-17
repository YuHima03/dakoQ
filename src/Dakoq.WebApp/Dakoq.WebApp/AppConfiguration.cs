namespace Dakoq.WebApp
{
    public class AppConfiguration
    {
        public Uri? DakoqBaseAddress { get; set; }

        public Uri? TraqApiBaseAddress { get; set; }

        public KnoqAuthenticationInfo? KnoqAuthInfo { get; set; }

        public string? DbConnectionString { get; set; }
    }

    public sealed class KnoqAuthenticationInfo
    {
        public string? TraqUsername { get; set; }

        public string? TraqPassword { get; set; }
    }
}

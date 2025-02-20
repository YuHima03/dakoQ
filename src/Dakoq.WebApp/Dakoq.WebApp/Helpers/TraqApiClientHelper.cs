namespace Dakoq.WebApp.Helpers
{
    internal static class TraqApiClientHelper
    {
        public static string? GetIconUrl(AppConfiguration appConf, string username)
        {
            return new Uri(appConf.TraqApiBaseAddress!, $"public/icon/{username}").ToString();
        }
    }
}

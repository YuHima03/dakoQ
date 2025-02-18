using System.Diagnostics.CodeAnalysis;

namespace Dakoq.WebApp
{
    public class AuthUserInfo
    {
        public Guid Id { get; set; }

        [NotNull]
        public string? Name { get; set; }

        [NotNull]
        public string? AccessToken { get; set; }

        public DateTimeOffset TokenExpiresAt { get; set; }
    }
}

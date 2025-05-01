namespace Dakoq.Domain.Models
{
    public sealed record class Knoq1Room(
        Guid Id,
        string Name,
        bool IsVerified,
        DateTimeOffset StartsAt,
        DateTimeOffset EndsAt,
        DateTimeOffset CreatedAt,
        DateTimeOffset UpdatedAt
        );
}

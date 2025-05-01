namespace Dakoq.Domain.Models
{
    public sealed record class Knoq1Event(
        Guid Id,
        string Name,
        Knoq1Room? Room,
        DateTimeOffset StartsAt,
        DateTimeOffset EndsAt,
        DateTimeOffset CreatedAt,
        DateTimeOffset UpdatedAt
        );
}

namespace Dakoq.Domain.Models
{
    public sealed record class KnoqV1Event(
        Guid Id,
        string Name,
        KnoqV1Room? Room,
        DateTimeOffset StartsAt,
        DateTimeOffset EndsAt,
        DateTimeOffset CreatedAt,
        DateTimeOffset UpdatedAt
        );
}

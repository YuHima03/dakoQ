namespace Dakoq.Domain.Models
{
    public record struct RoomOpeningHours(
        DateTimeOffset? StartsAt,
        DateTimeOffset? EndsAt
        );
}

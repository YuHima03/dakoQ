namespace Dakoq.Domain.Models
{
    public record struct RoomHours(
        DateTimeOffset? StartsAt,
        DateTimeOffset? EndsAt
        );
}

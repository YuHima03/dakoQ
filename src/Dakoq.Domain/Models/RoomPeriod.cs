namespace Dakoq.Domain.Models
{
    public record struct RoomPeriod(
        DateTimeOffset? StartsAt,
        DateTimeOffset? EndsAt
        );
}

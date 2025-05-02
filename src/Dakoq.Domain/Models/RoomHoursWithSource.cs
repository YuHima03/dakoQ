namespace Dakoq.Domain.Models
{
    public record struct RoomHoursWithSource(
        DateTimeOffset? StartsAt,
        DateTimeOffset? EndsAt,
        RoomSource? Source
        );
}

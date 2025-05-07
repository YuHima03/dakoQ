namespace Dakoq.Domain.Models
{
    public record struct PostRoomHoursRequest(
        DateTimeOffset? StartsAt,
        DateTimeOffset? EndsAt,
        PostRoomSourceRequest? Source
        );
}

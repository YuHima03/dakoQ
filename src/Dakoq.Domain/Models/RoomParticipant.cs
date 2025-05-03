namespace Dakoq.Domain.Models
{
    public record struct RoomParticipant(
        Guid UserId,
        DateTimeOffset JoinedAt,
        DateTimeOffset? LeftAt
        );
}

namespace Dakoq.Domain.Models
{
    public record struct UserParticipation(
        Guid RoomId,
        DateTimeOffset JoinedAt,
        DateTimeOffset? LeftAt
        );
}

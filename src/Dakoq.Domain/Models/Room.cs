namespace Dakoq.Domain.Models
{
    public sealed record class Room(
        Guid Id,
        string Name,
        string? Description,
        RoomSource? Source,
        RoomPeriod? CurrentPeriod,
        RoomParticipant[] Participants,
        DateTimeOffset CreatedAt,
        DateTimeOffset UpdatedAt
        );
}

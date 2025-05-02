namespace Dakoq.Domain.Models
{
    public sealed record PostRoomRequest(
        string Name,
        string? Description,
        RoomHours[] OpeningHours,
        RoomSource[] Sources
        );
}

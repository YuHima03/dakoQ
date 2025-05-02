namespace Dakoq.Domain.Models
{
    public sealed record PostRoomRequest(
        string Name,
        string? Description,
        RoomOpeningHours[] OpeningHours,
        RoomSource[] Sources
        );
}

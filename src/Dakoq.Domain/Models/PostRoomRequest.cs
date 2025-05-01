namespace Dakoq.Domain.Models
{
    public sealed record PostRoomRequest(
        string Name,
        string? Description,
        RoomPeriod[] Periods,
        RoomSource[] Sources
        );
}

namespace Dakoq.Domain.Models
{
    public sealed record class RoomSource(
        Knoq1Event? Knoq1Event,
        Knoq1Room? Knoq1Room
        );
}

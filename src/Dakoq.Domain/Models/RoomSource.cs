namespace Dakoq.Domain.Models
{
    public sealed record class RoomSource(
        KnoqV1Event? KnoqV1Event,
        KnoqV1Room? KnoqV1Room
        );
}

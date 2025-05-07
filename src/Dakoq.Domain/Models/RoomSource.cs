namespace Dakoq.Domain.Models
{
    public record struct RoomSource(
        Guid? KnoqV1EventId,
        Guid? KnoqV1RoomId
        )
    {
        public static RoomSource CreateKnoeV1Event(Guid eventId) => new(eventId, null);

        public static RoomSource CreateKnoeV1Room(Guid roomId) => new(null, roomId);
    }
}

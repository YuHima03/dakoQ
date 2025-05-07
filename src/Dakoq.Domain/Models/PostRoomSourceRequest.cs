namespace Dakoq.Domain.Models
{
    public record struct PostRoomSourceRequest(
        PostKnoqV1EventRequest? KnoqV1Event,
        PostKnoqV1RoomRequest? KnoqV1Room
        )
    {
        public static PostRoomSourceRequest Create(PostKnoqV1EventRequest? eventRequest) => new(eventRequest, null);

        public static PostRoomSourceRequest Create(PostKnoqV1RoomRequest? roomRequest) => new(null, roomRequest);
    }
}

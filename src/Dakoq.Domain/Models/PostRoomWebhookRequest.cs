namespace Dakoq.Domain.Models
{
    public sealed record class PostRoomWebhookRequest(
        Guid OwnerId,
        Guid RoomId
        );
}

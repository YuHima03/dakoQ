namespace Dakoq.Domain.Models
{
    public sealed record class PostRoomWebhookResult(
        uint Id,
        Guid OwnerId,
        Guid RoomId,
        string PlainSecret,
        DateTimeOffset CreatedAt
        );
}

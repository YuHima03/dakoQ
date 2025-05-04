namespace Dakoq.Domain.Models
{
    public sealed record class PostRoomWebhookResult(
        Guid Id,
        Guid OwnerId,
        Guid RoomId,
        string PlainSecret,
        DateTimeOffset CreatedAt
        );
}

namespace Dakoq.Domain.Models
{
    public sealed record class RoomWebhook(
        Guid Id,
        Guid OwnerId,
        Guid RoomId,
        string HashedSecret,
        DateTime CreatedAt
        );
}

namespace Dakoq.Domain.Models
{
    public sealed record class RoomWebhook(
        uint Id,
        Guid OwnerId,
        Guid RoomId,
        string HashedSecret,
        DateTime CreatedAt
        );
}

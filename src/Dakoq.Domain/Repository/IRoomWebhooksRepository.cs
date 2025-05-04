namespace Dakoq.Domain.Repository
{
    public interface IRoomWebhooksRepository
    {
        public ValueTask DeleteRoomWebhookAsync(Guid id, CancellationToken ct);

        public ValueTask<Models.RoomWebhook> GetRoomWebhookAsync(Guid id, CancellationToken ct);

        public ValueTask<Models.RoomWebhook[]> GetUsersRoomWebhooksAsync(Guid ownerId, CancellationToken ct);

        public ValueTask<Models.PostRoomWebhookResult> PostRoomWebhookAsync(Models.PostRoomWebhookRequest request, CancellationToken ct);

        public ValueTask<Models.RoomWebhook> TransferOwnershipAsync(Guid roomId, Guid newOwnerId, CancellationToken ct);

        public ValueTask<bool> VerifySecretAsync(Guid id, string secret, CancellationToken ct);
    }
}

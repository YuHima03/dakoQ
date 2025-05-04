using Dakoq.Domain.Models;
using Dakoq.Domain.Repository;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace Dakoq.Infrastructure.Repository
{
    public sealed partial class Repository : IRoomWebhooksRepository
    {
        public ValueTask DeleteRoomWebhookAsync(Guid id, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public async ValueTask<RoomWebhook> GetRoomWebhookAsync(Guid id, CancellationToken ct)
        {
            return await RoomWebhooks
                .Where(r => r.Id == id)
                .Select(r => new RoomWebhook(r.Id, r.OwnerId, r.RoomId, r.HashedSecret, r.CreatedAt))
                .FirstAsync(ct);
        }

        public async ValueTask<RoomWebhook[]> GetUsersRoomWebhooksAsync(Guid ownerId, CancellationToken ct)
        {
            return await RoomWebhooks
                .Where(r => r.OwnerId == ownerId)
                .Select(r => new RoomWebhook(r.Id, r.OwnerId, r.RoomId, r.HashedSecret, r.CreatedAt))
                .ToArrayAsync(ct);
        }

        public async ValueTask<PostRoomWebhookResult> PostRoomWebhookAsync(PostRoomWebhookRequest request, CancellationToken ct)
        {
            var (plainSecret, hashedSecret) = RoomWebhooksRepositoryHelper.GenerateSecret();
            Models.RoomWebhook rw = new()
            {
                Id = Guid.CreateVersion7(),
                RoomId = request.RoomId,
                OwnerId = request.OwnerId,
                HashedSecret = hashedSecret
            };
            await RoomWebhooks.AddAsync(rw, ct);
            await SaveChangesAsync(ct);

            return new PostRoomWebhookResult(rw.Id, rw.OwnerId, rw.RoomId, plainSecret, rw.CreatedAt);
        }

        public ValueTask<RoomWebhook> TransferOwnershipAsync(Guid roomId, Guid newOwnerId, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public async ValueTask<bool> VerifySecretAsync(Guid id, string secret, CancellationToken ct)
        {
            var w = await RoomWebhooks.Where(r => r.Id == id).SingleOrDefaultAsync(ct);
            if (w is null)
            {
                return false;
            }
            return RoomWebhooksRepositoryHelper.ValidateSecret(w, secret);
        }
    }

    file static class RoomWebhooksRepositoryHelper
    {
        static readonly byte[] SecretStringChars = [.. "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz"u8];

        const int SecretLength = 32;

        public static (string Plain, string Hash) GenerateSecret()
        {
            Span<byte> plainBytes = stackalloc byte[SecretLength];
            RandomNumberGenerator.GetItems(SecretStringChars, plainBytes);

            Span<byte> hashBytes = stackalloc byte[SHA256.HashSizeInBytes]; // 32bytes (64 hex chars)
            SHA256.HashData(plainBytes, hashBytes);

            return (
                string.Create(SecretLength, plainBytes, FromAsciiString),
                Convert.ToHexStringLower(hashBytes) // 64 hex chars
                );
        }

        public static bool ValidateSecret(Models.RoomWebhook roomWebhook, string plainSecret)
        {
            return HashEqualsTo(plainSecret, roomWebhook.HashedSecret);
        }

        static bool HashEqualsTo(string plain, string expectedHash)
        {
            Span<byte> plainBytes = stackalloc byte[plain.Length];
            Encoding.UTF8.GetBytes(plain, plainBytes);

            Span<byte> expectedHashBytes = stackalloc byte[expectedHash.Length / 2];
            Convert.FromHexString(expectedHash, expectedHashBytes, out _, out _);

            Span<byte> hashResultBytes = stackalloc byte[SHA256.HashSizeInBytes];
            SHA256.HashData(plainBytes, hashResultBytes);
            return hashResultBytes.SequenceEqual(expectedHashBytes);
        }

        static void FromAsciiString(this Span<char> s, Span<byte> b)
        {
            for (int i = 0; i < b.Length; i++)
            {
                s[i] = (char)b[i];
            }
        }
    }
}

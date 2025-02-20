using Dakoq.Repository.Exceptions;
using Dakoq.Repository.Models;
using Microsoft.EntityFrameworkCore;

namespace Dakoq.Repository
{
    public sealed partial class RepositoryContext : IRoomParticipantsRepository
    {
        public async ValueTask<RoomParticipant> JoinToRoomAsync(Guid userId, Guid roomId, CancellationToken ct = default)
        {
            using var tx = (Database.CurrentTransaction is null) ? (await Database.BeginTransactionAsync(ct)) : null;
            try
            {
                var utcNow = DateTimeOffset.UtcNow;
                var current = await RoomParticipants.Where(r => r.ParticipantId == userId && r.LeftAt == null).SingleOrDefaultAsync(ct);
                if (current is not null)
                {
                    if (current.RoomId == roomId)
                    {
                        return current;
                    }
                    current.LeftAt = utcNow;
                }

                RoomParticipant next = new()
                {
                    Id = Guid.CreateVersion7(),
                    ParticipantId = userId,
                    RoomId = roomId,
                    JoinedAt = utcNow
                };
                var entity = await RoomParticipants.AddAsync(next, ct);
                await SaveChangesAsync(ct);

                entity.State = EntityState.Detached;
                await (tx?.CommitAsync(ct) ?? Task.CompletedTask);
                return entity.Entity;
            }
            catch
            {
                await (tx?.RollbackAsync(ct) ?? Task.CompletedTask);
                throw;
            }
        }

        public async ValueTask<RoomParticipant> LeaveFromRoomAsync(Guid userId, CancellationToken ct = default)
        {
            using var tx = (Database.CurrentTransaction is null) ? (await Database.BeginTransactionAsync(ct)) : null;
            try
            {
                var utcNow = DateTimeOffset.UtcNow;
                var current = await RoomParticipants.Where(r => r.ParticipantId == userId && r.LeftAt == null).SingleOrDefaultAsync(ct)
                    ?? throw new SqlRowNotFoundException();

                current.LeftAt = utcNow;
                await SaveChangesAsync(ct);
                await (tx?.CommitAsync(ct) ?? Task.CompletedTask);
                return current;
            }
            catch
            {
                await (tx?.RollbackAsync(ct) ?? Task.CompletedTask);
                throw;
            }
        }
    }
}

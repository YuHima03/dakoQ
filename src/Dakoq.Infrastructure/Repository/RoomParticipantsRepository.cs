using Dakoq.Domain.Models;
using Dakoq.Domain.Repository;
using Microsoft.EntityFrameworkCore;

namespace Dakoq.Infrastructure.Repository
{
    public sealed partial class Repository : IRoomParticipantsRepository
    {
        public async ValueTask<UserParticipation?> GetUserCurrentParticipationAsync(Guid userId, CancellationToken ct)
        {
            return await RoomParticipants
                .Where(r => r.LeftAt == null && r.UserId == userId)
                .Select(r => new UserParticipation(r.RoomId, r.JoinedAt, null))
                .SingleOrDefaultAsync(ct);
        }

        public async ValueTask<UserParticipation> JoinToOrLeaveFromRoomAsync(Guid roomId, Guid userId, CancellationToken ct)
        {
            await using var tx = await Database.BeginTransactionAsync(ct);
            try
            {
                var prevParticipation = await RoomParticipants
                    .Where(r => r.LeftAt == null && r.UserId == userId)
                    .SingleOrDefaultAsync(ct);

                if (prevParticipation is not null && prevParticipation.RoomId == roomId)
                {
                    prevParticipation.LeftAt = DateTime.UtcNow;
                    await SaveChangesAsync(ct);
                    await tx.CommitAsync(ct);
                    return new UserParticipation(prevParticipation.RoomId, prevParticipation.JoinedAt, prevParticipation.LeftAt);
                }

                Models.RoomParticipant participation = new()
                {
                    RoomId = roomId,
                    UserId = userId
                };
                await RoomParticipants.AddAsync(participation, ct);
                await SaveChangesAsync(ct);

                if (prevParticipation is not null)
                {
                    prevParticipation.LeftAt = participation.JoinedAt;
                    await SaveChangesAsync(ct);
                }

                await tx.CommitAsync(ct);
                return new UserParticipation(participation.RoomId, participation.JoinedAt, null);
            }
            catch
            {
                await tx.RollbackAsync(ct);
                throw;
            }
        }

        public async ValueTask<UserParticipation> JoinToRoomAsync(Guid roomId, Guid userId, CancellationToken ct)
        {
            await using var tx = await Database.BeginTransactionAsync(ct);
            try
            {
                var prevParticipation = await RoomParticipants
                    .Where(r => r.LeftAt == null && r.UserId == userId)
                    .SingleOrDefaultAsync(ct);
                if (prevParticipation is not null && prevParticipation.RoomId == roomId)
                {
                    return new UserParticipation(prevParticipation.RoomId, prevParticipation.JoinedAt, null);
                }

                Models.RoomParticipant participation = new()
                {
                    RoomId = roomId,
                    UserId = userId
                };
                await RoomParticipants.AddAsync(participation, ct);
                await SaveChangesAsync(ct);

                if (prevParticipation is not null)
                {
                    prevParticipation.LeftAt = participation.JoinedAt;
                    await SaveChangesAsync(ct);
                }

                await tx.CommitAsync(ct);
                return new UserParticipation(participation.RoomId, participation.JoinedAt, null);
            }
            catch
            {
                await tx.RollbackAsync(ct);
                throw;
            }
        }

        public async ValueTask<UserParticipation> LeaveFromRoomAsync(Guid userId, CancellationToken ct)
        {
            await using var tx = await Database.BeginTransactionAsync(ct);
            try
            {
                var participation = await RoomParticipants
                    .Where(r => r.LeftAt == null && r.UserId == userId)
                    .SingleOrDefaultAsync(ct) ?? throw new InvalidOperationException("User is not in any room");

                participation.LeftAt = DateTime.UtcNow;
                await SaveChangesAsync(ct);

                await tx.CommitAsync(ct);
                return new UserParticipation(participation.RoomId, participation.JoinedAt, participation.LeftAt);
            }
            catch
            {
                await tx.RollbackAsync(ct);
                throw;
            }
        }
    }
}

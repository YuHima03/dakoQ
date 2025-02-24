using Dakoq.Repository.Exceptions;
using Dakoq.Repository.Models;
using Microsoft.EntityFrameworkCore;

namespace Dakoq.Repository
{
    public sealed partial class RepositoryContext : IRoomsRepository
    {
        public async ValueTask<Room> GetRoomAsync(Guid id, CancellationToken ct = default)
        {
            return await Rooms
                .Where(r => r.Id == id)
                .SingleOrDefaultAsync(ct)
                ?? throw new SqlRowNotFoundException();
        }

        public async ValueTask<Room> GetAvailableRoomAsync(string alias, CancellationToken ct = default)
        {
            var utcNow = DateTimeOffset.UtcNow;
            return await Rooms
                .Where(r => (r.StartsAt == null || r.StartsAt <= utcNow) && (r.EndsAt == null || utcNow <= r.EndsAt) && r.Alias == alias)
                .SingleOrDefaultAsync(ct)
                ?? throw new SqlRowNotFoundException();
        }

        public async ValueTask<List<Room>> GetAvailableRoomsAsync(CancellationToken ct = default)
        {
            var utcNow = DateTimeOffset.UtcNow;
            return await Rooms.Where(r => (r.StartsAt == null || r.StartsAt <= utcNow) && (r.EndsAt == null || utcNow <= r.EndsAt)).ToListAsync(ct);
        }

        public async ValueTask<Room> PostRoomAsync(PostRoomRequest req, CancellationToken ct = default)
        {
            var newTx = Database.CurrentTransaction is null;
            var tx = Database.CurrentTransaction ?? await Database.BeginTransactionAsync(ct);
            try
            {
                Room room = new();
                await ConfigureRoomAsync(room, req, ct);
                var entity = await Rooms.AddAsync(room, ct);
                await SaveChangesAsync(ct);

                if (newTx)
                {
                    await tx.CommitAsync(ct);
                }
                entity.State = EntityState.Detached;
                return entity.Entity;
            }
            catch
            {
                if (newTx)
                {
                    await tx.RollbackAsync(ct);
                }
                throw;
            }
        }

        async ValueTask ConfigureRoomAsync(Room room, PostRoomRequest req, CancellationToken ct = default)
        {
            if (req.Id is null && req.DataSource != Domain.RoomDataSources.Dakoq)
            {
                throw new ArgumentException("The id must be set if the data source is not \"dakoQ\"", nameof(req));
            }
            if (string.IsNullOrEmpty(req.Name))
            {
                throw new ArgumentException("The name must be set", nameof(req));
            }
            if (req.StartsAt is not null && req.EndsAt is not null && req.EndsAt <= req.StartsAt)
            {
                throw new ArgumentException("Invalid time period.", nameof(req));
            }

            var dataSource = await GetRoomDataSourceAsync(req.DataSource.ToString(), ct);
            if (!dataSource.IsActive)
            {
                throw new InvalidOperationException("The specified data source is not active");
            }

            room.Id = req.Id ?? Guid.CreateVersion7();
            room.Name = req.Name;
            room.DataSourceId = dataSource.Id;
            room.Alias = req.Alias;
            room.StartsAt = req.StartsAt;
            room.EndsAt = req.EndsAt;
        }
    }
}

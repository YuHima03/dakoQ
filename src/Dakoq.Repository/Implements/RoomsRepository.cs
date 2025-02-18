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

        public ValueTask<Room> PostRoomAsync(PostRoomRequest req, CancellationToken ct = default)
        {
            throw new NotImplementedException();
        }
    }
}

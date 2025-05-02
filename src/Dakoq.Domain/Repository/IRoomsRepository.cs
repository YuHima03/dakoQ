using Dakoq.Domain.Models;

namespace Dakoq.Domain.Repository
{
    public interface IRoomsRepository
    {
        public ValueTask<Room[]> GetAvailableRooms(DateTimeOffset time, CancellationToken ct);
        public ValueTask<Room[]> GetAvailableRooms(DateTimeOffset? after, DateTimeOffset? before, CancellationToken ct);
    }
}

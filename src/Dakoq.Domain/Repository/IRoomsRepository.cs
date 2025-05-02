using Dakoq.Domain.Models;

namespace Dakoq.Domain.Repository
{
    public interface IRoomsRepository
    {
        public ValueTask<Room[]> GetAvailableRoomsAsync(DateTimeOffset time, CancellationToken ct);
        public ValueTask<Room[]> GetAvailableRoomsAsync(DateTimeOffset? after, DateTimeOffset? before, CancellationToken ct);
    }
}

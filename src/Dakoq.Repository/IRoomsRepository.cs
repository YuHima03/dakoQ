using Dakoq.Repository.Models;

namespace Dakoq.Repository
{
    public interface IRoomsRepository
    {
        public ValueTask<Room> GetRoomAsync(Guid id, CancellationToken ct = default);

        public ValueTask<Room> GetAvailableRoomAsync(string alias, CancellationToken ct = default);

        public ValueTask<List<Room>> GetAvailableRoomsAsync(CancellationToken ct = default);

        public ValueTask<Room> PostRoomAsync(PostRoomRequest req, CancellationToken ct = default);
    }
}

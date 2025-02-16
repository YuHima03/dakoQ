using Dakoq.Repository.Models;

namespace Dakoq.Repository
{
    public interface IRoomsRepository
    {
        public ValueTask<Room> GetRoomAsync(Guid id, CancellationToken ct);

        public ValueTask<Room> GetRoomAsync(string alias, CancellationToken ct);

        public ValueTask<Room> PostRoomAsync(PostRoomRequest req, CancellationToken ct);
    }
}

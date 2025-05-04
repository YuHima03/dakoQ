using Dakoq.Domain.Models;

namespace Dakoq.Domain.Repository
{
    public interface IRoomsRepository
    {
        public ValueTask AddRoomOpeningHoursAsync(Guid roomId, RoomHours[] openingHours, CancellationToken ct);

        public ValueTask AddOrOverwriteOpeningHoursAsync(Guid roomId, RoomHours[] openingHours, CancellationToken ct);

        public ValueTask DeleteRoomOpeningHoursAsync(Guid roomId, RoomHours[] excludedHours, CancellationToken ct);

        public ValueTask<Room[]> GetAvailableRoomsAsync(DateTimeOffset time, CancellationToken ct);
        
        public ValueTask<Room[]> GetAvailableRoomsAsync(RoomHours hours, CancellationToken ct);
        
        public ValueTask<RoomHoursWithSource[]> GetRoomOpeningHoursAsync(Guid roomId, DateTimeOffset after, DateTimeOffset before, CancellationToken ct);

        public ValueTask<Room> PostRoomAsync(PostRoomRequest request, CancellationToken ct);

        public ValueTask<Room> UpdateRoomAsync(Guid roomId, PostRoomRequest request, CancellationToken ct);
    }
}

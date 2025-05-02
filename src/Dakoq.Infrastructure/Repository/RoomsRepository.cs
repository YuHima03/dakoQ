using Dakoq.Domain.Models;
using Dakoq.Domain.Repository;

namespace Dakoq.Infrastructure.Repository
{
    sealed partial class Repository : IRoomsRepository
    {
        public ValueTask AddOrOverwriteOpeningHoursAsync(Guid roomId, RoomHours[] openingHours, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public ValueTask AddRoomOpeningHoursAsync(Guid roomId, RoomHours[] openingHours, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public ValueTask DeleteRoomOpeningHoursAsync(Guid roomId, RoomHours[] excludedHours, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public ValueTask<Domain.Room[]> GetAvailableRoomsAsync(DateTimeOffset time, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public ValueTask<Domain.Room[]> GetAvailableRoomsAsync(RoomHours hours, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public ValueTask<RoomHoursWithSource[]> GetRoomOpeningHoursAsync(Guid roomId, DateTimeOffset after, DateTimeOffset before, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public ValueTask<Domain.Room> PostRoomAsync(PostRoomRequest request, CancellationToken ct)
        {
            throw new NotImplementedException();
        }

        public ValueTask<Domain.Room> UpdateRoomAsync(Guid roomId, PostRoomRequest request, CancellationToken ct)
        {
            throw new NotImplementedException();
        }
    }
}

using Dakoq.Domain.Models;

namespace Dakoq.Domain.Repository
{
    public interface IRoomParticipantsRepository
    {
        public ValueTask<UserParticipation?> GetUserCurrentParticipationAsync(Guid userId, CancellationToken ct);

        public ValueTask<UserParticipation> JoinToOrLeaveFromRoomAsync(Guid roomId, Guid userId, CancellationToken ct);

        public ValueTask<UserParticipation> JoinToRoomAsync(Guid roomId, Guid userId, CancellationToken ct);

        public ValueTask<UserParticipation> LeaveFromRoomAsync(Guid userId, CancellationToken ct);
    }
}

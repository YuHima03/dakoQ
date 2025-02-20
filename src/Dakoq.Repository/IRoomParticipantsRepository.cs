namespace Dakoq.Repository
{
    public interface IRoomParticipantsRepository
    {
        public ValueTask<Models.RoomParticipant> JoinToRoomAsync(Guid userId, Guid roomId, CancellationToken ct = default);

        public ValueTask<Models.RoomParticipant> LeaveFromRoomAsync(Guid userId, CancellationToken ct = default);
    }
}

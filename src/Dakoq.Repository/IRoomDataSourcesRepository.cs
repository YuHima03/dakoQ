namespace Dakoq.Repository
{
    public interface IRoomDataSourcesRepository
    {
        public ValueTask<List<KeyValuePair<int, string>>> GetActiveRoomDataSourceMappingAsync(CancellationToken ct = default);

        public ValueTask<Models.RoomDataSource> GetRoomDataSourceAsync(int id, CancellationToken ct = default);
    }
}

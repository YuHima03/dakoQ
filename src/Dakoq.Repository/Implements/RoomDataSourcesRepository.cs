using Dakoq.Repository.Exceptions;
using Dakoq.Repository.Models;
using Microsoft.EntityFrameworkCore;

namespace Dakoq.Repository
{
    public sealed partial class RepositoryContext : IRoomDataSourcesRepository
    {
        public async ValueTask<List<KeyValuePair<int, string>>> GetActiveRoomDataSourceMappingAsync(CancellationToken ct = default)
        {
            return await RoomDataSources.Select(r => KeyValuePair.Create(r.Id, r.Name)).ToListAsync(ct);
        }

        public async ValueTask<RoomDataSource> GetRoomDataSourceAsync(int id, CancellationToken ct = default)
        {
            return await RoomDataSources.Where(r => r.Id == id).SingleOrDefaultAsync(ct) ?? throw new SqlRowNotFoundException();
        }

        public async ValueTask<RoomDataSource> GetRoomDataSourceAsync(string name, CancellationToken ct = default)
        {
            return await RoomDataSources.Where(r => r.Name == name).SingleOrDefaultAsync(ct) ?? throw new SqlRowNotFoundException();
        }
    }
}

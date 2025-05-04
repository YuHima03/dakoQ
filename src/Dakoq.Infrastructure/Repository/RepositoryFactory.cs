using Dakoq.Domain.Repository;
using Microsoft.EntityFrameworkCore;

namespace Dakoq.Infrastructure.Repository
{
    public sealed class RepositoryFactory(IDbContextFactory<Repository> repositoryFactory) : IRepositoryFactory
    {
        public async ValueTask<IRepository> CreateAsync(CancellationToken ct)
        {
            return await repositoryFactory.CreateDbContextAsync(ct).ConfigureAwait(false);
        }
    }
}

namespace Dakoq.Domain.Repository
{
    public interface IRepositoryFactory
    {
        public ValueTask<IRepository> CreateAsync(CancellationToken ct);
    }
}

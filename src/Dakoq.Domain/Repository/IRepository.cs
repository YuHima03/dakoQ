namespace Dakoq.Domain.Repository
{
    public interface IRepository :
        IAsyncDisposable,
        IDisposable,
        IRoomsRepository,
        IRoomParticipantsRepository,
        IRoomWebhooksRepository;
}

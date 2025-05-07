namespace Dakoq.Domain.Models
{
    public record class PostKnoqV1EventRequest(
        Guid Id,
        string Name,
        Guid? RoomId,
        DateTimeOffset StartsAt,
        DateTimeOffset EndsAt,
        DateTimeOffset SourceCreatedAt,
        DateTimeOffset SourceUpdatedAt
        );
}

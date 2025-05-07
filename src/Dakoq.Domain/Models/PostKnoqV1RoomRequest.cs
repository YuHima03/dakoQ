namespace Dakoq.Domain.Models
{
    public record class PostKnoqV1RoomRequest(
        Guid Id,
        string Name,
        bool IsVerified,
        DateTimeOffset StartsAt,
        DateTimeOffset EndsAt,
        DateTimeOffset SourceCreatedAt,
        DateTimeOffset SourceUpdatedAt
        );
}

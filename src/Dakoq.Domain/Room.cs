namespace Dakoq.Domain
{
    public sealed record class Room(
        Guid Id,
        string Name,
        RoomDataSources DataSource,
        string? Alias,
        DateTimeOffset? StartsAt,
        DateTimeOffset? EndsAt,
        DateTimeOffset CreatedAt,
        DateTimeOffset UpdatedAt
    );

    public enum RoomDataSources : byte
    {
        Dakoq, KnoqRoom, KnoqEvent
    }
}

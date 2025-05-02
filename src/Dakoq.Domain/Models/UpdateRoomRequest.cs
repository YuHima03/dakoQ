namespace Dakoq.Domain.Models
{
    public sealed record class UpdateRoomRequest(
        string? Name,
        string? Description
        );
}

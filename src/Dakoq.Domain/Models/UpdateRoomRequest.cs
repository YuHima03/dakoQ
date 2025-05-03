namespace Dakoq.Domain.Models
{
    public sealed record class UpdateRoomRequest(
        Optional<string> Name,
        Optional<string?> Description
        );
}

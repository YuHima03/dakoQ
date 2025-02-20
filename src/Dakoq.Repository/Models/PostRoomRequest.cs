namespace Dakoq.Repository.Models
{
    public sealed class PostRoomRequest
    {
        public Guid? Id { get; set; }

        public string? Name { get; set; }

        public Domain.RoomDataSources DataSource { get; set; } = Domain.RoomDataSources.Dakoq;

        public string? Alias { get; set; }

        public DateTime? StartsAt { get; set; }

        public DateTime? EndsAt { get; set; }
    }
}

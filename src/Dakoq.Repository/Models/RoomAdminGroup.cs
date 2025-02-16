using System.ComponentModel.DataAnnotations.Schema;

namespace Dakoq.Repository.Models
{
    public sealed class RoomAdminGroup
    {
        [Column("room_id")]
        public Guid RoomId { get; set; }

        [Column("group_id")]
        public Guid GroupId { get; set; }

        [Column("created_at")]
        public DateTimeOffset CreatedAt { get; set; }
    }
}

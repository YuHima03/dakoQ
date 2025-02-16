using System.ComponentModel.DataAnnotations.Schema;

namespace Dakoq.Repository.Models
{
    public class RoomAdminUser
    {
        [Column("room_id")]
        public Guid RoomId { get; set; }

        [Column("user_id")]
        public Guid UserId { get; set; }

        [Column("created_at")]
        public DateTimeOffset CreatedAt { get; set; }
    }
}

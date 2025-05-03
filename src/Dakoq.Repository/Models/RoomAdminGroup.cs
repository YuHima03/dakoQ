using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dakoq.Repository.Models
{
    [Table("old_room_admin_groups")]
    [Keyless]
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

using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dakoq.Infrastructure.Repository.Models
{
    [Table("room_admin_groups")]
    [Keyless]
    sealed class RoomAdminGroup
    {
        [Column("room_id")]
        public Guid RoomId { get; set; }

        [Column("group_id")]
        public Guid GroupId { get; set; }

        [Column("created_at")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; }
    }
}

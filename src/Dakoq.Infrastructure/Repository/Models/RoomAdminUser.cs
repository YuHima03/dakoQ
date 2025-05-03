using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dakoq.Infrastructure.Repository.Models
{
    [Table("room_admin_users")]
    [Keyless]
    sealed class RoomAdminUser
    {
        [Column("room_id")]
        public Guid RoomId { get; set; }

        [Column("user_id")]
        public Guid UserId { get; set; }

        [Column("created_at")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; }
    }
}

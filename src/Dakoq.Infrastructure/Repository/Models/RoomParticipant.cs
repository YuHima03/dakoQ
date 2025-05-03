using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dakoq.Infrastructure.Repository.Models
{
    [Table("room_participants")]
    sealed class RoomParticipant
    {
        [Column("id")]
        [Key]
        public uint Id { get; set; }

        [Column("room_id")]
        public Guid RoomId { get; set; }

        [Column("user_id")]
        public Guid UserId { get; set; }

        [Column("joined_at")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime JoinedAt { get; set; }

        [Column("left_at")]
        public DateTime? LeftAt { get; set; }

        [Column("created_at")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedAt { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dakoq.Repository.Models
{
    [Table("room_participants")]
    public sealed class RoomParticipant
    {
        [Column("id")]
        [Key]
        public Guid Id { get; set; }

        [Column("room_id")]
        public Guid RoomId { get; set; }

        [Column("user_id")]
        public Guid ParticipantId { get; set; }

        [Column("joined_at")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime JoinedAt { get; set; }

        [Column("left_at")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime? LeftAt { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }
}

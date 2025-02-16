using System.ComponentModel.DataAnnotations.Schema;

namespace Dakoq.Repository.Models
{
    public sealed class RoomParticipant
    {
        [Column("room_id")]
        public Guid RoomId { get; set; }

        [Column("user_id")]
        public Guid ParticipantId { get; set; }

        [Column("joined_at")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTimeOffset? JoinedAt { get; set; }

        [Column("left_at")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTimeOffset? LeftAt { get; set; }

        [Column("created_at")]
        public DateTimeOffset CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTimeOffset UpdatedAt { get; set; }
    }
}

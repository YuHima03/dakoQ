using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dakoq.Infrastructure.Repository.Models
{
    [Table("room_opening_hours")]
    [Keyless]
    sealed class RoomOpeningHours
    {
        [Column("room_id")]
        public Guid RoomId { get; set; }

        [Column("source_knoq_v1_event_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid? SourceKnoqV1EventId { get; set; }

        [Column("source_knoq_v1_room_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid? SourceKnoqV1RoomId { get; set; }

        [Column("starts_at")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime? StartsAt { get; set; }

        [Column("ends_at")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime? EndsAt { get; set; }
    }
}

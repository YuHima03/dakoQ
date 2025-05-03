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

        [Column("source_id")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid? SourceId { get; set; }

        [Column("starts_at")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime? StartsAt { get; set; }

        [Column("ends_at")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime? EndsAt { get; set; }
    }
}

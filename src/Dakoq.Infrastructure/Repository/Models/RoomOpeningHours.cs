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
        public Guid? SourceId { get; set; }

        [Column("starts_at")]
        public DateTime? StartsAt { get; set; }

        [Column("ends_at")]
        public DateTime? EndsAt { get; set; }
    }
}

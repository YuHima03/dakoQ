using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Dakoq.Infrastructure.Repository.Models
{
    [Table("knoq_v1_events")]
    sealed class KnoqV1Event
    {
        [Column("id")]
        [Key]
        public Guid Id { get; set; }

        [Column("name")]
        [NotNull]
        public string? Name { get; set; }

        [Column("room_id")]
        public Guid? RoomId { get; set; }

        [Column("starts_at")]
        public DateTime StartsAt { get; set; }

        [Column("ends_at")]
        public DateTime EndsAt { get; set; }

        [Column("created_at")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedAt { get; set; }
    }
}

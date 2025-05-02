using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Dakoq.Infrastructure.Repository.Models
{
    [Table("rooms")]
    sealed class Room
    {
        [Key]
        [Column("id")]
        public Guid Id { get; set; }

        [Column("name")]
        [NotNull]
        public string? Name { get; set; }

        [Column("description")]
        [NotNull]
        public string? Description { get; set; }

        [Column("created_at")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTimeOffset CreatedAt { get; set; }

        [Column("updated_at")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTimeOffset UpdatedAt { get; set; }
    }
}

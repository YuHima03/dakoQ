using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Dakoq.Repository.Models
{
    [Table("rooms")]
    public sealed class Room
    {
        [Column("id")]
        [Key]
        public Guid Id { get; set; }

        [Column("name")]
        [NotNull]
        public string? Name { get; set; }

        [Column("data_source_id")]
        public int DataSourceId { get; set; }

        [Column("alias")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string? Alias { get; set; }

        [Column("starts_at")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime? StartsAt { get; set; }

        [Column("ends_at")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime? EndsAt { get; set; }

        [Column("created_at")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime UpdatedAt { get; set; }
    }
}

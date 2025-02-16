using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Dakoq.Repository.Models
{
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
        public DateTimeOffset? StartsAt { get; set; }

        [Column("ends_at")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTimeOffset? EndsAt { get; set; }

        [Column("created_at")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTimeOffset CreatedAt { get; set; }

        [Column("updated_at")]
        [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTimeOffset UpdatedAt { get; set; }
    }
}

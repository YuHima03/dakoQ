using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Dakoq.Repository.Models
{
    [Table("room_data_sources")]
    public sealed class RoomDataSource
    {
        [Column("id")]
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Column("name")]
        [NotNull]
        public string? Name { get; set; }

        [Column("is_active")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public bool IsActive { get; set; }
    }
}

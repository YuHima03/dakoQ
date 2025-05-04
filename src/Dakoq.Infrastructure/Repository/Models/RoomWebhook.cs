using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Dakoq.Infrastructure.Repository.Models
{
    [Table("room_webhooks")]
    internal class RoomWebhook
    {
        [Column("id")]
        [Key]
        public Guid Id { get; set; }

        [Column("room_id")]
        public Guid RoomId { get; set; }

        [Column("owner_id")]
        public Guid OwnerId { get; set; }

        [Column("hashed_secret")]
        [NotNull]
        public string? HashedSecret { get; set; }

        [Column("created_at")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public DateTime CreatedAt { get; set; }
    }
}

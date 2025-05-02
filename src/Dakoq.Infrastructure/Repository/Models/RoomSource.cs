using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dakoq.Infrastructure.Repository.Models
{
    [Table("room_sources")]
    sealed class RoomSource
    {
        [Column("id")]
        [Key]
        public uint Id { get; set; }

        [Column("knoq_v1_event_id")]
        public Guid? KnoqV1EventId { get; set; }

        [Column("knoq_v1_room_id")]
        public Guid? KnoqV1RoomId { get; set; }
    }
}

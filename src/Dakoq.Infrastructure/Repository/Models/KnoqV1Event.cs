using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dakoq.Infrastructure.Repository.Models
{
    [Table("knoq_v1_events")]
    sealed class KnoqV1Event
    {
        public Guid Id { get; set; }

        [NotNull]
        public string? Name { get; set; }

        public Guid? RoomId { get; set; }

        public DateTime StartsAt { get; set; }

        public DateTime EndsAt { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}

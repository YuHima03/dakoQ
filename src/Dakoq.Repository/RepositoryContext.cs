using Microsoft.EntityFrameworkCore;
using System.Diagnostics.CodeAnalysis;

namespace Dakoq.Repository
{
    public sealed partial class RepositoryContext(DbContextOptions<RepositoryContext> options) : DbContext(options)
    {
        [NotNull]
        public DbSet<Models.Room>? Rooms { get; set; }

        [NotNull]
        public DbSet<Models.RoomAdminGroup>? RoomAdminGroups { get; set; }

        [NotNull]
        public DbSet<Models.RoomAdminUser>? RoomAdminUsers { get; set; }

        [NotNull]
        public DbSet<Models.RoomDataSource>? RoomDataSources { get; set; }

        [NotNull]
        public DbSet<Models.RoomParticipant>? RoomParticipants { get; set; }
    }
}

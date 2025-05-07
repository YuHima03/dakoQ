using Dakoq.Domain.Repository;
using Microsoft.EntityFrameworkCore;

namespace Dakoq.Infrastructure.Repository
{
    public sealed partial class Repository(DbContextOptions<Repository> options) : DbContext(options), IRepository
    {
        DbSet<Models.Room> Rooms { get; set; }
        DbSet<Models.RoomAdminUser> RoomAdminUsers { get; set; }
        DbSet<Models.RoomAdminGroup> RoomAdminGroups { get; set; }
        DbSet<Models.RoomOpeningHours> RoomOpeningHours { get; set; }
        DbSet<Models.RoomParticipant> RoomParticipants { get; set; }
        DbSet<Models.RoomWebhook> RoomWebhooks { get; set; }
    }
}

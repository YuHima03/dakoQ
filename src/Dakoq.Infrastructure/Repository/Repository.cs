using Dakoq.Domain.Repository;
using Microsoft.EntityFrameworkCore;

namespace Dakoq.Infrastructure.Repository
{
    public sealed partial class Repository(DbContextOptions<Repository> options) : DbContext(options), IRepository
    {
        #region knoQ

        DbSet<Models.KnoqV1Event> KnoqV1Events { get; set; }
        DbSet<Models.KnoqV1Room> KnoqV1Rooms { get; set; }

        #endregion

        DbSet<Models.Room> Rooms { get; set; }
        DbSet<Models.RoomAdminUser> RoomAdminUsers { get; set; }
        DbSet<Models.RoomAdminGroup> RoomAdminGroups { get; set; }
        DbSet<Models.RoomOpeningHours> RoomOpeningHours { get; set; }
        DbSet<Models.RoomParticipant> RoomParticipants { get; set; }
        DbSet<Models.RoomSource> RoomSources { get; set; }
    }
}

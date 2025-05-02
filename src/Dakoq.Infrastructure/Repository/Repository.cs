using Microsoft.EntityFrameworkCore;

namespace Dakoq.Infrastructure.Repository
{
    sealed partial class Repository(DbContextOptions<Repository> options) : DbContext(options)
    {
        DbSet<Models.Room> Rooms { get; set; }

        DbSet<Models.RoomOpeningHours> RoomOpeningHours { get; set; }
    }
}

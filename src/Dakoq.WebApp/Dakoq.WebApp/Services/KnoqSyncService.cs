using Dakoq.Repository.Exceptions;
using Dakoq.Repository.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Data.SqlTypes;

namespace Dakoq.WebApp.Services
{
    public sealed class KnoqSyncService(IServiceProvider services) : BackgroundService
    {
        readonly Task<Knoq.IKnoqApiClient?> _knoq = services.GetRequiredService<Task<Knoq.IKnoqApiClient?>>();
        readonly KnoqSyncServiceOptions _options = services.GetService<IOptions<KnoqSyncServiceOptions>>()?.Value ?? KnoqSyncServiceOptions.Default;
        readonly IDbContextFactory<Repository.RepositoryContext> _repo = services.GetRequiredService<IDbContextFactory<Repository.RepositoryContext>>();

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var knoq = await _knoq;
            if (knoq is null)
            {
                throw new Exception("The knoQ API client is null.");
            }

            using Repository.IRepository repo = await _repo.CreateDbContextAsync(stoppingToken);
            using PeriodicTimer timer = new(_options.FetchInterval);

            var jstTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");
            DateTimeOffset jstPrev = default;
            string startDateTime = "";
            string endDateTime = "";

            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                DateTimeOffset jstNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, jstTimeZone); // JST (UTC+9)
                if (jstNow.Day != jstPrev.Day)
                {
                    jstPrev = jstNow;
                    startDateTime = $"{jstNow:yyyy-MM-dd}T00:00:00+09:00";
                    endDateTime = $"{jstNow:yyyy-MM-dd}T23:59:59+09:00";
                }

                // 進捗部屋とイベントを取得
                var todayRooms = (await knoq.RoomsApi.GetRoomsAsync(startDateTime, endDateTime, null, stoppingToken)).Where(r => r.Verified && DateTimeOffset.Parse(r.TimeStart) < DateTimeOffset.Parse(r.TimeEnd)).ToList();
                var todayEvents = (await knoq.EventsApi.GetEventsAsync(startDateTime, endDateTime, null, stoppingToken)).Where(r => DateTimeOffset.Parse(r.TimeStart) < DateTimeOffset.Parse(r.TimeEnd)).ToArray();

                var dataSources = (await repo.GetActiveRoomDataSourceMappingAsync(stoppingToken)).Select(kvp => (KeyValuePair<int, string>?)kvp);
                var knoqRoomSource = dataSources.FirstOrDefault(kvp => kvp!.Value.Value == Domain.RoomDataSources.KnoqRoom.ToString())?.Key;
                var knoqEventSource = dataSources.FirstOrDefault(kvp => kvp!.Value.Value == Domain.RoomDataSources.KnoqEvent.ToString())?.Key;

                if (knoqRoomSource is not null)
                {
                    foreach (var knoqRoom in todayRooms)
                    {
                        Room room;
                        try
                        {
                            room = await repo.GetRoomAsync(knoqRoom.RoomId, stoppingToken);
                            if (room.UpdatedAt == DateTimeOffset.Parse(knoqRoom.UpdatedAt))
                            {
                                continue;
                            }
                        }
                        catch (SqlRowNotFoundException)
                        {
                            _ = await repo.PostRoomAsync(new()
                            {
                                Id = knoqRoom.RoomId,
                                Name = $"進捗部屋 ({knoqRoom.Place})",
                                DataSource = Domain.RoomDataSources.KnoqRoom,
                                StartsAt = DateTimeOffset.Parse(knoqRoom.TimeStart).UtcDateTime,
                                EndsAt = DateTimeOffset.Parse(knoqRoom.TimeEnd).UtcDateTime
                            }, stoppingToken);
                            continue;
                        }

                        room.Name = $"進捗部屋 ({knoqRoom.Place})";
                        room.StartsAt = DateTimeOffset.Parse(knoqRoom.TimeStart).UtcDateTime;
                        room.EndsAt = DateTimeOffset.Parse(knoqRoom.TimeEnd).UtcDateTime;
                        room.UpdatedAt = DateTimeOffset.Parse(knoqRoom.UpdatedAt).UtcDateTime;
                    }
                }
                if (knoqEventSource is not null && todayEvents.Length != 0)
                {
                    foreach (var knoqEvent in todayEvents)
                    {
                        Room room;
                        try
                        {
                            room = await repo.GetRoomAsync(knoqEvent.EventId, stoppingToken);
                            if (room.UpdatedAt == DateTimeOffset.Parse(knoqEvent.UpdatedAt))
                            {
                                continue;
                            }
                        }
                        catch (SqlRowNotFoundException)
                        {
                            _ = await repo.PostRoomAsync(new()
                            {
                                Id = knoqEvent.EventId,
                                Name = knoqEvent.Name,
                                DataSource = Domain.RoomDataSources.KnoqEvent,
                                StartsAt = DateTimeOffset.Parse(knoqEvent.TimeStart).UtcDateTime,
                                EndsAt = DateTimeOffset.Parse(knoqEvent.TimeEnd).UtcDateTime
                            }, stoppingToken);
                            continue;
                        }

                        room.Name = knoqEvent.Name;
                        room.StartsAt = DateTimeOffset.Parse(knoqEvent.TimeStart).UtcDateTime;
                        room.EndsAt = DateTimeOffset.Parse(knoqEvent.TimeEnd).UtcDateTime;
                        room.UpdatedAt = DateTimeOffset.Parse(knoqEvent.UpdatedAt).UtcDateTime;
                    }
                }
            }
        }
    }

    public sealed class KnoqSyncServiceOptions
    {
        public static KnoqSyncServiceOptions Default = new() { FetchInterval = TimeSpan.FromMinutes(5) };

        public TimeSpan FetchInterval { get; set; }
    }
}

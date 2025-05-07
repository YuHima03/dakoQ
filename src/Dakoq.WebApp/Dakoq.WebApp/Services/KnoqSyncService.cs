using Dakoq.Repository.Exceptions;
using Dakoq.Repository.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace Dakoq.WebApp.Services
{
    public sealed class KnoqSyncService(IServiceProvider services) : BackgroundService
    {
        readonly Knoq.IKnoqApiClient _knoq = services.GetRequiredService<Knoq.IKnoqApiClient>();
        readonly KnoqSyncServiceOptions _options = services.GetService<IOptions<KnoqSyncServiceOptions>>()?.Value ?? KnoqSyncServiceOptions.Default;
        readonly IDbContextFactory<Repository.RepositoryContext> _repo = services.GetRequiredService<IDbContextFactory<Repository.RepositoryContext>>();

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var knoq = _knoq;
            using Repository.IRepository repo = await _repo.CreateDbContextAsync(stoppingToken);
            using PeriodicTimer timer = new(_options.FetchInterval);

            var jstTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");

            do
            {
                var jstNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, jstTimeZone);
                var jstTodayStart = new DateTimeOffset(jstNow.Date); // JST (UTC+9)
                var jstTodayEnd = jstTodayStart.AddDays(1).AddTicks(-1);

                // 進捗部屋とイベントを取得
                var todayRooms = (await knoq.RoomsApi.GetRoomsAsync(jstTodayStart.ToString("O"), jstTodayEnd.ToString("O"), null, stoppingToken))
                    .Where(r =>
                    {
                        var start = DateTimeOffset.Parse(r.TimeStart);
                        var end = DateTimeOffset.Parse(r.TimeEnd);
                        return r.Verified
                            && start < end
                            && start <= jstNow && jstNow <= end;
                    })
                    .ToList();
                var todayEvents = (await knoq.EventsApi.GetEventsAsync(jstTodayStart.ToString("O"), jstTodayEnd.ToString("O"), null, stoppingToken))
                    .Where(r =>
                    {
                        var start = DateTimeOffset.Parse(r.TimeStart);
                        var end = DateTimeOffset.Parse(r.TimeEnd);
                        return start < end
                            && start <= jstNow && jstNow <= end;
                    })
                    .ToArray();

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
            while (await timer.WaitForNextTickAsync(stoppingToken));
        }
    }

    sealed class KnoqSyncService_V2(
        Knoq.IKnoqApiClient knoq,
        ILogger<KnoqSyncService_V2> logger,
        IOptions<KnoqSyncServiceOptions>? options,
        Domain.Repository.IRepositoryFactory repositoryFactory
        ) : BackgroundService, IHealthCheck
    {
        readonly KnoqSyncServiceOptions _options = options?.Value ?? KnoqSyncServiceOptions.Default;

        public Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            if (_options.FetchInterval < TimeSpan.Zero || _options.TimeZoneInfo is null)
            {
                return Task.FromResult(HealthCheckResult.Degraded("Invalid options."));
            }
            return Task.FromResult(HealthCheckResult.Healthy());
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (_options.FetchInterval < TimeSpan.Zero)
            {
                return;
            }

            using PeriodicTimer timer = new(_options.FetchInterval);
            do
            {
                var timeZoneInfo = _options.TimeZoneInfo;
                var utcNow = DateTime.UtcNow;
                var now = TimeZoneInfo.ConvertTimeFromUtc(utcNow, timeZoneInfo);
                var today = now.Date;

                var todayStart_inUtc = TimeZoneInfo.ConvertTimeToUtc(today, timeZoneInfo);
                var todayEnd_inUtc = todayStart_inUtc.AddDays(1).AddTicks(-1);

                var todayStartString = todayStart_inUtc.ToString("O");
                var todayEndString = todayEnd_inUtc.ToString("O");

                try
                {
                    var todayVerifiedRooms = (await knoq.RoomsApi.GetRoomsAsync(todayStartString, todayEndString, null, stoppingToken))
                        .Where(r => r.Verified)
                        .Select(r => new Domain.Models.PostKnoqV1RoomRequest(
                            r.RoomId,
                            r.Place,
                            true,
                            DateTimeOffset.Parse(r.TimeStart),
                            DateTimeOffset.Parse(r.TimeEnd),
                            DateTimeOffset.Parse(r.CreatedAt),
                            DateTimeOffset.Parse(r.UpdatedAt)
                            ))
                        .Where(r => r.StartsAt < r.EndsAt && r.StartsAt <= now && now <= r.EndsAt);

                    var todayEvents = (await knoq.EventsApi.GetEventsAsync(todayStartString, todayEndString, null, stoppingToken))
                        .Select(r => new Domain.Models.PostKnoqV1EventRequest(
                            r.EventId,
                            r.Name,
                            r.RoomId,
                            DateTimeOffset.Parse(r.TimeStart),
                            DateTimeOffset.Parse(r.TimeEnd),
                            DateTimeOffset.Parse(r.CreatedAt),
                            DateTimeOffset.Parse(r.UpdatedAt)
                            ))
                        .Where(ev => ev.StartsAt < ev.EndsAt && ev.StartsAt <= now && now <= ev.EndsAt);

                    var sources = Enumerable.Concat(
                        todayVerifiedRooms.Select(Domain.Models.PostRoomSourceRequest.Create),
                        todayEvents.Select(Domain.Models.PostRoomSourceRequest.Create)
                        );
                    await using var repo = await repositoryFactory.CreateAsync(stoppingToken);
                    foreach (var req in sources)
                    {
                        await repo.AddOrUpdateRoomOpeningHoursWithSourceAsync(req, stoppingToken);
                    }
                }
                catch (Exception e)
                {
                    logger.LogError(e, "Failed to sync knoQ data.");
                }
            }
            while (await timer.WaitForNextTickAsync(stoppingToken));
        }
    }

    public sealed class KnoqSyncServiceOptions
    {
        public readonly static KnoqSyncServiceOptions Default = new();

        public TimeSpan FetchInterval { get; set; } = TimeSpan.FromMinutes(5);

        public TimeZoneInfo TimeZoneInfo { get; set; } = TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");
    }
}

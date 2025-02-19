
using Microsoft.Extensions.Options;

namespace Dakoq.WebApp.Services
{
    public class MemoryMonitorService(ILogger<MemoryMonitorService> logger, IOptions<MemoryMonitorServiceOptions>? options) : BackgroundService
    {
        readonly ILogger<MemoryMonitorService> _logger = logger;
        readonly MemoryMonitorServiceOptions _options = options?.Value ?? MemoryMonitorServiceOptions.Default;

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using PeriodicTimer timer = new(_options.CheckInterval);

            _logger.LogInformation("Monitoring allocated bytes.");

            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                var allocated = GC.GetTotalAllocatedBytes();
                _logger.LogDebug("Allocated bytes (approx): {} MiB", (double)allocated / (1024 * 1024));
                if (allocated >= _options.ThresholdBytes)
                {
                    _logger.LogWarning("The allocated bytes exceeds {}MiB: {}MiB", (double)_options.ThresholdBytes / (1024 * 1024), (double)allocated / (1024 * 1024));
                    GC.Collect();
                }
            }
        }
    }

    public sealed class MemoryMonitorServiceOptions
    {
        public static readonly MemoryMonitorServiceOptions Default = new()
        {
            CheckInterval = TimeSpan.FromSeconds(10),
            ThresholdBytes = 100 * 1024 * 1024
        };

        public TimeSpan CheckInterval { get; set; }

        public nint ThresholdBytes { get; set; }
    }
}

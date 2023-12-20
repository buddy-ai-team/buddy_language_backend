using System.Diagnostics;
using System.Runtime;

namespace BuddyLanguage.TelegramBot.BackgroundServices;

public class AppMemoryProfilerBackgroundService : BackgroundService
{
    private readonly ILogger<AppMemoryProfilerBackgroundService> _logger;

    public AppMemoryProfilerBackgroundService(ILogger<AppMemoryProfilerBackgroundService> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var mode = GCSettings.IsServerGC ? "Server" : "Workstation";
            var latencyMode = GCSettings.LatencyMode;
            var largeObjectHeapCompactionMode = GCSettings.LargeObjectHeapCompactionMode;
            var gcInfo = GC.GetGCMemoryInfo();
            var concurrent = gcInfo.Concurrent;
            _logger.LogInformation(
                "GC mode: {Mode}, LatencyMode: {LatencyMode}, LargeObjectHeapCompactionMode: {LargeObjectHeapCompactionMode}, " +
                "Concurrent: {Concurrent}, TotalMemory: {TotalMemory}, HeapSize: {HeapSize}, FragmentedBytes: {FragmentedBytes}",
                mode,
                latencyMode,
                largeObjectHeapCompactionMode,
                concurrent,
                gcInfo.TotalAvailableMemoryBytes,
                gcInfo.HeapSizeBytes,
                gcInfo.FragmentedBytes);

            var process = Process.GetCurrentProcess();
            var memory = process.PrivateMemorySize64 / 1024 / 1024;
            _logger.LogInformation("Memory: {Memory} MB", memory);

            await Task.Delay(TimeSpan.FromMinutes(60), stoppingToken);
        }
    }
}

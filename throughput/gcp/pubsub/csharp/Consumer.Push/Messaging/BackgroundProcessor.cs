using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Shared.Configuration;

namespace Consumer.Push.Messaging;

public sealed class BackgroundProcessor : BackgroundService
{
    private readonly ProcessingQueue _queue;
    private readonly ILogger<BackgroundProcessor> _logger;
    private readonly ThroughputSettings _settings;

    public BackgroundProcessor(
        ProcessingQueue queue,
        ILogger<BackgroundProcessor> logger,
        ThroughputSettings settings)
    {
        _queue = queue;
        _logger = logger;
        _settings = settings;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var tasks = new List<Task>();

        for (var worker = 0; worker < _settings.MaxConcurrentMessages; worker++)
        {
            // Dedicated workers let us scale background processing without blocking push responses.
            tasks.Add(ProcessQueueAsync(stoppingToken));
        }

        return Task.WhenAll(tasks);
    }

    private async Task ProcessQueueAsync(CancellationToken stoppingToken)
    {
        await foreach (var workItem in _queue.Reader.ReadAllAsync(stoppingToken))
        {
            try
            {
                await workItem(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Background work item failed");
            }
        }
    }
}

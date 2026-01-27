using System.Threading.Channels;
using Shared.Configuration;

namespace Consumer.Push.Messaging;

public sealed class ProcessingQueue
{
    private readonly Channel<Func<CancellationToken, Task>> _channel;

    public ProcessingQueue(ThroughputSettings settings)
    {
        // Bounded channel gives backpressure to push endpoints under load.
        _channel = Channel.CreateBounded<Func<CancellationToken, Task>>(
            new BoundedChannelOptions(settings.MaxOutstandingMessageCount)
            {
                // Wait mode applies backpressure instead of dropping work.
                FullMode = BoundedChannelFullMode.Wait
            });
    }

    // Enqueue blocks when full, providing backpressure to the push endpoint.
    public ValueTask EnqueueAsync(Func<CancellationToken, Task> workItem, CancellationToken cancellationToken) =>
        _channel.Writer.WriteAsync(workItem, cancellationToken);

    public ChannelReader<Func<CancellationToken, Task>> Reader => _channel.Reader;
}

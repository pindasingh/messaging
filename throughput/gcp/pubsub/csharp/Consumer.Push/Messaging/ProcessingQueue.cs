using System.Threading.Channels;
using Shared.Configuration;

namespace Consumer.Push.Messaging;

public sealed class ProcessingQueue
{
    private readonly Channel<Func<CancellationToken, Task>> _channel;

    public ProcessingQueue(ThroughputSettings settings)
    {
        // Bounded channel blocks writers when full (backpressure).
        _channel = Channel.CreateBounded<Func<CancellationToken, Task>>(
            new BoundedChannelOptions(settings.MaxOutstandingMessageCount)
            {
                FullMode = BoundedChannelFullMode.Wait
            });
    }

    public ValueTask EnqueueAsync(Func<CancellationToken, Task> workItem, CancellationToken cancellationToken) =>
        _channel.Writer.WriteAsync(workItem, cancellationToken);

    public ChannelReader<Func<CancellationToken, Task>> Reader => _channel.Reader;
}

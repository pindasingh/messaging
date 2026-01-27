using Google.Cloud.PubSub.V1;
using Google.Protobuf;
using Producer.Models;
using Shared.Configuration;
using Shared.Messaging;

namespace Producer.Handlers;

public class OrderCreatedPublisher
{
    private readonly IReadOnlyList<PublisherClient> _publishers;
    private readonly ThroughputSettings _throughputSettings;
    private readonly ILogger<OrderCreatedPublisher> _logger;
    private int _publisherIndex;

    public OrderCreatedPublisher(
        IReadOnlyList<PublisherClient> publishers,
        ThroughputSettings throughputSettings,
        ILogger<OrderCreatedPublisher> logger)
    {
        _publishers = publishers;
        _throughputSettings = throughputSettings;
        _logger = logger;
    }

    public async Task<IResult> Publish(PublishMessageRequest request)
    {
        var payload = new OrderCreatedV1 { Content = request.Message }.ToJson();

        // Compression trades CPU for bandwidth; use threshold to avoid wasting CPU on small payloads.
        var shouldCompress = request.Compress
            ?? (_throughputSettings.EnableCompressionByDefault && payload.Length >= _throughputSettings.CompressionThresholdBytes);

        var data = shouldCompress
            ? ByteString.CopyFrom(CompressionHelper.Compress(payload))
            : ByteString.CopyFromUtf8(payload);

        var attributes = shouldCompress
            ? new Dictionary<string, string> { { CompressionHelper.ContentEncodingAttribute, CompressionHelper.GzipEncoding } }
            : new Dictionary<string, string>();

        var publisher = _publishers[GetNextPublisherIndex()];
        var messageId = await publisher.PublishAsync(new PubsubMessage
        {
            Data = data,
            Attributes = { attributes }
        });

        _logger.LogInformation("Published message {MessageId} (compressed: {Compressed})", messageId, shouldCompress);

        return Results.Ok(new PublishMessageResponse(messageId, shouldCompress));
    }

    private int GetNextPublisherIndex()
    {
        if (_publishers.Count == 1)
        {
            return 0;
        }

        return Interlocked.Increment(ref _publisherIndex) % _publishers.Count;
    }
}

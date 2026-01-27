using Google.Api.Gax;
using Google.Cloud.PubSub.V1;
using Shared.Configuration;

namespace Shared.Messaging;

public static class PublisherSettingsFactory
{
    public static PublisherClientBuilder Create(ThroughputSettings settings)
    {
        var batchingSettings = new BatchingSettings(
            elementCountThreshold: settings.PublishBatchElementCount,
            delayThreshold: TimeSpan.FromMilliseconds(settings.PublishBatchDelayMilliseconds),
            byteCountThreshold: settings.PublishBatchByteThreshold);

        var publisherSettings = new PublisherClient.Settings
        {
            BatchingSettings = batchingSettings,
            EnableCompression = settings.EnableCompressionByDefault,
            CompressionBytesThreshold = settings.CompressionThresholdBytes
        };

        return new PublisherClientBuilder
        {
            Settings = publisherSettings
        };
    }
}

using Google.Api.Gax;
using Google.Cloud.PubSub.V1;
using Shared.Configuration;

namespace Shared.Messaging;

public static class SubscriberSettingsFactory
{
    public static SubscriberClientBuilder Create(ThroughputSettings settings)
    {
        var subscriberSettings = new SubscriberClient.Settings
        {
            // Flow control keeps memory bounded while still allowing enough in-flight work for throughput.
            FlowControlSettings = new FlowControlSettings(
                settings.MaxOutstandingMessageCount,
                settings.MaxOutstandingBytes),
            // Extends the lease before it expires to avoid redelivery on long processing.
            AckDeadline = TimeSpan.FromSeconds(settings.AckDeadlineSeconds),
            // Extension window starts early to reduce the chance of redelivery under load.
            AckExtensionWindow = TimeSpan.FromSeconds(settings.AckExtensionWindowSeconds),
            // Caps total extension time so stuck handlers eventually redeliver.
            MaxTotalAckExtension = TimeSpan.FromMinutes(settings.MaxTotalAckExtensionMinutes)
        };

        return new SubscriberClientBuilder
        {
            Settings = subscriberSettings
        };
    }
}

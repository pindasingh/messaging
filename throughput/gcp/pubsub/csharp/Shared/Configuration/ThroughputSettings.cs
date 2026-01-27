namespace Shared.Configuration;

public class ThroughputSettings
{
    public const string SectionName = "Throughput";

    public int MaxConcurrentMessages { get; set; } = 100;
    public int MaxOutstandingMessageCount { get; set; } = 1000;
    public long MaxOutstandingBytes { get; set; } = 20 * 1024 * 1024;
    public int PublishWorkerCount { get; set; } = 1;

    public int PublishBatchElementCount { get; set; } = 100;
    public int PublishBatchDelayMilliseconds { get; set; } = 50;
    public long PublishBatchByteThreshold { get; set; } = 1 * 1024 * 1024;

    public int AckDeadlineSeconds { get; set; } = 30;
    public int AckExtensionWindowSeconds { get; set; } = 10;
    public int MaxTotalAckExtensionMinutes { get; set; } = 10;

    public bool EnableCompressionByDefault { get; set; } = false;
    public int CompressionThresholdBytes { get; set; } = 4096;
}

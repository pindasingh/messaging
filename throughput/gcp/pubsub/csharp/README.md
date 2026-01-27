# High-Throughput Pub/Sub Examples

This folder contains high-throughput JSON Pub/Sub examples optimized for throughput and backpressure.

## Projects
- `Producer`: publishes JSON messages with batching, flow control, and optional compression.
- `Consumer.Pull`: pull subscriber using flow control and bounded concurrency.
- `Consumer.Push`: push subscriber that acks immediately and processes in background.
- `Shared`: shared configuration and helpers.

## Build / Run
- Restore: `dotnet restore throughput/gcp/pubsub/csharp/throughput.sln`
- Build: `dotnet build throughput/gcp/pubsub/csharp/throughput.sln`

Run examples:
- Producer: `dotnet run --project throughput/gcp/pubsub/csharp/Producer/Producer.csproj`
- Pull consumer: `dotnet run --project throughput/gcp/pubsub/csharp/Consumer.Pull/Consumer.Pull.csproj`
- Push consumer: `dotnet run --project throughput/gcp/pubsub/csharp/Consumer.Push/Consumer.Push.csproj`

## Throughput Techniques (What and When)
- Batching (`PublishBatchElementCount`, `PublishBatchDelayMilliseconds`, `PublishBatchByteThreshold`): reduces RPC overhead for many small messages.
- Flow control (`MaxOutstandingMessageCount`, `MaxOutstandingBytes`): caps in-flight work to prevent memory spikes.
- Concurrency (`MaxConcurrentMessages`): saturates CPU and I/O for message processing.
- Parallel publishers (`PublishWorkerCount`): increases throughput when a single client is CPU-bound.
- Ack deadline tuning (`AckDeadlineSeconds`, `AckExtensionWindowSeconds`, `MaxTotalAckExtensionMinutes`): avoids redelivery for long processing.
- Compression (`EnableCompressionByDefault`, `CompressionThresholdBytes`, per-request `Compress` flag): trades CPU for bandwidth on large payloads.
- Immediate push ack: returns 200 quickly and processes in background to keep delivery latency low.

## Compression Behavior
- Publisher sets Pub/Sub attribute `content-encoding=gzip` when compression is used.
- Consumers check the attribute and decompress before JSON parsing.
- Enable compression for large payloads or bandwidth-constrained environments.
- Disable compression for small payloads or CPU-bound workloads.

## Configuration
Each app includes `appsettings.json` with a `PubSub` and `Throughput` section.
- `PubSub:ProjectId`, `PubSub:TopicId` (producer), `PubSub:SubscriptionId` (consumers)
- `Throughput` tuning knobs listed above

## Message Contract
- `OrderCreatedV1` in `Shared/Messaging/OrderCreatedV1.cs`
- `POST /publish` expects `{"message": "...", "compress": true|false|null}`
- `compress` overrides the global default; `null` uses the config default + threshold.

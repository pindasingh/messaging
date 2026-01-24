# GCP Pub/Sub (C#)

Basic Pub/Sub example using a producer API, pull consumer, and push consumer.

## Projects
- `Producer`: sends JSON messages to a Pub/Sub topic.
- `Consumer.Pull`: processes subscription messages with a background service.
- `Consumer.Push`: receives Pub/Sub push deliveries via HTTP.
- `Shared`: configuration and shared event models.

## Build / Run
- Restore: `dotnet restore pub-sub/gcp/pubsub/csharp/pubsub.sln`
- Build: `dotnet build pub-sub/gcp/pubsub/csharp/pubsub.sln`

Run examples:
- Producer: `dotnet run --project pub-sub/gcp/pubsub/csharp/Producer/Producer.csproj`
- Pull consumer: `dotnet run --project pub-sub/gcp/pubsub/csharp/Consumer.Pull/Consumer.Pull.csproj`
- Push consumer: `dotnet run --project pub-sub/gcp/pubsub/csharp/Consumer.Push/Consumer.Push.csproj`

## Configuration
Update `appsettings.json` in each project with:
- `PubSub:ProjectId`
- `PubSub:TopicId` (producer only)
- `PubSub:SubscriptionId` (consumers)

## Message Contract
- `POST /publish` expects `{ "message": "..." }`.
- `POST /push` receives a Pub/Sub push envelope.
- `GET /health` returns `Healthy`.

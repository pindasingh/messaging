# Azure Event Hubs Pub/Sub (C#)

Basic Event Hubs example using a producer API and a pull-based consumer group reader.

## Projects
- `Producer`: publishes JSON events to Event Hubs.
- `Consumer`: reads events from the configured consumer group.
- `Shared`: configuration and shared event models.

## Build / Run
- Restore: `dotnet restore pub-sub/azure/event-hubs/csharp/azure-event-hubs.sln`
- Build: `dotnet build pub-sub/azure/event-hubs/csharp/azure-event-hubs.sln`

Run examples:
- Producer: `dotnet run --project pub-sub/azure/event-hubs/csharp/Producer/Producer.csproj`
- Consumer: `dotnet run --project pub-sub/azure/event-hubs/csharp/Consumer/Consumer.csproj`

## Configuration
Update `appsettings.json` in each project with:
- `EventHubs:ConnectionString`
- `EventHubs:EventHubName`
- `EventHubs:ConsumerGroup` (defaults to `$Default`)

## Message Contract
- `POST /publish` expects `{ "message": "..." }`.
- `GET /health` returns `Healthy`.

# Azure Service Bus Topics (C#)

Basic topic/subscription example using a producer API and a background consumer processor.

## Projects
- `Producer`: sends JSON messages to a Service Bus topic.
- `Consumer`: processes topic subscription messages with a background service.
- `Shared`: configuration and shared event models.

## Build / Run
- Restore: `dotnet restore pub-sub/azure/service-bus/csharp/azure-service-bus.sln`
- Build: `dotnet build pub-sub/azure/service-bus/csharp/azure-service-bus.sln`

Run examples:
- Producer: `dotnet run --project pub-sub/azure/service-bus/csharp/Producer/Producer.csproj`
- Consumer: `dotnet run --project pub-sub/azure/service-bus/csharp/Consumer/Consumer.csproj`

## Configuration
Update `appsettings.json` in each project with:
- `ServiceBus:ConnectionString`
- `ServiceBus:TopicName`
- `ServiceBus:SubscriptionName`

## Message Contract
- `POST /publish` expects `{ "message": "..." }`.
- `GET /health` returns `Healthy`.

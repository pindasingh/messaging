# Azure Service Bus Queue (C#)

Basic queue example using a producer API and a background consumer processor.

## Projects
- `Producer`: sends JSON messages to a Service Bus queue.
- `Consumer`: processes queue messages with a background service.
- `Shared`: configuration and shared event models.

## Build / Run
- Restore: `dotnet restore point-to-point/azure/service-bus/csharp/azure-service-bus.sln`
- Build: `dotnet build point-to-point/azure/service-bus/csharp/azure-service-bus.sln`

Run examples:
- Producer: `dotnet run --project point-to-point/azure/service-bus/csharp/Producer/Producer.csproj`
- Consumer: `dotnet run --project point-to-point/azure/service-bus/csharp/Consumer/Consumer.csproj`

## Configuration
Update `appsettings.json` in each project with:
- `ServiceBus:ConnectionString`
- `ServiceBus:QueueName`

## Message Contract
- `POST /publish` expects `{ "message": "..." }`.
- `GET /health` returns `Healthy`.

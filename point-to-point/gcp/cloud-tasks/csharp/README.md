# GCP Cloud Tasks (C#)

Point-to-point example using Cloud Tasks with a producer API and an HTTP consumer endpoint.

## Projects
- `Producer`: enqueues JSON tasks to a Cloud Tasks queue.
- `Consumer`: receives tasks via HTTP and processes the payload.
- `Shared`: configuration and shared event models.

## Build / Run
- Restore: `dotnet restore point-to-point/gcp/cloud-tasks/csharp/cloud-tasks.sln`
- Build: `dotnet build point-to-point/gcp/cloud-tasks/csharp/cloud-tasks.sln`

Run examples:
- Producer: `dotnet run --project point-to-point/gcp/cloud-tasks/csharp/Producer/Producer.csproj`
- Consumer: `dotnet run --project point-to-point/gcp/cloud-tasks/csharp/Consumer/Consumer.csproj`

## Configuration
Update `appsettings.json` in each project with:
- `Tasks:ProjectId`
- `Tasks:LocationId`
- `Tasks:QueueId`
- `Tasks:TargetUrl` (base URL for the consumer host)

## Message Contract
- `POST /publish` expects `{ "message": "..." }`.
- `POST /tasks` receives `OrderCreatedV1` JSON payloads.
- `GET /health` returns `Healthy`.

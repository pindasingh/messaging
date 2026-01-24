# AGENTS

## Repository overview
- This repo contains messaging examples organized by problem space.
- The top-level layout is `problem space → provider → language`.
- Schema examples add a format layer: `schema/<provider>/<language>/<format>`.
- Primary solutions today:
  - `schema/gcp-pubsub/csharp/json/schema-json.sln` (JSON payload examples)
  - `schema/gcp-pubsub/csharp/proto/schema-proto.sln` (protobuf payload examples)
  - `throughput/gcp-pubsub/csharp/throughput.sln` (high-throughput JSON examples)
- Each language folder should include its own README with build/run steps.
- Providers are lowercase kebab-case (for example, `gcp-pubsub`).
- Languages use canonical names (`csharp`, `go`, `rust`).
- Schema formats are lowercase (`json`, `proto`) and sit under language.

## Build / run / test
### Restore
- `dotnet restore schema/gcp-pubsub/csharp/json/schema-json.sln`
- `dotnet restore schema/gcp-pubsub/csharp/proto/schema-proto.sln`
- `dotnet restore throughput/gcp-pubsub/csharp/throughput.sln`

### Build
- `dotnet build schema/gcp-pubsub/csharp/json/schema-json.sln`
- `dotnet build schema/gcp-pubsub/csharp/proto/schema-proto.sln`
- `dotnet build throughput/gcp-pubsub/csharp/throughput.sln`

### Run (examples)
- JSON producer: `dotnet run --project schema/gcp-pubsub/csharp/json/Producer/Producer.csproj`
- JSON pull consumer: `dotnet run --project schema/gcp-pubsub/csharp/json/Consumer.Pull/Consumer.Pull.csproj`
- JSON push consumer: `dotnet run --project schema/gcp-pubsub/csharp/json/Consumer.Push/Consumer.Push.csproj`
- Proto producer: `dotnet run --project schema/gcp-pubsub/csharp/proto/Producer/Producer.csproj`
- Proto consumer: `dotnet run --project schema/gcp-pubsub/csharp/proto/Consumer/Consumer.csproj`
- High-throughput producer: `dotnet run --project throughput/gcp-pubsub/csharp/Producer/Producer.csproj`
- High-throughput pull consumer: `dotnet run --project throughput/gcp-pubsub/csharp/Consumer.Pull/Consumer.Pull.csproj`
- High-throughput push consumer: `dotnet run --project throughput/gcp-pubsub/csharp/Consumer.Push/Consumer.Push.csproj`
- Go throughput producer: `cd throughput/gcp-pubsub/go/producer` then `CONFIG_PATH=config.local.yaml go run .`
- Rust throughput producer: `cd throughput/gcp-pubsub/rust/producer` then `CONFIG_PATH=config.local.yaml cargo run`

### Tests
- No test projects are present today.
- If tests are added later:
  - All tests: `dotnet test <solution-or-test-project>`
  - Single test by name: `dotnet test <test-project> --filter FullyQualifiedName~Namespace.ClassName.TestName`
  - Single test by trait: `dotnet test <test-project> --filter "Category=Unit"`
  - Single test by class/file: `dotnet test <test-project> --filter FullyQualifiedName~Namespace.ClassName`

### Lint / format
- No formatter or lint config is present (`.editorconfig` not found).
- If added, prefer repo-local config and update this file.
- Suggested analyzer check (if introduced): `dotnet format --verify-no-changes`.

### Execution notes
- Requires Google credentials (e.g., `GOOGLE_APPLICATION_CREDENTIALS`).
- Populate `appsettings.json` or language-specific config files before running.
- Prefer `dotnet run --project <csproj>` to avoid solution ambiguity.
- Go/Rust apps use `config.local.yaml` with `CONFIG_PATH`.

## Configuration expectations
- Pub/Sub settings are read from `appsettings.json` via `Shared.Configuration.PubSubSettings`.
- Required fields depend on app:
  - Producer: `PubSub:ProjectId`, `PubSub:TopicId`
  - Consumers: `PubSub:ProjectId`, `PubSub:SubscriptionId`
- High-throughput examples also use `Throughput` settings (batching, flow control, compression, ack settings).

## Code style guidelines
### General C#
- Use file-scoped namespaces.
- Use `var` only when the type is obvious from the RHS; otherwise be explicit.
- Keep classes small and single-purpose; prefer handler/model classes over large `Program.cs` logic.
- Avoid one-letter variable names except for trivial loop indices.
- Keep public surface area minimal in example projects.

### Imports
- Keep `using` statements at the top of the file.
- Group system namespaces first, then third-party, then local.
- Remove unused `using` statements.
- Keep `using` order consistent with existing files.

### Naming
- PascalCase for public types, methods, and properties.
- camelCase for local variables and parameters.
- Use descriptive names for handlers and models (`OrderCreatedHandler`, `PublishMessageRequest`).
- Prefer `*Settings`, `*Options`, `*Handler`, `*Publisher` naming patterns.

### Formatting
- Use 4 spaces for indentation.
- One blank line between logical blocks in `Program.cs` (service setup, middleware, endpoints).
- Keep lambdas on a single line when short; break lines for readability when nested.
- Keep minimal APIs concise; avoid extra endpoint classes unless required.

### Types and nullability
- Nullable reference types are enabled; prefer non-nullable fields and initialize defaults.
- Use `string.Empty` and `new()` initializers for required properties.
- Return nullable types only when a value can truly be absent.
- Avoid using nullable values in message handlers without guards.

### Error handling
- Use try/catch at the boundary of message handling (see Consumer handlers).
- Log exceptions with context and include message identifiers if available.
- Prefer `Results.*` for HTTP handlers to keep minimal API consistent.
- Avoid swallowing exceptions; log with structured context.
- NACK on exceptions in pull consumers to allow retry.

### Logging
- Use structured logging (`_logger.LogInformation("... {Field}", value)`).
- Avoid string interpolation in logging calls when using placeholders.

### Dependency injection
- Register services in `Program.cs` using `AddSingleton`/`AddScoped`/`AddHostedService`.
- Prefer constructor injection for handlers and services.
- Prefer options binding via `Configure<TOptions>` for settings.

## Pub/Sub example conventions
- Producer endpoints:
  - `POST /publish` with a simple request model.
- Health checks:
  - Each app maps `GET /health`.
- Consumer patterns:
  - Pull consumer runs as a `BackgroundService`.
  - Push consumer exposes `POST /push` accepting a Pub/Sub push payload.
- Push handlers should return fast responses and log errors with message IDs.
- Pull handlers should NACK on exceptions to allow retry.

## Protobuf conventions (proto example)
- `.proto` definitions live in `schema/gcp-pubsub/csharp/proto/Shared/Proto/`.
- `Grpc.Tools` generates types; do not manually edit generated files.
- Use `ToByteString()` to publish and `Parser.ParseFrom()` to deserialize.

## JSON conventions (json example)
- JSON message model lives in `Shared/Events/OrderCreatedV1.cs`.
- Serialization uses `System.Text.Json`; keep payloads minimal and versioned.

## Cursor / Copilot rules
- No Cursor rules detected (`.cursor/rules/` or `.cursorrules`).
- No Copilot instructions detected (`.github/copilot-instructions.md`).

## House rules for agents
- Do not add new dependencies without a clear reason.
- Keep example apps minimal; avoid framework features not already in use.
- Prefer editing existing files over creating new ones unless required.
- Avoid adding inline comments unless explicitly requested.
- Do not edit generated protobuf files.
- Do not commit changes unless explicitly asked.

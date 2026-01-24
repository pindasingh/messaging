# Messaging Examples

This repository organizes messaging samples by **problem space → provider → product → language**. Problem spaces include both capabilities (like `schema`, `throughput`) and messaging patterns (like `pub-sub`, `point-to-point`).

## Directory convention
```
<problem-space>/
  <provider>/
    <product>/
      <language>/
```

Schema examples add a format layer when needed:
```
schema/<provider>/<product>/<language>/<format>/
```

## Conventions
- Use lowercase kebab-case for providers (e.g., `gcp`, `azure`, `aws`, `apache`).
- Allowed provider values are `gcp`, `azure`, `aws`, `apache`.
- Provider values must not include product names (e.g., avoid `gcp-pubsub`).
- Products/technologies live under providers (e.g., `gcp/pubsub`, `azure/service-bus`, `aws/sqs`, `apache/kafka`).
- Use canonical language folders (`csharp`, `go`, `rust`).
- Add a `README.md` inside each `<language>` folder with build/run steps.
- Keep example apps minimal and aligned with existing conventions.

# Messaging Examples

This repository organizes messaging samples by **problem space → provider → product → language**. Problem spaces describe patterns or capabilities (for example, `point-to-point`, `throughput`, `outbox`).

## Directory convention
```
<problem-space>/
  <provider>/
    <product>/
      <language>/
```

If a product has multiple approaches, insert an optional `<solution-name>` before the language:
```
<problem-space>/
  <provider>/
    <product>/
      <solution-name>/
        <language>/
```

Use a short, kebab-case approach label for `<solution-name>` (for example, `pull`, `push`, `http`, `worker`, `batch`).

## Conventions
- Use lowercase kebab-case for providers (e.g., `gcp`, `azure`, `aws`, `apache`).
- Allowed provider values are `gcp`, `azure`, `aws`, `apache`.
- Provider values must not include product names (e.g., avoid `gcp-pubsub`).
- Products/technologies live under providers (e.g., `gcp/pubsub`, `azure/service-bus`, `aws/sqs`, `apache/kafka`).
- Use canonical language folders (`csharp`, `go`, `rust`).
- Add a `README.md` inside each `<language>` folder with build/run steps.
- Keep example apps minimal and aligned with existing conventions.

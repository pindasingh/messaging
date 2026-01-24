# Messaging Examples

This repository organizes messaging samples by **problem space → provider → language**.

## Directory convention
```
<problem-space>/
  <provider>/
    <language>/
```

Schema examples add a format layer when needed:
```
schema/<provider>/<language>/<format>/
```

## Conventions
- Use lowercase kebab-case for providers (e.g., `gcp-pubsub`).
- Use canonical language folders (`csharp`, `go`, `rust`).
- Add a `README.md` inside each `<language>` folder with build/run steps.
- Keep example apps minimal and aligned with existing conventions.

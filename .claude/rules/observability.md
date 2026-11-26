---
paths:
  - "backend/**"
---

# Observability: Serilog + OpenTelemetry

> **Status:** the patterns below follow ADR-0004 D2 and D4 (hosts, `Caterly.ServiceDefaults`, backends), which is **Proposed**. Until the owner
> accepts it, they bind designs only. Implementing them (packages, hosts, schemas) waits for
> acceptance (`design-first.md`). If the owner changes a decision, this rule changes with it.

Design: `docs/architecture/system-architecture.md` §8. All hosts call `AddCaterlyDefaults()`
(`Caterly.ServiceDefaults`). Do not configure logging, tracing or metrics in individual hosts.

## Logging (Serilog)
- Inject `ILogger<T>`. Do not use the static `Log.*` outside host bootstrap.
- Use **message templates**, never string interpolation or concatenation:
  `logger.LogInformation("Selection {SelectionId} accepted for {ServiceDate}", id, date)`.
  Property names are PascalCase and stable; dashboards and alerts depend on them.
- For hot or important events, use source-generated `[LoggerMessage]` methods in a
  `<Module>Log` static partial class, with stable event IDs per module.
- Levels:
  - `Information`: business milestones, such as a selection accepted, a finalization
    completed or an invoice issued.
  - `Warning`: retries, degraded dependencies, unusual rule-violation rates.
  - `Error`: unhandled or failed operations that need attention, logged with the exception.
  - Do not log expected domain rejections (`selection.cutoff_passed`) as errors.
- Scope context comes from enrichment, not from each call: `TraceId`, `SpanId`,
  `CorrelationId`, `TenantId`, `UserId`, `Module`. Add use-case context with
  `logger.BeginScope`.
- **Never log:** passwords, tokens, cookies, authorization headers, OTPs, payment or card
  data, full request or response bodies, or personal data beyond internal IDs (no names,
  phone numbers or emails). Use destructuring policies or `[LogMasked]`-style DTOs.
- Catch an exception only to handle it or add context. Do not log and rethrow at every layer.
  The central exception handler logs unhandled exceptions once.

## Tracing
- Rely on the automatic instrumentation for HTTP, EF Core/Npgsql, Redis and RabbitMQ. Add a
  custom span (`ActivitySource` `Caterly.<Module>`, from `CaterlyTelemetry`) only for
  meaningful units: a use case spanning multiple I/O calls, finalization, an import or a
  billing run.
- Span names follow `<module>.<operation>` (e.g. `selection.finalize`). Attributes follow
  OpenTelemetry semantic conventions, plus `caterly.tenant_id` and `caterly.<entity>_id`.
  Never put PII or secrets in attributes.
- Asynchronous work continues the trace. The outbox stores `traceparent`, and consumers
  start their span as a child or link of it.
- Record exceptions on the span and set an error status when an operation fails.

## Metrics
- Instruments come from the single `Meter` named `Caterly`. Names follow
  `caterly.<module>.<thing>` with a unit. Use counters for events, histograms for durations,
  and observable gauges for backlog (for example outbox age).
- Labels have **low cardinality** only: outcome, reason code, module, meal period. Never
  user, employee, order or tenant IDs.
- A new feature design states its key metric and alert (`design-first.md` §8).

## Health checks
- Every new infrastructure dependency registers a readiness check tagged `ready`. Cache
  dependencies report `Degraded`, not `Unhealthy`.

## Tests
- Test that important log events, metrics and spans are emitted, using `FakeLogger` or an
  in-memory exporter. Example: a failed finalization logs an `Error` and increments
  `caterly.finalization.failed`.

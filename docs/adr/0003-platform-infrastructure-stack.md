# ADR-0003: Platform infrastructure stack

- **Status:** Accepted — product owner, 2026-11-27
- **Scope:** Whole repository
- **Amends:** ADR-0001 ("No … message brokers … without a new ADR"). This ADR is that ADR
  for RabbitMQ only.

## Context

Caterly needs to run reliably in production. That means:
- background and asynchronous work: notifications, imports, finalization follow-ups;
- caching for read-heavy data;
- structured logs, distributed tracing and monitoring across the API and background workers;
- standards-based authentication for several roles.

The product owner has decided the stack and set one constraint: **no paid tools or packages**.

## Decision

| Concern | Choice |
|---|---|
| Runtime | .NET 10 (ADR-0001, unchanged) |
| Database (system of record) | PostgreSQL (ADR-0001, unchanged) |
| Cache | Redis-compatible server: Redis Open Source (AGPL option) or Valkey (BSD), self-hosted |
| Message broker | RabbitMQ (MPL 2.0), self-hosted |
| Structured logging | Serilog |
| Tracing, metrics, log export | OpenTelemetry (OTLP) |
| Monitoring (application, logs, traces, alerts) | Self-hosted open-source stack (details in ADR-0004) |
| Authentication / authorization server | OAuth 2.0 / OpenID Connect with OpenIddict (Apache 2.0) |
| Licensing | Only free, open-source tools and packages (`.claude/rules/dependencies.md`) |

## Consequences

- PostgreSQL stays the only source of truth. Redis holds disposable data, and RabbitMQ
  carries messages, not state.
- Still forbidden without a new ADR: event sourcing, microservices, CQRS/mediator libraries,
  paid SaaS monitoring (Application Insights, Datadog, New Relic and similar), and commercial
  identity servers (Duende IdentityServer and similar).
- Local development needs Docker for PostgreSQL, Redis, RabbitMQ and an OTLP dashboard.
- How these pieces are wired together is in `docs/architecture/system-architecture.md`.
  Integration-level choices (messaging library, monitoring backends, identity hosting) are in
  ADR-0004.

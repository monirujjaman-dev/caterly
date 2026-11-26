# ADR-0004: Platform integration patterns

- **Status:** Proposed — awaiting product owner
- **Depends on:** ADR-0001, ADR-0003
- **Design:** `docs/architecture/system-architecture.md`

The product owner accepts or changes each decision below. Each one lists its alternatives
and its main trade-off.

## D1. Messaging: `RabbitMQ.Client` + an in-house transactional outbox and inbox
- **Decision:** Publish integration events through a `messaging.outbox` table, written in the
  same PostgreSQL transaction as the business change. A dispatcher in the Worker host sends
  them to RabbitMQ using the official `RabbitMQ.Client` package. Consumers deduplicate through
  a `messaging.inbox` table.
- **Alternatives:**
  - MassTransit: v9+ is commercial; v8 is maintained only until the end of 2026.
  - NServiceBus: commercial.
  - Wolverine: MIT licensed, but it is a mediator/CQRS framework that `ddd-strategic.md` forbids.
  - Publish directly with no outbox: this loses events if the process crashes after commit.
- **Trade-off:** about 400–600 lines of plumbing to own and test, in exchange for no license
  risk and exactly the semantics we need.

## D2. Hosts: `Caterly.Api` + `Caterly.Worker`
- **Decision:** Two deployables built from the same Domain, Application and Infrastructure
  projects. The API serves HTTP and OIDC. The Worker runs the outbox dispatcher, the message
  consumers and scheduled work.
- **Alternatives:** background services inside the API host. That is simpler, but HTTP and
  background load then scale together, and a slow consumer can hurt API latency.

## D3. Scheduled work: a database "due work" table instead of a scheduler library
- **Decision:** Time-based work (post-cutoff no-selection processing, finalization,
  reminders) goes into a `scheduling.due_work` table. Worker instances claim rows with
  `FOR UPDATE SKIP LOCKED`. Handlers are idempotent.
- **Alternatives:**
  - Quartz.NET with the ADO job store: Apache 2.0, but a heavier framework.
  - Hangfire: the core is LGPL, but useful features are in the paid Pro version.
- **Trade-off:** less built-in tooling, but no new framework, and scheduled work is
  transactional with our data.
- **Note:** when finalization runs (its trigger and due time) is an owner decision
  (system-architecture §12, item 8). This ADR chooses only the mechanism.
- **Note:** cutoff enforcement never depends on a job. Every selection command compares the
  current time with the cutoff on the server (CMD-038).

## D4. Observability backend
- **Decision:**
  - Applications export only OTLP, to an **OpenTelemetry Collector**.
  - The Collector sends metrics to **Prometheus**, traces to **Grafana Tempo** and logs to
    **Grafana Loki**.
  - **Grafana** provides the dashboards and alerting; **Alertmanager** is optional.
  - Local development uses the **.NET Aspire Dashboard** container (MIT), which receives OTLP
    directly.
- **Alternatives:**
  - Jaeger (Apache 2.0) instead of Tempo.
  - OpenSearch instead of Loki: heavier.
  - Seq: free only for a single user.
  - SigNoz: open core.
  - Elastic: license restrictions.
- **Trade-off:** Grafana, Loki and Tempo are AGPLv3. That is free to self-host unmodified,
  but any modification we distribute must be shared. Switching backends later only changes
  the Collector config, because the applications speak OTLP only.

## D5. Identity: OpenIddict inside the API host
- **Decision:**
  - The OpenIddict server runs in `Caterly.Api` at `/connect/*`.
  - User accounts use ASP.NET Core Identity.
  - The EF Core stores live in PostgreSQL, in the `identity` schema.
  - APIs validate tokens with the OpenIddict validation handler.
- **Alternatives:**
  - A separate identity host: better isolation, more to run.
  - Keycloak (Apache 2.0): a full external IdP, but heavier and Java-based.
- **Trade-off:** one deployable for now. The authorization server can be moved to its own host
  later without client changes, because clients only see OIDC endpoints.

## D6. Web authentication pattern (owner decision needed)
- **Option A (recommended): BFF.** The API host handles OIDC sign-in for the SPA and issues
  an HttpOnly, Secure, SameSite=Strict cookie with antiforgery protection. No tokens reach
  browser JavaScript. This follows the IETF guidance for browser-based apps.
- **Option B: authorization code + PKCE public client.** The SPA holds short-lived access
  tokens in memory and uses rotating refresh tokens. This is simpler server-side, but tokens
  are exposed to XSS.
- Both options keep OpenIddict as the authorization server, ready for future native apps or
  integrations.

## D7. Caching: `HybridCache` with a Redis L2
- **Decision:** Use `Microsoft.Extensions.Caching.Hybrid` with an in-memory L1 and Redis as
  L2 through `StackExchange.Redis`. Cached data is read-only: published menus, service
  configuration and permission scope.
- **Never cached for decisions:** capacity, cutoff state, balances.
- **No L1 backplane:** L1 expiry ≤ 30 s, and access-affecting entries (permission scope)
  are Redis-only with L1 disabled.

## D8. Database organization: one PostgreSQL database, one schema per module
- **Decision:** Each ADR-0001 module gets its own schema (`selection`, `billing`, …), plus
  `identity`, `messaging`, `scheduling` and `audit`. Tenant isolation is row-level through
  `TenantId` (ADR-0001).
- **Option for later:** PostgreSQL Row-Level Security as defense in depth. Owner decision;
  not in MVP.

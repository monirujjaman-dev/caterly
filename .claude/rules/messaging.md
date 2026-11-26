---
paths:
  - "backend/**"
---

# Messaging: RabbitMQ with transactional outbox and inbox

> **Status:** the patterns below follow ADR-0004 D1–D3, which is **Proposed**. Until the owner
> accepts it, they bind designs only. Implementing them (packages, hosts, schemas) waits for
> acceptance (`design-first.md`). If the owner changes a decision, this rule changes with it.

Design: `docs/architecture/system-architecture.md` §5, ADR-0004 D1–D3. Library:
`RabbitMQ.Client` only, behind Application ports. No MassTransit, NServiceBus, Wolverine or
mediator libraries.

## When to use messaging
- **Synchronous and transactional:** anything that decides a business outcome, such as
  selection acceptance, cutoff, capacity, eligibility, finalization, money and audit.
- **Integration events through the outbox:** consequences of a committed change, such as
  notifications, read-model or report refresh, cache invalidation fan-out, imports and exports.
- In-process domain events, if a design uses them, stay inside one transaction. An
  integration event crosses modules or processes.

## Publishing
- Never publish to RabbitMQ directly from a handler. Write an outbox row through
  `IIntegrationEventOutbox.Add(event)` in the **same transaction** as the business change.
  The Worker dispatcher publishes it.
- Event contracts are immutable `sealed record`s in
  `Caterly.Application/<Module>/IntegrationEvents/`, named in the past tense
  (`MenuItemChanged`), with a version suffix in the type metadata (`menus.menu_item_changed.v1`).
- Payloads carry IDs and the minimal facts needed, plus `TenantId`. They never carry
  secrets, payment data or unnecessary PII. The receiver re-reads current state if it needs
  more.
- Versions only change additively. A breaking change creates `v2`, and both versions are
  published until all consumers have migrated.

## Consuming
- Every consumer is **idempotent**. It records `(consumer, messageId)` in `messaging.inbox`
  in the same transaction as its effect, and skips duplicates.
- Consumers enforce tenancy from the envelope's `TenantId` and load data through the normal
  tenant-scoped repositories. They never trust IDs without scoping them.
- Handle out-of-order delivery. Compare versions or timestamps; do not assume ordering.
- Failures are retried a bounded number of times with backoff, then the message goes to the
  DLQ. Do not swallow exceptions to acknowledge a poison message silently.
- Each consumer owns its own queue: `caterly.<consumer-module>.<purpose>`. Exchange:
  `caterly.<publisher-module>`. Routing key: `<module>.<event>.v<n>`.

## Tests (written first)
- Unit tests: the handler writes the expected outbox row, and a rolled-back transaction
  writes none.
- Consumer tests: the effect happens; a duplicate `messageId` has no second effect; another
  tenant's IDs are rejected; a poison message ends in the DLQ.
- Integration tests (Testcontainers PostgreSQL + RabbitMQ) for the dispatcher and the
  end-to-end publish and consume path, including trace propagation.

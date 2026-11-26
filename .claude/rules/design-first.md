# System design first (mandatory)

Before implementing a feature or solving any non-trivial problem, design it, then build it
test-first (`tdd.md`). Procedure and template: `.claude/skills/design-first/SKILL.md`.
Platform baseline: `docs/architecture/system-architecture.md`.

## Which design is required
| Change | Design required before code |
|---|---|
| New module, aggregate, schema or table, API surface, integration event, cache entry, background job, auth flow; anything touching tenancy, money, cutoff/finalization, audit or identity | **Full design** by the `architect` agent in `docs/architecture/<area>.md`. The owner approves it before implementation. |
| A change inside an existing design: new rule on an existing aggregate, new query on existing data, a bug with a non-obvious cause | **Design note** in the plan or PR, covering problem, root cause or requirement, the options considered, the choice, impact on data, API, events, cache, security and observability, and the test list |
| Typo, copy change, a fully mechanical refactor, a dependency patch bump | None. Say "no design needed: <reason>" in the plan. |

When in doubt, write the design note.

## Every design answers
1. **Requirements.** Rule IDs, actors, the functional behavior, and the non-functional
   targets it must meet (§1 of the system architecture).
2. **Placement.** Module or bounded context, aggregate, layer. Is the work synchronous and
   transactional, or asynchronous through the outbox? Rule: anything that decides money,
   eligibility, cutoff or capacity is synchronous.
3. **Data.** Schema, keys, constraints, indexes, migration and backfill.
4. **Contracts.** API endpoints and integration events (name, version, payload).
5. **Cache.** What is cached, the key, TTL and invalidation — or why nothing is cached.
6. **Security.** Authorization policy, tenant and scope enforcement, audit events.
7. **Failure modes.** Retries, idempotency, what happens when RabbitMQ, Redis or the Worker
   is down, concurrency.
8. **Observability.** The spans, metrics, log events and alerts that show the feature working
   or failing.
9. **Trade-offs.** The alternatives rejected and why. What to revisit as the system grows.
10. **Owner decisions.** Every Open business question, with options and a recommendation.
    Never decide these yourself.

Implementing a design that is not yet approved, or silently departing from an approved one,
is a review **blocker**.

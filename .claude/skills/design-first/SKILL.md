---
name: design-first
description: Produce the system design for a Caterly feature or problem before any code — classify the change (full design vs design note), cover requirements and NFRs, placement (sync vs outbox), data, API/event contracts, cache, auth/tenancy, failure modes, observability, trade-offs and owner decisions, then hand off a TDD test list and slices. Use when starting any feature, fix with a non-obvious cause, integration, background job, cache, auth change, or when asked to "design", "architect", "plan the system", or "how should we build".
---

# Design first

Rules: `.claude/rules/design-first.md`. Baseline: `docs/architecture/system-architecture.md`
(read it first, and only describe how this change differs from it or builds on it).

## 1. Classify
Use the table in `design-first.md`:
- **Full design** → the `architect` agent writes `docs/architecture/<area>.md` using the
  template below, then stops for owner approval.
- **Design note** → put the short template in the plan or PR.
- **None** → write one line explaining why.

## 2. Gather
- Rule IDs and their exact text (`docs/requirements/`). List Open items as questions.
- Existing designs and ADRs for the area. Existing code in the affected modules.
- NFRs: the system targets (§1), plus anything specific to this change (peak load near
  cutoff, data volume, how long the data is kept).

## 3. Decide, using these defaults
| Question | Default (deviation needs a reason) |
|---|---|
| Where does it live? | One ADR-0001 module; the aggregate is chosen by its invariant (`domain-model` skill) |
| Synchronous or asynchronous? | Synchronous and transactional if it decides money, eligibility, cutoff, capacity or audit. Otherwise the consequence goes through the outbox to RabbitMQ (`messaging.md`). |
| Time-based work? | A `scheduling.due_work` row plus an idempotent Worker handler. Never only in-memory timers. |
| Read performance? | Indexed `AsNoTracking` projection first. Cache only if allowed by `caching.md`, with key, TTL and invalidation defined. |
| Authorization? | A named policy + a handler scope check + a 404 outside scope (`identity.md`) |
| Observability? | Automatic instrumentation; plus one business metric and one alert for anything that can silently fail (`observability.md`) |
| New dependency? | Only free/OSS, with the license verified (`dependencies.md`). A framework-level dependency needs an ADR. |

## 4. Full design template — `docs/architecture/<area>.md`
```markdown
# <Area> — design
- Status: Draft | Approved (owner, date) · Rules: CMD-… · Related ADRs: …
## 1. Requirements — functional (by actor), NFRs, out of scope, open questions
## 2. Placement — module(s), aggregates, sync vs async, sequence diagram (mermaid)
## 3. Domain model — aggregates, VOs, invariants, states, domain error codes
## 4. Data — tables (schema.table), columns, keys, constraints, indexes, migration/backfill
## 5. API — endpoints, request/response, validation, status codes, errorCodes, pagination, idempotency, policy
## 6. Integration events — name.vN, publisher, consumers, payload, ordering/idempotency
## 7. Scheduled work — due_work kinds, due time rule, idempotency key
## 8. Cache — entries, key, TTL, invalidation (or "none, because …")
## 9. Security & tenancy — scope enforcement, IDOR risks, audit events (fields)
## 10. Failure modes — dependency down, retries, concurrency, duplicate submit
## 11. Observability — spans, metrics (name/labels), log events, alerts, dashboard panel
## 12. Trade-offs — alternatives rejected, what to revisit as it grows
## 13. Test strategy — the TDD test list by layer (tdd skill §1)
## 14. Slices — ordered PRs with risk level
## 15. Owner decisions — numbered, options + recommendation
```

## 5. Design note template (plan or PR)
```markdown
### Design note
- Problem / requirement: … (rule IDs)
- Root cause (bugs): … (evidence: log/trace/test)
- Options: A … / B … → chosen: … because …
- Impact: data … · API … · events … · cache … · auth/tenancy … · observability …
- Failure modes: …
- Test list: …
```

## 6. Debugging problems design-first
For an incident or bug, before you change anything:
1. Reproduce it with a failing test (`tdd` skill §6).
2. Find the cause from evidence. Use the trace by `TraceId`, the Loki logs filtered by
   `TraceId`, `TenantId` or `CorrelationId`, the metrics, the outbox and DLQ state, and the
   database state.
3. Write the design note: root cause, options, chosen fix, and how to detect it next time
   (a metric or alert).
4. Then fix it, test-first.

## 7. Hand-off
Finish with the design path or note, the owner decisions, and the test list and slices.
**Stop for approval** on full designs. Implementation then follows the `add-use-case` and
`tdd` skills.

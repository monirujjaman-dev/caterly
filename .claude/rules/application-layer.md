---
paths:
  - "backend/src/Caterly.Application/**"
  - "backend/tests/Caterly.Application.UnitTests/**"
---

# Application layer (use cases)

- One use case = one folder `<Module>/<UseCase>/` with a command or query record, a plain
  handler class, a validator and response DTOs. No mediator library.
- A handler orchestrates and does not decide business rules:
  1. Validate the request shape.
  2. Resolve the actor, tenant and corporate scope from the `ICurrentUser`/context port.
     Never take them from the request body.
  3. Load aggregates through module repositories, scoped by `TenantId`.
  4. Check authorization: role + resource + scope (CMD-014).
  5. Call domain methods, passing `TimeProvider.GetUtcNow()`.
  6. Write the audit entry where required.
  7. Persist through the unit of work in one transaction.
  8. Return a `Result` mapped to DTOs.
- Repository interfaces live here (`Abstractions/`), are per aggregate, and express domain
  intent (`GetForEmployeeOnDateAsync`). Never expose `IQueryable`. Not-found and out-of-scope
  results look the same, so the API returns 404 for both (IDOR protection).
- Queries (read side) may project straight to DTOs through a read port. They bypass
  aggregates but **never** bypass tenant and scope filters. Collections are paginated.
- Selection and order submissions and payment callbacks accept an idempotency key
  (CLAUDE.md). Other commands need one only if the design says so.
- Every async method takes a `CancellationToken`.
- Tests (`tests/Caterly.Application.UnitTests`, created with the first use case): written
  first; use hand-written in-memory fakes for ports (no mocking library without a stated
  reason). Cover: success, each domain error surfacing, authorization denial, cross-tenant
  and out-of-scope IDs, the idempotent replay, and the audit entry content.

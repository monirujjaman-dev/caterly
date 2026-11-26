---
paths:
  - "backend/src/Caterly.Infrastructure/**"
---

# Infrastructure layer (persistence and adapters)

- Map aggregates with `IEntityTypeConfiguration<T>` per aggregate in `<Module>/`. Keep
  persistence concerns out of Domain: no data annotations on domain types.
- Map value objects as owned types or value conversions (strongly typed IDs → `uuid`,
  `Money` → `numeric(18,2)` + currency code, rates `numeric(18,4)`, instants → `timestamptz`).
- Every tenant-owned table has `TenantId`, an index starting with `TenantId`, and a global
  query filter. Handlers still check scope explicitly (defense in depth).
- Business uniqueness is a database unique constraint, not just a check in code. Example:
  one active selection per employee + service date + meal period, as a partial unique index
  where the service does not allow more (CMD-028).
- Use optimistic concurrency (`xmin`) on aggregates that face races near cutoff or capacity.
- Repositories load whole aggregates. Use `AsNoTracking` projections for reads. No N+1.
- Provider integrations (payment, SMS, email) are adapters behind Application ports. No
  vendor is chosen yet, so do not add one.
- Schema change → follow the `ef-migration` skill (`.claude/skills/ef-migration/SKILL.md`).
- Persistence is tested against real PostgreSQL (Testcontainers), never EF InMemory or
  SQLite. Those providers ignore query filters, constraints and `numeric` behavior differently.

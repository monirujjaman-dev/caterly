---
paths:
  - "backend/**"
---

# Caching: HybridCache + Redis

Design: `docs/architecture/system-architecture.md` §6, ADR-0004 D7.

- PostgreSQL is the source of truth. Code must be **correct with an empty cache or with
  Redis down**; it may only be slower.
- Use `HybridCache` through a module-owned Application port (for example
  `IPublishedMenuCache`). Do not inject `IConnectionMultiplexer` into handlers.
- Build keys only through `CacheKeys`:
  `caterly:{env}:{tenantId|global}:{module}:{name}:{id}:v{schema}`. Any tenant-owned value
  **must** include `tenantId` in the key. A test proves that two tenants never share a key.
- Every entry has an explicit TTL and tags for invalidation. Invalidate after the
  transaction commits, by tag or key. The resulting integration event invalidates other
  instances.
- **Cacheable:** published menus and calendars, service and agreement configuration (as read
  models), resolved permission scope (TTL ≤ 5 min, invalidated on permission change), and
  lookup data.
- **Never cache values used to decide** capacity or availability, cutoff, eligibility at
  submit time, prices or allowances applied to a transaction, balances, or invoice totals.
  Always read those from PostgreSQL inside the transaction.
- Never cache secrets or tokens. Cache personal data only when a design approves it, and
  then with a short TTL.
- Do not use Redis locks to protect correctness. Use PostgreSQL constraints, transactions
  and `xmin`.
- Tests: a cache hit and a miss give the same result; a write invalidates the entry; the key
  includes the tenant; the system behaves correctly when the cache throws or is unavailable.

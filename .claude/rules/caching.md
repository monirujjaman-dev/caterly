---
paths:
  - "backend/**"
---

# Caching: HybridCache + Redis

> **Status:** the patterns below follow ADR-0004 D7, which is **Proposed**. Until the owner
> accepts it, they bind designs only. Implementing them (packages, hosts, schemas) waits for
> acceptance (`design-first.md`). If the owner changes a decision, this rule changes with it.

Design: `docs/architecture/system-architecture.md` §6, ADR-0004 D7.

- PostgreSQL is the source of truth. Code must be **correct with an empty cache or with
  Redis down**; it may only be slower.
- Use `HybridCache` through a module-owned Application port (for example
  `IPublishedMenuCache`). Do not inject `IConnectionMultiplexer` into handlers.
- Build keys only through `CacheKeys`:
  `caterly:{env}:{tenantId|global}:{module}:{name}:{id}:v{schema}`. Any tenant-owned value
  **must** include `tenantId` in the key. A test proves that two tenants never share a key.
- Every entry has an explicit TTL and tags for invalidation. Invalidate after the
  transaction commits, by tag or key.
- `HybridCache` has **no L1 backplane**. An in-memory L1 entry on another API instance lives
  until its own L1 expiry; neither a Redis removal nor an integration event consumed in the
  Worker clears it. So:
  - set the L1 expiry to 30 s or less on every entry;
  - for any entry that affects access, disable L1 (`HybridCacheEntryFlags.DisableLocalCache`)
    so every instance reads the shared Redis entry.
- **Cacheable:** published menus and calendars, service and agreement configuration (as read
  models), lookup data, and the resolved permission scope under these rules:
  - Redis only (L1 disabled), TTL ≤ 60 s;
  - removed from Redis in the same request that changes or revokes the permission, right
    after commit;
  - if Redis is unavailable, read the scope from PostgreSQL (never fall back to a stale copy);
  - the maximum time a revocation may take to apply is an owner decision
    (system-architecture §12).
- **Never cache values used to decide** capacity or availability, cutoff, eligibility at
  submit time, prices or allowances applied to a transaction, balances, or invoice totals.
  Always read those from PostgreSQL inside the transaction.
- Never cache secrets or tokens. Cache personal data only when a design approves it, and
  then with a short TTL.
- Do not use Redis locks to protect correctness. Use PostgreSQL constraints, transactions
  and `xmin`.
- Tests: a cache hit and a miss give the same result; a write invalidates the entry; the key
  includes the tenant; the system behaves correctly when the cache throws or is unavailable.

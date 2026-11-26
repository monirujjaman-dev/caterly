---
paths:
  - "backend/src/Caterly.Domain/**"
  - "backend/tests/Caterly.Domain.UnitTests/**"
---

# Domain layer (tactical DDD)

The Domain project is pure C#. It references no Caterly project, ASP.NET Core, EF Core or
Npgsql. `DomainAssemblyTests` enforces this; never weaken that test.

## Aggregates
- An aggregate is the consistency boundary for invariants that must hold **in one
  transaction**. Keep aggregates small. Reference other aggregates by ID only.
- All state changes go through intention-revealing methods named in domain language
  (`selection.Change(menuItem, now)`, `agreement.Activate(now)`). No public setters, and no
  `Update(dto)` methods that copy fields.
- Constructors and factory methods (`MealSelection.Create(...)`) return only valid objects.
  Collections are exposed as `IReadOnlyCollection<T>`.
- Every tenant-owned aggregate carries an immutable `TenantId`.
- Invariants that span many aggregates (capacity across employees, uniqueness) cannot be
  enforced inside one aggregate. Name the enforcement mechanism in the design: a database
  constraint, a concurrency token, or a dedicated aggregate. Never enforce them only in the UI.

## Value objects
- Use value objects for domain concepts, not primitives: `Money` (decimal + currency),
  `ServiceDate` (`DateOnly`), `MealPeriodId`, `EmployeeId`, `TenantId`, effective date ranges.
- Make them immutable `readonly record struct` or `sealed record` types that validate on
  creation. Strongly typed IDs wrap `Guid`.
- `Money` never uses `double`/`float`. All rounding goes through the single rounding policy
  in `Domain/Common`. Its mode is pending finance (ADR-0001), so never hardcode a new one.

## Time and determinism
- Domain methods take the current instant as a parameter (`DateTimeOffset nowUtc`). The
  Domain never calls `DateTime.Now`, `DateTimeOffset.UtcNow` or `TimeProvider` itself.
- Cutoff and service-date logic converts explicitly between UTC and the service timezone
  (default `Asia/Dhaka`, CMD-037). The timezone comes in as data.
- Domain code does no I/O and is not async.

## Errors
- Expected business-rule failures return a `Result`/`Result<T>` carrying a `DomainError`
  with a stable code `<module>.<rule>`, e.g. `selection.cutoff_passed`,
  `selection.employee_not_eligible`. Tests assert on the code.
- Exceptions are only for programmer errors (null arguments, impossible states).

## History and immutability
- Finalized snapshots, issued invoices and completed payments are immutable once created.
  Later changes are new adjustment or version objects (CMD-042).
- Record selection origin (`Employee`, `Administrator`, `Automatic`) on the aggregate (CMD-015,
  §3.7). Post-cutoff overrides carry actor, reason, previous and new value (CMD-033).

## Tests
- Every invariant, state transition and calculation has unit tests, written first, with no
  mocks. Use builders or object mothers for valid aggregates.
- Cover boundaries: one tick before, exactly at, and after cutoff; effective-date start and
  end; zero amounts and rounding edges; inactive or suspended employees.

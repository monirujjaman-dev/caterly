---
name: add-use-case
description: Implement one Caterly backend use case end to end, test-first and inside-out — domain behavior, application handler with tenancy/authorization/audit, persistence, and a thin minimal-API endpoint — following the Clean Architecture + DDD conventions. Use when adding or changing a command or query in backend/, e.g. "add select-meal endpoint", "implement the handler", or "wire this use case".
---

# Add a use case (inside-out, test-first)

Preconditions: a model sketch (`domain-model` skill) and, for new modules, schema or API
surface, an approved `architect` design. Follow the `tdd` skill for every step below. Each
step starts with a failing test.

## 0. Shared kernel (first feature only)
If `Caterly.Domain/Common` lacks them, introduce the minimum, test-first: `TenantId`,
strongly typed ID pattern, `DomainError`, `Result`/`Result<T>`, `Money` + rounding policy
(mode pending finance — leave the mode as an explicit, documented parameter). Keep each one
small. Example shapes:

```csharp
namespace Caterly.Domain.Common;

public sealed record DomainError(string Code, string Message, ErrorKind Kind = ErrorKind.RuleViolation);

public enum ErrorKind { Validation, NotFound, Conflict, RuleViolation }

public readonly record struct TenantId(Guid Value);
```

## 1. Domain behavior — `tests/Caterly.Domain.UnitTests/<Module>/`
```csharp
public class MealSelectionTests
{
    [Fact, Trait("Rule", "CMD-038")]
    public void Change_exactly_at_cutoff_is_rejected_for_employee()
    {
        var selection = MealSelectionMother.ActiveBefore(cutoffUtc: Cutoff);

        var result = selection.Change(OtherItem, SelectionOrigin.Employee, nowUtc: Cutoff);

        Assert.Equal("selection.cutoff_passed", result.Error?.Code);
    }
}
```
Then implement the method on the aggregate. No I/O, `nowUtc` passed in.

## 2. Application handler — `tests/Caterly.Application.UnitTests/<Module>/<UseCase>/`
Create the test project on first use (mirror `Caterly.Domain.UnitTests`, reference
Application; add it to `Caterly.slnx`). Use hand-written fakes for ports: `InMemory<X>Repository`,
`FakeCurrentUser`, `RecordingAuditLog`, and a fake `TimeProvider`.

Test, in this order: success → each domain error propagates → wrong role is rejected →
another tenant's or out-of-scope ID returns NotFound → idempotent replay returns the first
result with no duplicate write → audit entry contains actor, action, entity, UTC time,
old/new value and reason.

Handler shape:
```csharp
public sealed class ChangeMealSelectionHandler(
    IMealSelectionRepository selections,
    ICurrentUser currentUser,
    IAuditLog audit,
    IUnitOfWork unitOfWork,
    TimeProvider time)
{
    public async Task<Result<MealSelectionDto>> HandleAsync(ChangeMealSelection command, CancellationToken ct)
    {
        // validate → scope from currentUser → load by (TenantId, id) → authorize
        // → domain call with time.GetUtcNow() → audit → unitOfWork.SaveChangesAsync(ct) → map
    }
}
```
Register the handler in `Caterly.Application/DependencyInjection.cs`.

## 3. Persistence — `Caterly.Infrastructure/<Module>/`
EF configuration, repository and global tenant filter. Add the migration with the
`ef-migration` skill. Prove constraints (uniqueness, concurrency) and the tenant filter with
integration tests against PostgreSQL (Testcontainers). If Docker is unavailable, say so in
the report. Do not substitute InMemory.

## 4. Endpoint — `Caterly.Api/<Module>/`
Write the integration test first (`Caterly.Api.IntegrationTests/<Module>/`): 2xx happy path,
400, 401, 403, cross-tenant 404, 409/422 with ProblemDetails `errorCode`, and idempotent
replay. Then map:
```csharp
group.MapPut("/selections/{id:guid}", async (Guid id, ChangeMealSelectionRequest body,
        ChangeMealSelectionHandler handler, CancellationToken ct) =>
    (await handler.HandleAsync(body.ToCommand(id), ct)).ToHttpResult())
    .RequireAuthorization(Policies.Employee)
    .WithName("ChangeMealSelection");
```

## 5. Finish
Run the full backend verification (`docs/engineering/commands.md`). Fill the PR's TDD evidence
table, and list the API and migration changes.

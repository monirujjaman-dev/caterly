---
name: tdd
description: Run Caterly's red-green-refactor loop for a backend or web change — build a test list from rule IDs, write one failing test, confirm it fails for the right reason, make it pass minimally, refactor, and record TDD evidence for the PR. Use whenever writing or changing production code, fixing a bug, or when asked to "do TDD", "test first", or "write the tests first".
---

# TDD loop for Caterly

The rules are in `.claude/rules/tdd.md`. This skill is the procedure.

## 1. Build the test list (before any code)
Read the rule IDs for the task (`docs/requirements/`). For each rule, write one line per
behavior into your todo list:

```
[CMD-038] Employee selection exactly at cutoff instant is rejected (selection.cutoff_passed)
[CMD-038] Employee selection one tick before cutoff is accepted
[CMD-032] Administrator with override permission can change after cutoff, reason required
[CMD-033] Override records actor, reason, previous and new item
[CMD-014] Admin of account A gets 404 for an employee of account B
[CMD-028] Second active selection for same employee/date/period is rejected
```
Cover every group: happy path, boundaries, rejections (one per error code), tenancy and
scope, authorization, idempotent replay, and audit content. If a behavior depends on an Open
or undecided rule, stop and ask. Do not write a test that encodes a guess.

Order the list from the innermost layer outward (outside-in is fine for *discovery*, but
make each item pass inside-out):
**Domain → Application → Infrastructure/API → Web.**

## 2. Red
Write exactly one test from the list. Run only that test:

| Area | Command (working dir) |
|---|---|
| Backend, one class | `dotnet test --filter "FullyQualifiedName~MealSelectionTests"` (`backend/`) |
| Backend, one rule | `dotnet test --filter "Rule=CMD-038"` (`backend/`) |
| Web, one test | `npx vitest run src/<path>.test.tsx -t "<name>"` (`web/`) |

Check the failure. A compile error for a not-yet-existing member is an acceptable first red
in C#. Add the empty member or stub, run again, and get to an **assertion** failure that
states the missing behavior. Write down the failure message, because it goes in the PR.

If the test passes immediately, either the behavior already exists (look at why, and keep the
test only if it adds protection) or the test is wrong. Fix it until it can fail.

## 3. Green
Write the simplest production code that passes. Hardcoding is fine if the next test will
force generalization. Do not add behavior that no test on the list needs. Run the test again.

## 4. Refactor
Only on green. Make names match the ubiquitous language. Remove duplication in production code
and in tests (builders, object mothers). Move logic inward: from the endpoint to the handler
to the domain. Then run the whole affected suite:
- Backend: `dotnet build --no-restore -warnaserror -c Release && dotnet test --no-build -c Release`
- Web: `npm run lint && npm run typecheck && npm test -- --run`

## 5. Repeat and commit
Tick the item and pick the next one. Commit at green after each coherent group of items
(Conventional Commits, rule IDs in the body). Never commit red tests to the branch unless they
are marked as the bug-reproduction step of a fix and the fix commit follows immediately.

## 6. Bug fixes
1. Write a test that reproduces the reported behavior and fails.
2. Fix the code.
3. Keep the test. Name it after the correct behavior, not the bug number.

## 7. Evidence for the PR
Add this section to the PR body:

```markdown
## TDD evidence
| Rule | Test | Red reason observed |
|---|---|---|
| CMD-038 | `MealSelectionTests.Change_exactly_at_cutoff_is_rejected_for_employee` | Expected error `selection.cutoff_passed`, got success |
```
List any exempt files (configuration or scaffolding) and why.

## Anti-patterns that reviewers reject
- Tests written after the code that only mirror its implementation.
- Mocking domain objects, or asserting call counts on internal collaborators.
- One giant test per use case instead of one behavior per test.
- `Thread.Sleep` or real clocks. Pass `nowUtc` into domain methods. In handlers, use a fake
  `TimeProvider` (`FakeTimeProvider` from `Microsoft.Extensions.TimeProvider.Testing`, added
  to `Directory.Packages.props` when it is first needed).
- EF InMemory or SQLite standing in for PostgreSQL in persistence tests.
- Snapshot tests of large component trees as the only web assertion.

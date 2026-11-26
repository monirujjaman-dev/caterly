## Summary

<!-- What and why. Link the issue: Closes #123 -->

## Business rules

<!-- Rule IDs implemented or affected, e.g. CMD-028, CMD-032, SEL-010. Note any open question. -->

## Changes

- **Database / migrations:** none | <migration name + summary; destructive? backfill?>
- **API:** none | <endpoints added/changed; breaking?>
- **Authorization / tenancy:** <how access and TenantId scoping are enforced>
- **Audit:** none | <actions audited>

## TDD evidence

<!-- Required for behavior changes (.claude/rules/tdd.md). Exempt files (config/scaffold/docs): say why. -->

| Rule | Test | Red reason observed |
|---|---|---|
| | | |

## Verification

| Check | Command | Result |
|---|---|---|
| Backend build/tests | `dotnet build -warnaserror && dotnet test` (backend/) | |
| Web lint/typecheck/tests/build | `npm run lint && npm run typecheck && npm test -- --run && npm run build` (web/) | |

## Risk

<!-- Output of .claude/scripts/pr-risk.sh --local. High risk needs the owner-approved label. -->

## Known limitations / follow-ups

## Screenshots (UI changes)

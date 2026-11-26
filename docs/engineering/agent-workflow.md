# Agent development workflow

Caterly is developed by eight agents in `.claude/agents/`, orchestrated by slash commands in
`.claude/commands/`. The product owner decides business rules, accepts ADRs, and approves
high-risk merges.

## Roles

| Agent | Does | Writes to | Never does |
|---|---|---|---|
| `architect` | Domain model, module boundaries, schema, API contracts, security/tenancy, test strategy, implementation slices; drafts Proposed ADRs | `docs/architecture/`, `docs/adr/` | Write app code, accept its own ADRs, invent policy |
| `ui-ux-designer` | User flows, screen inventory, wireframes, component states, copy, accessibility; maintains the design system | `docs/ux/` | Write app code, decide business rules |
| `backend-developer` | Domain, application use cases, EF Core persistence/migrations, API endpoints, backend tests | `backend/` | Change `web/`, merge |
| `frontend-developer` | Responsive, accessible React screens from the UX spec and API contract, web tests | `web/` | Put business rules in the UI, add frameworks without an ADR, merge |
| `developer` | Generalist for small cross-stack fixes that need no design or UX work; shared workflow rules used by all implementers | any code | Merge, change requirements, push to `main` |
| `code-reviewer` | Independent read-only review of the local diff against requirements and standards | — | Edit code |
| `pr-reviewer` | Reviews the GitHub PR, posts inline comments and a verdict tied to the head SHA, sets `agent-approved` or `changes-requested` | PR comments/labels | Edit code, merge |
| `pr-merger` | Checks the merge gate and squash-merges, then syncs `main` | — | Merge high-risk PRs without `owner-approved`, bypass checks |

## Routing

| The task involves… | Stages |
|---|---|
| New module/entity/schema, new or changed API, cross-module flow, integration, tenancy, money, cutoff/finalization, audit | `architect` first (owner approves the design) |
| A new screen, flow, or significant UI change | `ui-ux-designer` before `frontend-developer` |
| Backend code | `backend-developer` |
| Web code | `frontend-developer` (same branch/PR as the backend part of the feature) |
| Small fix, no design/UX need | `developer` |
| Always | `code-reviewer` → `pr-reviewer` → CI → `pr-merger` |

## Development method: design first, DDD + TDD

All implementation starts from a system design (full design or design note), then is
domain-driven and test-first. The agent-loaded guidance lives in
`.claude/`. Agents read it by path.

| Kind | File | Loaded |
|---|---|---|
| Rule | `.claude/rules/design-first.md` (full design vs design note) | Always |
| Rule | `.claude/rules/dependencies.md` (free/OSS only, license check) | Always |
| Rule | `.claude/rules/tdd.md` | Always |
| Rule | `.claude/rules/observability.md`, `messaging.md`, `caching.md` | `backend/**` |
| Rule | `.claude/rules/identity.md` | `backend/**`, `web/src/**` |
| Rule | `.claude/rules/ddd-strategic.md` (language, bounded contexts, layout) | `backend/**`, `docs/architecture/**`, `docs/adr/**` |
| Rule | `.claude/rules/domain-layer.md`, `application-layer.md`, `infrastructure-layer.md`, `api-layer.md` | That layer's source and tests |
| Rule | `.claude/rules/web.md` | `web/**` |
| Skill | `.claude/skills/design-first/` | System design procedure, full design and design-note templates |
| Skill | `.claude/skills/tdd/` | Red → green → refactor procedure and PR evidence |
| Skill | `.claude/skills/domain-model/` | Rule IDs → language, context, aggregates, errors, test list |
| Skill | `.claude/skills/add-use-case/` | Inside-out backend slice: domain → handler → persistence → endpoint |
| Skill | `.claude/skills/ef-migration/` | Safe migration generation and SQL review |
| Glossary | `docs/architecture/ubiquitous-language.md` | Terms used in code, API, UI |
| Baseline | `docs/architecture/system-architecture.md` | Platform architecture every design builds on |

Reviewers treat a behavior change without a test that fails without it as **major**.

## Pipeline (`/ship`)

```
task/issue
  → triage (routing table)
  → architect: design + owner decisions ──→ owner approves      (if needed)
  → ui-ux-designer: UX spec                                      (if UI)
  → backend-developer: implement + tests → draft PR              (if backend)
  → frontend-developer: implement + tests on the same branch     (if web)
  → code-reviewer: findings → owning agent fixes (max 3 rounds)
  → PR marked ready
  → pr-reviewer: review on GitHub ── changes requested ──→ owning agent fixes → re-review
  → CI green
  → pr-merger: gate check
       low risk  → squash-merge
       high risk → label `needs-owner-approval`, stop, notify the owner
```

Large features follow the architect's implementation slices: one PR per slice.

The owner resumes a high-risk PR by reviewing it, adding `owner-approved`, and running
`/merge-pr <n>` (or merging manually).

## Merge gate (all must hold)

1. PR is not draft and targets `main`.
2. Required checks `ci` and `PR title` succeeded on the current head SHA.
3. Latest `pr-reviewer` verdict is `APPROVED` and names the current head SHA
   (any new commit invalidates the approval).
4. No unresolved review threads; no `do-not-merge` label.
5. Branch is up to date with `main` (otherwise update the branch and wait for CI).
6. `.claude/scripts/pr-risk.sh <n>` returns `low`, **or** the PR has `owner-approved`.

## Labels

| Label | Set by | Meaning |
|---|---|---|
| `risk:low` / `risk:high` | CI (`pr-hygiene.yml`) and agents | Output of `pr-risk.sh` |
| `agent-approved` | `pr-reviewer` | Agent approval of the head SHA in the verdict comment |
| `changes-requested` | `pr-reviewer` | Blocking findings exist |
| `needs-owner-approval` | `pr-merger` | High-risk PR waiting for the owner |
| `owner-approved` | **Product owner only** | Owner accepts the high-risk change |
| `do-not-merge` | Anyone | Hard stop |

## Review standard (code-reviewer and pr-reviewer)

Report only validated findings, ranked by severity:

- **blocker** — wrong business behavior vs. `CMD-`/requirement IDs, data loss, financial
  inaccuracy, tenant leak/IDOR, missing authorization, unsafe migration, secrets, broken build/tests.
- **major** — missing tests for changed rules, missing audit, race/idempotency gaps, N+1, missing
  validation, breaking API change not called out.
- **minor/nit** — naming, readability, small simplifications.

Approve only when there are no blockers or majors.

## GitHub Actions

- `ci.yml` — backend and web build/test (skips parts that don't exist yet), secret scan, single
  required `ci` gate job.
- `pr-hygiene.yml` — Conventional Commit PR title (`PR title` check) and risk labels.
- `claude-review.yml` / `claude.yml` — optional Claude reviews on PRs and `@claude` mentions from
  the owner/collaborators. Requires the `ANTHROPIC_API_KEY` (or `CLAUDE_CODE_OAUTH_TOKEN`)
  repository secret; they are skipped without one.

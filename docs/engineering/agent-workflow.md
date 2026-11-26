# Agent development workflow

Caterly is developed by four agents in `.claude/agents/`, orchestrated by slash commands in
`.claude/commands/`. The product owner decides business rules and approves high-risk merges.

## Roles

| Agent | Does | Never does |
|---|---|---|
| `developer` | Understands the task, plans, implements with tests on a feature branch, runs build/tests, commits, pushes, opens/updates the PR | Merge, change requirements, push to `main` |
| `code-reviewer` | Independent read-only review of the local diff against requirements and standards | Edit code |
| `pr-reviewer` | Reviews the GitHub PR, posts inline comments and a verdict tied to the head SHA, sets `agent-approved` or `changes-requested` | Edit code, merge |
| `pr-merger` | Checks the merge gate and squash-merges, then syncs `main` | Merge high-risk PRs without `owner-approved`, bypass checks |

## Pipeline (`/ship`)

```
task/issue
  → developer: understand → plan (stop & ask on open business rules)
  → developer: implement + tests on branch → build/test green → commit → push → draft PR
  → code-reviewer: findings
  → developer: fix (max 3 rounds; then stop and report)
  → PR marked ready
  → pr-reviewer: review on GitHub  ── changes requested ──→ developer fixes → re-review
  → CI green
  → pr-merger: gate check
       low risk  → squash-merge
       high risk → label `needs-owner-approval`, stop, notify the owner
```

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

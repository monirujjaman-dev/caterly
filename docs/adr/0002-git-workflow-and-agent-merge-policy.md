# ADR-0002: Trunk-based git workflow and agent merge policy

- **Status:** Accepted — product owner, 2026-11-26

## Context

Development is done mostly by AI agents (developer, code reviewer, PR reviewer, PR merger) with
one product owner. We need a fast, automated flow without letting an agent merge changes that put
money, security or tenant data at risk.

## Decision

1. **Trunk-based on `main`**, short-lived branches, Conventional Commits, PRs required,
   **squash-merge only**, linear history, branches deleted on merge.
2. `main` is protected: PR required, required checks `ci` and `PR title`, branch up to date,
   conversations resolved, no force push, no deletion, rules enforced for admins.
3. **Risk-tiered merge gate**:
   - **Low risk** → the `pr-merger` agent may merge after green CI, agent approval on the
     current head SHA, and resolved threads.
   - **High risk** → also needs the product owner to add the `owner-approved` label.
     High risk = billing, invoices, payments, refunds, pricing/allowance, identity/auth,
     permissions, tenancy, cutoff/finalization, database migrations, CI/workflow or agent
     configuration, requirements and ADRs, dependency manifests, or a diff over 600 lines.
   - Classification is done by `.claude/scripts/pr-risk.sh` (single source; used by agents,
     the guard hook and CI).
4. Agents never add `owner-approved`, never use `--admin`, `--no-verify`, or force-push `main`.
   A PreToolUse hook (`.claude/hooks/guard-bash.mjs`) blocks these.

## Consequences

- Because agents act through the owner's GitHub account, GitHub cannot distinguish an agent from
  the owner. GitHub "required approvals" is therefore not used (an author can't approve their
  own PR); the gate is enforced by the hook, the agent instructions and the `risk` CI label. A
  separate bot account or GitHub App would make the gate enforceable by GitHub itself; that is a
  recommended later improvement.
- The initial bootstrap commit was pushed directly to `main` because the repository was empty.

# Caterly — Agent Instructions

Caterly is a multi-tenant SaaS for catering and food businesses (initial market: Bangladesh).
It covers corporate meal management (agreements, employees, meal calendars, selection, cutoff,
finalization, company/employee billing) and regular customer ordering (guest/registered,
immediate/scheduled/pre-order/event, payments, refunds). Treat it as a real commercial product.

Act as a senior architect and full-stack engineer: understand the business process before
writing code, and understand existing behavior before changing it.

## Sources of truth (priority order)

1. Explicit product-owner decisions (recorded in `docs/adr/` or in the issue/PR).
2. Finalized business rules — `docs/requirements/Caterly_Corporate_Meal_Business_Decisions_v1.0.md`
   (rule IDs `CMD-xxx`; authoritative for corporate meals).
3. Draft specs — see `docs/requirements/README.md` (SRS `v0.3`, Flows `v1.0`; items marked
   Proposed/Open are **not** approved).
4. Architecture decisions — `docs/adr/`.
5. Existing code and conventions.
6. General best practice, then your own recommendations.

When sources conflict, or a rule is Open, **stop and ask** — do not pick an interpretation for
anything touching business behavior, money, permissions, tenancy, or historical data.
Never invent business policy. Never assume: employee-contribution collection channel, payroll
integration, payment gateway, SMS/email vendor, tax/VAT treatment, native-app roadmap.

## Non-negotiable domain rules (summary — the docs win on detail)

- Tenant isolation on **every** tenant-scoped query and command, enforced server-side.
  Corporate admins act only within their assigned account and locations. Prevent IDOR.
- Effective-dated agreements; historical orders/invoices keep the terms they were created with.
- One active selection per employee + service date + meal period unless the service enables more.
- Selection validates eligibility, publication, availability, capacity and cutoff on the server.
- Cutoff configurable per service/date/meal period; default timezone `Asia/Dhaka`; store UTC.
- No-selection policy: No meal (default) / default meal / admin assigns. Record selection origin
  (employee / administrator / automatic).
- Finalization produces an immutable snapshot or version; later changes are auditable adjustments.
- Payment models: company-funded, employee-funded, shared allowance — per agreement. Track gross,
  company contribution, employee contribution, and each party's billable amount separately.
- Selected, finalized, planned, prepared, dispatched, delivered and billed quantities are distinct.
- Order status and payment status are independent. Creating an order never means it is paid.
- Money: `decimal`/`numeric` only, explicit rounding, price/allowance snapshots on each transaction.
  Never overwrite issued invoices or completed payments — use credit/debit/refund/reversal records.
- Audit (actor, action, entity, UTC timestamp, old/new values, reason where required) for:
  account/agreement changes, eligibility, menu publication, selections, overrides, cutoff,
  finalization, adjustments, order transitions, invoices, payments/refunds, permission changes.
- Payment callbacks and order/selection submissions must be idempotent.

## Tech stack and layout

See `docs/adr/0001-tech-stack.md`. Summary: .NET 10 (ASP.NET Core, EF Core + Npgsql),
PostgreSQL, React + TypeScript web, responsive (no native apps for MVP).

```
backend/   Caterly.slnx, src/Caterly.{Domain,Application,Infrastructure,Api}, tests/
web/       React + TypeScript app
docs/      requirements/, adr/, engineering/, architecture/ (architect designs), ux/ (UX specs)
```

Until code exists, the first feature PR that scaffolds a project must follow the ADR and keep it
minimal. Do not add frameworks, message buses, CQRS libraries, microservices or event sourcing
without an ADR approved by the product owner.

Engineering rules: thin endpoints; business rules in Domain/Application; validate every request;
centralized error handling with ProblemDetails; pagination on collections; `CancellationToken`
on async paths; OpenAPI; EF migrations for every schema change, never destructive without
approval; no secrets in code; no logging of passwords, tokens or payment data; avoid N+1.

## Commands

Commands live with their projects (run from the directory shown):

| Area | Working dir | Source |
|---|---|---|
| Backend build/test/format | `backend/` | `dotnet` CLI — see `docs/engineering/commands.md` |
| Web lint/typecheck/test/build | `web/` | `web/package.json` scripts |
| CI definition | repo root | `.github/workflows/ci.yml` |

A change is not done because it compiles: run the relevant tests and report results.

## Git workflow (summary — full rules in `docs/engineering/git-workflow.md`)

- Trunk-based on `main`. Never commit or push to `main` directly; never force-push `main`.
- Branch per task: `feat/<scope>-<slug>`, `fix/…`, `chore/…`, `docs/…`, `refactor/…`, `test/…`.
- Conventional Commits (`feat(selection): enforce cutoff on update`). Small, focused commits.
- PR title is also Conventional Commits (it becomes the squash commit). Squash-merge only.
- Keep branches current with `git fetch origin && git rebase origin/main`; use
  `--force-with-lease` only on your own feature branch.
- Never use `--no-verify`, `gh pr merge --admin`, or disable checks.

## Agent workflow

Full pipeline and merge gate: `docs/engineering/agent-workflow.md`.

| Task | Use |
|---|---|
| Implement a feature/fix end to end | `/ship <issue # or description>` |
| Design a module, schema, API or cross-module flow; draft ADRs | `architect` agent |
| Design a screen or flow (UX spec, wireframes, states) | `ui-ux-designer` agent |
| Implement backend (`backend/`) | `backend-developer` agent |
| Implement web UI (`web/`) | `frontend-developer` agent |
| Small cross-stack fix with no design/UX need | `developer` agent |
| Review local changes before a PR | `code-reviewer` agent |
| Review a GitHub PR | `/review-pr <n>` → `pr-reviewer` agent |
| Merge a reviewed PR | `/merge-pr <n>` → `pr-merger` agent |

Merge gate: the `pr-merger` agent merges only **low-risk** PRs that have green CI, an agent
approval on the current head SHA, and no unresolved threads. **High-risk** PRs (billing,
invoices, payments, auth/identity, tenancy, cutoff/finalization, migrations, CI/agent config,
requirements/ADRs) require the product owner to add the `owner-approved` label. Agents must
never add `owner-approved` themselves. `.claude/scripts/pr-risk.sh` classifies risk.

## Definition of done

Requested behavior implemented; approved rules respected; validation, authz and tenant
isolation enforced; audit where required; safe migrations; unit + integration tests (and E2E
where appropriate) added and passing; build green; docs updated. Final report lists: summary,
files changed, DB/migration changes, API changes, tests run and results, known limitations.

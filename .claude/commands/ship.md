---
description: End-to-end delivery of a task — plan, implement, local review, PR, PR review, merge gate.
argument-hint: <issue number or task description>
---

Deliver this task through the Caterly agent pipeline: $ARGUMENTS

Follow `docs/engineering/agent-workflow.md`. Track stages with a todo list.

Whenever any agent returns open business questions or decisions for the owner, stop and ask the
user; do not continue on assumptions.

1. **Triage** — decide which stages apply (see the routing table in
   `docs/engineering/agent-workflow.md`):
   - Needs design? New module/entity/schema, new or changed API, cross-module flow, integration,
     or anything touching tenancy, money, cutoff/finalization or audit → yes.
   - Needs UX? Any new screen, flow, or significant UI change → yes.
   - Which code areas? `backend/`, `web/`, or both. Small cross-stack fixes with no design or UX
     need may go to the generalist `developer` agent instead of the specialists.
2. **Design** (if needed) — run the `architect` agent. Show its design summary and owner
   decisions to the user and wait for approval before implementation. Proposed ADRs stay
   Proposed until the user accepts them.
3. **UX** (if needed) — run the `ui-ux-designer` agent with the task and the architect's design.
   Show open UX questions to the user.
4. **Backend** (if needed) — run the `backend-developer` agent with the task, design and branch.
   It creates the branch and the draft PR.
5. **Frontend** (if needed) — run the `frontend-developer` agent on the **same branch/PR**, with
   the UX spec and the API contract.
6. **Local review** — run the `code-reviewer` agent on the branch. Send findings to the agent
   that owns the affected files (backend/frontend/developer). Max 3 rounds; if blockers remain,
   stop and report.
7. **Ready** — `gh pr ready <n>`.
8. **PR review** — run the `pr-reviewer` agent. If `CHANGES_REQUESTED`, route findings to the
   owning agent, then re-run `pr-reviewer` (max 3 rounds).
9. **CI** — `gh pr checks <n> --watch --required`. On failure, give the logs
   (`gh run view --log-failed`) to the owning agent to fix, then go back to step 8.

Large features: follow the architect's implementation slices — one PR per slice, each through
steps 4–10, in order.
10. **Merge** — run the `pr-merger` agent. If it stops for owner approval, report that clearly.

Finish with: PR link, final state (merged / waiting for owner / blocked), risk level, tests run,
and any open questions.

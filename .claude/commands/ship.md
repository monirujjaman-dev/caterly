---
description: End-to-end delivery of a task — plan, implement, local review, PR, PR review, merge gate.
argument-hint: <issue number or task description>
---

Deliver this task through the Caterly agent pipeline: $ARGUMENTS

Follow `docs/engineering/agent-workflow.md`. Track stages with a todo list.

1. **Develop** — run the `developer` agent with the task. If it returns open business questions,
   stop and ask the user; do not continue on assumptions. If it returns a plan for approval
   (architectural/cross-module change), show it to the user and wait.
2. **Local review** — run the `code-reviewer` agent on the branch. If it requests changes, send the
   findings back to the `developer` agent. Repeat at most 3 rounds; if blockers remain, stop and
   report.
3. **Ready** — `gh pr ready <n>`.
4. **PR review** — run the `pr-reviewer` agent on the PR. If `CHANGES_REQUESTED`, send findings to
   the `developer` agent, then re-run `pr-reviewer` (max 3 rounds).
5. **CI** — `gh pr checks <n> --watch --required`. On failure, give the logs
   (`gh run view --log-failed`) to the `developer` agent to fix, then go back to step 4.
6. **Merge** — run the `pr-merger` agent. If it stops for owner approval, report that clearly.

Finish with: PR link, final state (merged / waiting for owner / blocked), risk level, tests run,
and any open questions.

---
description: Address review comments and CI failures on an existing PR (developer agent).
argument-hint: <pr number>
---

For pull request #$ARGUMENTS:

1. Collect open review threads, the latest `AGENT-REVIEW:` comment, and failing checks
   (`gh pr checks`, `gh run view --log-failed`).
2. Run the `developer` agent on the PR branch with those findings.
3. Run the `pr-reviewer` agent again on the new head.
4. Report what changed and the new verdict.

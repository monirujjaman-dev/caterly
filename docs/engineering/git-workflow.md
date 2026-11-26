# Git workflow

Policy: [ADR-0002](../adr/0002-git-workflow-and-agent-merge-policy.md).

## Branches

| Prefix | Use |
|---|---|
| `feat/<scope>-<slug>` | New behavior |
| `fix/<scope>-<slug>` | Bug fix |
| `refactor/…`, `test/…`, `docs/…`, `chore/…`, `ci/…`, `perf/…` | As named |

- Start from fresh `main`: `git fetch origin && git switch -c feat/selection-cutoff origin/main`.
- Include the issue number when there is one: `feat/123-selection-cutoff`.
- One concern per branch. Aim for PRs under ~400 changed lines; split larger work into a stack of
  PRs that each leave `main` releasable.

## Commits — Conventional Commits

```
<type>(<scope>): <imperative summary, ≤ 72 chars>

<why, not what; reference rule IDs, e.g. CMD-032>

Refs #123
```

Types: `feat fix refactor perf test docs build ci chore revert`. Breaking change: `feat!:` plus a
`BREAKING CHANGE:` footer. Scopes follow modules: `tenancy identity customers corporate agreements
employees menus selection orders kitchen delivery billing payments audit web api db ci agents`.

Never commit secrets, `.env` files, local appsettings, generated output, or unrelated changes.

## Keeping current

```
git fetch origin
git rebase origin/main
git push --force-with-lease   # only on your own feature branch
```

Prefer rebase over merge commits on feature branches. Resolve conflicts by understanding both
sides; never discard upstream changes to make a rebase pass.

## Pull requests

- Title = Conventional Commit (becomes the squash commit message on `main`).
- Body follows `.github/pull_request_template.md`: summary, rule IDs, DB/API changes, tests
  run with results, risks, screenshots for UI.
- Open as **draft** while work is in progress; mark ready when the local code review passes.
- Every review comment gets a fix commit or a reply; resolve threads after addressing them.
- Merge: `gh pr merge <n> --squash --delete-branch` (by the `pr-merger` agent or the owner).

## Forbidden

`git push` to `main`, `git push --force` (without lease), `--no-verify`, `gh pr merge --admin`,
rewriting published `main` history, deleting `main`, disabling or skipping CI checks.

## Releases

Tag `main` with SemVer (`v0.1.0`) when a release is cut; release notes are generated from the
Conventional Commit history. Release automation is not configured yet.

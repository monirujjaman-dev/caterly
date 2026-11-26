#!/usr/bin/env bash
# Classify a Caterly PR (or the local branch diff) as low or high risk.
# Usage: pr-risk.sh <pr-number>      (uses gh)
#        pr-risk.sh --local [base]   (diff against origin/main or [base])
# Prints "low" or "high" on the first line, then one reason per line. Exit 0 always.
# Policy: docs/adr/0002-git-workflow-and-agent-merge-policy.md
set -euo pipefail

MAX_LINES=600

if [[ "${1:-}" == "--local" ]]; then
  base="${2:-origin/main}"
  files=$(git diff --name-only "$base"...HEAD)
  lines=$(git diff --numstat "$base"...HEAD | awk '{a+=$1; d+=$2} END {print a+d+0}')
elif [[ -n "${1:-}" ]]; then
  files=$(gh pr view "$1" --json files --jq '.files[].path')
  lines=$(gh pr view "$1" --json additions,deletions --jq '.additions + .deletions')
else
  echo "usage: $0 <pr-number> | --local [base]" >&2
  exit 2
fi

reasons=()
while IFS= read -r f; do
  [[ -z "$f" ]] && continue
  p=$(printf '%s' "$f" | tr '[:upper:]' '[:lower:]')
  case "$p" in
    *migrations/*|*.sql)                                   reasons+=("db-migration: $f") ;;
    *billing*|*invoice*|*payment*|*refund*|*pricing*|*allowance*|*contribution*|*receivable*|*ledger*)
                                                           reasons+=("financial: $f") ;;
    *identity*|*auth*|*permission*|*role*|*security*|*jwt*|*accesstoken*|*refreshtoken*|*tokenservice*|*password*)
                                                           reasons+=("auth/security: $f") ;;
    *tenan*)                                               reasons+=("tenancy: $f") ;;
    *cutoff*|*finaliz*|*snapshot*)                         reasons+=("cutoff/finalization: $f") ;;
    .github/*|.claude/*|claude.md|codeowners)              reasons+=("ci/agent-config: $f") ;;
    docs/requirements/*|docs/adr/*)                        reasons+=("requirements/adr: $f") ;;
    *.csproj|*directory.packages.props|*directory.build.props|*package.json|*package-lock.json|*global.json|*nuget.config)
                                                           reasons+=("dependencies/build: $f") ;;
    *appsettings*.json|*.env*|*dockerfile*|*docker-compose*|infra/*|deploy/*)
                                                           reasons+=("config/deploy: $f") ;;
  esac
done <<< "$files"

if (( lines > MAX_LINES )); then
  reasons+=("size: $lines changed lines > $MAX_LINES")
fi

if (( ${#reasons[@]} > 0 )); then
  echo "high"
  printf '%s\n' "${reasons[@]}"
else
  echo "low"
fi

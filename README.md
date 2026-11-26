# Caterly

Multi-tenant catering and food business management SaaS (corporate meal management + regular
customer ordering), initial market Bangladesh.

- Requirements: [`docs/requirements/`](docs/requirements/)
- Architecture decisions: [`docs/adr/`](docs/adr/)
- Git workflow: [`docs/engineering/git-workflow.md`](docs/engineering/git-workflow.md)
- Agent workflow (developer → code review → PR review → merge): [`docs/engineering/agent-workflow.md`](docs/engineering/agent-workflow.md)
- Agent instructions: [`CLAUDE.md`](CLAUDE.md)
- Build, test and run commands: [`docs/engineering/commands.md`](docs/engineering/commands.md)

## Getting started

Prerequisites: .NET SDK 10.0.x, Node.js `^22.22.2 || ^24.15.0 || >=26.0.0` (see `web/.nvmrc`).

```
cd backend && dotnet restore && dotnet test        # API: dotnet run --project src/Caterly.Api
cd web && npm ci && npm test -- --run              # dev server: npm run dev
```

## Working with Claude Code

```
/ship <issue # or task>   # plan → implement → review → PR → PR review → merge gate
/review-pr <n>            # publish an agent review on a PR
/fix-pr <n>               # address review comments / CI failures
/merge-pr <n>             # merge when the gate passes
```

High-risk PRs (billing, payments, auth, tenancy, cutoff/finalization, migrations, CI/agent
config, requirements) wait for the owner to add the `owner-approved` label.

Stack: .NET 10 · ASP.NET Core · EF Core · PostgreSQL · React + TypeScript.

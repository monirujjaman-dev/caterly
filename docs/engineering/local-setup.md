# Local development setup

## Prerequisites

| Tool | Version | Notes |
|---|---|---|
| .NET SDK | 10.0.100+ | Pinned by `backend/global.json` |
| Node.js | 22.22.2+ (see `web/.nvmrc`) | `nvm use` in `web/` |
| Docker + Compose v2 | recent | Local PostgreSQL and Testcontainers integration tests |
| GitHub CLI (`gh`) | recent | Used by the agents and PR workflow |
| VS Code (optional) | — | Recommended extensions in `.vscode/extensions.json` |

## First run

```bash
cp .env.example .env            # edit POSTGRES_PASSWORD; .env is git-ignored
docker compose up -d --wait     # PostgreSQL 17 on 127.0.0.1:5432 (UTC)

cd backend && dotnet restore && dotnet build && dotnet test
dotnet run --project src/Caterly.Api        # http://localhost:5080/health

cd ../web && npm ci && npm run dev          # Vite dev server
```

Backend connection string: the API will read `ConnectionStrings__Caterly` once persistence is
added. Prefer per-developer user secrets over `.env` for the backend:

```bash
cd backend/src/Caterly.Api
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:Caterly" "Host=localhost;Port=5432;Database=caterly;Username=caterly;Password=<your password>"
```

## Database

- Start/stop: `docker compose up -d` / `docker compose stop`.
- Reset (deletes all local data): `docker compose down -v`.
- Shell: `docker compose exec postgres psql -U caterly -d caterly`.
- Migrations: see `docs/engineering/commands.md`. Agents only ever apply migrations to this local database.

## Seed data (planned)

There is no schema yet. When the first persistence feature lands, the backend adds a
**development-only** seed command (enabled only when `ASPNETCORE_ENVIRONMENT=Development`) that
creates fictional demo data: one catering tenant, one corporate customer with two locations, an
agreement for each payment model, a few employees, and a week of published menus. Seed data
must be fictional, idempotent, and never run in other environments.

## Secrets

Never commit `.env`, user secrets, or real credentials. `.env.example` holds local placeholders
only. Agents are denied read access to `.env` files (`.claude/settings.json`).

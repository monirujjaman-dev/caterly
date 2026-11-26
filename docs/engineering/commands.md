# Command catalog

Keep this file in sync with the projects; CI (`.github/workflows/ci.yml`) runs the same commands.

## Backend — working dir `backend/`

Solution `Caterly.slnx` (SDK pinned by `global.json` to 10.0.x, `rollForward: latestFeature`).
Package versions are centralized in `Directory.Packages.props`; shared build settings
(nullable, implicit usings, warnings as errors) in `Directory.Build.props`.

| Project | Purpose |
|---|---|
| `src/Caterly.Domain` | Domain model (no framework dependencies) |
| `src/Caterly.Application` | Use cases; references Domain |
| `src/Caterly.Infrastructure` | Persistence and providers; references Application |
| `src/Caterly.Api` | ASP.NET Core host; references Application + Infrastructure |
| `tests/Caterly.Domain.UnitTests` | xUnit v3 unit tests |
| `tests/Caterly.Api.IntegrationTests` | xUnit v3 + `WebApplicationFactory` API tests |

| Purpose | Command |
|---|---|
| Restore | `dotnet restore` |
| Build (warnings as errors) | `dotnet build --no-restore -warnaserror -c Release` |
| Format check | `dotnet format --verify-no-changes --no-restore` |
| Unit + integration tests | `dotnet test --no-build -c Release` |
| Run the API locally | `dotnet run --project src/Caterly.Api` (http://localhost:5080, `GET /health`, OpenAPI at `/openapi/v1.json` in Development) |
| Add migration | `dotnet ef migrations add <Name> -p src/Caterly.Infrastructure -s src/Caterly.Api` |
| Generate migration SQL for review | `dotnet ef migrations script --idempotent -p src/Caterly.Infrastructure -s src/Caterly.Api` |

There is no database yet. EF Core + Npgsql, the first migration and Testcontainers-based
PostgreSQL integration tests (which need Docker) are added with the first feature that needs a
schema. Agents never run `dotnet ef database update` against a non-local database.

## Web — working dir `web/`

Vite + React + TypeScript (strict). Node `^22.22.2 || ^24.15.0 || >=26.0.0` (minimum pinned in `web/.nvmrc`).

| Purpose | Command |
|---|---|
| Install | `npm ci` |
| Dev server | `npm run dev` |
| Lint | `npm run lint` |
| Type check | `npm run typecheck` |
| Unit tests | `npm test -- --run` (Vitest + Testing Library) |
| Build | `npm run build` |
| E2E | `npm run e2e` (Playwright; run `npx playwright install` once; not run in CI yet) |

CI calls the `lint`, `typecheck`, `test` and `build` scripts by these exact names.

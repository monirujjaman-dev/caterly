# Command catalog

Scaffolding has not happened yet; update this file in the PR that creates each project.

## Backend — working dir `backend/`

| Purpose | Command |
|---|---|
| Restore | `dotnet restore` |
| Build (warnings as errors) | `dotnet build --no-restore -warnaserror` |
| Format check | `dotnet format --verify-no-changes` |
| Unit + integration tests | `dotnet test --no-build` (integration tests need Docker for Testcontainers) |
| Add migration | `dotnet ef migrations add <Name> -p src/Caterly.Infrastructure -s src/Caterly.Api` |
| Generate migration SQL for review | `dotnet ef migrations script --idempotent -p src/Caterly.Infrastructure -s src/Caterly.Api` |

Agents never run `dotnet ef database update` against a non-local database.

## Web — working dir `web/`

| Purpose | Command |
|---|---|
| Install | `npm ci` |
| Lint | `npm run lint` |
| Type check | `npm run typecheck` |
| Unit tests | `npm test -- --run` |
| Build | `npm run build` |
| E2E | `npm run e2e` (Playwright) |

The scaffolding PR must define these `package.json` scripts with exactly these names; CI calls them.

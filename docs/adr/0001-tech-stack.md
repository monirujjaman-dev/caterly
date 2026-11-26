# ADR-0001: Technology stack

- **Status:** Accepted (core stack) — product owner, 2026-11-26
- **Scope:** Whole repository

## Context

The requirements need a secure, multi-tenant, transaction-safe SaaS with strong financial
correctness, responsive web portals for several roles, and Bangladesh defaults (BDT, Asia/Dhaka).
The repository started empty.

## Decision (accepted)

| Concern | Choice |
|---|---|
| Backend | .NET 10, ASP.NET Core Web API, Clean Architecture (Domain / Application / Infrastructure / Api) |
| Persistence | PostgreSQL, EF Core with Npgsql, EF Core migrations |
| Frontend | React + TypeScript, single responsive web app with role-based areas |
| Mobile | Responsive web only for MVP (CMD-010, CMD-011) |

## Defaults (proposed — use unless an ADR changes them)

These are the defaults agents use when scaffolding. Change one through a new ADR.

- Modular monolith: one deployable API, modules by bounded context (Tenancy, Identity,
  Customers, CorporateAccounts, Agreements, Employees, Menus, MealSelection, Orders, Kitchen,
  Delivery, Billing, Payments, Audit). Modules talk through Application interfaces, not other
  modules' tables.
- Tenant isolation: every tenant-owned entity carries `TenantId`. EF Core global query filters
  are applied, plus explicit tenant checks in command handlers. Integration tests prove isolation.
- Money as `decimal` mapped to `numeric(18,2)` (rates `numeric(18,4)`), currency code stored. Rounding
  is explicit and centralized in one rounding policy; the exact mode is **pending finance
  confirmation** and must be decided before the first invoicing feature.
- Time: store `timestamptz` UTC; business dates as `DateOnly` with a service timezone (default
  `Asia/Dhaka`); inject `TimeProvider` (no `DateTime.Now` in domain code).
- Tests: xUnit with built-in asserts; Testcontainers for PostgreSQL
  integration tests; Vitest + Testing Library for web; Playwright for E2E.
- Web tooling: Vite, ESLint, TypeScript strict mode.
- API: REST, ProblemDetails errors, OpenAPI, cursor or offset pagination.

## Consequences

- One CI pipeline with backend and web jobs (`.github/workflows/ci.yml`).
- No microservices, message brokers, event sourcing or CQRS frameworks without a new ADR.
- Provider integrations (payment, SMS, email, payroll) stay behind interfaces until chosen.

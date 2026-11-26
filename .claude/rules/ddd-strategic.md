---
paths:
  - "backend/**"
  - "docs/architecture/**"
  - "docs/adr/**"
---

# Domain-driven design — strategic rules

## Ubiquitous language
- Use the terms in `docs/architecture/ubiquitous-language.md` exactly in code, tests, API
  names, UI copy and docs: `MealSelection`, `ServiceDate`, `MealPeriod`, `Cutoff`,
  `FinalizedSnapshot`, `SelectionOrigin`, `ServiceAgreement`.
- Do not use synonyms. A meal selection is not an "order" or a "booking". A corporate account
  is not a "client". A catering provider is not a "vendor".
- A new domain concept needs a glossary entry backed by a finalized rule ID or a `CLAUDE.md`
  non-negotiable rule **before** it appears in code. If no rule defines it, stop and ask the product owner. Do not coin business terms.

## Bounded contexts = modules
Modules follow ADR-0001: Tenancy, Identity, Customers, CorporateAccounts, Agreements,
Employees, Menus, MealSelection, Orders, Kitchen, Delivery, Billing, Payments, Audit.
- Each module owns its aggregates and tables. No module reads or writes another module's
  tables or references another module's entity types.
- Cross-module needs go through an Application interface owned by the **consuming** module
  (e.g. MealSelection defines `IEmployeeEligibilityReader`; the Employees module implements
  it). Pass IDs and small read models across boundaries, never aggregates.
- Snapshot what you need at the boundary. For example, a finalized meal records the price and
  allowance in effect at that time (CMD-009). It does not look them up again later.
- A new module, or moving a concept between modules, needs an `architect` design.

## Layout per module
```
Caterly.Domain/<Module>/            aggregates, entities, value objects, domain errors
Caterly.Domain/Common/              shared kernel only: Money, TenantId, Result, DomainError,
                                    base types. Keep it small; adding to it needs a reason.
Caterly.Application/<Module>/<UseCase>/   command/query, handler, validator, DTOs
Caterly.Application/<Module>/Abstractions/  repository and cross-module ports
Caterly.Infrastructure/<Module>/    EF configurations, repositories, adapters
Caterly.Api/<Module>/               endpoint mapping only
tests/Caterly.<Layer>.<Kind>Tests/<Module>/   mirrors the source layout
```

## Forbidden without an approved ADR
MediatR or any CQRS/mediator library, message brokers, event sourcing, microservices, generic
repository frameworks, AutoMapper-style mappers. Use plain handler classes and explicit mapping.

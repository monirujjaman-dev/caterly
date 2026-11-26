---
paths:
  - "backend/src/Caterly.Api/**"
  - "backend/tests/Caterly.Api.IntegrationTests/**"
---

# API layer

- Endpoints are thin. Each one binds the request, calls one handler, and maps the `Result` to
  HTTP. Endpoints contain no business rules, no `DbContext` and no repositories.
- Group endpoints per module (`<Module>/<Module>Endpoints.cs`, `MapGroup("/api/<module>")`),
  with authorization policies on every group or endpoint. Anonymous endpoints are explicit.
- Map `DomainError` to ProblemDetails in one central place. This is the default unless the
  architect's contract says otherwise: validation → 400 with field errors, not found or out of
  scope → 404, duplicate or concurrency → 409, business-rule violation → 422. Put the
  `<module>.<rule>` code in an `errorCode` extension.
- OpenAPI metadata (names, response types) on every endpoint. Paginate collections.
- Integration tests (`WebApplicationFactory`, written first for each endpoint) prove: the
  happy path, validation errors, unauthenticated → 401, wrong role → 403, **another tenant's or
  out-of-scope resource → 404**, idempotent replay, and the ProblemDetails shape.

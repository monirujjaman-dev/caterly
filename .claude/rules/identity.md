---
paths:
  - "backend/**"
  - "web/src/**"
---

# Identity and access: OAuth 2.0 / OIDC with OpenIddict

Design: `docs/architecture/system-architecture.md` §7, ADR-0004 D5–D6. Any change here is
**high risk** and needs an approved design.

- **Authorization server:** OpenIddict, with ASP.NET Core Identity and EF Core stores in the
  `identity` schema.
  - Allowed flows: authorization code + PKCE (interactive), refresh token (rotating, with
    reuse detection), client credentials (machine clients).
  - Never enable implicit or password (ROPC) grants.
- **Clients and secrets:**
  - Client registrations are seeded from configuration, and secrets come from configuration
    or secret stores, never source code.
  - Signing and encryption keys are certificates from configuration. Development certificates
    are allowed only in Development.
- **Tokens:**
  - Short-lived access tokens (≤ 10 min). Validate issuer, audience, lifetime and signature
    with OpenIddict validation.
  - Never log or persist raw tokens outside the OpenIddict stores.
- **Claims are minimal:** `sub`, tenant membership and role. Do not put permitted locations,
  corporate scope or permissions in tokens. Resolve them server-side per request through
  `ICurrentUser` (cached briefly; see `caching.md`).
- **Authorization in the API:**
  - Every endpoint uses a named policy (`Policies.*`), with anonymous access explicit.
  - The handler checks resource + tenant + scope; out-of-scope resources return 404.
  - Platform-admin cross-tenant access is explicit and audited.
- **Accounts:** individual accounts only (CMD-020). Lockout after failed attempts, and rate
  limiting on `/connect/token` and sign-in. MFA and OTP depend on an owner decision about the
  SMS vendor; do not assume a vendor.
- **Audit:** sign-in success and failure, lockout, password reset, role and permission
  change, client change, token revocation.
- **Web (SPA):**
  - Follow the chosen D6 pattern. With a BFF, use the HttpOnly + Secure + SameSite=Strict
    session cookie plus antiforgery, and keep no tokens in JavaScript.
  - With PKCE, keep tokens in memory only — never in `localStorage` or `sessionStorage`.
  - The UI hides actions by role for usability only; the server decides.
- **Tests (written first):**
  - 401 without a token or cookie, 403 for the wrong role, 404 across tenants and scopes;
  - expired or tampered token rejected;
  - refresh token reuse revokes the token family;
  - lockout after N failures;
  - audit entries written.

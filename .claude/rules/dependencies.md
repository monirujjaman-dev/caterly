# Dependencies: free and open-source only

Product-owner constraint (ADR-0003): **no paid tools, services or packages.**

- Before adding any NuGet or npm package, container image, or tool, check its **current
  license on the exact version** you add, using the package page, the LICENSE file, and the
  release notes of the major version. Allowed licenses: MIT, Apache 2.0, BSD, MPL 2.0, ISC,
  PostgreSQL, LGPL (as a dependency only), and AGPL/GPL only for self-hosted servers we run
  unmodified (Grafana, Loki, Tempo, Redis). Anything else (dual commercial, "source
  available", BSL/SSPL/RSAL, free-tier limits, "free for non-commercial use") → stop and ask.
- **Known paid or restricted — do not add:**
  - MassTransit v9+, NServiceBus;
  - MediatR v13+, AutoMapper v15+, FluentAssertions v8+;
  - Duende IdentityServer;
  - Hangfire Pro;
  - Seq (beyond its free tier);
  - Application Insights, Datadog, New Relic;
  - Elastic stack.
  - A free older major version of a now-commercial library is not allowed either; it
    stops receiving fixes.
- Prefer what is already in the inventory (`docs/architecture/system-architecture.md` §11),
  then a Microsoft or BCL feature, then a new package. A new framework-level package (anything
  that shapes the architecture) needs an ADR.
- Versions are pinned: NuGet in `backend/Directory.Packages.props`, npm through the exact
  versions in `package.json` plus the lockfile, and container images by version tag (never
  `latest`).
- In the PR, state each new dependency's name, version, license, and why it is needed.

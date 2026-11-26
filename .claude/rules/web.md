---
paths:
  - "web/**"
---

# Web — TDD and domain language

The full rules are in `.claude/agents/frontend-developer.md`. These additions apply to all
web work:

- Test-first with Vitest + Testing Library. Write the failing test for the user-visible
  behavior (query by role, label or text), then build the component.
- For every data view, write tests for the loading, empty, error (including a ProblemDetails
  `errorCode` such as `selection.cutoff_passed`) and success states before the view is
  considered done.
- Mock the network at the API-client boundary with typed fixtures that match the API
  contract. Do not mock child components.
- Use the ubiquitous language (`docs/architecture/ubiquitous-language.md`) in component, type
  and prop names and in user-facing copy.
- The UI never re-implements domain rules (cutoff, eligibility, pricing, contributions). It
  renders the server's decision. Tests prove that it does.
- Run a single test while iterating: `npx vitest run <path> -t "<name>"`. Run the full
  `npm test -- --run` before committing.

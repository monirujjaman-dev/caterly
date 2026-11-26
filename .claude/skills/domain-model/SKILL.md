---
name: domain-model
description: Model a Caterly business capability with DDD before coding — extract the ubiquitous language from rule IDs, place it in a bounded context, choose aggregates by their invariants, define value objects, state machines and domain error codes, and produce the TDD test list. Use when starting a feature that adds or changes domain behavior, when asked to "model this", "design the aggregate", "which aggregate owns this", or before handing a task to the architect or backend-developer.
---

# Domain modeling for Caterly

Output a short **model sketch** (in the plan, PR or `docs/architecture/<area>.md`) and a
**test list**. If the task adds a module, an aggregate, schema or an API surface, the sketch
is input to the `architect` agent, which owns the final design. Do not implement a new
aggregate's persistence or API without that design.

## 1. Language
- Collect every rule ID the task touches and quote the rule text.
- Underline the nouns (candidate entities and value objects) and verbs (candidate domain
  methods and events). Check each against `docs/architecture/ubiquitous-language.md`.
- A term that is missing or ambiguous is a question for the product owner. It is not a
  naming choice for you.

## 2. Bounded context
Put the behavior in one ADR-0001 module. Ask these questions:
- Who owns the data that changes? That module owns the behavior.
- What does it need from other modules? Define a port in the consuming module with the
  minimal read model, for example `EligibilityView(employeeId, serviceId, locationId,
  mealPeriodId, validFrom, validTo)`.
- What must be snapshotted at this boundary so history doesn't change (CMD-009, CMD-042)?

## 3. Aggregates — choose by invariant
For each rule, write it as an invariant and ask: *must this hold immediately, in the same
transaction, or can it be eventually consistent?*

| Invariant kind | Where it lives |
|---|---|
| Within one object's lifecycle (cannot change after cutoff, override needs a reason) | Inside the aggregate method |
| Across instances of one type (one active selection per employee/date/period) | Unique constraint + domain check, or a parent aggregate that owns the set — decide in design |
| Across many actors (menu item capacity, CMD-054) | Concurrency-protected counter or reservation, or "flag the excess" — **business choice: ask** |
| Across modules (eligibility, publication, CMD-029/030) | Application handler checks via ports before calling the domain |

Keep aggregates small. Reference other aggregates by ID. If two invariants keep pulling
objects into one large aggregate, write down the trade-off and raise it with the architect.

## 4. Value objects
Replace primitives: IDs, `Money`, `ServiceDate`, `EffectivePeriod`, `CutoffRule`,
`SelectionOrigin`, quantities by kind (selected, finalized, prepared, delivered and billed are
**different types or fields**, never one reused number).

## 5. Lifecycle
Draw each stateful aggregate as a state table: state, command, who may do it, guard (rule ID),
next state, audit required? Every row becomes at least one test. Commands that no row allows
must return a domain error.

## 6. Domain errors
List every rejection as `<module>.<rule>` with the rule ID and the intended HTTP mapping, for
example `selection.cutoff_passed` (CMD-038, 422) or `selection.duplicate_active` (CMD-028, 409).

## 7. Test list
Turn sections 3–6 into the TDD test list (`.claude/skills/tdd/SKILL.md` §1), ordered by layer:
domain first, then application (ports, authorization, tenancy, audit, idempotency), then API and
persistence.

## Sketch template
```markdown
### <Capability> — model sketch
- Rules: CMD-…
- Module: <Module>; ports needed: <IXxxReader → owned by Module Y>
- Aggregate: <Name> (id, tenantId, …) — invariants: …
- Value objects: …
- States: | State | Command | Actor | Guard | Next | Audit |
- Domain errors: | Code | Rule | HTTP |
- Snapshots: …
- Open questions for the owner: 1. … (options, recommendation)
- Test list: …
```

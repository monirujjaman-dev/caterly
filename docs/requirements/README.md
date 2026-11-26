# Requirements

| Document | Status | Authority |
|---|---|---|
| [Caterly_Corporate_Meal_Business_Decisions_v1.0.md](Caterly_Corporate_Meal_Business_Decisions_v1.0.md) | Finalized baseline | Authoritative for corporate meals (rule IDs `CMD-001`…`CMD-056`) |
| `Caterly_Customer_Onboarding_Order_Billing_Flows_v1.0.md` | Draft — business workflow review | Workflow reference; its Open items (`FLOW-001`…`FLOW-018`) are **not decided** |
| `Caterly_SRS_Draft_v0.3.md` | Draft — business requirement review | Requirement IDs (`PLT-`, `COR-`, `SEL-`, `FIN-`, `BIL-`, …) are stable but mostly **Proposed** |

> The two draft documents live in the Caterly claude.ai Project. Add them to this folder
> (same file names) so agents running in the repo and in GitHub Actions can read them.
> Until then, agents must ask the product owner instead of inferring draft content.

## Rules for agents

- Cite rule IDs (`CMD-028`, `SEL-010`) in plans, tests and PR descriptions.
- Where the finalized decisions supersede a draft (e.g. SRS `OQ-001`…`OQ-004` and §7.8 are
  resolved by `CMD-001`…`CMD-004`), follow the finalized decision.
- Changing a requirement means a new document version or an ADR — never silent edits.
  Any PR touching this folder is high-risk and needs `owner-approved`.

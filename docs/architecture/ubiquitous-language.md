# Ubiquitous language

Status: living glossary. Every term comes from a finalized rule (`CMD-…`) or from
`CLAUDE.md`. Terms are used exactly as written here in code (PascalCase), APIs (camelCase), UI
copy and docs. Adding or changing a term needs a rule reference. Terms from draft specs stay
out until those specs are approved.

| Term (code name) | Module | Meaning | Source |
|---|---|---|---|
| Tenant (`Tenant`) | Tenancy | A catering or food business using Caterly. All its data is isolated. | CLAUDE.md |
| Catering provider | Tenancy | The tenant in its role of serving corporate customers. Not "vendor". | CMD-022 |
| Corporate account (`CorporateAccount`) | CorporateAccounts | A corporate customer of a tenant. Not "client". | CMD-005 |
| Location (`Location`) | CorporateAccounts | An office or branch of a corporate account. | CMD-005 |
| Department (`Department`) | CorporateAccounts | An optional grouping of employees. | CMD-017 |
| Service agreement (`ServiceAgreement`) | Agreements | A contract between a tenant and a corporate account with a distinct scope or term. Effective-dated. | CMD-006, CMD-009 |
| Service (`CorporateService`) | Agreements | The configured service under an agreement: schedule, locations, menus, pricing, cutoff, payment model and no-selection policy. | CMD-007, CMD-008 |
| Service calendar / non-service day | Agreements | The operating days and the holidays or closed dates of a service. | CMD-051, CMD-052 |
| Suspension (`ServiceSuspension`) | Agreements | A time-boxed pause of a service or location, with a reason. | CMD-053 |
| Corporate administrator | Identity / CorporateAccounts | A user who manages one corporate account within permitted locations. | CMD-012, CMD-014 |
| Employee (`Employee`) | Employees | A corporate account member with an individual login and a corporate employee identifier. | CMD-017, CMD-020 |
| Employment status | Employees | Active, inactive or suspended, effective-dated. Inactive or suspended employees cannot make new selections. | CMD-019 |
| Eligibility (`Eligibility`) | Employees | The right to select meals, limited by location, service, meal period and effective dates. | CMD-018 |
| Meal period (`MealPeriod`) | Menus | Breakfast, lunch, snacks, dinner and so on, defined per service. | CMD-026 |
| Service date (`ServiceDate`) | Menus / MealSelection | A business date in the service timezone (default Asia/Dhaka). | CMD-037 |
| Menu / menu publication | Menus | The items a provider publishes for a service and location, for a date or a date range. | CMD-022, CMD-023 |
| Availability / capacity | Menus | Quantity control per published item. | CMD-024, CMD-054 |
| Meal selection (`MealSelection`) | MealSelection | An employee's chosen item for a service date + meal period. Not "order" or "booking". | CMD-027, CMD-028 |
| Selection origin (`SelectionOrigin`) | MealSelection | `Employee`, `Administrator` or `Automatic` (policy-assigned). | CMD-015, §3.7 |
| Cutoff (`Cutoff`) | MealSelection | The instant after which normal employee changes stop. Configurable per service, date and meal period. | CMD-036, CMD-038 |
| Override (`SelectionOverride`) | MealSelection | An authorized post-cutoff change with actor, timestamp, reason, previous and new value. | CMD-032, CMD-033 |
| No-selection policy (`NoSelectionPolicy`) | Agreements | `NoMeal` (default), `DefaultMeal` or `AdministratorAssignment`. | §3.7 |
| Finalization / finalized snapshot (`FinalizedSnapshot`) | MealSelection | An immutable record of the selections and policy-assigned meals for a service date + meal period. | CMD-040, CMD-042 |
| Adjustment (`Adjustment`) | MealSelection / Billing | An auditable change after finalization. It never overwrites the snapshot. | CMD-041, CMD-042 |
| Payment model (`PaymentModel`) | Agreements | `CompanyFunded`, `EmployeeFunded` or `SharedAllowance`, set per agreement. | §3.8 |
| Gross meal value | Billing | The meal price before contributions. | §3.8 |
| Company contribution / allowance | Billing | The company's share. Can be a fixed amount, a percentage or another agreed rule. | §3.8 |
| Employee contribution | Billing | The employee's share. The collection channel is **open**. | §3.8 |
| Billable amount | Billing | What each party (company, employee) is billed. Stored separately per party. | §3.8 |
| Billing basis | Agreements / Billing | The quantity that is billed. Default: finalized quantity. | CMD-044, CMD-045 |
| Quantities | Kitchen / Delivery / Billing | Selected, finalized, planned, prepared, dispatched, delivered and billed are **distinct** values. | CMD-046, CLAUDE.md |
| Invoice (`Invoice`) | Billing | A corporate bill for a billing period. Immutable once issued. | CMD-048 |
| Dispute (`InvoiceDispute`) | Billing | The disputed amount, reason, owner and resolution history. | CMD-050 |

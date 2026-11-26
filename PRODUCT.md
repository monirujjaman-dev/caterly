# Product

<!-- impeccable:product-schema 1 -->

## Platform

web

Responsive web for desktop, tablet and mobile browsers (CMD-003, CMD-010). Installable as a PWA
(`web/public/manifest.webmanifest`). Native Android/iOS apps are not part of the initial release
(CMD-011), and no native-app roadmap has been decided.

## Users

**Primary: the catering operator.** This is the paying tenant: a catering or food business in
Bangladesh that runs corporate meal services and regular customer ordering. Operators use the
product all day on desktop to publish menus, watch selections approach cutoff, review consolidated
quantities, record authorized adjustments before production, finalize, and bill. When the needs of
different roles conflict, the design favors the operator's accuracy and speed.

Other confirmed roles, each with a scoped portal:

- **Corporate admin**: at the client company. Manages authorized locations, departments,
  employees, eligibility and selections on behalf of employees, only within their assigned account
  and locations (CMD-012, CMD-014).
- **Employee**: picks a meal from a calendar, usually on a phone, before cutoff. Can change or
  cancel the selection until cutoff and view their own history (CMD-013, CMD-021).
- **Regular customer**: guest or registered. Places immediate, scheduled, pre-order or event
  orders. Its flows are still draft (see Capabilities and Constraints).
- **Kitchen and dispatch staff**: work from finalized quantities. Selected, finalized, planned,
  prepared, dispatched, delivered and billed quantities are separate numbers.
- **Finance**: company and employee billing, invoices, payments, refunds.
- **Platform admin**: runs the multi-tenant platform.

## Product Purpose

Caterly is a multi-tenant SaaS that lets a catering business run corporate meal programs and
regular customer orders in one place. A corporate program covers the agreement, eligibility, the
meal calendar, selection, cutoff, the finalized snapshot, production quantities, and split company
and employee billing. Customer ordering runs on the same platform.

Success looks like this: every service date and meal period closes on time with a finalized
quantity the kitchen can trust, every rupee is attributable to the right party under the terms in
force on that day, and every change after cutoff has an actor and a reason.

## Positioning

Two claims, held equally:

1. **Corporate meal operations are built in.** The domain model has effective-dated agreements,
   eligibility by location, service, meal period and date, configurable cutoff, an immutable
   finalization snapshot, a configurable no-selection policy, and three payment models
   (company-funded, employee-funded, shared allowance) with gross, company and employee amounts
   tracked separately. Generic food-ordering and POS tools do not model this.
2. **Bangladesh comes first.** BDT money formatting, `Asia/Dhaka` as the default service timezone,
   a Bangla-ready interface (no text in images, Bengali font fallbacks, externalized strings), and
   mobile web as the main channel for employees.

## Operating Context

- **Deadlines drive the day.** Cutoff is set per service, date and meal period. At cutoff, employee
  changes stop automatically, the no-selection policy runs, and finalization produces a snapshot.
  Times appear as absolute local times ("Order by 10:00 AM, Thu 27 Nov").
- **Money is shown, not calculated, by the UI.** Prices, shares, allowances and eligibility come
  from the server. Issued invoices and completed payments are never overwritten. Corrections are
  credit, debit, refund or reversal records.
- **Order status and payment status are independent.** They are always shown as two separate
  badges.
- **Audit is part of the job.** Overrides after cutoff need a reason. The origin of each selection
  (employee, administrator, automatic) is recorded and shown.
- **Devices and density differ by role.** Operators and finance work at desktop density. Employees
  and customers work on phones, often on mobile networks.

## Capabilities and Constraints

- Corporate meal rules are final in `docs/requirements/Caterly_Corporate_Meal_Business_Decisions_v1.0.md`
  (CMD-001 to CMD-056).
- Regular customer ordering, onboarding and billing flows are **draft**: Flows v1.0 (FLOW-001 to
  FLOW-018 open) and SRS v0.3 (mostly Proposed). Do not design those flows as settled behavior.
- **Not decided. Do not assume or depict:** the payment gateway, the channel for collecting
  employee contributions, payroll integration, the SMS/email vendor, tax/VAT treatment, the
  native-app roadmap, the icon library (needs an ADR), and when Bangla localization ships (OQ-013).
- Dark mode is out of scope for the MVP.
- Stack: React + TypeScript (Vite) web, .NET 10 API, PostgreSQL (`docs/adr/0001-tech-stack.md`).
- Terminology: "employee", "corporate admin", "catering team", "service date", "meal period",
  "cutoff", "finalized".

## Brand Commitments

- **Name:** Caterly.
- **Mark:** a white serving cloche on a rounded green tile with a saffron knob
  (`web/public/brand/caterly-mark.svg`). The logo and the inverse logo pair the mark with a live-text
  Inter wordmark (`caterly-logo.svg`, `caterly-logo-inverse.svg`). Keep clear space of ¼ of the
  mark's width around it. Minimum size is 24 px.
- **Brand colors are fixed by the assets:** green `#166534` (the tile, the theme color and the
  primary action) and saffron `#F2A93B` (the knob, the accent).
- **Voice: calm and dependable.** It should sound like a reliable kitchen manager: precise and
  quiet, and trustworthy with money and deadlines. Plain English, sentence case, short sentences.
  Buttons name the action ("Save selection"). Errors say what happened and what to do next.
  Warmth comes from the food and the saffron accent, not from chatty copy.

## Evidence on Hand

- Brand assets: `web/public/brand/`, `web/public/favicon.svg`, and the PWA icons in `web/public/`.
- The design system v0.1 lives in `docs/ux/design-system.md`, with tokens in
  `web/src/styles/tokens.css` and `web/src/styles/base.css`.
- There are **no** customers, testimonials, case studies, usage metrics, pricing or
  partner logos yet. Never invent them in any surface.

## Product Principles

1. **The operator's numbers must be right.** Accurate quantities and money come before
   convenience. The UI shows what the server decided and never shows its own estimate as final.
2. **Deadlines are explicit.** Every selectable thing shows its cutoff as an absolute local time
   and shows clearly when it is locked.
3. **Every change has an owner.** Show who made a change and why, especially overrides and
   adjustments after cutoff.
4. **Status is never color alone.** Each status has text, plus an icon where there is space. Order
   and payment status stay separate.
5. **Same system, different density.** Employees get a mobile-first view and operators a
   desktop-efficient one, built from the same components.

## Accessibility & Inclusion

- WCAG 2.2 AA. Semantic tokens have their contrast checked (see `docs/ux/design-system.md`).
- Touch targets are at least 44 px. Visible labels, never placeholder-only. Errors are linked with
  `aria-describedby`. Async results are announced via `aria-live`.
- Respect `prefers-reduced-motion`.
- Support Bangla: flexible widths, Bengali font fallbacks, and no text baked into images.

# Caterly design system

Status: **v0.1** — foundation for the MVP. Owned by the `ui-ux-designer` agent; implemented in
`web/src/styles/tokens.css` (tokens) and `web/src/styles/base.css` (base styles). Change both in
the same PR.

## Principles

1. **Clear before clever** — operators, kitchen staff and employees must understand a screen at a
   glance, often on a phone.
2. **Status is never color alone** — every status has text, and an icon where space allows.
3. **The server is the authority** — the UI shows prices, shares, eligibility and cutoff that the
   API returns; it never presents its own calculation as final.
4. **Mobile-first for employees, desktop-efficient for operators** — same components, different
   density (CMD-010).
5. **Ready for Bangla** — no text baked into images, flexible widths, system fonts with Bengali
   fallbacks.

## Brand

| Asset | Path | Use |
|---|---|---|
| Mark | `web/public/brand/caterly-mark.svg` | App icon, favicon, compact header |
| Logo | `web/public/brand/caterly-logo.svg` | Light backgrounds |
| Logo inverse | `web/public/brand/caterly-logo-inverse.svg` | Dark/green backgrounds |
| Favicon | `web/public/favicon.svg` | Browser tab |
| PWA icons | `web/public/icon-192.png`, `icon-512.png`, `icon-maskable-512.png`, `apple-touch-icon.png` | Install/home screen |

The mark is a serving cloche on a green tile with a saffron knob. Keep clear space of ¼ of the
mark's width around it; minimum size 24 px. The wordmark uses live text (Inter / system sans);
convert it to outlines before using it in print or outside the web app.

## Color

Use **semantic tokens** in components. Contrast ratios are against white unless stated.

| Token | Value | Use | Contrast |
|---|---|---|---|
| `--color-text` | `#111827` | Body text | 17.7:1 |
| `--color-text-muted` | `#4B5563` | Secondary text | 7.6:1 |
| `--color-text-subtle` | `#6B7280` | Hints, meta | 4.8:1 |
| `--color-action` | `#166534` | Primary buttons, links | white on it 7.1:1 |
| `--color-action-hover` | `#14532D` | Hover/pressed | 9.1:1 |
| `--color-accent` | `#F2A93B` | Brand highlight, decorative only | dark text on it 7.4:1 |
| `--color-accent-text` | `#B45309` | Accent-colored text | 5.0:1 |
| `--color-border-input` | `#6B7280` | Input and control borders | 4.8:1 (≥3:1) |
| `--color-border` | `#E5E7EB` | Dividers only (decorative) | — |
| `--color-focus` | `#1E40AF` | Focus ring (3 px) | 8.6:1 |

Status pairs (text on background):

| Status | Text / background | Contrast |
|---|---|---|
| Success | `#166534` / `#DCFCE7` | 6.5:1 |
| Warning | `#92400E` / `#FEF3C7` | 6.4:1 |
| Danger | `#B91C1C` / `#FEE2E2` | 5.3:1 |
| Info | `#1E40AF` / `#DBEAFE` | 7.2:1 |
| Neutral | `#374151` / `#F3F4F6` | 9.4:1 |

Dark mode is not in scope for the MVP.

## Typography

System font stack with Bengali fallbacks (`--font-sans`); no web-font download.

| Token | Size | Use |
|---|---|---|
| `--text-3xl` | 30 px | Page title (desktop) |
| `--text-2xl` | 24 px | Page title (mobile), section title |
| `--text-xl` | 20 px | Card title |
| `--text-lg` | 18 px | Emphasis, key amounts |
| `--text-md` | 16 px | Body (minimum for form inputs, prevents iOS zoom) |
| `--text-sm` | 14 px | Table cells, meta |
| `--text-xs` | 12 px | Badges only |

Weights: 400 body, 500 labels, 600 headings, 700 brand/key numbers. Line height 1.5 body, 1.25 headings.
Use tabular numerals (`font-variant-numeric: tabular-nums`) for amounts and quantities.

## Spacing, shape, elevation

- Spacing on a 4 px scale: `--space-1` (4) … `--space-12` (48).
- Radius: `sm` 4 (badges), `md` 8 (inputs, buttons), `lg` 12 (cards, dialogs), `full` (pills).
- Shadows: `sm` for cards, `md` for menus/dialogs. Prefer borders over shadows in dense tables.

## Layout and breakpoints

| Name | Min width | Layout |
|---|---|---|
| mobile | 0 | Single column, bottom-anchored primary action, 16 px gutters |
| tablet | 768 px | Two columns where useful, side sheet for details |
| desktop | 1024 px | Left navigation + content, 24 px gutters |
| wide | 1280 px | Content max width 1280 px (`--container-max`) |

Touch targets ≥ 44 px (`--target-min`). Header height 56 px.

## Core components (MVP)

| Component | Notes |
|---|---|
| Button | Variants: primary, secondary (outlined), tertiary (text), danger. Shows a spinner and is disabled while submitting. |
| Text field / select / date | Visible label (never placeholder-only), hint, inline error linked with `aria-describedby`. |
| Form error summary | At the top of long forms; lists server validation errors with links to fields. |
| Status badge | Text + icon + status colors; never color alone. |
| Data table | Sort, filter, pagination; becomes a stacked card list below 768 px. |
| Card | Grouped summary content. |
| Dialog | Confirmations; focus trapped; destructive or financial actions name the consequence ("Cancel 12 selections"). |
| Toast / inline alert | Async results announced via `aria-live="polite"`; errors use `role="alert"`. |
| Empty state | Short explanation and the next action. |
| Skeleton | Loading placeholders for lists and calendars. |
| Meal calendar | Week view on mobile, month on desktop; see states below. |
| Money | Formats BDT with `Intl.NumberFormat('en-BD', { style: 'currency', currency: 'BDT' })`; shows company and employee shares separately where relevant. |
| Date/time | Formats in the service timezone (default `Asia/Dhaka`); cutoff shown as an absolute time ("Order by 10:00 AM, Thu 27 Nov"). |

## Meal calendar states

| State | Label | Icon | Colors |
|---|---|---|---|
| Available | "Available" | plate | neutral |
| Selected | "Selected: <meal>" | check | success |
| Changed by admin | "Selected by admin" | user-check | info |
| Auto-assigned | "Assigned automatically" | sparkle | info |
| Sold out | "Sold out" | slash | warning |
| Past cutoff (locked) | "Locked — cutoff passed" | lock | neutral |
| Closed / holiday | "No service" | calendar-x | neutral |
| Not eligible | "Not included in your plan" | info | neutral |

## Order, payment and invoice statuses

Order status and payment status are shown as **two separate badges**; never merge them. Labels
follow the lifecycles in the requirements; exception states (Cancelled, Failed, Disputed,
Refund pending) use the danger or warning pairs.

## Icons

Use one outline icon set at 20 px (24 px touch areas). Decorative icons get `aria-hidden="true"`;
meaningful icons next to text are decorative because the text carries the meaning.
The icon library is not chosen yet; choosing it is a frontend dependency decision (ADR).

## Content style

Plain English, sentence case, short sentences. Name the action on buttons ("Save selection",
not "Submit"). Errors say what happened and what to do. Use "employee", "corporate admin",
"catering team" consistently. All user-facing strings live in one module per feature so Bangla
can be added (OQ-013).

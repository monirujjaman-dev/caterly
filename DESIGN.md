---
name: Caterly
description: Catering and corporate meal management, built around cutoff, finalization and money you can trust.
colors:
  pass-green: "#166534"
  pass-green-deep: "#14532d"
  pass-green-wash: "#f0fdf4"
  pass-green-tint: "#dcfce7"
  saffron-knob: "#f2a93b"
  saffron-wash: "#fdf0dc"
  burnt-saffron: "#b45309"
  plate-white: "#ffffff"
  counter-mist: "#f9fafb"
  steel-tint: "#f3f4f6"
  divider-gray: "#e5e7eb"
  control-gray: "#6b7280"
  muted-ink: "#4b5563"
  slate-ink: "#374151"
  ink: "#111827"
  focus-blue: "#1e40af"
  info-tint: "#dbeafe"
  warning-ink: "#92400e"
  warning-tint: "#fef3c7"
  danger-red: "#b91c1c"
  danger-tint: "#fee2e2"
typography:
  page-title:
    fontFamily: "Inter, 'Segoe UI', Roboto, 'Noto Sans', 'Noto Sans Bengali', 'Hind Siliguri', system-ui, -apple-system, sans-serif"
    fontSize: "1.875rem"
    fontWeight: 600
    lineHeight: 1.25
  section-title:
    fontFamily: "Inter, 'Segoe UI', Roboto, 'Noto Sans', 'Noto Sans Bengali', 'Hind Siliguri', system-ui, -apple-system, sans-serif"
    fontSize: "1.5rem"
    fontWeight: 600
    lineHeight: 1.25
  card-title:
    fontFamily: "Inter, 'Segoe UI', Roboto, 'Noto Sans', 'Noto Sans Bengali', 'Hind Siliguri', system-ui, -apple-system, sans-serif"
    fontSize: "1.25rem"
    fontWeight: 600
    lineHeight: 1.25
  amount:
    fontFamily: "Inter, 'Segoe UI', Roboto, 'Noto Sans', 'Noto Sans Bengali', 'Hind Siliguri', system-ui, -apple-system, sans-serif"
    fontSize: "1.125rem"
    fontWeight: 700
    lineHeight: 1.25
    fontFeature: "tnum"
  body:
    fontFamily: "Inter, 'Segoe UI', Roboto, 'Noto Sans', 'Noto Sans Bengali', 'Hind Siliguri', system-ui, -apple-system, sans-serif"
    fontSize: "1rem"
    fontWeight: 400
    lineHeight: 1.5
  label:
    fontFamily: "Inter, 'Segoe UI', Roboto, 'Noto Sans', 'Noto Sans Bengali', 'Hind Siliguri', system-ui, -apple-system, sans-serif"
    fontSize: "1rem"
    fontWeight: 500
    lineHeight: 1.5
  meta:
    fontFamily: "Inter, 'Segoe UI', Roboto, 'Noto Sans', 'Noto Sans Bengali', 'Hind Siliguri', system-ui, -apple-system, sans-serif"
    fontSize: "0.875rem"
    fontWeight: 400
    lineHeight: 1.5
  badge:
    fontFamily: "Inter, 'Segoe UI', Roboto, 'Noto Sans', 'Noto Sans Bengali', 'Hind Siliguri', system-ui, -apple-system, sans-serif"
    fontSize: "0.75rem"
    fontWeight: 500
    lineHeight: 1.25
  mono:
    fontFamily: "ui-monospace, 'SFMono-Regular', Menlo, Consolas, monospace"
    fontSize: "0.875rem"
    fontWeight: 400
    lineHeight: 1.5
rounded:
  sm: "0.25rem"
  md: "0.5rem"
  lg: "0.75rem"
  full: "9999px"
spacing:
  "1": "0.25rem"
  "2": "0.5rem"
  "3": "0.75rem"
  "4": "1rem"
  "5": "1.25rem"
  "6": "1.5rem"
  "8": "2rem"
  "10": "2.5rem"
  "12": "3rem"
components:
  button-primary:
    backgroundColor: "{colors.pass-green}"
    textColor: "{colors.plate-white}"
    typography: "{typography.label}"
    rounded: "{rounded.md}"
    padding: "0.625rem 1rem"
    height: "2.75rem"
  button-primary-hover:
    backgroundColor: "{colors.pass-green-deep}"
    textColor: "{colors.plate-white}"
  button-secondary:
    backgroundColor: "{colors.plate-white}"
    textColor: "{colors.pass-green}"
    typography: "{typography.label}"
    rounded: "{rounded.md}"
    padding: "0.625rem 1rem"
    height: "2.75rem"
  button-secondary-hover:
    backgroundColor: "{colors.pass-green-wash}"
    textColor: "{colors.pass-green-deep}"
  button-danger:
    backgroundColor: "{colors.danger-red}"
    textColor: "{colors.plate-white}"
    typography: "{typography.label}"
    rounded: "{rounded.md}"
    padding: "0.625rem 1rem"
    height: "2.75rem"
  text-field:
    backgroundColor: "{colors.plate-white}"
    textColor: "{colors.ink}"
    typography: "{typography.body}"
    rounded: "{rounded.md}"
    padding: "0.625rem 0.75rem"
    height: "2.75rem"
  card:
    backgroundColor: "{colors.plate-white}"
    textColor: "{colors.ink}"
    rounded: "{rounded.lg}"
    padding: "1.5rem"
  badge-success:
    backgroundColor: "{colors.pass-green-tint}"
    textColor: "{colors.pass-green}"
    typography: "{typography.badge}"
    rounded: "{rounded.sm}"
    padding: "0.125rem 0.5rem"
  badge-warning:
    backgroundColor: "{colors.warning-tint}"
    textColor: "{colors.warning-ink}"
    typography: "{typography.badge}"
    rounded: "{rounded.sm}"
    padding: "0.125rem 0.5rem"
  badge-danger:
    backgroundColor: "{colors.danger-tint}"
    textColor: "{colors.danger-red}"
    typography: "{typography.badge}"
    rounded: "{rounded.sm}"
    padding: "0.125rem 0.5rem"
  badge-info:
    backgroundColor: "{colors.info-tint}"
    textColor: "{colors.focus-blue}"
    typography: "{typography.badge}"
    rounded: "{rounded.sm}"
    padding: "0.125rem 0.5rem"
  badge-neutral:
    backgroundColor: "{colors.steel-tint}"
    textColor: "{colors.slate-ink}"
    typography: "{typography.badge}"
    rounded: "{rounded.sm}"
    padding: "0.125rem 0.5rem"
---

# Design System: Caterly

> Implementation source of truth: `web/src/styles/tokens.css` and `web/src/styles/base.css`,
> specified in `docs/ux/design-system.md` (v0.1). This file records the same values, plus the
> intent behind them, for agents that design new screens. When a token changes, update all four
> in the same PR.

## Overview

**Creative North Star: "The Kitchen Pass"**

The pass is the counter between the kitchen and the floor. Every plate is checked there before it
leaves. Caterly's interface plays the same role for a catering business: selections, quantities
and money arrive from many hands and must be clear, ordered and final before they go out. Screens
read like a well-run pass. Each item sits in its place, its status is stated in words, and the
one thing that needs attention stands out because everything else is quiet.

The world is calm and dependable. Surfaces are plate white on a faint counter-mist ground, with
fine gray dividers and ink-dark text. **Pass Green**, the color of the brand tile, means
*confirmed, go, act here*: primary actions, links, success. **Saffron**, the knob on the cloche,
is used sparingly. It marks the brand and the moment that needs a human eye. It is never an
alarm, and it is never filled behind white text. Density follows the role: operators and finance
get compact tables and multi-column desktop layouts, while employees get one column on a phone
with the primary action anchored at the bottom.

Expression comes from precision, not ornament: tabular numbers that line up, absolute cutoff
times, status badges that always carry text, and a 3 px focus ring you cannot miss.

**Key Characteristics:**
- Quiet neutral surfaces, with green reserved for action and confirmation.
- Saffron as a rare brand and attention accent, never as a status or a button fill.
- A system font stack with Bengali fallbacks. No web-font download.
- Flat by default. Borders define structure, and shadows appear only on cards and floating layers.
- Soft corners: 8 px on controls, 12 px on containers.
- Status is always text plus color (plus an icon where there is space). Order and payment status
  are two badges.

## Colors

The palette is restrained: one brand green that carries every action, one saffron accent kept
rare, cool neutral grays, and four fixed status pairs. Every text pairing meets WCAG 2.2 AA.

### Primary
- **Pass Green** (#166534): the brand tile, the `theme-color`, primary buttons, links and
  success text. White on it reaches 7.1:1. This is the one color that means "do this" or "done".
- **Pass Green Deep** (#14532d): hover and pressed states of primary actions, and the wordmark on
  light backgrounds.
- **Pass Green Wash** (#f0fdf4): subtle action backgrounds, such as the secondary-button hover and
  a selected row or calendar cell.
- **Pass Green Tint** (#dcfce7): the background of the success badge.

### Secondary
- **Saffron Knob** (#f2a93b): the brand accent from the cloche knob. Decorative only, or as a
  surface behind **dark** text (ink on it reaches 7.4:1). Use it for small brand moments and
  attention markers, such as the dot on a "needs review" item.
- **Saffron Wash** (#fdf0dc): a soft highlight surface for an attention callout.
- **Burnt Saffron** (#b45309): saffron as readable text (5.0:1 on white).

### Neutral
- **Plate White** (#ffffff): page background, surfaces, cards, inputs, and text on green.
- **Counter Mist** (#f9fafb): muted surfaces, such as table headers, sidebars and the page ground
  behind cards.
- **Steel Tint** (#f3f4f6): the neutral badge background.
- **Divider Gray** (#e5e7eb): decorative dividers and card borders only. Never use it as the only
  boundary of a control.
- **Control Gray** (#6b7280): input and control borders (4.8:1, above the 3:1 UI minimum) and
  subtle hint text.
- **Muted Ink** (#4b5563): secondary text (7.6:1).
- **Slate Ink** (#374151): neutral badge text.
- **Ink** (#111827): body text and headings (17.7:1).

### Status
- **Focus Blue** (#1e40af): the 3 px focus ring, plus info text on **Info Tint** (#dbeafe).
- **Warning Ink** (#92400e) on **Warning Tint** (#fef3c7): warnings, such as sold out or
  approaching cutoff.
- **Danger Red** (#b91c1c) on **Danger Tint** (#fee2e2): errors, cancelled, failed, disputed.
- Success uses **Pass Green** on **Pass Green Tint**. Neutral uses **Slate Ink** on **Steel Tint**.

### Named Rules
**The One Go Color Rule.** Pass Green is the only color for primary actions and confirmation. If
two things on a screen are green-filled, one of them is wrong.

**The Saffron Is Seasoning Rule.** Saffron covers well under 5% of any screen. It is never a
button fill, never sits behind white text, and never means a status. Warning has its own amber
pair.

**The Semantic Token Rule.** Components use semantic tokens (`--color-action`,
`--color-text-muted`), never palette tokens (`--green-700`).

## Typography

**Body Font:** Inter when it is installed, otherwise Segoe UI, Roboto or Noto Sans. Bengali falls
back to Noto Sans Bengali or Hind Siliguri, then `system-ui`.
**Mono Font:** `ui-monospace`, SFMono-Regular, Menlo, Consolas. Use it for references and IDs only.

**Character:** one familiar sans for everything, so the interface feels native on every device
and renders Bangla correctly without a download. Hierarchy comes from size and weight, not from a
second typeface.

### Hierarchy
- **Page title** (600, 1.875rem / 30 px desktop, 1.5rem / 24 px mobile, 1.25): one per screen.
- **Section title** (600, 1.5rem / 24 px, 1.25): major groups on a page.
- **Card title** (600, 1.25rem / 20 px, 1.25): titles of cards and dialogs.
- **Amount** (700, 1.125rem / 18 px, tabular numerals): key money and quantity figures.
- **Body** (400, 1rem / 16 px, 1.5): running text and form inputs. 16 px is the minimum for inputs
  because it prevents zoom on iOS. Keep prose to about 70ch.
- **Label** (500, 1rem): field labels and button text.
- **Meta** (400, 0.875rem / 14 px): table cells, timestamps, hints.
- **Badge** (500, 0.75rem / 12 px): status badges only.

### Named Rules
**The Columns Line Up Rule.** Every amount, quantity and time uses
`font-variant-numeric: tabular-nums`. Right-align numeric table columns.

**The 12 px Is For Badges Rule.** `--text-xs` appears only inside badges. Nothing else readable
goes below 14 px.

## Layout

A 4 px spacing scale (`--space-1` 4 px to `--space-12` 48 px), mobile-first:

- **Mobile** (0 and up): single column, 16 px gutters, and the primary action anchored at the
  bottom. Data tables become stacked card lists.
- **Tablet** (768 px and up): two columns where useful, and details open in a side sheet.
- **Desktop** (1024 px and up): left navigation plus content, with 24 px gutters.
- **Wide** (1280 px and up): content capped at 1280 px (`--container-max`).

The header is 56 px tall. Touch targets are at least 44 px (`--target-min`). Operator screens may
pack more per row, but never shrink targets or type below the scale.

**The Absolute Time Rule.** Cutoffs and service windows always appear as absolute local times in
the service timezone ("Order by 10:00 AM, Thu 27 Nov"), never as only "in 2 hours".

## Elevation & Depth

Flat with a light lift. Structure comes from Divider Gray borders and the Counter Mist and Plate
White surfaces. Shadows are reserved for elements that sit above the page.

### Shadow Vocabulary
- **Card** (`box-shadow: 0 1px 2px rgb(17 24 39 / 0.06)`): a resting card on Counter Mist.
- **Floating** (`box-shadow: 0 4px 12px rgb(17 24 39 / 0.08)`): menus, popovers, dialogs and
  side sheets.

### Named Rules
**The Borders In Tables Rule.** Dense tables and lists use borders and row dividers, never
shadows. A shadow means "this floats", not "this is important".

## Shapes

Soft but not playful. The brand tile's rounded square sets the tone:

- **4 px** (`sm`): badges.
- **8 px** (`md`): buttons, inputs, selects, small tiles.
- **12 px** (`lg`): cards, dialogs, side sheets.
- **Full**: pills and avatars only.

Controls have a 1 px Control Gray border. Containers have a 1 px Divider Gray border. Icons are a
single outline set at 20 px inside 24 px touch areas. The library is not chosen yet and needs an
ADR.

## Components

These components are specified in `docs/ux/design-system.md` and are not built yet. Implement
them from the tokens here. Each one should feel precise and steady: no bounce, no flourish.

### Buttons
- **Shape:** gently rounded (8 px). At least 44 px tall.
- **Primary:** Pass Green fill with white label text (500), padding 10 × 16 px. Hover and pressed
  turn Pass Green Deep.
- **Secondary:** Plate White with a 1 px Pass Green border and Pass Green text. Hover turns Pass
  Green Wash.
- **Tertiary:** text only in Pass Green, underlined on hover.
- **Danger:** Danger Red fill with white text, for destructive or financial confirmations only.
- **Focus:** 3 px Focus Blue outline at a 2 px offset (global).
- **Submitting:** a spinner replaces the leading icon, and the button is disabled. The label names
  the action ("Save selection", "Finalize lunch").

### Status badges
- **Style:** 4 px radius, 12 px / 500 text, a status-pair background, and a leading icon where
  there is space. The label is always present.
- **Rule:** order status and payment status are two badges side by side and are never merged.

### Cards / Containers
- **Corner Style:** 12 px.
- **Background:** Plate White on Counter Mist.
- **Shadow Strategy:** the Card shadow, or a Divider Gray border alone in dense layouts.
- **Internal Padding:** 24 px desktop, 16 px mobile.

### Inputs / Fields
- **Style:** Plate White, 1 px Control Gray border, 8 px radius, 16 px text, 44 px tall.
- **Label:** visible above the field (Label style). Never placeholder-only. A hint below in Meta.
- **Focus:** the global 3 px Focus Blue ring.
- **Error:** a Danger Red border plus an inline message linked by `aria-describedby`. Long forms
  also show an error summary at the top.

### Navigation
- **Desktop:** a left navigation rail on Counter Mist. The active item has Pass Green text and a
  Pass Green Wash background.
- **Mobile:** a 56 px header with the mark (minimum 24 px). The primary task action sits at the
  bottom of the screen.

### Meal calendar (signature component)
Week view on mobile, month view on desktop. Each cell states its condition in words, using the
status pairs:

- **Available:** neutral, plate icon.
- **Selected:** success, check icon.
- **Selected by admin** and **Assigned automatically:** info, user-check and sparkle icons.
- **Sold out:** warning, slash icon.
- **Locked (cutoff passed)**, **No service** and **Not included in your plan:** neutral, with
  lock, calendar-x and info icons.

The cutoff time is printed on the day, not hidden in a tooltip.

### Money
BDT is formatted with `Intl.NumberFormat('en-BD', { style: 'currency', currency: 'BDT' })` in
the Amount style. Where shares apply, show the gross amount, the company share and the employee
share as separate labeled figures. Always show the values the server returns.

## Do's and Don'ts

### Do:
- **Do** use Pass Green (#166534) for the single primary action on a screen, and Pass Green Deep
  (#14532d) for its hover state.
- **Do** pair every status color with a text label, and add an icon where space allows.
- **Do** use tabular numerals for every amount, quantity and time.
- **Do** show cutoffs as absolute local times in Asia/Dhaka unless the service says otherwise.
- **Do** keep touch targets at least 44 px and input text at least 16 px.
- **Do** use the inverse logo on green or dark surfaces, keep ¼-width clear space around the mark,
  and never show it smaller than 24 px.
- **Do** name the consequence in destructive or financial dialogs ("Cancel 12 selections").

### Don't:
- **Don't** put white text on Saffron Knob (#f2a93b), and don't use saffron as a button fill or a
  status color.
- **Don't** use Divider Gray (#e5e7eb) as the only boundary of an input or control.
- **Don't** merge order status and payment status into one badge.
- **Don't** add a web font, or bake text into images. Both break Bangla support.
- **Don't** use shadows to add emphasis in tables and lists.
- **Don't** build dark-mode variants for the MVP.
- **Don't** show prices, shares or totals the client calculated as if they were final.

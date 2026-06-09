# Skill Improvements Summary

**Date**: 2026-05-20
**Retro files processed**:
- `2026-05-19-xrtable-layout-labels.md`
- `2026-05-19-summary-expression-function.md`
- `2026-05-19-xrtablecell-weight-vs-widthf.md`

---

## Changes Made

### New Patterns

- **Pattern 6 — Tabular column layout with XRTable**: Added to `SKILL.md` Common Patterns section. Shows `XRTable` / `XRTableRow` / `XRTableCell` with `WidthF` and `SizeF`, and an explicit "Do NOT use XRLabel at absolute positions" comment. Fills the dangling reference in Constraint 12 ("See Pattern 6") that was previously unresolved.
  Addresses: `2026-05-19-xrtable-layout-labels.md`

- **Pattern 7 — Group subtotal using summary expression**: Added to `SKILL.md` Common Patterns section. Shows `XRSummary` + `ExpressionBinding` with `sumSum([Field])`. Includes inline warning that `Sum([Field])` is not a valid DevExpress function and causes error XRE093. Lists other available functions (`sumAvg`, `sumCount`, `sumMax`, `sumMin`).
  Addresses: `2026-05-19-summary-expression-function.md`

### Troubleshooting Rows Added

- `Group footer label shows last value, or error XRE093 "no summary functions"` — cause: expression uses `Sum([Field])`; fix: use `sumSum([Field])` and set `label.Summary.Running`. See Pattern 7.
  Addresses: `2026-05-19-summary-expression-function.md`

### Clarifications

- **Quick Start section** (`SKILL.md`): Added a "Single-field display only" callout immediately after the Quick Start code block, directing agents to `references/report-controls.md` and `references/report-bands.md` before writing code for multi-column, grouped, or rich-control reports.
  Addresses: `2026-05-19-xrtable-layout-labels.md`

- **XRTableCell width guidance** (`references/report-controls.md`): Replaced misleading `"Use Weight on cells to control proportional widths"` with correct `WidthF` guidance. Updated the inline code example to assign `WidthF` on both cells and `SizeF` on the table, with a comment showing that the sum of `WidthF` values must equal `SizeF.Width`. Added explicit "Do not use `Weight` — internal use only" warning.
  Addresses: `2026-05-19-xrtablecell-weight-vs-widthf.md`

---

## Root Causes Fixed

1. **Navigation failure — agents read only SKILL.md Common Patterns** (retros 1 & 2 share this root cause): Both agents failed to consult reference files (`report-controls.md`, `expressions.md`) for the knowledge they needed. Generalized fix: added inline callout in Quick Start and added the two missing patterns (6 and 7) directly in Common Patterns with cross-references to the relevant reference files. Pattern presence in the main SKILL.md makes the knowledge discoverable without requiring a separate read step.

2. **Misleading API guidance for XRTableCell sizing** (retro 3): The `report-controls.md` file actively recommended `Weight` for cell sizing, which is an internal-use property. An agent following the documented guidance would produce incorrect code. Fixed by replacing the `Weight` recommendation with the correct `WidthF` pattern and a working code example.

---

## Validation Checklist

- [x] No new contradictions in CRITICAL constraints
- [x] Skill boundary (runtime API only, no viewer UI) is preserved
- [x] All three retro findings are addressed
- [x] Pattern 6 now resolves the dangling "See Pattern 6" reference in Constraint 12
- [x] Pattern 7 provides the exact `sumSum` syntax inline — no additional reference file read required to avoid the mistake
- [x] `report-controls.md` no longer recommends `Weight` — future agents will use `WidthF`
- [x] Version number not auto-bumped (metadata.version tracks DevExpress product versioning per repo convention)
- [x] A hypothetical agent reading the updated skill would: navigate to report-controls.md for complex layouts; use `sumSum([Field])` for group subtotals; use `WidthF` (not `Weight`) for XRTableCell widths

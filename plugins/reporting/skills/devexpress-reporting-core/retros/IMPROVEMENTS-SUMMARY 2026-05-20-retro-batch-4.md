# Skill Improvements Summary

**Date**: 2026-05-20
**Retro files processed**:
- `2025-07-16-xrsummary-vs-expression.md`
- `2025-07-16-format-expression-function.md`
- `2025-07-16-hardcoded-control-widths.md`
- `2025-07-16-xrtable-column-headers.md`

## Changes Made

### CRITICAL Warning Added to Pattern 2 (Expression Binding)
- Added callout block immediately after Pattern 2 code: "Expression function names are NOT .NET method names." Lists the common mistake (`Format()` → `FormatString(format, value)`) and directs to `references/expressions.md` and MCP for the full list. — Addresses `2025-07-16-format-expression-function.md`

### Pattern 6 Upgraded to CRITICAL (XRTable for all multi-column layouts)
- Added CRITICAL blockquote before the Pattern 6 code example, explicitly stating the rule applies to ALL bands: data rows in `DetailBand`, static column header rows in `GroupHeaderBand`/`PageHeaderBand`, and summary rows in `GroupFooterBand`. — Addresses `2025-07-16-xrtable-column-headers.md`
- Updated the comment inside the Pattern 6 code block to read "including static header rows and summary rows — NOT just data rows"

### Pattern 7 Expanded (XRSummary — both usage patterns + required note)
- Renamed from "Group subtotal using summary expression" to "Group or report summary using XRSummary"
- Added CRITICAL note: "Summary labels always require `XRSummary`." — Addresses `2025-07-16-xrsummary-vs-expression.md`
- Expanded to show Pattern A (XRSummary.Func + FormatString — simpler for standard aggregates) alongside the existing Pattern B (XRSummary.Running + ExpressionBinding with sum*())
- Updated attribution comment to reference both retros

### New Pattern 8 — Available width calculation
- Added Pattern 8 after Pattern 7 showing how to compute `availableWidth = PageWidthF - Margins.Left - Margins.Right` and use it for single-control bands and proportional multi-column tables. — Addresses `2025-07-16-hardcoded-control-widths.md`

### Troubleshooting Rows Added
- "Summary label displays expression literally or shows 0 / nothing" — XRSummary not set. — Addresses `2025-07-16-xrsummary-vs-expression.md`
- "Expression binding silently ignored or runtime error with unrecognized function" — function name guessed from C# knowledge. — Addresses `2025-07-16-format-expression-function.md`

### Constraint 12 Expanded
- Reworded to cover "Any multi-column layout" including static header rows and summary rows, not just data-bound rows. Added note that custom XRLabel x-offset helpers are also prohibited. — Addresses `2025-07-16-xrtable-column-headers.md`

### expressions.md — FormatString Added
- Added `FormatString(format, value)` examples to the String Functions section with a prominent note that `Format()`, `String.Format()`, `ToString()` do not exist in the DevExpress expression language. — Addresses `2025-07-16-format-expression-function.md`

## Root Causes Fixed

1. **Missing XRSummary pattern** — Pattern 7 only showed one variant (Running+ExpressionBinding) and had no CRITICAL gate preventing agents from skipping XRSummary entirely. Fixed by adding CRITICAL note + Pattern A variant.
2. **Expression function names guessed from C# knowledge** — Skill was silent on naming conventions for expression functions. Fixed by adding CRITICAL warning in Pattern 2 and `FormatString` to expressions.md.
3. **Hardcoded pixel widths across bands** — Quick Start used a single arbitrary width with no guidance on deriving it from page dimensions. Fixed by adding Pattern 8 with the `availableWidth` formula.
4. **XRTable advisory not strong enough for static header rows** — Constraint 12 existed but said "data-bound fields"; agents built custom label helpers for header rows, reasoning that headers are "static text" not "data-bound". Fixed by upgrading to CRITICAL and explicitly listing all band types.

## Validation Checklist

- [x] No new contradictions in CRITICAL constraints
- [x] Skill boundary (runtime API only, no viewer UI) is preserved
- [x] All retro findings are addressed or documented as intentionally not addressed
- [x] Pattern 7 (XRSummary) and expressions.md Summary Functions section are consistent
- [x] Pattern 8 (availableWidth) does not conflict with hardcoded widths in Pattern 6 example (Pattern 6 still shows 650/450/200 as a concrete example; Pattern 8 teaches the general principle)

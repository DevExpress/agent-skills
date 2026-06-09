# Skill Improvements Summary — Final Review

**Date**: 2026-05-21  
**Branch**: `reporting-core-improvements`  
**Reviewer scope**: Comprehensive cross-file audit comparing all branch changes vs `main`, all retro files, and all reference/example files for internal consistency.

---

## Findings and Changes

### 1. `examples/quickstart.cs` — Six distinct issues fixed

#### 1a. `XRTableCell.Weight` used instead of `WidthF`
**Problem**: All three tables (page header, detail) used `Weight = 3`, `Weight = 1`, etc. `Weight` is for internal use only (explicitly flagged in retro `2026-05-19-xrtablecell-weight-vs-widthf.md` and fixed in `report-controls.md` at batch 2, but the quickstart was not updated at that time).  
**Fix**: Replaced all `Weight = N` with `WidthF = availableWidth * fraction` using proportional column widths (60%/20%/20%).

#### 1b. Summary labels missing `XRSummary`
**Problem**: `subtotalLabel` and `grandTotalLabel` used `sumSum()` in `ExpressionBinding` without setting `.Summary`. This contradicts Pattern 7 (CRITICAL: "Summary labels always require `XRSummary`").  
**Fix**: Added `label.Summary = new XRSummary { Running = SummaryRunning.Group/Report }` to both labels before the `ExpressionBinding`.

#### 1c. Hardcoded widths `780` throughout
**Problem**: All control widths used magic number `780` regardless of the page/margin configuration. Contradicts Pattern 8 (use `availableWidth = PageWidthF - Margins.Left - Margins.Right`).  
**Fix**: Added `float availableWidth = PageWidthF - Margins.Left - Margins.Right` at the start of `BuildBands()` and replaced all `780` occurrences with `availableWidth` (or proportional fractions).

#### 1d. 2-argument `ExpressionBinding` inconsistency
**Problem**: Several places used `new ExpressionBinding("Text", "[Field]")` (2-arg) while the skill consistently teaches the 3-arg form `new ExpressionBinding("BeforePrint", "Text", "[Field]")`.  
**Fix**: Updated all `ExpressionBinding` calls to the canonical 3-arg form.

#### 1e. No `BeginInit/EndInit` on XRTable instances
**Problem**: `headerTable` and `dataTable` were populated (rows/cells added) without wrapping in `BeginInit/EndInit`. Pattern 6 and `report-controls.md` both require this pattern.  
**Fix**: Added `BeginInit()` before row/cell construction and `EndInit()` after `SizeF` is set on every `XRTable`.

#### 1f. `System.Drawing.Font` used instead of `DXFont`
**Problem**: `new Font("Arial", 16, FontStyle.Bold)` uses `System.Drawing.Font` which is Windows-only. `DevExpress.Drawing.DXFont` is the cross-platform equivalent and is shown in all reference files.  
**Fix**: Replaced all `new Font(...)` calls with `new DXFont(...)` / `DXFontStyle.*`. Added `DXFont, DXFontStyle` to the comment on the `using DevExpress.Drawing;` import.

---

### 2. `examples/quickstart.vb` — Four issues fixed

#### 2a. Two side-by-side `XRLabel` controls used in detail band
**Problem**: `nameLabel` at `X=0` and `priceLabel` at `X=360` — two labels at absolute positions simulating a table. This is the exact anti-pattern that Constraint 12 prohibits.  
**Fix**: Replaced both labels with an `XRTable` / `XRTableRow` / three `XRTableCell` layout using `WidthF` and `SizeF`.

#### 2b. Summary label missing `XRSummary`
Same as 1b. Fixed by adding `subtotalLabel.Summary = New XRSummary() With { .Running = SummaryRunning.Group }`.

#### 2c. 2-argument `ExpressionBinding`
Same as 1d. Fixed to 3-arg throughout.

#### 2d. `System.Drawing.Font` and hardcoded widths
Same as 1c/1f. Updated to `DXFont` and `availableWidth`.

---

### 3. `references/report-bands.md` — Three issues fixed

#### 3a. `BoundsF` set in object initializer (4 instances)
**Problem**: `catLabel`, `sumLabel`, `colHeader`, and `pageNum` all had `BoundsF` or layout properties set in their object initializers, before `Controls.Add()`. Constraint 14 (formerly 13) explicitly prohibits this: sizes set before parent assignment are silently recalculated.  
**Fix**: Moved all `BoundsF` assignments to after the `Controls.Add()` call with a comment `// set BoundsF AFTER Controls.Add (Constraint 14)`.

#### 3b. Group footer summary label missing `XRSummary`
**Problem**: The Grouping example showed `sumSum([Price])` in an `ExpressionBinding` on a plain `XRLabel` with no `.Summary` property set. Same root cause as issue 1b.  
**Fix**: Added `sumLabel.Summary = new XRSummary { Running = SummaryRunning.Group }` and the XRSummary note before the expression binding.

---

### 4. `SKILL.md` — Constraint 13 added, Troubleshooting row added

**Problem**: The `data-binding-expression.md` retro (2026-05-15) was present in the `reviewed/` folder but was never processed. It documented that the model used the legacy `DataBindings` API instead of `ExpressionBindings` because the skill had no prohibition.  
**Fix**:
- Added **Constraint 13** (`Never use DataBindings`) to the Constraints & Rules section.
- Added Troubleshooting row: symptom `DataBindings.Add("Text", null, "Field")` → fix references Constraint 13.
- Former Constraint 13 ("Set size/position properties AFTER adding to parent") renumbered to **Constraint 14**.

---

### 5. `references/data-binding.md` — CRITICAL callout added

Added a prominent `> CRITICAL` callout at the top of the file, immediately after the section title, warning that `DataBindings` is deprecated and `ExpressionBindings` is always required.

---

### 6. `references/report-bands.md` — Constraint reference numbers updated

All code comments in the file that referenced "Constraint 13" for the BoundsF-after-add rule were updated to "Constraint 14" to match the renumbering.

---

## Root Causes Fixed

1. **Example files not updated when reference files changed**: The `report-controls.md` fix for `Weight → WidthF` (batch 2) was applied correctly to the reference file but not backpropagated to `quickstart.cs`. Going forward, reference-file API changes should always trigger a corresponding review of the example files.

2. **Pattern 7 (XRSummary) not enforced in examples**: The Pattern 7 rule was added to the skill body but the quickstart was not updated to demonstrate it, making it possible for models to miss the `XRSummary` requirement by modeling the example.

3. **Unprocessed retro from 2026-05-15**: The `data-binding-expression.md` retro was placed in the `reviewed/` folder but never reflected in the skill. Retros in `reviewed/` should be systematically processed rather than archived without changes.

4. **Reference file consistency not checked after Constraint 14 addition**: When the "set BoundsF after Controls.Add" constraint was added (presumably manually), `report-bands.md` was not updated to fix its existing violations.

---

## Validation Checklist

- [x] `quickstart.cs`: no `Weight`, no raw `sumSum()` without XRSummary, all BoundsF after Controls.Add, BeginInit/EndInit present, DXFont used
- [x] `quickstart.vb`: XRTable in detail band, XRSummary on subtotal, 3-arg ExpressionBinding, DXFont, availableWidth
- [x] `report-bands.md`: no BoundsF in initializers, XRSummary present in grouping example
- [x] `SKILL.md`: DataBindings rule added as Constraint 13, constraint numbering consistent
- [x] `data-binding.md`: CRITICAL callout about DataBindings at top of file
- [x] No contradictions introduced with existing rules 1–12
- [x] Version number not auto-bumped

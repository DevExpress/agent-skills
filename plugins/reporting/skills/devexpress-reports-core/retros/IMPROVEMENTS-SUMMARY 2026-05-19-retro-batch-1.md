# Skill Improvements Summary

**Improved version**: 26.1 (version not bumped — metadata.version tracks DevExpress product versioning, not patch iterations)
**Date**: 2026-05-19
**Retro files processed**:
- `2026-05-19-duplicate-detailband.md`
- `2026-05-19-product-class-placement.md`
- `2026-05-19-wrong-model-folder.md`
- `2026-05-19-xlabel-instead-of-xrtable.md`

---

## Changes Made

### New CRITICAL Rules (Constraints & Rules section)

- **Rule 9 — Singleton bands in designer-backed reports**: Added warning that `DetailBand`, `TopMarginBand`, and `BottomMarginBand` already exist after `InitializeComponent()`. Calling `Bands.Add(new DetailBand())` throws `System.Exception: Incorrect band type`.  
  Addresses: `2026-05-19-duplicate-detailband.md`

- **Rule 10 — Project structure: designer file class ordering**: Added that the `XtraReport` subclass must be the first class in its `.cs` file; helper/model classes must go in a separate file.  
  Addresses: `2026-05-19-product-class-placement.md`

- **Rule 11 — Project structure: respect existing folders**: Added that the agent must inspect the existing project structure before creating folders, and match the existing naming convention exactly.  
  Addresses: `2026-05-19-wrong-model-folder.md`

- **Rule 12 — Tabular layout must use XRTable**: Added that two or more side-by-side data-bound fields must use `XRTable / XRTableRow / XRTableCell` — never multiple `XRLabel` controls at absolute positions.  
  Addresses: `2026-05-19-xlabel-instead-of-xrtable.md`

### Troubleshooting Rows Added

- `System.Exception: Incorrect band type` at report constructor — cause: second singleton band added after `InitializeComponent()`; fix: reuse existing designer band, see Constraint 9.  
  Addresses: `2026-05-19-duplicate-detailband.md`

### Clarifications (inline notes)

- **Quick Start section**: Added a callout block immediately after the code example warning that the pattern is for blank/code-only reports. In designer-backed partial classes, `InitializeComponent()` has already added the singleton bands — do not call `Bands.Add(new DetailBand())`.  
  Addresses: `2026-05-19-duplicate-detailband.md`

- **Core Classes table**: Added a `> CRITICAL — Tabular layout` blockquote directly after the table cross-referencing Pattern 6.  
  Addresses: `2026-05-19-xlabel-instead-of-xrtable.md`

### New Patterns

- **Pattern 6 — XRTable for tabular column layout**: Complete code example showing `XRTable / XRTableRow / XRTableCell` construction with `BeginInit/EndInit` and `ExpressionBindings`. Explicit `// Do NOT use` comment to prevent the label-as-columns anti-pattern.  
  Addresses: `2026-05-19-xlabel-instead-of-xrtable.md`

---

## Root Causes Fixed

1. **Designer-backed vs. blank-report confusion**: The skill's Quick Start only illustrated blank-report creation. Added an inline callout and Constraint 9 to make the designer-backed scenario explicit and impossible to miss.

2. **No project/file structure guidance**: Two separate retros (class ordering, folder naming) shared the same root cause — the skill gave no guidance on working inside an existing project. Generalized into two sub-rules (10 and 11) under the "Project structure" heading, covering both the VS Designer constraint and the folder-naming convention.

3. **Silent XRLabel vs. XRTable choice**: The Quick Start example uses `XRLabel`, which is correct for single-field display, but the skill never prohibited using it to simulate columns. Added a CRITICAL note at the Core Classes table, Constraint 12, and Pattern 6 to make the rule unavoidable.

---

## Validation Checklist

- [x] No new contradictions in CRITICAL constraints
- [x] Skill boundary (runtime API only, no viewer UI) is preserved
- [x] All retro findings are addressed
- [x] Version number not auto-bumped (metadata.version tracks DevExpress product versioning per repo convention)
- [x] A hypothetical agent reading the updated skill would: not add a second DetailBand; not place a model class before the report class; not create a duplicate folder; not use XRLabel for tabular layouts

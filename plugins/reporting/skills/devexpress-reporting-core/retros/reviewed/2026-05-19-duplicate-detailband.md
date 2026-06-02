# Retro — Adding a duplicate DetailBand to a report

## Mistake 4

### Task context
The user asked to fix the `XRLabel`-based detail band by replacing it with an `XRTable`. The fix threw `System.Exception: Incorrect band type` at runtime because a second `DetailBand` was being added to a report that already had one created by `InitializeComponent()`.

### What the skill said (or didn't say)
The skill's *Quick Start* example creates a fresh `XtraReport` from scratch and calls `Bands.Add(detail)` for a new `DetailBand`. The skill contains **no guidance** on how to build report content in code when `InitializeComponent()` has already added the mandatory singleton bands (`DetailBand`, `TopMarginBand`, `BottomMarginBand`), and no warning that `DetailBand` can only appear once.

### What you did wrong
I created `new DetailBand { ... }` and called `Bands.Add(detail)` inside `BuildReport()`, which is called from the constructor after `InitializeComponent()`. `InitializeComponent()` had already added `detailBand1` to the report, so adding a second `DetailBand` threw the exception.

### Why you made the mistake
The skill instruction was **missing entirely**. The *Quick Start* pattern assumes a blank `XtraReport()` with no pre-existing bands. There is no warning that in a designer-backed partial class the singleton bands are already present and must be reused, not re-created.

### What the correct behavior should have been
In a designer-backed report class (`InitializeComponent()` already ran), always **reuse** the existing singleton bands (`detailBand1`, `TopMargin`, `BottomMargin`) by clearing and reconfiguring them. Only call `Bands.Add(...)` for band types that are not yet present (e.g., `ReportHeaderBand`, `GroupHeaderBand`, `PageFooterBand`).

### Proposed skill fix
**New rule** — add as a CRITICAL constraint before the *Quick Start* section:

> **CRITICAL — Singleton bands in designer-backed reports**: `XtraReport` enforces exactly one `DetailBand`, one `ReportHeader`, one `ReportFooter`, one `TopMarginBand`, and one `BottomMarginBand`. When `BuildReport()` (or any setup method) is called after `InitializeComponent()`, these bands **already exist**. Calling `Bands.Add(new DetailBand())` will throw an exception at runtime.  
>
> **Rule**: In a designer-backed partial class, reuse the existing singleton bands (`DetailBand`, `ReportFooter`, `ReportHeader`, `TopMarginBand`, `BottomMarginBand`) declared in the designer file. Reconfigure their properties; do not create new instances and do not call `Bands.Add` for them.  
>
> Only call `Bands.Add(...)` for band types that may appear multiple times or that `InitializeComponent()` did not add (e.g., `ReportHeaderBand`, `GroupHeaderBand`, `GroupFooterBand`, `PageHeaderBand`, `PageFooterBand`).

**New troubleshooting row**:

| Symptom | Cause | Fix |
|---|---|---|
| `System.Exception: Incorrect band type` on report constructor | A second instance of a singleton band (`DetailBand`, `TopMarginBand`, or `BottomMarginBand`) was added via `Bands.Add()` after a similar band was already added. | Remove the `Bands.Add()` call. Instead, get the existing band from the designer field (e.g., `detailBand1`) and configure it directly. |

## Mistake 3

### Task context
The user asked to create an XtraReport in code with multiple bands and controls; the agent needed to assign `BoundsF` widths to all controls.

### What the skill said (or didn't say)
The skill is **completely silent** on how to calculate control widths. The Quick Start code example uses a hardcoded value (`BoundsF = new RectangleF(0, 0, 200, 25)`) without any note about deriving width from page dimensions. No mention of `PageWidthF`, `Margins`, or available width calculation anywhere in the skill body.

### What you did wrong
Assigned inconsistent hardcoded widths across controls in different bands: some controls had width `650`, others `600`, and the group footer label had `550`. None of the widths were derived from the page size and margins.

```csharp
// Wrong — inconsistent magic numbers across bands
titleLabel:    BoundsF = new RectangleF(0, 5, 650, 30)
groupLabel:    BoundsF = new RectangleF(0, 4, 650, 22)
colTable:      BoundsF = new RectangleF(0, 0, 600, 22)  // different!
detailTable:   BoundsF = new RectangleF(0, 0, 600, 22)
summaryLabel:  BoundsF = new RectangleF(0, 2, 550, 18)  // different again!
pageInfo:      BoundsF = new RectangleF(0, 0, 600, 23)
```

### Why you made the mistake
The skill was **missing** a design principle for computing control widths. The Quick Start example showed a single hardcoded width, which the agent imitated without questioning whether it would fill the band. There was no guidance to calculate `availableWidth` from page dimensions.

### What the correct behavior should have been
Calculate `availableWidth` once from `PageWidthF` and margins, then use it consistently for every single-control-per-band layout:

```csharp
// Correct
float availableWidth = PageWidthF - Margins.Left - Margins.Right;
// Then use availableWidth for every control width
titleLabel.BoundsF   = new RectangleF(0, 5, availableWidth, 30);
groupLabel.BoundsF   = new RectangleF(0, 4, availableWidth, 22);
colTable.BoundsF     = new RectangleF(0, 0, availableWidth, 22);
// etc.
```

For multi-column tables, column widths should be proportional fractions of `availableWidth`.

### Proposed skill fix
**New pattern** — Add Pattern 7 immediately after Pattern 1 (data-bound report) in the Common Patterns section:

```markdown
**Pattern 7 — Calculate available control width from page dimensions:**
```csharp
// Always calculate available width after setting Margins.
// Never hardcode pixel widths for controls that should span the full band.
Margins = new System.Drawing.Printing.Margins(50, 50, 50, 50);
float availableWidth = PageWidthF - Margins.Left - Margins.Right;

// Single control filling the band — always use availableWidth
label.BoundsF = new RectangleF(0, 0, availableWidth, bandHeight);

// Multi-column table — proportional fractions of availableWidth
colCell1.WidthF = availableWidth * 0.67f;
colCell2.WidthF = availableWidth * 0.33f;
```
> **Rule**: When a band contains a single control that should fill it horizontally,
> set `Width = availableWidth`. Never hardcode pixel values for layout-critical dimensions.
```

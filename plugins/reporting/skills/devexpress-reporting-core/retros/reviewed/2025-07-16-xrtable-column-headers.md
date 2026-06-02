## Mistake 2

### Task context
The user asked to create an XtraReport in code with column headers displayed side-by-side — "Country / Region" and "Area (km²)".

### What the skill said (or didn't say)
The skill **explicitly stated** in the Quick Start section:

> "For reports with multiple side-by-side columns, group subtotals, images, or charts — read `references/report-controls.md` (control types) and `references/report-bands.md` (band types) before writing code."

And in **Pattern 6**:

> "Use XRTable whenever two or more data-bound fields appear side-by-side as columns."

### What you did wrong
Created a custom helper method `AddColumnHeaderRow()` that manually positioned `XRLabel` controls side-by-side using calculated `x` offsets and fixed pixel widths:

```csharp
// Wrong — positions labels manually as a fake table
static void AddColumnHeaderRow(Band band, params (string Text, float Width)[] columns) {
	float x = 0;
	foreach (var (text, width) in columns) {
		var lbl = new XRLabel {
			BoundsF = new RectangleF(x, 2, width, 18),  // manual positioning
			...
		};
		band.Controls.Add(lbl);
		x += width;
	}
}
```

### Why you made the mistake
The skill instruction was **present but not strong enough**. The recommendation to use `XRTable` was written as an advisory note in the Quick Start rather than a hard constraint. Pattern 6 existed but the agent chose to build a custom workaround instead of following the pattern.

### What the correct behavior should have been
Use `XRTable` / `XRTableRow` / `XRTableCell` for any multi-column layout including static header rows:

```csharp
// Correct
var colTable = new XRTable { BoundsF = new RectangleF(0, 0, availableWidth, 22) };
var colRow = new XRTableRow();
var colCell1 = new XRTableCell { WidthF = availableWidth * 0.67f, Text = "Country / Region", ... };
var colCell2 = new XRTableCell { WidthF = availableWidth * 0.33f, Text = "Area (km²)", ... };
colRow.Cells.AddRange([colCell1, colCell2]);
colTable.Rows.AddRange([colRow]);
band.Controls.Add(colTable);
```

### Proposed skill fix
**New rule** — Upgrade the existing advisory note to a CRITICAL constraint directly before any multi-column example:

```markdown
> **CRITICAL — Never position XRLabel controls manually to simulate columns.**
> Any time two or more labels appear side-by-side (data rows, header rows, footer rows),
> use `XRTable` / `XRTableRow` / `XRTableCell`. This applies equally to:
> - Data rows in DetailBand
> - Static column header rows in GroupHeaderBand or PageHeaderBand
> - Summary rows in GroupFooterBand
```

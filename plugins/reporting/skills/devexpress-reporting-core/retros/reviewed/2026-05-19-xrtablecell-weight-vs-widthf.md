## Mistake 3

### Task context
The user asked to create an XtraReport in code with a table layout using `XRTableCell` controls.

### What the skill said (or didn't say)
The `references/report-controls.md` file mentions `XRTableCell` and states:

> Use `Weight` on cells to control proportional widths. `RowSpan` merges cells vertically.

However, this guidance is **ambiguous and incomplete**. It recommends `Weight` for proportional widths but does not mention `WidthF` (absolute pixels), does not clarify that `Weight` is for internal use only, and provides no code example showing how to actually set cell widths.

### What you did wrong
Following the skill's guidance, I used `XRTableCell.Weight` to set column widths, assigning fractional values like `0.6923076923076923` and `0.30769230769230768`. However, `Weight` is marked for internal use only and should not be set in code. The user had to correct this.

### Why you made the mistake
The skill's guidance on cell sizing is incomplete and misleading. It recommends `Weight` without clarifying its limitations or that `WidthF` is the correct public API for setting cell widths in code. With no working code example in the skill, I relied on the `Weight` property which is visible in IntelliSense but not part of the public API contract.

### What the correct behavior should have been
Use `XRTableCell.WidthF` to set absolute cell widths in pixels. The sum of all `WidthF` values in a row must equal the parent `XRTable.SizeF.Width`. Example:

```csharp
var nameCell = new XRTableCell { WidthF = 450 };
var priceCell = new XRTableCell { WidthF = 200 };
// Table SizeF.Width must equal 450 + 200 = 650
```

### Proposed skill fix
**Clarification** — replace the vague `Weight` guidance in the Table section of `report-controls.md` with clear guidance on `WidthF`:

```markdown
// Old:
Use `Weight` on cells to control proportional widths. `RowSpan` merges cells vertically.

// New:
Use `WidthF` on cells to set absolute column widths in pixels. The sum of all `WidthF` values in a row must equal the table's `SizeF.Width`. Do **not** use `Weight` — it is for internal use only. Use `RowSpan` to merge cells vertically.

Example:
```csharp
var nameCell = new XRTableCell { WidthF = 450 };  // 450 pixels
var priceCell = new XRTableCell { WidthF = 200 }; // 200 pixels
// Table SizeF = new SizeF(650, 25)  // sum of widths
```
```


## Mistake 1

### Task context
The user asked to create an XtraReport in code, bound to a sample list of products, grouped by category.

### What the skill said (or didn't say)
The skill's main `SKILL.md` Quick Start and Pattern examples use `XRLabel` for data display with no mention of `XRTable`. **However**, the `references/report-controls.md` file contains explicit guidance:

> **Tabular layout**: When displaying two or more fields side-by-side as columns inside a band, **always use `XRTable` / `XRTableRow` / `XRTableCell`**. Never simulate a table by placing multiple `XRLabel` controls at absolute X positions. Side-by-side labels produce misaligned columns and poor border rendering.

### What you did wrong
I used `XRLabel` controls with manually assigned `BoundsF` values to simulate a table layout across the group header, detail, and footer bands. The column positions were not synchronized — the label in the group footer used `BoundsF = new RectangleF(300, 0, 150, 25)` while the detail band used different offsets, producing misaligned columns. This also causes merged cell artifacts when exporting to Excel.

### Why you made the mistake
I did not read the `references/report-controls.md` file. I only consulted the main `SKILL.md` file and the Quick Start pattern, which predominantly uses `XRLabel`. The constraint on when to use tables vs. labels was already present in the skill but in a separate reference file that I failed to check. **This is a skill discovery / navigation issue, not a missing feature.**

### What the correct behavior should have been
Use `XRTable` / `XRTableRow` / `XRTableCell` with consistent `WidthF` values across all bands (group header, detail, footer). This ensures columns are pixel-perfectly aligned and export correctly to Excel without merged cells.

### Proposed skill fix
**Clarification** — add a callout/link in the main `SKILL.md` file to improve skill discovery. Add this note to the Core Classes Overview or Quick Start section:

```markdown
> **See also**: `references/report-controls.md` for all report control types.
```


In the Quick Start — Report in Code section, add a note that this example is relevant for a simple report with a single label and the following: 
```markdown
When a more complex report lauout is required, you need to use other report bands and controls. See: `references/report-controls.md` for all report control types and `report-bands.md` for report bands.
```
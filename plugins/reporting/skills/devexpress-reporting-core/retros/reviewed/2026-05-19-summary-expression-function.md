## Mistake 2

### Task context
The user asked to create an XtraReport in code, grouped by category, with a per-group subtotal in the group footer.

### What the skill said (or didn't say)
The skill's main `SKILL.md` shows `XRSummary` configuration but does not provide an expression binding example for summary controls. **However**, the `references/expressions.md` file contains a complete **Summary Functions** section with an explicit example:

```csharp
var totalLabel = new XRLabel { BoundsF = new RectangleF(200, 0, 100, 20) };
totalLabel.Summary = summary;
totalLabel.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "sumSum([Price])"));
groupFooter.Controls.Add(totalLabel);
```

This example clearly shows that summary expressions must use `sumSum([Field])` (and lists other functions like `sumAvg`, `sumCount`, etc.).

### What you did wrong
I set `ExpressionBinding("BeforePrint", "Text", "Sum([Price])")` — using `Sum()` which is not a valid DevExpress reporting expression function. This caused the control to show the last value in the group instead of the total, and the report designer raised error XRE093: *"Summary Running for the control is set to the 'Group' value, but its expression has no summary functions."*

### Why you made the mistake
I did not read the `references/expressions.md` file. I only consulted the main `SKILL.md` file, which mentions `XRSummary` but does not show the corresponding `ExpressionBinding` text needed to make it work. **This is a skill discovery / navigation issue, not a missing feature.** The correct guidance already exists in the references, but is not linked from the main file.

### What the correct behavior should have been
Use `sumSum([Price])` (lowercase `sum` prefix, camelCase) as the expression string:
```csharp
cell.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "sumSum([Price])"));
```

### Proposed skill fix
**Clarification** — add a callout/link in the main `SKILL.md` file to improve skill discovery. Add this note when discussing `XRSummary` or in the Common Patterns section:

```markdown
> **See also**: `references/expressions.md` for expression syntax and summary functions.
```


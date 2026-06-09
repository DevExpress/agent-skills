## Mistake 4

### Task context
The user asked to create an XtraReport in code with a group footer that displays the count of records in each group.

### What the skill said (or didn't say)
The skill's **Core Classes Overview** table listed `XRSummary` but only as a type name with no code example in the main body. The skill referenced `references/report-controls.md` for detailed control information but did not inline an `XRSummary` construction example in the Common Patterns section.

The dxdocs `Functions in Expressions` page lists `sumCount()`, `sumSum()` etc. under the heading **"Functions for Summary Expression Editor"** with the description:
> "Use the following functions when you calculate a summary across a report and its groups."

This confirms that `sumCount()` IS a valid standalone expression function usable in an `ExpressionBinding` on a label placed in a group/report footer band. It is NOT restricted to `XRSummary.FormatString` only.

The dxdocs also show the correct `XRSummary` initialization pattern:
```csharp
XRSummary summary = new XRSummary();
summary.Func = SummaryFunc.Count;
summary.Running = SummaryRunning.Group;
summary.FormatString = "{0}";
label.Summary = summary;
```

### What you did wrong
Failed to initialize `XRSummary` at all. Instead of using the proper `XRLabel.Summary` API — which is the designated, object-oriented way to attach summary behavior to a label — used a raw `ExpressionBinding` with `sumCount()` as a workaround:

```csharp
// Wrong — bypasses the XRSummary API entirely
summaryLabel.ExpressionBindings.Add(
	new ExpressionBinding("Text", "'Count: ' + sumCount([Region])"));
```

### Why you made the mistake
The skill was **missing a code pattern** for `XRSummary` in the Common Patterns section. `XRSummary` was listed in the Core Classes table by name only, with no construction example. When the agent needed a summary, it found `sumCount()` documented as a valid expression function and used it directly, bypassing the proper API. The skill did not include a hard-stop or pattern making clear that the intended approach for runtime-created summary labels is `XRSummary` — regardless of whether `sumCount()` also works as an expression.

### What the correct behavior should have been
Initialize `XRSummary` to set the summary scope (`Running` property), then use `ExpressionBinding` with a sum*() expression function. Both `XRSummary` and `ExpressionBinding` work together:

```csharp
// Correct — Pattern A: Use XRSummary.Func + FormatString
var summary = new XRSummary {
	Func = SummaryFunc.Count,
	Running = SummaryRunning.Group,
	FormatString = "Count: {0}"
};
summaryLabel.Summary = summary;

// Correct — Pattern B: Use XRSummary.Running + ExpressionBinding with sum*()
var summary = new XRSummary {
	Running = SummaryRunning.Group,
	FormatString = "{0}"
};
summaryLabel.Summary = summary;
summaryLabel.ExpressionBindings.Add(
	new ExpressionBinding("BeforePrint", "Text", "sumSum([Area])"));
```

Both patterns are valid:
- **Pattern A**: Set `Func` enum and `FormatString` directly on `XRSummary`
- **Pattern B**: Set `Running` scope on `XRSummary`, then use `ExpressionBinding` with `sum*()` function for dynamic calculation

### Proposed skill fix
**New pattern** — Add Pattern 8 for summaries in the Common Patterns section, immediately after the grouping example:

```markdown
**Pattern 8 — Group or report summary using XRSummary:**
```csharp
// Use XRSummary to attach calculated summary behavior to an XRLabel.
// Place the label in a GroupFooterBand or ReportFooterBand.
// XRSummary.FormatString controls the output; {0} is the calculated value.
var summaryLabel = new XRLabel {
	BoundsF = new RectangleF(0, 0, availableWidth, 18),
	TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight
};
var summary = new XRSummary {
	Func = SummaryFunc.Count,          // or .Sum, .Avg, .Max, .Min, etc.
	Running = SummaryRunning.Group,    // or .Page / .Report
	FormatString = "Count: {0}"        // {0} is replaced by the calculated value
};
summaryLabel.Summary = summary;
groupFooter.Controls.Add(summaryLabel);
```

> **Note**: `sumCount()`, `sumSum()` etc. are valid expression functions
> for use in `ExpressionBinding`. However, they still require `XRSummary` to be specified.


# Subreport Controls

## When to Use This Reference

Use when embedding another report's content inline: `XRSubreport`.

## XRSubreport — Embed Another Report

Embed another report's content inline:

```csharp
// Option 1: reference a report class instance
var sub = new XRSubreport();
detail.Controls.Add(sub);
sub.BoundsF = new RectangleF(0, 0, 400, 100);
sub.ReportSource = new DetailReport();

// Option 2: reference a .repx layout file (takes precedence over ReportSource)
sub.ReportSourceUrl = Path.Combine(AppContext.BaseDirectory, "DetailReport.repx");

// Pass a master report data field value to a subreport parameter
sub.ParameterBindings.Add(
    new ParameterBinding("subreportCategoryId", null, "Categories.CategoryID"));
```

**Main properties**:
- `ReportSource` — an `XtraReport` class instance whose content is included.
- `ReportSourceUrl` — path/URL to a `.repx` layout file. Takes precedence over `ReportSource`.

**Other key properties**:
- `ParameterBindings` — binds subreport parameters to fields or parameters of the master report.
- `GenerateOwnPages` — when `true`, the subreport generates its own page sequence.

> **Note**: `XRSubreport` cannot be placed inside `XRPanel`.

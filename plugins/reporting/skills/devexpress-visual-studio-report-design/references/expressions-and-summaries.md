# Expressions and Summaries Reference

## Criteria Language Quick Reference

| Element | Syntax | Example |
|---|---|---|
| Data field | `[FieldName]` | `[UnitPrice]` |
| Child collection aggregate | `[RelationName][].Count()` | `[Orders][].Count()` |
| Parameter | `?ParameterName` | `?StartDate` |
| String constant | `'text'` | `[Country] == 'France'` |
| Date constant | `#date#` | `[OrderDate] >= #2024-01-01#` |
| Arithmetic | `+` `-` `*` `/` | `[UnitPrice] * [Quantity]` |
| Conditional | `Iif(cond, trueVal, falseVal)` | `Iif([Amount] > 0, 'Pos', 'Neg')` |
| Nested conditional | `Iif(c1, v1, Iif(c2, v2, v3))` | `Iif([Status]=2, 'Done', Iif([Status]=1, 'WIP', 'New'))` |
| Multi-field format | `FormatString('{0}, {1}', [F1], [F2])` | `FormatString('{0:c2}', [Total])` |
| Null check | `IsNull([Field])` | `Iif(IsNull([Notes]), 'N/A', [Notes])` |
| String functions | `Trim()`, `Upper()`, `Lower()` | `Trim([Name])` |
| Date functions | `GetYear([Date])`, `GetDate([DateTime])` | `GetDate([OrderDate])` |
| Named image | `[Images.Name]` | `[Images.DeliveredIcon]` |

**Rules:**
- Single-quoted strings only — no double quotes
- `True` / `False` capitalized
- No C# ternary `? :`
- No `&&` or `||` — use `And`, `Or`
- Fields in square brackets `[FieldName]`

## Summary Functions (Footer Bands)

Use in `GroupFooterBand` or `ReportFooterBand`. These support Running scope — they accumulate within the specified scope.

| Function | Expression |
|---|---|
| Sum | `sumSum([Amount])` |
| Count | `sumCount([Id])` |
| Average | `sumAvg([Price])` |
| Min | `sumMin([Price])` |
| Max | `sumMax([Price])` |

```csharp
// Expression-based summary — most common approach
this.xrTotalCell.ExpressionBindings.AddRange(
    new DevExpress.XtraReports.UI.ExpressionBinding[] {
        new DevExpress.XtraReports.UI.ExpressionBinding(
            "BeforePrint", "Text", "sumSum([TotalAmount])") });
this.xrTotalCell.TextFormatString = "{0:c2}";
```

## XRSummary Object (Alternative)

Use when you need to control `SummaryRunning` scope explicitly via code:

```csharp
// Declared as local inline object at top of InitializeComponent (Step 1)
DevExpress.XtraReports.UI.XRSummary xrSummary1 =
    new DevExpress.XtraReports.UI.XRSummary();
xrSummary1.Running = DevExpress.XtraReports.UI.SummaryRunning.Report;
// Other options: SummaryRunning.Group, SummaryRunning.Page

this.xrTotalCell.Summary = xrSummary1;
this.xrTotalCell.ExpressionBindings.AddRange(
    new DevExpress.XtraReports.UI.ExpressionBinding[] {
        new DevExpress.XtraReports.UI.ExpressionBinding(
            "BeforePrint", "Text", "sumSum([TotalAmount])") });
this.xrTotalCell.TextFormatString = "{0:c2}";
```

## Child Collection Aggregates (Master Band)

Compute aggregates from a related child collection in the parent `DetailBand` — useful for showing order count or total directly on the master row without a footer:

```csharp
// Count of child records in master DetailBand
this.xrOrderCountCell.ExpressionBindings.AddRange(
    new DevExpress.XtraReports.UI.ExpressionBinding[] {
        new DevExpress.XtraReports.UI.ExpressionBinding(
            "BeforePrint", "Text", "[CustomersOrders_1][].Count()") });

// Average from child collection
this.xrAvgAmountCell.ExpressionBindings.AddRange(
    new DevExpress.XtraReports.UI.ExpressionBinding[] {
        new DevExpress.XtraReports.UI.ExpressionBinding(
            "BeforePrint", "Text", "[CustomersOrders_1][].Avg([TotalAmount])") });
this.xrAvgAmountCell.TextFormatString = "{0:c2}";
```

## Aggregate Functions — Avoid in DetailBand

`Sum([Amount])`, `Avg([Amount])` etc. apply to the **entire data source**, ignore `FilterString`, and are evaluated on every record. Never use them in `DetailBand`. Use `sumSum([Amount])` with `SummaryRunning.Group` or `Report` in footer bands instead.

## AllowMarkupText — Inline Formatting in XRLabel

```csharp
this.xrLabel1.AllowMarkupText = true;
// Supports HTML-like tags:
// <b>bold</b>  <i>italic</i>  <color=Red>colored</color>
// <size=12>size</size>  <br> line break
// Example from real report:
// "<color=170,175,189>Grand Total:</color>  {0:c2}"
```

## Watermarks

Set `DrawWatermark = true` on the report. Watermarks do not support data field expressions — apply conditional watermarks in code before document creation:

```csharp
report.Watermark.Text       = "CONFIDENTIAL";
report.Watermark.Font       = new DevExpress.Drawing.DXFont("Arial", 48f);
report.Watermark.ForeColor  = System.Drawing.Color.FromArgb(40, System.Drawing.Color.Red);
report.Watermark.ShowBehind = true;
report.DrawWatermark        = true;
```

# Sparkline Controls

## When to Use This Reference

Use when displaying compact inline charts for visualizing trends: `XRSparkline`.

## XRSparkline — Compact Inline Trend Charts

Compact inline chart for visualizing trends within a detail row:

```csharp
var sparkline = new XRSparkline();
detail.Controls.Add(sparkline);
sparkline.BoundsF = new RectangleF(200, 0, 150, 40);
sparkline.View = new DevExpress.Sparkline.LineSparklineView() {
    HighlightStartPoint = true,
    HighlightEndPoint = true,
    StartPointColor = System.Drawing.Color.Blue,
    EndPointColor = System.Drawing.Color.Red
};
// Use the report's own data source by default (omit DataSource).
// Provide a separate data source for the sparkline's data points:
sparkline.DataSource = salesHistoryDataSource;
sparkline.ValueMember = "Amount";  // data field with point values
```

**Available view types**: `LineSparklineView`, `AreaSparklineView`, `BarSparklineView`, `WinLossSparklineView`. Each view has `HighlightStartPoint`, `HighlightEndPoint`, `HighlightMinPoint`, `HighlightMaxPoint` to mark extreme values.

**Content properties**: 
- `DataSource` (optional separate data source for sparkline points)
- `DataMember`
- `ValueMember` (data field with point values)

**Other key properties**: `View`, `ValueRange`.

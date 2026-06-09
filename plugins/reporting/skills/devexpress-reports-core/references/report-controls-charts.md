# Chart Controls

## When to Use This Reference

Use when embedding data visualizations in reports: `XRChart`.

## XRChart — Embedded Chart Control

Embeds a DevExpress Chart control. The XRChart control is implemented based on the ChartControl for WinForms.

```csharp
var chart = new XRChart();
detail.Controls.Add(chart);
chart.BoundsF = new RectangleF(0, 0, 400, 200);
// Configure via chart.Series, chart.DataSource, etc.
// See DxDocs MCP for advanced chart configuration.
// Refer to the Chart Control (WinForms) help topics for information on possible configurations.
```

For complex chart configuration, use DevExpress MCP (dxdocs) documentation or refer to the Chart Control (WinForms) help topics.

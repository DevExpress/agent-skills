# Gauge Controls

## When to Use This Reference

Use when displaying numeric values as dial or linear gauges: `XRGauge`.

## XRGauge — Dial or Linear Gauge

Dial or linear gauge displaying a numeric value:

```csharp
var gauge = new XRGauge();
detail.Controls.Add(gauge);
gauge.BoundsF = new RectangleF(0, 0, 150, 150);
gauge.ViewType = DevExpress.XtraGauges.Core.Customization.DashboardGaugeType.Circular;  // Circular or Linear
gauge.ViewStyle = DevExpress.XtraGauges.Core.Customization.DashboardGaugeStyle.Full;    // Full, Half, QuarterLeft, QuarterRight, ThreeFourth, Horizontal, Vertical
gauge.ViewTheme = DevExpress.XtraGauges.Core.Customization.DashboardGaugeTheme.FlatLight;   // FlatLight or FlatDark
gauge.Minimum = 0;
gauge.Maximum = 100;
gauge.BoundsF = new RectangleF(0, 0, 150, 150);
gauge.TickmarkCount = 10;
gauge.ExpressionBindings.Add(
    new ExpressionBinding("BeforePrint", "ActualValue", "[Score]"));
```

**Content properties**: 
- `ActualValue` (the main property, bindable to a numeric data field)
- `TargetValue` (target marker)
- `Minimum`, `Maximum`

**Appearance properties**: `ViewType` (Circular/Linear), `ViewStyle`, `ViewTheme`, `TickmarkCount`, `ImageType` (Metafile/Bitmap/Svg rendering).

> **Note**: `XRGauge` export is unavailable in non-Windows environments using `System.Drawing.Common` v7+.

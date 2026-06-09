# Layout Controls

## When to Use This Reference

Use when grouping controls or creating visual structure: `XRPanel`, `XRPageBreak`, `XRLine`, `XRShape`, `XRCrossBandLine`, `XRCrossBandBox`.

## XRPanel — Container for Grouping Controls

Container for grouping controls into a single movable/stylable unit:

```csharp
var panel = new XRPanel();
detail.Controls.Add(panel);
panel.BoundsF = new RectangleF(0, 0, 300, 50);
panel.BackColor = Color.LightYellow;
panel.CanShrink = true;  // shrink height if inner controls are hidden
panel.Controls.Add(label1);
panel.Controls.Add(label2);
```

**Key properties**: `CanShrink` — automatically reduces panel height when inner controls are hidden (useful with conditional visibility).

**Cannot contain**: `XRPivotGrid`, `XRSubreport`, `XRPageBreak`, `XRTableOfContents`, `XRCrossBandLine`, `XRCrossBandBox`.

> **Note**: A panel cannot span across report bands. Use `XRCrossBandBox` if cross-band coverage is needed.

## XRPageBreak — Force a New Page

Force a new page:

```csharp
var pageBreak = new XRPageBreak();
detail.Controls.Add(pageBreak);
pageBreak.LocationF = new PointF(0, 25);
```

## XRLine — Horizontal or Vertical Rule

Horizontal or vertical rule:

```csharp
var line = new XRLine();
detail.Controls.Add(line);
line.LineDirection = DevExpress.XtraReports.UI.LineDirection.Vertical;
line.LineStyle = DevExpress.Drawing.DXDashStyle.Dash;
line.LineWidth = 1F;
line.LocationF = new System.Drawing.PointF(0, 0);
line.SizeF = new System.Drawing.SizeF(100F, 25F);
```

**Key properties**: `LineDirection` (Horizontal/Vertical/Slant/BackSlant), `LineStyle` (Solid/Dash/Dot/etc.), `LineWidth` in pixels, `ForeColor`.

## XRShape — Geometric Shapes

Geometric shapes (arrow, rectangle, ellipse, polygon, star, cross, etc.):

```csharp
using DevExpress.XtraPrinting.Shape;

var shape = new XRShape();
detail.Controls.Add(shape);
shape.BoundsF = new RectangleF(0, 0, 100, 80);
shape.Shape = new DevExpress.XtraPrinting.Shape.ShapeRectangle { Fillet = 20 };  // 0-100: corner roundness
shape.FillColor = Color.LightBlue;
shape.ForeColor = Color.DarkBlue;
shape.LineWidth = 2;
shape.Stretch = true;  // shape stretches when control is resized
```

**Available shape types**: `ShapeArrow`, `ShapeRectangle` (with `Fillet`), `ShapeEllipse`, `ShapeLine`, `ShapePolygon` (with `NumberOfSides`, `Fillet`), `ShapeStar`, `ShapeCross`, `ShapeBracket`, `ShapeBrace`.

**Key properties**: `Shape` (the type object), `FillColor`, `ForeColor`, `LineWidth`, `Angle` (rotation in degrees), `Stretch`.

## XRCrossBandLine / XRCrossBandBox — Vertical Lines/Boxes Spanning Bands

Vertical line or rectangle that spans across multiple report bands:

```csharp
// Both controls are added to report.CrossBandControls — NOT to band.Controls.
var crossBandBox = new XRCrossBandBox() {
    StartBand = report.Bands[BandKind.TopMargin],
    EndBand   = report.Bands[BandKind.BottomMargin],
    StartPointF = new DevExpress.Utils.PointFloat(10F, 0F),
    EndPointF   = new DevExpress.Utils.PointFloat(10F, 0F),
    WidthF = 780F,
    BackColor   = Color.Transparent,
    BorderColor = Color.DarkGray,
    BorderWidth = 1F
};
report.CrossBandControls.Add(crossBandBox);

var crossBandLine = new XRCrossBandLine() {
    StartBand   = report.Bands[BandKind.PageHeader],
    EndBand     = report.Bands[BandKind.PageFooter],
    StartPointF = new DevExpress.Utils.PointFloat(200F, 0F),
    EndPointF   = new DevExpress.Utils.PointFloat(200F, 0F),
    LineStyle   = System.Drawing.Drawing2D.DashStyle.Dash,
    WidthF      = 1F
};
report.CrossBandControls.Add(crossBandLine);
```

**Key properties (both controls)**: `StartBand`, `EndBand`, `StartPointF`, `EndPointF`, `WidthF`, `ForeColor`.
- `XRCrossBandBox` adds: `Borders`, `BorderColor`, `BorderWidth`, `BackColor`.
- `XRCrossBandLine` adds: `LineStyle` (`DashStyle` enum).

**Placement rule**: Start and end must be on *complementary* bands that appear together on every page: `TopMarginBand`↔`BottomMarginBand`, `PageHeaderBand`↔`PageFooterBand`, `ReportHeaderBand`↔`ReportFooterBand`, or `GroupHeaderBand`↔`GroupFooterBand` (both with `RepeatEveryPage` enabled). Mismatched bands produce an `XRE040` warning and unpredictable output.

**Limitations**:
- Expression bindings are **not** processed — use `BeforePrint` event handler instead.
- Cannot start/end on vertical bands.
- Lines/boxes thinner than 0.5 px are omitted from HTML (SingleFile/TableLayout), RTF (SingleFile), DOCX (SingleFile/TableLayout), CSV, TXT, XLS, XLSX exports.
- `CanShrink` on overlapping controls has no effect because cross-band controls do not shrink.

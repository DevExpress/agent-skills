# Axes

This reference covers the axis model on `XYDiagram` (and 3D / SwiftPlot variants): primary `AxisX`/`AxisY`, unlimited secondary axes, the four scale types (Numerical, DateTime, TimeSpan, Qualitative), their option classes, and how to set visual / whole ranges.

For axis title and label formatting see [axis-titles-and-labels.md](axis-titles-and-labels.md). For aggregation see [aggregation-and-summary.md](aggregation-and-summary.md).

## When to Use This Reference

- Configuring primary axes (`XYDiagram.AxisX`/`AxisY`): visibility, position, range, reverse, interlaced.
- Adding secondary axes for series with very different scales (revenue vs temperature).
- Picking the right scale type (Numerical / DateTime / DateTimeOffset / TimeSpan / Qualitative) and its option class.
- Limiting the visible range (zoom-like) or the whole computable range.
- Customizing tickmarks, grid lines, strips, and constant lines on an axis.

## Axis Model

```text
XYDiagram
├── AxisX              ← primary X axis (always exists)
├── AxisY              ← primary Y axis (always exists)
├── SecondaryAxesX     ← collection of additional X axes (0..N)
└── SecondaryAxesY     ← collection of additional Y axes (0..N)
```

| Member | Purpose |
|---|---|
| `XYDiagram.AxisX` / `AxisY` | The two primary axes. Type — `AxisXBase` / `AxisYBase`. |
| `XYDiagram.SecondaryAxesX` / `SecondaryAxesY` | Add as many `SecondaryAxisX` / `SecondaryAxisY` as needed. |
| `XYDiagram.Rotated` | Swap X and Y (turns vertical bar into horizontal). |
| `XYDiagram.Panes` | Split the plot area into multiple stacked panes — each pane can show different series and have its own Y axis. |
| `XYDiagram.EnableAxisXZooming` / `EnableAxisYZooming` / `EnableAxisXScrolling` / `EnableAxisYScrolling` | End-user zoom/scroll on each axis. |

3D charts use `XYDiagram3D.AxisX` / `AxisY` / `AxisZ` (no secondary axes). `SwiftPlotDiagram` exposes the same `AxisX` / `AxisY` (no secondaries either).

## Adding a Secondary Axis

```csharp
var diagram = (XYDiagram)chart.Diagram;

var tempAxis = new SecondaryAxisY("temperature") {
    Alignment   = AxisAlignment.Far,             // right side
    Title       = { Text = "°C", Visibility = DefaultBoolean.True },
    Label       = { TextPattern = "{V:0}°" }
};
diagram.SecondaryAxesY.Add(tempAxis);

// Assign the series to this axis
((LineSeriesView)temperatureSeries.View).AxisY = tempAxis;
```

Every series view on an `XYDiagram` exposes `.AxisX` and `.AxisY` properties (on the **view**, not on the series). Set them to bind a series to a specific secondary axis.

## Scale Types

`ScaleType` selects how arguments / values are interpreted on the axis:

| `ScaleType` | Axis class | Options class | Source data |
|---|---|---|---|
| `Numerical` | `AxisXBase` / `AxisYBase` | `NumericScaleOptions` | `int` / `double` / `decimal` |
| `DateTime` | same | `DateTimeScaleOptions` | `DateTime` |
| `DateTimeOffset` | same | `DateTimeScaleOptions` | `DateTimeOffset` |
| `TimeSpan` | same | `TimeSpanScaleOptions` | `TimeSpan` |
| `Qualitative` | same | `QualitativeScaleOptions` | `string` / any non-comparable category |
| `Auto` | same | inferred | — (default; detects type at runtime) |

Set on the **series** (not the axis) via `series.ArgumentScaleType` / `series.ValueScaleType`. The axis inspects its series and switches its option model accordingly.

```csharp
series.ArgumentScaleType = ScaleType.DateTime;
series.ValueScaleType    = ScaleType.Numerical;
```

After this, `diagram.AxisX.DateTimeScaleOptions` becomes the active option set. The other `*ScaleOptions` properties exist but are ignored.

## `NumericScaleOptions`

```csharp
diagram.AxisX.NumericScaleOptions.ScaleMode   = ScaleMode.Manual;
diagram.AxisX.NumericScaleOptions.GridSpacing = 5;          // tick every 5 units
diagram.AxisX.NumericScaleOptions.GridAlignment = NumericGridAlignment.Tens;
diagram.AxisX.NumericScaleOptions.AutoGrid    = false;
```

| Member | Purpose |
|---|---|
| `ScaleMode` | `Automatic` (auto major step) / `Manual` (use `GridSpacing`) / `Continuous`. |
| `GridSpacing` | Major-tick step. |
| `GridAlignment` | Snap ticks to `Ones` / `Tens` / `Hundreds` / `Thousands`. |
| `AutoGrid` | Automatic minor gridline computation. |
| `AggregateFunction` | Aggregation function when scale mode is `Automatic`. |

## `DateTimeScaleOptions`

```csharp
var opt = diagram.AxisX.DateTimeScaleOptions;
opt.ScaleMode         = ScaleMode.Automatic;          // enables aggregation
opt.MeasureUnit       = DateTimeMeasureUnit.Month;
opt.GridSpacing       = 1;
opt.AutoGrid          = false;
opt.AggregateFunction = AggregateFunction.Average;
opt.WorkdaysOnly      = true;                          // skip weekends
opt.WorkdaysOptions.Holidays.Add(new KnownDate("Christmas", new DateTime(2026, 12, 25)));
```

| Member | Purpose |
|---|---|
| `ScaleMode` | `Continuous` (proportional date axis), `Manual` (regular grid), `Automatic` (auto-aggregate). |
| `MeasureUnit` | Tick granularity (Year / Quarter / Month / Week / Day / Hour / Minute / Second / Millisecond). |
| `GridSpacing` | Number of measure units per tick. |
| `AggregateFunction` | Aggregation when `ScaleMode = Automatic`. |
| `WorkdaysOnly` | Skip weekends. |
| `WorkdaysOptions` | Customize weekdays + holidays. |
| `BeginItemsHorizontalIndent` / `EndItemsHorizontalIndent` | Leave margin so first/last point isn't on the axis edge. |

`ScaleMode` summary for date-time:

| Mode | Behavior |
|---|---|
| `Continuous` | True proportional time axis. No aggregation. Best for a few thousand points. |
| `Manual` | Treats time as a regular grid with `MeasureUnit` × `GridSpacing`. |
| `Automatic` | Aggregates dense data into buckets sized by `MeasureUnit`. See [aggregation-and-summary.md](aggregation-and-summary.md). |

## `TimeSpanScaleOptions`

Mirrors `DateTimeScaleOptions` with `TimeSpanMeasureUnit` (Day / Hour / Minute / Second / …).

```csharp
diagram.AxisX.TimeSpanScaleOptions.ScaleMode   = ScaleMode.Manual;
diagram.AxisX.TimeSpanScaleOptions.MeasureUnit = TimeSpanMeasureUnit.Hour;
diagram.AxisX.TimeSpanScaleOptions.GridSpacing = 6;
```

## `QualitativeScaleOptions`

```csharp
diagram.AxisX.QualitativeScaleOptions.AutoGrid = false;
diagram.AxisX.QualitativeScaleOptions.GridSpacing = 1;       // show every category
```

Qualitative axes have no continuous distance — points are placed at integer positions in `Series.Points` insertion order.

## Visible Range vs Whole Range

Each axis has two range models:

| Range | Meaning |
|---|---|
| `Axis.WholeRange` | The whole computable range — used by zoom (the chart cannot scroll beyond this). |
| `Axis.VisualRange` | Currently visible range — what the user sees and can pan. |

```csharp
var x = diagram.AxisX;

// Hard cap the entire range
x.WholeRange.SetMinMaxValues(new DateTime(2026, 1, 1), new DateTime(2026, 12, 31));
x.WholeRange.AutoSideMargins = true;

// Initial zoom
x.VisualRange.SetMinMaxValues(new DateTime(2026, 3, 1), new DateTime(2026, 5, 1));

// Allow auto computation (default)
x.WholeRange.Auto = true;
```

`SetMinMaxValues(min, max)` is the most common entry point — both arguments must match the axis scale type.

For numeric/date the visible range can be controlled by the end user via mouse wheel / scrollbar when `XYDiagram.EnableAxisXZooming = true`.

## Common Axis Properties

| Member | Purpose |
|---|---|
| `Axis.Alignment` | `Near` (left/bottom) / `Far` (right/top) / `Zero` (anchored to origin). |
| `Axis.Visibility` | `True` / `False` / `Default`. Hides the whole axis. |
| `Axis.Reverse` | Flips the axis direction. |
| `Axis.Interlaced` | Alternating stripes between major ticks (use `InterlacedColor` to set color). |
| `Axis.GridLines` | Major gridline pen, visibility. |
| `Axis.MinorGridLines` / `MinorCount` | Minor ticks between majors. |
| `Axis.Tickmarks` | Tickmark length, thickness, visibility. |
| `Axis.Strips` | Highlighted horizontal/vertical bands (e.g., recession period). |
| `Axis.ConstantLines` | Annotated reference line at a specific value. |
| `Axis.Logarithmic` | `Axis.Logarithmic = true` + `LogarithmicBase` for log scale. |

## Strips and Constant Lines

```csharp
diagram.AxisY.ConstantLines.Add(new ConstantLine("Target", 100) {
    Color = Color.OrangeRed,
    LineStyle = { Thickness = 2, DashStyle = DashStyle.Dash },
    Title    = { Text = "Q4 Target", Alignment = ConstantLineTitleAlignment.Far }
});

diagram.AxisX.Strips.Add(new Strip("Recession",
    new DateTime(2026, 6, 1), new DateTime(2026, 9, 1)) {
    Color = Color.FromArgb(64, Color.Gray)
});
```

## Logarithmic Axis

```csharp
diagram.AxisY.Logarithmic    = true;
diagram.AxisY.LogarithmicBase = 10;
```

Log axes need positive values only — clip or filter zero/negative data.

## Panes — Multiple Plot Areas Sharing One Chart

```csharp
var pane = new XYDiagramPane("temperature");
diagram.Panes.Add(pane);

// Bind series to pane via its axis
var paneAxisY = new SecondaryAxisY("tempY") { Alignment = AxisAlignment.Near };
diagram.SecondaryAxesY.Add(paneAxisY);
paneAxisY.Visibility = DefaultBoolean.True;

((LineSeriesView)tempSeries.View).Pane = pane;
((LineSeriesView)tempSeries.View).AxisY = paneAxisY;
```

Panes split the chart vertically (when not rotated) so each series group has its own plot area while sharing the X axis.

## Common Patterns

### Pattern 1 — Forced manual axis grid

```csharp
diagram.AxisY.NumericScaleOptions.ScaleMode   = ScaleMode.Manual;
diagram.AxisY.NumericScaleOptions.GridSpacing = 1000;
```

### Pattern 2 — Reverse Y axis (top = 0)

```csharp
diagram.AxisY.Reverse = true;
```

### Pattern 3 — Two Y axes (revenue + units sold)

```csharp
var unitsAxis = new SecondaryAxisY("units") { Alignment = AxisAlignment.Far };
diagram.SecondaryAxesY.Add(unitsAxis);
((BarSeriesView)unitsSeries.View).AxisY = unitsAxis;
```

### Pattern 4 — Date axis between two dates

```csharp
diagram.AxisX.DateTimeScaleOptions.ScaleMode = ScaleMode.Continuous;
diagram.AxisX.WholeRange.SetMinMaxValues(new DateTime(2026, 1, 1), new DateTime(2026, 12, 31));
diagram.AxisX.WholeRange.AutoSideMargins = false;
```

### Pattern 5 — Logarithmic Y axis for orders-of-magnitude data

```csharp
diagram.AxisY.Logarithmic     = true;
diagram.AxisY.LogarithmicBase = 10;
diagram.AxisY.Label.TextPattern = "{V:0}";
```

## Common Issues

- **Axis ignores `SetMinMaxValues`** — `Range.Auto = true` overrides explicit values. Set `Range.Auto = false` first.
- **Date axis shows duplicate ticks** — `ScaleMode = Manual` with too small a `GridSpacing` on `Continuous` data. Switch to `Continuous` or increase the spacing.
- **Secondary axis ignored** — the series view's `.AxisX`/`.AxisY` still points at the primary axis. Set it explicitly.
- **`WorkdaysOnly` shows gaps** — by design; weekends are removed. To keep continuous time, leave `WorkdaysOnly = false` and accept the gaps as visible.
- **`Logarithmic = true` throws** — series contains zero or negative values. Filter the data.
- **Reversed axis looks wrong with stacked series** — reverse the value axis, not the argument axis.
- **Cannot find `SecondaryAxesX` on `XYDiagram3D`** — 3D diagrams have only primary axes.

## Source Material

- `articles/controls-and-libraries/chart-control/diagram/axes.md` (`xref:WindowsForms.5476`).
- `articles/controls-and-libraries/chart-control/diagram/axis-scale-types.md` (`xref:WindowsForms.5443`).
- `articles/controls-and-libraries/chart-control/diagram/secondary-axes.md` (`xref:WindowsForms.5446`).
- `articles/controls-and-libraries/chart-control/diagram/visual-range-and-whole-range.md` (`xref:WindowsForms.114768`).
- `api/DevExpress.XtraCharts.XYDiagram.AxisX.yml`, `AxisY.yml`, `SecondaryAxesX.yml`, `SecondaryAxesY.yml`.
- `api/DevExpress.XtraCharts.AxisBase.yml`.
- `api/DevExpress.XtraCharts.NumericScaleOptions.yml`, `DateTimeScaleOptions.yml`, `TimeSpanScaleOptions.yml`, `QualitativeScaleOptions.yml`.
- `api/DevExpress.XtraCharts.Range.yml`.

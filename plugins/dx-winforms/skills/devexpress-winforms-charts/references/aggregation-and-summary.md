# Data Aggregation and Summary

This reference covers automatic data aggregation on numeric, date-time, and time-span axes — the `ScaleMode` modes, `AggregateFunction` choices, and when to switch to `SwiftPlotDiagram` instead.

For scale-type configuration see [axes.md](axes.md). For binding data see [data-binding.md](data-binding.md).

## When to Use This Reference

- A series has many points (tens of thousands or more) and renders too slowly.
- A date-time axis should show monthly / weekly buckets even though the underlying data is per-hour.
- Want to display an aggregation (Sum / Average / Min / Max / OHLC) without pre-computing it in code.
- Building a histogram from raw measurements.

## What Aggregation Does

When `ScaleMode = Automatic` (and `AggregateFunction != None`), the chart **groups** all `SeriesPoint`s whose argument falls into the same axis-grid bucket, applies the aggregate function, and replaces the bucket with a **single** visible point.

The underlying `series.Points` collection is **not modified** — aggregation is a render-time operation. Tooltips and crosshair show the aggregated value; the raw points remain in memory.

## `ScaleMode` Summary

| `ScaleMode` | Aggregates? | Notes |
|---|---|---|
| `Continuous` | No | True proportional axis; every point is rendered. Best for small/medium data. |
| `Manual` | No | Regular grid with fixed `GridSpacing`/`MeasureUnit`. Points keep their original positions. |
| `Automatic` | **Yes** | Buckets data by computed grid step; applies `AggregateFunction`. |

Aggregation is available on all four scale-option classes: `NumericScaleOptions`, `DateTimeScaleOptions`, `DateTimeOffsetScaleOptions`, `TimeSpanScaleOptions`. Qualitative axes do not aggregate (you handle that via `QualitativeSummaryOptions` instead).

## `AggregateFunction` Catalog

| Value | What it computes | Typical use |
|---|---|---|
| `None` | No aggregation (default). | Pre-aggregated data. |
| `Average` | Mean of bucket. | Sensor readings, percentages. |
| `Sum` | Sum of bucket. | Revenue, totals. |
| `Minimum` | Min of bucket. | Lower-bound stats. |
| `Maximum` | Max of bucket. | Upper-bound stats. |
| `Financial` | Picks Open/High/Low/Close of bucket. | Stock / Candlestick on hourly → daily data. |
| `Histogram` | Bucket *count* (frequency). | Frequency distributions. |

`Financial` is only meaningful for series with 4 values (Stock, CandleStick). Other series treat it as `Average`.

## Numeric Aggregation

```csharp
diagram.AxisX.NumericScaleOptions.ScaleMode         = ScaleMode.Automatic;
diagram.AxisX.NumericScaleOptions.AggregateFunction = AggregateFunction.Average;
```

The chart auto-computes a bucket width based on the visible range and the available pixels. To force a bucket width:

```csharp
diagram.AxisX.NumericScaleOptions.GridSpacing  = 10;     // 1 bucket per 10 units
diagram.AxisX.NumericScaleOptions.AutoGrid     = false;
```

## DateTime Aggregation

The most common scenario — millions of timestamps that should display as days / months / quarters.

```csharp
var opt = diagram.AxisX.DateTimeScaleOptions;
opt.ScaleMode         = ScaleMode.Automatic;
opt.MeasureUnit       = DateTimeMeasureUnit.Month;
opt.GridSpacing       = 1;                                 // 1 bucket per month
opt.AggregateFunction = AggregateFunction.Sum;
opt.AutoGrid          = false;
```

| `DateTimeMeasureUnit` | Bucket size |
|---|---|
| `Year`, `Quarter`, `Month`, `Week`, `Day` | Coarse buckets. |
| `Hour`, `Minute`, `Second`, `Millisecond` | Fine buckets. |

Combine with `GridSpacing` for custom intervals (e.g., `MeasureUnit = Minute` + `GridSpacing = 5` → 5-minute buckets).

### Automatic measure unit

Leave `MeasureUnit` at its default (`Year`) but set `AutoGrid = true` to let the chart pick the bucket size that fits the current zoom level. As the user zooms in, buckets get finer; as they zoom out, buckets coarsen. This is the **single best feature** for large time-series.

```csharp
var opt = diagram.AxisX.DateTimeScaleOptions;
opt.ScaleMode         = ScaleMode.Automatic;
opt.AutoGrid          = true;                              // adapt to zoom level
opt.AggregateFunction = AggregateFunction.Average;
```

## TimeSpan Aggregation

```csharp
var opt = diagram.AxisX.TimeSpanScaleOptions;
opt.ScaleMode         = ScaleMode.Automatic;
opt.MeasureUnit       = TimeSpanMeasureUnit.Hour;
opt.GridSpacing       = 1;
opt.AggregateFunction = AggregateFunction.Maximum;
```

## Per-Series Override

Each series can override the aggregate function set on the axis:

```csharp
series.QualitativeSummaryOptions.SummaryFunction = "SUM([Value])";     // qualitative
```

For numeric/date-time, aggregation is **axis-driven**, so all series sharing that axis use the same function. To aggregate one series differently, assign it to a secondary axis with its own settings.

## Qualitative Summary

Qualitative axes do not aggregate by bucket (there is no continuous distance). Instead, when two points share the **same argument**, `QualitativeSummaryOptions.SummaryFunction` decides how they combine. It is a **`string`** holding a summary expression (`[Value]` = the value data member), not an enum:

| `SummaryFunction` string | Behavior |
|---|---|
| `""` (empty) | Keep duplicates (multiple points at one argument). |
| `"SUM([Value])"` | Sum values for the same argument. |
| `"AVERAGE([Value])"` | Mean. |
| `"MIN([Value])"` / `"MAX([Value])"` / `"COUNT()"` | As named. |

```csharp
series.QualitativeSummaryOptions.SummaryFunction = "SUM([Value])";
```

## Histogram

To produce a frequency histogram from raw measurements:

```csharp
var samples = new Series("Distribution", ViewType.Bar);
samples.DataSource         = measurements;
samples.ArgumentDataMember = nameof(Measurement.Value);
samples.ValueDataMembers.AddRange(nameof(Measurement.Value));   // dummy — ignored
samples.ArgumentScaleType  = ScaleType.Numerical;
chart.Series.Add(samples);

var diagram = (XYDiagram)chart.Diagram;
var opt = diagram.AxisX.NumericScaleOptions;
opt.ScaleMode         = ScaleMode.Automatic;
opt.AggregateFunction = AggregateFunction.Histogram;
opt.GridSpacing       = 0.5;                                    // bucket width
opt.AutoGrid          = false;
```

Each bar's height is the count of points whose value falls in `[x, x+0.5)`.

## When NOT to Aggregate — Switch to SwiftPlot

If the user needs to see **every** point (zoom-in workflows, anomaly detection, raw sensor data), aggregation is the wrong answer because the chart shows fewer points than exist.

Switch to `SwiftPlotDiagram` instead:

```csharp
chart.Series.Clear();
var fast = new Series("Telemetry", ViewType.SwiftPlot);
fast.DataSource = millionsOfPoints;
fast.ArgumentDataMember = nameof(Sample.Timestamp);
fast.ValueDataMembers.AddRange(nameof(Sample.Value));
fast.ArgumentScaleType  = ScaleType.DateTime;
chart.Series.Add(fast);
```

`SwiftPlotDiagram` renders the full data set with hardware acceleration but disables per-point hit testing. Aggregation + SwiftPlot can be combined for the absolute fastest large-data rendering.

## Decision Matrix

| Data size | Show all points? | Pick |
|---|---|---|
| Up to ~10k | Yes | `XYDiagram` + `ScaleMode.Continuous` |
| 10k – 100k | Aggregated buckets ok | `XYDiagram` + `ScaleMode.Automatic` |
| 100k – 1M | Aggregated buckets ok | `XYDiagram` + `ScaleMode.Automatic` + `AutoGrid` |
| 100k – 1M | Yes (every point) | `SwiftPlotDiagram` |
| > 1M | Yes (every point) | `SwiftPlotDiagram` + axis aggregation off |
| > 10M | Aggregated buckets ok | `SwiftPlotDiagram` + aggregation |

## Common Patterns

### Pattern 1 — Monthly revenue sum from daily rows

```csharp
diagram.AxisX.DateTimeScaleOptions.ScaleMode         = ScaleMode.Automatic;
diagram.AxisX.DateTimeScaleOptions.MeasureUnit       = DateTimeMeasureUnit.Month;
diagram.AxisX.DateTimeScaleOptions.GridSpacing       = 1;
diagram.AxisX.DateTimeScaleOptions.AggregateFunction = AggregateFunction.Sum;
diagram.AxisX.DateTimeScaleOptions.AutoGrid          = false;
```

### Pattern 2 — Zoom-adaptive buckets

```csharp
var opt = diagram.AxisX.DateTimeScaleOptions;
opt.ScaleMode         = ScaleMode.Automatic;
opt.AutoGrid          = true;
opt.AggregateFunction = AggregateFunction.Average;
```

### Pattern 3 — Daily OHLC from intraday ticks

```csharp
diagram.AxisX.DateTimeScaleOptions.ScaleMode         = ScaleMode.Automatic;
diagram.AxisX.DateTimeScaleOptions.MeasureUnit       = DateTimeMeasureUnit.Day;
diagram.AxisX.DateTimeScaleOptions.GridSpacing       = 1;
diagram.AxisX.DateTimeScaleOptions.AggregateFunction = AggregateFunction.Financial;
```

### Pattern 4 — Histogram of measurement values

```csharp
diagram.AxisX.NumericScaleOptions.ScaleMode         = ScaleMode.Automatic;
diagram.AxisX.NumericScaleOptions.AggregateFunction = AggregateFunction.Histogram;
diagram.AxisX.NumericScaleOptions.GridSpacing       = 1;
```

### Pattern 5 — Qualitative sum (multiple rows per category)

```csharp
series.QualitativeSummaryOptions.SummaryFunction = "SUM([Value])";
```

## Common Issues

- **Aggregation does nothing** — `ScaleMode` left at `Continuous` (default). Set `Automatic`.
- **Buckets too coarse / too fine** — leave `AutoGrid = true` to let the chart adapt to zoom, or set `MeasureUnit` + `GridSpacing` + `AutoGrid = false` manually.
- **`Financial` aggregation produces flat bars** — series isn't OHLC. Use `ViewType.CandleStick` or `Stock` and `SetFinancialDataMembers`.
- **Histogram bars look uneven** — last bucket may be partial. Increase `GridSpacing` or filter outliers.
- **Tooltip shows aggregated value but user wants raw** — disable aggregation (`ScaleMode = Continuous`) or build a custom tooltip via the `CustomDrawCrosshair` / `BoundDataChanged` event.
- **Memory still high** — aggregation does not discard raw points; it only changes what's drawn. To reduce memory, pre-aggregate the data source.

## Source Material

- `articles/controls-and-libraries/chart-control/diagram/data-aggregation.md` (`xref:WindowsForms.114763`).
- `articles/controls-and-libraries/chart-control/diagram/axis-scale-types.md` (`xref:WindowsForms.5443`).
- `articles/controls-and-libraries/chart-control/diagram/summary-function.md` (`xref:WindowsForms.7395`).
- `api/DevExpress.XtraCharts.ScaleMode.yml`.
- `api/DevExpress.XtraCharts.AggregateFunction.yml`.
- `api/DevExpress.XtraCharts.DateTimeScaleOptions.yml`.
- `api/DevExpress.XtraCharts.NumericScaleOptions.yml`.
- `api/DevExpress.XtraCharts.QualitativeSummaryOptions.yml`.

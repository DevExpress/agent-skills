# Data Aggregation and Summaries

The Chart Control can **aggregate** raw data points into intervals — replacing many individual points with a smaller set of summary values (Average, Sum, Count, etc.). Aggregation runs in memory on the bound data; **summaries** (a separate feature) run on the server. This page covers when to choose each, and how to wire aggregation with the right scale options.

## When to Use This Reference

Use this when you need to:

- Reduce point count for a noisy or huge data set
- Bucket time-series data by day / month / quarter / year
- Bucket numeric data into ranges
- Pick between **aggregation** (in-memory) and **summary** (server-side)
- Apply different aggregate functions per series
- Implement a custom aggregate function

## Aggregation vs Summaries — Which to Choose

|  | Aggregation | Summary |
|---|---|---|
| **Where it runs** | In memory, on the client | On the server (data source) |
| **Stored values** | Raw values; chart computes aggregates on the fly | Already-aggregated values; chart just renders |
| **Behavior on zoom** | Recalculates as the user zooms (Automatic mode) | No recalculation — chart shows the supplied buckets |
| **Memory** | Higher — raw data lives in memory | Lower |
| **When to pick** | < 1M points, want zoom-aware detail, no server logic available | Very large data sets (10M+ points), server-side analytics already aggregating |

**Aggregation is the default choice for most apps.** Summaries are advanced (see `articles/.../calculate-summaries.md` https://docs.devexpress.com/content/WPF/400970?md=true).

> **Aggregation is x-axis only.** The y-axis can't aggregate values — it always renders the computed result.

## Aggregate Functions

`AggregateFunction` enum:

| Value | Use |
|---|---|
| `None` | Disable aggregation |
| `Average` (default) | Mean of each bucket |
| `Sum` | Sum of each bucket |
| `Count` | Number of points per bucket |
| `Minimum` / `Maximum` | Min / max per bucket |
| `Financial` | OHLC aggregation — for `StockSeries2D` / `CandleStickSeries2D` |
| `Histogram` | Bucket counts — used for histogram charts |
| `Custom` | Use a `CustomAggregateFunction` subclass |

## Scale Options That Drive Aggregation

Aggregation behavior depends on the scale options assigned to the **x-axis**:

| Options class | Behavior |
|---|---|
| `Automatic*ScaleOptions` | Chart picks the optimal measurement unit; recomputes aggregates on zoom |
| `Manual*ScaleOptions` | You set `MeasureUnit`; aggregation buckets to that unit |
| `Continuous*ScaleOptions` | **No aggregation** — every point is plotted as-is |
| `Interval*ScaleOptions` | Histogram-style intervals (`(100, 200]`, `(200, 300]`) |
| `QualitativeScaleOptions` | For string-category axes — aggregates points with equal arguments |

`Continuous*` is the opt-out: if a chart with `Continuous` options has `AggregateFunction` set on the options, it's ignored.

## Aggregate Date-Time Data

### Automatic — Chart Picks the Unit

```xaml
<dxc:AxisX2D.DateTimeScaleOptions>
    <dxc:AutomaticDateTimeScaleOptions AggregateFunction="Average"/>
</dxc:AxisX2D.DateTimeScaleOptions>
```

The chart inspects the data range and zoom level to pick day / month / year. Best for charts with zoom enabled.

### Manual — Fixed Unit (e.g., Monthly Average)

```xaml
<dxc:AxisX2D.DateTimeScaleOptions>
    <dxc:ManualDateTimeScaleOptions AggregateFunction="Average"
                                    MeasureUnit="Month"
                                    GridAlignment="Year"/>
</dxc:AxisX2D.DateTimeScaleOptions>
```

`MeasureUnit` controls aggregation granularity. `GridAlignment` controls label tick alignment (independently of the unit).

### Custom Bucket Size — Every Six Months

```xaml
<dxc:ManualDateTimeScaleOptions AggregateFunction="Average"
                                MeasureUnit="Month"
                                MeasureUnitMultiplier="6"
                                GridAlignment="Year"/>
```

`MeasureUnitMultiplier` lets you express quarters (`Month × 3`), half-years (`Month × 6`), and other custom intervals.

## Aggregate Numeric Data

### Automatic

```xaml
<dxc:AxisX2D.NumericScaleOptions>
    <dxc:AutomaticNumericScaleOptions AggregateFunction="Average"/>
</dxc:AxisX2D.NumericScaleOptions>
```

### Manual — Fixed Bucket Size

```xaml
<dxc:AxisX2D.NumericScaleOptions>
    <dxc:ManualNumericScaleOptions AggregateFunction="Average" MeasureUnit="1000"/>
</dxc:AxisX2D.NumericScaleOptions>
```

Each bucket spans 1000 units; the value field is averaged per bucket.

## Aggregate TimeSpan Data

```xaml
<dxc:AxisX2D.TimeSpanScaleOptions>
    <dxc:ManualTimeSpanScaleOptions AggregateFunction="Average"
                                    MeasureUnit="Hour"
                                    MeasureUnitMultiplier="3"/>
</dxc:AxisX2D.TimeSpanScaleOptions>
```

`MeasureUnit` for time-span: `Day`, `Hour`, `Minute`, `Second`, etc. Multiplier works the same way.

## Aggregate Qualitative Data

When multiple points share the same string argument, the chart can aggregate them:

```xaml
<dxc:AxisX2D.QualitativeScaleOptions>
    <dxc:QualitativeScaleOptions AggregateFunction="Sum"/>
</dxc:AxisX2D.QualitativeScaleOptions>
```

Without aggregation, repeated arguments either overlap or stack (depending on series type). With `Sum` (or other functions), they roll up into one point per unique argument.

## Per-Series Aggregation Override

A series can override the axis's aggregate function via `XYSeries.AggregateFunction`:

```xaml
<dxc:XYDiagram2D>
    <dxc:StockSeries2D x:Name="stockSeries"
                       ArgumentDataMember="DateTimeStamp"
                       OpenValueDataMember="Open" HighValueDataMember="High"
                       LowValueDataMember="Low" CloseValueDataMember="Close"
                       AggregateFunction="Financial"/>

    <dxc:BarSideBySideSeries2D x:Name="volumeSeries"
                               ArgumentDataMember="DateTimeStamp"
                               ValueDataMember="Volume"
                               AggregateFunction="Sum"/>
</dxc:XYDiagram2D>
```

Here the stock prices get **OHLC aggregation** (`Financial`) while volume bars get **summed** within the same axis intervals — typical financial-chart setup.

Per-series aggregation has higher priority than axis-level.

## Custom Aggregate Function

For aggregations not in the enum (median, standard deviation, custom calculations):

1. Set `AggregateFunction="Custom"` on the scale options or series.
2. Subclass `CustomAggregateFunction` and override `Calculate(GroupInfo)`.
3. Assign an instance to the `CustomAggregateFunction` property of the scale options or series.

```csharp
class StandardDeviationAggregate : CustomAggregateFunction {
    public override string ToString() => "StdDev";

    public override double[] Calculate(GroupInfo groupInfo) {
        var values = groupInfo.Values1.ToArray();
        double mean = values.Average();
        double sumSq = values.Sum(v => (v - mean) * (v - mean));
        return new[] { Math.Sqrt(sumSq / values.Length) };
    }
}
```

```xaml
<dxc:AxisX2D.DateTimeScaleOptions>
    <dxc:AutomaticDateTimeScaleOptions AggregateFunction="Custom">
        <dxc:AutomaticDateTimeScaleOptions.CustomAggregateFunction>
            <local:StandardDeviationAggregate/>
        </dxc:AutomaticDateTimeScaleOptions.CustomAggregateFunction>
    </dxc:AutomaticDateTimeScaleOptions>
</dxc:AxisX2D.DateTimeScaleOptions>
```

`GroupInfo` exposes `Values1`, `Values2`, ... (one per value member). For range/financial series, multiple input arrays are available.

## Choosing When to Aggregate

| Data size | Visualization | Approach |
|---|---|---|
| < 1,000 points | Anything | No aggregation needed |
| 1,000–100,000 points | Smooth trend | `AutomaticDateTimeScaleOptions` with `Average` |
| 100,000+ points | Smooth trend, zoomable | Same — automatic recalculates on zoom |
| 100,000+ points | OHLC over time | `Financial` per-series on stock series |
| Bucket counts (histogram) | Histogram | `Interval*ScaleOptions` with `Histogram` function |
| Very large (millions, server-side) | Pre-aggregated buckets | Use summaries instead |

## Common Issues

- **Aggregation looks like it's not doing anything** — used `Continuous*ScaleOptions` (which disables aggregation). Switch to `Automatic*` or `Manual*`.
- **`AggregateFunction` set but values look identical** — y-axis can't aggregate; you must set the function on the x-axis options. Verify the options are on `AxisX2D`, not `AxisY2D`.
- **Aggregation gives wrong sums for stock data** — used `Sum` for OHLC; switch to `Financial`.
- **Bar values shrink unexpectedly** — `Average` instead of `Sum`. For "total per month" bar charts, use `Sum`.
- **Histogram bars overlap or merge** — used `Manual*` instead of `Interval*ScaleOptions`. Histograms need interval options.
- **Aggregation doesn't recompute on zoom** — used `Manual*` (fixed unit) instead of `Automatic*`. Manual sticks to the configured `MeasureUnit` regardless of zoom level.

## Source Material

- `articles/controls-and-libraries/charts-suite/chart-control/data-aggregation.md` (https://docs.devexpress.com/content/WPF/16846?md=true)
- `articles/controls-and-libraries/charts-suite/chart-control/calculate-summaries.md` (https://docs.devexpress.com/content/WPF/400970?md=true)
- `articles/controls-and-libraries/charts-suite/chart-control/histogram.md` (https://docs.devexpress.com/content/WPF/400974?md=true)

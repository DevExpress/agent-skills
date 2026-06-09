# Series Types — 2D Series Inventory

A series is the visual representation of a data set (line, bar, area, pie, candlestick, etc.). DevExpress ships ~30 concrete 2D series classes. Each series has a required **parent diagram type** — for instance, `BarSideBySideSeries2D` only works inside `XYDiagram2D`; `PieSeries2D` only inside `SimpleDiagram2D`. This page is the picker: read across the table, find the data shape and visualization, get the class name and the diagram to put it in.

> **3D series exist** (`SideBySideBarSeries3D`, etc.) but use a separate `Chart3DControl`. They're rarely needed for business apps. This skill covers 2D only.

## When to Use This Reference

Use this when you need to:

- Pick the right series class for your data
- Determine which diagram type to use
- Understand which series can be combined in the same diagram
- Differentiate stacked / full-stacked / spline / step variants

## Diagram-Series Compatibility Matrix

| Diagram | Series families it accepts |
|---|---|
| `XYDiagram2D` | Area, Bar, Financial, Point/Line/Bubble, Box Plot |
| `SimpleDiagram2D` | Pie / Donut / Nested Donut, Funnel |
| `PolarDiagram2D` | Polar series (Area, Line, Point) |
| `RadarDiagram2D` | Radar series (Area, Line, Point) |

You **cannot mix diagrams** — a chart has exactly one diagram. Multi-series within the same diagram is fine as long as each series is compatible.

## XYDiagram2D Series (Cartesian)

### Area Series

| Class | Use |
|---|---|
| `AreaSeries2D` | Single value over a continuous axis, filled to baseline |
| `SplineAreaSeries2D` | Smoothed spline curve, filled to baseline |
| `AreaStepSeries2D` | Step-shaped area (no slope between points) |
| `AreaStackedSeries2D` | Multiple stacked areas — absolute values |
| `SplineAreaStackedSeries2D` | Stacked, smoothed |
| `AreaStepStackedSeries2D` | Stacked, step-shaped |
| `AreaFullStackedSeries2D` | Multiple stacked areas — percentages (0–100%) |
| `SplineAreaFullStackedSeries2D` | Full-stacked, smoothed |
| `AreaStepFullStackedSeries2D` | Full-stacked, step-shaped |
| `RangeAreaSeries2D` | Two value fields (`Value1DataMember`, `Value2DataMember`) — fills between them |

**When to use which area**: continuous time series + cumulative composition → stacked area; percentage breakdown over time → full-stacked area; min/max bands or confidence intervals → range area.

### Bar Series

| Class | Use |
|---|---|
| `BarSideBySideSeries2D` | Classic vertical bar chart; multiple series sit side-by-side per argument |
| `BarSideBySideStackedSeries2D` | Bars per group, stacked within group |
| `BarSideBySideFullStackedSeries2D` | Side-by-side groups, each full-stacked to 100% |
| `BarStackedSeries2D` | Single stack per argument — absolute values |
| `BarFullStackedSeries2D` | Single stack per argument — percentages |
| `RangeBarOverlappedSeries2D` | Two value fields, bars overlap |
| `RangeBarSideBySideSeries2D` | Two value fields, bars side-by-side |
| `WaterfallSeries2D` | Step-by-step contribution to a total (positive/negative bars) |

**When to use which bar**: comparing categories side-by-side → `BarSideBySideSeries2D`; composition within categories → stacked; percentage composition → full-stacked; flow/contribution analysis → waterfall.

### Point, Line, and Bubble

| Class | Use |
|---|---|
| `PointSeries2D` | Scatter points only |
| `LineSeries2D` | Connected line |
| `LineStackedSeries2D` | Multiple lines stacked vertically (absolute) |
| `LineFullStackedSeries2D` | Multiple lines stacked to 100% |
| `LineStepSeries2D` | Step-shaped line |
| `LineScatterSeries2D` | Connected scatter — used for X/Y data where points may not be in order |
| `SplineSeries2D` | Smoothed line |
| `BubbleSeries2D` | Bubbles sized by `WeightDataMember` (third dimension) |

**When to use which**: trend over time → `LineSeries2D`; smoothed trend → `SplineSeries2D`; raw distribution → `PointSeries2D` or `LineScatterSeries2D`; three-dimensional data (X/Y/size) → `BubbleSeries2D`.

### Financial (OHLC) Series

| Class | Use |
|---|---|
| `StockSeries2D` | Open/High/Low/Close bars (vertical line with horizontal ticks) |
| `CandleStickSeries2D` | OHLC candles — body filled or hollow by direction |

Both require `OpenValueDataMember`, `HighValueDataMember`, `LowValueDataMember`, `CloseValueDataMember`.

### Box Plot

| Class | Use |
|---|---|
| `BoxPlotSeries2D` | Min / Q1 / Median / Q3 / Max for each argument; outliers shown as points |

Five value members (`MinValueDataMember`, `Quartile1DataMember`, `MedianValueDataMember`, `Quartile3DataMember`, `MaxValueDataMember`).

## SimpleDiagram2D Series

| Class | Use |
|---|---|
| `PieSeries2D` | Single pie or donut (toggle via `HoleRadiusPercent`) |
| `NestedDonutSeries2D` | Concentric donuts — group hierarchy as nested rings |
| `FunnelSeries2D` | Funnel — typically a sales pipeline from top (largest) to bottom |

**When to use which**: composition of a whole → pie; multi-level composition → nested donut; conversion stages → funnel.

## PolarDiagram2D Series

| Class | Use |
|---|---|
| `PolarPointSeries2D` | Scatter points on a polar (radial) chart |
| `PolarLineSeries2D` | Connected line on a polar chart |
| `PolarAreaSeries2D` | Filled area on a polar chart |

**When to use**: cyclical data (wind direction, hourly patterns, compass bearings).

## RadarDiagram2D Series

| Class | Use |
|---|---|
| `RadarPointSeries2D` | Scatter points on radar (web-style) chart |
| `RadarLineSeries2D` | Connected radar lines |
| `RadarAreaSeries2D` | Filled radar areas |

**When to use**: multi-axis profile comparisons (skill ratings, product feature matrices). Radar charts have N categorical axes radiating from a center.

## Cross-Cutting Series Properties

| Property | Use |
|---|---|
| `DisplayName` | Legend / tooltip caption |
| `ArgumentDataMember` / `ValueDataMember` | Field bindings |
| `ArgumentScaleType` / `ValueScaleType` | `Auto` / `Numerical` / `DateTime` / `TimeSpan` / `Qualitative` |
| `Visible` / `ShowInLegend` | Visibility flags |
| `CheckableInLegend` / `CheckedInLegend` | Legend check-box behavior |
| `MarkerVisible` (line/area series) | Show markers at each point |
| `ColorEach` | Color each point individually (useful for pie/funnel) |
| `LegendTextPattern` / `ToolTipPointPattern` / `CrosshairLabelPattern` | Text formatters with `{A}`, `{V}`, etc. |
| `Brush` / `LineStyle` (line series) | Line color and thickness |
| `AggregateFunction` | Per-series aggregation override |
| `FilterCriteria` / `FilterString` | Per-series data filter |
| `Indicators` (XYSeries2D) | Add overlay indicators (Moving Average, Bollinger Bands, MACD, etc.) |

## Series Compatibility Within One Diagram

Within `XYDiagram2D`, you can mix:

- Any combination of **bar + line + area + point/bubble** that shares the same argument scale type.
- A financial series with a side-by-side bar (e.g., volume bars below a candlestick).
- Box plot can coexist with bar/line.

Cannot mix:

- Stacked + non-stacked of the same family on the same axis (they share a baseline calculation).
- Different families' "full stacked" variants on the same axis.

For radically different value ranges, add a **secondary y-axis** instead of fighting the stacking — see [axes.md](axes.md).

## Quick Picker by Visualization Goal

| Goal | Series |
|---|---|
| Compare categories | `BarSideBySideSeries2D` |
| Show composition | `BarStackedSeries2D` or `PieSeries2D` |
| Show composition as percentages | `BarFullStackedSeries2D`, `AreaFullStackedSeries2D`, or pie with `LegendTextPattern="{}{A}: {VP:p1}"` |
| Trend over time | `LineSeries2D` or `AreaSeries2D` |
| Smoothed trend | `SplineSeries2D` |
| Step-shaped data (rates, levels) | `LineStepSeries2D` or `AreaStepSeries2D` |
| Distribution / scatter | `PointSeries2D` or `LineScatterSeries2D` |
| Three-dimensional (size) | `BubbleSeries2D` |
| Stock prices | `CandleStickSeries2D` |
| Conversion funnel | `FunnelSeries2D` |
| Statistical summary per category | `BoxPlotSeries2D` |
| Contribution to total | `WaterfallSeries2D` |
| Cyclical / radial | Polar or Radar series family |

## Source Material

- `articles/controls-and-libraries/charts-suite/chart-control/series/2d-series-types.md` (https://docs.devexpress.com/content/WPF/114223?md=true)
- `articles/controls-and-libraries/charts-suite/chart-control/series/series-type-compatibility.md`
- `articles/controls-and-libraries/charts-suite/chart-control/series/series.md` (https://docs.devexpress.com/content/WPF/6339?md=true)

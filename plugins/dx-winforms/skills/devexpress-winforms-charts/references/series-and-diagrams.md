# Series and Diagrams (Series View Catalog)

This reference is the catalog of every series view supported by `ChartControl` 2D and 3D, grouped by its parent diagram. Use it to pick the right view for your data shape, and to know which views can coexist on the same chart.

For binding data to a series see [data-binding.md](data-binding.md). For axes (only on `XYDiagram`, `XYDiagram3D`, `SwiftPlotDiagram`) see [axes.md](axes.md).

## When to Use This Reference

- Picking a series view from the 50+ available types.
- Confirming that two series can live on the same chart (only views from the same diagram can).
- Changing a series' view at runtime (`Series.ChangeView`).
- Choosing between 2D and 3D variants of the same chart shape.
- Understanding the diagram-level options (`Rotated`, `Panes`, axes, etc.) that a given view exposes.

## How the Diagram is Picked

`ChartControl.Diagram` is **implicit** — the first added series' `ViewType` decides it. Adding a `Series` with `ViewType.Pie` produces a `SimpleDiagram`; `ViewType.Bar` produces an `XYDiagram`. After that, only series whose `ViewType` belongs to the same diagram family are accepted; others throw an `InvalidOperationException`.

### Compatibility at a glance

| Diagram | Holds | Notable Options |
|---|---|---|
| `XYDiagram` | Bar / Line / Area / Point / Bubble / Financial / Range / BoxPlot families | `AxisX`/`AxisY`, `SecondaryAxesX`/`Y`, `Rotated`, `Panes` |
| `SimpleDiagram` | Pie / Doughnut / NestedDoughnut / Funnel | `Dimension`, `LayoutDirection`, `EqualPieSize` |
| `GanttDiagram` | Side-by-side / Overlapped Gantt | Date-time axis, swimlanes |
| `RadarDiagram` | Radar Point / Line / ScatterLine / Area / RangeArea | `RotationDirection`, `StartAngleInDegrees`, `DrawAxisXAsLine` |
| `PolarDiagram` | Polar Point / Line / ScatterLine / Area / RangeArea | Continuous radial axis, `AxisX.Range` (angles) |
| `SwiftPlotDiagram` | SwiftPlot / SwiftPoint | Optimized for millions of points |
| `SimpleDiagram3D` | 3D Pie / Doughnut | Camera, rotation, light |
| `XYDiagram3D` | 3D Bar / Line / Area / Manhattan Bar | 3D camera, multiple panes not supported |

## `XYDiagram` Series Views

### Bar family

| `ViewType` | View class | Use when |
|---|---|---|
| `Bar` | `BarSeriesView` (or `SideBySideBarSeriesView`) | Compare values across categories. |
| `StackedBar` | `StackedBarSeriesView` | Compare totals AND their composition. |
| `FullStackedBar` | `FullStackedBarSeriesView` | Compare composition only (each bar = 100 %). |
| `SideBySideStackedBar` | `SideBySideStackedBarSeriesView` | Multiple stacked groups side-by-side. |
| `SideBySideFullStackedBar` | `SideBySideFullStackedBarSeriesView` | Multiple 100 %-stacked groups side-by-side. |
| `OverlappedBar` | `OverlappedBarSeriesView` | Two bars overlap in the same slot. |
| `RangeBar` / `SideBySideRangeBar` / `OverlappedRangeBar` | `RangeBarSeriesView` and variants | Show a value *range* (min/max) per category. |
| `Waterfall` | `WaterfallSeriesView` | Show running totals with up/down deltas. |

### Line family

| `ViewType` | View class | Use when |
|---|---|---|
| `Line` | `LineSeriesView` | Trend over a continuous argument. |
| `Spline` | `SplineSeriesView` | Smooth trend (Bezier curve). |
| `StepLine` | `StepLineSeriesView` | Discrete jumps between points. |
| `StackedLine` / `FullStackedLine` / `StackedSpline` / `FullStackedSpline` / `StackedStepLine` / `FullStackedStepLine` | Stacked variants | Total + composition with lines. |
| `ScatterLine` | `ScatterLineSeriesView` | Connects points in **insertion** order (allows looped paths). |

### Area family

| `ViewType` | View class | Use when |
|---|---|---|
| `Area` | `AreaSeriesView` | Volume under a line. |
| `SplineArea` / `StepArea` | Smooth / step variants | |
| `StackedArea` / `FullStackedArea` / `StackedSplineArea` / `FullStackedSplineArea` / `StackedStepArea` / `FullStackedStepArea` | Stacked variants | |
| `RangeArea` / `SplineRangeArea` | `RangeAreaSeriesView` and spline variant | Show value *range* (min/max) over time. |

### Point and Bubble

| `ViewType` | View class | Use when |
|---|---|---|
| `Point` | `PointSeriesView` | Scatter plot — correlation. |
| `Bubble` | `BubbleSeriesView` | Three dimensions (X, Y, weight). |

### Financial

| `ViewType` | View class | Use when |
|---|---|---|
| `Stock` | `StockSeriesView` | OHLC bars (high-low + open tick + close tick). |
| `CandleStick` | `CandleStickSeriesView` | OHLC candles (filled body shows direction). |

Both require 4 value members: `Low`, `High`, `Open`, `Close` (use `Series.SetFinancialDataMembers`).

### Statistical

| `ViewType` | View class | Use when |
|---|---|---|
| `BoxPlot` | `BoxPlotSeriesView` | Show distribution (min/quartiles/median/max + outliers). |

## `SimpleDiagram` Series Views

| `ViewType` | View class | Use when |
|---|---|---|
| `Pie` | `PieSeriesView` | Composition / share of a whole. |
| `Doughnut` | `DoughnutSeriesView` | Same as pie, hollow center allows a label. |
| `NestedDoughnut` | `NestedDoughnutSeriesView` | Two-level hierarchy in concentric rings. |
| `Funnel` | `FunnelSeriesView` | Stages of a pipeline / conversion. |

`SimpleDiagram` has **no axes**. Layout is controlled by `SimpleDiagram.LayoutDirection`, `Dimension` (rows × cols), and `EqualPieSize`.

## `GanttDiagram` Series Views

| `ViewType` | View class | Use when |
|---|---|---|
| `Gantt` (side-by-side) | `SideBySideGanttSeriesView` | Project tasks with separate rows for parallel resources. |
| `OverlappedGantt` | `OverlappedGanttSeriesView` | Tasks share a row (e.g., dependencies). |

`GanttDiagram` automatically uses a `DateTime` X-axis and a `Qualitative` Y-axis (task names).

## `RadarDiagram` and `PolarDiagram` Series Views

| `ViewType` | Diagram | Use when |
|---|---|---|
| `RadarPoint` / `RadarLine` / `RadarScatterLine` / `RadarArea` / `RadarRangeArea` | `RadarDiagram` | Compare multivariate items (Likert profile). |
| `PolarPoint` / `PolarLine` / `PolarScatterLine` / `PolarArea` / `PolarRangeArea` | `PolarDiagram` | Angle-based data (wind direction, phase). |

Radar uses categorical axes; Polar uses a continuous radial + angular axis.

## `SwiftPlotDiagram` Series Views

| `ViewType` | View class | Use when |
|---|---|---|
| `SwiftPlot` | `SwiftPlotSeriesView` | Continuous line for **millions** of points. |
| `SwiftPoint` | `SwiftPointSeriesView` | Scatter for millions of points. |

`SwiftPlotDiagram` is hardware-accelerated and skips per-point hit testing — you trade interactivity for raw throughput.

## 3D Series Views

### `SimpleDiagram3D`

| `ViewType` | View class |
|---|---|
| `Pie3D` | `Pie3DSeriesView` |
| `Doughnut3D` | `Doughnut3DSeriesView` |

### `XYDiagram3D`

| `ViewType` | View class | Notes |
|---|---|---|
| `Bar3D` and Stacked / FullStacked / SideBySide / SideBySideStacked / SideBySideFullStacked / Manhattan variants | `Bar3DSeriesView`, … | Cuboid bars. |
| `Line3D` / `StackedLine3D` / `FullStackedLine3D` | `Line3DSeriesView`, … | Ribbon lines. |
| `Area3D` / `StackedArea3D` / `FullStackedArea3D` | `Area3DSeriesView`, … | Sheet areas. |
| `Surface` | `SurfaceSeriesView` | Wireframe / surface plot of `f(x, y)`. |
| `Step3D` | `Step3DSeriesView` | Step variant of line in 3D. |

**3D charts do not support**: tooltips, crosshair cursor, runtime selection.

## Changing a Series' View

```csharp
series.ChangeView(ViewType.Spline);
```

Replaces `Series.View` with a fresh `SplineSeriesView` (default settings). Use this when you have a generic "show me this data" handler that lets the user pick a chart type at runtime. `ChangeView` throws if the target view is incompatible with the current diagram.

To keep view settings, set `Series.View = new SplineSeriesView { ... }` directly.

## Per-View Settings (`Series.View`)

Each view exposes its own appearance API. Examples:

```csharp
// BarSeriesView — bar width
((BarSeriesView)series.View).BarWidth = 0.6;
((BarSeriesView)series.View).Transparency = 64;

// LineSeriesView — line + markers
((LineSeriesView)series.View).LineMarkerOptions.Kind = MarkerKind.Triangle;
((LineSeriesView)series.View).LineMarkerOptions.Size = 12;

// PieSeriesView — runtime explosion
var pie = (PieSeriesView)series.View;
pie.ExplodeMode = PieExplodeMode.UseFilters;
pie.ExplodedDistancePercentage = 30;

// CandleStickSeriesView
var c = (CandleStickSeriesView)series.View;
c.ReductionOptions.Visible = true;
c.ReductionOptions.Level = StockLevel.Close;
c.ReductionOptions.Color = Color.IndianRed;
c.LevelLineLength = 0.5;
```

## Recommended View by Data Shape

| Data shape | First-choice view | Alternatives |
|---|---|---|
| Compare values across categories | `Bar` | `Line` (sequence), `Point` (no order) |
| Trend over time | `Line` / `Spline` | `Area`, `StepLine` |
| Composition of one total | `Pie` / `Doughnut` | `FullStackedBar` (compares two totals) |
| Composition + total comparison | `StackedBar` | `StackedArea` |
| Correlation between two metrics | `Point` (scatter) | `Bubble` (add a 3rd dimension) |
| Range over time (min/max) | `RangeArea` | `RangeBar` |
| OHLC | `CandleStick` | `Stock` |
| Distribution | `BoxPlot` | `Histogram` (via `AggregateFunction.Histogram`) |
| Pipeline / funnel | `Funnel` | `Bar` (sorted) |
| Multivariate profile | `RadarLine` | `RadarArea` |
| Schedule | `Gantt` | |
| Huge dataset (> 100 k points) | `SwiftPlot` | `SwiftPoint` |

## Common Patterns

### Pattern 1 — Two compatible series on one chart

```csharp
chart.Series.Add(new Series("Revenue", ViewType.Bar));
chart.Series.Add(new Series("Forecast", ViewType.Line));   // both XYDiagram → OK
```

### Pattern 2 — Pick view at runtime

```csharp
ViewType pick = chartTypeCombo.SelectedItem switch {
    "Bar"    => ViewType.Bar,
    "Spline" => ViewType.Spline,
    "Area"   => ViewType.Area,
    _        => ViewType.Bar
};
series.ChangeView(pick);
```

### Pattern 3 — Manhattan 3D bars

```csharp
var s = new Series("North", ViewType.ManhattanBar);
chart.Series.Add(s);                    // diagram becomes XYDiagram3D
```

### Pattern 4 — Switch to SwiftPlot for huge data

```csharp
chart.Series.Clear();
chart.Series.Add(new Series("Telemetry", ViewType.SwiftPlot));
```

## Common Issues

- **`InvalidOperationException` when adding series** — series view doesn't match the current diagram. Use a compatible view or clear `chart.Series`.
- **`Series.ChangeView` doesn't update colors** — `ChangeView` resets the view; reapply view-specific settings afterward.
- **Pie chart shows multiple circles** — `SimpleDiagram` lays out one circle *per series*. To show one pie with multiple slices, use **one series** with multiple points.
- **3D chart looks flat** — set `XYDiagram3D.RotationMatrix` or use `XYDiagram3D.PerspectiveAngle` to tilt the camera.
- **SwiftPlot points don't react to clicks** — by design; SwiftPlot skips hit testing for performance.

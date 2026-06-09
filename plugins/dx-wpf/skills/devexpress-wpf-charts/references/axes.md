# Axes — Primary, Secondary, and Scale Types

`XYDiagram2D` has two primary axes (`AxisX2D`, `AxisY2D`) and accepts an unlimited number of secondary axes (`SecondaryAxisX2D`, `SecondaryAxisY2D`). Each axis has a **scale type** — `Numerical`, `DateTime`, `TimeSpan`, or `Qualitative` — that determines how arguments/values are interpreted and how grid intervals work. Scale-type detection is automatic by default; explicit settings buy performance and predictability.

## When to Use This Reference

Use this when you need to:

- Configure the primary x-axis or y-axis (`AxisX2D` / `AxisY2D`)
- Add a secondary axis for series with a different value range
- Set scale options (Numerical, DateTime, TimeSpan, Qualitative)
- Configure grid spacing and alignment
- Enable a logarithmic scale
- Rotate the diagram (swap horizontal/vertical orientation)

## Axis Anatomy

```
XYDiagram2D
├── AxisX                — primary x-axis (AxisX2D)
├── AxisY                — primary y-axis (AxisY2D)
├── SecondaryAxesX       — collection of SecondaryAxisX2D
└── SecondaryAxesY       — collection of SecondaryAxisY2D
```

Each axis has:

- **Scale Options** — one of `NumericScaleOptions`, `DateTimeScaleOptions`, `TimeSpanScaleOptions`, or `QualitativeScaleOptions`. Determines unit, grid, aggregation.
- **`Title`** — `AxisTitle` (see [axis-titles-and-labels.md](axis-titles-and-labels.md))
- **`Label`** — `AxisLabel` for text formatting (see [axis-titles-and-labels.md](axis-titles-and-labels.md))
- **`WholeRange`** / **`VisualRange`** — data extent and viewport
- **`GridLinesVisible`**, **`Interlaced`**, **`TickmarksVisible`** — visual elements
- **`Logarithmic`**, **`LogarithmicBase`** — log scale toggle (numeric only)

## Scale Type Detection — Automatic vs Explicit

By default, the chart inspects series data to pick the scale type. `Series.ArgumentScaleType` defaults to `Auto`. This works fine for small data but costs CPU/RAM on large sets.

**Set the scale type explicitly when you know it**:

```xaml
<dxc:LineSeries2D ArgumentScaleType="DateTime"
                  ValueScaleType="Numerical"
                  ArgumentDataMember="Date"
                  ValueDataMember="Price"/>
```

| `ScaleType` | Use for |
|---|---|
| `Auto` | Default — chart detects type by scanning data |
| `Numerical` | `int` / `double` / `decimal` arguments or values |
| `DateTime` | `DateTime` arguments (timestamps, dates) |
| `TimeSpan` | `TimeSpan` arguments (durations) |
| `Qualitative` | `string` category arguments (x-axis only) |

## Scale Options by Axis and Scale Type

Each scale type has a corresponding `ScaleOptions` family. Assign one to the axis to configure aggregation, measurement unit, and grid behavior.

| Scale type | X-axis options (4 variants) | Y-axis (continuous only) |
|---|---|---|
| Numerical | `AutomaticNumericScaleOptions`, `ManualNumericScaleOptions`, `ContinuousNumericScaleOptions`, `IntervalNumericScaleOptions` | `ContinuousNumericScaleOptions` |
| DateTime | `AutomaticDateTimeScaleOptions`, `ManualDateTimeScaleOptions`, `ContinuousDateTimeScaleOptions`, `IntervalDateTimeScaleOptions` | `ContinuousDateTimeScaleOptions` |
| TimeSpan | `AutomaticTimeSpanScaleOptions`, `ManualTimeSpanScaleOptions`, `ContinuousTimeSpanScaleOptions`, `IntervalTimeSpanScaleOptions` | `ContinuousTimeSpanScaleOptions` |
| Qualitative | `QualitativeScaleOptions` (x-axis only) | n/a |

> **Y-axes support only continuous scale options.** Manual / Automatic / Interval scale options apply to x-axes only. The y-axis can't aggregate.

### What Each Variant Means

| Variant | Behavior |
|---|---|
| `Automatic*` | Chart picks the measurement unit based on data range, control size, zoom level. Aggregation can be on (`AggregateFunction != None`). |
| `Manual*` | You set `MeasureUnit` (Day / Month / Year, or a number for numeric); aggregation can be on. |
| `Continuous*` | No aggregation. Every data point is plotted as-is. Required for y-axes. |
| `Interval*` | Axis labels show intervals like `(100, 200]`, `(200, 300]`. Used for histograms. |

## Numeric Scale — Examples

### Automatic, with aggregation:

```xaml
<dxc:XYDiagram2D.AxisX>
    <dxc:AxisX2D>
        <dxc:AxisX2D.NumericScaleOptions>
            <dxc:AutomaticNumericScaleOptions AggregateFunction="Average"/>
        </dxc:AxisX2D.NumericScaleOptions>
    </dxc:AxisX2D>
</dxc:XYDiagram2D.AxisX>
```

### Manual, bucket size 100, average per bucket:

```xaml
<dxc:AxisX2D.NumericScaleOptions>
    <dxc:ManualNumericScaleOptions AggregateFunction="Average" MeasureUnit="100"/>
</dxc:AxisX2D.NumericScaleOptions>
```

### Logarithmic y-axis:

```xaml
<dxc:XYDiagram2D.AxisY>
    <dxc:AxisY2D Logarithmic="True" LogarithmicBase="10"/>
</dxc:XYDiagram2D.AxisY>
```

## DateTime Scale — Examples

### Automatic:

```xaml
<dxc:AxisX2D.DateTimeScaleOptions>
    <dxc:AutomaticDateTimeScaleOptions AggregateFunction="Average"/>
</dxc:AxisX2D.DateTimeScaleOptions>
```

### Manual, month-level buckets:

```xaml
<dxc:AxisX2D.DateTimeScaleOptions>
    <dxc:ManualDateTimeScaleOptions MeasureUnit="Month" GridAlignment="Year"/>
</dxc:AxisX2D.DateTimeScaleOptions>
```

### Custom interval — every 6 months:

```xaml
<dxc:ManualDateTimeScaleOptions MeasureUnit="Month" MeasureUnitMultiplier="6"/>
```

Use `MeasureUnit` + `MeasureUnitMultiplier` for custom intervals like quarters, half-years.

## TimeSpan Scale — Examples

```xaml
<dxc:LineSeries2D ArgumentScaleType="TimeSpan" ValueScaleType="TimeSpan"/>

<dxc:AxisX2D.TimeSpanScaleOptions>
    <dxc:ManualTimeSpanScaleOptions MeasureUnit="Hour" AggregateFunction="Average"/>
</dxc:AxisX2D.TimeSpanScaleOptions>
```

## Qualitative Scale — String Categories

Qualitative scale is **x-axis only**. Arguments are arbitrary strings; the chart treats each unique string as a category.

```xaml
<dxc:LineSeries2D ArgumentScaleType="Qualitative" ValueScaleType="Numerical"/>

<dxc:AxisX2D.QualitativeScaleOptions>
    <dxc:QualitativeScaleOptions AggregateFunction="Sum"/>
</dxc:AxisX2D.QualitativeScaleOptions>
```

By default, categories are sorted in the order they first appear in the data. To impose a custom order, assign an `IComparer` to `AxisBase.QualitativeScaleComparer`.

### Forcing all category labels to show

When categories are crowded, the chart hides some labels. To force all:

```xaml
<dxc:AxisX2D.QualitativeScaleOptions>
    <dxc:QualitativeScaleOptions AutoGrid="False" GridSpacing="1"/>
</dxc:AxisX2D.QualitativeScaleOptions>
<dxc:AxisX2D.Label>
    <dxc:AxisLabel>
        <dxc:Axis2D.ResolveOverlappingOptions>
            <dxc:AxisLabelResolveOverlappingOptions AllowHide="False"
                                                    AllowRotate="True"
                                                    AllowStagger="True"/>
        </dxc:Axis2D.ResolveOverlappingOptions>
    </dxc:AxisLabel>
</dxc:AxisX2D.Label>
```

## Primary Axes — Inline Definition

```xaml
<dxc:XYDiagram2D>
    <dxc:XYDiagram2D.AxisX>
        <dxc:AxisX2D>
            <dxc:AxisX2D.Title>
                <dxc:AxisTitle Content="Date"/>
            </dxc:AxisX2D.Title>
        </dxc:AxisX2D>
    </dxc:XYDiagram2D.AxisX>
    <dxc:XYDiagram2D.AxisY>
        <dxc:AxisY2D>
            <dxc:AxisY2D.Title>
                <dxc:AxisTitle Content="Price (USD)"/>
            </dxc:AxisY2D.Title>
        </dxc:AxisY2D>
    </dxc:XYDiagram2D.AxisY>

    <dxc:LineSeries2D ArgumentDataMember="Date" ValueDataMember="Price"
                      ArgumentScaleType="DateTime"/>
</dxc:XYDiagram2D>
```

If you don't customize an axis, you can omit the `<AxisX>` / `<AxisY>` blocks entirely — the chart creates defaults.

## Secondary Axes — Multiple Value Ranges

When two series have very different value ranges (e.g., revenue in millions vs conversion rate as a percentage), bind one to a secondary y-axis.

```xaml
<dxc:XYDiagram2D>
    <!-- Primary y-axis used by Revenue (default) -->
    <dxc:BarSideBySideSeries2D DisplayName="Revenue"
                               ArgumentDataMember="Month"
                               ValueDataMember="Revenue"/>

    <!-- Conversion is bound to the secondary axis -->
    <dxc:LineSeries2D DisplayName="Conversion %"
                      ArgumentDataMember="Month"
                      ValueDataMember="Conversion"
                      AxisY="{Binding ElementName=convAxis}"/>

    <dxc:XYDiagram2D.SecondaryAxesY>
        <dxc:SecondaryAxisY2D x:Name="convAxis" Alignment="Far">
            <dxc:SecondaryAxisY2D.Title>
                <dxc:AxisTitle Content="Conversion (%)"/>
            </dxc:SecondaryAxisY2D.Title>
        </dxc:SecondaryAxisY2D>
    </dxc:XYDiagram2D.SecondaryAxesY>
</dxc:XYDiagram2D>
```

`Alignment="Far"` places the secondary axis on the right. `Near` places it on the left (overlapping the primary axis is rarely desired).

Use `XYSeries2D.AxisX` / `XYSeries2D.AxisY` to bind a series to any specific axis. Without this, the series uses the primary axes.

### Multiple Secondary Axes

Both `SecondaryAxesX` and `SecondaryAxesY` are collections — add as many as needed.

### Secondary Axes from a ViewModel

For MVVM-driven axes:

```xaml
<dxc:XYDiagram2D SecondaryAxisYItemsSource="{Binding YAxes}">
    <dxc:XYDiagram2D.SecondaryAxisYItemTemplate>
        <DataTemplate>
            <dxc:SecondaryAxisY2D Alignment="Far">
                <dxc:SecondaryAxisY2D.Title>
                    <dxc:AxisTitle Content="{Binding Title}"/>
                </dxc:SecondaryAxisY2D.Title>
            </dxc:SecondaryAxisY2D>
        </DataTemplate>
    </dxc:XYDiagram2D.SecondaryAxisYItemTemplate>
</dxc:XYDiagram2D>
```

Properties: `SecondaryAxisXItemsSource`, `SecondaryAxisYItemsSource`, `SecondaryAxisXItemTemplate`, `SecondaryAxisYItemTemplate`, plus their `*TemplateSelector` variants.

## Axis Visibility

| Property | Use |
|---|---|
| `Axis2D.Visible` | Show / hide an axis entirely |
| `Axis2D.VisibilityInPanes` | Limit visibility to specific panes in multi-pane charts |

## Rotated Diagrams (Swap X / Y)

```xaml
<dxc:XYDiagram2D Rotated="True">
    <dxc:BarSideBySideSeries2D ArgumentDataMember="Country" ValueDataMember="Sales"/>
</dxc:XYDiagram2D>
```

When `Rotated="True"`, the x-axis becomes vertical and the y-axis horizontal — the classic "horizontal bar chart" look. Argument is still on `ArgumentDataMember`; only the orientation flips.

## Grid Spacing and Alignment

Use **`GridSpacing`** (interval count) and **`GridAlignment`** (anchor unit) to control major tick intervals:

```xaml
<dxc:AxisX2D.NumericScaleOptions>
    <dxc:ManualNumericScaleOptions AutoGrid="False" GridSpacing="500"/>
</dxc:AxisX2D.NumericScaleOptions>
```

**Always disable `AutoGrid` before setting `GridSpacing`** — otherwise the chart auto-overrides your value.

For date-time/time-span axes:

```xaml
<dxc:ManualDateTimeScaleOptions AutoGrid="False" GridAlignment="Year"/>
```

`GridAlignment` snaps gridlines to the specified date-time unit boundary (e.g., year start).

## Whole Range vs Visual Range

- **`WholeRange`** — the full extent of the data on this axis. Affects zoom limits.
- **`VisualRange`** — the currently visible portion (changes during zoom/scroll).

To force a specific viewport:

```xaml
<dxc:AxisY2D>
    <dxc:AxisY2D.VisualRange>
        <dxc:Range MinValue="0" MaxValue="100"/>
    </dxc:AxisY2D.VisualRange>
</dxc:AxisY2D>
```

To force the y-axis to always include 0:

```xaml
<dxc:Range dxc:AxisY2D.AlwaysShowZeroLevel="True"/>
```

## Common Issues

- **X-axis treats dates as strings** — `ArgumentScaleType` is `Auto` and the data field is `string`-typed. Change the field to `DateTime` or set `ArgumentScaleType="DateTime"`.
- **Y-axis won't accept `ManualNumericScaleOptions`** — y-axes only accept continuous scale options. Move aggregation/manual settings to the x-axis.
- **Logarithmic scale shows nothing** — data contains zero or negative values, which can't be plotted on log scale. Filter them or shift the range.
- **Categories appear in wrong order** — qualitative axis defaults to first-appearance order. Assign a custom `IComparer` to `QualitativeScaleComparer`.
- **Secondary axis appears on the wrong side** — `Alignment="Near"` (left) instead of `Far` (right). Switch.
- **Series follows the wrong axis** — `XYSeries2D.AxisY` not set; the series uses the primary axis by default. Set `AxisY="{Binding ElementName=secondaryY}"`.
- **Date axis shows too many ticks (daily)** — default `MeasureUnit="Day"`. Set `ManualDateTimeScaleOptions.MeasureUnit="Month"` or use `AutomaticDateTimeScaleOptions`.

## Source Material

- `articles/controls-and-libraries/charts-suite/chart-control/axes/axes.md` (https://docs.devexpress.com/content/WPF/6334?md=true)
- `articles/controls-and-libraries/charts-suite/chart-control/axes/primary-and-secondary-axes.md` (https://docs.devexpress.com/content/WPF/7845?md=true)
- `articles/controls-and-libraries/charts-suite/chart-control/axes/axis-scale-types.md` (https://docs.devexpress.com/content/WPF/115179?md=true)
- `articles/controls-and-libraries/charts-suite/chart-control/axes/whole-and-visual-ranges.md` (https://docs.devexpress.com/content/WPF/115158?md=true)
- `articles/controls-and-libraries/charts-suite/chart-control/axes/axis-layout-and-appearance.md` (https://docs.devexpress.com/content/WPF/115165?md=true)

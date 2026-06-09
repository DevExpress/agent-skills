# Tooltip and Crosshair Cursor

The Chart Control has two interactive hover mechanisms: **tooltip** (a popup attached to a hovered point) and **crosshair cursor** (intersecting lines + a label that follows the mouse). Both work in any 2D diagram and can be active simultaneously, though most apps pick one as the primary readout. Tooltips are simpler and ideal for single-point information; crosshairs excel at cross-series comparison.

## When to Use This Reference

Use this when you need to:

- Enable / disable tooltips per chart or per series
- Customize tooltip text via `ToolTipPointPattern` and templates
- Choose tooltip position (mouse / relative / free)
- Enable / disable the crosshair cursor
- Configure crosshair labels, value/argument lines
- Set crosshair snap modes (nearest argument vs nearest value)
- Format crosshair text with `CrosshairLabelPattern`

## Tooltip

### Enable Tooltips

```xaml
<dxc:ChartControl ToolTipEnabled="True">
    ...
</dxc:ChartControl>
```

Per-series opt-out:

```xaml
<dxc:LineSeries2D ToolTipEnabled="False"/>
```

> For series that show markers (line, spline, point), tooltips appear when **markers are visible** — set `LineSeries2D.MarkerVisible="True"` if the series style hides markers.
>
> The **crosshair cursor is enabled by default**. If you only want tooltips, set `ChartControlBase.CrosshairEnabled="False"`.

### Show Tooltip for Series (Series-Level Info, Not Point-Level)

```xaml
<dxc:ChartControl ToolTipEnabled="True">
    <dxc:ChartControl.ToolTipOptions>
        <dxc:ToolTipOptions ShowForSeries="True"
                            ShowForPoints="True"/>
    </dxc:ChartControl.ToolTipOptions>
</dxc:ChartControl>
```

| Property | Effect |
|---|---|
| `ToolTipOptions.ShowForSeries` | Show a tooltip about the series when hovering its area/line |
| `ToolTipOptions.ShowForPoints` | Show point-level tooltips |

Use the series-level tooltip alongside `Series.ToolTipSeriesPattern` to display series names — handy when there are many series.

### Format Tooltip Text

`Series.ToolTipPointPattern` controls per-point tooltip text. Patterns combine placeholders, format specifiers, and plain text.

```xaml
<dxc:BarSideBySideSeries2D ToolTipPointPattern="{}{A}: ${V:F2}M"/>
```

For series-level tooltips, use `Series.ToolTipSeriesPattern`. Plain text only is allowed beyond the `{S}` placeholder.

### Placeholders

| Placeholder | Use |
|---|---|
| `{A}` | Series point argument |
| `{V}` | Series point value |
| `{VP}` | Value as percentage (Pie, Full-Stacked) |
| `{S}` | Series name (`Series.Name`) |
| `{G}` | Stacked-group name |
| `{W}` | Weight (Bubble series) |
| `{V1}`, `{V2}`, `{VD}` | Range series (first / second / duration) |
| `{HV}`, `{LV}`, `{OV}`, `{CV}` | Financial series (High / Low / Open / Close) |
| `{HINT}` | The `Series.ToolTipHint` content |

Combine with .NET format specifiers: `{V:F2}` (2 decimals), `{A:yyyy-MM-dd}` (date), `{V:p1}` (percent).

> Always start XAML patterns with `{}` to escape the leading brace.

### Tooltip Hint

A hint is extra content attached to the tooltip — typically a label, button, or icon. Place any object inside via `Series.ToolTipHint` or `SeriesPoint.ToolTipHint`.

```xaml
<dxc:LineSeries2D ToolTipPointPattern="{}{HINT}&#x0a;{A}: {V:F2}">
    <dxc:LineSeries2D.ToolTipHint>
        <Image Source="/Images/line-logo.png" Width="20"/>
    </dxc:LineSeries2D.ToolTipHint>
</dxc:LineSeries2D>
```

Reference `{HINT}` in the pattern to embed the hint in the tooltip text flow.

### Tooltip Template

For full visual control:

```xaml
<dxc:LineSeries2D>
    <dxc:LineSeries2D.ToolTipPointTemplate>
        <DataTemplate>
            <Border Background="White" BorderBrush="Gray" BorderThickness="1" Padding="6">
                <StackPanel>
                    <TextBlock Text="{Binding SeriesPoint.Argument}" FontWeight="Bold"/>
                    <TextBlock Text="{Binding SeriesPoint.Value, StringFormat='Value: {0:F2}'}"/>
                </StackPanel>
            </Border>
        </DataTemplate>
    </dxc:LineSeries2D.ToolTipPointTemplate>
</dxc:LineSeries2D>
```

`Series.ToolTipPointTemplate` — point-level template. `Series.ToolTipSeriesTemplate` — series-level template. The `DataContext` exposes `SeriesPoint`, `Series`, and pattern-resolved properties.

### Tooltip Position

`ToolTipOptions.ToolTipPosition` accepts one of three position modes:

| Mode | Behavior |
|---|---|
| `ToolTipMousePosition` | Tooltip follows the mouse cursor (default) |
| `ToolTipRelativePosition` | Tooltip anchors to the hovered point (e.g., top of the bar) |
| `ToolTipFreePosition` | Tooltip is fixed to a corner of a dock target |

```xaml
<dxc:ToolTipOptions>
    <dxc:ToolTipOptions.ToolTipPosition>
        <dxc:ToolTipMousePosition Location="TopLeft"/>
    </dxc:ToolTipOptions.ToolTipPosition>
</dxc:ToolTipOptions>
```

`Location` and `Offset` further tune the placement for the mouse / relative modes.

### Tooltip Beak, Shadow, Behavior

```xaml
<dxc:ChartControl.ToolTipController>
    <dxc:ChartToolTipController ShowBeak="False"
                                ShowShadow="False"
                                OpenMode="OnHover"
                                CloseOnClick="True"
                                AutoPopDelay="0:0:5"
                                InitialDelay="0:0:0.2"/>
</dxc:ChartControl.ToolTipController>
```

| Property | Use |
|---|---|
| `ShowBeak` | Show the small arrow pointing to the data point |
| `ShowShadow` | Drop shadow under the tooltip |
| `OpenMode` | When the tooltip appears (e.g., `OnHover`, `OnClick`) |
| `CloseOnClick` | Hide tooltip on click |
| `InitialDelay` | Hover time before tooltip appears |
| `AutoPopDelay` | Time before tooltip auto-hides |

## Crosshair Cursor

### Enable / Disable

Crosshair is **on by default**. Disable globally:

```xaml
<dxc:ChartControl CrosshairEnabled="False"/>
```

Enable per-series after disabling globally:

```xaml
<dxc:ChartControl CrosshairEnabled="False">
    <dxc:XYDiagram2D>
        <dxc:LineSeries2D x:Name="s1"/>
        <dxc:LineSeries2D x:Name="s2" CrosshairEnabled="True"/>
    </dxc:XYDiagram2D>
</dxc:ChartControl>
```

### Crosshair Elements

The crosshair has five visual parts; each can be toggled independently:

```xaml
<dxc:ChartControl.CrosshairOptions>
    <dxc:CrosshairOptions ShowCrosshairLabels="True"
                          ShowArgumentLabels="True"
                          ShowArgumentLine="True"
                          ShowValueLabels="True"
                          ShowValueLine="True"/>
</dxc:ChartControl.CrosshairOptions>
```

| Property | What it shows |
|---|---|
| `ShowCrosshairLabels` | The popup label with argument + values |
| `ShowArgumentLabels` | Label of the argument at the x-axis edge |
| `ShowArgumentLine` | Vertical line at the cursor's argument |
| `ShowValueLabels` | Label of the value at the y-axis edge |
| `ShowValueLine` | Horizontal line at the highlighted point's value |

By default, only the crosshair label and argument line are visible.

### Format Crosshair Text

`XYSeries2D.CrosshairLabelPattern` controls per-series crosshair text:

```xaml
<dxc:LineSeries2D CrosshairLabelPattern="{}{S}: ${V:F2}M"/>
```

Same placeholder set as tooltips. Plus indicator-specific placeholders (`{I}`, `{AV}`, `{LV}`, `{UV}`, `{T}`, `{B}`, `{SV}`) for indicator crosshair labels.

Group header (when multiple series share one crosshair label):

```xaml
<dxc:CrosshairOptions GroupHeaderPattern="{}{A:MMMM d}"
                      ShowGroupHeaders="True"/>
```

Axis crosshair labels (the ones at the axis edges):

```xaml
<dxc:AxisY2D.CrosshairAxisLabelOptions>
    <dxc:CrosshairAxisLabelOptions Pattern="{}{V:.##}"/>
</dxc:AxisY2D.CrosshairAxisLabelOptions>
```

### Snap Modes

`CrosshairOptions.SnapMode` controls which points the crosshair highlights:

| Mode | Behavior |
|---|---|
| `NearestArgument` (default) | Highlights points with the nearest x-axis position to the cursor |
| `NearestValue` | Highlights points with the nearest y value to the cursor |

```xaml
<dxc:CrosshairOptions SnapMode="NearestValue"/>
```

### Crosshair Lines Mode

| `LinesMode` | Behavior |
|---|---|
| `Auto` (default) | Lines pass through highlighted points |
| `Free` | Lines intersect at the mouse cursor regardless of points |

```xaml
<dxc:CrosshairOptions LinesMode="Free"/>
```

### Label Position

`CrosshairOptions.CommonLabelPosition` accepts:

- **`CrosshairMousePosition`** (default) — label follows the mouse, with `Offset`
- **`CrosshairFreePosition`** — label fixed to a corner of a dock target (e.g., top-left of the chart)

```xaml
<dxc:CrosshairOptions>
    <dxc:CrosshairOptions.CommonLabelPosition>
        <dxc:CrosshairFreePosition DockCorner="TopLeft"
                                   DockTarget="{Binding ElementName=chart}"
                                   Offset="12,12"/>
    </dxc:CrosshairOptions.CommonLabelPosition>
</dxc:CrosshairOptions>
```

### Show Crosshair Content in the Legend

For dashboards where the legend doubles as a status panel:

```xaml
<dxc:ChartControl.CrosshairOptions>
    <dxc:CrosshairOptions ContentShowMode="Legend"/>
</dxc:ChartControl.CrosshairOptions>
<dxc:ChartControl.Legends>
    <dxc:Legend Orientation="Vertical"
                MaxCrosshairContentWidth="50"
                MaxCrosshairContentHeight="20"/>
</dxc:ChartControl.Legends>
```

### Individual Labels per Series (instead of a Common Label)

```xaml
<dxc:CrosshairOptions CrosshairLabelMode="ShowForEachSeries"/>
```

| `CrosshairLabelMode` | Behavior |
|---|---|
| `ShowCommonForAllSeries` (default) | One label aggregates all series |
| `ShowForEachSeries` | One label per series, attached to the highlighted point |
| `ShowForNearestSeries` | Single label, but only for the series nearest the cursor |

### Style the Crosshair

```xaml
<dxc:CrosshairOptions ArgumentLineBrush="Orange"
                      ValueLineBrush="DarkGray"
                      ShowValueLine="True">
    <dxc:CrosshairOptions.ArgumentLineStyle>
        <dxc:LineStyle Thickness="2">
            <dxc:LineStyle.DashStyle>
                <DashStyle Dashes="2 5"/>
            </dxc:LineStyle.DashStyle>
        </dxc:LineStyle>
    </dxc:CrosshairOptions.ArgumentLineStyle>
</dxc:CrosshairOptions>
```

Templates for full appearance control:

- `CrosshairOptions.PopupTemplate` — the whole crosshair label container
- `XYSeries2D.CrosshairLabelTemplate` — individual series rows in the crosshair label
- `Axis2D.CrosshairLabelTemplate` — the labels at axis edges

### Out-of-Range Points

Include points outside the current visual range in the crosshair label:

```xaml
<dxc:CrosshairOptions ShowOutOfRangePoints="True"/>
```

### Show Only in Focused Pane (Multi-Pane Charts)

```xaml
<dxc:CrosshairOptions ShowOnlyInFocusedPane="True"/>
```

### Show Crosshair Programmatically

```csharp
private void OnChartMouseUp(object sender, MouseButtonEventArgs e) {
    if (chartControl.Diagram is XYDiagram2D xy)
        xy.ShowCrosshair(e.GetPosition(chartControl));
}
```

Pass `Point.Empty` (i.e., `new Point()`) to hide it.

## Tooltip vs Crosshair — Picking One

| Use Tooltip | Use Crosshair |
|---|---|
| User wants quick info on one specific point | User wants to compare points across series at the same argument |
| Discrete categories (bar chart, pie) | Continuous data (line, area, stock, time-series) |
| Sparse data, few points | Dense data, many points |
| Need rich content (custom templates, images) | Need precise axis readouts at the cursor |

Both can be active at once; usually the chart looks cleaner with one.

## Common Issues

- **Tooltip doesn't appear on a line series** — `LineSeries2D.MarkerVisible="False"`. Tooltips require visible markers. Either show markers or rely on the crosshair instead.
- **Crosshair shows only argument label** — defaults hide value labels/lines. Enable `ShowValueLabels` / `ShowValueLine` etc. on `CrosshairOptions`.
- **Pattern shows literal `{V}`** — missing `{}` escape. Use `CrosshairLabelPattern="{}{V}"`, not `"{V}"`.
- **Crosshair shows both tooltip and crosshair** (visual clutter) — disable one: `ToolTipEnabled="False"` or `CrosshairEnabled="False"`.
- **Snap mode highlights wrong points** — default `NearestArgument` aligns with x; switch to `NearestValue` for scatter-style charts.
- **Crosshair labels overlap in dense charts** — use `CrosshairLabelMode.ShowForNearestSeries`, or move the label to a free position via `CrosshairFreePosition`.
- **Group header missing** — default behavior groups multi-series labels. Disable: `ShowGroupHeaders="False"`. Or format: `GroupHeaderPattern="{}{A:MMM d}"`.

## Source Material

- `articles/controls-and-libraries/charts-suite/chart-control/tooltip-and-crosshair-cursor/tooltip.md` (https://docs.devexpress.com/content/WPF/11975?md=true)
- `articles/controls-and-libraries/charts-suite/chart-control/tooltip-and-crosshair-cursor/crosshair-cursor.md` (https://docs.devexpress.com/content/WPF/14682?md=true)
- `articles/controls-and-libraries/charts-suite/chart-control/tooltip-and-crosshair-cursor/tooltip-and-crosshair-cursor.md` (https://docs.devexpress.com/content/WPF/11974?md=true)

# Legend

This reference covers the chart legend(s): the default `ChartControl.Legend`, the `ChartControl.Legends` collection for additional legends, alignment and layout options, the `LegendTextPattern` placeholder for per-series item text, legend titles, and check-box markers.

For tooltip / crosshair content formatting see [tooltips-and-crosshair.md](tooltips-and-crosshair.md).

## When to Use This Reference

- Positioning the legend inside or outside the plot area.
- Adding more than one legend (e.g., one per pane).
- Formatting legend item text via `SeriesBase.LegendTextPattern`.
- Showing check-box markers so the user can toggle series visibility.
- Adding a legend title and customizing fonts / colors.
- Using indicator legends (financial indicators get their own legend entry).

## Legend Model

| Member | Purpose |
|---|---|
| `ChartControl.Legend` | The default legend (always present). |
| `ChartControl.Legends` | Collection of additional `Legend` instances. |
| `Series.Legend` | Which legend the series appears in (`null` → default legend). |
| `Series.Visible` | Hides the series and its legend item together. |
| `Series.ShowInLegend` | `true`/`false` — show or hide the series's legend item only (the series stays visible on the diagram). |
| `Series.LegendTextPattern` | Format string for the legend item text. |

## Positioning

Two enum families control where the legend sits relative to the plot area.

```csharp
chart.Legend.AlignmentHorizontal = LegendAlignmentHorizontal.RightOutside;
chart.Legend.AlignmentVertical   = LegendAlignmentVertical.Top;
```

### `LegendAlignmentHorizontal`

| Value | Position |
|---|---|
| `Left` / `Center` / `Right` | Inside the plot area, aligned horizontally. |
| `LeftOutside` / `RightOutside` | Outside the plot area (the diagram shrinks). |

### `LegendAlignmentVertical`

| Value | Position |
|---|---|
| `Top` / `Center` / `Bottom` | Inside the plot area. |
| `TopOutside` / `BottomOutside` | Outside. |

Common combinations:

| Where you want it | Set |
|---|---|
| To the right, outside the plot area | `AlignmentHorizontal = RightOutside`, `AlignmentVertical = Center` |
| Below the plot area | `AlignmentHorizontal = Center`, `AlignmentVertical = BottomOutside` |
| Inside, top-right corner of the plot area | `AlignmentHorizontal = Right`, `AlignmentVertical = Top` |
| Above the plot area, right-aligned | `AlignmentHorizontal = Right`, `AlignmentVertical = TopOutside` |

## Layout — Direction and Limits

| Member | Purpose |
|---|---|
| `Legend.Direction` | `TopToBottom` (single column) / `LeftToRight` (single row) / `BottomToTop` / `RightToLeft`. |
| `Legend.MaxHorizontalPercentage` | Max width as % of chart width (0..100). Wraps items. |
| `Legend.MaxVerticalPercentage` | Max height as % of chart height (0..100). |
| `Legend.EquallySpacedItems` | All items get equal width. Off by default. |
| `Legend.MarkerSize` | Size of the color swatch. |
| `Legend.Padding` | Inner padding (top/left/right/bottom). |
| `Legend.TextOffset` | Gap between marker and text. |
| `Legend.HorizontalIndent` / `VerticalIndent` | Gap from the plot area edge. |
| `Legend.UseCheckBoxes` | Show check boxes that toggle series visibility. |

```csharp
chart.Legend.Direction              = LegendDirection.LeftToRight;
chart.Legend.MaxHorizontalPercentage = 80;
chart.Legend.AlignmentHorizontal    = LegendAlignmentHorizontal.Center;
chart.Legend.AlignmentVertical      = LegendAlignmentVertical.BottomOutside;
```

## Legend Title

```csharp
chart.Legend.Title.Visible    = true;
chart.Legend.Title.Text       = "Regions";
chart.Legend.Title.Font       = new Font("Segoe UI", 10, FontStyle.Bold);
chart.Legend.Title.Alignment  = StringAlignment.Center;
```

Like axis titles, the **title is hidden by default** — set `Visible = true` explicitly. (Unlike axis titles, `LegendTitle.Visible` is a plain `bool`, not a `DefaultBoolean`.)

## Per-Series Text — `LegendTextPattern`

Same placeholders as elsewhere — `{A}`, `{V}`, `{S}`, etc.

```csharp
series.LegendTextPattern = "{S} — total {V:c0}";
```

For a `SeriesTemplate`, set the pattern once and every auto-generated series inherits it.

| Pattern | Renders |
|---|---|
| `{S}` | Series name (default). |
| `{A}` | Argument — useful for pie/funnel where each *point* gets a legend entry. |
| `{V}` | Value. |
| `{VP}` | Percentage (pie/funnel). |
| `{S}: {V:c0}` | `Revenue: $12,300` |

## Pie / Funnel Legends — One Item per Point

For series whose visual elements are individual points (Pie, Doughnut, Funnel), the legend entry per **point** is built from the *point's* `Argument`, and `LegendTextPattern` defaults to `{A}`. Show the value too:

```csharp
pieSeries.LegendTextPattern = "{A}: {VP:p1}";        // "North: 32.5 %"
```

## Multiple Legends

Add additional legends and dock them to specific panes or use them for indicators:

```csharp
var legend2 = new Legend("indicators") {   // the ctor takes the legend Name (there is no Legend(chart) ctor)
    AlignmentHorizontal = LegendAlignmentHorizontal.Right,
    AlignmentVertical   = LegendAlignmentVertical.BottomOutside,
    Title = { Text = "Indicators", Visible = true }
};
chart.Legends.Add(legend2);

// Route a series to this legend
series.Legend = legend2;
```

### Dock a legend to a pane

```csharp
legend2.DockTargetName = "pricePane";    // name of an XYDiagramPane (DockTarget itself is a ChartElement)
```

When `DockTargetName` is set, the legend follows the pane's vertical position.

## Check-Box Markers

```csharp
chart.Legend.UseCheckBoxes = true;
chart.Legend.MarkerMode    = LegendMarkerMode.CheckBoxAndMarker;
```

| `LegendMarkerMode` | Behavior |
|---|---|
| `Marker` | Color marker only (default). |
| `CheckBox` | Only a check box. |
| `CheckBoxAndMarker` | Both. |

Clicking a check box toggles `Series.Visible`. Subscribe to `ChartControl.LegendItemChecked` to react.

## Fine Visual Tweaks

```csharp
chart.Legend.BackColor       = Color.FromArgb(248, 248, 248);
chart.Legend.Border.Visibility = DefaultBoolean.True;
chart.Legend.Border.Color    = Color.LightGray;
chart.Legend.Shadow.Visible  = false;
chart.Legend.Font            = new Font("Segoe UI", 9);
chart.Legend.TextColor       = Color.DimGray;
```

## Hiding a Series From the Legend

```csharp
helperSeries.ShowInLegend = false;               // series visible on the diagram, legend item hidden
```

## Custom Draw — `CustomDrawSeries` (Legend Items)

```csharp
chart.CustomDrawSeries += (s, e) => {
    if (e.Series.Name == "Forecast")
        e.LegendText = "Forecast (projected)";
};
```

`e.LegendText` overrides the legend text for one redraw — useful for runtime annotations.

## Common Patterns

### Pattern 1 — Right side, outside the plot area

```csharp
chart.Legend.AlignmentHorizontal = LegendAlignmentHorizontal.RightOutside;
chart.Legend.AlignmentVertical   = LegendAlignmentVertical.Center;
chart.Legend.Direction           = LegendDirection.TopToBottom;
```

### Pattern 2 — Below, horizontal, full width

```csharp
chart.Legend.AlignmentHorizontal     = LegendAlignmentHorizontal.Center;
chart.Legend.AlignmentVertical       = LegendAlignmentVertical.BottomOutside;
chart.Legend.Direction               = LegendDirection.LeftToRight;
chart.Legend.MaxHorizontalPercentage = 100;
```

### Pattern 3 — Series toggle via check boxes

```csharp
chart.Legend.UseCheckBoxes = true;
chart.Legend.MarkerMode    = LegendMarkerMode.CheckBoxAndMarker;
```

### Pattern 4 — Pie with value + percent

```csharp
pieSeries.LegendTextPattern = "{A}: {V:c0} ({VP:p1})";
```

### Pattern 5 — Two legends (one per pane)

```csharp
chart.Legend.DockTargetName = "revenuePane";

var legend2 = new Legend("tempLegend") {
    AlignmentHorizontal = LegendAlignmentHorizontal.Right,
    AlignmentVertical   = LegendAlignmentVertical.Center,
    DockTargetName      = "temperaturePane"
};
chart.Legends.Add(legend2);
tempSeries.Legend = legend2;
```

## Common Issues

- **Legend hides part of the plot area** — switch from `Right` to `RightOutside` so the diagram shrinks instead of being covered.
- **Legend title invisible** — `Title.Visible = true` is required.
- **`LegendTextPattern` shows literally** — make sure the placeholder name is right and the brace is closed.
- **Pie legend shows series name instead of slice names** — set `LegendTextPattern = "{A}"` (or default; some skin templates override).
- **Adding a `Legend` to `Legends` has no effect** — no series assigned. Set `series.Legend = newLegend`.
- **`LegendItemChecked` doesn't fire** — `UseCheckBoxes` is `false`.
- **Performance drops with many series** — every legend item is rendered separately. Limit `MaxVerticalPercentage` to enable scroll, or hide minor series with `ShowInLegend = false`.

## Source Material

- `articles/controls-and-libraries/chart-control/elements-of-a-chart/legend.md` (`xref:WindowsForms.5409`).
- `articles/controls-and-libraries/chart-control/elements-of-a-chart/legend-formatting-and-layout.md` (`xref:WindowsForms.7398`).
- `articles/controls-and-libraries/chart-control/examples/legend/how-to-display-multiple-legends.md` (`xref:WindowsForms.117585`).
- `api/DevExpress.XtraCharts.Legend.yml`.
- `api/DevExpress.XtraCharts.ChartControl.Legend.yml`.
- `api/DevExpress.XtraCharts.ChartControl.Legends.yml`.
- `api/DevExpress.XtraCharts.SeriesBase.LegendTextPattern.yml`.

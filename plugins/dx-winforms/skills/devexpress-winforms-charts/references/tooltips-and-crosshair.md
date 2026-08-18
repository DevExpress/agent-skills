# Tooltips and Crosshair Cursor

This reference covers the two ways the Chart Control reveals point details on hover: the **Tooltip** (small floating box near the cursor) and the **Crosshair Cursor** (full-height vertical/horizontal lines with labels on the axes and per-series annotations). It also covers which one is the default per diagram and how to format their content.

For axis-label format placeholders see [axis-titles-and-labels.md](axis-titles-and-labels.md).

## When to Use This Reference

- Enabling / disabling tooltip and crosshair on a chart.
- Understanding which one is the default for your diagram.
- Formatting tooltip / crosshair text with `{A}` / `{V}` / `{S}` / `{VP}` placeholders.
- Positioning tooltips, showing the "beak" arrow.
- Configuring crosshair lines, labels, group headers, snap mode.
- Customizing crosshair drawing via the `CustomDrawCrosshair` event.

## Tooltip vs Crosshair — Defaults

| Diagram | Default | Other option |
|---|---|---|
| `XYDiagram` | **Crosshair Cursor** | Tooltip |
| `SwiftPlotDiagram` | **Crosshair Cursor** | Tooltip |
| `SimpleDiagram` (Pie/Doughnut/Funnel) | **Tooltip** | n/a (crosshair makes no sense) |
| `GanttDiagram` | **Tooltip** | n/a |
| `RadarDiagram` / `PolarDiagram` | **Tooltip** | n/a |
| `XYDiagram3D`, `SimpleDiagram3D` | **Neither — 3D doesn't support either** | — |

Do **not** enable both at once on an XYDiagram — you get visual overlap. Disable the one you don't want:

```csharp
chart.CrosshairEnabled = DefaultBoolean.False;       // turn off crosshair
chart.ToolTipEnabled   = DefaultBoolean.True;         // turn on tooltips
```

## Tooltips

### Enable / disable

```csharp
chart.ToolTipEnabled = DefaultBoolean.True;           // chart-wide (ToolTipEnabled is a DefaultBoolean)
series.ToolTipEnabled = DefaultBoolean.False;          // per-series override
```

### Content templates — `ToolTipPointPattern` / `ToolTipSeriesPattern`

```csharp
series.ToolTipPointPattern  = "{S}\n{A:MMM yyyy}: {V:c0}";
series.ToolTipSeriesPattern = "{S} — {SV:c0} total";    // shown on series hover (e.g., pie ring header)
```

`ToolTipPointPattern` is the per-point template (most series). `ToolTipSeriesPattern` is shown when hovering the series visual element (rare; primarily for indicator series).

Use `\n` (or HTML `<br>`) for multi-line tooltips. HTML formatting works in tooltips when `ChartControl.ToolTipController.AllowHtmlText = true` (the default).

### Position

```csharp
chart.ToolTipOptions.ToolTipPosition = new ToolTipMousePosition();   // follows cursor (default)
```

Alternative positioners:

| `ToolTipPosition` | Behavior |
|---|---|
| `ToolTipMousePosition` | Tooltip floats near the cursor (default). |
| `ToolTipRelativePosition` | Offset from the hovered point (`OffsetX` / `OffsetY`). |
| `ToolTipFreePosition` | Docks to a corner of a parent element (`DockCorner`, type `ToolTipDockCorner`) plus an `OffsetX` / `OffsetY`. |

```csharp
// Offset relative to the point under the cursor
var relative = new ToolTipRelativePosition { OffsetX = 2, OffsetY = 2 };
chart.ToolTipOptions.ToolTipPosition = relative;

// Or dock the tooltip to a corner
chart.ToolTipOptions.ToolTipPosition = new ToolTipFreePosition {
    DockCorner = ToolTipDockCorner.TopRight    // TopRight / TopLeft / BottomRight / BottomLeft
};
```

### Tooltip controller

`ChartControl.ToolTipController` is a `DevExpress.Utils.ToolTipController` — same component that drives every DX tooltip in WinForms. Use it to share one tooltip style across the whole app:

```csharp
chart.ToolTipController = sharedToolTipController;
chart.ToolTipController.ShowBeak = true;                              // little arrow toward the point
chart.ToolTipController.ToolTipAnchor = ToolTipAnchor.Object;          // anchor to the point, not cursor
chart.ToolTipController.ToolTipType = DevExpress.Utils.ToolTipType.SuperTip;
```

### Custom tooltip text (patterns)

Tooltip text is driven by **pattern strings** with placeholders — `{A}` argument, `{V}` value, `{S}` series name, `{VP}` percentage:

```csharp
series.ToolTipPointPattern  = "{S}: {A} = {V:c0}";   // per-point tooltip text
series.ToolTipSeriesPattern = "{S}";                  // series-level tooltip text
```

> Note: `CustomDrawSeriesPoint` + `e.LabelText` customizes **point labels** (the text drawn on the diagram), **not** tooltips:
>
> ```csharp
> chart.CustomDrawSeriesPoint += (s, e) => {
>     if ((double)e.SeriesPoint.Values[0] > 1_000_000)
>         e.LabelText = "★ " + e.LabelText;   // affects the on-diagram point label
> };
> ```
>
> For tooltip content use the `ToolTipPointPattern` / `ToolTipSeriesPattern` above (or assign a `SuperTip` via the `ChartControl.ToolTipController`).

## Crosshair Cursor

Crosshair Cursor draws extended axis lines, axis labels at the cursor position, and per-series labels at the nearest data points — all while the mouse moves over the diagram.

### Enable / disable

```csharp
chart.CrosshairEnabled = DefaultBoolean.True;
series.CrosshairEnabled = DefaultBoolean.True;        // per-series
```

### `CrosshairOptions`

```csharp
var co = chart.CrosshairOptions;
co.ShowArgumentLine   = true;
co.ShowValueLine      = true;
co.ShowArgumentLabels = true;
co.ShowValueLabels    = true;
co.ShowCrosshairLabels = true;
co.ShowGroupHeaders   = true;
co.SnapMode           = CrosshairSnapMode.NearestArgument;
co.GroupHeaderPattern = "<b>{A:MMM yyyy}</b>";
```

| Member | Purpose |
|---|---|
| `ShowArgumentLine` / `ShowValueLine` | Toggle the vertical/horizontal crosshair lines. |
| `ShowArgumentLabels` / `ShowValueLabels` | Axis labels at the cursor position. |
| `ShowCrosshairLabels` | The per-series value labels next to the points. |
| `ShowGroupHeaders` | Header line(s) on the per-series label box (argument + series name). |
| `SnapMode` | `CrosshairSnapMode`: `NearestArgument` / `NearestValue`. |
| `CrosshairLabelMode` | `ShowCommonForAllSeries` / `ShowForEachSeries` / `ShowForNearestSeries`. |
| `ContentShowMode` | `Default` / `Header` / `Footer` — where to put the group header. (On a series, the equivalent member is `SeriesBase.CrosshairContentShowMode`.) |
| `ArgumentLineColor` / `ValueLineColor` / `ArgumentLineStyle` / `ValueLineStyle` | Line appearance. |
| `GroupHeaderPattern` | Format string for the group header (uses `{A}`). |
| `GroupHeaderTextColor` / `GroupHeaderFont` | Header appearance. |

### Per-series crosshair label

```csharp
series.CrosshairLabelPattern   = "<b>{S}</b><br>{V:c0}";
series.CrosshairEmptyValueLegendText = "—";                // shown when point is null
```

### Per-axis crosshair label appearance

```csharp
diagram.AxisX.CrosshairAxisLabelOptions.Pattern = "{A:dd MMM yyyy}";
diagram.AxisX.CrosshairAxisLabelOptions.BackColor = Color.LightSteelBlue;
diagram.AxisY.CrosshairAxisLabelOptions.Visibility = DefaultBoolean.False;
```

### `CrosshairLabelMode`

| Value | Behavior |
|---|---|
| `ShowCommonForAllSeries` | One combined label box for all series at the cursor argument. |
| `ShowForEachSeries` | One label per series at the cursor argument. |
| `ShowForNearestSeries` | Only the series under the cursor. |

### Custom draw — `CustomDrawCrosshair`

```csharp
chart.CustomDrawCrosshair += (s, e) => {
    foreach (CrosshairElementGroup group in e.CrosshairElementGroups) {
        foreach (CrosshairElement el in group.CrosshairElements) {
            double v = Convert.ToDouble(el.SeriesPoint.Values[0]);
            if (v < 0)
                el.LabelElement.TextColor = Color.IndianRed;
        }
    }
};
```

`CrosshairElementGroup` corresponds to one argument; its `CrosshairElements` are the per-series points. Each `CrosshairElement` has `LabelElement`, `LineElement`, `MarkerElement` you can mutate.

## Pattern Placeholders (Tooltip + Crosshair)

| Placeholder | Renders |
|---|---|
| `{A}` | Argument (formattable). |
| `{V}` | Value (formattable). |
| `{S}` | Series name. |
| `{SV}` | Series sum (total of values). |
| `{VP}` | Percentage of total. |
| `{V1}`, `{V2}`, … | Specific value when the series has multiple. |
| `{B}`, `{W}` | Beginning value, weight (range/bubble). |
| `<b>…</b>`, `<i>…</i>`, `<br>`, `<color=#RRGGBB>` | Inline HTML allowed. |

```csharp
series.CrosshairLabelPattern = "<b>{S}</b><br>{A:MMM}: <color=#FF7200>{V:c0}</color>";
```

## Common Patterns

### Pattern 1 — Default crosshair, formatted

```csharp
diagram.AxisX.CrosshairAxisLabelOptions.Pattern = "{A:dd MMM yyyy}";
diagram.AxisY.CrosshairAxisLabelOptions.Pattern = "{V:c0}";
chart.CrosshairOptions.GroupHeaderPattern = "<b>{A:dd MMM yyyy}</b>";
chart.Series[0].CrosshairLabelPattern = "{S}: {V:c0}";
```

### Pattern 2 — Switch from crosshair to tooltip on an XYDiagram

```csharp
chart.CrosshairEnabled = DefaultBoolean.False;
chart.ToolTipEnabled   = DefaultBoolean.True;
foreach (Series s in chart.Series)
    s.ToolTipPointPattern = "{S}\n{A:MMM yyyy}: {V:c0}";
```

### Pattern 3 — Crosshair only for the nearest series

```csharp
chart.CrosshairOptions.CrosshairLabelMode = CrosshairLabelMode.ShowForNearestSeries;
chart.CrosshairOptions.SnapMode           = CrosshairSnapMode.NearestValue;
```

### Pattern 4 — HTML-formatted tooltip with `SuperTip`

```csharp
chart.ToolTipController.AllowHtmlText = true;   // ToolTipController.AllowHtmlText is a bool
series.ToolTipPointPattern = "<b>{S}</b><br>{A:MMM yyyy}<br><size=+2><color=#0078D7>{V:c0}</color></size>";
```

### Pattern 5 — Disable crosshair on a single series

```csharp
helperSeries.CrosshairEnabled = DefaultBoolean.False;
```

## Common Issues

- **Neither tooltip nor crosshair appears on a 3D chart** — 3D doesn't support either. Switch to a 2D series view.
- **Both tooltip and crosshair appear at once** — both enabled. Disable one.
- **Crosshair labels show `Series 1` instead of names** — set `Series.Name` and `CrosshairLabelPattern = "{S}: {V}"`.
- **Group header empty** — `ShowGroupHeaders` is `false`, or `GroupHeaderPattern` has only static text without `{A}`.
- **Tooltip wraps awkwardly** — set `chart.ToolTipController.ShowBeak = true` and use `\n` for explicit wraps; or switch to a `SuperTip` for fine layout control.
- **Crosshair lags behind cursor with huge data** — switch to `SwiftPlotDiagram` (still supports crosshair but skips per-point hit testing) or set `SnapMode = NearestArgument` instead of `NearestValue`.
- **`CustomDrawCrosshair` not raised** — `CrosshairEnabled = False`. The event only fires while the cursor is drawn.

## Source Material

- `articles/controls-and-libraries/chart-control/end-user-features/crosshair-cursor.md` (`xref:WindowsForms.114775`).
- `articles/controls-and-libraries/chart-control/end-user-features/tooltips.md` (`xref:WindowsForms.5424`).
- `articles/controls-and-libraries/chart-control/end-user-features/crosshair-cursor-options.md` (`xref:WindowsForms.114776`).
- `articles/controls-and-libraries/chart-control/examples/crosshair/how-to-custom-draw-crosshair-cursor.md` (`xref:WindowsForms.117681`).
- `api/DevExpress.XtraCharts.ChartControl.CrosshairEnabled.yml`.
- `api/DevExpress.XtraCharts.CrosshairOptions.yml`.
- `api/DevExpress.XtraCharts.ChartControl.ToolTipEnabled.yml`.
- `api/DevExpress.XtraCharts.ChartControl.ToolTipOptions.yml`.
- `api/DevExpress.XtraCharts.SeriesBase.ToolTipPointPattern.yml`.
- `api/DevExpress.XtraCharts.SeriesBase.CrosshairLabelPattern.yml`.
- `api/DevExpress.XtraCharts.ChartControl.CustomDrawCrosshair.yml`.

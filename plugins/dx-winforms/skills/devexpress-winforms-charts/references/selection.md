# Selection

This reference covers chart-element selection: enabling selection, choosing what gets selected per click (entire series / single point / all points sharing an argument), the supported selection modes (Single / Multiple / Extended), the mouse / keyboard binding, runtime API to set or clear selection, and customization of the selected appearance.

For the visual model see [series-and-diagrams.md](series-and-diagrams.md). For hit-testing without selection use `ChartControl.CalcHitInfo` (covered in the docs).

## When to Use This Reference

- Letting the user click on series / points and react to the selection.
- Choosing whether a click selects the *series*, a *single point*, or *all points with the same argument*.
- Allowing multi-select with Ctrl / Shift, or rectangle (lasso) selection.
- Highlighting selected items with a custom color.
- Driving selection programmatically.

## Two Independent Selection Knobs

| Property | Decides |
|---|---|
| `ChartControl.SelectionMode` | How many items can be selected at once. |
| `ChartControl.SeriesSelectionMode` | What gets selected per click (granularity). |

```csharp
chart.SelectionMode       = ElementSelectionMode.Single;
chart.SeriesSelectionMode = SeriesSelectionMode.Point;
```

### `ElementSelectionMode`

| Value | Behavior |
|---|---|
| `None` | Selection disabled (default). |
| `Single` | Only one item can be selected. New click replaces the previous selection. |
| `Multiple` | Each click toggles the clicked item; many can be selected. |
| `Extended` | Standard Windows-style: click = single, Ctrl+click = toggle, Shift+click = range. Also enables rectangle selection. |

### `SeriesSelectionMode`

| Value | What clicking a point selects |
|---|---|
| `Series` (default) | The entire `Series` (all points highlight). |
| `Point` | Only the clicked `SeriesPoint`. |
| `Argument` | All points across all series that share the clicked argument (vertical slice). |

```csharp
// Click a bar → highlights all points in all series that share that X value
chart.SeriesSelectionMode = SeriesSelectionMode.Argument;
```

## Mouse / Keyboard Binding — `SelectionOptions`

`SelectionOptions` is a **diagram** property (`XYDiagram` / `SimpleDiagram` expose it), **not** a `ChartControl` member. Cast `chart.Diagram` to access it:

```csharp
using DevExpress.Portable.Input;   // PortableMouseButtons (MouseButton is NOT System.Windows.Forms.MouseButtons)

var diagram = (XYDiagram)chart.Diagram;
diagram.SelectionOptions.MouseAction.MouseButton = PortableMouseButtons.Left;
diagram.SelectionOptions.MouseAction.ModifierKeys = ChartModifierKeys.None;
diagram.SelectionOptions.RectangleSelectionMouseAction.MouseButton = PortableMouseButtons.Left;
diagram.SelectionOptions.RectangleSelectionMouseAction.ModifierKeys = ChartModifierKeys.Shift;
```

| Member | Purpose |
|---|---|
| `MouseAction` | Mouse + modifier-keys combo that triggers selection (`MouseButton` + `ModifierKeys`). |
| `RectangleSelectionMouseAction` | Combo for rectangle (lasso) select in `Extended` mode. |
| `ExtendedModeMouseAction` | Combo to add/remove an item from the selection in `Extended` mode. |
| `RectangleSelectionFillColor` / `RectangleSelectionOutlineColor` | Lasso rectangle visuals. |

> All of the above live on the diagram's `SelectionOptions` — access them via `((XYDiagram)chart.Diagram).SelectionOptions`.

## Events

| Event | When |
|---|---|
| `ChartControl.ObjectSelected` | Single object was selected/deselected (fires once per click). |
| `ChartControl.SelectedItemsChanged` | The collection changed (more granular for `Multiple` / `Extended`). |
| `ChartControl.ObjectHotTracked` | Mouse hovers over an item (hot-track). Independent of selection. |

```csharp
chart.ObjectSelected += (s, e) => {
    if (e.Object is SeriesPoint p) MessageBox.Show($"{p.Argument}: {p.Values[0]}");
    if (e.Object is Series sr)    MessageBox.Show($"Series {sr.Name}");
};

chart.SelectedItemsChanged += (s, e) => {
    statusLabel.Text = $"{chart.SelectedItems.Count} selected";
};
```

### `ChartHitInfo`

`ObjectSelected` / `ObjectHotTracked` provide a `Hot ChartHitInfo` with details:

```csharp
chart.ObjectHotTracked += (s, e) => {
    if (e.HitInfo.InSeriesPoint) Cursor = Cursors.Hand;
    else                          Cursor = Cursors.Default;
};
```

| `HitInfo` flag | Meaning |
|---|---|
| `InSeries` | Hover/click hit a series visual element. |
| `InSeriesPoint` | Hit a specific point (`HitInfo.SeriesPoint`). |
| `InLegend` / `InLegendItem` | Hit the legend area. |
| `InAxis` / `InAxisTitle` / `InAxisLabel` | Hit an axis or its label. |
| `InChartTitle` | Hit a chart title. |

## Working With Selection in Code

### Read

```csharp
foreach (var item in chart.SelectedItems) {
    switch (item) {
        case SeriesPoint p: Console.WriteLine($"Point {p.Argument}={p.Values[0]}"); break;
        case Series s:      Console.WriteLine($"Series {s.Name}");                  break;
    }
}
```

### Set / clear

```csharp
chart.SetObjectSelection(series);                           // select (single-argument overload)
chart.SetObjectSelection(seriesPoint);                      // select a point

chart.ClearSelection();                                      // clear all (there is no single-object deselect)
chart.ReplaceSelectedItems(new object[] { series });         // bulk replace — selects exactly these, deselecting the rest
```

`SetObjectSelection(object)` takes a single argument and respects `SelectionMode`. In `Single` mode, selecting a new item deselects the previous one. To deselect a specific item, rebuild the selection with `ReplaceSelectedItems` (or `ClearSelection` to drop everything).

## Customizing the Selected Appearance

The default behavior darkens the selected element. Override per-series via `CustomDrawSeriesPoint`:

```csharp
chart.CustomDrawSeriesPoint += (s, e) => {
    if (e.SelectionState == SelectionState.Selected) {
        e.SeriesDrawOptions.Color = Color.OrangeRed;
        if (e.SeriesDrawOptions is BarDrawOptions bar)
            bar.Border.Color = Color.DarkRed;
    }
};
```

`SelectionState` values: `None`, `Hot`, `Selected`. Apply different visuals for hover (Hot) vs selected.

## Rectangle (Lasso) Selection

Only available when `SelectionMode = Extended`. The default binding is **Ctrl+drag** (configurable):

```csharp
chart.SelectionMode = ElementSelectionMode.Extended;
var diagram = (XYDiagram)chart.Diagram;
diagram.SelectionOptions.RectangleSelectionMouseAction.MouseButton = DevExpress.Portable.Input.PortableMouseButtons.Left;
diagram.SelectionOptions.RectangleSelectionMouseAction.ModifierKeys = ChartModifierKeys.Control;
```

Drag a rectangle over the diagram → every visible element inside the rectangle gets selected.

## Hot-Track Without Selection

```csharp
chart.SelectionMode = ElementSelectionMode.None;
chart.ObjectHotTracked += (s, e) => {
    if (e.HitInfo.InSeriesPoint)
        toolTipBuilder.Text = $"{e.HitInfo.SeriesPoint.Values[0]}";
};
```

Hot-track works even when selection is off — useful for cursor-driven dashboards.

## Series-Level Opt-Out

`SeriesSelectionMode` has no `None` member (only `Series` / `Point` / `Argument`). To make a specific series non-selectable, cancel its selection in the `ObjectSelected` event (and, optionally, hot-tracking in `ObjectHotTracked`):

```csharp
chart.ObjectSelected += (s, e) => {
    if (e.Object is Series sr && sr == referenceSeries)
        e.Cancel = true;                       // this series cannot be selected
};
chart.ObjectHotTracked += (s, e) => {
    if (e.Object is Series sr && sr == referenceSeries)
        e.Cancel = true;                       // also block hot-track
};
```

## Common Patterns

### Pattern 1 — Single-point selection with handler

```csharp
chart.SelectionMode       = ElementSelectionMode.Single;
chart.SeriesSelectionMode = SeriesSelectionMode.Point;
chart.ObjectSelected += (s, e) => {
    if (e.Object is SeriesPoint p)
        ShowDetails(p);
};
```

### Pattern 2 — Multi-point selection with Ctrl/Shift

```csharp
chart.SelectionMode       = ElementSelectionMode.Extended;
chart.SeriesSelectionMode = SeriesSelectionMode.Point;
chart.SelectedItemsChanged += (s, e) =>
    detail.Text = $"{chart.SelectedItems.Count} points selected";
```

### Pattern 3 — Argument selection (compare across series)

```csharp
chart.SelectionMode       = ElementSelectionMode.Single;
chart.SeriesSelectionMode = SeriesSelectionMode.Argument;
chart.ObjectSelected += (s, e) => {
    if (e.HitInfo.SeriesPoint is SeriesPoint p)
        Highlight(p.Argument);
};
```

### Pattern 4 — Highlight selected points with a custom color

```csharp
chart.CustomDrawSeriesPoint += (s, e) => {
    if (e.SelectionState == SelectionState.Selected)
        e.SeriesDrawOptions.Color = Color.OrangeRed;
};
```

### Pattern 5 — Programmatic selection

```csharp
chart.ClearSelection();
chart.SetObjectSelection(series.Points[3]);
```

## Common Issues

- **Nothing selects on click** — `SelectionMode = None` (default). Set `Single` / `Multiple` / `Extended`.
- **Selection works on a single point but I want the whole series** — set `SeriesSelectionMode = Series`.
- **Click doesn't clear selection** — `AllowDeselectByClickInVoid = false`. Enable it, or call `ClearSelection` from another control.
- **3D chart doesn't respond to clicks** — 3D charts don't support runtime selection.
- **`ObjectSelected` not firing on rectangle drag** — for many items at once, use `SelectedItemsChanged`. `ObjectSelected` fires per click.
- **`SwiftPlotDiagram` points not selectable** — by design; SwiftPlot skips per-point hit testing for performance.
- **Selection lost after `RefreshData`** — `RefreshData` rebuilds points, dropping selection. Persist the selected argument/key and re-apply selection via `SetObjectSelection` in a follow-up.
- **Multiple-mode click doesn't toggle** — verify `SelectionOptions.MouseAction` matches the actual mouse button you press. The default is left-click.

## Source Material

- `articles/controls-and-libraries/chart-control/end-user-features/chart-control-selection.md` (`xref:WindowsForms.114777`).
- `articles/controls-and-libraries/chart-control/examples/selection/how-to-handle-element-selection.md` (`xref:WindowsForms.117682`).
- `api/DevExpress.XtraCharts.ChartControl.SelectionMode.yml`.
- `api/DevExpress.XtraCharts.ChartControl.SeriesSelectionMode.yml`.
- `api/DevExpress.XtraCharts.SelectionOptions.yml`.
- `api/DevExpress.XtraCharts.ChartControl.ObjectSelected.yml`.
- `api/DevExpress.XtraCharts.ChartControl.SelectedItemsChanged.yml`.
- `api/DevExpress.XtraCharts.ChartControl.SetObjectSelection.yml`.
- `api/DevExpress.XtraCharts.ChartControl.ClearSelection.yml`.
- `api/DevExpress.XtraCharts.SelectionState.yml`.

# User Interaction & Zoom in DevExpress Blazor Charts

## When to Use This Reference

- Enable zoom and pan (mouse wheel, touch gestures, drag-to-zoom)
- Add a scroll bar to the chart
- Handle point/series click events
- Enable point or series selection
- React to visual range changes (zoom/pan events)

> **Note**: All interactive features require an interactive render mode (`InteractiveServer`, `InteractiveWebAssembly`, or `InteractiveAuto`). Static render mode (SSR) displays the chart as a static image only.

## Selection

### Enable Multiple Point Selection

```razor
<DxChart Data="DataSource" PointSelectionMode="ChartSelectionMode.Multiple">
    <DxChartLineSeries ArgumentField="@((DataPoint v) => v.Country)"
                       ValueField="@((DataPoint v) => v.AppleProduction)" />
    <DxChartLegend Visible="false" />
</DxChart>
```

`ChartSelectionMode` values: `None`, `Single`, `Multiple`

### Series-Level Selection Mode

For line- and area-based series, use `SelectionMode` on `DxChartSeriesPoint`:

```razor
<DxChart Data="DataSource" PointSelectionMode="ChartSelectionMode.Multiple">
    <DxChartLineSeries ArgumentField="@((DataPoint v) => v.Country)"
                       ValueField="@((DataPoint v) => v.AppleProduction)">
        <DxChartSeriesPoint SelectionMode="ChartSeriesPointSelectionMode.AllPoints" />
    </DxChartLineSeries>
    <DxChartLegend Visible="false" />
</DxChart>
```

### Pie Chart Selection

```razor
<DxPieChart Data="DataSource" PointSelectionMode="ChartSelectionMode.Multiple">
    <DxPieChartSeries ArgumentField="@((DataPoint v) => v.Country)"
                      ValueField="@((DataPoint v) => v.AppleProduction)" />
    <DxChartLegend Visible="false" />
</DxPieChart>
```

### Polar Chart Selection

```razor
<DxPolarChart Data="DataSource" SeriesSelectionMode="ChartSelectionMode.Multiple">
    <DxPolarChartLineSeries ArgumentField="@((DataPoint v) => v.Country)"
                            ValueField="@((DataPoint v) => v.AppleProduction)" />
    <DxChartLegend Visible="false" />
</DxPolarChart>
```

## Zoom and Pan

Use `DxChartZoomAndPanSettings` to enable zoom and pan operations on `DxChart`:

```razor
<DxChart Data="DataSource">
    <DxChartAreaSeries ArgumentField="@((DataPoint v) => v.Date)"
                       ValueField="@((DataPoint v) => v.Value)" />
    <DxChartZoomAndPanSettings ArgumentAxisZoomAndPanMode="ChartAxisZoomAndPanMode.Zoom"
                               ValueAxisZoomAndPanMode="ChartAxisZoomAndPanMode.Pan" />
</DxChart>
```

`ChartAxisZoomAndPanMode` values: `None`, `Pan`, `Zoom`, `Both`

### Disable Mouse Wheel or Touch Gestures

```razor
<DxChartZoomAndPanSettings ArgumentAxisZoomAndPanMode="ChartAxisZoomAndPanMode.Both"
                           ValueAxisZoomAndPanMode="ChartAxisZoomAndPanMode.Both"
                           AllowMouseWheel="false"
                           AllowTouchGestures="true" />
```

### Drag-to-Zoom (Zoom Area)

```razor
<DxChartZoomAndPanSettings ArgumentAxisZoomAndPanMode="ChartAxisZoomAndPanMode.Both"
                           AllowDragToZoom="true"
                           PanKey="ChartPanKey.Shift" />
```

When `AllowDragToZoom="true"`, the user draws a rectangle to zoom. Hold the `PanKey` (default: `Shift`) to pan instead.

Use `DxChartZoomAndPanDragBoxStyle` to customize the drag box appearance.

### Scroll Bar

Add a horizontal scroll bar below (or above) the chart:

```razor
<DxChart Data="DataSource">
    <DxChartLineSeries ArgumentField="@((DataPoint v) => v.Date)"
                       ValueField="@((DataPoint v) => v.Value)" />
    <DxChartZoomAndPanSettings ArgumentAxisZoomAndPanMode="ChartAxisZoomAndPanMode.Both" />
    <DxChartScrollBarSettings ArgumentAxisScrollBarVisible="true"
                              ArgumentAxisScrollBarPosition="ChartScrollBarPosition.Bottom" />
</DxChart>
```

For rotated charts, use `ChartScrollBarPosition.Right` or `ChartScrollBarPosition.Left`.

## React to Visual Range Changes

Use the `VisualRangeChanged` event to detect when the user zooms or pans:

```razor
<DxChart Data="DataSource"
         VisualRangeChanged="OnVisualRangeChanged">
    <DxChartLineSeries ArgumentField="@((DataPoint v) => v.Date)"
                       ValueField="@((DataPoint v) => v.Value)" />
    <DxChartZoomAndPanSettings ArgumentAxisZoomAndPanMode="ChartAxisZoomAndPanMode.Both" />
</DxChart>

@code {
    void OnVisualRangeChanged(ChartVisualRangeChangedEventArgs e) {
        // e.AxisName, e.StartValue, e.EndValue
    }
}
```

## Programmatic Zoom Control

Use component methods to control zoom programmatically:

```csharp
// Reset zoom to default (show all data)
await Chart.ResetVisualRange();

// Set specific argument axis range
await Chart.SetArgumentAxisVisualRange(new List<object> { startDate, endDate });

// Set specific value axis range
await Chart.SetValueAxisVisualRange(new List<object> { 0, 100 }, axisName: null);
```

Reference the chart with `@ref`:
```razor
<DxChart @ref="Chart" Data="DataSource">
    ...
</DxChart>

@code {
    DxChart<DataPoint> Chart;
}
```

## Troubleshooting

| Issue | Solution |
|---|---|
| Zoom/pan not working | Add `@rendermode InteractiveServer` (or WASM/Auto) to the page |
| Scroll bar not visible | Set `ArgumentAxisScrollBarVisible="true"` on `DxChartScrollBarSettings` |
| Drag-to-zoom starts panning instead | Hold `PanKey` (default: Shift) to pan; release to drag-zoom |
| `VisualRangeChanged` fires too often | Debounce the handler in code to avoid excessive re-renders |

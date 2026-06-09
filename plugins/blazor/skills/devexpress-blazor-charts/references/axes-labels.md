# Axes & Labels in DevExpress Blazor Charts

## When to Use This Reference

- Configure argument axis (X) or value axis (Y) in `DxChart`
- Configure argument/value axis in `DxPolarChart`
- Add axis titles, strips, constant lines, and axis ranges
- Set axis position, rotation, or alignment
- Add series labels or axis labels
- Manage overlapping labels

## Axes in DxChart

`DxChart` uses `DxChartArgumentAxis` for the X-axis and `DxChartValueAxis` for the Y-axis.

### Axis Titles

```razor
<DxChart Data="DataSource">
    <DxChartLineSeries ArgumentField="@((DataPoint v) => v.Date)"
                       ValueField="@((DataPoint v) => v.Value)" />
    <DxChartArgumentAxis>
        <DxChartAxisTitle Text="Date" HorizontalAlignment="HorizontalAlignment.Right" />
    </DxChartArgumentAxis>
    <DxChartValueAxis>
        <DxChartAxisTitle Text="Temperature (°C)" />
    </DxChartValueAxis>
</DxChart>
```

### Axis Strips

Highlight a range of values between two axis values:

```razor
<DxChart Data="DataSource">
    <DxChartLineSeries ArgumentField="@((DataPoint v) => v.Date)"
                       ValueField="@((DataPoint v) => v.Value)" />
    <DxChartValueAxis>
        <DxChartAxisStrip StartValue="17"
                          EndValue="19"
                          Color="rgba(255, 155, 85, 0.15)">
            <DxChartAxisStripLabel Text="Average Temperature" />
        </DxChartAxisStrip>
    </DxChartValueAxis>
    <DxChartLegend Visible="false" />
</DxChart>
```

### Axis Range

Limit the visible scale range using `DxChartAxisRange`:

```razor
<DxChart T="DataPoint" Data="points">
    <DxChartScatterSeries T="DataPoint"
                          TArgument="double"
                          TValue="double"
                          ArgumentField="point => point.Y"
                          ValueField="point => point.X" />
    <DxChartValueAxis>
        <DxChartAxisRange StartValue="-20.0" EndValue="20.0" />
    </DxChartValueAxis>
    <DxChartArgumentAxis>
        <DxChartAxisRange StartValue="-20.0" EndValue="20.0" />
    </DxChartArgumentAxis>
</DxChart>
```

### Axis Rotation (Swap X/Y)

```razor
<DxChart Data="DataSource" Rotated="true">
    <DxChartBarSeries ArgumentField="@((DataPoint v) => v.Date)"
                      ValueField="@((DataPoint v) => v.Value)" />
</DxChart>
```

### Axis Alignment and Position

```razor
<DxChartValueAxis>
    <DxChartAxisTitle Text="Values" />
    <!-- Align axis to center of chart plane -->
</DxChartValueAxis>
```

Key axis properties:
- `DxChartAxis.CustomPosition` — set a custom numeric position for the axis
- `DxChartAxis.Offset` — shift the axis by a pixel offset
- `DxChartAxis.Alignment` — align the axis (`Near`, `Far`)

### Multiple Value Axes

You can add multiple `DxChartValueAxis` objects and link series to specific axes using the `Axis` property on the series.

## Axes in DxPolarChart

`DxPolarChart` uses a circular argument axis and a radial value axis. Declare `DxChartArgumentAxis` and `DxChartValueAxis` inside `DxPolarChart` to configure them.

## Series Labels

Declare a `DxChartSeriesLabel` inside a series to show labels on data points:

```razor
<DxChart Data="DataSource">
    <DxChartLineSeries ArgumentField="@((DataPoint v) => v.Date)"
                       ValueField="@((DataPoint v) => v.Value)">
        <DxChartSeriesLabel Visible="true" />
    </DxChartLineSeries>
</DxChart>
```

For `DxPieChart`:
```razor
<DxPieChart Data="DataSource">
    <DxPieChartSeries ArgumentField="@((DataPoint v) => v.Country)"
                      ValueField="@((DataPoint v) => v.AppleProduction)">
        <DxChartSeriesLabel Visible="true" />
    </DxPieChartSeries>
</DxPieChart>
```

### Managing Overlapping Labels

When many data points are close together, labels may overlap. Use the `LabelOverlap` property:

```razor
<!-- For DxChart -->
<DxChart Data="forecasts" LabelOverlap="ChartLabelOverlap.Hide">
    ...
</DxChart>

<!-- For DxPieChart -->
<DxPieChart Data="DataSource" LabelOverlap="PieChartLabelOverlap.Hide">
    ...
</DxPieChart>
```

`ChartLabelOverlap` values: `Hide`, `Stack`, `None`
`PieChartLabelOverlap` values: `Hide`, `Stack`, `ShiftInward`, `None`

## Chart Title and Subtitle

```razor
<DxChart Data="DataSource">
    ...
    <DxChartTitle Text="Fruit Production By Country">
        <DxChartSubTitle Text="(Millions of Tons)" />
    </DxChartTitle>
</DxChart>
```

`DxChartTitle` and `DxChartSubTitle` are used in all three chart components.

## Legend

```razor
<DxChart Data="DataSource">
    ...
    <DxChartLegend Position="RelativePosition.Outside"
                   Orientation="Orientation.Vertical"
                   HorizontalAlignment="HorizontalAlignment.Right">
        <DxChartTitle Text="Series" />
    </DxChartLegend>
</DxChart>
```

Key `DxChartLegend` properties:
- `Visible` — `bool`, default `true`
- `Position` — `RelativePosition.Outside` or `RelativePosition.Inside`
- `Orientation` — `Orientation.Horizontal` or `Orientation.Vertical`
- `HorizontalAlignment` — `HorizontalAlignment.Left`, `Center`, `Right`

## Troubleshooting

| Issue | Solution |
|---|---|
| Axis labels overlap after zoom | Set `DxChartAxisLabel.Overlap` to `Hide` or `DisplayMode` to `Rotate` |
| Labels not visible on small chart | Check `DxChartSeriesLabel.Visible` is `true` |
| Axis title not showing | Ensure `DxChartAxisTitle.Text` is set and axis component is declared inside the chart |

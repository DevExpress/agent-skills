# Customization in DevExpress Blazor Charts

## When to Use This Reference

- Apply a custom color palette to series or pie sectors
- Extend the palette when series count exceeds palette colors
- Add tooltips to a chart
- Add annotations (image or text) anchored to chart coordinates
- Customize font settings for the chart
- Change chart size (Width/Height properties)

## Color Palette

Assign an array of color strings to the `Palette` property:

```razor
<DxChart Data="@DataSource"
         Palette="@(new[] { "#5f8db5", "#e8a838", "#679e6d" })"
         PaletteExtensionMode="ChartPaletteExtensionMode.Alternate">
    <DxChartBarSeries ArgumentField="@((StatisticPoint v) => v.Country)"
                      ValueField="@((StatisticPoint v) => v.Population24)"
                      Name="2024" />
    <DxChartBarSeries ArgumentField="@((StatisticPoint v) => v.Country)"
                      ValueField="@((StatisticPoint v) => v.Population23)"
                      Name="2023" />
    <DxChartTitle Text="Population by European Country" />
    <DxChartLegend Position="RelativePosition.Outside"
                   Orientation="Orientation.Vertical"
                   HorizontalAlignment="HorizontalAlignment.Right">
        <DxChartTitle Text="Years" />
    </DxChartLegend>
</DxChart>
```

`ChartPaletteExtensionMode` values:
- `Alternate` — Alternate colors from the palette
- `Blend` — Create blended colors
- `Extrapolate` — Extrapolate beyond the palette

For `DxPieChart`, each sector takes one color from the palette.

## Tooltips

Declare `DxChartTooltip` inside the chart to configure tooltip appearance:

```razor
<DxChart Data="DataSource">
    <DxChartLineSeries ArgumentField="@((DataPoint v) => v.Date)"
                       ValueField="@((DataPoint v) => v.Value)" />
    <DxChartTooltip Enabled="true" />
</DxChart>
```

You can use a `RenderFragment` to customize tooltip content. See DxDocs MCP for `DxChartTooltip` details.

## Annotations

Add text or image annotations anchored to chart coordinates or series points.

### Text Annotation Anchored to Coordinates

```razor
<DxChart Data="DataSource">
    <DxChartLineSeries ArgumentField="@((DataPoint v) => v.Date)"
                       ValueField="@((DataPoint v) => v.Value)" />
    <DxChartAnnotation ArgumentValue="new DateTime(2023, 6, 1)"
                       Value="15"
                       Text="Peak Value"
                       Type="ChartAnnotationType.Text" />
</DxChart>
```

Key `DxChartAnnotation` properties:
- `ArgumentValue` — X-axis coordinate to anchor the annotation
- `Value` — Y-axis value to anchor the annotation
- `Text` — Annotation text (for text annotations)
- `ImageUrl` — Image URL (for image annotations)
- `Type` — `ChartAnnotationType.Text` or `ChartAnnotationType.Image`

## Font Customization

Use `DxChartFont` nested in legend, axis label, series label, or title components to change font properties:

```razor
<DxChart Data="DataSource">
    <DxChartTitle Text="My Chart">
        <DxChartFont Size="18" Weight="700" />
    </DxChartTitle>
    <DxChartLegend Visible="true">
        <DxChartFont Size="12" />
    </DxChartLegend>
</DxChart>
```

`DxChartFont` properties: `Size`, `Weight`, `Family`, `Color`, `Opacity`

## Chart Size

Use the `Width` and `Height` properties on the chart component:

```razor
<DxChart Data="DataSource" Width="800px" Height="400px">
    ...
</DxChart>
```

Or use CSS to control sizing from outside.

## Inner Component Customization

Inner chart components (legend items, axis labels, series labels) can be customized using their dedicated child components. Use `DxDocs MCP` to look up the exact nested component structure for less common customizations.

## Troubleshooting

| Issue | Solution |
|---|---|
| Palette colors not applied | Ensure `Palette` is a `string[]` of valid CSS color strings (hex, rgba) |
| Tooltip not showing | Set `Enabled="true"` on `DxChartTooltip`; ensure chart is interactive |
| Annotation position off | `ArgumentValue` type must match argument axis type (e.g., `DateTime` for date axes) |

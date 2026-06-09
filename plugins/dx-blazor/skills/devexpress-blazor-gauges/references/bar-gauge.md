# DxBarGauge Reference

`DxBarGauge` displays multiple numeric values as concentric arc bars on a circular scale.

API Reference: [DxBarGauge members](xref:DevExpress.Blazor.DxBarGauge._members)

## Basic Usage

```razor
@rendermode InteractiveServer
@using DevExpress.Blazor

<DxBarGauge Width="100%"
            Height="400px"
            StartValue="0"
            EndValue="100"
            Values="@Values">
</DxBarGauge>

@code {
    double[] Values = new double[] { 47.27, 65.32, 84.59 };
}
```

## Scale Configuration

| Property | Type | Description |
|---|---|---|
| `StartValue` | `double` | Minimum scale value |
| `EndValue` | `double` | Maximum scale value |
| `BaseValue` | `double` | Value from which bars grow (default: `StartValue`) |

Use `BaseValue` for positive/negative split displays (e.g., `BaseValue="0"` with negative start):

```razor
<DxBarGauge StartValue="-5"
            EndValue="5"
            BaseValue="0"
            Values="@Values">
</DxBarGauge>

@code {
    double[] Values = new double[] { -2.13, 1.48, -3.09, 4.52 };
}
```

## Geometry (Gauge Shape)

Use `DxBarGaugeGeometrySettings` to configure the angular span and start angle of the gauge arc:

```razor
<DxBarGauge StartValue="0" EndValue="100" Values="@Values">
    <DxBarGaugeGeometrySettings StartAngle="90"
                                EndAngle="-90" />
</DxBarGauge>
```

## Labels

Use `DxBarGaugeLabelSettings` to configure value labels on each bar:

```razor
<DxBarGauge StartValue="0" EndValue="100" Values="@Values">
    <DxBarGaugeLabelSettings Indent="30"
                             ConnectorColor="purple"
                             ConnectorWidth="4">
        <DxFontSettings Weight="600" />
        <DxTextFormatSettings LdmlString="##.#'%' " />
    </DxBarGaugeLabelSettings>
</DxBarGauge>
```

| Setting | Type | Description |
|---|---|---|
| `Indent` | `int` | Distance from the bar to the label |
| `ConnectorColor` | `string` | Color of the connector line |
| `ConnectorWidth` | `int` | Width of the connector line |
| `DxFontSettings` | child | Font configuration (size, weight, family, color) |
| `DxTextFormatSettings` | child | Number format string (`LdmlString`) |

## Legend

Use `DxBarGaugeLegendSettings` to display a legend below (or around) the gauge:

```razor
<DxBarGauge StartValue="0" EndValue="100" Values="@Values">
    <DxBarGaugeLegendSettings Visible="true"
                              ItemCaptions="@LegendItemCaptions"
                              VerticalAlignment="VerticalEdge.Bottom"
                              HorizontalAlignment="HorizontalAlignment.Center">
        <DxLegendTitleSettings Text="Series" />
        <DxBorderSettings Visible="true" Color="purple" />
        <DxMarginSettings Top="30" />
    </DxBarGaugeLegendSettings>
</DxBarGauge>

@code {
    double[] Values = new double[] { 47.27, 65.32, 84.59, 81.86, 99 };
    string[] LegendItemCaptions = new string[] { "Metacritic", "Ratingraph.com", "Rotten Tomatoes", "IMDb", "TV.com" };
}
```

`ItemCaptions` must have the same number of elements as `Values`.

## Palette (Colors)

Use the `Palette` property to specify bar colors:

```razor
<DxBarGauge Values="@Values"
            Palette="@(new[] { "#5f8b95", "#ba4d51", "#af8a53" })">
</DxBarGauge>
```

## Title and Subtitle

Use `DxTitleSettings` / `DxSubtitleSettings` to add descriptive headings:

```razor
<DxBarGauge StartValue="0" EndValue="100" Values="@Values">
    <DxTitleSettings Text="Review Scores" />
    <DxSubtitleSettings Text="(out of 100)" />
</DxBarGauge>
```

## Tooltip

Use `DxBarGaugeTooltipSettings` to configure the tooltip that appears on hover:

```razor
<DxBarGauge StartValue="0" EndValue="100" Values="@Values">
    <DxBarGaugeTooltipSettings Enabled="true" />
</DxBarGauge>
```

## Size

Use `Width` and `Height` to set the component dimensions (supports any valid CSS values):

```razor
<DxBarGauge Width="100%"
            Height="500px"
            ...>
</DxBarGauge>
```

## Export

Call `ExportAsync()` to export the gauge as an image (PNG, JPEG, GIF, SVG) or PDF:

```razor
<DxBarGauge @ref="Gauge" ... />
<button @onclick="Export">Export</button>

@code {
    DxBarGauge Gauge;
    async Task Export() {
        await Gauge.ExportAsync("gauge.png", ChartExportFormat.PNG);
    }
}
```

# DxSankey Reference

`DxSankey` renders a flow diagram (Sankey chart) that shows the volume of flows between a source set and a target set. Nodes represent entities; links represent the flow volume.

API Reference: [DxSankey members](xref:DevExpress.Blazor.DxSankey._members)

## Basic Usage

```razor
@rendermode InteractiveServer
@using DevExpress.Blazor

<DxSankey Data="@Data"
          Width="100%"
          Height="440px"
          SourceFieldName="Source"
          TargetFieldName="Target"
          WeightFieldName="Weight">
    <DxSankeyNodeSettings Width="8" Spacing="30" />
    <DxSankeyLinkSettings ColorMode="SankeyLinkColorMode.Gradient" />
    <DxTitleSettings Text="Commodity Turnover" />
</DxSankey>

@code {
    IEnumerable<SankeyDataPoint> Data = Enumerable.Empty<SankeyDataPoint>();

    protected override void OnInitialized() {
        Data = new List<SankeyDataPoint> {
            new SankeyDataPoint("Spain",   "United States of America", 2),
            new SankeyDataPoint("Germany", "United States of America", 8),
            new SankeyDataPoint("France",  "United States of America", 4),
            new SankeyDataPoint("Germany", "Great Britain", 2),
            new SankeyDataPoint("France",  "Great Britain", 4),
        };
    }
    record SankeyDataPoint(string Source, string Target, int Weight);
}
```

## Data Binding

`DxSankey` uses **string field names** (not lambda expressions):

| Property | Type | Description |
|---|---|---|
| `Data` | `IEnumerable<T>` | Data source containing flow records |
| `SourceFieldName` | `string` | Property name for the source node (e.g., `"Source"`) |
| `TargetFieldName` | `string` | Property name for the target node (e.g., `"Target"`) |
| `WeightFieldName` | `string` | Property name for the flow weight/volume (e.g., `"Weight"`) |

> Property names are **case-sensitive** and must match the C# class property names exactly.

## Node Settings

Use `DxSankeyNodeSettings` to configure node appearance:

```razor
<DxSankey ...>
    <DxSankeyNodeSettings Width="8"
                          Spacing="30" />
</DxSankey>
```

| Property | Description |
|---|---|
| `Width` | Pixel width of each node bar |
| `Spacing` | Pixel spacing between node bars |

## Link Settings

Use `DxSankeyLinkSettings` to configure the flow links:

```razor
<DxSankey ...>
    <DxSankeyLinkSettings ColorMode="SankeyLinkColorMode.Gradient" />
</DxSankey>
```

`SankeyLinkColorMode` values:
- `None` — No color (links use a default neutral color)
- `Source` — Link takes the color of its source node
- `Target` — Link takes the color of its target node
- `Gradient` — Link uses a gradient from source color to target color

## Labels

Use `DxSankeyNodeLabelSettings` to configure node labels:

```razor
<DxSankey ...>
    <DxSankeyNodeLabelSettings Visible="true"
                               OverlappingBehavior="SankeyLabelOverlappingBehavior.Hide" />
</DxSankey>
```

## Node Vertical Layout

Use `DxSankeyNodeAlignmentSettings` to control node alignment:

```razor
<DxSankey ...>
    <DxSankeyNodeAlignmentSettings Alignment="SankeyNodeAlignment.Top" />
</DxSankey>
```

## Tooltip

Use `DxSankeyTooltipSettings` to configure hover tooltips:

```razor
<DxSankey ...>
    <DxSankeyTooltipSettings Enabled="true" />
</DxSankey>
```

## Click and Hover Events

```razor
<DxSankey ...
          NodeClick="OnNodeClick"
          LinkClick="OnLinkClick">
</DxSankey>

@code {
    void OnNodeClick(SankeyNodeClickEventArgs args) {
        Console.WriteLine($"Node clicked: {args.NodeTitle}");
    }
    void OnLinkClick(SankeyLinkClickEventArgs args) {
        Console.WriteLine($"Link: {args.SourceTitle} → {args.TargetTitle}");
    }
}
```

## Title

```razor
<DxSankey ...>
    <DxTitleSettings Text="Commodity Turnover" />
    <DxSubtitleSettings Text="(2023)" />
</DxSankey>
```

## Size

```razor
<DxSankey Width="100%"
           Height="440px"
           ...>
</DxSankey>
```

## Export

```razor
<DxSankey @ref="Sankey" ...>...</DxSankey>
<button @onclick="Export">Export</button>

@code {
    DxSankey Sankey;
    async Task Export() {
        await Sankey.ExportAsync("sankey.png", ChartExportFormat.PNG);
    }
}
```

# DxSparkline Reference

`DxSparkline` is a compact, lightweight visualization component for embedding trend lines or bar charts inside other UI elements — such as Grid columns, cards, or list items.

API Reference: [DxSparkline members](xref:DevExpress.Blazor.DxSparkline._members)

Demo: `https://demos.devexpress.com/blazor/ChartSparkline`

## Basic Usage

```razor
@using DevExpress.Blazor

<DxSparkline Data="@DataSource"
             Type="SparklineType.Bar"
             ArgumentFieldName="Month"
             ValueFieldName="VisitorCount"
             Height="50px"
             Width="200px" />

@code {
    IEnumerable<SparklineDataPoint> DataSource = Enumerable.Empty<SparklineDataPoint>();

    protected override void OnInitialized() {
        DataSource = new List<SparklineDataPoint> {
            new SparklineDataPoint(1,  2210),
            new SparklineDataPoint(2,  2103),
            new SparklineDataPoint(3,  2132),
            new SparklineDataPoint(4,  2000),
            new SparklineDataPoint(5, -2062),
            new SparklineDataPoint(6,  1954),
        };
    }
    record SparklineDataPoint(int Month, int VisitorCount);
}
```

## Data Binding

`DxSparkline` uses **string property names**:

| Property | Type | Description |
|---|---|---|
| `Data` | `IEnumerable<T>` | Data source |
| `ArgumentFieldName` | `string` | Property name for the argument axis values |
| `ValueFieldName` | `string` | Property name for the value axis values |

## Series Types

Set the `Type` property to choose the visual style:

| Value | Description |
|---|---|
| `SparklineType.Line` | Line connecting data points |
| `SparklineType.Bar` | Vertical bars per data point |
| `SparklineType.Spline` | Smooth curved line |
| `SparklineType.SplineArea` | Smooth curve with shaded area |
| `SparklineType.Area` | Straight line with shaded area |
| `SparklineType.StepLine` | Step-function line |
| `SparklineType.StepArea` | Step-function with shaded area |
| `SparklineType.WinLoss` | Binary win/loss bars (positive vs negative) |

```razor
<DxSparkline Type="SparklineType.WinLoss" ... />
```

## Size

Always specify `Width` and `Height`; the sparkline is hidden if size is zero:

```razor
<DxSparkline Width="200px" Height="50px" ... />
```

CSS percentage values are also supported: `Width="100%"`.

## Embedding in a Grid Column

Use inside a `DxGridDataColumn` `CellDisplayTemplate`:

```razor
<DxGrid Data="@GridData">
    <Columns>
        <DxGridDataColumn FieldName="Name" />
        <DxGridDataColumn FieldName="Trend" Caption="Trend">
            <CellDisplayTemplate>
                @{
                    var row = (SalesRow)context.DataItem;
                }
                <DxSparkline Data="@row.MonthlyData"
                             Type="SparklineType.Line"
                             ArgumentFieldName="Month"
                             ValueFieldName="Amount"
                             Width="120px"
                             Height="40px" />
            </CellDisplayTemplate>
        </DxGridDataColumn>
    </Columns>
</DxGrid>
```

## Tooltip

Enable a tooltip to show value on hover:

```razor
<DxSparkline ...>
    <DxSparklineTooltipSettings Enabled="true" />
</DxSparkline>
```

## Export

```razor
<DxSparkline @ref="Sparkline" ...>...</DxSparkline>
<button @onclick="Export">Export</button>

@code {
    DxSparkline Sparkline;
    async Task Export() {
        await Sparkline.ExportAsync("sparkline.png", ChartExportFormat.PNG);
    }
}
```

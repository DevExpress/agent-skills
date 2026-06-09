# DxRangeSelector Reference

`DxRangeSelector` renders a horizontal scale with two draggable sliders. Users select a sub-range by moving the sliders. Optionally, a background chart provides context.

API Reference: [DxRangeSelector members](xref:DevExpress.Blazor.DxRangeSelector._members)

## Basic Usage (Numeric Scale)

```razor
@rendermode InteractiveServer
@using DevExpress.Blazor

<DxRangeSelector Width="100%"
                 Height="200px"
                 SelectedRangeStartValue="20"
                 SelectedRangeEndValue="60">
    <DxRangeSelectorScale StartValue="0"
                          EndValue="100"
                          TickInterval="@(ChartAxisInterval.Points(10))"
                          ValueType="ChartAxisDataType.Numeric" />
</DxRangeSelector>
```

## DateTime Scale

```razor
<DxRangeSelector Width="1100px"
                 Height="200px"
                 SelectedRangeStartValue="@(new DateTime(2024, 2, 1))"
                 SelectedRangeEndValue="@(new DateTime(2024, 2, 14))">
    <DxRangeSelectorScale StartValue="@(new DateTime(2024, 1, 1))"
                          EndValue="@(new DateTime(2024, 6, 1))"
                          TickInterval="ChartAxisInterval.Week"
                          MinorTickInterval="ChartAxisInterval.Day"
                          MinRange="ChartAxisInterval.Week"
                          MaxRange="ChartAxisInterval.Month"
                          ValueType="ChartAxisDataType.DateTime">
        <DxRangeSelectorScaleMarker>
            <DxRangeSelectorScaleMarkerLabel>
                <DxTextFormatSettings Type="TextFormat.MonthAndYear" />
            </DxRangeSelectorScaleMarkerLabel>
        </DxRangeSelectorScaleMarker>
    </DxRangeSelectorScale>
    <DxRangeSelectorSliderMarker>
        <DxTextFormatSettings Type="TextFormat.MonthAndDay" />
    </DxRangeSelectorSliderMarker>
</DxRangeSelector>
```

## Scale Configuration

Use `DxRangeSelectorScale` to configure the scale:

| Property | Type | Description |
|---|---|---|
| `StartValue` | `object` | Minimum scale value |
| `EndValue` | `object` | Maximum scale value |
| `TickInterval` | `ChartAxisInterval` | Major tick interval |
| `MinorTickInterval` | `ChartAxisInterval` | Minor tick interval |
| `MinRange` | `ChartAxisInterval` | Minimum allowed selection range |
| `MaxRange` | `ChartAxisInterval` | Maximum allowed selection range |
| `ValueType` | `ChartAxisDataType` | `Numeric`, `DateTime`, `String` |

### ChartAxisInterval Helpers

```csharp
ChartAxisInterval.Week           // predefined intervals
ChartAxisInterval.Month
ChartAxisInterval.Day
ChartAxisInterval.Points(10)    // numeric interval of 10
ChartAxisInterval.Weeks(2)      // 2-week interval
```

## Selected Range Properties

| Property | Type | Description |
|---|---|---|
| `SelectedRangeStartValue` | `object` | Initial start slider value |
| `SelectedRangeEndValue` | `object` | Initial end slider value |
| `SelectedRangeLength` | `ChartAxisInterval` | Alternative: set length instead of end value |

Setting start and length:

```razor
<DxRangeSelector SelectedRangeStartValue="@(new DateTime(2024, 2, 1))"
                 SelectedRangeLength="ChartAxisInterval.Weeks(2)">
    ...
</DxRangeSelector>
```

## Reacting to Selection Changes

Handle `ValueChanged` to update your view when the user moves a slider. `RangeSelectorValueChangedEventArgs.CurrentRange` is a `List<object>` that always contains two items: index 0 is the start value, index 1 is the end value. Use `.Count` (not `.Length`) and cast each item defensively.

```razor
<DxRangeSelector SelectedRangeStartValue="@Start"
                 SelectedRangeEndValue="@End"
                 ValueChanged="OnRangeChanged"
                 ValueChangeMode="RangeSelectorValueChangeMode.OnHandleRelease">
    ...
</DxRangeSelector>

<p>Selected: @Start — @End</p>

@code {
    DateTime Start = new DateTime(2024, 1, 1);
    DateTime End = new DateTime(2024, 3, 31);

    void OnRangeChanged(RangeSelectorValueChangedEventArgs args) {
        if (args.CurrentRange.Count >= 2) {
            Start = args.CurrentRange.FirstOrDefault() as DateTime? ?? Start;
            End   = args.CurrentRange.LastOrDefault()  as DateTime? ?? End;
        }
    }
}
```

## Background Chart

Display a chart behind the range scale to provide visual context for the selection:

```razor
<DxRangeSelector Data="@Data">
    <DxTitleSettings Text="Population by Country, 2023" />
    <DxRangeSelectorChart>
        <DxChartBarSeries ArgumentField="@((PopulationPoint s) => s.Country)"
                          ValueField="@((PopulationPoint s) => s.Value)" />
    </DxRangeSelectorChart>
</DxRangeSelector>

@code {
    List<PopulationPoint> Data;
    protected override void OnInitialized() {
        Data = new List<PopulationPoint> {
            new PopulationPoint("India",  1428627663),
            new PopulationPoint("China",  1425671352),
            new PopulationPoint("USA",    339996563),
        };
    }
    record PopulationPoint(string Country, long Value);
}
```

- `DxRangeSelectorChart` accepts the same series child components as `DxChart` (e.g., `DxChartBarSeries`, `DxChartLineSeries`).
- The `Data` property on `DxRangeSelector` feeds the background chart.

## Title and Size

```razor
<DxRangeSelector Width="100%"
                 Height="200px">
    <DxTitleSettings Text="Select Date Range" />
    ...
</DxRangeSelector>
```

## Discrete Scale

For string-based categories, use `ValueType="ChartAxisDataType.String"`:

```razor
<DxRangeSelectorScale ValueType="ChartAxisDataType.String" />
```

Demo: `https://demos.devexpress.com/blazor/RangeSelectorDiscreteScale`

## Export

```razor
<DxRangeSelector @ref="Selector" ...>...</DxRangeSelector>
<button @onclick="Export">Export</button>

@code {
    DxRangeSelector Selector;
    async Task Export() {
        await Selector.ExportAsync("range.png", ChartExportFormat.PNG);
    }
}
```

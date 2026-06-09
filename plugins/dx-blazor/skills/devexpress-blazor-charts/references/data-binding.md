# Data Binding in DevExpress Blazor Charts

## When to Use This Reference

- Bind a chart to a synchronous C# collection
- Load data asynchronously from a service or API
- Use `ObservableCollection<T>` for automatic chart updates
- Bind to a `DataTable`
- Trigger chart updates when data changes
- Provide different data sources to individual series

## Synchronous Binding

Assign a collection to the component's `Data` property. Populate it in `OnInitialized`:

```razor
<DxChart Data="@ChartData">
    <DxChartLineSeries ValueField="@((DataPoint i) => i.Value)"
                       ArgumentField="@(i => i.Date)"
                       Name="Temperature, °C" />
    <DxChartTitle Text="Weather Forecast, °C" />
    <DxChartLegend Visible="false" />
</DxChart>

@code {
    IEnumerable<DataPoint> ChartData;

    protected override void OnInitialized() {
        ChartData = new List<DataPoint> {
            new DataPoint(DateTime.Today.AddDays(-4), 12.3),
            new DataPoint(DateTime.Today.AddDays(-3), 15.1),
            new DataPoint(DateTime.Today.AddDays(-2), 11.8),
            new DataPoint(DateTime.Today.AddDays(-1), 17.4),
            new DataPoint(DateTime.Today,             14.0),
        };
    }

    record DataPoint(DateTime Date, double Value);
}
```

To load from a DI-registered service instead:
```csharp
// @inject IMyForecastService ForecastService
// ChartData = ForecastService.GetForecast();
```

## Asynchronous Binding

Use `OnInitializedAsync` for async data loads. Replace `LoadDataAsync()` with your real source — an HTTP call, EF query, or DI service:

```razor
<DxChart Data="@ChartData">
    <DxChartLineSeries ValueField="@((DataPoint i) => i.Value)"
                       ArgumentField="@(i => i.Date)"
                       Name="Temperature, °C" />
    <DxChartTitle Text="Weather Forecast, °C" />
</DxChart>

@code {
    IEnumerable<DataPoint> ChartData;

    protected override async Task OnInitializedAsync() {
        ChartData = await LoadDataAsync();
    }

    // Replace with your real async source, e.g.:
    //   @inject HttpClient Http
    //   ChartData = await Http.GetFromJsonAsync<List<DataPoint>>("api/forecast");
    //   — or —
    //   @inject IMyForecastService ForecastService
    //   ChartData = await ForecastService.GetForecastAsync();
    static async Task<IEnumerable<DataPoint>> LoadDataAsync() {
        await Task.Delay(10); // simulate network latency
        return new List<DataPoint> {
            new DataPoint(DateTime.Today.AddDays(-4), 12.3),
            new DataPoint(DateTime.Today.AddDays(-3), 15.1),
            new DataPoint(DateTime.Today.AddDays(-2), 11.8),
            new DataPoint(DateTime.Today.AddDays(-1), 17.4),
            new DataPoint(DateTime.Today,             14.0),
        };
    }

    record DataPoint(DateTime Date, double Value);
}
```

## Observable Collections (Real-Time Updates)

Assign an `ObservableCollection<T>` — the chart automatically re-renders when items are added or removed:

```razor
<DxChart Data="@ChartData">
    <DxChartLineSeries ValueField="@((DataPoint i) => i.Value)"
                       ArgumentField="@(i => i.Date)"
                       Name="Real-Time" />
</DxChart>

@code {
    ObservableCollection<DataPoint> ChartData = new ObservableCollection<DataPoint>();

    protected override void OnInitialized() {
        // Seed with initial data
        ChartData.Add(new DataPoint(DateTime.Today.AddDays(-2), 10.0));
        ChartData.Add(new DataPoint(DateTime.Today.AddDays(-1), 12.5));
        ChartData.Add(new DataPoint(DateTime.Today,             11.0));
    }

    void AddDataPoint(DataPoint point) {
        ChartData.Add(point);
        // No StateHasChanged() needed — ObservableCollection notifies the chart
    }

    record DataPoint(DateTime Date, double Value);
}
```

## Per-Series Data Sources

Override the data source on the series level using its `Data` property. This is useful when different series have different data models:

```razor
<DxChart>
    <DxChartLineSeries Data="TemperatureData"
                       ArgumentField="@((TemperaturePoint v) => v.Date)"
                       ValueField="@((TemperaturePoint v) => v.Celsius)"
                       Name="Temperature" />
    <DxChartBarSeries Data="RainfallData"
                      ArgumentField="@((RainfallPoint v) => v.Date)"
                      ValueField="@((RainfallPoint v) => v.Amount)"
                      Name="Rainfall" />
</DxChart>
```

`TemperaturePoint` and `RainfallPoint` are placeholder names — use your actual model classes.

When `Data` is specified at the series level, the component-level `Data` property is ignored for that series.

## Filtering Data with Series Filter

Use the `Filter` property to show a subset of data in a series:

```razor
<DxChart Data="AllData">
    <DxChartScatterSeries ArgumentField="@((DataPoint v) => v.X)"
                          ValueField="@((DataPoint v) => v.Y)"
                          Filter="@((DataPoint v) => v.Y >= 0)"
                          Color="Color.FromArgb(0, 169, 230)" />
    <DxChartScatterSeries ArgumentField="@((DataPoint v) => v.X)"
                          ValueField="@((DataPoint v) => v.Y)"
                          Filter="@((DataPoint v) => v.Y < 0)"
                          Color="Color.FromArgb(220, 53, 69)" />
</DxChart>
```

## Triggering Chart Redraw

When you replace the data collection (not modify in place), Blazor detects the new reference and re-renders. When you modify an existing non-observable collection, call `StateHasChanged()`:

```razor
@code {
    List<DataPoint> DataSource = new();

    void UpdateData() {
        DataSource.Add(new DataPoint("New", 5.0));
        StateHasChanged(); // Required for List<T> — not required for ObservableCollection<T>
    }
}
```

## Troubleshooting

| Issue | Solution |
|---|---|
| Chart does not update after data changes | Use `ObservableCollection<T>` or call `StateHasChanged()` after modifying a `List<T>` |
| Chart is empty after async load | Ensure you `await` the data call and that `Data` binding updates correctly |
| Series type inference error | Use explicit lambda syntax: `@((MyType v) => v.Property)` |

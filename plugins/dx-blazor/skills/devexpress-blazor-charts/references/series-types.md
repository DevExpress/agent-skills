# Series Types in DevExpress Blazor Charts

## When to Use This Reference

- Choose the right series component tag for your chart type
- Understand which series are available for `DxChart`, `DxPieChart`, and `DxPolarChart`
- Configure special series (Financial, Bubble, Range) with additional data fields
- Combine multiple series on one chart
- Switch the chart type at runtime without compile errors

## Series Declaration Pattern

Specify `ArgumentField` and `ValueField` as lambda expressions. The generic type is inferred from the lambda. Replace `DataPoint` with your own data class (e.g., `record DataPoint(DateTime Date, double Value, double Value2 = 0, double Size = 1.0);`):

```razor
<DxChart Data="DataSource">
    <DxChartAreaSeries ArgumentField="@((DataPoint v) => v.Date)"
                       ValueField="@((DataPoint v) => v.Value)" />
</DxChart>
```

Alternatively, set type parameters explicitly on the series:
```razor
<DxChartAreaSeries T="DataPoint"
                   TArgument="DateTime"
                   TValue="double"
                   ArgumentField="s => s.Date"
                   ValueField="s => s.Value" />
```

## DxChart Series Types

### Area Series

| View | Component |
|---|---|
| Area | `DxChartAreaSeries` |
| Range Area | `DxChartRangeAreaSeries` |
| Spline Area | `DxChartSplineAreaSeries` |
| Step Area | `DxChartStepAreaSeries` |
| Stacked Area | `DxChartStackedAreaSeries` |
| Stacked Spline Area | `DxChartStackedSplineAreaSeries` |
| Full-Stacked Area | `DxChartFullStackedAreaSeries` |
| Full-Stacked Spline Area | `DxChartFullStackedSplineAreaSeries` |

```razor
<DxChart Data="DataSource">
    <DxChartAreaSeries ArgumentField="@((DataPoint v) => v.Date)"
                       ValueField="@((DataPoint v) => v.Value)" />
</DxChart>
```

### Bar Series

| View | Component |
|---|---|
| Bar | `DxChartBarSeries` |
| Range Bar | `DxChartRangeBarSeries` |
| Stacked Bar | `DxChartStackedBarSeries` |
| Full-Stacked Bar | `DxChartFullStackedBarSeries` |

```razor
<DxChart Data="DataSource">
    <DxChartBarSeries ArgumentField="@((DataPoint v) => v.Date)"
                      ValueField="@((DataPoint v) => v.Value)" />
</DxChart>
```

For side-by-side stacked bars, use the `Stack` property to group series:
```razor
<DxChart Data="DataSource">
    <DxChartStackedBarSeries ArgumentField="@((DataPoint v) => v.Date)"
                             ValueField="@((DataPoint v) => v.Value)"
                             Stack="group1" />
    <DxChartStackedBarSeries ArgumentField="@((DataPoint v) => v.Date)"
                             ValueField="@((DataPoint v) => v.Value2)"
                             Stack="group2" />
</DxChart>
```

### Line Series

| View | Component |
|---|---|
| Line | `DxChartLineSeries` |
| Spline | `DxChartSplineSeries` |
| Step Line | `DxChartStepLineSeries` |
| Stacked Line | `DxChartStackedLineSeries` |
| Stacked Spline | `DxChartStackedSplineSeries` |
| Full-Stacked Line | `DxChartFullStackedLineSeries` |
| Full-Stacked Spline | `DxChartFullStackedSplineSeries` |

```razor
<DxChart Data="DataSource">
    <DxChartLineSeries ArgumentField="@((DataPoint v) => v.Date)"
                       ValueField="@((DataPoint v) => v.Value)" />
</DxChart>
```

### Scatter Series

```razor
<DxChart Data="DataSource">
    <DxChartScatterSeries ArgumentField="@((DataPoint v) => v.Date)"
                          ValueField="@((DataPoint v) => v.Value)" />
</DxChart>
```

### Bubble Series

Requires an additional `SizeField`:

```razor
<DxChart Data="DataSource">
    <DxChartBubbleSeries ArgumentField="@((DataPoint v) => v.Date)"
                         ValueField="@((DataPoint v) => v.Value)"
                         SizeField="@((DataPoint v) => v.Size)" />
</DxChart>
```

### Financial Series (Candlestick / OHLC)

Requires `OpenField`, `HighField`, `LowField`, `CloseField`:

Your data class must expose `Open`, `High`, `Low`, `Close` (numeric) and a date/argument field:

```razor
<DxChart Data="StockData">
    <DxChartCandlestickSeries OpenField="(StockPoint sdp) => sdp.Open"
                              HighField="sdp => sdp.High"
                              LowField="sdp => sdp.Low"
                              CloseField="sdp => sdp.Close"
                              ArgumentField="sdp => sdp.Date"
                              Name="ACME Corp" />
</DxChart>

@code {
    // record StockPoint(DateTime Date, double Open, double High, double Low, double Close);
}
```

For OHLC chart, use `DxChartStockSeries` with the same fields.

## DxPieChart Series

`DxPieChart` supports exactly one series type: `DxPieChartSeries`. Set `InnerDiameter` on `DxPieChart` to create a donut:

```razor
<DxPieChart Data="DataSource" InnerDiameter="0.4">
    <DxPieChartSeries ArgumentField="@((DataPoint v) => v.Country)"
                      ValueField="@((DataPoint v) => v.AppleProduction)"
                      Name="Apples">
        <DxChartSeriesLabel Visible="true" />
    </DxPieChartSeries>
</DxPieChart>
```

Multiple series in `DxPieChart` result in multiple nested rings (donut rings).

## DxPolarChart Series

| View | Component |
|---|---|
| Area | `DxPolarChartAreaSeries` |
| Bar | `DxPolarChartBarSeries` |
| Stacked Bar | `DxPolarChartStackedBarSeries` |
| Line | `DxPolarChartLineSeries` |
| Scatter | `DxPolarChartScatterSeries` |

```razor
<DxPolarChart Data="DataSource" UseSpiderWeb="true">
    <DxPolarChartLineSeries ArgumentField="@((DataPoint v) => v.Country)"
                            ValueField="@((DataPoint v) => v.AppleProduction)"
                            Name="Apples" />
</DxPolarChart>
```

## Multiple Series

Add multiple series to `DxChart` or `DxPolarChart` — they appear on the same plane in declaration order (first is on top):

```razor
<DxChart Data="DataSource">
    <DxChartLineSeries ArgumentField="@((DataPoint v) => v.Date)"
                       ValueField="@((DataPoint v) => v.Value)"
                       Name="Series A" />
    <DxChartBarSeries ArgumentField="@((DataPoint v) => v.Date)"
                      ValueField="@((DataPoint v) => v.Value2)"
                      Name="Series B" />
</DxChart>
```

You can mix series types. Each series can override the `Data` property with its own data source.

---

## Switching Chart Type at Runtime

When the user needs to change the chart type (e.g., Bar ↔ Line ↔ Area) via a button or dropdown, use **conditional rendering** (`@if` / `@switch`) to swap specific series components. This is the recommended approach.

**NEVER use a bare `DxChartCommonSeries` without explicit type attributes** — Blazor cannot infer its four generic parameters and will emit **RZ10001**.

### Recommended: Conditional Rendering

```razor
<DxChart Data="@ChartData">
    @if (SeriesType == "Bar")
    {
        <DxChartBarSeries ArgumentField="@((DataPoint v) => v.Country)"
                          ValueField="@((DataPoint v) => v.Population)"
                          Name="Population" />
    }
    else if (SeriesType == "Line")
    {
        <DxChartLineSeries ArgumentField="@((DataPoint v) => v.Country)"
                           ValueField="@((DataPoint v) => v.Population)"
                           Name="Population" />
    }
    else
    {
        <DxChartAreaSeries ArgumentField="@((DataPoint v) => v.Country)"
                           ValueField="@((DataPoint v) => v.Population)"
                           Name="Population" />
    }
    <DxChartTitle Text="Population by Country" />
</DxChart>

@code {
    string SeriesType { get; set; } = "Bar";

    record DataPoint(string Country, double Population);
    IEnumerable<DataPoint> ChartData = new List<DataPoint>
    {
        new("Germany", 83.2), new("France", 68.0), new("USA", 331.0)
    };
}
```

### Alternative: DxChartCommonSeries with Explicit Type Arguments

`DxChartCommonSeries` lets you change `SeriesType` via a property without conditional markup, but **all four type parameters must be specified explicitly** — Blazor cannot infer them:

```razor
@* WRONG — causes RZ10001: type cannot be inferred *@
<DxChartCommonSeries ArgumentField="s => s.Country"
                     ValueField="s => s.Population"
                     SeriesType="@CurrentType" />

@* CORRECT — all four type arguments are explicit *@
<DxChartCommonSeries T="DataPoint"
                     TGroup="string"
                     TValue="double"
                     TArgument="string"
                     ArgumentField="s => s.Country"
                     ValueField="s => s.Population"
                     SeriesType="@CurrentType" />
```

`DxChartCommonSeries` `SeriesType` accepts a `ChartBarSeriesType`, `ChartLineSeriesType`, etc. enum value. Use this approach only when the series type changes frequently — otherwise prefer conditional rendering.

---

## Troubleshooting

| Symptom | Fix |
|---|---|
| **RZ10001**: type of `DxChartCommonSeries` cannot be inferred | Add all four explicit type attributes: `T`, `TGroup`, `TValue`, `TArgument`. Or switch to conditional rendering with typed series components. |
| Wrong series rendered after type switch | Ensure the `@if` / `@switch` expression triggers a re-render (field is a `property`, not a local variable) |

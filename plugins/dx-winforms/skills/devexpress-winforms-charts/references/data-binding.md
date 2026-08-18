# Data Binding

This reference covers every way to feed data into a `ChartControl`: per-series `DataSource` + member names, the chart-level `DataAdapter`, the `SeriesTemplate` pattern that auto-creates one series per category, `SetFinancialDataMembers` for OHLC, and runtime updates via `RefreshData`.

For the element hierarchy see [getting-started.md](getting-started.md). For series view choice see [series-and-diagrams.md](series-and-diagrams.md).

## When to Use This Reference

- Binding a `Series` to a `DataTable`, `IList<T>`, `BindingList<T>`, `DbSet<T>`, `DataView`, or any `IEnumerable`.
- Auto-creating multiple series from a single flat data source via `SeriesTemplate` + `SeriesDataMember`.
- Binding candlestick / stock series (OHLC values).
- Driving the chart from a runtime data update (event-driven, polling, streaming).
- Choosing between per-series binding and chart-level binding.

## Two Binding Models

| Model | Where you set `DataSource` | When |
|---|---|---|
| Per-series | `Series.DataSource`, `Series.ArgumentDataMember`, `Series.ValueDataMembers` | You add each series explicitly; each series can use a different source. |
| Chart-level (template) | `ChartControl.DataSource`, `ChartControl.SeriesDataMember`, `ChartControl.SeriesTemplate` | One flat data source contains all categories; one series per category should be auto-generated. |

You can mix the two — explicit series sit alongside template-generated ones.

## Per-Series Binding

```csharp
public record SalesRow(DateTime Month, decimal Revenue);

var series = new Series("Revenue", ViewType.Line);
series.DataSource         = LoadSales();
series.ArgumentDataMember = nameof(SalesRow.Month);
series.ValueDataMembers.AddRange(nameof(SalesRow.Revenue));
series.ArgumentScaleType  = ScaleType.DateTime;

chart.Series.Add(series);
```

### Key Properties

| Member | Purpose |
|---|---|
| `Series.DataSource` | The data source (anything that implements `IEnumerable`, `IList`, `IListSource`, `DataTable`, `DataView`). For a `DataSet`, point `DataSource` at the table directly (`dataSet.Tables["Sales"]`) — `Series` has no `DataMember`. |
| `Series.DataAdapter` | Optional custom `DataSourceAdapter` (rarely needed). |
| `Series.ArgumentDataMember` | Column / property providing the **argument** (X) for each point. |
| `Series.ValueDataMembers` | Collection of columns / properties providing the **value(s)** (Y). Most series take one value; financial series take four; bubble takes two; range-area takes two. |
| `Series.ArgumentScaleType` | `Auto` / `Numerical` / `DateTime` / `DateTimeOffset` / `TimeSpan` / `Qualitative`. Set explicitly to avoid `Auto` cost. |
| `Series.ValueScaleType` | Same enum; usually `Numerical` (the default). |
| `Series.QualitativeSummaryOptions.SummaryFunction` | How to aggregate duplicate arguments — a `string` expression such as `"SUM([Value])"` / `"AVERAGE([Value])"` / `"COUNT()"` (empty = keep duplicates). |

### `ValueDataMembers` Per Series View

| Series View | Required `ValueDataMembers` (in order) |
|---|---|
| Bar / Line / Spline / StepLine / Area / Point / Pie / Funnel | `Value` |
| Bubble | `Value`, `Weight` |
| RangeArea / RangeBar / SideBySideRangeBar | `Value1`, `Value2` |
| CandleStick / Stock | `Low`, `High`, `Open`, `Close` |
| Gantt (side-by-side / overlapped) | `Start`, `End` |
| BoxPlot | `Min`, `Quartile1`, `Median`, `Quartile3`, `Max`, optionally `Mean`, optionally outliers |

Use `series.SetDataMembers(args, values)` as a shortcut, or `SetFinancialDataMembers` for OHLC.

## Chart-Level Template Binding (Auto-Create Series)

When the data source has a *category column* and rows of one category should become a single series:

```text
| Region | Month   | Revenue |
| North  | 2026-01 |  120k   |
| North  | 2026-02 |  145k   |
| South  | 2026-01 |  103k   |
| South  | 2026-02 |  131k   |
```

→ one series per `Region`, with `Month` as argument and `Revenue` as value.

```csharp
chart.DataSource       = sales;
chart.SeriesDataMember = nameof(SalesRow.Region);              // groups rows into series
chart.SeriesTemplate.ArgumentDataMember = nameof(SalesRow.Month);
chart.SeriesTemplate.ValueDataMembers.AddRange(nameof(SalesRow.Revenue));
chart.SeriesTemplate.ArgumentScaleType  = ScaleType.DateTime;
chart.SeriesTemplate.View               = new LineSeriesView();
chart.SeriesTemplate.LegendTextPattern  = "{S}";
```

### Key Properties

| Member | Purpose |
|---|---|
| `ChartControl.DataSource` / `DataAdapter` | Chart-wide data source (no chart-level `DataMember` — bind to the table directly). |
| `ChartControl.SeriesDataMember` | Column that splits rows into series. |
| `ChartControl.SeriesTemplate` | A "prototype" `Series` whose settings (view, scale, label, legend text) are copied to each auto-generated series. |
| `ChartControl.AutoBindingSettingsEnabled` | When `true`, applies the template; when `false`, you bind series manually. |

The template's `Name`, `Color`, and `View` color are overridden per generated series so each gets its own legend label and palette color.

### When to Use Template vs Per-Series

- **Per-series** — fixed, small number of series with different data sources or markedly different appearance.
- **Template** — variable number of categories known only at runtime, all sharing one source.

## Financial Series — `SetFinancialDataMembers`

```csharp
var ohlc = new Series("AAPL", ViewType.CandleStick);
ohlc.DataSource = quotes;
ohlc.ArgumentScaleType  = ScaleType.DateTime;
// Positional args: (argument, low, high, open, close)
ohlc.SetFinancialDataMembers(
    nameof(Quote.Date),
    nameof(Quote.Low),
    nameof(Quote.High),
    nameof(Quote.Open),
    nameof(Quote.Close));
chart.Series.Add(ohlc);
```

`SetFinancialDataMembers(argument, low, high, open, close)` sets the `ArgumentDataMember` and the four `ValueDataMembers` indexes in the correct order.

## Bubble Series

```csharp
var bubble = new Series("Markets", ViewType.Bubble);
bubble.DataSource         = markets;
bubble.ArgumentDataMember = nameof(Market.Population);
bubble.ValueDataMembers.AddRange(nameof(Market.GDPPerCapita), nameof(Market.MarketSize));
```

Index 0 → Y value; index 1 → bubble Weight (radius).

## Adding Points Without Binding

```csharp
series.Points.Add(new SeriesPoint("Apples",   58));
series.Points.Add(new SeriesPoint("Oranges",  42));
series.Points.Add(new SeriesPoint("Bananas",  35));
```

Use unbound mode for very small data sets, calculated values, or when the chart should keep state through filtering operations.

`SeriesPoint(arg, values…)` supports up to four values (for OHLC) and per-point color via `SeriesPoint.Color`.

## Runtime Data Updates

### Single-source mutation

If `DataSource` is an `IBindingList` (e.g., `BindingList<T>`), adding/removing/changing items raises `ListChanged` and the chart updates automatically.

### Bulk update with `RefreshData`

For non-binding sources (plain `List<T>`, custom collections) call:

```csharp
chart.RefreshData();           // re-reads every series' DataSource
```

### Batch updates without flicker

```csharp
chart.BeginInit();
try {
    series.Points.Clear();
    foreach (var p in newPoints) series.Points.Add(p);
} finally {
    chart.EndInit();
}
```

`BeginInit`/`EndInit` suppresses redraws until `EndInit`.

### Streaming / sliding window

```csharp
var window = TimeSpan.FromMinutes(5);
series.Points.Add(new SeriesPoint(DateTime.Now, latestValue));

var cutoff = DateTime.Now - window;
while (series.Points.Count > 0 && (DateTime)series.Points[0].Argument < cutoff)
    series.Points.RemoveAt(0);
```

For very high-frequency updates (> 100 Hz / millions of points), switch to `SwiftPlotDiagram`.

## Argument and Value Scale Types

`ScaleType.Auto` is the default — the control infers the type from the first non-null value in `ArgumentDataMember`. Set the scale type explicitly to:

- Avoid the runtime cost of type inference.
- Force `DateTime` interpretation when data is a `string` in ISO format.
- Treat numeric IDs as **categories** by setting `ArgumentScaleType = ScaleType.Qualitative`.

| `ScaleType` | When |
|---|---|
| `Numerical` | `int` / `double` / `decimal`. |
| `DateTime` | `DateTime` (date-time axis). |
| `DateTimeOffset` | `DateTimeOffset`. |
| `TimeSpan` | `TimeSpan` (duration axis). |
| `Qualitative` | `string` or any non-comparable category. |

See [axes.md](axes.md) for what each scale type does on the axis side.

## Common Patterns

### Pattern 1 — Bind a `DataTable`

```csharp
series.DataSource         = dataSet1.Tables["Sales"];
series.ArgumentDataMember = "Month";
series.ValueDataMembers.AddRange("Total");
```

### Pattern 2 — Bind to EF / `DbSet`

```csharp
series.DataSource         = dbContext.Sales.Local.ToBindingList();   // IBindingList for auto-refresh
series.ArgumentDataMember = nameof(Sale.Month);
series.ValueDataMembers.AddRange(nameof(Sale.Total));
```

### Pattern 3 — Multiple series from grouped LINQ

```csharp
var grouped = sales.GroupBy(s => s.Region)
                   .Select(g => new { Region = g.Key, Rows = g.ToList() });

foreach (var g in grouped) {
    var s = new Series(g.Region, ViewType.Line);
    s.DataSource = g.Rows;
    s.ArgumentDataMember = nameof(Sale.Month);
    s.ValueDataMembers.AddRange(nameof(Sale.Total));
    chart.Series.Add(s);
}
```

### Pattern 4 — Template + chart-level source

```csharp
chart.DataSource              = sales;
chart.SeriesDataMember        = nameof(Sale.Region);
chart.SeriesTemplate.ArgumentDataMember = nameof(Sale.Month);
chart.SeriesTemplate.ValueDataMembers.AddRange(nameof(Sale.Total));
chart.SeriesTemplate.View     = new LineSeriesView();
```

### Pattern 5 — Refresh on timer

```csharp
var timer = new System.Windows.Forms.Timer { Interval = 1000 };
timer.Tick += (s, e) => chart.RefreshData();
timer.Start();
```

## Common Issues

- **Legend shows `Series 1`, `Series 2`, …** — set `LegendTextPattern = "{S}"` on the template, and set the auto-generated series' `Name` via `SeriesDataMember`.
- **Empty chart after binding** — `ValueDataMembers` empty, or the member name is misspelled (`ValueDataMembers` is case-sensitive). Check `chart.Series[0].Points.Count`.
- **Dates show as numbers on the axis** — `ArgumentScaleType` left at `Auto` while the column is typed `object`. Set `ScaleType.DateTime` explicitly.
- **Duplicate arguments collapse incorrectly** — set `series.QualitativeSummaryOptions.SummaryFunction = ""` (empty string) to keep every point.
- **Template generates one series per row** — `SeriesDataMember` points to a column that is unique per row. Use a category column instead.
- **`RefreshData` does nothing** — series uses `Points.Add` (unbound). `RefreshData` only re-reads `DataSource`.
- **Memory leak on form close** — unsubscribe from `IBindingList.ListChanged` and dispose the chart before disposing the data source.

## Source Material

- `articles/controls-and-libraries/chart-control/end-user-features/providing-data.md` (`xref:WindowsForms.6022`).
- `articles/controls-and-libraries/chart-control/providing-data/series-data-binding.md` (`xref:WindowsForms.117571`).
- `articles/controls-and-libraries/chart-control/providing-data/series-template.md` (`xref:WindowsForms.117573`).
- `api/DevExpress.XtraCharts.Series.DataSource.yml`.
- `api/DevExpress.XtraCharts.SeriesBase.ArgumentDataMember.yml`.
- `api/DevExpress.XtraCharts.SeriesBase.ValueDataMembers.yml`.
- `api/DevExpress.XtraCharts.ChartControl.SeriesTemplate.yml`.
- `api/DevExpress.XtraCharts.ChartControl.RefreshData.yml`.

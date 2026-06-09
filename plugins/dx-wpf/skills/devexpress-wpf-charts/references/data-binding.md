# Data Binding — DevExpress WPF Chart Control

A `ChartControl` is populated by **series**, and each series is bound to a data source either through `ChartControl.DataSource` (most common) or via its own `Series.DataSource`. The series then reads point arguments and values from named fields on the data items. Beyond that, the chart supports **automatic series creation** from a ViewModel collection (`Diagram.SeriesItemsSource` / `SeriesItemTemplate`) and **manual point construction** (no data source — points defined inline).

## When to Use This Reference

Use this when you need to:

- Bind a chart to `IEnumerable<T>`, `ObservableCollection<T>`, `DataTable`, `IListSource`
- Map a series's X / Y to data fields via `ArgumentDataMember` / `ValueDataMember`
- Bind bubble / range / financial series with extra value fields
- Generate series dynamically from a ViewModel collection (`Diagram.SeriesItemsSource` + `SeriesItemTemplate`)
- Add series points manually without a data source

## Acceptable Data Source Types

`ChartControl.DataSource` and `Series.DataSource` accept anything that implements:

- `System.Collections.IEnumerable` — `List<T>`, `ObservableCollection<T>`, arrays
- `System.ComponentModel.IListSource` — `DataTable`, `DataView`
- Their descendants (LINQ query results, EF `DbSet`, etc.)

For dynamic data, prefer `ObservableCollection<T>` — the chart redraws when items are added/removed. Plain `List<T>` shows only the initial snapshot.

## Approach 1: Bind via `ChartControl.DataSource` (Common Case)

Set `DataSource` once at the chart level; each series declares its data field bindings.

```xaml
<dxc:ChartControl DataSource="{Binding Sales}">
    <dxc:XYDiagram2D>
        <dxc:BarSideBySideSeries2D DisplayName="Revenue"
                                   ArgumentDataMember="Country"
                                   ValueDataMember="Amount"/>
    </dxc:XYDiagram2D>
</dxc:ChartControl>
```

```csharp
public class SalePoint {
    public string Country { get; set; } = "";
    public double Amount { get; set; }
}

public class MainViewModel {
    public ObservableCollection<SalePoint> Sales { get; } = new() {
        new() { Country = "USA",    Amount = 320 },
        new() { Country = "Canada", Amount = 180 },
        new() { Country = "UK",     Amount = 240 },
    };
}
```

### Field-Member Properties on `Series`

| Property | Use |
|---|---|
| `ArgumentDataMember` | The argument (X) field name |
| `ValueDataMember` | The value (Y) field name |
| `ArgumentScaleType` | `Auto` (default), `Numerical`, `DateTime`, `TimeSpan`, `Qualitative` |
| `ValueScaleType` | Same set; usually `Numerical` |
| `DataSource` | Per-series override (use only if this series needs a *different* source than the chart) |
| `FilterCriteria` | Optional `CriteriaOperator` to filter rows |

> Names are **case-sensitive** and must match the data type's property names exactly.

### Series-Specific Extra Members

Some series need more than one Y value per point:

| Series | Extra members |
|---|---|
| `BubbleSeries2D` | `WeightDataMember` (bubble size) |
| `StockSeries2D` / `CandleStickSeries2D` | `OpenValueDataMember`, `HighValueDataMember`, `LowValueDataMember`, `CloseValueDataMember` |
| Range series (`RangeAreaSeries2D`, range bars) | `Value1DataMember`, `Value2DataMember` |
| `BoxPlotSeries2D` | `MinValueDataMember`, `Quartile1DataMember`, `MedianValueDataMember`, `Quartile3DataMember`, `MaxValueDataMember` |

Example — candlestick:

```xaml
<dxc:CandleStickSeries2D ArgumentDataMember="Date"
                         OpenValueDataMember="Open"
                         HighValueDataMember="High"
                         LowValueDataMember="Low"
                         CloseValueDataMember="Close"
                         ArgumentScaleType="DateTime"/>
```

## Approach 2: Per-Series Data Source

If two series have different data sources, set `Series.DataSource` per series and omit `ChartControl.DataSource`:

```xaml
<dxc:ChartControl>
    <dxc:XYDiagram2D>
        <dxc:LineSeries2D DisplayName="2023 Sales"
                          DataSource="{Binding Data2023}"
                          ArgumentDataMember="Month"
                          ValueDataMember="Amount"/>
        <dxc:LineSeries2D DisplayName="2024 Sales"
                          DataSource="{Binding Data2024}"
                          ArgumentDataMember="Month"
                          ValueDataMember="Amount"/>
    </dxc:XYDiagram2D>
</dxc:ChartControl>
```

## Approach 3: Manual Series Points (No Data Source)

For static or programmatically generated points:

```xaml
<dxc:LineSeries2D DisplayName="Q1 Sales">
    <dxc:SeriesPoint Argument="Jan" Value="120"/>
    <dxc:SeriesPoint Argument="Feb" Value="180"/>
    <dxc:SeriesPoint Argument="Mar" Value="210"/>
</dxc:LineSeries2D>
```

Use this for quick prototypes or charts with truly fixed data. Most real apps go through a data source.

## Approach 4: MVVM — Generate Series from ViewModel Collection

Bind a collection of "series descriptor" objects to the **Diagram's** `SeriesItemsSource` and provide a `SeriesItemTemplate`. The chart creates one series per item.

> `SeriesItemsSource` / `SeriesItemTemplate` (and the legacy `SeriesDataMember` / `SeriesTemplate`) live on the **Diagram**, not on `ChartControl`.

```csharp
public class SeriesVM {
    public string Name { get; set; } = "";
    public IEnumerable<SalePoint> Points { get; set; } = Array.Empty<SalePoint>();
}

public class MainViewModel {
    public ObservableCollection<SeriesVM> Series { get; } = new() {
        new() { Name = "2023", Points = LoadYear(2023) },
        new() { Name = "2024", Points = LoadYear(2024) },
    };
}
```

```xaml
<dxc:ChartControl>
    <dxc:XYDiagram2D SeriesItemsSource="{Binding Series}">
        <dxc:XYDiagram2D.SeriesItemTemplate>
            <DataTemplate>
                <dxc:LineSeries2D DisplayName="{Binding Name}"
                                  DataSource="{Binding Points}"
                                  ArgumentDataMember="Month"
                                  ValueDataMember="Amount"/>
            </DataTemplate>
        </dxc:XYDiagram2D.SeriesItemTemplate>
    </dxc:XYDiagram2D>
</dxc:ChartControl>
```

### Template Selector for Mixed Series Types

For different visualizations per item, use a `DataTemplateSelector`:

```csharp
public class SeriesTemplateSelector : DataTemplateSelector {
    public DataTemplate? Line { get; set; }
    public DataTemplate? Bar { get; set; }

    public override DataTemplate? SelectTemplate(object item, DependencyObject container) {
        var vm = (SeriesVM)item;
        return vm.UseBar ? Bar : Line;
    }
}
```

```xaml
<dxc:XYDiagram2D SeriesItemsSource="{Binding Series}"
                 SeriesItemTemplateSelector="{StaticResource SeriesPicker}"/>
```

## Update Behavior

| Data source | Updates the chart? |
|---|---|
| `List<T>` — replace the list reference | Yes (binding update) |
| `List<T>` — `Add` / `Remove` items | **No** — chart doesn't observe `List<T>` |
| `ObservableCollection<T>` — `Add` / `Remove` | Yes — fires `INotifyCollectionChanged` |
| `INotifyPropertyChanged` on point properties | Yes — point appearance/value updates |
| `DataTable` — `Rows.Add` | Yes |

For high-frequency updates (real-time data), see `articles/.../provide-data/best-practices-display-large-data.md`.

## Filtering Series Data

`Series.FilterCriteria` accepts a `CriteriaOperator` to filter the bound data:

```xaml
<dxc:LineSeries2D DisplayName="Recent"
                  ArgumentDataMember="Date"
                  ValueDataMember="Sales"
                  FilterString="[Date] &gt;= #2024-01-01#"/>
```

Or in code:

```csharp
series.FilterCriteria = CriteriaOperator.Parse("[Region] = 'Asia'");
```

## Common Issues

- **Chart axes show but data is empty** — `ArgumentDataMember` / `ValueDataMember` mismatch the data class's property names. Names are case-sensitive.
- **`List<T>` updates don't appear in the chart** — `List<T>` doesn't notify on changes. Use `ObservableCollection<T>`.
- **Wrong scale type detected** — `Auto` scale ran on partial data. Set `ArgumentScaleType` explicitly to override.
- **Numeric values plotted as strings (qualitative)** — data field is `string` but should be `double`/`int`. Either change the data type or set `ArgumentScaleType="Numerical"`.
- **Two series share one data source but want different filters** — use `Series.FilterCriteria` or `Series.FilterString` per series.
- **`SeriesItemTemplate` template doesn't apply binding** — template root must contain the series element directly; ensure `DataContext` flows through.

## Source Material

- `articles/controls-and-libraries/charts-suite/chart-control/provide-data/provide-data.md` (https://docs.devexpress.com/content/WPF/6854?md=true)
- `articles/controls-and-libraries/charts-suite/chart-control/provide-data/bind-a-series-to-a-data-source.md` (https://docs.devexpress.com/content/WPF/7921?md=true)
- `articles/controls-and-libraries/charts-suite/chart-control/provide-data/create-a-series-manually.md` (https://docs.devexpress.com/content/WPF/7919?md=true)
- `articles/controls-and-libraries/charts-suite/chart-control/provide-data/add-points-to-a-series-manually.md` (https://docs.devexpress.com/content/WPF/7920?md=true)
- `articles/controls-and-libraries/charts-suite/chart-control/provide-data/define-a-template-for-automatic-series.md` (https://docs.devexpress.com/content/WPF/7922?md=true)
- `articles/controls-and-libraries/charts-suite/chart-control/provide-data/series-scale-types.md` (https://docs.devexpress.com/content/WPF/8453?md=true)

# Chart Integration — DevExpress WPF Pivot Grid

`ChartControl` can pull visible Pivot Grid data — already aggregated and filtered — through `PivotGridControl.ChartDataSource`. This produces a live chart that reshapes automatically as the end user reorganizes the pivot.

## When to Use This Reference

Use this when you need to:

- Show a chart that mirrors the Pivot Grid's visible data
- Switch between row-as-series and column-as-series chart orientations
- Limit how many series/points the chart receives
- Chart only the user's selected pivot cells (live drill-into-chart)
- Customize the data type or values sent to the chart

## Required Package

```bash
dotnet add package DevExpress.Wpf.Charts
```

XAML namespace: `xmlns:dxc="http://schemas.devexpress.com/winfx/2008/xaml/charts"`.

## Basic Setup

```xaml
<dxpg:PivotGridControl x:Name="pivot"
                       DataSource="{Binding Sales}"
                       ChartProvideDataByColumns="True"
                       ChartSelectionOnly="False">
    <!-- field configuration -->
</dxpg:PivotGridControl>

<dxc:ChartControl DataSource="{Binding ElementName=pivot, Path=ChartDataSource}">
    <dxc:XYDiagram2D SeriesDataMember="Series">
        <dxc:XYDiagram2D.SeriesTemplate>
            <dxc:LineSeries2D ArgumentDataMember="Arguments"
                              ValueDataMember="Values"/>
        </dxc:XYDiagram2D.SeriesTemplate>
    </dxc:XYDiagram2D>
</dxc:ChartControl>
```

`ChartDataSource` exposes three implicit data members: **Series**, **Arguments**, **Values** — wire them to `SeriesDataMember`, `ArgumentDataMember`, `ValueDataMember`.

## How Pivot Layout Maps to Chart

With `ChartProvideDataByColumns="True"`:

| Pivot element | Chart element |
|---|---|
| Column field values | **Series** (one chart series per pivot column) |
| Row field values | **Arguments** (X-axis positions) |
| Data cell values | **Values** (Y-axis) |

With `ChartProvideDataByColumns="False"`: rows become series; columns become arguments.

If multiple fields stack in an area (e.g., Year + Quarter in Column area), their values are joined with `|` — for example, `"2025 | Q1"`, `"2025 | Q2"`, `"2025 | Q3"`.

## Tuning Properties

### Data Selection

| Property | Use |
|---|---|
| `ChartProvideDataByColumns` | Swap row-to-series / column-to-argument mapping. |
| `ChartProvideEmptyCells` | Emit chart points for blank pivot cells. |
| `ChartFieldValuesProvideMode` | Which field values to send (e.g., all levels vs leaves only). |
| `ChartSelectionOnly` | When `true`, chart shows only data from selected pivot cells. |
| `ChartUpdateDelay` | Debounce updates while the user drags fields around. |

### Total Handling

| Property | Use |
|---|---|
| `ChartProvideColumnGrandTotals` | Include column Grand Totals as chart points. |
| `ChartProvideColumnCustomTotals` | Include column custom totals. |
| `ChartProvideRowGrandTotals` | Include row Grand Totals. |
| `ChartProvideRowCustomTotals` | Include row custom totals. |
| `ChartProvideRowTotals` | Master flag for row-side totals. |

### Limits

| Property | Default | Use |
|---|---|---|
| `ChartMaxSeriesCount` | non-zero | Cap on chart series count. **Set to 0 to disable.** |
| `ChartMaxPointCountInSeries` | non-zero | Cap on points per series. **Set to 0 to disable.** |

### Type Conversion

```csharp
pivotGridControl1.ChartProvideCellValuesAsType = typeof(double);
pivotGridControl1.ChartProvideColumnFieldValuesAsType = typeof(string);
pivotGridControl1.ChartProvideRowFieldValuesAsType = typeof(string);
```

The chart only supports **numeric or DateTime** series-point values. If a cell value can't be converted, the pivot sends `0`. For non-trivial conversion, handle `CustomChartDataSourceData`:

```csharp
pivotGridControl1.CustomChartDataSourceData += (s, e) => {
    // Adjust e.Value (or other args — verify exact arg shape against your version).
};
```

## Constraints

- The chart only supports series with **a single value per point**. Multi-value series (high-low, stock) aren't compatible.
- The chart receives data **after** pivot filters and aggregation — you cannot draw raw rows that the pivot has aggregated away.
- Conditional formatting on the pivot does **not** flow to the chart.

## Pattern: Drill-Into-Chart by Cell Selection

```xaml
<dxpg:PivotGridControl x:Name="pivot"
                       ChartSelectionOnly="True"/>
<!-- User Ctrl-clicks several cells; the chart redraws to show only those. -->
```

Combine with cell selection events to update a side-panel chart based on what the user has clicked.

## Pattern: Bar Chart of Sales by Country, Series per Year

```xaml
<dxc:ChartControl DataSource="{Binding ElementName=pivot, Path=ChartDataSource}">
    <dxc:XYDiagram2D SeriesDataMember="Series">
        <dxc:XYDiagram2D.SeriesTemplate>
            <dxc:BarSideBySideSeries2D ArgumentDataMember="Arguments"
                                       ValueDataMember="Values"/>
        </dxc:XYDiagram2D.SeriesTemplate>
    </dxc:XYDiagram2D>
</dxc:ChartControl>
```

> `ChartControl` can host **only one** `Diagram` at a time (`XYDiagram2D`, `SimpleDiagram2D`, `XYDiagram3D`, etc. — pick one). To switch diagrams at runtime, swap the diagram in code or via a `DataTemplate`.

## See Also

For an even richer dashboard experience (combining Pivot Grid + Chart + Map + KPI cards in one canvas), see DevExpress Dashboard (`xref:116677`).

## Source Material

- `articles/controls-and-libraries/pivot-grid/data-analysis/integration-with-the-chart-control.md` (`xref:8016`)
- `articles/controls-and-libraries/pivot-grid/examples/miscellaneous/how-to-visualize-data-from-a-pivot-grid-control-using-the-chart-control.md`
- `articles/controls-and-libraries/pivot-grid/examples/miscellaneous/how-to-customize-a-pivotgrid-controls-data-before-displaying-it-in-a-chart-control.md`

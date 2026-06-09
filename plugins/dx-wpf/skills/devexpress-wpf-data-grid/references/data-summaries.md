# Data Summaries — DevExpress WPF Data Grid

The grid displays aggregate values (sum, count, average, min/max, etc.) as **total summaries** (one set for the whole data set) and **group summaries** (one set per group row). Both kinds use `GridSummaryItem`.

## When to Use This Reference

Use this when you need to:

- Display the total count, sum, or average for a column
- Show summaries inside group rows
- Calculate summaries only against selected rows
- Recalculate summaries when the bound source changes
- Define summaries in a ViewModel and bind them

## Key Classes

| Class | Purpose |
|---|---|
| `DevExpress.Xpf.Grid.GridSummaryItem` | One summary line. Specifies `FieldName`, `SummaryType`, `DisplayFormat`, `CalculationMode`. |
| `DevExpress.Xpf.Grid.GridControl.TotalSummary` | Collection of total summaries (calculated against all rows). |
| `DevExpress.Xpf.Grid.GridControl.GroupSummary` | Collection of group summaries (calculated per group). |
| `DevExpress.Xpf.Grid.GridSummaryCalculationMode` | `AllRows`, `SelectedRows`, or `Mixed`. |
| `DevExpress.Xpf.Grid.SummaryItemBase.CalculationMode` | Per-summary override of view-level calculation mode. |
| `DevExpress.Xpf.Grid.DataViewBase.SummaryCalculationMode` | View-level summary calculation mode. |

## Total Summary

```xaml
<dxg:GridControl ItemsSource="{Binding Orders}">
    <dxg:GridControl.TotalSummary>
        <dxg:GridSummaryItem SummaryType="Count" Alignment="Right"/>
        <dxg:GridSummaryItem FieldName="Freight" SummaryType="Sum"
                             DisplayFormat="Total=${0:N2}"/>
    </dxg:GridControl.TotalSummary>
    <dxg:GridControl.View>
        <dxg:TableView TotalSummaryPosition="Bottom" ShowFixedTotalSummary="True"/>
    </dxg:GridControl.View>
</dxg:GridControl>
```

`SummaryType` values: `None`, `Sum`, `Min`, `Max`, `Count`, `Average`, `Custom`.

## Group Summary

Group summaries render inside group rows. Same `GridSummaryItem` shape, different collection:

```xaml
<dxg:GridControl.GroupSummary>
    <dxg:GridSummaryItem FieldName="Freight" SummaryType="Sum"
                         DisplayFormat="Group Total=${0:N2}"/>
</dxg:GridControl.GroupSummary>
```

## Summary for Selection

To calculate summaries only against selected rows:

```xaml
<dxg:TableView SelectionMode="Row"
               SummaryCalculationMode="SelectedRows"
               TotalSummaryPosition="Bottom"/>
```

`SelectedRows` → summary against selected rows only.
`AllRows` → ignores selection.
`Mixed` → uses selected rows if more than one is selected; otherwise all rows.

Per-summary override:

```xaml
<dxg:GridSummaryItem FieldName="Total" SummaryType="Sum"
                     DisplayFormat="Selection Total=${0:N}"
                     CalculationMode="SelectedRows"/>
<dxg:GridSummaryItem FieldName="Total" SummaryType="Sum"
                     DisplayFormat="Grand Total=${0:N}"/>
```

> Server-mode and `ICollectionView` / virtual-source grids **do not** calculate summaries for selection.

## Recalculate on Data Changes

When the bound data source changes externally (not through the grid UI), the grid does not recalculate summaries unless you opt in:

```xaml
<dxg:GridControl AllowLiveDataShaping="True"/>
```

Or call `CommitEditing` from `CellValueChanged`:

```csharp
private void View_CellValueChanged(object sender, CellValueChangedEventArgs e) {
    view.CommitEditing();
}
```

## Optimized Recalculation

For data sources that implement `INotifyPropertyChanged` and `INotifyPropertyChanging`, enable optimized recalculation:

```xaml
<dxg:GridControl OptimizeSummaryCalculation="True"/>
```

The grid must be bound to `ObservableCollection<T>` or `DevExpress.Xpf.ChunkList.ChunkList<T>` with items implementing the two interfaces. Custom summaries and unbound-column summaries are not optimized.

## Custom Summary

For aggregates that don't fit `Sum/Min/Max/Count/Average`, use `SummaryType="Custom"` and handle the `GridControl.CustomSummary` event:

```csharp
// TODO: Verify exact event arg names (CustomSummaryEventArgs members).
// Use DxDocs MCP: devexpress_docs_search(technology="WPF Data Grid", query="CustomSummary event")
```

See `articles/controls-and-libraries/data-grid/data-summaries/` for the full custom summary article (https://docs.devexpress.com/content/WPF/6129?md=true).

## ViewModel-Defined Summaries

The grid supports binding `TotalSummary` / `GroupSummary` to ViewModel properties. See the GitHub example linked in `articles/controls-and-libraries/data-grid/data-summaries.md`: "How to Bind the GridControl to Total and Group Summaries Specified in a ViewModel".

## Source Material

- `articles/controls-and-libraries/data-grid/data-summaries.md`

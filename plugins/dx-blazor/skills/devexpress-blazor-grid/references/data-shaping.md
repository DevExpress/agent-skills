# Data Shaping — Blazor Grid

When you need to: sort, group, filter, or summarize grid data; configure the filter row, filter panel, search box, or column filter menu; create total or group summaries.

## Sorting

### Sort in the UI
Users click column headers to sort. Hold `Shift` to multi-sort; hold `Ctrl` to clear a column's sort.

Disable sorting globally:
```razor
<DxGrid AllowSort="false" ...>
```

Disable sorting per column:
```razor
<DxGridDataColumn FieldName="Id" AllowSort="false" />
```

### Initial Sort in Markup

```razor
<DxGridDataColumn FieldName="Name" SortOrder="GridColumnSortOrder.Ascending" SortIndex="0" />
<DxGridDataColumn FieldName="Price" SortOrder="GridColumnSortOrder.Descending" SortIndex="1" />
```

### Sort Programmatically

```csharp
// Wrap parameter changes in BeginUpdate / EndUpdate
Grid.BeginUpdate();
var col = Grid.GetDataColumns().First(c => c.FieldName == "Price");
col.SortIndex = 0;
col.SortOrder = GridColumnSortOrder.Descending;
Grid.EndUpdate();
```

## Grouping

Enable the group panel (users drag columns to group):

```razor
<DxGrid ShowGroupPanel="true" ...>
    <Columns>
        <DxGridDataColumn FieldName="Category" GroupIndex="0" />
        <DxGridDataColumn FieldName="Name" />
    </Columns>
</DxGrid>
```

Group by date interval:
```razor
<DxGridDataColumn FieldName="OrderDate"
                  GroupIndex="0"
                  GroupInterval="GridColumnGroupInterval.DateYear" />
```

## Filtering

### Search Box

```razor
<DxGrid ShowSearchBox="true" ...>
```

### Filter Row

```razor
<DxGrid ShowFilterRow="true" ...>
```

Users type values into filter cells below column headers.

### Filter Panel & Filter Builder

```razor
<DxGrid FilterPanelDisplayMode="GridFilterPanelDisplayMode.Always" ...>
```

Displays an active filter summary.

Users can click the summary to open the visual filter builder.

Use `GridFilterPanelDisplayMode.Auto` to show the panel only when a filter is active.

### Filter Builder Operator Customization

To customize operators that appear when users edit filter criteria from the filter panel, define a
`FilterBuilderTemplate` and handle `DxFilterBuilder.CustomizeOperators`.

`DxGrid.CustomizeFilterMenu` affects the column filter menu only. It does not customize the filter
panel or filter builder operators.

```razor
<DxGrid Data="@Items"
        KeyFieldName="Id"
        FilterPanelDisplayMode="GridFilterPanelDisplayMode.Always">
    <Columns>
        <DxGridDataColumn FieldName="Name" />
        <DxGridDataColumn FieldName="DueDate" DisplayFormat="d" />
    </Columns>
    <FilterBuilderTemplate Context="filterBuilderContext">
        <DxFilterBuilder @bind-FilterCriteria="filterBuilderContext.FilterCriteria"
                         CustomizeOperators="CustomizeOperators">
            <Fields>
                @filterBuilderContext.RenderDefaultFields()
            </Fields>
        </DxFilterBuilder>
    </FilterBuilderTemplate>
</DxGrid>

@code {
    static readonly FilterBuilderOperatorType[] DueDateMonthOperators = new[] {
        FilterBuilderOperatorType.IsJanuary,
        FilterBuilderOperatorType.IsFebruary,
        FilterBuilderOperatorType.IsMarch,
        FilterBuilderOperatorType.IsApril,
        FilterBuilderOperatorType.IsMay,
        FilterBuilderOperatorType.IsJune,
        FilterBuilderOperatorType.IsJuly,
        FilterBuilderOperatorType.IsAugust,
        FilterBuilderOperatorType.IsSeptember,
        FilterBuilderOperatorType.IsOctober,
        FilterBuilderOperatorType.IsNovember,
        FilterBuilderOperatorType.IsDecember
    };

    void CustomizeOperators(FilterBuilderCustomizeOperatorsEventArgs args) {
        if (args.FieldName != "DueDate")
            return;

        foreach (var operatorType in DueDateMonthOperators)
            args.Operators.Remove(operatorType);
    }
}
```

### Set Filter Programmatically

Requires `@using DevExpress.Data.Filtering` (add to `_Imports.razor`):

```csharp
// Apply a filter expression
Grid.SetFilterCriteria(new BinaryOperator("Price", 100, BinaryOperatorType.Greater));

// Clear all filters
Grid.ClearFilter();
```

## Summaries

### Total Summaries (Footer Row)

```razor
<DxGrid Data="@Orders">
    <Columns>
        <DxGridDataColumn FieldName="Amount" />
        <DxGridDataColumn FieldName="Quantity" />
    </Columns>
    <TotalSummary>
        <DxGridSummaryItem SummaryType="GridSummaryItemType.Sum" FieldName="Amount" />
        <DxGridSummaryItem SummaryType="GridSummaryItemType.Count" FieldName="Amount" />
        <DxGridSummaryItem SummaryType="GridSummaryItemType.Average" FieldName="Amount" />
    </TotalSummary>
</DxGrid>
```

### Group Summaries

```razor
<GroupSummary>
    <DxGridSummaryItem SummaryType="GridSummaryItemType.Sum" FieldName="Amount" />
</GroupSummary>
```

### Summary Item Types

| `GridSummaryItemType` | Description |
|---|---|
| `Sum` | Sum of values |
| `Count` | Row count |
| `Average` | Average of values |
| `Min` | Minimum value |
| `Max` | Maximum value |
| `Custom` | Custom calculation via `CustomSummary` event |

### Custom Summary

```razor
<DxGrid CustomSummary="OnCustomSummary">
    <TotalSummary>
        <DxGridSummaryItem SummaryType="GridSummaryItemType.Custom"
                           FieldName="Amount"
                           Name="aboveAverage" />
    </TotalSummary>
</DxGrid>

@code {
    void OnCustomSummary(GridCustomSummaryEventArgs e) {
        if (e.SummaryStage == GridCustomSummaryStage.Start)
            e.TotalValue = 0;
        else if (e.SummaryStage == GridCustomSummaryStage.Calculate) {
            var val = (decimal)e.GetValue("Amount");
            if (val > 100)
                e.TotalValue = (int)e.TotalValue + 1;
        }
    }
}
```

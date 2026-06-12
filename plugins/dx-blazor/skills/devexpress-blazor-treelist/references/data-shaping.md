# Data Shaping — Blazor TreeList

When you need to: sort columns, add filtering UI, or create total summaries.

## Sorting

Users click column headers to sort. Sorting does not break the tree hierarchy — children always stay under their parent.

Disable sorting globally:
```razor
<DxTreeList AllowSort="false" ...>
```

Disable sorting per column:
```razor
<DxTreeListDataColumn FieldName="Name" AllowSort="false" />
```

### Initial Sort

```razor
<DxTreeListDataColumn FieldName="DueDate"
                      SortOrder="TreeListColumnSortOrder.Ascending"
                      SortIndex="0" />
```

## Filtering

### Search Box

```razor
<DxTreeList ShowSearchBox="true" ...>
```

The search box searches across all visible columns.

### Filter Row

```razor
<DxTreeList ShowFilterRow="true" ...>
```

Users type filter values below column headers.

### Filter Panel

```razor
<DxTreeList FilterPanelDisplayMode="TreeListFilterPanelDisplayMode.Always" ...>
```

Displays the active filter summary.

Users can click the summary to open the filter builder.

Use `TreeListFilterPanelDisplayMode.Auto` to show the panel only when a filter is active.

### Filter Builder Operator Customization

To customize operators that appear when users edit filter criteria from the filter panel, define a
`FilterBuilderTemplate` and handle `DxFilterBuilder.CustomizeOperators`.

`DxTreeList.CustomizeFilterMenu` affects the column filter menu only. It does not customize the
filter panel or filter builder operators.

```razor
<DxTreeList Data="@Tasks"
            KeyFieldName="Id"
            ParentKeyFieldName="ParentId"
            FilterPanelDisplayMode="TreeListFilterPanelDisplayMode.Always">
    <Columns>
        <DxTreeListDataColumn FieldName="Name" />
        <DxTreeListDataColumn FieldName="DueDate" DisplayFormat="d" />
    </Columns>
    <FilterBuilderTemplate Context="filterBuilderContext">
        <DxFilterBuilder @bind-FilterCriteria="filterBuilderContext.FilterCriteria"
                         CustomizeOperators="CustomizeOperators">
            <Fields>
                @filterBuilderContext.RenderDefaultFields()
            </Fields>
        </DxFilterBuilder>
    </FilterBuilderTemplate>
</DxTreeList>

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

### Tree Filter Mode

The TreeList has two modes for showing filtered results:

| Mode | Description |
|---|---|
| `TreeListDataFilterMode.Default` | Only matching nodes are shown; their parents are shown for context |
| `TreeListDataFilterMode.EntireBranch` | When a node matches, its entire subtree (all descendants) is shown |

```razor
<DxTreeList DataFilterMode="TreeListDataFilterMode.EntireBranch" ...>
```

## Summaries

### Total Summaries

```razor
<DxTreeList Data="@Tasks">
    <Columns>
        <DxTreeListDataColumn FieldName="Name" />
        <DxTreeListDataColumn FieldName="EstimatedHours" />
    </Columns>
    <TotalSummary>
        <DxTreeListSummaryItem SummaryType="TreeListSummaryItemType.Count" FieldName="Name" />
        <DxTreeListSummaryItem SummaryType="TreeListSummaryItemType.Sum" FieldName="EstimatedHours" />
    </TotalSummary>
</DxTreeList>
```

### Summary Item Types

| `TreeListSummaryItemType` | Description |
|---|---|
| `Sum` | Sum of values |
| `Count` | Row count |
| `Average` | Average of values |
| `Min` | Minimum value |
| `Max` | Maximum value |

> **Note**: The TreeList does not support group summaries (there is no grouping panel — the tree hierarchy is the grouping structure).

# Data Shaping — Blazor TreeList

When you need to: sort tree columns; add a filter row, filter panel, or search box; create total summaries.

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
<DxTreeList ShowFilterPanel="true" ...>
```

Shows the active filter summary; users can click to open the filter builder.

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

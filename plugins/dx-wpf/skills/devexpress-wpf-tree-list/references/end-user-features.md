# End-User Features — DevExpress WPF TreeList

`TreeListControl` shares most end-user features with `GridControl` (sorting, filtering, summaries, conditional formatting, drag-and-drop) plus tree-specific capabilities (expand/collapse, multi-node selection).

## When to Use This Reference

Use this when you need to:

- Configure sorting, filtering, searching
- Display total or node-level summaries
- Enable multiple-node or multiple-cell selection
- Customize expand/collapse behavior
- Hide / reorder columns via the Column Chooser
- Allow keyboard navigation between nodes and cells

## Sorting

End users sort by clicking column headers (hold Shift for multi-sort). Sorting is permitted within each level — siblings under a common parent.

| Property | Where | Effect |
|---|---|---|
| `DataViewBase.AllowSorting` | TreeListView | Enable end-user sorting. |
| `ColumnBase.AllowSorting` | Per column | Override view-level setting. |
| `DataViewBase.ShowSortIndicator` | TreeListView | Show sort glyph on all headers. |

```xaml
<dxg:TreeListView AllowSorting="True" ShowSortIndicator="True"/>
<dxg:TreeListColumn FieldName="Name" SortOrder="Ascending" SortIndex="0"/>
```

Source: `articles/controls-and-libraries/tree-list/end-user-capabilities/sorting.md`.

## Filtering

`TreeListControl` supports all `GridControl` filter UIs:

- Column Drop-Down Filters
- Filter Editor
- Filter Elements
- Automatic Filter Row
- Filter Panel
- Filtering in Code

```xaml
<dxg:TreeListView ShowAutoFilterRow="True" AllowFilterEditor="True"/>
```

When filtering, parent nodes remain visible even if they don't match the filter, as long as one of their descendants matches. This keeps the path to matching nodes visible.

Source: `articles/controls-and-libraries/tree-list.md` references https://docs.devexpress.com/content/WPF/7356?md=true and the data-grid filtering article.

## Search Panel

```xaml
<dxg:TreeListView ShowSearchPanelMode="Always"
                  SearchString="{Binding SearchText, Mode=TwoWay}"/>
```

Source: `articles/controls-and-libraries/tree-list.md` references https://docs.devexpress.com/content/WPF/11403?md=true.

## Data Summaries

TreeList supports total summaries and per-node summaries (summarize a node's descendants).

```xaml
<dxg:TreeListControl ItemsSource="{Binding Employees}">
    <dxg:TreeListControl.TotalSummary>
        <dxg:TreeListSummaryItem SummaryType="Count" Alignment="Right"/>
        <dxg:TreeListSummaryItem FieldName="Salary" SummaryType="Sum"
                                 DisplayFormat="Payroll=${0:N2}"/>
    </dxg:TreeListControl.TotalSummary>
    <!-- ... -->
</dxg:TreeListControl>
```

For per-node summaries (sum of children's values shown on the parent node), see `articles/controls-and-libraries/tree-list/examples/how-to-display-totals.md`.

Source: `articles/controls-and-libraries/tree-list.md` § Display Summaries.

## Expand / Collapse

```xaml
<dxg:TreeListView AutoExpandAllNodes="True"/>     <!-- Expand on load -->
<dxg:TreeListView AutoExpandAllNodes="False"/>    <!-- Roots only -->
```

Programmatic control:

```csharp
treeListView1.ExpandAllNodes();
treeListView1.CollapseAllNodes();

// Per row handle:
treeListView1.ExpandNode(rowHandle);
treeListView1.CollapseNode(rowHandle);
```

> Verify exact method names (`ExpandAllNodes` vs `ExpandAll`) against the apidoc directory `apidoc/DevExpress.Xpf.Grid/TreeListView/` or via DxDocs MCP. The naming follows the public DevExpress conventions.

Source: `articles/controls-and-libraries/tree-list/end-user-capabilities/expanding-and-collapsing-nodes.md`.

## Multiple Node / Cell Selection

```xaml
<dxg:TreeListView SelectionMode="Row" MultiSelectMode="Row"/>
```

For cell-level selection:

```xaml
<dxg:TreeListView NavigationStyle="Cell" SelectionMode="Cell"/>
```

| Mode | Effect |
|---|---|
| `Row` | One row at a time |
| `MultipleRow` | Ctrl-click / Shift-click select multiple rows |
| `Cell` | Individual cell selection |
| `MultipleCell` | Drag-select / Ctrl-click multiple cells |

> Verify the exact enum values via DxDocs MCP if the names differ in your version:
> `devexpress_docs_search(technology="WPF TreeList", query="multi-row selection mode")`

Source: `articles/controls-and-libraries/tree-list/end-user-capabilities/selecting-multiple-nodes.md`.

## Column Chooser, Reordering, Resizing

- Drag a column header to reorder.
- Drag a header to the Column Chooser to hide; drag back to show.
- Right-click a header for the column context menu.

```xaml
<dxg:TreeListView AllowColumnMoving="True"/>
<dxg:TreeListColumn FieldName="Internal" ShowInColumnChooser="False"/>
```

Source: `articles/controls-and-libraries/tree-list/end-user-capabilities/showing-and-hiding-columns.md` and `resizing-columns.md`.

## Keyboard Navigation

```xaml
<dxg:TreeListView AllowHeaderNavigation="True"
                  NavigationStyle="Cell"/>
```

`AllowHeaderNavigation` includes the header row in Tab order. Arrow keys, Page Up/Down, Home/End navigate within the tree.

Source: `articles/controls-and-libraries/tree-list/end-user-capabilities/navigating-through-nodes-and-cells.md`.

## Source Material

- `articles/controls-and-libraries/tree-list/end-user-capabilities.md`
- `articles/controls-and-libraries/tree-list/end-user-capabilities/sorting.md`
- `articles/controls-and-libraries/tree-list/end-user-capabilities/expanding-and-collapsing-nodes.md`
- `articles/controls-and-libraries/tree-list/end-user-capabilities/selecting-multiple-nodes.md`
- `articles/controls-and-libraries/tree-list/end-user-capabilities/showing-and-hiding-columns.md`
- `articles/controls-and-libraries/tree-list/end-user-capabilities/navigating-through-nodes-and-cells.md`
- `articles/controls-and-libraries/tree-list/end-user-capabilities/data-editing.md`
- `articles/controls-and-libraries/tree-list/end-user-capabilities/customizing-summaries.md`

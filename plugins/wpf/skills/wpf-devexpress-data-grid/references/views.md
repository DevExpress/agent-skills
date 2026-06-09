# Views and Layout — DevExpress WPF Data Grid

The `GridControl` uses a pluggable **View** (`TableView`, `CardView`, or `TreeListView`) to render data. The View controls layout, column behavior, and which end-user features are available.

## When to Use This Reference

Use this when you need to:

- Choose between Table View, Card View, and TreeList View
- Auto-size columns to fit the grid or content
- Configure the Column Chooser, column reordering, resizing
- Display hierarchical data with `TreeListView`
- Switch between view types at design time (not recommended at runtime)

## Key Classes

| Class | Purpose |
|---|---|
| `DevExpress.Xpf.Grid.DataViewBase` | Base class of all views. Owns `AllowSorting`, `ShowSortIndicator`, `FocusedView`, `DataControl`, `CommitEditing()`. |
| `DevExpress.Xpf.Grid.GridViewBase` | Base of `TableView` and `CardView`. Owns `GroupedColumns`, `ShowGroupPanel`, `AllowGrouping`. |
| `DevExpress.Xpf.Grid.TableView` | Two-dimensional table layout. Default View. |
| `DevExpress.Xpf.Grid.CardView` | Cards layout. Each card arranges fields vertically. |
| `DevExpress.Xpf.Grid.TreeListView` | Tree layout for hierarchical or self-referential data. |

## Table View (Default)

```xaml
<dxg:GridControl ItemsSource="{Binding Orders}">
    <dxg:GridControl.View>
        <dxg:TableView AutoWidth="True"
                       BestFitModeOnSourceChange="VisibleRows"
                       ShowGroupPanel="True"
                       AllowSorting="True"
                       AllowGrouping="True"/>
    </dxg:GridControl.View>
</dxg:GridControl>
```

Common `TableView` properties:

| Property | Effect |
|---|---|
| `AutoWidth` | Distributes the grid's width across all columns. |
| `BestFitModeOnSourceChange` | Recalculate column widths when `ItemsSource` changes. Values: `None`, `Smart`, `VisibleRows`, `AllRows`. |
| `ShowGroupPanel` | Show the panel above the header where columns can be dragged to group. |
| `NewItemRowPosition` | `None`, `Top`, or `Bottom` for the "add new row" row. |
| `ShowUpdateRowButtons` | `OnCellEditorOpen`, `Always`, or `Never` for the confirm/cancel row buttons. |
| `ShowFixedTotalSummary` | Show the fixed total summary panel at the bottom. |
| `TotalSummaryPosition` | `Top` or `Bottom` for the inline total summary. |

Source: `articles/controls-and-libraries/data-grid/getting-started/code/lesson-2-display-and-edit-data.md`.

## Card View

```xaml
<dxg:GridControl ItemsSource="{Binding Orders}">
    <dxg:GridControl.View>
        <dxg:CardView/>
    </dxg:GridControl.View>
</dxg:GridControl>
```

Use Card View when each record has many fields and a vertical-per-record layout reads better than a wide table (e.g., contact lists, ticket details). Columns become "card fields"; the same `GridColumn` definitions apply.

Source: `articles/controls-and-libraries/data-grid/views.md`.

## TreeList View

Use when data is hierarchical. There are three derivation modes:

- **Self-referential** (flat data with parent-key field): set `TreeListView.TreeDerivationMode = "SelfReference"`, then `KeyFieldName` and `ParentFieldName`.
- **Hierarchical** (nested children collections): set `ChildNodesPath`, `ChildNodesSelector`, or use `HierarchicalDataTemplate`.

```xaml
<dxg:GridControl ItemsSource="{Binding Departments}">
    <dxg:GridControl.View>
        <dxg:TreeListView TreeDerivationMode="ChildNodesPath"
                          ChildNodesPath="Children"
                          AutoWidth="True"/>
    </dxg:GridControl.View>
    <dxg:GridColumn FieldName="Name"/>
    <dxg:GridColumn FieldName="HeadCount"/>
</dxg:GridControl>
```

Source: `articles/controls-and-libraries/data-grid/display-hierarchical-data.md`.

The flat self-referential pattern:

```xaml
<dxg:TreeListView TreeDerivationMode="SelfReference"
                  KeyFieldName="EmployeeId"
                  ParentFieldName="ReportsTo"/>
```

## Common Features Across All Views

Per the `views.md` article, every view type supports:

- **Sorting** (`DataViewBase.AllowSorting`)
- **Filtering** (Drop-down filter, Auto Filter Row, Filter Editor)
- **Summaries** (total and group)
- **Editing and validation**
- **Column reordering and resizing**
- **Show / hide columns via Column Chooser**
- **Context popup menus**

`TreeListView` has an additional `ShowNodeImages` toggle and node-specific options; `CardView` has card layout properties.

> **Do not switch views at runtime** — the article explicitly recommends against it. Pick a view at design time and keep it.

## Column Customization

```xaml
<dxg:GridColumn FieldName="OrderDate"
                Header="Date"
                AllowEditing="True"
                AllowSorting="True"
                AllowGrouping="True"
                ShowInColumnChooser="True"
                Width="120"
                MinWidth="60"
                FixedWidth="True"/>
```

Per-column overrides (`AllowSorting`, `AllowGrouping`, `ShowSortIndicator`) take priority over view-level settings. Set the column property to `null` to inherit the view-level value.

## Column Chooser and Reordering

End users can:

- Drag a column header to reorder.
- Drag a header to the Column Chooser to hide; drag back to show.
- Right-click a header for the column context menu.

To disable reordering for the whole view, set `TableView.AllowColumnMoving="False"`. To prevent hiding a specific column, set `GridColumn.ShowInColumnChooser="False"`.

## Hierarchical Source Material

For deeper coverage of TreeListView modes, see:

- `articles/controls-and-libraries/data-grid/display-hierarchical-data.md`

For the full view-overview article:

- `articles/controls-and-libraries/data-grid/views.md`

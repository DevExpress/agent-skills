# Master-Detail

This reference covers master-detail data presentation in `GridControl`. The `TreeList` does not have its own master-detail because the hierarchy is the relationship; this topic is grid-specific.

The Data Grid supports any number of nesting levels, mixed view types per level (e.g., `GridView` master with `CardView` detail), runtime relations via events, joint group panel, clone-view synchronization, and per-relation toggles.

## When to Use This Reference

- Showing related child rows when a master row is expanded.
- Mixing view types between master and detail levels.
- Hiding detail expand buttons for master rows with no children.
- Building master-detail in code without designer wiring.
- Enabling multi-select inside detail views.
- Understanding which features ServerMode and `VirtualServerModeSource` disallow.

## Concepts

| Concept | Description |
|---|---|
| **Master view** | The top-level view. Always `GridControl.MainView`. |
| **Detail view** | A view that displays children of a focused master row. |
| **Pattern view** | A view registered in `GridControl.ViewCollection` and associated with a relation via `GridControl.LevelTree`. Does *not* contain data; it is a template. |
| **Clone view** | A runtime copy of a pattern view, created when a user expands a master row and destroyed when collapsed. |
| **Level node** | An entry in `GridControl.LevelTree.Nodes` linking a relation name to a pattern view. Levels can be nested arbitrarily. |
| **Relation** | Source-level link between master and detail (e.g., `DataRelation` in a `DataSet`, navigation property in EF, collection property on a business object). |

## Build Master-Detail in Code

```csharp
var grid = new GridControl { Dock = DockStyle.Fill };
var master  = new GridView(grid);
var detail1 = new GridView(grid);          // Products grid
var detail2 = new CardView(grid);          // Order details as cards
grid.ViewCollection.AddRange(new BaseView[] { master, detail1, detail2 });
grid.MainView = master;

// Relation "CategoriesProducts" exists in the DataSet.
var level1 = new GridLevelNode {
    RelationName  = "CategoriesProducts",
    LevelTemplate = detail1
};
var level2 = new GridLevelNode {
    RelationName  = "ProductsOrder Details",
    LevelTemplate = detail2
};
level1.Nodes.Add(level2);                  // nested level
grid.LevelTree.Nodes.Add(level1);

grid.DataSource = categoriesBindingSource;
Controls.Add(grid);
```

`RelationName` must match the `DataRelation.RelationName` in the `DataSet`, or the collection property name on a business object (e.g., `Category.Products`).

### Show only declared details (suppress auto-relations)

```csharp
grid.ShowOnlyPredefinedDetails = true;
```

Otherwise the grid creates default details for every relation it finds.

## Access Master and Clone Views at Runtime

```csharp
GridView masterView = (GridView)grid.MainView;

// Clone view for an expanded master row (rowHandle in master view):
GridView childView = masterView.GetDetailView(rowHandle, relationIndex: 0) as GridView;

// Currently maximized / focused view:
var focused   = grid.FocusedView;
var maximized = grid.DefaultView;

// Subscribe to clone-view creation
grid.ViewRegistered += (s, e) => {
    if (e.View.IsDetailView) {
        // e.g., add a handler to a detail GridView
    }
};
```

Pattern views contain no rows — never call `GetRow` / `GetFocusedRow` on them. Operate on the clone views.

## Expand / Collapse Master Rows

```csharp
masterView.SetMasterRowExpanded(rowHandle, true);
masterView.SetMasterRowExpandedEx(rowHandle, relationIndex: 0);

bool expanded = masterView.GetMasterRowExpanded(rowHandle);
masterView.CollapseAllDetails();
```

Cancel expansion conditionally:

```csharp
masterView.MasterRowExpanding += (s, e) => {
    var orderId = (int)masterView.GetRowCellValue(e.RowHandle, "OrderID");
    if (!IsAllowed(orderId)) e.Allow = false;
};
```

Equivalents exist: `MasterRowExpanded`, `MasterRowCollapsing`, `MasterRowCollapsed`.

## Hide Expand Buttons for Master Rows with No Detail Data

```csharp
masterView.MasterRowEmpty += (s, e) => {
    var product = (Product)masterView.GetRow(e.RowHandle);
    e.IsEmpty = product.OrderDetails.Count == 0;
};
```

Empty masters render without the expand button, which improves UX significantly when many parents have no children.

## Runtime Relation Counts and Names

For business objects without explicit `DataRelation`, supply relation metadata via events:

```csharp
masterView.MasterRowGetRelationCount += (s, e) => e.RelationCount = 1;
masterView.MasterRowGetRelationName  += (s, e) => e.RelationName  = "OrderDetails";
masterView.MasterRowGetChildList     += (s, e) => {
    var order = (Order)masterView.GetRow(e.RowHandle);
    e.ChildList = order.Details;
};
```

The `RelationName` returned here must match a `LevelTree` node's `RelationName`.

## Synchronize Clone Views

If the user resizes a column or groups data in one detail clone view and you want every clone of the same pattern to follow:

```csharp
detail1.SynchronizeClones = true;
```

## Joint Group Panel

Use one group panel shared between master and detail. Requires Clone-View Synchronization.

```csharp
masterView.OptionsView.ShowGroupPanel       = true;
masterView.OptionsView.ShowChildrenInGroupPanel = true;
masterView.ChildGridLevelName = "OrderDetails";
```

## Zoom (Maximize) a Detail View

```csharp
masterView.ZoomView();          // maximize the focused detail
masterView.NormalView();         // restore
grid.FocusedView.ZoomView();
```

`OptionsBehavior.AllowZoomView` controls whether zoom is available.

## Multi-Select in Detail Views

Set `OptionsSelection.MultiSelect = true` on the detail pattern view. The clones inherit it. To toggle multi-select per clone, set it in the `ViewRegistered` handler.

```csharp
detail1.OptionsSelection.MultiSelect = true;
```

## Restrict Detail Drill-Down

```csharp
masterView.OptionsDetail.EnableMasterViewMode = true;          // overall toggle
masterView.OptionsDetail.ShowDetailTabs       = true;          // show tab strip per detail
masterView.OptionsDetail.AllowExpandEmptyDetails = false;      // hide ✚ for empty master rows
```

## Print and Export

```csharp
masterView.OptionsPrint.PrintDetails      = true;
masterView.OptionsPrint.ExpandAllDetails  = true;     // expand on export
grid.ExportToXlsx("orders.xlsx");
```

WYSIWYG export mode is required for master-detail (data-aware export does not include detail levels for Card/Layout/Tile/WinExplorer views).

## Limitations

- **ServerMode**: master-detail is **not supported**.
- **`VirtualServerModeSource`**: does not support grouping; master-detail typically does not work cleanly.
- **Pattern views are shared**: a property change on a pattern view affects all clones; per-clone customization happens in `ViewRegistered`.
- **Default details for unknown relations** fall back to a master-view copy and may look wrong; set `ShowOnlyPredefinedDetails = true`.

## Common Issues

- **Master rows expand into a copy of the master view**: `LevelTree` lacks a node for that relation. Add a `GridLevelNode` with the correct `RelationName`.
- **Detail data missing**: relation not defined on the data source, or `MasterRowGetChildList` not handled for business objects.
- **Pattern-view APIs return `null`**: pattern views have no rows. Use `GetDetailView` on the *master* view to get a clone, then call APIs on the clone.
- **Performance drops with many open details**: lower the open-detail count, set `SynchronizeClones = false` (synchronization has cost), and consider on-demand detail loading.

## Source Material

- `articles/controls-and-libraries/data-grid/master-detail-relationships.md` (`xref:WindowsForms.3473`).
- `articles/controls-and-libraries/data-grid/master-detail/working-with-master-detail-relationships-in-code.md` (`xref:WindowsForms.732`).
- `articles/controls-and-libraries/data-grid/examples/master-detail/index.md` (`xref:WindowsForms.3078`).
- `articles/controls-and-libraries/data-grid/examples/master-detail/how-to-hide-expand-buttons-for-master-rows-with-empty-details.md` (`xref:WindowsForms.403851`).
- `api/DevExpress.XtraGrid.GridControl.LevelTree.yml`.
- `api/DevExpress.XtraGrid.GridLevelNode.yml`.
- `api/DevExpress.XtraGrid.Views.Grid.GridView.GetDetailView.yml`.
- `api/DevExpress.XtraGrid.Views.Grid.GridView.MasterRowEmpty.yml`.

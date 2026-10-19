# Data Binding (Grid + TreeList)

This reference covers data binding for both `GridControl` and `TreeList` — the regular in-memory binding mode, the ServerMode/InstantFeedback large-data modes, the `VirtualServerModeSource` for infinite scrolling, the unbound source for purely calculated grids, and TreeList-specific binding modes (bound, virtual, unbound).

## When to Use This Reference

- Choosing between regular binding, ServerMode, InstantFeedback, and Infinite Scrolling for a given data volume.
- Wiring `GridControl` or `TreeList` to a `DataTable`, `BindingList<T>`, `IList`, EF DbSet, XPO `XPCollection`, or remote source.
- Configuring TreeList parent-child relationships (`KeyFieldName`, `ParentFieldName`, `RootValue`).
- Diagnosing "data shows once but never refreshes" or "filtering/grouping is slow" symptoms.

## Choose a Binding Mode

| Mode | Use When | Source Components | Limitations |
|---|---|---|---|
| **Regular (in-memory)** | Data fits in memory; rich UX (grouping, filtering, master-detail, custom row filter). | `DataTable`, `BindingSource`, `BindingList<T>`, `IList`, `List<T>`, `IBindingList`, `ITypedList`, EF DbSet materialized to `ToList`. | Loads all rows; sluggish past ~50–100 k rows or for wide tables. |
| **ServerMode (synchronous)** | Very large data sets, sort/group/filter on the server (SQL), UI can briefly freeze while a query runs. | `EntityServerModeSource` (EF), `LinqServerModeSource` (LINQ to SQL), `PLinqServerModeSource` (PLINQ), `WcfServerModeSource`, `ODataServerModeSource`, `XPServerCollectionSource` (XPO). | No master-detail; no `CustomRowFilter`; cannot add/delete grouped rows; no sort/filter by display value; limited search modes. |
| **InstantFeedback (async ServerMode)** | Same as ServerMode but UI must remain responsive while queries run. | `EntityInstantFeedbackSource`, `LinqInstantFeedbackSource`, `PLinqInstantFeedbackSource`, `WcfInstantFeedbackSource`, `ODataInstantFeedbackSource`, `XPInstantFeedbackSource`. | Same as ServerMode plus: max 10 000 visible groups; not supported in `GridLookUpEdit`; cannot add custom items to filter dropdown. |
| **Infinite Scrolling (Virtual ServerMode)** | Unknown or very large row count, paginated/streamed remote API, you control batching. | `VirtualServerModeSource`. | No grouping; data editing disabled by default (enable via `AcquireInnerList`). |
| **Unbound (with `UnboundSource`)** | All columns are calculated, no real data source. | `UnboundSource` component for row-count metadata, fill via `CustomUnboundColumnData`. | All cells must be supplied via events. |

> The `GridControl` cannot operate without a data source — even pure unbound grids need an `UnboundSource` to know the row count.

## Regular Binding

Assign any of the supported sources to `GridControl.DataSource`. Use `DataMember` to pick a sub-table inside a `DataSet`.

```csharp
gridControl1.DataSource = employees;            // List<T>, BindingList<T>, DataTable, BindingSource, …
gridControl1.DataMember = "";                   // table name for a DataSet
```

### ADO.NET DataSet

```csharp
nwindDataSet1.Categories.AddCategoriesRow(...);
this.categoriesTableAdapter.Fill(this.nwindDataSet1.Categories);
gridControl1.DataSource = categoriesBindingSource;
```

### Entity Framework (materialized)

```csharp
using var db = new Northwind();
gridControl1.DataSource = db.Customers.AsNoTracking().ToList();
```

For very large EF tables prefer ServerMode (next section).

### Refresh data

`GridControl.RefreshDataSource()` re-reads from the source without rebinding. For a fully new source, set `DataSource = null` then assign again.

```csharp
gridControl1.DataSource = null;
gridControl1.DataSource = LoadFreshOrders();
```

For batch in-memory updates wrap them in `BeginDataUpdate` / `EndDataUpdate` to suppress repaints:

```csharp
view.BeginDataUpdate();
try {
    foreach (var item in batch) bindingList.Add(item);
}
finally { view.EndDataUpdate(); }
```

## ServerMode (Synchronous)

ServerMode keeps only the visible rows in memory; sorting, grouping, filtering, and summary calculation execute on the server.

### Entity Framework

```csharp
using DevExpress.Data.Linq;

var ctx = new Northwind();
var source = new EntityServerModeSource {
    QueryableSource = ctx.Orders,
    KeyExpression   = "OrderID"
};
gridControl1.DataSource = source;
```

Or enable ServerMode on the grid itself (LINQ to SQL only):

```csharp
gridControl1.ServerMode = true;
gridControl1.DataSource = dc.Customers;
```

### XPO

```csharp
using DevExpress.Xpo;

var collection = new XPServerCollectionSource(session, typeof(Order));
gridControl1.DataSource = collection;
```

### Critical: do not wrap with `BindingSource`

Assigning a `BindingSource` between a ServerMode source and the grid materializes everything in memory and defeats ServerMode. Always assign the ServerMode source directly to `GridControl.DataSource`.

### ServerMode limitations

- Master-detail is **not** supported.
- Adding or deleting records while data is grouped is unsupported.
- `CustomRowFilter` is unsupported.
- Sorting/grouping by display text (`ColumnSortMode.DisplayText`) is unsupported — set `column.FieldNameSortGroup` to a sortable text field instead.
- `Find Panel` always lowercases the search term — use a case-insensitive collation or follow the DevExpress KB on case-insensitive filtering.
- Operations that touch every row (`ExpandAllGroups`, `SelectAll`) generate one query per row and should be avoided.

## InstantFeedback (Asynchronous ServerMode)

Same shape as ServerMode but with `*InstantFeedbackSource` types. Data loads on a background thread, the UI shows a loading indicator, and the grid never freezes.

```csharp
using DevExpress.Data.Linq;

var source = new EntityInstantFeedbackSource {
    KeyExpression = "OrderID"
};
source.Resolve += (s, e) => e.QueryableSource = new Northwind().Orders;
gridControl1.DataSource = source;
```

Recreate the `DbContext` inside `Resolve` because the source manages its own thread. Dispose contexts via the `DismissQueryable` event.

InstantFeedback inherits all ServerMode limitations plus: maximum 10 000 visible groups, no custom items in filter dropdowns, not supported in `GridLookUpEdit`.

## Infinite Scrolling (`VirtualServerModeSource`)

Use when you have a paginated remote API or do not know the total row count up front.

```csharp
using DevExpress.Data;

var source = new VirtualServerModeSource();
source.ConfigurationChanged += (s, e) => {
    // Re-initialize. Optionally pre-load the first batch by setting e.RowsList.
    e.RowsList = api.GetPage(0, pageSize, e.Sort, e.Filter);
    e.SourceState = api.SaveState();        // user-defined session token
};
source.MoreRows += (s, e) => {
    e.RowsList = api.GetNextPage(e.SourceState, e.RowsList?.Count ?? 0, pageSize);
};
gridControl1.DataSource = source;
```

Grouping is disabled. To enable editing, supply `AcquireInnerList`.

## Unbound Grid

The grid still requires a source for row-count metadata even when every column is calculated. Use `UnboundSource`:

```csharp
var source = new DevExpress.Data.UnboundSource();
source.RowCount = 500;
source.ValueNeeded += (s, e) => {
    if (e.Property == "Index")  e.Value = e.ListSourceRowIndex;
    if (e.Property == "Square") e.Value = e.ListSourceRowIndex * e.ListSourceRowIndex;
};
gridControl1.DataSource = source;
```

See [columns.md](columns.md) for the related `UnboundExpression` / `CustomUnboundColumnData` patterns on individual columns when you have a real bound source plus a few calculated columns.

## TreeList Binding

`TreeList` accepts the same in-memory sources as the grid but additionally requires hierarchy metadata via `KeyFieldName`, `ParentFieldName`, and `RootValue`.

### Bound mode (self-referenced data)

```csharp
treeList1.DataSource      = employees;          // flat list with Id + ManagerId
treeList1.KeyFieldName    = "Id";
treeList1.ParentFieldName = "ManagerId";
treeList1.RootValue       = 0;                  // rows with ManagerId == 0 become roots
```

- `KeyFieldName` and `ParentFieldName` must be the **same data type**. `RootValue` must match that type (default is `0`).
- For `null`-rooted hierarchies set `RootValue = null` and use a nullable key/parent type.
- Service columns (Key, Parent, Image) are not created by default. Enable `OptionsBehavior.PopulateServiceColumns = true` if you want them generated.
- Bound mode does not support virtual (on-demand) data loading.

### Virtual mode — collection property

Build a business object that exposes a child collection. `TreeList` walks it without needing key/parent fields.

```csharp
public class Department {
    public string Name { get; set; } = "";
    public List<Department> Children { get; } = new();
}

treeList1.DataSource = roots;        // List<Department>
// No KeyFieldName / ParentFieldName needed.
// TreeList auto-detects the collection property; specify it explicitly via
// treeList1.ChildListFieldName = "Children" if there are multiple collection properties.
```

### Virtual mode — `IVirtualTreeListData`

Implement on the data source for full control over children retrieval:

```csharp
public class VirtualSource : IVirtualTreeListData {
    public void VirtualTreeGetChildNodes(VirtualTreeGetChildNodesInfo info) {
        info.Children = api.GetChildren(info.Node);
    }
    public void VirtualTreeGetCellValue(VirtualTreeGetCellValueInfo info) {
        info.CellData = api.GetField(info.Node, info.Column.FieldName);
    }
    public void VirtualTreeSetCellValue(VirtualTreeSetCellValueInfo info) { /* … */ }
}
treeList1.DataSource = new VirtualSource();
```

### Virtual mode — TreeList events

Handle `BeforeExpand` + `VirtualTreeGetChildNodes` on the control directly to lazy-load children without a source-level interface.

### Unbound TreeList

When the TreeList has no `DataSource` and you build the tree manually via `AppendNode` — see [treelist-nodes.md](treelist-nodes.md).

## Choosing the Right Mode at a Glance

```
data size < ~50k rows  ─► regular binding
data size ≥ ~50k rows  ─► ServerMode (sync)        if blocking UI is OK
                       └► InstantFeedback (async)  if UI must stay responsive
                       └► VirtualServerModeSource  if remote/paginated/unknown count

hierarchical, full tree fits in memory   ─► TreeList bound mode (KeyFieldName/ParentFieldName)
hierarchical, lazy children              ─► TreeList virtual mode (IVirtualTreeListData / events)
business object with child collection    ─► TreeList virtual mode (ChildListFieldName)
```

## Common Issues

- **Server-mode grid loads everything**: a `BindingSource` is wrapped around the ServerMode source. Remove it; bind directly.
- **TreeList shows a flat list**: `KeyFieldName`, `ParentFieldName`, or `RootValue` is missing or types do not match. Default `RootValue = 0` fails for `string` or nullable keys.
- **TreeList missing service columns**: enable `OptionsBehavior.PopulateServiceColumns` if you need columns for Key/Parent/Image fields.
- **`UpdateCurrentRow` does not persist edits**: when EF tracks changes, ensure the entity is attached and `SaveChanges` is called — the grid posts to the binding, not directly to the database.
- **Filtering case-sensitive in ServerMode**: SQL collation is case-sensitive. Use a case-insensitive collation or rewrite filters with `LOWER`.

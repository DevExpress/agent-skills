# Data Binding — Bound, Unbound, Virtual

The TreeList supports three ways to supply data. Pick by data shape.

## When to Use This Reference

- Binding self-referential / flat data into a tree
- Building a tree in code with no data source
- Loading children on demand for large data
- Adding calculated (unbound) columns

## Bound Mode (Self-Referential)

The TreeList binds to any traditional data source — `BindingSource`, `BindingList<T>`, `List<T>`, `DataTable`, `DataView`, `DataSet`, `IList`/`IBindingList`, SQL/XML/Excel sources.

A hierarchy needs two fields plus a root marker:

| Property | Purpose |
|---|---|
| `DataSource` | The data source |
| `DataMember` | Table name when `DataSource` is a `DataSet` |
| `KeyFieldName` | Field with each node's unique ID |
| `ParentFieldName` | Field with each node's parent ID |
| `RootValue` | The `ParentFieldName` value that marks root nodes |

```csharp
treeList.KeyFieldName    = "ID";
treeList.ParentFieldName = "RegionID";
treeList.RootValue       = -1;             // records with RegionID == -1 are roots
treeList.OptionsBehavior.PopulateServiceColumns = true; // also create key/parent columns
treeList.DataSource      = SalesDataGenerator.CreateData();
```

Records whose `ParentFieldName` value equals `RootValue` become root nodes; every other record attaches under the node whose `KeyFieldName` matches its `ParentFieldName`. See the [Tree Generation Algorithm](https://docs.devexpress.com/content/WindowsForms/198?md=true) for the exact rules.

> Notes:
> - To filter/sort, use the TreeList's own API (not the data source's) — see [sorting-filtering-summaries.md](sorting-filtering-summaries.md).
> - Dynamic (on-demand) loading is **not** supported in bound mode. Use unbound mode for that.
> - Columns auto-generate by default; disable with `OptionsBehavior.AutoPopulateColumns = false` and call `PopulateColumns()` yourself.

### Virtual Mode (Hierarchical Business Object)

When the data is a hierarchical business object that changes at runtime and the tree must reflect changes immediately, bind to a hierarchical source whose objects implement `INotifyPropertyChanged`. Use the MCP tool for the exact `IVirtualTreeListData` / hierarchical-source setup:

```
devexpress_docs_search(technologies=["WindowsForms"], question="TreeList virtual mode bind hierarchical business object")
```

## Unbound Mode

Unbound mode means `DataSource = null`; you create nodes yourself.

1. Create columns first — via the `Columns` collection.
2. Add nodes with `AppendNode`.

```csharp
using DevExpress.XtraTreeList;
using DevExpress.XtraTreeList.Nodes;

TreeListNode parentForRoots = null;
// Root node — nodeData order must match the column order
TreeListNode root = treeList.AppendNode(
    new object[] { "Alfreds Futterkiste", "Germany, Obere Str. 57", "030-0074321" }, parentForRoots);
// Child of root
treeList.AppendNode(
    new object[] { "Michael Suyama", "Obere Str. 55", "030-0074263" }, root);
```

`AppendNode(object[] nodeData, TreeListNode parentNode)` — `parentNode = null` appends a root. There is also an overload that takes a parent node `Id` (`int`) and one that accepts a `DataRow` as `nodeData`. The array's value types must match the column data types.

### Batch Loading

Each `AppendNode` triggers an update. Wrap bulk creation to update only once:

```csharp
treeList.BeginUnboundLoad();
foreach (var item in items)
    treeList.AppendNode(item.ToArray(), FindParent(item));
treeList.EndUnboundLoad();
```

### Export / Import Unbound Data

```csharp
treeList.ExportToXml("tree.xml");
treeList.ImportFromXml("tree.xml");
```

## Dynamic (On-Demand) Loading

For large trees, create root nodes up front and supply children only when a node is expanded:

```csharp
// 1. Create root nodes (AppendNode)
// 2. Mark nodes that can have children so the expand button appears
rootNode.HasChildren = true;

// 3. Supply children on first expand
treeList.BeforeExpand += (s, e) => {
    if (e.Node.Nodes.Count == 0) {                 // load once
        foreach (var child in LoadChildren(e.Node)) {
            TreeListNode n = treeList.AppendNode(child.ToArray(), e.Node);
            n.HasChildren = child.MayHaveChildren;  // recurse on demand
        }
    }
};
```

## Unbound (Calculated) Columns

Add a column not present in the data source and compute its value from an expression:

```csharp
using DevExpress.XtraTreeList.Columns;

TreeListColumn change = treeList.Columns.AddField("Change");
change.Caption = "Change from prev";
change.UnboundDataType = typeof(decimal);
change.UnboundExpression = "[MarchSales] - [MarchSalesPrev]";
change.OptionsColumn.ShowInCustomizationForm = false;
```

For runtime-computed values that aren't a simple expression, handle the unbound-data event (use MCP to confirm the exact event name for your version).

## Source Material

- `articles/controls-and-libraries/tree-list/feature-center/data-binding/bound-mode.md` (`xref:116708`)
- `articles/controls-and-libraries/tree-list/feature-center/data-binding/unbound-mode.md` (`xref:5557`)
- `articles/controls-and-libraries/tree-list/feature-center/nodes.md` (`xref:5593`) — `AppendNode` parameters
- `examples/unbound-mode-beforeexpand-event13299.md` — dynamic loading via `BeforeExpand`

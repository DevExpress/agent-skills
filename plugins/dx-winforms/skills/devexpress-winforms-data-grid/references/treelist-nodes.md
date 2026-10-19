# TreeList — Working with Nodes

This reference covers the `TreeList`-specific node API: creating nodes (`AppendNode`, `Nodes.Add`), navigating (`FocusedNode`, `FirstNode`, `NextNode`, `ParentNode`), finding (`FindNodeByKeyID`, `FindNodeByFieldValue`, `FindNode(Predicate)`), moving and copying (`MoveNode`, `CopyNode`), iterating (`Nodes`, `GetVisibleNodeAt`), expand/collapse, checkbox state, and unbound vs. virtual modes.

`TreeList` columns share the design of `GridColumn` — see [columns.md](columns.md). Filtering, sorting, validation, conditional formatting, drag-and-drop, layout persistence, and export are similar to the grid — see those references.

## When to Use This Reference

- Building a TreeList in unbound mode (no `DataSource`, only manual nodes).
- Adding/removing nodes at runtime in bound mode.
- Locating a node by key, field value, or predicate.
- Iterating the tree top-down or visible-only.
- Implementing tri-state checkboxes (parent reflects children).
- Loading children on demand (virtual mode).

## `TreeListNode` Anatomy

A node has values for each column, an optional check state, image indexes, a `Tag` object, child `Nodes`, and several navigation properties:

| Member | Purpose |
|---|---|
| `Id` | Internal index in the data source (bound mode). |
| `Item[Object]` / `GetValue(column)` / `SetValue(column, value)` | Read/write cell values. |
| `Nodes` | Children collection. |
| `ParentNode` / `RootNode` | Parents up the chain. |
| `FirstNode` / `LastNode` / `NextNode` / `PrevNode` | Sibling navigation. |
| `NextVisibleNode` / `PrevVisibleNode` | Visible-only navigation. |
| `Expanded` | Open or closed. |
| `Visible` | Hidden by filtering or programmatic hide. |
| `Selected` | Multi-select state. |
| `CheckState` | `Checked` / `Unchecked` / `Indeterminate`. |
| `ImageIndex` / `SelectImageIndex` / `StateImageIndex` | Image slots. |
| `Level` | Depth from root (root level = 0). |
| `Tag` | Free-form object you attach. |
| `HasChildren` | Force-show the expand button for virtual-mode lazy loading. |

## Build a Tree in Unbound Mode

```csharp
treeList1.Columns.AddVisible("Name");
treeList1.Columns.AddVisible("Size", "Size, KB");

var root = treeList1.AppendNode(new object[] { "Documents", 0 }, parentNode: null);
treeList1.AppendNode(new object[] { "report.docx", 220 }, root);
treeList1.AppendNode(new object[] { "budget.xlsx", 84 }, root);

var media = treeList1.AppendNode(new object[] { "Media", 0 }, null);
treeList1.AppendNode(new object[] { "logo.png", 12 }, media);

treeList1.ExpandAll();
```

`AppendNode` has many overloads (key id, image indexes, check state, tag). The two common shapes:

```csharp
TreeListNode AppendNode(object nodeData, TreeListNode parentNode);
TreeListNode AppendNode(object nodeData, int parentNodeId);
TreeListNode AppendNode(object nodeData, TreeListNode parent, CheckState check, object tag);
```

`nodeData` is `object[]` aligned to the columns (or any object whose properties match column field names if the TreeList is unbound but uses `Columns[i].FieldName`).

## Add Nodes in Bound Mode

In bound mode, mutating the underlying `IList` adds/removes nodes automatically:

```csharp
var employees = (BindingList<Employee>)treeList1.DataSource;
employees.Add(new Employee { Id = 50, ParentId = 10, Name = "Pat" });
```

If your data source is not `IBindingList` (e.g., a plain `List<T>`), call `treeList1.RefreshDataSource()` after adding/removing.

## Navigate

```csharp
TreeListNode focused = treeList1.FocusedNode;
TreeListNode first   = treeList1.Nodes.FirstNode;
TreeListNode root    = focused.RootNode;
TreeListNode parent  = focused.ParentNode;
TreeListNode next    = focused.NextNode;
TreeListNode child   = focused.FirstNode;
```

Iterate all nodes (depth-first):

```csharp
void Walk(TreeListNodes nodes) {
    foreach (TreeListNode node in nodes) {
        Process(node);
        Walk(node.Nodes);
    }
}
Walk(treeList1.Nodes);
```

Iterate only visible nodes:

```csharp
for (var n = treeList1.Nodes.FirstNode; n != null; n = n.NextVisibleNode)
    Process(n);
```

## Find Nodes

```csharp
// By key (bound mode — KeyFieldName)
TreeListNode n = treeList1.FindNodeByKeyID(42);

// By any field
TreeListNode n2 = treeList1.FindNodeByFieldValue("Name", "Acme");

// By predicate
TreeListNode n3 = treeList1.FindNode(node => (decimal)node["Budget"] > 1_000_000m);

// By visible index
TreeListNode n4 = treeList1.GetNodeByVisibleIndex(5);

// At a screen point
TreeListNode n5 = treeList1.GetNodeAt(treeList1.PointToClient(Cursor.Position));
```

Focus / scroll to a found node:

```csharp
treeList1.FocusedNode = n;
treeList1.MakeNodeVisible(n);
n.Expanded = true;
```

## Move, Copy, Delete

```csharp
treeList1.MoveNode(child, newParent, isAfter: false);
treeList1.CopyNode(child, newParent, copyChildren: true);
treeList1.DeleteNode(child);

// Reorder among siblings:
treeList1.MoveNode(node, node.ParentNode, isAfter: true, indexInParent: 2);
```

## Expand / Collapse

```csharp
node.Expanded = true;
treeList1.ExpandAll();
treeList1.CollapseAll();
treeList1.ExpandToLevel(2);

// Save expanded state across reload. TreeList has no "expanded nodes" collection,
// so walk every node and capture the ones where Expanded is true (do NOT use
// treeList1.Selection — that is the *selected* nodes, not the expanded ones).
var expandedKeys = new HashSet<object>();
void CollectExpanded(TreeListNodes nodes) {
    foreach (TreeListNode n in nodes) {
        if (n.Expanded) expandedKeys.Add(n.GetValue(treeList1.KeyFieldName));
        CollectExpanded(n.Nodes);
    }
}
CollectExpanded(treeList1.Nodes);

// After reload, restore:
foreach (var key in expandedKeys) {
    var n = treeList1.FindNodeByKeyID(key);
    if (n != null) n.Expanded = true;
}
```

## Checkboxes

Enable per node:

```csharp
treeList1.OptionsView.ShowCheckBoxes = true;        // shows a check column on each node
node.CheckState = CheckState.Checked;
```

Recursive helpers:

```csharp
treeList1.SetCheckedChildNodesRecursive(node, CheckState.Checked);
treeList1.UpdateParentCheckState(node);             // toggles parents to tri-state
treeList1.AfterCheckNode += (s, e) => {
    treeList1.SetCheckedChildNodesRecursive(e.Node, e.Node.CheckState);
    if (e.Node.ParentNode != null)
        treeList1.UpdateParentCheckState(e.Node);
};
```

For column-level checkboxes (in addition to row-level), assign a `RepositoryItemCheckEdit` to a bool column.

## Virtual Mode — Lazy Children

Two flavors:

### TreeList-level events

```csharp
treeList1.BeforeExpand += (s, e) => {
    if (e.Node.Nodes.Count > 0) return;       // already loaded
    foreach (var child in api.GetChildren((int)e.Node["Id"]))
        treeList1.AppendNode(new object[] { child.Name }, e.Node);
};
```

Show the expand button on parents that have not loaded children yet:

```csharp
treeList1.VirtualTreeGetChildNodes += (s, e) => {
    e.Children = api.GetChildren((string)((MyItem)e.Node).Id);
};
```

### Source-level (`IVirtualTreeListData`)

Implement the interface on the data source — see [data-binding.md](data-binding.md).

## Custom Cell Values per Node

Override values without mutating the bound list:

```csharp
treeList1.CustomNodeCellEdit += (s, e) => {
    if (e.Column.FieldName == "Status" && (int)e.Node["Severity"] >= 5)
        e.RepositoryItem = warningEditor;
};

treeList1.CustomDrawNodeCell += (s, e) => { /* fully custom drawing */ };
```

## Filtering Nodes

`TreeList` participates in the standard filter pipeline (`ActiveFilterString`, `ActiveFilterCriteria`, Find Panel) — see [filtering-and-search.md](filtering-and-search.md). Choose `FilterMode`:

```csharp
treeList1.FilterMode = TreeListFilterMode.Smart;       // expand to show matching descendants
treeList1.OptionsFilter.NodesFiltrationMode = NodesFiltrationMode.HideOldestParents;
```

## Common Issues

- **`AppendNode(null, parent)` creates a phantom node**: pass an `object[]` of the right length (one item per column).
- **Nodes added but not visible**: call `treeList1.RefreshDataSource()` or wrap in `BeginUpdate` / `EndUpdate`.
- **`FindNodeByKeyID` returns `null`**: `KeyFieldName` is not set or the key value is the wrong type.
- **`Expanded = true` does nothing**: the node has no children, or `HasChildren` is `false` in virtual mode.
- **Checkboxes do not roll up to parents**: wire `AfterCheckNode` and call `UpdateParentCheckState`.

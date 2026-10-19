# Nodes — Access, Traverse, Expand, Check, Image

A TreeList node is a data row in the tree hierarchy, represented by `TreeListNode` (`DevExpress.XtraTreeList.Nodes`). In bound mode nodes are generated automatically; in unbound mode you create them with `AppendNode`.

## When to Use This Reference

- Finding and focusing nodes
- Reading / writing cell values
- Expanding / collapsing and traversing the tree
- Showing node checkboxes and images

## Finding Nodes

| Method | Finds by |
|---|---|
| `treeList.FindNodeByKeyID(object keyValue)` | `KeyFieldName` value |
| `treeList.FindNodeByFieldValue(string field, object value)` | any field value |
| `treeList.FindNodeByID(int id)` | node `Id` (the auto-assigned node identifier) |
| `treeList.FocusedNode` | the currently focused node (get/set) |

```csharp
TreeListNode node = treeList.FindNodeByFieldValue("Region", "Asia");
treeList.FocusedNode = node;
node.Expand();
```

## Cell Values

```csharp
// Via the TreeList (column by FieldName or TreeListColumn)
object v = treeList.GetRowCellValue(node, "Sales");
treeList.SetRowCellValue(node, "Sales", 12000m);
string text = treeList.GetRowCellDisplayText(node, treeList.Columns["Sales"]);

// Via the node
object v2 = node.GetValue("Sales");
node.SetValue("Sales", 12000m);
```

## Expand / Collapse

```csharp
node.Expanded = true;        // property
node.Expand();               // method
node.Collapse();

treeList.ExpandAll();        // whole tree
treeList.CollapseAll();
```

Expansion events: `BeforeExpand` / `AfterExpand` and `BeforeCollapse` / `AfterCollapse`. `BeforeExpand` is also where dynamic child loading happens (see [data-binding.md](data-binding.md)).

## Traversing the Tree

Walk children via `Nodes` and parents via `ParentNode`:

```csharp
void Walk(TreeListNode node) {
    foreach (TreeListNode child in node.Nodes) {
        // process child
        Walk(child);
    }
}

foreach (TreeListNode root in treeList.Nodes)
    Walk(root);
```

For large trees, the non-recursive `treeList.NodesIterator` (`OperationIterator` pattern) avoids deep recursion — use the MCP tool for the iterator API if needed.

## Add / Remove Nodes

```csharp
// Add (unbound) — see data-binding.md for AppendNode details
TreeListNode child = treeList.AppendNode(new object[] { "New", "..." }, parentNode);

// Remove
treeList.DeleteNode(child);          // or
node.TreeList.Nodes.Remove(child);
```

## Node Checkboxes

Show checkboxes in front of nodes (e.g., for multi-selection of items):

```csharp
treeList.OptionsView.ShowCheckBoxes = true;

// Recursively sync parent/child check states
treeList.OptionsBehavior.AllowRecursiveNodeChecking = true;

// Read / set a node's state (tri-state)
node.CheckState = System.Windows.Forms.CheckState.Checked;
```

When `AllowRecursiveNodeChecking` is on, checking a parent updates its children and vice versa (parents go to indeterminate when partially checked). React to changes via the `AfterCheckNode` event.

## Node Images

Assign images by index into an image list:

```csharp
treeList.SelectImageList = imageCollection1;     // ImageCollection / ImageList
node.ImageIndex = 0;                             // image when not focused
node.SelectImageIndex = 1;                       // image when focused
```

In `AppendNode`, optional `imageIndex` / `selectImageIndex` parameters set these directly. For SVG images, assign an `SvgImageCollection` to `StateImageList` / `SelectImageList` per your version (confirm via MCP).

## Source Material

- [TreeListNode](https://docs.devexpress.com/content/WindowsForms/DevExpress.XtraTreeList.Nodes.TreeListNode?md=true) — node reference

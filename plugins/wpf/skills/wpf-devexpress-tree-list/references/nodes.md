# Working With Nodes — DevExpress WPF TreeList

In `TreeListControl`, each data record renders as a **`TreeListNode`**. The tree itself is a nested structure: root nodes live in `TreeListView.Nodes`; each node has its own children in `TreeListNode.Nodes`. This reference covers the full node API — anatomy, obtaining, expand/collapse, iteration, checkboxes, images, and tree-restructuring (indent/outdent).

## When to Use This Reference

Use this when you need to:

- Understand what makes up a node visually (data cells, indents, expand button, checkbox, image)
- Find a specific node by key, content, cell value, row handle, or visible index
- Expand / collapse nodes programmatically or in response to UI
- Bind a node's expand state to a ViewModel property
- Iterate through nodes without recursion (`TreeListNodeIterator`)
- Show checkboxes on nodes (binary or tri-state, with recursive parent/child updates)
- Show images on nodes
- Move nodes within the tree (indent / outdent)
- Tune dynamic loading (`EnableDynamicLoading`, `FetchSublevelChildrenOnExpand`)
- Start the tree from a non-root level (`RootValue`)

## Node Anatomy

A node renders 5 visual elements (some optional):

| Element | Purpose | Controlled by |
|---|---|---|
| **Data Cell(s)** | One cell per visible column | `TreeListView.VisibleColumns` |
| **Indent(s)** | Horizontal offset showing nesting level | `TreeListView.RowIndent` (pixels), nesting depth, expand button presence |
| **Expand Button** | Toggles child visibility | Auto when node has children; force via `TreeListNode.IsExpandButtonVisible` |
| **Checkbox** | User can check/uncheck the node | `TreeListView.ShowCheckboxes`, `CheckBoxFieldName` |
| **Image** | Icon shown left of the data | `TreeListView.ShowNodeImages`, `ImageFieldName`, or `TreeListNode.Image` |

In **bound mode** (with `KeyFieldName` / `ParentFieldName` or `ChildNodesPath`), nodes are auto-created per data record. In **unbound mode**, you create `TreeListNode` instances and add them to `Nodes` collections. See [data-binding.md](data-binding.md) for the binding strategies.

Source: `articles/controls-and-libraries/data-grid/grid-view-data-layout/nodes/nodes-overview.md`.

## Root Nodes

Root nodes have no parent — their `ParentID` doesn't match any other node's key (or is `null` in nullable-key designs).

`TreeListView.Nodes` collection contains the root nodes:

```csharp
foreach (var root in view.Nodes) {
    // root is TreeListNode
    foreach (var child in root.Nodes) {
        // child of root
    }
}
```

### Start From a Different Hierarchy Level — `RootValue`

```xaml
<dxg:TreeListView KeyFieldName="ID"
                  ParentFieldName="ParentID">
    <dxg:TreeListView.RootValue>
        <sys:Int32>5</sys:Int32>
    </dxg:TreeListView.RootValue>
</dxg:TreeListView>
```

```xml
xmlns:sys="clr-namespace:System;assembly=mscorlib"
```

With `RootValue = 5`, only nodes whose `ParentID = 5` appear as roots. Nodes whose `ParentID` doesn't match `RootValue` AND doesn't match any other node's `ID` are hidden from the tree entirely.

The value type **must match** the `KeyFieldName` field's type (here `int`, so `sys:Int32`).

## Obtain Nodes

Six lookup methods on `TreeListView`:

| Method | Returns the node by |
|---|---|
| `FocusedNode` | (Property) — the currently focused node |
| `GetNodeByKeyValue(object key)` | Key value (bound mode only) |
| `GetNodeByCellValue(string fieldName, object value)` | First match on a column value |
| `GetNodeByContent(object dataItem)` | The bound data object's identity |
| `GetNodeByRowHandle(int rowHandle)` | Row handle (stable across visibility changes) |
| `GetNodeVisibleIndex(TreeListNode node)` | (Reverse direction) — gives the node's current visible index |

```csharp
// By key
var manager = view.GetNodeByKeyValue(42);

// By data object (e.g., after focusing in code)
var node = view.GetNodeByContent(selectedEmployee);

// Iterate visible nodes
for (int i = 0; i < view.DataControl.VisibleRowCount; i++) {
    int handle = view.DataControl.GetRowHandleByVisibleIndex(i);
    var node = view.GetNodeByRowHandle(handle);
    // ...
}
```

### Row Handles vs Visible Indices

- **Row handle** — stable integer identifier; remains valid even when the node is inside a collapsed parent.
- **Visible index** — index among currently visible nodes; `-1` if hidden inside a collapsed group; renumbers when nodes expand/collapse.

Use row handles for stable references; use visible indices for UI positioning (e.g., scroll-to-row).

Source: `articles/controls-and-libraries/data-grid/grid-view-data-layout/nodes/obtain-nodes.md`.

## Expand and Collapse Nodes

### UI Shortcuts

| Keyboard | Action |
|---|---|
| <kbd>Ctrl</kbd>+<kbd>←</kbd> / <kbd>Ctrl</kbd>+<kbd>→</kbd> | Collapse / expand focused node |
| <kbd>←</kbd> / <kbd>→</kbd> (without Ctrl) | Same — only when `NavigationStyle="Row"` OR `ExpandCollapseNodesOnNavigation="True"` |
| Click expand button | Toggle |

### Code API

| API | Effect |
|---|---|
| `TreeListNodeBase.IsExpanded` | Get / set node expanded state |
| `TreeListView.ExpandNode(int rowHandle)` | Expand one node |
| `TreeListView.CollapseNode(int rowHandle)` | Collapse one node |
| `TreeListView.ChangeNodeExpanded(int rowHandle, bool isExpanded)` | Toggle to specific state |
| `TreeListView.ExpandToLevel(int level)` | Expand all parents down to the given level (root = 0) |
| `TreeListNodeBase.ExpandAll()` / `CollapseAll()` | Expand / collapse all descendants of a specific node |
| `TreeListView.ExpandAllNodes()` / `CollapseAllNodes()` | Expand / collapse all nodes in the tree |
| `TreeListView.AutoExpandAllNodes` | Expand all on load (XAML / startup) |

```csharp
// Expand all
view.ExpandAllNodes();

// Expand to a specific depth
view.ExpandToLevel(2);   // Expand roots, level 1, and level 2

// Per-node
var node = view.GetNodeByKeyValue(42);
node.IsExpanded = true;
```

### Expand / Collapse Events

| Event | When |
|---|---|
| `NodeExpanding` | Before expand — `e.Cancel = true` blocks |
| `NodeExpanded` | After expand |
| `NodeCollapsing` | Before collapse — `e.Cancel = true` blocks |
| `NodeCollapsed` | After collapse |

```csharp
view.NodeExpanding += (s, e) => {
    if (ShouldBlockExpand(e.Node))
        e.Cancel = true;
};
```

### Bind Expand State to Data

Persist expand/collapse state across sessions or restore from a saved layout:

| Property | Effect |
|---|---|
| `TreeListView.ExpandStateBinding` | A `Binding` whose source is each node's data item; resolves to `bool` |
| `TreeListView.ExpandStateFieldName` | The name of a Boolean field in the data source |

```xaml
<dxg:TreeListView ExpandStateFieldName="IsExpanded"/>
```

Or:

```xaml
<dxg:TreeListView ExpandStateBinding="{Binding IsOpen}"/>
```

`ExpandStateBinding` takes precedence over `ExpandStateFieldName` when both are set.

In unbound mode (XAML node trees), use:

```xaml
<dxg:TreeListNode ExpandStateBinding="{Binding IsExpanded}">
    <dxg:TreeListNode.Content>
        <local:Item Name="Root"/>
    </dxg:TreeListNode.Content>
</dxg:TreeListNode>
```

Source: `articles/controls-and-libraries/data-grid/grid-view-data-layout/nodes/expand-and-collapse-nodes.md`.

### Dynamic Loading (Hierarchical Binding)

When binding to hierarchical data (`ChildNodesPath` / `ChildNodesSelector`), the grid creates child nodes lazily on expand by default. Tuning properties:

| Property | Default | Effect |
|---|---|---|
| `TreeListView.EnableDynamicLoading` | `true` | If `false`, all nodes are created at startup (slower load, faster expand) |
| `TreeListView.FetchSublevelChildrenOnExpand` | (verify default) | If `false`, children of children are NOT fetched until that level expands |
| `TreeListView.HasChildNodesPath` | — | Path / field used to determine whether to show the expand button before fetching children |
| `TreeListView.ExpandNodesOnFiltering` | `false` | If `true`, expand nodes whose children match the search/filter |

`HasChildNodesPath` matters when `FetchSublevelChildrenOnExpand = false`: without it, every node shows an expand button (since the grid doesn't yet know if children exist). With it, the grid evaluates the property to decide button visibility.

Source: `articles/controls-and-libraries/data-grid/grid-view-data-layout/nodes/expand-and-collapse-nodes.md` § "Related API".

## Iterate Through Nodes — `TreeListNodeIterator`

Walk the tree without writing recursion. Iteration order is depth-first (parent → children → grandchildren → ...).

```csharp
using DevExpress.Xpf.Grid;

// All nodes starting from the first root
var iterator = new TreeListNodeIterator(view.Nodes);
while (iterator.MoveNext()) {
    var node = iterator.Current;
    Debug.WriteLine($"{new string(' ', node.Level * 2)}{node.Content}");
}
```

### Constructors

`TreeListNodeIterator` has multiple constructors:
- `(TreeListNodeCollection nodes)` — start from a collection
- `(TreeListNode startNode)` — start from a specific node
- Variants with `bool visibleOnly` — only visible nodes (skips collapsed branches)

### Reset

```csharp
iterator.Reset();   // Restart from the initial state
```

Source: `articles/controls-and-libraries/data-grid/grid-view-data-layout/nodes/iterate-through-nodes.md`.

## Checkboxes

Embed checkboxes in each node for multi-select / mark-as-done / inclusion-list scenarios.

### Enable + Bind to a Boolean Field

```xaml
<dxg:TreeListView ShowCheckboxes="True"
                  CheckBoxFieldName="OnVacation"/>
```

```csharp
public class Employee {
    public int ID { get; set; }
    public int ParentID { get; set; }
    public string Name { get; set; } = "";
    public bool OnVacation { get; set; }
}
```

`CheckBoxFieldName` must point to a `bool` (or `bool?` for indeterminate) field on the data class. Two-way binding by default.

### Immediate Update

By default, the grid does NOT post the focused row's check value to the data source until the user moves to another row. To post immediately:

```xaml
<dxg:TreeListView ShowCheckboxes="True"
                  CheckBoxFieldName="OnVacation"
                  ImmediateUpdateCheckBoxState="True"/>
```

### Check / Uncheck in Code

```csharp
view.CheckAllNodes();
view.UncheckAllNodes();

// Per-node
var node = view.GetNodeByKeyValue(42);
node.IsChecked = true;    // null → indeterminate (when AllowIndeterminateCheckState=True)
```

`TreeListView.NodeCheckStateChanged` event fires when a check state changes.

### Indeterminate State (Tri-State Checkboxes)

```xaml
<dxg:TreeListView AllowIndeterminateCheckState="True"/>
```

`TreeListNode.IsChecked` is `bool?`:
- `true` — checked
- `false` — unchecked
- `null` — indeterminate

### Recursive Checking (Parent ↔ Children Sync)

```xaml
<dxg:TreeListView AllowRecursiveNodeChecking="True"/>
```

Behavior:
- Check / uncheck a parent → all children become checked / unchecked
- Check / uncheck all children → parent becomes checked / unchecked
- Check / uncheck some children → parent becomes indeterminate

Common pattern for tree-style filters or selection trees.

### Enable / Disable Checkbox Per Node

Bind whether the checkbox is interactive (greyed out vs clickable):

```xaml
<dxg:TreeListView IsCheckBoxEnabledFieldName="Enabled"/>
```

Or:

```xaml
<dxg:TreeListView IsCheckBoxEnabledBinding="{Binding CanModify}"/>
```

`IsCheckBoxEnabledBinding` takes precedence over `IsCheckBoxEnabledFieldName`.

Source: `articles/controls-and-libraries/data-grid/grid-view-data-layout/nodes/check-nodes.md`.

## Node Images

```xaml
<dxg:TreeListView ShowNodeImages="True"
                  ImageFieldName="Icon"/>
```

Where `Icon` is a column of `ImageSource` / `byte[]` / `BitmapImage`.

Or set the image explicitly on a node:

```csharp
node.Image = new BitmapImage(new Uri("pack://application:,,,/Images/folder.png"));
```

## Move Nodes (Indent / Outdent)

Restructure the tree by changing a node's parent.

### Indent — Make a Node a Child of Its Predecessor

```csharp
view.IndentNode(node);
view.IndentNodes(selectedNodes);     // Multiple
```

Or use the command for selected nodes:

```xaml
<Button Content="Indent" Command="{x:Static dxg:TreeListViewCommands.IndentSelectedNodes}"/>
```

### Outdent — Move a Node to the Parent Level

```csharp
view.OutdentNode(node);
view.OutdentNodes(selectedNodes);
```

Or:

```xaml
<Button Content="Outdent" Command="{x:Static dxg:TreeListViewCommands.OutdentSelectedNodes}"/>
```

> Both operations move children along with the node. Indent makes node a child of the previous sibling; outdent makes the node a sibling of its parent.

Source: `articles/controls-and-libraries/data-grid/grid-view-data-layout/nodes/move-nodes.md`.

## TreeListNode API Surface

`DevExpress.Xpf.Grid.TreeListNode` (inherits `DevExpress.Data.TreeList.TreeListNodeBase`):

| Member | Description |
|---|---|
| `Content` | The underlying data object (bound) or your assigned content (unbound) |
| `Nodes` | Collection of child nodes |
| `ParentNode` | Reference to parent (null for root) |
| `Level` | Nesting depth (0 = root) |
| `IsExpanded` | Expand state |
| `IsExpandButtonVisible` | Force show/hide expand button |
| `IsChecked` | Check state (`bool?` with tri-state support) |
| `Image` | Per-node image |
| `RowHandle` | Stable identifier |
| `ExpandStateBinding` | Per-node binding for expand state (unbound mode) |
| `ExpandAll()` / `CollapseAll()` | Expand / collapse all descendants of this node |

## Add / Remove Nodes Programmatically

### Bound Mode

Modify the underlying collection — the grid reflects the change automatically if the collection raises `INotifyCollectionChanged`:

```csharp
employees.Add(new Employee { ID = 99, ParentID = 0, Name = "New" });
employees.Remove(existing);
```

For self-referential mode, the `ParentID` of each new item must match an existing node's key (or root value).

### Unbound Mode

```csharp
using DevExpress.Xpf.Grid;

var newNode = new TreeListNode(new ProjectObject { Name = "New Phase" });
parentNode.Nodes.Add(newNode);

parentNode.Nodes.Remove(existingChild);
```

## Filter Nodes

When `TreeListView` filters data:
- Parent nodes remain visible if any descendant matches (path to match is preserved).
- `TreeListView.ExpandNodesOnFiltering="True"` auto-expands matching paths.

```xaml
<dxg:TreeListView ExpandNodesOnFiltering="True"/>
```

Per-node filter via `TreeListView.CustomNodeFilter` event / `CustomNodeFilterCommand` — see `articles/controls-and-libraries/data-grid/grid-view-data-layout/nodes/filter-nodes.md`.

## Sort Nodes

`TreeListView` sorts within each level — siblings are sorted relative to each other, hierarchy is preserved. See:
- `articles/controls-and-libraries/data-grid/grid-view-data-layout/nodes/sort-nodes.md`
- [end-user-features.md § Sorting](end-user-features.md)

## Common Issues

- **`GetNodeByContent` returns null** — the comparison uses object identity (`object.Equals`). Override `Equals` / `GetHashCode` on your data class or pass the exact same reference.
- **`RootValue` doesn't filter** — type mismatch with `KeyFieldName` field. Cast the XAML value explicitly with `sys:Int32` / `sys:String` etc.
- **Checkbox value not posted to source** — set `ImmediateUpdateCheckBoxState="True"`.
- **All nodes show expand button even when childless** — using hierarchical binding with `FetchSublevelChildrenOnExpand="False"`. Set `HasChildNodesPath` to a property that returns whether the item has children.
- **Indent fails / no effect** — node is already at the top of its sibling list (no predecessor to nest under) or recursive checking is interfering. Check node positions first.
- **`ExpandToLevel` doesn't expand level 0** — level 0 is the root; root nodes are always "expanded" in the sense of being visible. Use level 1+ to drill down.
- **Iterator skips collapsed nodes** — used `visibleOnly: true` in the constructor. Pass `false` (or omit) to walk all nodes.
- **Recursive checking causes performance issues on large trees** — every check propagates up and down. Use `BeginInit` / `EndInit` patterns on the view if available, or temporarily disable recursive checking during bulk updates.

## Source Material

- `articles/controls-and-libraries/data-grid/grid-view-data-layout/nodes/nodes-overview.md`
- `articles/controls-and-libraries/data-grid/grid-view-data-layout/nodes/obtain-nodes.md`
- `articles/controls-and-libraries/data-grid/grid-view-data-layout/nodes/expand-and-collapse-nodes.md`
- `articles/controls-and-libraries/data-grid/grid-view-data-layout/nodes/iterate-through-nodes.md`
- `articles/controls-and-libraries/data-grid/grid-view-data-layout/nodes/check-nodes.md`
- `articles/controls-and-libraries/data-grid/grid-view-data-layout/nodes/move-nodes.md`
- `articles/controls-and-libraries/data-grid/grid-view-data-layout/nodes/sort-nodes.md`
- `articles/controls-and-libraries/data-grid/grid-view-data-layout/nodes/filter-nodes.md`
- `articles/controls-and-libraries/tree-list/end-user-capabilities/expanding-and-collapsing-nodes.md`
- `articles/controls-and-libraries/tree-list/end-user-capabilities/navigating-through-nodes-and-cells.md`

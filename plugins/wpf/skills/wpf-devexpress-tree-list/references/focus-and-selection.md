# Focus and Selection — DevExpress WPF TreeList

`TreeListControl` shares the focus and selection infrastructure with `GridControl` — same `DataViewBase` / `DataControlBase` API surface with TreeList-specific extensions for nodes (`FocusedNode`, `GetSelectedNodes`, node expansion in selection state).

**For the full focus / selection reference (`NavigationStyle`, `SelectionMode`, `CurrentItem`, `SelectedItems`, `BeginSelection`/`EndSelection`, Check-Box Selector Column, Selection Rectangle, MVVM binding, keyboard navigation tables, custom selected appearance) see [the data-grid skill's `references/focus-and-selection.md`](../../wpf-devexpress-data-grid/references/focus-and-selection.md).** This reference covers only TreeList-specific concerns.

## When to Use This Reference

Use this when you need to:

- Read or set the focused node (`FocusedNode` vs `FocusedRowHandle` vs `CurrentItem`)
- Get selected nodes specifically (`GetSelectedNodes` instead of `GetSelectedRowHandles`)
- Configure multi-node or multi-cell selection
- Show the Check-Box Selector Column in a tree
- Bind selection state to a ViewModel
- Handle tree-specific focus events (expand/collapse during navigation)

## TreeList-Specific Focus API

### `TreeListView.FocusedNode`

The single most TreeList-specific focus property:

```csharp
TreeListNode focused = treeListView.FocusedNode;
treeListView.FocusedNode = someOtherNode;   // Set focus to a specific TreeListNode
```

`FocusedNode` is the **node object** (with full `Content`, `Nodes`, `Level`, `IsExpanded` access). Compare with:

| API | Type | Returns |
|---|---|---|
| `DataViewBase.FocusedRowHandle` | `int` | Row handle (stable identifier) |
| `DataControlBase.CurrentItem` | `object` | The bound data item |
| `TreeListView.FocusedNode` | `TreeListNode` | The node object (TreeList only) |

Use `FocusedNode` when you need access to node-level concepts: child collection, expand state, recursive operations. Use `CurrentItem` for MVVM binding (it's binding-friendly). Use `FocusedRowHandle` for stable references across visibility changes.

### Move Focus to a Node

```csharp
var node = view.GetNodeByKeyValue(42);
view.FocusedNode = node;
```

Or move to a node's parent / first child:

```csharp
view.FocusedNode = view.FocusedNode.ParentNode;      // Move up
view.FocusedNode = view.FocusedNode.Nodes.FirstOrDefault();   // Move down
```

### Focus Events

`TreeListView` raises the same events as `GridControl`:

- `FocusedRowHandleChanging` / `FocusedRowHandleChanged`
- `CurrentItemChanged`

Plus the tree-specific node expansion events (see [nodes.md](nodes.md)):
- `NodeExpanding` / `NodeExpanded` / `NodeCollapsing` / `NodeCollapsed`

These fire independently of focus changes. Expand state and focus are orthogonal.

## TreeList-Specific Selection API

### `GetSelectedNodes()` — Returns `TreeListNode` Collection

The TreeList-specific selection getter:

```csharp
IList<TreeListNode> selectedNodes = treeListControl.GetSelectedNodes();
foreach (var node in selectedNodes) {
    Debug.WriteLine($"Level {node.Level}: {node.Content}");
}
```

Returned nodes are **ordered by visible index** (top-to-bottom in the tree).

Compare with `DataControlBase.GetSelectedRowHandles()` — same concept, but returns `int[]` row handles instead of node objects. Use whichever is more convenient for your operation:

| When to use | API |
|---|---|
| Need node-level operations (`IsExpanded`, `Nodes`, `Level`, `ParentNode`) | `GetSelectedNodes()` |
| Need stable identifiers / pass to `GetRow(handle)` / `GetCellValue(handle, ...)` | `GetSelectedRowHandles()` |
| Need data objects only | `SelectedItems` |

### Selection Mode

```xaml
<dxg:TreeListControl SelectionMode="MultipleRow">
    <dxg:TreeListControl.View>
        <dxg:TreeListView/>
    </dxg:TreeListControl.View>
</dxg:TreeListControl>
```

Same `MultiSelectMode` enum as `GridControl`: `None`, `Row`, `MultipleRow`, `Cell`, `MultipleCell`. Same semantics.

### Selection on Tree Structure

When a parent node is collapsed, its descendant selections **stay selected** but are hidden. Re-expanding restores their visible selection state. `SelectedItems` always returns ALL selected items (visible + hidden under collapsed parents).

To preserve selection across `ItemsSource` reassignment, use `RestoreStateOnSourceChange="True"` + `RestoreStateKeyFieldName="..."` — see [save-restore-layout.md](save-restore-layout.md).

## Cell Selection in TreeList

Cell selection in a tree uses node + column instead of row handle + column:

```csharp
view.SelectCell(node, column);                                    // (TreeListView)
view.SelectCells(startNode, startColumn, endNode, endColumn);     // Range
view.UnselectCell(node, column);
```

Note the `TreeListNode` parameter — compare to `TableView.SelectCell(int rowHandle, GridColumn)`.

```xaml
<dxg:TreeListView CanSelectCell="View_CanSelectCell"
                  CanUnselectCell="View_CanUnselectCell"/>
```

```csharp
void View_CanSelectCell(object sender, CanSelectCellEventArgs e) {
    // e.Node, e.Column are TreeList-specific
    e.CanSelectCell = e.Column.FieldName != "InternalId";
}
```

> Verify exact `CanSelectCellEventArgs` member names against the TreeListView apidoc — the event signature may include `Node` directly or `RowHandle` (then derive node via `GetNodeByRowHandle`).

## Check-Box Selector Column in TreeList

Works the same as in `GridControl` (`ShowCheckBoxSelectorColumn`, `CheckBoxSelectorColumnPosition`, etc.). See data-grid's [focus-and-selection.md § Check-Box Selector Column](../../wpf-devexpress-data-grid/references/focus-and-selection.md).

> Don't confuse with **node checkboxes** (`ShowCheckboxes` + `CheckBoxFieldName`) — those are per-row data checkboxes for app-specific marking (e.g., "On Vacation"). Selector column is for row selection.

See [nodes.md § Checkboxes](nodes.md) for node-level checkboxes.

## MVVM Binding

```xaml
<dxg:TreeListControl ItemsSource="{Binding Employees}"
                     SelectedItem="{Binding SelectedEmployee, Mode=TwoWay}"
                     SelectedItems="{Binding SelectedEmployees, Mode=TwoWay}"
                     SelectionMode="MultipleRow">
    <dxg:TreeListControl.View>
        <dxg:TreeListView KeyFieldName="ID" ParentFieldName="ParentID"/>
    </dxg:TreeListControl.View>
</dxg:TreeListControl>
```

`CurrentItem`, `SelectedItem`, `SelectedItems` are all TwoWay-bindable. The ViewModel just sets the data object and the grid finds the matching node automatically.

## Navigation Style Note

`NavigationStyle="Row"` enables a tree-list-specific keyboard shortcut: pressing <kbd>Left</kbd> / <kbd>Right</kbd> (without <kbd>Ctrl</kbd>) expands / collapses the focused node. Without `Row` navigation, users need <kbd>Ctrl</kbd>+<kbd>←</kbd> / <kbd>Ctrl</kbd>+<kbd>→</kbd>.

Alternative: set `TreeListView.ExpandCollapseNodesOnNavigation="True"` to enable <kbd>Left</kbd> / <kbd>Right</kbd> regardless of `NavigationStyle`.

See [nodes.md § Expand and Collapse](nodes.md) for full keyboard shortcuts.

## Common Issues

- **`FocusedNode = newNode` doesn't work if newNode is hidden under collapsed parent** — expand the parent first: `newNode.ParentNode.IsExpanded = true; view.FocusedNode = newNode;`
- **`GetSelectedNodes()` returns nodes in unexpected order** — they're ordered by **visible index**, not source-collection order. Visible index depends on current expand state.
- **`SelectedItems` returns hidden items** — by design. Items selected before a parent collapsed remain in `SelectedItems` (just invisible). Use `GetSelectedNodes()` then filter `node.ParentNode.IsExpanded` if you need only visible.
- **Selection lost after `ItemsSource` reassign** — set `RestoreStateOnSourceChange="True"` + `RestoreStateKeyFieldName`. See [save-restore-layout.md](save-restore-layout.md).
- **TreeListView CellSelectionMode doesn't work** — TreeList cell selection uses node-based API (`SelectCell(node, column)`), not row-handle-based. Ensure your code passes `TreeListNode` objects.

## Apply to GridControl

For shared concepts (NavigationStyle, SelectionMode, BeginSelection / EndSelection, Selection Rectangle, hover effects, selection fade, MVVM binding, keyboard navigation):

See [the data-grid skill's focus-and-selection.md](../../wpf-devexpress-data-grid/references/focus-and-selection.md).

This tree-list reference covers only TreeList-specific extensions (`FocusedNode`, `GetSelectedNodes`, node-based cell selection, tree-specific keyboard shortcuts).

## Source Material

- `articles/controls-and-libraries/data-grid/focus-navigation-selection/focus.md`
- `articles/controls-and-libraries/data-grid/focus-navigation-selection/multiple-row-selection.md`
- `articles/controls-and-libraries/data-grid/focus-navigation-selection/multiple-cell-selection.md`
- `articles/controls-and-libraries/data-grid/focus-navigation-selection/common-selection-features.md`
- `articles/controls-and-libraries/tree-list/end-user-capabilities/selecting-multiple-nodes.md`
- `articles/controls-and-libraries/tree-list/end-user-capabilities/navigating-through-nodes-and-cells.md`
- The data-grid skill's `references/focus-and-selection.md` (shared API surface)

# Drag-and-Drop — DevExpress WPF TreeList

`TreeListControl` supports drag-and-drop with the same `DataViewBase`-level API as `GridControl` (six events, drag hint, drop marker, auto-expand on hover) — plus tree-specific semantics: dropping **inside** a node makes the dragged node a child (creating a parent-child relationship), and the `IndentNode` / `OutdentNode` API enables restructuring without drag.

**For the full reference (`AllowDragDrop`, the six events, `RecordDragDropData`, `DragDropHintTemplate`, `DropMarkerTemplate`, cross-grid drag, cross-app drag with .NET 9 limitations) see [the data-grid skill's `references/drag-and-drop.md`](../../devexpress-wpf-data-grid/references/drag-and-drop.md).** This reference covers TreeList-specific concerns.

## When to Use This Reference

Use this when you need to:

- Allow users to reorganize the tree by dragging nodes (change parent / siblings)
- Auto-expand collapsed parent nodes when hovering during drag
- Drag nodes between two `TreeListControl` instances
- Restructure the tree programmatically (`IndentNode` / `OutdentNode`)
- Block specific nodes from being dragged or being a drop target
- Handle TreeList-specific drop position semantics (`Inside` makes child)

## Enable Drag-and-Drop

```xaml
<dxg:TreeListControl>
    <dxg:TreeListControl.View>
        <dxg:TreeListView AllowDragDrop="True"
                          AutoExpandOnDrag="True"
                          AutoExpandDelayOnDrag="500"
                          AllowScrollingOnDrag="True"/>
    </dxg:TreeListControl.View>
</dxg:TreeListControl>
```

`AutoExpandOnDrag` is **especially important for trees**: when the user hovers a dragged item over a collapsed parent for `AutoExpandDelayOnDrag` ms, the parent expands so the user can drop into a specific child position.

Source: `articles/controls-and-libraries/data-grid/drag-and-drop/drag-and-drop-options.md`.

## TreeList-Specific Drop Position

The `DropPosition` enum (`DevExpress.Xpf.Core`) has four values:

| Value | Effect in TreeList |
|---|---|
| `Before` | Dropped item becomes a **previous sibling** of the target node |
| `After` | Dropped item becomes a **next sibling** of the target node |
| **`Inside`** | Dropped item becomes a **child** of the target node — the parent-child relationship changes |
| `Append` | Dropped item is appended as the **last child** of the target node |

For `GridControl`'s `TableView`, only `Before`/`After` are meaningful (rows have no parent-child). For TreeList, `Inside` is the most powerful — it lets users reparent nodes by drag.

Drop position is computed automatically from the hover position relative to the target node (top third = `Before`, middle = `Inside`, bottom third = `After`). Override in `DragRecordOver`:

```csharp
void OnDragRecordOver(object sender, DragRecordOverEventArgs e) {
    // Force only sibling drops (block re-parenting)
    if (e.DropPosition == DropPosition.Inside)
        e.DropPosition = DropPosition.After;
}
```

## Pattern: Block Re-Parenting

If your domain doesn't allow changing parent-child relationships (e.g., a strict org-chart hierarchy), block `Inside` drops:

```xaml
<dxg:TreeListView AllowDragDrop="True"
                  DragRecordOver="OnDragRecordOver"/>
```

```csharp
void OnDragRecordOver(object sender, DragRecordOverEventArgs e) {
    if (e.DropPosition == DropPosition.Inside) {
        e.Effects = DragDropEffects.None;   // Reject the drop
        e.Handled = true;
    }
}
```

## Pattern: Drag Only Leaf Nodes

Block dragging nodes that have children (e.g., disallow moving an entire subtree):

```csharp
void OnStartRecordDrag(object sender, StartRecordDragEventArgs e) {
    if (e.Records.Any(r => view.GetNodeByContent(r).HasChildren)) {
        e.AllowDrag = false;
        e.Handled = true;
    }
}
```

This is exact pattern from `articles/controls-and-libraries/data-grid/drag-and-drop/process-drag-and-drop-operations.md` § "Prevent Dragging Specific Records".

## Pattern: Restrict Drop to Specific Levels

```csharp
void OnDragRecordOver(object sender, DragRecordOverEventArgs e) {
    var targetNode = view.GetNodeByContent(e.TargetRecord);

    // Block drops onto root-level (Department) nodes
    if (targetNode.Level == 0 && e.DropPosition == DropPosition.Inside) {
        e.Effects = DragDropEffects.None;
        e.Handled = true;
    }
}
```

`TreeListNode.Level` gives nesting depth (0 = root, 1 = first level child, etc.).

## Pattern: Modify Data After Drop (Tree-Specific)

When a node is dropped inside another, update the data model's parent reference:

```csharp
void OnDropRecord(object sender, DropRecordEventArgs e) {
    var payload = (RecordDragDropData)e.Data.GetData(typeof(RecordDragDropData));
    var draggedItems = payload.Records.Cast<Employee>().ToList();
    var target = (Employee)e.TargetRecord;

    if (e.DropPosition == DropPosition.Inside) {
        // Dropped INTO target — target becomes new parent
        foreach (var emp in draggedItems)
            emp.ParentID = target.ID;
    } else {
        // Dropped Before/After — same parent as target (becomes sibling)
        foreach (var emp in draggedItems)
            emp.ParentID = target.ParentID;
    }
}
```

For self-referential trees (`KeyFieldName`/`ParentFieldName`), this is the key: change `ParentID` to reparent. The grid rebuilds the tree on the next data refresh.

For hierarchical trees (`ChildNodesPath`), move the item between parents' `Children` collections.

## Programmatic Restructure — `IndentNode` / `OutdentNode`

For tree-restructuring via toolbar buttons or commands (not drag-drop):

```csharp
view.IndentNode(node);                  // Make this node a child of its previous sibling
view.IndentNodes(selectedNodes);        // Bulk

view.OutdentNode(node);                 // Move node to its parent's level (becomes parent's sibling)
view.OutdentNodes(selectedNodes);       // Bulk
```

Or use built-in commands for selected nodes:

```xaml
<Button Content="Indent" Command="{x:Static dxg:TreeListViewCommands.IndentSelectedNodes}"/>
<Button Content="Outdent" Command="{x:Static dxg:TreeListViewCommands.OutdentSelectedNodes}"/>
```

Source: `articles/controls-and-libraries/data-grid/grid-view-data-layout/nodes/move-nodes.md`. See also [nodes.md § Move Nodes](nodes.md).

> Indent / outdent move the node **with its children**. If the node has 10 descendants, the whole subtree moves.

## Auto-Expand on Drag-Over

The most TreeList-specific dragging behavior. Without this, users can't drop into a collapsed branch:

```xaml
<dxg:TreeListView AutoExpandOnDrag="True"
                  AutoExpandDelayOnDrag="500"/>
```

- `AutoExpandOnDrag="True"` — required for any drag-into-collapsed-node UX
- `AutoExpandDelayOnDrag` — ms before the parent expands. Too short = jittery; too long = user gives up. 500 ms is a reasonable default.

When the user moves away before the delay elapses, the parent stays collapsed.

## Drop Marker — Tree-Specific Indicator

The drop marker shows where the drop will land (between siblings = line; into a node = highlighted node frame). Customize:

```xaml
<dxg:TreeListView.DropMarkerTemplate>
    <DataTemplate>
        <Path Stroke="DarkGreen" StrokeThickness="3" Data="M 0,0 L 30,0"/>
    </DataTemplate>
</dxg:TreeListView.DropMarkerTemplate>
```

> Tree-specific: when `DropPosition="Inside"`, the marker renders as a node frame (not a line), so a `Path`-based custom template may not visually fit `Inside` drops. Test with all three drop positions.

## Drag-Drop Between Two TreeLists

Identical setup on both:

```xaml
<dxg:TreeListControl x:Name="tree1" ItemsSource="{Binding LeftItems}">
    <dxg:TreeListControl.View>
        <dxg:TreeListView AllowDragDrop="True"/>
    </dxg:TreeListControl.View>
</dxg:TreeListControl>

<dxg:TreeListControl x:Name="tree2" ItemsSource="{Binding RightItems}">
    <dxg:TreeListControl.View>
        <dxg:TreeListView AllowDragDrop="True"/>
    </dxg:TreeListControl.View>
</dxg:TreeListControl>
```

Cross-tree drag works automatically when:
- Both `ItemsSource` collections implement `IList`
- Both bound collections accept the same data type (or one accepts a base type)

For `DropPosition="Inside"` to make sense across trees, the data class needs the parent ID concept on both sides.

## Drag-Drop Between TreeList and External Controls

Same approach as Grid: handle `StartRecordDrag` to package data, attach standard WPF `DragDrop` events on the target. See [data-grid drag-and-drop.md § Drag Between GridControl and Standard WPF Controls](../../devexpress-wpf-data-grid/references/drag-and-drop.md).

For `ListBoxEdit` specifically, use `ListBoxDragDropBehavior` attached properties on the target.

## Block Removal From Source (Copy Mode)

Same as Grid — handle `CompleteRecordDragDrop` and set `e.Handled = true`:

```csharp
void OnCompleteRecordDragDrop(object sender, CompleteRecordDragDropEventArgs e) {
    e.Handled = true;   // Don't remove from source = copy semantic
}
```

Useful when dragging from a "template tree" into a "working tree".

## TreeList-Specific Caveats

### Drag Sorted / Grouped Data

Same as Grid — drag-drop is disabled when data is sorted or grouped unless:

```xaml
<dxg:TreeListView AllowDragDrop="True"
                  AllowSortedDataDragDrop="True"/>
```

But the semantics get murky: dragging a node when groups are visible may move it into an unexpected position. Use with care.

### Unbound Tree Drag-Drop

When the tree is built in unbound mode (XAML / code `TreeListNode` tree), drag-drop still works at the View level. But data modifications (`e.Handled = true` to suppress removal) don't directly translate to node modifications — you must implement `DropRecord` to manipulate `TreeListNode.Nodes` collections explicitly.

### .NET 9 Cross-App Limitation

Inherits the same restriction as Grid: .NET 9 cross-app drag requires the BinaryFormatter compatibility package. See [data-grid drag-and-drop.md § .NET 9+ Limitation](../../devexpress-wpf-data-grid/references/drag-and-drop.md).

## Common Issues

- **Drag works but tree doesn't reparent** — your `DropRecord` handler doesn't update the data model's `ParentID` (self-referential) or doesn't move between `Children` collections (hierarchical). The grid doesn't auto-reparent — you must.
- **Auto-expand doesn't fire** — `AutoExpandOnDrag` is false, or hover time is shorter than `AutoExpandDelayOnDrag`. Set `AutoExpandDelayOnDrag` to a smaller value (e.g., 300 ms) for snappier UX.
- **`DropPosition.Inside` rejected by default** — check `e.Effects` in `DragRecordOver`. Default `Move` effect should accept Inside. If you set `Effects = None` conditionally, double-check the condition.
- **Drag-drop disabled even with `AllowDragDrop=True`** — data is sorted or grouped. Set `AllowSortedDataDragDrop="True"`.
- **`IndentNode` does nothing** — node is already at the top of its sibling list (no previous sibling to become parent of). This is expected behavior.

## Apply to GridControl

For shared concepts (`AllowDragDrop`, six events, `RecordDragDropData`, `DragDropHintTemplate`, `DropMarkerTemplate`, cross-app drag, .NET 9 BinaryFormatter): see [the data-grid skill's `references/drag-and-drop.md`](../../devexpress-wpf-data-grid/references/drag-and-drop.md).

This tree-list reference covers tree-specific semantics: `DropPosition.Inside`, `AutoExpandOnDrag`, indent/outdent, level-aware drop logic.

## Source Material

- `articles/controls-and-libraries/data-grid/drag-and-drop.md`
- `articles/controls-and-libraries/data-grid/drag-and-drop/drag-and-drop-options.md`
- `articles/controls-and-libraries/data-grid/drag-and-drop/process-drag-and-drop-operations.md`
- `articles/controls-and-libraries/data-grid/grid-view-data-layout/nodes/move-nodes.md`
- `articles/controls-and-libraries/tree-list.md` § Drag-and-Drop
- The data-grid skill's `references/drag-and-drop.md`

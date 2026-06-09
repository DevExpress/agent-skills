# DxTreeList — Drag-and-Drop Row Reordering

`DxTreeList` supports drag-and-drop for row reordering within the same TreeList, moving rows between TreeLists and Grids, and changing node hierarchy (re-parenting nodes).

## Key Properties and Events

| Property / Event | Type | Default | Description |
|---|---|---|---|
| `AllowDragRows` | `bool` | `false` | Renders drag handles on data rows; users can start drag operations |
| `AllowedDropTarget` | `TreeListAllowedDropTarget` | `Internal` | Controls where rows dragged **from this TreeList** can land (source-side setting) |
| `ItemsDropped` | `EventCallback<TreeListItemsDroppedEventArgs>` | — | Fires on the **receiving** component when rows are dropped; update the data source here |
| `DropTargetMode` | `TreeListDropTargetMode` | `BetweenRows` | `BetweenRows` — shows an insertion indicator between rows; `Component` — drops onto the entire data area |

## TreeListAllowedDropTarget Enum

`AllowedDropTarget` is a **source-side** property — it controls WHERE the dragged rows from this TreeList can be released, not what this TreeList accepts from others.

| Value | Description |
|---|---|
| `None` | Rows cannot be reordered or dropped onto other components |
| `Internal` (default) | Rows can be reordered within this TreeList only |
| `External` | Rows can be dropped onto other components (no internal reordering) |
| `All` | Rows can be reordered within this TreeList AND dropped onto other components |

## TreeListItemsDroppedEventArgs Members

| Member | Type | Description |
|---|---|---|
| `DroppedItems` | `IReadOnlyList<object>` | Rows that were dragged |
| `TargetItem` | `object` | The row near which the drop occurred; `null` if dropped at the end of the list |
| `TargetItemVisibleIndex` | `int` | Visible row index of `TargetItem` |
| `DropPosition` | `TreeListItemDropPosition` | `Before` or `After` relative to `TargetItem` |
| `TreeList` | `ITreeList` | The target TreeList that received the drop |
| `SourceComponent` | `object` | The component rows were dragged from |

## Data Source Requirement

The TreeList's `Data` must be an **`ObservableCollection<T>`** so insertions and removals are automatically reflected in the UI without calling `Reload()`. A plain `List<T>` requires `TreeList.Reload()` after every modification.

## Hierarchy Change

Unlike the Grid, the TreeList also supports **re-parenting** — changing a node's position in the tree hierarchy during a drag-and-drop. When the user drops a row onto another row (not between rows), the dropped row becomes a child of the target row. Update `ParentId` (or whichever field maps to `ParentKeyFieldName`) in the `ItemsDropped` handler.

## Pattern 1: Reorder Rows Within One TreeList

```razor
<DxTreeList Data="@Tasks"
            KeyFieldName="Id"
            ParentKeyFieldName="ParentId"
            AllowDragRows="true"
            AllowedDropTarget="TreeListAllowedDropTarget.Internal"
            ItemsDropped="OnItemsDropped">
    <Columns>
        <DxTreeListDataColumn FieldName="Name" Caption="Task" />
        <DxTreeListDataColumn FieldName="AssignedTo" />
    </Columns>
</DxTreeList>

@code {
    ObservableCollection<TaskItem> Tasks { get; set; }

    protected override void OnInitialized() {
        Tasks = new ObservableCollection<TaskItem>(DataService.GetTasks());
    }

    void OnItemsDropped(TreeListItemsDroppedEventArgs e) {
        var dropped = (TaskItem)e.DroppedItems[0];
        Tasks.Remove(dropped);
        var target = (TaskItem)e.TargetItem;
        var index = target != null
            ? Tasks.IndexOf(target) + (e.DropPosition == TreeListItemDropPosition.After ? 1 : 0)
            : Tasks.Count;
        Tasks.Insert(index, dropped);
    }
}
```

## Pattern 2: Move Rows Between Two TreeLists

```razor
<DxTreeList @ref="FirstTreeList"
            Data="@FirstItems"
            KeyFieldName="Id"
            ParentKeyFieldName="ParentId"
            AllowDragRows="true"
            AllowedDropTarget="TreeListAllowedDropTarget.External"
            ItemsDropped="OnItemsDropped">
    <Columns>
        <DxTreeListDataColumn FieldName="Name" Caption="Task" />
    </Columns>
</DxTreeList>

<DxTreeList @ref="SecondTreeList"
            Data="@SecondItems"
            KeyFieldName="Id"
            ParentKeyFieldName="ParentId"
            AllowDragRows="true"
            AllowedDropTarget="TreeListAllowedDropTarget.External"
            DropTargetMode="TreeListDropTargetMode.Component"
            ItemsDropped="OnItemsDropped">
    <Columns>
        <DxTreeListDataColumn FieldName="Name" Caption="Task" />
    </Columns>
</DxTreeList>

@code {
    ITreeList FirstTreeList { get; set; }
    ITreeList SecondTreeList { get; set; }
    ObservableCollection<TaskItem> FirstItems { get; set; }
    ObservableCollection<TaskItem> SecondItems { get; set; }

    protected override void OnInitialized() {
        FirstItems = new ObservableCollection<TaskItem>(DataService.GetFirstItems());
        SecondItems = new ObservableCollection<TaskItem>(DataService.GetSecondItems());
    }

    ObservableCollection<TaskItem> GetCollection(object treeList) =>
        treeList == FirstTreeList ? FirstItems : SecondItems;

    void OnItemsDropped(TreeListItemsDroppedEventArgs e) {
        var source = GetCollection(e.SourceComponent);
        var destination = GetCollection(e.TreeList);
        foreach (var item in e.DroppedItems)
            source.Remove((TaskItem)item);
        var target = (TaskItem)e.TargetItem;
        var index = target != null
            ? destination.IndexOf(target) + (e.DropPosition == TreeListItemDropPosition.After ? 1 : 0)
            : destination.Count;
        foreach (var item in e.DroppedItems.Reverse())
            destination.Insert(index, (TaskItem)item);
    }
}
```

## Common Mistakes

| Mistake | Fix |
|---|---|
| Using `List<T>` as the data source | Switch to `ObservableCollection<T>`; otherwise UI doesn't update after insert/remove |
| Setting `ItemsDropped` on the **source** TreeList | Only the **receiving** component needs `ItemsDropped` |
| Thinking `External` means "accept from external" | `External` on the source means "my rows can go to other components"; it is a source-side setting |
| Inserting multiple dropped rows without `.Reverse()` | Items end up in reverse order; always `.Reverse()` before looping inserts |
| Not updating `ParentId` when re-parenting | After a hierarchy change drop, update the `ParentId` (or the field mapped to `ParentKeyFieldName`) on the dropped item |

## External Resources

- [Live Demo: Drag and Drop — Reorder Rows](https://demos.devexpress.com/blazor/TreeList/DragDropRows/Reordering)
- [Live Demo: Drag and Drop — Between Components](https://demos.devexpress.com/blazor/TreeList/DragDropRows/Between)

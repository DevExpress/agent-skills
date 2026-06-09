# DxGrid — Drag-and-Drop Row Reordering

`DxGrid` supports drag-and-drop for row reordering within the same grid and moving rows between grids.

## Key Properties

| Property / Event | Type | Default | Description |
|---|---|---|---|
| `AllowDragRows` | `bool` | `false` | Renders drag handles on data rows; users can start drag operations |
| `AllowedDropTarget` | `GridAllowedDropTarget` | `Internal` | Controls where rows dragged **from this grid** can land (source-side setting) |
| `ItemsDropped` | `EventCallback<GridItemsDroppedEventArgs>` | — | Fires on the **receiving** grid when rows are dropped; update the data source here |
| `DropTargetMode` | `GridDropTargetMode` | `BetweenRows` | `BetweenRows` — drop between rows (shows insertion indicator); `Component` — drop onto the grid as a whole |
| `DragHintTextTemplate` | `RenderFragment<GridDragHintTextTemplateContext>` | — | Customizes the drag hint tooltip displayed while dragging |

## GridAllowedDropTarget Enum

`AllowedDropTarget` is a **source-side** property — it controls WHERE the dragged rows from this grid can be released, not what this grid accepts from others.

| Value | Description |
|---|---|
| `None` | Rows cannot be reordered or dropped onto other components |
| `Internal` (default) | Rows can be reordered within this grid only |
| `External` | Rows can be dropped onto other components (no internal reordering) |
| `All` | Rows can be reordered within this grid AND dropped onto other components |

## GridItemsDroppedEventArgs Members

| Member | Type | Description |
|---|---|---|
| `DroppedItems` | `IReadOnlyList<object>` | Rows that were dragged; items appear in display order (or selection order when virtual scrolling is active) |
| `TargetItem` | `object` | The row near which the drop occurred; `null` if dropped at the end of the list |
| `TargetItemVisibleIndex` | `int` | Visible row index of `TargetItem` |
| `DropPosition` | `GridItemDropPosition` | `Before` or `After` relative to `TargetItem` |
| `Grid` | `IGrid` | The target grid that received the drop |
| `SourceComponent` | `object` | The component rows were dragged from; cast to `IGrid` in grid-to-grid scenarios |
| `GetTargetDataSourceIndexAsync()` | `Task<int>` | Returns the zero-based index in the data source where dropped items should be inserted (convenience alternative to manual index calculation) |

## Data Source Requirement

The grid's `Data` must be an **`ObservableCollection<T>`** so insertions and removals are automatically reflected in the UI without calling `Reload()`. A plain `List<T>` requires `Grid.Reload()` after every modification.

## Pattern 1: Reorder Rows Within One Grid

```razor
<DxGrid Data="@Items"
        KeyFieldName="Id"
        AllowDragRows="true"
        AllowedDropTarget="GridAllowedDropTarget.Internal"
        ItemsDropped="OnItemsDropped">
    <Columns>
        <DxGridDataColumn FieldName="Name" />
        <DxGridDataColumn FieldName="Priority" />
    </Columns>
</DxGrid>

@code {
    ObservableCollection<MyItem> Items { get; set; }

    protected override void OnInitialized() {
        Items = new ObservableCollection<MyItem>(DataService.GetItems());
    }

    void OnItemsDropped(GridItemsDroppedEventArgs e) {
        var dropped = (MyItem)e.DroppedItems[0];
        Items.Remove(dropped);
        var target = (MyItem)e.TargetItem;
        var index = target != null
            ? Items.IndexOf(target) + (e.DropPosition == GridItemDropPosition.After ? 1 : 0)
            : Items.Count;
        Items.Insert(index, dropped);
    }
}
```

> `AllowedDropTarget="Internal"` is the default, so you can omit it when reordering within a single grid.

## Pattern 2: Move Rows Between Two Grids

The source grid uses `AllowedDropTarget="External"` so its rows can be released on other components (but not reordered within itself). The target uses `AllowedDropTarget="All"` and handles `ItemsDropped`.

Only the **target** grid needs `ItemsDropped`. Use `e.SourceComponent` and `e.Grid` (both typed as `object`) together with a helper to identify which `ObservableCollection<T>` to update. When inserting multiple rows, call `.Reverse()` on `e.DroppedItems` before inserting to preserve their original display order.

```razor
<DxGrid @ref="SourceGrid"
        Data="@SourceItems"
        KeyFieldName="Id"
        AllowDragRows="true"
        AllowedDropTarget="GridAllowedDropTarget.External">
    <Columns>
        <DxGridDataColumn FieldName="Name" />
    </Columns>
</DxGrid>

<DxGrid Data="@TargetItems"
        KeyFieldName="Id"
        AllowDragRows="true"
        AllowedDropTarget="GridAllowedDropTarget.All"
        ItemsDropped="OnTargetItemsDropped">
    <Columns>
        <DxGridDataColumn FieldName="Name" />
    </Columns>
</DxGrid>

@code {
    IGrid SourceGrid { get; set; }
    ObservableCollection<MyItem> SourceItems { get; set; }
    ObservableCollection<MyItem> TargetItems { get; set; }

    protected override void OnInitialized() {
        SourceItems = new ObservableCollection<MyItem>(DataService.GetSourceItems());
        TargetItems = new ObservableCollection<MyItem>(DataService.GetTargetItems());
    }

    ObservableCollection<MyItem> GetCollection(object grid) =>
        grid == SourceGrid ? SourceItems : TargetItems;

    void OnTargetItemsDropped(GridItemsDroppedEventArgs e) {
        var source = GetCollection(e.SourceComponent);
        var destination = GetCollection(e.Grid);
        foreach (var item in e.DroppedItems)
            source.Remove((MyItem)item);
        var target = (MyItem)e.TargetItem;
        var index = target != null
            ? destination.IndexOf(target) + (e.DropPosition == GridItemDropPosition.After ? 1 : 0)
            : destination.Count;
        foreach (var item in e.DroppedItems.Reverse())
            destination.Insert(index, (MyItem)item);
    }
}
```

## Pattern 3: Using GetTargetDataSourceIndexAsync (Simplified Index Calculation)

`GetTargetDataSourceIndexAsync()` computes the insertion index for you, avoiding manual index arithmetic. Use it when you don't need to inspect `TargetItem` or `DropPosition` directly.

```razor
@code {
    async Task Grid_ItemsDropped(GridItemsDroppedEventArgs e) {
        var dropped = (MyItem)e.DroppedItems[0];
        Items.Remove(dropped);
        var index = await e.GetTargetDataSourceIndexAsync();
        Items.Insert(index, dropped);
    }
}
```

## Common Mistakes

| Mistake | Fix |
|---|---|
| Using `List<T>` as the data source | Switch to `ObservableCollection<T>`; otherwise UI doesn't update after insert/remove |
| Setting `ItemsDropped` on the **source** grid | Only the **receiving** grid needs `ItemsDropped` |
| Thinking `External` means "accept from external" | `External` on the source means "my rows can go to other components"; it is a source-side setting |
| Inserting multiple dropped rows without `.Reverse()` | Items end up in reverse order; always `.Reverse()` before looping inserts |
| Using `AllowRowDragDrop`, `RowDrop`, or `OnRowDrop` | These do not exist on `DxGrid`; use `AllowDragRows` + `ItemsDropped` |

## External Resources

- [Live Demo: Drag and Drop — Reorder Rows](https://demos.devexpress.com/blazor/Grid/DragDropRows/Reordering)
- [Live Demo: Drag and Drop — Between Components](https://demos.devexpress.com/blazor/Grid/DragDropRows/Between)
- [GitHub Example: Row Drag and Drop](https://github.com/DevExpress-Examples/blazor-grid-drag-and-drop)

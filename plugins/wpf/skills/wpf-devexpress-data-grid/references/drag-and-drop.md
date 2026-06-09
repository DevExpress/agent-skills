# Drag-and-Drop — DevExpress WPF Data Grid

The Data Grid supports native drag-and-drop of records: within a single grid, between two grids, between the grid and external WPF controls (`ListBoxEdit`, standard `ListBox`, `TreeView`, etc.), and between applications. Six events let you control every stage — start, hover, feedback, continue, drop, completion — and templates customize the drag hint and drop marker.

## When to Use This Reference

Use this when you need to:

- Enable drag-and-drop within or between grids (`AllowDragDrop`)
- Allow or block sorted / grouped data dragging (`AllowSortedDataDragDrop`)
- Block specific records from being dragged (`StartRecordDrag` event)
- Block specific drop targets (`DragRecordOver` event)
- Customize the data being transferred (`DropRecord` event)
- Prevent records from being removed from the source after drop (`CompleteRecordDragDrop`)
- Provide visual feedback during drag (`GiveRecordDragFeedback`)
- Auto-expand collapsed groups when dragging over them (`AutoExpandOnDrag`)
- Auto-scroll when dragging near edges (`AllowScrollingOnDrag`)
- Customize drag hint / drop marker visuals (`DragDropHintTemplate`, `DropMarkerTemplate`)
- Drag between `GridControl` and `ListBoxEdit` (use `ListBoxDragDropBehavior`)
- Drag between applications (requires special setup on .NET 9+)

## Enable Drag-and-Drop

Master switch on the View:

```xaml
<dxg:GridControl>
    <dxg:GridControl.View>
        <dxg:TableView AllowDragDrop="True"/>
    </dxg:GridControl.View>
</dxg:GridControl>
```

Works on all three views (`TableView`, `TreeListView`, `CardView`) and on standalone `TreeListControl`.

### Allow Sorted / Grouped Data Drag

By default, the grid disables drag-drop when data is sorted or grouped (because the visible order is computed, not stored). To allow anyway:

```xaml
<dxg:TableView AllowDragDrop="True"
               AllowSortedDataDragDrop="True"/>
```

### `EditorShowMode` Gotcha

With drag-and-drop enabled, opening a cell editor requires **2 mouse clicks** (the first one initiates a potential drag). To restore single-click editing:

```xaml
<dxg:TableView AllowDragDrop="True"
               EditorShowMode="MouseDownFocused"/>
```

`EditorShowMode` values: `Default`, `MouseDown`, `MouseDownFocused`, `MouseUp`.

## Data Source Requirements

> The grid's `ItemsSource` collection should implement `IList` (e.g., `ObservableCollection<T>`, `List<T>`) for built-in drag-and-drop data modification to work correctly.

`IEnumerable<T>` alone is insufficient — the grid needs `Add`, `Insert`, `RemoveAt` to move records.

## The Six Drag-Drop Events

The drag-and-drop lifecycle has six events on `DataViewBase`. Listen to whichever stages you need to customize:

| Stage | Event | Purpose |
|---|---|---|
| Drag begins | `StartRecordDrag` | Block specific records from being dragged; modify the data payload |
| Drag in progress | `DragRecordOver` | Reject specific drop targets; control drop position |
| Drag in progress | `GiveRecordDragFeedback` | Customize cursor / visual feedback while dragging |
| Drag in progress | `ContinueRecordDrag` | Cancel on keyboard / mouse state change |
| Drop occurs | `DropRecord` | Process / modify dropped data |
| Drop completed | `CompleteRecordDragDrop` | Cancel removal from source; post-drop actions |

> The Data Grid does NOT raise the standard WPF events (`DragDrop.DragOver`, `DragDrop.Drop`, etc.). Use these grid-specific events instead.

Source: `articles/controls-and-libraries/data-grid/drag-and-drop/drag-and-drop-options.md`.

## Pattern 1: Block Specific Records From Being Dragged

```xaml
<dxg:TableView AllowDragDrop="True"
               StartRecordDrag="OnStartRecordDrag"/>
```

```csharp
using DevExpress.Xpf.Core;   // StartRecordDragEventArgs

void OnStartRecordDrag(object sender, StartRecordDragEventArgs e) {
    // e.Records is the IList of records about to be dragged
    if (e.Records.Cast<Employee>().Any(emp => emp.IsLocked)) {
        e.AllowDrag = false;
        e.Handled = true;
    }
}
```

In a tree, allow only leaf nodes:

```csharp
void OnStartRecordDrag(object sender, StartRecordDragEventArgs e) {
    if (e.Records.Any(r => treeListView.GetNodeByContent(r).HasChildren)) {
        e.AllowDrag = false;
        e.Handled = true;
    }
}
```

Source: `articles/controls-and-libraries/data-grid/drag-and-drop/process-drag-and-drop-operations.md` § "Prevent Dragging Specific Records".

## Pattern 2: Block Specific Drop Targets

```xaml
<dxg:TableView AllowDragDrop="True"
               DragRecordOver="OnDragRecordOver"/>
```

```csharp
void OnDragRecordOver(object sender, DragRecordOverEventArgs e) {
    var target = (Employee)e.TargetRecord;
    if (target.Position is "President" or "Vice President") {
        e.Effects = DragDropEffects.None;   // Reject drop
        e.Handled = true;
    }
}
```

`DragRecordOver` fires **continuously** while hovering. Keep the handler fast.

`DragDropEffects` values: `None`, `Copy`, `Move`, `Link`, `Scroll`, `All`. Default is `Move` for in-grid drag, `Copy` for cross-application.

### Drop Position

```csharp
void OnDragRecordOver(object sender, DragRecordOverEventArgs e) {
    // e.DropPosition is automatically determined from hover area
    // (top half = Before, middle = Inside, bottom half = After)
    // You can override:
    e.DropPosition = DropPosition.After;
}
```

`DropPosition` enum: `None`, `Before`, `Inside`, `After`.
- `Before` / `After` — sibling drop (move record next to target)
- `Inside` — drop into target (for tree-list, makes record a child)

## Pattern 3: Modify Dropped Data

```xaml
<dxg:TreeListView AllowDragDrop="True"
                  DropRecord="OnDropRecord"/>
```

```csharp
void OnDropRecord(object sender, DropRecordEventArgs e) {
    // Extract the dragged records
    var payload = (RecordDragDropData)e.Data.GetData(typeof(RecordDragDropData));
    var draggedEmployees = payload.Records.Cast<Employee>().ToList();

    var target = (Employee)e.TargetRecord;

    // Update department/position based on drop target
    foreach (var emp in draggedEmployees) {
        emp.Department = target.Department;
        emp.Position = target.Position;
    }

    // Clear position when dropped INSIDE a manager (becomes their report)
    if (e.DropPosition == DropPosition.Inside) {
        foreach (var emp in draggedEmployees)
            emp.Position = "";
    }
}
```

### `DropRecordEventArgs` Members

| Member | Description |
|---|---|
| `Data` | `IDataObject` — call `GetData(typeof(RecordDragDropData))` to extract |
| `TargetRecord` | The record being dropped on |
| `DropPosition` | `Before` / `Inside` / `After` |
| `Effects` | The effect (Move / Copy / etc.) |
| `Handled` | Set to `true` if you've taken full responsibility for processing |

`RecordDragDropData.Records` is an `IList` of the dragged data items.

Source: `articles/controls-and-libraries/data-grid/drag-and-drop/process-drag-and-drop-operations.md` § "Customize Dragged Items".

## Pattern 4: Prevent Removal From Source (Copy Instead of Move)

By default, dropping a record into another grid **removes** it from the source. To copy instead:

```xaml
<dxg:TableView AllowDragDrop="True"
               CompleteRecordDragDrop="OnCompleteRecordDragDrop"/>
```

```csharp
void OnCompleteRecordDragDrop(object sender, CompleteRecordDragDropEventArgs e) {
    e.Handled = true;   // Skip the default "remove from source" behavior
}
```

Now the dropped record appears in the target while remaining in the source.

To force copy behavior conditionally (e.g., when user holds Ctrl), check `e.Effects` against `DragDropEffects.Copy`.

Source: `articles/controls-and-libraries/data-grid/drag-and-drop/process-drag-and-drop-operations.md` § "Prevent Removing Items After Dropping".

## Visual Customization

### Drag Hint (Floating Preview)

| Property | Effect |
|---|---|
| `DataViewBase.ShowDragDropHint` | Show / hide the hint (default `true`) |
| `DataViewBase.ShowTargetInfoInDragDropHint` | Show drop-target info in the hint |
| `DataViewBase.DragDropHintTemplate` | Custom `DataTemplate` for the hint |

```xaml
<dxg:TableView.DragDropHintTemplate>
    <DataTemplate>
        <Border Background="LightYellow" Padding="6" BorderBrush="DarkGoldenrod" BorderThickness="1">
            <StackPanel>
                <TextBlock Text="{Binding RecordCount, StringFormat='Moving {0} item(s)'}"/>
                <TextBlock Text="{Binding TargetDescription}" FontStyle="Italic"/>
            </StackPanel>
        </Border>
    </DataTemplate>
</dxg:TableView.DragDropHintTemplate>
```

> Verify the template's `DataContext` shape (`RecordCount`, `TargetDescription`, etc.) via DxDocs MCP — names depend on the internal `DragDropHintData` type:
> `devexpress_docs_search(technology="WPF Data Grid", query="DragDropHintTemplate data context")`

Source: `articles/controls-and-libraries/data-grid/drag-and-drop/drag-and-drop-hint.md`.

### Drop Marker (Insertion Indicator)

The drop marker is the line / triangle that shows where the record will land.

```xaml
<dxg:TableView.DropMarkerTemplate>
    <DataTemplate>
        <Path Stroke="DarkGreen" StrokeThickness="3" Data="M 0,0 L 20,0"/>
    </DataTemplate>
</dxg:TableView.DropMarkerTemplate>
```

Source: `articles/controls-and-libraries/data-grid/drag-and-drop/drop-marker.md`.

## Auto-Expand and Auto-Scroll

### Auto-Expand on Drag (For Trees / Groups)

```xaml
<dxg:TreeListView AllowDragDrop="True"
                  AutoExpandOnDrag="True"
                  AutoExpandDelayOnDrag="500"/>
```

When the user hovers over a collapsed group / node for `AutoExpandDelayOnDrag` milliseconds, the grid auto-expands it so the user can drop inside.

### Auto-Scroll Near Edges

```xaml
<dxg:TableView AllowDragDrop="True"
               AllowScrollingOnDrag="True"/>
```

When the user drags near the top or bottom edge, the grid auto-scrolls in that direction.

## Drag Between Two Grids

Same XAML setup on both grids:

```xaml
<dxg:GridControl x:Name="grid1" ItemsSource="{Binding ItemsLeft}">
    <dxg:GridControl.View>
        <dxg:TableView AllowDragDrop="True"/>
    </dxg:GridControl.View>
</dxg:GridControl>

<dxg:GridControl x:Name="grid2" ItemsSource="{Binding ItemsRight}">
    <dxg:GridControl.View>
        <dxg:TableView AllowDragDrop="True"/>
    </dxg:GridControl.View>
</dxg:GridControl>
```

That's it. The grids automatically support cross-grid drag-and-drop as long as:
- Both `ItemsSource` collections implement `IList`
- Both bound collections accept the same data type (or one accepts a base type)
- `AllowDragDrop="True"` on both views

Source: `articles/controls-and-libraries/data-grid/drag-and-drop/process-drag-and-drop/drag-and-drop-between-gridcontrols.md`.

## Drag Between GridControl and ListBoxEdit

`ListBoxEdit` uses a different drag-drop mechanism — attached behaviors instead of events. To enable cross-control drag-drop:

```xaml
<dxg:GridControl ItemsSource="{Binding Source}">
    <dxg:GridControl.View>
        <dxg:TableView AllowDragDrop="True"/>
    </dxg:GridControl.View>
</dxg:GridControl>

<dxe:ListBoxEdit ItemsSource="{Binding Target}"
                 dxe:ListBoxDragDropBehavior.AllowDragDrop="True"/>
```

`DevExpress.Xpf.Core.ListBoxDragDropBehavior` exposes attached properties similar to the View's drag-drop options.

Source: `articles/controls-and-libraries/data-grid/drag-and-drop/process-drag-and-drop/drag-and-drop-between-gridcontrol-and-listboxedit.md`.

## Drag Between GridControl and Standard WPF Controls

For standard `ListBox`, `TreeView`, etc., implement drag-drop manually:

1. Handle `GridControl.StartRecordDrag` to package data:
   ```csharp
   void OnStartRecordDrag(object sender, StartRecordDragEventArgs e) {
       // Convert grid records to a format the target understands
       var stringList = e.Records.Cast<Order>().Select(o => o.CustomerName).ToList();
       e.Data.SetData(typeof(List<string>), stringList);
   }
   ```

2. Wire standard WPF `DragDrop.AllowDrop`, `DragEnter`, `Drop` events on the target control.

> Verify exact `StartRecordDragEventArgs.Data` API via DxDocs MCP if the property name differs in your version:
> `devexpress_docs_search(technology="WPF Data Grid", query="StartRecordDrag custom data payload")`

Source: `articles/controls-and-libraries/data-grid/drag-and-drop/process-drag-and-drop/drag-and-drop-between-gridcontrol-and-other-controls.md`.

## Drag Between Applications

Drag-and-drop works across application instances by default. The grid serializes the dragged data via the system clipboard.

### .NET 9+ Limitation

If your project targets **.NET 9**, you can drag and drop records **only within the same application** by default. Cross-application drag-drop requires two extra steps:

1. Install the [`System.Runtime.Serialization.Formatters`](https://learn.microsoft.com/en-us/dotnet/standard/serialization/binaryformatter-migration-guide/compatibility-package) (BinaryFormatter compatibility package).
2. Set `DevExpress.Utils.DeserializationSettings.EnableDataObjectBinarySerialization = true`.

This is due to Microsoft deprecating `BinaryFormatter` in .NET 9. See the linked migration guide.

Source: `articles/controls-and-libraries/data-grid/drag-and-drop/process-drag-and-drop/drag-and-drop-between-applications.md` and the note at the top of `drag-and-drop.md` root article.

## Drag Within Master-Detail

Drag-and-drop works inside the master grid but does NOT cross master-detail boundaries by default. To move a record from a detail grid to the master grid (or vice versa), implement `DropRecord` manually on each level — the grid does not automatically reparent items.

> See [master-detail.md](master-detail.md) § "Detail Event Handlers — Use `e.Source`" for accessing detail events.

## Common Issues

- **Drag doesn't start** — check `AllowDragDrop="True"` on the View (not the Grid). Also: data is sorted/grouped without `AllowSortedDataDragDrop="True"`.
- **Drop has no effect** — `ItemsSource` doesn't implement `IList`. Switch from `IEnumerable<T>` or `IReadOnlyList<T>` to `ObservableCollection<T>`.
- **Cell editor takes 2 clicks** — by design; set `EditorShowMode="MouseDownFocused"`.
- **Cross-app drag stopped working after .NET 9 upgrade** — see § ".NET 9+ Limitation" above.
- **`StartRecordDrag` doesn't fire on TreeList** — handler attached to the master grid; subscribe on the `TreeListView` directly, not the master.
- **Drop marker shows wrong position** — `DropPosition` is computed from hover area. Override in `DragRecordOver` if business logic differs.

## Apply to TreeListControl

All `DataViewBase`-level properties and events apply identically. Use `TreeListView` instead of `TableView`:

```xaml
<dxg:TreeListControl>
    <dxg:TreeListControl.View>
        <dxg:TreeListView AllowDragDrop="True"
                          AutoExpandOnDrag="True"
                          StartRecordDrag="OnStartRecordDrag"
                          DropRecord="OnDropRecord"/>
    </dxg:TreeListControl.View>
</dxg:TreeListControl>
```

The tree-specific feature: dragging **inside** a node makes the dragged record a child (rather than a sibling). `DropPosition.Inside` is the tree-specific drop position.

## Source Material

- `articles/controls-and-libraries/data-grid/drag-and-drop.md` (root)
- `articles/controls-and-libraries/data-grid/drag-and-drop/drag-and-drop-options.md`
- `articles/controls-and-libraries/data-grid/drag-and-drop/process-drag-and-drop-operations.md`
- `articles/controls-and-libraries/data-grid/drag-and-drop/drag-and-drop-hint.md`
- `articles/controls-and-libraries/data-grid/drag-and-drop/drop-marker.md`
- `articles/controls-and-libraries/data-grid/drag-and-drop/process-drag-and-drop/drag-and-drop-between-gridcontrols.md`
- `articles/controls-and-libraries/data-grid/drag-and-drop/process-drag-and-drop/drag-and-drop-between-gridcontrol-and-listboxedit.md`
- `articles/controls-and-libraries/data-grid/drag-and-drop/process-drag-and-drop/drag-and-drop-between-gridcontrol-and-other-controls.md`
- `articles/controls-and-libraries/data-grid/drag-and-drop/process-drag-and-drop/drag-and-drop-between-applications.md`

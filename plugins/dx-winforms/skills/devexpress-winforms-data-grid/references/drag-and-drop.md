# Drag and Drop

This reference covers drag-and-drop in `GridControl` and `TreeList`. DevExpress offers two engines:

1. **Drag-and-Drop Behavior** (recommended) — a `BehaviorManager`-based component that automatically initiates drags, shows previews, and handles drops between supported DevExpress controls.
2. **Standard .NET drag-and-drop** (`DoDragDrop`, `DragOver`, `DragDrop`) — for cross-vendor drag operations.

> The two engines are mutually exclusive. Setting `control.AllowDrop = true` disables the Behavior on that control.

## When to Use This Reference

- Reordering rows inside a `GridView` or `TileView`.
- Moving rows between two grids, or between a grid and a TreeList.
- Reordering or moving nodes inside a `TreeList`.
- Accepting drops from outside DevExpress controls (files, external apps).
- Customizing drop logic (insert position, multi-select handling, data transformation).

## Engine 1 — Drag-and-Drop Behavior

Supported targets:

- `TreeList` (set `TreeListOptionsDragAndDrop.DragNodesMode = None` to let the Behavior own dragging).
- `GridControl` (works with `GridView`, `BandedGridView`, `AdvBandedGridView`, `TileView`).
- `ListBoxControl`.

Drag works **only between these supported DevExpress controls**. For other targets use the standard engine.

### Attach in Code

```csharp
var manager = new BehaviorManager();
manager.Attach<DragDropBehavior>(gridControl1, b => {
    b.Properties.AllowDrop  = true;
    b.Properties.InsertIndicatorVisible = true;
});
manager.Attach<DragDropBehavior>(treeList1, _ => { });
```

Subscribe to the manager-level events via the `DragDropEvents` component, or per-behavior via `BehaviorProperties.DragDropEventsName`.

### Key options

```csharp
beh.Properties.AllowDrop          = true;   // accept drops
beh.Properties.AllowDrag          = true;   // source of drags
beh.Properties.InsertIndicatorVisible = true;
beh.Properties.PreviewVisible     = true;
beh.Properties.DragDropEventsName = "dragDropEvents1";
```

### Reorder rows inside a `GridView`

Out-of-the-box for `IList`, `DataTable`, `DataView`-bound grids. Just attach the Behavior to the view's grid; users can hold the left mouse button and drag a selected row to a new position.

```csharp
gridView1.OptionsDragDrop.DragSourceMode = GridDragSourceMode.Auto;     // or RowsAndSelectedCells
gridView1.OptionsDragDrop.DragDropFlags  = GridDragDropFlags.Default;
```

> Sorted or filtered `DataView` does not support reorder — Drag-and-Drop falls back to drag-as-copy.

### Drag rows between two grids

Attach the Behavior to both grids. If both grids share columns and use editable lists, rows move automatically. For different schemas, handle the `DragDrop` event:

```csharp
dragDropEvents1.DragDrop += (s, e) => {
    var args   = DragDropGridEventArgs.GetDragDropGridEventArgs(e);
    var source = e.Source as ColumnView;
    var target = e.Target as ColumnView;
    if (source is null || target is null) return;

    foreach (int handle in args.SelectedRowHandles) {
        var item = source.GetRow(handle);
        target.AddNewRow();
        target.SetRowCellValue(GridControl.NewItemRowHandle,
            target.Columns["FullName"], MapName(item));
    }
    e.Handled = true;
};
```

`DragDropGridEventArgs.GetDragDropGridEventArgs(e)` decorates the args with grid-specific properties — `SelectedRowHandles`, `HitInfo`, `InsertIndex`, etc.

### Move/insert nodes in a `TreeList`

```csharp
treeList1.OptionsDragAndDrop.DragNodesMode = DragNodesMode.Multiple;
treeList1.OptionsDragAndDrop.AcceptOuterNodes = true;     // accept drops from other TreeLists/Grids
```

Behavior fires `DragDrop` on the target TreeList. To customize how outer data becomes a node:

```csharp
treeList1.CustomizeNewNodeFromOuterData += (s, e) => {
    e.NewData["TaskName"]   = e.SourceNode["Name"];
    e.NewData["CreateDate"] = DateTime.Now;
};
```

### Drag from grid → TreeList (event-based)

```csharp
treeList1.AllowDrop = false;   // Behavior owns the drop
dragDropEvents1.DragDrop += (s, e) => {
    if (e.Target != treeList1) return;
    var argsTree = DragDropTreeListEventArgs.GetDragDropTreeListEventArgs(e);
    var rowHandles = (int[])e.Data;          // grid sends selected row handles
    var sourceView = (GridView)e.Source;

    foreach (int handle in rowHandles) {
        var row = sourceView.GetRow(handle);
        treeList1.AppendNode(new object[] { row.ToString() },
            argsTree.TargetNode, argsTree.InsertType);
    }
    e.Handled = true;
};
```

### Control drop position

The `DragOver` event reports `args.InsertIndicatorLocation`, `args.InsertType` (`Before`, `After`, `AsChild`, `None`). Override or cancel:

```csharp
dragDropEvents1.DragOver += (s, e) => {
    if (e.Source == e.Target && !sameSchema) e.Action = DragDropActions.None;
};
```

## Engine 2 — Built-in TreeList Drag-and-Drop (legacy)

Setting `TreeListOptionsDragAndDrop.DragNodesMode = Single` or `Multiple` enables the legacy engine specifically for in-control node moves:

```csharp
treeList1.OptionsDragAndDrop.DragNodesMode = DragNodesMode.Multiple;
treeList1.OptionsSelection.MultiSelect     = true;
treeList1.OptionsDragAndDrop.AcceptOuterDragging = true;
```

This engine is sufficient for "rearrange the tree" and predates the Behavior. Pick the Behavior for new code.

## Engine 3 — Standard .NET Drag-and-Drop

Use when the source or target is a non-DevExpress control (Explorer, browser, third-party grid). You implement the four standard events yourself.

```csharp
GridHitInfo downHitInfo = null;

gridView1.MouseDown += (s, e) => {
    var view = (GridView)s;
    downHitInfo = null;
    if (Control.ModifierKeys != Keys.None) return;
    if (e.Button != MouseButtons.Left) return;
    var hit = view.CalcHitInfo(e.Location);
    if (hit.InRow) downHitInfo = hit;
};

gridView1.MouseMove += (s, e) => {
    if (downHitInfo == null) return;
    var dragRect = new Rectangle(
        downHitInfo.HitPoint - SystemInformation.DragSize / 2,
        SystemInformation.DragSize);
    if (!dragRect.Contains(new Point(e.X, e.Y))) {
        var data = gridView1.GetRow(downHitInfo.RowHandle);
        gridControl1.DoDragDrop(data, DragDropEffects.Copy);
        downHitInfo = null;
    }
};

treeList1.AllowDrop = true;
treeList1.DragEnter += (s, e) => e.Effect = DragDropEffects.Copy;
treeList1.DragDrop  += (s, e) => {
    var data = e.Data.GetData(typeof(Order)) as Order;
    if (data is null) return;
    treeList1.AppendNode(new object[] { data.Customer }, null);
};
```

Setting `treeList1.AllowDrop = true` disables the Drag-and-Drop Behavior on the TreeList — pick one engine.

## Multi-Select Drag

To drag multiple selected rows, enable multi-select first:

```csharp
gridView1.OptionsBehavior.EditorShowMode  = EditorShowMode.MouseUp;
gridView1.OptionsSelection.MultiSelect    = true;
gridView1.OptionsSelection.MultiSelectMode = GridMultiSelectMode.RowSelect;
```

Long-hold drag is required for multi-select; Behavior auto-detects.

## Drag in Master-Detail

Attach Behavior to both the master view and the detail view. Use `ViewRegistered` to attach Behavior on the fly to clone detail views:

```csharp
gridControl1.ViewRegistered += (s, e) => {
    if (e.View.IsDetailView)
        behaviorManager1.Attach<DragDropBehavior>(e.View);
};
```

## Common Issues

- **Drag does nothing**: `Control.AllowDrop` is `true` — Behavior is disabled. Remove `AllowDrop = true` or use the standard engine.
- **`MouseDown` opens the editor before drag starts**: set `view.OptionsBehavior.EditorShowMode = EditorShowMode.Click` or `MouseUp`. Default `Default` opens on `MouseDown` and intercepts drags.
- **Reorder reshuffles a `DataTable` row but column values are blank**: Behavior calls `dataTable.Rows.Remove` + `InsertAt`. Disconnect any sorted/filtered `DataView` first or implement your own `DragDrop`.
- **Cross-control drag does nothing**: ensure Behavior is attached to *both* controls. Drag-and-Drop Behavior never crosses to non-supported controls.
- **TreeList does not accept outer nodes**: set `OptionsDragAndDrop.AcceptOuterNodes = true` and handle `CustomizeNewNodeFromOuterData`.

## Source Material

- `articles/common-features/behaviors/drag-and-drop-behavior.md` (`xref:WindowsForms.118656`).
- `articles/controls-and-libraries/data-grid/drag-and-drop.md` (`xref:WindowsForms.401989`).
- `articles/controls-and-libraries/tree-list/feature-center/drag-and-drop.md` (`xref:WindowsForms.401949`).
- `articles/controls-and-libraries/tree-list/feature-center/drag-and-drop/how-to-drag-xtragrid-rows-to-the-xtratreelist.md` (`xref:WindowsForms.3021`).
- `api/DevExpress.Utils.DragDrop.DragDropEvents.yml`.
- `api/DevExpress.XtraGrid.Views.Grid.DragDropGridEventArgs.yml`.
- `api/DevExpress.XtraTreeList.TreeListOptionsDragAndDrop.yml`.
- `api/DevExpress.XtraTreeList.TreeList.CustomizeNewNodeFromOuterData.yml`.

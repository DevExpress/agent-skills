# Focus and Selection

This reference covers row/cell focus and selection in `GridView`, `BandedGridView`, `AdvBandedGridView`, `CardView`, `LayoutView`, `WinExplorerView`, `TileView`, and `TreeList` — focused-row tracking, multi-select modes, Selection Binding to a data-source field, code APIs, and events.

## When to Use This Reference

- Reacting when the user changes the focused row (e.g., update a detail panel).
- Enabling multi-select with rows, cells, or web-style checkbox columns.
- Binding selection state to a `bool` field in the data source.
- Selecting many rows in code without flickering.
- Pinpointing why `GetFocusedRow()` returns `null`.

## Focus

### Track focused row

```csharp
gridView1.FocusedRowChanged += (s, e) => {
    var order = gridView1.GetFocusedRow() as Order;
    detailsControl.LoadOrder(order);
};
```

Other navigation events:

- `FocusedColumnChanged` — column changed (does not fire when navigating within the same column).
- `RowClick` / `DoubleClick` — for mouse-driven activation.
- `RowCellClick` — cell click with column information.
- `HiddenEditor` / `ShownEditor` — editor open/close.

### Programmatic focus

```csharp
gridView1.FocusedRowHandle = gridView1.GetRowHandle(0);   // first row in data source
gridView1.FocusedColumn    = gridView1.Columns["Total"];
gridView1.MakeRowVisible(gridView1.FocusedRowHandle, true);
gridView1.SelectRow(gridView1.FocusedRowHandle);
```

`GetRowHandle(dataSourceIndex)` converts a data-source index into the corresponding row handle. The reverse — `GetDataSourceRowIndex(rowHandle)` — converts the other way. Visible row indices (`GetVisibleRowHandle`) differ when grouping/filtering is active.

### Sentinel handles

| Constant | Meaning |
|---|---|
| `GridControl.InvalidRowHandle` (`int.MinValue`) | No row focused / not found. |
| `GridControl.NewItemRowHandle` (`-2147483647`) | The "new-item" row. |
| `GridControl.AutoFilterRowHandle` | The Auto Filter Row. |
| Negative handles in the range `[ -2147483646 … -1 ]` | Group rows. |

Use `view.IsValidRowHandle(handle)` to check before accessing.

## Single-Row Selection (default)

`MultiSelect = false`: the focused row *is* the selected row. `GetFocusedRow()` and `GetFocusedRowCellValue("name")` are the primary APIs.

## Multi-Row Selection

Enable on the view:

```csharp
gridView1.OptionsSelection.MultiSelect      = true;
gridView1.OptionsSelection.MultiSelectMode  = GridMultiSelectMode.RowSelect;
```

`MultiSelect` is supported by `GridView`, `BandedGridView`, `AdvBandedGridView`, `CardView`, `LayoutView`, and `WinExplorerView`.

### Modes

| `MultiSelectMode` | UX |
|---|---|
| `RowSelect` | Marquee, `Ctrl+Click`, `Shift+Click` select rows. The default. |
| `CellSelect` | Select individual cells and blocks of cells (not supported in `AdvBandedGridView`). |
| `CheckBoxRowSelect` | A built-in check column appears as column #0; clicking checks/unchecks the row. Excellent for web-style flows. |

```csharp
gridView1.OptionsSelection.MultiSelectMode = GridMultiSelectMode.CheckBoxRowSelect;
gridView1.OptionsSelection.ShowCheckBoxSelectorInColumnHeader = true;
gridView1.OptionsSelection.CheckBoxSelectorColumnWidth = 32;
gridView1.OptionsSelection.ResetSelectionClickOutsideCheckboxSelector = true;
```

## Read Selected Rows

```csharp
int[] handles = gridView1.GetSelectedRows();
foreach (int h in handles) {
    var row = gridView1.GetRow(h) as Order;
    // ...
}
```

`GetSelectedRows` returns row handles (positive for data rows; negative for group rows when included). To iterate only data rows:

```csharp
foreach (int h in handles)
    if (h >= 0) Process(gridView1.GetRow(h));
```

`GetSelectedCells()` returns selected cells (`GridCell[]`) in cell-select mode.

## Select / Unselect Programmatically

```csharp
gridView1.SelectRow(rowHandle);
gridView1.UnselectRow(rowHandle);
gridView1.SelectRange(startRowHandle, endRowHandle);
gridView1.SelectAll();
gridView1.ClearSelection();
gridView1.InvertSelection();

// Cell selection
gridView1.SelectCell(rowHandle, gridView1.Columns["Total"]);
```

Batch operations should be wrapped in `BeginSelection` / `EndSelection` to prevent flicker and per-row `SelectionChanged` events:

```csharp
gridView1.BeginSelection();
try {
    for (int i = 0; i < gridView1.DataRowCount; i++)
        if ((string)gridView1.GetRowCellValue(i, "Role") == "Admin")
            gridView1.SelectRow(i);
} finally { gridView1.EndSelection(); }
```

`SelectionChanged` fires once after `EndSelection`.

## Selection Binding to a Data-Source Field

Bind the selection state to a `bool` field on each row — useful for persistent multi-select across grouping/filtering, automatic filter-by-selected-state, and clean MVVM-ish data flows.

```csharp
gridView1.OptionsSelection.MultiSelect          = true;
gridView1.OptionsSelection.MultiSelectMode      = GridMultiSelectMode.CheckBoxRowSelect;
gridView1.OptionsSelection.CheckBoxSelectorField = "IsSelected";
```

Each row's `IsSelected` property is written when checked/unchecked. Selection survives grouping, sorting, and filtering. **Do not** use Selection Binding in Instant Feedback mode — it materializes data and defeats async loading.

For a fully unbound checkbox column (without writing into the data source), use a `RepositoryItemCheckEdit` on an unbound column instead.

## Web-Style Row Selection (Checkbox Mode)

`CheckBoxRowSelect` + `ResetSelectionClickOutsideCheckboxSelector = true` produces this UX:

- Click anywhere outside the check column focuses the row and clears multi-selection.
- The check column's checkbox toggles row inclusion.
- The header check selects/deselects all (`ShowCheckBoxSelectorInColumnHeader = true`).
- `ShowCheckBoxSelectorChangesSelectionNavigation` controls whether keyboard navigation toggles checkboxes.

## Row Click vs Row Focus

`RowClick` fires on every mouse click on a row, even if the row is already focused. `FocusedRowChanged` fires only when the handle changes. Choose the right event:

- "User clicked a row to confirm a choice" → `RowClick` (combine with `DoubleClick` for activation).
- "Show details for the currently focused row" → `FocusedRowChanged`.

To delay `FocusedRowChanged` (e.g., expensive detail fetches), set `view.OptionsBehavior.FocusedRowChangedDelay = 250;`.

## TreeList Equivalents

```csharp
treeList1.OptionsSelection.MultiSelect       = true;
treeList1.OptionsSelection.UseIndicatorForSelection = true;
treeList1.OptionsSelection.MultiSelectMode   = TreeListMultiSelectMode.RowSelect;

TreeListNode focused = treeList1.FocusedNode;
TreeListMultiSelection selection = treeList1.Selection;
foreach (TreeListNode node in selection)
    Process(node);
```

`treeList1.FocusedNodeChanged` is the analog of `FocusedRowChanged`.

## Common Issues

- **`GetFocusedRow()` returns `null`**: no row is focused (empty grid), or the grid is in design mode. Check `view.IsValidRowHandle(view.FocusedRowHandle)`.
- **`SelectionChanged` fires for every row when I call `SelectAll`**: wrap in `BeginSelection` / `EndSelection`.
- **`CheckBoxRowSelect` causes performance hit on large data sets**: turn off `ShowCheckBoxSelectorInColumnHeader` (the header check enumerates all rows) or switch to unbound check column.
- **Selection lost after grouping**: with `RowSelect` mode, expand/group operations may rebuild handles; use Selection Binding to a `bool` field for persistence.
- **`FocusedRowHandle` is `-2147483647`**: that is `NewItemRowHandle` — the new-item row is focused. Save first or call `view.CloseEditor()` + `view.UpdateCurrentRow()`.

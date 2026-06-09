# Focus and Selection — DevExpress WPF Data Grid

Focus (where the input cursor is) and Selection (which rows / cells are "picked") are two related but distinct concepts. The Data Grid's `NavigationStyle` controls focus granularity (row vs cell vs none) and `SelectionMode` controls what users can select (none / single row / multiple rows / cells). This reference covers both APIs, the events that fire on changes, and how to bind state to a ViewModel.

## When to Use This Reference

Use this when you need to:

- Switch between Row / Cell / None navigation (`NavigationStyle`)
- Read or set the focused row / cell in code (`FocusedRowHandle`, `CurrentItem`, `CurrentColumn`)
- Move focus programmatically (`MoveFirstRow`, `MoveNextRow`, `MoveFocusedRow`)
- Enable / disable cell focus on specific columns (`AllowFocus`, `TabStop`)
- Configure single-row vs multi-row vs multi-cell selection (`SelectionMode`)
- Read or set selected items in code (`SelectedItem`, `SelectedItems`)
- Control which rows / cells the user can select (`CanSelectRow`, `CanSelectCell` events)
- Show the **Check-box Selector Column** (Excel-style selection)
- Enable the **Selection Rectangle** (marquee selection)
- Bind selection state to a ViewModel (TwoWay)
- Customize the visual appearance of focused / selected rows
- Add keyboard navigation between column headers and the Filter Panel
- Handle the master-detail / server-mode / filter caveats

## Navigation Style

The `DataViewBase.NavigationStyle` property controls focus granularity:

| Value | Effect |
|---|---|
| `Cell` (**default**) | Focus is per-cell. Users can edit cells. Required for cell selection mode. |
| `Row` | Focus is per-row only. Cell editing is unavailable (use Edit Form instead). |
| `None` | No row / cell focus. Useful for read-only display where users shouldn't interact. |

```xaml
<dxg:TableView NavigationStyle="Cell"/>
```

> When `NavigationStyle="Row"`, in-place editing is disabled. Users edit through the [Edit Form](cell-display-and-editing.md) instead.

Source: `articles/controls-and-libraries/data-grid/focus-navigation-selection/focus.md`.

## Focused Row

### Read / Set in Code

| API | Type | Description |
|---|---|---|
| `DataControlBase.CurrentItem` | `object` | The focused data item (binding-friendly — bind TwoWay to a ViewModel) |
| `DataViewBase.FocusedRowHandle` | `int` | Internal row handle. `int.MinValue` = no row focused |
| `TreeListView.FocusedNode` | `TreeListNode` | The focused node (TreeList only) |
| `DataControlBase.VisibleRowCount` | `int` | Total visible rows (for bounds checks) |

```csharp
// Set focused row by handle
view.FocusedRowHandle = 2;

// Read the focused data object
var order = (Order)grid.CurrentItem;
```

### Move Focus Methods (`DataViewBase`)

| Method | Effect |
|---|---|
| `MoveFirstRow()` | Focus first visible row / card |
| `MoveLastRow()` | Focus last visible row / card |
| `MovePrevRow()` / `MoveNextRow()` | Move one row |
| `MovePrevPage()` / `MoveNextPage()` | Move by the visible-page size |
| `MoveFocusedRow(int delta)` | Move by N rows |
| `GridViewBase.MoveParentGroupRow()` | Jump to the parent group row of the current row |

These methods **cannot** focus hidden rows or rows in collapsed groups / collapsed detail Views.

### Events

| Event | Fires |
|---|---|
| `DataViewBase.FocusedRowHandleChanging` | Before focus changes — set `e.Cancel = true` to block |
| `DataViewBase.FocusedRowHandleChanged` | After focus changes |
| `DataControlBase.CurrentItemChanged` | After the focused data item changes (use this for MVVM observability) |

```csharp
view.FocusedRowHandleChanging += (s, e) => {
    if (ShouldBlockFocus(e.NewRowHandle))
        e.Cancel = true;
};
```

### Initial Focus

By default the first row is focused on load. To start without focus:

```xaml
<dxg:GridControl AllowInitiallyFocusedRow="False"/>
```

### Auto-Scroll to Focused Row

```xaml
<dxg:TableView AllowScrollToFocusedRow="True"/>
```

## Focused Cell

Requires `NavigationStyle="Cell"`.

### Read / Set in Code

| API | Description |
|---|---|
| `DataControlBase.CurrentColumn` | The focused column |
| `DataViewBase.FocusedRowHandle` | (Combined with `CurrentColumn` identifies the focused cell) |

```csharp
grid.CurrentColumn = view.VisibleColumns[2];
view.FocusedRowHandle = 0;
```

### Move Focus Methods

| Method | Effect |
|---|---|
| `DataViewBase.MoveNextCell()` | Focus next cell in row order |
| `DataViewBase.MovePrevCell()` | Focus previous cell |

### Per-Column Focus Control

```xaml
<dxg:GridColumn FieldName="InternalId" AllowFocus="False"/>
<dxg:GridColumn FieldName="Notes"      TabStop="False"/>
```

- `AllowFocus="False"` — cell is unreachable by mouse or keyboard.
- `TabStop="False"` — cell is skipped during <kbd>Tab</kbd> navigation but still reachable by mouse.

## Customize Focused-Element Appearance

| Property | Effect |
|---|---|
| `DataViewBase.ShowFocusedRectangle` | Show / hide the focus rectangle around the focused row or cell |
| `DataViewBase.FocusedCellBorderTemplate` | Custom `DataTemplate` for the focused cell's border |
| `TableView.FocusedRowBorderTemplate` / `TreeListView.FocusedRowBorderTemplate` | Custom row border template |
| `GridViewBase.FocusedGroupRowBorderTemplate` | Custom group-row border template |
| `CardView.FocusedCardBorderTemplate` | Card-specific border |

## Selection Mode

The `DataControlBase.SelectionMode` property controls what the user can select:

| Value | Effect |
|---|---|
| `None` | No selection (rows are still focusable when `NavigationStyle ≠ None`) |
| `Row` (**default**) | Single row selection. Click selects, Ctrl-click toggles |
| `MultipleRow` | Multi-row selection with Ctrl-click and Shift-click |
| `Cell` | Cell selection (requires `NavigationStyle="Cell"`) |
| `MultipleCell` (in some docs `Cell` covers both — verify) | Cell range selection via drag, Ctrl-click, Shift-click |

```xaml
<dxg:GridControl SelectionMode="MultipleRow"/>
```

> Verify exact `MultiSelectMode` enum values via DxDocs MCP if needed — naming has slight variation between versions (`Row` vs `MultipleRow`, `Cell` vs `MultipleCell`). The properties listed in this reference are accurate per the source articles.

Source: `articles/controls-and-libraries/data-grid/focus-navigation-selection/multiple-row-selection.md` and `multiple-cell-selection.md`.

## Selected Rows

### Read in Code

| API | Description |
|---|---|
| `DataControlBase.SelectedItem` | The first-selected data item (= `SelectedItems[0]`). In single-selection mode, this is the same as the focused row. |
| `DataControlBase.SelectedItems` | Collection of selected data items (in selection order) |
| `DataControlBase.GetSelectedRowHandles()` | Selected row handles ordered by visible index |
| `GridViewBase.GetSelectedRows()` | Returns the visible row indexes of selected rows |
| `TreeListControlBase.GetSelectedNodes()` | Selected nodes (TreeList only), ordered by visible index |
| `DataControlBase.CurrentItem` | The focused row (may differ from `SelectedItem` in multi-select mode) |

### Modify Selection in Code

| Method | Effect |
|---|---|
| `DataControlBase.SelectAll()` | Select all rows |
| `DataControlBase.UnselectAll()` | Clear selection |
| `DataControlBase.SelectItem(int rowHandle)` | Select a specific row |
| `DataControlBase.UnselectItem(int rowHandle)` | Unselect a specific row |
| `DataControlBase.SelectRange(int handle1, int handle2)` | Select a range of rows |
| `DataControlBase.BeginSelection()` / `EndSelection()` | Batch selection changes to avoid intermediate notifications |

Always wrap bulk selection in `BeginSelection / EndSelection`:

```csharp
grid.BeginSelection();
try {
    grid.UnselectAll();
    foreach (var handle in handles)
        grid.SelectItem(handle);
} finally {
    grid.EndSelection();
}
```

### Selection Events

| Event | Fires |
|---|---|
| `DataControlBase.SelectedItemChanged` | After the primary selected item changes |
| `GridControl.SelectionChanged` / `TreeListControlBase.SelectionChanged` | After any selection change (add or remove) |
| `DataViewBase.CanSelectRow` | Before a row is selected — set `e.CanSelectRow = false` to block |
| `DataViewBase.CanUnselectRow` | Before a row is unselected |

### Control Per-Row Select Permission

```csharp
void View_CanSelectRow(object sender, CanSelectRowEventArgs e) {
    var visits = Convert.ToDouble(grid.GetCellValue(e.RowHandle, "Visits"));
    e.CanSelectRow = visits < 8;   // Block rows with Visits ≥ 8
}
```

```xaml
<dxg:TableView CanSelectRow="View_CanSelectRow"/>
```

## Selected Cells

Requires `SelectionMode="Cell"`.

| API | Description |
|---|---|
| `TableView.SelectCell(rowHandle, GridColumn)` | Select a cell |
| `TableView.SelectCells(startRow, startCol, endRow, endCol)` | Select a cell range |
| `TableView.UnselectCell(...)` / `UnselectCells(...)` | Unselect |
| `TableView.GetSelectedCells()` | Returns selected cells |
| `TableView.CanSelectCell` event | Block specific cells from selection |
| `TableView.CanUnselectCell` event | Block specific cells from being unselected |

```csharp
view.CanSelectCell += (s, e) => {
    e.CanSelectCell = e.Column.FieldName != "InternalId";
};
```

```xaml
<dxg:GridControl SelectionMode="Cell">
    <dxg:GridControl.View>
        <dxg:TableView CanSelectCell="View_CanSelectCell"
                       CanUnselectCell="View_CanUnselectCell"/>
    </dxg:GridControl.View>
</dxg:GridControl>
```

For TreeListControl, use `TreeListNode` instead of row handle:

```csharp
view.SelectCell(node, column);
```

## Check-Box Selector Column (Excel-Style Selection)

Adds a column with check boxes that let users select rows with a single click:

```xaml
<dxg:GridControl SelectionMode="MultipleRow">
    <dxg:GridControl.View>
        <dxg:TableView ShowCheckBoxSelectorColumn="True"
                       CheckBoxSelectorColumnPosition="Left"
                       ShowCheckBoxSelectorInGroupRow="True"
                       RetainSelectionOnClickOutsideCheckBoxSelector="False"/>
    </dxg:GridControl.View>
</dxg:GridControl>
```

| Property | Effect |
|---|---|
| `TableView.ShowCheckBoxSelectorColumn` | Show the selector column |
| `TableView.CheckBoxSelectorColumnPosition` | `Left`, `Right`, `None` (use `CheckBoxSelectorColumnVisibleIndex` to place arbitrarily) |
| `TableView.CheckBoxSelectorColumnVisibleIndex` | Set explicit column position (when Position = `None`) |
| `TableView.ShowCheckBoxSelectorInGroupRow` | Show check box in group headers (select all children) |
| `TableView.RetainSelectionOnClickOutsideCheckBoxSelector` | If `true`, only check-box clicks affect selection (row body clicks don't) |
| `TableView.CheckBoxSelectorColumnName` | The `FieldName` of the selector column (use in code) |

> **Selector column limitations**: cannot be sorted, filtered, grouped, resized, or drag-and-dropped.

### Conflicts with Selection Rectangle

When `ShowCheckBoxSelectorColumn` and `RetainSelectionOnClickOutsideCheckBoxSelector` are both `true`, the Selection Rectangle is disabled.

## Selection Rectangle (Marquee Selection)

Lets users drag the mouse to select a range of rows / cells / cards:

```xaml
<dxg:TableView ShowSelectionRectangle="True">
    <dxg:TableView.SelectionRectangleStyle>
        <Style TargetType="Border">
            <Setter Property="Opacity" Value="0.37"/>
            <Setter Property="Background" Value="Green"/>
        </Style>
    </dxg:TableView.SelectionRectangleStyle>
</dxg:TableView>
```

`ShowSelectionRectangle` requires `SelectionMode` to be `Row` or `Cell` (not `MultipleRow`). Also disabled if drag-and-drop is enabled.

## Hover Effects

```xaml
<dxg:TableView HighlightItemOnHover="True"/>
```

Highlights the row (or cell, depending on `SelectionMode`) under the mouse pointer.

## Selection Fade on Lost Focus

When the grid loses focus, selected rows fade to a lighter color by default:

```xaml
<dxg:TableView FadeSelectionOnLostFocus="False"/>
```

Set to `false` to keep the full selection color regardless of focus. Does not affect Cell selection mode.

## Customize Selected-Row Appearance

Use `TableView.RowStyle` with triggers on `RowControl.SelectionState`:

```xaml
<dxg:TableView.RowStyle>
    <Style TargetType="dxg:RowControl">
        <Style.Triggers>
            <Trigger Property="SelectionState" Value="Selected">
                <Setter Property="Background" Value="LightGreen"/>
            </Trigger>
            <Trigger Property="SelectionState" Value="Focused">
                <Setter Property="Background" Value="LightYellow"/>
            </Trigger>
            <Trigger Property="SelectionState" Value="FocusedAndSelected">
                <Setter Property="Background" Value="Green"/>
            </Trigger>
        </Style.Triggers>
    </Style>
</dxg:TableView.RowStyle>
```

`SelectionState` values: `None`, `Focused`, `Selected`, `Highlighted`, `FocusedAndSelected`.

To disable the built-in selected-row coloring entirely:

```xaml
<dxg:TableView EnableSelectedRowAppearance="False"/>
```

## MVVM Binding

```xaml
<dxg:GridControl ItemsSource="{Binding Orders}"
                 CurrentItem="{Binding CurrentOrder, Mode=TwoWay}"
                 SelectedItem="{Binding SelectedOrder, Mode=TwoWay}"
                 SelectedItems="{Binding SelectedOrders, Mode=TwoWay}"
                 SelectionMode="MultipleRow"/>
```

`CurrentItem` is the focused row; `SelectedItem` is the first selected row; `SelectedItems` is the full collection. Bind whichever your ViewModel needs.

For a complete example of binding `SelectedItems` to a `ObservableCollection<T>` on the ViewModel, see `articles/controls-and-libraries/data-grid/mvvm-enhancements/examples/binding-to-a-collection-of-selected-items.md`.

## Keyboard Navigation

### Within Cells / Rows

| Key | Effect |
|---|---|
| <kbd>Tab</kbd> / <kbd>Shift</kbd>+<kbd>Tab</kbd> | Next / previous cell (skips `TabStop="False"` columns) |
| <kbd>Arrow keys</kbd> | Move focus between rows or cells |
| <kbd>Page Up</kbd> / <kbd>Page Down</kbd> | Move by visible-page size |
| <kbd>Home</kbd> / <kbd>End</kbd> | First / last visible row |
| <kbd>Ctrl</kbd>+<kbd>Home</kbd> / <kbd>Ctrl</kbd>+<kbd>End</kbd> | First / last cell of grid |
| <kbd>Enter</kbd> / <kbd>F2</kbd> | Open editor (in Cell mode) |
| <kbd>Esc</kbd> | Cancel edit |
| <kbd>Ctrl</kbd>+<kbd>A</kbd> | Select all (in multi-row / multi-cell modes) |
| <kbd>Shift</kbd>+<kbd>Click</kbd> | Range select |
| <kbd>Ctrl</kbd>+<kbd>Click</kbd> | Toggle selection |

### Column Headers (`AllowHeaderNavigation`)

```xaml
<dxg:TableView AllowHeaderNavigation="True"/>
```

| Key on header | Action |
|---|---|
| <kbd>Enter</kbd> | Sort |
| <kbd>Shift</kbd>+<kbd>Enter</kbd> | Add to multi-column sort |
| <kbd>Ctrl</kbd>+<kbd>Enter</kbd> | Remove from multi-column sort |
| <kbd>Alt</kbd>+<kbd>↓</kbd> or <kbd>F4</kbd> | Open filter dropdown |
| <kbd>Ctrl</kbd>+<kbd>←</kbd> / <kbd>→</kbd> | Reorder column |
| <kbd>Shift</kbd>+<kbd>←</kbd> / <kbd>→</kbd> | Resize column |
| <kbd>Shift</kbd>+<kbd>F10</kbd> | Context menu |
| <kbd>Ctrl</kbd>+<kbd>Shift</kbd>+<kbd>Tab</kbd> | Move from data area to header panel |
| <kbd>Ctrl</kbd>+<kbd>Tab</kbd> | Return from header to data |

### Filter Panel (`AllowFilterPanelNavigation`)

```xaml
<dxg:TableView AllowFilterPanelNavigation="True"
               AllowLeaveFocusOnTab="True"/>
```

| Action | Key |
|---|---|
| Focus filter panel | <kbd>Ctrl</kbd>+<kbd>Shift</kbd>+<kbd>F</kbd> |
| Toggle filter check box | <kbd>Space</kbd> |
| Open Filter Editor | <kbd>Space</kbd> or <kbd>Enter</kbd> on Edit Filter button |
| Clear filter | <kbd>Space</kbd> or <kbd>Enter</kbd> on Clear Filter button |
| Return to data area | <kbd>↑</kbd> |

## Caveats and Limitations

- **Master-detail**: `SelectedItems` returns only master-grid items. For detail selections, query each detail control individually via `GridControl.GetDetail(...)` → `GetSelectedRowHandles()`.
- **Server Mode / Virtual Sources**: `SelectedItems` returns an empty list. Use `GetSelectedRowHandles()` + `GetRow(handle)` / `GetRowAsync(handle)` to obtain items.
- **Filtering resets selection**: when the user applies a filter, the grid clears selection then re-applies it. To preserve, snapshot `SelectedItems` before the filter change and restore after.
- **Service rows** (Auto Filter Row, New Item Row) — selecting them does NOT fire `SelectionChanged` or `SelectedItemChanged`, and does not reset selection.
- **Custom `CellTemplate`**: full keyboard navigation requires either (a) a `BaseEdit` descendant named `PART_Editor`, or (b) manual handling of `GetIsEditorActivationAction` / `GetActiveEditorNeedsKey`. See [cell-display-and-editing.md](cell-display-and-editing.md).

## Apply to TreeListControl

| GridControl member | TreeListControl equivalent |
|---|---|
| `DataViewBase.NavigationStyle` | same |
| `DataViewBase.FocusedRowHandle` | same; `TreeListView.FocusedNode` for node objects |
| `DataControlBase.CurrentItem` / `SelectedItem` / `SelectedItems` | same |
| `DataControlBase.GetSelectedRowHandles()` | same; **`TreeListControlBase.GetSelectedNodes()`** for `TreeListNode` collection |
| `DataViewBase.CanSelectRow` / `CanUnselectRow` events | same |
| `TableView.SelectCell` / `UnselectCell` (by row handle) | **`TreeListView.SelectCell(TreeListNode, ColumnBase)`** |
| `TableView.ShowCheckBoxSelectorColumn` | same (works for TreeList too) |

## Source Material

- `articles/controls-and-libraries/data-grid/focus-navigation-selection.md`
- `articles/controls-and-libraries/data-grid/focus-navigation-selection/focus.md`
- `articles/controls-and-libraries/data-grid/focus-navigation-selection/common-selection-features.md`
- `articles/controls-and-libraries/data-grid/focus-navigation-selection/multiple-row-selection.md`
- `articles/controls-and-libraries/data-grid/focus-navigation-selection/multiple-cell-selection.md`
- `articles/controls-and-libraries/data-grid/end-user-capabilities/selecting-rows.md`
- `articles/controls-and-libraries/data-grid/mvvm-enhancements/examples/binding-to-a-collection-of-selected-items.md`
- `articles/controls-and-libraries/data-grid/visual-elements/table-view-elements/selector-column.md`
- `articles/controls-and-libraries/data-grid/conditional-formatting/conditional-formats/formatting-focused-cells-and-rows.md`

# Advanced Features — DevExpress WPF Data Grid

This reference collects features that don't fit cleanly into other reference files: drag-and-drop, custom sort/group/filter, MVVM patterns beyond basic CRUD, performance tuning for large data, and the relationship between `GridControl` and the surrounding DevExpress WPF infrastructure.

## When to Use This Reference

Use this when you need to:

- Configure drag-and-drop between two grids
- Implement custom sorting, grouping, or filtering logic
- Tune performance for 100,000+ row data sets
- Bind the grid's state (sort, group, filter, focused row) to a ViewModel
- Configure save / restore of grid layout

## Drag-and-Drop Between Grids

The grid supports drag-and-drop of rows out of itself, between two grids, and reordering within itself. See `articles/controls-and-libraries/data-grid/drag-and-drop.md` and the `drag-and-drop/process-drag-and-drop/` subdirectory for the event-driven API:

- `DataViewBase.DragRecordOver`
- `DataViewBase.DropRecord`
- `DataViewBase.StartRecordDrag`
- `DataViewBase.CompleteRecordDragDrop`

> Verify exact event signatures and event argument members via:
> `devexpress_docs_search(technology="WPF Data Grid", query="DragRecordOver DropRecord")`

## Custom Sorting / Grouping / Filtering

The grid raises events that let you supply custom comparison or matching logic:

| Event | Where | Purpose |
|---|---|---|
| `GridControl.CustomColumnSort` | Grid | Pluggable comparison for a column. |
| `GridControl.CustomColumnGroup` | Grid | Pluggable group-key generator. |
| `GridControl.CustomGroupDisplayText` | Grid | Custom group caption text. |
| `GridControl.CustomUnboundColumnData` | Grid | Supply values for unbound columns. |
| `GridControl.SubstituteFilter` | Grid | Replace a filter criterion before it's applied. |
| `GridControl.CustomSummary` | Grid | Implement `SummaryType="Custom"`. |

Source: `articles/controls-and-libraries/data-grid/sorting.md` ("Sorting Modes and Custom Sorting" link) and `grouping.md` ("Group Modes and Custom Grouping" link).

## Save / Restore Grid Layout

The grid serializes its layout (column order, widths, sort, group, filter) via the DXSerializer infrastructure:

```csharp
grid.SaveLayoutToXml("grid-layout.xml");
grid.RestoreLayoutFromXml("grid-layout.xml");
```

Or stream-based:

```csharp
grid.SaveLayoutToStream(stream);
grid.RestoreLayoutFromStream(stream);
```

Wire `RestoreStateKeyFieldName` (often the entity's primary key field) so focused-row restoration works after the data is refetched.

Source: `articles/common-concepts/save-and-restore-layouts.md`.

## MVVM: Bind Grid State to ViewModel

| Bindable Property | What It Holds |
|---|---|
| `DataControlBase.FocusedRow` (TwoWay) | The selected entity in the bound source |
| `GridControl.CurrentItem` | Active record (alias / sometimes equivalent) |
| `DataControlBase.SelectedItems` | Multi-select collection |
| `GridControl.GroupOperations` (paraphrase) | Group expansion state |

```xaml
<dxg:GridControl ItemsSource="{Binding Orders}"
                 SelectedItem="{Binding SelectedOrder, Mode=TwoWay}"/>
```

> Verify the exact property names against `apidoc/DevExpress.Xpf.Grid/DataControlBase/` and `GridControl/` if you need to bind less common state (group expansion, scroll offset, etc.).

## Performance Tuning

For 100,000+ rows:

1. **Switch to a server-mode source** — `EntityServerModeSource`, `XPServerCollectionSource`, etc. See [data-binding.md](data-binding.md).
2. **Enable Instant Feedback Mode** — the UI thread stays responsive while data is fetched in background.
3. **Disable `AllowLiveDataShaping`** unless you need it — recalculation is expensive.
4. **Enable Optimized Summary Calculation** if applicable. See [data-summaries.md](data-summaries.md).
5. **Avoid custom `CellTemplate`** when possible — virtualization-friendly built-in editors are faster.

See `articles/controls-and-libraries/data-grid/performance-improvement/` for the full performance guide.

## Compare With Standard WPF DataGrid

`GridControl` is not API-compatible with `System.Windows.Controls.DataGrid`. Migration touch points:

| Standard `DataGrid` | DevExpress `GridControl` |
|---|---|
| `DataGrid.ItemsSource` | `DataControlBase.ItemsSource` |
| `DataGridTextColumn` | `GridColumn` |
| `DataGridComboBoxColumn` | `GridColumn` + `ComboBoxEditSettings` |
| `Sorting`, `Filtering` (built-in) | Via View: `DataViewBase.AllowSorting`, drop-down filters |
| `CellEditEnding` event | `CellValueChanged` event, or `ValidateRow` MVVM command |
| `RowDetailsTemplate` | `DataControlDetailDescriptor` |
| `CanUserAddRows` | `TableView.NewItemRowPosition` |

Migration article (in the repo): `articles/controls-and-libraries/data-grid/compare-grids.md`.

## Source Material

- `articles/controls-and-libraries/data-grid/drag-and-drop.md`
- `articles/controls-and-libraries/data-grid/performance-improvement.md`
- `articles/controls-and-libraries/data-grid/mvvm-enhancements/`
- `articles/controls-and-libraries/data-grid/compare-grids.md`
- `articles/common-concepts/save-and-restore-layouts.md`

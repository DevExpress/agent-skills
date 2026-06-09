# Cell Display and Editing — DevExpress WPF Data Grid

The Data Grid offers **nine techniques** to control how a cell displays its value and **three modes** to let users edit it (in-place, edit entire row, edit form). This reference is a decision matrix plus the API surface for each technique. The same patterns apply to `TreeListControl` — see § "Apply to TreeListControl" at the end.

## When to Use This Reference

Use this when you need to:

- Pick the right technique for displaying a formatted value (currency, percentage, masked text, etc.)
- Decide between `CellTemplate` vs `CellDisplayTemplate` + `CellEditTemplate`
- Use `CustomColumnDisplayText` to compute display text per cell
- Use unbound columns or `ColumnBase.Binding` instead of `FieldName`
- Configure editor activation (click, Enter, F2, typing) and validation timing
- Make cells / columns / rows read-only or fully prohibit editing
- Choose between **Edit Entire Row** and **Edit Form** modes
- Customize the Edit Form layout (column count, captions, row spans, custom template)

## The Display-Formatting Decision Matrix

Every formatting technique has trade-offs across four dimensions. Pick the technique that ticks the boxes you need.

| Technique | Affects column type | Sort / filter / group | Print / export | Works in edit mode |
|---|---|---|---|---|
| `GridControl.CustomColumnDisplayText` event | No | **Yes**\* | Yes (WYSIWYG only) | No |
| Unbound Columns (`UnboundDataType` + `CustomUnboundColumnData`) | **Yes** | Yes\* | **Yes** | **Yes** |
| `ColumnBase.Binding` (instead of `FieldName`) | **Yes** | Yes\* | **Yes** | **Yes** |
| `BaseEditSettings.DisplayFormat` | No | Yes\* | **Yes** | No |
| `BaseEditSettings.DisplayTextConverter` | No | Yes\* | Yes (set `TextExportMode=Text`) | No |
| `EditSettings` Masked Input (`Mask`, `MaskUseAsDisplayFormat`) | No | Yes\* | **Yes** | **Yes** |
| `CellTemplate` → `BaseEdit.DisplayFormatString` | No | No | No | No |
| `CellTemplate` → `BaseEdit.DisplayTextConverter` | No | No | No | No |
| `CellTemplate` → `BaseEdit.Mask` | No | No | No | **Yes** |

\* Except in Server Mode and Virtual Sources (which compute sort / filter / group on the server).

**Rule of thumb**:

- Need formatting to participate in sort / filter / group AND in export AND in edit mode? → **Unbound Columns** or **`Binding`**.
- Need formatting on print / export but not in edit mode? → **EditSettings.DisplayFormat** or **DisplayTextConverter**.
- Need formatting only on display (not export / not search)? → **CellTemplate** with `DisplayFormatString` / `DisplayTextConverter`.
- Need full per-cell control of display text via code? → **`CustomColumnDisplayText` event**.
- Need different visuals for display vs edit (e.g., text label when not editing, full editor when editing)? → **`CellDisplayTemplate` + `CellEditTemplate`** (modern, preferred over single `CellTemplate`).

Source: `articles/controls-and-libraries/data-grid/appearance-customization/format-cell-values.md`.

## 1. `CustomColumnDisplayText` Event

Pure event-driven display text — no schema changes:

```xaml
<dxg:GridControl CustomColumnDisplayText="grid_CustomColumnDisplayText" ...>
    <dxg:GridColumn FieldName="Value"/>
</dxg:GridControl>
```

```csharp
void grid_CustomColumnDisplayText(object sender, CustomColumnDisplayTextEventArgs e) {
    if (e.Column.FieldName == "Value")
        e.DisplayText = string.Format("{0:n2}", e.Value);
}
```

MVVM-friendly alternative: bind `GridControl.CustomColumnDisplayTextCommand` to a `ViewModel` command.

Underlying value is unchanged — sort / filter / group still operate on the raw value. Display text is rendered as-is and printed (in WYSIWYG mode).

## 2. Unbound Columns

A column not bound to a data source field — its value is supplied per-row by a code handler:

```xaml
<dxg:GridColumn FieldName="ValueUnbound"
                UnboundDataType="{x:Type sys:String}"/>
```

```csharp
private void grid_CustomUnboundColumnData(object sender, GridColumnDataEventArgs e) {
    e.Value = string.Format("{0:n2}", e.GetListSourceFieldValue("Value"));
}
```

The `UnboundDataType` determines the column type — so sort / filter / group / export all work as if this were a real bound column. `e.GetListSourceFieldValue` pulls the raw value of any other column for the current row.

## 3. `ColumnBase.Binding` (Instead of `FieldName`)

Bind the column to a data source's property via a WPF `Binding`, with optional `Converter`:

```xaml
<dxg:GridColumn Binding="{Binding Value, Converter={StaticResource myConverter}}"/>
```

The converter's output becomes the column's effective value. Like Unbound Columns, this gives you full sort / filter / group / export support. Unlike `FieldName`, you can shape the value (project a property, apply a converter, combine fields).

> Note: `FieldName` and `Binding` are alternative ways to bind a column — use one, not both. `FieldName` is simpler; `Binding` is more flexible (binding source, mode, converter).

## 4. `EditSettings.DisplayFormat`

Format the editor's display value via a format string:

```xaml
<dxg:GridColumn FieldName="Value">
    <dxg:GridColumn.EditSettings>
        <dxe:TextEditSettings DisplayFormat="c2"/>
    </dxg:GridColumn.EditSettings>
</dxg:GridColumn>
```

`c2` → currency with 2 decimals (`$99.90`). See [Format Specifiers](https://learn.microsoft.com/en-us/dotnet/standard/base-types/standard-numeric-format-strings) for the full set.

Print / export uses the formatted value. Edit mode shows the raw value.

## 5. `EditSettings.DisplayTextConverter`

A converter that produces the display text from the cell value:

```xaml
<dxg:GridColumn FieldName="Value">
    <dxg:GridColumn.EditSettings>
        <dxe:TextEditSettings DisplayTextConverter="{StaticResource myConverter}"/>
    </dxg:GridColumn.EditSettings>
</dxg:GridColumn>
```

Use when you need richer logic than `DisplayFormat` can express (e.g., status code → "Open" / "Closed").

## 6. Masked Input (`EditSettings.Mask*`)

```xaml
<dxg:GridColumn FieldName="Phone">
    <dxg:GridColumn.EditSettings>
        <dxe:TextEditSettings MaskType="RegEx"
                              Mask="\(\d{3}\) \d{3}-\d{4}"
                              MaskUseAsDisplayFormat="True"/>
    </dxg:GridColumn.EditSettings>
</dxg:GridColumn>
```

`MaskUseAsDisplayFormat="True"` applies the mask in display mode as well. Both display and edit see the formatted value; sort / filter / group / export all work on the underlying data.

Common mask types: `Numeric`, `DateTime`, `RegEx`, `PhoneNumber`, `Simple`. See https://docs.devexpress.com/content/WPF/6945?md=true for the full mask reference.

## 7–9. Cell Templates (`DisplayFormatString` / `DisplayTextConverter` / `Mask` Inside CellTemplate)

When using a custom `CellTemplate`, the inner `BaseEdit` has its own `DisplayFormatString`, `DisplayTextConverter`, and `Mask*` properties:

```xaml
<dxg:GridColumn FieldName="Value">
    <dxg:GridColumn.CellTemplate>
        <DataTemplate>
            <dxe:TextEdit Name="PART_Editor" DisplayFormatString="c2"/>
        </DataTemplate>
    </dxg:GridColumn.CellTemplate>
</dxg:GridColumn>
```

**Critical**: the editor inside `CellTemplate` must be named `PART_Editor` for the grid to recognize it. Without this name, the cell renders but does not participate in the grid's edit lifecycle.

**Limitation**: `CellTemplate` formatting does NOT participate in sort / filter / group / print / export. Sort/filter/group operates on the raw value; print / export uses the editor's default formatting. For full coverage, use `EditSettings` techniques instead, or use `PrintCellStyle` separately for print/export.

## `CellTemplate` vs `CellDisplayTemplate` + `CellEditTemplate`

Single `CellTemplate` defines both display and edit appearance. The split-template approach is cleaner:

```xaml
<dxg:GridColumn FieldName="Status">
    <dxg:GridColumn.CellDisplayTemplate>
        <DataTemplate>
            <TextBlock Text="{Binding RowData.Row.Status}" FontWeight="Bold"/>
        </DataTemplate>
    </dxg:GridColumn.CellDisplayTemplate>
    <dxg:GridColumn.CellEditTemplate>
        <DataTemplate>
            <dxe:ComboBoxEdit Name="PART_Editor"
                              ItemsSource="{Binding View.DataContext.AvailableStatuses}"/>
        </DataTemplate>
    </dxg:GridColumn.CellEditTemplate>
</dxg:GridColumn>
```

`CellDisplayTemplate` runs when the cell is not in edit mode (read-only label, badge, icon). `CellEditTemplate` activates when the user clicks / presses Enter / F2 / types.

When to prefer split templates:
- Display should be lightweight (no editor allocation per cell)
- Display uses a TextBlock / Border / custom visual, while edit uses an editor
- You want different visual styling per mode (e.g., colored badge for display, plain editor for edit)

`CellTemplate` is still useful for templates where display and edit share the same control (e.g., `CheckEdit` always shows a checkbox).

Source: `articles/controls-and-libraries/data-grid/appearance-customization/format-cell-values.md` § Cell Templates tip block.

## Choosing Between Templates Based on Logic

Use `CellTemplateSelector`, `CellDisplayTemplateSelector`, or `CellEditTemplateSelector` to switch templates per row based on data:

```xaml
<dxg:GridColumn FieldName="Status"
                CellDisplayTemplateSelector="{StaticResource StatusTemplateSelector}"/>
```

The selector implements `DataTemplateSelector` and inspects the row data to pick a template. See `articles/controls-and-libraries/data-grid/appearance-customization/choosing-templates-based-on-custom-logic.md`.

## Read-Only vs. Disabled Editing

| Behavior | API | Effect |
|---|---|---|
| **Read-only column** | `GridColumn.ReadOnly="True"` | Users can open the editor (select / copy text) but cannot change the value. Code can still call `GridControl.SetCellValue`. |
| **Read-only entire grid** | Style `TargetType="dxg:GridColumn"` with `ReadOnly` setter in `Window.Resources` | Apply read-only to all columns. |
| **Read-only per row / cell (data-bound)** | `BaseColumn.IsReadOnlyBinding="{Binding YourBooleanProperty}"` | Bind read-only state to a row property. |
| **Disabled editing** | `DataViewBase.AllowEditing="False"` (view) or `ColumnBase.AllowEditing="False"` (column) | Users **cannot open the editor**. No selection / copy. |

`ColumnBase.AllowEditing` has a special `Default` value — meaning "inherit from the View". `True` / `False` override.

Source: `articles/controls-and-libraries/data-grid/data-editing-and-validation/modify-cell-values/disable-editing.md`.

## In-Place Editor Activation

| Property / Method | Effect |
|---|---|
| `DataViewBase.NavigationStyle` | `Cell` (default for editing) — Tab moves between cells; `Row` — only row focus, edit only via Edit Form |
| `DataViewBase.ShowEditor()` | Open editor on focused cell (code) |
| `DataViewBase.CloseEditor()` | Save changes and close |
| `DataViewBase.HideEditor()` | Discard changes and close |
| `DataViewBase.PostEditor()` | Validate and queue cell changes (without closing) |
| `DataViewBase.CommitEditing()` | Validate and post all changes in the focused row |
| `GridViewBase.ShowingEditor` event | Cancel via `e.Cancel = true` |
| `GridViewBase.ShownEditor` event | Fires after editor is shown |
| `GridViewBase.HiddenEditor` event | Fires after editor closes |
| `DataViewBase.GetIsEditorActivationAction` | Per-action control (key press, click, typing) |
| `DataViewBase.GetActiveEditorNeedsKey` | Tell the grid which keys the active editor should consume (vs. the grid handling for navigation) |

Source: `articles/controls-and-libraries/data-grid/data-editing-and-validation/modify-cell-values/inplace-editors.md`.

## Edit Entire Row

Show **Update** / **Cancel** buttons on the row being edited; commit / discard all changes at once:

```xaml
<dxg:TableView ShowUpdateRowButtons="OnCellEditorOpen"
               NavigationStyle="Cell"/>
```

`ShowUpdateRowButtons` values:
- `Never` — disabled (default)
- `OnCellEditorOpen` — buttons appear when an editor opens
- `OnCellValueChange` — buttons appear after the first change
- `Always` — always visible

End-user can also press `<kbd>Esc</kbd>` twice to discard. Programmatic equivalents:
- `TableView.UpdateRow()` / `TableViewCommands.UpdateRow`
- `TableView.CancelRowChanges()` / `TableViewCommands.CancelRowChanges`

> `Edit Entire Row` mode is only available when `NavigationStyle="Cell"`. With `NavigationStyle="Row"`, use **Edit Form** instead.

Source: `articles/controls-and-libraries/data-grid/data-editing-and-validation/modify-cell-values/edit-entire-row.md`.

## Edit Form

A separate form (popup or inline) with all column editors. Best for many fields per row, or when row-level navigation is preferred.

### Enable Edit Form

```xaml
<dxg:TableView EditFormShowMode="Inline"/>
```

`EditFormShowMode` values:

| Value | Where the form appears |
|---|---|
| `None` | Edit Form disabled |
| `Inline` | Below the row being edited |
| `InlineHideRow` | Replaces the row being edited |
| `Dialog` | Pop-up dialog window |

### Trigger the Form

```xaml
<dxg:TableView EditFormShowMode="Dialog"
               ShowEditFormOnDoubleClick="True"
               ShowEditFormOnEnterKey="True"
               ShowEditFormOnF2Key="True"/>
```

### Programmatic Control

| Method | Effect |
|---|---|
| `TableView.ShowEditForm()` | Show in current `EditFormShowMode` |
| `TableView.ShowInlineEditForm()` | Force inline |
| `TableView.ShowDialogEditForm()` | Force dialog |
| `TableView.CloseEditForm()` | Save changes + close |
| `TableView.HideEditForm()` | Discard + close |

### Customize Form Layout

| Property | Effect |
|---|---|
| `TableView.EditFormColumnCount` | Number of editors per row (default 3) |
| `ColumnBase.EditFormVisibleIndex` | Order within the form |
| `ColumnBase.EditFormStartNewRow` | Place this editor on a new row |
| `ColumnBase.EditFormVisible` | `False` to hide from form |
| `ColumnBase.EditFormCaption` | Override label (default = column header) |
| `ColumnBase.EditFormColumnSpan` / `EditFormRowSpan` | Grid spans |
| `TableView.EditFormCaptionBinding` | Custom window title (Dialog mode) |

### Custom Form Template

```xaml
<dxg:TableView.EditFormTemplate>
    <DataTemplate>
        <GroupBox Header="Customer Details">
            <StackPanel>
                <!-- Your custom form layout -->
            </StackPanel>
        </GroupBox>
    </DataTemplate>
</dxg:TableView.EditFormTemplate>
```

When you override `EditFormTemplate`, the `RowEditStarted` event does NOT fire automatically — wire it via `EventToCommand` in the template root if you need it. See the article's note (around line 96) for the workaround.

`TableView.EditFormDialogServiceTemplate` customizes the dialog window in `Dialog` mode (window chrome, buttons, etc.).

Source: `articles/controls-and-libraries/data-grid/data-editing-and-validation/modify-cell-values/edit-form.md`.

## Decision: Edit Entire Row vs. Edit Form vs. In-Place

| Scenario | Pick |
|---|---|
| Few columns, edits happen often, immediate feedback | **In-place editors** (default) |
| Many columns, user edits multiple cells per row before committing | **Edit Entire Row** |
| Editing should feel like opening a record dialog (CRUD-heavy workflow) | **Edit Form (Dialog)** |
| Editor needs more vertical space than a row (rich text, multi-line, complex composition) | **Edit Form (Inline / InlineHideRow)** |
| Row-only navigation (no per-cell focus) | **Edit Form** (required — In-place / Edit Row need cell navigation) |

## Refresh Display Without Re-Bind

After modifying the underlying data without going through the grid editor:

```csharp
grid.RefreshData();              // Refresh all rows
grid.RefreshRow(rowHandle);      // Refresh a specific row
```

If your data source raises `INotifyPropertyChanged` or `INotifyCollectionChanged`, the grid picks up changes automatically. Otherwise call refresh explicitly.

To recalculate sort / filter / summary on data changes:

```xaml
<dxg:GridControl AllowLiveDataShaping="True"/>
```

## Apply to TreeListControl

Everything in this reference applies identically to `TreeListControl` — the API surface is shared via `DataViewBase`, `ColumnBase`, `GridViewBase`:

| GridControl property / event | TreeListControl equivalent |
|---|---|
| `GridControl.CustomColumnDisplayText` | `TreeListView.CustomColumnDisplayText` |
| `GridControl.CustomColumnDisplayTextCommand` | `TreeListView.CustomColumnDisplayTextCommand` |
| `TableView.EditFormShowMode` | `TreeListView.EditFormShowMode` |
| `TableView.ShowUpdateRowButtons` | `TreeListView.ShowUpdateRowButtons` |
| `TableView.ShowEditForm()` etc. | `TreeListView.ShowEditForm()` etc. |
| `GridColumn.CellDisplayTemplate` / `CellEditTemplate` | `TreeListColumn.CellDisplayTemplate` / `CellEditTemplate` (inherited from `ColumnBase`) |

In the `wpf-devexpress-tree-list` skill, `references/editing.md` covers tree-specific concerns (node add/remove, expand state); refer to this file for the shared cell display + editing details.

## Source Material

- `articles/controls-and-libraries/data-grid/appearance-customization/format-cell-values.md` (the decision matrix)
- `articles/controls-and-libraries/data-grid/data-editing-and-validation/data-editing-and-validation.md`
- `articles/controls-and-libraries/data-grid/data-editing-and-validation/modify-cell-values/inplace-editors.md`
- `articles/controls-and-libraries/data-grid/data-editing-and-validation/modify-cell-values/edit-entire-row.md`
- `articles/controls-and-libraries/data-grid/data-editing-and-validation/modify-cell-values/edit-form.md`
- `articles/controls-and-libraries/data-grid/data-editing-and-validation/modify-cell-values/disable-editing.md`
- `articles/controls-and-libraries/data-grid/data-editing-and-validation/modify-cell-values/assign-an-editor-to-a-cell.md`
- `articles/controls-and-libraries/data-grid/appearance-customization/choosing-templates-based-on-custom-logic.md`

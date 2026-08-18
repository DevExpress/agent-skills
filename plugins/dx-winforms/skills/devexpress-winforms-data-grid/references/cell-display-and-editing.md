# Cell Display and Editing

This reference covers everything about how cells display their values and how users edit them — the relationship between cell editors (`RepositoryItem`s) and columns, formatting via `DisplayFormat` / `EditFormat` / `CustomColumnDisplayText`, the editing modes (`Inplace`, `EditForm`, `EditFormInplace`), per-cell editor switching, read-only enforcement, and Edit Form customization.

## When to Use This Reference

- Assigning an in-place editor (combo box, spin, date picker, lookup, progress bar, custom) to a column.
- Choosing the right way to format displayed text vs edited text.
- Deciding between inline cell editing, Edit Form per row, and editing the entire row.
- Making specific cells read-only based on data in other cells.
- Customizing the Edit Form layout via `EditFormPrepared`.

## Repository Items vs Real Editors

A **data editor** is a UI control like `SpinEdit`, `LookUpEdit`, `CheckEdit`, `DateEdit`. A **repository item** (`RepositoryItem*`) is the settings bag for that editor. The grid does not host live editor instances per cell — it creates a single shared **active editor** when the user starts editing a cell and destroys it when focus leaves. The repository item supplies all the settings needed to recreate the active editor.

```text
              ┌──────────────────────┐
              │ RepositoryItem*      │  ← settings (template)
              │   Mask, ReadOnly,    │
              │   Items, etc.        │
              └─────────┬────────────┘
                        │ assigned to
                        ▼
   GridColumn.ColumnEdit                     view.ActiveEditor (transient)
   (or ColumnEditName)                       ─► live SpinEdit / LookUpEdit / …
```

Each DevExpress editor has a corresponding repository item: `SpinEdit` ↔ `RepositoryItemSpinEdit`, `LookUpEdit` ↔ `RepositoryItemLookUpEdit`, `CheckEdit` ↔ `RepositoryItemCheckEdit`, `ProgressBarControl` ↔ `RepositoryItemProgressBar`, `RichEditControl` ↔ `RepositoryItemRichTextEdit`, etc.

## Assign an Editor to a Column

```csharp
var spin = new RepositoryItemSpinEdit {
    MinValue = 0,
    MaxValue = 1000,
    DisplayFormat = { FormatType = FormatType.Numeric, FormatString = "n0" }
};
gridControl1.RepositoryItems.Add(spin);          // always add to the grid's repository first
gridView1.Columns["Quantity"].ColumnEdit = spin; // assign by reference

// Or by name:
spin.Name = "riQuantity";
gridView1.Columns["Quantity"].ColumnEditName = "riQuantity";
```

You can share one repository item across multiple columns or even multiple controls. Settings changes propagate everywhere — be careful editing a shared instance at runtime.

## Default Cell Editors

If `ColumnEdit` is not set, the grid auto-picks based on the column's data type:

| Data type | Default editor |
|---|---|
| `string` | `RepositoryItemTextEdit` |
| numeric | `RepositoryItemTextEdit` (no spin) |
| `bool` | `RepositoryItemCheckEdit` |
| `DateTime` | `RepositoryItemDateEdit` |
| `byte[]` / `Image` | `RepositoryItemPictureEdit` |

## Format Cell Values

Three layers, picked in this precedence:

1. **`GridColumn.DisplayFormat`** — display mode, value not formatted in the editor.
2. **`RepositoryItem.DisplayFormat`** — used by the editor in display mode.
3. **`RepositoryItem.EditFormat`** — used by the editor when the cell enters edit mode. Use input masks instead of `EditFormat` for controlled input.

```csharp
// Numeric column displayed as currency, edited as plain number
GridColumn col = gridView1.Columns["Price"];
col.DisplayFormat.FormatType   = FormatType.Numeric;
col.DisplayFormat.FormatString = "c2";

// DateTime column shown as long date
GridColumn dateCol = gridView1.Columns["OrderDate"];
dateCol.DisplayFormat.FormatType   = FormatType.DateTime;
dateCol.DisplayFormat.FormatString = "D";
```

Per-cell formatting that depends on other column values uses repository items per row — see *Per-cell editor switching* below.

## `CustomColumnDisplayText`

Provides the exact string displayed in a cell (including group rows and summary rows). Use it when format strings are not expressive enough, when the displayed text must reference another column, or when group rows need custom labels.

```csharp
gridView1.CustomColumnDisplayText += (s, e) => {
    if (e.Column.FieldName == "Total") {
        var raw = Convert.ToDecimal(e.Value);
        if (raw < 0) e.DisplayText = $"({-raw:c2})";
    }
    if (e.IsForGroupRow && e.Column.FieldName == "OrderDate") {
        e.DisplayText = $"Year: {((DateTime)e.Value).Year}";
    }
};
```

`CustomColumnDisplayText` runs frequently — keep handlers cheap.

For exporting the custom text instead of the raw value, also set `column.ColumnEdit.ExportMode = ExportMode.DisplayText` (or `XlExportOptionsBase.TextExportMode = TextExportMode.Text` globally).

## Editing Modes

`view.OptionsBehavior.EditingMode` controls how the user edits rows:

| Value | Behavior |
|---|---|
| `Default` | Same as `Inplace`. |
| `Inplace` | Click → cell editor activates inline. The classic Excel-style flow. |
| `EditForm` | Pressing F2 or double-clicking a row opens an **Edit Form** with all editable fields. Cells outside the form are read-only. |
| `EditFormInplace` | Inline editing **and** a button per row that opens the Edit Form. Best of both. |

```csharp
gridView1.OptionsBehavior.EditingMode = GridEditingMode.EditFormInplace;
```

### Edit Form basics

The Edit Form is generated automatically from visible columns. Each column with `OptionsColumn.AllowEdit = true` becomes an editor. The form's title is the row's primary key (configurable).

Customize at runtime:

- **`GridView.EditFormShowing`** — cancel opening the form, or modify the form's `Bindable*` controls before display.
- **`GridView.EditFormPrepared`** — fires after the form is built and bindings are wired; modify field labels, layout, sizes.
- **`GridView.EditFormHiding`** — cancel commit / cancel.

```csharp
gridView1.EditFormShowing += (s, e) => {
    if ((string)gridView1.GetRowCellValue(e.RowHandle, "Country") == "France")
        e.Allow = false;        // disallow opening the Edit Form for French orders
};

gridView1.EditFormPrepared += (s, e) => {
    foreach (var ctl in e.BindableControls)
        if (ctl is BaseEdit be && be.Properties is RepositoryItemDateEdit)
            be.Properties.ReadOnly = true;     // make all dates read-only
};
```

For Edit Form layout customization (which fields appear, in which order, with what captions), open the Designer → right-click the view → **Edit Form Designer**.

## Edit Entire Row

There is no single property called "EditEntireRow" — instead, set `EditingMode = EditForm` (or `EditFormInplace`) which logically treats the row as one editable unit. Combine with:

- `view.OptionsBehavior.AllowAddRows` / `AllowDeleteRows`
- `view.OptionsView.NewItemRowPosition = NewItemRowPosition.Top` (or `Bottom`) — show the new-item row for adding records.
- `view.UpdateCurrentRow()` — programmatically commit the focused row.

### Edit Form vs in-place — quick guide

- Pick **`Inplace`** for spreadsheet-style scenarios where users frequently change individual cells.
- Pick **`EditForm`** when each row represents a complex entity edited holistically (e.g., a customer profile with 20 fields).
- Pick **`EditFormInplace`** when both flows are needed.

## Read-Only Cells

Several mechanisms, from coarsest to finest:

| Scope | Member | Behavior |
|---|---|---|
| Entire view | `view.OptionsBehavior.Editable = false` | No cell can be edited. |
| Entire column | `column.OptionsColumn.AllowEdit = false` | Editor cannot open. |
| Entire column (Edit Form respects this) | `column.OptionsColumn.ReadOnly = true` | Editor opens (so users can copy text) but is read-only. The Edit Form respects this. |
| Cell-level | `view.ShowingEditor` event | Cancel the event to block editing for a specific cell. |
| Cell-level | `view.CustomRowCellEdit` event | Assign a read-only repository item for specific cells. |
| Cell-level | `view.ShownEditor` event | Toggle `view.ActiveEditor.Properties.ReadOnly` after the editor opens. |
| Cell-level | Disabled Cell Behavior | Grays out and prevents editing for cells matching a condition. |

### Example — cancel editing based on another column

```csharp
gridView1.ShowingEditor += (s, e) => {
    var country = gridView1.GetRowCellValue(gridView1.FocusedRowHandle, "Country")?.ToString();
    if (country == "Germany") e.Cancel = true;
};
```

### Example — substitute a read-only editor per cell

```csharp
RepositoryItemTextEdit readOnly = new() { ReadOnly = true };
gridControl1.RepositoryItems.Add(readOnly);

gridView1.CustomRowCellEdit += (s, e) => {
    if (e.Column.FieldName == "Value"
        && !(bool)gridView1.GetRowCellValue(e.RowHandle, "AllowEdit"))
        e.RepositoryItem = readOnly;
};
```

When `EditingMode = EditForm`, only `OptionsColumn.ReadOnly` and the `EditFormShowing` / `EditFormPrepared` events are effective for field-level read-only — `OptionsColumn.AllowEdit` does **not** suppress fields inside the form.

## Per-Cell Editor Switching

Two events:

- **`CustomRowCellEdit`** — fires when the view prepares to *display* the cell. Returned editor is used for both display and edit.
- **`CustomRowCellEditForEditing`** — fires when the user *activates* the editor. Use this when display and edit need different editors (e.g., display a `ProgressBar` but edit with a `SpinEdit`).

```csharp
RepositoryItemProgressBar progress = new();
RepositoryItemSpinEdit    spin     = new() { MinValue = 0, MaxValue = 100 };
gridControl1.RepositoryItems.AddRange(new RepositoryItem[] { progress, spin });

gridView1.Columns["Completion"].ColumnEdit = progress;
gridView1.CustomRowCellEditForEditing += (s, e) => {
    if (e.Column.FieldName == "Completion") e.RepositoryItem = spin;
};
```

> Performance tip: do **not** allocate new repository items in event handlers. Pre-create and reuse them — handlers fire on every paint/edit.

## Immediate Post

By default, edits commit to the data source when focus moves to another row. To post immediately on value change:

- **Per-editor**: `editor.InplaceModeImmediatePostChanges = true` (on `BaseRepositoryItemCheckEdit` descendants — `CheckEdit`, `ToggleSwitch`, `RadioGroup`, `TrackBarControl`, `RatingControl`, popup editors).
- **Global**: `WindowsFormsSettings.InplaceEditorUpdateMode = InplaceEditorUpdateMode.Immediate`.

## Common Issues

- **`column.ColumnEdit` set but no editor shows**: the repository item was not added to `gridControl.RepositoryItems`. Always add to the collection first.
- **Format string ignored**: `DisplayFormat.FormatType` is `None`. Set it to `Numeric`, `DateTime`, or `Custom`.
- **`CustomColumnDisplayText` not called for group rows**: ensure `e.IsForGroupRow` branch is reachable — the event fires for group rows when grouping is active.
- **Editor reopens after `Esc`**: the cell value did not change; on validation failure either set `e.ExceptionMode = ExceptionMode.NoAction` or call `view.HideEditor()` to discard.
- **Edit Form opens with wrong fields**: customize the form in the Edit Form Designer or set `column.OptionsColumn.ShowInCustomizationForm = false` / `column.OptionsColumn.AllowEdit = false`.

## Source Material

- `articles/controls-and-libraries/data-grid/data-editing-and-validation.md` (`xref:WindowsForms.114634`).
- `articles/controls-and-libraries/data-grid/data-editing-and-validation/modify-and-validate-cell-values.md` — comprehensive editing reference (`xref:WindowsForms.753`).
- `articles/controls-and-libraries/data-grid/appearance-customization/format-cell-values.md` (`xref:WindowsForms.1473`).
- `articles/controls-and-libraries/data-grid/examples/data-editing/how-to-specify-different-in-place-editors-to-use-in-display-and-edit-modes.md` (`xref:WindowsForms.3084`).
- `api/DevExpress.XtraGrid.Columns.GridColumn.ColumnEdit.yml`.
- `api/DevExpress.XtraGrid.Views.Base.ColumnView.ShowingEditor.yml`.
- `api/DevExpress.XtraGrid.Views.Grid.GridView.EditFormShowing.yml`.
- `api/DevExpress.XtraEditors.Repository.RepositoryItem.yml`.

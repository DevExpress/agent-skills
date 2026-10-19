# Validation

This reference covers cell-level and row-level validation in `GridView`, `BandedGridView`, `AdvBandedGridView`, `CardView`, `LayoutView`, and `TreeList`. Pick a level (cell vs. row), choose the matching event (`ValidatingEditor` vs. `ValidateRow`), and decide how to surface errors (modal message, inline error icon, no UI).

## When to Use This Reference

- Rejecting bad input as it leaves a cell (length, range, regex).
- Validating cross-field rules at row commit (`ShipDate >= OrderDate`).
- Showing per-cell error icons with tooltips.
- Integrating `IDataErrorInfo` / `IDXDataErrorInfo` from the data model.
- Validating inside the Edit Form.

## Validation Levels

| Level | Event | Fires | Best for |
|---|---|---|---|
| **Cell** | `ColumnView.ValidatingEditor` + `InvalidValueException` | Just before the cell's value is committed (focus leaves the cell or row). | Format and range checks on a single value. |
| **Row** | `ColumnView.ValidateRow` + `InvalidRowException` | When the user moves focus to a different row (after the row was modified). | Cross-cell business rules. |
| **Model** | `IDataErrorInfo` / `IDXDataErrorInfo` on the data class | Whenever the grid asks the row for errors. | Reusing existing model-level validation. |

The two levels coexist — cell validation prevents a single value from being saved; row validation prevents an entire row from being saved.

## Cell Validation

Handle `ValidatingEditor` to inspect the candidate value before it is written to the cell. Set `e.Valid = false` and provide `e.ErrorText` to mark it invalid. Handle `InvalidValueException` to control the UI response.

```csharp
gridView1.ValidatingEditor += (s, e) => {
    var view   = (ColumnView)s;
    var column = (e as EditFormValidateEditorEventArgs)?.Column ?? view.FocusedColumn;
    if (column.FieldName != "Budget") return;

    if (e.Value is null || Convert.ToInt32(e.Value) < 0 || Convert.ToInt32(e.Value) > 1_000_000) {
        e.Valid     = false;
        e.ErrorText = "Budget must be between 0 and 1,000,000.";
    }
};

gridView1.InvalidValueException += (s, e) => {
    var view = (ColumnView)s;
    e.ExceptionMode = ExceptionMode.DisplayError;
    e.WindowCaption = "Input error";
    e.ErrorText     = "Please enter a value between 0 and 1,000,000.";

    // Optional: discard the invalid value and close the editor.
    view.HideEditor();
};
```

The `EditFormValidateEditorEventArgs` cast handles the case where validation fires inside an Edit Form — the `Column` property of that args type identifies which field is being validated.

### `ExceptionMode`

| Value | Behavior |
|---|---|
| `DisplayError` *(default)* | Shows a modal message with `e.ErrorText`, keeps focus in the editor. |
| `NoAction` | Suppresses the message box; keep focus in the editor. Combine with `view.SetColumnError` to show an inline error icon. |
| `ThrowException` | Re-throws as an exception (rarely useful). |
| `Ignore` | Discards the change without complaining. |

### Inline error icon

```csharp
gridView1.ValidatingEditor += (s, e) => {
    var view = (ColumnView)s;
    if (view.FocusedColumn.FieldName == "Email" && !((string)e.Value).Contains('@')) {
        e.Valid = false;
        view.SetColumnError(view.FocusedColumn, "Invalid e-mail address.", ErrorType.Critical);
    } else view.ClearColumnErrors();
};
gridView1.InvalidValueException += (s, e) => e.ExceptionMode = ExceptionMode.NoAction;
```

`SetColumnError(column, text, ErrorType)` paints a red/yellow icon in the row indicator and tooltips the message on hover. Clear with `view.ClearColumnErrors()`.

## Row Validation

Use `ValidateRow` to check rules that span multiple cells. The grid blocks navigation to a new row until the user fixes the errors or presses `Esc` to cancel changes.

> Data objects **must implement `IEditableObject`** for `Esc` to discard changes. Plain POCOs accept the changes anyway.

```csharp
gridView1.ValidateRow += (s, e) => {
    var view = (ColumnView)s;
    var inStockCol  = view.Columns["UnitsInStock"];
    var onOrderCol  = view.Columns["UnitsOnOrder"];

    var inStock = Convert.ToInt32(view.GetRowCellValue(e.RowHandle, inStockCol));
    var onOrder = Convert.ToInt32(view.GetRowCellValue(e.RowHandle, onOrderCol));

    if (inStock < onOrder) {
        e.Valid = false;
        view.SetColumnError(inStockCol, "Stock must be ≥ Units on order.");
        view.SetColumnError(onOrderCol, "Units on order must be ≤ Stock.");
    } else view.ClearColumnErrors();
};
gridView1.InvalidRowException += (s, e) => e.ExceptionMode = ExceptionMode.NoAction;
```

### Allow committing invalid rows

Set `view.OptionsBehavior.AllowValidationErrors = true` to let users move away from an invalid row (the icon stays as a warning).

### Force a row commit

```csharp
if (view.UpdateCurrentRow())
    Console.WriteLine("Row committed successfully.");
else
    Console.WriteLine("Row failed validation.");
```

`UpdateCurrentRow` returns `true` if commit succeeded; `false` if validation failed.

## Edit Form Validation

When `EditingMode` is `EditForm` or `EditFormInplace`, `ValidatingEditor` fires for each editor inside the form. The args are `EditFormValidateEditorEventArgs` and expose `Column`. `ValidateRow` fires when the user clicks **Update** on the form.

```csharp
gridView1.ValidatingEditor += (s, e) => {
    if (e is EditFormValidateEditorEventArgs args && args.Column.FieldName == "Phone") {
        if (!IsValidPhone(e.Value as string)) {
            e.Valid = false;
            e.ErrorText = "Phone number must be in E.164 format.";
        }
    }
};
```

## `IDXDataErrorInfo` / `IDataErrorInfo`

Push validation into the data model:

```csharp
public class Customer : IDXDataErrorInfo {
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";

    public void GetError(ErrorInfo info) { /* row-level error */ }
    public void GetPropertyError(string member, ErrorInfo info) {
        if (member == nameof(Email) && !Email.Contains('@')) {
            info.ErrorType = ErrorType.Critical;
            info.ErrorText = "Invalid e-mail.";
        }
    }
}
```

The grid displays per-cell error icons automatically. To also block navigation, wire `ValidateRow` to query the model:

```csharp
gridView1.ValidateRow += (s, e) => {
    var info = new ErrorInfo();
    (e.Row as IDXDataErrorInfo)?.GetPropertyError("Email", info);
    e.Valid = info.ErrorType != ErrorType.Critical;
};
gridView1.InvalidRowException += (s, e) => e.ExceptionMode = ExceptionMode.NoAction;
```

Standard `IDataErrorInfo` is also recognized but does not differentiate `ErrorType` levels.

## Choose the Right Approach

```
single field check, immediate feedback         ─► ValidatingEditor
cross-cell rule, commit at row change          ─► ValidateRow
reuse existing model validation                 ─► IDXDataErrorInfo / IDataErrorInfo
inside an Edit Form                            ─► ValidatingEditor (via EditFormValidateEditorEventArgs)
                                                 + ValidateRow on submit
quiet UI with inline icons                     ─► SetColumnError + ExceptionMode.NoAction
loud modal dialogs                             ─► default ExceptionMode.DisplayError
```

## TreeList Validation

Same pattern but on `TreeList`:

- `TreeList.ValidatingEditor` + `TreeList.InvalidValueException`
- `TreeList.ValidateNode` + `TreeList.InvalidNodeException`
- `TreeList.SetColumnError(node, column, text, ErrorType)`

## Common Issues

- **`ValidateRow` does not fire**: the row was not modified or the data class lacks change-tracking. Set values via `SetRowCellValue` and ensure the source supports `IBindingList` notifications.
- **Modal dialog appears even though I set `ExceptionMode.NoAction`**: handler set the mode on the wrong event. Use `InvalidValueException` for cell errors and `InvalidRowException` for row errors.
- **Inline error icons disappear after refresh**: clear and re-set in `RowStyle` / `ValidateRow` — they reset when the row is repainted.
- **`Esc` does not undo changes**: implement `IEditableObject` on the row class (or use `DataRow`/`DataRowView` which already does).
- **Validation runs but the column index is `-1`**: cast args to `EditFormValidateEditorEventArgs` first when inside an Edit Form — the focused column lives on the args, not on the view.

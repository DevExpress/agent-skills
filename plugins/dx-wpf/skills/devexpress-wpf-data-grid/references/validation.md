# Validation — DevExpress WPF Data Grid

The Data Grid supports two classes of validation: **GridControl-level** (events / MVVM commands that fire on user edits) and **Data-level** (interfaces or DataAnnotations on the data class itself). Each works at the cell or row scope. Pick the right one based on when validation must run, where the rules live, and how rich the error UX must be.

## When to Use This Reference

Use this when you need to:

- Pick between cell, row, interface-based, and attribute-based validation
- Wire MVVM validation commands (`ValidateCellCommand` / `ValidateRowCommand`)
- Surface errors with custom messages, icons, and severity levels
- Validate existing data on load (not just on user input)
- Override the default error window or error tooltip
- Allow / prevent posting an invalid value to the data source
- Validate data in a `TreeListControl` (uses `ValidateNodeCommand` instead of `ValidateRowCommand`)

## Choose Your Validation Strategy

| Need | Use |
|---|---|
| Validate a single cell's new value while the user is editing it (e.g., "Discount must be ≤ 30%") | **`ValidateCellCommand`** (Grid Level) |
| Validate cross-field rules at row commit time (e.g., "StartDate must be < EndDate") | **`ValidateRowCommand`** (Grid Level) |
| Validate existing data when the grid loads (data may already be invalid before the user touches anything) | **Interface-based** (`IDXDataErrorInfo` / `IDataErrorInfo` / `INotifyDataErrorInfo`) |
| Validation rules belong with the data class (DTOs, EF entities) and apply across all UI | **Attribute-based** (DataAnnotations: `[Required]`, `[Range]`, `[StringLength]`, `[RegularExpression]`, `[CustomValidation]`) |
| Multiple validation styles in one grid | Combine them — they coexist. Attribute-based runs first, then events. |

> Event- and command-based validation methods are **primarily designed to validate user input as it is entered**. They do NOT run on existing data when the grid loads. To validate data on load, use **interface-based validation**.

Source: `articles/controls-and-libraries/data-grid/data-editing-and-validation/input-validation.md`.

## GridControl-Level: Cell Validation

Fires when a user edits a single cell and tries to move focus / commit.

### MVVM Command

```xaml
<dxg:TableView ValidateCellCommand="{Binding ValidateCellCommand}"/>
```

```csharp
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Xpf;

public class MainViewModel : ViewModelBase {
    [Command]
    public void ValidateCell(RowCellValidationArgs args) {
        if (args.FieldName != nameof(Product.UnitPrice) ||
            !((Product)args.Item).Discontinued) return;

        var oldPrice = (double)args.OldValue;
        var newPrice = Convert.ToDouble(args.NewValue);
        var discountPct = 100 - newPrice / oldPrice * 100;

        if (discountPct > 30) {
            args.Result = new ValidationErrorInfo(
                $"Discount cannot exceed 30%. Max price drop: {oldPrice * 0.7:c2}.",
                ValidationErrorType.Critical);
        }
    }
}
```

### `RowCellValidationArgs` Properties

| Property | Description |
|---|---|
| `Item` | The data row object being edited |
| `FieldName` | The column's `FieldName` |
| `OldValue` | Value before the edit |
| `NewValue` | Value the user typed / picked |
| `Result` | Set to a `ValidationErrorInfo` or `string` to flag invalid; leave `null` for valid |

### Event Alternative (Non-MVVM)

```xaml
<dxg:TableView ValidateCell="View_ValidateCell"/>
```

```csharp
void View_ValidateCell(object sender, GridCellValidationEventArgs e) {
    // Same logic as above; e.Value vs args.NewValue, e.Row vs args.Item
}
```

### `GridColumn.Validate` Event

Per-column validation event (alternative to view-level for single-column rules):

```xaml
<dxg:GridColumn FieldName="UnitPrice" Validate="UnitPrice_Validate"/>
```

### Validation in Code (Not Just UI)

By default the grid **does not** validate when you change a cell value programmatically (`SetCellValue` or direct property change). To validate code-driven changes too:

```xaml
<dxg:TableView AllowLeaveInvalidEditor="True"/>
```

> This flag also lets users tab away from an invalid editor (the error is shown but doesn't trap focus). Without it, the user can't leave an invalid cell until they fix or undo it.

Source: `articles/controls-and-libraries/data-grid/data-editing-and-validation/input-validation/cell-validation.md`.

## GridControl-Level: Row Validation

Fires when the user moves focus to another row, or `CommitEditing()` is called. Use for cross-field rules.

```xaml
<dxg:TableView ValidateRowCommand="{Binding ValidateRowCommand}"/>
```

```csharp
[Command]
public void ValidateRow(RowValidationArgs args) {
    args.Result = GetValidationErrorInfo((Task)args.Item);
}

static ValidationErrorInfo GetValidationErrorInfo(Task task) {
    if (task.StartDate > task.EndDate)
        return new ValidationErrorInfo(
            "The start date must be before the end date.",
            ValidationErrorType.Critical);

    if (string.IsNullOrEmpty(task.TaskName))
        return new ValidationErrorInfo("The task name cannot be empty.");

    return null;   // Valid
}
```

### `RowValidationArgs` Properties

| Property | Description |
|---|---|
| `Item` | The data row object being committed |
| `IsNewItem` | `true` for the New Item Row (a newly-added record being saved) |
| `Result` | Set to flag invalid; leave `null` for valid |

### TreeListControl Equivalent

`TreeListView` uses `ValidateNodeCommand` and `ValidateNode` event (not `ValidateRowCommand`):

```xaml
<dxg:TreeListView ValidateNodeCommand="{Binding ValidateNodeCommand}"/>
```

Source: `articles/controls-and-libraries/data-grid/data-editing-and-validation/input-validation/row-validation.md`.

## Error Display: `ValidationErrorInfo`

`ValidationErrorInfo` is the result type for `args.Result`. Construct it directly, or assign a plain `string` (auto-promoted to a `ValidationErrorInfo` with default error icon).

### Constructor Overloads

```csharp
new ValidationErrorInfo("Error message");
new ValidationErrorInfo("Error message", ValidationErrorType.Critical);
new ValidationErrorInfo("Error message", ValidationErrorType.Warning);
new ValidationErrorInfo("Error message", ValidationErrorType.Information);
new ValidationErrorInfo("Error message", ValidationErrorType.None);
```

### Visual Indicators

- **Cell errors**: error icon inside the cell, tooltip on hover. The icon style depends on `ValidationErrorType` (red ✕ for Critical, yellow ⚠ for Warning, etc.).
- **Row errors**: error icon to the left of the row, tooltip on hover.
- **Error window** (rows only): on row commit, a `MessageBox`-style dialog asking "Do you want to correct the value?". Yes → focus the row; No → discard changes (requires `IEditableObject` on the data class for true rollback).

## Customize Row Error Behavior — `InvalidRowExceptionCommand`

Customize what happens after `ValidateRowCommand` flags an error:

```xaml
<dxg:TableView ValidateRowCommand="{Binding ValidateRowCommand}"
               InvalidRowExceptionCommand="{Binding InvalidRowCommand}"/>
```

```csharp
[Command]
public void InvalidRow(InvalidRowExceptionArgs args) {
    args.ExceptionMode = ExceptionMode.NoAction;   // Suppress the error window
}
```

### `InvalidRowExceptionArgs` Properties

| Property | Description |
|---|---|
| `ExceptionMode` | `MessageBox` (default), `ThrowException`, `DisplayError` (icon only), `NoAction` |
| `ErrorText` | Extra text shown before the "Do you want to correct the value?" prompt |
| `WindowCaption` | Caption of the error window |

Use `NoAction` if you don't want to block the user from leaving an invalid row.

## Check Whether the Grid Has Errors

```csharp
// True if any cell / row has a current validation error that blocks commit
bool blocking = grid.View.HasValidationError;

// True if any data-source object has errors (per Data Level validation)
bool any = grid.View.HasErrors;
```

`HasValidationError` covers Grid Level (events / commands). `HasErrors` covers Data Level (interfaces / attributes).

For `HasErrors` to work, set `ErrorsWatchMode`:

```xaml
<dxg:TableView ErrorsWatchMode="WhenVisible"/>
```

> Verify exact `ErrorsWatchMode` enum values via DxDocs MCP if needed — typical values are `Always`, `WhenVisible`, `None`.

## Data Level: Interface-Based Validation

Best for validating **existing data on load**, not just user edits. Works on data classes that implement one of these interfaces.

### `IDXDataErrorInfo` (Recommended for DevExpress)

DevExpress's enhanced interface — supports error icons / types per-property.

```csharp
using DevExpress.XtraEditors.DXErrorProvider;

public class TaskItem : IDXDataErrorInfo {
    public string TaskName { get; set; } = "";
    public DateTime StartDate { get; set; } = DateTime.Today;
    public DateTime EndDate { get; set; } = DateTime.Today.AddDays(1);

    // Called per property — for cell-level errors
    public void GetPropertyError(string propertyName, ErrorInfo info) {
        switch (propertyName) {
            case nameof(TaskName):
                if (string.IsNullOrWhiteSpace(TaskName)) {
                    info.ErrorText = "The task name cannot be empty.";
                    info.ErrorType = ErrorType.Critical;
                }
                break;
            case nameof(StartDate):
            case nameof(EndDate):
                if (StartDate > EndDate) {
                    info.ErrorText = "The start date must be before the end date.";
                    info.ErrorType = ErrorType.Warning;
                }
                break;
        }
    }

    // Called once per record — for row-level errors
    public void GetError(ErrorInfo info) {
        if (string.IsNullOrWhiteSpace(TaskName)) {
            info.ErrorText = "The task name is required.";
            info.ErrorType = ErrorType.Critical;
        } else if (StartDate > EndDate) {
            info.ErrorText = "Invalid date range.";
            info.ErrorType = ErrorType.Warning;
        }
    }
}
```

`ErrorType` values: `None`, `Default` (= Critical), `Critical`, `Warning`, `Information`, `User1`–`User4` (custom).

### `IDataErrorInfo` (Standard .NET)

```csharp
public class TaskItem : IDataErrorInfo {
    public string TaskName { get; set; } = "";

    public string Error => string.IsNullOrWhiteSpace(TaskName) ? "Name is required" : "";

    public string this[string propertyName] => propertyName switch {
        nameof(TaskName) when string.IsNullOrWhiteSpace(TaskName) => "Name cannot be empty",
        _ => string.Empty
    };
}
```

Simpler but has no `ErrorType` — all errors show as Critical.

### `INotifyDataErrorInfo` (Async Validation)

```csharp
public class TaskItem : INotifyDataErrorInfo {
    // Supports asynchronous validation (e.g., server-side rules)
    // Implement ErrorsChanged event + GetErrors + HasErrors
}
```

> Verify exact pattern via DxDocs MCP if needed:
> `devexpress_docs_search(technology="WPF Data Grid", query="INotifyDataErrorInfo validation")`

Source: `articles/controls-and-libraries/data-grid/data-editing-and-validation/input-validation/interface-based-validation.md`.

## Data Level: Attribute-Based Validation (DataAnnotations)

Reference `System.ComponentModel.DataAnnotations`. Apply attributes to data class properties; the grid picks them up automatically.

### Supported Attributes

| Attribute | Effect |
|---|---|
| `[Required]` | Property must have a non-null / non-empty value |
| `[Range(min, max)]` | Numeric value must be within bounds |
| `[StringLength(max, MinimumLength = N)]` | String length constraints |
| `[RegularExpression("pattern")]` | String must match regex |
| `[DataType(DataType.EmailAddress)]` (or `PhoneNumber`, `Url`, etc.) | Built-in format validation |
| `[EnumDataType(typeof(MyEnum))]` | Value must exist in the enum |
| `[CustomValidation(typeof(C), nameof(C.M))]` | Calls a static method `C.M(value, ValidationContext)` |

### Example

```csharp
public class Product {
    [Required(ErrorMessage = "Product name is required.")]
    [StringLength(50)]
    public string ProductName { get; set; } = "";

    [Range(0, 10000, ErrorMessage = "Price must be 0–10,000.")]
    public decimal UnitPrice { get; set; }

    [RegularExpression(@"^[A-Z]{2,3}-\d{3,5}$")]
    public string Sku { get; set; } = "";
}
```

The grid:
- Displays an error icon on cells whose new value violates an attribute
- Tooltip shows `ErrorMessage`
- Blocks focus from leaving the cell until the user fixes the value or undoes

### Customize Attribute Behavior

| Property | Effect |
|---|---|
| `ColumnBase.ShowValidationAttributeErrors` | `False` to hide attribute errors on this column |
| `DataViewBase.ShowValidationAttributeErrors` | `False` to hide attribute errors view-wide |
| `DataViewBase.AllowCommitOnValidationAttributeError` | `True` to allow invalid values to post to the data source anyway (errors stay visible but don't block) |

### Limitations

- **Attribute-based validation does NOT work with `ColumnBase.Binding`** — only `FieldName`. (Because `Binding` columns are treated as unbound and the grid can't resolve which property's attributes apply.)
- Validation runs only on bound properties. Unbound columns (`UnboundDataType`) need event-based validation.

Source: `articles/controls-and-libraries/data-grid/data-editing-and-validation/input-validation/attribute-based-validation.md`.

## Validation in Edit Forms

[Edit Forms](cell-display-and-editing.md) honor all three validation tiers:

- DataAnnotations and interface-based errors show next to the form editors
- `ValidateRowCommand` runs on form save
- `args.Result` errors show as form-level tooltips

When you define a custom `EditFormTemplate`, the `RowEditStarted` event does not auto-fire — wire it via `EventToCommand` in the template root. See [cell-display-and-editing.md § Custom Form Template](cell-display-and-editing.md).

## CellTemplate Caveat

When a column uses a custom `CellTemplate`, validation works **only if** the template contains a `BaseEdit` descendant named `PART_Editor`:

```xaml
<dxg:GridColumn.CellTemplate>
    <DataTemplate>
        <dxe:TextEdit Name="PART_Editor"/>   <!-- Name="PART_Editor" is mandatory -->
    </DataTemplate>
</dxg:GridColumn.CellTemplate>
```

Without `PART_Editor`, the cell renders but validation events / icons don't connect.

## ValidationService

For non-grid scenarios (a `BaseEdit` inside any container), `DevExpress.Xpf.Editors.ValidationService` provides cross-control validation. The Grid hooks into it:

```xaml
<dxg:GridControl ValidationService.IsValidationContainer="True"/>
```

Use when you have a form mixing editors inside and outside the grid, and want a single validation pass to check all of them.

> Verify exact `ValidationService` API via DxDocs MCP if needed.

## Apply to TreeListControl

| GridControl member | TreeListControl equivalent |
|---|---|
| `GridViewBase.ValidateCell` event / `ValidateCellCommand` | `TreeListView.ValidateCell` / `ValidateCellCommand` |
| `GridViewBase.ValidateRow` event / `ValidateRowCommand` | `TreeListView.ValidateNode` event / **`ValidateNodeCommand`** |
| `GridViewBase.InvalidRowExceptionCommand` | `TreeListView.InvalidNodeExceptionCommand` (verify exact name) |
| `IDXDataErrorInfo`, `IDataErrorInfo`, DataAnnotations | identical |

The naming switches from "Row" to "Node" for tree-list. Source: `row-validation.md` table at lines 23-35.

## Validation Decision Matrix (Summary)

| Validation type | When it runs | Where rules live | Scope | Async support | Works with `Binding` columns |
|---|---|---|---|---|---|
| `ValidateCellCommand` | On cell commit | ViewModel command | Cell | No | Yes |
| `ValidateRowCommand` | On row commit | ViewModel command | Row | No | Yes |
| `IDXDataErrorInfo` | On data load + on PropertyChanged | Data class | Cell + Row | Implicit (sync) | Yes |
| `IDataErrorInfo` | On data load + on PropertyChanged | Data class | Cell + Row | No | Yes |
| `INotifyDataErrorInfo` | On data load + on `ErrorsChanged` | Data class | Cell + Row | **Yes** | Yes |
| DataAnnotations | On cell commit | Data class attributes | Cell | No | **No** (FieldName only) |

## Common Issues

- **Validation doesn't fire on programmatic changes** — set `AllowLeaveInvalidEditor="True"`.
- **Custom CellTemplate breaks validation** — name the inner editor `PART_Editor`.
- **DataAnnotations don't show up** — column uses `Binding` instead of `FieldName`. Switch to `FieldName`.
- **Error window is intrusive** — set `InvalidRowExceptionCommand` + `args.ExceptionMode = ExceptionMode.NoAction`.
- **Existing data shows as valid even though it isn't** — events fire only on edits; use interface-based validation for load-time errors.
- **User can leave an invalid cell** — `AllowLeaveInvalidEditor="False"` (default behavior traps focus) or `AllowCommitOnValidationAttributeError="False"` for attribute-based.

## Source Material

- `articles/controls-and-libraries/data-grid/data-editing-and-validation/input-validation.md`
- `articles/controls-and-libraries/data-grid/data-editing-and-validation/input-validation/cell-validation.md`
- `articles/controls-and-libraries/data-grid/data-editing-and-validation/input-validation/row-validation.md`
- `articles/controls-and-libraries/data-grid/data-editing-and-validation/input-validation/interface-based-validation.md`
- `articles/controls-and-libraries/data-grid/data-editing-and-validation/input-validation/attribute-based-validation.md`
- `articles/controls-and-libraries/data-grid/data-editing-and-validation/data-editing-and-validation.md`

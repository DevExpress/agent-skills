# Editing and Validation — DevExpress WPF Data Grid

The grid uses in-place editors (`BaseEdit` descendants) to edit cell values. Editor type is auto-selected by data type but can be customized per column. Validation runs at the cell or row level; in MVVM apps, validation is wired through commands.

## When to Use This Reference

Use this when you need to:

- Replace the default in-place editor with a `ComboBoxEdit`, `DateEdit`, `CheckEdit`, or any `BaseEdit` descendant
- Validate input at the cell or row level
- Wire MVVM CRUD commands (`ValidateRowCommand`, `ValidateRowDeletionCommand`)
- Handle clipboard paste
- Use `CellValueChanged` to react to edits

## Key Classes

| Class | Purpose |
|---|---|
| `DevExpress.Xpf.Grid.ColumnBase.EditSettings` | Settings object for the in-place editor of a column. |
| `DevExpress.Xpf.Editors.BaseEdit` | Base class of every DevExpress WPF editor. |
| `DevExpress.Xpf.Editors.TextEdit` | Default editor for strings / numbers. |
| `DevExpress.Xpf.Editors.CheckEdit` | Default editor for Boolean values. |
| `DevExpress.Xpf.Editors.DateEdit` | Default editor for dates. |
| `DevExpress.Xpf.Editors.ComboBoxEdit` | Drop-down editor; settings via `ComboBoxEditSettings`. |
| `DevExpress.Xpf.Editors.Settings.TextEditSettings` | Settings for `TextEdit`-based in-place editors. |
| `DevExpress.Xpf.Editors.Settings.ComboBoxEditSettings` | Settings for `ComboBoxEdit`-based in-place editors. |
| `DevExpress.Xpf.Editors.Settings.LookUpEditSettingsBase` | Base for combo / look-up editor settings. Owns `ItemsSource`, `DisplayMember`, `ValueMember`. |
| `DevExpress.Xpf.Grid.GridViewBase.CellValueChanged` | Event raised after a cell value changes. |
| `DevExpress.Xpf.Grid.DataViewBase.CommitEditing` | Commit the active editor. |
| `DevExpress.Mvvm.Xpf.RowValidationArgs` | MVVM row-validation arguments. |
| `DevExpress.Mvvm.Xpf.ValidateRowDeletionArgs` | MVVM row-deletion arguments. |

## Default Editors by Data Type

| Data Type | Default Editor |
|---|---|
| `string`, numeric | `TextEdit` |
| `DateTime`, `DateTime?`, `DateOnly` | `DateEdit` |
| `bool`, `bool?` | `CheckEdit` |
| `enum` | `ComboBoxEdit` populated with enum values |

Auto-selection happens when `EnableSmartColumnsGeneration="True"` on the grid (default for columns generated via `AutoGenerateColumns="AddNew"`).

## Custom Editor: ComboBox for a Foreign Key

```xaml
<UserControl xmlns:dxe="http://schemas.devexpress.com/winfx/2008/xaml/editors" ...>
    <dxg:GridColumn FieldName="ShipVia">
        <dxg:GridColumn.EditSettings>
            <dxe:ComboBoxEditSettings ItemsSource="{Binding Shippers}"
                                      DisplayMember="CompanyName"
                                      ValueMember="ShipperId"/>
        </dxg:GridColumn.EditSettings>
    </dxg:GridColumn>
</UserControl>
```

`Shippers` is a separate collection on the ViewModel. The grid stores `ShipperId` in the bound entity while displaying `CompanyName`.

Source: `articles/controls-and-libraries/data-grid/getting-started/code/lesson-2-display-and-edit-data.md`.

## Custom Editor: Currency Mask

```xaml
<dxg:GridColumn FieldName="Freight">
    <dxg:GridColumn.EditSettings>
        <dxe:TextEditSettings Mask="c"
                              MaskType="Numeric"
                              MaskUseAsDisplayFormat="True"/>
    </dxg:GridColumn.EditSettings>
</dxg:GridColumn>
```

`Mask="c"` is the predefined currency mask. Set `MaskUseAsDisplayFormat="True"` so the mask formats the value even when the cell is not in edit mode.

## Read-Only Column

```xaml
<dxg:GridColumn FieldName="OrderId" ReadOnly="True"/>
```

For grid-wide read-only:

```xaml
<dxg:GridControl ShowBorder="True">
    <dxg:GridControl.View>
        <dxg:TableView NavigationStyle="Cell" AllowEditing="False"/>
    </dxg:GridControl.View>
</dxg:GridControl>
```

## CRUD with MVVM Commands

```xaml
<dxg:TableView NewItemRowPosition="Top"
               ShowUpdateRowButtons="OnCellEditorOpen"
               ValidateRowCommand="{Binding ValidateRowCommand}"
               ValidateRowDeletionCommand="{Binding ValidateRowDeletionCommand}"
               DataSourceRefreshCommand="{Binding DataSourceRefreshCommand}"/>
```

```csharp
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Xpf;
using System.Linq;

public class MainViewModel : ViewModelBase {
    [Command]
    public void ValidateRow(RowValidationArgs args) {
        var item = (Order)args.Item;
        if (string.IsNullOrEmpty(item.ShipCity))
            args.SetError("Ship City is required");

        if (args.IsNewItem) _context.Orders.Add(item);
        _context.SaveChanges();
    }

    [Command]
    public void ValidateRowDeletion(ValidateRowDeletionArgs args) {
        var item = (Order)args.Items.Single();
        _context.Orders.Remove(item);
        _context.SaveChanges();
    }
}
```

Source: `articles/controls-and-libraries/data-grid/getting-started/designer/lesson-1-add-a-gridcontrol-to-a-project.md`.

> The `RowValidationArgs.SetError(string)` API surface above is paraphrased from common DevExpress patterns. If you need the exact signature, verify with:
> `devexpress_docs_search(technology="WPF Data Grid", query="RowValidationArgs SetError")`

## CellValueChanged Event

```xaml
<dxg:TableView CellValueChanged="View_CellValueChanged"/>
```

```csharp
private void View_CellValueChanged(object sender,
        DevExpress.Xpf.Grid.CellValueChangedEventArgs e) {
    Debug.WriteLine($"Cell [{e.Row}][{e.Column.FieldName}] = {e.Value}");
    view.CommitEditing(); // Optional: post the change immediately
}
```

`CellValueChangedEventArgs` exposes `Row`, `Column`, `Value`, `OldValue`. Source: `articles/controls-and-libraries/data-grid/data-summaries.md` shows the pattern in context.

## Clipboard Paste

Clipboard paste support is on by default. Configure it via the view:

```xaml
<dxg:TableView ClipboardCopyMode="IncludeHeader"
               AllowClipboardPaste="True"/>
```

See `articles/controls-and-libraries/data-grid/data-editing-and-validation/clipboard-management.md` for the full clipboard pipeline (paste validation, custom paste handlers).

## Validation Topics in This Repo

- `articles/controls-and-libraries/data-grid/data-editing-and-validation/input-validation/` — cell and row validation
- `articles/controls-and-libraries/data-grid/data-editing-and-validation/modify-cell-values/` — programmatic cell updates
- `articles/controls-and-libraries/data-grid/data-editing-and-validation/clipboard-management/`

## Source Material

- `articles/controls-and-libraries/data-grid/getting-started/code/lesson-2-display-and-edit-data.md`
- `articles/controls-and-libraries/data-grid/getting-started/designer/lesson-1-add-a-gridcontrol-to-a-project.md`
- `articles/controls-and-libraries/data-grid/data-editing-and-validation/`

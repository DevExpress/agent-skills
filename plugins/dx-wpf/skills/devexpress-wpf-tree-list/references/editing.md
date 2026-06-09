# Editing — DevExpress WPF TreeList

`TreeListControl` uses the **same in-place editor infrastructure as `GridControl`** — `EditSettings`, `CellDisplayTemplate` + `CellEditTemplate`, Edit Forms, Edit Entire Row, `CustomColumnDisplayText`, unbound columns, read-only patterns. This reference covers TreeList-specific quirks; for the full editing decision matrix (9 display techniques, when to use which), see [the data-grid skill's `references/cell-display-and-editing.md`](../../devexpress-wpf-data-grid/references/cell-display-and-editing.md).

For validation (which has a renamed command in TreeList — `ValidateNodeCommand` instead of `ValidateRowCommand`), see [validation.md](validation.md).

## When to Use This Reference

Use this when you need to:

- Replace the default in-place editor with a `ComboBoxEdit`, `DateEdit`, `CheckEdit`, or any `BaseEdit` descendant
- Show or hide the New Item Row (for adding new nodes)
- Use **Edit Forms** for richer node editing in a popup
- Handle `CellValueChanged` to react to edits
- Add and remove nodes programmatically (bound and unbound modes)
- Understand TreeList-specific editor activation differences

> For the full **cell display + edit decision matrix** (9 techniques × 4 dimensions of coverage), `CellDisplayTemplate` vs `CellEditTemplate` split, editor activation API, `EditFormShowMode` modes, **Edit Entire Row** vs **Edit Form** decision — see [the data-grid `cell-display-and-editing.md`](../../devexpress-wpf-data-grid/references/cell-display-and-editing.md). It applies identically to TreeList (`TreeListView` replaces `TableView`, `TreeListColumn` replaces `GridColumn`).

## Key Classes

| Class | Purpose |
|---|---|
| `DevExpress.Xpf.Grid.TreeListColumn.EditSettings` | Settings object for the in-place editor of a column (inherited from `ColumnBase.EditSettings`). |
| `DevExpress.Xpf.Editors.BaseEdit` | Base class of every DevExpress WPF editor. |
| `DevExpress.Xpf.Editors.TextEdit` | Default editor for strings / numbers. |
| `DevExpress.Xpf.Editors.CheckEdit` | Default editor for Boolean values. |
| `DevExpress.Xpf.Editors.DateEdit` | Default editor for dates. |
| `DevExpress.Xpf.Editors.ComboBoxEdit` | Drop-down editor; settings via `ComboBoxEditSettings`. |
| `DevExpress.Xpf.Editors.Settings.TextEditSettings` | Mask, mask type, display format. |
| `DevExpress.Xpf.Editors.Settings.ComboBoxEditSettings` | `ItemsSource`, `DisplayMember`, `ValueMember`. |
| `DevExpress.Xpf.Grid.TreeListView.CellValueChanged` | Event raised after a cell value changes (inherited from `GridViewBase`). |
| `DevExpress.Mvvm.Xpf.NodeValidationArgs` | MVVM node-validation arguments (`ValidateNodeCommand`). The TreeList-specific sibling of `RowValidationArgs`. |

## Default Editors by Data Type

The same auto-selection as `GridControl`:

| Data Type | Default Editor |
|---|---|
| `string`, numeric | `TextEdit` |
| `DateTime`, `DateTime?`, `DateOnly` | `DateEdit` |
| `bool`, `bool?` | `CheckEdit` |
| `enum` | `ComboBoxEdit` populated with enum values |

Auto-selection happens when `EnableSmartColumnsGeneration="True"` on the TreeListControl.

## Custom Editor: ComboBox

```xaml
<UserControl xmlns:dxe="http://schemas.devexpress.com/winfx/2008/xaml/editors" ...>
    <dxg:TreeListColumn FieldName="Department">
        <dxg:TreeListColumn.EditSettings>
            <dxe:ComboBoxEditSettings ItemsSource="{Binding Departments}"
                                      DisplayMember="Name"
                                      ValueMember="Id"/>
        </dxg:TreeListColumn.EditSettings>
    </dxg:TreeListColumn>
</UserControl>
```

## Custom Editor: Numeric Mask

```xaml
<dxg:TreeListColumn FieldName="Salary">
    <dxg:TreeListColumn.EditSettings>
        <dxe:TextEditSettings Mask="c"
                              MaskType="Numeric"
                              MaskUseAsDisplayFormat="True"/>
    </dxg:TreeListColumn.EditSettings>
</dxg:TreeListColumn>
```

## Read-Only Column

```xaml
<dxg:TreeListColumn FieldName="ID" ReadOnly="True"/>
```

Tree-wide read-only:

```xaml
<dxg:TreeListView AllowEditing="False"/>
```

## Add / Remove Nodes — Bound Mode

In self-referential or hierarchical bound mode, add / remove items in the underlying collection. The tree updates automatically if the collection raises `INotifyCollectionChanged` (e.g., `ObservableCollection<T>`).

```csharp
public ObservableCollection<Employee> Employees { get; }

void AddManager(Employee parent, string name) {
    Employees.Add(new Employee {
        ID = NextId(),
        ParentID = parent.ID,
        Name = name,
        Position = "Manager"
    });
}
```

For non-observable sources (`List<T>`), you must rebind the `ItemsSource` after a change. Source: `articles/controls-and-libraries/tree-list.md` references `Add and Remove Nodes` at https://docs.devexpress.com/content/WPF/6123?md=true.

## Add / Remove Nodes — Unbound Mode

```csharp
using DevExpress.Xpf.Grid;

var newNode = new TreeListNode(new ProjectObject { Name = "New Phase", Executor = "..." });
parentNode.Nodes.Add(newNode);

parentNode.Nodes.Remove(existingChild);
```

## New Item Row (Add New Node Via UI)

```xaml
<dxg:TreeListView NewItemRowPosition="Top"
                  ShowUpdateRowButtons="OnCellEditorOpen"/>
```

`NewItemRowPosition`: `None`, `Top`, `Bottom`. The new item row inserts at the level the user clicked.

## Edit Forms

`TreeListControl` supports Edit Forms (a popup form for editing a row instead of inline editing). See `articles/controls-and-libraries/data-grid/data-editing-and-validation/` (shared with GridControl) and https://docs.devexpress.com/content/WPF/403491?md=true.

```xaml
<dxg:TreeListView EditFormShowMode="InlineHidePreviousRow"
                  ShowEditForm="True"/>
```

> Verify exact property names (`EditFormShowMode` / `ShowEditForm`) for the TreeListView class via DxDocs MCP if they differ between versions:
> `devexpress_docs_search(technology="WPF TreeList", query="edit form properties")`

## Validation — Cell and Row

The cell- and row-validation pipeline is shared with GridControl. See https://docs.devexpress.com/content/WPF/7358?md=true in the docs.

```xaml
<dxg:TreeListView ValidateNodeCommand="{Binding ValidateNodeCommand}"
                  InvalidNodeExceptionCommand="{Binding InvalidNodeCommand}"/>
```

```csharp
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Xpf;
using System.Linq;

public class MainViewModel : ViewModelBase {
    [Command]
    public void ValidateNode(NodeValidationArgs args) {
        var item = (Employee)args.Item;
        if (string.IsNullOrWhiteSpace(item.Name))
            args.Result = "Name is required";
        // SaveChanges() here for DB-backed sources
    }

    [Command]
    public void InvalidNode(InvalidNodeExceptionArgs args) {
        // Handle validation exception (optional)
    }
}
```

## `CellValueChanged` Event

```xaml
<dxg:TreeListView CellValueChanged="View_CellValueChanged"/>
```

```csharp
private void View_CellValueChanged(object sender,
        DevExpress.Xpf.Grid.CellValueChangedEventArgs e) {
    System.Diagnostics.Debug.WriteLine(
        $"Cell [{e.Row}][{e.Column.FieldName}] = {e.Value}");
}
```

## Source Material

- `articles/controls-and-libraries/tree-list.md` (Edit Data section)
- `articles/controls-and-libraries/tree-list/end-user-capabilities/data-editing.md`
- `articles/controls-and-libraries/data-grid/data-editing-and-validation/` (shared validation pipeline)

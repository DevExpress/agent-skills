# Editors — List Editors

Code snippets for accessing, customizing, and implementing List Editors.

---

## Accessing List Editor Controls (Blazor)

```csharp
public class CustomizeGridController : ViewController<ListView> {
    protected override void OnViewControlsCreated() {
        base.OnViewControlsCreated();

        if (View.Editor is DxGridListEditor gridEditor) {
            gridEditor.GridModel.ColumnResizeMode = GridColumnResizeMode.ColumnsContainer;
            gridEditor.GridModel.ShowGroupPanel = false;

            foreach (DxGridColumnWrapper col in gridEditor.Columns) {
                col.MinWidth = 50;
                if (col.PropertyName == "Status") {
                    col.DxGridDataColumnModel.FilterMenuButtonDisplayMode =
                        GridFilterMenuButtonDisplayMode.Never;
                }
            }
        }
    }
}
```

### Freeze (Pin) a Column

Set `FixedPosition` on the `DxGridDataColumnModel` to anchor a column to the grid's left or right edge:

```csharp
if (View.Editor is DxGridListEditor gridEditor) {
    foreach (DxGridColumnWrapper col in gridEditor.Columns) {
        if (col.PropertyName == "OrderId") {
            col.DxGridDataColumnModel.FixedPosition = GridColumnFixedPosition.Left;
        }
    }
}
```

`GridColumnFixedPosition` values: `Left`, `Right`, `None` (default).

## Accessing List Editor Controls (WinForms)

```csharp
protected override void OnViewControlsCreated() {
    base.OnViewControlsCreated();

    if (View.Editor is GridListEditor winEditor) {
        GridView gridView = winEditor.GridView;
        gridView.OptionsView.ShowGroupPanel = false;
        foreach (WinGridColumnWrapper col in winEditor.Columns) {
            col.Column.OptionsColumn.AllowSort = DefaultBoolean.False;
        }
    }
}
```

## Platform-Agnostic List Editor Customization

### ColumnWrapper Properties

Use `ColumnWrapper` properties to configure columns in code. These work on both Blazor and WinForms:

```csharp
using DevExpress.ExpressApp.Editors; // ColumnsListEditor, ColumnWrapper, ColumnSortOrder

// Works on both Blazor and WinForms
if (View.Editor is ColumnsListEditor listEditor) {
    foreach (ColumnWrapper column in listEditor.Columns) {
        switch (column.PropertyName) {
            case nameof(Order.InternalNotes):
                column.Visible = false;        // hide from the grid
                break;
            case nameof(Order.OrderDate):
                column.SortOrder = ColumnSortOrder.Descending; // sort newest first
                column.Width = 120;
                break;
            case nameof(Order.Status):
                column.Caption = "Order Status"; // override display caption
                break;
        }
    }
}
```

| Property | Type | Description |
|----------|------|-------------|
| `Caption` | `string` | Column header text displayed in the grid |
| `Visible` | `bool` | Show or hide the column |
| `SortOrder` | `ColumnSortOrder` | `None`, `Ascending`, or `Descending` |
| `Width` | `int` | Column width in pixels |

### AddColumn — Programmatically Add a Column

```csharp
if (View.Editor is ColumnsListEditor listEditor) {
    IModelColumn modelColumn = ((ListView)View).Model.Columns["Priority"];
    ColumnWrapper newColumn = listEditor.AddColumn(modelColumn);
    newColumn.Caption = "Priority Level";
    newColumn.Width = 100;
}
```

`AddColumn` accepts an `IModelColumn` from the ListView model and returns a `ColumnWrapper` you can configure immediately.

### Summary & Footer

```csharp
// Works on both Blazor and WinForms
if (View.Editor is ColumnsListEditor listEditor) {
    listEditor.Model.IsFooterVisible = true;
    foreach (ColumnWrapper column in listEditor.Columns) {
        if (column.PropertyName == nameof(Paycheck.NetPay)) {
            column.AllowSummaryChange = false;
        }
    }
}
```

## Access the Component Instance (Blazor)

```csharp
// DxGridModel.ComponentInstance gives direct access to the DxGrid component
if (View.Editor is DxGridListEditor editor) {
    var gridInstance = editor.GridModel.ComponentInstance; // DxGrid
    // Use to call methods or read runtime state
}
```

## Implementing a Custom List Editor (Blazor)

```csharp
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Blazor;
using DevExpress.ExpressApp.Blazor.Components;
using Microsoft.AspNetCore.Components;

[ListEditor(typeof(IPictureItem))]
public class PictureListEditor : ListEditor, IComponentContentHolder {
    private RenderFragment componentContent;

    public PictureItemListViewModel ComponentModel { get; private set; }

    public RenderFragment ComponentContent {
        get {
            componentContent ??= ComponentModelObserver.Create(
                ComponentModel, ComponentModel.GetComponentContent());
            return componentContent;
        }
    }

    public PictureListEditor(IModelListView model) : base(model) { }

    protected override object CreateControlsCore() {
        ComponentModel = new PictureItemListViewModel();
        return ComponentModel;
    }

    protected override void AssignDataSourceToControl(object dataSource) {
        if (ComponentModel != null && dataSource is IEnumerable items) {
            ComponentModel.Data = items.Cast<IPictureItem>();
        }
    }

    // Override SelectionType, GetSelectedObjects(), OnSelectionChanged()
    // as needed for your use case
}
```

Registration: `[ListEditor(typeof(IMyInterface))]` makes the editor default for that type.

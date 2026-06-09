# View Items & UI Controls — Code Snippets

**Important:** `FindItem` and `GetItems` must be called in or after `OnViewControlsCreated` — controls do not exist during `OnActivated`. However, `CustomizeViewItemControl` handles deferred control creation internally and should be called from `OnActivated` (the XAF-recommended pattern for Property Editor customization).

## FindItem (Get Property Editor by Name)

```csharp
public class MyDetailController : ViewController<DetailView> {
    protected override void OnViewControlsCreated() {
        base.OnViewControlsCreated();
        var nameEditor = View.FindItem("Name") as PropertyEditor;
        if (nameEditor != null) {
            nameEditor.ValueChanged += (s, e) => {
                // React to value change
            };
        }
    }
}
```

## GetItems (All Items of a Type)

```csharp
protected override void OnViewControlsCreated() {
    base.OnViewControlsCreated();
    var allPropertyEditors = View.GetItems<PropertyEditor>();
    var allViewItems = View.GetItems<ViewItem>();
}
```

## CustomizeViewItemControl (Access Underlying Control)

`CustomizeViewItemControl<T>` is an extension method (from `DetailViewExtensions`) that accepts a lambda receiving the typed view item directly. The extension method defers until controls are created, so it can be called from `OnActivated`.

```csharp
// Blazor — customize a StringPropertyEditor's component model
public class CustomizeEditorController : ViewController<DetailView> {
    protected override void OnActivated() {
        base.OnActivated();
        View.CustomizeViewItemControl<StringPropertyEditor>(this, editor => {
            // editor.ComponentModel (Blazor) or editor.Control (WinForms)
            if (editor.ComponentModel is DxTextBoxModel model) {
                model.CssClass += " my-custom-class";
            }
        });
    }
}
```

## Access List Editor's Control (OnViewControlsCreated)

```csharp
public class CustomizeGridController : ViewController<ListView> {
    protected override void OnViewControlsCreated() {
        base.OnViewControlsCreated();
        // Blazor
        if (View.Editor is DxGridListEditor gridEditor) {
            gridEditor.GridModel.ShowGroupPanel = false;
            gridEditor.GridModel.PagerVisible = true;
        }
        // WinForms
        if (View.Editor is GridListEditor winGridEditor) {
            winGridEditor.GridView.OptionsView.ShowGroupPanel = false;
        }
    }
}
```

## Access Nested ListView (ListPropertyEditor)

```csharp
public class NestedListController : ViewController<DetailView> {
    protected override void OnViewControlsCreated() {
        base.OnViewControlsCreated();
        var ordersEditor = View.FindItem("Orders") as ListPropertyEditor;
        if (ordersEditor?.ListView != null) {
            // Access the nested ListView
            ordersEditor.ListView.CollectionSource.Criteria["PendingOnly"] =
                CriteriaOperator.FromLambda<Order>(o => o.Status == "Pending");

            // Access the nested grid editor
            if (ordersEditor.ListView.Editor is DxGridListEditor gridEditor) {
                // Customize nested grid
            }
        }
    }
}
```

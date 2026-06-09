# Editors — Customizing Built-in Property Editors

Code snippets for CustomizeViewItemControl, read-only/hidden properties, custom buttons, and lookup button visibility.

---

## CustomizeViewItemControl (Blazor)

```csharp
// Customize DateTimePropertyEditor to show time
public class CustomizeDateEditorController : ViewController<DetailView> {
    protected override void OnActivated() {
        base.OnActivated();
        View.CustomizeViewItemControl<DateTimePropertyEditor>(this, editor => {
            editor.ComponentModel.TimeSectionVisible = true;
        });
    }
}
```

## CustomizeViewItemControl (WinForms)

```csharp
public class WinDateEditController : ObjectViewController<DetailView, Contact> {
    protected override void OnActivated() {
        base.OnActivated();
        View.CustomizeViewItemControl<DatePropertyEditor>(this, editor => {
            DateEdit dateEdit = editor.Control;
            dateEdit.Properties.CalendarView = CalendarView.TouchUI;
        }, nameof(Contact.Birthday));
    }
}
```

## Target a Specific Property

Pass property names to `CustomizeViewItemControl`:

```csharp
View.CustomizeViewItemControl<StringPropertyEditor>(this, editor => {
    // Customize only the "Name" property editor
}, nameof(Contact.Name), nameof(Contact.Email));
```

## Make Property Read-Only or Hidden

```csharp
// In OnActivated:
var editor = View.FindItem("SpouseName") as PropertyEditor;
if (editor != null) {
    editor.AllowEdit["MyReason"] = false; // Read-only
}

// Hide via Visibility:
var item = View.FindItem("InternalNotes") as ViewItem;
if (item != null) {
    item.Visibility = ViewItemVisibility.Hide;
}
```

## Adding Custom Buttons to Property Editors (Blazor)

```csharp
public class PhoneButtonController : ObjectViewController<DetailView, Contact> {
    protected override void OnActivated() {
        base.OnActivated();
        View.CustomizeViewItemControl<StringPropertyEditor>(this, editor => {
            var button = new DxEditorButtonModel {
                IconCssClass = "fluent-icon fluent-icon-phone",
                Tooltip = "Call this number",
                Click = EventCallback.Factory.Create<MouseEventArgs>(this,
                    e => Application.ShowViewStrategy.ShowMessage(
                        $"Calling {ViewCurrentObject.Phone}..."))
            };
            editor.Buttons.Add(button);
        }, nameof(Contact.Phone));
    }
}
```

Supported editors: `StringPropertyEditor`, `NumericPropertyEditor`, `DateTimePropertyEditor`, `LookupPropertyEditor`, `EnumPropertyEditor`, `BooleanPropertyEditor`, `DxFileDataPropertyEditor`, `TypePropertyEditor`.

## Lookup Property Editor — Button Visibility (Blazor)

```csharp
View.CustomizeViewItemControl<LookupPropertyEditor>(this, editor => {
    editor.HideNewButton();    // Hide "New" button
    editor.HideEditButton();   // Hide "Edit" button
    // editor.ResetNewButtonVisibility();  // Show again
    // editor.ResetEditButtonVisibility();
});
```

## IComplexViewItem — Access ObjectSpace & Application

When a custom Property Editor or ViewItem needs to query data:

```csharp
[PropertyEditor(typeof(string), false)]
public class DataAwareEditor : BlazorPropertyEditorBase, IComplexViewItem {
    private IObjectSpace objectSpace;
    private XafApplication application;

    public DataAwareEditor(Type objectType, IModelMemberViewItem model)
        : base(objectType, model) { }

    void IComplexViewItem.Setup(IObjectSpace objectSpace, XafApplication application) {
        this.objectSpace = objectSpace;
        this.application = application;
    }

    // Use objectSpace / application in CreateComponentModel, ReadValueCore, etc.
}
```

### XPO Session Access in Editors

When the project uses XPO, the `objectSpace` received in `Setup` can be cast to `XPObjectSpace` to access the underlying `Session` for XPO-specific queries:

```csharp
void IComplexViewItem.Setup(IObjectSpace objectSpace, XafApplication application) {
    this.objectSpace = objectSpace;
    this.application = application;

    if (objectSpace is XPObjectSpace xpObjectSpace) {
        Session session = xpObjectSpace.Session;
        // XPO-specific query, e.g.:
        var items = new XPQuery<Category>(session).Where(c => c.IsActive).ToList();
    }
}
```

> **Caution:** Never create a new `Session` or `UnitOfWork` inside an editor. Always use the `Session` from the provided `ObjectSpace` to stay within the same transaction and data context.

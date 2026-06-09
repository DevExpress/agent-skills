# Editors — Implementing Custom Property Editors

Code snippets for building custom Property Editors on Blazor and WinForms.

---

## Blazor — ComponentModel Pattern (Full)

```csharp
// 1. Component Model — object representation of the Razor component
using DevExpress.ExpressApp.Blazor.Components.Models;

public class InputTextModel : ComponentModelBase {
    public string Value {
        get => GetPropertyValue<string>();
        set => SetPropertyValue(value);
    }
    public EventCallback<string> ValueChanged {
        get => GetPropertyValue<EventCallback<string>>();
        set => SetPropertyValue(value);
    }
    public Expression<Func<string>> ValueExpression {
        get => GetPropertyValue<Expression<Func<string>>>();
        set => SetPropertyValue(value);
    }
    public override Type ComponentType => typeof(InputText);
}

// 2. Property Editor
using DevExpress.ExpressApp.Blazor.Editors;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Model;
using Microsoft.AspNetCore.Components;

[PropertyEditor(typeof(string), false)] // false = not default for type
public class CustomStringPropertyEditor : BlazorPropertyEditorBase {
    // PropertyValue is NOT available here — the editor is not yet bound to a data object.
    // Only initialise non-data state (helper objects, flags). PropertyValue is first
    // available inside CreateComponentModel() and ReadValueCore().
    public CustomStringPropertyEditor(Type objectType, IModelMemberViewItem model)
        : base(objectType, model) { }

    public override InputTextModel ComponentModel => (InputTextModel)base.ComponentModel;

    protected override IComponentModel CreateComponentModel() {
        var model = new InputTextModel();
        model.ValueExpression = () => model.Value;
        model.ValueChanged = EventCallback.Factory.Create<string>(this, value => {
            model.Value = value;
            OnControlValueChanged();
            WriteValue();
        });
        return model;
    }

    protected override void ReadValueCore() {
        base.ReadValueCore();
        ComponentModel.Value = (string)PropertyValue;
    }

    protected override object GetControlValueCore() => ComponentModel.Value;

    protected override void ApplyReadOnly() {
        base.ApplyReadOnly();
        ComponentModel?.SetAttribute("readonly", !AllowEdit);
    }
}
```

## Blazor — Simplified Variant (Wrapping DX Component)

```csharp
[PropertyEditor(typeof(string), false)]
public class CustomStringPropertyEditor : BlazorPropertyEditorBase {
    public CustomStringPropertyEditor(Type objectType, IModelMemberViewItem model)
        : base(objectType, model) { }
    public override DxTextBoxModel ComponentModel => (DxTextBoxModel)base.ComponentModel;
    protected override IComponentModel CreateComponentModel() => new DxTextBoxModel();
}
```

## Blazor — Display in List View Cells

Override `CreateViewComponentCore` to control grid cell rendering:

```csharp
protected override RenderFragment CreateViewComponentCore(object dataContext) {
    var componentModel = new InputTextModel();
    componentModel.Value = (string)this.GetPropertyValue(dataContext);
    componentModel.ValueExpression = () => componentModel.Value;
    componentModel.SetAttribute("readonly", true);
    return componentModel.GetComponentContent();
}
```

## WinForms — CreateControlCore + IInplaceEditSupport

```csharp
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Win.Editors;
using DevExpress.ExpressApp.Model;
using DevExpress.XtraEditors.Repository;

[PropertyEditor(typeof(int), false)]
public class CustomIntegerEditor : PropertyEditor, IInplaceEditSupport {
    private NumericUpDown control;

    // PropertyValue is NOT available here — the editor is not yet bound to a data object.
    // Only initialise non-data state. PropertyValue is first available in ReadValueCore().
    public CustomIntegerEditor(Type objectType, IModelMemberViewItem info)
        : base(objectType, info) { }

    protected override object CreateControlCore() {
        control = new NumericUpDown { Minimum = 0, Maximum = 100 };
        control.ValueChanged += (s, e) => {
            if (!IsValueReading) {
                OnControlValueChanged();
                WriteValueCore();
            }
        };
        return control;
    }

    protected override void ReadValueCore() {
        if (control != null && CurrentObject != null)
            control.Value = (int)PropertyValue;
    }

    protected override object GetControlValueCore() => (int)(control?.Value ?? 0);
```

> **`PropertyValue` vs `MemberInfo.GetValue`:** `PropertyValue` is the pre-processed value from XAF's conversion pipeline (formatting, null-handling, currency conversion). Always use `PropertyValue` in `ReadValueCore()` — never call `MemberInfo.GetValue(CurrentObject)` directly, as that bypasses the pipeline. Conversely, `GetControlValueCore()` must return the raw control value so XAF can assign it through `PropertyValue`'s setter — do not call `MemberInfo.SetValue()` inside `GetControlValueCore()`.

```csharp
    // IInplaceEditSupport — for editable List View columns
    RepositoryItem IInplaceEditSupport.CreateRepositoryItem() {
        return new RepositoryItemSpinEdit { MinValue = 0, MaxValue = 100 };
    }

    protected override void Dispose(bool disposing) {
        control = null;
        base.Dispose(disposing);
    }
}
```

## WinForms — Inheriting a Built-in Editor (DXPropertyEditor)

```csharp
[PropertyEditor(typeof(DateTime), false)]
public class CustomDateTimeEditor : DatePropertyEditor {
    public CustomDateTimeEditor(Type objectType, IModelMemberViewItem info)
        : base(objectType, info) { }

    protected override void SetupRepositoryItem(RepositoryItem item) {
        base.SetupRepositoryItem(item);
        var dateProps = (RepositoryItemDateTimeEdit)item;
        dateProps.CalendarTimeEditing = DefaultBoolean.True;
        dateProps.CalendarView = CalendarView.Vista;
    }
}
```

## PropertyEditorAttribute — Registration

```csharp
// Parameters: (Type propertyType, bool isDefaultForType)
[PropertyEditor(typeof(string), true)]   // Default for ALL string properties
[PropertyEditor(typeof(string), false)]  // Available but not default — activate via EditorAlias
```

### Opt-in Activation with EditorAlias

```csharp
// Editor class — registered with isDefault = false
[PropertyEditor(typeof(string), "MyRichTextEditorAlias", false)]
public class MyRichTextEditor : BlazorPropertyEditorBase { /* ... */ }

// Business class — property opts in to the custom editor
public class Customer : BaseObject {
    [EditorAlias("MyRichTextEditorAlias")]
    public virtual string Notes { get; set; }

    // This property keeps the default built-in string editor
    public virtual string Code { get; set; }
}
```

Place custom editors in:
- **Blazor**: `MySolution.Blazor.Server/Editors/`
- **WinForms**: `MySolution.Win/Editors/`
- **Cross-platform**: `MySolution.Module/` (platform-independent `PropertyEditor` descendants only)

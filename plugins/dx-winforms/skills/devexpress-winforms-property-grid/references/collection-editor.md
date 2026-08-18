# Editing Collection Properties

When a property's type is a collection (e.g., `List<T>`, `IList`, `ObservableCollection<T>`), the Property Grid shows an ellipsis (`…`) button in the editor cell. Clicking it opens a **collection editor dialog** where users can add, remove, and edit items.

## When to Use This Reference

- Letting users edit a collection property (`List<T>`, `IList`, …) through a dialog
- Switching to the DevExpress collection editor globally (`OptionsCollectionEditor.UseDXCollectionEditor`)
- Registering a custom editor per collection type (`DefaultCollectionEditors`)
- Overriding the editor for one property via the `[Editor]` attribute
- Implementing a custom `UITypeEditor` for full dialog control

There are three levels of control over which editor opens:

1. **Switch globally to DevExpress editor** (drop-in improvement, no code on the model)
2. **Register a custom editor per collection type** (`DefaultCollectionEditors`)
3. **Attribute on the property** (`[Editor]`) — highest priority, overrides the above

---

## 1. Use DevExpress Collection Editor (Global Opt-in)

By default, the Property Grid uses the standard WinForms `CollectionEditor` dialog. To switch to the DevExpress skinnable collection editor:

```csharp
propertyGridControl1.OptionsCollectionEditor.UseDXCollectionEditor = true;
```

The DevExpress editor also supports:

```csharp
var opts = propertyGridControl1.OptionsCollectionEditor;
opts.UseDXCollectionEditor = true;
opts.AllowSearch           = true;   // filter items by text
opts.AllowMultiSelect      = true;   // Ctrl/Shift multi-select
```

This setting applies to every collection-typed property displayed in the control.

---

## 2. Per-Type Custom Collection Editor

`DefaultCollectionEditors` is a dictionary that maps a collection type to a factory delegate returning a `UITypeEditor`. Use this when a specific collection type needs a special editor.

```csharp
// Register the standard CollectionEditor for List<MyItem>
propertyGridControl1.DefaultCollectionEditors.Add(
    typeof(List<MyItem>),
    (t) => new System.ComponentModel.Design.CollectionEditor(t)
);

// Register a completely custom editor for TagCollection
propertyGridControl1.DefaultCollectionEditors.Add(
    typeof(TagCollection),
    (t) => new TagCollectionEditor(t)
);
```

The `TagCollectionEditor` would inherit from `UITypeEditor` or `CollectionEditor` and override `EditValue`.

---

## 3. Attribute on the Property (Highest Priority)

Apply `[Editor]` directly to the property on your model class. This overrides both `UseDXCollectionEditor` and `DefaultCollectionEditors` for that specific property.

```csharp
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;

public class AppSettings {
    // Standard .NET collection editor for this specific property
    [Editor(typeof(CollectionEditor), typeof(UITypeEditor))]
    public List<string> AllowedPaths { get; set; } = new List<string>();

    // Custom editor for another collection property
    [Editor(typeof(ColorListEditor), typeof(UITypeEditor))]
    public List<Color> ThemeColors { get; set; } = new List<Color>();
}
```

---

## Collection Editor Priority

```
[Editor] attribute on property        ← highest priority
    ↓ (not set)
DefaultCollectionEditors[typeof(T)]   ← registered per-type
    ↓ (not registered)
OptionsCollectionEditor               ← global (DX or standard WinForms)
```

---

## Making Collection Items Editable

For the collection editor to show and edit item properties, each item type should have public settable properties. Apply `[DisplayName]`, `[Category]`, and `[Description]` to guide the editor's UI:

```csharp
public class ServerConfig {
    [DisplayName("Host Name")]
    public string Host { get; set; } = "localhost";

    [DisplayName("Port")]
    public int Port { get; set; } = 8080;

    [DisplayName("Use SSL")]
    public bool UseSsl { get; set; }

    // Used as the item label in the collection editor list
    public override string ToString() => $"{Host}:{Port}";
}

public class AppSettings {
    [DisplayName("Server Endpoints")]
    public List<ServerConfig> Servers { get; set; } = new();
}
```

The `ToString()` override controls how items appear in the left panel of the collection editor.

---

## Custom UITypeEditor for Full Control

Inherit `UITypeEditor` for complete control over the editing UI (opens any dialog):

```csharp
public class TagCollectionEditor : UITypeEditor {
    public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        => UITypeEditorEditStyle.Modal;

    public override object EditValue(
            ITypeDescriptorContext context,
            IServiceProvider provider,
            object value) {
        using var dlg = new TagEditorForm((List<string>)value);
        if (dlg.ShowDialog() == DialogResult.OK)
            return dlg.Result;
        return value;
    }
}
```

Apply as an attribute or register in `DefaultCollectionEditors`:

```csharp
propertyGridControl1.DefaultCollectionEditors.Add(
    typeof(List<string>),
    (t) => new TagCollectionEditor()
);
```

---

## Source Material

- `OptionsCollectionEditor`: `https://docs.devexpress.com/content/WindowsForms/DevExpress.XtraVerticalGrid.PropertyGridControl.OptionsCollectionEditor?md=true`
- `DefaultCollectionEditors`: `https://docs.devexpress.com/content/WindowsForms/DevExpress.XtraVerticalGrid.PropertyGridControl.DefaultCollectionEditors?md=true`

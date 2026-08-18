# Getting Started with DevExpress WinForms PropertyGridControl

## Overview

`PropertyGridControl` displays and edits public properties of any object — similar to Visual Studio's Properties window or settings panels in Microsoft Office. It supports two visual styles, automatic row generation from reflection, and rich customization via events and attributes.

---

## NuGet Package and Assembly

| Package | Assembly |
|---|---|
| `DevExpress.Win.VerticalGrid` | `DevExpress.XtraVerticalGrid.v26.1.dll` |
| `DevExpress.Win.Navigation` | (alternative, includes VerticalGrid) |

Install with `dotnet add package` (requires a DevExpress license):

```
DevExpress.Win.VerticalGrid
```

---

## Namespace

```csharp
using DevExpress.XtraVerticalGrid;
using DevExpress.XtraVerticalGrid.Rows;
using DevExpress.XtraVerticalGrid.Events;
```

---

## Add to a Form

1. Add a `PropertyGridControl` to an `XtraForm` (or `RibbonForm`).
2. Set `Dock = DockStyle.Fill` (or desired layout).
3. Assign an object to display:

```csharp
propertyGridControl1.SelectedObject = myObject;
```

The control immediately reflects on `myObject`, creates one row per public property, and groups them by `[Category]` attribute.

**Multiple objects at once** (shows only shared properties):

```csharp
propertyGridControl1.SelectedObjects = new object[] { obj1, obj2, obj3 };
```

---

## Visual Views

| View | Description | Set |
|---|---|---|
| **Classic** (default) | Visual Studio-inspired; supports `MultiEditorRow` | `ActiveViewType = PropertyGridView.Classic` |
| **Office** | Office-inspired; supports tabs, track bars, property markers | `ActiveViewType = PropertyGridView.Office` |

```csharp
propertyGridControl1.ActiveViewType = DevExpress.XtraVerticalGrid.PropertyGridView.Office;
```

---

## Row Generation

**Automatic (runtime)** — leave `Rows` collection empty; the control generates rows when you set `SelectedObject`:

```csharp
propertyGridControl1.SelectedObject = mySettings;
// Rows auto-created from public properties of mySettings
```

**Manual** — add `EditorRow` objects before setting `SelectedObject`. If the `Rows` collection is non-empty when `SelectedObject` is assigned, the control uses existing rows instead of auto-generating them.

```csharp
var row = new EditorRow();
row.Properties.FieldName = "Name";
row.Properties.Caption   = "Full Name";
propertyGridControl1.Rows.Add(row);
propertyGridControl1.SelectedObject = myObject;
```

---

## Minimal Working Example

```csharp
public partial class SettingsForm : XtraForm {
    public SettingsForm() {
        InitializeComponent();
        propertyGridControl1.SelectedObject = new AppSettings();
    }
}

public class AppSettings {
    [Category("General")]
    [DisplayName("Application Title")]
    [Description("Title shown in the main window.")]
    public string Title { get; set; } = "My App";

    [Category("General")]
    public bool StartMinimized { get; set; }

    [Category("Performance")]
    [Description("Maximum number of worker threads.")]
    public int MaxThreads { get; set; } = 4;
}
```

Result: two category groups ("General", "Performance"), each with their respective property rows.

---

## Companion Control: PropertyDescriptionControl

Displays the `[Description]` of the focused property below the grid:

```csharp
propertyDescriptionControl1.PropertyGrid = propertyGridControl1;
// AutoHeight = true makes the control resize to fit description text
propertyDescriptionControl1.AutoHeight = true;
```

---

## Source Material

- Property Grid overview: `https://docs.devexpress.com/content/WindowsForms/119885?md=true`
- `PropertyGridControl` class: `https://docs.devexpress.com/content/WindowsForms/DevExpress.XtraVerticalGrid.PropertyGridControl?md=true`
- Office View: `https://docs.devexpress.com/content/WindowsForms/120262?md=true`
- Classic View: `https://docs.devexpress.com/content/WindowsForms/401865?md=true`

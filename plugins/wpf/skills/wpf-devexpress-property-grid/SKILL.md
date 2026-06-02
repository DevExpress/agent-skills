---
name: wpf-devexpress-property-grid
description: Build WPF property editors with the DevExpress PropertyGridControl — bind any object via SelectedObject (or multiple via SelectedObjects), define which properties show via PropertyDefinition, edit collections inline via CollectionDefinition (with NewItemInitializer for add/remove), group properties via [Category] attribute and CategoryDefinition (flat / Visible / Tabbed modes), and control expandability of nested objects via TypeConverter / ExpandableObjectConverter / AllowExpanding. Use when building visual designers, settings dialogs, diagram property panels, report designers, or any view that lets a user inspect and edit an arbitrary object's properties at runtime. Also use when someone mentions "PropertyGridControl", "PropertyDefinition", "CollectionDefinition", "CategoryDefinition", "ShowCategories", "ShowProperties", "ExpandableObjectConverter", "AllowExpanding", or "dxprg:". Covers .NET (6/7/8+) and .NET Framework 4.6.2+.
compatibility: Requires .NET 6+ or .NET Framework 4.6.2+ targeting Windows. NuGet package `DevExpress.Wpf.PropertyGrid` (or assembly reference `DevExpress.Xpf.PropertyGrid.v26.1.dll`). DevExpress NuGet packages are published on nuget.org and via the local Unified Component Installer feed. A valid DevExpress license is required.
metadata:
  author: DevExpress
  version: "26.1"
  source-commit: b16066c633b85ee391d1e6188ffc4cd815ee5e8f
---

# DevExpress WPF Property Grid

`PropertyGridControl` is a data-bound control that displays and edits the properties of an arbitrary object. Set `SelectedObject` to any `INotifyPropertyChanged`/POCO object, optionally describe which properties to show and how with `PropertyDefinition` / `CollectionDefinition` / `CategoryDefinition`, and the grid renders a correct editor per type, supports collection add/remove, multi-object editing, categories (flat / grouped / tabbed), and nested-object expansion.

This skill covers project setup, defining `PropertyDefinition` objects, editing collection properties via `CollectionDefinition`, grouping via property categories, and controlling expandability of complex/nested properties.

## When to Use This Skill

Use this skill when you need to:

- Bind a `PropertyGridControl` to any CLR object and render an editor per property
- Choose which properties show — and assign custom editors per property/type via `PropertyDefinition`
- Let users add / edit / remove items in a collection property inline via `CollectionDefinition`
- Provide default values for new collection items (`NewItemInitializer`)
- Group properties under category rows or category tabs (`[Category]` attribute + `ShowCategories`)
- Make a nested object expandable (or block expansion) via `[TypeConverter]` / `AllowExpanding`

## Prerequisites & Installation

### NuGet Packages

| Package | Provides |
|---------|---------|
| `DevExpress.Wpf.PropertyGrid` | `PropertyGridControl`, `PropertyDefinition`, `CollectionDefinition`, `CategoryDefinition` |

```bash
dotnet add package DevExpress.Wpf.PropertyGrid
```

`DevExpress.Wpf.PropertyGrid` transitively pulls `DevExpress.Wpf.Core` and `DevExpress.Wpf.Grid.Core` (the grid engine that renders rows).

A valid DevExpress license is required. All DevExpress packages in a project must share the same version.

## XAML Namespaces

```xml
xmlns:dxprg="http://schemas.devexpress.com/winfx/2008/xaml/propertygrid"
xmlns:dxe="http://schemas.devexpress.com/winfx/2008/xaml/editors"
xmlns:dx="http://schemas.devexpress.com/winfx/2008/xaml/core"
xmlns:sys="clr-namespace:System;assembly=mscorlib"
```

| Prefix | Use for |
|---|---|
| `dxprg:` | `PropertyGridControl`, `PropertyDefinition`, `CollectionDefinition`, `CategoryDefinition`, `CellEditorPresenter`, `XamlInitializer` |
| `dxe:` | Editor settings (`TextEditSettings`, `DateEditSettings`, `ComboBoxEditSettings`, ...) |
| `dx:` | `DXImage`, common core primitives |
| `sys:` | Built-in CLR types (`sys:String`, `sys:DateTime`) used with `PropertyDefinition Type="..."` |

## Before You Start — Ask the Developer

1. **What object** is being edited — a single POCO, an `INotifyPropertyChanged` view model, or multiple objects (`SelectedObjects`)?
2. **Show all properties or only declared ones?** — `ShowProperties="All"` displays everything by default; `WithPropertyDefinitions` only shows what you explicitly define.
3. **Need categories?** — If properties carry `[Category]` attributes or you want a tabbed/grouped layout, see [categories.md](references/categories.md).
4. **Need collection editing inline?** — If the bound object has a list/observable collection that users should add/remove items to, see [collection-definitions.md](references/collection-definitions.md).
5. **Need nested-object expansion?** — Complex properties (an `Address` inside a `Customer`) need either `[TypeConverter(typeof(ExpandableObjectConverter))]` or `AllowExpanding="Force"` — see [expandability.md](references/expandability.md).

## Documentation & Navigation Guide

### Getting Started
Refer to [references/getting-started.md](references/getting-started.md)

When you need to:
- Install the NuGet package, configure the project
- Bind `PropertyGridControl` to a data object
- Get a property grid up and running with minimal XAML

### Property Definitions
Refer to [references/property-definitions.md](references/property-definitions.md)

When you need to:
- Pick which properties show via `PropertyDefinition Path="..."`
- Match by property type (`Type="sys:String"`) instead of name
- Assign a custom editor per property (`EditSettings`)
- Use `ShowProperties` to switch between "show all" and "only what's defined"
- Mark properties read-only, add descriptions, change headers

### Collection Definitions
Refer to [references/collection-definitions.md](references/collection-definitions.md)

When you need to:
- Show a list / observable collection inline with the built-in collection editor (add / remove items)
- Provide default values for new items via `NewItemInitializer` / `XamlInitializer`
- Hide the **Add** or **Remove** buttons (`AllowAddItems`, `AllowRemoveItems`, `CollectionButtonsVisibility` event)
- Customize what happens when a user clicks **Add** (`CollectionButtonClick` event)
- Disable the collection editor and treat the collection as a plain property

### Property Categories
Refer to [references/categories.md](references/categories.md)

When you need to:
- Group properties under `[Category("...")]` headers
- Switch between flat / collapsible / tabbed views (`ShowCategories`)
- Customize category headers and glyphs via `CategoryDefinition`
- Auto-expand all categories on object change (`ExpandCategoriesWhenSelectedObjectChanged`)

### Expandability of Complex Properties
Refer to [references/expandability.md](references/expandability.md)

When you need to:
- Display nested object properties inline as expandable rows
- Force-expand all properties regardless of type converter (`AllowExpanding="Force"`)
- Block expansion for a property that would otherwise expand (`AllowExpanding="Never"`)
- Apply `ExpandableObjectConverter` (or a custom `TypeConverter`) at the model level

## Quick Start

### Minimal Property Grid

```xaml
<Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:dxprg="http://schemas.devexpress.com/winfx/2008/xaml/propertygrid"
        xmlns:local="clr-namespace:MyApp"
        Title="Settings" Width="500" Height="400">
    <Window.DataContext>
        <local:Customer/>
    </Window.DataContext>
    <dxprg:PropertyGridControl SelectedObject="{Binding}"/>
</Window>
```

```csharp
public class Customer {
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public DateTime BirthDate { get; set; }
}
```

By default, `ShowProperties="All"` — every public property of the bound object is shown with a type-appropriate editor.

### Only Specific Properties

```xaml
<dxprg:PropertyGridControl SelectedObject="{Binding}"
                           ShowProperties="WithPropertyDefinitions">
    <dxprg:PropertyDefinition Path="FirstName"/>
    <dxprg:PropertyDefinition Path="LastName"/>
    <dxprg:PropertyDefinition Path="BirthDate" IsReadOnly="True"/>
</dxprg:PropertyGridControl>
```

### Custom Editor per Type

```xaml
<dxprg:PropertyGridControl SelectedObject="{Binding}" ShowProperties="All">
    <dxprg:PropertyDefinition Type="sys:DateTime">
        <dxprg:PropertyDefinition.EditSettings>
            <dxe:DateEditSettings DisplayFormat="MMM-dd-yyyy"/>
        </dxprg:PropertyDefinition.EditSettings>
    </dxprg:PropertyDefinition>
</dxprg:PropertyGridControl>
```

### Categorized View

```csharp
public class Customer {
    [Category("Personal")]  public string FirstName { get; set; }
    [Category("Personal")]  public string LastName { get; set; }
    [Category("Contact")]   public string Email { get; set; }
    [Category("Contact")]   public string Phone { get; set; }
}
```

```xaml
<dxprg:PropertyGridControl SelectedObject="{Binding}"
                           ShowCategories="Visible"
                           ExpandCategoriesWhenSelectedObjectChanged="True"/>
```

### Editable Collection

```csharp
public class Order {
    public string Number { get; set; }
    public List<LineItem> Items { get; set; } = new();
}

public class LineItem {
    public string Sku { get; set; }
    public int Qty { get; set; }
}
```

```xaml
<dxprg:PropertyGridControl SelectedObject="{Binding}">
    <dxprg:CollectionDefinition Path="Items"/>
</dxprg:PropertyGridControl>
```

The collection renders with built-in **Add Item** / **Remove Item** buttons.

## Key API Surface

### `PropertyGridControl` Members

| Member | Use |
|---|---|
| `SelectedObject` | The object whose properties are displayed |
| `SelectedObjects` | Multiple objects edited simultaneously (common values shown; differing show blank) |
| `ShowProperties` | `All` (every property) or `WithPropertyDefinitions` (only declared) |
| `ShowCategories` | `Hidden`, `Visible` (collapsible groups), `Tabbed` (one tab per category) |
| `AllowExpanding` | `Default`, `Force`, `ForceIfNoTypeConverter`, `Never` |
| `ExpandCategoriesWhenSelectedObjectChanged` | Auto-expand all category groups when `SelectedObject` changes |
| `UseCollectionEditor` | Global on/off for inline collection editor |
| `PropertyDefinitions` | The collection of `PropertyDefinition` / `CollectionDefinition` / `CategoryDefinition` |
| `ReadOnly` | Make the entire grid read-only |
| `CollectionButtonClick` (event) | Handle Add/Remove clicks |
| `CollectionButtonsVisibility` (event) | Dynamically hide Add/Remove buttons |

### `PropertyDefinition` Members

| Member | Use |
|---|---|
| `Path` | Property path on the bound object (use `*` as wildcard) |
| `Scope` | Path to the parent property (for nested) |
| `Type` | Match by property type instead of name (e.g. `sys:String`) |
| `TypeMatchMode` | `Exact`, `Convertible` — how `Type` matching works |
| `EditSettings` | Editor configuration (`dxe:TextEditSettings`, `dxe:DateEditSettings`, ...) |
| `IsReadOnly` | Make this property read-only |
| `Header` | Custom row header (overrides property name) |
| `Description` | Description text shown in the description area |
| `AllowExpanding` | Per-property expandability override |
| `ApplyingMode` | `Always`, `WhenGrouping`, `WhenNoGrouping` — apply only in some category modes |

### `CollectionDefinition` Members (inherits `PropertyDefinition`)

| Member | Use |
|---|---|
| `UseCollectionEditor` | Enable/disable the inline editor for this collection |
| `NewItemInitializer` | Provide default values / handle creation of new items |
| `AllowAddItems` / `AllowRemoveItems` | Hide the Add / Remove buttons |
| `AllowRemoveItemsWithoutNewItemInitializer` | Allow Remove even without an initializer for Add |
| `HeaderShowMode` | How the collection's header row is shown |

### `CategoryDefinition` Members (inherits `PropertyDefinitionBase`)

| Member | Use |
|---|---|
| `Path` | The category name (matches `[Category("...")]` value) |
| `Header` | Custom header text (overrides the `[Category]` value as displayed) |
| `Glyph` | Icon for the tab header (in `Tabbed` mode) |
| `ColorizeGlyph` | Tint the glyph with the theme color |

## Common Patterns

### Pattern 1: Settings Dialog with Categories

```csharp
public class AppSettings {
    [Category("General")] [Description("Application start page")]
    public string StartPage { get; set; } = "Home";

    [Category("General")]
    public bool AutoSave { get; set; } = true;

    [Category("Appearance")]
    public string Theme { get; set; } = "Office2019Colorful";

    [Category("Appearance")]
    public double FontSize { get; set; } = 12;
}
```

```xaml
<dxprg:PropertyGridControl SelectedObject="{Binding Settings}"
                           ShowCategories="Visible"
                           ExpandCategoriesWhenSelectedObjectChanged="True"/>
```

### Pattern 2: Designer Property Panel

```xaml
<dxprg:PropertyGridControl
        SelectedObject="{Binding SelectedShape}"
        SelectedObjects="{Binding SelectedShapes}"
        ShowCategories="Visible"/>
```

Use `SelectedObjects` (collection) for multi-selection — common values shown, differing show blank, edits applied to all.

### Pattern 3: Nested Object with `ExpandableObjectConverter`

```csharp
public class Customer {
    public string Name { get; set; }
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public Address Address { get; set; }
}

public class Address {
    public string City { get; set; }
    public string Street { get; set; }
}
```

The `Address` row becomes an expandable group showing `City` and `Street` underneath.

### Pattern 4: Editable Collection with Default-Value Initializer

```xaml
<Window.Resources>
    <dxprg:XamlInitializer x:Key="lineItemInit">
        <dxprg:TypeDefinition Type="{x:Type local:LineItem}"
                              Name="Line Item" Description="New Line Item"/>
    </dxprg:XamlInitializer>
</Window.Resources>

<dxprg:PropertyGridControl SelectedObject="{Binding Order}">
    <dxprg:CollectionDefinition Path="Items"
                                NewItemInitializer="{StaticResource lineItemInit}"/>
</dxprg:PropertyGridControl>
```

### Pattern 5: Tabbed Categories with Glyphs

```xaml
<dxprg:PropertyGridControl SelectedObject="{Binding}"
                           ShowProperties="All"
                           ShowCategories="Tabbed">
    <dxprg:CategoryDefinition Path="Personal"
                              Glyph="{dx:DXImage Image=User_32x32.png}"/>
    <dxprg:CategoryDefinition Path="Contact"
                              Glyph="{dx:DXImage Image=Contact_32x32.png}"/>
</dxprg:PropertyGridControl>
```

## Troubleshooting

| Symptom | Cause | Solution |
|---|---|---|
| No properties show up | `ShowProperties="WithPropertyDefinitions"` but no `PropertyDefinition` set | Either define them, or switch to `ShowProperties="All"`. |
| A nested object shows as a single value, not expandable | No type converter, and `AllowExpanding="Default"` | Add `[TypeConverter(typeof(ExpandableObjectConverter))]` on the property, or set `AllowExpanding="Force"`. |
| Collection row doesn't show Add/Remove buttons | `UseCollectionEditor="False"`, or `AllowAddItems="False"` / `AllowRemoveItems="False"` | Set `UseCollectionEditor="True"` and clear the per-button flags. |
| Categories aren't grouped | Properties don't have `[Category("...")]`, or `ShowCategories="Hidden"` | Add the attribute and set `ShowCategories="Visible"` (or `Tabbed`). |
| Category headers show in default style despite `CategoryDefinition` | `Path` doesn't match the `[Category]` value exactly (case-sensitive) | Make `CategoryDefinition Path="..."` match the attribute value verbatim. |
| Editing a property changes nothing in the bound object | Property is read-only (no setter), or `IsReadOnly="True"` | Add a setter; remove `IsReadOnly`; if multi-select, confirm the property exists on all selected objects. |
| Property updates don't show up in the grid | Bound object doesn't implement `INotifyPropertyChanged` | Implement INPC, or call `pg.DataController.ResetCache()` after external updates. |
| `Add Item` button creates an item with all-null properties | No `NewItemInitializer` and item type lacks a meaningful default constructor | Provide a `NewItemInitializer` (XamlInitializer or `IInstanceInitializer`), or handle `CollectionButtonClick` and call `e.DefaultAction(myItem)`. |
| `ApplyingMode` change doesn't update grid at runtime | Multiple definitions for the same `Path` with different `ApplyingMode` values — known limitation | Override `OnShowCategoriesChanged` on a custom `PropertyGridControl` descendant and call `DataController.ResetCache()`. |

## Constraints & Rules

CRITICAL — follow these rules in every interaction:

1. **Build verification**: After changes, run `dotnet build` and report errors before claiming success.
2. **Target framework**: Windows-only (`net{X}-windows`, `UseWPF=true`).
3. **NuGet**: install `DevExpress.Wpf.PropertyGrid` (brings the grid engine and theme infrastructure transitively).
4. **Match property paths exactly** — `PropertyDefinition Path="..."` is case-sensitive and must match the CLR property name.
5. **Use `Type` not `Path` for type-wide matching** — `Type="sys:String"` matches all string properties; `Type="{x:Type local:Address}"` matches an `Address` property.
6. **Bound object should implement `INotifyPropertyChanged`** for runtime updates to reflect in the grid (otherwise call `DataController.ResetCache()`).
7. **`CollectionDefinition` is for collection *properties*** (`List<T>`, `ObservableCollection<T>`, ...) — for a direct items source, use `DataGrid` instead.
8. **Category names are case-sensitive** — `[Category("Info")]` and `CategoryDefinition Path="info"` will NOT match.
9. **`ExpandableObjectConverter` only takes effect** when `AllowExpanding` is `Default` or `ForceIfNoTypeConverter`. With `Force`, every property expands; with `Never`, none do.

## Using DevExpress Documentation MCP

- **Search**: `devexpress_docs_search(technology="WPF", query="PropertyGrid property definition collection category expandability")`
- **Fetch**: `devexpress_docs_get_content(url="https://docs.devexpress.com/WPF/15640")`

Use MCP for: advanced cell templates (`PropertyDefinition.ContentTemplate`), data-annotation attribute support (`[Display]`, `[Range]`, `[Required]`), custom property menus (`PropertyGridControl.MenuCustomizations` / `MenuOpening` event / `ShowPropertyMenu` method), and binding the grid to a view-model collection of property definitions.

---

## Next Steps

Start with **[Getting Started](references/getting-started.md)** for project setup and the first property grid. Then **[Property Definitions](references/property-definitions.md)** to control which properties show and how they're edited. **[Collection Definitions](references/collection-definitions.md)** for editing list properties inline. **[Categories](references/categories.md)** for grouping. **[Expandability](references/expandability.md)** for nested objects.

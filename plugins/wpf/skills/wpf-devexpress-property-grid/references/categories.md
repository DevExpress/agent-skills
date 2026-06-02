# Property Categories

The Property Grid can group properties under category headers (or category tabs) using the standard `[Category("...")]` attribute. Set `ShowCategories` to switch between flat, collapsible-group, and tabbed views. Use `CategoryDefinition` to customize the header text, glyph, or appearance of a specific category.

## When to Use This Reference

Use this when you need to:

- Group properties under named headers (e.g., "Personal", "Contact", "Appearance")
- Switch between flat and categorized views (`ShowCategories`)
- Render each category as a tab (`ShowCategories="Tabbed"`)
- Customize category header text or icon via `CategoryDefinition`
- Auto-expand all groups when the bound object changes

## Step 1 — Decorate Properties with `[Category]`

```csharp
using System.ComponentModel;

public class Supplier {
    [Category("Info")]
    public int ID { get; set; }
    [Category("Info")]
    public string Name { get; set; }
    [Category("Contact")]
    public string Email { get; set; }
    [Category("Contact")]
    public string Phone { get; set; }
}
```

> Category names are **case-sensitive**. `"Info"` and `"info"` are different categories.

## Step 2 — Set `ShowCategories`

```xaml
<dxprg:PropertyGridControl SelectedObject="{Binding DemoSupplier}"
                           ShowProperties="All"
                           ShowCategories="Visible"/>
```

| `CategoriesShowMode` | Behavior |
|---|---|
| `Hidden` | All properties are shown flat (no grouping) |
| `Visible` | Expandable/collapsible category groups |
| `Tabbed` | Each category becomes a tab |

The end user can also toggle categorized vs. flat from the built-in **tool panel** at runtime.

## Step 3 — (Optional) Customize Category Headers

Decorating with `[Category]` produces a default header equal to the attribute value. To customize, add a `CategoryDefinition`:

```xaml
<dxprg:PropertyGridControl SelectedObject="{Binding DemoSupplier}"
                           ShowProperties="All"
                           ShowCategories="Visible">
    <dxprg:CategoryDefinition Path="Info"    Header="Personal Information"/>
    <dxprg:CategoryDefinition Path="Contact" Header="Contact Information"/>
</dxprg:PropertyGridControl>
```

| `CategoryDefinition` member | Use |
|---|---|
| `Path` | The category name (matches the `[Category("...")]` value exactly) |
| `Header` | The displayed header text |
| `Glyph` | Icon (typically a `DXImage`) — for the **Tabbed** mode |
| `ColorizeGlyph` | Tint the glyph with the theme color |

## Tabbed View with Glyphs

```xaml
<dxprg:PropertyGridControl SelectedObject="{Binding DemoSupplier}"
                           ShowProperties="All"
                           ShowCategories="Tabbed">
    <dxprg:CategoryDefinition Path="Info"
                              Glyph="{dx:DXImage Image=Female_32x32.png}"/>
    <dxprg:CategoryDefinition Path="Contact"
                              Glyph="{dx:DXImage Image=Contact_32x32.png}"/>
</dxprg:PropertyGridControl>
```

Each category becomes a tab in the grid header with its glyph and name. Use `dx:DXImage` to pull from the DevExpress icon library, or supply your own image URI.

## Auto-Expand on Object Change

When the user picks a different object (e.g., master-detail), you typically want all category groups to be expanded for the new selection:

```xaml
<dxprg:PropertyGridControl
        SelectedObject="{Binding SelectedItem, ElementName=grid}"
        ExpandCategoriesWhenSelectedObjectChanged="True"/>
```

## Programmatic Expand / Collapse

```csharp
propertyGrid.Expand("Info");
propertyGrid.Collapse("Contact");
```

To hide the expand button per category row, use `PropertyGridControl.ExpandButtonsVisibility`.

## Custom Expand Logic — `CustomExpand`

Handle `PropertyGridControl.CustomExpand` to intercept expansion of a category (or a property) and run your own logic — useful for lazy-loading subproperties or showing a confirmation before expanding a heavy collection.

## `ApplyingMode` — Different Definitions per View Mode

Some properties make sense in only one view mode. `PropertyDefinitionBase.ApplyingMode` decides when a definition is applied:

| `ApplyingMode` | When applied |
|---|---|
| `Always` (default) | Always |
| `WhenGrouping` | Only when `ShowCategories` is `Visible` or `Tabbed` |
| `WhenNoGrouping` | Only when `ShowCategories` is `Hidden` |

```xaml
<dxprg:PropertyGridControl SelectedObject="{Binding DemoSupplier}"
                           ShowProperties="WithPropertyDefinitions">
    <dxprg:CategoryDefinition Path="Info"    Header="Personal Information"/>
    <dxprg:CategoryDefinition Path="Contact" Header="Contact Information"/>

    <dxprg:PropertyDefinition Path="ID"    ApplyingMode="Always"
                              Description="Customer's ID"/>
    <dxprg:PropertyDefinition Path="Email" ApplyingMode="WhenGrouping"
                              Description="Email"/>
    <dxprg:PropertyDefinition Path="Name"  ApplyingMode="WhenGrouping"
                              Description="Name (categorized)"/>
    <dxprg:PropertyDefinition Path="Name"  ApplyingMode="WhenNoGrouping"
                              Description="Name (flat)"/>
</dxprg:PropertyGridControl>
```

Behavior:

- **ID** appears in both modes.
- **Email** appears only when categorized.
- **Name** has a different description in each mode.

### Runtime `ShowCategories` Switch — Known Limitation

When two definitions share the same `Path` but differ in `ApplyingMode`, switching `ShowCategories` at runtime does **not** re-evaluate the definitions. Workaround — derive from `PropertyGridControl` and reset the cache:

```csharp
public class MyPropertyGridControl : PropertyGridControl {
    protected override void OnShowCategoriesChanged(CategoriesShowMode oldValue) {
        base.OnShowCategoriesChanged(oldValue);
        this.DataController.ResetCache();
    }
}
```

Then use `<local:MyPropertyGridControl ...>` in XAML instead of `PropertyGridControl`.

## Full Example — Application Settings Dialog

```csharp
using System.ComponentModel;

public class AppSettings {
    [Category("General")] [Description("Application start page")]
    public string StartPage { get; set; } = "Home";

    [Category("General")]
    public bool AutoSave { get; set; } = true;

    [Category("Appearance")]
    public string Theme { get; set; } = "Office2019Colorful";

    [Category("Appearance")]
    public double FontSize { get; set; } = 12;

    [Category("Advanced")] [Description("Enable verbose logging")]
    public bool VerboseLogging { get; set; }
}
```

```xaml
<Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:dxprg="http://schemas.devexpress.com/winfx/2008/xaml/propertygrid"
        xmlns:dx="http://schemas.devexpress.com/winfx/2008/xaml/core"
        xmlns:local="clr-namespace:MyApp"
        Title="Settings" Width="500" Height="400">
    <Window.DataContext>
        <local:AppSettings/>
    </Window.DataContext>

    <dxprg:PropertyGridControl SelectedObject="{Binding}"
                               ShowCategories="Tabbed"
                               ExpandCategoriesWhenSelectedObjectChanged="True">
        <dxprg:CategoryDefinition Path="General"
                                  Header="General"
                                  Glyph="{dx:DXImage Image=Settings_32x32.png}"/>
        <dxprg:CategoryDefinition Path="Appearance"
                                  Header="Look &amp; Feel"
                                  Glyph="{dx:DXImage Image=Theme_32x32.png}"/>
        <dxprg:CategoryDefinition Path="Advanced"
                                  Header="Advanced"
                                  Glyph="{dx:DXImage Image=ToolBox_32x32.png}"/>
    </dxprg:PropertyGridControl>
</Window>
```

Three tabs across the top, each with an icon. Users edit each section independently.

## Common Issues

- **Custom header doesn't apply** — `CategoryDefinition.Path` is case-sensitive; it must match the `[Category("...")]` value byte-for-byte.
- **Categories don't appear** — `ShowCategories="Hidden"`, or the properties don't carry `[Category]` (uncategorized properties fall into a "Misc" group).
- **`CategoryDefinition` ignored** — placed *before* `ShowCategories="Visible"` is set (less common), or the control is showing only properties without categories. Confirm with `ShowProperties="All"` first.
- **Glyph doesn't show** — `ShowCategories` is `Visible` (glyphs are a `Tabbed`-only feature) or the `DXImage` path is wrong.
- **Switching from flat to categorized doesn't update `ApplyingMode`-scoped definitions** — see the known limitation above; override `OnShowCategoriesChanged`.
- **All properties end up in one category** — properties without `[Category]` aren't grouped. Add the attribute, or use `CategoryDefinition` to provide explicit groupings via a custom data-source pattern.

## Source Material

- `articles/controls-and-libraries/property-grid/property-categories.md` (`xref:117082`)
- `articles/controls-and-libraries/property-grid/visual-elements/category.md` (`xref:116320`)
- `articles/controls-and-libraries/property-grid/visual-elements/tool-panel.md` (`xref:15624`)

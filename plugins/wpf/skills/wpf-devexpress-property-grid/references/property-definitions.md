# Property Definitions

`PropertyDefinition` is the central authoring primitive of `PropertyGridControl`. Each definition is linked to one or more properties of the bound object and controls **which** properties appear, **how** they're matched, what **editor** is used, the row **header / description**, and whether the property is **read-only**. Definitions live in `PropertyGridControl.PropertyDefinitions`.

## When to Use This Reference

Use this when you need to:

- Decide whether to show all properties of the bound object or only specific ones
- Match by property name (path), by parent path (scope), or by type
- Assign a custom editor / format per property or per type
- Add a row header, description, or mark a property read-only
- Show subproperties of a nested object explicitly

## `ShowProperties` — Show All vs. Only Defined

```xaml
<dxprg:PropertyGridControl SelectedObject="{Binding}"
                           ShowProperties="WithPropertyDefinitions">
    <dxprg:PropertyDefinition Path="ID" IsReadOnly="True"/>
    <dxprg:PropertyDefinition Path="FirstName"/>
</dxprg:PropertyGridControl>
```

| `ShowProperties` value | Behavior |
|---|---|
| `All` (default) | The grid shows every public property. Definitions still apply where they match. |
| `WithPropertyDefinitions` | Only properties with a matching `PropertyDefinition` are shown. |

**Pattern**: use `All` when the bound object's surface is already what you want and definitions only refine a few rows; use `WithPropertyDefinitions` when you need explicit control over what's visible.

## Matching — `Path`, `Scope`, `Type`

A definition can be linked to a property in three ways. When multiple definitions could match, the highest-priority one wins:

| Criterion | Priority | Property | Example |
|---|---|---|---|
| **Property path** | High | `Path` | `Path="FirstName"` |
| **Parent path** | Medium | `Scope` | `Scope="Address" Path="City"` |
| **Property type** | Low | `Type` | `Type="sys:String"` |

### Match by Name

```xaml
<dxprg:PropertyDefinition Path="FirstName"/>
<dxprg:PropertyDefinition Path="LastName"/>
```

Use `*` as a wildcard:

```xaml
<!-- Match every public property -->
<dxprg:PropertyDefinition Path="*"/>
```

### Match by Type

Useful for applying a single editor configuration to all properties of a type (e.g., all `string` properties get a 15-char limit):

```xaml
xmlns:sys="clr-namespace:System;assembly=mscorlib"

<dxprg:PropertyDefinition Type="sys:String">
    <dxprg:PropertyDefinition.EditSettings>
        <dxe:TextEditSettings MaxLength="15"/>
    </dxprg:PropertyDefinition.EditSettings>
</dxprg:PropertyDefinition>

<dxprg:PropertyDefinition Type="sys:DateTime">
    <dxprg:PropertyDefinition.EditSettings>
        <dxe:DateEditSettings DisplayFormat="MMM-dd-yyyy"/>
    </dxprg:PropertyDefinition.EditSettings>
</dxprg:PropertyDefinition>
```

`TypeMatchMode` controls how `Type` matches:

| `TypeMatchMode` | Behavior |
|---|---|
| `Direct` (default) | Only properties whose declared type matches exactly |
| `Extended` | Properties whose type is, derives from, or implements the specified type/interface |

### Match by Parent Scope (Nested)

```xaml
<dxprg:PropertyDefinition Scope="Address" Path="State" IsReadOnly="True"/>
```

Matches the `State` property of whatever is at the `Address` path on the bound object.

### Combined: Path Wildcards with Type

```xaml
<dxprg:CollectionDefinition Path="Suppliers">
    <dxprg:PropertyDefinition Path="*.Name"/>
    <dxprg:PropertyDefinition Path="*.Phone"/>
</dxprg:CollectionDefinition>
```

Inside a `CollectionDefinition`, `*` matches each item; `*.Name` matches the `Name` property of every collection item.

## Custom Editors via `EditSettings`

`EditSettings` accepts any `BaseEditSettings` subclass from `DevExpress.Xpf.Editors`. Common ones:

| Editor settings | For |
|---|---|
| `TextEditSettings` | Strings |
| `MemoEditSettings` | Multi-line strings |
| `DateEditSettings` | Dates |
| `TimeEditSettings` | Times |
| `SpinEditSettings` | Numerics with spin buttons |
| `CalcEditSettings` | Numeric with calculator dropdown |
| `ComboBoxEditSettings` | Dropdown with `ItemsSource` |
| `CheckEditSettings` | Booleans |
| `ColorEditSettings` | Colors |
| `ImageEditSettings` | Images |
| `TokenComboBoxStyleSettings` | Multi-value comma-separated tokens (requires `IsReadOnly="True"` on the definition) |

```xaml
<dxprg:PropertyDefinition Path="Discount">
    <dxprg:PropertyDefinition.EditSettings>
        <dxe:SpinEditSettings MinValue="0" MaxValue="100"
                              DisplayFormat="0.##\%"/>
    </dxprg:PropertyDefinition.EditSettings>
</dxprg:PropertyDefinition>
```

```xaml
<dxprg:PropertyDefinition Path="Country">
    <dxprg:PropertyDefinition.EditSettings>
        <dxe:ComboBoxEditSettings ItemsSource="{Binding Countries}"
                                  IsTextEditable="False"/>
    </dxprg:PropertyDefinition.EditSettings>
</dxprg:PropertyDefinition>
```

## Header, Description, Read-Only

```xaml
<dxprg:PropertyDefinition Path="Email"
                          Header="E-mail address"
                          Description="Customer's primary e-mail"
                          IsReadOnly="False"/>
```

- `Header` — replaces the column-1 label (defaults to the property name).
- `Description` — text shown in the description panel (also driven by `[Description("...")]` on the model property).
- `IsReadOnly` — set `True` to disable editing for this row.

Set `PropertyGridControl.ReadOnly="True"` to make the whole grid read-only.

## Nested Properties

To show the subproperties of a nested object inline (e.g., `Customer.Address.State` under an expandable `Address` row), the framework needs to know the property is expandable. Two ways:

### At the model — `ExpandableObjectConverter`

```csharp
public class Customer {
    public string FirstName { get; set; }
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public SimpleAddress Address { get; set; }
}

public class SimpleAddress {
    public string State { get; set; }
    public string City { get; set; }
}
```

The `Address` row is now expandable.

### At the control — `AllowExpanding="Force"`

```xaml
<dxprg:PropertyGridControl SelectedObject="{Binding}" AllowExpanding="Force"/>
```

Every property expands when possible. See [expandability.md](expandability.md) for the full picker.

### Defining Subproperty Rows Explicitly

Three equivalent forms — pick the one that reads best:

```xaml
<!-- Nested PropertyDefinition -->
<dxprg:PropertyDefinition Path="Address">
    <dxprg:PropertyDefinition Path="State" IsReadOnly="True"/>
</dxprg:PropertyDefinition>

<!-- Scope at the top level -->
<dxprg:PropertyDefinition Scope="Address" Path="State" IsReadOnly="True"/>

<!-- Match by sub-type -->
<dxprg:PropertyDefinition Type="{x:Type local:SimpleAddress}">
    <dxprg:PropertyDefinition Path="State" IsReadOnly="True"/>
</dxprg:PropertyDefinition>
```

## `ApplyingMode` — Different Definitions for Different View Modes

When the grid switches between categorized and flat views, you may want different definitions to apply:

| `ApplyingMode` | When applied |
|---|---|
| `Always` (default) | Always |
| `WhenGrouping` | Only when `ShowCategories` is `Visible` / `Tabbed` |
| `WhenNoGrouping` | Only when `ShowCategories` is `Hidden` |

```xaml
<dxprg:PropertyDefinition Path="Name" Description="Name (categorized)"
                          ApplyingMode="WhenGrouping"/>
<dxprg:PropertyDefinition Path="Name" Description="Name (flat)"
                          ApplyingMode="WhenNoGrouping"/>
```

See the limitation in [categories.md](categories.md) about runtime `ShowCategories` changes.

## Full Example

```xaml
<Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:dxprg="http://schemas.devexpress.com/winfx/2008/xaml/propertygrid"
        xmlns:dxe="http://schemas.devexpress.com/winfx/2008/xaml/editors"
        xmlns:sys="clr-namespace:System;assembly=mscorlib"
        xmlns:local="clr-namespace:MyApp">
    <Window.DataContext>
        <local:CustomerViewModel/>
    </Window.DataContext>
    <dxprg:PropertyGridControl SelectedObject="{Binding Customer}"
                               ShowProperties="WithPropertyDefinitions">
        <dxprg:PropertyDefinition Path="ID" IsReadOnly="True"
                                  Header="Customer ID"/>
        <dxprg:PropertyDefinition Path="FirstName"/>
        <dxprg:PropertyDefinition Path="LastName"/>

        <dxprg:PropertyDefinition Type="sys:DateTime">
            <dxprg:PropertyDefinition.EditSettings>
                <dxe:DateEditSettings DisplayFormat="MMM-dd-yyyy"/>
            </dxprg:PropertyDefinition.EditSettings>
        </dxprg:PropertyDefinition>

        <dxprg:PropertyDefinition Path="Country">
            <dxprg:PropertyDefinition.EditSettings>
                <dxe:ComboBoxEditSettings ItemsSource="{Binding Countries}"
                                          IsTextEditable="False"/>
            </dxprg:PropertyDefinition.EditSettings>
        </dxprg:PropertyDefinition>
    </dxprg:PropertyGridControl>
</Window>
```

## Defining Properties from a View Model

For MVVM scenarios, definitions can be generated from a view-model collection rather than declared in XAML. Use `PropertyGridControl.PropertyDefinitionsSource` (binds to a collection) with `PropertyDefinitionTemplate` / `PropertyDefinitionTemplateSelector` to produce one definition per view-model item.

```xaml
<dxprg:PropertyGridControl
        PropertyDefinitionsSource="{Binding Properties}"
        PropertyDefinitionTemplateSelector="{StaticResource defSelector}"
        SelectedObject="{Binding EditObject}"
        ShowProperties="WithPropertyDefinitions"/>
```

This pattern is in the docs page "Binding to a collection of PropertyDefinitions" (`xref:115668`) — use the DevExpress MCP to fetch the full sample when needed.

## Common Issues

- **Property doesn't show** — `ShowProperties="WithPropertyDefinitions"` is on but `Path` doesn't match (it's case-sensitive). Confirm with `Path="*"` first to see everything.
- **Editor settings ignored** — `EditSettings` placed on the wrong definition; the definition isn't winning the priority race (e.g., a `Type`-based definition is being shadowed by a `Path`-based one for the same property).
- **`Type="sys:String"` doesn't work** — missing `xmlns:sys="clr-namespace:System;assembly=mscorlib"`.
- **Nested property doesn't show subproperties** — no `[TypeConverter(typeof(ExpandableObjectConverter))]` on the property *and* `AllowExpanding` is `Default`. Set the attribute or change `AllowExpanding`.
- **Read-only setting overridden** — the property itself has no setter, so the grid forces read-only regardless of `IsReadOnly`.
- **TokenComboBox editor doesn't accept edits** — must set `IsReadOnly="True"` on the property definition (the editor handles editing through its dropdown, not the cell).

## Source Material

- `articles/controls-and-libraries/property-grid/property-definitions.md` (`xref:15521`)
- `articles/controls-and-libraries/property-grid/getting-started/creating-property-definitions.md` (`xref:15610`)
- `articles/controls-and-libraries/property-grid/property-definitions/binding-to-a-collection-of-propertydefinitions.md` (`xref:115668`)
- `articles/controls-and-libraries/property-grid/property-definitions/displaying-multiple-properties-in-a-single-cell.md` (`xref:118197`)

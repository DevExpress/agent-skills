# Expandability of Complex Properties

A "complex" property is one whose value is an object with its own properties (e.g., a `Customer.Address` property of type `SimpleAddress`). The Property Grid can render such a property as an expandable group — clicking the row's expand arrow reveals the subproperties (`State`, `City`, `ZIP`, ...) as nested rows. By default the grid decides whether to expand based on **type converters**; you can override that at the **control level** or **per definition**.

## When to Use This Reference

Use this when you need to:

- Make a nested object property expandable inline
- Force every property to expand (even without type converters)
- Block expansion on a property that would otherwise expand
- Understand how `AllowExpanding` interacts with `[TypeConverter]`
- Decide between model-level (attribute) and control-level (XAML) configuration

## Two Layers of Configuration

The Property Grid evaluates expandability in two layers:

1. **Control / definition level** — the `AllowExpanding` property.
2. **Model level** — the `[TypeConverter]` attribute on the property.

The two interact: the type converter only matters when `AllowExpanding` is `Default` or `ForceIfNoTypeConverter`.

## Layer 1 — `AllowExpanding` at the Control / Definition

`PropertyGridControl.AllowExpanding` sets the policy for the whole grid; `PropertyDefinition.AllowExpanding` overrides it for one property.

| `AllowExpanding` | Behavior |
|---|---|
| `Default` (default) | Use type converters — expand only when the property has an `ExpandableObjectConverter` (or another converter that returns true from `GetPropertiesSupported`). |
| `Force` | Expand all properties unconditionally — ignore type converters. |
| `ForceIfNoTypeConverter` | Expand all properties that have no type converter; honor type converters when one is set. |
| `Never` | Properties are never expandable. |

> **Precedence**: `PropertyDefinition.AllowExpanding` wins over `PropertyGridControl.AllowExpanding`.

### Example — Force All Properties to Expand

```xaml
<dxprg:PropertyGridControl Grid.Column="1"
                           SelectedObject="{Binding Data}"
                           ShowProperties="All"
                           AllowExpanding="Force">
    <dxprg:PropertyDefinition Path="PublicInfo"/>

    <!-- Override the global Force for this collection: -->
    <dxprg:CollectionDefinition Path="PrivateInfo" AllowExpanding="Never"/>
</dxprg:PropertyGridControl>
```

`PublicInfo` is expanded (per `Force`); `PrivateInfo` is **not** (per its own `Never`).

## Layer 2 — `[TypeConverter]` at the Model

When `AllowExpanding` is `Default` or `ForceIfNoTypeConverter`, expandability is driven by the property's type converter.

### `ExpandableObjectConverter` — Make It Expandable

```csharp
using System.ComponentModel;

public class Customer {
    public string FirstName { get; set; }
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public Address Address { get; set; }
}

public class Address {
    public string State { get; set; }
    public string City { get; set; }
    public string ZIP { get; set; }
}
```

With `AllowExpanding="Default"`, the `Address` property expands into `State`, `City`, `ZIP`.

### Custom `TypeConverter` — Block Expansion

A type converter whose `GetPropertiesSupported` returns `false` blocks expansion:

```csharp
public class NotExpandableConverter : TypeConverter {
    public override bool GetPropertiesSupported(ITypeDescriptorContext context) {
        return false;
    }
}

public class Container {
    public ClassWithProperties Simple { get; set; }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public ClassWithProperties Expandable { get; set; }

    [TypeConverter(typeof(NotExpandableConverter))]
    public ClassWithProperties NotExpandable { get; set; }

    public List<ClassWithProperties> Collection { get; set; }
}
```

With `AllowExpanding="Default"`:

| Property | Has converter? | Expands? |
|---|---|---|
| `Simple` | No | No |
| `Expandable` | `ExpandableObjectConverter` | Yes |
| `NotExpandable` | `NotExpandableConverter` (returns false) | No |
| `Collection` | No (but it's a collection) | Yes (collections always expand) |

## Picker — Which Layer Should I Use?

| Situation | Recommendation |
|---|---|
| You own the model and want certain properties to always expand | `[TypeConverter(typeof(ExpandableObjectConverter))]` on those properties |
| You don't own the model (third-party types) | `AllowExpanding="Force"` (grid) or per-`PropertyDefinition` override |
| You want everything expandable, but block one or two specific properties | `AllowExpanding="Force"` + `PropertyDefinition AllowExpanding="Never"` per exception |
| You want types with explicit `[TypeConverter]` to drive behavior, and types without converters to also expand | `AllowExpanding="ForceIfNoTypeConverter"` |
| You want the grid never to show expanders | `AllowExpanding="Never"` |

## Defining the Visible Subproperties

Even when a property is expandable, the grid by default shows every subproperty of its value object. To control what shows under the expanded row, declare child `PropertyDefinition`s. Three equivalent forms:

```xaml
<!-- 1. Nested PropertyDefinition (under the parent path) -->
<dxprg:PropertyDefinition Path="Address">
    <dxprg:PropertyDefinition Path="State" IsReadOnly="True"/>
</dxprg:PropertyDefinition>

<!-- 2. Scope at top level -->
<dxprg:PropertyDefinition Scope="Address" Path="State" IsReadOnly="True"/>

<!-- 3. Match by type -->
<dxprg:PropertyDefinition Type="{x:Type local:SimpleAddress}">
    <dxprg:PropertyDefinition Path="State" IsReadOnly="True"/>
</dxprg:PropertyDefinition>
```

Pick by intent:

- **Form 1** — visually scoped to the parent; clear when one parent has many definition children.
- **Form 2** — flat layout; clear when you only need one or two children.
- **Form 3** — applies to *every* property whose type is `SimpleAddress`, regardless of where it appears.

## Complete Example — Customer + Address

```csharp
using System.ComponentModel;

public class Customer {
    public string FirstName { get; set; }
    public string LastName { get; set; }

    [TypeConverter(typeof(ExpandableObjectConverter))]
    public SimpleAddress Address { get; set; }
}

public class SimpleAddress {
    public string State { get; set; }
    public string City { get; set; }
    public string ZIP { get; set; }
}
```

```xaml
<dxprg:PropertyGridControl SelectedObject="{Binding Data}"
                           ShowProperties="WithPropertyDefinitions">
    <dxprg:PropertyDefinition Path="FirstName"/>
    <dxprg:PropertyDefinition Path="LastName"/>

    <dxprg:PropertyDefinition Path="Address">
        <dxprg:PropertyDefinition Path="State" IsReadOnly="True"/>
        <dxprg:PropertyDefinition Path="City"/>
        <dxprg:PropertyDefinition Path="ZIP" Header="Postal Code"/>
    </dxprg:PropertyDefinition>
</dxprg:PropertyGridControl>
```

The grid shows three top-level rows: `FirstName`, `LastName`, and an expandable `Address`. Inside `Address`, `State` is read-only, `City` is editable, `ZIP` is shown with a custom header.

## Collections Are Always Expandable

A collection property (`List<T>`, `ObservableCollection<T>`, ...) is treated as expandable regardless of `AllowExpanding`, because the framework needs to expand it to render the inline collection editor. To opt out, set the **collection definition**'s `AllowExpanding="Never"` (which hides the expander and prevents the collection editor from opening).

## Lazy Expansion — `CustomExpand`

For costly subproperty enumeration (e.g., remote data), handle `PropertyGridControl.CustomExpand`:

```csharp
propertyGrid.CustomExpand += (s, e) => {
    if (e.Path == "RemoteData") {
        // Load asynchronously, then let the framework expand
    }
};
```

## Common Issues

- **Nested object shows as a single value (its `ToString()`), not expandable** — no `[TypeConverter(typeof(ExpandableObjectConverter))]` on the property, and `AllowExpanding="Default"`. Add the attribute, or set `AllowExpanding="Force"`.
- **Setting `AllowExpanding="Force"` didn't help** — a `PropertyDefinition` for that property has its own `AllowExpanding="Default"` or `Never`. Definition wins over control.
- **A property with `[TypeConverter(typeof(ExpandableObjectConverter))]` doesn't expand** — the grid's `AllowExpanding` is `Never` (overrides everything), or the property's value is `null`.
- **Wrong subproperties shown** — by default, all of the value's public properties show. Declare explicit child definitions or set `ShowProperties="WithPropertyDefinitions"`.
- **Custom `NotExpandableConverter` is ignored** — `AllowExpanding="Force"` overrides it; switch to `Default` or `ForceIfNoTypeConverter`.

## Source Material

- `articles/controls-and-libraries/property-grid/expandability-customization.md` (`xref:117149`)
- `articles/controls-and-libraries/property-grid/property-definitions.md` (`xref:15521`)
- `articles/controls-and-libraries/property-grid/property-attributes.md` (`xref:15623`)

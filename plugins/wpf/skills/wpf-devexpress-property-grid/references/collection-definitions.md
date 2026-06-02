# Collection Definitions

A `CollectionDefinition` is a `PropertyDefinition` for a collection property (`List<T>`, `ObservableCollection<T>`, ...). It tells the property grid to display the collection inline with a built-in **collection editor** — users can browse items, edit per-item properties, and (by default) **Add** / **Remove** items via per-row buttons.

## When to Use This Reference

Use this when you need to:

- Show a collection property of the bound object as an expandable, editable group
- Provide default values for newly added items (`NewItemInitializer`)
- Hide the **Add** or **Remove** buttons
- Customize what happens when a user clicks **Add** / **Remove**
- Turn the collection editor off and treat the collection as a plain value

## Basic Declaration

```csharp
public class Product {
    public int ID { get; set; }
    public string ProductName { get; set; }
    public SupplierList Suppliers { get; set; }
}

public class Supplier {
    public int ID { get; set; }
    public string Name { get; set; }
    public string Phone { get; set; }
}

public class SupplierList : List<Supplier> { }
```

```xaml
<dxprg:PropertyGridControl SelectedObject="{Binding DemoProduct}">
    <dxprg:CollectionDefinition Path="Suppliers"/>
</dxprg:PropertyGridControl>
```

This renders the `Suppliers` row with an expandable list of items and **Add / Remove** buttons:

- Expand the row to see each `Supplier` as a nested group with its own properties.
- Click **+** to add a new item; click **−** on an item to remove it.

## Filtering Which Item Properties Show

By default, every public property of the collection's item type is shown. To narrow down — for example, show only the string properties of `Supplier`:

```xaml
<dxprg:CollectionDefinition Path="Suppliers">
    <dxprg:PropertyDefinition Type="sys:String"/>
</dxprg:CollectionDefinition>
```

Or by name (use `*.` for "any item, this property"):

```xaml
<dxprg:CollectionDefinition Path="Suppliers">
    <dxprg:PropertyDefinition Path="*.Name"/>
    <dxprg:PropertyDefinition Path="*.Phone"/>
</dxprg:CollectionDefinition>
```

> When `ShowProperties="WithPropertyDefinitions"`, the grid does not show collection item properties unless you declare them inside the `CollectionDefinition`.

## Linking Criteria — Same as `PropertyDefinition`

A `CollectionDefinition` matches a collection property by the same three-tier criteria:

| Criterion | Priority | Property |
|---|---|---|
| Collection property path | High | `Path` |
| Parent path | Medium | `Scope` |
| Collection property type | Low | `Type` |

`CollectionDefinition` has **higher** priority than `PropertyDefinition` for the same collection property — unless a `PropertyDefinition` specifies the exact `Path` to the collection (in which case the path-based `PropertyDefinition` wins).

## Toggling the Collection Editor

| Property | Use |
|---|---|
| `PropertyGridControl.UseCollectionEditor` | Global on/off |
| `CollectionDefinition.UseCollectionEditor` | Per-collection on/off |

Set to `false` to render the collection as a single property value (no expand button, no Add/Remove). Useful when:

- The collection is informational only
- A more specialized editor handles editing elsewhere

```xaml
<dxprg:CollectionDefinition Path="Suppliers" UseCollectionEditor="False"/>
```

## `NewItemInitializer` — Default Values for New Items

By default, **Add** invokes the parameterless constructor of the item type. To provide default property values, or to support item types without a parameterless constructor, use a `NewItemInitializer`.

### `XamlInitializer` (declarative)

```xaml
<Window.Resources>
    <dxprg:XamlInitializer x:Key="supplierInit"
                           Initialize="OnInitializeSupplier">
        <dxprg:TypeDefinition Type="{x:Type local:Supplier}"
                              Name="Supplier"
                              Description="A new supplier"/>
    </dxprg:XamlInitializer>
</Window.Resources>

<dxprg:PropertyGridControl SelectedObject="{Binding DemoProduct}">
    <dxprg:CollectionDefinition Path="Suppliers"
                                NewItemInitializer="{StaticResource supplierInit}"/>
</dxprg:PropertyGridControl>
```

```csharp
void OnInitializeSupplier(object sender, InstanceInitializeEventArgs e) {
    if (e.Type == typeof(Supplier)) {
        e.Instance = new Supplier { Name = "New Supplier", Phone = "" };
    }
}
```

Multiple `TypeDefinition` children make **Add** show a sub-menu so users can pick the kind of item to add (useful when the collection holds a base type with several derived types).

### `IInstanceInitializer` (code)

For `Dictionary<TKey, TValue>` and other non-default-constructor scenarios:

1. Implement `IInstanceInitializer` (defines `CreateInstance` and a `Types` collection).
2. Override `CreateInstance` to return the new item.
3. Pass an instance to `CollectionDefinition.NewItemInitializer`.

The full sample is on GitHub: *wpf-propertygrid-add-an-item-to-a-collection-or-a-dictionary*.

## Hiding the Add / Remove Buttons

### Per Collection — Static

```xaml
<dxprg:CollectionDefinition Path="Suppliers"
                            AllowAddItems="False"
                            AllowRemoveItems="False"/>
```

| Property | Effect |
|---|---|
| `AllowAddItems="False"` | Hides the **+** button |
| `AllowRemoveItems="False"` | Hides the **−** button on each item |
| `AllowRemoveItemsWithoutNewItemInitializer="True"` | Show **−** even when the type cannot be added (e.g., no default ctor and no initializer) |

### Dynamic — `CollectionButtonsVisibility` Event

To hide buttons based on state (count, item identity, etc.), handle the `PropertyGridControl.CollectionButtonsVisibility` event:

```xaml
<dxprg:PropertyGridControl SelectedObject="{Binding DemoProduct}"
                           CollectionButtonsVisibility="OnCollectionButtonsVisibility"/>
```

```csharp
using DevExpress.Xpf.PropertyGrid;

void OnCollectionButtonsVisibility(object sender, CollectionButtonsVisibilityEventArgs e) {
    if (e.ButtonKind == CollectionButtonKind.Add) {
        var collection = (SupplierList)e.Value;
        e.IsVisible = collection.Count <= 5;   // cap at 5
    }
    if (e.ButtonKind == CollectionButtonKind.Remove) {
        var supplier = e.Value as Supplier;
        if (supplier != null && supplier.ID == 0)
            e.IsVisible = false;               // protect supplier ID 0
    }
}
```

## Customizing Add / Remove Behavior — `CollectionButtonClick`

Handle this event to take control over what happens when the user clicks **Add** or **Remove**. The handler can perform a custom action and call `e.DefaultAction(item)` to merge it with the framework's standard behavior.

```xaml
<dxprg:PropertyGridControl SelectedObject="{Binding DemoProduct}"
                           CollectionButtonClick="OnCollectionButtonClick"/>
```

```csharp
using DevExpress.Xpf.PropertyGrid;

void OnCollectionButtonClick(object sender, CollectionButtonClickEventArgs e) {
    if (e.ButtonKind == CollectionButtonKind.Add) {
        var collection = (SupplierList)e.Value;
        var newId = collection.Count == 0 ? 0 : collection.Max(x => x.ID) + 1;
        e.DefaultAction(new Supplier { ID = newId, Name = "", Phone = "" });
        e.Handled = true;
    }
}
```

Set `e.Handled = true` to prevent the framework's default add-or-remove from also running.

## Complete Example — Editable Order Lines

```csharp
using System.Collections.Generic;

public class Order {
    public string Number { get; set; } = "ORD-001";
    public List<LineItem> Items { get; set; } = new();
}

public class LineItem {
    public string Sku { get; set; }
    public int Qty { get; set; } = 1;
    public decimal Price { get; set; }
}
```

```xaml
<Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:dxprg="http://schemas.devexpress.com/winfx/2008/xaml/propertygrid"
        xmlns:local="clr-namespace:MyApp"
        Title="Order Editor" Width="600" Height="500">
    <Window.Resources>
        <dxprg:XamlInitializer x:Key="lineItemInit">
            <dxprg:TypeDefinition Type="{x:Type local:LineItem}"
                                  Name="Line Item"
                                  Description="New order line"/>
        </dxprg:XamlInitializer>
    </Window.Resources>

    <Window.DataContext>
        <local:Order/>
    </Window.DataContext>

    <dxprg:PropertyGridControl SelectedObject="{Binding}">
        <dxprg:PropertyDefinition Path="Number"/>
        <dxprg:CollectionDefinition Path="Items"
                                    NewItemInitializer="{StaticResource lineItemInit}"/>
    </dxprg:PropertyGridControl>
</Window>
```

Result: a property grid with the `Number` field on top and an expandable `Items` row underneath. Users can click **+** to add a `LineItem`, expand each item to edit its fields, and **−** to remove items.

## Dictionary Editing

A `Dictionary<TKey, TValue>` property can be edited via the collection editor, but it needs:

1. An `IInstanceInitializer` implementation that returns a `KeyValuePair<TKey, TValue>`-shaped item.
2. The initializer wired up via `CollectionDefinition.NewItemInitializer`.

See the GitHub sample referenced above for the full pattern.

## Common Issues

- **No Add/Remove buttons** — `UseCollectionEditor="False"` somewhere; or `AllowAddItems="False"` / `AllowRemoveItems="False"`; or the item type has no parameterless constructor and no `NewItemInitializer`.
- **Add creates an item with all defaults that aren't valid** — provide a `NewItemInitializer` to set meaningful defaults.
- **Remove button missing but Add works** — the framework hides Remove for collections that don't support add. Set `AllowRemoveItemsWithoutNewItemInitializer="True"` if Remove should be available regardless.
- **Collection item properties don't show with `ShowProperties="WithPropertyDefinitions"`** — declare `PropertyDefinition`s inside the `CollectionDefinition` (use `*.` paths or `Type` matching).
- **Edits don't propagate back to the model** — the collection's item type must have settable properties; if the collection itself is rebound, ensure the bound object raises `INotifyPropertyChanged` for the collection property.
- **Multiple item types in one collection** — declare multiple `TypeDefinition` entries in `XamlInitializer` so the **+** button shows a menu.

## Source Material

- `articles/controls-and-libraries/property-grid/property-definitions/collection-definitions.md` (`xref:15719`)
- `articles/controls-and-libraries/property-grid/getting-started/managing-collection-properties.md` (`xref:15718`)
- GitHub: *wpf-propertygrid-add-an-item-to-a-collection-or-a-dictionary*
- GitHub: *wpf-property-grid-specify-custom-collection-edit-actions*

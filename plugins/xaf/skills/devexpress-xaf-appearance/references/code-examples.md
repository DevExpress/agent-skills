# Conditional Appearance — Code Examples

Complete code snippets for common `AppearanceAttribute` patterns.

---

## Highlight Entire Row in ListView

```csharp
using DevExpress.ExpressApp.ConditionalAppearance;

// Highlight entire row red in ListView when Price > 50
[Appearance("HighPrice", AppearanceItemType = "ViewItem", TargetItems = "*",
    Criteria = "Price > 50", Context = "ListView",
    BackColor = "Red", FontColor = "Maroon", Priority = 2)]
public class Product : BaseObject {
    public virtual string Name { get; set; }
    public virtual decimal Price { get; set; }
    public virtual ProductStatus Status { get; set; }
}
```

## Color a Specific Property in ListView

```csharp
[Appearance("SeafoodBlue", AppearanceItemType = "ViewItem",
    TargetItems = "Category",
    Criteria = "Category = 'Seafood'", Context = "ListView",
    FontColor = "Blue")]
public class Product : BaseObject { /* ... */ }
```

## Disable Property Editor in DetailView

```csharp
[Appearance("DisableSpouseName", AppearanceItemType = "ViewItem",
    TargetItems = "SpouseName",
    Criteria = "IsMarried = false", Context = "DetailView",
    Enabled = false)]
public class Contact : BaseObject {
    public virtual bool IsMarried { get; set; }
    public virtual string SpouseName { get; set; }
}
```

## Hide a Layout Group

```csharp
[Appearance("HideAddressGroup", AppearanceItemType = "LayoutItem",
    TargetItems = "Address",
    Criteria = "IsMarried = false", Context = "DetailView",
    Visibility = ViewItemVisibility.Hide)]
public class Contact : BaseObject { /* ... */ }
```

## Disable an Action Based on Object State

The Action ID in `TargetItems` contains the class name (`"Product.Deactivate"`) because the Action is declared via `[Action]` attribute. For Controller-declared Actions, use the ID without the class name (e.g. `"Delete"`).

```csharp
[Appearance("DisableDeactivate", AppearanceItemType = "Action",
    TargetItems = "Product.Deactivate",
    Criteria = "Status = 'Inactive'", Context = "Any",
    Enabled = false)]
public class Product : BaseObject {
    public virtual ProductStatus Status { get; set; }

    [Action(PredefinedCategory.RecordEdit, Caption = "Deactivate",
        AutoCommit = true,
        TargetObjectsCriteria = "Status = 'Active'")]
    public void Deactivate() {
        Status = ProductStatus.Inactive;
    }
}
```

> **Alternative:** When attribute-based criteria are insufficient (e.g., the condition depends on runtime state not expressible in a criteria string), disable the action inside a controller instead:
> ```csharp
> action.Enabled["MyReason"] = false;
> ```
> Use a keyed reason string so other code can independently re-enable its own reasons without overriding yours.

## Method-Based Rule (No Criteria String)

Place `[Appearance]` on a `public bool` method. The rule activates when the method returns `true`.

```csharp
public class Product : BaseObject {
    public virtual decimal Price { get; set; }
    public virtual ProductStatus Status { get; set; }

    [Appearance("CheapActiveGreen", AppearanceItemType = "ViewItem",
        TargetItems = "*", Context = "ListView",
        BackColor = "Green", FontColor = "Black")]
    public bool CheapActiveRule() {
        return Price < 10 && Status == ProductStatus.Active;
    }
}
```

## Multiple Rules on One Class

You can stack multiple `[Appearance]` attributes. `Priority` controls evaluation order when rules conflict.

```csharp
using DevExpress.Drawing;
using DevExpress.ExpressApp.ConditionalAppearance;
[Appearance("HighPrice", AppearanceItemType = "ViewItem", TargetItems = "*",
    Criteria = "Price > 100", Context = "ListView",
    BackColor = "Red", FontColor = "White", Priority = 2)]
[Appearance("MediumPrice", AppearanceItemType = "ViewItem", TargetItems = "*",
    Criteria = "Price > 50 AND Price <= 100", Context = "ListView",
    BackColor = "Yellow", FontColor = "Black", Priority = 1)]
[Appearance("InactiveStrikeout", AppearanceItemType = "ViewItem", TargetItems = "*",
    Criteria = "Status = 'Inactive'", Context = "ListView",
    FontStyle = DXFontStyle.Strikeout, FontColor = "Gray", Priority = 3)]
public class Product : BaseObject {
    public virtual string Name { get; set; }
    public virtual decimal Price { get; set; }
    public virtual ProductStatus Status { get; set; }
}
```

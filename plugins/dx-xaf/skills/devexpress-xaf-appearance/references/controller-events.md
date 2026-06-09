# Conditional Appearance — AppearanceController Events

Programmatic customization of appearance behavior via `AppearanceController` events.

---

## Available Events

| Event | Purpose |
|-------|---------|
| `AppearanceApplied` | Runs after a rule is applied to a UI element. Use to override or reset styling. |
| `CustomApplyAppearance` | Runs before applying. Use to cancel or fully replace the default apply logic. |
| `CollectAppearanceRules` | Runs during rule collection. Use to add rules dynamically at runtime. |

## Interfaces for Accessing Applied Appearance

| Interface | Properties |
|-----------|-----------|
| `IAppearanceFormat` | `FontColor`, `BackColor`, `FontStyle`, `ResetFontColor()`, `ResetBackColor()`, `ResetFontStyle()` |
| `IAppearanceEnabled` | `Enabled`, `ResetEnabled()` |
| `IAppearanceVisibility` | `Visibility`, `ResetVisibility()` |

Cast `e.Item` in the event handler to the appropriate interface.

## Reset Appearance for Selected Row

When a row is selected in a ListView, colored text may become unreadable against the selection highlight. Reset the font color for the selected item:

```csharp
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ConditionalAppearance;
using DevExpress.ExpressApp.Editors;

public class ResetSelectedAppearanceController : ViewController<ListView> {
    private AppearanceController appearanceController;

    protected override void OnActivated() {
        base.OnActivated();
        appearanceController = Frame.GetController<AppearanceController>();
        if (appearanceController != null) {
            appearanceController.AppearanceApplied += OnAppearanceApplied;
        }
    }

    protected override void OnDeactivated() {
        if (appearanceController != null) {
            appearanceController.AppearanceApplied -= OnAppearanceApplied;
        }
        base.OnDeactivated();
    }

    private void OnAppearanceApplied(object sender, ApplyAppearanceEventArgs e) {
        if (e.ItemType == AppearanceItemType.ViewItem.ToString()
            && e.ItemName == "Category"
            && e.ContextObjects.Length > 0
            && View.SelectedObjects.Contains(e.ContextObjects[0])
            && e.Item is IAppearanceFormat format) {
            format.ResetFontColor();
        }
    }
}
```

## Add Rules Dynamically

Use `CollectAppearanceRules` to inject rules that depend on runtime state (e.g. user role, configuration):

```csharp
public class DynamicAppearanceController : ViewController {
    protected override void OnActivated() {
        base.OnActivated();
        var ac = Frame.GetController<AppearanceController>();
        if (ac != null) {
            ac.CollectAppearanceRules += OnCollectRules;
        }
    }

    protected override void OnDeactivated() {
        var ac = Frame.GetController<AppearanceController>();
        if (ac != null) {
            ac.CollectAppearanceRules -= OnCollectRules;
        }
        base.OnDeactivated();
    }

    private void OnCollectRules(object sender, CollectAppearanceRulesEventArgs e) {
        if (e.Name == "Status") { // only inject for the "Status" view item
            // Build criteria with a runtime value — never concatenate user input
            string criteria = CriteriaOperator.Parse(
                "AssignedTo = CurrentUserId()").ToString();
            var rule = new AppearanceAttribute(
                "DynamicOwnerHighlight", "ViewItem", criteria) {
                TargetItems = "*",
                BackColor = "Gold",
                FontColor = "Black",
                Priority = 10
            };
            e.AppearanceRules.Add(rule);
        }
    }
}
```

> **Performance note:** This handler fires every time the appearance engine refreshes for each UI element. Keep it lightweight — avoid database queries or heavy allocations inside the handler.

## Cancel or Override Appearance for Specific Items

Use `CustomApplyAppearance` to prevent default appearance processing and apply platform-specific styling instead. Set `e.Handled = true` to stop the default appearance engine, then manipulate the control directly:

**Blazor example** — add a CSS class to a component:

```csharp
public class CustomAppearanceBlazorController : ViewController {
    private AppearanceController appearanceController;

    protected override void OnActivated() {
        base.OnActivated();
        appearanceController = Frame.GetController<AppearanceController>();
        if (appearanceController != null) {
            appearanceController.CustomApplyAppearance += OnCustomApplyAppearance;
        }
    }

    protected override void OnDeactivated() {
        if (appearanceController != null) {
            appearanceController.CustomApplyAppearance -= OnCustomApplyAppearance;
        }
        base.OnDeactivated();
    }

    private void OnCustomApplyAppearance(object sender, ApplyAppearanceEventArgs e) {
        if (e.ItemName == "Status" && e.Item is DxComponentEditorAdapter adapter) {
            e.Handled = true; // Prevent default appearance processing
            adapter.ComponentModel.SetAttribute("class", "highlighted-field");
        }
    }
}
```

**WinForms example** — set a control property directly:

```csharp
private void OnCustomApplyAppearance(object sender, ApplyAppearanceEventArgs e) {
    if (e.ItemName == "Status" && e.Item is DXPropertyEditor editor
        && editor.Control is TextEdit textEdit) {
        e.Handled = true; // Prevent default appearance processing
        textEdit.Properties.Appearance.BackColor = Color.LightYellow;
    }
}
```

# Controllers — Scope & State

Code snippets for controller activation conditions, action Active/Enabled state, target properties, and PredefinedCategory placement.

---

## Controller Scope via Properties (Constructor)

```csharp
public class ScopedController : ViewController {
    public ScopedController() {
        // Object type — activates for Contact views only
        TargetObjectType = typeof(Contact);
        // View type — ListView, DetailView, DashboardView, or Any
        TargetViewType = ViewType.ListView;
        // Nesting — Root, Nested, or Any
        TargetViewNesting = Nesting.Root;
        // Specific View ID
        TargetViewId = "Contact_ListView";
        // Multiple View IDs (semicolon-separated)
        TargetViewId = "Contact_ListView;Contact_DetailView";
    }
}
```

## Controller Scope via Active Property (Dynamic)

```csharp
public class DynamicScopeController : ViewController {
    protected override void OnActivated() {
        base.OnActivated();
        // Each key is a separate reason — ALL must be true for activation
        Active["OnlyForRoot"] = View.IsRoot;
        Active["NotLookup"] = Frame.Context != TemplateContext.LookupControl;
    }
}
```

## Controller Scope via Generic Type Parameters (Preferred)

```csharp
// Activates only for nested Paycheck List Views
public class PaycheckListController : ObjectViewController<ListView, Paycheck> {
    public PaycheckListController() {
        TargetViewNesting = Nesting.Nested;
    }
}
```

## Action Active (Visibility) vs Enabled (Grayed Out)

```csharp
// Hide an action (invisible in UI)
myAction.Active["MyReason"] = false;
// Show again
myAction.Active.RemoveItem("MyReason");

// Disable an action (visible but grayed out)
myAction.Enabled["MyReason"] = false;
// Enable again
myAction.Enabled.RemoveItem("MyReason");
```

Both `Active` and `Enabled` are `BoolList` — all entries must be `true` for the action to be active/enabled.

## Action Target Properties

Set in constructor or dynamically:

```csharp
var action = new SimpleAction(this, "MyAction", PredefinedCategory.Edit) {
    TargetObjectType = typeof(Task),         // Only for Task views
    TargetViewType = ViewType.ListView,       // Only in List Views
    TargetViewNesting = Nesting.Root,         // Only root views
    TargetObjectsCriteria = "[Status] = 'Active'", // Enable when criteria met
    TargetObjectsCriteriaMode = TargetObjectsCriteriaMode.TrueAtLeastForOne,
    SelectionDependencyType = SelectionDependencyType.RequireMultipleObjects,
};
```

## PredefinedCategory (Action Placement)

| Category | Location |
|----------|----------|
| `PredefinedCategory.Edit` | Edit toolbar |
| `PredefinedCategory.View` | View toolbar |
| `PredefinedCategory.ObjectsCreation` | New/creation area |
| `PredefinedCategory.Save` | Save area |
| `PredefinedCategory.RecordEdit` | Record-level edit |
| `PredefinedCategory.Filters` | Filter area |
| `PredefinedCategory.Export` | Export area |
| `PredefinedCategory.Unspecified` | Default toolbar |
| Custom string | Custom Action Container |

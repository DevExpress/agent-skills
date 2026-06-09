# Business Logic — ObjectSpace Event Handling

Code snippets for subscribing to ObjectSpace events in controllers.

---

## Pre-Save Logic (Committing Event)

```csharp
public class AuditController : ObjectViewController<DetailView, Employee> {
    protected override void OnActivated() {
        base.OnActivated();
        ObjectSpace.Committing += ObjectSpace_Committing;
    }
    protected override void OnDeactivated() {
        ObjectSpace.Committing -= ObjectSpace_Committing;
        base.OnDeactivated();
    }
    void ObjectSpace_Committing(object sender, System.ComponentModel.CancelEventArgs e) {
        var modifiedObjects = ObjectSpace.GetObjectsToSave(false);
        // Perform audit, validation, or enrichment before save
    }
}
```

To abort the commit when a validation condition is not met, set `e.Cancel = true`:

```csharp
void ObjectSpace_Committing(object sender, System.ComponentModel.CancelEventArgs e) {
    foreach (var obj in ObjectSpace.GetObjectsToSave(false)) {
        if (obj is Employee emp && string.IsNullOrWhiteSpace(emp.Email)) {
            e.Cancel = true; // Prevent saving — required field is empty
            throw new UserFriendlyException("Email is required before saving.");
        }
    }
}
```

## React to Property Changes (ObjectChanged Event)

```csharp
public class PriceController : ObjectViewController<DetailView, Order> {
    protected override void OnActivated() {
        base.OnActivated();
        ObjectSpace.ObjectChanged += ObjectSpace_ObjectChanged;
    }
    protected override void OnDeactivated() {
        ObjectSpace.ObjectChanged -= ObjectSpace_ObjectChanged;
        base.OnDeactivated();
    }
    void ObjectSpace_ObjectChanged(object sender, ObjectChangedEventArgs e) {
        if (e.Object is OrderLine line && e.PropertyName == nameof(OrderLine.Quantity)) {
            line.Total = line.Quantity * line.UnitPrice;
            ObjectSpace.SetModified(View.CurrentObject);
        }
    }
}
```

## Event Subscription Rules

- **Always unsubscribe** in `OnDeactivated` to avoid memory leaks and duplicate firing.
- **Root views only**: Subscribe in root views unless explicitly needed for nested views. Use `View.IsRoot` to check.
- Events available on `IObjectSpace`:

| Event | When It Fires |
|-------|--------------|
| `Committing` | Before `CommitChanges` persists data |
| `Committed` | After `CommitChanges` completes |
| `ObjectSaving` | For each object being saved |
| `CustomCommitChanges` | Allows custom save logic |
| `ObjectChanged` | When any property of any object changes |
| `ModifiedChanged` | When `IsModified` transitions between true/false |
| `ObjectDeleting` | Before deletion |
| `CustomDeleteObjects` | Allows custom delete logic |

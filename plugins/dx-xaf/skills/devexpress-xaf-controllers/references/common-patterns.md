# Controllers — Common Patterns

Code snippets for accessing built-in controllers, dependency injection, customizing actions, initializing objects, and accessing UI controls.

---

## Accessing Built-in Controllers

Use `Frame.GetController<T>()` to access any controller in the current Frame:

```csharp
protected override void OnActivated() {
    base.OnActivated();

    // Access the New Action
    var newController = Frame.GetController<NewObjectViewController>();
    if (newController != null) {
        newController.ObjectCreated += OnObjectCreated;
    }

    // Access the Delete Action
    var deleteController = Frame.GetController<DeleteObjectsViewController>();
    deleteController?.DeleteAction.Active["MyReason"] = false; // Hide delete

    // Access the Filter Controller
    var filterController = Frame.GetController<FilterController>();

    // Access the Modifications Controller (Save/Cancel)
    var modController = Frame.GetController<ModificationsController>();
}

protected override void OnDeactivated() {
    var newController = Frame.GetController<NewObjectViewController>();
    if (newController != null) {
        newController.ObjectCreated -= OnObjectCreated;
    }
    base.OnDeactivated();
}
```

### Key Built-in Controllers

| Controller | Actions/Purpose |
|-----------|-----------------|
| `NewObjectViewController` | New Action, `ObjectCreated` event |
| `DeleteObjectsViewController` | Delete Action |
| `ModificationsController` | Save, Cancel Actions |
| `FilterController` | FullTextSearch, SetFilter Actions |
| `ListViewProcessCurrentObjectController` | ProcessCurrentObject (double-click) |
| `ShowNavigationItemController` | Navigation items |
| `LinkUnlinkController` | Link/Unlink in nested views |
| `RecordsNavigationController` | Previous/Next navigation |
| `RefreshController` | Refresh Action |
| `WindowTemplateController` | Window caption, template |
| `ViewNavigationController` | NavigateBack, NavigateForward Actions (view navigation history) |
| `ExportController` | Export Action |

## Dependency Injection in Controllers

XAF controllers support DI in ASP.NET Core Blazor and WinForms:

```csharp
using Microsoft.Extensions.DependencyInjection;

public class NotificationController : ViewController<DetailView> {
    private readonly INotificationService notificationService;

    // Constructor injection
    public NotificationController(INotificationService notificationService) {
        this.notificationService = notificationService;
    }

    // Or property injection via IServiceProvider:
    protected override void OnActivated() {
        base.OnActivated();
        var service = Application.ServiceProvider.GetRequiredService<IMyService>();
    }
}
```

## Customizing a Built-in Action's Items

```csharp
// Remove "Person" from the New Action's dropdown
public class CustomizeNewController : ViewController {
    protected override void OnActivated() {
        base.OnActivated();
        var newController = Frame.GetController<NewObjectViewController>();
        if (newController != null) {
            newController.CollectDescendantTypes += OnCollectDescendantTypes;
        }
    }
    private void OnCollectDescendantTypes(object sender, CollectTypesEventArgs e) {
        e.Types.Remove(typeof(Person));
    }
    protected override void OnDeactivated() {
        var newController = Frame.GetController<NewObjectViewController>();
        if (newController != null) {
            newController.CollectDescendantTypes -= OnCollectDescendantTypes;
        }
        base.OnDeactivated();
    }
}
```

## Initialize Objects Created by New Action

```csharp
public class InitTaskController : ViewController {
    protected override void OnActivated() {
        base.OnActivated();
        var newController = Frame.GetController<NewObjectViewController>();
        if (newController != null)
            newController.ObjectCreated += OnObjectCreated;
    }
    private void OnObjectCreated(object sender, ObjectCreatedEventArgs e) {
        if (e.CreatedObject is Task task) {
            task.StartDate = DateTime.Now;
            task.AssignedTo = ObjectSpace.GetObjectByKey<Employee>(
                SecuritySystem.CurrentUserId);
        }
    }
    protected override void OnDeactivated() {
        var newController = Frame.GetController<NewObjectViewController>();
        if (newController != null)
            newController.ObjectCreated -= OnObjectCreated;
        base.OnDeactivated();
    }
}
```

## Access Underlying UI Controls

### Access a Property Editor's Control

Use `View.FindItem("PropertyName")` to locate a property editor by its property name, cast to the editor type, then access the underlying control. Do this in `OnViewControlsCreated` — controls are not yet created during `OnActivated`:

```csharp
protected override void OnViewControlsCreated() {
    base.OnViewControlsCreated();
    if (View is DetailView detailView) {
        var item = detailView.FindItem("Notes");
        if (item is StringPropertyEditor editor) {
            // Access the underlying platform control
            var control = editor.Control;
            // Customize control properties directly
        }
    }
}
```

> The item ID passed to `FindItem` matches the property name by default. Cast the result to the appropriate editor type (`StringPropertyEditor`, `IntegerPropertyEditor`, `LookupPropertyEditor`, or a platform-specific subclass) to access `editor.Control`.

### Access a List View Editor's Control

```csharp
protected override void OnViewControlsCreated() {
    base.OnViewControlsCreated();
    // Blazor grid example:
    if (View.Editor is DxGridListEditor gridEditor) {
        gridEditor.GridModel.ShowGroupPanel = false;
    }
}
```

## Accessing the XPO Session Inside a Controller

When `DevExpress.Xpo` / `DevExpress.ExpressApp.Xpo` is detected in the project and you need XPO-specific operations (e.g., `Session.Evaluate`, `XPQuery<T>`, direct SQL), cast the controller's `ObjectSpace` to `XPObjectSpace`:

```csharp
using DevExpress.ExpressApp.Xpo;

protected override void OnActivated() {
    base.OnActivated();
    if (ObjectSpace is XPObjectSpace xpObjectSpace) {
        var session = xpObjectSpace.Session;
        // Use session for XPO-specific operations only
        var total = session.Evaluate<decimal>(typeof(Order),
            CriteriaOperator.Parse("Sum(Amount)"), null);
    }
}
```

> **Use `IObjectSpace` for all standard CRUD** (`CreateObject`, `FindObject`, `GetObjectsQuery`, `CommitChanges`, etc.). Cast to `XPObjectSpace.Session` only when you need XPO-specific features not available through the `IObjectSpace` API.

> **Do not create a new `Session` or `UnitOfWork` inside a controller.** The controller's `ObjectSpace` already manages the session lifecycle. Creating an independent `Session` bypasses change tracking, causes "session mixing" exceptions, and breaks the view's data consistency.

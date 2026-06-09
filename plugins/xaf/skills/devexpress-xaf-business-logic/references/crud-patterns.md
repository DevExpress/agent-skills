# Business Logic — CRUD Patterns

Code snippets for creating, reading, updating, and deleting objects via `IObjectSpace`.

---

## Quick Start — Create, Modify, Save

> **Warning:** Never instantiate persistent objects with `new T()`. This bypasses the Object Space lifecycle — `OnCreated()` is not called, the object is not registered for change tracking, and `CommitChanges()` will not persist it. Always use `ObjectSpace.CreateObject<T>()`.

Controller that creates an object and saves it:

```csharp
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;

public class ProjectTaskController : ObjectViewController<ListView, ProjectTask> {
    public ProjectTaskController() {
        var addAction = new SimpleAction(this, "AddDemoTask", PredefinedCategory.Edit) {
            Caption = "Add Demo Task"
        };
        addAction.Execute += AddAction_Execute;
    }

    private void AddAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
        var projectTask = ObjectSpace.CreateObject<ProjectTask>();
        projectTask.Subject = "Demo Task";
        projectTask.DueDate = DateTime.Today.AddDays(7);
        View.CollectionSource.Add(projectTask);
        ObjectSpace.CommitChanges();
        View.Refresh();
    }
}
```

## LINQ Query with GetObjectsQuery

```csharp
// In a controller
var recentOrders = ObjectSpace.GetObjectsQuery<Order>(true)
    .Where(o => o.OrderDate >= DateTime.Today.AddDays(-30))
    .OrderByDescending(o => o.OrderDate)
    .Take(50)
    .ToList();
```

## Soft Delete (Custom Delete Logic)

```csharp
public class SoftDeleteController : ObjectViewController<ListView, Employee> {
    protected override void OnActivated() {
        base.OnActivated();
        ObjectSpace.CustomDeleteObjects += ObjectSpace_CustomDeleteObjects;
    }
    protected override void OnDeactivated() {
        ObjectSpace.CustomDeleteObjects -= ObjectSpace_CustomDeleteObjects;
        base.OnDeactivated();
    }
    void ObjectSpace_CustomDeleteObjects(object sender, CustomDeleteObjectsEventArgs e) {
        foreach (var obj in e.Objects.OfType<Employee>()) {
            obj.IsDeleted = true; // soft-delete flag instead of actual deletion
        }
        ObjectSpace.CommitChanges();
        e.Handled = true;
    }
}
```

## Check for Unsaved Changes Before Navigation

Use `ObjectSpace.IsModified` inside an action handler to detect dirty state before navigating away. Optionally inspect `ModifiedObjects` to identify which objects have pending changes:

```csharp
public class NavigateController : ObjectViewController<DetailView, Employee> {
    public NavigateController() {
        var navigateAction = new SimpleAction(this, "GoToDashboard", PredefinedCategory.View) {
            Caption = "Dashboard"
        };
        navigateAction.Execute += NavigateAction_Execute;
    }

    private void NavigateAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
        if (ObjectSpace.IsModified) {
            // Inspect dirty objects if needed
            var dirtyObjects = ObjectSpace.ModifiedObjects;
            throw new UserFriendlyException(
                $"Save or cancel your changes first ({dirtyObjects.Count} unsaved object(s)).");
        }

        // Safe to navigate — no unsaved changes
        IObjectSpace dashboardOs = Application.CreateObjectSpace(typeof(DashboardReport));
        var report = dashboardOs.CreateObject<DashboardReport>();
        e.ShowViewParameters.CreatedView = Application.CreateDetailView(dashboardOs, report);
    }
}
```

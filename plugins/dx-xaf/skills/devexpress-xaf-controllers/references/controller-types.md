# Controllers — Controller Type Examples

Code snippets for ViewController, ViewController&lt;T&gt;, ObjectViewController, and WindowController.

---

## ViewController (Non-Generic)

Activates for all views:

```csharp
using DevExpress.ExpressApp;

public class MyController : ViewController {
    protected override void OnActivated() {
        base.OnActivated();
        // View, ObjectSpace, Frame are available here
    }
    protected override void OnDeactivated() {
        // Unsubscribe from events, release resources
        base.OnDeactivated();
    }
}
```

## ViewController&lt;ViewType&gt; (Generic)

Constrains activation to a specific View type:

```csharp
// Activates only for List Views
public class MyListController : ViewController<ListView> {
    protected override void OnActivated() {
        base.OnActivated();
        ListView listView = View; // Strongly typed
    }
}

// Activates only for Detail Views
public class MyDetailController : ViewController<DetailView> {
    protected override void OnActivated() {
        base.OnActivated();
        DetailView detailView = View;
    }
}
```

## ObjectViewController&lt;ViewType, ObjectType&gt;

Constrains to both View type and business object type — typed access to current object:

```csharp
using DevExpress.ExpressApp;

public class PersonDetailController : ObjectViewController<DetailView, Person> {
    protected override void OnActivated() {
        base.OnActivated();
        Person person = ViewCurrentObject; // Strongly typed, no cast needed
    }
}

public class TaskListController : ObjectViewController<ListView, Task> {
    protected override void OnActivated() {
        base.OnActivated();
        // Activated only for Task List Views
    }
}
```

## WindowController

Activates when a Window is created. Use for UI-level features not tied to specific Views:

```csharp
using DevExpress.ExpressApp;

public class MainWindowController : WindowController {
    public MainWindowController() {
        TargetWindowType = WindowType.Main; // Main, Child, or Any
    }
    protected override void OnActivated() {
        base.OnActivated();
        Window window = Window; // Access to the Window object
    }
}
```

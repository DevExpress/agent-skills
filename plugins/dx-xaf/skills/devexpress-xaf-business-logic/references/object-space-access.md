# Business Logic — Ways to Access Object Space

Complete code snippets for accessing `IObjectSpace` from different contexts.

---

## In a Controller

```csharp
// ViewController exposes ObjectSpace directly:
public class MyController : ObjectViewController<DetailView, Employee> {
    protected override void OnActivated() {
        base.OnActivated();
        // Use this.ObjectSpace for current view operations
        var emp = (Employee)View.CurrentObject;
    }
}
```

## Creating a New Object Space (Popup Views, Bulk Ops)

```csharp
// In a controller — creates an independent Object Space
IObjectSpace os = Application.CreateObjectSpace(typeof(Employee));
try {
    var emp = os.CreateObject<Employee>();
    emp.FirstName = "John";
    os.CommitChanges();
}
finally {
    os.Dispose(); // Always dispose if not assigned to a View
}
```

## In an EF Core Business Class

```csharp
using DevExpress.ExpressApp;

// BaseObject already implements IObjectSpaceLink and IXafEntityObject.
// XAF assigns the ObjectSpace at runtime via change-tracking proxies.
public class Invoice : BaseObject {
    private IObjectSpace ObjectSpace => ((IObjectSpaceLink)this).ObjectSpace;

    public override void OnCreated() {
        var defaultCustomer = ObjectSpace.FirstOrDefault<Customer>(c => c.IsDefault);
        if (defaultCustomer != null) Customer = defaultCustomer;
    }
}
```

## In Module Updater

```csharp
public class Updater : ModuleUpdater {
    public Updater(IObjectSpace objectSpace, Version currentDBVersion)
        : base(objectSpace, currentDBVersion) { }

    public override void UpdateDatabaseAfterUpdateSchema() {
        base.UpdateDatabaseAfterUpdateSchema();
        // Use the inherited ObjectSpace property directly
        if (ObjectSpace.FirstOrDefault<Department>(d => d.Name == "HQ") == null) {
            var dept = ObjectSpace.CreateObject<Department>();
            dept.Name = "HQ";
        }
        ObjectSpace.CommitChanges();
    }
}
```

## In ASP.NET Core Services (DI)

```csharp
using DevExpress.ExpressApp;

public class MyService {
    readonly IObjectSpaceFactory objectSpaceFactory;

    public MyService(IObjectSpaceFactory factory) {
        objectSpaceFactory = factory;
    }

    public void DoWork() {
        using IObjectSpace os = objectSpaceFactory.CreateObjectSpace(typeof(Employee));
        var employees = os.GetObjectsQuery<Employee>()
            .Where(e => e.Department.Name == "HQ")
            .ToList();
        // ...
        os.CommitChanges();
    }
}
```

- **`IObjectSpaceFactory`** creates Object Spaces that enforce the current user's security permissions — use this for normal application logic.
- **`INonSecuredObjectSpaceFactory`** creates Object Spaces that bypass the XAF security system entirely. Use it only for system-level or background operations that must run outside the current user's security context (e.g., scheduled jobs, data migration, admin auditing).
- **Lifetime:** Never store an `IObjectSpace` in a singleton field. Object Spaces are lightweight and tied to a single unit of work — create them per operation with a `using` block and dispose immediately. Register services that depend on `IObjectSpaceFactory` as scoped or transient.
- **Security context:** When running inside an authenticated request, inject `IObjectSpaceFactory` so the Object Space inherits the current user's permissions. For background services or hosted jobs with no user context, inject `INonSecuredObjectSpaceFactory` instead.

## Working with Objects from Different Object Spaces

When you have an object from another ObjectSpace, import it with `GetObject`:

```csharp
IObjectSpace newOs = Application.CreateObjectSpace(typeof(Employee));
Employee localCopy = (Employee)newOs.GetObject(employeeFromAnotherOs);
localCopy.LastName = "Updated";
newOs.CommitChanges();
newOs.Dispose();
```

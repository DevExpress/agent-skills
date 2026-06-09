# Business Logic — IXafEntityObject Lifecycle Hooks

Code snippets for implementing business logic in EF Core entity lifecycle methods.

---

## Available Hooks

| Method | When It Runs |
|--------|-------------|
| `OnCreated()` | After `CreateObject<T>()` — set defaults |
| `OnLoaded()` | After object is loaded from database |
| `OnSaving()` | Before object is persisted — set timestamps, computed values |

These are virtual methods on `BaseObject` (EF Core). Override them directly.

## Auto-Set Timestamps

```csharp
using DevExpress.ExpressApp;
using DevExpress.Persistent.BaseImpl.EF;

public class Document : BaseObject {
    public virtual string Title { get; set; }
    public virtual DateTime CreatedOn { get; set; }
    public virtual DateTime? ModifiedOn { get; set; }
    public virtual string CreatedBy { get; set; }

    public override void OnCreated() {
        CreatedOn = DateTime.Now;
    }
    public override void OnSaving() {
        ModifiedOn = DateTime.Now;
    }
}
```

## Set Default Values on Creation

```csharp
public class Invoice : BaseObject {
    public virtual DateTime InvoiceDate { get; set; }
    public virtual InvoiceStatus Status { get; set; }
    public virtual string InvoiceNumber { get; set; }

    public override void OnCreated() {
        InvoiceDate = DateTime.Today;
        Status = InvoiceStatus.Draft;
    }
}
```

## Access ObjectSpace in Business Class (IObjectSpaceLink)

`BaseObject` already implements `IObjectSpaceLink`. XAF assigns the Object Space at runtime via change-tracking proxies. Access via explicit cast:

```csharp
public class Order : BaseObject {
    private IObjectSpace ObjectSpace => ((IObjectSpaceLink)this).ObjectSpace;

    public override void OnCreated() {
        var defaultShipper = ObjectSpace.FirstOrDefault<Shipper>(s => s.IsDefault);
        if (defaultShipper != null) Shipper = defaultShipper;
    }
}
```

> **Performance warning:** Database queries in `OnCreated()` execute once per object creation. In bulk scenarios (e.g., seed data in `Updater`, import loops), this causes N+1 round-trips. For bulk operations, query the default value once before the loop and assign it to each object directly.

## Important Notes

- **Do not call `CommitChanges()` inside lifecycle hooks** — they run within the existing save transaction.
- **Object Space in entity methods is the view's Object Space.** Any operation you perform through it (queries, object creation, modifications) directly affects the view's state. Avoid long-running queries, modifying unrelated objects, or triggering re-entrant saves — these can cause unexpected UI refreshes, data inconsistencies, or infinite loops.
- Lifecycle hooks run for both UI-created objects and programmatically created objects.
- For XPO-specific lifecycle patterns (`AfterConstruction`, `OnSaving` override, `Session.Evaluate`), load the `devexpress-xaf-business-logic-xpo` sub-skill.

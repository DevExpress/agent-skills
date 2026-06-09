# XPO Business Logic — Lifecycle Overrides

Code snippets for AfterConstruction, OnSaving, OnDeleting, and OnLoaded in XPO business classes.

---

## Available Overrides

| Method | When It Runs |
|--------|-------------|
| `AfterConstruction()` | After a new object is created — set defaults |
| `OnSaving()` | Before object is persisted — compute values, validate |
| `OnDeleting()` | Before object is deleted — clean up references, archive |
| `OnLoaded()` | After object is loaded from database — initialize transient state |

## AfterConstruction (Object Creation)

Called once when a new object is created (equivalent to `IXafEntityObject.OnCreated`):

```csharp
using DevExpress.Persistent.BaseImpl;
using DevExpress.Xpo;

[DefaultClassOptions]
public class ProjectTask : BaseObject {
    public ProjectTask(Session session) : base(session) { }

    public override void AfterConstruction() {
        base.AfterConstruction();
        CreatedDate = DateTime.Now;
        Status = TaskStatus.New;
        // Session is available here for lookups:
        var defaultCategory = Session.FindObject<Category>(
            CriteriaOperator.FromLambda<Category>(c => c.IsDefault == true));
        if (defaultCategory != null) Category = defaultCategory;
    }

    DateTime fCreatedDate;
    public DateTime CreatedDate {
        get => fCreatedDate;
        set => SetPropertyValue(nameof(CreatedDate), ref fCreatedDate, value);
    }
    // ... other properties
}
```

## OnSaving (Before Persist)

```csharp
protected override void OnSaving() {
    base.OnSaving();
    ModifiedDate = DateTime.Now;
    // Validate or compute derived values before saving
    if (string.IsNullOrEmpty(Code)) {
        Code = GenerateCode();
    }
}
```

## OnDeleting (Before Delete)

```csharp
protected override void OnDeleting() {
    base.OnDeleting();
    // Clean up references, archive, or audit before deletion
    foreach (var item in Session.CollectReferencingObjects(this)) {
        if (item is OrderLine line) {
            line.Product = null;
        }
    }
}
```

## OnLoaded (After Load from DB)

Fires after all persistent properties are populated from the database row. Use it to initialize non-persistent (transient) fields based on loaded data:

```csharp
[NonPersistent]
string fDisplayLabel;
public string DisplayLabel {
    get { return GetPropertyValue(nameof(DisplayLabel), ref fDisplayLabel); }
}

protected override void OnLoaded() {
    base.OnLoaded();
    // Use direct field assignment — not SetPropertyValue:
    fDisplayLabel = $"{FirstName} {LastName} ({Department?.Name})";
}
```

> **Warning:** Never call `SetPropertyValue` inside `OnLoaded()`. It marks the object as dirty immediately after load, causing spurious saves on the next `CommitChanges()`. Always assign the backing field directly (`_field = value`).

## Important Notes

- **Do not call `Session.Save()` or `CommitChanges()` inside lifecycle overrides** — they run within the existing save transaction.
- For EF Core lifecycle patterns (`OnCreated`, `OnSaving` on `BaseObject`), refer to the base `devexpress-xaf-business-logic` skill.
- **Do not implement `IXafEntityObject`** in XPO classes — use these overrides instead (see XAF0024 diagnostic).

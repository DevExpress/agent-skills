# XPO Business Logic — Session & UnitOfWork

Code snippets for Session, UnitOfWork, and NestedUnitOfWork usage in XAF.

---

## Accessing UnitOfWork from XPObjectSpace

```csharp
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo;

// In a controller:
XPObjectSpace xpOs = (XPObjectSpace)ObjectSpace;
UnitOfWork uow = (UnitOfWork)xpOs.Session;
```

> **Important**: Prefer `IObjectSpace` methods over raw `Session` access. Use `Session` only for XPO-specific features not available through `IObjectSpace`.

## Session Key Methods

| Method | Description |
|--------|-------------|
| `Session.FindObject<T>(CriteriaOperator)` | Finds a single object matching criteria |
| `Session.GetObjects(XPClassInfo, CriteriaOperator, SortingCollection, int, bool, bool)` | Low-level object loading |
| `Session.GetObjectByKey<T>(object)` | Loads object by primary key |
| `Session.Save(object)` | Saves a single object immediately |
| `Session.Delete(object)` | Deletes a single object immediately |
| `Session.CollectReferencingObjects(object)` | Finds all objects referencing the given object |
| `Session.Query<T>()` | Returns `XPQuery<T>` for LINQ queries |
| `Session.QueryInTransaction<T>()` | LINQ query that includes uncommitted in-memory changes |

## UnitOfWork — Deferred Saves

Changes accumulate until `CommitChanges()` is called (transactional):

```csharp
using DevExpress.Xpo;

using (UnitOfWork uow = new UnitOfWork()) {
    var person = new Person(uow) { Name = "Mike" };
    var person2 = new Person(uow) { Name = "John" };
    // Nothing saved yet
    uow.CommitChanges(); // All changes persisted in one transaction
}
```

Key differences from `Session`:
- `Session.Save()` persists immediately
- `UnitOfWork.CommitChanges()` persists all accumulated changes at once
- XAF always uses `UnitOfWork` internally (via `XPObjectSpace`)

## NestedUnitOfWork — Nested Transactions

Changes committed to a `NestedUnitOfWork` merge into the parent but are not persisted to the database until the parent commits:

```csharp
using DevExpress.Xpo;

// In XAF, use IObjectSpace.CreateNestedObjectSpace():
IObjectSpace nestedOs = ObjectSpace.CreateNestedObjectSpace();
var projectTask = nestedOs.CreateObject<ProjectTask>();
projectTask.Subject = "Nested Task";
nestedOs.CommitChanges(); // Merges into parent ObjectSpace, NOT to DB
// To persist: parent ObjectSpace.CommitChanges()
```

Use cases:
- Popup dialogs that should only save if user clicks OK
- Complex multi-step operations that may need rollback
- Editing aggregated child objects in separate views

### Rollback — Dispose Without Committing

To discard all changes made in a nested object space, dispose it without calling `CommitChanges()`:

```csharp
IObjectSpace nestedOs = ObjectSpace.CreateNestedObjectSpace();
var projectTask = nestedOs.CreateObject<ProjectTask>();
projectTask.Subject = "Will be discarded";

// Discard: dispose without committing — nothing merges into the parent
nestedOs.Dispose();
// Parent ObjectSpace is completely unaffected; its objects remain unchanged
```

> **Isolation guarantee:** Disposing a nested object space (or its underlying `NestedUnitOfWork`) without committing discards all uncommitted changes. The parent object space retains its original state — no objects are added, modified, or removed.

# Nested (Child) Object Spaces

`IObjectSpace.CreateNestedObjectSpace()` creates a child Object Space whose changes merge into the parent on commit — the parent then commits to the database as a second step. This two-phase pattern is useful for popup detail views or multi-step editing where you want to discard changes without affecting the parent.

**XPO**: Full support — built on `NestedUnitOfWork`. Commit on the nested Object Space writes to the parent; only the parent's `CommitChanges()` persists to the database.

```csharp
// XPO — create a nested Object Space for a popup editor
using IObjectSpace nestedOs = ObjectSpace.CreateNestedObjectSpace();
var copy = nestedOs.GetObject(currentEmployee);
copy.Name = "Updated";
nestedOs.CommitChanges(); // merges into parent, not yet in DB
// Parent ObjectSpace.CommitChanges() later saves to DB
```

**EF Core**: `CreateNestedObjectSpace()` exists in the API but EF Core does not support true nested units of work. Use an independent Object Space instead:

```csharp
// EF Core — use an independent Object Space as the workaround
using IObjectSpace independentOs = Application.CreateObjectSpace(typeof(Employee));
var emp = independentOs.GetObjectByKey<Employee>(key);
emp.Name = "Updated";
independentOs.CommitChanges(); // saves directly to DB

// Refresh the original view to pick up changes
View.ObjectSpace.Refresh();
```

> **Key difference:** A nested Object Space (XPO) commits to the parent and can be rolled back without database impact. An independent Object Space (EF Core workaround) commits directly to the database — there is no intermediate merge step.

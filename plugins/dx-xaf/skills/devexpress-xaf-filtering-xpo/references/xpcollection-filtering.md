# XPO Filtering — XPCollection with Criteria

Code snippets for filtering XPCollection<T> via constructor, Criteria property, sorting, and TopReturnedObjects.

---

## XPCollection with CriteriaOperator

```csharp
using DevExpress.Xpo;
using DevExpress.Data.Filtering;

// Constructor with criteria
var activeEmployees = new XPCollection<Employee>(Session,
    CriteriaOperator.FromLambda<Employee>(e => e.IsActive == true));

// Constructor with programmatic criteria
var salesTeam = new XPCollection<Employee>(Session,
    new GroupOperator(GroupOperatorType.And,
        new BinaryOperator("Department.Name", "Sales"),
        new BinaryOperator("IsActive", true)));

// With sorting (two-step — add SortProperty after construction)
var sorted = new XPCollection<Employee>(Session,
    CriteriaOperator.FromLambda<Employee>(e => e.Department.Name == "Sales"));
sorted.Sorting.Add(new SortProperty("LastName", DevExpress.Xpo.DB.SortingDirection.Ascending));

// Replace existing sort order — clear first, then add new
sorted.Sorting.Clear();
sorted.Sorting.Add(new SortProperty("HireDate", DevExpress.Xpo.DB.SortingDirection.Descending));

// Three-argument constructor — criteria + inline sorting in one call
var invoices = new XPCollection<Invoice>(Session,
    CriteriaOperator.FromLambda<Invoice>(i => i.Status == "Unpaid"),
    new SortProperty("DueDate", DevExpress.Xpo.DB.SortingDirection.Ascending));

// Modify criteria after creation
var collection = new XPCollection<Employee>(Session);
collection.Criteria = CriteriaOperator.FromLambda<Employee>(
    e => e.HireDate >= new DateTime(2024, 1, 1));
// Assigning a new Criteria automatically reloads the collection from the database.
// Set Criteria = null to remove the filter and load all objects:
collection.Criteria = null;

// Top N results — maps to a DB-level TOP (SQL Server) / LIMIT (PostgreSQL, MySQL) clause
collection.TopReturnedObjects = 10;
// Always combine with explicit Sorting for deterministic results —
// without an ORDER BY, the DB may return arbitrary rows each time.

// In a business class — association collection (always use GetCollection)
[Association("Dept-Employees")]
public XPCollection<Employee> Employees => GetCollection<Employee>(nameof(Employees));
```

> Setting `Criteria` or modifying `Sorting` on an `XPCollection` automatically triggers a reload from the database — no explicit `Reload()` call is needed. This means any assignment to `collection.Criteria` or addition to `collection.Sorting` immediately re-queries the data source.

> **Reassign `Criteria` — don't replace the collection.** To update a live-bound collection, reassign `collection.Criteria` in place. The collection reloads automatically and preserves all existing grid/list bindings. Replacing the entire data source breaks bindings and forces the UI control to re-initialize. Use `Session.GetObjects<T>(criteria)` only for one-off reads where an observable, bindable collection is not needed (e.g., batch processing or validation checks).

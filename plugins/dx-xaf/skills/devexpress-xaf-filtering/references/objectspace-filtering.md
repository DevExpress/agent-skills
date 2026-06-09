# Filtering — IObjectSpace Filtering Methods

Code snippets for GetObjects, GetObjectsQuery, FindObject, GetObjectByKey, and GetObjectsCount.

---

## GetObjects — Load with CriteriaOperator

```csharp
using DevExpress.Data.Filtering;

// In a controller:
var activeContacts = ObjectSpace.GetObjects<Contact>(
    CriteriaOperator.FromLambda<Contact>(c => c.IsActive == true));

// With sorting:
var sorted = ObjectSpace.GetObjects<Contact>(
    CriteriaOperator.FromLambda<Contact>(c => c.Department.Name == "Sales"),
    new List<SortProperty> { new SortProperty("LastName", SortingDirection.Ascending) },
    inTransaction: false);
```

## GetObjectsQuery — LINQ Queries (EF Core preferred)

```csharp
// LINQ query via IObjectSpace
var results = ObjectSpace.GetObjectsQuery<Contact>()
    .Where(c => c.IsActive && c.Department.Name == "Sales")
    .OrderBy(c => c.LastName)
    .ToList();

// With projection
var names = ObjectSpace.GetObjectsQuery<Contact>()
    .Where(c => c.Country == "France")
    .Select(c => new { c.FirstName, c.LastName })
    .ToList();
```

## FindObject — Single Object by Criteria

```csharp
// Returns first match or null
var admin = ObjectSpace.FindObject<ApplicationUser>(
    CriteriaOperator.FromLambda<ApplicationUser>(u => u.UserName == "Admin"));

// With inTransaction flag — include uncommitted objects
var pending = ObjectSpace.FindObject<Order>(
    CriteriaOperator.FromLambda<Order>(o => o.Status == "Pending"), inTransaction: true);
```

> **`FindObject<T>` vs `GetObjects<T>`:** Use `FindObject<T>` when exactly one match is expected — it returns the first match or `null`. Note that if multiple objects satisfy the criteria, the result ordering is non-deterministic (no guaranteed sort). Use `GetObjects<T>` when multiple results are possible or when you need a specific ordering via `SortProperty`.

## GetObjectByKey — Load by Primary Key

```csharp
var contact = ObjectSpace.GetObjectByKey<Contact>(contactId);
```

> `GetObjectByKey` retrieves an object directly by its primary key — it checks the identity map (in-memory cache) first, then queries the database. No criteria evaluation is performed, making it the most efficient lookup when the key is known. Use `FindObject<T>` when you need to search by a non-key property via criteria.

## GetObjectsCount — Count without Loading

```csharp
int count = ObjectSpace.GetObjectsCount(typeof(Order),
    CriteriaOperator.FromLambda<Order>(o => o.Status == "Active"));
```

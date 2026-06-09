# XPO Filtering — XPQuery (LINQ to XPO)

Code snippets for type-safe LINQ queries over XPO objects, including aggregation, projection, and XAF controller access.

---

## Basic XPQuery Usage

```csharp
using System.Linq;
using DevExpress.Xpo;

// Access from Session
XPQuery<Customer> customers = Session.Query<Customer>();

// Basic Where
var germanCustomers = Session.Query<Customer>()
    .Where(c => c.Country == "Germany")
    .OrderBy(c => c.ContactName)
    .ToList();

// Complex filter
var recentHighValue = Session.Query<Order>()
    .Where(o => o.OrderDate >= DateTime.Today.AddDays(-30)
             && o.Total > 1000m)
    .OrderByDescending(o => o.OrderDate)
    .Take(20)
    .ToList();

// Multi-level sorting — both levels translate to a server-side ORDER BY clause
var products = Session.Query<Product>()
    .OrderBy(p => p.Category.Name)
    .ThenByDescending(p => p.Price)
    .ToList();
```

## Aggregation with GroupBy

```csharp
var stats = Session.Query<Order>()
    .GroupBy(o => o.Customer.CompanyName)
    .Select(g => new {
        Company = g.Key,
        OrderCount = g.Count(),
        TotalAmount = g.Sum(o => o.Total)
    })
    .Where(x => x.OrderCount > 5)
    .ToList();
```

## Projection

```csharp
var names = Session.Query<Employee>()
    .Where(e => e.IsActive)
    .Select(e => new { e.FirstName, e.LastName, e.Email })
    .ToList();
```

## Any / All on Collections

```csharp
// Any — departments that have at least one active employee
var depsWithActiveEmployees = Session.Query<Department>()
    .Where(d => d.Employees.Any(e => e.IsActive))
    .ToList();

// All — departments where every task is completed
var fullyCompleteDepts = Session.Query<Department>()
    .Where(d => d.Tasks.All(t => t.IsCompleted))
    .ToList();
```

> Both `.Any()` and `.All()` translate to existence/universal sub-queries at the database level — they do not load all related objects into memory.

## QueryInTransaction — Includes Uncommitted Changes

```csharp
var pending = Session.QueryInTransaction<Order>()
    .Where(o => o.Status == OrderStatus.Pending)
    .ToList();
```

> **XPQuery limitations**: Some LINQ operators are not supported (e.g., `Join` between unrelated types, certain string methods). Fall back to `CriteriaOperator` with `Session.GetObjects` when LINQ fails.

## In XAF — Accessing XPQuery from a Controller

```csharp
using DevExpress.ExpressApp.Xpo;

var xpOs = (XPObjectSpace)ObjectSpace;
var session = xpOs.Session;
var results = session.Query<Customer>()
    .Where(c => c.IsActive)
    .ToList();
```

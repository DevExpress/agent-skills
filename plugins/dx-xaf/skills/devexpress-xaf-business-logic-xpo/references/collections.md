# XPO Business Logic — Collections & Data Sources

Code snippets for XPCollection, XPQuery (LINQ to XPO), XPView, and server-mode sources.

---

## XPCollection\<T\>

Lazy-loaded, server-aware collection bound to a `Session`:

```csharp
using DevExpress.Xpo;
using DevExpress.Data.Filtering;

// In a business class — standard pattern for association collections:
[Association("Department-Employees")]
public XPCollection<Employee> Employees {
    get { return GetCollection<Employee>(nameof(Employees)); }
}

// Standalone collection with criteria:
var activeEmployees = new XPCollection<Employee>(Session,
    CriteriaOperator.FromLambda<Employee>(e => e.IsActive == true));

// With sorting:
var sorted = new XPCollection<Employee>(Session,
    CriteriaOperator.FromLambda<Employee>(e => e.Department.Name == "Sales"));
sorted.Sorting.Add(new SortProperty("LastName", DevExpress.Xpo.DB.SortingDirection.Ascending));

// Remove an object from the collection (also clears the association link):
Employee emp = activeEmployees.First();
activeEmployees.Remove(emp);

// Reload from the database (re-fetches all objects matching criteria):
activeEmployees.Reload();
```

> `Reload()` re-queries the database and refreshes the collection contents. Use it when the underlying data may have changed outside the current session (e.g., after another object space committed changes).

## XPQuery\<T\> (LINQ to XPO)

LINQ-based querying over XPO objects:

```csharp
using System.Linq;
using DevExpress.Xpo;

// Create from Session:
XPQuery<Customer> customers = Session.Query<Customer>();

// Standard LINQ operations:
var results = from c in customers
              where c.Country == "Germany"
              orderby c.ContactName
              select c;

// Method syntax:
var topOrders = Session.Query<Order>()
    .Where(o => o.ShippedDate >= DateTime.Today.AddDays(-30))
    .OrderByDescending(o => o.ShippedDate)
    .Take(5)
    .ToList();

// Aggregation:
var stats = Session.Query<Order>()
    .GroupBy(o => o.Customer.CompanyName)
    .Select(g => new { Company = g.Key, Count = g.Count(), Total = g.Sum(o => o.Total) })
    .ToList();
```

### XPQuery\<T\> vs XPCollection\<T\>

| Aspect | `XPQuery<T>` | `XPCollection<T>` |
|--------|-------------|-------------------|
| API style | LINQ composition (`Where`, `Select`, `GroupBy`) | Criteria-based (`CriteriaOperator`) |
| Result type | One-shot query — returns a snapshot (`ToList()`) | Observable, bindable collection — reacts to session changes |
| Change events | None — static result set | Fires `ListChanged` / `CollectionChanged` when objects are added, removed, or modified in the session |
| UI binding | Not directly bindable (use materialized `List<T>`) | Directly bindable to grids and list views |

> **Preference rule in XAF controllers:** Use `IObjectSpace.GetObjectsQuery<T>()` instead of `Session.Query<T>()` — it returns a LINQ queryable scoped to the controller's object space, respecting security filters and change tracking. Reserve `XPQuery<T>` (via `Session.Query<T>()`) for code that operates directly on a `Session` outside the XAF object space layer (e.g., data migration scripts, background jobs).

## XPView

Lightweight read-only data view (does not load full objects — returns raw column values):

```csharp
using DevExpress.Xpo;
using DevExpress.Data.Filtering;

var view = new XPView(Session, typeof(Order));
view.Properties.AddRange(new ViewProperty[] {
    new ViewProperty("CustomerName", SortDirection.Ascending, "[Customer.CompanyName]", true, true),
    new ViewProperty("Total", SortDirection.None, "[Amount]", false, true),
    new ViewProperty("OrderCount", SortDirection.None, "Count()", false, true)
});

// Iterate rows — XPView is read-only (no Add/Remove/CommitChanges):
for (int i = 0; i < view.Count; i++) {
    string customerName = (string)view[i]["CustomerName"];
    decimal total = (decimal)view[i]["Total"];
    int orderCount = (int)view[i]["OrderCount"];
    // Process values...
}
```

> XPView returns raw column values, not persistent objects. Each `view[i]["PropertyName"]` returns `object` — cast to the expected type. The view is **read-only**: you cannot add, remove, or commit objects through it.

> **When to use XPView over XPCollection:** Prefer `XPView` for read-only summary or projection queries against large data sets. Unlike `XPCollection`, which materialises full persistent objects into memory, `XPView` fetches only the requested columns from the database — significantly reducing memory overhead and load time. Use `XPCollection` when you need to modify objects or access their full property graph.

## Server-Mode Data Sources

For large datasets — load data on demand from the server:

| Class | Description |
|-------|-------------|
| `XPInstantFeedbackSource` | Asynchronous data loading for grids, best for read-only scenarios |
| `XPServerCollectionSource` | Server-mode collection with edit support |

> **When to use which:** Use `XPInstantFeedbackSource` for large, read-only data sets where non-blocking async loading is needed — each fetch creates its own session instance (requires a session factory delegate, not a shared session). Use `XPServerCollectionSource` for server-mode scenarios that require editing support or simpler synchronous operation with smaller data sets.

### XPInstantFeedbackSource — Controller Example

```csharp
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Xpo;
using DevExpress.Xpo;

public class InstantFeedbackController : ViewController<ListView> {
    private XPInstantFeedbackSource feedbackSource;

    protected override void OnActivated() {
        base.OnActivated();
        var xpObjectSpace = (XPObjectSpace)ObjectSpace;
        string connectionString = xpObjectSpace.Session.Connection?.ConnectionString
            ?? xpObjectSpace.Session.ConnectionString;

        feedbackSource = new XPInstantFeedbackSource(typeof(Order));
        feedbackSource.ResolveSession += (s, e) => {
            // Each async fetch creates its own UnitOfWork — never share a single session
            e.Session = new UnitOfWork { ConnectionString = connectionString };
        };
        feedbackSource.DismissSession += (s, e) => {
            (e.Session as IDisposable)?.Dispose();
        };

        View.CollectionSource.Collection = feedbackSource;
    }

    protected override void OnDeactivated() {
        feedbackSource?.Dispose();
        feedbackSource = null;
        base.OnDeactivated();
    }
}
```

> **Threading:** `ResolveSession` must return a new `UnitOfWork` (or `Session`) each time — never assign the controller's `ObjectSpace.Session`. XPO calls these events on background threads, so a shared session would cause concurrency exceptions.

# XPO Filtering — XPView & Session Methods

Code snippets for XPView lightweight queries and direct Session.FindObject / Session.GetObjects usage.

---

## XPView — Lightweight Read-Only Queries

Returns raw column values without creating full persistent objects:

```csharp
using DevExpress.Xpo;
using DevExpress.Data.Filtering;

var view = new XPView(Session, typeof(Order));
view.Properties.AddRange(new ViewProperty[] {
    new ViewProperty("CustomerName", SortDirection.Ascending,
        "[Customer.CompanyName]", true, true),
    new ViewProperty("OrderTotal", SortDirection.None,
        "[Total]", false, true),
    new ViewProperty("OrderCount", SortDirection.None,
        "Count()", true, true)
});
view.Criteria = CriteriaOperator.Parse("[OrderDate] >= ?", new DateTime(2024, 1, 1));

foreach (ViewRecord record in view) {
    string name = (string)record["CustomerName"];
    int count = (int)record["OrderCount"];
}
```

## Session.FindObject & Session.GetObjects

Direct Session methods for querying (use when `IObjectSpace` is not available):

```csharp
using DevExpress.Xpo;
using DevExpress.Data.Filtering;

// FindObject — returns first match or null
var admin = Session.FindObject<ApplicationUser>(
    CriteriaOperator.Parse("UserName = ?", "Admin"));

// GetObjects — returns a collection
var activeOrders = Session.GetObjects(
    Session.GetClassInfo<Order>(),
    CriteriaOperator.Parse("[Status] = 'Active'"),
    new SortingCollection(new SortProperty("OrderDate", DevExpress.Xpo.DB.SortingDirection.Descending)),
    topSelectedRecords: 100, // Maps to a DB-level TOP/LIMIT — not a client-side slice
    selectDeleted: false,
    force: false);

// GetObjectByKey — returns the object with the given primary key, or null if not found
var order = Session.GetObjectByKey<Order>(orderId);
// Checks the identity map (session cache) first; only queries the DB if the object is not cached.
// In XAF controllers, prefer IObjectSpace.GetObjectByKey<T>(key) for security filtering.
```

> **`Session.FindObject<T>` vs `IObjectSpace.FindObject<T>`:** `Session.FindObject<T>` bypasses the XAF object space layer — no security permission filtering, no object space events (`ObjectChanged`, `Committed`), and no integration with XAF controllers' lifecycle. In XAF controllers and modules, always use `IObjectSpace.FindObject<T>()` (or `ObjectSpace.FindObject<T>()`) to ensure security filters are applied and changes are tracked. Reserve `Session.FindObject<T>` and `Session.GetObjects` for raw XPO code outside the object space (e.g., data migration scripts, background services without XAF context).

## XPView vs XPQuery\<T\> vs XPCollection\<T\> — Decision Rule

| Aspect | `XPView` | `XPQuery<T>` | `XPCollection<T>` |
|--------|----------|-------------|-------------------|
| **Use case** | Read-only projections, summaries, aggregates | LINQ composition, one-shot queries | Observable, data-bindable, mutable collections |
| **Mutability** | Read-only — no Add/Remove/Commit | Read-only snapshot (`ToList()`) | Full read-write — Add, Remove, modify, CommitChanges |
| **UI binding** | Not directly bindable | Not directly bindable (materialize to `List<T>` first) | Directly bindable — fires `ListChanged` on session changes |
| **Memory cost** | Low — fetches only requested columns, no object materialization | Medium — materializes full persistent objects on enumeration | Medium-High — loads and tracks full persistent objects |
| **Returns** | Raw column values (`object` — must cast) | Typed persistent objects | Typed persistent objects |

**When to prefer each:**

- **`XPView`** — Large data sets where you need only a few columns (reports, dashboards, export). Lowest memory footprint.
- **`XPQuery<T>`** — Complex queries benefiting from LINQ composition (`Where`, `Select`, `GroupBy`, `Join`). Use `IObjectSpace.GetObjectsQuery<T>()` inside XAF controllers; reserve `Session.Query<T>()` for code outside the object space.
- **`XPCollection<T>`** — Live-bound UI lists that must react to session changes (grids, lookups, detail views). The only option when you need Add/Remove or automatic reload on Criteria/Sorting changes.

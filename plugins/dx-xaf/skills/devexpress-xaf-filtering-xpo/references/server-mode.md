# XPO Filtering — Server-Mode Data Sources

Code snippets for XPServerCollectionSource and XPInstantFeedbackSource for large datasets.

---

## XPServerCollectionSource

Supports filtering, sorting, grouping on the server with edit capability:

```csharp
using DevExpress.Xpo;
using DevExpress.Data.Filtering;

var serverSource = new XPServerCollectionSource(Session, typeof(Order));
serverSource.FixedFilterCriteria = CriteriaOperator.FromLambda<Order>(
    o => o.OrderDate >= startDate);
// Bind to a grid or XAF list view
```

> All filtering, sorting, and paging operations on `XPServerCollectionSource` are executed as SQL on the database server — not in memory. Only the rows needed for the current grid page are fetched, making it suitable for tables with hundreds of thousands of records.

## XPInstantFeedbackSource

Asynchronous server-mode for read-only grids — best performance for large lists:

```csharp
using DevExpress.Xpo;

var feedbackSource = new XPInstantFeedbackSource(typeof(Order));
feedbackSource.ResolveSession += (s, e) => {
    e.Session = new UnitOfWork();
};
feedbackSource.DismissSession += (s, e) => {
    ((UnitOfWork)e.Session).Dispose();
};
// Bind to grid
```

> In XAF, server mode is typically set via `IModelListView.DataAccessMode = Server` or `ServerView`, configured in code through the Application Model API.

### Setting DataAccessMode Programmatically

**Option 1 — ModelNodesGeneratorUpdater (applied at model generation time):**

```csharp
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.NodeGenerators;

public class ServerModeUpdater : ModelNodesGeneratorUpdater<ModelListViewNodesGenerator> {
    public override void UpdateNode(ModelNode node) {
        if (node is IModelListView listView && listView.ModelClass?.TypeInfo?.Type == typeof(Order)) {
            listView.DataAccessMode = CollectionSourceDataAccessMode.Server;
        }
    }
}
// Register in Module.Setup: EditorDescriptorsFactory.RegisterListEditorAlias(...); not needed —
// register the updater in AddGeneratorUpdaters:
// public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters) {
//     base.AddGeneratorUpdaters(updaters);
//     updaters.Add(new ServerModeUpdater());
// }
```

**Option 2 — Controller (applied at runtime):**

```csharp
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;

public class ServerModeController : ViewController<ListView> {
    protected override void OnActivated() {
        base.OnActivated();
        ((IModelListView)View.Model).DataAccessMode = CollectionSourceDataAccessMode.Server;
    }
}
```

> **Thread safety:** `ResolveSession` is invoked on background threads. XPO sessions are **not** thread-safe — a single shared session must never be reused across invocations. Each `ResolveSession` call must create and return a new `UnitOfWork` (or `Session`). The corresponding `DismissSession` handler must dispose that session. Reusing a shared session causes concurrency exceptions or data corruption.

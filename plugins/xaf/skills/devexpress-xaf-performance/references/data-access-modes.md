# Data Access Modes — Code Snippets

> **ORM compatibility:** All 7 data access modes — Client, Queryable, Server, ServerView, DataView, InstantFeedback, InstantFeedbackView — are supported by both EF Core and XPO object space providers. No mode is ORM-exclusive.

## Setting Data Access Mode in Code

```csharp
using DevExpress.ExpressApp;

// Via CollectionSource constructor
var collectionSource = new CollectionSource(
    objectSpace, typeof(Product), CollectionSourceDataAccessMode.Server);

// Or programmatically via ModelNodesGeneratorUpdater
public class SetDataAccessModeUpdater : ModelNodesGeneratorUpdater<ModelViewsNodesGenerator> {
    public override void UpdateNode(ModelNode node) {
        var views = (IModelViews)node;
        var listView = (IModelListView)views["Product_ListView"];
        if (listView != null) {
            listView.DataAccessMode = CollectionSourceDataAccessMode.Server;
        }
    }
}
```

## Proxy Objects: Getting Real Business Objects

In ServerView, InstantFeedback, InstantFeedbackView, and DataView modes, `View.CurrentObject` returns a proxy (`ObjectRecord` or `XafDataViewRecord`), not the real business object.

| Mode | Processed Object |
|------|------------------|
| Client, Queryable, Server | Original Object |
| ServerView, InstantFeedback, InstantFeedbackView | `ObjectRecord` |
| DataView | `XafDataViewRecord` |

```csharp
// Get the real business object from a proxy
var realObject = View.ObjectSpace.GetObject(View.CurrentObject);
```

## Non-Persistent Properties in Server Modes

Non-persistent properties are **not displayed** in ServerView, DataView, and InstantFeedbackView modes. In Server, InstantFeedback, and Queryable modes they display but cannot be filtered, sorted, or grouped.

**Solution**: Use `PersistentAlias` to make calculated properties server-evaluable:

```csharp
// EF Core
using DevExpress.ExpressApp.DC;

public class Order : BaseObject {
    public virtual decimal UnitPrice { get; set; }
    public virtual int Quantity { get; set; }

    [PersistentAlias("UnitPrice * Quantity")]
    public decimal Total => (decimal)EvaluateAlias("Total");
}
```

```csharp
// XPO
using DevExpress.Xpo;

public class Order : BaseObject {
    // ...
    [PersistentAlias("UnitPrice * Quantity")]
    public decimal Total => (decimal)EvaluateAlias(nameof(Total));
}
```

# List View Modes — Code Snippets

## Setting Data Access Mode via Generator Updater

`DefaultListViewOptionsAttribute` does **not** have a `DataAccessMode` property. Set data access mode via a `ModelNodesGeneratorUpdater` or controller:

```csharp
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;

public class DataAccessModeUpdater : ModelNodesGeneratorUpdater<ModelViewsNodesGenerator> {
    public override void UpdateNode(ModelNode node) {
        var views = (IModelViews)node;
        if (views["Order_ListView"] is IModelListView listView) {
            listView.DataAccessMode = CollectionSourceDataAccessMode.Server;
        }
    }
}

// Register in module:
public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters) {
    base.AddGeneratorUpdaters(updaters);
    updaters.Add(new DataAccessModeUpdater());
}
```

### EF Core vs XPO Data Access Mode Defaults

- Default for all regular List Views (both EF Core and XPO): **`Client`** mode — all matching objects loaded into memory.
- `Queryable` is the default only for ASP.NET Core Blazor Tree List Views and Lookup List Views, regardless of ORM.
- All 7 modes (`Client`, `Queryable`, `Server`, `DataView`, `ServerView`, `InstantFeedback`, `InstantFeedbackView`) are available for both EF Core and XPO — no modes are exclusive to a single ORM.
- `Client` is appropriate for development and small datasets; evaluate `Server`, `InstantFeedback`, or similar for production.

### InstantFeedback Session Lifecycle

`InstantFeedback` uses a separate session/connection for asynchronous data fetches. The view's `ObjectSpace` session must not be shared with the InstantFeedback source. Objects retrieved in InstantFeedback mode are not materialised as full persistent objects — use `GetObjectByKey` to load the selected object into the current object space. Modifications must be done in a separate `ObjectSpace` because the InstantFeedback source is read-only.

## In-Place Editing

### AllowEdit via Attribute

```csharp
// Positional constructor (AllowEdit is a read-only property, cannot use as named parameter)
[DefaultListViewOptions(MasterDetailMode.ListViewOnly, true, NewItemRowPosition.None)]
public class ProjectTask : BaseObject {
    // AllowEdit = true (second positional parameter)
}
```

### AllowEdit via Controller (BoolList)

```csharp
public class EnableEditController : ObjectViewController<ListView, ProjectTask> {
    protected override void OnActivated() {
        base.OnActivated();
        // AllowEdit is a BoolList, not a simple bool
        View.AllowEdit.SetItemValue("EnableInline", true);
    }
}
```

### IModelView.AllowEdit in Application Model

```csharp
public class EnableEditController : ObjectViewController<ListView, ProjectTask> {
    protected override void OnActivated() {
        base.OnActivated();
        View.Model.AllowEdit = true;
    }
}
```

## Split Layout (MasterDetailMode)

Show ListView and DetailView side-by-side:

```csharp
// Via attribute on business class:
[DefaultListViewOptions(MasterDetailMode.ListViewAndDetailView)]
public class Order : BaseObject { }

// Or via Application Model in code:
public class SplitLayoutController : ObjectViewController<ListView, Order> {
    protected override void OnActivated() {
        base.OnActivated();
        View.Model.MasterDetailMode = MasterDetailMode.ListViewAndDetailView;
        ((IModelSplitLayout)View.Model.SplitLayout).Direction = FlowDirection.Horizontal;
    }
}
```

## Blazor InlineEditMode

Blazor-specific inline editing configuration (distinct from WinForms `AllowEdit`):

```csharp
using DevExpress.ExpressApp.Blazor.SystemModule;

public class BlazorInlineEditController : ObjectViewController<ListView, Order> {
    protected override void OnActivated() {
        base.OnActivated();
        if (View.Model is IModelListViewNewItemRow modelNewRow) {
            modelNewRow.NewItemRowPosition = NewItemRowPosition.Top;
        }
        // Set inline edit mode for the Blazor grid
        if (View.Model is IModelListViewInlineEditMode inlineModel) {
            inlineModel.InlineEditMode = InlineEditMode.Batch;
        }
    }
}
```

Blazor `InlineEditMode` values: `Inline`, `Batch`, `EditForm`, `PopupEditForm`.

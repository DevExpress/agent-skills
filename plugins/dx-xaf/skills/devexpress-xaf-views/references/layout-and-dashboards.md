# Detail View Layout & Dashboard Views — Code Snippets

## DetailViewLayoutAttribute (Code-Based)

Organize properties into groups and tabs:

```csharp
using DevExpress.ExpressApp.Model;

[DefaultClassOptions]
public class Contact : BaseObject {
    public virtual string FirstName { get; set; }
    public virtual string LastName { get; set; }

    [DetailViewLayout("Address", LayoutGroupType.SimpleEditorsGroup, 10)]
    public virtual string Street { get; set; }
    [DetailViewLayout("Address", LayoutGroupType.SimpleEditorsGroup, 10)]
    public virtual string City { get; set; }

    [DetailViewLayout("NotesAndRemarks", LayoutGroupType.TabbedGroup, 100)]
    public virtual string Notes { get; set; }
    [DetailViewLayout("NotesAndRemarks", LayoutGroupType.TabbedGroup, 100)]
    public virtual string Remarks { get; set; }
}
```

The attribute approach affects the default generated layout. Changes made via the Application Model API override it.

### FreezeLayout

Prevent auto-regeneration of layout when business class changes. Set `IModelDetailView.FreezeLayout = true` via a generator updater:

```csharp
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;

public class FreezeLayoutUpdater : ModelNodesGeneratorUpdater<ModelViewsNodesGenerator> {
    public override void UpdateNode(ModelNode node) {
        var views = (IModelViews)node;
        if (views["Contact_DetailView"] is IModelDetailView detailView) {
            detailView.FreezeLayout = true;
            // Preserves custom layout but requires manual addition of new properties
        }
    }
}
```

Can also be set at runtime in a controller (applies to the specific view):

```csharp
public class FreezeLayoutController : ViewController<DetailView> {
    protected override void OnActivated() {
        base.OnActivated();
        View.Model.FreezeLayout = true;
    }
}
```

## Dashboard Views via Application Model

### Creating a DashboardView with Items

```csharp
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;

public class DashboardUpdater : ModelNodesGeneratorUpdater<ModelViewsNodesGenerator> {
    public override void UpdateNode(ModelNode node) {
        var views = (IModelViews)node;
        var dashboard = views.AddNode<IModelDashboardView>("CustomerDashboard");
        dashboard.Caption = "Customer Overview";

        // Left pane: Customer List View
        var listItem = dashboard.Items.AddNode<IModelDashboardViewItem>("CustomerList");
        listItem.View = views["Customer_ListView"] as IModelView;

        // Right pane: Customer Detail View
        var detailItem = dashboard.Items.AddNode<IModelDashboardViewItem>("CustomerDetail");
        detailItem.View = views["Customer_DetailView"] as IModelView;

        // Link items so selecting in the list updates the detail view
        detailItem.ActionsToolbarVisibility = ActionsToolbarVisibility.Hide;
    }
}
```

### Add Navigation Item for Dashboard

```csharp
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;

public class DashboardNavigationUpdater : ModelNodesGeneratorUpdater<NavigationItemNodeGenerator> {
    public override void UpdateNode(ModelNode node) {
        var navigationItems = (IModelRootNavigationItems)node;
        var defaultGroup = navigationItems.Items["Default"];
        if (defaultGroup != null) {
            var navItem = defaultGroup.Items.AddNode<IModelNavigationItem>("CustomerDashboardNav");
            navItem.Caption = "Customer Overview";
            navItem.View = navItem.Application.Views["CustomerDashboard"] as IModelObjectView;
        }
    }
}

// Register both updaters in module:
public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters) {
    base.AddGeneratorUpdaters(updaters);
    updaters.Add(new DashboardUpdater());
    updaters.Add(new DashboardNavigationUpdater());
}
```

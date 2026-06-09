# Filtering — List View Filtering Techniques

Code snippets for CollectionSource.Criteria, ListViewFilterAttribute, ModelNodesGeneratorUpdater, grid-level filtering, and FullTextSearch/FilterController.

---

## CollectionSource.Criteria (Data Source Level)

Filter at the data source level — only matching objects are retrieved from the database:

```csharp
using DevExpress.ExpressApp;
using DevExpress.Data.Filtering;

public class FilterContactsController : ObjectViewController<ListView, Contact> {
    protected override void OnActivated() {
        base.OnActivated();
        // Add a named filter — use unique key to add/remove/replace
        View.CollectionSource.Criteria["ActiveOnly"] =
            CriteriaOperator.FromLambda<Contact>(c => c.IsActive == true);
    }
    protected override void OnDeactivated() {
        // Remove the filter on deactivation
        View.CollectionSource.Criteria.Remove("ActiveOnly");
        base.OnDeactivated();
    }
}
```

Multiple named criteria are combined with AND:

```csharp
View.CollectionSource.Criteria["ByDepartment"] =
    CriteriaOperator.FromLambda<Contact>(c => c.Department.Name == "Sales");
View.CollectionSource.Criteria["ByDate"] =
    CriteriaOperator.Parse("[CreatedDate] >= LocalDateTimeToday()"); // Parse needed for XAF function operators
// Effective filter: ByDepartment AND ByDate
```

## ListViewFilterAttribute (Predefined Filters)

Apply to a business class to create predefined SetFilter Action items:

```csharp
using DevExpress.ExpressApp.SystemModule;
using DevExpress.Persistent.Base;

[DefaultClassOptions]
[ListViewFilter("Today", "GetDate([DueDate]) = LocalDateTimeToday()", true)] // true = selected by default when the List View opens
[ListViewFilter("In three days",
    @"[DueDate] >= ADDDAYS(LocalDateTimeToday(), -3) AND [DueDate] < LocalDateTimeToday()")]
[ListViewFilter("This week",
    @"GetDate([DueDate]) > LocalDateTimeThisWeek() AND
      GetDate([DueDate]) <= ADDDAYS(LocalDateTimeThisWeek(), 5)")]
public class ProjectTask : BaseObject {
    public virtual DateTime DueDate { get; set; }
}
```

## Filters Node via ModelNodesGeneratorUpdater (Code)

Add filter nodes programmatically without attributes:

```csharp
using DevExpress.ExpressApp.Model;
using DevExpress.ExpressApp.Model.Core;
using DevExpress.ExpressApp.SystemModule;

public sealed class MyModule : ModuleBase {
    public override void AddGeneratorUpdaters(ModelNodesGeneratorUpdaters updaters) {
        base.AddGeneratorUpdaters(updaters);
        updaters.Add(new ProjectTaskFilterUpdater());
    }
}

public class ProjectTaskFilterUpdater : ModelNodesGeneratorUpdater<ModelListViewFiltersGenerator> {
    public override void UpdateNode(ModelNode node) {
        var filtersNode = (IModelListViewFilters)node;
        if (((IModelListView)filtersNode.Parent).ModelClass.TypeInfo.Type == typeof(ProjectTask)) {
            var filter = filtersNode.AddNode<IModelListViewFilterItem>("OverdueFilter");
            filter.Criteria = "[DueDate] < LocalDateTimeToday() And [Status] != 'Completed'";
            filter.Caption = "Overdue";
            filter.Index = 0;
        }
    }
}
```

## Grid-Level Filtering

Control grid filtering features from controllers:

```csharp
// Show Auto Filter Row
public class AutoFilterRowController : ViewController<ListView> {
    protected override void OnActivated() {
        base.OnActivated();
        var listViewInfo = (IModelListViewShowAutoFilterRow)View.Model;
        listViewInfo.ShowAutoFilterRow = true;
    }
}

// Show Find Panel (cross-platform, model-level)
public class FindPanelController : ViewController<ListView> {
    protected override void OnActivated() {
        base.OnActivated();
        var listViewInfo = (IModelListViewShowFindPanel)View.Model;
        listViewInfo.ShowFindPanel = true;
    }
}
```

### Blazor — Find Panel via DxGridModel

In Blazor, the Find Panel renders as a DxGrid Search Box. You can further configure it via the `DxGridModel` in `OnViewControlsCreated`:

```csharp
public class BlazorFindPanelController : ViewController<ListView> {
    protected override void OnActivated() {
        base.OnActivated();
        // Enable the Find Panel (cross-platform)
        ((IModelListViewShowFindPanel)View.Model).ShowFindPanel = true;
    }

    protected override void OnViewControlsCreated() {
        base.OnViewControlsCreated();
        if (View.Editor is DxGridListEditor gridEditor) {
            // Blazor-specific: configure search box parse mode
            gridEditor.GridModel.SearchTextParseMode = GridSearchTextParseMode.GroupWordsByOr;
        }
    }
}
```

`GridSearchTextParseMode` values: `GroupWordsByAnd` (default — all words must match), `GroupWordsByOr` (any word matches), `ExactMatch`.

## Empty-Until-Filtered Pattern (Blazor)

Show an empty grid until the user applies a filter — useful for large datasets:

```csharp
public class EmptyUntilFilteredController : ObjectViewController<ListView, MyEntity> {
    private const string FalseCriteriaKey = "FalseCriteria";
    private readonly CriteriaOperator FalseCriteria = CriteriaOperator.Parse("1=0");

    protected override void OnViewControlsCreated() {
        base.OnViewControlsCreated();
        if (View.Editor is DxGridListEditor gridListEditor) {
            View.CollectionSource.Criteria[FalseCriteriaKey] = FalseCriteria;
            gridListEditor.GridModel.FilterCriteriaChanged =
                EventCallback.Factory.Create<GridFilterCriteriaChangedEventArgs>(this, args => {
                    if (ReferenceEquals(args.FilterCriteria, null)) {
                        View.CollectionSource.Criteria[FalseCriteriaKey] = FalseCriteria;
                    } else if (View.CollectionSource.Criteria.ContainsKey(FalseCriteriaKey)) {
                        View.CollectionSource.Criteria.Remove(FalseCriteriaKey);
                    }
                });
        }
    }
}
```

## FullTextSearch Action & FilterController

The `FilterController` provides the built-in `FullTextSearch` and `SetFilter` Actions:

```csharp
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;

public class CustomSearchController : ViewController<ListView> {
    protected override void OnActivated() {
        base.OnActivated();
        var filterController = Frame.GetController<FilterController>();

        // Customize which properties are searched
        filterController.CustomGetFullTextSearchProperties += (s, e) => {
            e.Properties.Add("LastName");
            e.Properties.Add("Email");
            e.Handled = true; // Exclude all other properties
        };
    }
}
```

Access the Actions directly:

```csharp
var filterController = Frame.GetController<FilterController>();
var searchAction = filterController.FullTextFilterAction;
var setFilterAction = filterController.SetFilterAction;
```

### Activate a Named Filter Programmatically

```csharp
protected override void OnActivated() {
    base.OnActivated();
    var filterController = Frame.GetController<FilterController>();
    if (filterController != null) {
        // Find the filter item by its caption (matches the ListViewFilter id/caption)
        var item = filterController.SetFilterAction.Items
            .FirstOrDefault(i => i.Caption == "Today");
        if (item != null) {
            filterController.SetFilterAction.SelectedItem = item;
        }
    }
}
```

> This must be done in `OnActivated` (not the constructor) — `Frame.GetController<T>()` and the filter items are not available until the controller is active.

Use `ListViewFindPanelAttribute` to control the Find Panel per class:

```csharp
[DefaultClassOptions]
[ListViewFindPanel(true)]
public class Contact : BaseObject { }
```

## Filter Builder (Visual Criteria Builder)

### WinForms — Show the Filter Builder Panel

```csharp
public class FilterBuilderController : ViewController<ListView> {
    protected override void OnViewControlsCreated() {
        base.OnViewControlsCreated();
        if (View.Editor is GridListEditor gridEditor) {
            gridEditor.GridView.OptionsView.ShowFilterPanelMode =
                DevExpress.XtraGrid.Views.Base.ShowFilterPanelMode.ShowAlways;
        }
    }
}
```

### Blazor — Enable the Filter Panel

```csharp
public class BlazorFilterPanelController : ViewController<ListView> {
    protected override void OnViewControlsCreated() {
        base.OnViewControlsCreated();
        if (View.Editor is DxGridListEditor gridEditor) {
            gridEditor.GridModel.FilterPanelDisplayMode =
                GridFilterPanelDisplayMode.Always;
        }
    }
}
```

`GridFilterPanelDisplayMode` values: `Always` (always visible), `Default` (hidden until a filter is applied), `Never`.

# Creating Views Programmatically — Code Snippets

Use `XafApplication` methods to create Views. Always create a dedicated `ObjectSpace` before creating a View — do not reuse `this.ObjectSpace` from the controller for new views.

## ListView

```csharp
// Simple: from type
IObjectSpace os = Application.CreateObjectSpace(typeof(Contact));
ListView listView = Application.CreateListView(os, typeof(Contact), true);

// With CollectionSourceBase for custom setup
string listViewId = Application.FindListViewId(typeof(Contact));
CollectionSourceBase collectionSource = Application.CreateCollectionSource(os, typeof(Contact), listViewId);
ListView listView = Application.CreateListView(listViewId, collectionSource, true);
```

## DetailView

```csharp
// New object
IObjectSpace os = Application.CreateObjectSpace(typeof(Contact));
Contact contact = os.CreateObject<Contact>();
DetailView detailView = Application.CreateDetailView(os, contact);

// Existing object by key
IObjectSpace os = Application.CreateObjectSpace(typeof(Contact));
Contact contact = os.GetObjectByKey<Contact>(key);
DetailView detailView = Application.CreateDetailView(os, contact);

// isRoot parameter controls Save/Cancel visibility and ObjectSpace lifecycle
DetailView rootView = Application.CreateDetailView(os, contact, true);  // own ObjectSpace, shows Save/Cancel
DetailView nestedView = Application.CreateDetailView(os, contact, false); // shares parent ObjectSpace, no Save/Cancel

// With specific View ID
DetailView detailView = Application.CreateDetailView(os, "Contact_DetailView", true, contact);
```

## DashboardView

```csharp
IObjectSpace os = Application.CreateObjectSpace();
DashboardView dashboard = Application.CreateDashboardView(os, "MyDashboardView", true);
```

## Non-Persistent Object Views

```csharp
// NonPersistentObjectSpace is created automatically for [DomainComponent] types
IObjectSpace os = Application.CreateObjectSpace(typeof(ReportParameters));
var parameters = os.CreateObject<ReportParameters>();
DetailView view = Application.CreateDetailView(os, parameters);
// Show in popup
var svp = new ShowViewParameters(view);
svp.TargetWindow = TargetWindow.NewModalWindow;
svp.Context = TemplateContext.PopupWindow;
Application.ShowViewStrategy.ShowView(svp, new ShowViewSource(Frame, null));
```

### Non-Persistent List View with ObjectsGetting

```csharp
// Populate a non-persistent List View from external data (e.g., REST API)
// Subscribe in a controller's OnActivated or in module setup
((NonPersistentObjectSpace)objectSpace).ObjectsGetting += (s, e) => {
    var records = FetchFromApi(); // your data source
    e.Objects = records; // populate the collection
};
// Handle CommitChanges if write-back is needed
```

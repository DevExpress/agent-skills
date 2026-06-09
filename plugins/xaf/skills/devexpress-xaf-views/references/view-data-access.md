# Accessing View Data — Code Snippets

## Current Object

```csharp
// In a ViewController:
object currentObj = View.CurrentObject; // null for empty List Views or before data loads

// In an ObjectViewController<DetailView, Contact>:
Contact contact = ViewCurrentObject; // Strongly typed

// Subscribe to changes:
View.CurrentObjectChanged += (s, e) => {
    var newCurrent = View.CurrentObject; // may be null
};
```

## Selected Objects (ListView)

```csharp
// In a ViewController for ListView:
if (View is ListView listView) {
    IList selected = listView.SelectedObjects; // IList of selected objects

    listView.SelectionChanged += (s, e) => {
        // React to selection changes
    };
}

// In an Action Execute handler (preferred for multi-select):
foreach (Contact c in e.SelectedObjects) {
    // Process each selected object
}
```

## CollectionSource (ListView Data)

```csharp
if (View is ListView listView) {
    // Add named filter criteria (keyed so it can be removed independently)
    listView.CollectionSource.Criteria["ActiveOnly"] =
        CriteriaOperator.FromLambda<Contact>(c => c.IsActive == true);

    // Remove a specific filter
    listView.CollectionSource.Criteria.Remove("ActiveOnly");

    // Sorting
    listView.CollectionSource.Sorting.Add(
        new SortProperty("Name", DevExpress.Xpo.DB.SortingDirection.Ascending));

    // Force reload after criteria or data changes
    listView.CollectionSource.ResetCollection();
}
```

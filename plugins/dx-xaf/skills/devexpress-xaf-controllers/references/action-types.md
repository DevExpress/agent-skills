# Controllers — Action Type Examples

Code snippets for SimpleAction, SingleChoiceAction, PopupWindowShowAction, ParametrizedAction, and ActionAttribute.

---

## SimpleAction — Button Click

```csharp
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.Persistent.Base;

public class MarkCompleteController : ObjectViewController<ListView, ProjectTask> {
    public MarkCompleteController() {
        var markComplete = new SimpleAction(this, "MarkComplete", PredefinedCategory.Edit) {
            Caption = "Mark Complete",
            ImageName = "State_Task_Completed",
            ConfirmationMessage = "Mark selected tasks as complete?",
            SelectionDependencyType = SelectionDependencyType.RequireMultipleObjects,
            TargetObjectsCriteria = "Not [IsCompleted]"
        };
        markComplete.Execute += MarkComplete_Execute;
    }

    private void MarkComplete_Execute(object sender, SimpleActionExecuteEventArgs e) {
        foreach (ProjectTask projectTask in e.SelectedObjects.Cast<ProjectTask>()) {
            projectTask.IsCompleted = true;
        }
        ObjectSpace.CommitChanges();
        View.ObjectSpace.Refresh();
    }
}
```

## SingleChoiceAction — Dropdown / Radio List

```csharp
public class FilterByStatusController : ObjectViewController<ListView, Task> {
    private SingleChoiceAction filterAction;

    public FilterByStatusController() {
        filterAction = new SingleChoiceAction(this, "FilterByStatus", PredefinedCategory.Filters) {
            Caption = "Status",
            ItemType = SingleChoiceActionItemType.ItemIsMode // Radio-style
        };
        filterAction.Items.Add(new ChoiceActionItem("All", null));
        filterAction.Items.Add(new ChoiceActionItem("Active", "Active"));
        filterAction.Items.Add(new ChoiceActionItem("Completed", "Completed"));
        filterAction.Execute += FilterAction_Execute;
    }

    private void FilterAction_Execute(object sender, SingleChoiceActionExecuteEventArgs e) {
        string status = e.SelectedChoiceActionItem.Data as string;
        if (status != null) {
            View.CollectionSource.Criteria["StatusFilter"] =
                CriteriaOperator.Parse("[Status] = ?", status);
        } else {
            View.CollectionSource.Criteria.Remove("StatusFilter");
        }
    }
}
```

## PopupWindowShowAction — Show a Popup View

```csharp
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;

public class SelectContactController : ViewController<DetailView> {
    public SelectContactController() {
        var selectAction = new PopupWindowShowAction(this, "SelectContact", PredefinedCategory.Edit) {
            Caption = "Select Contact"
        };
        selectAction.CustomizePopupWindowParams += SelectAction_CustomizePopupWindowParams;
        selectAction.Execute += SelectAction_Execute;
    }

    private void SelectAction_CustomizePopupWindowParams(
            object sender, CustomizePopupWindowParamsEventArgs e) {
        IObjectSpace os = Application.CreateObjectSpace(typeof(Contact));
        e.View = Application.CreateListView(os, typeof(Contact), true);
    }

    private void SelectAction_Execute(
            object sender, PopupWindowShowActionExecuteEventArgs e) {
        // The popup uses its own ObjectSpace, separate from the parent view's ObjectSpace.
        // Use ObjectSpace.GetObject() to obtain a reference valid in the parent object space.
        Contact selected = (Contact)e.PopupWindowViewCurrentObject;
        Contact localCopy = ObjectSpace.GetObject(selected);

        // Now safe to assign localCopy to properties of parent-space objects
        var order = (Order)View.CurrentObject;
        order.Customer = localCopy;
        ObjectSpace.CommitChanges();
    }
}
```

> **Two patterns for popup data in Execute:**
> - **Link to parent object** (shown above): call `ObjectSpace.GetObject(selected)` to import the popup result into the parent `ObjectSpace`, assign it, then commit the parent `ObjectSpace`.
> - **Persist popup data independently**: call `e.PopupWindowView.ObjectSpace.CommitChanges()` to save the popup object's own data directly to the database — useful when the popup object should be persisted on its own (e.g., a standalone `Note` or `LogEntry`) without linking it to a parent-space object.

### CustomizeTemplate — Customise the Popup Window

Use `CustomizeTemplate` to modify the popup window's template (e.g., resize or hide UI elements) after it is created:

```csharp
selectAction.CustomizeTemplate += (s, e) => {
    if (e.Template is IWindowTemplate windowTemplate) {
        // Platform-specific customisation, e.g. set popup size
        windowTemplate.IsSizeable = true;
    }
};
```

## ParametrizedAction — Text Input / Search Box

```csharp
public class SearchController : ViewController<ListView> {
    public SearchController() {
        var searchAction = new ParametrizedAction(this, "QuickSearch",
            PredefinedCategory.View, typeof(string)) {
            Caption = "Search",
            NullValuePrompt = "Enter search text..."
        };
        searchAction.Execute += SearchAction_Execute;
    }

    private void SearchAction_Execute(object sender, ParametrizedActionExecuteEventArgs e) {
        string searchText = e.ParameterCurrentValue as string;
        if (!string.IsNullOrEmpty(searchText)) {
            View.CollectionSource.Criteria["Search"] =
                CriteriaOperator.Parse("Contains([Name], ?)", searchText);
        } else {
            View.CollectionSource.Criteria.Remove("Search");
        }
    }
}
```

## ActionAttribute — On Business Class Methods

For simple data operations that don't need UI access:

```csharp
using DevExpress.Persistent.Base;

[DefaultClassOptions]
public class Task : BaseObject {
    public virtual string Subject { get; set; }
    public virtual bool IsCompleted { get; set; }

    [Action(Caption = "Complete", TargetObjectsCriteria = "Not [IsCompleted]",
        ImageName = "State_Task_Completed")]
    public void Complete() {
        IsCompleted = true;
    }
}
```

> **Limitation**: `ActionAttribute` actions always require a selected object. They cannot access `XafApplication`, Views, or other UI entities. For complex scenarios, use a Controller with Actions instead.

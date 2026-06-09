# Showing Views — Code Snippets

## From Action Execute Handlers (ShowViewParameters)

`ShowViewParameters` has four key properties: `CreatedView`, `TargetWindow`, `Context`, and `Controllers` (a collection to add custom controllers to the target frame).

```csharp
using DevExpress.ExpressApp;

private void MyAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
    IObjectSpace os = Application.CreateObjectSpace(typeof(Contact));
    e.ShowViewParameters.CreatedView = Application.CreateListView(os, typeof(Contact), true);
    e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
    e.ShowViewParameters.Context = TemplateContext.PopupWindow;
    // e.ShowViewParameters.Controllers.Add(myCustomController);
}
```

## ShowViewStrategy.ShowView (Full Call with ShowViewSource)

```csharp
var svp = new ShowViewParameters();
IObjectSpace os = Application.CreateObjectSpace(typeof(Order));
svp.CreatedView = Application.CreateDetailView(os, os.CreateObject<Order>(), true);
svp.TargetWindow = TargetWindow.NewWindow;

Application.ShowViewStrategy.ShowView(svp, new ShowViewSource(Frame, null));
```

For modal popups, `DialogController` is automatically added by the framework and provides Accept/Cancel actions.

## PopupWindowShowAction

```csharp
var popup = new PopupWindowShowAction(this, "ShowPopup", PredefinedCategory.Edit);
popup.CustomizePopupWindowParams += (s, e) => {
    IObjectSpace os = Application.CreateObjectSpace(typeof(Contact));
    e.View = Application.CreateListView(os, typeof(Contact), true);
};
popup.Execute += (s, e) => {
    Contact selected = (Contact)e.PopupWindowViewCurrentObject;
    // Process selected object
};
```

## ShowViewInPopupWindow (No Action Required)

Opens a view in a modal popup without creating a `PopupWindowShowAction`:

```csharp
IObjectSpace os = Application.CreateObjectSpace(typeof(Contact));
DetailView view = Application.CreateDetailView(os, os.CreateObject<Contact>());
Application.ShowViewStrategy.ShowViewInPopupWindow(view,
    okDelegate: () => { os.CommitChanges(); },
    cancelDelegate: () => { /* cancel logic */ });
```

## Frame.SetView (Replace Current View)

Replaces the current view in the existing frame without opening a new window. Lower-level than `ShowViewStrategy.ShowView()` — use for programmatic in-frame navigation. The view's ObjectSpace lifecycle becomes the frame's responsibility after `SetView`.

```csharp
IObjectSpace os = Application.CreateObjectSpace(typeof(Contact));
ListView listView = Application.CreateListView(os, typeof(Contact), true);
Frame.SetView(listView);
```

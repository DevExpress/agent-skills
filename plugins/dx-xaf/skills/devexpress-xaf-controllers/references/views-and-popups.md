# Controllers — Views & Popups

Code snippets for showing views from actions, ShowViewParameters, and DialogController.

---

## ShowViewParameters (from Execute handler)

```csharp
private void MyAction_Execute(object sender, SimpleActionExecuteEventArgs e) {
    IObjectSpace os = Application.CreateObjectSpace(typeof(Contact));
    var contact = os.CreateObject<Contact>();
    DetailView detailView = Application.CreateDetailView(os, contact);

    e.ShowViewParameters.CreatedView = detailView;
    e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;
    e.ShowViewParameters.Context = TemplateContext.PopupWindow;
    // Add DialogController for OK/Cancel buttons
    e.ShowViewParameters.Controllers.Add(
        Application.CreateController<DialogController>());
}
```

## DialogController (Popup OK/Cancel)

```csharp
private void ShowPopup_Execute(object sender, SimpleActionExecuteEventArgs e) {
    IObjectSpace os = Application.CreateObjectSpace(typeof(MyEntity));
    var entity = os.CreateObject<MyEntity>();
    DetailView view = Application.CreateDetailView(os, entity);

    e.ShowViewParameters.CreatedView = view;
    e.ShowViewParameters.TargetWindow = TargetWindow.NewModalWindow;

    var dialogController = Application.CreateController<DialogController>();
    dialogController.SaveOnAccept = true;
    dialogController.AcceptAction.Caption = "Confirm";
    dialogController.Accepting += (s, args) => {
        // Custom validation before accept
        if (string.IsNullOrEmpty(entity.Name)) {
            args.Cancel = true;
            throw new UserFriendlyException("Name is required.");
        }
    };
    e.ShowViewParameters.Controllers.Add(dialogController);
}
```

## DialogController via PopupWindowShowAction.CustomizePopupWindowParams

Inside a `CustomizePopupWindowParams` handler, `e.DialogController` gives direct access to the `DialogController` that will manage the popup's OK/Cancel buttons. Use it to customise captions, subscribe to events, or add extra actions:

```csharp
private void AddNote_CustomizePopupWindowParams(
    object sender, CustomizePopupWindowParamsEventArgs e) {
    IObjectSpace os = Application.CreateObjectSpace(typeof(Note));
    e.View = Application.CreateDetailView(os, os.CreateObject<Note>());

    // Customise existing Accept / Cancel actions
    e.DialogController.AcceptAction.Caption = "Save Note";
    e.DialogController.CancelAction.Caption = "Discard";
    e.DialogController.SaveOnAccept = true;

    // Validate before accepting
    e.DialogController.Accepting += (s, args) => {
        var note = (Note)e.View.CurrentObject;
        if (string.IsNullOrEmpty(note.Text)) {
            args.Cancel = true;
            throw new UserFriendlyException("Note text is required.");
        }
    };

    // Add an extra button to the popup
    var resetAction = new SimpleAction(e.DialogController, "ResetNote", "PopupActions") {
        Caption = "Reset"
    };
    resetAction.Execute += (s, args) => {
        var note = (Note)e.View.CurrentObject;
        note.Text = string.Empty;
    };
}
```

> `e.DialogController` is the preferred way to access the popup's `DialogController` when using `PopupWindowShowAction`. You do not need to create a separate `DialogController` instance — XAF provides one automatically.

## TargetWindow Options

| Value | Behavior |
|-------|----------|
| `TargetWindow.Current` | Replace current view in the same frame |
| `TargetWindow.NewWindow` | Open in a new browser tab / MDI window |
| `TargetWindow.NewModalWindow` | Open as a modal popup dialog |
| `TargetWindow.Default` | Use XAF's default behavior based on context |

## Show a View Outside an Action Execute Handler

When you need to open a view from a lifecycle method, event handler, or service callback where `e.ShowViewParameters` is not available, use `Application.ShowViewStrategy.ShowView`:

```csharp
protected override void OnActivated() {
    base.OnActivated();
    ObjectSpace.Committed += OnCommitted;
}

private void OnCommitted(object sender, EventArgs e) {
    IObjectSpace os = Application.CreateObjectSpace(typeof(AuditLog));
    var log = os.GetObjectsQuery<AuditLog>().OrderByDescending(l => l.Date).First();
    DetailView view = Application.CreateDetailView(os, log);

    var svp = new ShowViewParameters(view);
    svp.TargetWindow = TargetWindow.NewModalWindow;
    Application.ShowViewStrategy.ShowView(svp, new ShowViewSource(Frame, null));
}

protected override void OnDeactivated() {
    ObjectSpace.Committed -= OnCommitted;
    base.OnDeactivated();
}
```

> **Prefer Actions when possible.** DevExpress diagnostic XAF0022 warns that `ShowViewStrategy.ShowView` is intended for advanced scenarios. For standard workflows, use an Action's `e.ShowViewParameters` (see above) or `Application.ShowViewStrategy.ShowViewInPopupWindow(view)` for simple popups. Reserve the `ShowView` + `ShowViewSource` pattern for cases where no Action Execute context is available.

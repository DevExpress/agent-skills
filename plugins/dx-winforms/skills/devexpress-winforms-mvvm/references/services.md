# MVVM Services

Services are the mechanism that lets a ViewModel trigger UI actions (show a dialog, open a file, navigate) without referencing any View classes. The ViewModel works with an interface; the actual UI control implementation is registered in the View.

---

## The Pattern

```
ViewModel  →  calls interface method  →  Service  →  controls UI element
```

Three steps:
1. **Register** the service in the View (`mvvmContext1.RegisterService(...)` or a static `MVVMContext.Register...()` call)
2. **Expose** a service-typed property in the ViewModel (uses `this.GetService<IServiceInterface>()`)
3. **Use** the property in ViewModel methods

---

## Standard DevExpress Services

| Service | Interface | What it does |
|---|---|---|
| **MessageBoxService** | `IMessageBoxService` | Show XtraMessageBox / FlyoutDialog / standard MessageBox |
| **DialogService** | `IDialogService` | Show a custom dialog hosting a child ViewModel |
| **CurrentDialogService** | `ICurrentDialogService` | Control a currently open dialog (close, get result) |
| **CurrentWindowService** | `ICurrentWindowService` | Activate, hide, minimize the current form |
| **WindowService** | `IWindowService` | Open a View in a separate window (XtraForm, RibbonForm, Flyout) |
| **DocumentManagerService** | `IDocumentManagerService` | Create/manage tabs in DocumentManager, XtraTabControl, NavigationFrame |
| **WindowedDocumentManagerService** | `IDocumentManagerService` | Create/manage floating windows or dialog forms |
| **NavigationService** | `INavigationService` | Navigate between Views inside a NavigationFrame; GoBack/GoForward |
| **DispatcherService** | `IDispatcherService` | Marshal an action back to the UI thread from a background task |
| **NotificationService** | `INotificationService` | Show Toast/Alert notifications |
| **SplashScreenService** | `ISplashScreenService` | Show/hide splash screens and wait forms from the ViewModel |
| **OpenFileDialogService** | `IOpenFileDialogService` | Show an Open File dialog |
| **SaveFileDialogService** | `ISaveFileDialogService` | Show a Save File dialog |
| **FolderBrowserDialogService** | `IFolderBrowserDialogService` | Show a Folder Browser dialog |

Several services are already globally registered and ready to use without explicit registration: `MessageBoxService`, `XtraDialogService`, `DispatcherService`, `OpenFileDialogService`, `SaveFileDialogService`, `FolderBrowserDialogService`.

---

## How to Call a Service from a ViewModel

### Example: IMessageBoxService

This is the most common example. The pattern is the same for all other services — only the interface type and method names differ.

**Step 1 — No explicit registration needed** (automatically registered globally):

```csharp
// Already registered; you can optionally override globally:
MVVMContext.RegisterXtraMessageBoxService();
```

Or register locally (this View only):

```csharp
// In the Form constructor or Form.Load handler:
mvvmContext1.RegisterService(MessageBoxService.CreateXtraMessageBoxService(this));
```

**Step 2 — Expose the service in the ViewModel:**

```csharp
// POCO or Code Generator ViewModel
protected IMessageBoxService MessageBoxService =>
    this.GetService<IMessageBoxService>();
```

> `GetService<T>()` is a DevExpress POCO extension method available on any ViewModel that is
> managed by `MVVMContext`. It searches the service container hierarchy (local first, then global).
> It is **not thread-safe** — call only from the UI thread.

**Step 3 — Use the service:**

```csharp
public void Notify() {
    MessageBoxService.ShowMessage("Hello from ViewModel!");
}

public void AskConfirmation() {
    var result = MessageBoxService.ShowMessage(
        "Delete this record?",
        "Confirm",
        MessageButton.YesNo,
        MessageIcon.Warning
    );
    if (result == MessageResult.Yes)
        DeleteSelected();
}
```

---

## IDialogService — Showing a Child ViewModel in a Dialog

```csharp
// ViewModel
protected IDialogService DialogService =>
    this.GetService<IDialogService>();

public void ShowFindDialog() {
    var findVm = ViewModelSource.Create<FindDialogViewModel>();
    var result = DialogService.ShowDialog(
        MessageButton.OKCancel,
        "Find Record",
        findVm                      // child ViewModel; framework resolves its View
    );
    if (result == MessageResult.OK)
        ApplyFilter(findVm.SearchText);
}
```

Register in View:

```csharp
MVVMContext.RegisterXtraDialogService();  // global
// or
mvvmContext1.RegisterService(DialogService.CreateXtraDialogService(this));  // local (title optional)
```

> **.NET 8 SDK-style projects:** the unified `DevExpress.Mvvm` package also carries WPF dialog
> overloads, so `IDialogService.ShowDialog(...)` can fail to compile with a `MessageBoxResult` /
> `PresentationFramework` reference error in a WinForms-only project. If you hit this, add
> `<UseWPF>true</UseWPF>` alongside `<UseWindowsForms>true</UseWindowsForms>` in the `.csproj`.
> `IMessageBoxService.ShowMessage(...)` is unaffected.

---

## IDispatcherService — Updating UI from a Background Task

```csharp
// ViewModel
protected IDispatcherService DispatcherService =>
    this.GetService<IDispatcherService>();

public async Task LoadData() {
    var data = await FetchAsync();
    await DispatcherService.BeginInvoke(() => {
        Items = new ObservableCollection<Item>(data);
        this.RaisePropertyChanged(x => x.Items);
    });
}
```

---

## INavigationService — Navigate Between Views

```csharp
// Register locally (requires NavigationFrame control on the form)
mvvmContext1.RegisterService(NavigationService.Create(navigationFrame1));

// ViewModel
protected INavigationService NavigationService =>
    this.GetService<INavigationService>();

public void GoToOrders() {
    NavigationService.Navigate("OrdersView", param: null, parentViewModel: this);
}

public void GoBack() {
    if (NavigationService.CanGoBack)
        NavigationService.GoBack();
}
```

---

## IWindowService — Open a View in a Separate Window

```csharp
mvvmContext1.RegisterService(WindowService.CreateXtraFormService(this, "Details"));

// ViewModel
protected IWindowService WindowService =>
    this.GetService<IWindowService>();

public void ShowDetails() {
    WindowService.Show("DetailsView", detailsViewModel);
}
```

---

## ISplashScreenService — Show Loading Indicator from ViewModel

```csharp
// Register (requires SplashScreenManager on the form)
mvvmContext1.RegisterService(SplashScreenService.Create(splashScreenManager1));

// ViewModel
protected ISplashScreenService SplashScreenService =>
    this.GetService<ISplashScreenService>();

public async Task Refresh() {
    SplashScreenService.ShowSplashScreen("#Overlay#");  // show overlay form
    try {
        await LoadAsync();
    } finally {
        SplashScreenService.HideSplashScreen();
    }
}
```

Special string IDs: `"#Overlay#"` (Overlay Form), `"#FluentSplashScreen#"` (Fluent Splash Screen), or the name of a custom `SplashScreen1` class.

---

## Custom Services

For custom UI actions not covered by standard services:

1. **Define the interface** (in a shared/ViewModel project):
```csharp
public interface IClipboardService {
    void CopyText(string text);
}
```

2. **Implement it** (in the View project):
```csharp
class ClipboardService : IClipboardService {
    public void CopyText(string text) => Clipboard.SetText(text);
}
```

3. **Register in the View:**
```csharp
mvvmContext1.RegisterService(new ClipboardService());
```

4. **Use in the ViewModel:**
```csharp
protected IClipboardService ClipboardService =>
    this.GetService<IClipboardService>();

public void CopyTitle() {
    ClipboardService.CopyText(Title);
}
```

---

## Service Keys — Disambiguating Multiple Same-Interface Services

When two services implement the same interface, use a string key:

```csharp
// Register with keys
mvvmContext1.RegisterService("primary",   new ServiceA());
mvvmContext1.RegisterService("secondary", new ServiceB());

// Retrieve by key (non-POCO)
IMyService svc = this.GetService<IMyService>("primary");

// POCO attribute approach
[ServiceProperty(Key = "secondary")]
protected virtual IMyService SecondaryService {
    get { throw new NotImplementedException(); }
}
```

---

## Source Material

- Services concept: `https://docs.devexpress.com/content/WindowsForms/113971?md=true`
- Standard Services reference: `https://docs.devexpress.com/content/WindowsForms/114024?md=true`
- Navigation and View Management: `https://docs.devexpress.com/content/WindowsForms/114173?md=true`

# Services

In DX MVVM, **services** are UI-aware bits of functionality that view models can invoke without knowing anything about views. The view declares a service in its `<dxmvvm:Interaction.Behaviors>` collection; the view model resolves the service by interface (`GetService<IMessageBoxService>()`) and calls methods. This keeps view models free of `MessageBox.Show()` and similar view-level calls.

DevExpress ships 25+ predefined services. This page catalogs them by purpose and shows the calling pattern for one (`IMessageBoxService`) — the rest follow the same shape.

## When to Use This Reference

Use this when you need to:

- Show a message box, dialog, file picker, or notification from a view model
- Register a service in XAML and resolve it in the view model
- Pick a service from the predefined catalog
- Use services in each view-model style (compile-time, POCO, `ViewModelBase`, custom)

## How Services Work

1. **Register** the service on a view via `<dxmvvm:Interaction.Behaviors>` (or `dx:Interaction.Behaviors` for some core services).
2. The view model **resolves** it by interface via `GetService<T>()`.
3. **Call** methods on the service from view-model code (typically inside a command).

The view model never references the concrete service type — only the interface. This means:

- The same view-model code works with any implementation of the interface (e.g., `DXMessageBoxService` or `WinUIMessageBoxService`).
- View models are unit-testable: substitute a fake `IMessageBoxService` in tests.

## Predefined Services — Full Catalog

### Message Box Services

| Service | Interface | Use |
|---|---|---|
| `DXMessageBoxService` | `IMessageBoxService` | DevExpress-styled message boxes |
| `WinUIMessageBoxService` | `IMessageBoxService` | Windows 8 Modern UI style |

### Dialog Services

| Service | Interface | Use |
|---|---|---|
| `DialogService` | `IDialogService` | Show a custom view as a modal dialog |
| `WinUIDialogService` | `IDialogService` | Windows 8 Modern UI style |

### Document Manager Services

| Service | Interface | Use |
|---|---|---|
| `WindowedDocumentUIService` | `IDocumentManagerService` | Show documents as windows |
| `DockingDocumentUIService` | `IDocumentManagerService` | Show docked documents |
| `TabbedDocumentUIService` | `IDocumentManagerService` | Show tabbed documents |
| `FrameDocumentUIService` | `IDocumentManagerService` | Show framed documents |
| `TabbedWindowDocumentUIService` | `IDocumentManagerService` | Show tabbed documents in a window |

### File / Folder Dialog Services

| Service | Interface | Use |
|---|---|---|
| `OpenFileDialogService` | `IOpenFileDialogService` | Standard "Open File" dialog |
| `SaveFileDialogService` | `ISaveFileDialogService` | Standard "Save File" dialog |
| `DXOpenFileDialogService` | `IOpenFileDialogService` | DevExpress-styled |
| `DXSaveFileDialogService` | `ISaveFileDialogService` | DevExpress-styled |
| `DXOpenFolderDialogService` | `IOpenFolderDialogService` | DevExpress folder picker |
| `FolderBrowserDialogService` | `IFolderBrowserDialogService` | Standard folder browser |

### Report Services

| Service | Interface | Use |
|---|---|---|
| `GridReportManagerService` | `IReportManagerService` | Export GridControl data via XtraReports |
| `StandaloneReportManagerService` | `IReportManagerService` | Export arbitrary data via XtraReports |

### Window / Navigation Services

| Service | Interface | Use |
|---|---|---|
| `WindowService` | `IWindowService` | Show a view as a window |
| `CurrentWindowService` | `ICurrentWindowService` | Control the hosting window (state, close, hide) |
| `CurrentDialogService` | `ICurrentDialogService` | Same for modal dialogs |
| `FrameNavigationService` | `INavigationService` | Navigate inside a `NavigationFrame` |
| `ViewInjectionService` | `IViewInjectionService` | Inject any VM (+ view) into a target control |
| `NotifyIconService` | `INotifyIconService` | System tray icon |

### Utility Services

| Service | Interface | Use |
|---|---|---|
| `NotificationService` | `INotificationService` | Toast / Windows 8 style notifications |
| `DispatcherService` | `IDispatcherService` | Marshal work onto the UI dispatcher |
| `TaskbarButtonService` | `ITaskbarButtonService` | Customize taskbar buttons |
| `ApplicationJumpListService` | `IApplicationJumpListService` | Jump list customization |
| `SplashScreenManagerService` | `ISplashScreenManagerService` | Show a splash from a view model |
| `LayoutSerializationService` | `ILayoutSerializationService` | Save/restore layout of serializable DX controls |
| `WizardService` | `IWizardService` | MVVM-compliant `Wizard` control usage |
| `UIObjectService` | `IUIObjectService` | Access view-only UI objects from a VM |

## Calling Pattern — IMessageBoxService

This is the universal pattern; every other service works the same way (register in XAML, resolve via `GetService`, call methods).

### Step 1: Register on the View

```xaml
<UserControl x:Class="MyApp.Views.MainView"
             xmlns:dxmvvm="http://schemas.devexpress.com/winfx/2008/xaml/mvvm"
             xmlns:dx="http://schemas.devexpress.com/winfx/2008/xaml/core"
             xmlns:vm="clr-namespace:MyApp.ViewModels">
    <UserControl.DataContext>
        <vm:MainViewModel/>
    </UserControl.DataContext>
    <dxmvvm:Interaction.Behaviors>
        <dx:DXMessageBoxService/>
    </dxmvvm:Interaction.Behaviors>
    ...
</UserControl>
```

The service is now available to the view's `DataContext` (the view model).

### Step 2: Resolve in the View Model

How you resolve depends on the view-model style.

#### Compile-Time (`[GenerateViewModel]`)

Add `ImplementISupportServices = true`:

```csharp
using DevExpress.Mvvm;
using DevExpress.Mvvm.CodeGenerators;

[GenerateViewModel(ImplementISupportServices = true)]
partial class MainViewModel {
    IMessageBoxService MessageBox =>
        ServiceContainer.GetService<IMessageBoxService>(ServiceSearchMode.PreferParents);

    [GenerateCommand]
    void ShowGreeting() => MessageBox.ShowMessage("Hello!");
}
```

The generator emits `ServiceContainer`, `GetService<T>()`, and `GetRequiredService<T>()` methods.

#### `ViewModelBase`

```csharp
public class MainViewModel : ViewModelBase {
    public IMessageBoxService MessageBox => GetService<IMessageBoxService>();

    [Command]
    public void ShowGreeting() => MessageBox.ShowMessage("Hello!");
}
```

`ViewModelBase` exposes `GetService<T>()` directly.

#### POCO

POCOs implement `ISupportServices` automatically when created via `ViewModelSource.Create<T>`:

```csharp
public class MainViewModel : ISupportServices {
    IServiceContainer? serviceContainer;
    IServiceContainer ISupportServices.ServiceContainer =>
        serviceContainer ??= new ServiceContainer(this);

    IMessageBoxService MessageBox =>
        ((ISupportServices)this).ServiceContainer.GetService<IMessageBoxService>();

    public void ShowGreeting() => MessageBox.ShowMessage("Hello!");
}
```

`ViewModelSource.Create<T>()` injects `ServiceContainer` automatically — the boilerplate above is often unnecessary; POCO view models can typically use `this.GetService<IMessageBoxService>()` directly.

### Step 3: Use in a Command

```csharp
[GenerateCommand]
void Delete() {
    var result = MessageBox.ShowMessage(
        "Delete this item?",
        "Confirm",
        MessageButton.YesNo,
        MessageIcon.Question);
    if (result == MessageResult.Yes) {
        DeleteItem();
    }
}
```

The same pattern applies to all services: declare a property that resolves the service, call methods on it inside commands.

## Multiple Services of the Same Type

If you need two `IMessageBoxService` instances on one view (rare, but possible), assign each a `Name`:

```xaml
<dxmvvm:Interaction.Behaviors>
    <dx:DXMessageBoxService x:Name="service1"/>
    <dx:DXMessageBoxService x:Name="service2"/>
</dxmvvm:Interaction.Behaviors>
```

```csharp
IMessageBoxService Service1 => GetService<IMessageBoxService>("service1");
IMessageBoxService Service2 => GetService<IMessageBoxService>("service2");
```

## Parent View Model Service Resolution

A child view model can access services registered on its **parent** view's view model:

```csharp
[GenerateViewModel(ImplementISupportServices = true)]
partial class ChildViewModel {
    IMessageBoxService MessageBox =>
        ServiceContainer.GetService<IMessageBoxService>(ServiceSearchMode.PreferParents);
}
```

`ServiceSearchMode.PreferParents` walks up the view-model hierarchy (via `ISupportParentViewModel`) before falling back to the local container.

See [viewmodel-communication.md](viewmodel-communication.md) for setting up parent-child relationships.

## Services in `App.xaml.cs` — Application-Wide

To make a service available throughout the app, register it on the application:

```xaml
<Application x:Class="MyApp.App"
             xmlns:dxmvvm="http://schemas.devexpress.com/winfx/2008/xaml/mvvm"
             xmlns:dx="http://schemas.devexpress.com/winfx/2008/xaml/core">
    <dxmvvm:Interaction.Behaviors>
        <dx:DXMessageBoxService/>
    </dxmvvm:Interaction.Behaviors>
    <Application.Resources>...</Application.Resources>
</Application>
```

> Application-level services work in some scenarios but are less common than view-level registration. Most teams keep services local to the view that needs them.

## Brief Examples — Other Services

### IDialogService — Show a View as a Modal Dialog

```xaml
<dxmvvm:Interaction.Behaviors>
    <dx:DialogService x:Name="EditDialog">
        <dx:DialogService.ViewTemplate>
            <DataTemplate>
                <local:EditView/>
            </DataTemplate>
        </dx:DialogService.ViewTemplate>
        <dx:DialogService.DialogStyle>
            <Style TargetType="dx:ThemedWindow">
                <Setter Property="Width" Value="400"/>
                <Setter Property="Height" Value="300"/>
                <Setter Property="Title" Value="Edit Item"/>
            </Style>
        </dx:DialogService.DialogStyle>
    </dx:DialogService>
</dxmvvm:Interaction.Behaviors>
```

```csharp
IDialogService EditDialog => GetService<IDialogService>("EditDialog");

[GenerateCommand]
void Edit() {
    var vm = new EditViewModel { Item = SelectedItem };
    var result = EditDialog.ShowDialog(
        MessageButton.OKCancel,
        title: "Edit",
        viewModel: vm);
    if (result == MessageResult.OK) ApplyChanges(vm);
}
```

### IOpenFileDialogService — Pick a File

```xaml
<dxmvvm:Interaction.Behaviors>
    <dx:DXOpenFileDialogService Filter="Text|*.txt|All|*.*"/>
</dxmvvm:Interaction.Behaviors>
```

```csharp
IOpenFileDialogService OpenFile => GetService<IOpenFileDialogService>();

[GenerateCommand]
void OpenDocument() {
    if (OpenFile.ShowDialog()) {
        var path = OpenFile.File.GetFullName();
        // load path
    }
}
```

### IDispatcherService — Marshal to UI Thread

```xaml
<dxmvvm:Interaction.Behaviors>
    <dx:DispatcherService/>
</dxmvvm:Interaction.Behaviors>
```

```csharp
IDispatcherService Dispatcher => GetService<IDispatcherService>();

async Task LoadDataAsync() {
    var data = await _api.FetchAsync();
    Dispatcher.BeginInvoke(() => {
        // safe to touch bound properties here
        Items.Clear();
        foreach (var item in data) Items.Add(item);
    });
}
```

### INavigationService — Frame Navigation

`NavigationFrame` and `FrameNavigationService` live in `DevExpress.Xpf.WindowsUI` and require their own XAML namespaces (not the `dx:` core namespace):

```xaml
<!-- xmlns:dxwui="http://schemas.devexpress.com/winfx/2008/xaml/windowsui"
     xmlns:dxwuin="http://schemas.devexpress.com/winfx/2008/xaml/windowsui/navigation" -->
<dxwui:NavigationFrame x:Name="frame">
    <dxmvvm:Interaction.Behaviors>
        <dxwuin:FrameNavigationService/>
    </dxmvvm:Interaction.Behaviors>
</dxwui:NavigationFrame>
```

```csharp
INavigationService Nav => GetService<INavigationService>();

[GenerateCommand]
void GoToDetails() => Nav.Navigate("DetailsView", parameter: SelectedItem, parentViewModel: this);
```

## Custom Services

For app-specific functionality, create your own service interface and implementation. The skeleton:

```csharp
public interface IMyService {
    void DoWork();
}

public class MyService : ServiceBase, IMyService {
    public void DoWork() {
        // Use AssociatedObject to access the view
        var view = AssociatedObject as FrameworkElement;
        // ...
    }
}
```

Register and use exactly like a predefined service:

```xaml
<dxmvvm:Interaction.Behaviors>
    <local:MyService/>
</dxmvvm:Interaction.Behaviors>
```

```csharp
IMyService MyService => GetService<IMyService>();
```

See `articles/mvvm-framework/services/create-a-custom-service.md` (https://docs.devexpress.com/content/WPF/16920?md=true) for the full pattern.

## Common Issues

- **`GetService<T>()` returns `null`** — service not registered, registered on the wrong view (a child view but resolved from the parent's VM), or the VM doesn't implement `ISupportServices`. Check all three.
- **Service "found" but `AssociatedObject` is null** — service was resolved before the view loaded. Resolve services lazily (via a property getter) rather than in the VM constructor.
- **Compile-time VM has no `ServiceContainer`** — missing `ImplementISupportServices = true` on `[GenerateViewModel]`.
- **`IDialogService` shows blank window** — `ViewTemplate` not specified, or the bound `DataContext` doesn't match what `EditView` expects.
- **Multi-instance services collide** — must use `x:Name` on each + `GetService<T>("name")`.
- **`DispatcherService.BeginInvoke` deadlocks** — used `Invoke` (sync) when on the UI thread already. Prefer `BeginInvoke`.
- **`INavigationService.Navigate` doesn't switch view** — view name doesn't match what's registered with the `NavigationFrame`. Check the view name registration.

## Source Material

- `articles/mvvm-framework/services.md` (https://docs.devexpress.com/content/WPF/17414?md=true)
- `articles/mvvm-framework/services/getting-started.md` (https://docs.devexpress.com/content/WPF/17444?md=true)
- `articles/mvvm-framework/services/predefined-set.md` (https://docs.devexpress.com/content/WPF/113931?md=true)
- `articles/mvvm-framework/services/services-in-viewmodelbase-descendants.md` (https://docs.devexpress.com/content/WPF/17446?md=true)
- `articles/mvvm-framework/services/services-in-generated-view-models.md` (https://docs.devexpress.com/content/WPF/17447?md=true)
- `articles/mvvm-framework/services/services-in-custom-viewmodels.md` (https://docs.devexpress.com/content/WPF/17450?md=true)
- `articles/mvvm-framework/services/create-a-custom-service.md` (https://docs.devexpress.com/content/WPF/16920?md=true)
- `articles/mvvm-framework/services/predefined-set/dxmessageboxservice.md` (https://docs.devexpress.com/content/WPF/17415?md=true)

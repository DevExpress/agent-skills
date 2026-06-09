# View-Model Communication

The DevExpress MVVM Framework provides three mechanisms for passing data between view models, each suited to a different relationship:

| Mechanism | Purpose | Coupling |
|---|---|---|
| **`ISupportParameter`** | Pass initial / context data when a child view is shown | Caller knows the child VM |
| **`ISupportParentViewModel`** | Give a child VM a reference to its parent VM (and walk-up service access) | Parent-child container relationship |
| **`Messenger`** | Broadcast notifications between any view models | Loosely coupled (pub/sub) |

## When to Use This Reference

Use this when you need to:

- Hand a detail view its selected item from a master view
- Let a child VM call a service registered on the parent's view
- Let a status-bar VM react to events from any other VM without direct references
- Pick the right communication mechanism for a given scenario

## Picker — Which Mechanism Fits?

| Scenario | Use |
|---|---|
| MasterDetailView, NavigationFrame, or any child view that needs initial data | `ISupportParameter` |
| Child VM needs to call a service from the parent's view (`IMessageBoxService`, `IDialogService`, ...) | `ISupportParentViewModel` |
| Child VM needs to read parent state (e.g., selected item, current user) | `ISupportParentViewModel` |
| One VM needs to notify many unrelated VMs (e.g., "data refreshed") | `Messenger` |
| Decoupled status / log / toast updates from any layer | `Messenger` |

You can combine them — a child VM commonly uses both `ISupportParameter` (for initial data) and `ISupportParentViewModel` (for service walk-up).

## 1. `ISupportParameter` — Pass Initial Data

Implement `ISupportParameter` on a view model to receive a parameter when the view is loaded. When the framework changes the parameter, it sets the `Parameter` property — and if the VM derives from `ViewModelBase`, it also calls `OnParameterChanged(object)`.

### View Model (compile-time)

```csharp
using DevExpress.Mvvm.CodeGenerators;

[GenerateViewModel]
partial class DetailViewModel {
    [GenerateProperty]
    Product current;

    public object Parameter {
        get => Current;
        set {
            Current = value as Product;
            OnParameterChanged();
        }
    }

    void OnParameterChanged() {
        // Initialize based on the new parameter — e.g., load related data
    }
}
```

> The compile-time generator does not auto-implement `ISupportParameter` — declare the `Parameter` property manually.

### View Model (`ViewModelBase`)

```csharp
using DevExpress.Mvvm;

public class DetailViewModel : ViewModelBase {
    public Product Current { get; private set; }

    protected override void OnParameterChanged(object parameter) {
        Current = parameter as Product;
        // Reload, refresh, etc.
    }
}
```

`ViewModelBase` implements `ISupportParameter` for you. Override `OnParameterChanged(object)` and read `Parameter` via `GetParameter()` or use the override's argument.

### View Model (runtime POCO)

```csharp
public class DetailViewModel {
    public virtual Product Current { get; set; }

    protected virtual void OnParameterChanged(object parameter) {
        Current = parameter as Product;
    }
}
```

The runtime POCO generator detects the `OnParameterChanged(object)` method by name.

### Setting the Parameter — XAML

The framework attached property is `ViewModelExtensions.Parameter`. The most common pattern: a master view binds the detail view's parameter to its own selected item.

```xaml
<UserControl xmlns:dxmvvm="http://schemas.devexpress.com/winfx/2008/xaml/mvvm"
             xmlns:vm="clr-namespace:MyApp.ViewModels"
             xmlns:v="clr-namespace:MyApp.Views">
    <UserControl.DataContext>
        <vm:MasterViewModel/>
    </UserControl.DataContext>

    <Grid x:Name="Root">
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width="200"/>
            <ColumnDefinition Width="*"/>
        </Grid.ColumnDefinitions>

        <ListBox ItemsSource="{Binding Items}"
                 SelectedItem="{Binding Selected}"
                 Grid.Column="0"/>

        <!-- Detail view's DataContext is set elsewhere (or it has its own).
             Pass the master's Selected as the detail VM's Parameter. -->
        <v:DetailView Grid.Column="1"
                      dxmvvm:ViewModelExtensions.Parameter="{Binding Selected}"/>
    </Grid>
</UserControl>
```

### Setting the Parameter — Code

```csharp
DetailViewModel detail = new DetailViewModel();
ViewModelHelper.SetParameter(detail, selectedProduct);   // ViewModelBase / POCO
// or assign directly if the VM exposes a settable Parameter property
detail.Parameter = selectedProduct;
```

### Container-Aware Parameter Passing

Both `IDocumentManagerService` and `INavigationService` accept a `parameter` argument when creating a document or navigating:

```csharp
// Document manager — parameter forwarded to the document's view model
documentManagerService.CreateDocument(
    documentType: "DetailView",
    parameter: selectedProduct).Show();
```

```csharp
// Navigation service — parameter forwarded to the navigated view model
navigationService.Navigate(
    viewName: "DetailView",
    parameter: selectedProduct,
    parentViewModel: this);
```

In both cases the child VM receives the parameter through `ISupportParameter` — no extra wiring needed.

### Picker for Property Style

| VM strategy | Parameter mechanism |
|---|---|
| Compile-time `[GenerateViewModel]` | Declare `public object Parameter { get; set; }` manually and react to set |
| `ViewModelBase` | Override `OnParameterChanged(object)` — `Parameter` is already implemented |
| Runtime POCO | Define `protected virtual void OnParameterChanged(object)` method (detected by name) |

## 2. `ISupportParentViewModel` — Parent-Child Relationship

Implement `ISupportParentViewModel` so a child VM knows its parent. This unlocks two patterns:

1. The child can read parent state (selected item, current user, settings).
2. The child can call services registered on the parent's view via `ServiceSearchMode.PreferParents`.

The framework sets `ParentViewModel` automatically when the child is created inside a navigation / document container, or when `ViewModelExtensions.ParentViewModel` is bound in XAML.

### View Model (compile-time)

The compile-time generator can implement `ISupportParentViewModel` for you:

```csharp
using DevExpress.Mvvm.CodeGenerators;

[GenerateViewModel(ImplementISupportParentViewModel = true)]
partial class DetailViewModel {
    partial void OnParentViewModelChanged(object parentViewModel) {
        // React to parent assignment — e.g., subscribe to parent events
        if (parentViewModel is MasterViewModel master) {
            master.RefreshRequested += OnParentRefresh;
        }
    }
}
```

Without `ImplementISupportParentViewModel = true`, write the property manually.

### View Model (`ViewModelBase`)

```csharp
public class DetailViewModel : ViewModelBase {
    protected override void OnParentViewModelChanged(object parentViewModel) {
        // Walk-up will now find services on the parent's view
    }
}
```

### View Model (runtime POCO)

```csharp
public class DetailViewModel {
    protected virtual void OnParentViewModelChanged(object parentViewModel) {
        // ...
    }
}
```

### Setting the Parent — XAML

```xaml
<UserControl xmlns:dxmvvm="http://schemas.devexpress.com/winfx/2008/xaml/mvvm">
    <Grid x:Name="Root">
        <v:DetailView dxmvvm:ViewModelExtensions.ParentViewModel="{Binding DataContext,
                                                                  ElementName=Root}"/>
    </Grid>
</UserControl>
```

`{Binding DataContext, ElementName=Root}` resolves to the master VM. Setting it on the detail view's element propagates the parent into the detail's VM via `ISupportParentViewModel`.

### Setting the Parent — Code

```csharp
DetailViewModel detail = new DetailViewModel();
ViewModelHelper.SetParentViewModel(detail, this);
```

Both `IDocumentManagerService.CreateDocument` and `INavigationService.Navigate` accept a `parentViewModel` argument and set it for you.

### Service Walk-Up via `ServiceSearchMode.PreferParents`

A child VM that wants to use a service registered on the **parent's** view (e.g., the main window's `IMessageBoxService`) resolves it with `ServiceSearchMode.PreferParents`:

```csharp
[GenerateViewModel(
    ImplementISupportServices = true,
    ImplementISupportParentViewModel = true)]
partial class DetailViewModel {
    IMessageBoxService MessageBox =>
        ServiceContainer.GetService<IMessageBoxService>(ServiceSearchMode.PreferParents);

    [GenerateCommand]
    void Delete() {
        if (MessageBox.ShowMessage("Delete this item?", "Confirm",
                                   MessageButton.YesNo) == MessageResult.Yes) {
            // Delete
        }
    }
}
```

The framework first searches the child's own services, then walks up the `ISupportParentViewModel` chain to the parent's services. This means the child doesn't have to register its own `IMessageBoxService` — it just borrows the main window's.

> `ServiceContainer.GetService<T>(ServiceSearchMode.PreferParents)` is the canonical form. `ViewModelBase.GetService<T>(ServiceSearchMode.PreferParents)` works too.

### Timing — `OnParentViewModelChanged` and View Loading

`OnParentViewModelChanged` fires as soon as the framework assigns the parent. If you need to access parent services **after** the parent view has fully loaded, defer the work — `IDispatcherService.BeginInvoke` or wait for an explicit "ready" signal from the parent VM.

## 3. `Messenger` — Loosely Coupled Pub/Sub

`Messenger.Default` is a global publisher/subscriber bus. Any VM can `Send<TMessage>(message)`; any subscriber that called `Register<TMessage>(recipient, callback)` receives it. The sender and receiver have no direct reference to each other.

### Basic Usage

```csharp
using DevExpress.Mvvm;

// --- Sender ---
[GenerateViewModel]
partial class CartViewModel {
    [GenerateCommand]
    void AddItem(Product product) {
        // ...
        Messenger.Default.Send(new ItemAddedMessage(product));
    }
}

public record ItemAddedMessage(Product Product);
```

```csharp
// --- Receiver ---
[GenerateViewModel]
partial class StatusBarViewModel {
    [GenerateProperty]
    string status = "";

    public StatusBarViewModel() {
        Messenger.Default.Register<ItemAddedMessage>(this, OnItemAdded);
    }

    void OnItemAdded(ItemAddedMessage message) {
        Status = $"Added {message.Product.Name}";
    }
}
```

### Strongly-Typed Tokens

Use `Send<T>(message, token)` / `Register<T>(recipient, token, callback)` to scope a message to a particular channel:

```csharp
Messenger.Default.Send(new RefreshMessage(), token: "orders");

Messenger.Default.Register<RefreshMessage>(this, token: "orders", OnRefresh);
```

Only receivers registered for the same token are invoked.

### Cleaning Up — `Unregister`

```csharp
Messenger.Default.Unregister<ItemAddedMessage>(this, OnItemAdded);
Messenger.Default.Unregister(this);   // remove every registration for this recipient
```

### Custom Messenger Instances

`Messenger.Default` is a convenient singleton. For per-feature isolation (e.g., unit tests, sub-applications), create your own instance:

```csharp
Messenger appMessenger = new Messenger();
appMessenger.Register<RefreshMessage>(this, OnRefresh);
appMessenger.Send(new RefreshMessage());
```

Inject it via constructor or a service — don't rely on `Messenger.Default` everywhere if you need testability or scope control.

### Weak References — The Gotcha

By default, `Messenger.Default` holds **weak references** to recipients. If the recipient is garbage-collected, the handler stops firing — no error, just silence.

**Symptoms**:

- Register handler in a temporary VM that goes out of scope → never fires.
- Register handler in an anonymous lambda → fires for a while, then stops.

**Fix**: keep a strong reference to the recipient (typically the recipient VM is itself owned by something else, like a window's DataContext). Don't `Register` anonymous lambdas — use instance methods so the recipient is the VM, which the framework keeps alive.

```csharp
// BAD — lambda capture, no strong reference to "this" in the closure
Messenger.Default.Register<Msg>(this, m => DoSomething(m));

// GOOD — instance method; recipient is the VM
Messenger.Default.Register<Msg>(this, OnMessage);
void OnMessage(Msg m) { DoSomething(m); }
```

### When *Not* to Use Messenger

- **Tightly coupled parent-child**: use `ISupportParameter` or `ISupportParentViewModel`.
- **Sequential workflow**: when an explicit method call or service is clearer than an indirect message.
- **High-frequency events**: messenger overhead and the weak-reference machinery can hurt perf — prefer a direct interface.

Use Messenger for genuinely orthogonal layers: status bar reacting to any change, plugin notifications, undo/redo broadcasts, app-wide settings changes.

## Combined Pattern — Detail View with Parent Services

A common shape: master-detail with the detail VM receiving the selected item AND calling a service registered on the main window.

### Main Window XAML

```xaml
<dx:ThemedWindow xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                 xmlns:dx="http://schemas.devexpress.com/winfx/2008/xaml/core"
                 xmlns:dxmvvm="http://schemas.devexpress.com/winfx/2008/xaml/mvvm"
                 xmlns:vm="clr-namespace:MyApp.ViewModels"
                 xmlns:v="clr-namespace:MyApp.Views">
    <dxmvvm:Interaction.Behaviors>
        <dx:DXMessageBoxService/>          <!-- Registered on the main window -->
    </dxmvvm:Interaction.Behaviors>

    <dx:ThemedWindow.DataContext>
        <vm:MasterViewModel/>
    </dx:ThemedWindow.DataContext>

    <Grid x:Name="Root">
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width="200"/>
            <ColumnDefinition Width="*"/>
        </Grid.ColumnDefinitions>

        <ListBox ItemsSource="{Binding Items}"
                 SelectedItem="{Binding Selected}"
                 Grid.Column="0"/>

        <v:DetailView Grid.Column="1"
                      dxmvvm:ViewModelExtensions.Parameter="{Binding Selected}"
                      dxmvvm:ViewModelExtensions.ParentViewModel="{Binding DataContext,
                                                                  ElementName=Root}"/>
    </Grid>
</dx:ThemedWindow>
```

### Detail View Model

```csharp
[GenerateViewModel(
    ImplementISupportServices = true,
    ImplementISupportParentViewModel = true)]
partial class DetailViewModel {
    [GenerateProperty]
    Product current;

    public object Parameter {
        get => Current;
        set => Current = value as Product;
    }

    IMessageBoxService MessageBox =>
        ServiceContainer.GetService<IMessageBoxService>(ServiceSearchMode.PreferParents);

    [GenerateCommand]
    void Delete() {
        if (Current is null) return;
        if (MessageBox.ShowMessage($"Delete {Current.Name}?", "Confirm",
                                   MessageButton.YesNo) == MessageResult.Yes) {
            // perform delete
        }
    }
}
```

The detail VM gets the selected `Product` via `ISupportParameter` and reaches the main window's `IMessageBoxService` via `ISupportParentViewModel` + `ServiceSearchMode.PreferParents`.

## Common Issues

- **`OnParameterChanged` never fires** — `ViewModelExtensions.Parameter` was bound with the wrong source. The binding source should be the master VM (or an element that holds it), not the detail VM. Use `{Binding Selected, ElementName=Root}` not bare `{Binding Selected}` when the detail view has its own DataContext.
- **`GetService<T>(ServiceSearchMode.PreferParents)` returns `null`** — `ISupportParentViewModel` wasn't set on the child, or the parent's view doesn't have the service registered. Bind `ViewModelExtensions.ParentViewModel` on the child view's element, and verify the parent has `<dxmvvm:Interaction.Behaviors>` with the service.
- **Messenger handler stops firing after a while** — recipient was garbage-collected (weak reference). Register on a long-lived VM (window DataContext, app-wide singleton), or pass `keepTargetAlive: true` to `Register`.
- **Multiple identical messages** — recipient was registered more than once (e.g., in a re-created VM ctor with the same instance held alive). `Unregister` first, or guard with a flag.
- **Compile-time generator doesn't add `Parameter` property** — generator does not auto-implement `ISupportParameter`; declare the property manually (only `ISupportParentViewModel` is auto-generated via `ImplementISupportParentViewModel = true`).
- **Parent VM is `null` in `OnParentViewModelChanged`** — initial assignment can be `null` (the framework calls the handler on assignment / reassignment). Guard with a null check.

## Source Material

- `articles/mvvm-framework/viewmodels/passing-data-between-viewmodels-isupportparameter.md` (https://docs.devexpress.com/content/WPF/17448?md=true)
- `articles/mvvm-framework/viewmodels/viewmodel-relationships-isupportparentviewmodel.md` (https://docs.devexpress.com/content/WPF/17449?md=true)
- `articles/mvvm-framework/messenger.md` (https://docs.devexpress.com/content/WPF/17474?md=true)
- `articles/mvvm-framework/services.md` (https://docs.devexpress.com/content/WPF/17414?md=true) — for `ServiceSearchMode.PreferParents`

# ViewModel Communication

ViewModels should not directly reference each other's concrete types unless there is a clear parent-child ownership. Use the pattern that matches the coupling level and data-flow direction.

---

## Pattern Overview

| Pattern | Coupling | Direction | Best for |
|---|---|---|---|
| **Messenger** | Loose | Many-to-many broadcast | Notifications with no direct reference |
| **Parent-Child (direct)** | Tight | Top-down / bottom-up | VM owns a child VM; UI binds to nested props |
| **Service (Dialog/Window)** | Medium | Modal call + result | Show a dialog, pass data, receive result |
| **ISupportParameter** | Medium | Caller → Callee (one way) | Navigation or dialog that needs input data |
| **SetParentViewModel** | Medium | Bottom-up | Child needs to call parent or resolve parent-shared services |

---

## 1. Messenger (Loose Coupling, Broadcast)

`DevExpress.Mvvm.Messenger` is a publish/subscribe hub. ViewModels communicate without holding references to each other.

### Sending a Message

```csharp
// Any ViewModel (or other class)
public void OnOrderSaved() {
    Messenger.Default.Send(new OrderSavedMessage { OrderId = CurrentOrder.Id });
}

// Simple string messages
Messenger.Default.Send("RefreshAll");
```

Define a message class for typed messages (recommended):

```csharp
public class OrderSavedMessage {
    public int OrderId { get; set; }
}
```

### Receiving a Message

Register in the constructor (or `OnViewLoaded` / `OnCreate`):

```csharp
public class DashboardViewModel {
    public DashboardViewModel() {
        Messenger.Default.Register<OrderSavedMessage>(this, OnOrderSaved);
    }

    void OnOrderSaved(OrderSavedMessage msg) {
        // React — e.g., refresh grid
        Refresh();
    }
}
```

**Unregister when done** (e.g., on form close or ViewModel dispose) to avoid memory leaks:

```csharp
Messenger.Default.Unregister(this);      // unregisters all subscriptions of this receiver
// or
Messenger.Default.Unregister<OrderSavedMessage>(this, OnOrderSaved);  // specific handler
```

---

## 2. Parent-Child (Direct Reference)

When a ViewModel owns a child ViewModel, expose the child as a property and bind directly through it.

### Parent owns Child

```csharp
// Compile-time approach
[GenerateViewModel]
partial class OrderFormViewModel {
    public CustomerViewModel Customer { get; }

    public OrderFormViewModel() {
        // POCO: use ViewModelSource.Create; compile-time: plain new() is fine
        Customer = new CustomerViewModel();
    }
}

[GenerateViewModel]
partial class CustomerViewModel {
    [GenerateProperty] string name = string.Empty;
    [GenerateProperty] string email = string.Empty;
}
```

### View binds through parent to child

```csharp
var fluent = mvvmContext1.OfType<OrderFormViewModel>();
fluent.SetBinding(txtCustomerName,  te => te.Text,       vm => vm.Customer.Name);
fluent.SetBinding(txtCustomerEmail, te => te.Text,       vm => vm.Customer.Email);
```

The `MVVMContext` Fluent API resolves property chains (including nested POCO VMs) automatically.

### Child needs to call parent back

Use `SetParentViewModel`:

```csharp
// Parent sets itself on the child (POCO API)
var child = ViewModelSource.Create<DetailViewModel>();
child.SetParentViewModel(this);  // 'this' is the parent POCO VM

// Child accesses parent:
public class DetailViewModel {
    protected MainViewModel Parent =>
        this.GetParentViewModel<MainViewModel>();

    public void CloseAndReturn() {
        Parent.OnDetailClosed(Result);
    }
}
```

---

## 3. Service-Based Communication (Dialog with Result)

Use `IDialogService` when a parent ViewModel needs to open a child ViewModel in a dialog, collect user input, and use the result.

```csharp
// Parent ViewModel
[GenerateViewModel]
partial class OrderListViewModel {
    protected IDialogService DialogService =>
        this.GetService<IDialogService>();

    [GenerateCommand]
    void AddOrder() {
        var newOrderVm = new NewOrderViewModel();

        var result = DialogService.ShowDialog(
            MessageButton.OKCancel,
            "New Order",
            newOrderVm
        );

        if (result == MessageResult.OK) {
            Orders.Add(newOrderVm.CreatedOrder);
        }
    }
}
```

The DevExpress dialog service resolves the child ViewModel's View by convention (`NewOrderViewModel` → looks for `NewOrderView` UserControl or XtraForm) or you can specify a View name explicitly.

---

## 4. ISupportParameter (Pass Data into a Child ViewModel)

When navigating to a View or opening a dialog, the framework injects a parameter object into the target ViewModel's `Parameter` property.

```csharp
// Child ViewModel implements ISupportParameter
public class OrderDetailsViewModel : ISupportParameter {
    object _parameter;

    public object Parameter {
        get => _parameter;
        set {
            _parameter = value;
            if (_parameter is Order order)
                LoadOrder(order);
        }
    }

    void LoadOrder(Order order) {
        // initialize from the passed object
    }
}
```

Pass the parameter when opening the dialog / navigating:

```csharp
// Via DialogService
DialogService.ShowDialog(MessageButton.OK, "Details", "OrderDetailsView",
    selectedOrder,       // this becomes Parameter
    parentViewModel: this
);

// Via NavigationService
NavigationService.Navigate("OrderDetailsView",
    param: selectedOrder,
    parentViewModel: this
);
```

---

## 5. NavigationService (Page-Style Navigation with Parameter)

For single-form applications with a `NavigationFrame` hosting multiple Views:

```csharp
// Register in View
mvvmContext1.RegisterService(NavigationService.Create(navigationFrame1));

// ViewModel
protected INavigationService NavigationService =>
    this.GetService<INavigationService>();

public void ShowOrderDetails(Order order) {
    NavigationService.Navigate("OrderDetailsView", order, this);
}

public void GoBack() {
    if (NavigationService.CanGoBack)
        NavigationService.GoBack();
}
```

The navigated ViewModel receives the parameter through `ISupportParameter.Parameter`.

---

## Choosing the Right Pattern

```
Do the VMs know about each other?
├── No → use Messenger (broadcast notification)
├── Parent owns child → use Parent-Child (direct property chain)
└── Modal interaction with result?
    ├── Yes → use IDialogService + ISupportParameter
    └── Page navigation → use INavigationService + ISupportParameter
```

---

## Source Material

- Messenger: `https://docs.devexpress.com/content/WindowsForms/113982?md=true`
- Navigation and View Management: `https://docs.devexpress.com/content/WindowsForms/114173?md=true`
- Data and Property Bindings (nested VMs): `https://docs.devexpress.com/content/WindowsForms/113956?md=true`
- ViewModel Management: `https://docs.devexpress.com/content/WindowsForms/119492?md=true`

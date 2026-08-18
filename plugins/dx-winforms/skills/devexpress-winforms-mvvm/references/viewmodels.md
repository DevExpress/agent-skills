# ViewModel Types: Properties and Commands

DevExpress WinForms supports four ViewModel styles. Each differs in how properties raise `INotifyPropertyChanged` and how commands are declared.

---

## Priority Order

1. **Compile-Time (DevExpress Code Generator)** ← recommended for new projects
2. **Runtime POCO** ← recommended when you use the VS designer heavily (`.NET Framework` projects)
3. **ViewModelBase** ← use only when POCO features cannot be used (e.g., inheritance constraints)
4. **CommunityToolkit.Mvvm** ← use when the project already uses this library

---

## 1. Compile-Time: DevExpress Code Generator (Recommended)

**NuGet:** `DevExpress.Mvvm.CodeGenerators` (free, NuGet.org)

The source generator runs at compile time. The class is a `partial class` decorated with `[GenerateViewModel]`. The generator creates all INPC infrastructure as a second `partial` file.

### Properties

```csharp
using DevExpress.Mvvm.CodeGenerators;

[GenerateViewModel]
partial class OrderViewModel {
    // Generates: public string Title { get; set; } + RaisePropertyChanged.
    // A plain [GenerateProperty] does NOT auto-call a change callback — wire one with OnChangedMethod.
    [GenerateProperty(OnChangedMethod = nameof(OnTitleChanged))]
    string title = string.Empty;

    // IsVirtual = false generates a non-virtual property (omit it for the default).
    // To restrict the setter instead, use SetterAccessModifier (e.g. protected).
    [GenerateProperty(IsVirtual = false)]
    int orderId;

    // Called automatically after Title changes; the callback receives the OLD value.
    // (Use a parameterless OnTitleChanged() if you don't need it, or OnChangingMethod for a
    //  pre-change hook that receives the NEW value.) It is a regular method — not partial.
    void OnTitleChanged(string oldValue) {
        // react to property change
    }
}
```

Generated property naming: field `orderName` → property `OrderName`.

To opt out of a field being a property: simply don't apply `[GenerateProperty]`.

### Synchronous Command

```csharp
[GenerateViewModel]
partial class OrderViewModel {
    [GenerateProperty]
    string title = string.Empty;

    // Generates: SaveCommand (DelegateCommand)
    [GenerateCommand]
    public void Save() {   // public so the View can bind it via vm => vm.Save
        // business logic
    }

    // CanExecute — method named "Can" + command method name
    bool CanSave() => !string.IsNullOrEmpty(Title);
}
```

Bind in View:
```csharp
var fluent = mvvmContext1.OfType<OrderViewModel>();
fluent.BindCommand(btnSave, vm => vm.Save);
```

### Async Command

```csharp
[GenerateViewModel]
partial class OrderViewModel {
    // Generates: LoadAsyncCommand (AsyncCommand)
    // The button is disabled automatically while the command runs
    [GenerateCommand]
    public async Task LoadAsync() {   // public so the View can bind it via vm => vm.LoadAsync
        await FetchOrdersAsync();
    }

    // CanExecute for async command
    bool CanLoadAsync() => IsConnected;
}
```

Bind in View:
```csharp
fluent.BindCommand(btnLoad, vm => vm.LoadAsync);
// Button with cancel support:
fluent.BindCancelCommand(btnCancel, vm => vm.LoadAsync);
```

---

## 2. Runtime POCO ViewModel

**NuGet:** Any `DevExpress.Win.*` package (POCO runtime is included)

Assign the class to `mvvmContext1.ViewModelType`. The framework uses dynamic code generation (IL emit/proxy) at runtime to add `INotifyPropertyChanged` infrastructure.

**Conventions (must be followed for auto-generation to work):**
- Bindable properties: `public virtual` auto-implemented properties (no backing field)
- Commands: `public void` methods (with zero or one parameter)
- Async commands: `public async Task` methods (or returning `Task`)
- CanExecute: method named `Can` + command method name
- Property change callback: method named `On` + property name + `Changed`

### Properties

```csharp
// No attribute, no base class — just conventions
public class OrderViewModel {
    public virtual string Title { get; set; }       // bindable (two-way)
    public virtual int OrderId { get; protected set; } // bindable, setter protected

    // Called automatically when Title changes
    protected void OnTitleChanged() {
        this.RaisePropertyChanged(x => x.DisplayTitle);
    }

    public string DisplayTitle => $"Order: {Title}";
}
```

Properties **with** backing fields are ignored by the POCO mechanism. To include them, use `[BindableProperty]`:

```csharp
string name;
[BindableProperty]
public virtual string Name {
    get => name;
    set => name = value;
}
```

### Synchronous Command

```csharp
public class OrderViewModel {
    public virtual string Title { get; set; }

    // Treated as a command by convention (public void, zero or one param)
    public void Save() {
        // business logic
    }

    // CanExecute by convention
    public bool CanSave() => !string.IsNullOrEmpty(Title);

    // Re-check CanSave when Title changes
    protected void OnTitleChanged() {
        this.RaiseCanExecuteChanged(x => x.Save());
    }
}
```

### Async Command

```csharp
public class OrderViewModel {
    // public async Task → async command; button disabled while running
    public async Task Load() {
        await FetchOrdersAsync();
    }

    public bool CanLoad() => IsConnected;
}
```

Use `ViewModelSource.Create<T>()` if you need a typed instance:

```csharp
var vm = ViewModelSource.Create<OrderViewModel>();
mvvmContext1.SetViewModel(typeof(OrderViewModel), vm);
```

---

## 3. ViewModelBase

**NuGet:** `DevExpress.Mvvm` or any `DevExpress.Win.*` package

Inherit from `DevExpress.Mvvm.ViewModelBase`. Properties are declared explicitly with backing fields; the base class provides `SetProperty` and `RaisePropertyChanged`. Commands are declared as `DelegateCommand` / `AsyncCommand` properties.

> **Note:** The DevExpress docs explicitly say: *"We do not recommend this approach because you lose all the features POCO models provide."*
> Use only when class hierarchy constraints prevent using POCO.

### Properties

```csharp
using DevExpress.Mvvm;

public class OrderViewModel : ViewModelBase {
    string title;
    public string Title {
        get => title;
        set => SetProperty(ref title, value, () => Title);
    }

    int orderId;
    public int OrderId {
        get => orderId;
        private set => SetProperty(ref orderId, value, () => OrderId);
    }
}
```

### Synchronous Command

```csharp
public class OrderViewModel : ViewModelBase {
    string title;
    public string Title {
        get => title;
        set {
            SetProperty(ref title, value, () => Title);
            SaveCommand.RaiseCanExecuteChanged();
        }
    }

    public DelegateCommand SaveCommand { get; }

    public OrderViewModel() {
        SaveCommand = new DelegateCommand(Save, CanSave);
    }

    void Save() { /* business logic */ }
    bool CanSave() => !string.IsNullOrEmpty(Title);
}
```

### Async Command

```csharp
public AsyncCommand LoadCommand { get; }

public OrderViewModel() {
    LoadCommand = new AsyncCommand(Load);
}

async Task Load() {
    await FetchOrdersAsync();
}
```

Associate with `MVVMContext`:

```csharp
var vm = new OrderViewModel();
mvvmContext1.SetViewModel(typeof(OrderViewModel), vm);
var fluent = mvvmContext1.OfType<OrderViewModel>();
fluent.SetBinding(textEdit1, te => te.Text, vm2 => vm2.Title);
```

---

## 4. CommunityToolkit.Mvvm

**NuGet:** `CommunityToolkit.Mvvm` (Microsoft, free, NuGet.org)

Works with DevExpress `MVVMContext` Fluent API for bindings and command binding. Does **not** support DevExpress Services or Behaviors.

### Properties

```csharp
using CommunityToolkit.Mvvm.ComponentModel;

public partial class OrderViewModel : ObservableObject {
    [ObservableProperty]
    string title = string.Empty;   // generates Title property + OnTitleChanged partial
}
```

### Synchronous and Async Commands

```csharp
using CommunityToolkit.Mvvm.Input;

public partial class OrderViewModel : ObservableObject {
    [ObservableProperty]
    string title = string.Empty;

    [RelayCommand(CanExecute = nameof(CanSave))]
    public void Save() { /* business logic */ }   // public so the View can bind it via vm => vm.Save
    bool CanSave() => !string.IsNullOrEmpty(Title);

    [RelayCommand]
    public async Task LoadAsync() {                // public so the View can bind it via vm => vm.LoadAsync
        await FetchOrdersAsync();
    }
}
```

### View Binding

```csharp
mvvmContext1.ViewModelType = typeof(OrderViewModel);
var fluent = mvvmContext1.OfType<OrderViewModel>();
fluent.SetBinding(textEdit1, te => te.Text, vm => vm.Title);
fluent.BindCommand(btnSave, vm => vm.Save);
// Async command — CommunityToolkit's [RelayCommand] on LoadAsync() generates a
// LoadCommand property; bind via the method group below.
fluent.BindCommand(btnLoad, vm => vm.LoadAsync);
```

---

## Comparison Summary

| Feature | Code Generator | POCO (runtime) | ViewModelBase | CommunityToolkit |
|---|---|---|---|---|
| Compile-time safety | ✅ Full | ❌ Runtime proxy | ✅ Full | ✅ Full |
| No inheritance required | ✅ | ✅ | ❌ (must inherit) | ❌ (must inherit) |
| Property syntax | `[GenerateProperty]` field | `public virtual` auto-prop | Manual + `SetProperty` | `[ObservableProperty]` field |
| Command syntax | `[GenerateCommand]` method | `public void` / `async Task` | `DelegateCommand` property | `[RelayCommand]` method |
| CanExecute by convention | `CanMethodName()` | `CanMethodName()` | Lambda in ctor | `CanExecute =` attribute param |
| DevExpress Services | ✅ | ✅ | ✅ | ❌ |
| DevExpress Behaviors | ✅ | ✅ | ✅ | ❌ |
| Requires .NET 6+ | ✅ (source generators) | ❌ (works on .NET FW) | ❌ | ✅ |

---

## Source Material

- MVVM Commands: `https://docs.devexpress.com/content/WindowsForms/113965?md=true`
- Data and Property Bindings: `https://docs.devexpress.com/content/WindowsForms/113956?md=true`
- MVVM Integration overview: `https://docs.devexpress.com/content/WindowsForms/113955?md=true`
- ViewModel Management: `https://docs.devexpress.com/content/WindowsForms/119492?md=true`
- Conventions and Attributes: `https://docs.devexpress.com/content/WindowsForms/117014?md=true`
- CodeGenerators GitHub: `https://github.com/DevExpress/DevExpress.Mvvm.CodeGenerators`
- `ViewModelBase` class: `xref:DevExpress.Mvvm.ViewModelBase`

# Getting Started with DevExpress WinForms MVVM

> **.NET Framework?** For .NET Framework 4.6.2+ project setup, see [getting-started-dotnet-fw.md](getting-started-dotnet-fw.md).


## Framework Options

DevExpress WinForms supports two MVVM approaches — choose one based on your project:

| Approach | Package(s) | .NET Requirement | Priority |
|---|---|---|---|
| **DevExpress Compile-Time (Code Generator)** | `DevExpress.Mvvm.CodeGenerators` + `DevExpress.Mvvm` | .NET 6+ (C# source generators) | **Recommended for new projects** |
| DevExpress Runtime POCO | `DevExpress.Utils` (included in all DevExpress WinForms packages) | .NET Framework 4.6.2+ or .NET 8+ | Legacy / design-time-first |
| Microsoft CommunityToolkit.Mvvm | `CommunityToolkit.Mvvm` | .NET 6+ | Alternative; no DevExpress Services/Behaviors |

All approaches use the `MVVMContext` component (`DevExpress.Utils.MVVM` namespace, in `DevExpress.Utils.v26.1.dll`) for binding ViewModel to View.

---

## Installation

### Option A: DevExpress Compile-Time (Recommended)

Install via NuGet Package Manager or `dotnet add package`:

```
DevExpress.Mvvm.CodeGenerators    (free; from NuGet.org — no DevExpress license required)
DevExpress.Mvvm                   (free runtime; from NuGet.org)
DevExpress.Win.Navigation         (or any DevExpress.Win.* package — includes MVVMContext)
```

The `DevExpress.Mvvm.CodeGenerators` package is open-source and free: `https://github.com/DevExpress/DevExpress.Mvvm.CodeGenerators`

### Option B: DevExpress Runtime POCO

No extra NuGet needed — `MVVMContext` and all POCO infrastructure ship with every DevExpress WinForms package:

```
DevExpress.Win.Navigation    (or any DevExpress.Win.* / DevExpress.Utils package)
```

---

## Required Namespaces

```csharp
using DevExpress.Mvvm;                  // ViewModelBase, DelegateCommand, Messenger
using DevExpress.Mvvm.DataAnnotations;  // BindableProperty, Command attributes
using DevExpress.Mvvm.POCO;             // ViewModelSource (runtime POCO only)
using DevExpress.Utils.MVVM;            // MVVMContext
using DevExpress.XtraEditors;           // XtraForm, fluent binding entry points
```

For compile-time generator:

```csharp
using DevExpress.Mvvm.CodeGenerators;   // GenerateViewModel, GenerateProperty, GenerateCommand
```

---

## MVVMContext Component — Setup

The `MVVMContext` is the bridge between a View (Form / UserControl) and its ViewModel.

**Design-time (recommended):**
1. Open the form in the designer.
2. Drag `MVVMContext` from the Toolbox onto the form (appears in the component tray).
3. Click its smart tag → **Add ViewModel** to create a bound ViewModel class, or use the drop-down to assign an existing one.

**Code-only (compile-time approach):**

```csharp
// In the Form constructor
public partial class MainForm : XtraForm {
    public MainForm() {
        InitializeComponent();
        mvvmContext1.ViewModelType = typeof(MainViewModel);
        Bindings();
    }

    void Bindings() {
        var fluent = mvvmContext1.OfType<MainViewModel>();
        fluent.SetBinding(textEdit1, te => te.Text, vm => vm.UserName);
        fluent.BindCommand(btnSave, vm => vm.Save);
    }
}
```

**Using `SetViewModel` (for ViewModelBase or DI-created instances):**

```csharp
var vm = new MainViewModel();
mvvmContext1.SetViewModel(typeof(MainViewModel), vm);
```

---

## Minimal Working Example (Compile-Time)

**1. Install packages** (`.csproj`):
```xml
<PackageReference Include="DevExpress.Mvvm.CodeGenerators" Version="*" />
<PackageReference Include="DevExpress.Mvvm" Version="*" />
```

**2. ViewModel:**
```csharp
using DevExpress.Mvvm.CodeGenerators;

[GenerateViewModel]
partial class MainViewModel {
    [GenerateProperty]
    string userName = string.Empty;

    [GenerateCommand]
    void Save() {
        // business logic — no UI references
    }
    bool CanSave() => !string.IsNullOrEmpty(UserName);

    [GenerateCommand]
    async Task LoadAsync() {
        await Task.Delay(1000); // simulate work
    }
}
```

The generator creates `UserName` property (with `RaisePropertyChanged`), `SaveCommand` (`DelegateCommand`), `LoadCommand` (`AsyncCommand`), and wires `CanSave()` automatically.

**3. View:**
```csharp
public partial class MainForm : XtraForm {
    public MainForm() {
        InitializeComponent();
        mvvmContext1.ViewModelType = typeof(MainViewModel);
        var fluent = mvvmContext1.OfType<MainViewModel>();
        fluent.SetBinding(textEdit1, te => te.Text, vm => vm.UserName);
        fluent.BindCommand(btnSave,  vm => vm.Save);
        fluent.BindCommand(btnLoad,  vm => vm.LoadAsync);
    }
}
```

---

## Fluent API Binding Methods

| Method | Purpose |
|---|---|
| `fluent.SetBinding(control, controlProp, vmProp)` | Two-way or one-way property binding |
| `fluent.BindCommand(button, vm => vm.Command)` | Bind a button's click to a command |
| `fluent.BindCancelCommand(cancelBtn, vm => vm.AsyncCommand)` | Bind cancel button for async command |
| `fluent.SetTrigger(vm => vm.Prop, value => { ... })` | React to ViewModel property changes in the View |
| `fluent.SetItemsSourceBinding(control, ...)` | Bind a collection to a multi-item control |

---

## Source Material

- MVVM Integration overview: `https://docs.devexpress.com/content/WindowsForms/113955?md=true`
- MVVMContext component: `https://docs.devexpress.com/content/WindowsForms/113969?md=true`
- Data and Property Bindings: `https://docs.devexpress.com/content/WindowsForms/113956?md=true`
- ViewModel Management: `https://docs.devexpress.com/content/WindowsForms/119492?md=true`
- Fluent API: `https://docs.devexpress.com/content/WindowsForms/117019?md=true`
- CodeGenerators GitHub: `https://github.com/DevExpress/DevExpress.Mvvm.CodeGenerators`
- `MVVMContext` class: `xref:DevExpress.Utils.MVVM.MVVMContext`

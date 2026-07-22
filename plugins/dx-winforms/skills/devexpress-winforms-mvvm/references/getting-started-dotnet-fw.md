# Getting Started with DevExpress WinForms MVVM (.NET Framework 4.6.2+)

This guide is for **.NET Framework** projects. For .NET 8+, see [getting-started.md](getting-started.md). Most of this skill applies to both frameworks — the one important difference is the ViewModel approach (below).

## System Requirements

- .NET Framework 4.6.2 or newer (Windows)
- Visual Studio 2022+ (2019 also supported)
- A valid DevExpress license (for `MVVMContext` and UI integration; `DevExpress.Mvvm` itself is free)

## Critical Difference — No Compile-Time Code Generation

The compile-time generator (`DevExpress.Mvvm.CodeGenerators` with `[GenerateViewModel]` / `[GenerateProperty]` / `[GenerateCommand]`) **requires .NET 6+** and is therefore **not available on .NET Framework**.

On .NET Framework, use one of the runtime approaches instead:

- **Runtime POCO** — a plain class with `public virtual` properties and `public void` / `public async Task` methods; `MVVMContext` generates a proxy at runtime. Preferred for new .NET Framework ViewModels.
- **`ViewModelBase`** — inherit and use `SetProperty`, `GetProperty`, `DelegateCommand`, `AsyncCommand` when you need an explicit base class.

## Installation

```powershell
Install-Package DevExpress.Mvvm          # framework (free, NuGet.org)
```

`MVVMContext` and the runtime POCO framework ship with `DevExpress.Utils` / any `DevExpress.Win.*` package (added automatically by the Unified Component Installer or when you drop `MVVMContext` from the toolbox).

## Required Assemblies (Manual Reference)

- `DevExpress.Mvvm.v26.1.dll` (commands, services, POCO, `ViewModelBase`)
- `DevExpress.Utils.v26.1.dll` (`MVVMContext` and the WinForms fluent API)
- `DevExpress.Data.v26.1.dll` (core dependency)

## Minimal Example — Runtime POCO

```csharp
using DevExpress.XtraEditors;
using DevExpress.Utils.MVVM;

// POCO ViewModel — virtual properties, public methods (no base class, no codegen)
public class MainViewModel {
    public virtual string UserName { get; set; } = string.Empty;
    public void Save() { /* persist */ }
    public bool CanSave() => !string.IsNullOrEmpty(UserName);
}

public partial class MainForm : XtraForm {
    MVVMContext mvvmContext1;     // dropped from the toolbox
    TextEdit textEdit1;
    SimpleButton btnSave;

    public MainForm() {
        InitializeComponent();
        mvvmContext1.ViewModelType = typeof(MainViewModel);
        var fluent = mvvmContext1.OfType<MainViewModel>();
        fluent.SetBinding(textEdit1, te => te.Text, vm => vm.UserName);
        fluent.BindCommand(btnSave, vm => vm.Save);
    }
}
```

The Fluent API, services, behaviors, and ViewModel communication patterns in the other references work identically on .NET Framework — only swap the compile-time ViewModel for runtime POCO / `ViewModelBase`. See [getting-started.md](getting-started.md) for the compile-time (.NET 6+) variant and [viewmodels.md](viewmodels.md) for the full priority order.

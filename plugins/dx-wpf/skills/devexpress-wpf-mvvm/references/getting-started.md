# Getting Started — DevExpress WPF MVVM Framework

The DevExpress MVVM Framework lives in `DevExpress.Mvvm` and is bundled into `DevExpress.Wpf.Core`. Compile-time view-model generation needs an additional analyzer package (`DevExpress.Mvvm.CodeGenerators`). This guide gets a new WPF project set up and walks through the first view model in each style.

## System Requirements

- .NET 8.0+ targeting Windows (or .NET Framework 4.6.2+)
- Visual Studio 2022+ or JetBrains Rider
- For compile-time generation: **C# 9+**, .NET Framework 4.6.1+ or .NET Core 3.0+ (.NET 5+ recommended), Visual Studio 16.9.0+
- A valid DevExpress license

## Step 1: Install Packages

For a typical DevExpress WPF app (UI controls + MVVM):

```bash
dotnet add package DevExpress.Wpf.Core
```

For compile-time view-model generation (recommended for new projects):

```bash
dotnet add package DevExpress.Mvvm.CodeGenerators
```

For a pure-MVVM library that doesn't reference DevExpress UI controls:

```bash
dotnet add package DevExpress.Mvvm
```

All DevExpress packages in a project must share the same version.

## Step 2: Configure the Project

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net8.0-windows</TargetFramework>
    <UseWPF>true</UseWPF>
    <Nullable>enable</Nullable>
    <LangVersion>9</LangVersion>           <!-- Required for [GenerateViewModel] -->
  </PropertyGroup>
</Project>
```

For .NET Core projects using `<PackageReference>` for the code generator, also add:

```xml
<PropertyGroup>
  <IncludePackageReferencesDuringMarkupCompilation>true</IncludePackageReferencesDuringMarkupCompilation>
</PropertyGroup>
```

## Step 3: Add the XAML Namespace

```xaml
<Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:dxmvvm="http://schemas.devexpress.com/winfx/2008/xaml/mvvm"
        xmlns:dx="http://schemas.devexpress.com/winfx/2008/xaml/core"
        xmlns:vm="clr-namespace:MyApp.ViewModels"
        Title="MyApp" Height="400" Width="600">
    ...
</Window>
```

## Step 4: Create the First View Model — Pick a Style

### Option A: Compile-Time (Recommended)

```csharp
// ViewModels/MainViewModel.cs
using DevExpress.Mvvm.CodeGenerators;

namespace MyApp.ViewModels;

[GenerateViewModel]
partial class MainViewModel {
    [GenerateProperty]
    string username = "";

    [GenerateProperty]
    string status = "";

    [GenerateCommand]
    void Login() => Status = $"Welcome, {Username}!";
    bool CanLogin() => !string.IsNullOrEmpty(Username);
}
```

What the source generator emits (verifiable via **Peek Definition** F12):

- `public string Username` with INPC raise
- `public string Status` with INPC raise
- `public DelegateCommand LoginCommand`
- `INotifyPropertyChanged` implementation

Bind in XAML:

```xaml
<Window.DataContext>
    <vm:MainViewModel/>
</Window.DataContext>
<StackPanel Margin="16">
    <TextBox Text="{Binding Username, UpdateSourceTrigger=PropertyChanged}"/>
    <Button Content="Login" Command="{Binding LoginCommand}" Margin="0,8,0,0"/>
    <TextBlock Text="{Binding Status}" Margin="0,8,0,0"/>
</StackPanel>
```

### Option B: Runtime POCO

```csharp
namespace MyApp.ViewModels;

public class MainViewModel {
    public virtual string Username { get; set; } = "";
    public virtual string Status { get; set; } = "";

    public void Login() => Status = $"Welcome, {Username}!";
    public bool CanLogin() => !string.IsNullOrEmpty(Username);

    protected MainViewModel() { }

    public static MainViewModel Create() =>
        DevExpress.Mvvm.POCO.ViewModelSource.Create(() => new MainViewModel());
}
```

```xaml
<Window xmlns:dxmvvm="http://schemas.devexpress.com/winfx/2008/xaml/mvvm"
        xmlns:vm="clr-namespace:MyApp.ViewModels"
        DataContext="{dxmvvm:ViewModelSource Type=vm:MainViewModel}">
```

`ViewModelSource` wraps your POCO at runtime — it generates a descendant class with INPC on virtual properties and a `LoginCommand` property from the `Login` method.

### Option C: ViewModelBase (Inheritance)

```csharp
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;

namespace MyApp.ViewModels;

public class MainViewModel : ViewModelBase {
    public string Username {
        get => GetProperty(() => Username);
        set => SetProperty(() => Username, value);
    }

    public string Status {
        get => GetProperty(() => Status);
        set => SetProperty(() => Status, value);
    }

    [Command]
    public void Login() => Status = $"Welcome, {Username}!";
    public bool CanLogin() => !string.IsNullOrEmpty(Username);
}
```

The `[Command]` attribute is auto-discovered by `ViewModelBase` and exposes `LoginCommand` via `ICustomTypeDescriptor`.

### Picker

| Project state | Recommendation |
|---|---|
| New project, C# 9+, .NET 5+ | **Compile-time (Option A)** |
| Existing project already using `ViewModelBase` | Continue with **Option C** |
| Existing project using POCO | Continue with **Option B** |
| VB.NET project | **Option B** or **C** (compile-time is C#-only) |

See [viewmodels.md](viewmodels.md) for the full strategy comparison.

## Step 5: Build and Run

```bash
dotnet build
dotnet run
```

## Verify Compile-Time Generation (Option A)

Right-click `MainViewModel` in the editor → **Peek Definition** (Alt+F12). The generated partial class appears in a peek window with the full implementation. You can set breakpoints in generated code if needed.

Alternatively, expand **Solution Explorer** → project → **Dependencies** → **Analyzers** → **DevExpress.Mvvm.CodeGenerators** to browse generated files.

## .NET Framework Variant

For `<PackageReference>`-style projects on .NET Framework, follow the analyzer-installation alternative described in the DevExpress docs (download the analyzer DLL and reference it as `<Analyzer Include="...">`). For `packages.config` projects, the NuGet install handles everything.

For full .NET Framework setup — assembly references, the compile-time generator install, and version support — see [getting-started-dotnet-fw.md](getting-started-dotnet-fw.md).

## What to Learn Next

- [View Models](viewmodels.md) — strategies in depth, properties, commands (sync and async) in each style, migration
- [Services](services.md) — predefined services (message box, dialog, file pickers, notifications, etc.) and how to call them from view models
- [Behaviors](behaviors.md) — `EventToCommand` and friends for routing events to commands without code-behind
- [View-Model Communication](viewmodel-communication.md) — `ISupportParameter`, `ISupportParentViewModel`, `Messenger`

## Source Material

- `articles/mvvm-framework/viewmodels.md` (https://docs.devexpress.com/content/WPF/17439?md=true)
- `articles/mvvm-framework/viewmodels/compile-time-generated-viewmodels.md` (https://docs.devexpress.com/content/WPF/402989?md=true)
- `articles/mvvm-framework/viewmodels/runtime-generated-poco-viewmodels.md` (https://docs.devexpress.com/content/WPF/17352?md=true)
- `articles/mvvm-framework/viewmodels/viewmodelbase.md` (https://docs.devexpress.com/content/WPF/17351?md=true)
- `articles/mvvm-framework/viewmodels/bindablebase.md` (https://docs.devexpress.com/content/WPF/17350?md=true)

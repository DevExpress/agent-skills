# Getting Started — DevExpress WPF MVVM Framework (.NET Framework)

This guide covers setting up the DevExpress MVVM Framework in a **.NET Framework 4.6.2+** WPF project. The MVVM API itself is identical to the .NET 8+ build — only project setup (assembly references vs. NuGet) and the compile-time source generator installation differ. For .NET 8+, see [getting-started.md](getting-started.md).

## When to Use This Reference

Use this when you need to:
- Set up the MVVM Framework in a .NET Framework 4.6.2+ WPF project
- Know which DLL assemblies to reference (installer-based projects)
- Install the `[GenerateViewModel]` compile-time generator under .NET Framework — including the `PackageReference` vs `packages.config` difference
- Understand framework-specific constraints

## System Requirements

- .NET Framework 4.6.2 or later targeting Windows (`UseWPF` is implicit for .NET Framework WPF projects)
- Visual Studio 2022+ recommended (Visual Studio 16.9.0+ required for the compile-time generator)
- For compile-time view-model generation: **C# 9+** and .NET Framework **4.6.1+**
- DevExpress installation (Unified Component Installer) for assembly references, or DevExpress NuGet packages from nuget.org
- A valid DevExpress license

> Runtime POCO (`ViewModelSource.Create<T>()`), `ViewModelBase`, and `BindableBase` work on .NET Framework 4.5.2+. Only the compile-time generator needs 4.6.1+ and C# 9+.

## Installation Option A: DevExpress Installer (assembly references)

After running the DevExpress installer, reference the assemblies from the local install folder (typically `C:\Program Files\DevExpress 26.1\Components\Bin\Framework\`):

| Assembly | Provides |
|----------|---------|
| `DevExpress.Data.v26.1.dll` | Core data layer (dependency) |
| `DevExpress.Mvvm.v26.1.dll` | `ViewModelBase`, `BindableBase`, `Messenger`, commands, `ISupportParameter` / `ISupportParentViewModel`, the predefined-service interfaces |
| `DevExpress.Xpf.Core.v26.1.dll` | WPF service implementations (`DXMessageBoxService`, `DialogService`, …), behaviors, and the `dxmvvm:` / `dx:` XAML namespaces |

For a pure-MVVM library that does not reference WPF controls, reference only `DevExpress.Mvvm.v26.1.dll` (+ `DevExpress.Data.v26.1.dll`).

## Installation Option B: NuGet

The same NuGet packages used on .NET work on .NET Framework:

```
Install-Package DevExpress.Wpf.Core
Install-Package DevExpress.Mvvm.CodeGenerators   # for compile-time [GenerateViewModel]
```

**All DevExpress packages in a project must share the same version.**

## Compile-Time Generator on .NET Framework

`[GenerateViewModel]` needs C# 9+, so set the language version explicitly in the project file:

```xml
<PropertyGroup>
  <LangVersion>9</LangVersion>
</PropertyGroup>
```

How you install `DevExpress.Mvvm.CodeGenerators` depends on the project's package format:

- **`packages.config` projects** — the NuGet install wires the analyzer automatically. Nothing else to do.
- **`PackageReference` (SDK-style) projects** — the analyzer is not always picked up from a transitive package on .NET Framework. Reference the analyzer DLL directly:

  1. Remove the `<PackageReference Include="DevExpress.Mvvm.CodeGenerators">` entry.
  2. Download `DevExpress.Mvvm.CodeGenerators.XX.Y.Z.dll` from the [GitHub Releases](https://github.com/DevExpress/DevExpress.Mvvm.CodeGenerators/releases).
  3. Reference it as an analyzer:

     ```xml
     <ItemGroup>
       <Analyzer Include="[PATH_TO_FOLDER]\DevExpress.Mvvm.CodeGenerators.XX.Y.Z.dll" />
     </ItemGroup>
     ```

If C# 9 is unavailable in your toolchain, use **Runtime POCO** (`ViewModelSource.Create<T>()`) or `ViewModelBase` instead — both support older language versions and VB.NET.

## XAML Namespaces

Same as on .NET:

```xaml
xmlns:dxmvvm="http://schemas.devexpress.com/winfx/2008/xaml/mvvm"
xmlns:dx="http://schemas.devexpress.com/winfx/2008/xaml/core"
```

## First View Model

The view-model code is identical to the .NET guide. A compile-time view model:

```csharp
using DevExpress.Mvvm.CodeGenerators;

namespace MyApp.ViewModels {
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
}
```

Bind it directly (compile-time view models are instantiated as plain classes — do **not** use `ViewModelSource`):

```xaml
<Window.DataContext>
    <vm:MainViewModel/>
</Window.DataContext>
```

For the runtime-POCO and `ViewModelBase` variants (which work without the generator), see [getting-started.md](getting-started.md) and [viewmodels.md](viewmodels.md).

## Supported .NET Framework Versions

| Framework | Runtime POCO / `ViewModelBase` / `BindableBase` | Compile-time `[GenerateViewModel]` |
|-----------|---|---|
| .NET Framework 4.5.2 / 4.6 | Yes | No (needs 4.6.1+ and C# 9) |
| .NET Framework 4.6.1+ | Yes | Yes (with C# 9 + VS 16.9.0+) |
| .NET Framework 4.6.2 / 4.7.x / 4.8 | Yes | Yes |

## Troubleshooting

- **`[GenerateViewModel]` emits nothing** — `<LangVersion>9</LangVersion>` missing, the analyzer isn't wired (see the `PackageReference` steps above), or the framework is < 4.6.1. Verify all three.
- **`FileNotFoundException` for a DevExpress assembly** — assembly path/version mismatch; ensure every referenced `DevExpress.*` DLL is the same version and `Copy Local` is `true`.
- **`ViewModelSource.Create` throws "type must be public and not sealed"** — make the POCO class `public`, non-sealed, with `virtual` bindable properties.
- **License error at startup** — register the DevExpress license or run where the installer has activated it.

## What to Learn Next

- [getting-started.md](getting-started.md) — the .NET 8+ setup
- [viewmodels.md](viewmodels.md) — strategies, properties, commands, migration
- [services.md](services.md) — predefined services and how to call them
- [behaviors.md](behaviors.md) — `EventToCommand` and friends
- [viewmodel-communication.md](viewmodel-communication.md) — `ISupportParameter`, `ISupportParentViewModel`, `Messenger`

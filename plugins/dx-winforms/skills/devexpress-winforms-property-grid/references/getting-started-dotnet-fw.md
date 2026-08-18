# Getting Started with PropertyGridControl (.NET Framework 4.6.2+)

This guide is for **.NET Framework** projects. For .NET 8+, see [getting-started.md](getting-started.md). Once the project is configured, all `PropertyGridControl` code (rows, editors, categories, complex properties, collections) is identical on both platforms.

## System Requirements

- .NET Framework 4.6.2 or newer, targeting Windows
- Visual Studio 2022+ (Visual Studio 2019 also supported)
- DevExpress WinForms subscription with the [Unified Component Installer](https://www.devexpress.com/Products/Try/), or DevExpress NuGet packages from nuget.org
- A valid DevExpress license

## Install the NuGet Package

```powershell
# SDK-style project
dotnet add package DevExpress.Win.VerticalGrid

# legacy packages.config project (Package Manager Console)
Install-Package DevExpress.Win.VerticalGrid
```

> The DevExpress Unified Component Installer is only needed for the license and the local offline NuGet feed — the package is also on nuget.org. Do **not** hand-edit a non-SDK `.csproj` to add `<Reference>` entries and do **not** copy DevExpress DLLs with shell commands; that routinely leaves the project unable to build.

`PackageReference` form (keep every DevExpress package on the same version):

```xml
<ItemGroup>
  <PackageReference Include="DevExpress.Win.VerticalGrid" Version="26.1.*" />
</ItemGroup>
```

> Both commands above add the required assembly references.

This package ships `DevExpress.XtraVerticalGrid.v<version>.dll`, which contains the `DevExpress.XtraVerticalGrid` namespace (`PropertyGridControl`, the row types, and the event-args classes).

## Required Assemblies (Manual Reference)

If you reference assemblies directly (no NuGet, no installer), add references from `C:\Program Files\DevExpress {version}\Components\Bin\Framework\` (replace `<version>` with your DevExpress version, e.g., `26.1`):

- `DevExpress.XtraVerticalGrid.v<version>.dll` (the control)
- `DevExpress.XtraEditors.v<version>.dll`, `DevExpress.Utils.v<version>.dll`, `DevExpress.Data.v<version>.dll` (core dependencies)

Prefer the NuGet package over manual references so all DevExpress assemblies stay on one version.

## Namespace Imports

```csharp
using DevExpress.XtraVerticalGrid;
using DevExpress.XtraVerticalGrid.Rows;
using DevExpress.XtraVerticalGrid.Events;
```

## Add to a Form

1. Add a `PropertyGridControl` to the form; set `Dock = Fill`.
2. Assign an object to display — the control reflects on it and auto-creates one row per public property:

```csharp
propertyGridControl1.SelectedObject = myObject;
```

## Minimal Working Example

The control API is identical to the .NET 8+ guide:

```csharp
public partial class SettingsForm : XtraForm {
    public SettingsForm() {
        InitializeComponent();
        propertyGridControl1.SelectedObject = new AppSettings();
    }
}

public class AppSettings {
    [Category("General")]
    [DisplayName("Application Title")]
    public string Title { get; set; } = "My App";

    [Category("General")]
    public bool StartMinimized { get; set; }

    [Category("Performance")]
    public int MaxThreads { get; set; } = 4;
}
```

## .NET Framework–Specific Gotchas

- **Platform architecture**: 64-bit .NET Framework targets need 64-bit DevExpress assemblies — they ship in the same location.
- **MSBuild Targets**: The installer path registers MSBuild `.targets` files that resolve version-specific assembly references (for example, `DevExpress.XtraVerticalGrid.v26.1.dll`) automatically. Without the installer (NuGet only), make sure project references use real version strings.
- **License file**: .NET Framework projects must include the `licenses.licx` file in the project (the installer adds it automatically). NuGet-only projects need to add it manually — see the DevExpress licensing documentation.

## What to Learn Next

The remaining references in this skill ([property-definitions.md](property-definitions.md), [collection-editor.md](collection-editor.md), [categories.md](categories.md), [complex-properties.md](complex-properties.md)) apply identically to both .NET and .NET Framework once the project is configured.

# Getting Started with PropertyGridControl (.NET Framework 4.6.2+)

This guide is for **.NET Framework** projects. For .NET 8+, see [getting-started.md](getting-started.md). Once the project is configured, all `PropertyGridControl` code (rows, editors, categories, complex properties, collections) is identical on both platforms.

## System Requirements

- .NET Framework 4.6.2 or newer, targeting Windows
- Visual Studio 2022+ (Visual Studio 2019 also supported)
- DevExpress WinForms subscription with the [Unified Component Installer](https://www.devexpress.com/Products/Try/) for the designer-first workflow, or DevExpress NuGet packages from nuget.org
- A valid DevExpress license

## Two Installation Paths

1. **Unified Component Installer** (recommended for designer-first workflows). Run the installer, then drag `PropertyGridControl` from the Visual Studio Toolbox onto a form. References are added automatically.
2. **NuGet package** (recommended for source control and CI builds). Install `DevExpress.Win.VerticalGrid` (or `DevExpress.Win.Navigation`) from nuget.org.

## Path 2: NuGet Package

```powershell
Install-Package DevExpress.Win.VerticalGrid
```

`PackageReference` form (keep every DevExpress package on the same version):

```xml
<ItemGroup>
  <PackageReference Include="DevExpress.Win.VerticalGrid" Version="26.1.*" />
</ItemGroup>
```

> If your project still uses `packages.config`, the `Install-Package` command works the same — Visual Studio installs the package and adds the assembly references.

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

1. Drag `PropertyGridControl` from the Toolbox onto the form; set `Dock = Fill`.
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

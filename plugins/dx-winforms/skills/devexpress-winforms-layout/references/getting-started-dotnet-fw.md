# Getting Started (.NET Framework 4.6.2+)

This guide is for **.NET Framework** projects. For .NET 8+, see [getting-started.md](getting-started.md). Once the project is configured, all layout/docking code (`LayoutControl`, `DataLayoutControl`, `DockManager`, `StackPanel`, `TablePanel`) is identical on both platforms.

## System Requirements

- .NET Framework 4.6.2 or newer, targeting Windows
- Visual Studio 2022+ (Visual Studio 2019 also supported)
- DevExpress WinForms subscription with the [Unified Component Installer](https://www.devexpress.com/Products/Try/), or DevExpress NuGet packages from nuget.org
- A valid DevExpress license

## Install the NuGet Package

```powershell
# SDK-style project
dotnet add package DevExpress.Win.Navigation

# legacy packages.config project (Package Manager Console)
Install-Package DevExpress.Win.Navigation
```

> The DevExpress Unified Component Installer is only needed for the license and the local offline NuGet feed — the package is also on nuget.org. Do **not** hand-edit a non-SDK `.csproj` to add `<Reference>` entries and do **not** copy DevExpress DLLs with shell commands; that routinely leaves the project unable to build.

`PackageReference` form (keep every DevExpress package on the same version):

```xml
<ItemGroup>
  <PackageReference Include="DevExpress.Win.Navigation" Version="26.1.*" />
</ItemGroup>
```

> Both commands above add the required assembly references.

## Required Assemblies (Manual Reference)

If you reference assemblies directly (no NuGet, no installer), add references from `C:\Program Files\DevExpress {version}\Components\Bin\Framework\` (replace `<version>` with your DevExpress version, e.g., `26.1`). The layout controls span several assemblies:

| Control | Assembly | Namespace |
|---|---|---|
| `LayoutControl` | `DevExpress.XtraLayout.v<version>.dll` | `DevExpress.XtraLayout` |
| `DataLayoutControl` | `DevExpress.XtraLayout.v<version>.dll` | `DevExpress.XtraDataLayout` |
| `DockManager` + `DockPanel` | `DevExpress.XtraBars.v<version>.dll` | `DevExpress.XtraBars.Docking` |
| `StackPanel` / `TablePanel` | `DevExpress.Utils.v<version>.dll` | `DevExpress.Utils.Layout` |

Also add the core dependencies `DevExpress.Data.v<version>.dll` and `DevExpress.Utils.v<version>.dll`. Prefer the NuGet package over manual references so all DevExpress assemblies stay on one version.

## Common Namespaces

```csharp
using DevExpress.XtraLayout;          // LayoutControl, LayoutControlItem, LayoutControlGroup
using DevExpress.XtraLayout.Utils;    // LayoutMode (Flow/Table layout mode for a group)
using DevExpress.XtraDataLayout;      // DataLayoutControl
using DevExpress.XtraBars.Docking;    // DockManager, DockPanel, DockingStyle
using DevExpress.XtraEditors;         // XtraForm, TextEdit, SimpleButton
using DevExpress.Utils.Layout;        // StackPanel, TablePanel
```

## LayoutControl — Minimum Setup

The API is identical to the .NET 8+ guide:

```csharp
public partial class MainForm : DevExpress.XtraEditors.XtraForm {
    public MainForm() {
        InitializeComponent();

        var lc = new LayoutControl { Dock = DockStyle.Fill };
        Controls.Add(lc);

        lc.BeginUpdate();
        try {
            lc.AddItem("Name",  new TextEdit { Name = "edName" });
            lc.AddItem("Email", new TextEdit { Name = "edEmail" });
        }
        finally {
            lc.EndUpdate();
        }
    }
}
```

## .NET Framework–Specific Gotchas

- **Platform architecture**: 64-bit .NET Framework targets need 64-bit DevExpress assemblies — they ship in the same location.
- **MSBuild Targets**: The installer path registers MSBuild `.targets` files that resolve version-specific assembly references (for example, `DevExpress.XtraLayout.v26.1.dll`) automatically. Without the installer (NuGet only), make sure project references use real version strings.
- **License file**: .NET Framework projects must include the `licenses.licx` file in the project (the installer adds it automatically). NuGet-only projects need to add it manually — see the DevExpress licensing documentation.

## What to Learn Next

The remaining references in this skill ([layout-controls.md](layout-controls.md), [building-layouts.md](building-layouts.md), [saving-restoring-layout.md](saving-restoring-layout.md)) apply identically to both .NET and .NET Framework once the project is configured.

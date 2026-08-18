# Getting Started (.NET Framework 4.6.2+)

This guide is for **.NET Framework** projects. For .NET 8+, see [getting-started.md](getting-started.md). Once the project is configured, all editor code (`BaseEdit` descendants, masks, buttons, repository items) is identical on both platforms.

## System Requirements

- .NET Framework 4.6.2 or newer, targeting Windows
- Visual Studio 2022+ (Visual Studio 2019 also supported)
- DevExpress WinForms subscription with the [Unified Component Installer](https://www.devexpress.com/Products/Try/), or DevExpress NuGet packages from nuget.org
- A valid DevExpress license

## Install the NuGet Packages

Editors live in `DevExpress.Win.Navigation`. Add `DevExpress.Win.Grid` for grid-based lookups (`GridLookUpEdit`/`SearchLookUpEdit`), `DevExpress.Win.TreeList` for `TreeListLookUpEdit`, and `DevExpress.Win.Dialogs` for the file/folder pickers:

```powershell
# SDK-style project
dotnet add package DevExpress.Win.Navigation
dotnet add package DevExpress.Win.Grid       # grid-based lookups
dotnet add package DevExpress.Win.Dialogs    # file/folder pickers

# legacy packages.config project (Package Manager Console)
Install-Package DevExpress.Win.Navigation
```

> The DevExpress Unified Component Installer is only needed for the license and the local offline NuGet feed — the packages are also on nuget.org. Do **not** hand-edit a non-SDK `.csproj` to add `<Reference>` entries and do **not** copy DevExpress DLLs with shell commands; that routinely leaves the project unable to build.

Keep every DevExpress package on the **same** version:

```xml
<ItemGroup>
  <PackageReference Include="DevExpress.Win.Navigation" Version="26.1.*" />
  <PackageReference Include="DevExpress.Win.Grid"       Version="26.1.*" />
  <PackageReference Include="DevExpress.Win.Dialogs"    Version="26.1.*" />
</ItemGroup>
```

> Both commands above add the required assembly references. The `DevExpress.Win` umbrella package covers all of the above.

## Required Assemblies (Manual Reference)

If you reference assemblies directly (no NuGet, no installer), add references from `C:\Program Files\DevExpress {version}\Components\Bin\Framework\` (replace `<version>` with your DevExpress version, e.g., `26.1`):

- `DevExpress.XtraEditors.v<version>.dll` (all editors, repository items)
- `DevExpress.Utils.v<version>.dll`, `DevExpress.Data.v<version>.dll` (core dependencies)
- `DevExpress.XtraGrid.v<version>.dll` (only for grid-based lookups), `DevExpress.Dialogs.v<version>.Core.dll` (only for the dialog components)

Prefer the NuGet packages over manual references so all DevExpress assemblies stay on one version.

## Common Imports

```csharp
using DevExpress.XtraEditors;                // TextEdit, SpinEdit, ButtonEdit, LookUpEdit, ...
using DevExpress.XtraEditors.Controls;       // EditorButton, ButtonPredefines, TextEditStyles
using DevExpress.XtraEditors.Repository;     // RepositoryItem*, RepositoryItemButtonEdit
using DevExpress.XtraEditors.Mask;           // MaskType, MaskSettings nested types
using DevExpress.Utils;                      // DefaultBoolean, HorzAlignment
using DevExpress.Utils.Svg;                  // SvgImage
```

## Host Form

Use `DevExpress.XtraEditors.XtraForm` (or `RibbonForm`) for correct skin integration. A plain `Form` works but appearance may be inconsistent; in that case call `DevExpress.Skins.SkinManager.EnableFormSkins()` once at startup.

## Minimal Setup in Code

The editor API is identical to the .NET 8+ guide:

```csharp
public partial class MainForm : DevExpress.XtraEditors.XtraForm {
    public MainForm() {
        InitializeComponent();

        var name = new TextEdit { Width = 240 };
        name.Properties.NullValuePrompt = "Full name";
        Controls.Add(name);
    }
}
```

## .NET Framework–Specific Gotchas

- **Platform architecture**: 64-bit .NET Framework targets need 64-bit DevExpress assemblies — they ship in the same location.
- **MSBuild Targets**: The installer path registers MSBuild `.targets` files that resolve version-specific assembly references (for example, `DevExpress.XtraEditors.v26.1.dll`) automatically. Without the installer (NuGet only), make sure project references use real version strings.
- **License file**: .NET Framework projects must include the `licenses.licx` file in the project (the installer adds it automatically). NuGet-only projects need to add it manually — see the DevExpress licensing documentation.

## What to Learn Next

The remaining references in this skill ([editor-variants.md](editor-variants.md), [masks.md](masks.md), [buttons-in-editors.md](buttons-in-editors.md), [non-baseedit-controls.md](non-baseedit-controls.md)) apply identically to both .NET and .NET Framework once the project is configured.

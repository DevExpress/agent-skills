# Getting Started (.NET Framework 4.6.2+)

This guide is for **.NET Framework** projects. For .NET 8+, see [getting-started.md](getting-started.md). Once the project is configured, all loading-indicator code (Splash Screen, Wait Form, Overlay Form, ProgressPanel) is identical on both platforms.

## System Requirements

- .NET Framework 4.6.2 or newer, targeting Windows
- Visual Studio 2022+ (Visual Studio 2019 also supported)
- DevExpress WinForms subscription with the [Unified Component Installer](https://www.devexpress.com/Products/Try/) for the designer-first workflow, or DevExpress NuGet packages from nuget.org
- A valid DevExpress license

## Two Installation Paths

1. **Unified Component Installer** (recommended for designer-first workflows). Run the installer, then drop a `SplashScreenManager` / `ProgressPanel` from the Visual Studio Toolbox onto a form. References are added automatically.
2. **NuGet package** (recommended for source control and CI builds). Install `DevExpress.Win.Navigation` from nuget.org.

## Path 2: NuGet Package

```powershell
Install-Package DevExpress.Win.Navigation
```

`PackageReference` form (keep every DevExpress package on the same version):

```xml
<ItemGroup>
  <PackageReference Include="DevExpress.Win.Navigation" Version="26.1.*" />
</ItemGroup>
```

> If your project still uses `packages.config`, the `Install-Package` command works the same — Visual Studio installs the package and adds the assembly references.

All loading-indicator classes ship in `DevExpress.XtraEditors.v<version>.dll` (namespaces `DevExpress.XtraSplashScreen` and `DevExpress.XtraWaitForm`).

## Required Assemblies (Manual Reference)

If you reference assemblies directly (no NuGet, no installer), add references from `C:\Program Files\DevExpress {version}\Components\Bin\Framework\` (replace `<version>` with your DevExpress version, e.g., `26.1`):

- `DevExpress.XtraEditors.v<version>.dll` (`SplashScreenManager`, Overlay Form, Wait Form, `ProgressPanel`)
- `DevExpress.Utils.v<version>.dll`, `DevExpress.Data.v<version>.dll` (core dependencies)

Prefer the NuGet package over manual references so all DevExpress assemblies stay on one version.

## Common Namespaces

```csharp
using DevExpress.XtraSplashScreen;   // SplashScreenManager, IOverlaySplashScreenHandle
using DevExpress.XtraWaitForm;        // WaitForm base class, ProgressPanel
```

## Splash Screen — Minimum Code

The API is identical to the .NET 8+ guide. Show the splash before `Application.Run`, and **always** close it once the main form is ready (a missing `CloseForm()` can keep the app alive on exit):

```csharp
// Program.cs — before Application.Run(new MainForm())
SplashScreenManager.ShowFluentSplashScreen(
    title:                "My Application",
    subtitle:             "Version 1.0",
    loadingIndicatorType: FluentLoadingIndicatorType.Dots
);
Application.Run(new MainForm());
```

```csharp
// MainForm.cs — close the splash once the form is visible
protected override void OnShown(EventArgs e) {
    base.OnShown(e);
    SplashScreenManager.CloseForm();
}
```

## .NET Framework–Specific Gotchas

- **Platform architecture**: 64-bit .NET Framework targets need 64-bit DevExpress assemblies — they ship in the same location.
- **MSBuild Targets**: The installer path registers MSBuild `.targets` files that resolve version-specific assembly references (for example, `DevExpress.XtraEditors.v26.1.dll`) automatically. Without the installer (NuGet only), make sure project references use real version strings.
- **License file**: .NET Framework projects must include the `licenses.licx` file in the project (the installer adds it automatically). NuGet-only projects need to add it manually — see the DevExpress licensing documentation.

## What to Learn Next

The [indicators-comparison.md](indicators-comparison.md) reference (choosing between the four indicator types, API patterns, custom Overlay Form painters, `SendCommand`) applies identically to both .NET and .NET Framework once the project is configured.

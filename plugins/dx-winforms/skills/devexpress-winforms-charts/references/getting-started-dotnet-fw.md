# Getting Started (.NET Framework 4.6.2+)

This guide is for **.NET Framework** projects. For .NET 8+, see [getting-started.md](getting-started.md). Once the project is configured, all `ChartControl` code (series, diagrams, axes, legends, tooltips) is identical on both platforms.

## System Requirements

- .NET Framework 4.6.2 or newer, targeting Windows
- Visual Studio 2022+ (Visual Studio 2019 also supported)
- DevExpress WinForms subscription with the [Unified Component Installer](https://www.devexpress.com/Products/Try/) for the designer-first workflow, or DevExpress NuGet packages from nuget.org
- A valid DevExpress license

## Two Installation Paths

1. **Unified Component Installer** (recommended for designer-first workflows). Run the installer, then drag a `ChartControl` from the Visual Studio Toolbox onto a form. References are added automatically.
2. **NuGet package** (recommended for source control and CI builds). Install `DevExpress.Win.Charts` from nuget.org.

## Path 2: NuGet Package

```powershell
Install-Package DevExpress.Win.Charts
```

`PackageReference` form (keep every DevExpress package on the same version):

```xml
<ItemGroup>
  <PackageReference Include="DevExpress.Win.Charts" Version="26.1.*" />
</ItemGroup>
```

> If your project still uses `packages.config`, the `Install-Package` command works the same — Visual Studio installs the package and adds the assembly references. The `DevExpress.Win` umbrella package also includes Charts.

## Required Assemblies (Manual Reference)

If you reference assemblies directly (no NuGet, no installer), add references from `C:\Program Files\DevExpress {version}\Components\Bin\Framework\` (replace `<version>` with your DevExpress version, e.g., `26.1`):

- `DevExpress.XtraCharts.v<version>.UI.dll` — WinForms-specific (the `ChartControl` itself)
- `DevExpress.XtraCharts.v<version>.dll` — cross-platform core (series, diagrams, scales)
- `DevExpress.XtraCharts.v<version>.Design.dll` — only if you host the runtime Chart Designer
- `DevExpress.Data.v<version>.dll`, `DevExpress.Utils.v<version>.dll`, `DevExpress.XtraEditors.v<version>.dll` (core dependencies)

Prefer the NuGet package over manual references so all DevExpress assemblies stay on one version.

## Namespaces

```csharp
using DevExpress.XtraCharts;          // ChartControl, Series, ViewType, all diagrams and views
using DevExpress.Utils;               // DefaultBoolean (used by Visibility properties)
using DevExpress.XtraEditors;         // XtraForm if you want a skin-aware host form
```

## Minimal First Chart (Code)

The chart API is identical to the .NET 8+ guide:

```csharp
public partial class MainForm : DevExpress.XtraEditors.XtraForm {
    public MainForm() {
        InitializeComponent();

        var chart = new ChartControl { Dock = DockStyle.Fill };
        var series = new Series("Sales", ViewType.Bar);
        series.Points.Add(new SeriesPoint("Q1", 120));
        series.Points.Add(new SeriesPoint("Q2", 180));
        chart.Series.Add(series);
        Controls.Add(chart);
    }
}
```

## .NET Framework–Specific Gotchas

- **Platform architecture**: 64-bit .NET Framework targets need 64-bit DevExpress assemblies — they ship in the same location.
- **MSBuild Targets**: The installer path registers MSBuild `.targets` files that resolve version-specific assembly references (for example, `DevExpress.XtraCharts.v26.1.UI.dll`) automatically. Without the installer (NuGet only), make sure project references use real version strings.
- **License file**: .NET Framework projects must include the `licenses.licx` file in the project (the installer adds it automatically). NuGet-only projects need to add it manually — see the DevExpress licensing documentation.

## What to Learn Next

The remaining references in this skill (data binding, series and diagrams, axes, legend, tooltips/crosshair, selection, aggregation) apply identically to both .NET and .NET Framework once the project is configured.

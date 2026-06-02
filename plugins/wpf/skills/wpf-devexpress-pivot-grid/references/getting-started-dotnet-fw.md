# Getting Started with the DevExpress WPF Pivot Grid (.NET Framework 4.6.2+)

This guide is for **.NET Framework** projects. For .NET 6/7/8+, see [getting-started.md](getting-started.md).

## System Requirements

- .NET Framework 4.6.2 or newer
- Visual Studio 2022+ (2019 also supported)
- DevExpress WPF subscription with the [Unified Component Installer](https://www.devexpress.com/Products/Try/) or DevExpress packages from nuget.org (see [getting-started.md § Step 1](getting-started.md) for the full source comparison)
- A valid DevExpress license

## Two Installation Paths

1. **Unified Component Installer** (designer-first workflow). Run the installer, then drag `PivotGridControl` from the toolbox onto your XAML form. The toolbox lives under **DX.{version}: Data & Analytics**.
2. **NuGet packages** (recommended for source control and CI builds). Install `DevExpress.Wpf.PivotGrid` from nuget.org or the local installer feed.

## Path 1: Toolbox + Reset All

1. Install the DevExpress Unified Component Installer.
2. File → New → Project → DevExpress Template Gallery → **Blank MVVM Application (.NET Framework)**.
3. Drag a **PivotGridControl** from the **DX.{version}: Data & Analytics** toolbox tab onto `MainView.xaml`.
4. Right-click the PivotGrid → **Layout → Reset All** to stretch it to fill the window.
5. Use Quick Actions to bind `DataSource` via the **Items Source Configuration Wizard** (supports SQL, OLAP, ADO.NET DataSets, Entity Framework, OData, and more).

Source: `articles/controls-and-libraries/pivot-grid/getting-started/NET-Framework/lesson-1-bind-a-pivot-grid-to-an-mdb-database.md`.

## Path 2: NuGet Package

```powershell
Install-Package DevExpress.Wpf.PivotGrid
```

## Required Assemblies (Manual Reference)

If you reference assemblies directly (no NuGet, no installer), add references to (replace `<:xx.x:>` with your DevExpress version, e.g., `26.1`):

- `DevExpress.Data.v<:xx.x:>.dll`
- `DevExpress.Data.Desktop.v<:xx.x:>.dll`
- `DevExpress.Drawing.v<:xx.x:>.dll`
- `DevExpress.Mvvm.v<:xx.x:>.dll`
- `DevExpress.Printing.v<:xx.x:>.Core.dll`
- `DevExpress.Xpf.Core.v<:xx.x:>.dll`
- `DevExpress.Xpf.PivotGrid.v<:xx.x:>.dll`

## Minimal XAML Example

```xaml
<Window x:Class="MyApp.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:dxpg="http://schemas.devexpress.com/winfx/2008/xaml/pivotgrid"
        Title="My Pivot" Height="500" Width="800"
        Loaded="Window_Loaded">
    <Grid>
        <dxpg:PivotGridControl x:Name="pivotGridControl1"
                               DataProcessingEngine="Optimized"/>
    </Grid>
</Window>
```

The code-behind for binding and field setup is identical to the .NET 6+ version — see [getting-started.md § Step 6](getting-started.md).

## .NET Framework–Specific Notes

- **MDB binding**: requires the [Microsoft Access Database Engine Redistributable](https://www.microsoft.com/en-us/download/details.aspx?id=54920) — install the bitness matching your process target (x86 or x64).
- **Items Source Configuration Wizard**: supports `Connect to Database` (SQL), `Bind to OLAP`, `Bind to a Microsoft Excel Data Source`, and others. Available via Quick Actions in the designer.
- **Licensing**: `licenses.licx` is added automatically by the installer. NuGet-only projects need to add it manually.

The remaining references ([data-binding.md](data-binding.md), [data-shaping.md](data-shaping.md), etc.) apply identically to both .NET and .NET Framework once the project is configured.

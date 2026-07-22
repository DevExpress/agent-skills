# Getting Started with the DevExpress WPF Data Grid (.NET Framework 4.6.2+)

This guide is for **.NET Framework** projects. For .NET 8+, see [getting-started.md](getting-started.md).

## System Requirements

- .NET Framework 4.6.2 or newer
- Visual Studio 2022+ (Visual Studio 2019 also supported)
- DevExpress WPF subscription with the [Unified Component Installer](https://www.devexpress.com/Products/Try/) for the designer-first workflow, or DevExpress NuGet packages from nuget.org
- A valid DevExpress license

## Two Installation Paths

You can add DevExpress to a .NET Framework project in two ways:

1. **Unified Component Installer** (recommended for designer-first workflows). Run the installer, then use the **DevExpress Template Gallery** in Visual Studio to create a "Blank MVVM Application", or drag `GridControl` from the toolbox.
2. **NuGet packages** (recommended for source control and CI builds). Install `DevExpress.Wpf.Grid` from nuget.org.

## Path 1: Unified Component Installer + Template Gallery

1. Install the DevExpress Universal/WPF Component Installer.
2. In Visual Studio: **File → New → Project**.
3. Pick a DevExpress Template Gallery entry such as **Blank MVVM Application (.NET Framework)**. Visual Studio scaffolds a project with the View Model wired to the View.
4. Open `MainView.xaml`, drag a **GridControl** from the toolbox onto the design surface. Visual Studio automatically adds the required references to your project.
5. Use the **Quick Actions** smart tag on `GridControl` and click **Bind to a Data Source** to launch the **Items Source Wizard**. The wizard supports Entity Framework, XPO, OData, and other sources and generates binding + CRUD code in the View Model.

This path matches the article: `articles/controls-and-libraries/data-grid/getting-started/designer/lesson-1-add-a-gridcontrol-to-a-project.md`.

## Path 2: NuGet Package

Install via the Package Manager Console:

```powershell
Install-Package DevExpress.Wpf.Grid
```

> If your project uses the older `packages.config` instead of `PackageReference`, the same command works — Visual Studio installs the package and adds the required assembly references.

## Required Assemblies (Manual Reference)

If you reference assemblies directly (no NuGet, no installer), add references to the following from `C:\Program Files (x86)\DevExpress {version}\Components\Bin\Framework\` (replace `<version>` with your DevExpress version, e.g., `26.1`):

- `DevExpress.Data.Desktop.v<version>.dll`
- `DevExpress.Data.v<version>.dll`
- `DevExpress.Drawing.v<version>.dll`
- `DevExpress.Mvvm.v<version>.dll`
- `DevExpress.Printing.v<version>.Core.dll`
- `DevExpress.Xpf.Core.v<version>.dll`
- `DevExpress.Xpf.Grid.v<version>.dll`
- `DevExpress.Xpf.Grid.v<version>.Core.dll`

Source: `articles/controls-and-libraries/data-grid/getting-started/code/lesson-1-add-a-gridcontrol-to-a-project.md`.

## Minimal XAML Example

Once the project is set up, the XAML for the grid is identical to the .NET 8+ version:

```xaml
<Window x:Class="MyApp.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:dxg="http://schemas.devexpress.com/winfx/2008/xaml/grid"
        xmlns:vm="clr-namespace:MyApp.ViewModels"
        Title="My First Grid" Height="450" Width="800">
    <Window.DataContext>
        <vm:MainViewModel/>
    </Window.DataContext>
    <dxg:GridControl ItemsSource="{Binding Orders}"
                     AutoGenerateColumns="AddNew"
                     EnableSmartColumnsGeneration="True">
        <dxg:GridControl.View>
            <dxg:TableView AutoWidth="True"/>
        </dxg:GridControl.View>
    </dxg:GridControl>
</Window>
```

## .NET Framework–Specific Gotchas

- **Platform architecture**: 64-bit .NET Framework targets need 64-bit DevExpress assemblies — they ship in the same location.
- **Entity Framework**: .NET Framework typically uses the classic Entity Framework 6 (`EntityFramework.dll`), not Entity Framework Core. Server-mode classes for EF6 are `DevExpress.Data.Linq.EntityServerModeSource` and `EntityInstantFeedbackSource`.
- **MSBuild Targets**: When using the installer path, the installer registers MSBuild `.targets` files that resolve version-specific assembly references (for example, `DevExpress.Xpf.Grid.v26.1.dll`) automatically. Without the installer (NuGet only), make sure your project references use real version strings.
- **License file**: .NET Framework projects must include the `licenses.licx` file in the project (the installer adds it automatically). NuGet-only projects need to add it manually — see the DevExpress licensing documentation.

## What to Learn Next

The remaining references in this skill ([data-binding.md](data-binding.md), [views.md](views.md), [sorting-filtering-grouping.md](sorting-filtering-grouping.md), etc.) apply identically to both .NET and .NET Framework once the project is configured.

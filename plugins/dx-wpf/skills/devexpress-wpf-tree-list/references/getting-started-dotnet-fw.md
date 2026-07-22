# Getting Started with the DevExpress WPF TreeList (.NET Framework 4.6.2+)

This guide is for **.NET Framework** projects. For .NET 8+, see [getting-started.md](getting-started.md).

## System Requirements

- .NET Framework 4.6.2 or newer
- Visual Studio 2022+ (Visual Studio 2019 also supported)
- DevExpress WPF subscription with the [Unified Component Installer](https://www.devexpress.com/Products/Try/) for the designer-first workflow, or DevExpress NuGet packages from nuget.org
- A valid DevExpress license

## Two Installation Paths

1. **Unified Component Installer** (designer-first workflow). Run the installer, then use the **DevExpress Template Gallery** in Visual Studio to create a "Blank MVVM Application (.NET Framework)", or drag `TreeListControl` from the toolbox.
2. **NuGet packages** (recommended for source control and CI builds). Install `DevExpress.Wpf.Grid` from nuget.org.

## Path 1: Template Gallery + Toolbox

1. Install the DevExpress Universal/WPF Component Installer.
2. File → New → Project → DevExpress Template Gallery → **Blank MVVM Application (.NET Framework)**.
3. Drag a **TreeListControl** from the toolbox onto `MainView.xaml`. Visual Studio adds the required references automatically.
4. Use the TreeListControl's **Quick Actions** smart tag to:
   - Set `ItemsSource` via **Create Data Binding**
   - Add columns via **Add Columns**
   - Configure the View (`KeyFieldName`, `ParentFieldName`, `AutoExpandAllNodes`, etc.)

Source: `articles/controls-and-libraries/tree-list/getting-started/lesson-1-add-a-treelistcontrol-to-a-project.md`.

## Path 2: NuGet Package

```powershell
Install-Package DevExpress.Wpf.Grid
```

## Required Assemblies (Manual Reference)

If you reference assemblies directly (no NuGet, no installer), add references to (replace `<:xx.x:>` with your DevExpress version, e.g., `26.1`):

- `DevExpress.Data.Desktop.v<:xx.x:>.dll`
- `DevExpress.Data.v<:xx.x:>.dll`
- `DevExpress.Drawing.v<:xx.x:>.dll`
- `DevExpress.Mvvm.v<:xx.x:>.dll`
- `DevExpress.Printing.v<:xx.x:>.Core.dll`
- `DevExpress.Xpf.Core.v<:xx.x:>.dll`
- `DevExpress.Xpf.Grid.v<:xx.x:>.dll`
- `DevExpress.Xpf.Grid.v<:xx.x:>.Core.dll`

TreeListControl ships with the same `DevExpress.Xpf.Grid.v<:xx.x:>.dll` as GridControl.

## Minimal XAML Example

```xaml
<Window x:Class="MyApp.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:dxg="http://schemas.devexpress.com/winfx/2008/xaml/grid"
        xmlns:vm="clr-namespace:MyApp.ViewModels"
        Title="My TreeList" Height="500" Width="800">
    <Window.DataContext>
        <vm:MainViewModel/>
    </Window.DataContext>
    <dxg:TreeListControl ItemsSource="{Binding Employees}"
                         AutoGenerateColumns="AddNew">
        <dxg:TreeListControl.View>
            <dxg:TreeListView KeyFieldName="ID"
                              ParentFieldName="ParentID"
                              AutoWidth="True"
                              AutoExpandAllNodes="True"/>
        </dxg:TreeListControl.View>
    </dxg:TreeListControl>
</Window>
```

The remaining references ([data-binding.md](data-binding.md), [editing.md](editing.md), etc.) apply identically to both .NET and .NET Framework once the project is configured.

# Getting Started — DevExpress WPF Layout Management

DevExpress layout-management splits across two packages: **`DevExpress.Wpf.Docking`** (for `DockLayoutManager` — Visual-Studio-style dockable panels and MDI) and **`DevExpress.Wpf.LayoutControl`** (for the in-panel layout containers — `LayoutControl`, `DataLayoutControl`, `DockLayoutControl`, `FlowLayoutControl`, `TileLayoutControl`). Install only what you need.

## System Requirements

- .NET 8.0+ targeting Windows (or .NET Framework 4.6.2+)
- Visual Studio 2022+ or JetBrains Rider
- A valid DevExpress license

## Step 1: Pick a Package (or Both)

| You want... | Install |
|---|---|
| Visual-Studio-style window with dockable panels and document tabs | `DevExpress.Wpf.Docking` |
| Compound data-entry form / settings panel / tile dashboard | `DevExpress.Wpf.LayoutControl` |
| Both — full app shell with docking + per-panel layout | Install both |

```bash
dotnet add package DevExpress.Wpf.Docking
dotnet add package DevExpress.Wpf.LayoutControl
```

Both transitively pull in `DevExpress.Wpf.Core`. All DevExpress packages in a project must share the same version.

## Step 2: Configure the Project

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net8.0-windows</TargetFramework>
    <UseWPF>true</UseWPF>
    <Nullable>enable</Nullable>
  </PropertyGroup>
</Project>
```

## Step 3: Add the XAML Namespaces

In `MainWindow.xaml`:

```xaml
<Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:dxdo="http://schemas.devexpress.com/winfx/2008/xaml/docking"
        xmlns:dxlc="http://schemas.devexpress.com/winfx/2008/xaml/layoutcontrol"
        xmlns:dxe="http://schemas.devexpress.com/winfx/2008/xaml/editors"
        Title="MyApp" Height="600" Width="900">
    ...
</Window>
```

| Prefix | URI | For |
|---|---|---|
| `dxdo:` | `.../winfx/2008/xaml/docking` | `DockLayoutManager` and its panels/groups |
| `dxlc:` | `.../winfx/2008/xaml/layoutcontrol` | All five `*LayoutControl` variants, `LayoutItem`, `LayoutGroup`, `Tile`, `GroupBox` |
| `dxe:` | `.../winfx/2008/xaml/editors` | Editors (`TextEdit`, `ComboBoxEdit`, etc.) often used inside layout controls |

> **Critical**: `LayoutGroup` exists in both `dxdo:` and `dxlc:` namespaces and refers to different types. Always check which namespace you're in.

## Step 4: Pick a Quick-Start Scenario

### A: Visual-Studio-Style Shell (`DockLayoutManager`)

```xaml
<Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:dxdo="http://schemas.devexpress.com/winfx/2008/xaml/docking"
        Title="Shell" Height="600" Width="900">
    <dxdo:DockLayoutManager>
        <dxdo:LayoutGroup Orientation="Horizontal">
            <dxdo:LayoutPanel Caption="Solution Explorer" ItemWidth="240"/>
            <dxdo:DocumentGroup>
                <dxdo:DocumentPanel Caption="Start Page"/>
                <dxdo:DocumentPanel Caption="MainWindow.xaml"/>
            </dxdo:DocumentGroup>
            <dxdo:LayoutPanel Caption="Properties" ItemWidth="240"/>
        </dxdo:LayoutGroup>
    </dxdo:DockLayoutManager>
</Window>
```

Users can drag panels, dock them to edges, float them, switch documents via the document selector.

### B: Data-Entry Form (`LayoutControl`)

```xaml
<dxlc:LayoutControl Orientation="Vertical">
    <dxlc:LayoutGroup View="GroupBox" Header="Personal" Orientation="Vertical">
        <dxlc:LayoutItem Label="First Name">
            <dxe:TextEdit EditValue="{Binding FirstName}"/>
        </dxlc:LayoutItem>
        <dxlc:LayoutItem Label="Last Name">
            <dxe:TextEdit EditValue="{Binding LastName}"/>
        </dxlc:LayoutItem>
    </dxlc:LayoutGroup>
    <dxlc:LayoutGroup View="GroupBox" Header="Contact" Orientation="Vertical">
        <dxlc:LayoutItem Label="Phone">
            <dxe:TextEdit EditValue="{Binding Phone}"/>
        </dxlc:LayoutItem>
        <dxlc:LayoutItem Label="Email">
            <dxe:TextEdit EditValue="{Binding Email}"/>
        </dxlc:LayoutItem>
    </dxlc:LayoutGroup>
</dxlc:LayoutControl>
```

Labels auto-align across groups, including across vertically stacked groups.

### C: Auto-Generated Form (`DataLayoutControl`)

```csharp
public class Person {
    [Display(GroupName = "<Name>", Name = "First name", Order = 0)]
    public string FirstName { get; set; } = "";

    [Display(GroupName = "<Name>", Name = "Last name", Order = 1)]
    public string LastName { get; set; } = "";

    [Display(GroupName = "{Tabs}/Contact"), DataType(DataType.PhoneNumber)]
    public string Phone { get; set; } = "";

    [Display(GroupName = "{Tabs}/Contact")]
    public string Email { get; set; } = "";

    [Display(GroupName = "{Tabs}/Job")]
    public string Title { get; set; } = "";
}
```

```xaml
<dxlc:DataLayoutControl CurrentItem="{Binding CurrentPerson}"/>
```

No layout XAML needed — the control generates labels, editors (chosen by type), and groups/tabs from the attributes.

### D: Tile Dashboard (`TileLayoutControl`)

```xaml
<dxlc:TileLayoutControl>
    <dxlc:Tile Header="Mail"    Size="Small"      Background="#FF1976D2"/>
    <dxlc:Tile Header="People"  Size="Small"      Background="#FF7B1FA2"/>
    <dxlc:Tile Header="Calendar" Size="Large"     Background="#FF388E3C"/>
    <dxlc:Tile Header="News"    Size="ExtraLarge" Background="#FFE53935"/>
</dxlc:TileLayoutControl>
```

### E: Wrapping Cards (`FlowLayoutControl`)

```xaml
<dxlc:FlowLayoutControl Orientation="Horizontal">
    <dxlc:GroupBox Header="Card 1" Width="200" Height="150"/>
    <dxlc:GroupBox Header="Card 2" Width="200" Height="150"/>
    <dxlc:GroupBox Header="Card 3" Width="200" Height="150"/>
    <dxlc:GroupBox Header="Card 4" Width="200" Height="150"/>
</dxlc:FlowLayoutControl>
```

### F: Edge Docking in One Panel (`DockLayoutControl`)

```xaml
<dxlc:DockLayoutControl>
    <dxlc:GroupBox dxlc:DockLayoutControl.Dock="Top"    Height="60" Header="Toolbar"/>
    <dxlc:GroupBox dxlc:DockLayoutControl.Dock="Left"   Width="200" Header="Sidebar"/>
    <dxlc:GroupBox dxlc:DockLayoutControl.Dock="Bottom" Height="30" Header="Status"/>
    <dxlc:GroupBox dxlc:DockLayoutControl.Dock="Client" Header="Content"/>
</dxlc:DockLayoutControl>
```

For a simple shell without detachable panels. For Visual-Studio-style features (floating, MDI documents), use `DockLayoutManager` instead.

## Step 5: Build and Run

```bash
dotnet build
dotnet run
```

## .NET Framework Variant

Required assemblies (when not using NuGet):

- `DevExpress.Data.v<XX.X>.dll`
- `DevExpress.Xpf.Core.v<XX.X>.dll`
- `DevExpress.Xpf.Docking.v<XX.X>.dll` (for DockLayoutManager)
- `DevExpress.Xpf.LayoutControl.v<XX.X>.dll` (for the layout-control family)
- `DevExpress.Mvvm.v<XX.X>.dll`

## What to Learn Next

- [Control Varieties](control-varieties.md) — the picker. Pick one of the six controls.
- [Building Layouts](building-layouts.md) — per-control rules for assembling layouts.
- [Save and Restore Layout](save-restore-layout.md) — persistence patterns.

## Source Material

- `articles/controls-and-libraries/layout-management/tile-and-layout.md` (https://docs.devexpress.com/content/WPF/8085?md=true)
- `articles/controls-and-libraries/layout-management/dock-windows.md` (https://docs.devexpress.com/content/WPF/6191?md=true)

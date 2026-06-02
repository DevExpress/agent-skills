# Getting Started — Property Grid

This guide walks through adding the DevExpress `PropertyGridControl` to a .NET (6/7/8+) WPF project. Property Grid is a data-bound control: set `SelectedObject` to any CLR object, and the control automatically renders an appropriate editor for each public property.

## System Requirements

- .NET 6.0 / 7.0 / 8.0+ targeting Windows (or .NET Framework 4.6.2+)
- Visual Studio 2022+ or JetBrains Rider
- A valid DevExpress license

## Step 1: Install NuGet Packages

```bash
dotnet add package DevExpress.Wpf.PropertyGrid
```

| Package | Provides |
|---------|---------|
| `DevExpress.Wpf.PropertyGrid` | `PropertyGridControl`, `PropertyDefinition`, `CollectionDefinition`, `CategoryDefinition` |

`DevExpress.Wpf.PropertyGrid` transitively brings `DevExpress.Wpf.Core` and `DevExpress.Wpf.Grid.Core` (the grid engine that renders rows).

DevExpress publishes on **nuget.org** (recommended) and via the local feed from the Unified Component Installer. All DevExpress packages in a project must share the same version.

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

The `-windows` suffix is required.

## Step 3: Add the Property Grid

`MainWindow.xaml` — bind to any CLR object:

```xaml
<Window x:Class="MyApp.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:dxprg="http://schemas.devexpress.com/winfx/2008/xaml/propertygrid"
        xmlns:local="clr-namespace:MyApp"
        Title="MyApp" Width="500" Height="400">
    <Window.DataContext>
        <local:Customer/>
    </Window.DataContext>
    <dxprg:PropertyGridControl SelectedObject="{Binding}"/>
</Window>
```

`Customer.cs`:

```csharp
using System;

namespace MyApp;

public class Customer {
    public int Id { get; set; } = 1;
    public string FirstName { get; set; } = "Nancy";
    public string LastName { get; set; } = "Davolio";
    public Gender Gender { get; set; } = Gender.Female;
    public DateTime BirthDate { get; set; } = new DateTime(1948, 8, 12);
    public string Phone { get; set; } = "7138638137";
}

public enum Gender { Male, Female }
```

That's it — every public property is auto-rendered with a type-appropriate editor (text box for strings, date picker for `DateTime`, combo for enums, etc.).

## Step 4: Build and Run

```bash
dotnet build
dotnet run
```

## Step 5: Show Only Some Properties

To control which properties appear, set `ShowProperties="WithPropertyDefinitions"` and declare `PropertyDefinition`s:

```xaml
<dxprg:PropertyGridControl SelectedObject="{Binding}"
                           ShowProperties="WithPropertyDefinitions">
    <dxprg:PropertyDefinition Path="FirstName"/>
    <dxprg:PropertyDefinition Path="LastName"/>
    <dxprg:PropertyDefinition Path="BirthDate" IsReadOnly="True"/>
</dxprg:PropertyGridControl>
```

Other values:

| `ShowProperties` | Behavior |
|---|---|
| `All` (default) | Show every public property |
| `WithPropertyDefinitions` | Show only properties that have a `PropertyDefinition` |

See [property-definitions.md](property-definitions.md) for the full set of options.

## Step 6: MVVM Binding

The property grid works with any DataContext shape. For an MVVM app:

```xaml
<dxprg:PropertyGridControl SelectedObject="{Binding SelectedCustomer}"/>
```

The grid observes `INotifyPropertyChanged` on the bound object and refreshes when properties change externally. If your model does not implement INPC, call `propertyGrid.DataController.ResetCache()` after external mutations.

### Multi-Object Editing

```xaml
<dxprg:PropertyGridControl SelectedObjects="{Binding SelectedCustomers}"/>
```

Properties common to all selected objects show their shared value; properties whose values differ across objects show blank. Edits apply to all.

## .NET Framework 4.6.2+

For .NET Framework projects, install the same NuGet package (or add an assembly reference to `DevExpress.Xpf.PropertyGrid.v<version>.dll` from the Unified Component Installer). The XAML is identical. The csproj uses the classic format:

```xml
<Reference Include="DevExpress.Xpf.PropertyGrid.v26.1"/>
<Reference Include="DevExpress.Xpf.Core.v26.1"/>
<Reference Include="DevExpress.Xpf.Grid.Core.v26.1"/>
```

## What to Learn Next

- [Property Definitions](property-definitions.md) — pick which properties show, assign custom editors, mark read-only
- [Collection Definitions](collection-definitions.md) — let users add/edit/remove items in a collection property
- [Categories](categories.md) — group properties under headers or tabs
- [Expandability](expandability.md) — make nested objects expandable

## Source Material

- `articles/controls-and-libraries/property-grid/overview.md` (`xref:15641`)
- `articles/controls-and-libraries/property-grid/getting-started/creating-property-definitions.md` (`xref:15610`)
- `articles/controls-and-libraries/property-grid/getting-started/managing-collection-properties.md` (`xref:15718`)

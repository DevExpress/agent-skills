# Getting Started with the DevExpress WPF TreeList (.NET 8+)

This guide walks you through adding `DevExpress.Xpf.Grid.TreeListControl` to a .NET 8+ WPF project and binding it to a self-referential employee tree. For .NET Framework 4.x, see [getting-started-dotnet-fw.md](getting-started-dotnet-fw.md).

## System Requirements

- .NET 8.0+ targeting Windows
- Visual Studio 2022+ or JetBrains Rider
- A NuGet source providing DevExpress packages — see Step 1
- A valid DevExpress license

## Step 1: NuGet Source

Install the DevExpress packages from nuget.org — it's registered by default in Visual Studio and the .NET SDK.

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

The `-windows` suffix is required — TreeList is Windows-only.

## Step 3: Install NuGet Packages

```bash
dotnet add package DevExpress.Wpf.Grid.Core
```

For designer-first workflow (toolbox + Quick Actions), install `DevExpress.Wpf.Grid` instead.

Optional add-ons:

```bash
dotnet add package DevExpress.Wpf.Printing
```

## Step 4: Create the ViewModel and Data Model

```csharp
using DevExpress.Mvvm;
using System.Collections.Generic;

namespace MyApp.ViewModels;

public class MainViewModel : ViewModelBase {
    public List<Employee> Employees { get; }

    public MainViewModel() {
        Employees = new List<Employee> {
            new Employee { ID = 1,              Name = "Gregory S. Price",     Position = "President" },
            new Employee { ID = 2, ParentID=1,  Name = "Irma R. Marshall",     Department = "Marketing",  Position = "Vice President" },
            new Employee { ID = 3, ParentID=1,  Name = "John C. Powell",       Department = "Operations", Position = "Vice President" },
            new Employee { ID = 4, ParentID=1,  Name = "Christian P. Laclair", Department = "Production", Position = "Vice President" },
            new Employee { ID = 5, ParentID=2,  Name = "Brian C. Cowling",     Department = "Marketing",  Position = "Manager" },
            new Employee { ID = 6, ParentID=2,  Name = "Thomas C. Dawson",     Department = "Marketing",  Position = "Manager" },
            new Employee { ID = 9, ParentID=3,  Name = "Harold S. Brandes",    Department = "Operations", Position = "Manager" },
        };
    }
}

public class Employee {
    public int ID { get; set; }
    public int? ParentID { get; set; }   // null → root node
    public string Name { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
}
```

The President (`ID = 0`) is the root because no other record has `ID = 0` as its `ParentID` parent — wait, actually `0` is used both as the root's ID AND as the children's `ParentID`. The trick: the root's own `ParentID` is the default value (`0`), and there's no record with `ID = default` *other than the root*. So the root is "the row whose ParentID points to itself or to a non-existent ID". The article uses `ID = 0` for the root with implicit `ParentID = 0` (default for `int`).

**Robust alternative**: use `int?` for `ParentID` and set the root's `ParentID = null`. Or pick a sentinel value that no record uses (e.g., `-1`).

## Step 5: Add the TreeListControl to MainWindow

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
                         AutoGenerateColumns="AddNew"
                         EnableSmartColumnsGeneration="True">
        <dxg:TreeListControl.View>
            <dxg:TreeListView KeyFieldName="ID"
                              ParentFieldName="ParentID"
                              AutoWidth="True"
                              AutoExpandAllNodes="True"/>
        </dxg:TreeListControl.View>
    </dxg:TreeListControl>
</Window>
```

Run (`dotnet run` or F5). The tree builds itself from the flat `Employees` list and expands all nodes.

## Step 6: Define Columns Explicitly (Recommended for Production)

```xaml
<dxg:TreeListControl ItemsSource="{Binding Employees}">
    <dxg:TreeListControl.View>
        <dxg:TreeListView KeyFieldName="ID" ParentFieldName="ParentID"
                          AutoWidth="True" AutoExpandAllNodes="False"/>
    </dxg:TreeListControl.View>
    <dxg:TreeListColumn FieldName="Name"       Header="Employee"  Width="220"/>
    <dxg:TreeListColumn FieldName="Department" Header="Dept"      Width="120"/>
    <dxg:TreeListColumn FieldName="Position"   Header="Position"  Width="150"/>
</dxg:TreeListControl>
```

When you define columns explicitly, set `AutoGenerateColumns="None"` (the default).

## Step 7: Build and Run

```bash
dotnet build
dotnet run
```

## What to Learn Next

- [Data Binding](data-binding.md) — hierarchical data, child-nodes selector, async loading, unbound mode
- [Editing](editing.md) — in-place editors, validation, edit forms
- [End-User Features](end-user-features.md) — sorting, filtering, multi-selection
- [Advanced Features](advanced-features.md) — drag-drop, conditional formatting, printing/export

## Source Material

- `articles/controls-and-libraries/tree-list/getting-started/lesson-1-add-a-treelistcontrol-to-a-project.md`

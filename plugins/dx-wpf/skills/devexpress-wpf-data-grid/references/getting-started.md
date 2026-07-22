# Getting Started with the DevExpress WPF Data Grid (.NET 8+)

This guide walks you through adding the `DevExpress.Xpf.Grid.GridControl` to a .NET 8+ WPF project and binding it to data. For .NET Framework 4.x, see [getting-started-dotnet-fw.md](getting-started-dotnet-fw.md).

## System Requirements

- .NET 8.0+ targeting Windows
- Visual Studio 2022+ (recommended) or JetBrains Rider
- Access to a NuGet source that has DevExpress packages — see Step 1
- A valid DevExpress license (TRIAL is OK for evaluation builds; production needs a valid subscription key matching the package version)

## Step 1: NuGet Source

Install the DevExpress packages from nuget.org — it's registered by default in Visual Studio and the .NET SDK.

If `nuget.org` is disabled in your environment, enable it:
```bash
dotnet nuget enable source nuget.org
```

## Step 2: Configure the Project

Your WPF `.csproj` must target Windows and enable WPF:

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

The `-windows` suffix is required — the Data Grid is Windows-only.

## Step 3: Install NuGet Packages

For a **code-first / MVVM** workflow, install the minimal `Core` package:

```bash
dotnet add package DevExpress.Wpf.Grid.Core
```

For a **designer-based** workflow (toolbox items, Items Source Wizard), install the full `DevExpress.Wpf.Grid` package instead:

```bash
dotnet add package DevExpress.Wpf.Grid
```

Optional add-ons:

```bash
dotnet add package DevExpress.Wpf.Printing                    # For Print Preview and report export
```

**All DevExpress packages in a project must use the same version.**

## Step 4: Create the ViewModel

```csharp
using DevExpress.Mvvm;
using System.Collections.ObjectModel;

namespace MyApp.ViewModels;

public class MainViewModel : ViewModelBase {
    public ObservableCollection<Order> Orders { get; }

    public MainViewModel() {
        Orders = new ObservableCollection<Order> {
            new Order { OrderId = 10248, OrderDate = new DateTime(1996, 7, 4), ShipCity = "Reims",  Freight = 32.38m },
            new Order { OrderId = 10249, OrderDate = new DateTime(1996, 7, 5), ShipCity = "Munster", Freight = 11.61m },
            new Order { OrderId = 10250, OrderDate = new DateTime(1996, 7, 8), ShipCity = "Rio de Janeiro", Freight = 65.83m },
        };
    }
}

public class Order {
    public int OrderId { get; set; }
    public DateTime OrderDate { get; set; }
    public string ShipCity { get; set; } = string.Empty;
    public decimal Freight { get; set; }
}
```

`ViewModelBase` lives in `DevExpress.Mvvm` (part of the MVVM Framework included with the Data Grid Core package).

## Step 5: Add the GridControl to MainWindow

```xaml
<Window x:Class="MyApp.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:dxg="http://schemas.devexpress.com/winfx/2008/xaml/grid"
        xmlns:dxe="http://schemas.devexpress.com/winfx/2008/xaml/editors"
        xmlns:vm="clr-namespace:MyApp.ViewModels"
        Title="My First Grid" Height="450" Width="800">
    <Window.DataContext>
        <vm:MainViewModel/>
    </Window.DataContext>
    <Grid>
        <dxg:GridControl ItemsSource="{Binding Orders}"
                         AutoGenerateColumns="AddNew"
                         EnableSmartColumnsGeneration="True">
            <dxg:GridControl.View>
                <dxg:TableView AutoWidth="True"
                               BestFitModeOnSourceChange="VisibleRows"
                               ShowGroupPanel="True"/>
            </dxg:GridControl.View>
        </dxg:GridControl>
    </Grid>
</Window>
```

Run the project (`dotnet run` or F5). The grid auto-generates a column per public property on `Order` and uses the smart-columns generator to pick appropriate editors (text for strings, numeric for `decimal`, date picker for `DateTime`).

## Step 6: Add Columns Explicitly (Recommended for Production)

Auto-generated columns are great for prototypes. For production, define columns explicitly to control order, format, and editor:

```xaml
<dxg:GridControl ItemsSource="{Binding Orders}">
    <dxg:GridControl.View>
        <dxg:TableView AutoWidth="True"/>
    </dxg:GridControl.View>
    <dxg:GridColumn FieldName="OrderId" ReadOnly="True" Header="Order #"/>
    <dxg:GridColumn FieldName="OrderDate" Header="Date"/>
    <dxg:GridColumn FieldName="ShipCity" Header="Ship to"/>
    <dxg:GridColumn FieldName="Freight">
        <dxg:GridColumn.EditSettings>
            <dxe:TextEditSettings Mask="c"
                                  MaskType="Numeric"
                                  MaskUseAsDisplayFormat="True"/>
        </dxg:GridColumn.EditSettings>
    </dxg:GridColumn>
</dxg:GridControl>
```

When you define columns explicitly, set `AutoGenerateColumns="None"` (the default) — otherwise both explicit and auto-generated columns appear.

## Step 7: Bind to a Database (Entity Framework Core)

Replace the in-memory collection with an Entity Framework Core source:

```csharp
using DevExpress.Mvvm;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using MyApp.Models;

public class MainViewModel : ViewModelBase {
    NorthwindContext _db;

    public ICollection<Order> Orders {
        get => GetValue<ICollection<Order>>();
        private set => SetValue(value);
    }

    public MainViewModel() {
        _db = new NorthwindContext();
        _db.Orders.Load();
        Orders = _db.Orders.Local;
    }
}
```

`_db.Orders.Local` returns an observable view of the local change tracker, so the grid reflects edits in real time.

## Step 8: Build and Run

```bash
dotnet build
dotnet run
```

The grid opens with the bound data, full sorting/filtering/grouping enabled, and smart editors per data type.

## What to Learn Next

- [Data Binding](data-binding.md) — server mode, virtual sources, CRUD with the MVVM pattern
- [Views](views.md) — switch to Card View or TreeList View, configure column auto-sizing
- [Sorting, Filtering, Grouping](sorting-filtering-grouping.md) — fine-tune end-user features
- [Editing](editing.md) — in-place editors, validation, clipboard paste
- [Conditional Formatting](conditional-formatting.md) — highlight cells based on rules

## Source Material

The instructions above are derived from these DevExpress articles in this docs repo:
- `articles/controls-and-libraries/data-grid/getting-started/code/lesson-1-add-a-gridcontrol-to-a-project.md`
- `articles/controls-and-libraries/data-grid/getting-started/code/lesson-2-display-and-edit-data.md`

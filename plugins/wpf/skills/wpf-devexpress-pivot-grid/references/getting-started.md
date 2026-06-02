# Getting Started with the DevExpress WPF Pivot Grid (.NET 6/7/8+)

This guide walks you through adding `DevExpress.Xpf.PivotGrid.PivotGridControl` to a .NET (6/7/8+) WPF project and binding it to a data source. For .NET Framework 4.x, see [getting-started-dotnet-fw.md](getting-started-dotnet-fw.md).

## System Requirements

- .NET 6.0 / 7.0 / 8.0+ targeting Windows
- Visual Studio 2022+ or JetBrains Rider
- A NuGet source providing DevExpress packages
- (For MDB binding) [Microsoft Access Database Engine 2016 Redistributable](https://www.microsoft.com/en-us/download/details.aspx?id=54920)
- A valid DevExpress license

## Step 1: Pick a NuGet Source

DevExpress packages are published in three places:

- **`nuget.org`** (recommended) — public feed, configured by default. If disabled: `dotnet nuget enable source nuget.org`.
- **Local feed from the Unified Component Installer** — `C:\Program Files\DevExpress {version}\Components\System\Components\Packages`.
- **Private feed** at `https://nuget.devexpress.com/api` — only for non-DE/ES/JA satellite assemblies or specific alpha scenarios.

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

## Step 3: Install NuGet Packages

```bash
dotnet add package DevExpress.Wpf.PivotGrid
```

Optional:

```bash
dotnet add package DevExpress.Wpf.Printing       # For Print Preview / export
dotnet add package DevExpress.Wpf.Charts         # For Chart integration
```

## Step 4: Add PivotGridControl to MainWindow

The Pivot Grid's XAML namespace is **different from GridControl**: use `dxpg:`, not `dxg:`.

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

`DataProcessingEngine="Optimized"` enables the modern high-performance engine (set automatically when the control is added from the Visual Studio toolbox).

Source: `templates/add-pivot-to-the-net-core-wpf-project.md` (template include, referenced by Lesson 1).

## Step 5: Build the Data Source

A sample sales table for the pivot:

```csharp
using System;
using System.Collections.Generic;

public class Sale {
    public string Country { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public decimal Amount { get; set; }
}

public static class SalesData {
    public static List<Sale> Build() => new() {
        new Sale { Country = "USA",    Category = "Beverages", OrderDate = new DateTime(2024, 1, 15),  Amount = 1200m },
        new Sale { Country = "USA",    Category = "Beverages", OrderDate = new DateTime(2024, 6, 10),  Amount =  900m },
        new Sale { Country = "USA",    Category = "Produce",   OrderDate = new DateTime(2024, 3,  5),  Amount =  450m },
        new Sale { Country = "Canada", Category = "Beverages", OrderDate = new DateTime(2024, 2, 20),  Amount =  800m },
        new Sale { Country = "Canada", Category = "Produce",   OrderDate = new DateTime(2024, 9, 12),  Amount = 1500m },
        new Sale { Country = "France", Category = "Beverages", OrderDate = new DateTime(2025, 4,  8),  Amount = 1100m },
        new Sale { Country = "France", Category = "Produce",   OrderDate = new DateTime(2025, 7, 22),  Amount =  700m },
        new Sale { Country = "Japan",  Category = "Produce",   OrderDate = new DateTime(2025, 11, 3),  Amount =  650m },
    };
}
```

## Step 6: Wire the Pivot Grid in Code-Behind

```csharp
using DevExpress.Xpf.PivotGrid;
using System.Windows;

namespace MyApp;

public partial class MainWindow : System.Windows.Window {
    public MainWindow() => InitializeComponent();

    private void Window_Loaded(object sender, RoutedEventArgs e) {
        pivotGridControl1.DataSource = SalesData.Build();

        pivotGridControl1.BeginUpdate();
        AddField("Country",  FieldArea.RowArea,    "Country");
        AddField("Category", FieldArea.RowArea,    "Category", areaIndex: 1);
        AddField("Year",     FieldArea.ColumnArea, "OrderDate", interval: FieldGroupInterval.DateYear);
        AddField("Sales",    FieldArea.DataArea,   "Amount");
        pivotGridControl1.EndUpdate();
    }

    void AddField(string caption, FieldArea area, string columnName,
                  int areaIndex = 0,
                  FieldGroupInterval interval = FieldGroupInterval.Default) {
        var field = pivotGridControl1.Fields.Add();
        field.Caption = caption;
        field.Area = area;
        field.DataBinding = new DataSourceColumnBinding(columnName) { GroupInterval = interval };
        field.AreaIndex = areaIndex;     // Set AFTER adding to the Fields collection.
    }
}
```

### What this builds

- **Rows**: Country → Category (two levels — Country first, then Category nested)
- **Columns**: Year (rolled up from daily `OrderDate` values via `DateYear` interval)
- **Cells**: Sum of `Amount` for each (Country, Category, Year) combination

Run (`dotnet run` or F5). Drag Country / Category / Year headers between areas at runtime to reshape the report.

## Step 7: Save and Restore Layout

To persist user-chosen layout across sessions:

```csharp
pivotGridControl1.SaveLayoutToXml("pivot-layout.xml");
pivotGridControl1.RestoreLayoutFromXml("pivot-layout.xml");
```

Stream-based overloads also exist. See [layout-and-fields.md](layout-and-fields.md) § Save / Restore Layout.

## What to Learn Next

- [Data Binding](data-binding.md) — DataTable, EF, OLAP, server mode, async, in-memory
- [Data Shaping](data-shaping.md) — aggregation, grouping intervals, sorting, filtering, calculated fields
- [Layout and Fields](layout-and-fields.md) — areas, field groups, customization form
- [End-User Features](end-user-features.md) — drag-drop, drill-down, filter dropdown, KPI
- [Advanced Features](advanced-features.md) — conditional formatting, chart integration, print/export

## Source Material

- `articles/controls-and-libraries/pivot-grid/getting-started/NET-Core/lesson-1-bind-a-pivot-grid-to-an-mdb-database-net.md`
- `articles/controls-and-libraries/pivot-grid/fundamentals/fields.md`
- `templates/add-pivot-to-the-net-core-wpf-project.md` (template include resolved)

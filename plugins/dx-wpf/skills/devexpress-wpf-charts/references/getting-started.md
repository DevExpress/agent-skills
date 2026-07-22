# Getting Started — DevExpress WPF Chart Control

This guide walks through setting up a `ChartControl` in a .NET 8+ WPF project and binding it to a simple data collection. For .NET Framework 4.6.2+, the same NuGet packages work; `App.xaml.cs` differs slightly (see end of this guide).

## System Requirements

- .NET 8.0+ targeting Windows
- Visual Studio 2022+ or JetBrains Rider
- A valid DevExpress license

## Step 1: Install the NuGet Package

```bash
dotnet add package DevExpress.Wpf.Charts
```

`DevExpress.Wpf.Charts` transitively brings `DevExpress.Wpf.Core`, `DevExpress.Data`, and the chart core libraries.

Optional add-ons:

```bash
dotnet add package DevExpress.Wpf.Printing
```

All DevExpress packages in a project must share the same version.

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

## Step 3: Add the XAML Namespace

In `MainWindow.xaml`:

```xaml
<Window x:Class="MyApp.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:dxc="http://schemas.devexpress.com/winfx/2008/xaml/charts"
        Title="MyApp" Height="400" Width="650">
    ...
</Window>
```

## Step 4: Build a Data Model

```csharp
public class SalesPoint {
    public string Region { get; set; } = "";
    public double Amount { get; set; }
}

public class MainViewModel {
    public ObservableCollection<SalesPoint> Data { get; }

    public MainViewModel() {
        Data = new ObservableCollection<SalesPoint> {
            new() { Region = "Asia",          Amount = 5.29 },
            new() { Region = "Australia",     Amount = 2.27 },
            new() { Region = "Europe",        Amount = 3.73 },
            new() { Region = "North America", Amount = 4.18 },
            new() { Region = "South America", Amount = 2.12 },
        };
    }
}
```

The data source must implement `IEnumerable` or `IListSource`. `ObservableCollection<T>`, `List<T>`, `DataTable`, EF results — all work.

## Step 5: Place the ChartControl

```xaml
<Window x:Class="MyApp.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:dxc="http://schemas.devexpress.com/winfx/2008/xaml/charts"
        xmlns:vm="clr-namespace:MyApp.ViewModels"
        Title="Sales by Region" Height="400" Width="650">
    <Window.DataContext>
        <vm:MainViewModel/>
    </Window.DataContext>
    <Grid>
        <dxc:ChartControl DataSource="{Binding Data}">

            <dxc:ChartControl.Titles>
                <dxc:Title Content="Sales by Region" HorizontalAlignment="Center"/>
            </dxc:ChartControl.Titles>

            <dxc:ChartControl.Legends>
                <dxc:Legend HorizontalPosition="Right" VerticalPosition="Top"/>
            </dxc:ChartControl.Legends>

            <dxc:XYDiagram2D>
                <dxc:BarSideBySideSeries2D DisplayName="Annual Sales"
                                           ArgumentDataMember="Region"
                                           ValueDataMember="Amount"
                                           CrosshairLabelPattern="${V:f2}M"/>
            </dxc:XYDiagram2D>
        </dxc:ChartControl>
    </Grid>
</Window>
```

## Anatomy of the Snippet

1. **`ChartControl.DataSource`** — the data is set once at the chart level. Series inherit it.
2. **`Diagram`** — `XYDiagram2D` for Cartesian (X/Y) charts. Pick the diagram **before** picking a series; they must be compatible.
3. **Series** (here `BarSideBySideSeries2D`) — defines how data is plotted.
   - `ArgumentDataMember="Region"` — the X (category) field
   - `ValueDataMember="Amount"` — the Y (value) field
4. **Titles** and **Legends** are sibling collections on `ChartControl`.

## Step 6: Build and Run

```bash
dotnet build
dotnet run
```

You should see a bar chart with one bar per region, a title at the top, and a legend on the right.

## .NET Framework Variant

Required assembly references (when not using NuGet):

- `DevExpress.Data.v<XX.X>.dll`
- `DevExpress.Xpf.Core.v<XX.X>.dll`
- `DevExpress.Charts.v<XX.X>.Core.dll`
- `DevExpress.Xpf.Charts.v<XX.X>.dll`
- `DevExpress.Mvvm.v<XX.X>.dll`
- `DevExpress.Xpf.Printing.v<XX.X>.dll` (only for print/export)

## What to Learn Next

- [Data Binding](data-binding.md) — full reference for `DataSource`, `*DataMember`, and MVVM series generation via `Diagram.SeriesItemsSource`
- [Series Types](series-types.md) — the 2D series inventory and how to pick one
- [Axes](axes.md) — `AxisX2D` / `AxisY2D`, secondary axes, scale types
- [Tooltip and Crosshair Cursor](tooltip-and-crosshair.md) — interactivity
- [Selection](selection.md) — let users click to select chart elements

## Source Material

- `articles/controls-and-libraries/charts-suite/chart-control/getting-started/getting-started.md` (https://docs.devexpress.com/content/WPF/5854?md=true)
- `articles/controls-and-libraries/charts-suite/chart-control/getting-started/lesson-1-bind-chart-series-to-data.md` (https://docs.devexpress.com/content/WPF/9757?md=true)

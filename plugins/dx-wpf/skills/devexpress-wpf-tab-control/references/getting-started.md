# Getting Started — Tab Control

This guide walks through adding the DevExpress `DXTabControl` to a .NET 8+ WPF project. The critical step is hosting the control inside `dx:ThemedWindow` — not a plain `Window`. Without `ThemedWindow`, you'll get visual artifacts (double title bar, mismatched colors between the title bar and the tab strip, broken hit-testing for left/right control box areas).

## System Requirements

- .NET 8.0+ targeting Windows (or .NET Framework 4.6.2+)
- Visual Studio 2022+ or JetBrains Rider
- A valid DevExpress license

## Step 1: Install NuGet Packages

```bash
dotnet add package DevExpress.Wpf.Core
```

| Package | Provides |
|---------|---------|
| `DevExpress.Wpf.Core` | `DXTabControl`, `DXTabItem`, `ThemedWindow` |

`DXTabControl` lives in `DevExpress.Xpf.Core.v<version>.dll` — the core package is enough for tab UIs.

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

## Step 3: Convert the Main Window to `ThemedWindow` (REQUIRED)

A plain `<Window>` will *technically* host `DXTabControl`, but you'll see:

- A double title bar (WPF's default chrome + the DevExpress theme's bar)
- Mismatched colors between the title bar and the tab strip
- Broken hit-testing for `ControlBoxLeftTemplate` / `ControlBoxRightTemplate` (they expect to render in the themed title bar area)
- Drag-out pop-out windows that don't pick up the theme

Convert `MainWindow.xaml` from `<Window>` to `<dx:ThemedWindow>`:

### Before

```xaml
<Window x:Class="MyApp.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="MyApp" Width="600" Height="400">
</Window>
```

### After

```xaml
<dx:ThemedWindow x:Class="MyApp.MainWindow"
                 xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                 xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                 xmlns:dx="http://schemas.devexpress.com/winfx/2008/xaml/core"
                 Title="MyApp" Width="600" Height="400">
</dx:ThemedWindow>
```

Then update `MainWindow.xaml.cs`:

```csharp
using DevExpress.Xpf.Core;

namespace MyApp;

public partial class MainWindow : ThemedWindow {
    public MainWindow() {
        InitializeComponent();
    }
}
```

### Visual Studio Shortcut

If the Window is selected in the designer, you can use **Quick Actions** → **Convert to ThemedWindow** to perform the conversion automatically.

## Step 4: Add the Tab Control

In `MainWindow.xaml`:

```xaml
<dx:ThemedWindow x:Class="MyApp.MainWindow"
                 xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                 xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                 xmlns:dx="http://schemas.devexpress.com/winfx/2008/xaml/core"
                 Title="MyApp" Width="600" Height="400">
    <dx:DXTabControl>
        <dx:DXTabItem Header="General">
            <Label Content="General settings…"/>
        </dx:DXTabItem>
        <dx:DXTabItem Header="Appearance">
            <Label Content="Appearance settings…"/>
        </dx:DXTabItem>
        <dx:DXTabItem Header="Advanced">
            <Label Content="Advanced settings…"/>
        </dx:DXTabItem>
    </dx:DXTabControl>
</dx:ThemedWindow>
```

## Step 5: Build and Run

```bash
dotnet build
dotnet run
```

Expected: a themed window with three tab headers across the top. Clicking a header switches to its content area.

## Step 6: Try a View

`DXTabControl` uses a `View` object to control how the header panel handles overflow. The default is a scroll view; pick another:

```xaml
<dx:DXTabControl>
    <dx:DXTabControl.View>
        <dx:TabControlMultiLineView/>
    </dx:DXTabControl.View>
    ...
</dx:DXTabControl>
```

See [views.md](views.md) for the three available views and when to use each.

## Step 7: Bind to a Data Source (Optional)

For dynamic tabs from a view-model collection:

```xaml
<dx:DXTabControl ItemsSource="{Binding Documents}">
    <dx:DXTabControl.ItemHeaderTemplate>
        <DataTemplate>
            <TextBlock Text="{Binding Title}"/>
        </DataTemplate>
    </dx:DXTabControl.ItemHeaderTemplate>
    <dx:DXTabControl.ItemTemplate>
        <DataTemplate>
            <ContentControl Content="{Binding Editor}"/>
        </DataTemplate>
    </dx:DXTabControl.ItemTemplate>
</dx:DXTabControl>
```

```csharp
public class MainViewModel {
    public ObservableCollection<DocumentViewModel> Documents { get; } = new();
}
```

Adding to / removing from `Documents` automatically adds / removes tabs. See [defining-tabs.md](defining-tabs.md).

## What to Learn Next

- [Defining Tabs](defining-tabs.md) — explicit `DXTabItem` children vs. `ItemsSource` + templates
- [Views](views.md) — MultiLine / Scroll / Stretch overflow behavior
- [Appearance](appearance.md) — per-tab colors, custom theming, templated regions

## Source Material

- `articles/controls-and-libraries/layout-management/tab-control.md` (https://docs.devexpress.com/content/WPF/7975?md=true)
- `DevExpress.Xpf.Core.DXTabControl` class reference
- `articles/controls-and-libraries/layout-management/tab-control/fundamentals/views.md` (https://docs.devexpress.com/content/WPF/7979?md=true)

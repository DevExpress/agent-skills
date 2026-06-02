# Getting Started — DevExpress WPF Loading Indicators

All three loading indicators (`SplashScreenManager`, `LoadingDecorator`, `WaitIndicator`) live in **a single NuGet package** — `DevExpress.Wpf.Core`. Install it once and all three are available. This guide gets the project set up and shows the first usage of each.

## System Requirements

- .NET 6.0 / 7.0 / 8.0+ targeting Windows (or .NET Framework 4.6.2+)
- Visual Studio 2022+ or JetBrains Rider
- A valid DevExpress license

## Step 1: Install the NuGet Package

```bash
dotnet add package DevExpress.Wpf.Core
```

This brings `SplashScreenManager`, `LoadingDecorator`, and `WaitIndicator` plus `ThemedWindow` and MVVM helpers — no extra dependency needed.

All DevExpress packages in a project must share the same version. DevExpress publishes on **nuget.org** (recommended) and via the local Unified Component Installer feed.

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

## Step 3: Set Up `App.xaml.cs` — Theme Required Before Splash

If you use **`SplashScreenManager.CreateThemed`**, set the theme **before** creating the splash. The splash picks its colors from the active theme.

```csharp
using DevExpress.Xpf.Core;

namespace MyApp;

public partial class App : System.Windows.Application {
    public App() {
        CompatibilitySettings.UseLightweightThemes = true;
        ApplicationThemeHelper.ApplicationThemeName = LightweightTheme.Win11Light.Name;
        SplashScreenManager.CreateThemed(new DXSplashScreenViewModel {
            Title = "MyApp",
            Subtitle = "Loading...",
            IsIndeterminate = true,
        }).ShowOnStartup();
    }
}
```

`ShowOnStartup()` shows the splash immediately on construction and auto-closes it when the main window's loaded event fires.

> **Why `System.Windows.Application` explicitly?** `DevExpress.Wpf.Core` transitively references `System.Windows.Forms`. With `<ImplicitUsings>enable</ImplicitUsings>` (default for `dotnet new wpf` on .NET 6+), unqualified `Application` is ambiguous.

## Step 4: Add the XAML Namespace

In `MainWindow.xaml`:

```xaml
<Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:dx="http://schemas.devexpress.com/winfx/2008/xaml/core"
        Title="MyApp" Height="500" Width="800">
    ...
</Window>
```

`LoadingDecorator` and `WaitIndicator` are XAML elements under this prefix:

- `<dx:LoadingDecorator>`
- `<dx:WaitIndicator>`

`SplashScreenManager` is code-only — no XAML element.

## Step 5: First Usage of Each Indicator

### A. SplashScreenManager — Startup Splash (Already Set Up in Step 3)

`SplashScreenManager` shines for **application startup** because it runs on a **separate UI thread** and continues animating even when the main thread is blocked loading data.

Variants (replace `CreateThemed` in Step 3 with one of these):

| Method | Look |
|---|---|
| `CreateThemed(DXSplashScreenViewModel?)` | Themed: colors derive from the application theme. |
| `CreateFluent(DXSplashScreenViewModel?)` | Fluent Design acrylic background. |
| `CreateWaitIndicator(DXSplashScreenViewModel?)` | Compact circular spinner + status text. |
| `Create(...)` | Custom splash window — see `xref:401690`. |

Show on demand instead of at startup:

```csharp
var manager = SplashScreenManager.CreateWaitIndicator(
    new DXSplashScreenViewModel { Status = "Loading data..." });
manager.Show();
// ... do long work
manager.Close();
```

### B. LoadingDecorator — Wrap a Slow-Loading Region

`LoadingDecorator` is a content container that **automatically** shows a loading indicator while the wrapped content is loading, then hides when content is ready.

```xaml
<Window xmlns:dx="http://schemas.devexpress.com/winfx/2008/xaml/core"
        xmlns:dxg="http://schemas.devexpress.com/winfx/2008/xaml/grid"
        ...>
    <Grid>
        <dx:LoadingDecorator>
            <dxg:GridControl ItemsSource="{Binding LargeDataset}">
                <!-- grid columns, etc. -->
            </dxg:GridControl>
        </dx:LoadingDecorator>
    </Grid>
</Window>
```

The decorator shows automatically while the GridControl renders, then disappears. No explicit visibility toggling needed for this pattern.

For manual control:

```xaml
<dx:LoadingDecorator IsSplashScreenShown="{Binding IsLoading}"
                     OwnerLock="LoadingContent">
    <Grid>...</Grid>
</dx:LoadingDecorator>
```

Set `IsLoading = true` on the ViewModel to show, `false` to hide.

### C. WaitIndicator — Simple Inline Busy Indicator

`WaitIndicator` is a popup panel you toggle inside your layout. Bind `DeferedVisibility` to a `bool`:

```xaml
<Grid>
    <Grid x:Name="mainContent">
        <!-- main UI -->
    </Grid>

    <dx:WaitIndicator DeferedVisibility="{Binding IsBusy}"
                      Content="Loading..."/>
</Grid>
```

```csharp
public class MainViewModel : ViewModelBase {
    public bool IsBusy { get => GetValue<bool>(); set => SetValue(value); }

    public async Task LoadAsync() {
        IsBusy = true;
        try { await DoWorkAsync(); }
        finally { IsBusy = false; }
    }
}
```

> **Use `DeferedVisibility`, not `Visibility`.** `DeferedVisibility` waits a short interval before showing, so quick (<100ms) operations don't briefly flash the indicator.

## Step 6: Build and Run

```bash
dotnet build
dotnet run
```

## Quick Picker

| Scenario | Use |
|---|---|
| Application startup | `SplashScreenManager.CreateThemed(...).ShowOnStartup()` |
| Long-running modal operation (export, import, sync) | `SplashScreenManager.CreateWaitIndicator(...).Show(this, ..., InputBlockMode.Owner)` |
| A specific region (a grid, a chart) takes time to load | `<dx:LoadingDecorator>` wrap |
| A button-triggered operation; want a quick inline spinner | `<dx:WaitIndicator DeferedVisibility="{Binding IsBusy}"/>` |

See [comparison.md](comparison.md) for the full picker with threading caveats and customization options.

## .NET Framework Variant

`<ImplicitUsings>` isn't enabled by default on .NET Framework, so the `Application` clash doesn't occur:

```csharp
using DevExpress.Xpf.Core;
using System.Windows;

namespace MyApp;

public partial class App : Application {
    public App() {
        CompatibilitySettings.UseLightweightThemes = true;
        ApplicationThemeHelper.ApplicationThemeName = LightweightTheme.Win11Light.Name;
        SplashScreenManager.CreateThemed().ShowOnStartup();
    }
}
```

Required assemblies (when not using NuGet):

- `DevExpress.Data.v<XX.X>.dll`
- `DevExpress.Xpf.Core.v<XX.X>.dll`
- `DevExpress.Mvvm.v<XX.X>.dll`

## Migration: Legacy `DXSplashScreen` → `SplashScreenManager`

`DXSplashScreen` is marked **legacy** in current DevExpress docs. New apps should use `SplashScreenManager`.

| Old (`DXSplashScreen`) | New (`SplashScreenManager`) |
|---|---|
| `DXSplashScreen.Show<SplashScreenView>()` | `SplashScreenManager.CreateThemed(...).ShowOnStartup()` (or `.Show()`) |
| `DXSplashScreen.Close()` | `SplashScreenManager.CloseAll()` or per-instance `.Close()` |
| Custom splash via project template | `SplashScreenManager.Create(typeof(CustomSplashWindow))` — see `xref:401690` |
| Auto-invoke via `[STAThread]` and `DXSplashScreen.Show...` | `SplashScreenManager.Create*().ShowOnStartup()` in `App` constructor |

Both run on a separate UI thread; the migration is mostly an API rename + restructuring the call site to the `Create*().Show*()` chain.

## What to Learn Next

- [Comparison](comparison.md) — when to pick each indicator, all configuration options, MVVM patterns

## Source Material

- `articles/controls-and-libraries/windows-and-utility-controls/splash-screen-manager.md` (`xref:401685`)
- `articles/controls-and-libraries/windows-and-utility-controls/wait-indicator.md` (`xref:114373`)
- `articles/controls-and-libraries/windows-and-utility-controls.md` (`xref:115521`)
- `articles/controls-and-libraries/windows-and-utility-controls/dxsplashscreen.md` (`xref:9949`) — legacy
- DxDocs API: `DevExpress.Xpf.Core.LoadingDecorator` (verified)

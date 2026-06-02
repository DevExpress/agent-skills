---
name: wpf-devexpress-loading-indicators
description: Display loading indicators in WPF apps with the DevExpress utility controls — SplashScreenManager (a window-level splash screen shown during startup or long-running operations, runs on a separate UI thread), LoadingDecorator (a content container that wraps any UI and shows an indicator while it loads, with owner-lock modes), and WaitIndicator (a simple popup panel that runs on the main UI thread). Use when adding a startup splash screen via SplashScreenManager.CreateThemed / CreateFluent / CreateWaitIndicator + ShowOnStartup; wrapping content with dx:LoadingDecorator and configuring OwnerLock and SplashScreenTemplate; toggling dx:WaitIndicator visibility with DeferedVisibility; deciding which indicator to use for which scenario. Also use when someone mentions "DevExpress.Xpf.Core.SplashScreenManager", "DXSplashScreenViewModel", "LoadingDecorator", "WaitIndicator", "DXSplashScreen" (legacy). Covers .NET (6/7/8+) and .NET Framework 4.6.2+.
compatibility: Requires .NET 6+ or .NET Framework 4.6.2+ targeting Windows (net8.0-windows). DevExpress NuGet packages are published on nuget.org and via the local Unified Component Installer feed. A valid DevExpress license is required.
metadata:
  author: DevExpress
  version: "26.1"
  source-commit: b16066c633b85ee391d1e6188ffc4cd815ee5e8f
---

# DevExpress WPF Loading Indicators

Three different controls for "show the user that something is happening" — but they're not interchangeable. **`SplashScreenManager`** is a window-level splash screen for app startup or long operations (runs on a **separate UI thread**, immune to main-thread freezes). **`LoadingDecorator`** is a content container that wraps any UI and shows an indicator while the content loads (configurable owner-lock). **`WaitIndicator`** is a simple panel shown inside a window (runs on the **main UI thread**). All three live in `DevExpress.Xpf.Core`.

> The older **`DXSplashScreen`** API is **legacy**. New apps should use `SplashScreenManager` instead.

## When to Use This Skill

Use this skill when you need to:

- Show a splash screen at application startup
- Show a loading indicator over a region while it's fetching data
- Show a quick spinner while a button-triggered operation runs
- Decide between SplashScreenManager / LoadingDecorator / WaitIndicator
- Migrate from the legacy `DXSplashScreen` to `SplashScreenManager`

## Prerequisites & Installation

### NuGet Packages

| Package | Provides |
|---------|---------|
| `DevExpress.Wpf.Core` | `SplashScreenManager`, `LoadingDecorator`, `WaitIndicator`, `ThemedWindow`, themes |
| `DevExpress.Wpf.ThemesLW` | Lightweight Themes (e.g. `LightweightTheme.Win11Light` / `Win11Dark`) — recommended for faster startup and lower memory |

All three indicators live in the **`DevExpress.Wpf.Core` package**. Add `DevExpress.Wpf.ThemesLW` when using Lightweight Themes.

```bash
dotnet add package DevExpress.Wpf.Core
dotnet add package DevExpress.Wpf.ThemesLW
```

All DevExpress packages in a project must share the same version. A valid DevExpress license is required.

## XAML Namespace

```xml
xmlns:dx="http://schemas.devexpress.com/winfx/2008/xaml/core"
```

`LoadingDecorator` and `WaitIndicator` are XAML elements (`<dx:LoadingDecorator>`, `<dx:WaitIndicator>`). `SplashScreenManager` is API-only — call methods from code (typically `App.xaml.cs`).

## The Three Indicators at a Glance

| | SplashScreenManager | LoadingDecorator | WaitIndicator |
|---|---|---|---|
| **What it is** | Window-level splash screen | Content container with built-in loading indicator | Popup panel inside a window |
| **Runs on** | **Separate UI thread** — immune to main-thread freezes | Main UI thread | Main UI thread |
| **Typical use** | App startup; very long blocking operations | Wrap a panel/grid/chart while data loads | Quick "busy" indicator during a button-triggered operation |
| **API style** | Code (`Create*().Show()`) | XAML element (`<dx:LoadingDecorator>...</dx:LoadingDecorator>`) | XAML element (`<dx:WaitIndicator/>`) |
| **Trigger** | `Show()` / `ShowOnStartup()` / `CloseAll()` | Wraps content — auto-shows during content loading; `IsSplashScreenShown` for manual control | `DeferedVisibility` toggle |
| **Locks UI** | Optional, configurable (`InputBlockMode`) | Configurable per-element via `OwnerLock` (`Full` / `InputOnly` / `LoadingContent` / `None`) | Doesn't block input by itself |
| **Recommended for** | Startup, long-running modal operations | Slow-loading panels / regions | Simple inline "busy" |

## Before You Start — Ask the Developer

1. **What's the trigger?**
   - App startup → **SplashScreenManager** (`ShowOnStartup`)
   - A region that takes a long time to render → **LoadingDecorator**
   - A button click that triggers an operation → **SplashScreenManager.CreateWaitIndicator()** (separate thread, simple), or **WaitIndicator** with `DeferedVisibility` (main thread, in-window)
2. **Does the main thread get blocked?** If yes → only `SplashScreenManager` works (separate UI thread). The other two animate on the main thread and freeze with it.
3. **Should the user still see the rest of the app?** LoadingDecorator's `OwnerLock` decides what's locked while content loads (entire window / input only / just the decorator / nothing).
4. **MVVM?** All three have MVVM-friendly options: `SplashScreenManagerService`, `LoadingDecorator.IsSplashScreenShown` (bindable), `WaitIndicator.DeferedVisibility` (bindable).

## Documentation & Navigation Guide

### Getting Started
Refer to [references/getting-started.md](references/getting-started.md)

When you need to:
- Add the NuGet package
- Place each indicator on a window for the first time
- Migrate from legacy `DXSplashScreen` to `SplashScreenManager`

### Comparison and Picking the Right One
Refer to [references/comparison.md](references/comparison.md)

When you need to:
- Pick between the three for a specific scenario
- Understand the thread-safety implications (main vs separate UI thread)
- Configure `SplashScreenManager` (Themed / Fluent / WaitIndicator / Custom)
- Configure `LoadingDecorator` (`OwnerLock`, `SplashScreenTemplate`, `SplashScreenLocation`, fade effects, border)
- Configure `WaitIndicator` (`DeferedVisibility`, custom `ContentTemplate`)

## Quick Start Examples

### 1. SplashScreenManager — Startup

```csharp
// App.xaml.cs
using DevExpress.Xpf.Core;

namespace MyApp;

public partial class App : System.Windows.Application {
    public App() {
        CompatibilitySettings.UseLightweightThemes = true;
        ApplicationThemeHelper.ApplicationThemeName = LightweightTheme.Win11Dark.Name;
        SplashScreenManager.CreateThemed(new DXSplashScreenViewModel {
            Title = "MyApp",
            Subtitle = "Loading...",
            Copyright = "© 2026",
            IsIndeterminate = true,
        }).ShowOnStartup();
    }
}
```

`ShowOnStartup` auto-closes the splash when the main window finishes loading.

### 2. LoadingDecorator — Slow-Loading Content

```xaml
<dx:ThemedWindow xmlns:dx="http://schemas.devexpress.com/winfx/2008/xaml/core"
                 ...>
    <Grid>
        <dx:LoadingDecorator>
            <dxg:GridControl ItemsSource="{Binding HugeDataset}"/>
        </dx:LoadingDecorator>
    </Grid>
</dx:ThemedWindow>
```

The decorator wraps the grid; the loading indicator shows automatically while the grid is loading. When loading completes, the indicator disappears.

### 3. WaitIndicator — Inline Busy Indicator

```xaml
<Grid>
    <SomeUserControl/>
    <dx:WaitIndicator DeferedVisibility="{Binding IsBusy}"
                      Content="Loading..."/>
</Grid>
```

Bind `DeferedVisibility` to a ViewModel boolean. When `IsBusy` becomes `true`, the indicator appears over the content.

### 4. SplashScreenManager — On-Demand

```csharp
// Triggered by a button click — operation takes 2-5 seconds
private async void OnExport_Click(object sender, RoutedEventArgs e) {
    var manager = SplashScreenManager.CreateWaitIndicator(
        new DXSplashScreenViewModel { Status = "Exporting..." });
    manager.Show(this, WindowStartupLocation.CenterOwner,
                 trackOwnerPosition: true,
                 inputBlock: InputBlockMode.Owner);

    try {
        await ExportAsync();
    } finally {
        manager.Close();
    }
}
```

## Key Properties & API Surface

### `SplashScreenManager`

| Method | Use |
|---|---|
| `CreateThemed(DXSplashScreenViewModel?)` | Themed splash with app colors |
| `CreateFluent(DXSplashScreenViewModel?)` | Fluent Design acrylic background |
| `CreateWaitIndicator(DXSplashScreenViewModel?)` | Compact loading-indicator splash |
| `Create(...)` | Custom splash window — see docs |
| `Show(owner, startupLocation, trackOwnerPosition, inputBlock, timeout)` — and overload `Show(showDelay, minDuration, owner, ..., timeout)`. All time values are `int` milliseconds. | Show the splash |
| `ShowOnStartup()` | Show + auto-close on main window load |
| `Close()` | Close this splash |
| `CloseAll()` (static) | Close all active splashes |
| `ActiveSplashScreens` (static) | Iterate currently-shown splashes |
| `ViewModel` | Bindable `DXSplashScreenViewModel` instance (update its props at runtime) |

| `DXSplashScreenViewModel` | Use |
|---|---|
| `Title` / `Subtitle` / `Copyright` | Caption text |
| `Logo` | URI to a logo image |
| `Status` | Status text (shown in WaitIndicator style) |
| `Progress` / `IsIndeterminate` | Progress bar value / animation toggle |
| `Tag` | Custom data object (for `Create` custom splashes) |

| `InputBlockMode` | Effect |
|---|---|
| `None` | All user input allowed |
| `Owner` | Blocks input to the element passed as `owner` |
| `Window` | Blocks input to the entire window the splash was shown from |
| `WindowContent` | Partially blocks the owner window — window can still be dragged / minimized / maximized |

### `LoadingDecorator`

| Property | Use |
|---|---|
| `OwnerLock` | `Full` (default — block entire window), `InputOnly` (allow window drag, block input), `LoadingContent` (block only the decorator's content), `None` (nothing blocked) |
| `IsSplashScreenShown` | Bindable bool — manually toggle the indicator |
| `UseSplashScreen` | `false` to disable the indicator entirely (rare) |
| `SplashScreenTemplate` | `DataTemplate` for the indicator content |
| `SplashScreenDataContext` | DataContext for the template |
| `SplashScreenLocation` | `CenterWindow`, `CenterScreen`, `CenterContainer` |
| `BorderEffect` / `BorderEffectColor` | Highlight border around the decorated content |
| `UseFadeEffect` / `FadeInDuration` / `FadeOutDuration` | Fade animation control |

### `WaitIndicator`

| Property | Use |
|---|---|
| `DeferedVisibility` | Show/hide with a delay (bindable). Use this instead of `Visibility`. |
| `Content` | Text or object shown as the indicator caption |
| `ContentTemplate` | Custom layout template |

## Common Patterns

### Startup with Logo and Status

```csharp
public App() {
    CompatibilitySettings.UseLightweightThemes = true;
    ApplicationThemeHelper.ApplicationThemeName = LightweightTheme.Win11Light.Name;
    SplashScreenManager.CreateThemed(new DXSplashScreenViewModel {
        Logo = new Uri("pack://application:,,,/Images/Logo.png", UriKind.RelativeOrAbsolute),
        Title = "Contoso ERP",
        Subtitle = "Edition: Pro",
        Copyright = "© Contoso 2026",
        Status = "Loading modules...",
        IsIndeterminate = true,
    }).ShowOnStartup();
}
```

### Progress Bar Updates from Background Work

```csharp
async Task RunImport() {
    var manager = SplashScreenManager.CreateThemed(new DXSplashScreenViewModel {
        Title = "Import",
        IsIndeterminate = false,
    });
    manager.Show();

    for (int i = 0; i <= 100; i++) {
        await DoStepAsync();
        manager.ViewModel.Progress = i;
        manager.ViewModel.Status = $"Processing {i}%...";
    }

    manager.Close();
}
```

Updates flow into the splash via the bindable `ViewModel.Progress` / `ViewModel.Status` even though the splash runs on a separate UI thread.

### LoadingDecorator with Manual Trigger (MVVM)

```xaml
<dx:LoadingDecorator IsSplashScreenShown="{Binding IsLoading}"
                     OwnerLock="LoadingContent">
    <Grid>
        <!-- main panel content -->
    </Grid>
</dx:LoadingDecorator>
```

```csharp
public class MainViewModel : ViewModelBase {
    public bool IsLoading {
        get => GetValue<bool>();
        set => SetValue(value);
    }

    public async Task LoadDataAsync() {
        IsLoading = true;
        try { /* ... */ }
        finally { IsLoading = false; }
    }
}
```

`OwnerLock="LoadingContent"` blocks input only on the decorated panel — the rest of the window stays interactive.

### LoadingDecorator with Custom Indicator

```xaml
<dx:LoadingDecorator SplashScreenDataContext="Saving your changes...">
    <dx:LoadingDecorator.SplashScreenTemplate>
        <DataTemplate>
            <dx:WaitIndicator Content="{Binding}" DeferedVisibility="True">
                <dx:WaitIndicator.ContentTemplate>
                    <DataTemplate>
                        <StackPanel Orientation="Vertical">
                            <TextBlock Text="Please wait" FontSize="20" FontWeight="Bold"/>
                            <TextBlock Text="{Binding}" Margin="0,4,0,0"/>
                        </StackPanel>
                    </DataTemplate>
                </dx:WaitIndicator.ContentTemplate>
            </dx:WaitIndicator>
        </DataTemplate>
    </dx:LoadingDecorator.SplashScreenTemplate>

    <Grid>...</Grid>
</dx:LoadingDecorator>
```

The `LoadingDecorator` uses a `WaitIndicator` internally — you can supply your own template via `SplashScreenTemplate`.

### Inline WaitIndicator Bound to IsBusy

```xaml
<Grid>
    <ContentPresenter Content="{Binding MainContent}"/>
    <dx:WaitIndicator DeferedVisibility="{Binding IsBusy}"
                      Content="Processing..."/>
</Grid>
```

For an inline indicator inside a region without locking the rest of the window.

## Troubleshooting

| Symptom | Cause | Solution |
|---|---|---|
| `dx:` prefix unresolved | Missing namespace / NuGet | Add `xmlns:dx="http://schemas.devexpress.com/winfx/2008/xaml/core"`; install `DevExpress.Wpf.Core`. |
| Splash screen animation freezes during long startup work | Tried to use `WaitIndicator` or `LoadingDecorator` at startup; both run on the main thread | Use `SplashScreenManager` instead — runs on a separate UI thread, immune to main-thread blocks. |
| `LoadingDecorator` shows but doesn't hide | Wrapped content doesn't trigger "loaded" event, or `IsSplashScreenShown="True"` was set without setting it back to `false` | Bind `IsSplashScreenShown` to a ViewModel boolean and toggle it; or omit the binding and let the decorator detect content loading. |
| `WaitIndicator` shows but isn't centered | Container layout pushes it; `Visibility` toggled but not `DeferedVisibility` | Use `DeferedVisibility` (not `Visibility`); ensure the indicator is the last child in the parent panel so it overlays. |
| Splash appears for a fraction of a second on fast startups | Operation was faster than the splash setup | Pass `showDelay` to `Show()` — splash only appears if operation takes longer than `showDelay`. Use `minDuration` to ensure the splash stays visible long enough to read. |
| Splash blocks the owner window's title bar / resize | `InputBlockMode = Window` | Use `InputBlockMode = WindowContent` to keep drag / minimize / maximize working; or `Owner` to block only the owner element; or `None`. |
| `LoadingDecorator.OwnerLock="None"` doesn't block anything (intentional) | By design | If you want input blocking, use `Full` / `InputOnly` / `LoadingContent`. |
| `error CS0104: 'Application' is an ambiguous reference` | `DevExpress.Wpf.Core` transitively references `System.Windows.Forms`; `<ImplicitUsings>enable</ImplicitUsings>` on .NET 6+ creates the clash | Qualify `System.Windows.Application` in `App.xaml.cs`. |
| Apps using `DXSplashScreen` get a deprecation note | Legacy API | Migrate to `SplashScreenManager`. APIs are similar but not identical — see [comparison.md](references/comparison.md). |

## Constraints & Rules

CRITICAL — follow these rules in every interaction:

1. **Build verification**: After changes, run `dotnet build` and report errors before claiming success.
2. **Target framework**: Windows-only (`net{X}-windows`, `UseWPF=true`).
3. **NuGet**: `DevExpress.Wpf.Core` for all three. All DevExpress packages share one version.
4. **XAML namespace**: `dx:` (core).
5. **Match the indicator to the threading need**:
   - Main thread will freeze → `SplashScreenManager`
   - Region within an interactive UI → `LoadingDecorator` or `WaitIndicator`
6. **Use `DeferedVisibility` on `WaitIndicator`** — not standard `Visibility`. This avoids briefly showing the indicator for sub-100ms operations.
7. **Apply theme before showing a themed splash** — `CompatibilitySettings.UseLightweightThemes = true;` (when using LW) and `ApplicationThemeHelper.ApplicationThemeName = ...;` must run **before** `SplashScreenManager.CreateThemed().ShowOnStartup()`.
8. **Application ambiguity**: When generating `App.xaml.cs` on .NET 6+, qualify `System.Windows.Application`.
9. **Don't use the legacy `DXSplashScreen`** in new code — it's been superseded by `SplashScreenManager`.

## Using DevExpress Documentation MCP

- **Search**: `devexpress_docs_search(technology="WPF", query="SplashScreenManager LoadingDecorator WaitIndicator")`
- **Fetch**: `devexpress_docs_get_content(url="https://docs.devexpress.com/WPF/DevExpress.Xpf.Core.LoadingDecorator")`

Use MCP for: custom splash creation (`SplashScreenManager.Create` with custom window), `SplashScreenManagerService` MVVM details, custom `LoadingDecorator` content templates with animation.

---

## Next Steps

Start with **[Getting Started](references/getting-started.md)** for the NuGet package and first usage of each indicator. Then **[Comparison](references/comparison.md)** for the in-depth picker, threading details, and configuration of each control.

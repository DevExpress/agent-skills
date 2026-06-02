# Comparison — SplashScreenManager vs LoadingDecorator vs WaitIndicator

The three indicators look similar but solve different problems. The decisive factor is **threading**: `SplashScreenManager` runs on a separate UI thread (immune to main-thread freezes), while `LoadingDecorator` and `WaitIndicator` run on the main thread (and freeze with it). This page is the full picker plus configuration reference for each.

## When to Use This Reference

Use this when you need to:

- Pick between the three for a specific scenario
- Understand the threading implications
- Configure `SplashScreenManager` modes and `DXSplashScreenViewModel`
- Configure `LoadingDecorator` (`OwnerLock`, `SplashScreenTemplate`, `SplashScreenLocation`, fade, border)
- Configure `WaitIndicator` (`DeferedVisibility`, `ContentTemplate`)
- Wire any of them up in MVVM

## Side-by-Side Comparison

| Aspect | SplashScreenManager | LoadingDecorator | WaitIndicator |
|---|---|---|---|
| **Class** | `DevExpress.Xpf.Core.SplashScreenManager` | `DevExpress.Xpf.Core.LoadingDecorator` | `DevExpress.Xpf.Core.WaitIndicator` |
| **API style** | Code-only (no XAML element) | XAML container — wraps content | XAML element — placed inside layout |
| **Threading** | **Separate UI thread** — animations continue when main thread blocks | Main UI thread | Main UI thread |
| **Use case** | App startup; long modal ops | Slow-loading region (grid, chart, panel) | Inline "busy" indicator |
| **Visibility trigger** | `Show()` / `ShowOnStartup()` / `Close()` | Auto (during content loading) or `IsSplashScreenShown` (manual) | `DeferedVisibility` (bind to bool) |
| **Locks input** | Optional via `InputBlockMode` | Yes — configurable via `OwnerLock` | No (only renders on top; doesn't block) |
| **Built-in delay before showing** | Yes (`showDelay` parameter) | Yes (fade-in) | Yes (deferred) — short delay before showing |
| **Position** | Centered on screen / owner | Inside the wrapped region | Inside its parent container |
| **MVVM support** | `SplashScreenManagerService` | `IsSplashScreenShown` is bindable | `DeferedVisibility` is bindable |

## Decision Tree

```
Need to show a loading indicator. Where?

├── During application startup
│   └── SplashScreenManager.CreateThemed(...).ShowOnStartup()
│
├── During a long blocking operation (export, sync, heavy calc)
│   ├── Main thread WILL freeze → SplashScreenManager (separate thread)
│   └── Operation is async (doesn't freeze main) → LoadingDecorator or WaitIndicator
│
├── A specific region is slow to render (grid, chart, panel)
│   └── Wrap with <dx:LoadingDecorator>
│
└── An async button-triggered op; want a quick inline spinner
    └── <dx:WaitIndicator DeferedVisibility="{Binding IsBusy}"/>
```

## SplashScreenManager — In Depth

### Why It's Different

`SplashScreenManager` creates a **separate UI thread** for the splash window. This is critical: if your app freezes the main thread during startup (loading large data, initializing modules, blocking I/O), the main thread can't paint. But a splash on the main thread also can't paint and the spinner stops animating. `SplashScreenManager`'s separate thread keeps animating no matter what the main thread is doing.

`LoadingDecorator` and `WaitIndicator` both run on the main thread. If you block it, they freeze too.

### Four Predefined Styles

| Method | Look | Use for |
|---|---|---|
| `CreateThemed(DXSplashScreenViewModel?)` | Color scheme from the active theme | Standard branded startup splash |
| `CreateFluent(DXSplashScreenViewModel?)` | Fluent Design acrylic background | Modern Windows-style startup |
| `CreateWaitIndicator(DXSplashScreenViewModel?)` | Compact circular spinner + status text | Long modal operations (not startup) |
| `Create(...)` | Custom splash window class | Fully bespoke splashes |

### `DXSplashScreenViewModel`

Pass an instance with the splash you want:

```csharp
SplashScreenManager.CreateThemed(new DXSplashScreenViewModel {
    Logo = new Uri("pack://application:,,,/Images/Logo.png", UriKind.RelativeOrAbsolute),
    Title = "Contoso ERP",
    Subtitle = "Pro Edition",
    Status = "Loading...",
    IsIndeterminate = true,
}).ShowOnStartup();
```

| Property | Use |
|---|---|
| `Title` / `Subtitle` / `Copyright` | Caption text |
| `Logo` | URI to a logo image (pack URI or file://) |
| `Status` | Status text — visible in WaitIndicator style |
| `Progress` | Progress bar value 0–100 |
| `IsIndeterminate` | Whether the progress bar animates without a fixed value |
| `Tag` | Custom data for `Create(...)` custom splashes |

### Update at Runtime

`SplashScreenManager.ViewModel` exposes the running splash's view model. Update its properties from your main thread:

```csharp
var manager = SplashScreenManager.CreateThemed(
    new DXSplashScreenViewModel { IsIndeterminate = false });
manager.Show();

for (int i = 0; i <= 100; i++) {
    DoWork();
    manager.ViewModel.Progress = i;
    manager.ViewModel.Status = $"Processing {i}%...";
}

manager.Close();
```

The cross-thread marshalling is handled internally — set properties from any thread.

### `Show(...)` Parameters

`Show()` has rich parameters for fine control:

There are two overloads:

```csharp
// Basic
public void Show(
    DependencyObject owner = null,
    WindowStartupLocation startupLocation = WindowStartupLocation.CenterOwner,
    bool trackOwnerPosition = true,
    InputBlockMode inputBlock = InputBlockMode.None,
    int timeout = 700);

// With showDelay / minDuration (these come FIRST in this overload)
public void Show(
    int showDelay,
    int minDuration,
    DependencyObject owner = null,
    WindowStartupLocation startupLocation = WindowStartupLocation.CenterOwner,
    bool trackOwnerPosition = true,
    InputBlockMode inputBlock = InputBlockMode.None,
    int timeout = 700);
```

```csharp
// Show after 500 ms (operation faster than this — splash skipped); keep visible at least 700 ms
manager.Show(
    showDelay: 500,
    minDuration: 700,
    owner: this,
    startupLocation: WindowStartupLocation.CenterOwner,
    trackOwnerPosition: true,
    inputBlock: InputBlockMode.Owner);
```

| Parameter | Type | Use |
|---|---|---|
| `owner` | `DependencyObject` | UI element anchoring the splash |
| `startupLocation` | `WindowStartupLocation` | `CenterScreen` (default for startup) or `CenterOwner` |
| `trackOwnerPosition` | `bool` | Move splash with the owner when user drags it |
| `inputBlock` | `InputBlockMode` | `None`, `Owner`, `Window`, `WindowContent` |
| `timeout` | `int` (ms, default `700`) | Time the splash initialization is prioritized over the main app |
| `showDelay` | `int` (ms) | Don't show until operation runs this long. Prevents flashes for fast ops. |
| `minDuration` | `int` (ms) | Once shown, keep visible at least this long. Prevents flickers. |

### `ShowOnStartup`

A convenience wrapper:

```csharp
SplashScreenManager.CreateThemed().ShowOnStartup();

// Equivalent to:
SplashScreenManager.CreateThemed().Show(
    owner: null,
    startupLocation: WindowStartupLocation.CenterScreen,
    trackOwnerPosition: true,
    inputBlock: InputBlockMode.None,
    timeout: 700);
```

Auto-closes when the main window loads.

### Close

| Method | Effect |
|---|---|
| `manager.Close()` | Close this specific splash |
| `SplashScreenManager.CloseAll()` (static) | Close all active splashes |
| `SplashScreenManager.ActiveSplashScreens` | Enumerate active splashes for manual control |

### MVVM — `SplashScreenManagerService`

For full MVVM compliance, use the `SplashScreenManagerService`:

```xaml
<Window xmlns:dxmvvm="http://schemas.devexpress.com/winfx/2008/xaml/mvvm">
    <dxmvvm:Interaction.Behaviors>
        <dx:SplashScreenManagerService/>
    </dxmvvm:Interaction.Behaviors>
    ...
</Window>
```

```csharp
public class MainViewModel : ViewModelBase {
    ISplashScreenManagerService SplashService => GetService<ISplashScreenManagerService>();

    public async Task RunImport() {
        SplashService.Show();
        try { await ImportAsync(); }
        finally { SplashService.Close(); }
    }
}
```

See `xref:401692` in DxDocs for the full service API.

## LoadingDecorator — In Depth

### Anatomy

A content container that wraps any UI. Internally it uses a `WaitIndicator` for the loading visual:

```xaml
<dx:LoadingDecorator>
    <Grid>
        <!-- wrapped content -->
    </Grid>
</dx:LoadingDecorator>
```

By default, it shows the loading indicator while the content is loading and hides it when content is ready.

### `OwnerLock` — Which Part of the App Is Blocked

`LoadingDecorator.OwnerLock` (a `SplashScreenLock` enum, default `Full`) controls what's locked while the decorator is visible:

| Value | Effect |
|---|---|
| `Full` (default) | Blocks the entire window |
| `InputOnly` | Blocks input to the entire window. The title bar stays active — user can move/resize. |
| `LoadingContent` | Blocks only the decorator's content. The rest of the window stays interactive. |
| `None` | Nothing blocked — purely visual indicator |

```xaml
<dx:LoadingDecorator OwnerLock="LoadingContent">
    <dxg:GridControl ItemsSource="{Binding Data}"/>
</dx:LoadingDecorator>
```

**Use `LoadingContent`** when only one region is loading and the rest of the UI should remain usable (typical "reload this panel" scenario). Use `Full` for whole-screen blocking states. Use `None` when you want to show the indicator but trust users not to interact.

### Manual Toggle via `IsSplashScreenShown`

```xaml
<dx:LoadingDecorator IsSplashScreenShown="{Binding IsLoading}"
                     OwnerLock="LoadingContent">
    <Grid>...</Grid>
</dx:LoadingDecorator>
```

`IsSplashScreenShown` is bindable. Set the bound `IsLoading` to `true` to show; `false` to hide. Useful for explicit MVVM-driven control instead of relying on auto-detection of content loading.

### Customize the Indicator — `SplashScreenTemplate`

The decorator's indicator is, by default, a `WaitIndicator`. Override with `SplashScreenTemplate`:

```xaml
<dx:LoadingDecorator SplashScreenDataContext="Saving your changes...">
    <dx:LoadingDecorator.SplashScreenTemplate>
        <DataTemplate>
            <dx:WaitIndicator Content="{Binding}" DeferedVisibility="True">
                <dx:WaitIndicator.ContentTemplate>
                    <DataTemplate>
                        <StackPanel>
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

| Property | Use |
|---|---|
| `SplashScreenTemplate` | `DataTemplate` rendering the indicator |
| `SplashScreenDataContext` | DataContext passed to the template |
| `UseSplashScreen` | Set `false` to disable the indicator entirely (rare) |

### Position — `SplashScreenLocation`

```xaml
<dx:LoadingDecorator SplashScreenLocation="CenterContainer">
```

| `SplashScreenLocation` | Where |
|---|---|
| `CenterWindow` | Center of the parent window |
| `CenterScreen` | Center of the screen |
| `CenterContainer` | Center of the decorator's content area |

### Border Effect

Highlight the decorated content while loading:

```xaml
<dx:LoadingDecorator BorderEffect="Default" BorderEffectColor="Red">
    ...
</dx:LoadingDecorator>
```

| Property | Use |
|---|---|
| `BorderEffect` | `Default`, `None`, plus other modes |
| `BorderEffectColor` | Highlight color |

### Fade Animation

```xaml
<dx:LoadingDecorator UseFadeEffect="True"
                     FadeInDuration="0:0:0.3"
                     FadeOutDuration="0:0:0.2">
```

| Property | Use |
|---|---|
| `UseFadeEffect` | Toggle fade in/out (default `true`) |
| `FadeInDuration` / `FadeOutDuration` | Timing |

## WaitIndicator — In Depth

The simplest of the three. It's a popup panel you place inside a window's layout. Visibility is controlled via `DeferedVisibility`.

### `DeferedVisibility` vs `Visibility`

**Always use `DeferedVisibility`**, not standard WPF `Visibility`. `DeferedVisibility`:

- Waits a short interval (~300ms by default) before actually showing — so fast operations don't briefly flash the indicator.
- Is bindable to a `bool` (not `Visibility` enum).

```xaml
<dx:WaitIndicator DeferedVisibility="{Binding IsBusy}"
                  Content="Loading..."/>
```

```csharp
public bool IsBusy { get => GetValue<bool>(); set => SetValue(value); }
```

### Custom Content

Override the indicator layout with `ContentTemplate`:

```xaml
<dx:WaitIndicator DeferedVisibility="True" Content="Loading...">
    <dx:WaitIndicator.ContentTemplate>
        <DataTemplate>
            <StackPanel Orientation="Vertical">
                <TextBlock Text="Please wait" FontSize="20" FontWeight="Bold"/>
                <TextBlock Text="{Binding}"/>
            </StackPanel>
        </DataTemplate>
    </dx:WaitIndicator.ContentTemplate>
</dx:WaitIndicator>
```

`{Binding}` inside the template resolves to the `Content` value.

### Placement

`WaitIndicator` is a normal WPF element. To overlay it on existing content, put it as the **last child** of a panel (so it renders on top):

```xaml
<Grid>
    <ContentPresenter Content="{Binding MainContent}"/>
    <dx:WaitIndicator DeferedVisibility="{Binding IsBusy}" Content="Loading..."/>
</Grid>
```

`Grid` z-orders by child order; the indicator is last, so it overlays.

It doesn't block input by default — user can still click through. If you need blocking, wrap the area in a `LoadingDecorator` with `OwnerLock="LoadingContent"` instead.

### Customization Notes

- The default indicator look (animation + text label) follows the active **theme**. No theme set → default appearance.
- The text label sits next to the animation. To hide it, set `Content=""` (empty string).

## When `WaitIndicator` Freezes — Use SplashScreenManager Instead

The note in the `WaitIndicator` docs is explicit:

> The Wait Indicator works within the main application's UI thread. The UI freezes may affect the Wait Indicator animation.

If your operation blocks the main thread (heavy sync work, blocking I/O, slow database call without async), the `WaitIndicator` will freeze with it. Same for `LoadingDecorator` (which uses `WaitIndicator` internally).

For freeze-immune indicators, switch to **`SplashScreenManager`**. It runs on a separate UI thread and keeps animating regardless of main-thread state.

The simplest swap:

```csharp
// Was: WaitIndicator visible during sync work
void OnButtonClick(object sender, RoutedEventArgs e) {
    IsBusy = true;
    SyncBlockingOperation();   // Freezes UI → WaitIndicator stops too
    IsBusy = false;
}

// Better: SplashScreenManager (splash runs on a separate UI thread)
void OnButtonClick(object sender, RoutedEventArgs e) {
    var manager = SplashScreenManager.CreateWaitIndicator(
        new DXSplashScreenViewModel { Status = "Working..." });
    manager.Show(this, WindowStartupLocation.CenterOwner, true, InputBlockMode.Owner);

    try { SyncBlockingOperation(); }   // Still freezes UI, but splash keeps animating
    finally { manager.Close(); }
}
```

Or — best — refactor to async so the main thread doesn't block. Then `WaitIndicator` works fine.

## Common Patterns

### Pattern 1: Startup Splash with Progress Updates

```csharp
// App.xaml.cs
public App() {
    CompatibilitySettings.UseLightweightThemes = true;
    ApplicationThemeHelper.ApplicationThemeName = LightweightTheme.Win11Light.Name;

    var splash = SplashScreenManager.CreateThemed(new DXSplashScreenViewModel {
        Title = "MyApp",
        Status = "Initializing...",
        IsIndeterminate = false,
    });
    splash.Show();

    InitializeDataAccess();
    splash.ViewModel.Progress = 30;
    splash.ViewModel.Status = "Loading user preferences...";

    InitializeUserSettings();
    splash.ViewModel.Progress = 70;
    splash.ViewModel.Status = "Starting up...";

    // Main window opens normally; splash auto-closes when it loads
    // (alternatively call splash.Close() explicitly)
}
```

### Pattern 2: LoadingDecorator over a Grid with Refresh Button

```xaml
<DockPanel>
    <Button DockPanel.Dock="Top"
            Content="Refresh"
            Command="{Binding RefreshCommand}"/>

    <dx:LoadingDecorator IsSplashScreenShown="{Binding IsRefreshing}"
                         OwnerLock="LoadingContent">
        <dxg:GridControl ItemsSource="{Binding Records}">
            ...
        </dxg:GridControl>
    </dx:LoadingDecorator>
</DockPanel>
```

```csharp
public class MainViewModel : ViewModelBase {
    public bool IsRefreshing { get => GetValue<bool>(); set => SetValue(value); }

    public ICommand RefreshCommand => new AsyncCommand(async () => {
        IsRefreshing = true;
        try { Records = await _api.FetchAsync(); }
        finally { IsRefreshing = false; }
    });
}
```

The refresh button stays clickable; only the grid area is blocked + shows the indicator.

### Pattern 3: WaitIndicator for Quick Save Operations

```xaml
<Grid>
    <Editor x:Name="editor"/>
    <dx:WaitIndicator DeferedVisibility="{Binding IsSaving}" Content="Saving..."/>
</Grid>
```

```csharp
public bool IsSaving { get => GetValue<bool>(); set => SetValue(value); }

public async Task SaveAsync() {
    IsSaving = true;
    try { await _store.SaveAsync(); }
    finally { IsSaving = false; }
}
```

Since `DeferedVisibility` delays before showing, sub-300ms saves don't flash the indicator.

### Pattern 4: Custom LoadingDecorator with Status Text

```xaml
<dx:LoadingDecorator SplashScreenDataContext="{Binding LoadingStatus}"
                     IsSplashScreenShown="{Binding IsLoading}"
                     OwnerLock="LoadingContent">
    <dx:LoadingDecorator.SplashScreenTemplate>
        <DataTemplate>
            <dx:WaitIndicator Content="{Binding}" DeferedVisibility="True"/>
        </DataTemplate>
    </dx:LoadingDecorator.SplashScreenTemplate>

    <Grid>...</Grid>
</dx:LoadingDecorator>
```

```csharp
public string LoadingStatus { get => GetValue<string>(); set => SetValue(value); }

public async Task LoadAsync() {
    IsLoading = true;
    LoadingStatus = "Fetching data...";
    var data = await FetchAsync();
    LoadingStatus = "Processing...";
    Process(data);
    IsLoading = false;
}
```

Status text updates while the indicator is visible.

## Picking Summary

| If you need... | Use |
|---|---|
| Startup splash with logo + progress | `SplashScreenManager.CreateThemed(...).ShowOnStartup()` |
| Splash that survives a blocking main-thread operation | `SplashScreenManager.CreateWaitIndicator(...).Show(...)` |
| To wrap a region (grid/chart/panel) so an indicator shows while loading | `<dx:LoadingDecorator>...</dx:LoadingDecorator>` |
| To block input only on the loading region, not the whole window | `LoadingDecorator OwnerLock="LoadingContent"` |
| A simple inline "busy" spinner controlled by an `IsBusy` boolean | `<dx:WaitIndicator DeferedVisibility="{Binding IsBusy}"/>` |
| Custom indicator look | `LoadingDecorator.SplashScreenTemplate` or `WaitIndicator.ContentTemplate` |
| Fluent Design / acrylic splash | `SplashScreenManager.CreateFluent(...)` |
| Fully custom splash window | `SplashScreenManager.Create(typeof(MySplash))` |

## Common Issues

- **Indicator freezes during operation** — using `WaitIndicator` or `LoadingDecorator` while blocking the main thread. Refactor to async, or switch to `SplashScreenManager`.
- **`Visibility="Visible"` on `WaitIndicator` shows instantly** — should use `DeferedVisibility` instead, which delays briefly.
- **Splash never closes** — forgot to call `manager.Close()` (or to set `IsLoading = false` in the `finally` block). Always pair show with close in `try`/`finally`.
- **`SplashScreenManager.CreateThemed` shows the wrong theme** — `ApplicationThemeHelper.ApplicationThemeName` was set **after** the splash was created. Set the theme first.
- **`OwnerLock="None"` looks broken** — that's intentional: `None` means "don't block anything", so users can click through the decorator. Use `Full`, `InputOnly`, or `LoadingContent` if you want blocking.
- **Splash position drifts when user drags the main window** — `trackOwnerPosition: false`. Set it to `true` (default in `ShowOnStartup`).
- **Splash flashes for sub-second operations** — supply `showDelay: 500` (milliseconds) to `Show()` so quick operations don't trigger a splash.
- **Trying to use legacy `DXSplashScreen` in new code** — migrate to `SplashScreenManager`. The API is similar but newer and supported going forward.

## Source Material

- `articles/controls-and-libraries/windows-and-utility-controls/splash-screen-manager.md` (`xref:401685`)
- `articles/controls-and-libraries/windows-and-utility-controls/wait-indicator.md` (`xref:114373`)
- `articles/mvvm-framework/services/predefined-set/splashscreenmanagerservice.md` (`xref:401692`)
- DxDocs API: `DevExpress.Xpf.Core.LoadingDecorator` — `OwnerLock`, `IsSplashScreenShown`, `UseSplashScreen`, `SplashScreenTemplate`, `SplashScreenDataContext`, `SplashScreenLocation`, `BorderEffect`, `UseFadeEffect`
- DxDocs API: `DevExpress.Xpf.Core.SplashScreenLock` enum — `Full`, `InputOnly`, `LoadingContent`, `None`

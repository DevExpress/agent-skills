# Getting Started — Ribbon and Bars

This guide walks through adding the DevExpress Ribbon control (and/or toolbars) to a .NET (6/7/8+) WPF project. The critical step is converting the main window from `System.Windows.Window` to `DevExpress.Xpf.Core.ThemedWindow` — without this, Ribbon integration won't render correctly (you'll see two title bars, broken hit-testing in the header, and no support for `WindowKind="Ribbon"`).

## System Requirements

- .NET 6.0 / 7.0 / 8.0+ targeting Windows (or .NET Framework 4.6.2+)
- Visual Studio 2022+ or JetBrains Rider
- A valid DevExpress license

## Step 1: Install NuGet Packages

```bash
dotnet add package DevExpress.Wpf.Ribbon
```

`DevExpress.Wpf.Ribbon` transitively brings `DevExpress.Wpf.Bars` and `DevExpress.Wpf.Core` — you don't install those separately for Ribbon UIs.

If you're building a **bars-only UI** (no Ribbon, just `MainMenuControl` / `ToolBarControl` / `StatusBarControl`), install the Bars package instead:

```bash
dotnet add package DevExpress.Wpf.Bars
```

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

## Step 3: Convert the Main Window to `ThemedWindow` (REQUIRED for Ribbon)

A regular `<Window>` won't host a `RibbonControl` properly. Open `MainWindow.xaml` and change `Window` → `dx:ThemedWindow`. Add the core namespace and set `WindowKind="Ribbon"`:

### Before

```xaml
<Window x:Class="MyApp.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        Title="MyApp" Width="900" Height="600">
    ...
</Window>
```

### After

```xaml
<dx:ThemedWindow x:Class="MyApp.MainWindow"
                 xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                 xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                 xmlns:dx="http://schemas.devexpress.com/winfx/2008/xaml/core"
                 Title="MyApp" Width="900" Height="600"
                 WindowKind="Ribbon">
    ...
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

### `WindowKind` Values

| Value | Use |
|---|---|
| `Ribbon` | The window hosts a `RibbonControl`. Ribbon tab headers and the QAT integrate into the title bar. |
| `Dialog` | A modal-dialog look. |
| `Default` | Plain ThemedWindow. |

For a bars-only UI (no Ribbon), `ThemedWindow` is still helpful (themed title bar, `HeaderItems` / `ToolbarItems` integration) but not strictly required. A plain `Window` works fine for `MainMenuControl` / `ToolBarControl` placed inside the client area.

## Step 4: Add the Ribbon

In `MainWindow.xaml`:

```xaml
<dx:ThemedWindow x:Class="MyApp.MainWindow"
                 xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                 xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                 xmlns:dx="http://schemas.devexpress.com/winfx/2008/xaml/core"
                 xmlns:dxr="http://schemas.devexpress.com/winfx/2008/xaml/ribbon"
                 xmlns:dxb="http://schemas.devexpress.com/winfx/2008/xaml/bars"
                 Title="MyApp" Width="900" Height="600"
                 WindowKind="Ribbon">
    <DockPanel>
        <dxr:RibbonControl DockPanel.Dock="Top" RibbonStyle="Office2019">
            <dxr:RibbonDefaultPageCategory>
                <dxr:RibbonPage Caption="Home">
                    <dxr:RibbonPageGroup Caption="Clipboard">
                        <dxb:BarButtonItem Content="Paste"
                                           Glyph="{dx:DXImage Image=Paste_16x16.png}"
                                           LargeGlyph="{dx:DXImage Image=Paste_32x32.png}"
                                           Command="{Binding PasteCommand}"/>
                    </dxr:RibbonPageGroup>
                </dxr:RibbonPage>
            </dxr:RibbonDefaultPageCategory>
        </dxr:RibbonControl>

        <Grid/>
    </DockPanel>
</dx:ThemedWindow>
```

## Step 5: Pick a Ribbon Style

| `RibbonStyle` | Look |
|---|---|
| `Office2019` | Microsoft Office 2019 — most modern, default recommendation |
| `Office2010` | Office 2010 — use `BackstageViewControl` as the application menu |
| `Office2007` | Office 2007 — use `ApplicationMenu` (older modal menu) |
| `OfficeSlim` | Single-line "Office Universal" inspired ribbon |
| `TabletOffice` | Microsoft Office for iPad inspired |

`Office2019` is the safest default for new applications.

## Step 6: Bars-Only Setup (No Ribbon)

For a classic toolbar + menu + status bar layout (no Ribbon), `ThemedWindow` is optional and `WindowKind` doesn't apply:

```xaml
<Window x:Class="MyApp.MainWindow"
        xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:dx="http://schemas.devexpress.com/winfx/2008/xaml/core"
        xmlns:dxb="http://schemas.devexpress.com/winfx/2008/xaml/bars"
        Title="MyApp" Width="800" Height="500">
    <DockPanel>
        <dxb:MainMenuControl DockPanel.Dock="Top">
            <dxb:BarSubItem Content="File">
                <dxb:BarButtonItem Content="Open" Command="{Binding OpenCommand}"/>
                <dxb:BarButtonItem Content="Exit" Command="{Binding ExitCommand}"/>
            </dxb:BarSubItem>
        </dxb:MainMenuControl>

        <dxb:ToolBarControl DockPanel.Dock="Top">
            <dxb:BarButtonItem Content="Cut"
                               Glyph="{dx:DXImage Cut_16x16.png}"/>
            <dxb:BarButtonItem Content="Copy"
                               Glyph="{dx:DXImage Copy_16x16.png}"/>
            <dxb:BarButtonItem Content="Paste"
                               Glyph="{dx:DXImage Paste_16x16.png}"/>
        </dxb:ToolBarControl>

        <dxb:StatusBarControl DockPanel.Dock="Bottom">
            <dxb:BarStaticItem Content="Ready"/>
        </dxb:StatusBarControl>

        <Grid/>
    </DockPanel>
</Window>
```

You can still convert to `ThemedWindow` to get a themed title bar — the bars don't require it.

## Step 7: Build and Run

```bash
dotnet build
dotnet run
```

## What to Learn Next

- [Items and Links](items-and-links.md) — how to add `BarButtonItem`, `BarCheckItem`, etc., the difference between items and links, MVVM
- [Ribbon Structure](ribbon-structure.md) — categories, pages, groups, QAT, application menu, status bar
- [Bars and Layout](bars-and-layout.md) — standalone bar controls vs `BarManager`, `BarContainerControl`
- [Appearance Customization](appearance-customization.md) — what you can / cannot customize via properties and triggers
- [Merging](merging.md) — MDI bar/ribbon merging

## Source Material

- `articles/controls-and-libraries/ribbon-bars-and-menu/ribbon/overview/getting-started.md` (`xref:11443`)
- `articles/controls-and-libraries/ribbon-bars-and-menu/ribbon/ribbon-structure/ribbon-control.md` (`xref:7954`)
- `articles/controls-and-libraries/ribbon-bars-and-menu/ribbon/ribbon-structure/dxribbonwindow.md` (`xref:7980`)

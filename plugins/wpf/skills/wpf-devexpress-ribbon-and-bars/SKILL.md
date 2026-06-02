---
name: wpf-devexpress-ribbon-and-bars
description: Build WPF applications with the DevExpress Ribbon control, toolbars, and menus. Use when adding RibbonControl, ToolBarControl, MainMenuControl, StatusBarControl, BarManager, or RibbonStatusBarControl to a WPF window; defining bar items (BarButtonItem, BarCheckItem, BarSplitButtonItem, BarSubItem, BarEditItem, BarStaticItem, BarItemSeparator) and bar item links; building Ribbon structure with RibbonPageCategory, RibbonPage, RibbonPageGroup; configuring Quick Access Toolbar, Application Menu, BackstageView, Ribbon Status Bar; using glyphs, ItemDisplayMode, RibbonStyle; merging child window bars/ribbons into the parent (MDI merging via DockLayoutManager). Also use when someone mentions "ThemedWindow", "WindowKind=Ribbon", "DevExpress.Xpf.Ribbon", "DevExpress.Xpf.Bars", "dxr:", "dxb:", "BarItem", or asks about Office-style UIs in WPF. Covers .NET (6/7/8+) and .NET Framework 4.6.2+.
compatibility: Requires .NET 6+ or .NET Framework 4.6.2+ targeting Windows (net8.0-windows). DevExpress NuGet packages are published on nuget.org and via the local feed registered by the Unified Component Installer. The main window must be a ThemedWindow (not a regular Window) for full Ribbon integration. A valid DevExpress license is required.
metadata:
  author: DevExpress
  version: "26.1"
  source-commit: b16066c633b85ee391d1e6188ffc4cd815ee5e8f
---

# DevExpress WPF Ribbon and Bars

DevExpress.Xpf.Ribbon and DevExpress.Xpf.Bars provide an Office-style command surface for WPF: a tabbed `RibbonControl` with categories/pages/groups, standalone bar controls (`MainMenuControl`, `ToolBarControl`, `StatusBarControl`), the legacy `BarManager` aggregator for multi-bar layouts, popup menus, status bars, and a rich set of bar items (`BarButtonItem`, `BarCheckItem`, `BarSplitButtonItem`, `BarSubItem`, `BarEditItem`, etc.) that work uniformly across Ribbon, bars, and menus.

> **`ThemedWindow` is required**, not a regular `System.Windows.Window`. The Ribbon control needs `ThemedWindow` to render correctly into the title bar, support `WindowKind="Ribbon"` integration, and provide MDI merging hooks. Convert plain `Window` → `ThemedWindow` before adding Ribbon UI.

## When to Use This Skill

Use this skill when you need to:

- Build a Microsoft Office–style Ribbon UI in a WPF window
- Add toolbars, main menus, or status bars (modern way: `ToolBarControl` / `MainMenuControl` / `StatusBarControl`; legacy: `BarManager` with `Bar` objects)
- Define bar items (buttons, check buttons, sub-menus, editors, separators, galleries)
- Wire bar item actions to MVVM commands or code-behind events
- Populate Ribbon pages / groups / bars from a ViewModel collection
- Configure the Quick Access Toolbar, Application Menu, BackstageView, or Ribbon Status Bar
- Set glyphs and switch between large/small icon display modes
- Customize bar/ribbon element appearance via standard WPF properties and triggers
- Merge child-window bars/ribbons into the parent in an MDI layout

## Prerequisites & Installation

### NuGet Packages

| Package | Purpose |
|---------|---------|
| `DevExpress.Wpf.Core` | `ThemedWindow`, themes, shared infrastructure |
| `DevExpress.Wpf.Ribbon` | `RibbonControl`, `RibbonPageCategory`, `RibbonPage`, `RibbonPageGroup`, `BackstageViewControl`, `RibbonStatusBarControl`. Includes `DevExpress.Xpf.Bars`. |
| `DevExpress.Wpf.Docking` | Only if you need MDI merging via `DockLayoutManager` |

`DevExpress.Wpf.Ribbon` brings `DevExpress.Wpf.Bars` transitively — you don't install Bars separately.

### .NET (6/7/8+)

```bash
dotnet add package DevExpress.Wpf.Ribbon
```

Add to `.csproj`:

```xml
<PropertyGroup>
  <TargetFramework>net8.0-windows</TargetFramework>
  <UseWPF>true</UseWPF>
</PropertyGroup>
```

### .NET Framework (4.6.2+)

Same NuGet packages, or reference the installed assemblies directly (`DevExpress.Xpf.Ribbon.v<XX.X>.dll`, `DevExpress.Xpf.Bars.v<XX.X>.dll`).

All DevExpress packages must share a version. A valid DevExpress license is required.

## Before You Start — Ask the Developer

1. **Target framework**: .NET 8+, .NET 6/7, or .NET Framework 4.x?
2. **Ribbon or bars**: Office-style Ribbon, classic toolbar/menu/status bar layout, or both? **Don't mix `RibbonControl` and `BarManager` in the same window — pick one paradigm.**
3. **Window**: Is the main window already a `ThemedWindow`? If not, it must be converted before adding Ribbon UI.
4. **Bar layout**: For non-ribbon UIs, prefer standalone `ToolBarControl` / `MainMenuControl` / `StatusBarControl` over the older `BarManager`. Use `BarManager` only when you need centralized item sharing across many bars.
5. **MVVM**: Define items declaratively in XAML, or generate from a ViewModel collection? See [items-and-links.md § MVVM](references/items-and-links.md).
6. **MDI merging**: Are there child documents/panels that contribute their own bars/ribbons to merge into the parent?

## Component Overview

### XAML Namespaces

```xml
xmlns:dxr="http://schemas.devexpress.com/winfx/2008/xaml/ribbon"
xmlns:dxb="http://schemas.devexpress.com/winfx/2008/xaml/bars"
xmlns:dx="http://schemas.devexpress.com/winfx/2008/xaml/core"
```

| Prefix | Use for |
|---|---|
| `dxr:` | `RibbonControl`, `RibbonPageCategory`, `RibbonPage`, `RibbonPageGroup`, `RibbonStatusBarControl`, `BackstageViewControl`, `BarButtonGroup`, `RibbonGalleryBarItem`, `ApplicationMenu` |
| `dxb:` | All `BarItem` types and links, `ToolBarControl`, `MainMenuControl`, `StatusBarControl`, `BarContainerControl`, `BarManager`, `PopupMenu` |
| `dx:` | `ThemedWindow`, themes, `DXMessageBoxService` |

### Items vs Links (Critical Concept)

- **`BarItem`** — the item definition (button, check, split, sub-item, edit, static, etc.). Owns content, glyph, command/click handler.
- **`BarItemLink`** — a *reference* to a `BarItem`. Lets the same `BarItem` appear in multiple bars / menus / ribbon groups simultaneously.

In simple cases (item defined directly inside its bar), the link is created implicitly. In `BarManager` layouts (items defined in `BarManager.Items`, referenced from bars), you typically write `BarButtonItemLink BarItemName="bPaste"` explicitly. See [items-and-links.md](references/items-and-links.md).

## Documentation & Navigation Guide

### Getting Started
Refer to [references/getting-started.md](references/getting-started.md)

When you need to:
- Set up a new WPF project with Ribbon / bars
- Add required NuGet packages
- Convert a plain `Window` to `ThemedWindow` (required for Ribbon)
- Place the first `RibbonControl` and configure `WindowKind="Ribbon"`
- Pick a `RibbonStyle` (Office2007 / Office2010 / Office2019 / OfficeSlim / TabletOffice)

### Items and Item Links
Refer to [references/items-and-links.md](references/items-and-links.md)

When you need to:
- Add `BarButtonItem`, `BarCheckItem`, `BarSplitButtonItem`, `BarSubItem`, `BarEditItem`, separators
- Understand the difference between **item** and **link** (and when to use each)
- Set glyphs (`Glyph` / `LargeGlyph` / `MediumGlyph`), display modes (`Default`, `Content`, `ContentAndGlyph`), alignment
- Wire items to MVVM commands (`Command` / `CommandParameter`) or `ItemClick` events
- Generate items dynamically from a ViewModel collection (`ItemLinksSource` + templates)

### Appearance Customization
Refer to [references/appearance-customization.md](references/appearance-customization.md)

When you need to:
- Customize background / foreground / borders / padding via standard WPF properties
- Use `BarItem.Triggers` for conditional appearance (state-based)
- Switch Ribbon paint styles (`Office2019`, `OfficeSlim`, `TabletOffice`, etc.)
- Understand **what cannot** be customized via properties / triggers (template-only territory)

### Merging
Refer to [references/merging.md](references/merging.md)

When you need to:
- Merge a child window's bars / ribbons into the parent window automatically (MDI mode)
- Trigger merge / unmerge manually (`Bar.Merge` / `Bar.UnMerge`, `RibbonControl.UnMerge`)
- Tune merge behavior via `MergeType` and `MergeOrder` on items, pages, groups
- Disable merging for a specific panel or layout
- Merge a `ThemedWindow`'s toolbar items with a child panel's bars

### Bar Varieties and Layout
Refer to [references/bars-and-layout.md](references/bars-and-layout.md)

When you need to:
- Decide between standalone `ToolBarControl` / `MainMenuControl` / `StatusBarControl` (**recommended**) vs `BarManager` + `Bar` (legacy)
- Place a bar at the top / left / right / bottom of a window using `BarContainerControl`
- Allow bars to float at runtime
- Use the `BarManager` for centralized multi-bar layouts

### Ribbon Structure
Refer to [references/ribbon-structure.md](references/ribbon-structure.md)

When you need to:
- Understand the nesting: `RibbonControl` → `RibbonPageCategory` → `RibbonPage` → `RibbonPageGroup` → `BarItem`
- Add the default page category (always visible) vs custom (contextual) categories
- Configure the Quick Access Toolbar (`ToolbarItems`) and Page Header Region (`PageHeaderItems`)
- Add an `ApplicationMenu` (Office 2007 style) or `BackstageViewControl` (Office 2010+ style)
- Add a `RibbonStatusBarControl` at the bottom of the window
- Show contextual pages on demand (visible only when relevant)

## Quick Start Example

A `ThemedWindow` with `RibbonControl`, Quick Access Toolbar, Application Menu, two pages, and bar items wired to MVVM commands:

```xaml
<dx:ThemedWindow x:Class="MyApp.MainWindow"
                 xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                 xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
                 xmlns:dx="http://schemas.devexpress.com/winfx/2008/xaml/core"
                 xmlns:dxr="http://schemas.devexpress.com/winfx/2008/xaml/ribbon"
                 xmlns:dxb="http://schemas.devexpress.com/winfx/2008/xaml/bars"
                 xmlns:vm="clr-namespace:MyApp.ViewModels"
                 Title="My App" Width="900" Height="600"
                 WindowKind="Ribbon">
    <dx:ThemedWindow.DataContext>
        <vm:MainViewModel/>
    </dx:ThemedWindow.DataContext>
    <DockPanel>
        <dxr:RibbonControl DockPanel.Dock="Top" RibbonStyle="Office2019">

            <!-- Quick Access Toolbar -->
            <dxr:RibbonControl.ToolbarItems>
                <dxb:BarButtonItem Content="Save"
                                   Glyph="{dx:DXImage SvgImages/Save/Save.svg}"
                                   Command="{Binding SaveCommand}"/>
                <dxb:BarButtonItem Content="Undo"
                                   Glyph="{dx:DXImage SvgImages/Edit/Undo.svg}"
                                   Command="{Binding UndoCommand}"/>
            </dxr:RibbonControl.ToolbarItems>

            <!-- Application Menu -->
            <dxr:RibbonControl.ApplicationMenu>
                <dxr:ApplicationMenu>
                    <dxb:BarButtonItem Content="New"
                                       Command="{Binding NewCommand}"/>
                    <dxb:BarButtonItem Content="Open"
                                       Command="{Binding OpenCommand}"/>
                    <dxb:BarButtonItem Content="Exit"
                                       Command="{Binding ExitCommand}"/>
                </dxr:ApplicationMenu>
            </dxr:RibbonControl.ApplicationMenu>

            <!-- Default page category (always visible) -->
            <dxr:RibbonDefaultPageCategory>
                <dxr:RibbonPage Caption="Home">
                    <dxr:RibbonPageGroup Caption="Clipboard">
                        <dxb:BarButtonItem Content="Cut"
                                           Glyph="{dx:DXImage Image=Cut_16x16.png}"
                                           LargeGlyph="{dx:DXImage Image=Cut_32x32.png}"
                                           Command="{Binding CutCommand}"/>
                        <dxb:BarButtonItem Content="Copy"
                                           Glyph="{dx:DXImage Image=Copy_16x16.png}"
                                           LargeGlyph="{dx:DXImage Image=Copy_32x32.png}"
                                           Command="{Binding CopyCommand}"/>
                        <dxb:BarButtonItem Content="Paste"
                                           Glyph="{dx:DXImage Image=Paste_16x16.png}"
                                           LargeGlyph="{dx:DXImage Image=Paste_32x32.png}"
                                           Command="{Binding PasteCommand}"/>
                    </dxr:RibbonPageGroup>
                </dxr:RibbonPage>
                <dxr:RibbonPage Caption="View">
                    <dxr:RibbonPageGroup Caption="Layout">
                        <dxb:BarCheckItem Content="Grid Lines"
                                          IsChecked="{Binding ShowGridLines, Mode=TwoWay}"/>
                    </dxr:RibbonPageGroup>
                </dxr:RibbonPage>
            </dxr:RibbonDefaultPageCategory>
        </dxr:RibbonControl>

        <!-- Ribbon Status Bar at the bottom -->
        <dxr:RibbonStatusBarControl DockPanel.Dock="Bottom">
            <dxr:RibbonStatusBarControl.LeftItems>
                <dxb:BarStaticItem Content="{Binding StatusText}"/>
            </dxr:RibbonStatusBarControl.LeftItems>
        </dxr:RibbonStatusBarControl>

        <!-- Main content -->
        <Grid/>
    </DockPanel>
</dx:ThemedWindow>
```

## Key Properties & API Surface

### `RibbonControl`

| Property | Purpose |
|---|---|
| `RibbonStyle` | `Office2007`, `Office2010`, `Office2019`, `OfficeSlim`, `TabletOffice` |
| `ApplicationMenu` | The application menu (Office 2007 style) or `BackstageViewControl` (Office 2010+) |
| `ToolbarItems` | Quick Access Toolbar items collection |
| `PageHeaderItems` | Items shown at the right edge of the title bar |
| `Categories` / `CategoriesSource` | Page categories (declarative or ViewModel-bound) |
| `RibbonHeaderVisibility` / `RibbonTitleBarVisibility` | Show/hide the header / title bar areas |
| `AllowSimplifiedRibbon` / `IsSimplified` | Office-2019-style single-line Ribbon mode |
| `MDIMergeStyle` | Block this Ribbon from being merged (`Never`) |

### `BarItem` (base class)

| Property | Purpose |
|---|---|
| `Content` | Caption text (or any UI element) |
| `Glyph` / `LargeGlyph` / `MediumGlyph` | 16x16 / 32x32 / 20x20 icons |
| `RibbonStyle` | Per-item Ribbon display: `SmallWithText`, `SmallWithoutText`, `Large`, `Default` |
| `BarItemDisplayMode` | `Default`, `Content` (caption only), `ContentAndGlyph` |
| `Command` / `CommandParameter` | MVVM command |
| `ItemClick` | Click event (alternative to `Command`) |
| `KeyGesture` | Keyboard shortcut (e.g., `Ctrl+V`) |
| `IsVisible`, `IsEnabled` | Standard visibility / enabled state |

### `RibbonPageGroup`, `RibbonPage`, `RibbonPageCategory`

| Property | Purpose |
|---|---|
| `Caption` | Header text |
| `Glyph` | Icon (RibbonPageGroup only) |
| `IsVisible` | Toggle visibility (e.g., contextual pages/categories) |
| `MergeType` / `MergeOrder` | MDI merging behavior — see [merging.md](references/merging.md) |
| `Color` | Background tint (`RibbonPageCategoryBase.Color`) |

## Common Patterns

### Pattern 1: Items Declared Directly Inside a Bar/Group

```xaml
<dxr:RibbonPageGroup Caption="Clipboard">
    <dxb:BarButtonItem Content="Paste" Glyph="{dx:DXImage Paste_16x16.png}"
                       Command="{Binding PasteCommand}"/>
</dxr:RibbonPageGroup>
```

The link is created implicitly. Use this for most cases.

### Pattern 2: Shared Items via Item Links (BarManager Style)

```xaml
<dxb:BarManager>
    <dxb:BarManager.Items>
        <dxb:BarButtonItem x:Name="bPaste" Content="Paste"
                           Glyph="{dx:DXImage Paste_16x16.png}"
                           KeyGesture="Ctrl+V"
                           Command="{Binding PasteCommand}"/>
    </dxb:BarManager.Items>

    <!-- Same item referenced by both toolbar and menu -->
    <dxb:ToolBarControl>
        <dxb:BarButtonItemLink BarItemName="bPaste"/>
    </dxb:ToolBarControl>
</dxb:BarManager>
```

Use explicit links when an item must appear in multiple places.

### Pattern 3: MVVM — Items from ViewModel Collection

```xaml
<dxr:RibbonPageGroup Caption="Actions"
                     ItemLinksSource="{Binding Actions}">
    <dxr:RibbonPageGroup.ItemTemplate>
        <DataTemplate>
            <ContentControl>
                <dxb:BarButtonItem Content="{Binding Caption}"
                                   Glyph="{Binding Glyph}"
                                   Command="{Binding ExecuteCommand}"/>
            </ContentControl>
        </DataTemplate>
    </dxr:RibbonPageGroup.ItemTemplate>
</dxr:RibbonPageGroup>
```

`ContentControl` is mandatory as the template root.

### Pattern 4: Modern Standalone Bar Controls (Preferred over BarManager)

```xaml
<DockPanel>
    <dxb:MainMenuControl DockPanel.Dock="Top">
        <dxb:BarSubItem Content="File">
            <dxb:BarButtonItem Content="Open" Command="{Binding OpenCommand}"/>
            <dxb:BarButtonItem Content="Save" Command="{Binding SaveCommand}"/>
        </dxb:BarSubItem>
    </dxb:MainMenuControl>

    <dxb:ToolBarControl DockPanel.Dock="Top">
        <dxb:BarButtonItem Content="Cut"   Glyph="{dx:DXImage Cut_16x16.png}"/>
        <dxb:BarButtonItem Content="Copy"  Glyph="{dx:DXImage Copy_16x16.png}"/>
        <dxb:BarButtonItem Content="Paste" Glyph="{dx:DXImage Paste_16x16.png}"/>
    </dxb:ToolBarControl>

    <dxb:StatusBarControl DockPanel.Dock="Bottom">
        <dxb:BarStaticItem Content="{Binding StatusText}"/>
    </dxb:StatusBarControl>

    <Grid/>
</DockPanel>
```

## Troubleshooting

| Symptom | Cause | Solution |
|---|---|---|
| Ribbon doesn't integrate with title bar; window has two title bars | Window is a regular `System.Windows.Window` | Convert to `dx:ThemedWindow` and set `WindowKind="Ribbon"`. |
| `dxr:` / `dxb:` prefix unresolved | Missing namespace declaration or missing NuGet package | Add `xmlns:dxr=".../winfx/2008/xaml/ribbon"`, `xmlns:dxb=".../winfx/2008/xaml/bars"`. Install `DevExpress.Wpf.Ribbon`. |
| `error CS0104: 'Application' is an ambiguous reference` | `<ImplicitUsings>enable</ImplicitUsings>` + DevExpress.Wpf.Core transitively references `System.Windows.Forms` | Qualify `System.Windows.Application` in `App.xaml.cs`. |
| Items appear but glyphs don't | Glyph path resolution failed | Use `{dx:DXImage Image=...png}` (raster) or `{dx:DXImage SvgImages/...svg}` (vector), or set `Glyph` to a `Uri` to a packed resource. |
| Ribbon items move from large to small icons inconsistently | Items haven't been given both `Glyph` (small) and `LargeGlyph` (large) | Supply both; the Ribbon auto-swaps based on width and `RibbonStyle`. |
| `BarItemLink` shows nothing | `BarItemName` doesn't match any `BarItem` `x:Name` in scope; or item is outside the `BarManager` that owns the link | Verify `x:Name`s and that both item and link are within the same `BarManager` or accessible scope. |
| MDI merge doesn't happen | Child panel isn't inside a `DockLayoutManager` with `DocumentGroup` / `DocumentPanel`; or `MDIMergeStyle` is `Never` | Wrap docs in `DocumentGroup`. Use default `MDIMergeStyle.WhenChildActivated` or `Always`. |
| Trying to style a bar item's pressed state via `Style.Triggers` doesn't apply | Standard WPF triggers on `BarItem` don't customize the rendered `BarItemLink` | Use `BarItem.Triggers` (the DevExpress collection) — it evaluates per link. See appearance-customization.md. |
| Mixing `RibbonControl` and `BarManager` in one window causes layout glitches | Two paradigms in one container | Pick one: Ribbon UI, or BarManager + bars. Don't combine. |

## Constraints & Rules

CRITICAL — follow these rules in every interaction:

1. **Window must be `ThemedWindow`** for Ribbon UI. Set `WindowKind="Ribbon"` for full title-bar integration.
2. **Build verification**: After changes, run `dotnet build` and report errors before claiming success.
3. **Target framework**: Windows-only (`net{X}-windows`, `UseWPF=true`).
4. **NuGet**: Use `DevExpress.Wpf.Ribbon` (brings Bars transitively). All DevExpress packages share one version.
5. **XAML namespaces**: `dxr:` Ribbon, `dxb:` Bars, `dx:` Core. Don't mix them up.
6. **Items vs Links**: Use direct items for the common case; use `*Link` types only when sharing items across multiple bars (typically with `BarManager`).
7. **Prefer modern bar controls**: Use `MainMenuControl` / `ToolBarControl` / `StatusBarControl` for new code. Reach for `BarManager` only when you need centralized cross-bar item sharing.
8. **Don't mix Ribbon and BarManager** in the same window.
9. **For Office 2010+ Ribbon style**, use `BackstageViewControl` for the application menu. For Office 2007, use `ApplicationMenu`.
10. **Application ambiguity**: When generating `App.xaml.cs` on .NET 6+, qualify `System.Windows.Application`.

## Using DevExpress Documentation MCP

- **Search**: `devexpress_docs_search(technology="WPF Ribbon", query="<your question>")`
- **Search**: `devexpress_docs_search(technology="WPF Bars", query="<your question>")`
- **Fetch**: `devexpress_docs_get_content(url="<documentation URL>")`

Use MCP when you need exact API signatures for less-common items (`BarLinkContainerItem`, `BarItemSelector`, `ToolbarListItem`, `LinkListItem`) or version-specific bindings.

---

## Next Steps

Start with **[Getting Started](references/getting-started.md)** for project setup and the ThemedWindow conversion. Then **[Ribbon Structure](references/ribbon-structure.md)** for the page/group anatomy, or **[Bars and Layout](references/bars-and-layout.md)** if you're building a classic toolbar+menu+statusbar UI.

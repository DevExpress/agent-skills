# Ribbon Structure

The Ribbon is a tree of well-defined elements: `RibbonControl` → `RibbonPageCategory` → `RibbonPage` → `RibbonPageGroup` → `BarItem` / `BarItemLink`. Around this tree sit the Quick Access Toolbar, Application Menu, Page Header items, and the optional Ribbon Status Bar. This page documents the nesting and the role of each element — once the shape is clear, a single XAML snippet covers most cases.

## When to Use This Reference

Use this when you need to:

- Understand the Ribbon's element hierarchy
- Add a default category (always visible) vs custom (contextual) categories
- Build pages and page groups
- Add a Quick Access Toolbar (QAT)
- Add commands to the Page Header region (right edge of the title bar)
- Use an `ApplicationMenu` (Office 2007 style) or `BackstageViewControl` (Office 2010+)
- Add a `RibbonStatusBarControl` at the bottom of the window

## Nesting at a Glance

- `RibbonControl`
  - `ToolbarItems` — Quick Access Toolbar (bar items)
  - `ApplicationMenu` — `ApplicationMenu` or `BackstageViewControl`
  - `PageHeaderItems` — right-edge title-bar items
  - `RibbonDefaultPageCategory` — always visible, no caption
    - `RibbonPage[]` — tabs
      - `RibbonPageGroup[]` — sections inside a tab
        - `BarItem[]` — buttons, edits, galleries, ...
  - `RibbonPageCategory[]` — custom / contextual categories (visible on demand via `IsVisible`)
    - `RibbonPage[]` → `RibbonPageGroup[]` → `BarItem[]`
- `RibbonStatusBarControl` — separate control, docked to bottom of window
  - `LeftItems` — left-aligned status items
  - `RightItems` — right-aligned status items

Place the Ribbon inside a `ThemedWindow` with `WindowKind="Ribbon"` so it integrates into the title bar.

## Single Complete Snippet

This snippet exercises every structural element. Reading it once is usually enough to understand the layout.

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

            <!-- 1. Quick Access Toolbar — frequently used commands -->
            <dxr:RibbonControl.ToolbarItems>
                <dxb:BarButtonItem Content="Save"
                                   Glyph="{dx:DXImage SvgImages/Save/Save.svg}"
                                   Command="{Binding SaveCommand}"/>
                <dxb:BarButtonItem Content="Undo"
                                   Glyph="{dx:DXImage SvgImages/Edit/Undo.svg}"
                                   Command="{Binding UndoCommand}"/>
            </dxr:RibbonControl.ToolbarItems>

            <!-- 2. Application Menu (Office 2007 style modal menu) -->
            <dxr:RibbonControl.ApplicationMenu>
                <dxr:ApplicationMenu>
                    <dxb:BarButtonItem Content="New"   Command="{Binding NewCommand}"/>
                    <dxb:BarButtonItem Content="Open"  Command="{Binding OpenCommand}"/>
                    <dxb:BarItemSeparator/>
                    <dxb:BarButtonItem Content="Exit"  Command="{Binding ExitCommand}"/>
                </dxr:ApplicationMenu>
            </dxr:RibbonControl.ApplicationMenu>

            <!-- 3. Page Header items — right edge of the title bar -->
            <dxr:RibbonControl.PageHeaderItems>
                <dxb:BarButtonItem Content="Help"
                                   Glyph="{dx:DXImage SvgImages/Help/Help.svg}"
                                   Command="{Binding HelpCommand}"/>
            </dxr:RibbonControl.PageHeaderItems>

            <!-- 4. Default page category — pages are ALWAYS visible -->
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
                    <dxr:RibbonPageGroup Caption="Font">
                        <dxb:BarCheckItem Content="Bold"   GroupIndex="0"/>
                        <dxb:BarCheckItem Content="Italic" GroupIndex="0"/>
                    </dxr:RibbonPageGroup>
                </dxr:RibbonPage>

                <dxr:RibbonPage Caption="View">
                    <dxr:RibbonPageGroup Caption="Layout">
                        <dxb:BarCheckItem Content="Grid Lines"
                                          IsChecked="{Binding ShowGridLines, Mode=TwoWay}"/>
                    </dxr:RibbonPageGroup>
                </dxr:RibbonPage>
            </dxr:RibbonDefaultPageCategory>

            <!-- 5. Custom (contextual) page category — visible only on demand -->
            <dxr:RibbonPageCategory Caption="Picture Tools"
                                    Color="OrangeRed"
                                    IsVisible="{Binding IsPictureSelected}">
                <dxr:RibbonPage Caption="Format">
                    <dxr:RibbonPageGroup Caption="Adjust">
                        <dxb:BarButtonItem Content="Crop"
                                           Command="{Binding CropCommand}"/>
                    </dxr:RibbonPageGroup>
                </dxr:RibbonPage>
            </dxr:RibbonPageCategory>
        </dxr:RibbonControl>

        <!-- 6. Ribbon Status Bar — bottom of the window -->
        <dxr:RibbonStatusBarControl DockPanel.Dock="Bottom">
            <dxr:RibbonStatusBarControl.LeftItems>
                <dxb:BarStaticItem Content="{Binding StatusText}"/>
            </dxr:RibbonStatusBarControl.LeftItems>
            <dxr:RibbonStatusBarControl.RightItems>
                <dxb:BarStaticItem Content="{Binding LineColumn}"/>
            </dxr:RibbonStatusBarControl.RightItems>
        </dxr:RibbonStatusBarControl>

        <!-- Main content -->
        <Grid/>
    </DockPanel>
</dx:ThemedWindow>
```

## Element Reference

### `RibbonControl`

The root. Holds toolbar items, application menu, page header items, and a flat list of page categories (default + custom).

| Property | Purpose |
|---|---|
| `RibbonStyle` | Paint style: `Office2007`, `Office2010`, `Office2019`, `OfficeSlim`, `TabletOffice` |
| `ApplicationMenu` | The Office button menu — assign `ApplicationMenu` (2007 look) or `BackstageViewControl` (2010+ look) |
| `ToolbarItems` | Items in the Quick Access Toolbar |
| `PageHeaderItems` | Items at the right edge of the title bar |
| `Categories` / `CategoriesSource` | Page categories (declarative or MVVM) |
| `AllowSimplifiedRibbon` / `IsSimplified` | Office 2019 single-line Simplified mode |
| `RibbonHeaderVisibility` / `RibbonTitleBarVisibility` | Hide parts of the header for minimal layouts |
| `SupportSidePanels` | Allow side-panel layouts when integrated into the window header |

### `RibbonPageCategory`

There are two kinds:

| Category | When to use |
|---|---|
| `RibbonDefaultPageCategory` | Always-visible pages. Has **no caption**. There's only one per Ribbon, holding the main tabs (Home, View, Insert, etc.). |
| `RibbonPageCategory` | Custom / **contextual** category. Shows on demand via `IsVisible`. Has a colored caption above the page tabs. Used for "Picture Tools," "Drawing Tools," etc., that appear when the relevant selection is active. |

`RibbonPageCategory.Color` tints the category caption and page tabs — the typical orange/blue/green contextual highlight in Office.

```xaml
<dxr:RibbonPageCategory Caption="Table Tools"
                        Color="MediumPurple"
                        IsVisible="{Binding IsTableSelected}">
    <dxr:RibbonPage Caption="Design">...</dxr:RibbonPage>
    <dxr:RibbonPage Caption="Layout">...</dxr:RibbonPage>
</dxr:RibbonPageCategory>
```

### `RibbonPage`

A tab. Owns one or more `RibbonPageGroup`s.

| Property | Purpose |
|---|---|
| `Caption` | Tab header text |
| `Groups` / `GroupsSource` | Page groups (declarative or MVVM) |
| `IsVisible` | Hide a page conditionally |
| `MergeType` / `MergeOrder` | MDI merging — see [merging.md](merging.md) |

### `RibbonPageGroup`

A section inside a page. Hosts the actual bar items.

| Property | Purpose |
|---|---|
| `Caption` | Group caption at the bottom of the group |
| `Glyph` | Small icon shown when the group is collapsed to a button (due to width constraints) |
| `ItemLinks` / `ItemLinksSource` | Child items (declarative or MVVM) |
| `IsCaptionVisible` | Hide the group caption (e.g., for very compact groups) |
| `ShowLauncherButton` | Show the small launcher button at the bottom-right corner of the group |

Place `BarItem`s directly inside `RibbonPageGroup`:

```xaml
<dxr:RibbonPageGroup Caption="Clipboard" ShowLauncherButton="True">
    <dxb:BarButtonItem Content="Paste"
                       Glyph="{dx:DXImage Paste_16x16.png}"
                       LargeGlyph="{dx:DXImage Paste_32x32.png}"
                       Command="{Binding PasteCommand}"/>
    <dxb:BarItemSeparator/>
    <dxb:BarButtonItem Content="Cut"/>
    <dxb:BarButtonItem Content="Copy"/>
</dxr:RibbonPageGroup>
```

The Ribbon **auto-shrinks** groups: when the window narrows, large buttons collapse to small ones, then groups collapse to a single dropdown button. Always provide both `Glyph` and `LargeGlyph` on bar items inside a Ribbon so the swap looks right.

### Quick Access Toolbar (QAT)

Items in `RibbonControl.ToolbarItems` show in a small toolbar at the top of the window — above or below the Ribbon, depending on `RibbonControl.ToolbarShowMode`. End-users can right-click ribbon items to add them to the QAT at runtime.

```xaml
<dxr:RibbonControl.ToolbarItems>
    <dxb:BarButtonItem Content="Save"  Glyph="{dx:DXImage SvgImages/Save/Save.svg}"
                       Command="{Binding SaveCommand}"/>
    <dxb:BarButtonItem Content="Undo"  Glyph="{dx:DXImage SvgImages/Edit/Undo.svg}"
                       Command="{Binding UndoCommand}"/>
    <dxb:BarButtonItem Content="Redo"  Glyph="{dx:DXImage SvgImages/Edit/Redo.svg}"
                       Command="{Binding RedoCommand}"/>
</dxr:RibbonControl.ToolbarItems>
```

### Page Header Region

`RibbonControl.PageHeaderItems` shows items at the **right edge of the title bar**, next to the window controls. Use for help, account, settings — commands that should always be reachable regardless of the selected tab.

### Application Menu — Two Styles

| Choice | Look | Pair with |
|---|---|---|
| `ApplicationMenu` | Office 2007 modal menu | `RibbonStyle="Office2007"` |
| `BackstageViewControl` | Office 2010+ full-screen Backstage | `RibbonStyle="Office2010"` / `Office2019` |

Both are assigned to `RibbonControl.ApplicationMenu`. Pick one and match the `RibbonStyle`.

#### Office 2007 `ApplicationMenu`

```xaml
<dxr:RibbonControl.ApplicationMenu>
    <dxr:ApplicationMenu>
        <dxb:BarButtonItem Content="New"   Command="{Binding NewCommand}"/>
        <dxb:BarButtonItem Content="Open"  Command="{Binding OpenCommand}"/>
        <dxb:BarButtonItem Content="Save"  Command="{Binding SaveCommand}"/>
        <dxb:BarItemSeparator/>
        <dxb:BarButtonItem Content="Exit"  Command="{Binding ExitCommand}"/>
    </dxr:ApplicationMenu>
</dxr:RibbonControl.ApplicationMenu>
```

#### Office 2010+ `BackstageViewControl`

```xaml
<dxr:RibbonControl.ApplicationMenu>
    <dxr:BackstageViewControl>
        <dxr:BackstageButtonItem Content="New"   Command="{Binding NewCommand}"/>
        <dxr:BackstageButtonItem Content="Open"  Command="{Binding OpenCommand}"/>
        <dxr:BackstageTabItem Content="Info">
            <!-- Tab content -->
            <Grid>...</Grid>
        </dxr:BackstageTabItem>
        <dxr:BackstageTabItem Content="Print">
            <Grid>...</Grid>
        </dxr:BackstageTabItem>
    </dxr:BackstageViewControl>
</dxr:RibbonControl.ApplicationMenu>
```

Use Backstage when you have content-rich application sections (Info, Print preview, Account, Options) — it provides a full-screen modal experience like modern Office.

### `RibbonStatusBarControl`

A separate control (not inside the `RibbonControl`). Dock it to the bottom of the same window. Items are split into **left-aligned** and **right-aligned** collections:

```xaml
<DockPanel
    xmlns:dxr="http://schemas.devexpress.com/winfx/2008/xaml/ribbon"
    xmlns:dxb="http://schemas.devexpress.com/winfx/2008/xaml/bars"
    xmlns:dxe="http://schemas.devexpress.com/winfx/2008/xaml/editors">
    <dxr:RibbonStatusBarControl DockPanel.Dock="Bottom">
        <dxr:RibbonStatusBarControl.LeftItems>
            <dxb:BarStaticItem Content="{Binding DocumentStatus}"/>
            <dxb:BarStaticItem Content="{Binding WordCount}"/>
        </dxr:RibbonStatusBarControl.LeftItems>
        <dxr:RibbonStatusBarControl.RightItems>
            <dxb:BarStaticItem Content="{Binding ZoomLevel}"/>
            <dxb:BarEditItem EditValue="{Binding Zoom, Mode=TwoWay}"
                            EditWidth="80">
                <dxb:BarEditItem.EditSettings>
                    <dxe:SpinEditSettings MinValue="50" MaxValue="200" Increment="10"/>
                </dxb:BarEditItem.EditSettings>
            </dxb:BarEditItem>
        </dxr:RibbonStatusBarControl.RightItems>
    </dxr:RibbonStatusBarControl>
</DockPanel>
```

`RibbonStatusBarControl` participates in MDI merging — child windows can contribute their own status items.

## Simplified Ribbon Mode (Office 2019)

A single-line Ribbon that fits the look of Office Universal apps:

```xaml
<dxr:RibbonControl RibbonStyle="Office2019"
                   AllowSimplifiedRibbon="True"
                   IsSimplified="True">
    ...
</dxr:RibbonControl>
```

Provide a 20×20 `MediumGlyph` on bar items used in Simplified mode (fallback to `Glyph` if absent). You can also restrict items to one mode via the attached `SimplifiedModeSettings.Location` property. The `SimplifiedModeLocation` enum values are: `All` (show in both modes), `Simplified`, `Classic`, `OverflowMenu`, `ClassicAndOverflowMenu`.

## Common Issues

- **Ribbon shows but doesn't integrate with title bar** — window is `Window`, not `ThemedWindow`, or `WindowKind="Ribbon"` is missing.
- **Default category caption is visible** — used `RibbonPageCategory` (with caption) instead of `RibbonDefaultPageCategory` (no caption) for the main tabs.
- **Custom category always shows** — `IsVisible` isn't bound to a boolean property that changes based on context.
- **`BackstageViewControl` looks broken** — paired with `RibbonStyle="Office2007"`. Switch to `Office2010` or `Office2019`.
- **`ApplicationMenu` looks broken with modern style** — paired with `Office2019`. Switch to `BackstageViewControl` for modern Ribbon styles.
- **Status bar items overlap or float in the wrong place** — used `StatusBarControl` (bars-family) instead of `RibbonStatusBarControl` (ribbon-family). For Ribbon UIs, prefer `RibbonStatusBarControl`.
- **Items in QAT are duplicates of Ribbon items** — by design, the QAT is a *separate* collection of items, not links to existing ones. Use the same `Command` to share behavior.

## Source Material

- `articles/controls-and-libraries/ribbon-bars-and-menu/ribbon/ribbon-structure/ribbon-control.md` (`xref:7954`)
- `articles/controls-and-libraries/ribbon-bars-and-menu/ribbon/ribbon-structure/ribbon-page-categories-and-contextual-pages.md` (`xref:7960`)
- `articles/controls-and-libraries/ribbon-bars-and-menu/ribbon/ribbon-structure/ribbon-page.md` (`xref:7955`)
- `articles/controls-and-libraries/ribbon-bars-and-menu/ribbon/ribbon-structure/ribbon-page-group.md` (`xref:7956`)
- `articles/controls-and-libraries/ribbon-bars-and-menu/ribbon/ribbon-structure/ribbon-quick-access-toolbar.md` (`xref:7957`)
- `articles/controls-and-libraries/ribbon-bars-and-menu/ribbon/ribbon-structure/ribbon-status-bar.md` (`xref:7958`)
- `articles/controls-and-libraries/ribbon-bars-and-menu/ribbon/ribbon-structure/dxribbonwindow.md` (`xref:7980`)

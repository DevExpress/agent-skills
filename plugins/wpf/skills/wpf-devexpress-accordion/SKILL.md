---
name: wpf-devexpress-accordion
description: Build WPF applications with the DevExpress Accordion Control (AccordionControl) — a hierarchical navigation control for compact app sidebars and category-based UIs. Use when adding AccordionControl to a WPF project; defining static items in XAML (AccordionItem with Header, Glyph, Items); binding to a data source with ChildrenPath, ChildrenSelector, or HierarchicalDataTemplate; switching between Default and NavigationPane view modes; configuring expand modes (Single/Multiple/None); enabling the search field; integrating with OfficeNavigationBar; customizing items (header, glyph, content) and the collapsed/expanded UI. Also use when someone asks about "AccordionControl vs NavBarControl", "AccordionControl vs HamburgerMenu", "AccordionItem", "ChildrenPath", "AccordionViewMode", "dxa:", or "DevExpress.Xpf.Accordion". Covers .NET (6/7/8+) and .NET Framework 4.6.2+.
compatibility: Requires .NET 6+ or .NET Framework 4.6.2+ targeting Windows (net8.0-windows). DevExpress NuGet packages are published on nuget.org and via the local Unified Component Installer feed. A valid DevExpress license is required.
metadata:
  author: DevExpress
  version: "26.1"
  source-commit: b16066c633b85ee391d1e6188ffc4cd815ee5e8f
---

# DevExpress WPF Accordion Control

`DevExpress.Xpf.Accordion.AccordionControl` is a hierarchical navigation control with an unlimited tree depth, multiple selection / expand modes, optional Office-style navigation-pane layout, built-in search, and Office Navigation Bar integration. It's the recommended replacement for the older `NavBarControl` in new projects — simpler API, more customization room, modern look.

## When to Use This Skill

Use this skill when you need to:

- Add a hierarchical navigation sidebar to a WPF app
- Define items statically in XAML, or bind to a ViewModel collection
- Switch between Default (expand/collapse tree) and Navigation Pane (top-level items as tabs) view modes
- Show items with glyphs, headers, custom content (editors, images)
- Add a search field for end users to filter items
- Collapse the panel into a compact strip with glyph-only items
- Decide between `AccordionControl`, `NavBarControl`, and `HamburgerMenu` for navigation

## Prerequisites & Installation

### NuGet Packages

| Package | Purpose |
|---------|---------|
| `DevExpress.Wpf.Accordion` | Main package — `AccordionControl`, `AccordionItem` |
| `DevExpress.Wpf.Navigation` | Required when integrating with `OfficeNavigationBar` |

`DevExpress.Wpf.Accordion` transitively brings `DevExpress.Wpf.Core`.

### .NET (6/7/8+)

```bash
dotnet add package DevExpress.Wpf.Accordion
```

Add to `.csproj`:

```xml
<PropertyGroup>
  <TargetFramework>net8.0-windows</TargetFramework>
  <UseWPF>true</UseWPF>
</PropertyGroup>
```

DevExpress publishes packages on **nuget.org** (recommended). All DevExpress packages in a project must share the same version. A valid DevExpress license is required.

## Before You Start — Ask the Developer

1. **Target framework**: .NET 8+, .NET 6/7, or .NET Framework 4.x?
2. **Items source**: static (defined in XAML) or dynamic (bound to a ViewModel collection)?
3. **Data shape**: all items the same type with a children property → `ChildrenPath`; mixed item types → `ChildrenSelector`; rich templating per level → `HierarchicalDataTemplate`. See [data-binding.md](references/data-binding.md).
4. **View mode**: `Default` (classic accordion) or `NavigationPane` (Outlook-style root tabs)?
5. **Expand mode**: how many items can be open at once — `Multiple`, `MultipleOrNone`, `Single`, `SingleOrNone`?
6. **Search**: should end users search the items? (`ShowSearchControl`)
7. **Collapse**: should the panel collapse into a compact strip? (`IsCollapseButtonVisible`)
8. **Why Accordion vs alternatives**: see [when-to-use.md](references/when-to-use.md) for the comparison with `NavBarControl` and `HamburgerMenu`.

## Component Overview

### XAML Namespace

```xml
xmlns:dxa="http://schemas.devexpress.com/winfx/2008/xaml/accordion"
```

### Element Hierarchy

```
AccordionControl
├── (ItemsSource OR Items)
└── AccordionItem (root)
    ├── Header             — caption or any UIElement
    ├── Glyph              — icon
    └── Items              — child AccordionItem objects (or any nested UIElement)
```

`AccordionItem` plays two roles depending on context:

- **Root item** — top-level entry in the accordion
- **Subitem** — nested under a parent `AccordionItem`

The same class for both — the position in the tree (and `AccordionItem.ItemLevel`, where `0` is root) tells the runtime which.

### Two Population Approaches

1. **Static items in XAML** — define `AccordionItem` elements directly under `AccordionControl`. Good for fixed navigation menus.
2. **Data binding** — set `ItemsSource` to a collection. Good when the menu reflects a ViewModel, runtime data, or runtime additions.

Don't mix the two: if `ItemsSource` is set, the XAML-defined items are ignored.

## Documentation & Navigation Guide

### Getting Started
Refer to [references/getting-started.md](references/getting-started.md)

When you need to:
- Set up a .NET project with `DevExpress.Wpf.Accordion`
- Place the first `AccordionControl` on a window
- Bind it to a `List<T>` and see a working sidebar

### Data Binding
Refer to [references/data-binding.md](references/data-binding.md)

When you need to:
- Pick between **static XAML items**, `ChildrenPath`, `ChildrenSelector`, and `HierarchicalDataTemplate`
- Bind to homogeneous data (same item type all the way down)
- Bind to heterogeneous data (different types at different levels)
- Use rich per-level templates

### When to Use AccordionControl
Refer to [references/when-to-use.md](references/when-to-use.md)

When you need to:
- Compare `AccordionControl` against `NavBarControl` and `HamburgerMenu`
- Pick the right navigation control for the app's style and depth requirements
- Migrate a legacy `NavBarControl` to `AccordionControl`

### View Modes
Refer to [references/view-modes.md](references/view-modes.md)

When you need to:
- Switch between `Default` (classic accordion) and `NavigationPane` (Outlook-style root tabs)
- Integrate with `OfficeNavigationBar`
- Configure the Compact Navigation mode
- Use the Peek Form

### AccordionItem Settings and Customization
Refer to [references/items.md](references/items.md)

When you need to:
- Customize an item's header (text, custom UI, glyph position)
- Specify or template glyphs (`Glyph`, `GlyphTemplate`, `GlyphPosition`)
- Embed editors, images, or other custom content inside an item
- Configure `ItemDisplayMode`, `ExpandButtonPosition`, expand/collapse animation
- Manage selection and root-item handling

### Search
Refer to [references/search.md](references/search.md)

When you need to:
- Enable the built-in search box (`ShowSearchControl`)
- Customize search-tag, filter condition, null text
- Customize filter logic via `CustomItemFilter`
- Search across static and bound items

## Quick Start Examples

### Static Items in XAML

```xaml
<Window xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:dx="http://schemas.devexpress.com/winfx/2008/xaml/core"
        xmlns:dxa="http://schemas.devexpress.com/winfx/2008/xaml/accordion"
        Title="MyApp" Height="500" Width="800">
    <DockPanel>
        <dxa:AccordionControl DockPanel.Dock="Left" Width="240"
                              ShowSearchControl="True">
            <dxa:AccordionItem Header="Sales" Glyph="{dx:DXImage SvgImages/Reports/Reports.svg}">
                <dxa:AccordionItem Header="Orders"/>
                <dxa:AccordionItem Header="Invoices"/>
                <dxa:AccordionItem Header="Customers"/>
            </dxa:AccordionItem>
            <dxa:AccordionItem Header="Inventory" Glyph="{dx:DXImage SvgImages/Business Objects/BO_Product.svg}">
                <dxa:AccordionItem Header="Products"/>
                <dxa:AccordionItem Header="Stock Levels"/>
            </dxa:AccordionItem>
            <dxa:AccordionItem Header="Settings" Glyph="{dx:DXImage SvgImages/Outlook Inspired/Settings.svg}"/>
        </dxa:AccordionControl>

        <Grid/>
    </DockPanel>
</Window>
```

### Data-Bound (ChildrenPath)

```csharp
public class MenuNode {
    public string Title { get; set; } = "";
    public ObservableCollection<MenuNode> Children { get; } = new();
}

public class MainViewModel {
    public ObservableCollection<MenuNode> Sections { get; } = new() {
        new() {
            Title = "Sales",
            Children = {
                new() { Title = "Orders" },
                new() { Title = "Invoices" },
            }
        },
        new() { Title = "Settings" },
    };
}
```

```xaml
<dxa:AccordionControl ItemsSource="{Binding Sections}"
                      ChildrenPath="Children"
                      DisplayMemberPath="Title"/>
```

## Key Properties & API Surface

### `AccordionControl`

| Property | Use |
|---|---|
| `ItemsSource` | Data collection to populate items from |
| `ChildrenPath` | Property name on items that holds their children (homogeneous data) |
| `ChildrenSelector` | `IChildrenSelector` for heterogeneous data |
| `ItemTemplate` | `HierarchicalDataTemplate` defining per-level appearance |
| `DisplayMemberPath` | Property name used as the visible header text |
| `Items` | The imperative collection (when using static items) |
| `ViewMode` | `Default` or `NavigationPane` |
| `ExpandMode` | `Multiple`, `MultipleOrNone`, `Single`, `SingleOrNone` |
| `SelectedItem` | Bindable currently-selected item |
| `SelectedRootItem` | Selected root item (NavigationPane mode) |
| `ShowSearchControl` | Show the built-in search box |
| `SearchText` | Current search query |
| `SearchControlFilterCondition` | `Contains`, `Equals`, `StartsWith`, etc. |
| `IsExpanded` / `IsCollapseButtonVisible` | Collapse-mode toggle |
| `CompactNavigation` | Compact NavigationPane behavior |
| `ItemGlyphPosition` | Default glyph position (`Left`, `Top`, `Right`, `Bottom`) |
| `RootItemExpandButtonPosition` / `SubItemExpandButtonPosition` | Where the expand arrow renders |
| `ExpandItemOnHeaderClick` | Click anywhere on the header to expand (`true` by default) |
| `AllowAnimation` | Expand/collapse animation |

### `AccordionItem`

| Property | Use |
|---|---|
| `Header` | Caption text or any `UIElement` |
| `Glyph` | Icon image |
| `GlyphTemplate` / `GlyphTemplateSelector` | Custom glyph rendering |
| `GlyphPosition` | `Left`, `Top`, `Right`, `Bottom` (overrides control default) |
| `Items` | Child `AccordionItem` collection (or any nested UI) |
| `ItemDisplayMode` | `Default` or `Header` |
| `ItemVisibilityMode` | `Visible`, `Collapsed`, `ShowSubItems` |
| `ItemLevel` (read-only) | Zero-based depth (`0` = root) |
| `IsExpanded` | Bindable expand state (inherited from `TreeViewItem`) |
| `IsSelected` | Bindable selection state (inherited from `TreeViewItem`) |
| `ExpandButtonPosition` | Override the control's expand-button position |
| `AllowAnimation` | Per-item animation override |
| `SearchTag` | Extra string included in the search index |
| `ShowInCollapsedMode` | Show this item in the collapsed strip |
| `PeekFormTemplate` | Hover-popup content (NavigationPane mode) |

## Common Patterns

### Pattern 1: Bind to Sidebar Sections (MVVM)

```xaml
<dxa:AccordionControl ItemsSource="{Binding Sections}"
                      ChildrenPath="Children"
                      DisplayMemberPath="Title"
                      SelectedItem="{Binding CurrentSection, Mode=TwoWay}"/>
```

### Pattern 2: Static Items with Glyphs and Commands

```xaml
<dxa:AccordionControl>
    <dxa:AccordionItem Header="Dashboard"
                       Glyph="{dx:DXImage SvgImages/Dashboards/Dashboards.svg}"
                       Click="OnDashboardClick"/>
    <dxa:AccordionItem Header="Reports"
                       Glyph="{dx:DXImage SvgImages/Reports/Reports.svg}">
        <dxa:AccordionItem Header="Sales Report"/>
        <dxa:AccordionItem Header="Inventory Report"/>
    </dxa:AccordionItem>
</dxa:AccordionControl>
```

### Pattern 3: NavigationPane Mode with Office Bar

```xaml
<dxa:AccordionControl ViewMode="NavigationPane" CompactNavigation="True">
    <dxa:AccordionItem Header="Mail" Glyph="..."/>
    <dxa:AccordionItem Header="Calendar" Glyph="..."/>
    <dxa:AccordionItem Header="People" Glyph="..."/>
</dxa:AccordionControl>
```

### Pattern 4: Search Enabled

```xaml
<dxa:AccordionControl ItemsSource="{Binding Items}"
                      ChildrenPath="Children"
                      DisplayMemberPath="Title"
                      ShowSearchControl="True"
                      SearchControlNullText="Search menu..."/>
```

### Pattern 5: Collapsible Sidebar (Hamburger-Like)

```xaml
<dxa:AccordionControl IsCollapseButtonVisible="True"
                      CollapsedItemDisplayMode="Glyph"
                      SummaryItemHeader="Menu"
                      SummaryItemGlyph="{dx:DXImage SvgImages/Outlook Inspired/More.svg}">
    <dxa:AccordionItem Header="Inbox" Glyph="..." ShowInCollapsedMode="True">...</dxa:AccordionItem>
    <dxa:AccordionItem Header="Drafts" Glyph="..." ShowInCollapsedMode="True">...</dxa:AccordionItem>
</dxa:AccordionControl>
```

## Troubleshooting

| Symptom | Cause | Solution |
|---|---|---|
| `dxa:` prefix unresolved | Missing namespace or NuGet package | Add `xmlns:dxa="http://schemas.devexpress.com/winfx/2008/xaml/accordion"`; install `DevExpress.Wpf.Accordion`. |
| Static XAML items don't appear when `ItemsSource` is set | `ItemsSource` takes precedence over inline items | Choose one or the other. To start from XAML, leave `ItemsSource` unbound. |
| Children don't appear in data-bound mode | `ChildrenPath` doesn't match the property name | Verify the property name (case-sensitive). For mixed types, use `ChildrenSelector`. |
| Header shows class name instead of caption | `DisplayMemberPath` not set | Set `DisplayMemberPath="Title"` (or whichever property holds the visible text). Alternatively, override `ToString()` on the data class. |
| Search panel doesn't appear | `ShowSearchControl="False"` (default) | Set it to `True`. |
| `error CS0104: 'Application' is an ambiguous reference` | `DevExpress.Wpf.Accordion` transitively references `System.Windows.Forms`; `<ImplicitUsings>enable</ImplicitUsings>` on .NET 6+ creates the clash | Qualify `System.Windows.Application` in `App.xaml.cs`. |
| Selection doesn't bind | `SelectedItem` two-way binding missing `Mode=TwoWay`, or the bound type doesn't match | Add `Mode=TwoWay`; verify the property type matches what the accordion exposes. |
| Glyph doesn't show | Wrong path or missing image | Use `{dx:DXImage SvgImages/...svg}` or `{dx:DXImage Image=...png}`. |
| Items expand twice (clicks + arrow) | This is default behavior | Set `ExpandItemOnHeaderClick="False"` to require the arrow click only. |

## Constraints & Rules

CRITICAL — follow these rules in every interaction:

1. **Build verification**: After changes, run `dotnet build` and report errors before claiming success.
2. **Target framework**: Windows-only (`net{X}-windows`, `UseWPF=true`).
3. **NuGet**: Use `DevExpress.Wpf.Accordion`. All DevExpress packages share one version.
4. **XAML namespace**: `dxa:` (accordion). Do not use `dxn:` (navigation bar) or others.
5. **Pick one population method**: static items in XAML **or** `ItemsSource` — not both. `ItemsSource` wins when both are present.
6. **`DisplayMemberPath` is case-sensitive** and must match a property on the bound data class. For complex header content, use `ItemTemplate` (HierarchicalDataTemplate).
7. **For new projects, prefer `AccordionControl` over `NavBarControl`**. NavBarControl is older and is being phased out — its docs explicitly recommend AccordionControl. See [when-to-use.md](references/when-to-use.md).
8. **Application ambiguity**: When generating `App.xaml.cs` on .NET 6+, qualify `System.Windows.Application`.

## Using DevExpress Documentation MCP

- **Search**: `devexpress_docs_search(technology="WPF Accordion", query="<your question>")`
- **Fetch**: `devexpress_docs_get_content(url="<documentation URL>")`

Use MCP for specialized scenarios — `OfficeNavigationBar` integration details, `Peek Form` design, drag-and-drop reorder, custom collapse-mode templates.

---

## Next Steps

Start with **[Getting Started](references/getting-started.md)** for setup and a first accordion. Then **[Data Binding](references/data-binding.md)** to pick the right binding approach, or **[When to Use](references/when-to-use.md)** if you're still deciding between `AccordionControl`, `NavBarControl`, and `HamburgerMenu`.

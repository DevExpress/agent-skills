# When to Use AccordionControl vs NavBarControl vs Hamburger Menu

DevExpress ships three navigation controls that solve overlapping problems: **`AccordionControl`** (the modern, recommended one), **`NavBarControl`** (the legacy Outlook-2003-style nav pane), and **`HamburgerMenu`** (Windows-10-style modern UI nav). This page is a picker: read what each is for, match to your project's needs, pick one.

> **For new projects, start with `AccordionControl`.** The official DevExpress docs explicitly recommend it over `NavBarControl`: simpler API, deeper hierarchy support, more customization. Reach for `NavBarControl` only when migrating a legacy app or strictly emulating Outlook 2003/2007. Use `HamburgerMenu` when your design language is Modern UI / Windows 10.

## The Quick Picker

| You want... | Use |
|---|---|
| Hierarchical sidebar with expand/collapse groups, optional Outlook-style Navigation Pane mode | **`AccordionControl`** |
| Microsoft Office–style command sidebar (collapsible side panel, single tab focus) | **`AccordionControl`** in `NavigationPane` mode |
| Outlook 2003/2007 look exactly (legacy support) | **`NavBarControl`** (Navigation Pane view) |
| Microsoft Explorer-style side panel with collapsing groups | **`NavBarControl`** (Explorer view) |
| Windows-10-style hamburger menu with side-sliding nav | **`HamburgerMenu`** |
| Adaptive nav that adjusts to window size | **`HamburgerMenu`** |
| Top-level horizontal nav bar across the top of the window | Use `RibbonControl` (see the ribbon-and-bars skill) or `OfficeNavigationBar` |

## Detailed Comparison

| Feature | AccordionControl | NavBarControl | HamburgerMenu |
|---|---|---|---|
| **Recommended for new projects?** | **Yes** | No (legacy) | Yes, for Modern UI design |
| **Hierarchy depth** | Unlimited | 2 levels (groups → items) | 2 levels (items → submenu items) |
| **View modes** | Default + NavigationPane | Explorer, Navigation Pane, Side Bar | Inline + Compact bar |
| **Search built-in** | Yes (`ShowSearchControl`) | No | No |
| **Adaptive layout** | Manual via `IsCollapseButtonVisible` | No | Yes (auto-adjusting compact mode) |
| **Office Navigation Bar integration** | Yes | Yes | No |
| **Item content** | Any UI element | Any UI element | Limited — buttons, checks, radios, hyperlinks |
| **Custom items per node** | Glyph + Header + content via `ItemDisplayMode` | Per-view templates | Item type chosen per item |
| **MVVM via collection binding** | `ItemsSource`, `ChildrenPath`, `ChildrenSelector`, `HierarchicalDataTemplate` | `ItemsSource` + `ItemTemplate` (groups + items, no native two-level binding) | `ItemsSource` + `ItemTemplate` |
| **Visual style** | Modern flat | Office 2003 / 2007 / 2010 | Windows 10 / Universal apps |
| **Typical use** | Business app sidebar, settings menu | Legacy Office-style apps | Modern UI apps, touch-first apps |

## When to Use AccordionControl

**Choose AccordionControl when:**

1. **You're starting a new project** — it's the modern default. DevExpress documentation explicitly recommends migrating away from `NavBarControl` toward `AccordionControl`.
2. **The menu has more than 2 levels** — `AccordionControl` supports unlimited depth; `NavBarControl` is groups + items only (effectively 2).
3. **You want a search box in the menu** — only `AccordionControl` has this built in.
4. **You want flexible customization** — items can hold any content (editors, images, custom UI). Headers, glyphs, and content can all be templated independently.
5. **You want the option to switch between Default (classic accordion) and NavigationPane (Outlook-style tabs)** with one property — `ViewMode`. NavBar requires you to pick the view at design time and lose flexibility.

**Pattern**: app sidebar, settings tree, properties panel with collapsible groups.

## When to Use NavBarControl

**Choose NavBarControl when:**

1. **You're maintaining a legacy app** that already uses it.
2. **You need pixel-exact Outlook 2003/2007 look** — `NavBarControl` was designed to replicate it. The Explorer / Navigation Pane / Side Bar views all map to specific historical Office layouts.
3. **You explicitly want only 2 levels** — `NavBarControl` enforces a strict groups → items structure; some teams find this clearer than a tree.

**Pattern**: enterprise apps with Outlook-style left pane (Mail / Calendar / Contacts), older line-of-business apps.

> Tip in the DevExpress docs: _"If you start a new project, consider using the AccordionControl. The AccordionControl can also be used as a navigation pane, but it has a simpler API and provides more room for customization."_

## When to Use HamburgerMenu

**Choose HamburgerMenu when:**

1. **Your app uses Windows 10 / Modern UI design** — the hamburger menu is the iconic component of that style.
2. **You want adaptive layout** — the menu auto-collapses to a compact bar on narrow widths and expands on wide ones, similar to UWP / WinUI apps.
3. **The navigation is shallow** (2 levels) — that matches the hamburger menu's typical structure.
4. **You want item-type variety with minimal config** — `HamburgerMenu` exposes regular buttons, check buttons, radio buttons, and hyperlinks as first-class item types.

**Pattern**: consumer-style apps, settings panes in Modern UI apps, touch-friendly apps.

## Migration: NavBarControl → AccordionControl

If you're moving a legacy `NavBarControl` to `AccordionControl`:

| NavBar concept | AccordionControl equivalent |
|---|---|
| `NavBarControl.Groups` | `AccordionControl.Items` (root `AccordionItem` objects) |
| `NavBarGroup.Items` | `AccordionItem.Items` (children) |
| `NavBarControl.View` (Explorer / NavigationPane / SideBar) | `AccordionControl.ViewMode` (`Default` / `NavigationPane`) |
| `NavBarGroup.IsExpanded` | `AccordionItem.IsExpanded` |
| `NavBarControl.ItemsSource` + `ItemTemplate` | `AccordionControl.ItemsSource` + `ItemTemplate` (or `ChildrenPath`) |
| `NavBarItem.Content` | `AccordionItem.Header` (nested UI in the item tag for body content) |

The migration is straightforward when the existing app uses 2-level hierarchy (most do). The main differences:

- **Search** — newly available in `AccordionControl`. Free upgrade.
- **Unlimited depth** — if you want to flatten old "Group A → Item Group → Sub-Item" workarounds, you can now express them naturally.
- **`ItemsSource`** — picks up the same patterns; no major rewrite needed.

## When None of These Fit

For top-bar navigation (across the top of the window), don't use any of the three above. Use:

- **`RibbonControl`** — Office-style tabbed command surface. See the `wpf-devexpress-ribbon-and-bars` skill.
- **`OfficeNavigationBar`** — Outlook-2013-style bar with click-to-switch sections. Often combined with `AccordionControl` in NavigationPane mode.
- **`MainMenuControl`** / **`ToolBarControl`** — classic menu / toolbar. See the ribbon-and-bars skill.

## Common Issues

- **Picking NavBarControl by default for a new project** — outdated. Use `AccordionControl`. The DevExpress docs explicitly recommend it.
- **Trying to express 3+ levels in NavBarControl** — design constraint; switch to `AccordionControl`.
- **Trying to make HamburgerMenu look like Outlook** — wrong tool. Use `AccordionControl` in `NavigationPane` mode for that look.
- **Wanting search but using NavBarControl** — not supported in NavBar. Migrate to `AccordionControl`.

## Source Material

- `articles/controls-and-libraries/navigation-controls/navigation-bar.md` (`xref:6189`) — see the tip recommending `AccordionControl` for new projects
- `articles/controls-and-libraries/navigation-controls/accordion-control.md` (`xref:118347`)
- `articles/controls-and-libraries/windows-modern-ui/hamburger-menu.md` (`xref:119536`)

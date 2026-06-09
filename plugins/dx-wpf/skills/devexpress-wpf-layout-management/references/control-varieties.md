# Control Varieties — Picking the Right Layout Control

Six controls, six different layout problems. Picking the wrong one is expensive — switching containers later usually means rewriting the layout. Use this page as the picker before opening any of the others.

## When to Use This Reference

Use this when you need to:

- Pick one of the six layout controls
- Understand the difference between similar-sounding names (`DockLayoutManager` vs `DockLayoutControl`, `LayoutControl` vs `DataLayoutControl`)
- Compare `TileLayoutControl` and `FlowLayoutControl`
- Migrate from a legacy choice

## The Six Controls — One-Line Summary

| Control | What it's for | Library |
|---|---|---|
| **`DockLayoutManager`** | Visual-Studio-style window: dockable panels, floating windows, MDI document tabs, document selector | `DevExpress.Xpf.Docking` |
| **`LayoutControl`** | Compound data-entry form with auto-aligned labels and nested groups (tabs, group boxes) | `DevExpress.Xpf.LayoutControl` |
| **`DataLayoutControl`** | A `LayoutControl` auto-populated from a POCO via `[Display]` / `[DataType]` attributes | `DevExpress.Xpf.LayoutControl` |
| **`TileLayoutControl`** | Windows UI tile dashboard with four tile sizes and tile groups | `DevExpress.Xpf.LayoutControl` |
| **`FlowLayoutControl`** | Wrapping flow of cards/group boxes with resizing and maximization | `DevExpress.Xpf.LayoutControl` |
| **`DockLayoutControl`** | Simple edge docking (Top/Left/Right/Bottom/Client) inside one panel | `DevExpress.Xpf.LayoutControl` |

## Picker by Scenario

### "I want a Visual-Studio-style application shell"

→ **`DockLayoutManager`**

Visual-Studio-like means: panels you can drag, float, and dock to any edge; document tabs; the customization window; persisted layout. The DockLayoutManager is the only control that does this. It also handles MDI bar/ribbon merging (see ribbon-and-bars skill).

### "I want a settings panel or data-entry form with labels and groups"

→ **`LayoutControl`** (static XAML) or **`DataLayoutControl`** (auto-generated from a class)

The LayoutControl auto-aligns labels not only within a group but across sibling groups — so columns of editors look professional without manual margin tweaking. Use `DataLayoutControl` when the form should be driven entirely by the data class (typical for "edit any entity" generic CRUD UIs).

### "I want a Windows UI tile dashboard"

→ **`TileLayoutControl`**

Use this for an Office365/Outlook-like hub with rectangular tiles. Tiles have four pre-defined sizes, auto-cycling content, and drag-and-drop. Order is decided by tile size, not by `Width`/`Height`.

### "I want a wrapping flow of cards"

→ **`FlowLayoutControl`**

Use this for catalogs, dashboards built from group boxes, or any UI where items should wrap to a new row/column when they don't fit. Supports per-item flow breaks and the maximize-one-item-take-the-rest pattern.

### "I want to dock items to the edges of a single area (no detach, no floating)"

→ **`DockLayoutControl`**

This is a simplified docking container — `WPF.DockPanel` with DevExpress-grade theming and runtime resizing. Items dock to `Top`/`Bottom`/`Left`/`Right`/`Client`. If you need detachable panels, MDI, or a customization window, use `DockLayoutManager` instead — `DockLayoutControl` is intentionally lighter.

## DockLayoutManager vs DockLayoutControl — Don't Confuse Them

| Feature | DockLayoutManager (`dxdo:`) | DockLayoutControl (`dxlc:`) |
|---|---|---|
| Visual-Studio-style panels | Yes | No |
| Floating windows | Yes | No |
| MDI / document tabs | Yes | No |
| Customization window | Yes | No |
| Save/restore via `SaveLayoutToStream` | Yes | Yes (`WriteToXML`) |
| Right-click panel options | Yes | No |
| Auto-hide ("pin") behavior | Yes | No |
| Bar / Ribbon merging across child windows | Yes | No |
| Simple edge docking | Yes (and more) | Yes — main use case |
| API complexity | High | Low |

**Rule of thumb:** if the app shell needs ANY of "floating", "tabbed documents", "user customization", "panel pin/unpin" — pick `DockLayoutManager`. If you only need "items docked to one panel's edges in fixed positions with optional resize thumbs" — pick `DockLayoutControl`.

## LayoutControl vs DataLayoutControl

| Aspect | `LayoutControl` | `DataLayoutControl` |
|---|---|---|
| Layout author | You (XAML) | DataAnnotations on the bound POCO |
| Editor selection | You pick each editor (`TextEdit`, `DateEdit`, etc.) | Automatically chosen by property type |
| Group / tab structure | XAML `<LayoutGroup>` nesting | `[Display(GroupName = "{Tabs}/Contact")]` attribute paths |
| When the data class changes | Re-edit XAML | Re-decorate the class |
| Best for | Bespoke forms, designer-controlled layouts | Generic edit forms, plugin-style apps, "show whatever object is bound" |
| Sub-customization | Total | Limited (constrained by attribute grammar) |

If you want a totally custom form — `LayoutControl`. If you want "throw any class at it and get a usable form" — `DataLayoutControl`.

You can also use `DataLayoutControl` for the bulk of a form and override individual items in XAML; but for most apps, picking one approach upfront keeps things clean.

## TileLayoutControl vs FlowLayoutControl

Both wrap items, both support drag-and-drop. The difference is:

| | TileLayoutControl | FlowLayoutControl |
|---|---|---|
| Item type | `Tile` (size enum: Small / Large / ExtraSmall / ExtraLarge) | Any `UIElement` (typically `GroupBox`) |
| Item sizing | By `Tile.Size` enum only | Free (`Width`, `Height`) |
| Group headers | `TileLayoutControl.GroupHeader` attached property | Manual headers inside items |
| Animated content cycling | Yes (`ContentSource`, `ContentChangeInterval`) | No |
| Per-item maximize-to-fill | No | Yes (`MaximizedElement`) |
| Layer separators (drag to resize rows/cols) | No | Yes |
| Touch optimization | Yes (UI primary target) | Yes |

**Pick `TileLayoutControl`** for a tile/launcher UI with branded fixed tile sizes (think Windows Start screen, Outlook tiles).

**Pick `FlowLayoutControl`** when items have natural sizes and you want a flexible flow (cards, group boxes, list of widgets). Or when one item should "maximize" while others shrink to fit alongside.

The two are related — `TileLayoutControl` inherits from `FlowLayoutControl`, so flow features (`BreakFlowToFit`, `IsFlowBreak`, `AllowItemMoving`, `ItemPositionChanged`) work in both. But `TileLayoutControl`'s ordering is driven by `Tile.Size`, not by item dimensions.

## Decision Tree (Quick)

```
Do you need dockable panels with floating / MDI?
├── YES → DockLayoutManager
└── NO
    ├── Is the form auto-generated from a class? → DataLayoutControl
    ├── Is it a form with labels + editors + groups (custom XAML)? → LayoutControl
    ├── Is it a tile-based dashboard? → TileLayoutControl
    ├── Is it a wrapping flow of cards? → FlowLayoutControl
    └── Is it simple edge docking inside one container? → DockLayoutControl
```

## What About Standard WPF Controls?

DevExpress controls coexist with the standard WPF layout primitives (`Grid`, `StackPanel`, `DockPanel`, `WrapPanel`). The DevExpress controls add value when you need:

- Auto-aligned labels across groups (LayoutControl)
- Data-driven form generation (DataLayoutControl)
- User customization / drag-drop (all)
- Save/restore (all)
- Tile-style UI with sizing animation (TileLayoutControl)
- Maximize-one-item-take-the-rest (FlowLayoutControl)
- Visual-Studio-style docking (DockLayoutManager)

For trivial layouts (simple two-column form, single sidebar), a plain `Grid` or `DockPanel` is often enough — don't reach for `LayoutControl` if you don't need its features.

## Migration Cheatsheet

| From | To | Why |
|---|---|---|
| WPF `Grid` for forms | `LayoutControl` | Auto label alignment, customization, save/restore |
| Hand-written attribute-driven form | `DataLayoutControl` | Use existing annotations |
| WPF `DockPanel` for shell | `DockLayoutManager` | When app grows to need floating / MDI / customization |
| WPF `WrapPanel` for cards | `FlowLayoutControl` | When you need drag-drop, resize separators, maximize |
| Custom tile UI | `TileLayoutControl` | Battle-tested Modern-UI presentation |
| WinForms `BarManager`-style docking | `DockLayoutManager` | The WPF-native replacement |

## Common Issues

- **Picked `DockLayoutControl` and now want floating panels** — wrong choice. Migrate to `DockLayoutManager` (the API is different; not a drop-in replacement).
- **Wanted auto-aligned labels but used a `Grid`** — switch to `LayoutControl` with `LayoutItem`s; alignment "just works" across groups.
- **`DataLayoutControl` ignores attributes** — wrong namespace. Use `System.ComponentModel.DataAnnotations`, not Avalonia / WinUI / MAUI variants.
- **TileLayoutControl's tiles arrange wrong** — set `Tile.Size`, not `Width`/`Height`. Width/Height are ignored for ordering.
- **`LayoutGroup` reference doesn't resolve** — typed `<LayoutGroup>` without prefix and got the wrong namespace's class. Use `<dxlc:LayoutGroup>` or `<dxdo:LayoutGroup>` explicitly.

## Source Material

- `articles/controls-and-libraries/layout-management/tile-and-layout.md` (https://docs.devexpress.com/content/WPF/8085?md=true)
- `articles/controls-and-libraries/layout-management/dock-windows.md` (https://docs.devexpress.com/content/WPF/6191?md=true)
- `articles/controls-and-libraries/layout-management/tile-and-layout/layout-and-data-layout-controls/layout-control.md` (https://docs.devexpress.com/content/WPF/8147?md=true)
- `articles/controls-and-libraries/layout-management/tile-and-layout/layout-and-data-layout-controls/data-layout-control.md` (https://docs.devexpress.com/content/WPF/11540?md=true)
- `articles/controls-and-libraries/layout-management/tile-and-layout/tile-layout-control.md` (https://docs.devexpress.com/content/WPF/11541?md=true)
- `articles/controls-and-libraries/layout-management/tile-and-layout/flow-layout-control.md` (https://docs.devexpress.com/content/WPF/8148?md=true)
- `articles/controls-and-libraries/layout-management/tile-and-layout/dock-layout-control.md` (https://docs.devexpress.com/content/WPF/8149?md=true)

# Bars and Layout

DevExpress offers two approaches to building a bars-based UI (no Ribbon): **standalone bar controls** (`MainMenuControl` / `ToolBarControl` / `StatusBarControl`) and the legacy **`BarManager`** with `Bar` objects. **Standalone controls are the recommended approach** for new code — they're simpler, more declarative, and integrate with `BarContainerControl` for positioning. `BarManager` is still supported but should only be used when you genuinely need centralized item sharing across many bars in one window.

## When to Use This Reference

Use this when you need to:

- Build a classic toolbar + menu + status bar UI (no Ribbon)
- Decide between standalone bar controls and `BarManager`
- Place bars at the top / left / right / bottom edges of a window
- Make a bar draggable / floatable
- Arrange multiple bars side-by-side in one row

## Two Approaches at a Glance

| Approach | When |
|---|---|
| **Standalone controls** (`MainMenuControl`, `ToolBarControl`, `StatusBarControl`) | Most apps. One main menu + a few toolbars + a status bar. Recommended. |
| **`BarManager`** + child `Bar` objects | When you need many bars that share items, centralized customization (`ToolbarListItem`, `LinkListItem`), or you're migrating WinForms `BarManager` code. |

> Do not mix standalone bar controls with a `BarManager` in the same window. Pick one. Inside a Ribbon UI, use the Ribbon — not bars — for the main command surface.

## Approach 1: Standalone Bar Controls (Recommended)

There are exactly three standalone bar controls, each specialized for its role:

| Control | Purpose |
|---|---|
| `MainMenuControl` | A horizontal main menu, usually docked to the top of a window |
| `ToolBarControl` | A toolbar, usually docked top/side, can float |
| `StatusBarControl` | A status bar, usually docked to the bottom |

You define items directly between the bar control's tags. Item links also work as items here:

```xaml
<dxb:ToolBarControl VerticalAlignment="Top">
    <dxb:BarButtonItem Content="Cut"/>
    <dxb:BarButtonItem Content="Copy"/>
    <dxb:BarButtonItem Content="Paste"/>
</dxb:ToolBarControl>
```

### Layout with `DockPanel`

The simplest layout — `DockPanel` docks each bar to an edge:

```xaml
<DockPanel>
    <dxb:MainMenuControl DockPanel.Dock="Top">
        <dxb:BarSubItem Content="File">
            <dxb:BarButtonItem Content="Open"  Command="{Binding OpenCommand}"/>
            <dxb:BarButtonItem Content="Save"  Command="{Binding SaveCommand}"/>
            <dxb:BarItemSeparator/>
            <dxb:BarButtonItem Content="Exit"  Command="{Binding ExitCommand}"/>
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

### Side-by-Side Bars — `BarContainerControl`

To put multiple `ToolBarControl` / `MainMenuControl` / `StatusBarControl` next to each other on the **same row**, wrap them in a `BarContainerControl`:

```xaml
<dxb:BarContainerControl>
    <dxb:ToolBarControl>
        <dxb:BarButtonItem Content="Cut"/>
        <dxb:BarButtonItem Content="Copy"/>
        <dxb:BarButtonItem Content="Paste"/>
    </dxb:ToolBarControl>

    <dxb:ToolBarControl>
        <dxb:BarCheckItem Content="Bold"/>
        <dxb:BarCheckItem Content="Italic"/>
        <dxb:BarCheckItem Content="Underline"/>
    </dxb:ToolBarControl>
</dxb:BarContainerControl>
```

Two side benefits of `BarContainerControl`:

1. The wrapped bars get a **drag widget** at their left edge — users can drag them to reorder or detach (float) at runtime.
2. The bars don't overlap; the container manages widths.

A standalone bar control NOT wrapped in `BarContainerControl` doesn't display a drag widget.

### Docking Bars to Window Edges

For full window-edge docking (left/right/bottom in addition to top), use one `BarContainerControl` per edge — typically inside a `DockPanel`:

```xaml
<DockPanel>
    <dxb:BarContainerControl DockPanel.Dock="Top">
        <dxb:MainMenuControl>...</dxb:MainMenuControl>
        <dxb:ToolBarControl>...</dxb:ToolBarControl>
    </dxb:BarContainerControl>

    <dxb:BarContainerControl DockPanel.Dock="Left">
        <dxb:ToolBarControl>...</dxb:ToolBarControl>
    </dxb:BarContainerControl>

    <dxb:BarContainerControl DockPanel.Dock="Bottom">
        <dxb:StatusBarControl>...</dxb:StatusBarControl>
    </dxb:BarContainerControl>

    <Grid/>
</DockPanel>
```

### Item Links Inside Standalone Bars

You can use item links if items are defined elsewhere (e.g., when the bar shares items with a context menu):

```xaml
<dxb:MainMenuControl>
    <dxb:BarSubItem Content="Edit">
        <dxb:BarButtonItemLink BarItemName="btnOpen"/>
    </dxb:BarSubItem>
</dxb:MainMenuControl>
```

`BarItemName` must resolve to a `BarItem`'s `x:Name`. For simple cases, define items directly — implicit links are simpler than tracking names.

## Approach 2: BarManager (Legacy)

`BarManager` is a container that owns:

1. **`BarManager.Bars`** — a collection of `Bar` objects (the actual toolbars / menus)
2. **`BarManager.Items`** — a centralized collection of bar items
3. **Implicit `BarContainerControl`s** at top/left/right/bottom (when `CreateStandardLayout="True"` — the default)

The pattern:

```xaml
<dxb:BarManager>
    <!-- Items defined once, in one place -->
    <dxb:BarManager.Items>
        <dxb:BarButtonItem x:Name="bCut"   Content="Cut"
                           Glyph="{dx:DXImage Cut_16x16.png}"
                           KeyGesture="Ctrl+X"/>
        <dxb:BarButtonItem x:Name="bCopy"  Content="Copy"
                           Glyph="{dx:DXImage Copy_16x16.png}"
                           KeyGesture="Ctrl+C"/>
        <dxb:BarButtonItem x:Name="bPaste" Content="Paste"
                           Glyph="{dx:DXImage Paste_16x16.png}"
                           KeyGesture="Ctrl+V"/>
    </dxb:BarManager.Items>

    <!-- Bars reference items by name via item links -->
    <dxb:BarManager.Bars>
        <dxb:Bar Caption="Main Toolbar">
            <dxb:Bar.DockInfo>
                <dxb:BarDockInfo ContainerType="Top"/>
            </dxb:Bar.DockInfo>
            <dxb:BarButtonItemLink BarItemName="bCut"/>
            <dxb:BarButtonItemLink BarItemName="bCopy"/>
            <dxb:BarButtonItemLink BarItemName="bPaste"/>
        </dxb:Bar>
    </dxb:BarManager.Bars>

    <Grid/>
</dxb:BarManager>
```

### `Bar.DockInfo` — Positioning

A `Bar` object alone has no on-screen presence. It needs association with a `BarContainerControl` via `Bar.DockInfo.ContainerType`. When `BarManager.CreateStandardLayout="True"`, the four implicit containers handle this — set `ContainerType` to `Top`, `Left`, `Right`, `Bottom`, or `Floating`.

### Floating Bars

```xaml
<dxb:Bar Caption="Formatting">
    <dxb:Bar.DockInfo>
        <dxb:BarDockInfo ContainerType="Floating"
                         FloatPos="200,300"
                         FloatSize="150,30"/>
    </dxb:Bar.DockInfo>
    ...
</dxb:Bar>
```

Floating bars don't need a `BarContainerControl` — they live in their own pop-out window.

### When BarManager Is Worth It

- **Multiple bars share the same items**. With `BarManager`, change the item once → all bars update. With standalone controls, you'd write multiple `BarItemLink BarItemName="..."` references (still possible but heavier).
- **You want runtime "show/hide bars" UI**. `ToolbarListItem` and `LinkListItem` (built-in items that list bars/links with check boxes) work naturally inside `BarManager`.
- **You're migrating from DevExpress WinForms `BarManager`-based code**.

### Where Standalone Controls Are Better

- Simpler XAML — items defined inline, no name plumbing.
- Cleaner integration with `DockPanel` / `Grid` — each bar is a normal WPF control.
- Easier MVVM binding (`ItemsSource` on the bar itself, no `BarManager.BarsSource` indirection).

## Other Layout Constructs

### `Bar.AllowQuickCustomization`

`ToolBarControl` and `Bar` both support runtime customization via a dropdown chevron — users can show/hide items, add/remove buttons. Set on the bar:

```xaml
<dxb:ToolBarControl AllowQuickCustomization="True">
    ...
</dxb:ToolBarControl>
```

### `Bar.IsCloseButtonVisible`

For runtime "close this bar" behavior on floating bars.

### Glyph Sizes

Standalone bar controls and `BarManager` share the same `BarItem.Glyph` / `LargeGlyph` system. Toolbar buttons typically show small glyphs; sub-menus also show small glyphs unless explicitly enlarged. See `articles/.../common-concepts/displaying-glyphs.md` (`xref:117633`).

## Choosing the Right Container — Decision Table

| What you have | Use |
|---|---|
| One main menu + few toolbars + status bar | `MainMenuControl`, `ToolBarControl`, `StatusBarControl` (standalone) |
| Bars that float, drag, dock to any edge | Standalone bars wrapped in `BarContainerControl` per edge |
| Many bars sharing items, runtime show/hide | `BarManager` + `Bar` objects |
| Office-style ribbon | `RibbonControl` (not bars at all) |

## Common Issues

- **Bar appears but no drag handle** — bar is not wrapped in `BarContainerControl`. Wrap it.
- **`Bar` defined but never appears on screen** — used `BarManager.Bars` with no `BarContainerControl` or `CreateStandardLayout="False"` and no explicit container. Either keep `CreateStandardLayout="True"` (default) or add a `BarContainerControl` explicitly.
- **Two bars overlap on the same edge** — both are docked to the same `DockPanel` edge without a `BarContainerControl`. Wrap them.
- **MVVM `ItemsSource` doesn't work on `Bar`** — `Bar` uses `ItemLinksSource`, not `ItemsSource`. The standalone controls (`ToolBarControl`, etc.) use `ItemsSource` directly.
- **`StatusBarControl` items aren't right-aligned** — `StatusBarControl` arranges items in source order; for right-aligned content, prefer `RibbonStatusBarControl` (`LeftItems` / `RightItems` collections) or wrap items in a layout panel inside a `BarStaticItem`.

## Source Material

- `articles/controls-and-libraries/ribbon-bars-and-menu/bars/bar-layout/standalone-bar-controls.md` (`xref:115355`)
- `articles/controls-and-libraries/ribbon-bars-and-menu/bars/bar-layout/bar-manager.md` (`xref:115354`)
- `articles/controls-and-libraries/ribbon-bars-and-menu/bars/bar-layout/bar-containers.md` (`xref:6556`)
- `articles/controls-and-libraries/ribbon-bars-and-menu/bars/bar-layout/bars.md`

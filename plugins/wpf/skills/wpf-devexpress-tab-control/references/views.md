# Views — MultiLine, Scroll, Stretch

`DXTabControl`'s layout and behavior are controlled by a **View** object assigned to `DXTabControl.View`. Three views are available, each with different overflow behavior — what happens when there are more tab headers than fit in the header panel.

## When to Use This Reference

Use this when you need to:

- Pick a view based on how header overflow should behave
- Position the header panel on a non-default edge (`HeaderLocation`)
- Configure scroll buttons, animations, header orientation
- Enable drag-drop reordering, tab pinning, pop-out windows (Stretch view only)

## Picker — Which View?

| View | Overflow behavior | Typical use |
|---|---|---|
| **`TabControlMultiLineView`** | Wrap into multiple lines; all headers visible | Settings dialogs, fixed UIs |
| **`TabControlScrollView`** | Single line with scroll buttons | Document workspaces, many-tab UIs |
| **`TabControlStretchView`** | Shrink tab widths; supports pin/drag-out | Browser-style tab UIs |

Default view: `TabControlScrollView`.

## Applying a View

```xaml
<dx:DXTabControl>
    <dx:DXTabControl.View>
        <dx:TabControlMultiLineView/>
    </dx:DXTabControl.View>
    ...
</dx:DXTabControl>
```

In code:

```csharp
tabControl.View = new TabControlMultiLineView();
```

## Common Functionality — `TabControlViewBase`

All views inherit from `TabControlViewBase`. Common members:

| Property | Use |
|---|---|
| `HeaderLocation` | Where the header panel sits — `Top` (default), `Bottom`, `Left`, `Right` |

```xaml
<dx:DXTabControl>
    <dx:DXTabControl.View>
        <dx:TabControlScrollView HeaderLocation="Left" HeaderOrientation="Vertical"/>
    </dx:DXTabControl.View>
    ...
</dx:DXTabControl>
```

Other view-wide features (Header Menu, Hide / New buttons) work in every view — they're documented in the `xref:7977` (Header Menu) and `xref:113904` (Adding and Removing Tab Items) topics.

## `TabControlMultiLineView` — Wrap into Multiple Lines

When tab headers exceed the available width, they wrap onto additional lines, so **all** headers stay visible.

```xaml
<dx:DXTabControl>
    <dx:DXTabControl.View>
        <dx:TabControlMultiLineView/>
    </dx:DXTabControl.View>
    ...
</dx:DXTabControl>
```

| Property | Use |
|---|---|
| `FixedHeaders` | `False` (default): the line containing the selected tab moves to the front. `True`: lines stay in fixed positions, the selected tab stays where it is. |

```xaml
<dx:TabControlMultiLineView FixedHeaders="True"/>
```

**Use it for**: settings dialogs, fixed-set UIs where users see all tabs at once and rapid line-reflow during selection is jarring.

**Avoid it for**: document workspaces (too many lines waste vertical space).

## `TabControlScrollView` — Single Line with Scroll Buttons

Default view. Headers stay in a single row; scroll buttons appear when overflow happens.

```xaml
<dx:TabControlScrollView ScrollButtonShowMode="AutoHideBothButtons"
                         AllowAnimation="True"
                         AllowScrollOnMouseWheel="True"/>
```

### Key Properties

| Property | Use |
|---|---|
| `ScrollButtonShowMode` | `Always`, `Never`, `AutoHideBothButtons`, `AutoHideEachButton` |
| `AllowScrollOnMouseWheel` | Mouse wheel scrolls the header strip |
| `AllowAnimation` | Smooth scroll animation |
| `HeaderAutoFill` | When `True`, tabs stretch to fill available header width |
| `HeaderOrientation` | `Horizontal` (default) or `Vertical` (good with `HeaderLocation="Left"` / `"Right"`) |

### `ScrollButtonShowMode` Values

| Mode | Behavior |
|---|---|
| `Always` | Buttons always visible |
| `Never` | Hidden, even on overflow |
| `AutoHideBothButtons` | Both appear together when overflow happens |
| `AutoHideEachButton` | Each appears/disappears independently based on whether you can scroll that direction |

### Stretched Tabs in Scroll View

```xaml
<dx:TabControlScrollView HeaderAutoFill="True"/>
```

Tabs stretch evenly across the header panel (no scrolling needed when totals fit).

### Vertical Header Orientation

```xaml
<dx:DXTabControl>
    <dx:DXTabControl.View>
        <dx:TabControlScrollView HeaderLocation="Left"
                                 HeaderOrientation="Vertical"/>
    </dx:DXTabControl.View>
    ...
</dx:DXTabControl>
```

Tabs run vertically down the left edge — useful for settings-style apps with many sections.

### Programmatic Scrolling

| Method | Effect |
|---|---|
| `ScrollNext()` / `ScrollPrev()` | Scroll one tab forward / back |
| `ScrollFirst()` / `ScrollLast()` | Jump to first / last |
| `ScrollToSelectedTabItem(animate)` | Scroll until the selected tab is visible |
| `CanScroll` / `CanScrollNext` / `CanScrollPrev` | Bind buttons' enabled state |

```csharp
((TabControlScrollView)tabControl.View).ScrollToSelectedTabItem(true);
```

**Use it for**: most general-purpose tab UIs — predictable behavior, no surprises.

## `TabControlStretchView` — Shrink, Pin, Drag-Drop

Headers stay in one row but **shrink** rather than scroll when overflow happens. Browser-style.

```xaml
<dx:TabControlStretchView TabNormalSize="120"
                          TabMinSize="32"
                          SelectedTabMinSize="80"
                          DragDropMode="All"/>
```

### Tab Sizes

| Property | Use |
|---|---|
| `TabNormalSize` | Preferred width when there's room |
| `TabMinSize` | Smallest a non-selected tab shrinks to |
| `SelectedTabMinSize` | Floor for the selected tab — it stays wider than others |

### Pinning

```xaml
<dx:DXTabItem Header="Pinned" dx:TabControlStretchView.PinMode="Pinned">...</dx:DXTabItem>
```

| Property | Use |
|---|---|
| `TabControlStretchView.PinMode` (attached) | `Unpinned` (default), `Pinned` |
| `PinnedTabSize` | Width of pinned tabs (typically narrower than `TabNormalSize`) |
| `PinnedTabAllowDrag` | Whether pinned tabs can be dragged |
| `PinnedTabAllowHide` | Whether pinned tabs show the close (×) button |

Pinned tabs stay on the left, unaffected by scroll/shrink of other tabs.

### Drag and Drop

```xaml
<dx:TabControlStretchView DragDropMode="All" DragDropRegion="Workspace"/>
```

| `DragDropMode` | What's allowed |
|---|---|
| `None` (default) | No drag-drop |
| `Reorder` | Reorder tabs within the same control |
| `DragDrop` | Drag tabs between different `DXTabControl`s |
| `OutsideWindow` | Drag a tab outside any tab control to create a new pop-out window |
| `All` | Reorder + DragDrop + OutsideWindow |

`DragDropRegion` (string) gates drag-drop between controls — only controls with matching `DragDropRegion` exchange tabs. Defaults to empty (allowed between any).

| Property | Use |
|---|---|
| `NewWindowStyle` | Style for the pop-out window |
| `NewTabControlStyle` | Style for the tab control inside the pop-out window |

`DXTabControl` exposes `TabControlNewTabbedWindow` / `TabControlNewTabbedWindowEventArgs` events to customize the pop-out behavior.

**Use it for**: browser-like UIs, document workspaces with rich tab management.

## Examples

### Browser-Style with Pinning + Drag-Drop

```xaml
<dx:DXTabControl ItemsSource="{Binding Tabs}">
    <dx:DXTabControl.View>
        <dx:TabControlStretchView
                TabNormalSize="140"
                TabMinSize="40"
                SelectedTabMinSize="100"
                DragDropMode="All"
                PinnedTabSize="32"
                PinnedTabAllowHide="False"/>
    </dx:DXTabControl.View>
    <dx:DXTabControl.ItemContainerStyle>
        <Style TargetType="dx:DXTabItem">
            <Setter Property="AllowHide" Value="True"/>
            <Setter Property="Header"    Value="{Binding Title}"/>
        </Style>
    </dx:DXTabControl.ItemContainerStyle>
    ...
</dx:DXTabControl>
```

### Vertical Side Tabs

```xaml
<dx:DXTabControl Width="600" Height="400">
    <dx:DXTabControl.View>
        <dx:TabControlScrollView HeaderLocation="Left"
                                 HeaderOrientation="Vertical"
                                 ScrollButtonShowMode="AutoHideBothButtons"/>
    </dx:DXTabControl.View>
    <dx:DXTabItem Header="Profile"      Glyph="{dx:DXImage User_16x16.png}"/>
    <dx:DXTabItem Header="Notifications" Glyph="{dx:DXImage Bell_16x16.png}"/>
    <dx:DXTabItem Header="Security"      Glyph="{dx:DXImage Lock_16x16.png}"/>
</dx:DXTabControl>
```

### MultiLine for a Settings Dialog

```xaml
<dx:DXTabControl Width="600">
    <dx:DXTabControl.View>
        <dx:TabControlMultiLineView FixedHeaders="True"/>
    </dx:DXTabControl.View>
    <dx:DXTabItem Header="General"/>
    <dx:DXTabItem Header="Appearance"/>
    <dx:DXTabItem Header="Editor"/>
    <dx:DXTabItem Header="Keyboard"/>
    <dx:DXTabItem Header="Extensions"/>
    <dx:DXTabItem Header="Privacy"/>
    <dx:DXTabItem Header="Advanced"/>
</dx:DXTabControl>
```

## Common Issues

- **Scroll buttons never appear** — `ScrollButtonShowMode="Never"`, or the panel has enough room for all tabs. Use `Always` to verify.
- **Drag-drop doesn't work** — drag-drop is **Stretch-view-only**; switch to `TabControlStretchView` and set `DragDropMode`.
- **Pinned tabs don't show** — `PinMode` is an attached property on `TabControlStretchView`, not on `DXTabItem`. Use `dx:TabControlStretchView.PinMode="Pinned"` on the item.
- **Vertical headers but tabs are still horizontal text** — set both `HeaderLocation="Left"` and `HeaderOrientation="Vertical"`; one without the other gives partial behavior.
- **`MultiLineView` line moves around on every selection** — default is `FixedHeaders="False"`. Set `FixedHeaders="True"` if the reordering is disorienting.
- **Pop-out window doesn't pick up the theme** — host window is `Window`, not `ThemedWindow`; pop-out clones the host's chrome.

## Source Material

- `articles/controls-and-libraries/layout-management/tab-control/fundamentals/views.md` (`xref:7979`)
- `articles/controls-and-libraries/layout-management/tab-control/fundamentals/views/views-overview.md` (`xref:113984`)
- `articles/controls-and-libraries/layout-management/tab-control/fundamentals/views/multiline-view.md` (`xref:113875`)
- `articles/controls-and-libraries/layout-management/tab-control/fundamentals/views/scroll-view.md` (`xref:113876`)
- `articles/controls-and-libraries/layout-management/tab-control/fundamentals/views/stretch-view.md` (`xref:113877`)
- `articles/controls-and-libraries/layout-management/tab-control/concepts/header-menu.md` (`xref:7977`)
- `articles/controls-and-libraries/layout-management/tab-control/concepts/adding-and-removing-tab-items.md` (`xref:113904`)

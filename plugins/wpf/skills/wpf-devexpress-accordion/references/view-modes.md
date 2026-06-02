# View Modes

`AccordionControl` has two distinct rendering modes — **Default** (classic accordion: root items stack vertically, expanding to show children inline) and **NavigationPane** (Outlook-style: root items appear as a tab strip, the selected root's children fill the rest of the pane). On top of these, the **Collapse Mode** orthogonally compacts the whole control into a strip with glyph-only items. This page covers all three concepts plus the `OfficeNavigationBar` integration.

## When to Use This Reference

Use this when you need to:

- Switch between Default and NavigationPane modes (`ViewMode`)
- Integrate with `OfficeNavigationBar` (Outlook-2013-style)
- Configure Compact Navigation (`CompactNavigation`)
- Use the Peek Form (hover preview in NavigationPane)
- Collapse the whole panel into a glyph-only strip (`IsCollapseButtonVisible`)
- Show items in collapsed mode (`ShowInCollapsedMode`)
- Display a popup on hover when collapsed

## ViewMode — Default vs NavigationPane

```xaml
<dxa:AccordionControl ViewMode="NavigationPane">
    ...
</dxa:AccordionControl>
```

| `AccordionViewMode` | Behavior |
|---|---|
| `Default` (default) | Classic accordion. All root items stack vertically. Expanding one shows its children inline. |
| `NavigationPane` | Outlook-style. Root items appear as a horizontal tab strip at the bottom; the selected root's children fill the rest of the panel. |

### NavigationPane Mode

- **Only one root item's children are shown at a time** — selected via the strip at the bottom, or programmatically via `SelectedRootItem`.
- Root items the strip can't fit collapse into an **overflow panel** (the `...` menu).
- When the user clicks the accordion's collapse button (or `IsExpanded="False"`), the overflow panel switches to vertical orientation along the side.

```xaml
<dxa:AccordionControl ViewMode="NavigationPane"
                      SelectedRootItem="{Binding CurrentSection, Mode=TwoWay}">
    <dxa:AccordionItem Header="Mail" Glyph="..."/>
    <dxa:AccordionItem Header="Calendar" Glyph="..."/>
    <dxa:AccordionItem Header="People" Glyph="..."/>
</dxa:AccordionControl>
```

Set `SelectedRootItem` (not `SelectedItem`) to drive the active section from the ViewModel.

### When to Pick Each

| Use Default when | Use NavigationPane when |
|---|---|
| Multiple categories should be visible/expandable at once | Switching between top-level sections (Mail / Calendar / People) is the primary navigation |
| Settings tree, properties panel | Outlook-like app, line-of-business app sections |
| Hierarchy is more than 2 levels deep | Hierarchy is shallow (root + children) |

## Integration with OfficeNavigationBar

The `OfficeNavigationBar` (the Outlook 2013+ tab strip at the bottom) pairs with `AccordionControl` in NavigationPane mode:

```xaml
<DockPanel>
    <dxn:OfficeNavigationBar DockPanel.Dock="Bottom"
                             AccordionControl="{Binding ElementName=accordion}"/>
    <dxa:AccordionControl x:Name="accordion" ViewMode="NavigationPane">
        ...
    </dxa:AccordionControl>
</DockPanel>
```

(`xmlns:dxn="http://schemas.devexpress.com/winfx/2008/xaml/navigation"`)

- The accordion **auto-switches to NavigationPane mode** when linked to an `OfficeNavigationBar`.
- The bar shows root items as tabs; the accordion shows the selected root's children.

### Compact Navigation

`AccordionControl.CompactNavigation` toggles between two NavigationPane layouts:

| `CompactNavigation` | Effect |
|---|---|
| `false` (default) | The `OfficeNavigationBar` is visible; root items appear as tabs in the bar. |
| `true` | The `OfficeNavigationBar` is **hidden**, replaced by the accordion's own **Overflow Panel** at the bottom of the panel — a more compact look. |

```xaml
<dxa:AccordionControl ViewMode="NavigationPane" CompactNavigation="True">
    ...
</dxa:AccordionControl>
```

End users can toggle this from the `OfficeNavigationBar`'s **Navigation Options** menu via the **Compact Navigation** check box (shown only when the nav bar is linked to an accordion).

### Peek Form

In NavigationPane mode, hovering over a root item's tab shows a **Peek Form** popup — useful for quick previews (recent emails, today's calendar) without switching sections.

```xaml
<dxa:AccordionItem Header="Calendar" Glyph="...">
    <dxa:AccordionItem.PeekFormTemplate>
        <DataTemplate>
            <StackPanel Width="240" Margin="8">
                <TextBlock Text="Today" FontWeight="Bold"/>
                <TextBlock Text="10:00 — Daily standup"/>
                <TextBlock Text="14:30 — Design review"/>
            </StackPanel>
        </DataTemplate>
    </dxa:AccordionItem.PeekFormTemplate>
</dxa:AccordionItem>
```

## Collapse Mode (Orthogonal to ViewMode)

Collapse mode is a **separate concept** from view mode. It turns the whole panel into a compact strip — useful for sidebars that should optionally shrink to icons only. Works in both `Default` and `NavigationPane` view modes.

### Enable the Collapse Button

```xaml
<dxa:AccordionControl IsCollapseButtonVisible="True">
    ...
</dxa:AccordionControl>
```

Users click the button to collapse / expand the panel. Programmatic toggle:

```xaml
<dxa:AccordionControl IsExpanded="False">
    ...
</dxa:AccordionControl>
```

When collapsed, the panel shows:

1. The **summary item** (a generic "menu" entry that opens a popup with all items)
2. Optionally, individual items that opted in via `ShowInCollapsedMode="True"`

### Summary Item

```xaml
<dxa:AccordionControl IsCollapseButtonVisible="True"
                      SummaryItemHeader="Items"
                      SummaryItemGlyph="{dx:DXImage Image=Technology_16x16.png}"
                      SummaryItemPosition="Top">
    ...
</dxa:AccordionControl>
```

| Property | Use |
|---|---|
| `SummaryItemHeader` | Caption shown on the summary entry |
| `SummaryItemGlyph` | Icon shown on the summary entry |
| `SummaryItemPosition` | `Top` or `Bottom` placement |

Clicking the summary entry opens a popup with the full menu.

### Show Specific Items When Collapsed

```xaml
<dxa:AccordionControl IsCollapseButtonVisible="True">
    <dxa:AccordionItem Header="Mail" Glyph="..." ShowInCollapsedMode="True">
        <dxa:AccordionItem Header="Inbox"/>
        <dxa:AccordionItem Header="Drafts"/>
    </dxa:AccordionItem>
    <dxa:AccordionItem Header="Settings" Glyph="..."/>  <!-- not shown when collapsed -->
</dxa:AccordionControl>
```

Items with `ShowInCollapsedMode="True"` appear directly in the collapsed strip; others are reachable only via the summary popup.

### Glyph-Only Collapsed Look

```xaml
<dxa:AccordionControl IsCollapseButtonVisible="True"
                      CollapsedItemDisplayMode="Glyph">
    ...
</dxa:AccordionControl>
```

`CollapsedItemDisplayMode` controls what's shown for each collapsed-mode item:

| Value | Effect |
|---|---|
| `Default` (default) | Header text + glyph |
| `Glyph` | Glyph only (hamburger-bar-style) |
| `Header` | Header only |

### Show Popup on Hover

```xaml
<dxa:AccordionControl IsCollapseButtonVisible="True"
                      ShowPopupOnHover="True"
                      PopupHideDelay="0:0:0.5">
    ...
</dxa:AccordionControl>
```

When `ShowPopupOnHover="True"`, hovering an item in the collapsed strip opens its child popup (no click required). `PopupHideDelay` controls how long the popup stays after the mouse leaves; if the user starts interacting with the popup contents, it stays open.

## Common Patterns

### Hamburger-Style Collapsible Sidebar

```xaml
<dxa:AccordionControl IsCollapseButtonVisible="True"
                      IsExpanded="{Binding IsMenuExpanded, Mode=TwoWay}"
                      CollapsedItemDisplayMode="Glyph"
                      ShowPopupOnHover="True">
    <dxa:AccordionItem Header="Home"     Glyph="..." ShowInCollapsedMode="True"/>
    <dxa:AccordionItem Header="Reports"  Glyph="..." ShowInCollapsedMode="True">
        <dxa:AccordionItem Header="Sales"/>
        <dxa:AccordionItem Header="Inventory"/>
    </dxa:AccordionItem>
    <dxa:AccordionItem Header="Settings" Glyph="..." ShowInCollapsedMode="True"/>
</dxa:AccordionControl>
```

Bind `IsExpanded` to a ViewModel boolean to programmatically expand / collapse (e.g., on window resize, on hotkey).

### Outlook-Style Pane with Compact Navigation

```xaml
<DockPanel>
    <dxn:OfficeNavigationBar DockPanel.Dock="Bottom"
                             AccordionControl="{Binding ElementName=acc}"/>
    <dxa:AccordionControl x:Name="acc"
                          ViewMode="NavigationPane"
                          CompactNavigation="True"
                          SelectedRootItem="{Binding ActiveSection, Mode=TwoWay}">
        <dxa:AccordionItem Header="Mail">
            <dxa:AccordionItem Header="Inbox"/>
            <dxa:AccordionItem Header="Sent"/>
        </dxa:AccordionItem>
        <dxa:AccordionItem Header="Calendar">
            <dxa:AccordionItem Header="Today"/>
            <dxa:AccordionItem Header="This Week"/>
        </dxa:AccordionItem>
    </dxa:AccordionControl>
</DockPanel>
```

## Common Issues

- **`SelectedItem` bind doesn't change the active section in NavigationPane** — use `SelectedRootItem`, not `SelectedItem`. Root selection is a separate property in NavigationPane mode.
- **Collapse button doesn't appear** — `IsCollapseButtonVisible` defaults to `false`. Set it to `true`.
- **Items disappear when collapsed** — they need `ShowInCollapsedMode="True"` to appear in the collapsed strip. Items without it are reachable only via the summary popup.
- **Hovering items doesn't show a popup** — `ShowPopupOnHover` defaults to `false`. Set it to `true` (only meaningful when collapsed).
- **NavigationPane shows children of multiple roots** — that's `Default` view mode behavior. NavigationPane shows only the selected root's children. Verify `ViewMode="NavigationPane"`.
- **`OfficeNavigationBar` link doesn't switch the view mode** — the accordion auto-switches, but verify `AccordionControl` is set on the nav bar (it's the `AccordionControl` property, not the binding direction).
- **Peek Form doesn't appear** — Peek Form requires NavigationPane mode and a defined `PeekFormTemplate`. Both must be present.

## Source Material

- `articles/controls-and-libraries/navigation-controls/accordion-control/view-mode.md` (`xref:120285`)
- `articles/controls-and-libraries/navigation-controls/accordion-control/collapse-mode.md` (`xref:119732`)
- `articles/controls-and-libraries/navigation-controls/accordion-control/expanding-and-collapsing.md` (`xref:119808`)

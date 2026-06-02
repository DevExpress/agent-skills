# AccordionItem Settings and Customization

`AccordionItem` is the single element class used for both **root items** (top-level entries) and **subitems** (nested children). It has three customizable regions — **Header**, **Glyph**, and **Content** — plus a set of state and behavior properties (`ItemDisplayMode`, `ExpandButtonPosition`, animation, selection). This page covers what's configurable through properties and templates.

## When to Use This Reference

Use this when you need to:

- Customize an item's caption (`Header`) — text, custom UI, multi-line layouts
- Set or template the glyph (`Glyph`, `GlyphTemplate`, `GlyphPosition`)
- Embed editors / images / panels as item content
- Control expand-button placement and click behavior
- Configure per-item or per-control animation
- Hide or replace items via `ItemDisplayMode`

## AccordionItem Anatomy

Each item has three regions:

- **Header** — the caption area: text by default, but accepts any `UIElement`.
- **Glyph** — an icon, typically next to or above the header.
- **Nested content** — children placed directly inside the item element. Use sub-`AccordionItem` elements for menu hierarchy, or any other `UIElement` (editors, panels) for an inline body.
- **Expand-Collapse button** — the arrow that toggles children visibility.

## Header Customization

### Plain Text Caption

```xaml
<dxa:AccordionItem Header="Root Item">
    <dxa:AccordionItem Header="SubItem"/>
</dxa:AccordionItem>
```

### Custom UI in the Header

`Header` accepts any object:

```xaml
<dxa:AccordionItem>
    <dxa:AccordionItem.Header>
        <StackPanel Orientation="Horizontal">
            <Label Foreground="Red">Sub Item</Label>
            <dxe:SpinEdit Margin="20,0" Value="1"/>
        </StackPanel>
    </dxa:AccordionItem.Header>
</dxa:AccordionItem>
```

The header can host editors, badges, status indicators — anything WPF can render.

### Header as Whole Item

Setting `ItemDisplayMode="Header"` replaces the entire item with the `Header` content — the glyph, expand button, and item margins are dropped. Use this when the "item" is really just a custom UI element disguised as a menu entry:

```xaml
<dxa:AccordionItem ItemDisplayMode="Header">
    <dxa:AccordionItem.Header>
        <Border Background="LightCoral" Padding="8">
            <TextBlock Text="Custom Section Divider"/>
        </Border>
    </dxa:AccordionItem.Header>
</dxa:AccordionItem>
```

## Glyph Customization

### Specify a Glyph

```xaml
<dxa:AccordionItem Header="Root Item"
                   Glyph="{dx:DXImage Image=Image_32x32.png}">
    <dxa:AccordionItem Header="SubItem"
                       Glyph="{dx:DXImage Image=Map_16x16.png}"/>
</dxa:AccordionItem>
```

Use the `{dx:DXImage Image=...}` markup extension to reference the bundled DevExpress images, or `{dx:DXImage SvgImages/.../File.svg}` for SVG icons. Custom images: assign any `ImageSource`.

### Glyph Position

`AccordionItem.GlyphPosition` overrides the control-wide `AccordionControl.ItemGlyphPosition`:

```xaml
<dxa:AccordionItem Header="Root Item"
                   Glyph="{dx:DXImage Image=Walking_16x16.png}"
                   GlyphPosition="Right">
    ...
</dxa:AccordionItem>
```

| `GlyphPosition` | Where the icon renders |
|---|---|
| `Left` (default) | Before the header text |
| `Top` | Above the header text |
| `Right` | After the header text |
| `Bottom` | Below the header text |

To set it for all items at once: `AccordionControl.ItemGlyphPosition`.

### Custom Glyph Template

For non-trivial glyph rendering (border, badge, overlay):

```xaml
<dxa:AccordionItem Header="Root Item"
                   Glyph="{dx:DXImage Image=Walking_16x16.png}">
    <dxa:AccordionItem.GlyphTemplate>
        <DataTemplate>
            <Border BorderThickness="1" BorderBrush="Black" Background="LightYellow">
                <Image Source="{Binding}" Margin="5"/>
            </Border>
        </DataTemplate>
    </dxa:AccordionItem.GlyphTemplate>
</dxa:AccordionItem>
```

The `Binding` in the template resolves to the `Glyph` property value. If you have multiple templates depending on item state, set `GlyphTemplateSelector` to a `DataTemplateSelector`.

## Item Content

`AccordionItem` derives from `TreeViewItem`. Nested elements inside the item tag form the item's content — both sub-items and arbitrary UI work:

```xaml
<dxa:AccordionItem Header="Volume" Glyph="...">
    <dxe:TrackBarEdit Minimum="0" Maximum="100" EditValue="{Binding Volume}"/>
</dxa:AccordionItem>
```

Editors, charts, custom panels — anything that fits the panel width works inside an `AccordionItem`.

### ItemDisplayMode

Controls how the item is rendered:

| `ItemDisplayMode` | Effect |
|---|---|
| `Default` (default) | Header + Glyph + expand button + nested children |
| `Header` | Replace the whole item with the `Header` content (no glyph, no margin) |

To hide an item, use `ItemVisibilityMode` (values: `Visible`, `Collapsed`, `ShowSubItems`).

## Expand / Collapse Behavior

### Expand on Click

By default, clicking anywhere on the header expands / collapses the item. To require clicking only the expand-collapse arrow:

```xaml
<dxa:AccordionControl ExpandItemOnHeaderClick="False">
    ...
</dxa:AccordionControl>
```

### Expand-Button Position

| Property | Scope |
|---|---|
| `AccordionItem.ExpandButtonPosition` | Per-item override |
| `AccordionControl.RootItemExpandButtonPosition` | All root items |
| `AccordionControl.SubItemExpandButtonPosition` | All subitems |

Values: `Left`, `Right`, `Hidden`.

```xaml
<dxa:AccordionControl RootItemExpandButtonPosition="Left"
                      SubItemExpandButtonPosition="Right">
    ...
</dxa:AccordionControl>
```

### Expand Modes (Control-Wide)

`AccordionControl.ExpandMode` decides how many items can be open at the same time:

| `ExpandMode` | Behavior |
|---|---|
| `Multiple` (default) | Multiple items can be expanded. At least one stays expanded. |
| `MultipleOrNone` | Multiple items can be expanded, or all collapsed. |
| `Single` | One item is always expanded; collapsing the active one isn't allowed. |
| `SingleOrNone` | One item at a time, or all collapsed. |

```xaml
<dxa:AccordionControl ExpandMode="SingleOrNone">
    ...
</dxa:AccordionControl>
```

### Programmatic Expand / Collapse

```csharp
accordion.ExpandItem(item);   // Expand a specific item
accordion.CollapseItem(item); // Collapse it
accordion.ExpandAll();
accordion.CollapseAll();
```

Or bind `AccordionItem.IsExpanded`:

```xaml
<dxa:AccordionItem Header="Settings"
                   IsExpanded="{Binding ShowSettings, Mode=TwoWay}"/>
```

### Animation

Default: expand / collapse animates smoothly. To disable:

```xaml
<dxa:AccordionControl AllowAnimation="False"/>
```

Or per item:

```xaml
<dxa:AccordionItem AllowAnimation="False"/>
```

## Selection

`AccordionItem.IsSelected` is bindable; `AccordionControl.SelectedItem` exposes the currently selected item.

```xaml
<dxa:AccordionControl SelectedItem="{Binding CurrentSection, Mode=TwoWay}"
                      ItemsSource="{Binding Sections}"
                      DisplayMemberPath="Name"/>
```

When binding to data, the `SelectedItem` is the underlying data object (e.g., `Section`), not the `AccordionItem` wrapper.

For NavigationPane view mode, use `SelectedRootItem` — see [view-modes.md](view-modes.md).

## Per-Item Properties Quick Reference

| Property | Use |
|---|---|
| `Header` | Caption text or `UIElement` |
| `Glyph` | Icon (`ImageSource`) |
| `GlyphTemplate` / `GlyphTemplateSelector` | Custom glyph rendering |
| `GlyphPosition` | `Left`, `Top`, `Right`, `Bottom` |
| `Items` | Child items (or any nested UI) |
| `ItemDisplayMode` | `Default` or `Header` |
| `ItemVisibilityMode` | `Visible`, `Collapsed`, `ShowSubItems` |
| `IsExpanded` | Expand state (bindable; inherited from `TreeViewItem`) |
| `IsSelected` | Selection state (bindable; inherited from `TreeViewItem`) |
| `ItemLevel` (read-only) | Zero-based depth (`0` = root) |
| `ExpandButtonPosition` | Per-item arrow placement |
| `AllowAnimation` | Per-item animation toggle |
| `SearchTag` | Extra text included in the search index — see [search.md](search.md) |
| `ShowInCollapsedMode` | Show in the collapsed strip (see [view-modes.md](view-modes.md)) |
| `PeekFormTemplate` | Hover preview content (NavigationPane mode) |
| `Click` | Click event |

## Common Patterns

### Item with Editor Inside

```xaml
<dxa:AccordionItem Header="Volume" Glyph="{dx:DXImage SvgImages/Audio/Audio_16x16.svg}">
    <dxe:TrackBarEdit Minimum="0" Maximum="100"
                      EditValue="{Binding Volume, Mode=TwoWay}"/>
</dxa:AccordionItem>
```

### Item with Click Action (No Children)

```xaml
<dxa:AccordionItem Header="Refresh"
                   Glyph="{dx:DXImage SvgImages/Actions/Refresh.svg}"
                   Click="OnRefreshClick"/>
```

Leaf items (no children) can act like buttons. Combine with `Click` event or bind `IsSelected` to a command pattern.

### Item with Multi-Line Header

```xaml
<dxa:AccordionItem>
    <dxa:AccordionItem.Header>
        <StackPanel>
            <TextBlock Text="Notifications" FontWeight="Bold"/>
            <TextBlock Text="3 new alerts" FontSize="11" Foreground="Gray"/>
        </StackPanel>
    </dxa:AccordionItem.Header>
</dxa:AccordionItem>
```

### Glyph with Badge

```xaml
<dxa:AccordionItem Header="Inbox" Glyph="{dx:DXImage SvgImages/Mail/Inbox.svg}">
    <dxa:AccordionItem.GlyphTemplate>
        <DataTemplate>
            <Grid>
                <Image Source="{Binding}"/>
                <TextBlock Text="3" Background="Red" Foreground="White"
                           HorizontalAlignment="Right" VerticalAlignment="Top"
                           Padding="3,0" FontSize="10"/>
            </Grid>
        </DataTemplate>
    </dxa:AccordionItem.GlyphTemplate>
</dxa:AccordionItem>
```

## Common Issues

- **Custom header is rendered as text, not UI** — `Header` was set to a string; use `<dxa:AccordionItem.Header>` element syntax for a UI element.
- **Glyph doesn't show** — wrong source. Use `{dx:DXImage Image=...png}`, `{dx:DXImage SvgImages/...svg}`, or a packed `Uri`. Verify the image exists.
- **`ItemDisplayMode="Header"` loses the icon** — by design. `Header` mode replaces the whole item; the glyph won't render. Put it inside the header content if you want it back.
- **`GlyphPosition` ignored** — the control might have an explicit `ItemGlyphPosition` overriding it. Per-item beats per-control, but verify nothing else is interfering.
- **Item doesn't expand on header click** — `AccordionControl.ExpandItemOnHeaderClick="False"`. Set to `True` (default), or click the arrow.
- **Embedded editor doesn't bind** — `DataContext` of the item is its underlying data object (when bound), not the window's DataContext. For ViewModel commands, bind through `RelativeSource` or use the `dx:` MVVM helpers.

## Source Material

- `articles/controls-and-libraries/navigation-controls/accordion-control/accordion-items.md` (`xref:118548`)
- `articles/controls-and-libraries/navigation-controls/accordion-control/accordion-items/adding-accordion-items.md` (`xref:119831`)
- `articles/controls-and-libraries/navigation-controls/accordion-control/accordion-items/item-header-customization.md` (`xref:119847`)
- `articles/controls-and-libraries/navigation-controls/accordion-control/accordion-items/item-glyph-customization.md` (`xref:119848`)
- `articles/controls-and-libraries/navigation-controls/accordion-control/accordion-items/item-content-customization.md` (`xref:119849`)
- `articles/controls-and-libraries/navigation-controls/accordion-control/expanding-and-collapsing.md` (`xref:119808`)
- `articles/controls-and-libraries/navigation-controls/accordion-control/selection.md` (`xref:118345`)

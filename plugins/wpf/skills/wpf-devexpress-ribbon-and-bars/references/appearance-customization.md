# Appearance Customization

This page distinguishes **what can** be customized via standard WPF properties / triggers vs **what requires template overrides** (or isn't customizable at all on `BarItem` directly). Understanding this split is important — many DevExpress bar/ribbon visuals are rendered by the `BarItemLink` element, not the `BarItem` itself, so a `<Style TargetType="dxb:BarButtonItem">` with property setters can silently have no effect.

## When to Use This Reference

Use this when you need to:

- Change colors, fonts, padding, or borders of bar items / ribbon groups
- Apply a state-conditional style (hover, pressed, disabled)
- Switch the Ribbon paint style (Office 2019, Office Slim, etc.)
- Understand why a `<Style>` with setters has no effect on a bar item
- Decide between property customization vs template override

## What You CAN Customize via Properties

The following standard WPF properties **work directly** on bar items, links, and ribbon elements:

- `Background`
- `BorderBrush`
- `BorderThickness`
- `CornerRadius` (DevExpress-specific, but property-settable)
- `Foreground`
- `Margin`
- `Padding`

```xaml
<dxb:BarButtonItem Content="Save"
                   Background="#FFE3F2FD"
                   Foreground="#0B61C9"
                   BorderBrush="#0B61C9"
                   BorderThickness="1"
                   Padding="6,2"/>
```

These apply at the **link** rendering: when the same item is shown in two locations, both links use these values unless overridden.

## State-Based Conditional Styling — `BarItem.Triggers`

**Use `BarItem.Triggers`, not `Style.Triggers`.** Standard WPF triggers attached via `<Style TargetType="dxb:BarButtonItem"><Style.Triggers>...</Style.Triggers></Style>` do NOT propagate to the rendered `BarItemLink` — they evaluate against the item, not the visual element.

```xaml
<dxb:BarButtonItem Content="Delete"
                   Glyph="{dx:DXImage Image=Delete_16x16.png}">
    <dxb:BarItem.Triggers>
        <Trigger Property="IsEnabled" Value="False">
            <Setter Property="Foreground" Value="Gray"/>
            <Setter Property="Opacity" Value="0.5"/>
        </Trigger>
    </dxb:BarItem.Triggers>
</dxb:BarButtonItem>
```

The `BarItem.Triggers` collection is evaluated **per rendered link** — so the trigger fires correctly even when the item appears in multiple bars.

> If the agent tries to set per-state colors via a `<Style.Triggers>` and you see "nothing changes when hovered," the cause is almost always this distinction.

## What You CANNOT Customize via Properties or Triggers

The following are **NOT** controllable via property setters or triggers on `BarItem` / `BarItemLink`:

| What | Why | Workaround |
|---|---|---|
| Internal element layout (e.g., glyph position relative to caption beyond `GlyphAlignment`) | Rendered by the control template | Override the `ControlTemplate` on the bar item / link |
| Ribbon page header look (separator, hover line color, etc.) | Rendered by the Ribbon template per-style | Switch `RibbonStyle`, or override `NormalPageCaptionTextStyle` / `SelectedPageCaptionTextStyle` / `HoverPageCaptionTextStyle` |
| Application Menu inner panes / Backstage View navigation | Rendered by the menu template | Override the `ApplicationMenu` / `BackstageViewControl` template |
| Tooltip rendering (beyond `ToolTip` content) | Tooltip uses its own template | Override `BarItem.ToolTipTemplate` or define a custom WPF tooltip |
| Dropdown popup chrome (popup background, border behind a `BarSplitButtonItem`'s dropdown) | Rendered by `PopupMenu` template | Override the `PopupMenu` style |
| Animations (page slide-in, hover ripple) | Built into theme | Switch theme; not property-customizable |

For these, you need to override the **`ControlTemplate`** of the element, or override a Ribbon-level style property (the table below).

### Ribbon-Specific Style Overrides

These give you control over Ribbon-rendered text and chrome **without** rewriting templates:

| Visual element | Style property |
|---|---|
| Ribbon page content border | `RibbonPage.ContentBorderStyle` |
| Ribbon page group header | `RibbonPageGroup.HeaderBorderStyle` |
| Normal page caption | `RibbonControl.NormalPageCaptionTextStyle` |
| Selected page caption | `RibbonControl.SelectedPageCaptionTextStyle` |
| Hovered page caption | `RibbonControl.HoverPageCaptionTextStyle` |

Example — bold selected page caption:

```xaml
<dxr:RibbonControl>
    <dxr:RibbonControl.SelectedPageCaptionTextStyle>
        <Style TargetType="TextBlock">
            <Setter Property="FontWeight" Value="Bold"/>
        </Style>
    </dxr:RibbonControl.SelectedPageCaptionTextStyle>
    ...
</dxr:RibbonControl>
```

## BarSplitButtonItem — Separate Arrow Appearance

`BarSplitButtonItem` has its caption-button part (inheriting standard `BarItem` appearance) AND a separate arrow part with its own brushes:

| Caption part | Arrow part |
|---|---|
| `Background`, `Foreground`, `BorderBrush`, etc. | `ArrowBackground`, `ArrowForeground`, `ArrowBorderBrush`, `ArrowBorderThickness`, `ArrowCornerRadius`, `ArrowPadding`, `ArrowAlignment` |

```xaml
<dxb:BarSplitButtonItem Content="Save"
                        Background="LightBlue"
                        ArrowBackground="SteelBlue"
                        ArrowForeground="White">
    <dxb:BarSplitButtonItem.PopupControl>
        <dxb:PopupMenu>
            <dxb:BarButtonItem Content="Save As..." Command="{Binding SaveAsCommand}"/>
        </dxb:PopupMenu>
    </dxb:BarSplitButtonItem.PopupControl>
</dxb:BarSplitButtonItem>
```

## Ribbon Paint Styles (Coarse-Grained)

Before reaching for template overrides, switch the `RibbonStyle` — it changes the whole rendering language at once:

| `RibbonStyle` | Look |
|---|---|
| `Office2019` | Office 2019 (default for new apps) |
| `Office2010` | Office 2010 (pair with `BackstageViewControl`) |
| `Office2007` | Office 2007 (pair with `ApplicationMenu`) |
| `OfficeSlim` | Single-line "Office Universal" / Windows 10 inspired |
| `TabletOffice` | Office for iPad |

You can also override the paint style on individual items:

- `BarItem.RibbonStyle` — `SmallWithText`, `SmallWithoutText`, `Large`, `Default`
- `BarItemLinkBase.RibbonStyle` — overrides the item's style for *this specific link*

If `BarItemLinkBase.RibbonStyle = Default`, the link inherits from `BarItem.RibbonStyle`. Otherwise, the link wins.

## Practical Decision Tree

When the agent receives "customize this bar item's look":

1. Wanted change is a **color / font / border / padding** → set `Background` / `Foreground` / `BorderBrush` / `Padding` directly on the item or link.
2. Change should react to **state (hover/pressed/checked/disabled/IsEnabled)** → use `BarItem.Triggers` (NOT `Style.Triggers`).
3. Change should react to **WPF visual state** (focus, mouseover with a custom brush) → override the `ControlTemplate` (not in scope here).
4. Wanted change is a **Ribbon page caption / selected tab look** → override one of the Ribbon's `*PageCaptionTextStyle` properties.
5. Wanted change is the **overall Office look** (chrome, ribbon corners, dropdown shadow) → switch `RibbonStyle`.
6. Wanted change is **layout/structure** inside a bar item (e.g., put the glyph below the caption with custom spacing) → override `ControlTemplate` on the bar item.

## Common Issues

- **`<Style TargetType="dxb:BarButtonItem">` setters or triggers do nothing** → standard WPF styles target the item class, but visuals come from the link. Use direct property setters on the item, or use `BarItem.Triggers` for state-based logic.
- **Ribbon tab header colors won't change** → use `NormalPageCaptionTextStyle` / `SelectedPageCaptionTextStyle` / `HoverPageCaptionTextStyle` on the `RibbonControl`. Setting `Foreground` on a `RibbonPage` doesn't affect its tab header.
- **Background change ignored on a sub-menu popup** → popup is rendered by `PopupMenu`, not by the parent `BarSubItem`. Style the `PopupMenu` or override its template.
- **Disabled state doesn't dim the glyph as expected** → most themes already dim disabled glyphs; verify `IsEnabled` is genuinely false, then adjust via `BarItem.Triggers` with an `IsEnabled` trigger.
- **`ArrowBackground` ignored on a regular `BarButtonItem`** — that property exists only on `BarSplitButtonItem`. Switch the item type if you need a split-button arrow.

## Source Material

- `articles/controls-and-libraries/ribbon-bars-and-menu/common-concepts/appearance-customization.md` (`xref:401859`)

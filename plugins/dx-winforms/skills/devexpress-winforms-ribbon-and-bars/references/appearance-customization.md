# Appearance Customization

This reference covers how to change the look of `RibbonControl` and `BarManager` — the global skin, the ribbon style family (Office 2007 → Office 2019 → Office Universal), the per-element `Appearance*` properties, the form/app-scope `BarAndDockingController`s, and — crucially — **what cannot be re-templated via simple properties** and requires `CustomDraw*` events.

Unlike WPF, WinForms controls do not expose XAML control templates and triggers. The skin engine *is* the template system; per-element overrides happen via `AppearanceObject` properties (fonts, colors, gradients) and the `CustomDraw*` events. Setting a property that the active skin owner-draws often has no effect — knowing that boundary is the key insight in this topic.

## When to Use This Reference

- Picking a `RibbonControl.RibbonStyle` (the visual family — Office 2007/2010/2013/Tablet/2019/Universal).
- Applying a skin and a color palette.
- Tweaking per-element appearance (`BackColor`, `ForeColor`, `Font`, `BorderColor`).
- Restyling the application across all bars/ribbons via `DefaultBarAndDockingController`.
- Re-coloring contextual page categories.
- Recognizing when a property is ignored because the skin owner-draws the area, and switching to `CustomDraw*` events.

## Three Layers of Customization

```text
1. Skin  (UserLookAndFeel / SkinManager)          — paint everything
2. RibbonControl.RibbonStyle                       — pick the Office era
3. Per-element AppearanceObject (where exposed)    — local overrides
                                                  ↓
4. CustomDrawItem                                  — owner-draw escape hatch
```

Each layer overrides the one above for the area it touches. Start with the highest layer that solves the problem.

## Layer 1 — Skin and Palette

Apply once, before any form is shown:

```csharp
using DevExpress.LookAndFeel;
using DevExpress.Skins;

UserLookAndFeel.Default.SetSkinStyle(SkinStyle.WXI);
// Other examples:
// UserLookAndFeel.Default.SetSkinStyle(SkinStyle.Bezier);
// UserLookAndFeel.Default.SetSkinStyle(SkinStyle.Office2019Colorful);
// UserLookAndFeel.Default.SetSkinStyle(SkinStyle.DevExpressDark);
// A specific palette (color variant) uses the (skinName, paletteName) string overload:
// UserLookAndFeel.Default.SetSkinStyle("WXI", "Sharp");
SkinManager.EnableFormSkins();
```

Skin families: **WXI** (Windows 11), **Bezier**, **The Bezier**, **DevExpress Style**, **DevExpress Dark Style**, **Office 2019 Colorful**, **Office 2019 Dark Gray**, **Office 2019 Black**, **Office 2019 White**, **Office 2016 Colorful**, **Office 2016 Dark**, **Visual Studio 2013**, **Visual Studio 2017**, **High Contrast**.

Many skins ship with multiple **palettes** (color variants). Pass the skin name and palette name to the `SetSkinStyle(string skinName, string paletteName)` overload, or use the `SkinHelper`-based skin chooser at runtime.

End-user skin picker — add `SkinDropDownButtonItem` and `SkinPaletteDropDownButtonItem` to the ribbon:

```csharp
ribbon.Items.Add(new SkinDropDownButtonItem());
ribbon.Items.Add(new SkinPaletteDropDownButtonItem());
groupView.ItemLinks.Add(ribbon.Items[ribbon.Items.Count - 2]);
groupView.ItemLinks.Add(ribbon.Items[ribbon.Items.Count - 1]);
```

## Layer 2 — Ribbon Style Family

`RibbonControl.RibbonStyle` (type `RibbonControlStyle`; note it shares a name with `BarItem.RibbonStyle`, which is an unrelated per-item sizing bitmask) picks the Office era. The skin paints inside the chosen style. There is no separate `RibbonControl.Style` property — `RibbonStyle` is the only one.

| Value | Look |
|---|---|
| `Office2007` | Round Application Button overlapping the title bar; old Application Menu. |
| `Office2010` | "File" tab opens Backstage View. |
| `Office2013` | Flat, no chrome around groups. |
| `TabletOffice` | Touch-friendly tablet style. |
| `Office2019` | Modern flat with subdued tab borders. |
| `OfficeUniversal` | Office 365 / Office for the Web look. QAT is unavailable in this style. |

```csharp
ribbon.RibbonStyle = RibbonControlStyle.Office2019;
```

> The Application Menu vs Backstage View depends on this. Office 2007 takes an `ApplicationMenu`; everything 2010+ takes a `BackstageViewControl`.

## Layer 3 — Per-Element Appearance

Every visual element exposes one or more `AppearanceObject` properties on `Appearance` or `Appearances` (depending on the class). An `AppearanceObject` has `Font`, `ForeColor`, `BackColor`, `BackColor2`, `BorderColor`, `GradientMode`, `TextOptions`, `Image*`, plus a `UseFont` / `UseForeColor` / `UseBackColor` flag for each one that you must enable for the override to take effect (when the property is exposed as `Use…` instead of being implicit).

There is **no** `RibbonControl.Appearance` property. Ribbon-wide per-element appearances are exposed through a `BarAndDockingController`'s `AppearancesRibbon` (type `RibbonAppearances`) — reached via `DefaultBarAndDockingController.Controller.AppearancesRibbon` (see the next section). `RibbonAppearances` exposes members such as `PageGroupCaption`, `PageHeader`, `PageHeaderSelected`, `PageHeaderHovered`, `PageCategory`, `Item`, `ItemHovered`, `ItemPressed`, and `ApplicationButton`.

Per-item appearance — `BarItem` exposes `Appearance` and `AppearanceDisabled`:

```csharp
saveCmd.Appearance.Font = new Font("Segoe UI", 9, FontStyle.Bold);
saveCmd.Appearance.Options.UseFont = true;
```

Per-bar appearance (`Bar.Appearance`, an `AppearanceObject`):

```csharp
toolBar.Appearance.BackColor = Color.WhiteSmoke;
toolBar.Appearance.Options.UseBackColor = true;
```

## Application- and Form-Scope Controllers

Two non-visual components let you override defaults for **all** bars and ribbons:

- **`DefaultBarAndDockingController`** — application-scope. Add one to your startup form, set its `Default = true`. All `BarManager`/`RibbonControl`/`DockManager`/`DocumentManager` instances in the app inherit its settings unless overridden.
- **`BarAndDockingController`** — form-scope. Add it to a form, assign each `BarManager`/`RibbonControl`/`DockManager` to it via the `Controller` property.

Settings include `LookAndFeel`, `AppearancesBar`, `AppearancesRibbon`, `BarItemStyle`, `PaintStyle`, `PaintStyleName`, `EnableLogicalSkinning`, `MenuAnimationType`, `OptionsRibbonMiniToolbar`, etc.

```csharp
// DefaultBarAndDockingController exposes the appearances through its Controller property.
defaultController.Controller.AppearancesRibbon.PageGroupCaption.BackColor = Color.FromArgb(45, 45, 48);
defaultController.Controller.AppearancesRibbon.PageGroupCaption.Options.UseBackColor = true;

defaultController.Controller.AppearancesBar.Bar.ForeColor = Color.White;
defaultController.Controller.AppearancesBar.Bar.Options.UseForeColor = true;
```

Use this when you want consistent appearance across many forms without setting `Appearance` on every individual `RibbonControl` / `BarManager`.

## Contextual Category Colors

The `RibbonPageCategory(string text, Color color, bool visible)` constructor paints a contextual category with a colored highlight. The third argument is the category's initial **`Visible`** state, not a color flag:

```csharp
var picCat = new RibbonPageCategory("Picture Tools", Color.OrangeRed, true);
ribbon.PageCategories.Add(picCat);
```

How the `Color` tints the category (whole strip vs. tab borders) is skin-dependent — verify visually for your chosen skin.

## What Cannot Be Changed Via Properties

The DevExpress skin engine owner-draws large parts of the ribbon and bars. Properties exposed on `Appearance*` typically work for **font**, **fore/back color**, **gradient mode**, and **border color** — but **shapes**, **corner radii**, **glyph rendering**, **selected-tab indicators**, and **complex hover effects** are baked into the skin and **cannot** be overridden with property tweaks alone.

For those, `RibbonControl` exposes a single owner-draw hook — `CustomDrawItem`:

| Event | Element |
|---|---|
| `CustomDrawItem` | A bar item link. |

`RibbonControl` does **not** expose `CustomDrawTab`, `CustomDrawCategory`, `CustomDrawGalleryItem`, `CustomDrawPageGroupCaption`, `CustomDrawApplicationButton`, or `CustomDrawItemBackground` — for those areas, switch skins or edit the skin instead.

```csharp
ribbon.CustomDrawItem += (s, e) => {
    // e.LinkInfo is a BarLinkViewInfo; its Link.Item is the BarItem being drawn.
    if (e.LinkInfo?.Link?.Item is BarButtonItem btn && btn.Caption == "Save") {
        e.Cache.FillRectangle(new SolidBrush(Color.MediumSeaGreen), e.Bounds);
        e.Handled = true;
    }
};
```

Set `e.Handled = true` to suppress default painting; leave it `false` to let the skin paint first.

### Things that are not styleable by simple property and require custom drawing

- Tab corner radius / shape — the skin defines it.
- Selected-tab underline color/thickness — owner-drawn.
- Group separators inside the ribbon — owner-drawn.
- Application button outline / glyph alignment — owner-drawn.
- Hover ripple/animation effects — controlled by the skin's animation parameters, not exposed as standalone properties.
- Backstage View tab indicator — owner-drawn (use `BackstageViewControl.ParentAppearance` + `AppearanceSelected`/`AppearanceHover` for colors only).
- The exact placement of QAT inside the title bar — controlled by `ToolbarLocation` + `RibbonStyle` only.

When you find a customization you cannot make through `Appearance*`, the answer is almost always one of:

1. Pick a different skin / palette.
2. Override the skin element via a custom `Skin` (advanced — see DevExpress Skin Editor).
3. For a bar item link, use `CustomDrawItem` to paint your version.

## Bars Appearance

`Bar.Appearance` covers a single toolbar's chrome. Bar- and menu-wide appearances live on a `BarAndDockingController` (app-scope `DefaultBarAndDockingController`, or a form-scope `BarAndDockingController` assigned via `BarManager.Controller`): `AppearancesBar.Bar` / `.MainMenu` / `.SubMenu` / `.StatusBar`. Subitem captions, hovered items, and disabled items each have their own `AppearanceObject`.

```csharp
// Menu header appearance via the controller (BarManager has no AppearanceMenu property).
var menu = defaultController.Controller.AppearancesBar.SubMenu.HeaderItemAppearance;
menu.BackColor = Color.FromArgb(50, 50, 50);
menu.ForeColor = Color.White;
menu.Options.UseBackColor = true;
menu.Options.UseForeColor = true;
```

For a paint-style override on a single link (`link.PaintStyle` is read-only — use `UserPaintStyle`):

```csharp
link.UserPaintStyle = BarItemPaintStyle.CaptionGlyph;
```

## DPI and Glyph Rendering

- Always prefer SVG (`ImageOptions.SvgImage`) for icons. SVG glyphs are re-recolored to match the active skin; raster glyphs are not.
- Control icon dimensions through the glyphs you supply: set the `ImageSize` of the `SvgImageCollection` (or provide separate small/large images via `RibbonControl.Images` / `LargeImages`). SVG glyphs then scale automatically to the size the ribbon requests.
- High-DPI scaling works automatically when `<ApplicationHighDpiMode>` is set in `Program.Main`:

```csharp
Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
```

## Common Issues

- **Setting `BackColor` does nothing**: forgot `Options.UseBackColor = true`.
- **Skin not applied**: `SetSkinStyle` called after a form is shown. Move it to `Main`.
- **Custom font ignored on a state**: `BarItem` exposes `Appearance` and `AppearanceDisabled`; set `Font` + `Options.UseFont = true` on the relevant one. For hover/pressed/selected ribbon states, use the controller's `AppearancesRibbon` members (`ItemHovered`, `ItemPressed`, `PageHeaderHovered`, `PageHeaderSelected`).
- **Want round tabs / different shape**: skins decide. Pick a different skin or edit the skin.
- **Contextual category color not visible**: depends on `RibbonStyle`; *Office 2007* shows only the strip border, *Office 2019* tints the whole strip.
- **Application button image looks wrong**: small (32×32) vs large (40×40) requirements differ per `RibbonStyle`. Provide both via `ApplicationButtonImageOptions.SvgImage` + `Image`.

## Source Material

- `articles/controls-and-libraries/ribbon-bars-and-menu/ribbon/ribbon-styles.md` (`xref:WindowsForms.14624`).
- `articles/controls-and-libraries/ribbon-bars-and-menu/ribbon/visual-elements/categories-and-contextual-tabs.md` (`xref:WindowsForms.3327`).
- `articles/common-features/application-appearance-and-skin-colors/index.md` — skins overview.
- `articles/common-features/application-appearance-and-skin-colors/glyph-skinning.md` (`xref:WindowsForms.15635`).
- `api/DevExpress.XtraBars.Ribbon.RibbonControl.RibbonStyle.yml`.
- `api/DevExpress.XtraBars.DefaultBarAndDockingController.yml`.
- `api/DevExpress.XtraBars.BarAndDockingController.yml`.
- `api/DevExpress.XtraBars.Ribbon.RibbonControl.CustomDrawItem.yml`.

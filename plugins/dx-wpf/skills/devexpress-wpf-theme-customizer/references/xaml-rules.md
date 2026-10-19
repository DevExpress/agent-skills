# Step 7: Prepare the Customized XAML

Copy the original resource from the theme source and apply the user's changes. Follow these rules — violating them can break the control silently or at runtime.

**Copy the original template verbatim as your starting point**, then edit only for the goal. Which edits are allowed depends on the goal:

- **Recolor / restyle** (the common case): change only the properties you must (usually a single brush). Do **not** wrap, insert, remove, or re-nest elements — a needlessly restructured template can build cleanly yet hide the element or throw at runtime.
- **Augmentation** (deliberately adding UI — e.g. inserting a *Go-To page* editor into a pager template): here inserting elements **is** the goal, so it is allowed. But keep everything you don't own intact — never remove or rename `PART_` elements, don't drop existing states/triggers, and preserve GradientStop counts/offsets (see "What not to touch"). Any interactive element you add must be **wired correctly** (bindings/commands): a clean build proves nothing about whether it actually functions.

A clean build never confirms an override took effect, that a restructured template still works, or that an added control is wired right — only a run/visual check does.

---

## Theme scope and resource key format

Use the scope identified in Step 1:

| Scope | Key format |
|-------|-----------|
| All themes | `{ThemeKeyExt ResourceKey=X, IsThemeIndependent=true}` |
| Specific theme | `{ThemeKeyExt ResourceKey=X, ThemeName=TheThemeName}` |
| No specifier | DeepBlue theme only — almost never what the user wants |

The `ResourceKey=` form above is for **regular** theme keys (`…ThemeKey`). **Lightweight** keys work differently:

### Lightweight keys (`LWKey` / `LWDefaultStyleKey`) — positional syntax

The source shows `{dxdot:LWKey Something.Name}` / `{dx:LWDefaultStyleKey '…'}` — the key is **positional**, not `ResourceKey=`:

| Scope | Key format |
|-------|-----------|
| All lightweight themes | `{dxdot:LWKey <Element.ResourceName>}` |
| One lightweight theme | `{dxdot:LWKey <Element.ResourceName>, ThemeName=<TheThemeName>}` |

- A **regular** theme-key override also works under lightweight — it just becomes **theme-independent** (`ThemeName` is ignored, per WPF/404442). So an **all-lightweight-themes** change needs only a plain regular override (simpler, and it avoids the `LWKey` pitfalls below); reach for `LWKey` only to scope to **one specific** LW theme.
- **Never write `{dxdot:LWKey ResourceKey=X, ThemeName=Y}`** — `LWKeyExtension` has only a positional constructor, so the named form throws a **`XamlParseException` while parsing `App.xaml` → no window at all** (the build still succeeds; only a run catches it).
- Copy the key **verbatim** (positional); add `, ThemeName=<active theme>` only to scope to one active LW theme (Step 3).

---

## Hardcoded color literals under an all-themes override

An all-themes restate (`IsThemeIndependent=true`) copies the template's hardcoded color literals (`Background="#FFF8F8F8"`, … — values, not refs) verbatim, freezing one theme's colors onto every theme. Wrong under other/switched themes, and invisible in the authoring theme (a single-theme screenshot won't catch it). The `dxi:ThemeResource` conversion below doesn't help — it redirects refs, not literals.

- **Literal is the requested change** ("make it red") → keep it.
- **Literal is incidental** (copied along, not what you set out to change):
  1. **Avoid the restate.** Re-check Step 4 — change the target property via a public API, `BasedOn`, a scoped setter, or one keyed brush, without copying the template. Then nothing freezes.
  2. **Restate unavoidable → no reliable auto-fix** (a literal can't be reverse-mapped across themes; baked literals rarely have a theme key for `dxi:ThemeResource`; the app's theme set is unknowable — switchers, `Themes.All`). Don't silently freeze or guess the themes — **ask the user**: one fixed color for all themes, per-theme `ThemeName=` overrides for themes they name, or drop the change.

---

## What not to touch

### `PART_*` elements
Never remove or rename them — controls reference them in code-behind by exact name. If a `PART_` element is missing at runtime, the control silently breaks or throws.
- To *hide* a `PART_` element: set `Visibility="Collapsed"`
- Exception: if the control sets `Visibility` itself in code, use `Opacity="0"` instead — setting Visibility would conflict

### GradientStop elements inside LinearGradientBrush
Animated controls depend on their exact count and offsets. Removing or reordering them breaks animations silently.

---

## Verify a `DXImage` path before you write it

DevExpress image resource names are **not guessable**, and a wrong one builds clean and renders
nothing — indistinguishable from an override that never took effect. So resolve the path against
something real before it goes into the markup: enumerate the resource names in the version-matched
`DevExpress.Images.v<version>.dll`, or pick the image in the **DevExpress Image Picker** and copy the
path it hands you.

Watch the two naming schemes in particular. A raster image name and an `SvgImages/<Folder>/<Name>.svg`
path are different catalogs; a real name from one, written in the other's shape, looks entirely
plausible and does not exist (observed: `SvgImages/Actions/Down.svg` is not in
`DevExpress.Images.v25.2.dll`).

The path is the **cheapest** thing to rule out, so rule it out first. That is an order of checks, not
a diagnosis — a template that lands and shows nothing is just as often the wrong key, or an element
the control never builds at all (`references/find-resource.md`).

---

## Handling `cs:Name` attributes

If the source contains `cs:Name="..."`, either remove the attribute or add these namespaces to the root element and keep it:

```xml
xmlns:mc="http://schemas.openxmlformats.org/markup-compatibility/2006"
xmlns:cs="colorscheme_ignorable_uri"
mc:Ignorable="cs"
```

---

## Dynamic resources and `ThemeResource`

**Regular themes only** — under lightweight themes `DynamicResource`/`StaticResource` have no such restriction (WPF/404442), so skip this conversion there.

If the original uses `DynamicResource` pointing at a theme key, and the scope is *all themes* (`IsThemeIndependent=true`), the `DynamicResource` will resolve from the DeepBlue theme regardless — producing wrong colors on other themes.

To make it follow the active theme instead, replace `DynamicResource` with `dxi:ThemeResource`:

```xml
xmlns:dxi="http://schemas.devexpress.com/winfx/2008/xaml/core/internal"
...
<Border BorderBrush="{dxi:ThemeResource {someNs:SomeControlThemeKey ResourceKey=BorderBrush}}"/>
```

---

## Override an object resource, not just a brush

Swapping a keyed brush works only while the element reads its value from that brush. When the value lives inside an **object** resource — a template provider, a style, any resource that is not a plain brush — and the element is handed the object as a whole, the leaf brush is never consulted and overriding it changes nothing on screen (`references/find-resource.md` covers how to recognise this). What has to be overridden then is the object itself.

Copy the object under **the same theme key** it carries in the source, change the one property you own, and pull every other part back from the theme:

```xml
<!-- the one property you own is set directly; the rest are re-attached in the form the theme uses -->
<someNs:SomeProvider x:Key="{someNs:SomeControlThemeKey ResourceKey=SomeProvider, ThemeName=TheThemeName}"
                     NormalForeground="#FFFF0000"
                     SomeTemplate="{StaticResource {someNs:SomeControlThemeKey ResourceKey=SomeTemplate}}"
                     SomeStyle="{StaticResource {someNs:SomeControlThemeKey ResourceKey=SomeStyle}}"/>
```

- **Keep the properties you are not changing.** Your object replaces the theme's one entirely, so every property you leave unset falls back to the type's default — the usual result is an element that renders empty, unstyled, or invisible, with a perfectly green build. Point each of those properties back at the theme's own resource, **using the same reference form the theme source uses for it** — see the two caveats below before choosing between `StaticResource` and `DynamicResource`.
- **Confirm those properties are dependency properties first.** `DynamicResource` on a plain CLR property is accepted while the XAML is parsed and then never resolves — nothing fails, the value simply never arrives. Use the reflection probe (`references/api-check.md`) on the version-matched assembly to check the object's type declares `<PropertyName>Property` for each one. A property that is not a dependency property cannot take `DynamicResource`: set a literal value there, or take a different route.
- **Dependency properties are necessary but not sufficient — prefer the reference form the theme's own source uses.** A resource object declared in a dictionary is free-standing: it is not in the visual or logical tree, so it may have no inheritance context for a deferred reference to resolve through. `DynamicResource` then resolves to nothing even on a genuine dependency property, and the part you meant to re-attach comes back **empty** — in a real case this made the glyphs it was supposed to preserve disappear entirely, with a green build and a rendered window. Copy the reference form from the theme source: where the theme writes `StaticResource` for that property, write `StaticResource`. Reach for `DynamicResource` only where the theme itself does, or where you have confirmed by render that it resolves.
- **Verify by render, always.** An object assembled from the wrong parts is exactly what a build cannot catch. This route is finished only when Step 10 measures the element rendering correctly — both the property you changed and the parts you re-attached.

---

## Lightweight: pick the route that reaches the element (and all its states)

Under **lightweight themes** (Step 3), before overriding a template to restyle a sub-element (e.g. a glyph, a row/item background), check in the theme source how its brush is set, then take the first route that applies:

1. **Bound to an inherited property** — e.g. `Fill="{TemplateBinding Foreground}"` the theme leaves unset — set it with a scoped implicit style on the **element's own type** (`<Style TargetType="…">` + that one setter) in the nearest control's `Resources`, not on a distant ancestor via inheritance (an intermediate element can intercept it). A bare `TargetType`-only style keeps the themed template (an implicit style layers *over* the default/theme style, so the `Template` it omits still comes from there); don't add `BasedOn="{StaticResource {x:Type …}}"` — needless, and it throws under lightweight. Simplest; prefer it when it applies.
2. **Driven by leaf theme-key brushes via `DynamicResource`** — the LW style and its state triggers (focused / selected / hover) point Background/Foreground at leaf theme-key brushes (e.g. `{dxgt:GroupRowThemeKey ResourceKey=GroupRowBackground}` and its `…-Focused` / `…-Selected` siblings). Override those leaf brush **resources** — a `SolidColorBrush` keyed with the *same* theme key — in the **nearest control's `Resources`** (never App, or you recolor every control that shares the key). Every state's trigger re-resolves to your brush at once, so this reaches the focused/selected/hover states that a style-level setter or a state-incomplete restated template would miss, and it is cheaper than restating the style/template. **Prefer this over a template override when the brush is keyed.**
3. **Hardcoded, or not reachable either way** — restate the template (per the recolor/restyle rule at the top of this file — copy verbatim, change only what you must, never re-nest). Scope rule proven under LW: a keyed **template/brush** resource override takes effect at nearest-control scope (its `DynamicResource` walks up the tree), but a **default-style key** redeclaration (`LWDefaultStyleKey` / `DefaultStyleThemeKey`) at control scope is visually **inert** — the framework resolves the default style from the theme dictionaries, not the control's `Resources`. So restate the template; don't redeclare the default-style key and expect it to apply.

---

## Static resource dependencies

The search sub-agent (Step 5) already resolved the resource's `StaticResource` dependencies and returned them alongside the main resource block. Include each of those dependency blocks in the output — do **not** re-search the source for them here.

If, while preparing the XAML, you discover a `StaticResource` key that the sub-agent did not return (rare), treat it as an incomplete search result: note the missing key and re-delegate to a search sub-agent rather than grepping the theme source from the orchestrator.

---

## Next step

Read `references/apply.md` to apply the prepared XAML to the project and run the final verification.

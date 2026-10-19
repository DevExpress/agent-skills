# Step 5: Locate the Theme Resource (search sub-agent)

You are running as a **dedicated search sub-agent**. Your job is to locate the theme resource for the requested customization and return a distilled result to the orchestrator. Keep the noisy file searching inside this sub-agent — only the final result crosses back.

**You receive** from the orchestrator: the target control/element, the desired change, the theme scope, and the resolved `XPF` source root + theme folder (SKILL.md Step 3).

> **No installed source tree?** If the orchestrator says Step 3 found **no** `XPF` source root (the "Source Code" component was never installed — whether the project uses NuGet packages or an MSI install), the source-grep flow below cannot run. **Read `references/assembly-fallback.md` as well** — it recovers the shipped template from the compiled assemblies the orchestrator resolved (reflect the Core DLL for the key names, extract the template from the theme assembly's BAML). Return via the same "Found" / "Not found" contract. **Only the greps are unavailable to you, not the checks:** "First: is the element your override targets created at all?" and the indirection checks below are about the pipeline and the key, not about where the template came from — they still decide the case, and on that branch they come *first*.

**You must NOT ask the user anything.** User-facing recovery (asking for a Snoop hint, choosing resource placement) belongs to the orchestrator. If automated search fails, return the "not found" signal described at the end and stop.

> **Token efficiency:** never load full source files. Use `grep`/`glob` to narrow, then read only the required block via `view_range`. Each search narrows scope before the next.

---

## Search the source files — narrow progressively

Search within a single theme rather than all themes. Use the theme the user already specified (from Step 1). If no theme was specified, use **Office2019Colorful** as a default.

**All searches use ripgrep syntax (`--glob`, not `--include`).**

### 1. Derive the identifying string from context

Use what is already known — do not ask for anything. From the description and control name, derive one or more candidate strings likely to appear uniquely in the XAML source:

- Control type name or a part of it (e.g., `TokenEditor`, `ColumnHeader`, `FilterButton`)
- A visual description translated to XAML terms (e.g., "funnel icon" → a `Path` with a specific `Data` geometry)
- Any string the user mentioned verbatim that could appear in XAML (a hex color, a known key name fragment, an element name they saw in Snoop)

Start with the most specific string. If it returns no results, try a broader one.

### 2. Find the file

`{XPF root}` below is the DevExpress source root resolved in SKILL.md Step 3 (do not assume `C:\Program Files\…` — it may be under `Program Files (x86)` or a custom path).

```
grep -rl "<identifying string>" "{XPF root}\DevExpress.Xpf.Themes\{ThemeName}\" --glob "*.xaml"
```

Example (default install, Office2019Colorful, v25.2):
```
grep -rl "TokenEditor" "C:\Program Files\DevExpress 25.2\Components\Sources\XPF\DevExpress.Xpf.Themes\Office2019Colorful\" --glob "*.xaml"
```

Returns file paths only — cheap.

### 3. Find the resource key

Once you have the file, narrow the search using the control type or area name you already have — do not list all keys blindly:

```
grep -n "{ControlName}|{AreaName}" "<matched file>"
```

If that returns too many lines, add `ControlTemplate` or `ResourceKey` as a second pass to zero in on the right block.

Note the line number.

### 4. Extract the resource block

Use `view_range` starting at the found line. Start with N to N+100, extend if the closing tag isn't reached — do not load the whole file:

```
view "<matched file>" [N, N+100]
```

Note the `x:Key` — typically a theme key markup extension:
- `{SomeThemeNamespace:SomeControlThemeKey ResourceKey=ControlTemplate}`
- `{SomeThemeNamespace:SomeControlThemeKey ResourceKey=Background}`

---

## Resolve `StaticResource` dependencies

The resource you found usually references other resources (converters, brushes, sub-templates) via `StaticResource`. The orchestrator must not re-enter the search you just did, so **resolve these dependencies here** and include them in your result.

For every `StaticResource` key the resource block uses:
1. Grep the same source file for the key name.
2. If not found there, grep across the entire theme folder: `--glob "*.xaml"`.
3. Extract each dependency with the same `view_range` approach.
4. Recurse: if a dependency itself uses `StaticResource` keys, resolve those too. Stop when no new keys appear. Skip keys that are theme keys resolved by markup extensions (those don't need copying).

---

## Confirm the key by its value, not by its name

A name that matches the request does not prove the resource is the one in effect. The same name occurs in several namespaces, and those copies resolve to **different** values — by name alone they are indistinguishable.

So before you report a resource as found, compare its **actual value** — the colour hex, the number, the geometry — with what the application currently shows. The observed value is in what the orchestrator passed you: the baseline description, a hex the user read out of Snoop, a colour sampled from the screenshot.

- Values match → the candidate is consistent with what renders. Report it.
- Values differ → **first ask which of the two is wrong.** The candidate may be the wrong resource — but the observed value may also have been read off the wrong pixel: an area that hosts other controls is mostly covered by them, so a sample taken on a child returns the child's colour, not the area's. Read the resource block to settle it: if the element you are after is a container whose children carry their own backgrounds, and the observed value matches one of those children, the mismatch is in the observation, not the resource. Otherwise it is the wrong resource, however well the name fits — keep searching, and list in your result the candidates you rejected on value.
- No observed value was passed to you → report the key as matched **by name only**, so the orchestrator knows it is unconfirmed.

---

## First: is the element your override targets created at all?

The indirection checks below all assume the element **exists** and the *value* routes around your key.
A different failure wears the same symptom: the element is never instantiated, because the control
draws itself through an **optimized rendering path** whose elements — and whose resource set — are not
the ones the theme's `Control`/`ControlTemplate` XAML describes. No key, no key type and no scoping
trick reaches a `ControlTemplate` for an element that is never constructed, so when this is the case
every check below is wasted budget.

Settle it before spending an attempt, and again the moment an override measures **zero** change.

**Bar items and ribbon items — the silent case.** In the themes that enable it, a `BarItem` renders
through DevExpress's own `DrawingVisual` "Render chrome" (`LightweightBarItemLinkControl`,
`dxcn:RenderTemplate`, `RenderArrowControl`, …), and the classic `Control`-based elements
(`dxb:ArrowControl`, `BarItemLayoutPanel`, `BarButtonItemLinkControl`, `BarSubItemLinkControl`,
`BarSplitButtonItemLinkControl`) are **absent from the visual tree**. Nothing warns you: the build is
clean, the app renders, the override does nothing.

- Turned on by the **theme**, not by where the item sits. `True` in Win11, Win10, Office2019, VS2019,
  VS2017 and Office2016 **SE**; `False` in the older families (Office2016 non-SE, Office2013/2010/2007,
  Metropolis, LightGray, DXStyle, HybridApp, Seven, TouchlineDark, DeepBlue). Confirm for the
  project's own theme rather than trusting the list:
  ```
  grep -rn "ResourceKey=UseLightweightTemplates" "{XPF root}\DevExpress.Xpf.Themes\{ThemeName}\" --glob "*.xaml"
  ```
- ⚠️ **This is not the lightweight-vs-regular _themes_ flag from SKILL.md Step 3.** Two unrelated
  features whose names collide. A project on regular themes still renders its bar items this way, so
  Step 3 answering "regular" settles nothing here.
- Also off in touch themes and when `RibbonStyle` is `OfficeSlim`/`TabletOffice`. Some items are never
  lightweight regardless: `BarButtonGroup`, `BarItemSeparator`, gallery items.
- **The lever is public API, at three scopes** — report which one the fix needs, the orchestrator has
  to tell the user: `UseLightweightTemplates="False"` on the **item**, `UseLightweightTemplates` on the
  **link**, `CompatibilitySettings.UseLightweightBarItems = false` for the **whole app**. Prefer the
  narrowest that covers the request: one item if one item was asked about, app-wide only if the request
  itself is app-wide.
- ⚠️ **The other route — overriding the optimized path's own resources instead of leaving it — cannot
  be scoped to one item, and trying to do so fails silently.** Restating the theme's own
  `RenderTemplate`/selector works, but it is a **keyed resource**: its reach is decided by *where you
  put the dictionary*, and a bar item gives you no narrow place to put it. `BarItem`, `BarItemLink`,
  `RibbonPageGroup` and `RibbonPage` are `FrameworkContentElement`s, **not visual elements**
  (`Bars\Items\_BarItem\BarItem.cs:86`); the element that consumes the key is the links control, which
  is not their child. So a dictionary placed on the item or on the group **builds, runs and does
  nothing** — no error, measured. The narrowest placement that *works* is a **visual** ancestor
  (`RibbonControl`, the window, the application), and then **every** item of that kind inside it
  changes. If the request named one item, only the per-item lever above can honour that — say so
  instead of presenting a wider fix as if it were scoped.

**Data controls announce it themselves — don't build guesswork around them.** `GridControl` views and
`PivotGridControl` have an optimized mode too (`UseLightweightTemplates`, on by default, independent of
the theme and long predating the bars one), and the two modes expect **different** styles and templates
for the same public property — but these controls **throw `InvalidOperationException`** naming the
unsupported member instead of failing silently. If such an exception appears, act on what it names
(switch the mode, or the style's `TargetType`); it is not this check's silent case.

**Anything else:** treat "the element does not exist" as a live hypothesis whenever a zero-effect
override is not explained by the four checks below. Confirm it by **dumping the visual tree** — a
`Loaded` handler walking `VisualTreeHelper` and printing type names — never by reasoning about it.

### Once the pipeline is switched and the result is still zero, the pipeline is no longer the answer

This check is a gate you pass through, not a verdict you keep returning to. The moment you have
**switched the optimized path off and measured zero anyway**, the pipeline hypothesis is spent. What
remains is the override itself: its **key form**, its **scope**, or the element it targets. Move on to
the indirection checks below and to `references/xaml-rules.md`.

Two ways this goes wrong, both observed:

- ⛔ **Do not go back to source to explain the null.** A null result is an under-determined
  observation — the code will happily supply a story for it, and a plausible-sounding one drawn from
  precedence logic or a cached static field is not evidence about *your* app.
  The measurement is the authority — if you doubt whether the switch took effect,
  **dump the visual tree and look at what type the item's link control is**, which settles it in one
  pass. Reasoning about precedence cannot.
- ⛔ **A real key copied out of the theme source is not automatically an overridable one.** Theme files
  hold two kinds of key, and only one of them is a lookup key. A style keyed with a **plain string** —
  `<Style x:Key="BarSplitButtonItemLinkControlDefaultStyle" …>` — is an internal base style, consumed
  by other theme styles through `BasedOn="{StaticResource …}"`, resolved at parse time **inside the
  theme dictionary's own scope**; nothing at runtime ever looks that string up, and `StaticResource`
  resolution there never reaches `Application.Resources`. Redefining it app-side builds, runs and does
  nothing. The style DevExpress actually applies is found through WPF's own `DefaultStyleKey`, which
  DevExpress sets to a **`{dxdst:DefaultStyleThemeKey FullName=…}`** resource key
  (`Themes\ThemeKey.cs:238-297`, `ThemeManager\Helpers.cs:108-114`) — and *that* walk does pass through
  `Application.Resources`. So when a grep hands you a plain-string key, **find the ThemeKey-keyed entry
  that is `BasedOn` it** and override that one instead:
  ```
  grep -rn "DefaultStyleThemeKey FullName=.*<ControlTypeName>" "{XPF root}\DevExpress.Xpf.Themes\{ThemeName}\" --glob "*.xaml"
  ```
  (For `BarSplitButtonItemLinkControl` in Win11Light that is `…\Bars\Themes\Win11Light.xaml:335`.)
  ⚠️ **Match on the namespace, never on the prefix.** `dxdst:`, `dxt:` and whatever else a given theme
  file happens to declare are just per-file aliases — the source is not consistent about them, and two
  files can name the same extension differently or the same prefix differently. What identifies
  `DefaultStyleThemeKey` is the namespace its prefix is bound to
  (`http://schemas.devexpress.com/winfx/2008/xaml/core/themekeys`); read the `xmlns:` declarations at
  the top of the file you are actually in, and grep for the **extension name**, not for a prefix.
- ⛔ **Do not read "the diagnostic colour is absent" as "the switch did nothing."** Those are different
  claims. Ask the verification sub-agent for the **whole-window** changed-pixel count, not only the
  target's: switching a control family off an optimized path reflows its layout and moves a large,
  measurable area even when your override never lands. A large count with an unchanged target is
  positive evidence that the switch worked and the **key** is wrong.

---

## Indirection: the name is right but the resource is not the one in effect

A theme rarely sets a visible property where you would expect it. The value usually reaches the element through a chain of resources, and a grep on the property name lands somewhere in that chain that nothing reads. Suspect indirection when the orchestrator tells you an override of the resource you found changed nothing on screen, and check for it up front when the search returns several plausible keys.

Four things move the value away from where you looked. Check them in this order — each has a cheap grep.

**1. The value sits in an object, not in a brush.** Themes group appearance values into objects (template providers and the like) and feed the element from the object. The leaf brush is then never consulted, so overriding it does nothing. Read the element's style and look for a setter whose value is such an object:

```
grep -n "Provider" "<style file>"
```

**2. That object is assigned through an attached property, by a different style.** The style you are reading often does not name the object it uses — a style further out sets it. Find who sets the attached property:

```
grep -rn "Property=\"<Type>\.<AttachedProperty>\"" "{XPF root}\DevExpress.Xpf.Themes\{ThemeName}\" --glob "*.xaml"
```

The setter you find names the key that is really in effect, and it is frequently in a **different namespace** than the one you started from.

**3. The resource is swapped by context.** The same element can be served by different keys depending on the kind of window or host it sits in, or on its state. When several keys exist with near-identical names, decide which one applies by finding the style that assigns it (check 2) — never by which name reads best.

A key is identified by **two** things: the `ResourceKey=` name *and the key type* — the markup extension in front of it (`{dxt:SomeThemeKey …}`, `{dxrt:OtherThemeKey …}`), which are separate classes, often in separate assemblies. **The same `ResourceKey=` name can exist under two different key types, and those are two different resources** that a grep on the name cannot tell apart. Overriding the right name under the wrong key type builds clean and changes nothing on screen. So list every occurrence and compare the prefixes:

```
grep -rn "ResourceKey=<KeyName>" "{XPF root}\DevExpress.Xpf.Themes\{ThemeName}\" --glob "*.xaml"
```

More than one distinct prefix means the name alone is ambiguous — take the key type from the setter that assigns it (check 2), not from whichever file you grepped first, and report both.

**4. The value is baked in.** Two forms, both immune to an override placed in `App.xaml`:

- consumed through `StaticResource`, which resolves once while the theme is parsed and never sees your override — only `DynamicResource` consumption is overridable;
- a plain literal in the setter, with no key at all.

```
grep -n "Value=\"\{StaticResource" "<style file>"
grep -n "<TargetProperty>=\"#" "<style file>"
```

In both forms no leaf key can carry the change. The resource that has to be overridden is the **object or style that contains the baked value** — return that block, and state why the leaf key cannot work.

**These chain.** They are not four alternatives with one right answer — a single visible property can pass through several of them in sequence, and fixing one layer only exposes the next. So a resource that finally responds to your override is not proof you are done: the value may travel on through another object, in another namespace, before it reaches the pixel. Expect to repeat these checks on each layer, and report the chain you walked, not just its last link.

Report which of these you found. The orchestrator needs it to choose the shape of the override (`references/xaml-rules.md` covers overriding an object resource).

---

## Fallback: resource not found in the theme folder

DevExpress themes share base resources. If the search returns nothing:
1. Search the parent theme directory (one level up from the theme folder) for `*.xaml` files with the same identifying string.
2. If still not found, search the `DevExpress.Xpf.Core` sources:
   ```
   {XPF root}\DevExpress.Xpf.Core\
   ```
3. If still not found, try a broader or alternative search term derived from the control name.

---

## What to return to the orchestrator

Return **one** of the following — and nothing else (no raw search transcripts):

### Found
- The resource block(s), each with its `x:Key` — including the **key type** (markup extension) the theme uses to select it, not the name alone.
- The resolved `StaticResource` dependency blocks (with their keys).
- The source file path and theme name.
- The resource's **actual value** and whether it matches the observed one — or "matched by name only" when no observed value was available.
- Any indirection you found (object resource, attached property, context swap, baked-in value), and the candidates you rejected on value.

### Not found + what you tried
Only when every search above returned nothing. Return:
- The identifying strings you searched for.
- The folders/files you covered (theme folder, parent theme, `DevExpress.Xpf.Core`).
- What is still ambiguous about the target element.

The orchestrator will use this to ask the user for an identifying characteristic (via Snoop / Live Visual Tree) and then re-delegate a new search sub-agent with that hint. Do not attempt that user interaction yourself.

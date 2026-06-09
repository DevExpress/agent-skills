---
name: devextreme-theming
description: >
  Apply, customize, and switch DevExtreme themes (Generic, Material, Fluent). Covers
  predefined theme CSS import, ThemeBuilder UI and CLI, SCSS variables, custom CSS overrides,
  color swatches, and runtime theme switching.
compatibility: DevExtreme 26.1+. Supports Angular 20+, React 18+, Vue 3, jQuery 3.x or 4.x.
metadata:
  author: DevExpress
  version: "26.1"
---

# DevExtreme Theming Skill

## When to Use This Skill

- Applying a predefined DevExtreme theme to a project.
- Creating a custom theme with ThemeBuilder (UI or CLI).
- Overriding theme styles with CSS classes or component `elementAttr`.
- Implementing dark/light mode or runtime theme switching.
- Using color swatches for mixed-theme layouts.
- Setting up SCSS-based theme imports.

---

## Before You Start

> ⚠️ **Always use DevExtreme theme CSS imports and `themes.current()` from `devextreme/ui/themes` for runtime switching. Never manually swap CSS classes on `document.body`, use Bootstrap, Ant Design, Material UI, or any other theming system. For scoped section overrides, use `.dx-swatch-*` wrapper classes — do not edit DevExtreme's internal CSS files directly.**

## Architecture Overview

```text
DevExtreme Theme Pipeline
──────────────────────────────────────────────────────────────────
  Option A │ Import prebuilt CSS  →  dx.[theme].css
  Option B │ SCSS import  →  devextreme/scss/bundles/dx.[theme].scss
  Option C │ ThemeBuilder UI/CLI  →  export dx.[base].[color].css
──────────────────────────────────────────────────────────────────
  Runtime  │ DevExpress.ui.themes.current(name)  — same theme family
  Swatch   │ .dx-swatch-xxx wrapper class  — section-level overrides
  CSS API  │ .dx-* classes / elementAttr / wrapperAttr  — component-level
```

---

## Theme Families

| Family | Base name pattern | Notes |
|---|---|---|
| Generic | `generic.light`, `generic.dark`, `generic.contrast`, `generic.carmine`, etc. | Also compact variants: `generic.light.compact` |
| Material Design | `material.blue.light`, `material.blue.dark`, `material.teal.light`, etc. | Color + mode segments |
| Fluent | `fluent.blue.light`, `fluent.blue.dark`, etc. | Requires icon archive when exporting |

**CSS file name pattern:** `dx.[theme-family].[color-variant].css`  
Examples: `dx.light.css`, `dx.dark.css`, `dx.material.blue.light.css`, `dx.fluent.blue.light.css`

> Compact themes reduce all component sizes. Use these instead of `zoom`/`transform` CSS — DevExtreme does not support those properties.

---

## Documentation Reference

| Topic | Reference File |
|---|---|
| Predefined themes — applying and CSS file names | [references/predefined-themes.md](references/predefined-themes.md) |
| ThemeBuilder UI and CLI — creating custom themes | [references/themebuilder.md](references/themebuilder.md) |
| Custom CSS overrides — classes, elementAttr, defaultOptions | [references/custom-css.md](references/custom-css.md) |
| Runtime theme switching and color swatches | [references/runtime-switching.md](references/runtime-switching.md) |
| Icons — built-in library, custom icons, external libraries, Fluent archive setup | [references/icons.md](references/icons.md) |

---

## Quick-Start: Apply a Theme

### React / Vue (Vite)

```ts
// main.ts or main.tsx — import exactly ONE theme
import 'devextreme/dist/css/dx.fluent.blue.light.css';
```

### Angular

```ts
// angular.json — add to "styles" array
"styles": [
  "node_modules/devextreme/dist/css/dx.fluent.blue.light.css",
  "src/styles.css"
]
```

### jQuery

```html
<!-- index.html <head> -->
<link rel="stylesheet" href="node_modules/devextreme/dist/css/dx.fluent.blue.light.css" />
```

> Only one prebuilt theme should be active at a time. For runtime switching, see [references/runtime-switching.md](references/runtime-switching.md).

---

## Key API

| API | Description |
|---|---|
| `DevExpress.ui.themes.current()` | Returns the currently active theme name |
| `DevExpress.ui.themes.current(name)` | Switches to another theme in the same family (no page reload) |
| `DevExpress.ui.themes.ready(callback)` | Fires after a theme finishes loading |
| `elementAttr` | Adds CSS classes or attributes to a component's root element |
| `wrapperAttr` | Adds CSS classes to a popup/overlay wrapper element (Popup, Tooltip, etc.) |
| `defaultOptions(rule)` | Sets default options for all instances of a component type |

---

## Constraints and Rules

1. **One theme family at a time.** You cannot mix Generic and Material themes on the same page without color swatches. Switching between families requires a page reload.
2. **Compact is the right way to reduce size.** DevExtreme does not support `zoom` or `transform` for sizing. Use compact theme variants instead.
3. **`DevExpress.ui.themes.current()` only works within a theme family.** Cross-family switches (e.g., Generic → Material) need page reload with `localStorage`/`sessionStorage` persisting the choice.
4. **Color swatches cannot style SVG-based components.** SVG components (Chart, PieChart, Gauge, etc.) use separate JSON-based theme configurations, not CSS.
5. **ThemeBuilder is for HTML-based components only.** For SVG-based components, use the `palette` and `theme` component options.
6. **CSS class overrides may break on DevExtreme upgrades.** Internal markup structure can change without notice. Prefer the component API (`elementAttr`, `defaultOptions`) over `.dx-*` class overrides.
7. **Angular/Vue scoped styles do not reach overlay content.** Popup, Tooltip, DropDownButton, and similar components render content in the document root. Use global (unscoped) styles for those.
8. **No fabricated API**: Never guess SCSS variable names, CLI option names, or ThemeBuilder property names. Use the DxDocs MCP or official docs to verify if unsure.
9. **Use `devextreme-cli`, not `devextreme-themebuilder`**: The CLI tool for generating themes is `devextreme-cli`. The correct invocation is `npx -p devextreme-cli devextreme build-theme ...`. Do not use `devextreme-themebuilder` — it is a separate, older package with a different interface and different option names.
10. **`--output-color-scheme` is NOT a color.** It is the name for the output file's color scheme (e.g., `pink` → `dx.material.pink.css`). To set actual colors (accent, text, background, etc.) you MUST create a metadata JSON file with `items` containing SCSS token key/value pairs (e.g., `"$base-accent": "#e91e63"`) and pass it via `--input-file`. See [references/themebuilder.md](references/themebuilder.md) for the full pattern.

---

## Official Resources

- [Predefined Themes](https://js.devexpress.com/Documentation/Guide/Themes_and_Styles/Predefined_Themes/)
- [ThemeBuilder UI](https://devexpress.github.io/ThemeBuilder/)
- [ThemeBuilder CLI docs with examples](https://js.devexpress.com/jQuery/Documentation/Guide/Common/DevExtreme_CLI/#ThemeBuilder)
- [ThemeBuilder UI guide](https://js.devexpress.com/Documentation/Guide/Themes_and_Styles/ThemeBuilder/)
- [HTML-Based Components Customization](https://js.devexpress.com/Documentation/Guide/Themes_and_Styles/HTML-Based_Components_Customization/)
- [SVG-Based Components Customization](https://js.devexpress.com/Documentation/Guide/Themes_and_Styles/SVG-Based_Components_Customization/)
- [Color Swatches](https://js.devexpress.com/Documentation/Guide/Themes_and_Styles/Predefined_Themes/#Color_Swatches)

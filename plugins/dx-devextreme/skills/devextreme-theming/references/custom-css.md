# Custom CSS Overrides

## Recommended Customization Priority

Use these approaches in order of preference:

1. **Component API options** (`elementAttr`, `wrapperAttr`, `defaultOptions`) — stable across releases.
2. **ThemeBuilder** — safe SCSS variable customization without touching internal classes.
3. **CSS custom properties (`--dx-*`)** — override theme tokens at runtime without a build step.
4. **CSS class overrides** — last resort; DevExtreme markup may change without notice.

---

## `elementAttr` — Add Classes to a Component Root

Use `elementAttr` to attach CSS classes or attributes to a component's outermost element.

### All Frameworks

```ts
// jQuery
$('#grid').dxDataGrid({ elementAttr: { class: 'my-grid' } });
```

```html
<!-- Angular -->
<dx-data-grid [elementAttr]="{ class: 'my-grid' }"></dx-data-grid>
```

```html
<!-- Vue -->
<DxDataGrid :element-attr="{ class: 'my-grid' }" />
```

```tsx
// React
const myGridAttr = { class: 'my-grid' };

function App() {
    return <DataGrid elementAttr={myGridAttr} />;
}
```

Then target the class in CSS:

```css
.my-grid .dx-header-row {
    background-color: #1a1a2e;
    color: #fff;
}
```

---

## `wrapperAttr` — Add Classes to Overlay Wrappers

For overlay components (Popup, Tooltip, DropDownButton, SelectBox dropdown, etc.) that render content outside the component's DOM position, use `wrapperAttr` to add a class to the wrapper element:

```html
<!-- Angular -->
<dx-popup [wrapperAttr]="{ class: 'my-popup' }"></dx-popup>
```

```tsx
// React
const myPopupAttr = { class: 'my-popup' };

function App() {
    return <Popup wrapperAttr={myPopupAttr} />;
}
```

```css
/* Global CSS — scoped styles will NOT reach overlay content */
.my-popup .dx-popup-content {
    padding: 0;
}
```

> For Angular and Vue, overlay styles must be **global** (not scoped to the component). Angular: use `styles.css`; Vue: use `<style>` without `scoped`.

---

## `defaultOptions` — Component-Wide Defaults

Set default configuration for all instances of a component type. Useful for applying a consistent style or behavior across an entire application without touching each usage.

```ts
// jQuery / TypeScript — set at app startup
import DataGrid from 'devextreme/ui/data_grid';

DataGrid.defaultOptions({
    options: {
        elementAttr: { class: 'app-grid' },
        showBorders: true,
        rowAlternationEnabled: true
    }
});
```

```ts
// Angular — call in APP_INITIALIZER (app.config.ts) to run before components render
import { ApplicationConfig, APP_INITIALIZER } from '@angular/core';
import DataGrid from 'devextreme/ui/data_grid';

function initDefaults() {
    return () => {
        DataGrid.defaultOptions({
            options: {
                elementAttr: { class: 'app-grid' },
                showBorders: true
            }
        });
    };
}

export const appConfig: ApplicationConfig = {
    providers: [
        { provide: APP_INITIALIZER, useFactory: initDefaults, multi: true }
    ]
};
```

```ts
// Vue — call in main.ts before createApp()
import DataGrid from 'devextreme/ui/data_grid';
DataGrid.defaultOptions({ options: { showBorders: true } });
```

```ts
// React — call in main.tsx before ReactDOM.createRoot()
import DataGrid from 'devextreme/ui/data_grid';
DataGrid.defaultOptions({ options: { showBorders: true } });
```

You can also target specific devices using the `device` field:

```ts
DataGrid.defaultOptions({
    device: { deviceType: 'phone' },
    options: { columnAutoWidth: true }
});
```

---

## CSS Custom Properties (`--dx-*`)

Every DevExtreme theme exposes a set of CSS custom properties on `:root`. Overriding them lets you adjust global design tokens — accent color, text, borders, etc. — without ThemeBuilder or touching internal `.dx-*` classes.

### Available tokens (Generic themes)

| Variable | Maps to | Default role |
|---|---|---|
| `--dx-color-primary` | `$base-accent` | Accent / active-state color |
| `--dx-color-text` | `$base-text-color` | Main text color |
| `--dx-color-bg` | `$base-bg` | Component background |
| `--dx-color-main-bg` | `$base-bg` | Page / typography background |
| `--dx-component-color-bg` | `$base-bg` | Widget container background |
| `--dx-color-border` | `$base-border-color` | Border / separator color |
| `--dx-color-separator` | `$base-border-color` | Separator lines |
| `--dx-color-icon` | `$base-icon-color` | Icon fill color |
| `--dx-color-spin-icon` | `$base-icon-color` | Spinner / loading icon |
| `--dx-color-link` | `$base-link-color` | Link text color |
| `--dx-color-shadow` | `$base-shadow-color` | Drop-shadow base color |
| `--dx-color-danger` | `$base-danger` | Error / invalid state |
| `--dx-color-success` | `$base-success` | Success state |
| `--dx-color-warning` | `$base-warning` | Warning state |

> These variables are set by the theme CSS and reflect the active color scheme. Other theme families (Material, Fluent) expose the same tokens with their own values.

### Global Override

Redefine tokens in a stylesheet loaded **after** the DevExtreme theme CSS:

```css
/* styles.css  (Angular) | index.css  (React/Vue) */
:root {
    --dx-color-primary: #7c3aed;      /* purple accent */
    --dx-color-border:  #c4b5fd;      /* lighter borders */
}
```

All components that use `--dx-color-primary` internally pick up the change immediately — no class overrides needed.

### Scoped Override (section of the page)

Scope overrides to a container instead of `:root` to affect only part of the UI:

```css
.sidebar {
    --dx-color-primary: #059669;      /* green accent for the sidebar only */
    --dx-color-bg:      #f0fdf4;
}
```

```html
<div class="sidebar">
    <dx-tree-view ...></dx-tree-view>
    <dx-menu ...></dx-menu>
</div>
```

### Runtime Override from JavaScript

Change tokens at runtime — useful for user-configurable brand colors or live theme previews:

```ts
document.documentElement.style.setProperty('--dx-color-primary', '#e11d48');
```

To reset to the theme default, remove the override:

```ts
document.documentElement.style.removeProperty('--dx-color-primary');
```

### Combining with Auto Dark Mode

Pair custom properties with a `prefers-color-scheme` listener to apply a dark palette without switching theme files:

```ts
// Only viable when both palettes belong to the same theme family
const root = document.documentElement;
const mq = window.matchMedia('(prefers-color-scheme: dark)');

function applyPalette(dark: boolean): void {
    if (dark) {
        root.style.setProperty('--dx-color-primary', '#1ca8dd');
        root.style.setProperty('--dx-color-bg',      '#2a2a2a');
        root.style.setProperty('--dx-color-text',    '#dedede');
    } else {
        root.style.removeProperty('--dx-color-primary');
        root.style.removeProperty('--dx-color-bg');
        root.style.removeProperty('--dx-color-text');
    }
}

applyPalette(mq.matches);
mq.addEventListener('change', (e) => applyPalette(e.matches));
```

> **Limitation:** CSS custom properties override individual tokens but do not change every themed value the way ThemeBuilder does. For a full visual retheme, use ThemeBuilder or switch theme files. For accent-color-level changes, `--dx-color-primary` alone often covers 80 % of visible differences.

---

## Individual CSS Class Overrides

When the component API is insufficient, override `.dx-*` classes directly.

### Identify Classes

Inspect the component in the browser DevTools. The markup structure is not documented; inspect it directly.

Useful references:
- [CSS Classes API reference](https://js.devexpress.com/Documentation/ApiReference/UI_Components/CSS_Classes/)
- DevTools guide: [Chrome device mode](https://developer.chrome.com/docs/devtools/device-mode/), [Edge DevTools](https://learn.microsoft.com/en-us/microsoft-edge/devtools/device-mode/)

### Example: Override Header Row Background

```css
/* Global CSS file */
.dx-datagrid .dx-header-row > td {
    background-color: #003366;
    color: #ffffff;
    font-weight: 600;
}
```

### Angular: Global vs Scoped Styles

```css
/* ✅ styles.css — global, reaches overlay content */
.dx-popup-content { padding: 8px; }

/* ❌ app.css with ViewEncapsulation (default) — will NOT work for overlays */
```

For components that render outside the Angular component tree (Popup, Tooltip, DropDownList overlay), always define styles in the global `styles.css` or use `ViewEncapsulation.None`.

### Vue: Non-Scoped Styles

```vue
<template>
    <DxDataGrid ... />
</template>

<style>
/* No "scoped" attribute — styles are global */
.dx-datagrid-headers { font-size: 13px; }
</style>
```

---

## SVG-Based Components

SVG-based components (Chart, PieChart, Gauge, Sankey, etc.) are **not** styled with CSS. Customize them via component options:

```tsx
// React — Chart palette and theme
<Chart palette="Harmony Light" theme="material.blue.light">
    ...
</Chart>
```

```html
<!-- Angular -->
<dx-chart palette="Harmony Light" theme="material.blue.light">
</dx-chart>
```

Available palettes: `Default`, `Harmony Light`, `Soft Pastel`, `Bright`, `Ocean`, `Vintage`, `Violet`, `Carmine`, `Dark Moon`, `Dark Violet`, `Green Mist`, `Soft Blue`, `Material`, `Office`.

For a fully custom SVG theme, create a theme configuration object and register it — see the [SVG-Based Components Customization](https://js.devexpress.com/Documentation/Guide/Themes_and_Styles/SVG-Based_Components_Customization/) guide.

---

## SVG Fill Effects: Gradients, Patterns, and Images

Use `registerGradient()` and `registerPattern()` from `devextreme/common/charts` to apply gradient or pattern fills to series, bars, pie slices, and gauge ranges. Both return a `fillId` string that you pass alongside a `base` fallback color.

### Gradient Fill

```ts
import { registerGradient } from 'devextreme/common/charts';

const seriesColor = {
    base: '#f5564a',                     // fallback color
    fillId: registerGradient('linear', {
        colors: [
            { offset: '20%', color: '#97c95c' },
            { offset: '90%', color: '#eb3573' }
        ]
    })
};
```

Pass `'radial'` instead of `'linear'` for a radial gradient.

### Pattern Fill

```ts
import { registerPattern } from 'devextreme/common/charts';

const seriesColor = {
    base: '#f5564a',
    fillId: registerPattern({
        width: 5,
        height: 5,
        template: (container) => {
            const path = document.createElementNS('http://www.w3.org/2000/svg', 'path');
            path.setAttribute('stroke', '#97c95c');
            path.setAttribute('stroke-width', 1.5);
            path.setAttribute('d', 'M 2.5 -2.5 L -2.5 2.5 M 0 5 L 5 0 M 7.5 2.5 L 2.5 7.5');
            container.appendChild(path);
        }
    })
};
```

`template` receives an SVG `<g>` container — append any valid SVG elements to it.

### Applying to a Series

```tsx
// React
<Chart>
    <CommonSeriesSettings color={seriesColor} />
</Chart>
```

```html
<!-- Angular -->
<dx-chart>
    <dxo-chart-common-series-settings [color]="seriesColor"></dxo-chart-common-series-settings>
</dx-chart>
```

### Image Fill on Point Markers

To use an image as a point marker (not a fill), use the `point.image` option — no registration needed:

```js
// jQuery
commonSeriesSettings: {
    point: {
        image: {
            url: '/img/marker.png',
            width: 20,
            height: 20
        }
    }
}
```

> **Source**: [`registerGradient`](https://js.devexpress.com/jQuery/Documentation/ApiReference/Common/Utils/viz/#registerGradienttype_options) / [`registerPattern`](https://js.devexpress.com/jQuery/Documentation/ApiReference/Common/Utils/viz/#registerPatternoptions) — `devextreme/common/charts`. jQuery equivalent: `DevExpress.common.charts.registerGradient(...)`. Also applies to PieChart, PolarChart, CircularGauge, LinearGauge.

# Predefined Themes

## CSS File Names

DevExtreme ships prebuilt CSS files in `node_modules/devextreme/dist/css/`.

### Generic Themes

| Theme | CSS file |
|---|---|
| Light | `dx.light.css` |
| Light Compact | `dx.light.compact.css` |
| Dark | `dx.dark.css` |
| Dark Compact | `dx.dark.compact.css` |
| Carmine | `dx.carmine.css` |
| Soft Blue | `dx.softblue.css` |
| Dark Moon | `dx.darkmoon.css` |
| Dark Violet | `dx.darkviolet.css` |
| Green Mist | `dx.greenmist.css` |
| Contrast | `dx.contrast.css` |
| Contrast Compact | `dx.contrast.compact.css` |

### Material Design Themes

| Theme | CSS file |
|---|---|
| Blue Light | `dx.material.blue.light.css` |
| Blue Light Compact | `dx.material.blue.light.compact.css` |
| Blue Dark | `dx.material.blue.dark.css` |
| Lime Light | `dx.material.lime.light.css` |
| Orange Light | `dx.material.orange.light.css` |
| Purple Light | `dx.material.purple.light.css` |
| Teal Light | `dx.material.teal.light.css` |

### Fluent Themes

| Theme | CSS file |
|---|---|
| Blue Light | `dx.fluent.blue.light.css` |
| Blue Dark | `dx.fluent.blue.dark.css` |
| Saas Light | `dx.fluent.saas.light.css` |
| Saas Dark | `dx.fluent.saas.dark.css` |

> For Fluent themes, export the archive from ThemeBuilder — the icon assets must be placed alongside the CSS file.

---

## Applying a Theme

### jQuery

Add a stylesheet link in `<head>`. Use local files (npm) or CDN.

```html
<!-- index.html -->
<head>
    <!-- From npm -->
    <link rel="stylesheet" href="node_modules/devextreme/dist/css/dx.light.css" />

    <!-- or from CDN -->
    <link rel="stylesheet" href="https://cdn3.devexpress.com/jslib/26.1.0/css/dx.light.css" />
</head>
```

### Angular

Add the theme to the `"styles"` array in `angular.json`:

```json
// angular.json
"architect": {
  "build": {
    "options": {
      "styles": [
        "node_modules/devextreme/dist/css/dx.material.blue.light.css",
        "src/styles.css"
      ]
    }
  }
}
```

### Vue

Import in the root entry file (Vite project):

```ts
// main.ts
import { createApp } from 'vue';
import App from './App.vue';
import 'devextreme/dist/css/dx.material.blue.light.css';

createApp(App).mount('#app');
```

### React

Import in the root entry file:

```tsx
// main.tsx
import React from 'react';
import ReactDOM from 'react-dom/client';
import App from './App';
import 'devextreme/dist/css/dx.material.blue.light.css';

ReactDOM.createRoot(document.getElementById('root')!).render(<App />);
```

---

## SCSS Import (All Frameworks)

Instead of prebuilt CSS, import a theme bundle from the SCSS source:

```scss
/* Bundle import — simplest approach */
@use 'devextreme/scss/bundles/dx.material.blue.light.scss';
```

Or import only specific components to reduce bundle size:

```scss
/* Selective component import — Generic Dark Moon example */
@use "devextreme/scss/widgets/generic/colors" with ($color: "darkmoon");
@use "devextreme/scss/widgets/generic/sizes" with ($size: "default");
@use "devextreme/scss/widgets/generic/typography";
@use "devextreme/scss/widgets/generic/icons";
@use "devextreme/scss/widgets/generic/widget";
@use "devextreme/scss/widgets/generic/card";
@use "devextreme/scss/widgets/generic/fieldset";
@use "devextreme/scss/widgets/generic/common";
@use "devextreme/scss/widgets/base/resizable";
@use "devextreme/scss/widgets/base/draggable";
@use "devextreme/scss/widgets/base/ui";
/* Only the components you need: */
@use "devextreme/scss/widgets/generic/textBox";
@use "devextreme/scss/widgets/generic/button";
@use "devextreme/scss/widgets/generic/dataGrid";
```

Available `$size` values: `default`, `compact`  
Available `$mode` values: `light`, `dark` (Material themes only)  
Available `$color` values: derived from theme name (e.g., `blue`, `darkmoon`, `carmine`)

> The SCSS internal structure (variable names, import order, folder layout) is not officially documented and may change between DevExtreme releases without notice. Use ThemeBuilder to safely customize internal variables.

---

## Color Swatches

Color swatches let you apply a secondary color scheme to a section of the page — for example, a dark sidebar next to a light content area.

### What a Swatch Is

A swatch is a CSS file where every rule is prefixed by a `.dx-swatch-xxx` selector. Wrap any HTML element with the swatch class to apply it:

```html
<div>
    <!-- Uses the primary theme -->
    <main>Main content area</main>

    <!-- Uses the dark swatch -->
    <nav class="dx-swatch-dark">
        Navigation sidebar
    </nav>
</div>
```

### Generating a Swatch with DevExtreme CLI

```bash
# Generate a "dark" swatch based on Generic Dark
npx -p devextreme-cli devextreme build-theme \
  --base-theme="generic.dark" \
  --make-swatch \
  --output-color-scheme="dark"
# Result: dx.generic.dark.css with every rule prefixed by .dx-swatch-dark
```

### Generating a Swatch with ThemeBuilder UI

1. Open [ThemeBuilder UI](https://devexpress.github.io/ThemeBuilder/).
2. Customize the theme as needed.
3. Click **Export** → enter the swatch name → check **Save as a color swatch** → **Download CSS File**.

### Constraints

- Swatches must belong to the same theme family as the main theme.
- Color swatches **do not work** for SVG-based components (Chart, PieChart, Gauge, etc.) because those use JSON-based theme configuration, not CSS.

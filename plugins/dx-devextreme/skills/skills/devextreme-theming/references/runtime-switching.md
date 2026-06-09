# Runtime Theme Switching

## Two Approaches

| Approach | Constraint | Mechanism |
|---|---|---|
| Without page reload | Same theme family only (e.g., Generic Light → Generic Dark) | `DevExpress.ui.themes.current(name)` |
| With page reload | Any theme, including cross-family | `localStorage` + `location.reload()` |

---

## Without Page Reload (Same Family)

### Step 1: Register All Themes in `index.html`

Mark all theme links with `rel="dx-theme"` and `data-theme`. Set `data-active="true"` on the initial theme.

```html
<!-- jQuery / any framework — index.html <head> -->

<!-- Generic themes -->
<link rel="dx-theme" data-theme="generic.light"    href="css/dx.light.css"    data-active="true">
<link rel="dx-theme" data-theme="generic.dark"     href="css/dx.dark.css"     data-active="false">
<link rel="dx-theme" data-theme="generic.contrast" href="css/dx.contrast.css" data-active="false">

<!-- or Material themes -->
<link rel="dx-theme" data-theme="material.blue.light" href="css/dx.material.blue.light.css" data-active="true">
<link rel="dx-theme" data-theme="material.blue.dark"  href="css/dx.material.blue.dark.css"  data-active="false">
```

For Angular, reference the CSS files in `angular.json` assets so they stay up to date with the package, then use `assets/` paths in `index.html`:

```json
// angular.json
"assets": [
  "src/favicon.ico",
  "src/assets",
  {
    "glob": "**/*",
    "input": "./node_modules/devextreme/dist/css/",
    "output": "assets"
  }
]
```

```html
<!-- Angular index.html -->
<link rel="dx-theme" data-theme="generic.light" href="assets/dx.light.css" data-active="true">
<link rel="dx-theme" data-theme="generic.dark"  href="assets/dx.dark.css"  data-active="false">
```

For Vue/React (Vite), copy the CSS files to the `public/` folder and reference them by path:

```html
<!-- Vue or React index.html (Vite) -->
<link rel="dx-theme" data-theme="generic.light" href="/dx.light.css" data-active="true">
<link rel="dx-theme" data-theme="generic.dark"  href="/dx.dark.css"  data-active="false">
```

### Step 2: Switch the Theme

```ts
// Any framework — TypeScript
import themes from 'devextreme/ui/themes';

function switchTheme(themeName: string): void {
    themes.current(themeName); // e.g., 'generic.dark'
}

// Read the current theme
const current: string = themes.current();
```

```js
// jQuery
DevExpress.ui.themes.current('generic.dark');
```

### Step 3: React to Theme Change

```ts
themes.ready(() => {
    // Fires after the new theme finishes loading
    console.log('Theme applied:', themes.current());
});
```

---

## With Page Reload (Cross-Family)

Use this approach when switching between theme families (e.g., Generic → Material).

### Step 1: Register All Themes in `index.html` with `data-active="false"`

```html
<link rel="dx-theme" data-theme="generic.light"      href="css/dx.light.css"            data-active="false">
<link rel="dx-theme" data-theme="material.blue.light" href="css/dx.material.blue.light.css" data-active="false">
```

### Step 2: Persist the Chosen Theme and Reload

```ts
// TypeScript
import themes from 'devextreme/ui/themes';

const STORAGE_KEY = 'dx-theme';

function applyStoredTheme(): void {
    const saved = localStorage.getItem(STORAGE_KEY) ?? 'generic.light';
    themes.current(saved);
}

function switchThemeWithReload(themeName: string): void {
    localStorage.setItem(STORAGE_KEY, themeName);
    location.reload();
}

// Call on app startup
applyStoredTheme();
```

---

## Switching Between Color Swatches at Runtime

Color swatches are toggled via the browser's [StyleSheet API](https://developer.mozilla.org/en-US/docs/Web/API/StyleSheet) by setting `stylesheet.disabled`. This does not require a page reload.

### jQuery

```html
<!-- index.html -->
<div id="theme-selector"></div>
```

```js
// index.js
$(() => {
    const themeMarker = 'theme.';

    $('#theme-selector').dxSelectBox({
        dataSource: ['light', 'dark'],
        value: 'light',
        onValueChanged(e) {
            const accent = e.value;
            for (const index in document.styleSheets) {
                const styleSheet = document.styleSheets[index];
                const href = styleSheet.href;
                if (!href) continue;

                const markerPos = href.indexOf(themeMarker);
                if (markerPos === -1) continue;

                const start = markerPos + themeMarker.length;
                const end = href.indexOf('.css');
                const fileNamePart = href.substring(start, end);

                if (fileNamePart.includes('custom-scheme')) {
                    styleSheet.disabled = accent !== fileNamePart.substring(fileNamePart.indexOf('.') + 1);
                }
            }
        }
    });
});
```

### Angular

```ts
// app.ts
import { Component } from '@angular/core';
import { DxSelectBoxComponent } from 'devextreme-angular/ui/select-box';
import { DxSelectBoxTypes } from 'devextreme-angular/ui/select-box';

@Component({
    standalone: true,
    selector: 'app-root',
    templateUrl: './app.html',
    imports: [DxSelectBoxComponent]
})
export class AppComponent {
    themeData = ['light', 'dark'];
    private themeMarker = 'theme.';

    onThemeChanged(e: DxSelectBoxTypes.ValueChangedEvent): void {
        const accent = e.value;
        for (const index in document.styleSheets) {
            const styleSheet = document.styleSheets[index];
            const href = styleSheet.href;
            if (!href) continue;

            const markerPos = href.indexOf(this.themeMarker);
            if (markerPos === -1) continue;

            const start = markerPos + this.themeMarker.length;
            const fileNamePart = href.substring(start, href.indexOf('.css'));

            if (fileNamePart.includes('custom-scheme')) {
                styleSheet.disabled = accent !== fileNamePart.substring(fileNamePart.indexOf('.') + 1);
            }
        }
    }
}
```

```html
<!-- app.html -->
<dx-select-box
    [dataSource]="themeData"
    [value]="themeData[0]"
    (onValueChanged)="onThemeChanged($event)">
</dx-select-box>
```

### Vue

```vue
<template>
    <DxSelectBox
        :data-source="themeData"
        :value="themeData[0]"
        @value-changed="onThemeChanged"
    />
</template>

<script setup lang="ts">
import DxSelectBox from 'devextreme-vue/select-box';

const themeData = ['light', 'dark'];
const themeMarker = 'theme.';

function onThemeChanged(e: { value: string }): void {
    const accent = e.value;
    for (const index in document.styleSheets) {
        const styleSheet = document.styleSheets[index] as CSSStyleSheet;
        const href = styleSheet.href;
        if (!href) continue;
        const markerPos = href.indexOf(themeMarker);
        if (markerPos === -1) continue;
        const fileNamePart = href.substring(markerPos + themeMarker.length, href.indexOf('.css'));
        if (fileNamePart.includes('custom-scheme')) {
            styleSheet.disabled = accent !== fileNamePart.substring(fileNamePart.indexOf('.') + 1);
        }
    }
}
</script>
```

### React

```tsx
import { useCallback } from 'react';
import { SelectBox, type SelectBoxTypes } from 'devextreme-react/select-box';

const themeData = ['light', 'dark'];
const themeMarker = 'theme.';

function ThemeSwitcher() {
    const onThemeChanged = useCallback((e: SelectBoxTypes.ValueChangedEvent) => {
        const accent = e.value as string;
        for (const index in document.styleSheets) {
            const styleSheet = document.styleSheets[index] as CSSStyleSheet;
            const href = styleSheet.href;
            if (!href) continue;
            const markerPos = href.indexOf(themeMarker);
            if (markerPos === -1) continue;
            const fileNamePart = href.substring(markerPos + themeMarker.length, href.indexOf('.css'));
            if (fileNamePart.includes('custom-scheme')) {
                styleSheet.disabled = accent !== fileNamePart.substring(fileNamePart.indexOf('.') + 1);
            }
        }
    }, []);

    return (
        <SelectBox
            dataSource={themeData}
            defaultValue={themeData[0]}
            onValueChanged={onThemeChanged}
        />
    );
}
```

> The swatch file names in the examples follow the pattern `theme.custom-scheme.[light|dark].css`. Adjust the `includes('custom-scheme')` check to match your actual file names.

---

## Auto Dark Mode (`prefers-color-scheme`)

Automatically apply a dark or light theme based on the user's OS preference. Combine `window.matchMedia` with `themes.current()`, then listen for changes.

> **Prerequisite:** Register both light and dark theme links in `index.html` with `data-active="false"` as shown in the [With Page Reload](#with-page-reload-cross-family) section above. Same-family light/dark pairs (e.g., `generic.light` / `generic.dark`) do not require a reload.

### Utility (framework-agnostic TypeScript)

```ts
// theme-auto.ts
import themes from 'devextreme/ui/themes';

const LIGHT_THEME = 'generic.light';
const DARK_THEME  = 'generic.dark';

function applySystemTheme(): void {
    const prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
    themes.current(prefersDark ? DARK_THEME : LIGHT_THEME);
}

// Apply on load
applySystemTheme();

// React to OS-level changes while the page is open
window.matchMedia('(prefers-color-scheme: dark)')
    .addEventListener('change', applySystemTheme);
```

Call this once at app startup (before any DevExtreme components render) and it stays in sync automatically.

### React

```tsx
// main.tsx — call before ReactDOM.createRoot()
import themes from 'devextreme/ui/themes';

const mq = window.matchMedia('(prefers-color-scheme: dark)');

function applySystemTheme(): void {
    themes.current(mq.matches ? 'generic.dark' : 'generic.light');
}

applySystemTheme();
mq.addEventListener('change', applySystemTheme);
```

### Angular

```ts
// app.config.ts — use APP_INITIALIZER
import { ApplicationConfig, APP_INITIALIZER } from '@angular/core';
import themes from 'devextreme/ui/themes';

function initTheme(): () => void {
    return () => {
        const mq = window.matchMedia('(prefers-color-scheme: dark)');
        const apply = () => themes.current(mq.matches ? 'generic.dark' : 'generic.light');
        apply();
        mq.addEventListener('change', apply);
    };
}

export const appConfig: ApplicationConfig = {
    providers: [
        { provide: APP_INITIALIZER, useFactory: initTheme, multi: true }
    ]
};
```

### Vue

```ts
// main.ts — call before createApp()
import themes from 'devextreme/ui/themes';

const mq = window.matchMedia('(prefers-color-scheme: dark)');
const applySystemTheme = () => themes.current(mq.matches ? 'generic.dark' : 'generic.light');

applySystemTheme();
mq.addEventListener('change', applySystemTheme);
```

### Combining with a Manual Toggle

If the user can also override the theme manually, persist their choice in `localStorage` and only fall back to the OS preference when no saved value exists:

```ts
import themes from 'devextreme/ui/themes';

const STORAGE_KEY = 'dx-theme';
const mq = window.matchMedia('(prefers-color-scheme: dark)');

function resolveTheme(): string {
    return localStorage.getItem(STORAGE_KEY)
        ?? (mq.matches ? 'generic.dark' : 'generic.light');
}

function applyTheme(name: string, persist = false): void {
    if (persist) localStorage.setItem(STORAGE_KEY, name);
    themes.current(name);
}

// On startup — respect saved preference, fall back to OS
applyTheme(resolveTheme());

// Keep in sync when no manual preference is saved
mq.addEventListener('change', () => {
    if (!localStorage.getItem(STORAGE_KEY)) applyTheme(resolveTheme());
});
```

> Cross-family switches (e.g., auto-switching between `generic.dark` and `material.blue.dark`) require a page reload. In that case, call `localStorage.setItem(STORAGE_KEY, themeName)` followed by `location.reload()` instead of `themes.current()`.

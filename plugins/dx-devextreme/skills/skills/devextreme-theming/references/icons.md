# Icons

## Built-In Icon Library

DevExtreme ships an icon library with both **font icons** and **SVG icons** that work across all themes.

Browse the full list on the DevExtreme demo site:
[https://js.devexpress.com/jQuery/Demos/WidgetsGallery/Demo/Button/Icons/](https://js.devexpress.com/jQuery/Demos/WidgetsGallery/Demo/Button/Icons/)

Source files in the DevExtreme repository:
- Font icons: `packages/devextreme-scss/icons/`
- SVG icons: `packages/devextreme-scss/images/icons/`

> **macOS rendering note:** If icons appear with uneven line weights on macOS, add `-webkit-font-smoothing: antialiased;` to the icon elements.

---

## Using Icons in Components

Any DevExtreme component that has an `icon` property accepts a built-in icon name as a string:

```js
// jQuery
$('#saveButton').dxButton({ icon: 'save' });
```

```html
<!-- Angular -->
<dx-button icon="save" text="Save"></dx-button>
```

```html
<!-- Vue -->
<DxButton icon="save" text="Save" />
```

```tsx
// React
<Button icon="save" text="Save" />
```

### Icons in Data-Bound Items

Components with item templates (ContextMenu, List, Menu, TreeView, etc.) accept `icon` in their data items:

```js
// jQuery
$('#contextMenu').dxContextMenu({
    dataSource: [
        { text: 'Zoom In',  icon: 'plus'     },
        { text: 'Share',    icon: 'message'  },
        { text: 'Download', icon: 'download' }
    ]
});
```

```html
<!-- Angular -->
<dx-context-menu [dataSource]="items"></dx-context-menu>
```

```ts
// app.component.ts
items = [
    { text: 'Zoom In',  icon: 'plus'     },
    { text: 'Share',    icon: 'message'  },
    { text: 'Download', icon: 'download' }
];
```

---

## Icons in Plain HTML Elements

Use the `dx-icon-{IconName}` CSS class to render a built-in icon on any HTML element. Use inline elements (`<i>`, `<span>`):

```html
<i class="dx-icon-home"></i>
<span class="dx-icon-search"></span>
```

The DevExtreme theme CSS must already be loaded on the page for this to work.

---

## Customizing Icon Appearance

DevExtreme adds the `.dx-icon` class to every generated icon element, and `.dx-icon-{name}` for named icons. Override these with CSS to change color, size, or weight.

```css
/* Global — resize all icons inside #toolbar */
#toolbar .dx-icon {
    font-size: 20px;
    color: #1a73e8;
}

/* Target a specific icon type */
#toolbar .dx-icon-refresh {
    color: #e53935;
}
```

```css
/* Angular — use ::ng-deep to pierce view encapsulation */
::ng-deep #toolbar .dx-icon {
    font-size: 20px;
    color: #1a73e8;
}
```

```vue
<!-- Vue — omit the scoped attribute -->
<style>
#toolbar .dx-icon { font-size: 20px; }
</style>
```

To customize icons added directly to HTML (via `dx-icon-*` class), define your own class and combine selectors:

```html
<i class="dx-icon-home my-icon"></i>
```

```css
.my-icon { font-size: 24px; color: navy; }
```

---

## Custom Images as Icons

### Image URL

Pass a URL directly to the `icon` property. This wraps the image in an `<img>` tag — CSS rules cannot style the image content:

```js
$('#myButton').dxButton({ icon: '/assets/icons/star.png' });
```

### Base64 Encoded Image

For icons that should not require an HTTP request, define a `.dx-icon-{name}` CSS class with a Base64 background image, then reference the name:

```css
/* Global CSS */
.dx-icon-mystar {
    background-image: url('data:image/png;base64,...BASE64...');
    background-repeat: no-repeat;
    background-position: 0 0;
}
```

```js
$('#myButton').dxButton({ icon: 'mystar' });
```

Angular requires `::ng-deep`:

```css
::ng-deep .dx-icon-mystar {
    background-image: url('data:image/png;base64,...BASE64...');
    background-repeat: no-repeat;
    background-position: 0 0;
}
```

### State-Specific Icons

Use component state classes to swap icons on interaction:

```css
/* Show a different star when the tab is selected */
.dx-tab-selected .dx-icon-mystar {
    background-image: url('data:image/png;base64,...BASE64_SELECTED...');
}
```

> State classes like `dx-tab-selected` are not publicly documented — inspect the DOM to find the correct selectors.

---

## SVG Icons

DevExtreme ships the built-in library as SVG files in addition to the font format. Three usage modes:

### 1. URL reference (image tag — not CSS-stylable)

```js
$('#myButton').dxButton({ icon: 'https://path/to/icon.svg' });
```

### 2. Inline SVG string (CSS-stylable)

Pass the raw SVG markup as a string. The content is rendered directly in the DOM and can be styled with CSS:

```js
const svgContent = '<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24">...</svg>';
$('#myButton').dxButton({ icon: svgContent });
```

### 3. Import (Vite / Webpack — wraps in `<img>`, not CSS-stylable)

```ts
import myIcon from './assets/icon.svg';
$('#myButton').dxButton({ icon: myIcon });
```

> **Security:** SVG files can contain executable code. Only use SVG icons from trusted sources.

---

## External Icon Libraries

DevExtreme renders icons as `<i class="dx-icon ...">` elements. Any CSS icon library that works with `<i>` elements is compatible. Install the library normally, then pass its class names to the `icon` property:

```js
// jQuery
$('#btn').dxButton({ icon: 'fas fa-home' });          // Font Awesome 6/5
$('#btn').dxButton({ icon: 'fa fa-home' });            // Font Awesome 4
$('#btn').dxButton({ icon: 'glyphicon glyphicon-home' }); // Glyphicons
$('#btn').dxButton({ icon: 'icon ion-md-home' });     // Ionicons
$('#btn').dxButton({ icon: 'ms-Icon ms-Icon--Home' }); // Fabric/Fluent UI
```

```html
<!-- Angular -->
<dx-button icon="fas fa-home"></dx-button>
<dx-button icon="ms-Icon ms-Icon--Home"></dx-button>
```

```html
<!-- Vue -->
<DxButton icon="fas fa-home" />
```

```tsx
// React
<Button icon="fas fa-home" />
```

---

## Fluent Theme: Icon Archive Setup

Fluent themes require a separate icon archive. When exporting from ThemeBuilder, choose **Download Archive** — the ZIP contains both the CSS file and the icon assets.

After extracting, your project should have this structure next to your custom CSS file:

```
src/
  themes/
    dx.fluent.custom.light.css   ← your exported theme file
    icons/                        ← extracted from the archive
      dxicons.woff2
      dxicons.woff
      dxicons.ttf
    fonts/                        ← extracted from the archive (if included)
      ...
```

The `icons/` and `fonts/` folders must be **sibling directories of the CSS file**. The theme CSS references them with relative paths.

If you are using a predefined Fluent theme (not a custom ThemeBuilder export), copy the folders from `node_modules`:

```bash
# Copy icons and fonts alongside your theme CSS
cp -r node_modules/devextreme/dist/css/icons  src/themes/
cp -r node_modules/devextreme/dist/css/fonts  src/themes/
```

For Vite (React/Vue), add the copy step to `vite.config.ts` or the build script so it runs automatically. For Angular, use the `assets` array in `angular.json`:

```json
// angular.json — inside "assets" of the build target
{
    "glob": "**/*",
    "input": "node_modules/devextreme/dist/css/icons",
    "output": "/icons"
},
{
    "glob": "**/*",
    "input": "node_modules/devextreme/dist/css/fonts",
    "output": "/fonts"
}
```

> **Source:** [DevExtreme Icons guide](https://js.devexpress.com/jQuery/Documentation/Guide/Themes_and_Styles/Icons/)

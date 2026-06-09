# ThemeBuilder

## When to Use ThemeBuilder

Use ThemeBuilder when you need colors, fonts, or spacing that differ from any predefined theme. ThemeBuilder outputs a standard CSS file that you register exactly like a predefined theme.

> ThemeBuilder works with **HTML-based** components only. For SVG-based components (Chart, PieChart, Gauge, etc.), use the component's `palette` and `theme` options instead.

---

## ThemeBuilder UI

URL: [https://devexpress.github.io/ThemeBuilder/](https://devexpress.github.io/ThemeBuilder/)

### Workflow

1. **Select a base theme** — Generic, Material, or Fluent. Pick a color variant and light/dark mode.
2. **Customize** using the settings panel:
   - **Basic Settings** — colors shared across components (accent, background, border, text).
   - **Typography Settings** — font family, sizes.
   - **Per-component sections** — fine-grained control for DataGrid, Button, Form, etc.
3. **Preview** updates automatically in the right panel.
4. **Export** the result:
   - Click **Export** → name the color scheme.
   - Optionally check **Save as a color swatch** for section-level theming.
   - Click **Download CSS File** (for Generic/Material) or **Download Archive** (for Fluent — includes icons).

### Save Metadata for Later

Before closing ThemeBuilder, always save your theme's metadata so you can resume customization:

- Click **Export** → **Download Metadata File** → saves a `.json` file.
- To resume: click **Import** → **Upload File** → select the `.json`.

> DevExtreme may remap or remove SCSS constants between releases. Saved metadata lets you re-import and re-export against the latest version without rebuilding from scratch.

### Register the Exported CSS

After downloading, add the CSS file to your project and register it the same way as a predefined theme (see [predefined-themes.md](predefined-themes.md)).

Copy the `icons/` and `fonts/` folders from `node_modules/devextreme/dist/css/` into the same directory as your custom theme file.

---

## ThemeBuilder CLI

The CLI generates themes programmatically — useful for CI/CD pipelines or scripted builds.

### Install

```bash
npm install -g devextreme-cli
# or use npx without global install
```

### Build a Custom Theme

```bash
# Build a custom theme based on Material Blue Light, export as "brand"
npx -p devextreme-cli devextreme build-theme \
  --base-theme="material.blue.light" \
  --output-color-scheme="brand" \
  --input-file="metadata.json"
# Output: dx.material.brand.css
```

### Build a Theme with a Custom Accent Color

> **Common mistake:** `--output-color-scheme` sets the **output file name**, not the accent color. To change colors you must supply a metadata JSON file via `--input-file`.

**Step 1 — Create a metadata JSON file** with the color tokens you want to override:

```json
// pink-theme.json
{
    "items": [
        { "key": "$base-accent", "value": "rgba(233, 30, 99, 1)" },
        { "key": "$base-text-color", "value": "rgba(33, 33, 33, 0.87)" }
    ],
    "baseTheme": "material.blue.light",
    "outputColorScheme": "pink",
    "version": "25.2.7"
}
```

Key color token names (Generic / Material themes):

| Token | Role |
|---|---|
| `$base-accent` | Primary / accent color |
| `$base-text-color` | Main text color |
| `$base-bg` | Background color |
| `$base-border-color` | Border color |
| `$base-success` | Success state color |
| `$base-danger` | Danger / error color |
| `$base-warning` | Warning state color |

To get the full list of available tokens for a base theme, export them first:

```bash
# Export all variable names for generic.light as SCSS
npx -p devextreme-cli devextreme export-theme-vars \
  --base-theme="generic.light" \
  --output-format="scss" \
  --output-file="theme-vars.scss" \
  --base
```

**Step 2 — Build the theme using the JSON file:**

```bash
npx -p devextreme-cli devextreme build-theme \
  --base-theme="material.blue.light" \
  --input-file="pink-theme.json" \
  --output-color-scheme="pink"
# Output: dx.material.pink.css
```

The `version` field in the JSON must match the `devextreme` package version in your project. Mismatches will cause a build error.

---

### Build a Color Swatch

```bash
npx -p devextreme-cli devextreme build-theme \
  --base-theme="generic.dark" \
  --make-swatch \
  --output-color-scheme="dark"
# Output: dx.generic.dark.css (all rules prefixed with .dx-swatch-dark)
```

### Common CLI Options

| Option | Description |
|---|---|
| `--base-theme` | Base theme name (e.g., `generic.light`, `material.blue.dark`) |
| `--output-color-scheme` | Name for the output color scheme |
| `--input-file` | Path to a metadata JSON file from a previous ThemeBuilder export |
| `--make-swatch` | Generates a color swatch instead of a full theme |
| `--output-file` | Custom output file path |
| `--output-format` | Output format: `css` (default), `scss`, or `less` |
| `--widgets` | Comma-separated list of components to include (reduces file size) |

### Export Metadata from CLI

```bash
# Export metadata from an existing theme for future re-customization
npx -p devextreme-cli devextreme export-theme-meta \
  --base-theme="generic.light" \
  --output-file="theme-metadata.json"
```

---

## Applying the Custom Theme

The output CSS file is used identically to a predefined theme file:

```ts
// React / Vue
import 'path/to/dx.material.brand.css';
```

```json
// Angular — angular.json
"styles": ["src/assets/dx.material.brand.css"]
```

```html
<!-- jQuery -->
<link rel="stylesheet" href="css/dx.material.brand.css" />
```

For runtime switching with the custom theme, use the `data-theme` attribute with the name from the ThemeBuilder metadata's `baseTheme` field (visible in the **Copy Metadata** output).

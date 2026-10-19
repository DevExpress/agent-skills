# Color Scheme / Theming

## When to Use This Reference

Use this when you need to:
- Switch the Document Viewer or Report Designer to a different predefined color scheme (Light, Dark, Carmine, etc.)
- Build and apply a custom color scheme for either component
- Understand which CSS files control which component's appearance

Both the Document Viewer and the Report Designer (including its integrated Preview) use the same color scheme mechanism — linked CSS files. The instructions below apply to either component; only the CSS filenames used in the view differ.

## Predefined Color Schemes

Both components ship two families of predefined color schemes: **Generic** and **Material Design**.

### Generic Color Schemes

8 schemes, each built from 9 base colors: **Light** (default), **Dark**, **Carmine**, **Dark Moon**, **Soft Blue**, **Dark Violet**, **Green Mist**, **Contrast**.

Only the default **Light** theme can rely on one CSS file. Other themes require **two** CSS files:

- `dx.{color_scheme}.css` from `node_modules/devextreme-dist/css` — the DevExtreme widget theme
- `dx-analytics.{color_scheme}.css` from `node_modules/@devexpress/analytics-core/dist/css` — the component's own theme

where `{color_scheme}` is one of `light`, `dark`, `carmine`, `darkmoon`, `softblue`, `darkviolet`, `greenmist`, `contrast`. 

Copy `dx.{color_scheme}.css` to `wwwroot/css` and `dx-analytics.{color_scheme}.css` to `wwwroot/css/reporting` — the same two destinations [getting-started.md](getting-started.md) Step 3 uses for their Light counterparts. Link them **after the component's own CSS** (e.g. `dx-webdocumentviewer.css`), DevExtreme theme first — regardless of whether that CSS is a direct `<link>` (the default Pattern A) or a bundle (`viewer.part.bundle.css`, Pattern B).

If switching from the default Light scheme, remove **both** Light links — `~/css/dx.light.css` and `~/css/reporting/dx-analytics.light.css` — and replace them with the new scheme's two files. Leave the `~/css/reporting/dx-analytics.common.css` link in place; it is scheme-independent.

> The snippets below show the component's own CSS/JS in abbreviated form — just the file the theme must follow — to keep the focus on theme-CSS placement. The complete tag list (jQuery, Knockout, `dx.all.js`, analytics-core, …) in load order is in [getting-started.md](getting-started.md) Step 9.

**Document Viewer** — apply the Dark Violet scheme:

```cshtml
<link rel="stylesheet" href="~/css/reporting/dx-webdocumentviewer.css" />
<link rel="stylesheet" href="~/css/dx.darkviolet.css" />
<link rel="stylesheet" href="~/css/reporting/dx-analytics.darkviolet.css" />

<script src="~/js/reporting/dx-webdocumentviewer.min.js"></script>

@Html.DevExpress().WebDocumentViewer("DocumentViewer").Height("1000px").Bind("SalesReport")
```

**Report Designer** — apply the Dark Violet scheme:

```cshtml
<link rel="stylesheet" href="~/css/reporting/dx-webdocumentviewer.css" />
<link rel="stylesheet" href="~/css/reporting/dx-reportdesigner.css" />
<link rel="stylesheet" href="~/css/dx.darkviolet.css" />
<link rel="stylesheet" href="~/css/reporting/dx-analytics.darkviolet.css" />

<script src="~/js/reporting/dx-webdocumentviewer.min.js"></script>
<script src="~/js/reporting/dx-reportdesigner.min.js"></script>

@(Html.DevExpress().ReportDesigner("reportDesigner")
    .Height("1000px")
    .Bind("SalesReport"))
```

> If a color scheme is already referenced from the component's own CSS, its CSS file doesn't need to be linked explicitly. Usually only the Light scheme is included (whether via a direct `dx-analytics.light.css` link or a bundle); other schemes need to be linked explicitly.

> **Compact variants.** Each scheme also ships a `.compact.css` pair (`dx.dark.compact.css` + `dx-analytics.dark.compact.css`). Pick either, never mix a compact file with a non-compact one.

### Material Design Color Schemes

10 schemes, each pairing an accent color (**Blue**, **Lime**, **Orange**, **Purple**, **Teal**) with a base (**Light** or **Dark**): Blue Light, Blue Dark, Lime Light, Lime Dark, Orange Light, Orange Dark, Purple Light, Purple Dark, Teal Light, Teal Dark.

Unlike Generic schemes, a Material Design scheme requires **two** CSS files linked together — the DevExtreme widget theme and the component's own analytics theme — plus the Material icon font:

- `dx.material.{accent}.{base}.css` from `node_modules/devextreme-dist/css`
- `dx-analytics.material.{accent}.{base}.css` from `node_modules/@devexpress/analytics-core/dist/css`

where `{accent}` is one of `blue`, `lime`, `orange`, `purple`, `teal` and `{base}` is `light` or `dark`.

Copy them to the same destinations as the Generic schemes — `dx.material.*.css` to `wwwroot/css`, `dx-analytics.material.*.css` to `wwwroot/css/reporting` — and link them after the component's own CSS, DevExtreme theme first:

**Document Viewer** — apply the Material Orange Light scheme:

```cshtml
<link rel="stylesheet" href="~/css/reporting/dx-webdocumentviewer.css" />
<link rel="stylesheet" href="~/css/dx.material.orange.light.css" />
<link rel="stylesheet" href="~/css/reporting/dx-analytics.material.orange.light.css" />

<script src="~/js/reporting/dx-webdocumentviewer.min.js"></script>

@Html.DevExpress().WebDocumentViewer("DocumentViewer").Height("1000px").Bind("SalesReport")
```

Report Designer follows the same pattern, adding the designer's own tags (`dx-reportdesigner.css`/`.min.js`) alongside the viewer's client-asset tags, as shown above for Generic schemes.

Material Design themes also require their icon font. Copy the following files from `node_modules/devextreme-dist/css/icons` to `wwwroot/css/icons` (e.g. via LibMan):

```
dxiconsmaterial.ttf
dxiconsmaterial.woff
dxiconsmaterial.woff2
```

> Without the Material icon font copied to `wwwroot`, toolbar and menu icons render as blank boxes even though the color scheme itself applies correctly.

Material themes also pull the **Roboto** family from `node_modules/devextreme-dist/css/fonts`. Add a second LibMan entry with `"destination": "wwwroot/css/fonts"` and `"files": [ "Roboto-*.ttf", "Roboto-*.woff", "Roboto-*.woff2" ]` — all four weights (300/400/500/700) are referenced, so copy the family, not the weights named in whatever 404 you happen to be looking at.

> **Do not** clear a font 404 by pointing the page at a different theme stylesheet (e.g. swapping the Material bundle for `dx.light.css`). That silently changes the app's color scheme and leaves every other page on the old theme still broken.

## Custom Color Scheme

Building a custom color scheme requires interactive visual design (color pickers, live preview) that you, as an AI agent, cannot perform reliably. **Do not** try to drive the demo site or a theme builder yourself. Instead:

1. **Ask the user to design the scheme themselves** and send you the resulting CSS files. Tell them to:
   - Open the [Color Scheme Customization demo](https://demos.devexpress.com/xtrareportsdemos/WebSpecificFeatures/ColorSchemeCustomization.aspx?ShowDesigner=true).
   - Select a base predefined color scheme from the demo's drop-down, then adjust individual colors with the color picker — changes apply instantly in the preview.
   - Click **Save Changes** to download `{color_scheme}.customization.zip`, which contains:
     - `reporting.{color_scheme}.custom.css` — component appearance styles.
     - `devextreme.{color_scheme}.custom.css` — DevExtreme widget styles.
     - `dx.themebuilder.metadata.json` — metadata for further edits in the [DevExtreme Theme Builder](https://devexpress.github.io/ThemeBuilder/), if the user wants finer control.
   - Extract the zip and share the two CSS files with you, along with which base scheme they started from.

2. **Once the user provides the CSS files**, integrate them yourself:
   - Copy the two CSS files into `wwwroot/css` in the ASP.NET Core project.
   - Link them in the correct order in the view, after the base scheme CSS.

   Example result (Document Viewer with Dark Violet custom scheme):

   ```cshtml
   <link rel="stylesheet" href="~/css/reporting/dx-webdocumentviewer.css" />
   <link rel="stylesheet" href="~/css/reporting/dx-analytics.darkviolet.css" />
   <link href="~/css/devextreme.darkviolet.custom.css" rel="stylesheet" />
   <link href="~/css/reporting.darkviolet.custom.css" rel="stylesheet" />

   <script src="~/js/reporting/dx-webdocumentviewer.min.js"></script>

   @Html.DevExpress().WebDocumentViewer("DocumentViewer").Height("1000px").Bind("SalesReport")
   ```

   Report Designer follows the same pattern, including both `dx-reportdesigner.css` and `dx-reportdesigner.min.js`.

## Custom Color Scheme — CSS Structure

In case you need to understand or hand-tweak a downloaded custom scheme: `reporting.*.custom.css` holds the component's 9 base palette classes (`.dxd-back-primary`/`-secondary`, `.dxd-border-primary`/`-secondary`, `.dxd-icon-fill`, `.dxd-text-primary`/`-secondary`/`-info`), each scoped under `.dx-designer-viewport` so it applies only inside the reporting component; `devextreme.*.custom.css` holds the DevExtreme widget styles from the [DevExtreme Theme Builder](https://devexpress.github.io/ThemeBuilder/). See the [Themes and Styles reference](https://docs.devexpress.com/XtraReports/403927) for the full class list.

## Related

- Full predefined theme filename reference, including compact variants and other frameworks (ASP.NET MVC, Web Forms, JS): [Themes and Styles in Reporting Components for Web](https://docs.devexpress.com/XtraReports/403927).

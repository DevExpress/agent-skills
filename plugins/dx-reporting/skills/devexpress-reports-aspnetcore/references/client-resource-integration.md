# Client Resource Integration Patterns — ASP.NET Core

## When to Use This Reference

The **decision rule** for choosing a pattern lives inline in Phase 2 of the Integration track workflow in [../SKILL.md](../SKILL.md) — you do not need this file to decide. Load this file **only when that rule selects Pattern B (Bundled) or Pattern C (Reuse Existing Bundle)**: it holds the Bundled `bundleconfig.json`/`libman.json` templates and the Pattern C guardrails. The default — **Pattern A (Direct References)** — is documented inline in [getting-started.md](getting-started.md) / [getting-started-razor-pages.md](getting-started-razor-pages.md), so an agent on the default path doesn't need this file at all. The pattern table is repeated below for reference.

DevExpress ships two officially documented ways to register these assets, and a third that applies when the app already has a working bundler:

| Pattern | Mechanism | Best for |
|---|---|---|
| **A — Direct References** | `package.json` + `libman.json` (copies **all** JS/CSS, not just icons) + plain `<script>`/`<link>` tags; no `bundleconfig.json`, no `BuildBundlerMinifier` | **Default.** Integrating into an existing application — the common case. Matches the official DevExtreme/Razor Pages quick start. Full templates inline in `getting-started.md`/`getting-started-razor-pages.md`. |
| **B — Bundled** | `BuildBundlerMinifier` NuGet + `bundleconfig.json`; LibMan copies only icon fonts | Greenfield/demo projects, or an explicit request for a minified build; matches the official MVC quick start tutorial. Templates below. |
| **C — Reuse Existing Bundle** | Add DevExpress entries to whatever bundler the app already uses | An existing app whose current bundler setup can precisely accommodate the required assets, load order, and Designer self-containment — **only with the developer's explicit permission** |

---

## Pattern A — Direct References (`package.json` + `libman.json`, no bundling) — the default

This is the default pattern for adding a Viewer/Designer to an application that doesn't already bundle its client assets. LibMan does all the work — copying files from `node_modules` into `wwwroot` — except it copies each file as-is (no concatenation, no minification) and you reference each copied file with its own `<script>`/`<link>` tag.

The **full templates** are inline in the getting-started walkthroughs:

- **MVC:** [getting-started.md](getting-started.md) — `package.json` (Step 2), the Viewer and Designer `libman.json` (Steps 3–4), the `_Layout.cshtml` scope rule (Step 8), and the direct `<script>`/`<link>` tag lists in load order (Step 9).
- **Razor Pages:** [getting-started-razor-pages.md](getting-started-razor-pages.md) — the same Steps 2–4 and 8–9. The `libman.json` content is identical to MVC; only the layout/page wiring differs (`@page`, `Pages/`, and `app.MapDefaultControllerRoute()` in `Program.cs`).

Complete examples of this pattern: [examples/quickstart-viewer.md](../examples/quickstart-viewer.md) and [examples/quickstart-designer.md](../examples/quickstart-designer.md).

There is no "self-contained bundle" concept in this pattern — each script tag loads independently. **Don't add a second, duplicate `<script>` tag for a file already loaded on the page.** The Designer's script/link list is a **superset** of the Viewer's — it shares every viewer-level file and adds the query builder, designer, and ace-builds files. Load it once; the Designer list is not an addition on top of a separately-loaded viewer page.

### Troubleshooting (Direct References)

| Symptom | Cause | Fix |
|---|---|---|
| HTTP 404 for `~/js/reporting/dx-webdocumentviewer.min.js` (or any `~/js/…`/`~/css/…` path referenced by a `<script>`/`<link>` tag) | LibMan didn't copy the file — `npm install` didn't run before build, or the `libman.json` `destination`/`files` entry doesn't match the `<script>`/`<link>` `src`/`href` | Run `npm install` before `dotnet build`; verify the `libman.json` entry's `destination` folder matches the tag's path exactly |
| Script tag 200s but `DevExpress is not defined` | A `<script>` tag was pasted in the wrong order, or a load-order-critical file (`dx.all.js`) was left out when copying the viewer's tag list onto a new page | Re-check the tag list against the dependencies in [../SKILL.md](../SKILL.md) Constraint 2 (load order) — Bootstrap's position is free; **ace's is not** (it must precede `dx.all.js`) |
| Designer's Expression Editor is a blank box, no line numbers, **no console error** | `ace.js` tagged after the DevExpress scripts | Move the ace tag above `dx.all.js` |
| Same file loaded twice (duplicate registration errors) | Both `_Layout.cshtml` and the reporting page tag the same file, or a LibMan-copied library duplicates one the app already ships in `wwwroot/lib` | Remove the duplicate tag; keep reporting-only libraries scoped to the reporting page(s), and keep the app's existing copy where it has one |
| Viewer works, but the app's own navbar/dropdowns/modals break | A second, older Bootstrap (npm 4.x) was registered next to the template's 5.x, and/or its stylesheet replaced, while the layout's markup uses `data-bs-*` | Drop Bootstrap and jQuery from the reporting registration; keep the versions the app's markup was written against. `scripts/Verify-ReportingPage.ps1` reports duplicate loads |

---

## Pattern B — Bundled (`BuildBundlerMinifier` + `bundleconfig.json`)

Use this for greenfield/demo projects, or when the user explicitly wants a minified build. It adds the `BuildBundlerMinifier` NuGet package (Pattern A does not), plus a `bundleconfig.json` that concatenates the `node_modules` files into a handful of `wwwroot/*.bundle.js`/`*.bundle.css` outputs. This matches the official DevExpress MVC quick start tutorial.

> **LibMan is still required.** Under Pattern B its role narrows to the icon fonts, but it is not optional and `BuildBundlerMinifier` does **not** replace it: `Microsoft.Web.LibraryManager.Build` must still be installed, or `libman.json` is never processed and the DevExtreme icon fonts are never copied — the viewer renders, but every toolbar icon is a tofu box. Pattern B needs **both** packages; `BuildBundlerMinifier` is the *additional* one.

### package.json (same dependencies regardless of pattern)

```json
{
  "version": "1.0.0",
  "name": "asp.net",
  "private": true,
  "dependencies": {
    "bootstrap": "^4.3.1",
    "devextreme-dist": "26.1-stable",
    "@devexpress/analytics-core": "26.1-stable",
    "devexpress-reporting": "26.1-stable"
  }
}
```

> **Keep `bootstrap` here even if the app already has one in `wwwroot/lib`.** It is an exception to the no-duplication rule because `package.json` does not register anything on the page.

Run `npm install` **before** `dotnet build` — LibMan's `filesystem` provider reads from `node_modules/` at build time (LIB002 if missing).

> **Optional — inline Rich Text Editor (Designer only).** For editable `XRRichText` controls, add `"devexpress-richedit": "26.1-stable"` (its `jszip` dependency comes transitively) and its files to the designer bundles below. [Docs](https://docs.devexpress.com/XtraReports/401255).

### bundleconfig.json — Document Viewer only (MVC)

> **Before copying, drop any entry for a library the host app already loads.** "Greenfield" means `dotnet new` defaults are present — **not** that jQuery/Bootstrap are absent: the templates ship both in `wwwroot/lib` and tag them in `_Layout.cshtml`. Bundling the npm copies too loads each twice, at a conflicting major version (npm `bootstrap@4.x` vs. the template's 5.x, whose `data-bs-*` markup the layout needs). Keep the app's copies — moving their tags from the end of `<body>` into `<head>` so they precede the helper's inline init script — and bundle only what's missing: for a Viewer on a stock template, Knockout plus the DevExpress files.

```json
[
  {
    "outputFileName": "wwwroot/css/thirdparty.bundle.css",
    "inputFiles": [
      "node_modules/bootstrap/dist/css/bootstrap.min.css",
      "node_modules/devextreme-dist/css/dx.light.css"
    ],
    "minify": { "enabled": false, "adjustRelativePaths": false }
  },
  {
    "outputFileName": "wwwroot/css/viewer.part.bundle.css",
    "inputFiles": [
      "node_modules/@devexpress/analytics-core/dist/css/dx-analytics.common.css",
      "node_modules/@devexpress/analytics-core/dist/css/dx-analytics.light.css",
      "node_modules/devexpress-reporting/dist/css/dx-webdocumentviewer.css"
    ],
    "minify": { "enabled": false, "adjustRelativePaths": false }
  },
  {
    "outputFileName": "wwwroot/js/thirdparty.bundle.js",
    "inputFiles": [
      "node_modules/jquery/dist/jquery.min.js",
      "node_modules/knockout/build/output/knockout-latest.js",
      "node_modules/bootstrap/dist/js/bootstrap.min.js"
    ],
    "minify": { "enabled": false },
    "sourceMap": false
  },
  {
    "outputFileName": "wwwroot/js/viewer.part.bundle.js",
    "inputFiles": [
      "node_modules/devextreme-dist/js/dx.all.js",
      "node_modules/@devexpress/analytics-core/dist/js/dx-analytics-core.min.js",
      "node_modules/devexpress-reporting/dist/js/dx-webdocumentviewer.min.js"
    ],
    "minify": { "enabled": false },
    "sourceMap": false
  }
]
```

#### Pre-trimmed variant — stock `dotnet new mvc`/`webapp` scaffold (the usual case)

When Phase 1 finds the template's jQuery and Bootstrap already tagged in `_Layout.cshtml` — true of every stock scaffold — applying the trim above yields exactly this. Transcribe it as-is. **Keep these four `outputFileName` values even though `thirdparty.bundle.js` now holds only Knockout.**

```json
[
  {
    "outputFileName": "wwwroot/css/thirdparty.bundle.css",
    "inputFiles": [ "node_modules/devextreme-dist/css/dx.light.css" ],
    "minify": { "enabled": false, "adjustRelativePaths": false }
  },
  {
    "outputFileName": "wwwroot/css/viewer.part.bundle.css",
    "inputFiles": [
      "node_modules/@devexpress/analytics-core/dist/css/dx-analytics.common.css",
      "node_modules/@devexpress/analytics-core/dist/css/dx-analytics.light.css",
      "node_modules/devexpress-reporting/dist/css/dx-webdocumentviewer.css"
    ],
    "minify": { "enabled": false, "adjustRelativePaths": false }
  },
  {
    "outputFileName": "wwwroot/js/thirdparty.bundle.js",
    "inputFiles": [ "node_modules/knockout/build/output/knockout-latest.js" ],
    "minify": { "enabled": false },
    "sourceMap": false
  },
  {
    "outputFileName": "wwwroot/js/viewer.part.bundle.js",
    "inputFiles": [
      "node_modules/devextreme-dist/js/dx.all.js",
      "node_modules/@devexpress/analytics-core/dist/js/dx-analytics-core.min.js",
      "node_modules/devexpress-reporting/dist/js/dx-webdocumentviewer.min.js"
    ],
    "minify": { "enabled": false },
    "sourceMap": false
  }
]
```

`thirdparty.bundle.css`/`.js` load in `_Layout.cshtml`'s `<head>`; `viewer.part.bundle.css`/`.js` load on the viewer page above the helper.

> **Renaming these outputs, or moving a file between them, is a *structural* adaptation — not the name adaptation Step 3.0 permits.** If you do it anyway, say so in the Step 3.0 manifest with the reason.

### bundleconfig.json — End-User Report Designer, self-contained (MVC)

> **Same caveat as above:** strip `jquery`/`bootstrap` from `thirdparty.bundle.js` (and `bootstrap.min.css` from `thirdparty.bundle.css`) when the app already supplied them in `~/lib/…`.

```json
[
  {
    "outputFileName": "wwwroot/css/thirdparty.bundle.css",
    "inputFiles": [
      "node_modules/bootstrap/dist/css/bootstrap.min.css",
      "node_modules/devextreme-dist/css/dx.light.css"
    ],
    "minify": { "enabled": false, "adjustRelativePaths": false }
  },
  {
    "outputFileName": "wwwroot/css/ace/ace.bundle.css",
    "inputFiles": [
      "node_modules/ace-builds/css/ace.css",
      "node_modules/ace-builds/css/theme/dreamweaver.css",
      "node_modules/ace-builds/css/theme/ambiance.css"
    ],
    "minify": { "enabled": false, "adjustRelativePaths": false }
  },
  {
    "outputFileName": "wwwroot/css/designer.part.bundle.css",
    "inputFiles": [
      "node_modules/@devexpress/analytics-core/dist/css/dx-analytics.common.css",
      "node_modules/@devexpress/analytics-core/dist/css/dx-analytics.light.css",
      "node_modules/devexpress-reporting/dist/css/dx-webdocumentviewer.css",
      "node_modules/@devexpress/analytics-core/dist/css/dx-querybuilder.css",
      "node_modules/devexpress-reporting/dist/css/dx-reportdesigner.css"
    ],
    "minify": { "enabled": false, "adjustRelativePaths": false }
  },
  {
    "outputFileName": "wwwroot/js/thirdparty.bundle.js",
    "inputFiles": [
      "node_modules/jquery/dist/jquery.min.js",
      "node_modules/knockout/build/output/knockout-latest.js",
      "node_modules/bootstrap/dist/js/bootstrap.min.js",
      "node_modules/ace-builds/src-min-noconflict/ace.js",
      "node_modules/ace-builds/src-min-noconflict/ext-language_tools.js",
      "node_modules/ace-builds/src-min-noconflict/theme-dreamweaver.js",
      "node_modules/ace-builds/src-min-noconflict/theme-ambiance.js",
      "node_modules/ace-builds/src-min-noconflict/snippets/text.js"
    ],
    "minify": { "enabled": false },
    "sourceMap": false
  },
  {
    "outputFileName": "wwwroot/js/designer.part.bundle.js",
    "inputFiles": [
      "node_modules/devextreme-dist/js/dx.all.js",
      "node_modules/@devexpress/analytics-core/dist/js/dx-analytics-core.min.js",
      "node_modules/devexpress-reporting/dist/js/dx-webdocumentviewer.min.js",
      "node_modules/@devexpress/analytics-core/dist/js/dx-querybuilder.min.js",
      "node_modules/devexpress-reporting/dist/js/dx-reportdesigner.min.js"
    ],
    "minify": { "enabled": false },
    "sourceMap": false
  }
]
```

> **Critical**: The current designer bundle (`designer.part.bundle.js`) is self-contained — it includes `dx.all.js`, analytics core, viewer, query builder, and designer scripts in one bundle. **Never** load `viewer.part.bundle.js` on the same page as `designer.part.bundle.js`. Loading both causes duplicate module registration (`Component dx-filtereditor-plain is already registered`) and `DxReportDesigner undefined` at runtime.

> **Optional — inline Rich Text Editor (Designer only).** If you enabled `devexpress-richedit` (see the package.json note above), add `node_modules/devexpress-richedit/dist/dx.richedit.css` to `designer.part.bundle.css`, and `node_modules/jszip/dist/jszip.min.js` + `node_modules/devexpress-richedit/dist/dx.richedit.min.js` to `designer.part.bundle.js` — both **before** the `dx-querybuilder`/`dx-reportdesigner` entries.

### libman.json — icon fonts only

Under Pattern B, LibMan copies just the icon fonts (the `bundleconfig.json` handles JS/CSS):

```json
{
  "version": "1.0",
  "defaultProvider": "filesystem",
  "libraries": [
    {
      "library": "node_modules/devextreme-dist/css/icons/",
      "destination": "wwwroot/css/icons",
      "files": [ "dxicons*.ttf", "dxicons*.woff2", "dxicons*.woff" ]
    }
  ]
}
```

For the designer (which includes the Ace code editor), also add **both** entries, in this order:
```json
{
  "library": "node_modules/ace-builds/css",
  "destination": "wwwroot/css/ace",
  "files": [ "*.png", "*.svg" ]
},
{
  "library": "wwwroot/css/ace",
  "destination": "wwwroot/css",
  "files": [ "dreamweaver-1.png", "dreamweaver-2.png", "ambiance-1.png", "ambiance-2.png", "ambiance-3.png" ]
}
```

> The second entry copies from `wwwroot/css/ace` — the folder the first entry just filled — and must be declared **after** it. Both are needed under Pattern B only: bundling flattens `theme/*.css` into `wwwroot/css/ace/ace.bundle.css`, so its `../dreamweaver-1.png` references resolve one directory higher, to `wwwroot/css/`. Two entries may never share the same `library` value: LibMan keys on it and silently restores only one of a duplicate pair — no error, clean build, no files.

### View wiring (MVC)

`_Layout.cshtml` — `thirdparty.bundle.js` must render **before the component's init script**; `<head>` is the safe default under every page shape (see [../SKILL.md](../SKILL.md) Constraint 2):

```cshtml
<head>
    <link rel="stylesheet" href="~/css/thirdparty.bundle.css" />
    <script src="~/js/thirdparty.bundle.js"></script>
</head>
```

Viewer page: `<link rel="stylesheet" href="~/css/viewer.part.bundle.css" />` + `<script src="~/js/viewer.part.bundle.js"></script>`. Designer page: the `designer.part.bundle.css`/`.js` and `ace.bundle.css` instead — never alongside the viewer bundle.

### Split site/reporting grouping (the DevExpress project-template layout)

The example above uses a self-contained designer bundle. The DevExpress ASP.NET Core Reporting project templates split it instead: third-party JS into `site.thirdparty.bundle.js` (jQuery/Bootstrap) and `reporting.thirdparty.bundle.js` (Knockout/ace), DevExpress JS into `viewer.part.bundle.js` (`dx.all.js` + viewer) and `designer.part.bundle.js` (query builder + designer) — the last two loaded together on the designer page. Both groupings are valid; what matters is that the full script set ends up on the final page in the correct order.

> **Same caveat again:** drop `site.thirdparty.bundle.js`'s `jquery`/`bootstrap` (and `thirdparty.bundle.css`'s `bootstrap.min.css`) when the app already supplied them in `~/lib/…`.

```json
[
  {
    "outputFileName": "wwwroot/css/thirdparty.bundle.css",
    "inputFiles": [
      "node_modules/bootstrap/dist/css/bootstrap.min.css"
    ],
    "minify": { "enabled": false, "adjustRelativePaths": false }
  },
  {
    "outputFileName": "wwwroot/js/site.thirdparty.bundle.js",
    "inputFiles": [
      "node_modules/jquery/dist/jquery.min.js",
      "node_modules/bootstrap/dist/js/bootstrap.min.js"
    ],
    "minify": { "enabled": false },
    "sourceMap": false
  },
  {
    "outputFileName": "wwwroot/css/viewer.part.bundle.css",
    "inputFiles": [
      "node_modules/@devexpress/analytics-core/dist/css/dx-analytics.common.css",
      "node_modules/devexpress-reporting/dist/css/dx-webdocumentviewer.css"
    ],
    "minify": { "enabled": false, "adjustRelativePaths": false }
  },
  {
    "outputFileName": "wwwroot/css/designer.part.bundle.css",
    "inputFiles": [
      "node_modules/ace-builds/css/ace.css",
      "node_modules/ace-builds/css/theme/dreamweaver.css",
      "node_modules/ace-builds/css/theme/ambiance.css",
      "node_modules/@devexpress/analytics-core/dist/css/dx-querybuilder.css",
      "node_modules/devexpress-reporting/dist/css/dx-reportdesigner.css"
    ],
    "minify": { "enabled": false, "adjustRelativePaths": false }
  },
  {
    "outputFileName": "wwwroot/js/reporting.thirdparty.bundle.js",
    "inputFiles": [
      "node_modules/knockout/build/output/knockout-latest.js",
      "node_modules/ace-builds/src-min-noconflict/ace.js",
      "node_modules/ace-builds/src-min-noconflict/ext-language_tools.js",
      "node_modules/ace-builds/src-min-noconflict/theme-dreamweaver.js",
      "node_modules/ace-builds/src-min-noconflict/theme-ambiance.js",
      "node_modules/ace-builds/src-min-noconflict/snippets/text.js"
    ],
    "minify": { "enabled": false },
    "sourceMap": false
  },
  {
    "outputFileName": "wwwroot/js/viewer.part.bundle.js",
    "inputFiles": [
      "node_modules/devextreme-dist/js/dx.all.js",
      "node_modules/@devexpress/analytics-core/dist/js/dx-analytics-core.min.js",
      "node_modules/devexpress-reporting/dist/js/dx-webdocumentviewer.min.js"
    ],
    "minify": { "enabled": false },
    "sourceMap": false
  },
  {
    "outputFileName": "wwwroot/js/designer.part.bundle.js",
    "inputFiles": [
      "node_modules/@devexpress/analytics-core/dist/js/dx-querybuilder.min.js",
      "node_modules/devexpress-reporting/dist/js/dx-reportdesigner.min.js"
    ],
    "minify": { "enabled": false },
    "sourceMap": false
  }
]
```

> **Note**: Because this grouping splits the viewer and designer scripts into separate bundles, both `viewer.part.bundle.js` and `designer.part.bundle.js` must be loaded together on the designer page — omitting either causes missing-script errors (e.g. `DxReportDesigner undefined`). This is a property of this particular bundle grouping, not a Razor Pages requirement; the self-contained grouping and this split grouping both work in either project type.

> **Optional — inline Rich Text Editor (Designer only).** As with the self-contained grouping, if you enabled `devexpress-richedit` add `node_modules/devexpress-richedit/dist/dx.richedit.css` to `designer.part.bundle.css`, and `node_modules/jszip/dist/jszip.min.js` + `node_modules/devexpress-richedit/dist/dx.richedit.min.js` to `designer.part.bundle.js` — **before** the `dx-querybuilder`/`dx-reportdesigner` entries.

---

## Pattern C — Reuse an Existing Bundle (requires the developer's explicit permission)

Applies when Phase 1 reconnaissance finds an existing bundler (`BuildBundlerMinifier`/`bundleconfig.json`) already wired into the app. **Before editing it, explicitly ask the developer for permission.** A bundle configuration the agent didn't author may be relied on by parts of the app the agent hasn't seen, and a bad edit can break them in ways that don't surface until runtime — that's a materially bigger blast radius than adding new files under Pattern A. Do not treat "the fit looks precise" as license to edit it unasked.

Once the developer agrees, add the missing DevExpress entries (see the Pattern B `bundleconfig.json` templates above for the exact `node_modules` source paths and output groupings) to the **existing** bundle configuration rather than introducing a new mechanism. This is only a valid choice when you're confident the existing setup can absorb the change correctly:

- The existing bundle's output still satisfies the **load-order dependencies** in [../SKILL.md](../SKILL.md) Constraint 2 (load order). Concatenation counts: a mis-ordered `inputFiles` entry *inside* one output file breaks the page just as a mis-ordered `<script>` tag would.
- The app's existing libraries are not **re-bundled**: if the layout already loads jQuery/Bootstrap, the DevExpress entries must not add a second copy.
- The Designer's self-containment requirement is preserved: whichever bundle(s) load on the Designer page must include the full script chain (`dx.all.js` through `dx-reportdesigner.min.js`) exactly once — see the split site/reporting grouping in the Pattern B section above, which shows this can be split across bundles as long as all of them load together on the final page.
- You are not guessing at an unfamiliar bundler's config format or a non-standard grouping scheme.

If any of these is in doubt, or the developer hasn't explicitly agreed to the edit, don't force the fit — fall back to Pattern A instead. A half-correct bundle edit is harder to debug than a clean set of direct `<script>`/`<link>` tags, and making it without asking risks breaking something the developer never asked the agent to touch.
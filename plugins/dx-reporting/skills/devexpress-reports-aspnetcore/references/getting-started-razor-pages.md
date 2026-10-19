# Getting Started — ASP.NET Core Razor Pages

Setup for integrating DevExpress Document Viewer or Report Designer into an ASP.NET Core Razor Pages application.

> **Applicability.** This file is a standard-template starting point. Before copying anything from here, run Phase 1 (Preflight) and Phase 2 (Planning) of the Integration track workflow in [../SKILL.md](../SKILL.md), and pick a client-resource pattern. **Steps 1–9 below show Pattern A (Direct References: `package.json` + `libman.json` + plain `<script>`/`<link>` tags)** — the default for adding reporting to an existing application, with no `BuildBundlerMinifier` and no `bundleconfig.json`. **If you're scaffolding a greenfield/demo project, or you explicitly want a bundled/minified build, use Pattern B (Bundled) instead** — its full `bundleconfig.json` templates (with the split site/reporting grouping) live in [client-resource-integration.md](client-resource-integration.md); add the `BuildBundlerMinifier` package and swap Steps 3–4 and 8–9 for the Bundled equivalents there. Steps 5–7 and 10 (Program.cs, controllers, page wiring) are identical for both patterns. If the project already has a bundler, existing library registrations, or scripts in `_Layout.cshtml`, adapt — do not blindly duplicate. In particular:
> - **No duplication** — never *register* a library the project already ships: no second `<script>`/`<link>` tag, no second bundle entry. An exception is `package.json` (Step 2), which registers nothing on the page.
> - **Scope** — register a library in the layout only if it is used outside reporting on multiple pages; keep reporting-only libraries (Knockout, ace-builds) on the reporting page(s) only.
> - **Preserve load order** — the tag order below satisfies every dependency in [../SKILL.md](../SKILL.md) Constraint 2 (load order). It's one valid ordering, not the only one (Knockout/Bootstrap are independent) — don't reshuffle an app's working tags to match it.

## System Requirements

.NET 8.0+, Node.js LTS (LibMan copies client assets from `node_modules/`), nuget.org access, and a valid DevExpress license. IDE: Visual Studio 2022 (v17.0+), JetBrains Rider, or VS Code.

## Key Differences from MVC

| Aspect | MVC | Razor Pages |
|--------|-----|-------------|
| Service registration | `AddControllersWithViews()` | `AddRazorPages()` + `AddMvcCore()` |
| Endpoint mapping | `MapControllerRoute(...)` | `MapRazorPages()` + `MapDefaultControllerRoute()` |
| View directive | none (controller action) | `@page` at top of file |

These are the only real framework differences. Client-resource registration is identical to MVC.

## Step 1: Install NuGet Packages

Install via NuGet Package Manager or CLI:

```bash
dotnet add package DevExpress.AspNetCore.Reporting
dotnet add package Microsoft.Web.LibraryManager.Build
```

> Install **without** a `--version` pin so NuGet resolves the current patch. Pinning a major.minor like `26.1` emits an `NU1603` "was not found … resolved instead" warning on every build, because `26.1.0` is not a published version. Align the npm packages to the major.minor NuGet actually resolved.
>
> **Existing project with floating references?** If the `.csproj` already pins DevExpress packages as `26.1.*`, add yours the same way (`dotnet add package <name> --version "26.1.*"`) instead — a fixed pin beside a floating one builds today and fails with `NU1107` once the next patch ships.
> `Microsoft.Web.LibraryManager.Build` is required under **every** pattern — including Pattern B, where LibMan copies only the icon fonts. Omitting it leaves `libman.json` unprocessed and every toolbar icon a tofu box.

Add `BuildBundlerMinifier` too **only if you chose Pattern B (Bundled)** — see [client-resource-integration.md](client-resource-integration.md):
```bash
dotnet add package BuildBundlerMinifier
```

**For Linux/macOS hosting** (also required for Docker/Azure App Service on Linux):
```bash
dotnet add package DevExpress.Drawing.Skia
```

### Non-Windows Platform Support (Linux, macOS, Docker, Cloud)

DevExpress Reporting uses a platform-specific drawing engine: GDI+ on Windows, SkiaSharp elsewhere. **The SkiaSharp-based engine is enabled automatically on non-Windows platforms.** Enable `Settings.DrawingEngine` at app startup only to force Skia *on Windows* (e.g., to work around the 10K GDI-handle limit).
If reports include PDF content (`XRPdfContent`) or you export/preview to PDF, also install:

```bash
dotnet add package DevExpress.Pdf.SkiaRenderer
```

On **Linux**, ensure the required font libraries are installed on the target host/container image (these are commands for the developer/operator to run):

- Debian/Ubuntu: `apt-get install -y libc6 libicu-dev libfontconfig1`
- RHEL/CentOS/Fedora: `yum install -y glibc-devel libicu fontconfig`

See [Use Reporting on Linux and macOS](https://docs.devexpress.com/XtraReports/404221/common-information/dot-net-and-net-core-support/use-reporting-on-linux) for the full platform guide.

## Step 2: Install npm Packages (package.json)

Add `package.json` to the project root:

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

> **These four entries are the complete list.** While DevExpress packages also require `jquery` and `knockout`, npm automatically resolves them, so **do not** add them here explicitly. There is no other separate npm package for the viewer, designer, or query builder scripts: they all ship inside `devexpress-reporting` and `@devexpress/analytics-core`. 

> **Keep `bootstrap` here even if the app already has one in `wwwroot/lib`.** It is an exception to the no-duplication rule because `package.json` does not register anything on the page.

Restore packages:
```bash
npm install
```

> **Important**: Run `npm install` before `dotnet build`. LibMan's `filesystem` provider reads from `node_modules/` at build time. If `node_modules/` does not exist, the build fails with a LibMan error (LIB002).

> **Optional — inline Rich Text Editor (Designer only).** For editable `XRRichText` controls, add `"devexpress-richedit": "26.1-stable"` to `dependencies`, then wire its resources in Steps 4 and 9. [Docs](https://docs.devexpress.com/XtraReports/401255).

## Step 3: Configure Client Resources (libman.json) - Pattern A (Direct References)

Following Pattern B (Bundled) instead? Skip the current Client Resources steps (Step 3 and Step 4), and use the `bundleconfig.json` + icon-only `libman.json` templates in [client-resource-integration.md](client-resource-integration.md).

This is the default (Pattern A — Direct References): no `bundleconfig.json`, no `BuildBundlerMinifier`. `libman.json` copies the DevExpress JS/CSS out of `node_modules` directly into `wwwroot`, where the `<script>`/`<link>` tags in Steps 8–9 reference each file by its own path. The `libman.json` content for Razor pages is identical to the MVC file — only the layout/page wiring in Steps 8–9 differs. 

jQuery and Knockout are transitive npm dependencies. There might be two ways to setup them depending on whether the app already supplies them in `wwwroot/lib` or not. Use the jQuery/Bootstrap branch Phase 1 recorded:
If **App-supplied** — the app already has them (i.e., any `dotnet new` scaffold), **delete the first three entries** below and use `~/lib/…` tags.
If **npm-supplied** — the app has neither, keep every entry below and use `~/js\|css/thirdparty/…` tags.

### Document Viewer Only

One file, one `libraries` array. The **first three** entries are the branch-conditional jQuery/Bootstrap ones; everything from `knockout` down is unconditional — Knockout is reporting-only and always stays.

```json
{
  "version": "1.0",
  "defaultProvider": "filesystem",
  "libraries": [
    {
      "library": "node_modules/jquery/dist/",
      "destination": "wwwroot/js/thirdparty",
      "files": [ "jquery.min.js" ]
    },
    {
      "library": "node_modules/bootstrap/dist/css/",
      "destination": "wwwroot/css/thirdparty",
      "files": [ "bootstrap.min.css" ]
    },
    {
      "library": "node_modules/bootstrap/dist/js/",
      "destination": "wwwroot/js/thirdparty",
      "files": [ "bootstrap.min.js" ]
    },
    {
      "library": "node_modules/knockout/build/output/",
      "destination": "wwwroot/js/thirdparty",
      "files": [ "knockout-latest.js" ]
    },
    {
      "library": "node_modules/devextreme-dist/css/",
      "destination": "wwwroot/css",
      "files": [ "dx.light.css" ]
    },
    {
      "library": "node_modules/devextreme-dist/css/icons/",
      "destination": "wwwroot/css/icons",
      "files": [ "dxicons*.ttf", "dxicons*.woff2", "dxicons*.woff" ]
    },
    {
      "library": "node_modules/devextreme-dist/js/",
      "destination": "wwwroot/js",
      "files": [ "dx.all.js" ]
    },
    {
      "library": "node_modules/@devexpress/analytics-core/dist/css/",
      "destination": "wwwroot/css/reporting",
      "files": [ "dx-analytics.common.css", "dx-analytics.light.css" ]
    },
    {
      "library": "node_modules/@devexpress/analytics-core/dist/js/",
      "destination": "wwwroot/js/reporting",
      "files": [ "dx-analytics-core.min.js" ]
    },
    {
      "library": "node_modules/devexpress-reporting/dist/css/",
      "destination": "wwwroot/css/reporting",
      "files": [ "dx-webdocumentviewer.css" ]
    },
    {
      "library": "node_modules/devexpress-reporting/dist/js/",
      "destination": "wwwroot/js/reporting",
      "files": [ "dx-webdocumentviewer.min.js" ]
    }
  ]
}
```

> **`dx.light.css` must stay at the root of `wwwroot/css/` — do not "tidy" it into a subfolder.** It is the only DevExpress stylesheet with an external asset reference (`url("icons/dxicons.woff2")`), and CSS resolves that against **the stylesheet's own URL**, not the page's. Keeping it at the `css/` root is what makes `wwwroot/css/icons` the correct font destination under Pattern A *and* Pattern B alike. Move it to `wwwroot/css/devextreme/` and the browser asks for `/css/devextreme/icons/…`: every toolbar icon becomes a tofu box while the font files still sit on disk, and every `<link>` on the page still returns 200. `scripts/Verify-ReportingPage.ps1` checks this.

## Step 4: Configure Client Resources (libman.json additions — End-User Report Designer) - Pattern A (Direct References)

For the Designer, extend the two `@devexpress/analytics-core` entries and the two `devexpress-reporting` entries from Step 3 with the query builder / designer files, and add the `ace-builds` entries (the Designer's embedded SQL/expression editor):

```json
    {
      "library": "node_modules/ace-builds/css/",
      "destination": "wwwroot/css/ace",
      "files": [ "ace.css", "*.png", "*.svg" ]
    },
    {
      "library": "node_modules/ace-builds/src-min-noconflict/",
      "destination": "wwwroot/js/ace",
      "files": [ "ace.js", "ext-language_tools.js", "theme-dreamweaver.js", "theme-ambiance.js" ]
    },
    {
      "library": "node_modules/ace-builds/css/theme/",
      "destination": "wwwroot/css/ace/theme",
      "files": [ "dreamweaver.css", "ambiance.css" ]
    },
    {
      "library": "node_modules/ace-builds/src-min-noconflict/snippets/",
      "destination": "wwwroot/js/ace/snippets",
      "files": [ "text.js" ]
    }
```

> **LibMan does not recurse.** A `library` path lists only the files directly inside it, so a nested path like `theme/dreamweaver.css` or `snippets/text.js` in a `files` array fails the build with `LIB018` ("does not contain the following"). Each subdirectory needs its own entry — which is why the theme CSS and snippets appear above as separate `library` blocks. The listed files do exist; do **not** delete them to clear the error.

— and add `dx-querybuilder.css`/`dx-querybuilder.min.js` to the existing `@devexpress/analytics-core` css/js `files` arrays, and `dx-reportdesigner.css`/`dx-reportdesigner.min.js` to the existing `devexpress-reporting` css/js `files` arrays.

**Optional — inline Rich Text Editor.** If you added `devexpress-richedit` in Step 2, also copy its resources:

```json
    {
      "library": "node_modules/devexpress-richedit/dist/",
      "destination": "wwwroot/lib/richedit",
      "files": [ "dx.richedit.css", "dx.richedit.min.js" ]
    },
    {
      "library": "node_modules/devexpress-richedit/dist/icons/",
      "destination": "wwwroot/lib/richedit/icons",
      "files": [ "dxre-icons.ttf", "dxre-icons.woff", "dxre-icons.woff2" ]
    },
    {
      "library": "node_modules/jszip/dist/",
      "destination": "wwwroot/lib/richedit",
      "files": [ "jszip.min.js" ]
    }
```

## Step 5: Configure Application on Startup in Program.cs

```csharp
using DevExpress.AspNetCore;
using DevExpress.AspNetCore.Reporting; // required for ConfigureReportingServices

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddMvcCore(); // required for reporting controllers

builder.Services.AddDevExpressControls();
builder.Services.ConfigureReportingServices(configurator => {
    // configurator: ReportingConfigurationBuilder — app-wide reporting settings
    // enables verbose error output + version check
    if (builder.Environment.IsDevelopment())
        configurator.UseDevelopmentMode();

    configurator.ConfigureWebDocumentViewer(viewerConfigurator => {
        // viewerConfigurator: WebDocumentViewerConfigurationBuilder — storage,
        // caching, and Viewer-scoped data services
        viewerConfigurator.UseCachedReportSourceBuilder();
    });
    // Add this block only if Report Designer is also integrated:
    configurator.ConfigureReportDesigner(designerConfigurator => {
        // designerConfigurator: ReportDesignerConfigurationBuilder — Data Source
        // Wizard providers and Designer-scoped services (see Step 10)
    });
    // Optional — configurator.ConfigureQueryBuilder(queryBuilderConfigurator => { ... })
    // targets QueryBuilderConfigurationBuilder, for the Designer's embedded Query
    // Builder and the standalone one alike.
});

var app = builder.Build();

app.UseDevExpressControls(); // MUST precede MapStaticAssets/UseStaticFiles and any other static-file handling. Keep whichever call the scaffold already has — never add a second.
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapRazorPages();
app.MapDefaultControllerRoute(); // required for reporting controllers in Razor Pages

app.Run();
```

> If the report has a custom data source class (Designer, or storage-backed Viewer), register it as trusted **before** `var builder = WebApplication.CreateBuilder(args);`. See [report-storage.md](report-storage.md#data-source-trust-registration--custom-data-sources) for the exact calls. Omitting this throws `NonTrustedTypeDeserializationException` at runtime.

## Step 5b (Optional): Async Document Generation

For long-running reports, enable the async engine in `Program.cs`:

```csharp
builder.Services.ConfigureReportingServices(configurator => {
    configurator.UseAsyncEngine(); // enables IReportProviderAsync and async methods of the report storage
    configurator.ConfigureWebDocumentViewer(viewerConfigurator => {
        viewerConfigurator.UseCachedReportSourceBuilder();
    });
});
```

## Step 6: Add Infrastructure Controllers

DevExpress Reporting requires **infrastructure** controllers — they answer the
viewer's/designer's background AJAX calls and never call `View()`. Inheriting the base class is
what establishes their routes (`DXXRDV`, `DXXRD`, `DXXQB`): never write an action method with
one of those names yourself, and never return `View()` from one.

```csharp
// Controllers/ReportingControllers.cs
using DevExpress.AspNetCore.Reporting.QueryBuilder;
using DevExpress.AspNetCore.Reporting.QueryBuilder.Native.Services;
using DevExpress.AspNetCore.Reporting.ReportDesigner;
using DevExpress.AspNetCore.Reporting.ReportDesigner.Native.Services;
using DevExpress.AspNetCore.Reporting.WebDocumentViewer;
using DevExpress.AspNetCore.Reporting.WebDocumentViewer.Native.Services;

public class CustomWebDocumentViewerController : WebDocumentViewerController {
    public CustomWebDocumentViewerController(IWebDocumentViewerMvcControllerService s) : base(s) { }
}

public class CustomReportDesignerController : ReportDesignerController {
    public CustomReportDesignerController(IReportDesignerMvcControllerService s) : base(s) { }
}

public class CustomQueryBuilderController : QueryBuilderController {
    public CustomQueryBuilderController(IQueryBuilderMvcControllerService s) : base(s) { }
}
```

> Omit `CustomReportDesignerController` and `CustomQueryBuilderController` if only the Document Viewer is needed.

In Razor Pages, other controllers are not required - the page itself is served by its `.cshtml` file (Step 9).

## Step 7: _ViewImports.cshtml

Add to the file located in Phase 1 (`Pages/_ViewImports.cshtml` in standard Razor Pages scaffolding), keeping its existing lines:

```cshtml
@using DevExpress.AspNetCore
```

## Step 8: _Layout.cshtml

Register only libraries used across the app (scope rule) in `<head>` — jQuery/Bootstrap are typical site-wide candidates. There are two ways to register them depending on whether the app already supplies them in `wwwroot/lib` (**App-supplied** ) or not (**npm-supplied**). Use the jQuery/Bootstrap branch Phase 1 recorded.

Below is the code snippet for the **App-supplied** branch — it uses `~/lib/…` paths to register jQuery/Bootstrap. For the **npm-supplied branch**, use same three tags but with LibMan-copied paths instead: `~/css/thirdparty/bootstrap.min.css`, `~/js/thirdparty/jquery.min.js`, `~/js/thirdparty/bootstrap.min.js`. 

Either branching, the tags should sit in `<head>`, so they precede the helper's inline init script. If they already sit in `<body>`, **move** these tags into `<head>`, don't add a second copy at a different path.


```cshtml
<head>
    <title>@ViewData["Title"]</title>
    <link rel="stylesheet" href="~/lib/bootstrap/dist/css/bootstrap.min.css" />
    <script src="~/lib/jquery/dist/jquery.min.js"></script>
    <script src="~/lib/bootstrap/dist/js/bootstrap.bundle.min.js"></script>
</head>
<body>
    @RenderBody()
</body>
```

Knockout and ace-builds are reporting-only and stay on the reporting page (Step 9) - do not include them in the layout or other pages.

> **Critical**: The base scripts must already be loaded when the DevExpress HTML helper renders — it emits inline JavaScript immediately as `@RenderBody()` processes the page. The reporting page's own tags (Knockout, `ace.js`, `dx.all.js`, …) go above the helper in load order.

## Step 9: View Files

It is recommended to use dedicated views rather than `Index.cshtml`. Only drop the control onto `Index.cshtml` when a user explicitly asks for it.

### Document Viewer Page (dedicated page, e.g. `Pages/Reports/Viewer.cshtml`)

```cshtml
@page

<link rel="stylesheet" href="~/css/dx.light.css" />
<link rel="stylesheet" href="~/css/reporting/dx-analytics.common.css" />
<link rel="stylesheet" href="~/css/reporting/dx-analytics.light.css" />
<link rel="stylesheet" href="~/css/reporting/dx-webdocumentviewer.css" />

<script src="~/js/thirdparty/knockout-latest.js"></script>
<script src="~/js/dx.all.js"></script>
<script src="~/js/reporting/dx-analytics-core.min.js"></script>
<script src="~/js/reporting/dx-webdocumentviewer.min.js"></script>

@(Html.DevExpress().WebDocumentViewer("DocumentViewer")
    .Height("1000px")   @* keep px unless a user explicitly wants a different height *@
    .Bind(new TestReport()))
```

### Report Designer Page (dedicated page, e.g. `Pages/Reports/Designer.cshtml`)

```cshtml
@page

<link rel="stylesheet" href="~/css/dx.light.css" />
<link rel="stylesheet" href="~/css/reporting/dx-analytics.common.css" />
<link rel="stylesheet" href="~/css/reporting/dx-analytics.light.css" />
<link rel="stylesheet" href="~/css/reporting/dx-webdocumentviewer.css" />
<link rel="stylesheet" href="~/css/reporting/dx-querybuilder.css" />
<link rel="stylesheet" href="~/css/reporting/dx-reportdesigner.css" />
<link rel="stylesheet" href="~/css/ace/ace.css" />
<link rel="stylesheet" href="~/css/ace/theme/dreamweaver.css" />
<link rel="stylesheet" href="~/css/ace/theme/ambiance.css" />
@* Optional — inline Rich Text Editor (only if you enabled devexpress-richedit in Step 2): *@
@* <link rel="stylesheet" href="~/lib/richedit/dx.richedit.css" /> *@

<script src="~/js/thirdparty/knockout-latest.js"></script>
<script src="~/js/ace/ace.js"></script>
<script src="~/js/ace/ext-language_tools.js"></script>
<script src="~/js/ace/theme-dreamweaver.js"></script>
<script src="~/js/ace/theme-ambiance.js"></script>
<script src="~/js/ace/snippets/text.js"></script>
<script src="~/js/dx.all.js"></script>
<script src="~/js/reporting/dx-analytics-core.min.js"></script>
<script src="~/js/reporting/dx-webdocumentviewer.min.js"></script>
@* Optional — inline Rich Text Editor (load after the viewer, before the query builder): *@
@* <script src="~/lib/richedit/jszip.min.js"></script> *@
@* <script src="~/lib/richedit/dx.richedit.min.js"></script> *@
<script src="~/js/reporting/dx-querybuilder.min.js"></script>
<script src="~/js/reporting/dx-reportdesigner.min.js"></script>

@(Html.DevExpress().ReportDesigner("reportDesigner")
    .Height("1000px")   @* keep px unless a user explicitly wants a different height. Use null + CSS for CSP scenarios *@
    .Bind("SalesReport"))
```

> **Designer completeness (load-order rule).** The Designer's tag list is a **superset** of the Viewer's — it shares every viewer-level file and adds the query builder, designer, and ace-builds files, in the order shown. Load this list **once**; do not also load a separate viewer tag list on the same page (double-tagging `dx-webdocumentviewer.min.js` causes `Component dx-filtereditor-plain is already registered` / `DxReportDesigner undefined`). If you enabled the inline Rich Text Editor, `jszip.min.js` → `dx.richedit.min.js` load after `dx-webdocumentviewer.min.js` and before `dx-querybuilder.min.js`. **`ace.js` must precede `dx.all.js`**.

> These are the two simplest binding styles — a bare `@page` with the report bound directly in the view (`new TestReport()` or `"SalesReport"`). 
> To choose the report per request or hand the control a prebuilt client-side model, add an `OnGet` handler; 
> see [controller-view-binding.md](controller-view-binding.md) for the `PageModel` versions (Viewer and Designer), 
> alongside the MVC equivalents.

## Step 10 (Optional): Configure Data Sources

Pre-configure connection strings, predefined data sources, and Object Data Source types so end users don't have to configure them from scratch in the Report Wizard/Data Source Wizard. Applies to Designer scenarios only — it has no effect on a Document-Viewer-only setup.

### Register Connection String Providers

Store connection strings in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "NWindConnectionString": "XpoProvider=SQLite;Data Source=|DataDirectory|/Data/nwind.db",
    "JsonConnection": "Uri=https://raw.githubusercontent.com/DevExpress-Examples/DataSources/master/JSON/customers.json",
    "MongoDBConnection": "mongodb://localhost:27017/"
  }
}
```

Each needs its data-provider package referenced. Packages per database: [Supported Data Providers](https://docs.devexpress.com/CoreLibraries/403638).

Register the built-in providers in `Program.cs` so these connection strings appear in the Report Wizard and Data Source Wizard:

```csharp
builder.Services.ConfigureReportingServices(configurator => {
    configurator.ConfigureReportDesigner(designerConfigurator => {
        designerConfigurator.RegisterDataSourceWizardConfigFileConnectionStringsProvider();
        designerConfigurator.RegisterDataSourceWizardConfigFileJsonConnectionStringsProvider();
        designerConfigurator.RegisterDataSourceWizardConfigFileMongoDBConnectionStringsProvider();
    });
});
```

To source connection strings from elsewhere (a database, vault, or external service), implement a custom provider instead — [data-sources.md](data-sources.md#custom-providers).

To predefine a data source without a connection-string provider, build it in the page's `OnGet` and pass it via `.DataSources(...)` — this needs a `PageModel` (the bare `@page` styles in Step 9 can't build a model): [controller-view-binding.md](controller-view-binding.md#report-designer) for the `PageModel` shape, [data-sources.md](data-sources.md#adding-data-sources-in-the-controller) for the call.

Everything else data-source-related lives in [data-sources.md](data-sources.md): Object Data Source type/constructor/member providers, database schema shaping, EF Core and JSON connection factories, MongoDB providers, JSON schema discovery.

## Troubleshooting

Symptom lookup lives in [troubleshooting.md](troubleshooting.md) — read it in full rather than diagnosing from this file. It covers the defects this setup most often produces, including HTTP 404 on `DXXRDV`/`DXXRD` (missing `MapDefaultControllerRoute()`), `"Unable to process binding"` (Knockout after the reporting scripts), and `DxReportDesigner undefined` (an incomplete designer tag list — see the Step 9 superset note above).

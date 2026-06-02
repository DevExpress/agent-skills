# Getting Started — ASP.NET Core Razor Pages

Setup for integrating DevExpress Document Viewer or Report Designer into an ASP.NET Core Razor Pages application.

## Key Differences from MVC

| Aspect | MVC | Razor Pages |
|--------|-----|-------------|
| Service registration | `AddControllersWithViews()` | `AddRazorPages()` + `AddMvcCore()` |
| Endpoint mapping | `MapControllerRoute(...)` | `MapRazorPages()` + `MapDefaultControllerRoute()` |
| View directive | none (controller action) | `@page` at top of file |
| Bundle structure | designer bundle is self-contained | viewer + designer bundles loaded together |
| Scripts in layout | `thirdparty.bundle.js` in `<head>` | `site.thirdparty.bundle.js` in `<head>` |

> **CRITICAL**: In Razor Pages, Knockout must be in a **separate bundle** (`reporting.thirdparty.bundle.js`) loaded per-page, not in the layout head. This is because Razor Pages templates render differently than MVC views.

## Step 1: NuGet Packages

Same as MVC — see [getting-started.md](getting-started.md) Step 1.

## Step 2: npm Packages (package.json)

Same as MVC — see [getting-started.md](getting-started.md) Step 2.

## Step 3: bundleconfig.json (Razor Pages variant)

The Razor Pages bundle split is different from MVC. `dx.all.js` is in `viewer.part.bundle.js`, and `designer.part.bundle.js` contains only the query builder and designer scripts. Both are loaded together on the designer page.

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
    "outputFileName": "wwwroot/js/site.thirdparty.bundle.js",
    "inputFiles": [
      "node_modules/jquery/dist/jquery.min.js",
      "node_modules/bootstrap/dist/js/bootstrap.min.js"
    ],
    "minify": { "enabled": false },
    "sourceMap": false
  },
  {
    "outputFileName": "wwwroot/js/reporting.thirdparty.bundle.js",
    "inputFiles": [
      "node_modules/knockout/build/output/knockout-latest.js",
      "node_modules/ace-builds/src-min-noconflict/ace.js",
      "node_modules/ace-builds/src-min-noconflict/ext-language_tools.js",
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

> **Note**: `viewer.part.bundle.js` and `designer.part.bundle.js` are loaded **together** on designer pages in Razor Pages. This differs from MVC where the designer bundle is self-contained.

## Step 4: libman.json

Same as MVC — see [getting-started.md](getting-started.md) Step 4.

## Step 5: Program.cs

```csharp
using DevExpress.AspNetCore;
using DevExpress.AspNetCore.Reporting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddDevExpressControls();
builder.Services.AddMvcCore(); // required for reporting controllers

builder.Services.ConfigureReportingServices(configurator => {
    if (builder.Environment.IsDevelopment())
        configurator.UseDevelopmentMode();
    configurator.ConfigureWebDocumentViewer(c => c.UseCachedReportSourceBuilder());
    configurator.ConfigureReportDesigner(_ => { }); // omit if designer not needed
});

var app = builder.Build();

app.UseDevExpressControls(); // MUST be before UseStaticFiles
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapRazorPages();
app.MapDefaultControllerRoute(); // required for reporting controllers in Razor Pages

app.Run();
```

## Step 6: Controllers

Same three controllers as MVC — see [getting-started.md](getting-started.md) Step 6.

## Step 7: _ViewImports.cshtml

```cshtml
@using DevExpress.AspNetCore
```

## Step 8: _Layout.cshtml

In Razor Pages, put only site-level scripts (jQuery, Bootstrap) in `<head>`. Knockout must **not** go in layout — load it per-page via `reporting.thirdparty.bundle.js`.

```cshtml
<head>
    <title>@ViewData["Title"]</title>
    <link rel="stylesheet" href="~/css/thirdparty.bundle.css" />
    <script src="~/js/site.thirdparty.bundle.js"></script>
</head>
<body>
    @RenderBody()
</body>
```

## Step 9: View Files

### Document Viewer Page (Index.cshtml)

```cshtml
@page

<link rel="stylesheet" href="~/css/viewer.part.bundle.css" />

<script src="~/js/reporting.thirdparty.bundle.js"></script>
<script src="~/js/viewer.part.bundle.js"></script>

@(Html.DevExpress().WebDocumentViewer("DocumentViewer").Height("1000px").Bind(new TestReport()))
```

### Report Designer Page (Designer.cshtml)

```cshtml
@page

<link rel="stylesheet" href="~/css/viewer.part.bundle.css" />
<link rel="stylesheet" href="~/css/designer.part.bundle.css" />

<script src="~/js/reporting.thirdparty.bundle.js"></script>
<script src="~/js/viewer.part.bundle.js"></script>
<script src="~/js/designer.part.bundle.js"></script>

@(Html.DevExpress().ReportDesigner("reportDesigner")
    .Height("800px")
    .Bind(new DevExpress.XtraReports.UI.XtraReport()))
```

> **Critical**: Multi-line fluent chains must be wrapped in `@(...)`. Without the wrapper, trailing method calls (`.Height(...)`, `.Bind(...)`) render as literal text on the page.

## Validation

```bash
npm install
dotnet build
dotnet run --project ./YourApp/YourApp.csproj

# Verify routing works for reporting controllers
curl -o /dev/null -s -w "%{http_code}" http://localhost:5000/DXXRDV/GetDocumentData
# Should return JSON (not 404)
```

## Troubleshooting

| Symptom | Cause | Fix |
|---------|-------|-----|
| HTTP 404 on `DXXRDV` or `DXXRD` | `MapDefaultControllerRoute()` missing | Add `app.MapDefaultControllerRoute()` in Program.cs |
| `"Unable to process binding"` | Knockout not loaded before reporting scripts | Move Knockout to `reporting.thirdparty.bundle.js`, load per-page before `viewer.part.bundle.js` |
| Designer page crashes with `DxReportDesigner undefined` | `viewer.part.bundle.js` missing on designer page | In Razor Pages, both `viewer.part.bundle.js` and `designer.part.bundle.js` must be loaded on designer pages |

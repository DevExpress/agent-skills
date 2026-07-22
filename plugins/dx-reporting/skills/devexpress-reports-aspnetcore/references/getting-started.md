# Getting Started — ASP.NET Core MVC

Complete setup for integrating DevExpress Document Viewer and/or End-User Report Designer into an ASP.NET Core MVC application.

## System Requirements

- .NET 8.0 or later
- Visual Studio 2022 (v17.0+) or JetBrains Rider
- Node.js LTS (for npm bundling)
- Access to nuget.org for DevExpress NuGet packages
- Valid DevExpress license

## Step 1: NuGet Packages

Install via NuGet Package Manager or CLI:

```bash
dotnet add package DevExpress.AspNetCore.Reporting
dotnet add package BuildBundlerMinifier
dotnet add package Microsoft.Web.LibraryManager.Build
```

**For Linux/macOS hosting** (also required for Docker/Azure App Service on Linux):
```bash
dotnet add package DevExpress.Drawing.Skia
```

## Non-Windows Platform Support (Linux, macOS, Docker, Cloud)

DevExpress Reporting uses a platform-specific drawing engine: GDI+ on Windows, SkiaSharp elsewhere. **The SkiaSharp-based engine is enabled automatically on non-Windows platforms.** Enable `Settings.DrawingEngine` at app startup only to force Skia *on Windows* (e.g., to work around the 10K GDI-handle limit).

If reports include PDF content (`XRPdfContent`) or you export/preview to PDF, also install:

```bash
dotnet add package DevExpress.Pdf.SkiaRenderer
```

On **Linux**, ensure the required font libraries are installed on the target host/container image (these are commands for the developer/operator to run):

- Debian/Ubuntu: `apt-get install -y libc6 libicu-dev libfontconfig1`
- RHEL/CentOS/Fedora: `yum install -y glibc-devel libicu fontconfig`

### Non-Windows Troubleshooting

| Issue | Cause | Solution |
|-------|-------|----------|
| `System.DllNotFoundException` for Skia at runtime | `DevExpress.Drawing.Skia` (or `DevExpress.Pdf.SkiaRenderer` for PDF content) not installed on Linux/macOS | `dotnet add package DevExpress.Drawing.Skia` — see the [DevExpress.Drawing troubleshooting guide](https://docs.devexpress.com/CoreLibraries/404254/devexpress-drawing-library/troubleshooting) if the exception persists after installing |
| `System.TypeInitializationException` on Linux/Docker | Missing native font libraries | Install `libc6 libicu-dev libfontconfig1` (Debian/Ubuntu) or `glibc-devel libicu fontconfig` (RHEL/CentOS) |
| On Windows Azure, rendering fails or falls back unexpectedly | Azure has limited GDI API support | Set [PdfPrintingOptions.RenderingEngine](https://docs.devexpress.com/CoreLibraries/DevExpress.XtraPrinting.PdfPrintingOptions.RenderingEngine) to `Skia` explicitly |

See [Use Reporting on Linux and macOS](https://docs.devexpress.com/XtraReports/404221/common-information/dot-net-and-net-core-support/use-reporting-on-linux) for the full platform guide.

## Step 2: npm Packages (package.json)

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

Restore packages:
```bash
npm install
```

> **Important**: Run `npm install` before `dotnet build`. LibMan's `filesystem` provider reads from `node_modules/` at build time. If `node_modules/` does not exist, the build fails with a LibMan error (LIB002).

## Step 3: bundleconfig.json

### Document Viewer Only

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

### End-User Report Designer (includes Viewer)

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

> **Critical**: The designer bundle (`designer.part.bundle.js`) is self-contained — it includes `dx.all.js`, analytics core, viewer, query builder, and designer scripts in one bundle. **Never** load `viewer.part.bundle.js` on the same page as `designer.part.bundle.js`. Loading both causes duplicate module registration (`Component dx-filtereditor-plain is already registered`) and `DxReportDesigner undefined` at runtime.

## Step 4: libman.json

```json
{
  "version": "1.0",
  "defaultProvider": "filesystem",
  "libraries": [
    {
      "library": "node_modules/devextreme-dist/css/icons/",
      "destination": "wwwroot/css/icons",
      "files": [
        "dxicons.ttf",
        "dxicons.woff2",
        "dxicons.woff"
      ]
    }
  ]
}
```

> For the designer (which includes the Ace code editor), also add:
> ```json
> {
>   "library": "node_modules/ace-builds/css/",
>   "destination": "wwwroot/css/ace",
>   "files": ["*.png", "*.svg"]
> }
> ```

## Step 5: Program.cs

```csharp
using DevExpress.AspNetCore;
using DevExpress.AspNetCore.Reporting; // required for ConfigureReportingServices

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

builder.Services.AddDevExpressControls();
builder.Services.ConfigureReportingServices(configurator => {
    if (builder.Environment.IsDevelopment())
        configurator.UseDevelopmentMode(); // enables verbose error output + version check
    configurator.ConfigureWebDocumentViewer(viewerConfigurator => {
        viewerConfigurator.UseCachedReportSourceBuilder();
    });
    // Add this block only if Report Designer is also integrated:
    configurator.ConfigureReportDesigner(designerConfigurator => {
        // optional: designerConfigurator.RegisterDataSourceWizardConfigFileConnectionStringsProvider()
    });
});

var app = builder.Build();

app.UseDevExpressControls(); // MUST come before UseStaticFiles
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
app.Run();
```

## Step 6: Controllers

### Document Viewer Only

```csharp
// Controllers/ReportingControllers.cs
using DevExpress.AspNetCore.Reporting.WebDocumentViewer;
using DevExpress.AspNetCore.Reporting.WebDocumentViewer.Native.Services;

public class CustomWebDocumentViewerController : WebDocumentViewerController {
    public CustomWebDocumentViewerController(IWebDocumentViewerMvcControllerService controllerService)
        : base(controllerService) { }
}
```

### Document Viewer + Report Designer (all three required)

```csharp
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

## Step 7: _ViewImports.cshtml

```cshtml
@using DevExpress.AspNetCore
```

## Step 8: _Layout.cshtml

```cshtml
<head>
    <title>@ViewData["Title"]</title>
    <link rel="stylesheet" href="~/css/thirdparty.bundle.css" />
    <script src="~/js/thirdparty.bundle.js"></script>
</head>
<body>
    @RenderBody()
</body>
```

> **Critical**: `thirdparty.bundle.js` must be in `<head>`, not in `@section Scripts {}`. The DevExpress HTML helper renders inline JavaScript immediately when `@RenderBody()` processes the view — all base scripts (jQuery, Knockout, Bootstrap, DevExtreme) must already be available at that point.

## Step 9: View Files

### Document Viewer Page (Index.cshtml or dedicated view)

```cshtml
@using YourApp.Reports

<link rel="stylesheet" href="~/css/viewer.part.bundle.css" />
<script src="~/js/viewer.part.bundle.js"></script>

@Html.DevExpress().WebDocumentViewer("DocumentViewer").Height("1000px").Bind(new TestReport())
```

### Report Designer Page

```cshtml
@using YourApp.Reports

<link rel="stylesheet" href="~/css/designer.part.bundle.css" />
<link rel="stylesheet" href="~/css/ace/ace.bundle.css" />
<script src="~/js/designer.part.bundle.js"></script>

@(Html.DevExpress().ReportDesigner("reportDesigner")
    .Height("1000px")
    .Bind(new TestReport()))
```

> **Critical**: Multi-line fluent chains require `@(...)`. Without the explicit Razor expression wrapper, trailing method calls render as literal text in the HTML output (you'd see `.Height("1000px")` printed on the page).

## Validation

After setup, verify:

```bash
# 1. Restore npm packages first
npm install

# 2. Build the project
dotnet build --project ./YourApp/YourApp.csproj

# 3. Run the app
dotnet run --project ./YourApp/YourApp.csproj

# 4. Smoke-check the viewer endpoint
curl -o /dev/null -s -w "%{http_code}" http://localhost:5000/

# 5. Verify no console errors in browser:
#    - No "DevExpress is not defined"
#    - No "Unable to process binding"
#    - Viewer renders the report
```

## Razor Pages Variant

See [getting-started-razor-pages.md](getting-started-razor-pages.md) for the Razor Pages setup. Key differences:
- App template: "ASP.NET Core Web App" (not MVC)
- Requires `endpoints.MapDefaultControllerRoute()` in Program.cs so reporting controllers are routed correctly
- Knockout must be in its own `vendor.js` bundle (Razor Pages renders differently from MVC views)
- View files use `@page` directive instead of controller actions

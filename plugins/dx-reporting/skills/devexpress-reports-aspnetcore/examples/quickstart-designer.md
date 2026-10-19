# Quickstart: ASP.NET Core MVC End-User Report Designer

Complete, runnable example of adding the DevExpress End-User Report Designer (which includes an embedded Document Viewer and Query Builder) to an ASP.NET Core MVC project. Every artifact getting-started.md Steps 1-9 require is included in full below — nothing is abbreviated or referenced by name only.

It follows **Pattern A (Direct References) + App-supplied jQuery/Bootstrap** : `libman.json` + plain `<script>`/`<link>` tags, no bundling; jQuery/Bootstrap come from the app's own `wwwroot/lib`. Assumes a default `dotnet new mvc` scaffold (which already provides `HomeController`/`Views/Home` and jQuery/Bootstrap). It adds the reporting-specific `ReportsController`, a dedicated `Views/Reports/Designer.cshtml` page, and the required infrastructure.

## Prerequisites

```bash
dotnet add package DevExpress.AspNetCore.Reporting
dotnet add package Microsoft.Web.LibraryManager.Build
```

**For Linux/macOS** (also required for Docker/Azure App Service on Linux):
```bash
dotnet add package DevExpress.Drawing.Skia
```

Run `npm install` **before** `dotnet build` — LibMan's `filesystem` provider reads from `node_modules/` at build time (missing it fails the build with LIB002).

## package.json

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

## libman.json

The following `libman.json` copies each DevExpress JS/CSS file out of `node_modules` into `wwwroot`; the `<script>`/`<link>` tags below reference each one by its own path (no bundling). This Designer set is a superset of the Viewer, the Query Builder, the Designer itself, and ace-builds files. There are no jQuery or Bootstrap entries because it assumes they are already **App-supplied** from `wwwroot/lib`.

```json
{
  "version": "1.0",
  "defaultProvider": "filesystem",
  "libraries": [
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
      "files": [ "dx-analytics.common.css", "dx-analytics.light.css", "dx-querybuilder.css" ]
    },
    {
      "library": "node_modules/@devexpress/analytics-core/dist/js/",
      "destination": "wwwroot/js/reporting",
      "files": [ "dx-analytics-core.min.js", "dx-querybuilder.min.js" ]
    },
    {
      "library": "node_modules/devexpress-reporting/dist/css/",
      "destination": "wwwroot/css/reporting",
      "files": [ "dx-webdocumentviewer.css", "dx-reportdesigner.css" ]
    },
    {
      "library": "node_modules/devexpress-reporting/dist/js/",
      "destination": "wwwroot/js/reporting",
      "files": [ "dx-webdocumentviewer.min.js", "dx-reportdesigner.min.js" ]
    },
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
  ]
}
```

> **LibMan does not recurse.** A `library` path lists only the files directly inside it, so a nested path like `theme/dreamweaver.css` or `snippets/text.js` in a `files` array fails the build with `LIB018` ("does not contain the following"). Each subdirectory needs its own entry — which is why the theme CSS and snippets appear above as separate `library` blocks. The listed files do exist; do **not** delete them to clear the error.

> **Load once, in order**: the designer page (below) loads the full script chain `dx.all.js` → `dx-analytics-core.min.js` → `dx-webdocumentviewer.min.js` → `dx-querybuilder.min.js` → `dx-reportdesigner.min.js` exactly once. Don't also add a separate viewer tag list on the same page — the designer list already includes every viewer file, and double-tagging causes duplicate module registration and `DxReportDesigner undefined`.

## Program.cs

```csharp
using DevExpress.AspNetCore;
using DevExpress.AspNetCore.Reporting;
using DevExpress.XtraReports.Web.Extensions;
using YourApp.Reports.Data;

DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(SalesRepository));

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllersWithViews();

builder.Services.AddDevExpressControls();
builder.Services.ConfigureReportingServices(configurator => {
    if (builder.Environment.IsDevelopment())
        configurator.UseDevelopmentMode();
    configurator.ConfigureWebDocumentViewer(viewerConfigurator => {
        viewerConfigurator.UseCachedReportSourceBuilder();
    });
    configurator.ConfigureReportDesigner(_ => { });
});

// ReportStorageWebExtension MUST be registered AFTER AddDevExpressControls
builder.Services.AddScoped<ReportStorageWebExtension, CustomReportStorageWebExtension>();

var app = builder.Build();

app.UseDevExpressControls(); // MUST be before UseStaticFiles/MapStaticAssets and any other static-file handling
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
app.Run();
```


## Data/SalesData.cs
using System.Collections.Generic;

```csharp
namespace YourApp.Reports.Data {
    public class SalesData {
        public string Product { get; set; }
        public int Quantity { get; set; }
    }
    
    public static class SalesRepository {
        public static List<SalesData> GetSales() {
            return new List<SalesData> {
                new SalesData { Product = "Widget A", Quantity = 1 },
                new SalesData { Product = "Widget B", Quantity = 72 }
            };
        }
    }
}
```

## Reports/SalesReport.cs

These classes are out of this skill's scope, not a pattern to copy. It is included here so that this quickstart is runnable without a separate `.repx` file. When the report class doesn't already exist, delegate its creation per Skill.md Step 3.1 instead of writing something like this yourself.

```csharp
using DevExpress.DataAccess.ObjectBinding;
using DevExpress.XtraReports.UI;
using YourApp.Reports.Data;

namespace YourApp.Reports {
    public class SalesReport : XtraReport {
        public SalesReport() {
            DataSource = new ObjectDataSource {
                DataSource = typeof(SalesRepository),
                DataMember = "GetSales"
            };

            var detail = new DetailBand();
            Bands.Add(detail);
            detail.HeightF = 25;

            var label = new XRLabel();
            label.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "[Product]"));
            detail.Controls.Add(label);
        }
    }
}
```

## Services/CustomReportStorageWebExtension.cs

```csharp
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Web.Extensions;
using YourApp.Reports;

public class CustomReportStorageWebExtension : ReportStorageWebExtension {
    readonly string reportDirectory;
    const string Extension = ".repx";

    public CustomReportStorageWebExtension() {
        reportDirectory = Path.Combine(AppContext.BaseDirectory, "Reports");
        if (!Directory.Exists(reportDirectory))
            Directory.CreateDirectory(reportDirectory);
    }

    private bool IsWithinReportsFolder(string url, string folder) {
        var rootDirectory = new DirectoryInfo(folder);
        var fileInfo = new FileInfo(Path.Combine(folder, url));
        return fileInfo.Directory.FullName.ToLower().StartsWith(rootDirectory.FullName.ToLower());
    }

    public override bool IsValidUrl(string url) => Path.GetFileName(url) == url;

    public override bool CanSetData(string url) => true;

    public override byte[] GetData(string url) {
        var filePath = Path.Combine(reportDirectory, url + Extension);
        if (File.Exists(filePath))
            return File.ReadAllBytes(filePath);
        var report = ResolveReport(url);
        if (report != null) {
            using var stream = new MemoryStream();
            report.SaveLayoutToXml(stream);
            return stream.ToArray();
        }
        throw new DevExpress.XtraReports.Web.ClientControls.FaultException($"Report '{url}' not found.");
    }

    public override Dictionary<string, string> GetUrls() {
        return Directory.GetFiles(reportDirectory, "*" + Extension)
            .Select(f => Path.GetFileNameWithoutExtension(f))
            .Union(BuiltInReports)
            .ToDictionary(name => name, name => name);
    }

    public override void SetData(XtraReport report, string url) {
        if (!IsWithinReportsFolder(url, reportDirectory))
            throw new InvalidOperationException("Invalid report name.");
        report.SaveLayoutToXml(Path.Combine(reportDirectory, url + Extension));
    }

    public override string SetNewData(XtraReport report, string defaultUrl) {
        if (string.IsNullOrWhiteSpace(defaultUrl)) 
            throw new DevExpress.XtraReports.Web.ClientControls.FaultException("An invalid report name was provided. It should not be empty or whitespace.");
        SetData(report, defaultUrl);
        return defaultUrl;
    }

    static readonly string[] BuiltInReports = { "SalesReport" };

    XtraReport? ResolveReport(string name) => name switch {
        "SalesReport" => new SalesReport(),
        _ => null
    };
}
```

## Controllers/ReportingControllers.cs (reporting API controllers)

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

## Controllers/ReportsController.cs (page controller — serves the view)

```csharp
public class ReportsController : Microsoft.AspNetCore.Mvc.Controller {
    public Microsoft.AspNetCore.Mvc.IActionResult Designer() => View();
}
```

## Views/_ViewImports.cshtml

```cshtml
@using DevExpress.AspNetCore
```

## Views/Shared/_Layout.cshtml

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

## Views/Reports/Designer.cshtml

```cshtml
@using YourApp.Reports

<link rel="stylesheet" href="~/css/dx.light.css" />
<link rel="stylesheet" href="~/css/reporting/dx-analytics.common.css" />
<link rel="stylesheet" href="~/css/reporting/dx-analytics.light.css" />
<link rel="stylesheet" href="~/css/reporting/dx-webdocumentviewer.css" />
<link rel="stylesheet" href="~/css/reporting/dx-querybuilder.css" />
<link rel="stylesheet" href="~/css/reporting/dx-reportdesigner.css" />
<link rel="stylesheet" href="~/css/ace/ace.css" />
<link rel="stylesheet" href="~/css/ace/theme/dreamweaver.css" />
<link rel="stylesheet" href="~/css/ace/theme/ambiance.css" />

<script src="~/js/thirdparty/knockout-latest.js"></script>
<script src="~/js/ace/ace.js"></script>
<script src="~/js/ace/ext-language_tools.js"></script>
<script src="~/js/ace/theme-dreamweaver.js"></script>
<script src="~/js/ace/theme-ambiance.js"></script>
<script src="~/js/ace/snippets/text.js"></script>
<script src="~/js/dx.all.js"></script>
<script src="~/js/reporting/dx-analytics-core.min.js"></script>
<script src="~/js/reporting/dx-webdocumentviewer.min.js"></script>
<script src="~/js/reporting/dx-querybuilder.min.js"></script>
<script src="~/js/reporting/dx-reportdesigner.min.js"></script>

@(Html.DevExpress().ReportDesigner("reportDesigner")
    .Height("1000px")   @* keep px unless a user explicitly wants a different height. Use null + CSS for CSP scenarios *@
    .Bind("SalesReport"))
```
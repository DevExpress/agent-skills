# Quickstart: ASP.NET Core MVC Document Viewer

Complete, runnable example of adding the DevExpress Document Viewer (which includes an embedded Parameters Panel) to an ASP.NET Core MVC project. Every artifact getting-started.md Steps 1-9 require is included in full below — nothing is abbreviated or referenced by name only.

It follows **Pattern A (Direct References) + App-supplied jQuery/Bootstrap** : `libman.json` + plain `<script>`/`<link>` tags, no bundling; jQuery/Bootstrap come from the app's own `wwwroot/lib`. Assumes a default `dotnet new mvc` scaffold (which already provides `HomeController`/`Views/Home` and jQuery/Bootstrap). It adds the reporting-specific `ReportsController`, a dedicated `Views/Reports/Viewer.cshtml` page, and the required infrastructure.

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

The following `libman.json` copies each DevExpress JS/CSS file out of `node_modules` into `wwwroot`; the `<script>`/`<link>` tags below reference each one by its own path (no bundling). There are no jQuery or Bootstrap entries because it assumes they are already **App-supplied** from `wwwroot/lib`.

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

## Program.cs

```csharp
using DevExpress.AspNetCore;
using DevExpress.AspNetCore.Reporting;
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
});

// Optional: register a report provider for string-based binding
builder.Services.AddScoped<DevExpress.XtraReports.Services.IReportProvider, MyReportProvider>();

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

## Services/MyReportProvider.cs

```csharp
using DevExpress.XtraReports.Services;
using DevExpress.XtraReports.UI;
using YourApp.Reports;

public class MyReportProvider : IReportProvider {
    public XtraReport GetReport(string id, ReportProviderContext context) {
        if (string.IsNullOrEmpty(id))
            return null;
        return id switch {
            "SalesReport" => new SalesReport(),
            _             => new XtraReport() // unknown name — return a blank report, don't throw
        };
    }
}
```

## Controllers/ReportingControllers.cs (reporting API controller)

```csharp
using DevExpress.AspNetCore.Reporting.WebDocumentViewer;
using DevExpress.AspNetCore.Reporting.WebDocumentViewer.Native.Services;

public class CustomWebDocumentViewerController : WebDocumentViewerController {
    public CustomWebDocumentViewerController(IWebDocumentViewerMvcControllerService controllerService)
        : base(controllerService) { }
}
```

## Controllers/ReportsController.cs (page controller — serves the view)

```csharp
public class ReportsController : Microsoft.AspNetCore.Mvc.Controller {
    public Microsoft.AspNetCore.Mvc.IActionResult Viewer() => View();
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

## Views/Reports/Viewer.cshtml

```cshtml
@using YourApp.Reports

<link rel="stylesheet" href="~/css/dx.light.css" />
<link rel="stylesheet" href="~/css/reporting/dx-analytics.common.css" />
<link rel="stylesheet" href="~/css/reporting/dx-analytics.light.css" />
<link rel="stylesheet" href="~/css/reporting/dx-webdocumentviewer.css" />

<script src="~/js/thirdparty/knockout-latest.js"></script>
<script src="~/js/dx.all.js"></script>
<script src="~/js/reporting/dx-analytics-core.min.js"></script>
<script src="~/js/reporting/dx-webdocumentviewer.min.js"></script>

@* keep px unless a user explicitly wants a different height. Use null + CSS for CSP scenarios *@
@Html.DevExpress().WebDocumentViewer("DocumentViewer").Height("1000px").Bind(new SalesReport())
```

Instance binding (above) requires no `IReportProvider`. String binding instead requires the provider registered in Program.cs:

```cshtml
@Html.DevExpress().WebDocumentViewer("DocumentViewer").Height("1000px").Bind("SalesReport")
```

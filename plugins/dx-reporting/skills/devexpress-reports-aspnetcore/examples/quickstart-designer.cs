// Quickstart: ASP.NET Core MVC End-User Report Designer
// Prerequisites:
//   dotnet add package DevExpress.AspNetCore.Reporting
//   dotnet add package BuildBundlerMinifier
//   dotnet add package Microsoft.Web.LibraryManager.Build
//   npm install  (run BEFORE dotnet build)
// For Linux/macOS:
//   dotnet add package DevExpress.Drawing.Skia

// ─── Program.cs ──────────────────────────────────────────────────────────────

using DevExpress.AspNetCore;
using DevExpress.AspNetCore.Reporting;
using DevExpress.XtraReports.Web.Extensions;

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

app.UseDevExpressControls(); // MUST be before UseStaticFiles
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
app.Run();

// ─── Services/CustomReportStorageWebExtension.cs ─────────────────────────────

using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Web.Extensions;

public class CustomReportStorageWebExtension : ReportStorageWebExtension {
    readonly string reportDirectory;
    const string Extension = ".repx";

    public CustomReportStorageWebExtension(IWebHostEnvironment env) {
        reportDirectory = Path.Combine(env.ContentRootPath, "Reports");
        Directory.CreateDirectory(reportDirectory);
    }

    public override bool IsValidUrl(string url) =>
        !Path.IsPathRooted(url) && !url.Contains("..");

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
        throw new InvalidOperationException($"Report '{url}' not found.");
        // NOTE: Do NOT use FaultException (System.ServiceModel) — not available in ASP.NET Core
    }

    public override Dictionary<string, string> GetUrls() {
        return Directory.GetFiles(reportDirectory, "*" + Extension)
            .ToDictionary(
                f => Path.GetFileNameWithoutExtension(f),
                f => Path.GetFileNameWithoutExtension(f));
    }

    public override void SetData(XtraReport report, string url) {
        report.SaveLayoutToXml(Path.Combine(reportDirectory, url + Extension));
    }

    public override string SetNewData(XtraReport report, string defaultUrl) {
        var url = string.IsNullOrEmpty(defaultUrl) ? Guid.NewGuid().ToString() : defaultUrl;
        SetData(report, url);
        return url;
    }

    XtraReport? ResolveReport(string name) => name switch {
        "SalesReport" => new SalesReport(),
        _ => null
    };
}

// ─── Controllers/ReportingControllers.cs ─────────────────────────────────────

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

// ─── Views/Home/Designer.cshtml ───────────────────────────────────────────────
// <link rel="stylesheet" href="~/css/designer.part.bundle.css" />
// <link rel="stylesheet" href="~/css/ace/ace.bundle.css" />
// <script src="~/js/designer.part.bundle.js"></script>
//
// Note: designer.part.bundle.js is self-contained (includes dx.all.js + analytics + viewer +
// querybuilder + designer). Do NOT load viewer.part.bundle.js on the same page.
//
// @(Html.DevExpress().ReportDesigner("reportDesigner")
//     .Height("1000px")
//     .Bind("SalesReport"))
//
// WARNING: The multi-line fluent chain REQUIRES @(...) wrapper.
// Without it, .Height(...) and .Bind(...) appear as literal text on the page.

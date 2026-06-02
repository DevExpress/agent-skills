// Quickstart: ASP.NET Core MVC Document Viewer
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

app.UseDevExpressControls(); // MUST be before UseStaticFiles
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();
app.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
app.Run();

// ─── Services/MyReportProvider.cs ────────────────────────────────────────────

using DevExpress.XtraReports.Services;
using DevExpress.XtraReports.UI;

public class MyReportProvider : IReportProvider {
    public XtraReport GetReport(string id, ReportProviderContext context) {
        return id switch {
            "SalesReport"   => new SalesReport(),
            "InvoiceReport" => new InvoiceReport(),
            _               => throw new InvalidOperationException($"Report '{id}' not found.")
        };
    }
}

// ─── Controllers/ReportingControllers.cs ─────────────────────────────────────

using DevExpress.AspNetCore.Reporting.WebDocumentViewer;
using DevExpress.AspNetCore.Reporting.WebDocumentViewer.Native.Services;

public class CustomWebDocumentViewerController : WebDocumentViewerController {
    public CustomWebDocumentViewerController(IWebDocumentViewerMvcControllerService controllerService)
        : base(controllerService) { }
}

// ─── Views/_ViewImports.cshtml ────────────────────────────────────────────────
// @using DevExpress.AspNetCore

// ─── Views/Shared/_Layout.cshtml (head section) ───────────────────────────────
// <link rel="stylesheet" href="~/css/thirdparty.bundle.css" />
// <script src="~/js/thirdparty.bundle.js"></script>   ← MUST be in <head>, not @section Scripts

// ─── Views/Home/Index.cshtml ──────────────────────────────────────────────────
// <link rel="stylesheet" href="~/css/viewer.part.bundle.css" />
// <script src="~/js/viewer.part.bundle.js"></script>
//
// Instance binding (no IReportProvider needed):
// @Html.DevExpress().WebDocumentViewer("DocumentViewer").Height("1000px").Bind(new SalesReport())
//
// String binding (requires IReportProvider):
// @Html.DevExpress().WebDocumentViewer("DocumentViewer").Height("1000px").Bind("SalesReport")

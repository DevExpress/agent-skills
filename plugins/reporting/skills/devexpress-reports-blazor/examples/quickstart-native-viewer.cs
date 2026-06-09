// Quickstart: Native Blazor Report Viewer (DxReportViewer)
// Prerequisites:
//   dotnet add package DevExpress.Blazor.Reporting.Viewer
// For WASM or Linux/macOS hosting:
//   dotnet add package DevExpress.Drawing.Skia

// ─── Program.cs ──────────────────────────────────────────────────────────────

using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Register native Report Viewer services
builder.Services.AddDevExpressServerSideBlazorReportViewer();

// Optional: resolve reports by name via IReportProvider
builder.Services.AddScoped<DevExpress.XtraReports.Services.IReportProvider, MyReportProvider>();

builder.WebHost.UseWebRoot("wwwroot");
builder.WebHost.UseStaticWebAssets();

var app = builder.Build();
app.UseStaticFiles();
app.UseRouting();
app.UseAntiforgery();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
app.Run();

// ─── App.razor (head section) ─────────────────────────────────────────────────
// <head>
//     @DxResourceManager.RegisterScripts()
//     @DxResourceManager.RegisterTheme(Themes.BlazingBerry)
//     <link rel="stylesheet"
//           href="_content/DevExpress.Blazor.Reporting.Viewer/css/dx-blazor-reporting-components.bs5.css" />
// </head>

// ─── _Imports.razor ───────────────────────────────────────────────────────────
// @using DevExpress.Blazor
// @using DevExpress.Blazor.Reporting

// ─── Services/MyReportProvider.cs ────────────────────────────────────────────

using DevExpress.XtraReports.Services;
using DevExpress.XtraReports.UI;

public class MyReportProvider : IReportProvider {
    public XtraReport GetReport(string reportName, ReportProviderContext context) {
        return reportName switch {
            "SalesReport"   => new SalesReport(),
            "InvoiceReport" => new InvoiceReport(),
            _ => throw new InvalidOperationException($"Report '{reportName}' not found.")
        };
    }
}

// ─── Pages/ReportViewer.razor ─────────────────────────────────────────────────
//
// @page "/viewer"
// @page "/viewer/{ReportName?}"
// @rendermode InteractiveServer
//
// @using DevExpress.Blazor.Reporting
// @using DevExpress.XtraReports.Services
//
// @inject IReportProvider ReportProvider
//
// <DxReportViewer @ref="reportViewer" Report="@Report" />
//
// @code {
//     DxReportViewer reportViewer;
//     DevExpress.XtraReports.UI.XtraReport Report;
//
//     [Parameter] public string ReportName { get; set; } = "SalesReport";
//
//     protected override void OnInitialized() {
//         Report = ReportProvider.GetReport(ReportName, null);
//     }
// }

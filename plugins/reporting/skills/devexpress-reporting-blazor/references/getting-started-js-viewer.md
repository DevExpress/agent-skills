# Getting Started — JavaScript-Based Document Viewer (Blazor)

## When to Use This Reference

Use this when you need to:
- Add `DxDocumentViewer` (SignalR, no ASP.NET Core backend) or `DxWasmDocumentViewer` (fetch, with backend) to a Blazor app
- Use the JS-based viewer when you need JavaScript client-side events or right-to-left UI

## Component Comparison

| Component | Communication | Requires ASP.NET Core Backend | Requires MVC Controllers |
|-----------|---------------|-------------------------------|--------------------------|
| `DxDocumentViewer` | SignalR | No | No |
| `DxWasmDocumentViewer` | HTTP fetch | Yes | Yes |

For the simpler Server-mode case, use `DxDocumentViewer`. For a WASM frontend with a separate backend, use `DxWasmDocumentViewer`.

## NuGet Packages

```bash
# Blazor project
dotnet add package DevExpress.Blazor.Reporting.JSBasedControls

# If using DxWasmDocumentViewer with an ASP.NET Core backend:
dotnet add package DevExpress.AspNetCore.Reporting  # backend project
```

## Program.cs (Blazor Web App — Interactive Server with DxDocumentViewer)

```csharp
using DevExpress.Blazor.Reporting;
using DevExpress.XtraReports.Web.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMvc();
builder.Services.AddDevExpressBlazorReporting();

// Register IReportProvider to resolve report names
builder.Services.AddScoped<DevExpress.XtraReports.Services.IReportProvider, MyReportProvider>();

// Or if using ReportStorageWebExtension for designer support:
// builder.Services.AddScoped<ReportStorageWebExtension, CustomReportStorageWebExtension>();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.UseAntiforgery();
app.UseDevExpressBlazorReporting();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
app.Run();
```

> **Order**: `UseDevExpressBlazorReporting()` must be called after `UseRouting()`.

## _Imports.razor

```razor
@using DevExpress.Blazor
@using DevExpress.Blazor.Reporting
```

## App.razor

```razor
<head>
    @DxResourceManager.RegisterScripts()
</head>
```

## DocumentViewer.razor

```razor
@page "/documentviewer"
@rendermode InteractiveServer

<DxDocumentViewer ReportName="SalesReport" Height="1000px" Width="100%">
    <DxDocumentViewerTabPanelSettings Width="340" />
</DxDocumentViewer>
```

## IReportProvider for Report Name Resolution

```csharp
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
```

## DxWasmDocumentViewer Setup (with ASP.NET Core Backend)

The WASM viewer communicates via HTTP fetch and requires a separate ASP.NET Core backend project with `WebDocumentViewerController`. See [getting-started.md in the aspnetcore skill](../../devexpress-reporting-aspnetcore/references/getting-started.md) for controller setup.

```razor
@page "/viewer"
@rendermode InteractiveWebAssembly

<DxWasmDocumentViewer ReportName="SalesReport" Height="1000px" Width="100%">
    <DxWasmDocumentViewerRequestOptions
        GetDocumentViewerModelAction="DXXRDV/GetDocumentViewerModel" />
</DxWasmDocumentViewer>
```

## Nested Customization Components

```razor
<DxDocumentViewer ReportName="SalesReport" Height="1000px" Width="100%">
    <DxDocumentViewerTabPanelSettings Width="340" />
    <DxDocumentViewerProgressBarSettings Position="ProgressBarPosition.TopRight" />
    <DxDocumentViewerSearchSettings SearchEnabled="true" />
    <DxDocumentViewerExportSettings UseAsynchronousExport="true" />
    <DxDocumentViewerCallbacks
        DocumentReady="onDocumentReady"
        CustomizeMenuActions="onCustomizeMenuActions" />
</DxDocumentViewer>
```

## Client-Side Events (JavaScript)

```javascript
// wwwroot/js/viewer-events.js (reference from _Host.cshtml or per-page)
function onDocumentReady(s, e) {
    // e.PageCount — number of pages in the document
    s.GoToPage(e.PageCount - 1);
}

function onCustomizeMenuActions(s, e) {
    var printAction = e.GetById(DevExpress.Reporting.Viewer.ActionId.Print);
    if (printAction) printAction.visible = false;
}
```

## Accessibility

```razor
<DxDocumentViewer ReportName="SalesReport" AccessibilityCompliant="true" Height="1000px" Width="100%" />
```

## WebAssembly Limitations

- Only JSON and Object data sources supported (no SQL)
- Skia package required: `DevExpress.Drawing.Skia`
- `WasmBuildNative=true` required in project file

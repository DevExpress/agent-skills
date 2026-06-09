# Getting Started — JS-Based Document Viewer: Blazor Web App (Interactive Server)

## When to Use This Reference

Use this when you are building a **Blazor Web App with Interactive Server render mode** and need a document viewer with JavaScript client-side events, right-to-left UI support, or enhanced mobile compatibility.

- Single-project deployment
- Communication via SignalR
- Report rendering happens server-side
- Component: `DxDocumentViewer`

> **Not your scenario?**
> — Standalone WebAssembly with no backend → 📄 [getting-started-js-viewer-standalone-wasm.md](getting-started-js-viewer-standalone-wasm.md)
> — Interactive WebAssembly (Client + Server) → 📄 [getting-started-js-viewer-interactive-wasm.md](getting-started-js-viewer-interactive-wasm.md)
> — Native Blazor viewer (no JS events needed) → 📄 [getting-started-native-viewer.md](getting-started-native-viewer.md)

## Setup Workflow

Follow these steps to integrate `DxDocumentViewer` into your **Interactive Server** Blazor app:

1. **[Install NuGet Packages](#install-nuget-packages)** — Add `DevExpress.Blazor.Reporting.JSBasedControls` and `DevExpress.AspNetCore.Reporting`
2. **[Register client resources in App.razor](#register-client-resources-in-apprazor)** 
3. **[Register namespaces in _Imports.razor](#register-namespaces-in-_importsrazor)**
4. **[Implement Report Name Resolution Service](#implement-a-report-name-resolution-service)** — Register `IReportProvider` or `ReportStorageWebExtension` to load reports by name
5. **[Configure Program.cs](#configure-programcs)** — Register services in correct order: `AddMvc()` → `AddDevExpressBlazorReporting()`; then `UseRouting()` → `UseDevExpressBlazorReporting()` → `MapControllers()`
6. **[Add viewer to a page](#add-a-document-viewer-to-a-page)** — Place `<DxDocumentViewer>` with `@rendermode InteractiveServer`
7. **[Customize the Viewer (optional)](#customize-the-viewer-optional)** — Tab panel, progress bar, export settings, client-side events

## Install NuGet Packages

```bash
dotnet add package DevExpress.Blazor.Reporting.JSBasedControls
dotnet add package DevExpress.AspNetCore.Reporting
```

> **Version consistency**: If your project already references other DevExpress packages, ensure both packages above use the **same version** as existing ones. 

## Register Namespaces in _Imports.razor

```razor
@using DevExpress.Blazor
@using DevExpress.Blazor.Reporting
```

## Register Client Resources in App.razor

```razor
<head>
    @DxResourceManager.RegisterScripts()
</head>
```

## Implement a Report Name Resolution Service

When you pass a report name to `DxDocumentViewer` via the `ReportName` property, you must register a service to resolve that name to a report instance. The recommended service is `IReportProvider`. It is also possible to use `ReportStorageWebExtension` instead of `IReportProvider`.

See 📄 [resolving-report-names.md](resolving-report-names.md) for full details and alternatives.

## Configure Program.cs

```csharp
// required DevExpress Blazor Reporting namespaces:
using DevExpress.Blazor.Reporting;
using DevExpress.XtraReports.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

// Register MVC infrastructure and DevExpress services:
builder.Services.AddMvc();
builder.Services.AddDevExpressBlazorReporting();

// Register Report Name Resolution Service implemented on the previous step - must come after AddDevExpressBlazorReporting()
builder.Services.AddScoped<IReportProvider, MyReportProvider>();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.UseAntiforgery();
// Use DevExpress middleware - must come after UseRouting():
app.UseDevExpressBlazorReporting();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
app.MapControllers();
app.Run();
```

**Critical order rules:**
- `AddMvc()` must come **before** `AddDevExpressBlazorReporting()` — JS-based components need MVC infrastructure (`IUrlHelperFactory`). Missing this causes `Unable to resolve service for type 'IUrlHelperFactory'` at startup.
- `AddScoped<IReportProvider, ...>()` must come **after** `AddDevExpressBlazorReporting()` — DevExpress registers an internal proxy inside that call; .NET DI uses last-registered-wins.
- `UseDevExpressBlazorReporting()` must come **after** `UseRouting()`.
- `MapControllers()` must be present — it maps DevExpress reporting controller routes.

## Add a Document Viewer to a Page

```razor
@page "/documentviewer"
@rendermode InteractiveServer

<DxDocumentViewer ReportName="TestReport" Height="1000px" Width="100%">
    <DxDocumentViewerTabPanelSettings Width="340" />
</DxDocumentViewer>
```

## Customize the Viewer (Optional)

For viewer customization options, see 📄 [customizing-js-viewer.md](customizing-js-viewer.md).

## Accessibility

```razor
<DxDocumentViewer ReportName="TestReport" AccessibilityCompliant="true" Height="1000px" Width="100%" />
```

## Error Handling

Implement `IWebDocumentViewerExceptionHandler` to intercept server-side errors:

```csharp
using DevExpress.XtraReports.Web.WebDocumentViewer;

public class CustomExceptionHandler : IWebDocumentViewerExceptionHandler {
    public void HandleException(Exception e, string operationName, OperationResult result) {
        result.Message = "An error occurred while generating the report.";
    }
}
```

Register in `Program.cs`:
```csharp
builder.Services.AddScoped<IWebDocumentViewerExceptionHandler, CustomExceptionHandler>();
```

## Constraints

1. **MVC infrastructure is mandatory**: Call `AddMvc()` before `AddDevExpressBlazorReporting()`, and `UseDevExpressBlazorReporting()` after `UseRouting()`. Missing any step causes startup crashes or 404 errors on report requests.
2. **`IReportProvider` registration order**: Register after `AddDevExpressBlazorReporting()`.
3. **`IReportProvider` and `ReportStorageWebExtension` are mutually exclusive**: Do not register both in the same project. See 📄 [resolving-report-names.md](resolving-report-names.md) for details.

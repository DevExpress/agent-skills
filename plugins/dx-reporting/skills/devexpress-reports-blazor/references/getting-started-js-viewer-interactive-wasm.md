# Getting Started — JS-Based Document Viewer: Blazor Web App (Interactive WebAssembly with ASP.NET Core Backend)

## When to Use This Reference

Use this when you are building a **Blazor Web App with Interactive WebAssembly render mode** backed by an ASP.NET Core server, and need a document viewer with JavaScript client-side events.

- Two-project structure: ASP.NET Core backend + Blazor WASM client
- Report rendering happens **server-side** (backend); the viewer UI runs in the browser
- Full data source support (SQL, JSON, MongoDB, Object)
- Component: `DxWasmDocumentViewer` (client) + `WebDocumentViewerController` (backend)
- Also compatible with the Blazor WebAssembly Hosted template (ASP.NET Core 7.0 and earlier)

> **Not your scenario?**
> — Interactive Server (single project) → 📄 [getting-started-js-viewer-server.md](getting-started-js-viewer-server.md)
> — Interactive WebAssembly Standalone (no backend, browser-only) → 📄 [getting-started-js-viewer-standalone-wasm.md](getting-started-js-viewer-standalone-wasm.md)
> — Native Blazor viewer → 📄 [getting-started-native-viewer.md](getting-started-native-viewer.md)

## Setup Workflow

Follow these steps to integrate `DxWasmDocumentViewer` into your **Hosted Blazor WebAssembly** app:

**Backend (server project):**
1. **[Install backend NuGet Package](#install-nuget-packages)** — Add `DevExpress.AspNetCore.Reporting`
2. **[Register client resources in App.razor](#asp.net-core-backend-server-project-register-client-resources-in-apprazor)** — Add `DxResourceManager.RegisterScripts()` to the backend `App.razor <head>`
3. **[Register namespaces in _Imports.razor](#asp.net-core-backend-server-project-register-namespaces-in-_importsrazor)**
4. **[Implement Report Name Resolution Service](#asp.net-core-backend-server-project-implement-a-report-name-resolution-service)** — Register `IReportProvider` or `ReportStorageWebExtension` to load reports by name
5. **[Implement a WebDocumentViewer Controller](#asp.net-core-backend-server-project-implement-a-webdocumentviewer-controller)** — Create `WebDocumentViewerController` to handle viewer requests
6. **[Configure backend Program.cs](#asp.net-core-backend-server-project-configure-programcs)** — Call `AddDevExpressControls()` + `IReportProvider`; then `UseDevExpressControls()` + `MapControllers()`

**Client (WASM project):**
7. **[Install client NuGet Package](#install-nuget-packages)** — Add `DevExpress.Blazor.Reporting.JSBasedControls.WebAssembly`
8. **[Register namespaces in client _Imports.razor](#blazor-wasm-client-project-register-namespaces-in-_importsrazor)** — Add `DevExpress.Blazor.Reporting` to the client
9. **[Add viewer to a page](#blazor-wasm-client-project-add-a-document-viewer-to-a-page)** — Place `<DxWasmDocumentViewer>` with `@rendermode InteractiveWebAssembly`

## Project Structure

```
YourSolution/
├── BlazorWasmApp/              ← ASP.NET Core backend (server project)
│   ├── Program.cs
│   ├── App.razor
│   ├── Controllers/
│   │   └── ReportingControllers.cs
│   ├── Reports/                ← .repx files stored here
│   ├── ReportProvider.cs
│   └── ReportFactory.cs
└── BlazorWasmApp.Client/       ← Blazor WASM frontend (client project)
    ├── Program.cs
    └── Pages/
        └── DocumentViewer.razor
```

## Install NuGet Packages

| Project | Package |
|---------|---------|
| Backend (server) | `DevExpress.AspNetCore.Reporting` |
| Client (WASM) | `DevExpress.Blazor.Reporting.JSBasedControls.WebAssembly` |

```bash
# Backend project
dotnet add package DevExpress.AspNetCore.Reporting

# Client project
dotnet add package DevExpress.Blazor.Reporting.JSBasedControls.WebAssembly
```

> **Version consistency**: If your project already references other DevExpress packages, ensure both packages above use the **same version** as existing ones. 

## ASP.NET Core backend (server project): Register Namespaces in _Imports.razor

```razor
@using DevExpress.Blazor
```

## ASP.NET Core backend (server project): Register Client Resources in App.razor

The `DxResourceManager.RegisterScripts()` call must be in the **backend** project's `App.razor`, since that is the host page:

```razor
<head>
    @DxResourceManager.RegisterScripts()
</head>
```

## ASP.NET Core backend (server project): Implement a Report Name Resolution Service

When you pass a report name to `DxDocumentViewer` via the `ReportName` property, you must register a service to resolve that name to a report instance. The recommended service is `IReportProvider`. It is also possible to use `ReportStorageWebExtension` instead of `IReportProvider`.

See 📄 [resolving-report-names.md](resolving-report-names.md) for full details and alternatives.

## ASP.NET Core backend (server project): Implement a WebDocumentViewer Controller

Create `Controllers/ReportingControllers.cs` in the backend project:

```csharp
using DevExpress.AspNetCore.Reporting.WebDocumentViewer;
using DevExpress.AspNetCore.Reporting.WebDocumentViewer.Native.Services;

public class CustomWebDocumentViewerController : WebDocumentViewerController {
    public CustomWebDocumentViewerController(IWebDocumentViewerMvcControllerService controllerService)
        : base(controllerService) { }
}
```

## ASP.NET Core backend (server project): Configure Program.cs

```csharp
// required DevExpress Blazor Reporting namespaces:
using DevExpress.AspNetCore;
using DevExpress.XtraReports.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
// Register DevExpress services:
builder.Services.AddDevExpressControls();

// Register IReportProvider AFTER AddDevExpressControls():
builder.Services.AddScoped<IReportProvider, ReportProvider>();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
// Use DevExpress middleware:
app.UseDevExpressControls();

app.MapRazorPages();
app.MapControllers();
app.Run();
```

**Critical order rules:**
- `AddDevExpressControls()` must be called to register DevExpress services.
- `IReportProvider` must be registered **after** `AddDevExpressControls()`.
- `MapControllers()` must be present — it maps the `WebDocumentViewerController` routes.

## Blazor WASM client project: Register Namespaces in _Imports.razor

```razor
@using DevExpress.Blazor.Reporting
```

## Blazor WASM client project: Add a Document Viewer to a Page

```razor
@page "/viewer"
@rendermode InteractiveWebAssembly

<DxWasmDocumentViewer ReportName="TestReport" Height="700px" Width="100%">
    <DxDocumentViewerExportSettings UseSameTab="false" />
    <DxWasmDocumentViewerRequestOptions InvokeAction="DXXRDV" />
</DxWasmDocumentViewer>
```

`InvokeAction="DXXRDV"` is the default route prefix for `WebDocumentViewerController` — it must match the controller's route.

## Constraints

1. **`WebDocumentViewerController` is required**: `DxWasmDocumentViewer` communicates via HTTP fetch to the backend controller. Without it, all viewer requests return 404.
2. **`App.razor` is in the backend project**: `DxResourceManager.RegisterScripts()` must go in the backend's `App.razor` (the host page), not the client project.
3. **No `AddMvc()` needed**: The backend uses `AddControllersWithViews()` or `AddControllers()`, not `AddMvc()`.
4. **`IReportProvider` and `ReportStorageWebExtension` are mutually exclusive**: Do not register both. See 📄 [resolving-report-names.md](resolving-report-names.md) for details.

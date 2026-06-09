# Getting Started — JS-Based Report Designer: Blazor Web App (Interactive WebAssembly with ASP.NET Core Backend)

## When to Use This Reference

Use this when you are building a **Blazor Web App with Interactive WebAssembly render mode**, where a WASM frontend communicates with an ASP.NET Core backend for report processing.

- Two-project structure: ASP.NET Core backend + Blazor WASM client
- Report rendering happens **server-side** (backend); the designer UI is delivered to the WASM client
- Full data source support (SQL, JSON, MongoDB, Object)
- Component: `DxWasmReportDesigner` (client) + three MVC controllers (backend)
- Also compatible with the Blazor WebAssembly Hosted template (ASP.NET Core 7.0 and earlier)

> **Not your scenario?**
> — Interactive Server (single project) → 📄 [getting-started-js-designer-server.md](getting-started-js-designer-server.md)
> — Interactive WebAssembly Standalone (no backend, browser-only) → 📄 [getting-started-js-designer-standalone-wasm.md](getting-started-js-designer-standalone-wasm.md)

## Setup Workflow

Follow these steps to integrate `DxWasmReportDesigner` into your **Hosted Blazor WebAssembly** app (ASP.NET Core backend + WASM client):

**Backend (server project):**
1. **[Install backend NuGet Package](#install-nuget-packages)** — Add `DevExpress.AspNetCore.Reporting` and `DevExpress.Drawing.Skia`
2. **[Register client resources in App.razor](#asp.net-core-backend-server-project-register-client-resources-in-apprazor)** — Add `DxResourceManager.RegisterScripts()` to the backend `App.razor <head>`
3. **[Register namespaces](#asp.net-core-backend-server-project-register-namespaces-in-_importsrazor)** — Update backend `_Imports.razor`
4. **[Implement Report Storage](#asp.net-core-backend-server-project-implement-a-report-storage)** — Create `ReportStorageWebExtension` for Save/Load/Open dialogs
5. **[Create three Reporting Controllers](#asp.net-core-backend-server-project-create-three-reporting-controllers)** — Implement `WebDocumentViewer`, `ReportDesigner`, and `QueryBuilder` controllers
6. **[Configure backend Program.cs](#asp.net-core-backend-server-project-register-devexpress-resources-and-add-a-report-storage-in-programcs)** — Call `AddDevExpressControls()` + storage, then `UseDevExpressControls()` + `MapControllers()`
7. **[Configure Data Sources (optional)](#asp.net-core-backend-server-project-configure-data-sources-optional)** — Register SQL, JSON, MongoDB, or Object data sources

**Client (WASM project):**
8. **[Install client NuGet Package](#install-nuget-packages)** — Add `DevExpress.Blazor.Reporting.JSBasedControls.WebAssembly`
9. **[Configure client Program.cs](#blazor-wasm-frontend-client-project-add-an-http-client-in-programcs)** — Add HTTP client pointing to the backend base address
10. **[Add designer to a page](#blazor-wasm-frontend-client-project-add-a-report-designer-to-a-page)** — Place `<DxWasmReportDesigner>` with `@rendermode InteractiveWebAssembly`
11. **[Customize the Designer (optional)](#customize-the-designer-ui-and-behavior-optional)** — Toolbar commands, wizard flow, UI panels, event callbacks

## Project Structure

```
YourSolution/
├── BlazorWasmApp/              ← ASP.NET Core backend (server project)
│   ├── Program.cs
│   ├── App.razor
│   ├── Controllers/
│   │   └── ReportingControllers.cs
│   ├── Reports/                ← .repx files stored here
│   ├── ReportStorage.cs
│   └── ReportFactory.cs
└── BlazorWasmApp.Client/       ← Blazor WASM frontend (client project)
    ├── Program.cs
    └── Pages/
        └── ReportDesigner.razor
```

## Install NuGet Packages

| Project | Package |
|---------|---------|
| Backend (server) | `DevExpress.AspNetCore.Reporting`, `DevExpress.Drawing.Skia` |
| Client (WASM) | `DevExpress.Blazor.Reporting.JSBasedControls.WebAssembly` |

```bash
# Backend project
dotnet add package DevExpress.AspNetCore.Reporting
dotnet add package DevExpress.Drawing.Skia

# Client project
dotnet add package DevExpress.Blazor.Reporting.JSBasedControls.WebAssembly
```
> **Version consistency**: If your project already references other DevExpress packages, ensure all three DevExpress packages above use the **same version** as existing ones. Run `dotnet list package` to verify your current DevExpress package versions before installing.

## ASP.NET Core backend (server project): Register namespaces in _Imports.razor

```razor
@using DevExpress.Blazor
```

## ASP.NET Core backend (server project): Register client resources in App.razor

The `DxResourceManager.RegisterScripts()` call must be in the **backend** project's `App.razor`, since that is the host page:

```razor
<head>
    @DxResourceManager.RegisterScripts()
</head>
```

## ASP.NET Core backend (server project): Implement a Report Storage

Implement `ReportStorageWebExtension` in the backend project. See 📄 [resolving-report-names.md](resolving-report-names.md) for the full implementation including `IsValidUrl`, `CanSetData`, `GetData`, `GetUrls`, `SetData`, and `SetNewData`.

## ASP.NET Core backend (server project): Register DevExpress Resources and Add a Report Storage in Program.cs

```csharp
using DevExpress.AspNetCore;
using DevExpress.AspNetCore.Reporting;
using DevExpress.XtraReports.Web.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();
builder.Services.AddDevExpressControls();

// Register report storage for the designer's Save/Load/Open dialogs.
// Must come AFTER AddDevExpressControls().
builder.Services.AddScoped<ReportStorageWebExtension, ReportStorage>();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.UseDevExpressControls();

app.MapRazorPages();
app.MapControllers();
app.Run();
```

## ASP.NET Core backend (server project): Create Three Reporting Controllers

Create `Controllers/ReportingControllers.cs` in the backend project. All three controllers are required — the designer embeds a viewer internally and uses the query builder for SQL data sources:

```csharp
using DevExpress.AspNetCore.Reporting.QueryBuilder;
using DevExpress.AspNetCore.Reporting.QueryBuilder.Native.Services;
using DevExpress.AspNetCore.Reporting.ReportDesigner;
using DevExpress.AspNetCore.Reporting.ReportDesigner.Native.Services;
using DevExpress.AspNetCore.Reporting.WebDocumentViewer;
using DevExpress.AspNetCore.Reporting.WebDocumentViewer.Native.Services;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Web.ReportDesigner;

public class CustomWebDocumentViewerController : WebDocumentViewerController {
    public CustomWebDocumentViewerController(IWebDocumentViewerMvcControllerService controllerService)
        : base(controllerService) { }
}

public class CustomReportDesignerController(IReportDesignerMvcControllerService controllerService)
    : ReportDesignerController(controllerService) {

    [HttpPost("[action]")]
    public async Task<object> GetReportDesignerModel(
        [FromForm] string reportUrl,
        [FromForm] ReportDesignerSettingsBase designerModelSettings,
        [FromServices] IReportDesignerClientSideModelGenerator designerClientSideModelGenerator) {

        ReportDesignerModel model = string.IsNullOrEmpty(reportUrl)
            ? await designerClientSideModelGenerator.GetModelAsync(
                new XtraReport(), null, "/DXXRD", "/DXXRDV", "/DXXQB")
            : await designerClientSideModelGenerator.GetModelAsync(
                reportUrl, null, "/DXXRD", "/DXXRDV", "/DXXQB");

        model.Assign(designerModelSettings);
        return Content(designerClientSideModelGenerator.GetJsonModelScript(model), "application/json");
    }
}

public class CustomQueryBuilderController : QueryBuilderController {
    public CustomQueryBuilderController(IQueryBuilderMvcControllerService controllerService)
        : base(controllerService) { }
}
```

## ASP.NET Core backend (server project): Configure Data Sources (Optional)

### Register Connection String Providers

Store connection strings in `appsettings.json` and register built-in providers to make them available in the Report Wizard and Data Source Wizard.

Add `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "NWindConnectionString": "XpoProvider=SQLite;Data Source=|DataDirectory|/Data/nwind.db",
    "JsonConnection": "Uri=https://raw.githubusercontent.com/DevExpress-Examples/DataSources/master/JSON/customers.json",
    "MongoDBConnection": "mongodb://localhost:27017/"
  }
}
```

`Program.cs`:
```csharp
builder.Services.ConfigureReportingServices(configurator => {
    configurator.ConfigureReportDesigner(designerConfigurator => {
        designerConfigurator.RegisterDataSourceWizardConfigFileConnectionStringsProvider();
        designerConfigurator.RegisterDataSourceWizardConfigFileJsonConnectionStringsProvider();
        designerConfigurator.RegisterDataSourceWizardConfigFileMongoDBConnectionStringsProvider();
    });
});
```

### Predefine Data Sources

Register data sources in `GetReportDesignerModel` in `CustomReportDesignerController`.

```csharp
// Inside GetReportDesignerModel in CustomReportDesignerController:
using DevExpress.DataAccess.Json;
using DevExpress.DataAccess.Sql;

Dictionary<string, object> dataSources = new();

// SQL data source example
var sqlDataSource = new SqlDataSource("NWindConnectionString");
SelectQuery query = SelectQueryFluentBuilder.AddTable("Products")
    .SelectAllColumnsFromTable().Build("Products");
sqlDataSource.Queries.Add(query);
sqlDataSource.RebuildResultSchema();
dataSources.Add("SqlDataSource", sqlDataSource);

// JSON data source example
var jsonDataSource = new JsonDataSource();
jsonDataSource.JsonSource = new UriJsonSource(
    new Uri("https://raw.githubusercontent.com/DevExpress-Examples/DataSources/master/JSON/customers.json"));
jsonDataSource.Fill();
dataSources.Add("JsonDataSource", jsonDataSource);
```

Pass `dataSources` as the second parameter to `GetModelAsync` instead of `null`:

```csharp
ReportDesignerModel model = string.IsNullOrEmpty(reportUrl)
    ? await designerClientSideModelGenerator.GetModelAsync(
        new XtraReport(), dataSources, "/DXXRD", "/DXXRDV", "/DXXQB")
    : await designerClientSideModelGenerator.GetModelAsync(
        reportUrl, dataSources, "/DXXRD", "/DXXRDV", "/DXXQB");
```

### Register Object Data Source Provider

```csharp
// 1. Implement IObjectDataSourceWizardTypeProvider
using DevExpress.DataAccess.Web;

public class ObjectDataSourceWizardCustomTypeProvider : IObjectDataSourceWizardTypeProvider {
    public IEnumerable<Type> GetAvailableTypes(string context) {
        return new[] { typeof(Employees.DataSource) };
    }
}
// 2. Register the provider and trust the custom type in Program.cs
builder.Services.ConfigureReportingServices(configurator => {
    configurator.ConfigureReportDesigner(designerConfigurator => {
        designerConfigurator.RegisterObjectDataSourceWizardTypeProvider
            <ObjectDataSourceWizardCustomTypeProvider>();
    });
});

DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(Employees.DataSource));
```

## ASP.NET Core backend (server project): Register Trusted Classes for Data Source Deserialization

**Critical step for designer-based projects:** When a Report Designer deserializes saved reports (on preview or during save operations), it validates the data source types embedded in the report XML against a trusted-types list. If your reports use **custom data source classes** (not built-in SQL/JSON), they must be registered **before** `WebApplication.CreateBuilder()` is called.

> **This applies ONLY to custom business classes**, not to built-in DevExpress types (`SqlDataSource`, `JsonDataSource`, etc.).

### Quick Checklist

1. **Search your report files** for `[DataSource]` attributes, `ObjectDataSource`, or `List<T>` properties
2. **Extract class names** — e.g., `SalesData`, `ProductRecord`, `CustomerDto`
3. **For each class, register both the type and its array variant** at the top of `Program.cs`:
   ```csharp
   DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(YourClass));
   DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(YourClass[]));
   ```

### Example: Registering Custom Data Source Classes

If your reports use `SalesData` and `ProductRecord` as data sources:

**Program.cs (top of file, before `WebApplication.CreateBuilder()`):**
```csharp
using DevExpress.Utils;

// Register trusted types for Report Designer data source deserialization.
// The designer deserializes report XML during preview and save operations.
// Custom data source classes must be trusted BEFORE builder creation.
// Array types must be registered separately — deserialization treats them as distinct types.
DeserializationSettings.RegisterTrustedClass(typeof(SalesData));
DeserializationSettings.RegisterTrustedClass(typeof(SalesData[]));
DeserializationSettings.RegisterTrustedClass(typeof(ProductRecord));
DeserializationSettings.RegisterTrustedClass(typeof(ProductRecord[]));

var builder = WebApplication.CreateBuilder(args);
// ... rest of Program.cs
```

### Why Array Types Matter

When the Report Designer serializes a report with a `List<YourClass>` data source, the XML includes both `YourClass` (for individual records) and `YourClass[]` (for collection deserialization). If either is missing from the trusted list, the designer will throw `NonTrustedTypeDeserializationException` on preview or save.

### Common Mistakes

❌ **Registering only the report class instead of the data source class**
- **Symptom**: "The Report class is not trusted" error during save
- **Fix**: Register the data source classes (`SalesData`, `ProductRecord`), not the report wrapper class

❌ **Forgetting array types**
- **Symptom**: Designer preview works but save fails with "The SalesData[] type is not trusted"
- **Fix**: Always register both `typeof(SalesData)` and `typeof(SalesData[])`

❌ **Placing registration after `CreateBuilder()`**
- **Symptom**: `NonTrustedTypeDeserializationException` at runtime
- **Fix**: Move all registrations to the top of `Program.cs`, **before** `WebApplication.CreateBuilder()`

## Blazor WASM frontend (client project): Add an HTTP client in Program.cs

The client project uses a typical setup and only needs the HTTP client to communicate with the backend reporting controllers:

```csharp
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Required:
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

await builder.Build().RunAsync();
```

## Blazor WASM frontend (client project): Add a Report Designer to a Page

```razor
@page "/designer"
@using DevExpress.Blazor.Reporting
@rendermode InteractiveWebAssembly

<DxWasmReportDesigner ReportName="TestReport" Height="700px" Width="100%">
    <DxWasmReportDesignerRequestOptions GetDesignerModelAction="DXXRD/GetReportDesignerModel" />
</DxWasmReportDesigner>
```

`GetDesignerModelAction` must match the route in `CustomReportDesignerController` — `DXXRD` is the default prefix.

## Customize the Designer UI and Behavior (Optional)

For toolbar commands, wizard flow, UI panels, and event callbacks on `DxWasmReportDesigner`, see 📄 [customizing-js-designer.md](customizing-js-designer.md).

## Antipatterns to Avoid

### Wrong NuGet package variant on client project

- **Antipattern**: Installing `DevExpress.Blazor.Reporting.JSBasedControls` (without `.WebAssembly` suffix) on the client (WASM) project in a hosted architecture
- **Why**: That package is for [Interactive Server](getting-started-js-designer-server.md) and includes server-side dependencies (MVC helpers, backend middleware hooks) that do not belong in a client-only WASM project. This causes build or runtime failures
- **Correct Pattern**:
  - Client project: `DevExpress.Blazor.Reporting.JSBasedControls.WebAssembly`
  - Backend project: `DevExpress.AspNetCore.Reporting`

### Missing `AddDevExpressControls()` before registering `ReportStorageWebExtension`

- **Antipattern**: Registering `ReportStorageWebExtension` before calling `AddDevExpressControls()` in backend Program.cs
- **Why**: DevExpress registers internal infrastructure inside `AddDevExpressControls()`; .NET DI uses last-registered-wins semantics, so custom storage must come after to override the default
- **Correct Pattern**:
  ```csharp
  builder.Services.AddDevExpressControls();  // ← First
  builder.Services.AddScoped<ReportStorageWebExtension, CustomReportStorageWebExtension>();  // ← Then custom storage
  ```

### A5: Missing `MapControllers()` in backend Program.cs

- **Antipattern**: Forgetting `app.MapControllers()` in the backend Program.cs
- **Why**: MVC routing must be mapped for all three reporting controllers (`WebDocumentViewerController`, `ReportDesignerController`, `QueryBuilderController`) to be discoverable. Without it, the client receives 404 errors on designer model requests
- **Correct Pattern**:
  ```csharp
  app.UseDevExpressControls();
  app.MapRazorPages();
  app.MapControllers();  // ← Must be present
  app.Run();
  ```


## Troubleshooting

**For generic issues** (render mode, report names, data source trust), see [troubleshooting.md](troubleshooting.md). Issues specific to Interactive WebAssembly (Client + Server) architecture:

| Symptom | Cause | Fix |
|---------|-------|-----|
| 404 on designer model requests | Required controllers missing or `MapControllers()` missing in backend | Verify all three controllers exist: `WebDocumentViewerController`, `ReportDesignerController`, `QueryBuilderController`; add `app.MapControllers()` to backend Program.cs |
| `UseDevExpressControls()` not found | Backend middleware missing | Add `app.UseDevExpressControls()` after `app.UseStaticFiles()` in backend Program.cs |
| Save/Load/Open buttons do nothing | `ReportStorageWebExtension` registration order wrong or `AddMvc()` missing | Register storage **after** `AddDevExpressControls()`; ensure `AddMvc()` is present before. [Registration order →](troubleshooting.md#common-registration-order-mistakes) |
| Designer shows blank page | Missing scripts in backend `App.razor` or render mode missing in client | Add `@DxResourceManager.RegisterScripts()` to backend `App.razor <head>`; ensure client page has render mode. [Details →](troubleshooting.md#quick-diagnostic-checklist) |
| `NonTrustedTypeDeserializationException` during preview/save | Custom data source class not trusted | Register each class **before** `CreateBuilder()`: `DeserializationSettings.RegisterTrustedClass(typeof(YourClass))` and `.RegisterTrustedClass(typeof(YourClass[]))`. [Diagnostic steps →](troubleshooting.md#data-source-trust-registration-errors) |
| CORS errors from client to backend | Backend CORS policy not configured | Add `builder.Services.AddCors(...)` and `app.UseCors(...)` to backend Program.cs |
# Getting Started — JS-Based Report Designer: Blazor Web App (Interactive Server)

## When to Use This Reference

Use this when you are building a **Blazor Web App with Interactive Server render mode** and need to let end users create, edit, and save reports in the browser.

- Single-project deployment
- Communication via SignalR
- Full data source support (SQL, JSON, MongoDB, Object)
- Component: `DxReportDesigner`

> **Not your scenario?**
> — Standalone WebAssembly with no backend → 📄 [getting-started-js-designer-standalone-wasm.md](getting-started-js-designer-standalone-wasm.md)
> — Interactive WebAssembly (Client + Server) → 📄 [getting-started-js-designer-interactive-wasm.md](getting-started-js-designer-interactive-wasm.md)

## Setup Workflow

Follow these steps to integrate `DxReportDesigner` into your **Interactive Server** Blazor app:

1. **[Install NuGet Packages](#install-nuget-packages)** — Add `DevExpress.Blazor.Reporting.JSBasedControls` and `DevExpress.AspNetCore.Reporting`
2. **[Register namespaces](#register-namespaces-in-_importsrazor)** — Update `_Imports.razor` with DevExpress namespaces
3. **[Register client resources](#register-client-resources-in-apprazor)** — Add `DxResourceManager.RegisterScripts()` to `App.razor <head>`
4. **[Implement Report Storage](#implement-a-report-storage)** — Create `ReportStorageWebExtension` to enable Save/Load/Open dialogs
5. **[Configure Program.cs](#register-resources-and-add-a-report-storage-in-programcs)** — Register services in correct order: `AddMvc()` → `AddDevExpressBlazorReporting()` → storage; then `UseRouting()` → `UseDevExpressBlazorReporting()` → `MapControllers()`
6. **[Add designer to a page](#add-a-razor-report-designer-component-to-a-page)** — Place `<DxReportDesigner>` with `@rendermode InteractiveServer`
7. **[Configure Data Sources (optional)](#configure-data-sources-optional)** — Register SQL, JSON, MongoDB, or Object data sources
8. **[Customize the Designer (optional)](#customize-the-designer-ui-and-behavior-optional)** — Toolbar commands, wizard flow, UI panels, event callbacks

## Install NuGet Packages

```bash
dotnet add package DevExpress.Blazor.Reporting.JSBasedControls
dotnet add package DevExpress.AspNetCore.Reporting
```

> **Version consistency**: If your project already references other DevExpress packages, ensure both packages above use the **same version** as existing ones.

## Register namespaces in _Imports.razor

```razor
@using DevExpress.Blazor
@using DevExpress.Blazor.Reporting
```

## Register client resources in App.razor

```razor
<head>
    @DxResourceManager.RegisterScripts()
</head>
```

## Implement a Report Storage

`DxReportDesigner` requires `ReportStorageWebExtension` to enable its Save, Save As, and Open dialogs. See 📄 [resolving-report-names.md](resolving-report-names.md) for the full `ReportStorageWebExtension` implementation, deserialization security (`RegisterTrustedClass`), and the file-system storage pattern.

## Register Resources and Add a Report Storage in Program.cs

```csharp
// Required namespaces:
using DevExpress.Blazor.Reporting;
using DevExpress.XtraReports.Web.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Required: JS-based components depend on MVC infrastructure:
builder.Services.AddMvc();
builder.Services.AddDevExpressBlazorReporting();

// Required: ReportStorageWebExtension for the designer's Save, Save As, and Open dialogs. Must come after AddDevExpressBlazorReporting:
builder.Services.AddScoped<ReportStorageWebExtension, CustomReportStorageWebExtension>();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.UseAntiforgery();

// Required: Must come after UseRouting:
app.UseDevExpressBlazorReporting(); 

app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
app.MapControllers();
app.Run();
```

**Critical order rules:**
- `AddMvc()` must come **before** `AddDevExpressBlazorReporting()` — JS-based components depend on MVC infrastructure (`IUrlHelperFactory`). Without it, startup throws `Unable to resolve service for type 'IUrlHelperFactory'`.
- `AddScoped<ReportStorageWebExtension, ...>()` must come **after** `AddDevExpressBlazorReporting()` — DevExpress registers an internal proxy inside that call; .NET DI uses last-registered wins, so user storage must come after.
- `UseDevExpressBlazorReporting()` must come **after** `UseRouting()`.
- `MapControllers()` must be present — it maps DevExpress reporting controller routes.

## Add a Razor Report Designer component to a Page

The minimal setup requires only size properties and the `ReportName` property to load a report.

```razor
@page "/reportdesigner"
@rendermode InteractiveServer

<DxReportDesigner ReportName="TestReport" Height="1000px" Width="100%" />
```

## Configure Data Sources (Optional)

### Register Connection String Providers

Store connection strings in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "NWindConnectionString": "XpoProvider=SQLite;Data Source=|DataDirectory|/Data/nwind.db",
    "JsonConnection": "Uri=https://raw.githubusercontent.com/DevExpress-Examples/DataSources/master/JSON/customers.json",
    "MongoDBConnection": "mongodb://localhost:27017/"
  }
}
```
Register built-in providers to make connection strings available in the Report Wizard and Data Source Wizard:
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

It is also possible to implement custom providers if connection strings need to be retrieved from a different source (e.g., database or external service):
[Register SQL Data Connections](https://docs.devexpress.com/XtraReports/400279)
[Register JSON Data Connections](https://docs.devexpress.com/XtraReports/401655)
[Register MongoDB Data Connections](https://docs.devexpress.com/XtraReports/405584)

### Predefine Data Sources

Pass data sources directly to the designer component. These data sources will be available in the Wizard. They are not required to open it:

```razor
@using DevExpress.DataAccess.Json
@using DevExpress.DataAccess.Sql

@page "/reportdesigner"
@rendermode InteractiveServer

<DxReportDesigner ReportName="TestReport" Height="calc(100vh - 130px)" Width="100%"
                  DataSources="DataSources">
</DxReportDesigner>

@code {
    Dictionary<string, object> DataSources = new();

    protected override async Task OnInitializedAsync() {
        await base.OnInitializedAsync();

        SqlDataSource sqlDataSource = new SqlDataSource("NWindConnectionString");
        SelectQuery query = SelectQueryFluentBuilder.AddTable("Products")
            .SelectAllColumnsFromTable().Build("Products");
        sqlDataSource.Queries.Add(query);
        sqlDataSource.RebuildResultSchema();

        JsonDataSource jsonDataSource = new JsonDataSource();
        jsonDataSource.JsonSource = new UriJsonSource(
            new Uri("https://raw.githubusercontent.com/DevExpress-Examples/DataSources/master/JSON/customers.json"));
        jsonDataSource.Fill();

        DataSources.Add("SqlDataSource", sqlDataSource);
        DataSources.Add("JsonDataSource", jsonDataSource);
    }
}
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

## Register Trusted Classes for Data Source Deserialization

**Critical step for designer-based projects:** When a Report Designer deserializes saved reports (on preview or during save operations), it validates the data source types embedded in the report XML against a trusted-types list. If your reports use **custom data source classes** (not built-in SQL/JSON), they must be registered **before** `WebApplication.CreateBuilder()` is called.

> **This applies ONLY to custom business classes**, not to built-in DevExpress types (`SqlDataSource`, `JsonDataSource`, etc.) or .NET collections.

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

❌ **Registering the report class instead of the data source class**
- **Symptom**: "The Report class is not trusted" error during save
- **Fix**: Register the data source classes (`SalesData`, `ProductRecord`), not the report wrapper class

❌ **Forgetting array types**
- **Symptom**: Designer preview works but save fails with "The SalesData[] type is not trusted"
- **Fix**: Always register both `typeof(SalesData)` and `typeof(SalesData[])`

❌ **Placing registration after `CreateBuilder()`**
- **Symptom**: `NonTrustedTypeDeserializationException` at runtime
- **Fix**: Move all registrations to the top of `Program.cs`, **before** `WebApplication.CreateBuilder()`

## Customize the Designer UI and Behavior (Optional)

For toolbar commands, wizard flow, UI panels, and event callbacks, see 📄 [customizing-js-designer.md](customizing-js-designer.md).

## Antipatterns to Avoid

### A3: Missing `AddMvc()` before `AddDevExpressBlazorReporting()`

- **Antipattern**: Calling `AddDevExpressBlazorReporting()` without first calling `AddMvc()`
- **Why**: `DxReportDesigner` is a JS-based component that depends on MVC infrastructure (`IUrlHelperFactory`). Without it, the app crashes at startup with `Unable to resolve service for type 'IUrlHelperFactory'`
- **Correct Pattern**:
  ```csharp
  builder.Services.AddMvc();  // ← Must come FIRST
  builder.Services.AddDevExpressBlazorReporting();  // ← Then this
  ```

### Missing `AddDevExpressBlazorReporting()` before registering `ReportStorageWebExtension`

- **Antipattern**: Registering `ReportStorageWebExtension` before calling `AddDevExpressBlazorReporting()`
- **Why**: DevExpress registers an internal proxy inside `AddDevExpressBlazorReporting()`; .NET DI uses last-registered-wins semantics, so your storage must be registered after to override the proxy
- **Correct Pattern**:
  ```csharp
  builder.Services.AddDevExpressBlazorReporting();  // ← First
  builder.Services.AddScoped<ReportStorageWebExtension, CustomReportStorageWebExtension>();  // ← Then custom storage
  ```

### A4: Incorrect middleware ordering with `UseDevExpressBlazorReporting()`

- **Antipattern**: Calling `UseDevExpressBlazorReporting()` before `UseRouting()`, or forgetting it altogether
- **Why**: DevExpress middleware must be initialized **after** routing is set up to intercept and handle API requests
- **Correct Pattern**:
  ```csharp
  app.UseRouting();  // ← First
  app.UseDevExpressBlazorReporting();  // ← After routing
  app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
  app.MapControllers();
  ```

### A5: Missing `MapControllers()`

- **Antipattern**: Forgetting `app.MapControllers()` in Program.cs
- **Why**: MVC routing must be mapped for DevExpress reporting controllers to be discoverable; without it, designer Save/Open buttons fail with 404 errors
- **Correct Pattern**:
  ```csharp
  app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
  app.MapControllers();  // ← Must be present
  ```

## Troubleshooting

For generic troubleshooting steps (render mode, service registration, data source trust, report not found), see [troubleshooting.md](troubleshooting.md). Common scenarios for this component:

| Symptom | Cause | Fix |
|---------|-------|-----|
| `Unable to resolve service for type 'IUrlHelperFactory'` at startup | `AddMvc()` missing from service registration | Add `builder.Services.AddMvc()` **before** `AddDevExpressBlazorReporting()`. [Detailed guidance →](troubleshooting.md#quick-diagnostic-checklist) |
| Save/Load/Open buttons do nothing | `ReportStorageWebExtension` registration order wrong | Register storage **after** `AddDevExpressBlazorReporting()`. [Common mistakes →](../troubleshooting.md#common-registration-order-mistakes) |
| Designer shows blank page | Missing render mode or resource scripts | Ensure `@rendermode InteractiveServer` on page; add `@DxResourceManager.RegisterScripts()` to `App.razor <head>`. [Details →](troubleshooting.md#quick-diagnostic-checklist) |
| 404 errors when saving reports | Middleware or routing order wrong | Add `app.MapControllers()`; ensure `UseDevExpressBlazorReporting()` is called **after** `UseRouting()`. [Common mistakes →](troubleshooting.md#common-registration-order-mistakes) |
| `NonTrustedTypeDeserializationException` during designer preview/save | Custom data source class not trusted | Register each class **before** `CreateBuilder()`: `DeserializationSettings.RegisterTrustedClass(typeof(YourClass))` and `.RegisterTrustedClass(typeof(YourClass[]))`. [Diagnostic steps →](troubleshooting.md#data-source-trust-registration-errors) |
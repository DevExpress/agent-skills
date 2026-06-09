# Troubleshooting — Blazor Reporting

**Prevent most issues by reviewing** 📄 [SKILL.md — Antipatterns to Avoid](../SKILL.md#antipatterns-to-avoid) **first.** This section covers diagnostic steps for issues that still occur.

## Quick Diagnostic Checklist

1. Does the component page have `@rendermode InteractiveServer` (or `InteractiveWebAssembly`)?
2. Is `AddDevExpressServerSideBlazorReportViewer()` (native) or `AddDevExpressBlazorReporting()` (JS-based) called in Program.cs?
3. Is `@DxResourceManager.RegisterScripts()` in `App.razor <head>`?
4. Is a theme registered (either via `DxResourceManager.RegisterTheme()` or per-page CSS links)?
5. Is `UseDevExpressBlazorReporting()` called after `UseRouting()` (JS-based only)?
6. For `DxReportDesigner`: is `ReportStorageWebExtension` registered **after** `AddDevExpressBlazorReporting()`?
7. For WASM: is `DevExpress.Drawing.Skia` installed? Is `WasmBuildNative=true` in the project file?
8. For `DxWasmDocumentViewer`/`DxWasmReportDesigner`: is the ASP.NET Core backend with MVC controllers running and accessible?

## Symptom → Fix Table

| Symptom | Most Likely Cause | Fix |
|---------|-------------------|-----|
| Viewer renders blank, no errors | `@rendermode` missing | Add `@rendermode InteractiveServer` to the page directive |
| `AddDevExpressServerSideBlazorReportViewer is not defined` | Wrong package | For native viewer, use `DevExpress.Blazor.Reporting.Viewer`; for JS-based, use `DevExpress.Blazor.Reporting.JSBasedControls` |
| No theme / viewer looks unstyled | Theme CSS missing | Add `@DxResourceManager.RegisterTheme(Themes.BlazingBerry)` + viewer CSS link in `App.razor <head>` |
| `System.DllNotFoundException` for Skia | `DevExpress.Drawing.Skia` not installed | `dotnet add package DevExpress.Drawing.Skia` (**ensure it matches your other DevExpress package versions** — run `dotnet list package` first); for WASM set `<WasmBuildNative>true</WasmBuildNative>` |
| WASM: only Object/JSON data sources work | By design — SQL unavailable in browser | Use JSON or Object data sources for WASM; use Server-mode for SQL |
| Designer Save/Load buttons do nothing | `ReportStorageWebExtension` not registered | Register after `AddDevExpressBlazorReporting()` (see **Antipattern A2**): `builder.Services.AddScoped<ReportStorageWebExtension, Custom...>()` |
| `FaultException` compile error in storage | WCF type used | Replace `FaultException` with `InvalidOperationException` in all storage throw statements (see **Antipattern A15**) |
| `Report not found: ReportName` | `IReportProvider.GetReport` returns null or throws | Implement `GetReport` to return the correct `XtraReport` instance for the given name |
| `UseDevExpressBlazorReporting` must be called after `UseRouting` | Middleware ordering wrong | Call in order: `UseRouting()` → `UseDevExpressBlazorReporting()` → `MapRazorComponents()` (see **Antipattern A4**) |
| `"Unable to resolve service for type 'IUrlHelperFactory'"` at startup | `AddMvc()` or `AddControllersWithViews()` missing from service registration | Add `builder.Services.AddMvc()` or `AddControllersWithViews()` **before** `AddDevExpressBlazorReporting()` — Interactive Server apps use `AddMvc()`; hosted WebAssembly backends use `AddControllersWithViews()`. JS-based components require MVC infrastructure (see **Antipattern A3**) |
| JS-based viewer: CORS errors from backend | Backend CORS not configured for Blazor origin | Add `UseCors()` to backend pipeline; order: `UseRouting()` → `UseCors()` → `UseEndpoints()` |
| Designer shows blank page | Interactive render mode not supported statically | Ensure Blazor app uses Interactive Server or WebAssembly rendering for the designer page |
| HTTP 404 on `DXXRDV` / `DXXRD` (WASM variant) | ASP.NET Core backend not running or wrong URL | Start backend; verify `GetDocumentViewerModelAction` URL in `DxWasmDocumentViewerRequestOptions` (see **Antipattern A5**) |
| Report parameters panel does not appear | Parameters are hidden on the report instance | Remove `report.Parameters["X"].Visible = false` or use the Parameters tab in the viewer |
| `XtraSerializationSecurityTrace+NonTrustedTypeDeserializationException: The <YourType> type is not in the list of trusted types` | Custom data source type not registered with DevExpress deserialization security | Add to Program.cs **before** `var builder = WebApplication.CreateBuilder(args)`: `DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(YourDataSourceClass));` per type (see **Antipatterns A7 & A8**) |
| Reports saved via designer don't appear in native viewer | Both `IReportProvider` and `ReportStorageWebExtension` registered | Remove `IReportProvider` registration; use `ReportStorageWebExtension` only (see **Antipattern A1** in `resolving-report-names.md`) |

## Data Source Trust Registration Errors (Designer & Viewer + Storage)

When Report Designer components or Viewer components loading reports from storage deserialize report XML, DevExpress validates custom types against the trusted-types list. If your custom data source classes are not trusted, you will encounter these errors:

### Error Symptoms and Fixes

| Error Message | When It Occurs | Cause | Fix |
|---|---|---|---|
| `XtraSerializationSecurityTrace+NonTrustedTypeDeserializationException: The SalesData type is not in the list of trusted types` | Designer save/load/preview; Viewer loading from storage | Custom data source class not registered | Add `DeserializationSettings.RegisterTrustedClass(typeof(SalesData));` before `CreateBuilder()` |
| `The SalesData[] type is not in the list of trusted types` | Designer save/load/preview; Viewer loading from storage | Array variant of data source not registered separately | Also add `DeserializationSettings.RegisterTrustedClass(typeof(SalesData[]))` |
| `Internal Server Error` during designer/viewer operations | Designer preview/save with custom data sources; Viewer opening report from storage | One or more custom data source types not trusted | Systematically register each data source class; check browser DevTools → Network tab for detailed error |

### Diagnostic Steps

1. **Identify all custom data sources** in your reports:
   - Search `.cs` report files for `ObjectDataSource`, `[DataSource]`, `List<T>`
   - List unique class names: `SalesData`, `EmployeeRecord`, etc.
   - Note: This issue occurs when designers OR viewers deserialize reports containing these classes

2. **Check Program.cs for registrations:**
   ```csharp
   // At the TOP of Program.cs, before CreateBuilder():
   DeserializationSettings.RegisterTrustedClass(typeof(SalesData));
   ```

3. **Verify placement:**
   - ✅ **Correct**: Registrations appear **before** `var builder = WebApplication.CreateBuilder(args)`
   - ❌ **Wrong**: Registrations appear after `CreateBuilder()` (too late; deserialization already initialized)

4. **Check for common mistakes:**
   - ❌ Registered the **report class** instead of the **data source class**
   - ❌ Used `RegisterTrustedAssembly()` (too permissive; use `RegisterTrustedClass()` per type)

### Anti-Pattern: Registering Report Class Instead of Data Source Class

**❌ WRONG:**
```csharp
// This is incorrect — the report class does not need registration
DeserializationSettings.RegisterTrustedClass(typeof(SalesReport));  // NOT NEEDED
```

**✅ CORRECT:**
```csharp
// Register the DATA SOURCE class, not the report class
DeserializationSettings.RegisterTrustedClass(typeof(SalesData));  // ← What the report USES
```

The report class (`SalesReport`) is instantiated in memory by your code; deserialization security only validates types **embedded in saved report XML**, which are the data source types.

## Enable Development Mode (JS-based viewer)

For `DxDocumentViewer` / `DxReportDesigner`, enable development mode on the ASP.NET Core services:

```csharp
builder.Services.ConfigureReportingServices(configurator => {
    if (builder.Environment.IsDevelopment())
        configurator.UseDevelopmentMode();
});
```

And in `appsettings.Development.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "DevExpress": "Debug"
    }
  }
}
```

## Native Viewer: Check for Render Mode Issues

If the viewer renders blank after page load without errors, check:

1. Open browser DevTools → Console — look for SignalR connection errors
2. Verify the page has `@rendermode InteractiveServer`
3. Verify `AddDevExpressServerSideBlazorReportViewer()` is registered
4. Check that `DxResourceManager.RegisterScripts()` is in `App.razor <head>`

## WASM: Verify Build

For WASM apps, confirm the AOT/native build works:

```bash
dotnet build -c Release
# Should complete without errors
# Watch for: "error : DevExpress.Drawing.Skia is required"
```

## Common Registration Order Mistakes

**Native viewer (wrong order):**
```csharp
// WRONG: nothing registered for SignalR viewer
builder.Services.AddRazorComponents()...
// Missing: builder.Services.AddDevExpressServerSideBlazorReportViewer()
```

**JS-based designer (wrong order):**
```csharp
// WRONG: storage registered before reporting services
builder.Services.AddScoped<ReportStorageWebExtension, MyStorage>(); // ← too early
builder.Services.AddDevExpressBlazorReporting();
```

**Correct order:**
```csharp
builder.Services.AddMvc();
builder.Services.AddDevExpressBlazorReporting();
builder.Services.AddScoped<ReportStorageWebExtension, MyStorage>(); // ← after
```

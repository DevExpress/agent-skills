# Troubleshooting — Blazor Reporting

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
| `System.DllNotFoundException` for Skia | `DevExpress.Drawing.Skia` not installed | `dotnet add package DevExpress.Drawing.Skia`; for WASM set `<WasmBuildNative>true</WasmBuildNative>` |
| WASM: only Object/JSON data sources work | By design — SQL unavailable in browser | Use JSON or Object data sources for WASM; use Server-mode for SQL |
| Designer Save/Load buttons do nothing | `ReportStorageWebExtension` not registered | Register after `AddDevExpressBlazorReporting()`: `builder.Services.AddScoped<ReportStorageWebExtension, Custom...>()` |
| `FaultException` compile error in storage | WCF type used | Replace `FaultException` with `InvalidOperationException` in all storage throw statements |
| `Report not found: ReportName` | `IReportProvider.GetReport` returns null or throws | Implement `GetReport` to return the correct `XtraReport` instance for the given name |
| `UseDevExpressBlazorReporting` must be called after `UseRouting` | Middleware ordering wrong | Call in order: `UseRouting()` → `UseDevExpressBlazorReporting()` → `MapRazorComponents()` |
| JS-based viewer: CORS errors from backend | Backend CORS not configured for Blazor origin | Add `UseCors()` to backend pipeline; order: `UseRouting()` → `UseCors()` → `UseEndpoints()` |
| Designer shows blank page | Interactive render mode not supported statically | Ensure Blazor app uses Interactive Server or WebAssembly rendering for the designer page |
| HTTP 404 on `DXXRDV` / `DXXRD` (WASM variant) | ASP.NET Core backend not running or wrong URL | Start backend; verify `GetDocumentViewerModelAction` URL in `DxWasmDocumentViewerRequestOptions` |
| Report parameters panel does not appear | Parameters are hidden on the report instance | Remove `report.Parameters["X"].Visible = false` or use the Parameters tab in the viewer |

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

# Troubleshooting — ASP.NET Core Reporting

## Quick Diagnostic Checklist

Run through these before deep-diving into specific symptoms:

1. Is `app.UseDevExpressControls()` **before** `app.UseStaticFiles()`?
2. Is `thirdparty.bundle.js` in the `<head>` of `_Layout.cshtml`, not in `@section Scripts {}`?
3. Did `npm install` run before `dotnet build`?
4. Do npm and NuGet package versions match (both `26.1`)?
5. Is `using DevExpress.AspNetCore.Reporting;` present where `ConfigureReportingServices` is called?
6. Is `using DevExpress.AspNetCore;` present where `AddDevExpressControls` is called?
7. On Linux/macOS: is `DevExpress.Drawing.Skia` NuGet package installed?
8. Is `configurator.UseDevelopmentMode()` enabled to see detailed errors?

## Symptom → Fix Table

| Symptom | Most Likely Cause | Fix |
|---------|-------------------|-----|
| Viewer renders blank, no browser errors | `UseDevExpressControls()` missing or after `UseStaticFiles()` | Move `app.UseDevExpressControls()` before `app.UseStaticFiles()` |
| `"Unable to process binding"` in viewer popup | Script load order wrong (Bootstrap missing, or scripts in `@section Scripts {}`) | Move `thirdparty.bundle.js` to `<head>` in layout; include Bootstrap in thirdparty bundle |
| Literal text `.Height("...")` visible on page | Multi-line Razor helper not wrapped in `@(...)` | Wrap: `@(Html.DevExpress().ReportDesigner(...).Height(...).Bind(...))` |
| `Component dx-filtereditor-plain is already registered` | Viewer bundle + designer bundle both loaded on designer page | Remove `viewer.part.bundle.js` from designer page; designer bundle is self-contained |
| `DevExpress is not defined` in browser console | `dx.all.js` missing from bundle or loaded after reporting scripts | Ensure `dx.all.js` is first entry in viewer/designer part bundle |
| `DevExpress.Analytics.Widgets is undefined` | Analytics core not included before query builder/designer | Ensure `dx-analytics-core.min.js` comes before `dx-querybuilder.min.js` in bundle |
| HTTP 400 on `DXXRDV` or `DXXRD` requests | Anti-forgery token rejected by global filter | Add `[IgnoreAntiforgeryToken]` to all reporting controllers |
| HTTP 404 on `DXXRDV` or `DXXRD` | Controller not found or routing missing | Ensure controllers exist; in Razor Pages, add `endpoints.MapDefaultControllerRoute()` |
| HTTP 500 with "Internal Server Error" popup | Server exception; error details hidden | Enable `configurator.UseDevelopmentMode()` and set `"DevExpress": "Debug"` in appsettings |
| `System.DllNotFoundException` for Skia at runtime | `DevExpress.Drawing.Skia` not installed on Linux/macOS | `dotnet add package DevExpress.Drawing.Skia` |
| `"Report not found: ReportName"` | `GetData("ReportName")` in storage returns null or throws | Implement `GetData` to resolve the name; for quickstart, use `Bind(new MyReport())` |
| Build error: `LIB002` (LibMan cannot resolve path) | npm install not run; `node_modules` missing | Run `npm install` before `dotnet build` |
| `ConfigureReportingServices` not recognized | Missing `using DevExpress.AspNetCore.Reporting;` | Add the using directive to Program.cs |
| `FaultException` compile error in storage class | WCF type used; not available in ASP.NET Core | Replace `FaultException` with `InvalidOperationException` |
| Viewer version mismatch warning | npm and NuGet versions differ | Align all packages to same major.minor (e.g., `26.1`) |
| CORS errors from SPA | `UseCors()` in wrong pipeline position | Order: `UseRouting()` → `UseCors()` → `UseEndpoints()` |
| `ko is not defined` in browser console | Knockout not in bundle, or loaded after `dx.all.js` | Add `knockout-latest.js` to `thirdparty.bundle.js` before Bootstrap. `thirdparty.bundle.js` loads in `<head>` so Knockout is available before `viewer.part.bundle.js` (which contains `dx.all.js`) |
| `DevExtreme library is included before Knockout` console warning | `dx.all.js` loads before Knockout initializes | Knockout must be in `thirdparty.bundle.js` (layout head). `dx.all.js` must stay in `viewer.part.bundle.js` (per-page). Required load order: jQuery → Knockout → Bootstrap → dx.all.js → dx-analytics-core → dx-webdocumentviewer |
| `"DXMargins type not found"` compile error | Incorrect type used for report margins | Do not set margins via `DXMargins` in this scenario; use the Report Designer UI or omit |
| Report designer: Save/Load buttons do nothing | `ReportStorageWebExtension` not registered | Register after `AddDevExpressControls()`: `builder.Services.AddScoped<ReportStorageWebExtension, CustomStorage>()` |
| SignalR disconnect / document lost on server restart | No shared document storage on web farm | Configure `UseFileDocumentStorage` with shared path and `StorageSynchronizationMode.InterProcess` |
| `"Report not found"` after storage timeout | Document cache expired before export completed | Increase `StorageCleanerSettings` interval or `CacheCleanerSettings` expiration |

## Enable Development Mode

```csharp
builder.Services.ConfigureReportingServices(configurator => {
    if (builder.Environment.IsDevelopment())
        configurator.UseDevelopmentMode();
});
```

Development mode:
- Shows full server exception stack traces in the viewer error dialog
- Validates npm ↔ NuGet version consistency and displays mismatches
- Enables verbose logging for reporting components

## Enable Debug Logging

In `appsettings.Development.json`:

```json
{
  "Logging": {
    "LogLevel": {
      "DevExpress": "Debug"
    }
  }
}
```

## Network Tab Diagnostics

When the viewer shows an error popup or stays blank:

1. Open browser DevTools → Network tab
2. Filter for `DXXRDV` requests
3. Check the response:
   - HTTP 400 → anti-forgery token issue (`[IgnoreAntiforgeryToken]`)
   - HTTP 401/403 → authorization issue (`[Authorize]` without proper auth)
   - HTTP 404 → controller not found or routing gap
   - HTTP 500 → server exception (enable development mode for details)
4. Filter for `.bundle.js` files — confirm all loaded with HTTP 200
5. Check Console tab for JavaScript errors:
   - `DevExpress is not defined` → script load order issue
   - `Unable to process binding` → Knockout binding failed; thirdparty bundle not in head

## Running and Checking Endpoints

```bash
# Run with explicit project path (avoids "no project found" error in multi-folder workspaces)
dotnet run --project ./src/WebApp/WebApp.csproj

# Check home page
curl -s -o /dev/null -w "%{http_code}" http://localhost:5000/

# Check reporting API (should return JSON, not redirect)
curl -s -o /dev/null -w "%{http_code}" http://localhost:5000/DXXRDV/GetDocumentData

# If curl is unavailable, use grep/findstr to check response content:
# Windows:  curl http://localhost:5000/DXXRDV/GetDocumentData 2>&1 | findstr "200\|error"
# Unix:     curl http://localhost:5000/DXXRDV/GetDocumentData 2>&1 | grep -i "200\|error"
```

## Post-Build Browser Verification

`dotnet build` succeeding does not confirm JS runtime health — script order errors are runtime-only. After any change to bundles, Program.cs, or view files, always verify in the browser:

1. Run the app: `dotnet run --project ./YourApp/YourApp.csproj`
2. Open the viewer page in a browser
3. Open **DevTools → Console** — confirm **0 JS errors**. Watch for:
   - `ko is not defined` → Knockout missing from bundle or loaded after `dx.all.js`
   - `DevExtreme library is included before Knockout` → reverse Knockout and `dx.all.js` load order
   - `DevExpress is not defined` → `dx.all.js` not loaded or wrong bundle on the page
   - `Unable to process binding` → `thirdparty.bundle.js` not in layout `<head>`
4. Open **DevTools → Network** — filter for `.bundle.js`. Confirm all loaded with **HTTP 200**, especially:
   - `thirdparty.bundle.js` (must include `knockout-latest.js`)
   - `viewer.part.bundle.js` or `designer.part.bundle.js`
5. Do a **hard refresh** (Ctrl+Shift+R / Cmd+Shift+R) to bypass cache and retest
6. Confirm the viewer renders the report without a blank or error state

## Production Deployment Checklist

- [ ] `UseDevelopmentMode()` disabled (or guarded by environment check)
- [ ] All npm and NuGet packages at same version
- [ ] `DevExpress.Drawing.Skia` installed for Linux/macOS hosts
- [ ] `wwwroot/css/icons/` contains font files (`dxicons.ttf`, `.woff2`, `.woff`)
- [ ] `app.UseDevExpressControls()` before `UseStaticFiles()` in pipeline
- [ ] Anti-forgery exemption on reporting controllers if global validation is enabled
- [ ] `UseCachedReportSourceBuilder()` enabled for production
- [ ] For web farms: shared file storage configured with `StorageSynchronizationMode.InterProcess`
- [ ] Report Designer machine key (validationKey/decryptionKey) consistent across servers

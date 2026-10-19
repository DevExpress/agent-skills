# Troubleshooting — ASP.NET Core Reporting

**This file is the only symptom→fix lookup in this skill.** `SKILL.md` carries no troubleshooting table — it points here. Load this file in full at the first sign of trouble; do not diagnose from memory.

**It diagnoses and fixes; it never decides whether a page is valid.** Validation is defined once, in [../SKILL.md](../SKILL.md) **Phase 4**. Come here after that script reports a FAIL, or when a symptom appears in the browser.

## Quick Diagnostic Checklist

Run through these before deep-diving into specific symptoms:

1. Is `app.UseDevExpressControls()` **before** `app.UseStaticFiles()`/`app.MapStaticAssets()` (whichever the project uses)?
2. Are the base scripts (jQuery, Knockout, Bootstrap, `dx.all.js`) loaded, in order, **before** the reporting helper renders — as `<script>` tags in `_Layout.cshtml` `<head>` and above the helper on the page (Pattern A — Direct References), or in `thirdparty.bundle.js` in the `<head>` (Pattern B — Bundled)?
3. Did `npm install` run before `dotnet build`?
4. Do npm and NuGet package versions match (both `26.1`)?
5. Is `using DevExpress.AspNetCore.Reporting;` present where `ConfigureReportingServices` is called?
6. Is `using DevExpress.AspNetCore;` present where `AddDevExpressControls` is called?
7. On Linux/macOS: is `DevExpress.Drawing.Skia` NuGet package installed?
8. Is there a `WebDocumentViewerController` descendant? Required at startup in **every** reporting app, Viewer page or not.
9. If the page reads data: is the connection string's driver package referenced? ([Supported Data Providers](https://docs.devexpress.com/CoreLibraries/403638/devexpress-data-library/data-sources/use-the-sql-data-source/supported-data-providers))
10. Is `configurator.UseDevelopmentMode()` enabled to see detailed errors?

## Symptom → Fix Table

| Symptom | Most Likely Cause | Fix |
|---------|-------------------|-----|
| Viewer renders blank, no browser errors | `UseDevExpressControls()` missing or after `UseStaticFiles()`/`MapStaticAssets()` | Move `app.UseDevExpressControls()` before whichever static-file call the project uses |
| `"Unable to process binding"` in viewer popup | Script load order wrong — base scripts not loaded before the reporting helper renders | Load jQuery/Knockout/Bootstrap/`dx.all.js` before the helper — as `<script>` tags in `<head>`/above the helper (Pattern A), or `thirdparty.bundle.js` in `<head>` (Pattern B) |
| Literal text `.Height("...")` visible on page | Multi-line Razor helper not wrapped in `@(...)` | Wrap: `@(Html.DevExpress().ReportDesigner(...).Height(...).Bind(...))` |
| `Component dx-filtereditor-plain is already registered` | Viewer and designer client assets both loaded on the designer page | Load the designer's asset list once; don't also load the viewer's — remove the duplicate viewer tags (Pattern A) or `viewer.part.bundle.js` (Pattern B) |
| Designer page crashes with `DxReportDesigner undefined` | A required script is missing from the designer page — its tag list is a **superset** of the viewer's, so a viewer-only list leaves the designer scripts absent | Carry the full designer list, loaded once and in order: `dx.all.js` → `dx-analytics-core` → `dx-webdocumentviewer` → `dx-querybuilder` → `dx-reportdesigner` ([getting-started.md](getting-started.md) Step 9). The same message with the assets *doubled* is the row above |
| Designer's **Expression Editor / Filter Editor** does not open. Console shows `Cannot read properties of undefined (reading 'setCompleters')` plus several `Unable to process binding` errors | An ace-builds file is missing — usually `ext-language_tools.js`. Loading `ace.js` alone is the broken middle state | Load **all 5** ace scripts and **all 3** ace stylesheets ([getting-started.md](getting-started.md) Step 9), or none at all. If all 8 are present, check `ace.js` precedes `dx.all.js` ([../SKILL.md](../SKILL.md) Constraint 2); under Pattern B keep ace in `thirdparty.bundle.js`, not `designer.part.bundle.js` |
| `DevExpress is not defined` in browser console | `dx.all.js` missing or loaded after the reporting scripts | Ensure `dx.all.js` loads before analytics-core/webdocumentviewer — as a `<script>` tag in load order (Pattern A) or the first entry in the viewer/designer part bundle (Pattern B) |
| `DevExpress.Analytics.Widgets is undefined` | Analytics core not loaded before the designer | Ensure `dx-analytics-core.min.js` comes before `dx-querybuilder.min.js` — in tag order (Pattern A) or in the bundle (Pattern B) |
| HTTP 400 on `DXXRDV` or `DXXRD` requests | Anti-forgery token rejected by global filter | Add `[IgnoreAntiforgeryToken]` to all reporting controllers |
| HTTP 404 on `DXXRDV` or `DXXRD` | Controller not found or routing missing | Ensure controllers exist; in Razor Pages, add `app.MapDefaultControllerRoute()` |
| HTTP 500 with "Internal Server Error" popup | Server exception; error details hidden | Enable `configurator.UseDevelopmentMode()` and set `"DevExpress": "Debug"` in appsettings |
| App **exits at startup**: `ReportingConfigurationException: Cannot find a custom WebDocumentViewerController class descendant` | No `WebDocumentViewerController` descendant. The check is unconditional — whatever component the pages host | Add one, even in a Parameters-Panel-only app. Don't add `ReportDesignerController` unless a Designer page exists — not enforced, dead code |
| Component renders, console clean, verification passes — but every data call returns HTTP **200** with `{"error":"Unable to open a database."}` | The connection string's data-provider assembly is missing. DevExpress loads drivers reflectively and ships none; the real `TypeLoadException: None of the following assemblies found: …` is swallowed | Reference the driver package — per-database list: [Supported Data Providers](https://docs.devexpress.com/CoreLibraries/403638/devexpress-data-library/data-sources/use-the-sql-data-source/supported-data-providers) (see also [data-sources.md](data-sources.md)). `UseDevelopmentMode()` surfaces the real exception |
| `System.DllNotFoundException` for Skia at runtime | `DevExpress.Drawing.Skia` (or `DevExpress.Pdf.SkiaRenderer` for PDF content) not installed on Linux/macOS | `dotnet add package DevExpress.Drawing.Skia` — see the [DevExpress.Drawing troubleshooting guide](https://docs.devexpress.com/CoreLibraries/404254/devexpress-drawing-library/troubleshooting) if the exception persists after installing |
| `System.TypeInitializationException` on Linux/Docker | Missing native font libraries | Install `libc6 libicu-dev libfontconfig1` (Debian/Ubuntu) or `glibc-devel libicu fontconfig` (RHEL/CentOS) |
| On Windows Azure, rendering fails or falls back unexpectedly | Azure has limited GDI API support | Set [PdfPrintingOptions.RenderingEngine](https://docs.devexpress.com/CoreLibraries/DevExpress.XtraPrinting.PdfPrintingOptions.RenderingEngine) to `Skia` explicitly |
| `"Report not found: ReportName"` | Binding by name with nothing resolving it — `GetData("ReportName")` returns null or throws | **Viewer**: `Bind(new MyReport())` is a valid quickstart. **Designer**: implement `ReportStorageWebExtension.GetData("ReportName")` — instance binding leaves Save As/Open dead and is not a valid end state |
| Designer saves a report and it appears in the Open dialog, but reopening shows none of the changes | `IReportProvider` and `ReportStorageWebExtension` both registered — `IReportProvider` takes priority for loading, `ReportStorageWebExtension` is only called for saving | Keep `ReportStorageWebExtension` and remove `IReportProvider` — see [report-storage.md](report-storage.md) |
| `NonTrustedTypeDeserializationException` | Custom data source class not registered as trusted before report deserialization | Add `DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(T))` before `WebApplication.CreateBuilder(args)`, where `T` is the **exact type named in the exception message** — see [report-storage.md](report-storage.md#data-source-trust-registration--custom-data-sources) |
| Build error: `LIB002` (LibMan cannot resolve path), or `libman.json` file not found | npm install not run; `node_modules` missing | Run `npm install` before `dotnet build` |
| Viewer renders and works, but every toolbar icon is an empty box; `wwwroot/css/icons/` missing | `Microsoft.Web.LibraryManager.Build` not installed, so `libman.json` was never processed (build log shows no LibMan step) | `dotnet add package Microsoft.Web.LibraryManager.Build`, rebuild, confirm the icon font files exist at the `destination` your `libman.json` declares. Required under **both** Pattern A and Pattern B — `BuildBundlerMinifier` does not replace it |
| Viewer renders unstyled, or CSS 404s despite a clean build | A `.css` path was placed in a `.js` output bundle, or an analytics CSS input was omitted | Re-transcribe `bundleconfig.json` from [client-resource-integration.md](client-resource-integration.md) — CSS and JS outputs are separate entries and neither input list is optional |
| HTTP 404 for a `~/js/…`/`~/css/…` file referenced by a direct `<script>`/`<link>` tag (Pattern A — Direct References, the default) | `libman.json` `destination` doesn't match the tag path, or `npm install` didn't run before build | See [client-resource-integration.md](client-resource-integration.md) troubleshooting section; align `libman.json` `destination` with the tag's `src`/`href` exactly |
| `ConfigureReportingServices` not recognized | Missing `using DevExpress.AspNetCore.Reporting;` | Add the using directive to Program.cs |
| `FaultException` compile error in storage class | Used `System.ServiceModel.FaultException` (WCF type) instead of the DevExpress one | Throw `DevExpress.XtraReports.Web.ClientControls.FaultException` instead — it works in ASP.NET Core and its `Message` is shown to the end user |
| Viewer version mismatch warning | npm and NuGet versions differ | Align all packages to same major.minor (e.g., `26.1`) |
| CORS errors from SPA | `UseCors()` in wrong pipeline position | Order: `app.UseRouting()` → `app.UseCors()` → the endpoint mapping calls (`app.MapControllerRoute(...)`/`app.MapRazorPages()`) |
| `ko is not defined` in browser console | Knockout missing, or loaded after `dx.all.js` | Place the `knockout-latest.js` `<script>` tag before `dx.all.js` on the reporting page (Pattern A), or put `knockout-latest.js` before `dx.all.js` in the bundle (Pattern B). Only the relative order matters — see the dependency list in [../SKILL.md](../SKILL.md) Constraint 2 (load order) |
| `DevExtreme library is included before Knockout` console warning | `dx.all.js` loads before Knockout initializes | Move the Knockout tag ahead of `dx.all.js` on the reporting page (Pattern A), or into an earlier bundle so it loads before `dx.all.js` (Pattern B). Bootstrap's position relative to Knockout is irrelevant — see [../SKILL.md](../SKILL.md) Constraint 2 (load order) |
| Viewer works, but the app's own navbar/dropdowns/modals break | A second, older Bootstrap (npm 4.x) loads next to the template's 5.x, and/or its stylesheet was replaced | Drop Bootstrap and jQuery from the reporting registration; keep the versions the app's markup was written against. `scripts/Verify-ReportingPage.ps1` reports duplicate loads |
| A duplicate or order violation isn't visible in the `<script>` tag list | A bundle concatenates the library, so the defect is *inside* one file | This is what Phase 4's `Verify-ReportingPage.ps1` run already covers — it identifies libraries by file content, so it sees inside bundles |
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

## Deep-Dive on a Live Page

Use this only to localize a defect Phase 4 already reported, or to inspect behavior the script cannot see (it does not execute JavaScript). **Do not start the app yourself** — run Phase 4 step 3 with `-KeepAlive`, which leaves the server running and prints its URL and PID, work against that URL, and stop the printed PID when you are done. Nothing you observe here upgrades a FAIL or a missing script run into a pass.

### Browser inspection

With an MCP that can open a URL and read the console:

1. **Console** — confirm **0 JS errors**. Watch for:
   - `ko is not defined` → Knockout missing or loaded after `dx.all.js`
   - `DevExtreme library is included before Knockout` → reverse Knockout and `dx.all.js` load order
   - `DevExpress is not defined` → `dx.all.js` not loaded or wrong assets on the page
   - `Unable to process binding` → base scripts not loaded before the reporting helper rendered
2. **Network** — confirm each DevExpress `.js`/`.css` the page references returns **HTTP 200**, especially `knockout-latest.js`, `dx.all.js`, `dx-analytics-core.min.js`, and `dx-webdocumentviewer.min.js` (the direct-reference files under Pattern A, or the `.bundle.js` files under Pattern B). Also check the **icon fonts** (`dxicons.woff2`/`.woff`/`.ttf`) — those are requested by `dx.light.css`, not by the page, so they are easy to miss; a 404 there renders every toolbar icon as a tofu box
3. Do a **hard refresh** (Ctrl+Shift+R / Cmd+Shift+R) to bypass cache and retest
4. When the viewer shows an error popup or stays blank, filter the Network tab for `DXXRDV` requests and read their status:
   - HTTP 400 → anti-forgery token issue (`[IgnoreAntiforgeryToken]`)
   - HTTP 401/403 → authorization issue (`[Authorize]` without proper auth)
   - HTTP 404 → controller not found or routing gap
   - HTTP 500 → server exception (enable development mode for details)

### Reading raw responses

When a script message or console error does not localize the failure, request the endpoint directly against the `-KeepAlive` server (substitute the URL the script printed):

```bash
# Reporting API — should return JSON, not a redirect
curl -s -o /dev/null -w "%{http_code}" http://localhost:5000/DXXRDV/GetDocumentData
```

Read the status code to pick a row in the symptom table above. **A response body never establishes that the page is healthy** — a substring match such as `-match 'id="DocumentViewer"'` passes on a page that also renders literal `.Bind(...)` text, loads jQuery twice, or 404s every icon font. Only the verification script's exit code and `VERIFIED:` stamp establish that.

## Production Deployment Checklist

- [ ] `UseDevelopmentMode()` disabled (or guarded by environment check)
- [ ] All npm and NuGet packages at same version
- [ ] `DevExpress.Drawing.Skia` installed for Linux/macOS hosts
- [ ] `wwwroot/css/icons/` contains font files (`dxicons.ttf`, `.woff2`, `.woff`)
- [ ] `app.UseDevExpressControls()` before `UseStaticFiles()`/`MapStaticAssets()` in pipeline
- [ ] Anti-forgery exemption on reporting controllers if global validation is enabled
- [ ] `UseCachedReportSourceBuilder()` enabled for production
- [ ] For web farms: shared file storage configured with `StorageSynchronizationMode.InterProcess`
- [ ] Report Designer machine key (validationKey/decryptionKey) consistent across servers

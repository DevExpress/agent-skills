# Getting Started — JS-Based Report Designer: Blazor WebAssembly Standalone App

## When to Use This Reference

Use this when you are building a **Blazor WebAssembly Standalone App** (no server-side project) and need to let end users design reports entirely in the browser.

- Single-project deployment (pure client-side)
- All report processing (rendering, preview) happens in the browser via the Skia graphics engine
- No SignalR, no ASP.NET Core backend required for the designer itself
- **Data source support: JSON and Object only** (SQL and MongoDB require a backend)
- Component: `DxReportDesigner`
- Report saving requires a separate strategy (custom web API or cloud storage)

> **Not your scenario?**
> — Interactive Server (single project, server renders) → 📄 [getting-started-js-designer-server.md](getting-started-js-designer-server.md)
> — Interactive WebAssembly (Client + Server) → 📄 [getting-started-js-designer-interactive-wasm.md](getting-started-js-designer-interactive-wasm.md)

## Setup Workflow

Follow these steps to integrate `DxReportDesigner` into your **Standalone Blazor WebAssembly** app (no backend required):

1. **[Install NuGet Packages](#install-nuget-packages-and-dependencies)** — Add `JSBasedControls`, Skia packages, and the `<NativeFileReference>` to the `.csproj`
2. **[Register namespaces](#register-namespaces-in-_importsrazor)** — Update `_Imports.razor`
3. **[Register client resources](#register-client-resources-in-apprazor)** — Add `DxResourceManager.RegisterScripts()` to `App.razor <HeadContent>`
4. **[Implement Report Name Resolution Service](#implement-a-report-name-resolution-service)** — Choose between read-only (`IReportProviderAsync`) and full Save/Load/Open (`ReportStorageWebExtension`)
5. **[Configure Program.cs](#register-services-in-programcs)** — Call `AddDevExpressBlazorReportingWebAssembly()` and register your chosen service
6. **[Add designer to a page](#add-a-report-designer-to-a-page)** — Place `<DxReportDesigner>`
7. **[Load Fonts](#load-fonts-required-for-skia)** — Load fonts into `DXFontRepository` before the designer renders (required for Skia)
8. **[Customize the Designer (optional)](#customize-the-designer-ui-and-behavior-optional)** — Toolbar commands, wizard flow, UI panels, event callbacks

## Install NuGet Packages and Dependencies

```bash
dotnet add package DevExpress.Drawing.Skia
dotnet add package DevExpress.Blazor.Reporting.JSBasedControls
dotnet add package SkiaSharp.Views.Blazor
dotnet add package SkiaSharp.NativeAssets.WebAssembly
dotnet add package HarfBuzzSharp.NativeAssets.WebAssembly
```

> **Version consistency**: If your project already references other DevExpress packages, ensure `DevExpress.Drawing.Skia` and `DevExpress.Blazor.Reporting.JSBasedControls` use the **same version** as existing DevExpress packages.

Also add the native dependency to the `.csproj` file:

```xml
<ItemGroup>
    <NativeFileReference Include="$(HarfBuzzSharpStaticLibraryPath)\2.0.23\*.a" />
</ItemGroup>
```

## Register namespaces in _Imports.razor

```razor
@using DevExpress.Blazor.Reporting
```

## Register client resources in App.razor

```razor
<HeadContent>
    @DxResourceManager.RegisterScripts()
</HeadContent>
```

## Implement a Report Name Resolution Service

For detailed instructions on implementing `IReportProviderAsync` or `ReportStorageWebExtension` in a Standalone WebAssembly app, see 📄 [resolving-report-names.md — Client-Side WebAssembly Scenarios](resolving-report-names.md#client-side-webassembly-scenarios).

The reference covers:
- **Option 1: `IReportProviderAsync`** — Read-only reports (view and edit, no save) with code samples and Program.cs registration
- **Option 2: `ReportStorageWebExtension`** — Full designer persistence (Save, Save As, Open) via backend API with async methods
- Backend API endpoint requirements
- Critical constraints specific to Standalone WASM
- Common antipatterns and their fixes

## Add a Report Designer to a Page

No `@rendermode` attribute is needed — in Interactive WebAssembly Standalone everything is interactive by default.

```razor
@page "/reportdesigner"

<DxReportDesigner ReportName="TestReport">
</DxReportDesigner>
```

## Load Fonts (Required for Skia)

Skia requires fonts to be loaded into `DXFontRepository` before the designer renders. Without this step, report previews will show blank or incorrectly rendered text.

Place font files (e.g., `opensans.ttf`) in `wwwroot/fonts/`, then load them during app startup:

```csharp
// Services/FontLoader.cs
using DevExpress.Drawing;

public static class FontLoader {
    public async static Task LoadFonts(HttpClient httpClient, List<string> fontNames) {
        foreach(var fontName in fontNames) {
            var fontBytes = await httpClient.GetByteArrayAsync($"fonts/{fontName}");
            DXFontRepository.Instance.AddFont(fontBytes);
        }
    }
}
```

Call `LoadFonts` from `MainLayout.razor` `OnInitializedAsync` — before the designer renders:

```razor
@code {
    [Inject] HttpClient Http { get; set; }
    List<string> RequiredFonts = new() {
        "opensans.ttf"
    };

    protected async override Task OnInitializedAsync() {
        await FontLoader.LoadFonts(Http, RequiredFonts);
        await base.OnInitializedAsync();
    }
}
```

## Data Source Limitations

| Data Source Type | Supported in Interactive WebAssembly Standalone |
|-----------------|------------------------------|
| JSON | ✅ Yes |
| Object (custom class) | ✅ Yes |
| SQL (`SqlDataSource`) | ❌ No — requires a backend |
| MongoDB | ❌ No — requires a backend |

For full SQL/MongoDB support, use [getting-started-js-designer-interactive-wasm.md](getting-started-js-designer-interactive-wasm.md) or [getting-started-js-designer-server.md](getting-started-js-designer-server.md).

## Register Trusted Classes for Data Source Deserialization

**Critical step for designer-based projects:** When a Report Designer deserializes saved reports (on preview or during save operations), it validates the data source types embedded in the report XML against a trusted-types list. If your reports use **custom data source classes** (not built-in Object/JSON types), they must be registered **before** `WebAssemblyHostBuilder.CreateDefault()` is called.

> **This applies ONLY to custom business classes**, not to built-in DevExpress types or .NET collections.

### Quick Checklist

1. **Search your report files** for `[DataSource]` attributes or `List<T>` properties
2. **Extract class names** — e.g., `SalesData`, `ProductRecord`, `CustomerDto`
3. **For each class, register both the type and its array variant** at the top of `Program.cs`:
   ```csharp
   DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(YourClass));
   DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(YourClass[]));
   ```

### Example: Registering Custom Data Source Classes

If your reports use `SalesData` and `ProductRecord` as data sources:

**Program.cs (top of file, before `WebAssemblyHostBuilder.CreateDefault()`):**
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

var builder = WebAssemblyHostBuilder.CreateDefault(args);
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

❌ **Placing registration after `CreateDefault()`**
- **Symptom**: `NonTrustedTypeDeserializationException` at runtime
- **Fix**: Move all registrations to the top of `Program.cs`, **before** `WebAssemblyHostBuilder.CreateDefault(args)` is called

## Customize the Designer UI and Behavior (Optional)

For toolbar commands, wizard flow, UI panels, and event callbacks, see 📄 [customizing-js-designer.md](customizing-js-designer.md).

## Antipatterns to Avoid

### Installing server-side packages in Interactive WebAssembly Standalone

- **Antipattern**: Installing `DevExpress.AspNetCore.Reporting` in an Interactive WebAssembly Standalone project
- **Why**: There is no server-side project in this architecture; that package includes ASP.NET Core dependencies that do not belong in a client-only project. It adds unnecessary bloat and can cause build conflicts
- **Correct Pattern**:
  - Use only: `DevExpress.Blazor.Reporting.JSBasedControls`, `DevExpress.Drawing.Skia`, `SkiaSharp.*`, `HarfBuzzSharp.*`
  - Do NOT install: `DevExpress.AspNetCore.Reporting`

### Wrong NuGet package variant for Interactive WebAssembly Standalone

- **Antipattern**: Installing `DevExpress.Blazor.Reporting.JSBasedControls.WebAssembly` in an Interactive WebAssembly Standalone project
- **Why**: That package is for [Interactive WebAssembly (Client + Server)](getting-started-js-designer-interactive-wasm.md), not standalone. It includes server communication logic that creates overhead in a pure client-side app
- **Correct Pattern**: Use `DevExpress.Blazor.Reporting.JSBasedControls` (without `.WebAssembly` suffix)

### Calling server-side registration method in Interactive WebAssembly Standalone Program.cs

- **Antipattern**: Calling `builder.Services.AddDevExpressBlazorReporting()` in an Interactive WebAssembly Standalone project
- **Why**: That is the Interactive Server registration method (single-project backend); calling it in Interactive WebAssembly Standalone causes runtime initialization errors
- **Correct Pattern**:
  ```csharp
  // Interactive WebAssembly Standalone — use this method:
  builder.Services.AddDevExpressBlazorReportingWebAssembly(configure => {
      configure.UseDevelopmentMode();  // Remove in production
  });
  ```

### Adding MVC infrastructure to Interactive WebAssembly Standalone

- **Antipattern**: Calling `builder.Services.AddMvc()` in an Interactive WebAssembly Standalone project Program.cs
- **Why**: There is no MVC pipeline in an Interactive WebAssembly Standalone app; this adds unused dependencies and can cause service resolution conflicts
- **Correct Pattern**: Remove `AddMvc()` completely; it must not appear in Interactive WebAssembly Standalone `Program.cs`

### Using synchronous blocking in async context

- **Antipattern**: Using `.Wait()`, `.Result`, or `.GetAwaiter().GetResult()` in `ReportStorageWebExtension` or `IReportProviderAsync` methods
- **Why**: Interactive WebAssembly Standalone runs on a single thread; blocking on async operations deadlocks the thread, hanging the UI. All methods must be fully async
- **Correct Pattern**:
  ```csharp
  var data = await httpClient.GetByteArrayAsync(url);
  ```
  All `ReportStorageWebExtension` methods must be `async` and use `await`

## Troubleshooting

**For generic issues** (render mode, service registration, report names, data source trust, middleware order), see [troubleshooting.md](troubleshooting.md). Issues specific to Interactive WebAssembly Standalone:

| Symptom | Cause | Fix |
|---------|-------|-----|
| `DllNotFoundException` for Skia at startup | Missing native Skia assets or `<NativeFileReference>` absent | Verify `SkiaSharp.NativeAssets.WebAssembly`, `HarfBuzzSharp.NativeAssets.WebAssembly` are installed; add `<NativeFileReference>` to `.csproj` |
| Fonts not rendering / blank report preview | Fonts not loaded into `DXFontRepository` | Call `FontLoader.LoadFonts(...)` in `MainLayout.razor` `OnInitializedAsync` before the designer renders |
| Designer shows blank page | Missing render mode or scripts in backend `App.razor` | Ensure backend `App.razor <head>` has `@DxResourceManager.RegisterScripts()`. [Details →](troubleshooting.md#quick-diagnostic-checklist) |
| `IReportProviderAsync` or `ReportStorageWebExtension` not registered | Service registration order or missing registration | Register **after** `AddDevExpressBlazorReportingWebAssembly()` in Program.cs. [Registration order →](troubleshooting.md#common-registration-order-mistakes) |
| Save button throws error | Backend API endpoint not responding or not configured | Verify backend is running and returns HTTP 200 on `api/reports/save`; check browser Network tab for errors |
| `NonTrustedTypeDeserializationException` during preview/save | Custom data source class not trusted | Register each class **before** `WebAssemblyHostBuilder.CreateDefault()`: `DeserializationSettings.RegisterTrustedClass(typeof(YourClass))` and `.RegisterTrustedClass(typeof(YourClass[]))`. [Diagnostic steps →](troubleshooting.md#data-source-trust-registration-errors) |
| `SetDataAsync` call hangs or `InvalidOperationException` | Using `.Result` or `.Wait()` instead of `await` | Replace all `.Result`/`.Wait()` calls with `await` — single-threaded WASM cannot block |
| Report names not showing in Open dialog | `GetUrlsAsync()` returning empty dictionary or backend endpoint not responding | Verify backend `api/reports/list` endpoint is working and returns list of report names; check browser console for errors |
| Backend returns 401 (Unauthorized) | CORS policy not configured or authentication missing | Configure CORS headers in backend; check if authentication is required and pass credentials in HttpClient |
| SQL data source wizard is empty | SQL not supported in Interactive WebAssembly Standalone | Switch to Interactive WebAssembly (Client + Server) or Interactive Server architecture for full SQL support |
| Build fails with Skia linker errors | `<NativeFileReference>` missing in `.csproj` | Add `<NativeFileReference Include="$(HarfBuzzSharpStaticLibraryPath)\2.0.23\*.a" />` to `.csproj` |
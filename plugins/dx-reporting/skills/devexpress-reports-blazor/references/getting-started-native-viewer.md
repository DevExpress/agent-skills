# Getting Started — Native Blazor Report Viewer (DxReportViewer)

## When to Use This Reference

Use this when you need to:
- Add the native `DxReportViewer` component to a Blazor app
- Load a report instance or resolve reports by name

> **Different component types?**
> — Need to let users create and save reports → 📄 [getting-started-js-designer-server.md](getting-started-js-designer-server.md)
> — Need a JS-Based Document Viewer (JavaScript callbacks, mobile-friendly, includes rich export panel) → 📄 [getting-started-js-viewer-server.md](getting-started-js-viewer-server.md)

## Setup Workflow

Follow these steps to integrate `DxReportViewer` into your Blazor app:

1. **[Install NuGet Packages](#install-nuget-packages)** — Add `DevExpress.Blazor.Reporting.Viewer` and (for WASM only) `DevExpress.Drawing.Skia`
2. **[Register namespaces in _Imports.razor](#register-namespaces-in-_importsrazor)**
3. **[Register client resources and apply a theme in App.razor](#register-client-resources-and-apply-a-theme-in-apprazor)**
4. **[Configure Program.cs](#configure-programcs)**
5. **[Add viewer to a page](#add-report-viewer-to-a-page)** — Place `<DxReportViewer>` with appropriate `@rendermode`
6. **[Load a report](#load-a-report)** — Use direct instance or load by name
7. **(WASM only) [Enable native build in your project for Interactive WebAssembly Standalone](#enable-native-build-in-your-project-for-interactive-webassembly-standalone)**
8. **(WASM only) [Configure fonts for Interactive WebAssembly Standalone](#font-loading-for-interactive-webassembly-standalone)**
9. **[Customize the viewer (optional)](#customize-the-viewer-optional)**

## Install NuGet Packages

### For Interactive Server or Interactive WebAssembly (Client + Server) backend:
```bash
dotnet add package DevExpress.Blazor.Reporting.Viewer
dotnet add package DevExpress.Drawing.Skia
```

### For Interactive WebAssembly Standalone:
```bash
dotnet add package DevExpress.Blazor.Reporting.Viewer
dotnet add package DevExpress.Drawing.Skia
dotnet add package SkiaSharp.Views.Blazor
dotnet add package SkiaSharp.NativeAssets.WebAssembly
dotnet add package SkiaSharp.HarfBuzz
dotnet add package HarfBuzzSharp.NativeAssets.WebAssembly
```

> **Version consistency**: If your project already references other DevExpress packages, ensure all DevExpress packages listed above use the **same version** as existing ones.

## Register namespaces in _Imports.razor

```razor
@using DevExpress.Blazor
@using DevExpress.Blazor.Reporting
```

## Register client resources and apply a theme in App.razor

```razor
<head>
    @DxResourceManager.RegisterScripts()
    @DxResourceManager.RegisterTheme(Themes.Fluent)
</head>
```
Available themes: Fluent, BlazingBerry, BlazingDark, OfficeWhite, Purple.

## Configure Program.cs

Use the appropriate code snippet for your hosting model to register services and middleware for the native Report Viewer:


### For Interactive Server or Interactive WebAssembly (Client + Server) backend:

```csharp
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

// Register native Report Viewer services
builder.Services.AddDevExpressServerSideBlazorReportViewer();

// AddInteractiveServerComponents() is required for Interactive WebAssembly (Client + Server) backend
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents()
    .AddInteractiveWebAssemblyComponents();

var app = builder.Build();
// ...
app.UseAntiforgery();
// AddInteractiveServerRenderMode() is required for Interactive WebAssembly (Client + Server) backend
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(typeof(ClientProject._Imports).Assembly);

app.Run();
```

### For Interactive WebAssembly Standalone:

```csharp
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
// required DevExpress Blazor Reporting namespaces:
using DevExpress.Blazor.Reporting;
using DevExpress.DataAccess.Web;
using DevExpress.XtraReports.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
// Register HttpClient for report storage calls and font loading
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Register native Report Viewer services for Interactive WebAssembly Standalone mode:
builder.Services.AddDevExpressWebAssemblyBlazorReportViewer();

await builder.Build().RunAsync();
```

## Add Report Viewer to a Page

The minimal setup requires a report source. Rendering modes vary by hosting model.

```razor
@page "/viewer"
@* Required for Interactive Server and Interactive WebAssembly (Client + Server) backend, not Interactive WebAssembly Standalone *@
@rendermode InteractiveServer

<div style="width: 100%; height: calc(100vh - 120px);">
    <DxReportViewer @ref="reportViewer" 
                    Report="@Report" 
                    RootCssClasses="w-100 h-100" />
</div>

@code {
    DxReportViewer reportViewer;
    XtraReport Report;
}
```

> **Note:** The `DxReportViewer` component does not have `Height` or `Width` parameters. Wrap it in a container `<div>` or use `RootCssClasses="h-100 w-100"` to apply CSS dimensions.

## Load a report

Choose the loading strategy that matches your scenario:

**Option A** - Create a Direct Report Instance: For projects without a Report Designer (`DxReportDesigner` / `DxWasmReportDesigner`) or Document Viewer (`DxDocumentViewer` / `DxWasmDocumentViewer`) and when a report doesn't contain subreports. See: [Create a Direct Report Instance](#option-a-create-a-direct-report-instance) below.
**Option B** - Use a Report Provider: For larger projects but without a Report Designer (`DxReportDesigner` / `DxWasmReportDesigner`), inject `IReportProvider` / `IReportProviderAsync` to resolve reports by name. See: [Use a Report Provider (Name Resolution)](#option-b-use-a-report-provider-name-resolution) below.
**Option C**: For projects with `DxReportDesigner`, use `ReportStorageWebExtension` for consistency between Designer and Viewer. See: [Use ReportStorageWebExtension](#option-c-use-reportstoragewebextension-designer-projects) below.

### Option A: Create a Direct Report Instance

Create a report instance directly in OnInitialized:
```razor
@using DevExpress.XtraReports.UI

@code {
    XtraReport Report;

    protected override void OnInitialized() {
        Report = new SalesReport();
    }
}
```

### Option B: Use a Report Provider (Name Resolution)

**Choose this option when:** reports are resolved by name string and you don't need a designer.

#### For Interactive Server or Interactive WebAssembly (Client + Server) backend

1. Implement an `IReportProvider` — see **Server-Side Scenarios → IReportProvider** in 📄 [resolving-report-names.md](resolving-report-names.md#ireportprovider)
2. Register it in `Program.cs` (must come **after** `AddDevExpressServerSideBlazorReportViewer()`)
3. Inject `IReportProvider` into a viewer page:
```razor
@page "/viewer/{ReportName?}"
@rendermode InteractiveServer

@using DevExpress.XtraReports.UI
@using DevExpress.XtraReports.Services

@inject IReportProvider ReportProvider

@code {
    XtraReport Report;

    [Parameter] 
    public string ReportName { get; set; } = "SalesReport";

    protected override void OnInitialized() {
        Report = ReportProvider.GetReport(ReportName, null!);
    }
}
```

#### For Interactive WebAssembly Standalone

1. Implement an `IReportProviderAsync` — see **Client-Side WebAssembly Scenarios → IReportProviderAsync** in 📄 [resolving-report-names.md](resolving-report-names.md#ireportproviderasync-interactive-webassembly-standalone)
2. Register it in `Program.cs` (must come **after** `AddDevExpressBlazorReportingWebAssembly()`)
3. Inject `IReportProviderAsync` into a viewer page:
```razor
@page "/viewer/{ReportName?}"

@using DevExpress.XtraReports.UI
@using DevExpress.XtraReports.Services

@inject IReportProviderAsync ReportProviderAsync

@code {
    XtraReport Report;

    [Parameter] 
    public string ReportName { get; set; } = "SalesReport";

    protected override async Task OnInitializedAsync() {
        Report = await ReportProviderAsync.GetReportAsync(ReportName, null!);
    }
}
```

### Option C: Use ReportStorageWebExtension (Designer Projects)

**Choose this option when:** you also have `DxReportDesigner` in your project and need Save / Open dialogs, or you prefer a single storage service for both viewer and designer.

#### For Interactive Server or Interactive WebAssembly (Client + Server) backend

1. Implement a `ReportStorageWebExtension` — see **Server-Side Scenarios → ReportStorageWebExtension** in 📄 [resolving-report-names.md](resolving-report-names.md#reportstoragewebextension)
2. Register it in `Program.cs` (must come **after** `AddDevExpressServerSideBlazorReportViewer()`)
3. Inject `ReportStorageWebExtension` into a viewer page:

```razor
@page "/viewer/{ReportName?}"

@using DevExpress.XtraReports.UI
@using DevExpress.XtraReports.Web.Extensions

@inject ReportStorageWebExtension ReportStorage

@code {
    XtraReport Report;

    [Parameter] 
    public string ReportName { get; set; } = "SalesReport";

    protected override void OnInitialized() {
        var bytes = ReportStorage.GetData(ReportName);
        using var stream = new MemoryStream(bytes);
        Report = XtraReport.FromXmlStream(stream);
    }
}
```

#### For Interactive WebAssembly Standalone

1. Implement a `ReportStorageWebExtension` with async I/O — see **Client-Side WebAssembly Scenarios → ReportStorageWebExtension (Interactive WebAssembly Standalone with Backend API)** in 📄 [resolving-report-names.md](resolving-report-names.md#reportstoragewebextension-interactive-webassembly-standalone-with-backend-api)
2. Register it in `Program.cs` (must come **after** `AddDevExpressBlazorReportingWebAssembly()`)
3. Inject `ReportStorageWebExtension` into a viewer page:

```razor
@page "/viewer/{ReportName?}"

@using DevExpress.XtraReports.UI
@using DevExpress.XtraReports.Web.Extensions

@inject ReportStorageWebExtension ReportStorage

@code {
    XtraReport Report;

    [Parameter] 
    public string ReportName { get; set; } = "SalesReport";

    protected override async Task OnInitializedAsync() {
        var bytes = await ReportStorage.GetDataAsync(ReportName);
        using var stream = new MemoryStream(bytes);
        Report = XtraReport.FromXmlStream(stream);
    }
}
```

## Enable native build in your project for Interactive WebAssembly Standalone

`.csproj`:
```xml
<PropertyGroup>
    <WasmBuildNative>true</WasmBuildNative>
</PropertyGroup>
```

---

## Font Loading for Interactive WebAssembly Standalone

Interactive WebAssembly Standalone cannot access system fonts. You **must** load fonts into `DXFontRepository.Instance`. Fonts are typically bundled in `wwwroot/fonts/`.

Add the FontLoader.cs file with the following code to your project:
```csharp
using DevExpress.Drawing;

public static class FontLoader {
    public async static Task LoadFonts(HttpClient httpClient, List<string> fontNames) {
        foreach (var fontName in fontNames) {
            var fontBytes = await httpClient.GetByteArrayAsync($"fonts/{fontName}");
            DXFontRepository.Instance.AddFont(fontBytes);
        }
    }
}
```

Call a `FontLoader.LoadFonts` method on the page that includes the DxReportViewer control:
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

Without fonts, WASM viewers crash with "Skia font not found" errors.

## Customize the Viewer (Optional)

For viewer customization options, see 📄 [customizing-native-viewer.md](customizing-native-viewer.md).

## Antipatterns — Native Viewer Specific

Common mistakes when using `DxReportViewer` (native component).

**❌ A9: Set `Height` or `Width` on `DxReportViewer` directly**
- **Symptom**: Component renders at an awkward size; CSS dimensions do not apply
- **Why**: `DxReportViewer` does not have `Height`/`Width` parameters (unlike JS-based viewers)
- **Fix**: 
  Option 1. Wrap the viewer in a container `<div>` with CSS dimensions:
  ```razor
  <div style="height: calc(100vh - 60px); width: 100%;">
      <DxReportViewer @ref="reportViewer" Report="@report" />
  </div>
  ```
  Option 2. Use RootCssClasses on the viewer to apply CSS dimensions:
  ```razor
  <DxReportViewer @ref="reportViewer" Report="@report" RootCssClasses="w-100 h-100" />
  ```

**❌ A10: Use the `ReportName` property on `DxReportViewer`**
- **Symptom**: Compiler error or runtime error — property does not exist
- **Why**: `ReportName` is a JS-based-only property (on `DxDocumentViewer` / `DxWasmDocumentViewer`)
- **Fix**: For native `DxReportViewer`, pass the report instance directly using the `Report` parameter, or inject a Report Name Resolution service (`IReportProvider` or `IReportProviderAsync`) and call it in `OnInitialized()` to resolve by name:
  ```razor
  @inject IReportProvider ReportProvider
  
  <DxReportViewer Report="@report" />
  
  @code {
      XtraReport report;
      protected override void OnInitialized() {
          report = ReportProvider.GetReport("SalesReport", null!);
      }
  }
  ```
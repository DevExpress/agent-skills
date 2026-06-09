# Getting Started — JS-Based Document Viewer: Blazor WebAssembly Standalone App

## When to Use This Reference

Use this when you are building a **Blazor WebAssembly Standalone App** — no ASP.NET Core backend, the app runs entirely in the browser — and need a document viewer with JavaScript client-side events.

- Single-project deployment, entirely client-side
- No server communication required for report rendering
- Report rendering happens **client-side** using the Skia graphics library
- Component: `DxDocumentViewer`
- Report loading: `IReportProviderAsync` (fetches `.repx` files from `wwwroot/reports/` via `HttpClient`)

> **Limitations**: No SQL or MongoDB data sources (JSON and Object data sources only). Fonts must be loaded manually before rendering.
>
> **Not your scenario?**
> — Interactive Server (single project, server-side rendering) → 📄 [getting-started-js-viewer-server.md](getting-started-js-viewer-server.md)
> — Interactive WebAssembly (Client + Server) → 📄 [getting-started-js-viewer-interactive-wasm.md](getting-started-js-viewer-interactive-wasm.md)
> — Native Blazor viewer → 📄 [getting-started-native-viewer.md](getting-started-native-viewer.md)

## Setup Workflow

Follow these steps to integrate `DxDocumentViewer` into your **Standalone Blazor WebAssembly** app:

1. **[Install NuGet Packages](#install-nuget-packages)** — Add `DevExpress.Blazor.Reporting.JSBasedControls`, Skia, and native assets (5 packages total)
2. **[Configure the project file](#configure-the-project-file)** — Add `NativeFileReference` for HarfBuzz
3. **[Register client resources in App.razor](#register-client-resources-in-apprazor)** 
4. **[Register namespaces in _Imports.razor](#register-namespaces-in-_importsrazor)**
5. **[Implement IReportProviderAsync](#implement-ireportproviderasync)** — Fetch `.repx` files from `wwwroot` and load via `XtraReport.FromXmlStream()`
6. **[Configure Program.cs](#configure-programcs)** — Call `AddDevExpressBlazorReportingWebAssembly()` and register `IReportProviderAsync`
7. **[Load fonts](#load-fonts)** — Register fonts for Skia rendering via `DXFontRepository`
8. **[Add viewer to a page](#add-a-document-viewer-to-a-page)** — Place `<DxDocumentViewer>` component

## Install NuGet Packages

```bash
dotnet add package DevExpress.Blazor.Reporting.JSBasedControls
dotnet add package DevExpress.Drawing.Skia
dotnet add package SkiaSharp.Views.Blazor
dotnet add package SkiaSharp.NativeAssets.WebAssembly
dotnet add package HarfBuzzSharp.NativeAssets.WebAssembly
```

> **Version consistency**: If your project already references other DevExpress packages, ensure `DevExpress.Blazor.Reporting.JSBasedControls` and `DevExpress.Drawing.Skia` use the **same version** as existing DevExpress packages. 

## Configure the Project File

Add a `NativeFileReference` for native HarfBuzz support. In the `.csproj` file:

```xml
<ItemGroup>
    <NativeFileReference Include="$(HarfBuzzSharpStaticLibraryPath)\2.0.23\*.a" />
</ItemGroup>
```

Build the project after adding this reference.

## Register Namespaces in _Imports.razor

```razor
@using DevExpress.Blazor.Reporting
```

## Register Client Resources in App.razor

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
- Critical constraints specific to Standalone WASM
- Common antipatterns and their fixes

## Add a Document Viewer to a Page

```razor
@page "/documentviewer"

<DxDocumentViewer ReportName="TestReport" Height="1000px" Width="100%" />
```

> No `@rendermode` directive is needed — Interactive WebAssembly Standalone apps are entirely client-side.

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

## Constraints

1. **No SQL or MongoDB data sources**: Only JSON and Object data sources are supported client-side.
2. **Fonts must be loaded before rendering**: Without `DXFontRepository.Instance.AddFont(...)`, Skia cannot render text — the viewer displays blank content or garbled text.
3. **Use `IReportProviderAsync`, not `IReportProvider`**: The async variant is required because `HttpClient` operations are inherently async in WASM.

## Antipatterns to Avoid

### Installing server-side packages in Interactive WebAssembly Standalone

- **Antipattern**: Installing `DevExpress.AspNetCore.Reporting` in an Interactive WebAssembly Standalone project
- **Why**: There is no server-side project in this architecture; that package includes ASP.NET Core dependencies that do not belong in a client-only project. It adds unnecessary bloat and can cause build conflicts
- **Correct Pattern**:
  - Use only: `DevExpress.Blazor.Reporting.JSBasedControls`, `DevExpress.Drawing.Skia`, `SkiaSharp.*`, `HarfBuzzSharp.*`
  - Do NOT install: `DevExpress.AspNetCore.Reporting`

### Wrong NuGet package variant for Interactive WebAssembly Standalone

- **Antipattern**: Installing `DevExpress.Blazor.Reporting.JSBasedControls.WebAssembly` in an Interactive WebAssembly Standalone project
- **Why**: That package is for [Interactive WebAssembly (Client + Server)](getting-started-js-viewer-interactive-wasm.md), not standalone. It includes server communication logic that creates overhead in a pure client-side app
- **Correct Pattern**: Use `DevExpress.Blazor.Reporting.JSBasedControls` (without `.WebAssembly` suffix)

### Calling server-side registration method in Interactive WebAssembly Standalone Program.cs

- **Antipattern**: Calling `builder.Services.AddDevExpressBlazorReporting()` in an Interactive WebAssembly Standalone project
- **Why**: That is the Interactive Server registration method; calling it in Interactive WebAssembly Standalone causes runtime initialization errors
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
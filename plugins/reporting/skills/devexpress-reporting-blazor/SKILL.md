---
name: devexpress-reporting-blazor
description: Expert skill for integrating DevExpress Report Viewer and Report Designer into Blazor applications. Use when someone needs to add a DevExpress report viewer or designer to a Blazor Server, Blazor WebAssembly, or Blazor Web App; register AddDevExpressServerSideBlazorReportViewer or AddDevExpressBlazorReporting in Program.cs; configure DxResourceManager.RegisterScripts in App.razor; use DxReportViewer, DxDocumentViewer, DxReportDesigner, DxWasmDocumentViewer, or DxWasmReportDesigner components; load a report via OpenReportAsync or ReportName property; customize the toolbar, tab panel, or parameter editors; implement IReportProvider or ReportStorageWebExtension for Blazor; troubleshoot a blank viewer, "Service not registered" errors, missing render mode, or Skia DllNotFoundException on WebAssembly; or choose between native DxReportViewer, JS-based, and WASM component families.
compatibility: Requires .NET 8.0+. NuGet packages from the DevExpress feed (nuget.devexpress.com). DevExpress.Drawing.Skia required for Blazor WebAssembly native viewer. Reporting components require interactive render mode — static render mode is not supported.
metadata:
  author: DevExpress
  version: "26.1"
  source-commit: 4ffd3390ef53dde5c173f83c51a71d523d0c6eb1
---

# DevExpress Blazor Reporting — Viewer & Designer Integration

Integrates DevExpress report viewing and designing into Blazor applications. Three component families are available, each with different capabilities and setup requirements — choose the right one before writing any code.

This skill covers **Blazor UI component integration only**. For creating reports programmatically (bands, controls, data binding, export), use the `devexpress-reporting-core` skill.

## When to Use This Skill

- Add a report viewer or designer to a Blazor Server, Blazor WebAssembly, or Blazor Web App project
- Choose between `DxReportViewer` (native), `DxDocumentViewer`/`DxReportDesigner` (JS-based Server), and `DxWasmDocumentViewer`/`DxWasmReportDesigner` (WASM)
- Configure Program.cs services for Blazor reporting
- Set up `DxResourceManager.RegisterScripts()` in App.razor
- Load reports via `OpenReportAsync`, `Report` property, or `ReportName` property
- Implement `IReportProvider` or `ReportStorageWebExtension` for Blazor
- Customize toolbar, tab panel, export options, or parameter editors
- Configure themes (Fluent, BlazingBerry/Bootstrap 5)
- Troubleshoot blank viewers, service registration errors, or WASM font/Skia issues

## Component Family Comparison — Choose Before Writing Code

| | `DxReportViewer` | `DxDocumentViewer` / `DxReportDesigner` | `DxWasmDocumentViewer` / `DxWasmReportDesigner` |
|---|---|---|---|
| Type | **Native Blazor** | JS-based (Server) | JS-based (WASM) |
| Designer included | No (viewer only) | Yes (`DxReportDesigner`) | Yes (`DxWasmReportDesigner`) |
| Render mode | Interactive Server **or** WebAssembly | Interactive Server only | Interactive WebAssembly only |
| Customization | C# (`OnCustomizeToolbar`, etc.) | JavaScript callbacks | JavaScript callbacks |
| Requires ASP.NET Core backend | No (self-contained) | No (Server) | Yes (for WASM) |
| Requires MVC controllers | No | Yes | Yes |
| NuGet | `DevExpress.Blazor.Reporting.Viewer` | `DevExpress.Blazor.Reporting.JSBasedControls` + `DevExpress.AspNetCore.Reporting` | Client: `JSBasedControls.WebAssembly`; Server: `DevExpress.AspNetCore.Reporting` |

**Primary recommendation**: Use `DxReportViewer` (native) for viewer-only scenarios. Use `DxDocumentViewer`/`DxReportDesigner` (JS-based Server) when you need a designer or JavaScript customization.

## Prerequisites & Installation

### NuGet Packages

**Native viewer (DxReportViewer)**:
```
DevExpress.Blazor.Reporting.Viewer
```

**JS-based Server (DxDocumentViewer + DxReportDesigner)**:
```
DevExpress.Blazor.Reporting.JSBasedControls
DevExpress.AspNetCore.Reporting
```

**JS-based WASM (DxWasmDocumentViewer + DxWasmReportDesigner)**:
- Client project: `DevExpress.Blazor.Reporting.JSBasedControls.WebAssembly`
- Server project: `DevExpress.AspNetCore.Reporting`

**Linux/macOS or Blazor WebAssembly native viewer**:
```
DevExpress.Drawing.Skia      (+ SkiaSharp.Views.Blazor, SkiaSharp.NativeAssets.WebAssembly for WASM)
```

## Before You Start — Ask the Developer

Before generating code, ask:

1. **Blazor hosting model**: Blazor Web App (Server/WASM interactivity), standalone Blazor WebAssembly, or Blazor Server?
2. **Component family**: Viewer only (native `DxReportViewer`), or also designer/JS-customization (JS-based)?
3. **Render mode for the viewer page**: Interactive Server, Interactive WebAssembly, or Auto?
4. **Report source**: Load by C# instance (`Report="@report"`) or by name string (`ReportName="TestReport"`)?
5. **Target OS**: Windows, Linux, or macOS? (Skia package required for non-Windows and WASM)
6. **Customization needed?** If toolbar/export customization is required, this determines the component family — see the decision gate below.

### Component Selection Decision Gate

Use this before generating any customization code:

| User need | → Component |
|-----------|-------------|
| View reports, C# customization (toolbar, exports via `ExportModel`) | **`DxReportViewer`** (native) |
| View reports, JavaScript customization OR designer in Server mode | **`DxDocumentViewer` / `DxReportDesigner`** (JS-based Server) |
| WASM frontend with ASP.NET Core backend | **`DxWasmDocumentViewer` / `DxWasmReportDesigner`** |

> **Critical**: Do not mix APIs across families. `OnCustomizeToolbar`/`ExportModel` are `DxReportViewer`-only (C#). `CustomizeMenuActions`/`CustomizeExportOptions` are JS callbacks for `DxDocumentViewer`/`DxWasmDocumentViewer`. Applying native patterns to JS-based components (or vice versa) causes silent failures or `undefined` errors.

## Setup: Native DxReportViewer (Viewer Only)

### Step 1 — Program.cs

```csharp
using DevExpress.Blazor.Reporting;
using Microsoft.AspNetCore.Components;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorComponents().AddInteractiveServerComponents();

builder.Services.AddDevExpressServerSideBlazorReportViewer(); // Interactive Server
// builder.Services.AddDevExpressWebAssemblyBlazorReportViewer(); // Interactive WebAssembly

builder.WebHost.UseStaticWebAssets();
var app = builder.Build();
app.UseStaticFiles();
app.UseAntiforgery();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
app.Run();
```

### Step 2 — App.razor

```razor
<head>
    @* Register DevExpress scripts — REQUIRED *@
    @DxResourceManager.RegisterScripts()
    @* Choose one CSS theme: *@
    @DxResourceManager.RegisterTheme(Themes.BlazingBerry)
    <link rel="stylesheet"
          href="_content/DevExpress.Blazor.Reporting.Viewer/css/dx-blazor-reporting-components.bs5.css" />
    @* For Fluent theme: *@
    @* @DxResourceManager.RegisterTheme(Themes.Fluent) *@
    @* <link href="..../dx-blazor-reporting-components.fluent.css" rel="stylesheet" /> *@
</head>
```

### Step 3 — _Imports.razor

```razor
@using DevExpress.Blazor
@using DevExpress.Blazor.Reporting
```

### Step 4 — Viewer Page

```razor
@page "/viewer"
@rendermode InteractiveServer

<DxReportViewer @ref="reportViewer" Report="@report" />

@code {
    DxReportViewer? reportViewer;
    XtraReport report = new TestReport(); // or load via IReportProvider
}
```

> **Troubleshooting**: If the viewer is blank, verify `DxResourceManager.RegisterScripts()` is in App.razor `<head>`. If you get "Service not registered", verify `AddDevExpressServerSideBlazorReportViewer()` is in Program.cs and the page has `@rendermode InteractiveServer`.

## Setup: JS-Based Components (DxDocumentViewer + DxReportDesigner)

See [references/getting-started-js-viewer.md](references/getting-started-js-viewer.md) and [references/getting-started-js-designer.md](references/getting-started-js-designer.md) for complete setup.

Key differences from native viewer:
- Program.cs: `AddDevExpressBlazorReporting()` + `UseDevExpressBlazorReporting()` + `UseDevExpressControls()` + `MapControllers()`
- Razor component: `<DxDocumentViewer ReportName="TestReport" Height="1000px" Width="100%" />`
- Requires `IReportProvider` or `ReportStorageWebExtension` registered as a service
- JavaScript callbacks instead of C# event handlers for customization

## Documentation & Navigation Guide

### Native Viewer — Full Setup and Theming
📄 [references/getting-started-native-viewer.md](references/getting-started-native-viewer.md)

When you need to: complete Program.cs for WASM native viewer; set themes; load fonts in WASM; use `IReportProvider`; configure Skia for non-Windows.

### JS-Based Document Viewer (Blazor Server / WASM Hosted)
📄 [references/getting-started-js-viewer.md](references/getting-started-js-viewer.md)

### JS-Based Report Designer (Blazor Server / WASM Hosted)
📄 [references/getting-started-js-designer.md](references/getting-started-js-designer.md)

When you need to: full designer setup; ReportStorageWebExtension for Blazor; WASM designer controllers.

### Toolbar, Tab Panel, Parameters, and Themes (Native Viewer)
📄 [references/viewer-customization.md](references/viewer-customization.md)

When you need to: customize toolbar items, hide tab panel tabs, set zoom, customize parameter editors, configure export formats.

### Troubleshooting
📄 [references/troubleshooting.md](references/troubleshooting.md)

## Key API Surface

### DxReportViewer

| Member | Type | Description |
|--------|------|-------------|
| `Report` | `[Parameter] IReport` | Load report by C# instance |
| `OpenReportAsync(IReport)` | `Task` | Load/reload report programmatically |
| `OnCustomizeToolbar` | `[Parameter] EventCallback<ToolbarModel>` | Customize toolbar items |
| `OnCustomizeTabs` | `[Parameter] EventCallback<List<TabModel>>` | Customize tab panel tabs |
| `OnCustomizeParameters` | `[Parameter] EventCallback<ParametersModel>` | Replace parameter editors |
| `TabPanelModel` | `TabPanelModel` | Access tab visibility after render |
| `ExportModel` | `ExportModel` | Access export format list |
| `Zoom` | `[Parameter] double` | Initial zoom level (1.0 = 100%) |
| `SinglePagePreview` | `[Parameter] bool` | Display single page at a time |
| `TabPanelWidth/MinWidth/MaxWidth` | `[Parameter] int` | Tab panel dimensions |

### DxDocumentViewer / DxReportDesigner

| Member | Description |
|--------|-------------|
| `ReportName` | String name resolved by `IReportProvider` or `ReportStorageWebExtension` |
| `Height`, `Width` | CSS dimensions (e.g., `"1000px"`, `"100%"`, `"calc(100vh - 60px)"`) |
| `<DxDocumentViewerCallbacks>` | Nested component for JavaScript event handlers |
| `<DxDocumentViewerTabPanelSettings>` | Nested component: `Width`, `Position` |
| `AllowMDI` (Designer) | Allow user to close all report tabs |

### IReportProvider (Blazor Server)

```csharp
using DevExpress.XtraReports.Services;

public class ReportProvider : IReportProvider {
    public XtraReport GetReport(string id, ReportProviderContext context) {
        return id switch {
            "TestReport" => new TestReport(),
            _ => throw new InvalidOperationException($"Unknown report: '{id}'")
        };
    }
}
// Program.cs:
builder.Services.AddScoped<IReportProvider, ReportProvider>();
```

## Common Patterns

### Pattern 1: Hide a tab panel tab (native viewer)

```razor
@code {
    DxReportViewer? reportViewer;

    protected override async Task OnAfterRenderAsync(bool firstRender) {
        if (firstRender && reportViewer != null) {
            reportViewer.TabPanelModel[TabContentKind.Search].Visible = false;
            reportViewer.TabPanelModel[TabContentKind.DocumentMap].Visible = false;
        }
    }
}
```

### Pattern 2: Customize toolbar (native viewer)

```razor
<DxReportViewer @ref="reportViewer" Report="@report"
    OnCustomizeToolbar="@CustomizeToolbar" />

@code {
    void CustomizeToolbar(ToolbarModel toolbar) {
        // Hide XLS export formats
        toolbar.ExportModel.AvailableFormats.RemoveAll(f => f.Name.Contains("Xls"));
    }
}
```

### Pattern 3: Custom parameter editor (native viewer)

```razor
<DxReportViewer @ref="reportViewer" Report="@report"
    OnCustomizeParameters="@CustomizeParameters" />

@code {
    void CustomizeParameters(ParametersModel model) {
        foreach (var p in model.VisibleItems) {
            if (p.Type == typeof(DateTime))
                p.ValueTemplate = DatePickerTemplate; // RenderFragment<ParameterModel>
        }
    }
}
```

### Pattern 4: Filter export formats — JS-based DxDocumentViewer

The JS-based viewer uses the same client-side JS API as the ASP.NET Core viewer. Use `CustomizeExportOptions` (not `CustomizeMenuActions`) to filter export formats. Both callbacks can coexist in `DxDocumentViewerCallbacks`:

```razor
<DxDocumentViewer ReportName="SalesReport" Height="1000px" Width="100%">
    <DxDocumentViewerCallbacks
        CustomizeMenuActions="onCustomizeMenuActions"
        CustomizeExportOptions="onCustomizeExportOptions" />
</DxDocumentViewer>
```

```javascript
// CustomizeMenuActions → toolbar item visibility (Print, Search, etc.)
function onCustomizeMenuActions(s, e) {
    var printAction = e.GetById(DevExpress.Reporting.Viewer.ActionId.Print);
    if (printAction) printAction.visible = false;
}

// CustomizeExportOptions → export format availability (completely separate)
function onCustomizeExportOptions(s, e) {
    // Hide all formats except PDF
    e.HideFormat(DevExpress.Reporting.Viewer.ExportFormatID.XLS);
    e.HideFormat(DevExpress.Reporting.Viewer.ExportFormatID.XLSX);
    e.HideFormat(DevExpress.Reporting.Viewer.ExportFormatID.DOCX);
    e.HideFormat(DevExpress.Reporting.Viewer.ExportFormatID.RTF);
    e.HideFormat(DevExpress.Reporting.Viewer.ExportFormatID.CSV);
    e.HideFormat(DevExpress.Reporting.Viewer.ExportFormatID.IMAGE);
    e.HideFormat(DevExpress.Reporting.Viewer.ExportFormatID.TEXT);
    // PDF remains visible
}
```

> **Common mistake**: Using `CustomizeMenuActions` to hide the Export To menu item does not remove formats from the export dropdown. Use `CustomizeExportOptions` to control which formats appear.
>
> See [references/viewer-customization.md](references/viewer-customization.md) for the full `ExportFormatID` list and native viewer equivalent.

### Pattern 6: JS-based viewer with IReportProvider

```csharp
// Program.cs
builder.Services.AddScoped<IReportProvider, ReportProvider>();
builder.Services.AddDevExpressBlazorReporting();
// ...
app.UseDevExpressBlazorReporting();
app.UseDevExpressControls();
app.UseEndpoints(e => e.MapControllers()); // required for JS-based viewer API
```

```razor
@page "/docviewer"
@rendermode InteractiveServer
<DxDocumentViewer ReportName="TestReport" Height="1000px" Width="100%">
    <DxDocumentViewerTabPanelSettings Width="340" />
</DxDocumentViewer>
```

## Troubleshooting

| Symptom | Cause | Fix |
|---------|-------|-----|
| Blank native viewer | `DxResourceManager.RegisterScripts()` missing from App.razor | Add to `<head>` in App.razor |
| "Service not registered" at startup | `AddDevExpressServerSideBlazorReportViewer()` or `AddDevExpressBlazorReporting()` missing | Add to Program.cs |
| 404 on reporting API calls (JS-based) | `UseDevExpressBlazorReporting()` or `MapControllers()` missing | Add both to Program.cs pipeline |
| Components not found / compile error | Missing `@using DevExpress.Blazor.Reporting` | Add to `_Imports.razor` |
| `@rendermode` attribute error | Page using static render mode | Add `@rendermode InteractiveServer` (or `InteractiveWebAssembly`) to the page |
| CSS not loaded (native viewer) | Missing `<link>` for reporting CSS in App.razor | Add `_content/DevExpress.Blazor.Reporting.Viewer/css/dx-blazor-reporting-components.bs5.css` |
| `DllNotFoundException` for Skia (WASM or Linux) | `DevExpress.Drawing.Skia` package missing | Add `DevExpress.Drawing.Skia` (+ SkiaSharp packages for WASM) |
| Fonts not rendering in WASM native viewer | Font bytes not loaded into `DXFontRepository` | Load fonts via `DXFontRepository.Instance.AddFont(bytes)` in `OnInitializedAsync` |
| JS-based designer blank / no toolbar | `UseDevExpressBlazorReporting()` middleware missing | Add before `UseAntiforgery()` in Program.cs |
| Designer can't save/load reports | `ReportStorageWebExtension` not registered | Register after `AddDevExpressControls()`: `builder.Services.AddScoped<ReportStorageWebExtension, CustomStorage>()` |
| "All export formats disappeared" in JS-based viewer | `CustomizeMenuActions` used to filter exports instead of `CustomizeExportOptions` | Switch to `CustomizeExportOptions` callback; call `e.HideFormat(ExportFormatID.XLS)` etc. `CustomizeMenuActions` only controls toolbar item visibility, not the export dropdown content |
| `e.HideFormat is not a function` in JS console | Wrong callback — `CustomizeMenuActions` instead of `CustomizeExportOptions` | Use `<DxDocumentViewerCallbacks CustomizeExportOptions="handler" />`; `e.HideFormat` is only available in the `CustomizeExportOptions` handler |
| Hiding "Export To" button removes it but formats still accessible elsewhere | Only the toolbar button is hidden, not the underlying format list | Add a `CustomizeExportOptions` handler in addition to the `CustomizeMenuActions` handler |
| Native viewer `OnCustomizeToolbar` code applied to `DxDocumentViewer` | API mismatch — native C# API used on JS-based component | `DxDocumentViewer` uses `DxDocumentViewerCallbacks` with JavaScript functions, not C# event handlers |

## Constraints & Rules

CRITICAL — follow in every interaction:

1. **Choose component family first**: Before writing any code, confirm `DxReportViewer` vs. `DxDocumentViewer`/`DxReportDesigner` vs. WASM variants. Setup differs significantly.
2. **Interactive render mode required**: Reporting components do not support static render mode. Always add `@rendermode InteractiveServer` or `@rendermode InteractiveWebAssembly` to the page.
3. **RegisterScripts in App.razor head**: `DxResourceManager.RegisterScripts()` must be in the `<head>` element of App.razor. Without it, no client-side scripts load.
4. **Service registration**: Native viewer: `AddDevExpressServerSideBlazorReportViewer()`. JS-based Server: `AddDevExpressBlazorReporting()`. Do not mix registrations.
5. **Middleware for JS-based**: Call `UseDevExpressBlazorReporting()` + `UseDevExpressControls()` + `MapControllers()` in Program.cs for JS-based components.
6. **CreateDocumentAsync in web**: Never call `report.CreateDocument()` synchronously. Always `await report.CreateDocumentAsync()`.
7. **Skia for non-Windows**: Add `DevExpress.Drawing.Skia` for any Linux/macOS deployment and for WASM native viewer.
8. **WASM font loading**: For WASM native viewer, load fonts into `DXFontRepository.Instance` during `OnInitializedAsync` before the viewer renders.
9. **Version consistency**: All DevExpress NuGet packages must use the same version.
10. **Build verification**: Run `dotnet build --project <path>` after changes. Check for errors before reporting success.
11. **Never use `CustomizeMenuActions` to filter export formats in JS-based viewer**: `CustomizeMenuActions` controls toolbar item visibility only. To control which formats appear in the Export To dropdown, use the `CustomizeExportOptions` callback and call `e.HideFormat()`. Both callbacks can coexist in `<DxDocumentViewerCallbacks>` — they handle different concerns.
12. **JS-based customization API = ASP.NET Core viewer API**: `DxDocumentViewer` and `DxWasmDocumentViewer` are Blazor wrappers around the same HTML5 JS viewer used in ASP.NET Core. All JS client-side APIs (`CustomizeMenuActions`, `CustomizeExportOptions`, `ExportFormatID`, `ActionId`) are identical. Reference [devexpress-reporting-aspnetcore references/viewer-customization.md] for the full API list.

## Using DevExpress Documentation MCP

If DxDocs MCP tools are available:

- **Search**: `devexpress_docs_search(technology="Blazor", query="your question")`
- **Fetch**: `devexpress_docs_get_content(url="docs.devexpress.com/...")`

Useful queries:
- `"AddDevExpressServerSideBlazorReportViewer DxReportViewer Program.cs"` — native viewer setup
- `"AddDevExpressBlazorReporting UseDevExpressBlazorReporting Program.cs"` — JS-based setup
- `"DxReportViewer OpenReportAsync Report property"` — report loading
- `"DxResourceManager RegisterScripts App.razor"` — script registration
- `"IReportProvider Blazor GetReport"` — report name resolution
- `"ReportStorageWebExtension Blazor designer save"` — designer storage
- `"OnCustomizeToolbar ToolbarModel DxReportViewer"` — toolbar customization
- `"DXFontRepository AddFont WASM fonts"` — WASM font loading
- `"DevExpress.Drawing.Skia Blazor WebAssembly"` — Skia for WASM

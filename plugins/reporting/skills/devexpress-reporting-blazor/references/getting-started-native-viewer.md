# Getting Started — Native Blazor Report Viewer (DxReportViewer)

## When to Use This Reference

Use this when you need to:
- Add the native `DxReportViewer` component to a Blazor Server or WebAssembly app
- Load a report instance or resolve reports by name
- Apply themes
- Understand WASM-specific limitations (Skia, font loading, data sources)

## NuGet Package

```bash
dotnet add package DevExpress.Blazor.Reporting.Viewer
# For WASM or Linux/macOS hosting:
dotnet add package DevExpress.Drawing.Skia
```

## Program.cs (Blazor Web App — Server mode)

```csharp
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Register native Report Viewer services
builder.Services.AddDevExpressServerSideBlazorReportViewer();

// Optional: register IReportProvider to resolve reports by name
builder.Services.AddScoped<DevExpress.XtraReports.Services.IReportProvider, MyReportProvider>();

builder.WebHost.UseWebRoot("wwwroot");
builder.WebHost.UseStaticWebAssets();

var app = builder.Build();
app.UseStaticFiles();
app.UseRouting();
app.UseAntiforgery();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
app.Run();
```

## App.razor — Register Scripts and Theme

```razor
<head>
    @* Register DevExpress component scripts *@
    @DxResourceManager.RegisterScripts()

    @* Apply a theme — choose one: *@
    @DxResourceManager.RegisterTheme(Themes.BlazingBerry)
    <link rel="stylesheet"
          href="_content/DevExpress.Blazor.Reporting.Viewer/css/dx-blazor-reporting-components.bs5.css" />

    @* Or for Fluent theme: *@
    @* @DxResourceManager.RegisterTheme(Themes.Fluent) *@
    @* <link rel="stylesheet" *@
    @*       href="_content/DevExpress.Blazor.Reporting.Viewer/css/dx-blazor-reporting-components.fluent.css" /> *@
</head>
```

## _Imports.razor

```razor
@using DevExpress.Blazor
@using DevExpress.Blazor.Reporting
```

## ReportViewer.razor — Basic Usage

```razor
@page "/viewer"
@rendermode InteractiveServer

@using DevExpress.Blazor.Reporting
@using DevExpress.XtraReports.UI

<DxReportViewer @ref="reportViewer" Report="@Report" />

@code {
    DxReportViewer reportViewer;
    XtraReport Report = new SalesReport();
}
```

## Load Report by Name (via IReportProvider)

```csharp
// Services/MyReportProvider.cs
using DevExpress.XtraReports.Services;
using DevExpress.XtraReports.UI;

public class MyReportProvider : IReportProvider {
    public XtraReport GetReport(string reportName, ReportProviderContext context) {
        return reportName switch {
            "SalesReport" => new SalesReport(),
            "InvoiceReport" => new InvoiceReport(),
            _ => throw new InvalidOperationException($"Report '{reportName}' not found.")
        };
    }
}
```

```razor
@page "/viewer/{ReportName?}"
@rendermode InteractiveServer

@using DevExpress.Blazor.Reporting
@using DevExpress.XtraReports.Services

@inject IReportProvider ReportProvider

<DxReportViewer @ref="reportViewer" Report="@Report" />

@code {
    DxReportViewer reportViewer;
    DevExpress.XtraReports.UI.XtraReport Report;

    [Parameter] public string ReportName { get; set; } = "SalesReport";

    protected override void OnInitialized() {
        Report = ReportProvider.GetReport(ReportName, null);
    }
}
```

## Load Report Asynchronously

Use `OpenReportAsync` when the report isn't available at component initialization time:

```razor
@code {
    DxReportViewer reportViewer;

    protected override async Task OnAfterRenderAsync(bool firstRender) {
        if (firstRender) {
            var report = new SalesReport();
            await reportViewer.OpenReportAsync(report);
        }
    }
}
```

## Themes

Apply per-page (without `DxResourceManager.RegisterTheme`):

```razor
@* Blazing Berry (Classic) *@
<link href="_content/DevExpress.Blazor.Themes/blazing-berry.bs5.min.css" rel="stylesheet" />
<link href="_content/DevExpress.Blazor.Reporting.Viewer/css/dx-blazor-reporting-components.bs5.css" rel="stylesheet" />

@* Fluent Light Blue *@
<link href="_content/DevExpress.Blazor.Themes.Fluent/core.min.css" rel="stylesheet" />
<link href="_content/DevExpress.Blazor.Themes.Fluent/global.min.css" rel="stylesheet" />
<link href="_content/DevExpress.Blazor.Themes.Fluent/modes/light.min.css" rel="stylesheet" />
<link href="_content/DevExpress.Blazor.Themes.Fluent/accents/blue.min.css" rel="stylesheet" />
<link href="_content/DevExpress.Blazor.Reporting.Viewer/css/dx-blazor-reporting-components.fluent.css" rel="stylesheet" />
```

## Render Mode Requirements

| Render Mode | Supported |
|-------------|-----------|
| Static Server | No |
| Interactive Server | Yes |
| Interactive WebAssembly | Yes |
| Interactive Auto | Not recommended |

Always add `@rendermode InteractiveServer` or `@rendermode InteractiveWebAssembly` to viewer pages.

## WebAssembly Specifics

For WASM (standalone or hosted), these requirements apply:

**1. Install Skia package:**
```bash
dotnet add package DevExpress.Drawing.Skia
```

**2. Enable native build in .csproj:**
```xml
<PropertyGroup>
    <WasmBuildNative>true</WasmBuildNative>
</PropertyGroup>
```

**3. Font loading (WASM cannot use system fonts):**
```csharp
// In Program.cs (WASM client project)
using DevExpress.Drawing;

DXFontRepository.Instance.AddFont("wwwroot/fonts/OpenSans-Regular.ttf");
```

**4. Data sources:** Only JSON and Object data sources are supported in pure WASM. SQL connections are not available client-side.

## Subreports

Register `IReportProviderAsync` to resolve subreport names asynchronously:

```csharp
using DevExpress.XtraReports.Services;
using DevExpress.XtraReports.UI;

public class AsyncReportProvider : IReportProviderAsync {
    public async Task<XtraReport> GetReportAsync(string id, ReportProviderContext ctx) {
        var report = await LoadFromStorageAsync(id);
        return report;
    }
}

// Program.cs
builder.Services.AddScoped<IReportProviderAsync, AsyncReportProvider>();
```

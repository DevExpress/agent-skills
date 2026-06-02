# Getting Started — JavaScript-Based Report Designer (Blazor)

## When to Use This Reference

Use this when you need to:
- Add `DxReportDesigner` (SignalR, no separate ASP.NET Core backend) or `DxWasmReportDesigner` (fetch, with backend) to a Blazor app
- Allow end users to create, edit, and save reports in the browser

## Component Comparison

| Component | Communication | Requires ASP.NET Core Backend | Requires MVC Controllers |
|-----------|---------------|-------------------------------|--------------------------|
| `DxReportDesigner` | SignalR | No | No |
| `DxWasmReportDesigner` | HTTP fetch | Yes | Yes |

## NuGet Packages

```bash
# Blazor project
dotnet add package DevExpress.Blazor.Reporting.JSBasedControls

# If using DxWasmReportDesigner with an ASP.NET Core backend:
dotnet add package DevExpress.AspNetCore.Reporting  # backend project
```

## Program.cs (Blazor Web App — Interactive Server with DxReportDesigner)

```csharp
using DevExpress.Blazor.Reporting;
using DevExpress.XtraReports.Web.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddMvc();
builder.Services.AddDevExpressBlazorReporting();

// ReportStorageWebExtension is required for the designer's Save/Load/Open dialogs
// Must be registered AFTER AddDevExpressBlazorReporting
builder.Services.AddScoped<ReportStorageWebExtension, CustomReportStorageWebExtension>();

var app = builder.Build();

app.UseStaticFiles();
app.UseRouting();
app.UseAntiforgery();
app.UseDevExpressBlazorReporting();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();
app.Run();
```

## ReportStorageWebExtension for Blazor

The designer requires `ReportStorageWebExtension` for Save, Save As, and Open dialogs. Use `InvalidOperationException` for error cases — not `FaultException` (WCF type, unavailable in .NET):

```csharp
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Web.Extensions;

public class CustomReportStorageWebExtension : ReportStorageWebExtension {
    readonly string reportDirectory;
    const string Extension = ".repx";

    public CustomReportStorageWebExtension(IWebHostEnvironment env) {
        reportDirectory = Path.Combine(env.ContentRootPath, "Reports");
        Directory.CreateDirectory(reportDirectory);
    }

    public override bool IsValidUrl(string url) =>
        !Path.IsPathRooted(url) && !url.Contains("..");

    public override bool CanSetData(string url) => true;

    public override byte[] GetData(string url) {
        var filePath = Path.Combine(reportDirectory, url + Extension);
        if (File.Exists(filePath))
            return File.ReadAllBytes(filePath);
        var report = ResolveReport(url);
        if (report != null) {
            using var stream = new MemoryStream();
            report.SaveLayoutToXml(stream);
            return stream.ToArray();
        }
        throw new InvalidOperationException($"Report '{url}' not found.");
    }

    public override Dictionary<string, string> GetUrls() {
        return Directory.GetFiles(reportDirectory, "*" + Extension)
            .ToDictionary(
                f => Path.GetFileNameWithoutExtension(f),
                f => Path.GetFileNameWithoutExtension(f));
    }

    public override void SetData(XtraReport report, string url) {
        var resolved = Path.GetFullPath(Path.Combine(reportDirectory, url + Extension));
        if (!resolved.StartsWith(Path.GetFullPath(reportDirectory) + Path.DirectorySeparatorChar))
            throw new InvalidOperationException("Invalid report name.");
        report.SaveLayoutToXml(resolved);
    }

    public override string SetNewData(XtraReport report, string defaultUrl) {
        var url = string.IsNullOrEmpty(defaultUrl) ? Guid.NewGuid().ToString() : defaultUrl;
        SetData(report, url);
        return url;
    }

    XtraReport? ResolveReport(string name) => name switch {
        "SalesReport" => new SalesReport(),
        _ => null
    };
}
```

> **Important**: Register `ReportStorageWebExtension` **after** `AddDevExpressBlazorReporting()`. Registering before causes the built-in no-op storage to take precedence.

## _Imports.razor

```razor
@using DevExpress.Blazor
@using DevExpress.Blazor.Reporting
```

## App.razor

```razor
<head>
    @DxResourceManager.RegisterScripts()
</head>
```

## ReportDesigner.razor

```razor
@page "/reportdesigner"
@rendermode InteractiveServer

<DxReportDesigner ReportName="SalesReport" Height="1000px" Width="100%" />
```

## Advanced: Nested Customization Components

```razor
<DxReportDesigner ReportName="SalesReport"
                  Height="calc(100vh - 130px)"
                  Width="100%"
                  AllowMDI="true">
    <DxReportDesignerParameterEditingSettings AllowEditParameterCollection="false" />
    <DxReportDesignerDataSourceSettings AllowAddDataSource="false" />
    <DxReportDesignerWizardSettings
        ReportWizardTemplatesSearchBoxVisibility="SearchBoxVisibility.Always" />
    <DxReportDesignerReportPreviewSettings
        ExportSettings="new ExportSettings { UseSameTab = false, UseAsynchronousExport = false }" />
</DxReportDesigner>
```

## DxWasmReportDesigner (with ASP.NET Core Backend)

For a WASM frontend communicating with a separate ASP.NET Core backend, use `DxWasmReportDesigner`. The backend needs all three reporting MVC controllers — see [getting-started in the aspnetcore skill](../../devexpress-reporting-aspnetcore/references/getting-started.md).

```razor
@page "/reportdesigner"
@rendermode InteractiveWebAssembly

<DxWasmReportDesigner ReportName="SalesReport" Height="100%">
    <DxWasmReportDesignerRequestOptions
        GetDesignerModelAction="DXXRD/GetDesignerModel" />
    <DxReportDesignerModelSettings AllowMDI="true">
        <DxReportDesignerParameterEditingSettings AllowEditParameterCollection="false" />
        <DxReportDesignerDataSourceSettings AllowAddDataSource="false" />
    </DxReportDesignerModelSettings>
</DxWasmReportDesigner>
```

## Client-Side Events (JavaScript)

```razor
<DxReportDesigner ReportName="SalesReport" Height="1000px" Width="100%">
    <DxReportDesignerCallbacks
        CustomizeMenuActions="onDesignerCustomizeMenuActions"
        ReportSaved="onReportSaved" />
</DxReportDesigner>
```

```javascript
function onDesignerCustomizeMenuActions(s, e) {
    var newAction = e.GetById(DevExpress.Reporting.Designer.Actions.ActionId.NewReport);
    if (newAction) newAction.visible = false;
}

function onReportSaved(s, e) {
    console.log("Report saved: " + e.Url);
}
```

## Troubleshooting

| Symptom | Cause | Fix |
|---------|-------|-----|
| Save/Load buttons do nothing | `ReportStorageWebExtension` not registered | Register after `AddDevExpressBlazorReporting()` |
| `FaultException` compile error | WCF type used in storage | Replace with `InvalidOperationException` |
| Designer shows blank page | Interactive render mode missing | Add `@rendermode InteractiveServer` to page |

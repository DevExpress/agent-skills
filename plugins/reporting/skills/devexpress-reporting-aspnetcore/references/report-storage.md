# Report Storage — ASP.NET Core

## When to Use This Reference

Use this when you need to:
- Enable Save, Load, Open, Save As in the End-User Report Designer
- Serve reports by string name (e.g., `Bind("SalesReport")`)
- List available reports in the designer's Open dialog
- Choose between `ReportStorageWebExtension`, `IReportProvider`, and `IWebDocumentViewerReportResolver`

## Choosing the Right Service

| Service | Use When | Designer support |
|---------|---------|-----------------|
| `IReportProvider` / `IReportProviderAsync` | Viewer-only; map name → `XtraReport` | No (viewer only) |
| `ReportStorageWebExtension` | Designer present; full CRUD (load/save/list) | Yes |
| `IWebDocumentViewerReportResolver` | Highest priority override; viewer only; no async | No |

> **Rule**: If the End-User Report Designer is integrated, use `ReportStorageWebExtension` — it supports all designer file operations. `IReportProvider` is for viewer-only scenarios.

## IReportProvider (Viewer Only)

```csharp
// Services/ReportProvider.cs
using DevExpress.XtraReports.Services;

public class ReportProvider : IReportProvider {
    public XtraReport GetReport(string id, ReportProviderContext context) {
        return id switch {
            "SalesReport"   => new SalesReport(),
            "InvoiceReport" => new InvoiceReport(),
            _               => throw new InvalidOperationException($"Report '{id}' not found.")
        };
    }
}
```

```csharp
// Program.cs — register after AddDevExpressControls
builder.Services.AddScoped<IReportProvider, ReportProvider>();
```

```cshtml
@* View — bind by name string *@
@Html.DevExpress().WebDocumentViewer("DocumentViewer").Height("1000px").Bind("SalesReport")
```

For async scenarios (subreports, concurrent requests), implement `IReportProviderAsync` with `Task<XtraReport> GetReportAsync(...)`.

## ReportStorageWebExtension (Viewer + Designer)

Implement all six methods. Use `InvalidOperationException` for error cases — not `FaultException` (WCF type, unavailable in ASP.NET Core).

```csharp
// Services/CustomReportStorageWebExtension.cs
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Web.Extensions;

public class CustomReportStorageWebExtension : ReportStorageWebExtension {
    readonly string reportDirectory;
    const string Extension = ".repx";

    public CustomReportStorageWebExtension(IWebHostEnvironment env) {
        reportDirectory = Path.Combine(env.ContentRootPath, "Reports");
        Directory.CreateDirectory(reportDirectory);
    }

    // Called to check whether a URL can be used to identify a report
    public override bool IsValidUrl(string url) =>
        !Path.IsPathRooted(url) && !url.Contains("..");

    // Called before SetData/SetNewData — return false to prevent save
    public override bool CanSetData(string url) => true;

    // Load report by URL — returns REPX bytes
    public override byte[] GetData(string url) {
        // First try file on disk
        var filePath = Path.Combine(reportDirectory, url + Extension);
        if (File.Exists(filePath))
            return File.ReadAllBytes(filePath);
        // Then try resolving a predefined report instance
        var report = ResolveReport(url);
        if (report != null) {
            using var stream = new MemoryStream();
            report.SaveLayoutToXml(stream);
            return stream.ToArray();
        }
        throw new InvalidOperationException($"Report '{url}' not found.");
    }

    // Return all URLs visible in the designer's Open dialog
    public override Dictionary<string, string> GetUrls() {
        var files = Directory.GetFiles(reportDirectory, "*" + Extension);
        return files.ToDictionary(
            f => Path.GetFileNameWithoutExtension(f),
            f => Path.GetFileNameWithoutExtension(f));
    }

    // Save an existing report
    public override void SetData(XtraReport report, string url) {
        report.SaveLayoutToXml(Path.Combine(reportDirectory, url + Extension));
    }

    // Save a new report — return the assigned URL
    public override string SetNewData(XtraReport report, string defaultUrl) {
        var url = string.IsNullOrEmpty(defaultUrl) ? Guid.NewGuid().ToString() : defaultUrl;
        SetData(report, url);
        return url;
    }

    XtraReport? ResolveReport(string name) => name switch {
        "SampleReport" => new SampleReport(),
        _ => null
    };
}
```

```csharp
// Program.cs — register AFTER AddDevExpressControls
using DevExpress.XtraReports.Web.Extensions;

builder.Services.AddScoped<ReportStorageWebExtension, CustomReportStorageWebExtension>();
```

> **Important**: Register `ReportStorageWebExtension` **after** `AddDevExpressControls()`. Registering before causes the built-in no-op storage to take precedence.

### GetData must resolve every URL used in Bind()

If you call `.Bind("ReportName")` in the view, `GetData("ReportName")` must return valid REPX bytes. A common pattern is to seed predefined reports from assembled instances when no `.repx` file exists on disk (see `ResolveReport` above).

## Binding: String vs. Instance

| Pattern | Code | When to use |
|---------|------|------------|
| Instance binding | `Bind(new SalesReport())` | Quickstart; no storage needed; report cannot be saved by users |
| String binding | `Bind("SalesReport")` | Storage-backed; enables designer Open/Save; requires `GetData("SalesReport")` to resolve |

For instance binding with designer: the report opens in the designer but Save As writes via `SetNewData`. The user cannot reload the original instance — use string binding for full round-trip support.

## Web Farm / Multi-Server Setup

For applications running on multiple servers or processes, use shared file storage:

```csharp
builder.Services.ConfigureReportingServices(configurator => {
    configurator.ConfigureWebDocumentViewer(c => {
        c.UseCachedReportSourceBuilder();
        // Shared storage — all servers read/write the same directory
        c.UseFileDocumentStorage(@"\\server\shared\documents",
            StorageSynchronizationMode.InterProcess);
        c.UseFileReportStorage(@"\\server\shared\reports",
            StorageSynchronizationMode.InterProcess);
        c.UseFileExportedDocumentStorage(@"\\server\shared\exports",
            StorageSynchronizationMode.InterProcess);
    });
});
```

Also configure ASP.NET Core Data Protection to share keys across servers, and set a consistent `validationKey`/`decryptionKey` for the Report Designer's machine key.

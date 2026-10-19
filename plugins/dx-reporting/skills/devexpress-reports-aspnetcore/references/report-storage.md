# Report Storage — ASP.NET Core

## When to Use This Reference

Use this when you need to:
- Enable Save, Load, Open, Save As in the End-User Report Designer
- Resolve a report name string to an `XtraReport` instance (e.g., `Bind("SalesReport")`)
- Choose between `IReportProvider`, `IReportProviderAsync`, and `ReportStorageWebExtension`
- Implement and register a report name resolution service for a viewer or designer component
- Understand mutual exclusivity and registration order rules
- List available reports in the designer's Open dialog
- Register custom data source classes as trusted for report deserialization
- Diagnose why designer-saved reports don't show up in the Viewer (or vice versa)
- Configure report/document storage and caching for a web farm or web garden (multi-server) deployment
- Route storage (through a database or the file system), tune or disable the background storage cleaner, or switch the components to the asynchronous engine

## Overview

ASP.NET Core Reporting components (Document Viewer, Report Designer, or Parameter Panel) can be bound to either a report instance or a report name string:

| Pattern | Code | When to use |
|---------|------|------------|
| Report Instance binding | `Bind(new SalesReport())` | Quickstart; no storage needed; report cannot be saved by users |
| Report Name string binding | `Bind("SalesReport")` | Storage-backed; enables designer Open/Save; requires `GetData("SalesReport")` to resolve |

When bound to a report instance (`Bind(new SalesReport())`), the component uses that instance directly. In this case, no additional service is needed — the Viewer or Designer receives the instance directly and never needs to resolve a name. Use this when the app only ever shows one (or a small fixed set of, individually bound) report and you don't need Save/Save As/Open by name.

When bound to a report name string (`Bind("SalesReport")`), the component calls a registered service to resolve that name to an `XtraReport` instance — this supports save/load round-trips. 

### Choosing the Right Service:

| Service | Use When | Designer support |
|---------|---------|-----------------|
| `IReportProvider` / `IReportProviderAsync` | Viewer / Parameters Panel; map name → `XtraReport` | No (viewer only) |
| `ReportStorageWebExtension` | Designer present; full CRUD (load/save/list) | Yes |

> **Rule**: If the End-User Report Designer is integrated, use `ReportStorageWebExtension` — it supports all designer file operations. `IReportProvider` / `IReportProviderAsync` are for viewer-only scenarios.

> **Legacy service — `IWebDocumentViewerReportResolver`**: a sync-only, viewer-only name resolver kept for backward compatibility. If registered, it takes priority over `IReportProvider`/`ReportStorageWebExtension`. Don't register it in new code; use the other services instead. Recognize it only if you find one already registered in an existing app.

> **Conflict warning — do not register both**: `IReportProvider`/ `IReportProviderAsync` and `ReportStorageWebExtension` both solve the same task (resolve a report by name) through different mechanisms. If both are registered, `IReportProvider` has priority over `ReportStorageWebExtension` for loading reports, but the `ReportStorageWebExtension` may still handle save operations. **Symptom**: reports saved via the End-User Report Designer never reappear when reopened through the Viewer — no exception, just a stale, missing, outdated, or blank result. **Fix**: pick one service based on the table above and remove the other registration entirely; never run both side by side.

## IReportProvider

Recommended for Document Viewer when reports are created at runtime or loaded from REPX files.

### Implementation — in-memory report instances

```csharp
// Services/MyReportProvider.cs
using DevExpress.XtraReports.Services;
using DevExpress.XtraReports.UI;

public class MyReportProvider : IReportProvider {
    public XtraReport GetReport(string reportName, ReportProviderContext context) {
        if (string.IsNullOrEmpty(reportName))
            return null;
        return reportName switch {
            "SalesReport"   => new SalesReport(),
            "InvoiceReport" => new InvoiceReport(),
            _ => new XtraReport() // unknown name — return a blank report, don't throw
        };
    }
}
```

### Implementation — loading from REPX files

```csharp
public class MyReportProvider : IReportProvider {
    public XtraReport GetReport(string reportName, ReportProviderContext context) {
        if (string.IsNullOrEmpty(reportName))
            return null;
        string reportFolder = "Reports";
        if (Directory.EnumerateFiles(reportFolder)
                .Select(Path.GetFileNameWithoutExtension).Contains(reportName)) {
            byte[] reportBytes = File.ReadAllBytes(Path.Combine(reportFolder, reportName + ".repx"));
            using var ms = new MemoryStream(reportBytes);
            // Use XtraReport.FromXmlStream — NOT new XtraReport().LoadLayoutFromXml()
            // The static method preserves the concrete report subclass stored in the XML.
            return XtraReport.FromXmlStream(ms);
        }
        return new XtraReport(); // unknown name — return a blank report, don't throw
    }
}
```

### Registration in Program.cs

```csharp
// Program.cs — Must come AFTER AddDevExpressControls()
builder.Services.AddScoped<IReportProvider, MyReportProvider>();
```

## IReportProviderAsync

Use with asynchronous engine mode (`UseAsyncEngine()`) or for resolving subreport names (`XRSubreport.ReportSourceUrl`) asynchronously.

### Implementation — loading from REPX files

```csharp
// Services/MyReportProviderAsync.cs
using DevExpress.XtraReports.Services;
using DevExpress.XtraReports.UI;
using Microsoft.AspNetCore.Hosting;

public class MyReportProviderAsync : IReportProviderAsync {
    private readonly IWebHostEnvironment env;

    public MyReportProviderAsync(IWebHostEnvironment env) {
        this.env = env;
    }

    public async Task<XtraReport> GetReportAsync(string id, ReportProviderContext context) {
        if (string.IsNullOrEmpty(id)) return null;

        var reportPath = Path.Combine(env.ContentRootPath, "Reports", id + ".repx");
        if (!File.Exists(reportPath))
            return new XtraReport(); // unknown name — return a blank report, don't throw

        var bytes = await File.ReadAllBytesAsync(reportPath);
        using var ms = new MemoryStream(bytes);
        return XtraReport.FromXmlStream(ms);
    }
}
```

### Registration in Program.cs

```csharp
builder.Services.ConfigureReportingServices(configurator => {
    configurator.UseAsyncEngine();
});
// Program.cs — Must come AFTER AddDevExpressControls()
builder.Services.AddScoped<IReportProviderAsync, MyReportProviderAsync>();
```

> **`UseAsyncEngine()`** (on `ReportingConfigurationBuilder`) switches the reporting components to asynchronous report resolution and export — `IReportProviderAsync` and async export-result URI generation. Call it **once** if any `*Async` service variant is registered anywhere in the app: registering an async variant without it means the components keep using the synchronous path and the async service is silently ignored.

## ReportStorageWebExtension (Viewer + Designer)

Required when the Report Designer is in the project — its Save, Save As, and Open dialogs depend on this service. Also used as a fallback viewer resolver when `IReportProvider` is not registered.

### Implementation — File-Based Report Storage

Implement all six methods. In `GetData`, throw `DevExpress.XtraReports.Web.ClientControls.FaultException` when a report cannot be resolved — the Viewer/Designer surfaces its `Message` property to the end user.

> **Optional Dependency:** Two lines below are marked `// Requires ReportFactory`. For file-based reports only, remove **both** — leaving one in without implementing `ReportFactory` breaks compilation. To also serve built-in report classes, keep both and implement `ReportFactory` from the section below.

```csharp
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using DevExpress.XtraReports.Web.ClientControls;
using DevExpress.XtraReports.Web.Extensions;
using DevExpress.XtraReports.UI;

public class CustomReportStorageWebExtension : DevExpress.XtraReports.Web.Extensions.ReportStorageWebExtension
{
    readonly string reportDirectory;
    const string FileExtension = ".repx";
    public CustomReportStorageWebExtension() {
        reportDirectory = Path.Combine(AppContext.BaseDirectory, "Reports");
        if (!Directory.Exists(reportDirectory)) {
            Directory.CreateDirectory(reportDirectory);
        }
    }

    private bool IsWithinReportsFolder(string url, string folder) {
        var rootDirectory = new DirectoryInfo(folder);
        var fileInfo = new FileInfo(Path.Combine(folder, url));
        return fileInfo.Directory.FullName.ToLower().StartsWith(rootDirectory.FullName.ToLower());
    }

    public override bool CanSetData(string url) {
        return true;
    }

    public override bool IsValidUrl(string url) {
        return Path.GetFileName(url) == url;
    }

    public override byte[] GetData(string url) {
        try {
            var reportPath = Path.Combine(reportDirectory, url + FileExtension);
            if (File.Exists(reportPath))
            {
                return File.ReadAllBytes(reportPath);
            }
            // Requires ReportFactory (see "Report Factory for Built-In Reports (Optional)" section below)
            if (ReportFactory.Reports.ContainsKey(url))
            {
                using (MemoryStream ms = new MemoryStream()) {
                    ReportFactory.Reports[url]().SaveLayoutToXml(ms);
                    return ms.ToArray();
                }
            }
        } catch (Exception) {
            throw new FaultException("Could not get report data.");
        }
        throw new FaultException($"Could not find report '{url}'.");
    }

    public override Dictionary<string, string> GetUrls() {
        return Directory.GetFiles(reportDirectory, "*" + FileExtension)
                                    .Select(Path.GetFileNameWithoutExtension)
                                    // Requires ReportFactory (see "Report Factory for Built-In Reports (Optional)" section below)
                                    .Union(ReportFactory.Reports.Select(x => x.Key))
                                    .ToDictionary<string, string>(x => x);
    }

    public override void SetData(XtraReport report, string url) {
        if(!IsWithinReportsFolder(url, reportDirectory))
            throw new InvalidOperationException("Invalid report name.");
        report.SaveLayoutToXml(Path.Combine(reportDirectory, url + FileExtension));
    }

    public override string SetNewData(XtraReport report, string defaultUrl) {
        if (string.IsNullOrWhiteSpace(defaultUrl)) 
            throw new DevExpress.XtraReports.Web.ClientControls.FaultException("An invalid report name was provided. It should not be empty or whitespace.");
        SetData(report, defaultUrl);
        return defaultUrl;
    }
}
```

### Registration in Program.cs

```csharp
using DevExpress.XtraReports.Web.Extensions;

// Program.cs - Must come AFTER AddDevExpressControls()
builder.Services.AddScoped<ReportStorageWebExtension, CustomReportStorageWebExtension>();
```

> **Important**: Register `ReportStorageWebExtension` **after** `AddDevExpressControls()`. Registering before causes the built-in no-op storage to take precedence.

### GetData must resolve every URL used in Bind()

If you call `.Bind("ReportName")` in the view, `GetData("ReportName")` must return valid REPX bytes. A common pattern is to seed predefined reports from assembled instances when no `.repx` file exists on disk — see the `ReportFactory` block below.

### Report Factory for Built-In Reports (Optional)

It is possible to add a `ReportFactory` to serve built-in report instances (reports included in the project as classes):

```csharp
// ReportFactory.cs
using DevExpress.XtraReports.UI;

public static class ReportFactory {
    public static Dictionary<string, Func<XtraReport>> Reports = new() {
        ["TestReport"] = () => new TestReport()
    };
}
```

The storage's `GetData` method should check both the file directory and `ReportFactory.Reports`.

### Verifying storage from the browser console

A Designer with **no storage registered** renders, logs no console error, and passes `Verify-ReportingPage.ps1` (exit 0) — so check storage separately, from any MCP that can evaluate JavaScript.

The component is a global named after the helper: `ReportDesigner("reportDesigner")` → `window.reportDesigner`, a [`JSReportDesigner`](https://docs.devexpress.com/XtraReports/js-DevExpress.Reporting.Designer.JSReportDesigner). Keep the probe **read-only**: never test storage by saving a report. 

```js
const d = window.reportDesigner;   // the verifier prints the actual name
const t = (p, ms) => new Promise(res => { let done = false; const f = v => { if (!done) { done = true; res(v); } };
  setTimeout(() => f({ ok: false, outcome: 'TIMEOUT - storage unreachable' }), ms);
  p && p.done ? (p.done(v => f({ ok: true, value: v })), p.fail && p.fail(e => f({ ok: false, outcome: 'FAILED', error: String(e) })))
              : Promise.resolve(p).then(v => f({ ok: true, value: v }), e => f({ ok: false, outcome: 'FAILED', error: String(e) })); });
const tabs = d.GetTabs();
const urls = await t(d.ReportStorageGetUrls(), 6000);
return { boundByName: !!(tabs[0] && tabs[0].url()),        // false => instance-bound, not storage-bound
         reportCount: urls.ok ? urls.value.length : urls.outcome };
// PASS: boundByName === true AND reportCount > 0
```

## Web Farm / Multi-Server Setup

A **web farm** (multiple nodes behind a load balancer) or **web garden** (multiple worker processes on one machine) breaks the Viewer/Designer's default in-memory, per-process document/report cache: a request routed to a different node/process can't find the in-progress document. Fix it one of two ways — don't mix them:

1. **ARR affinity (sticky sessions)** — the load balancer pins each client to one node/process. No code, but it defeats load balancing and doesn't survive a node recycle mid-session.
2. **Shared cache/storage across nodes** — share state so any node can serve any request. Use shared file storage in `InterProcess` mode:

```csharp
configurator.ConfigureWebDocumentViewer(c => {
    c.UseCachedReportSourceBuilder();
    c.UseFileDocumentStorage(@"\\server\shared\documents", StorageSynchronizationMode.InterProcess);
    c.UseFileReportStorage(@"\\server\shared\reports", StorageSynchronizationMode.InterProcess);
    c.UseFileExportedDocumentStorage(@"\\server\shared\exports", StorageSynchronizationMode.InterProcess); // only if async export / content editing is used
});
```

Or replace shared files with `c.UseDistributedCache()` (Redis, NCache, SQL Server, … via `IDistributedCache`).

A web farm also needs **shared ASP.NET Core Data Protection keys** and a **shared machine key** (`validationKey`/`decryptionKey`) across all nodes, or a payload encrypted on one node fails to decrypt on another. Two escape hatches avoid the shared secrets: implement `ISecureDataConverter`/`IDataSourceProtectionService` to supply your own connection-string encryption ([security.md](security.md#protecting-connection-strings-sent-to-the-client)), and implement `IConnectionProviderFactory` so reports reference connections by **name** (named connections need no encryption, hence no shared machine key).

Full guidance: [Web Farm and Web Garden Support](https://docs.devexpress.com/XtraReports/5199).

## Database-Backed Storage and Cleanup Tuning

Beyond the file-based and distributed-cache options above, `WebDocumentViewerConfigurationBuilder` exposes:

| Method | Purpose |
|---|---|
| `UseDbStorage(string connectionStringName)` | Routes report, document, and exported-document storage through a **database** (via the named connection string) instead of the file system — the alternative to the three `UseFile*Storage` calls for environments with no shared file path but with a shared database. |
| `UseEmptyStoragesCleaner()` | Replaces the background job that periodically purges stale report/document storage entries with a **no-op** cleaner. Use when storage lifetime is managed externally (your own retention policy or infrastructure-level cleanup) instead of by the built-in cleaner. |
| `DisableCachedDocumentSource()` | Turns off the Cached Document Source component — the Viewer regenerates the document on every request instead of reusing a cached one. This removes the performance benefit `UseCachedReportSourceBuilder()` provides; only disable it if caching itself causes a problem (e.g., data that must be fresh on every request). |

To tune *how aggressively* the built-in cleaner runs rather than replacing it, register these via plain DI:

| Service | Why / When |
|---|---|
| `StorageCleanerSettings` | Controls how long stale entries are retained in the report/document **storage** before the background cleaner purges them. Adjust when the defaults retain data too long, or too briefly, for your traffic pattern. |
| `CacheCleanerSettings` | Same idea, for the in-memory/distributed **cache** rather than storage. |

## Data Source Trust Registration — Custom Data Sources

When a report's data source is a custom class (not a built-in .NET type) — e.g., bound via `ObjectDataSource`, a `[DataSource]` attribute, or a `List<T>`/`IList` property — register the type as trusted **before** deserialization runs, or loading the report throws `NonTrustedTypeDeserializationException`. This applies to:

- The Report Designer loading, saving, or previewing a report with a custom data source
- A Viewer loading a report from `ReportStorageWebExtension` or an `IReportProvider` that returns REPX bytes containing a custom data source

```csharp
// Program.cs — after the usings, before var builder = WebApplication.CreateBuilder(args);
DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(SalesData));
```

> **Rules**:
> - Prefer `RegisterTrustedClass(typeof(T))` per concrete type over `RegisterTrustedAssembly()` — the latter trusts every type in the assembly, which is broader than necessary.
> - Registration must run before `WebApplication.CreateBuilder(args)` is called; registering later can miss reports deserialized during startup or early requests.


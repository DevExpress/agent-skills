# Report Name Resolution Services

## When to Use This Reference

Use this when you need to:
- Resolve a report name string to an `XtraReport` instance
- Choose between `IReportProvider`, `IReportProviderAsync`, and `ReportStorageWebExtension`
- Implement and register a report name resolution service for a viewer or designer component
- Understand mutual exclusivity and registration order rules
- Determine whether to use **server-side** or **client-side (WebAssembly)** resolution patterns

## Overview

When a Blazor reporting component (`DxDocumentViewer`, `DxReportDesigner`, `DxReportViewer`, `DxWasmDocumentViewer`, `DxWasmReportDesigner`) receives a **report name string** rather than a report instance, it calls a registered service to resolve that name to an `XtraReport`.

**This reference covers two scenarios:**
- **Server-Side Scenarios** — For Interactive Server, and Interactive WebAssembly (Client + Server) with an ASP.NET Core backend
- **Client-Side WebAssembly Scenarios** — For Interactive WebAssembly Standalone apps (browser-only, no backend)

Choose the section that matches your hosting model. The implementation patterns differ significantly due to I/O constraints in Interactive WebAssembly Standalone (all I/O must be async via `HttpClient`).

---

## Server-Side Scenarios

These patterns apply to **Interactive Server** and **Interactive WebAssembly (Client + Server)** projects where you have access to the server file system or backend APIs.

| Service | Recommended for | Notes |
|---------|-----------------|-------|
| `IReportProvider` | Viewers, Designer (read-only) | Sync. Best for runtime-created reports and REPX files. |
| `IReportProviderAsync` | Viewers + async engine | Async variant of `IReportProvider`. Requires `UseAsyncEngine()`. |
| `ReportStorageWebExtension` | Designer (Save/Load/Open) | Manages serialized REPX from external storage. Required for `DxReportDesigner`. |

### IReportProvider

Recommended for `DxDocumentViewer` when reports are created at runtime or loaded from REPX files.

### Implementation — in-memory report instances

```csharp
// Services/MyReportProvider.cs
using DevExpress.XtraReports.Services;
using DevExpress.XtraReports.UI;

public class MyReportProvider : IReportProvider {
    public XtraReport GetReport(string reportName, ReportProviderContext context) {
        return reportName switch {
            "SalesReport"   => new SalesReport(),
            "InvoiceReport" => new InvoiceReport(),
            _ => throw new InvalidOperationException($"Report '{reportName}' not found.")
        };
    }
}
```

### Implementation — loading from REPX files

```csharp
public class MyReportProvider : IReportProvider {
    public XtraReport GetReport(string reportName, ReportProviderContext context) {
        string reportFolder = "Reports";
        if (Directory.EnumerateFiles(reportFolder)
                .Select(Path.GetFileNameWithoutExtension).Contains(reportName)) {
            byte[] reportBytes = File.ReadAllBytes(Path.Combine(reportFolder, reportName + ".repx"));
            using var ms = new MemoryStream(reportBytes);
            // Use XtraReport.FromXmlStream — NOT new XtraReport().LoadLayoutFromXml()
            // The static method preserves the concrete report subclass stored in the XML.
            return XtraReport.FromXmlStream(ms);
        }
        throw new InvalidOperationException($"Report '{reportName}' not found.");
    }
}
```

### Registration in Program.cs

```csharp
// Must come AFTER AddDevExpressBlazorReporting() or AddDevExpressServerSideBlazorReportViewer()
builder.Services.AddScoped<IReportProvider, MyReportProvider>();
```

> **Calling `GetReport` directly** (e.g., from a page's `OnInitialized`): pass `null!` for the context parameter — it is unused in simple name-resolution scenarios:
> ```csharp
> report = ReportProvider.GetReport("SalesReport", null!);
> ```

## IReportProviderAsync

Use with asynchronous engine mode (`UseAsyncEngine()`) or for resolving subreport names (`XRSubreport.ReportSourceUrl`) asynchronously.

### Implementation — loading from REPX files

```csharp
// Services/MyReportProviderAsync.cs
using DevExpress.XtraReports.Services;
using DevExpress.XtraReports.UI;
using Microsoft.AspNetCore.Hosting;

public class MyReportProviderAsync : IReportProviderAsync {
    private readonly IWebHostEnvironment _env;

    public MyReportProviderAsync(IWebHostEnvironment env) {
        _env = env;
    }

    public async Task<XtraReport> GetReportAsync(string id, ReportProviderContext context) {
        if (string.IsNullOrEmpty(id)) return null;

        var reportPath = Path.Combine(_env.ContentRootPath, "Reports", id + ".repx");
        if (!File.Exists(reportPath))
            throw new InvalidOperationException($"Report '{id}' not found.");

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
// Must come AFTER AddDevExpressBlazorReporting()
builder.Services.AddScoped<IReportProviderAsync, MyReportProviderAsync>();
```

## ReportStorageWebExtension

Required when `DxReportDesigner` is in the project — its Save, Save As, and Open dialogs depend on this service. Also used as a fallback viewer resolver when `IReportProvider` is not registered.

### Implementation — File-Based Report Storage

> **Optional Dependency:** The implementation below includes references to `ReportFactory.Reports` (to serve built-in report classes alongside file-based reports). If you only need file-based reports, omit the `ReportFactory` checks. If you need built-in reports, implement the `ReportFactory` class defined in the **"Report Factory for Built-In Reports (Optional)"** section below.

```csharp
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using DevExpress.XtraReports.Web.Extensions;
using DevExpress.XtraReports.UI;

public class CustomReportStorageWebExtension : DevExpress.XtraReports.Web.Extensions.ReportStorageWebExtension
{
    readonly string reportDirectory;
    const string FileExtension = ".repx";
    public CustomReportStorageWebExtension(string reportDirectory) {
        if (!Directory.Exists(reportDirectory)) {
            Directory.CreateDirectory(reportDirectory);
        }
        this.reportDirectory = reportDirectory;
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
            throw new InvalidOperationException("Could not get report data.");
        }
        throw new InvalidOperationException($"Could not find report '{url}'.");
    }

    public override Dictionary<string, string> GetUrls() {
        return Directory.GetFiles(reportDirectory, "*" + FileExtension)
                                    .Select(Path.GetFileNameWithoutExtension)
                                    .Union(ReportFactory.Reports.Select(x => x.Key))
                                    .ToDictionary<string, string>(x => x);
    }

    public override void SetData(XtraReport report, string url) {
        if(!IsWithinReportsFolder(url, reportDirectory))
            throw new InvalidOperationException("Invalid report name.");
        report.SaveLayoutToXml(Path.Combine(reportDirectory, url + FileExtension));
    }

    public override string SetNewData(XtraReport report, string defaultUrl) {
        SetData(report, defaultUrl);
        return defaultUrl;
    }
}
```

### Registration in Program.cs

```csharp
// Must come AFTER AddDevExpressBlazorReporting()
builder.Services.AddScoped<ReportStorageWebExtension, CustomReportStorageWebExtension>();
```

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

### Security: Trusted Types for Custom Data Source Deserialization

**Critical when deserializing reports with custom data sources:** When a Report Designer or Viewer deserializes reports from storage (during preview, save, load, or viewer storage access), DevExpress validates the report XML against a trusted-types security list. **You must register custom data source classes, not the report class itself.**

> **Key Clarification**: Only **data source classes** need registration for deserialization. The critical XML contains the data source type names (e.g., `SalesData`, `CustomerRecord`).
>
> **Scope of this issue**:
> - **Designer components** (`DxReportDesigner`, `DxWasmReportDesigner`) — Deserialize when saving/loading/previewing reports with custom data sources
> - **Viewer components** (`DxDocumentViewer`, `DxWasmDocumentViewer`) — Deserialize when loading reports from storage via `ReportStorageWebExtension` or `IReportProvider` that returns REPX files
> - **Subreports** — Also require registration if they use custom data sources

#### When Trust Registration Is Required

- ✅ **Always for designer components**: If you have `DxReportDesigner` or `DxWasmReportDesigner` and reports use custom data sources
- ✅ **Always for viewer + storage**: If you have `DxDocumentViewer` or `DxWasmDocumentViewer` with `ReportStorageWebExtension` or `IReportProvider` that loads REPX files containing custom data sources
- ✅ **Always for custom object data sources**: If your `ObjectDataSource` uses a custom class like `SalesData` or `CustomerDto`
- ✅ **For subreports with custom data sources**: If subreports bind to custom types
- ❌ **Not needed for SQL/JSON**: Built-in `SqlDataSource` and `JsonDataSource` do not require registration
- ❌ **Not needed for viewer-only with in-memory reports**: If `DxReportViewer` (native) or `DxDocumentViewer` loads report instances created in C# (not deserialized from storage), no registration is needed

#### Step-by-Step Registration

**1. Identify data source classes used in your reports:**

Search your report class definitions for:
- `[DataSource]` attributes
- `ObjectDataSource` properties
- `List<T>` or `IList<T>` properties with custom types

Example report:
```csharp
public class SalesReport : XtraReport {
    public SalesReport() {
        // Data source is SalesData — this class needs registration
        var detailBand = new DetailBand();
        var table = new XRTable();
        
        // Example: binding to List<SalesData>
        ObjectDataSource dataSource = new ObjectDataSource();
        dataSource.DataSource = typeof(SalesData);  // ← Register this type
    }
}
```

**2. Register the class at the top of Program.cs — BEFORE `WebApplication.CreateBuilder()`:**

```csharp
using DevExpress.Utils;

// ✅ CORRECT: Register data source classes
DeserializationSettings.RegisterTrustedClass(typeof(SalesData));

// Repeat for each custom data source class
DeserializationSettings.RegisterTrustedClass(typeof(CustomerDto));

var builder = WebApplication.CreateBuilder(args);
```


#### Best Practice: RegisterTrustedClass vs RegisterTrustedAssembly

```csharp
// ✅ GOOD: Register specific types
DeserializationSettings.RegisterTrustedClass(typeof(SalesData));

// ⚠ AVOID: RegisterTrustedAssembly trusts every type in the assembly — too broad
// DeserializationSettings.RegisterTrustedAssembly(typeof(SalesData).Assembly);
```

Prefer per-type registration (`RegisterTrustedClass`) — it trusts only what you explicitly need. Assembly-level registration is a security anti-pattern (see **Antipattern A8**).

#### Full Example: Multi-Class Report with Custom Data Sources

```csharp
// Program.cs — top of file, before builder creation
using DevExpress.Utils;

// Register all custom data source classes used across all reports
DeserializationSettings.RegisterTrustedClass(typeof(SalesData));

DeserializationSettings.RegisterTrustedClass(typeof(EmployeeRecord));

DeserializationSettings.RegisterTrustedClass(typeof(ProductInfo));

var builder = WebApplication.CreateBuilder(args);

// ... rest of Program.cs
```

See **Antipatterns A7 & A8** for common mistakes and **Troubleshooting** in each getting-started file for specific error symptoms.

---

## Client-Side WebAssembly Scenarios

These patterns apply to **Standalone WebAssembly** apps (browser-only, no ASP.NET Core backend). In this architecture, all I/O must be async and performed via `HttpClient` to fetch resources from `wwwroot/` or a remote API.

| Service | Read Reports | Save Reports | Best For |
|---------|--|--|----------|
| **`IReportProviderAsync`** | ✅ Yes | ❌ No (view-only) | Viewing and editing existing reports; no persistence needed |
| **`ReportStorageWebExtension` (async with HttpClient)** | ✅ Yes | ✅ Yes (via backend API) | Full designer with Save, Save As, Open dialogs |

### IReportProviderAsync (Interactive WebAssembly Standalone)

Use `IReportProviderAsync` when you only need to **view and edit** existing reports without persisting changes to storage. All I/O in Interactive WebAssembly Standalone must be async (synchronous `IReportProvider` is not supported).

**`Services/CustomReportProvider.cs`:**

```csharp
using DevExpress.XtraReports.Services;
using DevExpress.XtraReports.UI;

public class CustomReportProvider : IReportProviderAsync {
    private readonly HttpClient _httpClient;

    public CustomReportProvider(HttpClient httpClient) {
        _httpClient = httpClient;
    }
    
    public Task<XtraReport> GetReportAsync(string id, ReportProviderContext context) {
        return ReportsFactory.GetReport(id, _httpClient);
    }
}
```

**`Services/ReportsFactory.cs`:**

```csharp
using DevExpress.XtraReports.UI;

public static class ReportsFactory {
    public static readonly Dictionary<string, XtraReport> Reports = new() {
        ["SalesReport"] = new SalesReport(),
        ["InvoiceReport"] = new InvoiceReport()
    };

    public static async Task<XtraReport> GetReport(string reportName, HttpClient httpClient) {
        // Try built-in reports first
        if (Reports.ContainsKey(reportName)) {
            return Reports[reportName];
        }
        
        // Fall back to loading from wwwroot/reports/
        try {
            var reportBytes = await httpClient.GetByteArrayAsync($"reports/{reportName}.repx");
            using var stream = new MemoryStream(reportBytes);
            return XtraReport.FromXmlStream(stream);
        } catch (Exception ex) {
            throw new InvalidOperationException($"Report '{reportName}' not found.", ex);
        }
    }
}
```

**Registration in Program.cs:**

```csharp
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using DevExpress.Blazor.Reporting;
using DevExpress.XtraReports.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient {
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
});

// Register WASM reporting services — NOT AddDevExpressBlazorReporting():
builder.Services.AddDevExpressBlazorReportingWebAssembly(configure => {
    configure.UseDevelopmentMode();
});

// Register IReportProviderAsync AFTER AddDevExpressBlazorReportingWebAssembly()
builder.Services.AddScoped<IReportProviderAsync, CustomReportProvider>();

await builder.Build().RunAsync();
```

> Store `.repx` files in the `wwwroot/reports/` folder so they are served as static assets.

### ReportStorageWebExtension (Interactive WebAssembly Standalone with Backend API)

Use `ReportStorageWebExtension` when you need **Save, Save As, and Open** dialogs in the designer. This requires a backend API endpoint to persist report definitions since Interactive WebAssembly Standalone cannot write to the server file system directly.

**`Services/CustomReportStorageWebExtension.cs`:**

```csharp
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Web.Extensions;

public class CustomReportStorageWebExtension : ReportStorageWebExtension {
    private readonly HttpClient _httpClient;
    private const string FileExtension = ".repx";
    private const string ReportsPath = "reports/";
    private const string BackendApiUrl = "api/reports"; // Backend endpoint

    public CustomReportStorageWebExtension(HttpClient httpClient) {
        _httpClient = httpClient;
    }

    public override bool IsValidUrl(string url) {
        // Validate report name — allow only filename without path separators
        return Path.GetFileName(url) == url;
    }

    public override async Task<Dictionary<string, string>> GetUrlsAsync() {
        try {
            // Fetch list of available reports from backend
            var response = await _httpClient.GetAsync($"{BackendApiUrl}/list");
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            
            // Parse JSON and return dictionary of report names
            // Example response: {"reports": ["SalesReport", "InvoiceReport"]}
            var reports = new Dictionary<string, string> {
                ["SalesReport"] = "Sales Report",
                ["InvoiceReport"] = "Invoice Report"
            };
            return reports;
        } catch (Exception ex) {
            throw new InvalidOperationException("Could not fetch report list.", ex);
        }
    }

    public override async Task<byte[]> GetDataAsync(string url) {
        try {
            // Load report from backend or wwwroot/reports
            var reportPath = $"{ReportsPath}{url}{FileExtension}";
            return await _httpClient.GetByteArrayAsync(reportPath);
        } catch (Exception ex) {
            throw new InvalidOperationException($"Could not get report data for '{url}'.", ex);
        }
    }

    public override async Task SetDataAsync(XtraReport report, string url) {
        try {
            // Save report to backend via multipart form upload
            using (var stream = new MemoryStream()) {
                report.SaveLayoutToXml(stream);
                stream.Position = 0;

                using (var content = new MultipartFormDataContent()) {
                    content.Add(new StreamContent(stream), "file", $"{url}{FileExtension}");
                    var response = await _httpClient.PostAsync($"{BackendApiUrl}/save", content);
                    response.EnsureSuccessStatusCode();
                }
            }
        } catch (Exception ex) {
            throw new InvalidOperationException($"Could not save report '{url}'.", ex);
        }
    }

    public override async Task<string> SetNewDataAsync(XtraReport report, string defaultUrl) {
        // Validate and sanitize the URL
        if (!IsValidUrl(defaultUrl)) {
            throw new InvalidOperationException($"Invalid report name: '{defaultUrl}'.");
        }
        
        await SetDataAsync(report, defaultUrl);
        return defaultUrl;
    }
}
```

> The `CustomReportStorageWebExtension` implementation expects the following API endpoints in your ASP.NET Core backend project:
> - `GET api/reports/list` — returns a JSON list of available report names
> - `GET api/reports/{reportName}` — returns the report definition as a byte array
> - `POST api/reports/save` — accepts multipart form data with the report definition

**Registration in Program.cs:**

```csharp
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using DevExpress.Blazor.Reporting;
using DevExpress.DataAccess.Web;
using DevExpress.XtraReports.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { 
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) 
});

// Register WASM reporting services — NOT AddDevExpressBlazorReporting():
builder.Services.AddDevExpressBlazorReportingWebAssembly(configure => {
    configure.UseDevelopmentMode(); 
});

// Register ReportStorageWebExtension AFTER AddDevExpressBlazorReportingWebAssembly()
builder.Services.AddScoped<ReportStorageWebExtension, CustomReportStorageWebExtension>();

await builder.Build().RunAsync();
```

**Critical Constraints for WASM Storage:**

1. **All methods must be async**: Replace synchronous overrides (`GetData`, `SetData`, `GetUrls`) with async variants (`GetDataAsync`, `SetDataAsync`, `GetUrlsAsync`)
2. **No blocking calls**: Never use `.Result`, `.Wait()`, or `.GetAwaiter().GetResult()` — Interactive WebAssembly Standalone runs on a single thread; blocking deadlocks the UI
3. **Use `await` throughout**: All I/O operations must be fully async
4. **Backend API is required**: You cannot directly access the server file system; all persistence must go through HTTP API endpoints

---

## Restoring a Report Instance from XML Bytes

When reading a report back from a byte array (e.g., from `ReportStorageWebExtension.GetData` or a viewer page in a mixed project), use the static factory:

```csharp
var bytes = reportStorage.GetData("MyReport");
using var stream = new MemoryStream(bytes);

// Correct (Antipattern A6 fix): preserves the concrete report subclass stored in the XML
XtraReport report = XtraReport.FromXmlStream(stream);
```

See **Antipattern A6** for details about why `FromXmlStream()` is required.

## Critical Constraints — Report Name Resolution

### Universal Constraints (All Scenarios)

1. **`IReportProvider` and `ReportStorageWebExtension` are mutually exclusive**: Both services solve the same problem — resolving a report by name. Do **not** register both in the same project. For native `DxReportViewer` pages in the same project, load the report instance directly from storage: `var bytes = storageExt.GetData("name"); report = XtraReport.FromXmlStream(new MemoryStream(bytes));`

2. **Use `XtraReport.FromXmlStream()` not `LoadLayoutFromXml()`**: When restoring a report instance from XML bytes or a stream, always use the static factory `XtraReport.FromXmlStream(stream)`. The static method preserves the concrete report subclass stored in the XML; the instance method always returns a base `XtraReport` and loses custom properties.

3. **Register trusted types for custom data source classes**: When a report's `DataSource` is a custom class, you must register it as trusted **before** any deserialization occurs. Add `DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(MyDataRecord));` in Program.cs **before** `WebApplication.CreateBuilder(args)` for each custom class. This prevents `XtraSerializationSecurityTrace+NonTrustedTypeDeserializationException` when the designer loads or reloads the report. Prefer `RegisterTrustedClass()` over `RegisterTrustedAssembly()` — the latter trusts every type in the assembly, which is overly permissive.

### Server-Side Constraints

4. **Register `IReportProvider` after `AddDevExpressBlazorReporting()`**: DevExpress registers a proxy inside `AddDevExpressBlazorReporting()`; .NET DI resolves last-registered-wins. Always register your `IReportProvider` implementation **after** calling `AddDevExpressBlazorReporting()` or `AddDevExpressServerSideBlazorReportViewer()` so your implementation takes precedence.

### Client-Side (Interactive WebAssembly Standalone) Constraints

5. **All I/O must be async via `HttpClient`**: Synchronous operations are not supported in Standalone WebAssembly. Implement `IReportProviderAsync` (not `IReportProvider`) and use `async Task<XtraReport> GetReportAsync()`, not sync `GetReport()`.

6. **No blocking operations**: Never use `.Result`, `.Wait()`, or `.GetAwaiter().GetResult()`. Interactive WebAssembly Standalone runs on a single thread; blocking deadlocks the UI. All operations must use full `async/await`.

7. **Backend API required for persistence**: When using `ReportStorageWebExtension` in Interactive WebAssembly Standalone, you must implement backend API endpoints to handle report persistence — the client cannot write directly to the server file system. Implement `GetDataAsync`, `SetDataAsync`, `GetUrlsAsync` (not their synchronous counterparts).

8. **Register async methods in `ReportStorageWebExtension`**: Override `GetDataAsync`, `SetDataAsync`, `GetUrlsAsync`, and `SetNewDataAsync` (async variants). Do not override the synchronous versions (`GetData`, `SetData`, etc.) — they will not be called in WebAssembly.

---

## Antipatterns — Report Name Resolution & Storage

Common mistakes when implementing report name resolution services and storage.

**❌ A1: Register both `IReportProvider` and `ReportStorageWebExtension`**
- **Symptom**: Designer's saved reports do not appear in the viewer, or `ReportStorageWebExtension is not registered` errors
- **Why**: Both interfaces solve the same problem. When both are registered, they conflict — viewers may skip one, and designer-saved reports become invisible
- **Fix**: Use **only** `ReportStorageWebExtension` if `DxReportDesigner` is in the project (requires save/load). Use **only** `IReportProvider` for viewers without a designer. 

**❌ A2: Register `IReportProvider` before `AddDevExpressBlazorReporting()`**
- **Symptom**: `ReportStorageWebExtension is not registered` at runtime (JS-based viewers)
- **Why**: DevExpress registers a proxy inside `AddDevExpressBlazorReporting()`; .NET DI resolves last-registered-wins
- **Fix**: Always register `AddScoped<IReportProvider, ...>()` **after** `AddDevExpressBlazorReporting()`. This ensures your implementation is used instead of the DevExpress proxy. 

**❌ A15: Use `FaultException` in `ReportStorageWebExtension` throw statements**
- **Symptom**: Compile error — `FaultException` is undefined or build fails
- **Why**: `FaultException` is a WCF type (`System.ServiceModel`). Blazor reporting does not use WCF; using it causes build/runtime errors
- **Fix**: Use `InvalidOperationException` instead in all storage throw statements:
  ```csharp
  // ✅ Correct
  public override byte[] GetData(string url) {
      try {
          if (Directory.EnumerateFiles(reportDirectory)...){
              return File.ReadAllBytes(...);
          }
      } catch (Exception) {
          throw new InvalidOperationException("Could not get report data.");
      }
      throw new InvalidOperationException($"Could not find report '{url}'.");
  }
  ```

### Client-Side (Interactive WebAssembly Standalone) Antipatterns

**❌ A17: Blocking async calls in Interactive WebAssembly Standalone**
- **Symptom**: UI freezes, app hangs, or becomes unresponsive
- **Why**: Interactive WebAssembly Standalone runs on a single thread; `.Result` or `.Wait()` blocks that thread indefinitely (deadlock)
- **Fix**: Use full `async/await`:
  ```csharp
  // ❌ Wrong — deadlocks the thread
  var data = httpClient.GetByteArrayAsync(url).Result;
  
  // ✅ Correct
  var data = await httpClient.GetByteArrayAsync(url);
  ```

**❌ A18: Implementing synchronous overrides of `ReportStorageWebExtension` in Interactive WebAssembly Standalone**
- **Symptom**: Designer Save/Load/Open dialogs don't work or throw "not implemented" errors
- **Why**: Interactive WebAssembly Standalone cannot use synchronous file I/O or HTTP calls. Only async overrides are called
- **Fix**: Override async methods (`GetDataAsync`, `SetDataAsync`, `GetUrlsAsync`), not sync ones:
  ```csharp
  // ❌ Wrong — these are never called in WASM
  public override byte[] GetData(string url) { ... }
  public override void SetData(XtraReport report, string url) { ... }
  
  // ✅ Correct — use async variants
  public override async Task<byte[]> GetDataAsync(string url) { ... }
  public override async Task SetDataAsync(XtraReport report, string url) { ... }
  ```

**❌ A19: Omitting `AddDevExpressBlazorReportingWebAssembly()` in Interactive WebAssembly Standalone Program.cs**
- **Symptom**: Components don't render or initialization errors
- **Why**: Different Interactive WebAssembly Standalone apps require different registration methods than server apps
- **Fix**: Use the correct registration:
  ```csharp
  // ❌ Wrong — this is for Interactive Server / Interactive WebAssembly (Client + Server), not Interactive WebAssembly Standalone
  builder.Services.AddDevExpressBlazorReporting();
  
  // ✅ Correct — for Interactive WebAssembly Standalone
  builder.Services.AddDevExpressBlazorReportingWebAssembly(configure => {
      configure.UseDevelopmentMode();
  });
  ```

**❌ A20: Using synchronous `IReportProvider` in Interactive WebAssembly Standalone**
- **Symptom**: Compile error or runtime "not registered" error
- **Why**: Interactive WebAssembly Standalone does not support synchronous I/O; `IReportProvider` is sync-only
- **Fix**: Use `IReportProviderAsync` instead:
  ```csharp
  // ❌ Wrong (won't compile in WASM)
  public class MyReportProvider : IReportProvider {
      public XtraReport GetReport(string id, ReportProviderContext context) { ... }
  }
  
  // ✅ Correct
  public class MyReportProvider : IReportProviderAsync {
      public async Task<XtraReport> GetReportAsync(string id, ReportProviderContext context) { ... }
  }
  ```
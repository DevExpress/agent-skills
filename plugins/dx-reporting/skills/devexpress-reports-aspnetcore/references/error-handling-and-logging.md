# Error Handling & Operation Logging — ASP.NET Core Reporting

## When to Use This Reference

Use this when you need to:
- Control the error message an end user sees when a server-side exception occurs in the Report Designer, the Web Document Viewer, or the Designer's integrated Preview — and log the real exception instead of losing it behind a generic message
- Get a callback whenever the Viewer creates, exports, or releases a report/document (audit trail, usage metrics)
- Run a custom server-side operation against the document currently open in the Viewer
- Enable verbose development-time error output and version-mismatch checks

This file is scoped to **error handling and operation logging/auditing only**. Other server-side services live in concern-based files:

- Data Source Wizard connection strings, schema providers, Object Data Source filters, JSON schema discovery — [data-sources.md](data-sources.md)
- Authorization, connection-string protection, custom SQL, multi-tenant filtering, Bearer-token export URIs — [security.md](security.md)
- Report storage, name resolution, web-farm storage/caching — [report-storage.md](report-storage.md)
- Client-side event handlers (a different mechanism entirely) — [designer-customization.md](designer-customization.md) and [viewer-customization.md](viewer-customization.md)

## Registration Patterns

Every service on this page has **no dedicated `*ConfigurationBuilder` method** — register each via plain DI. The one builder method here (`UseDevelopmentMode`) goes on `ReportingConfigurationBuilder`:

```csharp
using DevExpress.AspNetCore;
using DevExpress.AspNetCore.Reporting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureReportingServices(configurator => {
    if (builder.Environment.IsDevelopment())
        configurator.UseDevelopmentMode();
});

// Plain DI — the exception handlers, logger, and document operation service
builder.Services.AddSingleton<IReportDesignerExceptionHandler, CustomReportDesignerExceptionHandler>();

var app = builder.Build();
```

## Services

| Service / Method | Why / When |
|---|---|
| `IReportDesignerExceptionHandler` | Handles server-side errors in the Report Designer. Implement this to log the real exception and control exactly what message is shown to the end user, instead of the Designer's default generic message. Register via plain DI. |
| `IWebDocumentViewerExceptionHandler` | The same idea for the standalone Web Document Viewer — and for the Designer's integrated Preview, which is a Viewer instance. Implement it to log the real exception and control the message the end user sees. Register via plain DI. |
| `WebDocumentViewerOperationLogger` | There's no built-in logging of viewer activity otherwise — implement this to get a callback whenever the Viewer creates, exports, or releases a report/document, for audit trails or usage metrics. Register via plain DI. |
| `DocumentOperationService` | Performs custom operations against the currently opened document. Works together with the Document Viewer's callbacks — see [PerformCustomDocumentOperation Method](https://docs.devexpress.com/XtraReports/js-DevExpress.Reporting.Viewer.JSReportViewer#js_devexpress_reporting_viewer_jsreportviewer_performcustomdocumentoperation_customdata_hidemessagefromuser). Register via plain DI. |
| `UseDevelopmentMode()` / `UseDevelopmentMode(Action<DevelopmentModeOptions>)` | On `ReportingConfigurationBuilder`. Enables verbose error output and version-mismatch checks — the fastest way to turn an opaque HTTP 500 into an actionable message. Gate it on `IsDevelopment()`; see [troubleshooting.md](troubleshooting.md). |

> `WebDocumentViewerOperationLogger` is also the hook the per-user authorization example in [security.md](security.md#per-user-document-isolation) uses to map document ids to their owning user — a single class can implement the logger and the authorization services together.

## Representative Example — `IReportDesignerExceptionHandler`

Subclass `ReportDesignerExceptionHandler` (or implement `IReportDesignerExceptionHandler` directly) and override `GetExceptionMessage` to control the message shown to the end user for any server-side error:

```csharp
using System;
using DevExpress.XtraReports.Web.ReportDesigner.Services;

public class CustomReportDesignerExceptionHandler : ReportDesignerExceptionHandler {
    public override string GetExceptionMessage(Exception ex) {
        // Log ex here if needed.
        return ex.Message;
    }
}
```

```csharp
builder.Services.AddSingleton<IReportDesignerExceptionHandler, CustomReportDesignerExceptionHandler>();
```

> `ReportDesignerExceptionHandler` also exposes `GetUnknownExceptionMessage` (for exception types that otherwise show a generic "Internal Server Error") and `GetFaultExceptionMessage` (for `FaultException`) — override either instead of `GetExceptionMessage` to handle only those specific cases.

> Returning `ex.Message` verbatim, as above, can leak connection strings, file paths, or SQL to the end user. In production, log the exception and return a sanitized message instead.

## Related Client-Side Error Events

These handlers cover the **server** side. The client side has its own `OnServerError` event on the Viewer and the Designer — use it to react in the browser (show a toast, redirect) after the server has already decided what message to send. See [viewer-customization.md](viewer-customization.md#error-handling-onservererror) and [designer-customization.md](designer-customization.md#client-side-events-reference).

## See Also

- [troubleshooting.md](troubleshooting.md) — symptom→fix table, development mode, and logging configuration (`"DevExpress": "Debug"`).
- [report-storage.md](report-storage.md) — throwing `DevExpress.XtraReports.Web.ClientControls.FaultException` from `GetData` so its message reaches the end user.

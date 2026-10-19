# Security — ASP.NET Core Reporting

## When to Use This Reference

Use this when you need to:
- Restrict reporting endpoints to authenticated users
- Fix HTTP 400 errors caused by anti-forgery token validation
- Implement per-user document isolation
- Serve print/export results in an app that authenticates with a header token (Bearer)
- Set up Content Security Policy (CSP) with a nonce
- Restrict data access per user (multi-tenancy)
- Protect data source connection strings sent to the client, or handle a data source that isn't a built-in DevExpress type
- Decide whether (and how safely) to allow custom SQL queries in the Data Source Wizard
- Load images from a `file://` URL in `XRPictureBox`/`XRRichText`, or otherwise restrict image URL access
- Respond to a request to enable report scripts

## Authorization on Reporting Controllers

Built-in ASP.NET authorization allows you to restrict access to handlers and controller actions. 

When necessary, use the `[Authorize]` attribute. Apply `[Authorize]` to `CustomWebDocumentViewerController`, `CustomReportDesignerController` and `CustomQueryBuilderController`.

## CSRF / Anti-Forgery (Fix HTTP 400 Errors)

If the app uses global anti-forgery validation (via `AddControllersWithViews()` default or a global filter), reporting controllers receive a 400 response. Fix by adding `[IgnoreAntiforgeryToken]`:

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[Authorize]
[IgnoreAntiforgeryToken]
public class CustomWebDocumentViewerController : WebDocumentViewerController {
    public CustomWebDocumentViewerController(IWebDocumentViewerMvcControllerService s) : base(s) { }
}
```

Apply to all three reporting controllers.

## Per-User Document Isolation

Implement `IWebDocumentViewerAuthorizationService` to control which reports and documents a user can access, and `IExportingAuthorizationService` (`CanReadExportedDocument`) to restrict access to the result of an **asynchronous** export. Both need to know which user owns a given report/document/export id — that mapping comes from a `WebDocumentViewerOperationLogger`, so the canonical implementation is **one class that derives from the logger and implements both interfaces**, registered three times:

```csharp
builder.Services.AddScoped<IWebDocumentViewerAuthorizationService, DocumentViewerAuthorizationService>();
builder.Services.AddScoped<WebDocumentViewerOperationLogger, DocumentViewerAuthorizationService>();
builder.Services.AddScoped<IExportingAuthorizationService, DocumentViewerAuthorizationService>();
```

This is a security-critical, rarely-changed API. **Do not write the implementation from memory** — fetch the complete, current ASP.NET Core example (id→owner maps, every method) from the API topics before generating code:

- [IWebDocumentViewerAuthorizationService](https://docs.devexpress.com/XtraReports/DevExpress.XtraReports.Web.WebDocumentViewer.IWebDocumentViewerAuthorizationService) — per-method semantics plus a full ASP.NET Core sample
- [IExportingAuthorizationService](https://docs.devexpress.com/XtraReports/DevExpress.XtraReports.Web.WebDocumentViewer.IExportingAuthorizationService)

## Export URIs Under Header/Token Authentication

If the reporting endpoint is protected by a header-carried token (for example, a Bearer token), the browser cannot fetch print/export results — a plain browser navigation carries no `Authorization` header. Implement `IWebDocumentViewerExportResultUriGenerator` (`CreateUri`) to store the exported document elsewhere and return a URI the browser *can* reach, and register the service at startup. The same service also produces friendlier print-job/tab names.

See [IWebDocumentViewerExportResultUriGenerator](https://docs.devexpress.com/XtraReports/DevExpress.XtraReports.Web.WebDocumentViewer.IWebDocumentViewerExportResultUriGenerator) for the required steps and the linked sample project.

## Content Security Policy (CSP) with Nonce

A Content Security Policy (CSP) is an additional layer of security built into most modern browsers. 
It allows the browser to recognize and mitigate certain types of risks, including Cross Site Scripting (XSS) and data injection attacks. 

When the app enforces CSP headers, the viewer/designer render inline scripts that require a nonce. 

This **requires the prebuilt client-side model style** (Style 3) — the nonce must be generated on the server *before* the model is built, so the report instance/name styles cannot be used. 

Start from the model-builder pattern in [controller-view-binding.md](controller-view-binding.md#document-viewer); the
CSP-specific additions are: generate a nonce, set the response header, wrap the model so the view can read the nonce, and pass the nonce to `.Nonce(...)`.

Under CSP the component must be sized by CSS, not by the helper: set `.Height(null)` and `.Width(null)` and pass a class to `.CssClassName(...)` that carries the height/width.

For details, see [Content Security Policy for ASP.NET Core Applications](https://docs.devexpress.com/XtraReports/404141/web-reporting/asp-net-core-reporting/content-security-policy).

## Protecting Connection Strings Sent to the Client

When a report resolves a SQL connection by name, the connection's parameters (not just its name) are sometimes serialized and sent to the client so the Designer can round-trip them. By default, these parameters are encrypted using the ASP.NET Core Data Protection service. 

If you want to use a different encryption mechanism, implement one of two `DevExpress.DataAccess` interfaces and register it in DI — both are used by the Designer and the Viewer:

- [`ISecureDataConverter`](https://docs.devexpress.com/CoreLibraries/DevExpress.DataAccess.ISecureDataConverter) — `Protect`/`Unprotect` on any critical string that is serialized between client and server. Use it to substitute your own encryption.
- [`IDataSourceProtectionService`](https://docs.devexpress.com/CoreLibraries/DevExpress.DataAccess.IDataSourceProtectionService) — required when the report reads from a data source that is **not** a built-in DevExpress data source type or a descendant of one.

Both are rarely-used security APIs; take the member signatures and the registration call from the linked topics rather than from memory.

## Allowing Custom SQL Queries

`ReportDesignerConfigurationBuilder.EnableCustomSql()` and `ISqlDataSourceWizardCustomizationService.IsCustomSqlDisabled = false` both let end users type arbitrary SQL in the Data Source Wizard instead of only using the visual Query Builder. DevExpress's default validation only blocks statements other than `SELECT` (and `SELECT INTO`) — it does **not** guarantee safety against a malicious `SELECT` query. 

Only enable this for trusted users and add your own validation by implementing `ICustomQueryValidator` inside an `ISqlDataSourceWizardCustomizationService`. Application-level validation alone is not a complete defense — inform the user that the database account used by the connection should also be restricted to read-only permissions, since that's a database-level change outside the application code.

See the following help topic for the full guidance on hardening custom SQL and database-level access control: [Data Access Security in Web Applications](https://docs.devexpress.com/XtraReports/119202/web-reporting/common-features/application-security/data-access-security-in-web-applications).

To learn about Custom SQL Query Validation, refer to [Custom SQL Query Validation (Web)](https://docs.devexpress.com/XtraReports/115119/web-reporting/common-features/application-security/custom-sql-query-validation).

## Multi-Tenant Data Filtering

Implement `ISelectQueryFilterService` to add `WHERE` conditions to every SQL query. 

See: [Multi-Tenant Support (Row Filtering in Shared SQL Database)](https://docs.devexpress.com/XtraReports/403076/web-reporting/common-features/application-security/multitenancy-support).

## Ensure Safe Image Loading from URL

`XRPictureBox.ImageUrl` (and `XRRichText` `IncludePicture` fields) only accept the `http://`, `https://`, and `ftp://` protocols out of the box — `file://` is rejected.

If you need to load images via `file://` (e.g., from a local or network share), use the `AccessSettings.StaticResources.SetRules` method (`DevExpress.Security.Resources`) to explicitly allow access to a directory that contains images.

See: [XRPictureBox.ImageUrl](https://docs.devexpress.com/XtraReports/DevExpress.XtraReports.UI.XRPictureBox.ImageUrl#aspnet-core).

## Report Scripts

Report scripts (event handlers written in C#/VB.NET and stored with the report) let a report author run arbitrary code on the server or on another user's machine when the report opens or prints. **This is not secure.** Script execution is disabled by default (`ExecutionMode.Deny`): the Report Designer hides the Scripts Editor, and the Document Viewer will not run any script attached to a report.

If a user asks how to enable report scripts, tell them scripts are unsafe and that expression bindings (`Expressions`/`ExpressionsAdvanced` data binding modes) should be used instead — see [Data Binding Modes](https://docs.devexpress.com/XtraReports/119236/feature-guide-to-devexpress-reports/use-expressions/data-binding-modes). Only fall back to scripts if the reports come from a fully trusted source and expression bindings genuinely cannot express the logic.

If they still want scripts enabled, register a `ScriptPermissionManager` at application startup with unrestricted execution:

```csharp
using DevExpress.XtraReports.Security;

ScriptPermissionManager.GlobalInstance = new ScriptPermissionManager(ExecutionMode.Unrestricted);
```

See: [Scripts - Security Considerations](https://docs.devexpress.com/XtraReports/2616/feature-guide-to-devexpress-reports/reporting-api/use-report-scripts/scripts-security).
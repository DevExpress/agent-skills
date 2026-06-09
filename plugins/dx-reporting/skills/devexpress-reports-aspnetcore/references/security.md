# Security — ASP.NET Core Reporting

## When to Use This Reference

Use this when you need to:
- Restrict reporting endpoints to authenticated users
- Fix HTTP 400 errors caused by anti-forgery token validation
- Implement per-user document isolation
- Set up Content Security Policy (CSP) with a nonce
- Authenticate Angular/React SPAs that call reporting APIs with Bearer tokens
- Restrict data access per user (multi-tenancy)

## Authorization on Reporting Controllers

Apply `[Authorize]` to restrict all reporting HTTP endpoints:

```csharp
using Microsoft.AspNetCore.Authorization;
using DevExpress.AspNetCore.Reporting.WebDocumentViewer;
using DevExpress.AspNetCore.Reporting.WebDocumentViewer.Native.Services;

[Authorize]
public class CustomWebDocumentViewerController : WebDocumentViewerController {
    public CustomWebDocumentViewerController(IWebDocumentViewerMvcControllerService s) : base(s) { }
}
```

Apply `[Authorize]` similarly to `CustomReportDesignerController` and `CustomQueryBuilderController`.

## CSRF / Anti-Forgery (Fix HTTP 400 Errors)

If the app uses global anti-forgery validation (via `AddControllersWithViews()` default or a global filter), reporting controllers receive a 400 response. Fix by adding `[IgnoreAntiforgeryToken]`:

```csharp
using Microsoft.AspNetCore.Mvc;

[Authorize]
[IgnoreAntiforgeryToken]
public class CustomWebDocumentViewerController : WebDocumentViewerController {
    public CustomWebDocumentViewerController(IWebDocumentViewerMvcControllerService s) : base(s) { }
}
```

Apply to all three reporting controllers.

## Per-User Document Isolation

Implement `IWebDocumentViewerAuthorizationService` to control which documents a user can access:

```csharp
using DevExpress.XtraReports.Web.WebDocumentViewer.Authorization;

public class UserAuthorizationService : IWebDocumentViewerAuthorizationService {
    readonly IHttpContextAccessor _ctx;
    public UserAuthorizationService(IHttpContextAccessor ctx) { _ctx = ctx; }

    public bool CanCreateDocument() => _ctx.HttpContext?.User.Identity?.IsAuthenticated ?? false;
    public bool CanCreateReport() => true;
    public bool CanReadDocument(string documentId) => true; // add per-user logic here
    public bool CanReadReport(string reportId) => true;
    public bool CanReleaseDocument(string documentId) => true;
    public bool CanReleaseReport(string reportId) => true;
}

public class UserExportingAuthorizationService : IExportingAuthorizationService {
    public bool CanReadExportedDocument(string exportDocumentId) => true;
}
```

```csharp
// Program.cs
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IWebDocumentViewerAuthorizationService, UserAuthorizationService>();
builder.Services.AddScoped<IExportingAuthorizationService, UserExportingAuthorizationService>();
```

## Content Security Policy (CSP) with Nonce

When the app enforces CSP headers, the viewer/designer render inline scripts that require a nonce:

```csharp
// Controller action
public async Task<IActionResult> Viewer(
    [FromServices] IWebDocumentViewerClientSideModelGenerator modelGenerator,
    [FromQuery] string reportName = "SalesReport")
{
    var nonceBytes = new byte[32];
    using var rng = RandomNumberGenerator.Create();
    rng.GetBytes(nonceBytes);
    var nonce = Convert.ToBase64String(nonceBytes);

    Response.Headers["Content-Security-Policy"] =
        $"script-src 'self' 'nonce-{nonce}';" +
        "img-src 'self' data:;" +
        "style-src 'self';" +
        "connect-src 'self';" +
        "worker-src 'self' blob:;" +
        "frame-src 'self' blob:;";

    var model = new ViewerModel {
        ViewerModel = await modelGenerator.GetModelAsync(reportName, WebDocumentViewerController.DefaultUri),
        Nonce = nonce
    };
    return View(model);
}
```

```cshtml
@* View — use Height(null) and Width(null) when CSP nonce is set; size via CSS *@
@{
    var viewerRender = Html.DevExpress().WebDocumentViewer("DocumentViewer")
        .Height(null)
        .Width(null)
        .Nonce(Model.Nonce)
        .CssClassName("my-viewer-container")
        .Bind(Model.ViewerModel);
    @viewerRender.RenderHtml()
}
```

```css
.my-viewer-container { width: 100%; height: calc(100vh - 60px); }
```

## Bearer Token Auth (Angular / React SPA)

For SPAs that authenticate with JWT Bearer tokens, attach the token to every reporting fetch:

```javascript
// In Angular or React app init
import { fetchSetup } from "@devexpress/analytics-core/analytics-utils";

fetchSetup.fetchSettings = {
    headers: {
        Authorization: `Bearer ${getAccessToken()}`
    }
};
```

## Multi-Tenant Data Filtering

Implement `ISelectQueryFilterService` to add WHERE conditions to every SQL query:

```csharp
using DevExpress.DataAccess.Sql;
using DevExpress.XtraReports.Services;

public class TenantQueryFilter : ISelectQueryFilterService {
    readonly IHttpContextAccessor _ctx;
    public TenantQueryFilter(IHttpContextAccessor ctx) { _ctx = ctx; }

    public CriteriaOperator CustomizeFilterExpression(SelectQuery query, CriteriaOperator filter) {
        var tenantId = _ctx.HttpContext?.User.FindFirst("TenantId")?.Value;
        if (tenantId == null) return filter;
        var tenantFilter = CriteriaOperator.Parse($"[TenantId] = '{tenantId}'");
        return filter is null ? tenantFilter : new GroupOperator(GroupOperatorType.And, filter, tenantFilter);
    }
}

// Program.cs
builder.Services.AddScoped<ISelectQueryFilterService, TenantQueryFilter>();
```

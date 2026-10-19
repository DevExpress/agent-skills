# Controller → View Binding Patterns

## When to Use This Reference

Use this when you need to decide **how the application page that hosts the
Document Viewer or Report Designer supplies its report** — the connection between
the controller action (or Razor Page handler) that serves the page and the
`Html.DevExpress().WebDocumentViewer(...)` / `.ReportDesigner(...)` helper in the
view. Specifically:

- Choose between the three realistic binding styles — a **report instance**, a
  **report name**, or a **prebuilt client-side model**.
- Add the missing page-serving action that [getting-started.md](getting-started.md)
  Step 6 does not show (its controllers are the DevExpress *infrastructure*
  controllers, which never call `View()`).
- Wire a controller-built model to a `@model`-typed view (the pattern the DevExpress
  Template Kit generates, and the one CSP-nonce scenarios require — see
  [security.md](security.md#content-security-policy-csp-with-nonce)).

This file is about **controller → view plumbing only**. Report *name resolution*
itself (how `.Bind("SalesReport")` finds the report) lives in
[report-storage.md](report-storage.md); this file only shows how the name reaches
the control.

> Do not confuse these two controllers. The **infrastructure controller**
> (`CustomWebDocumentViewerController : WebDocumentViewerController`) handles the
> viewer's background AJAX calls and has no custom actions. The **page controller**
> below (e.g. `ReportsController`) is an ordinary MVC controller whose action calls
> `View()` to render the page a user navigates to. Both exist in the same app.

## The Three Binding Styles

| Style | View call | When |
|---|---|---|
| 1. Report instance | `.Bind(new TestReport())` | Simplest; report type known at compile time, no per-request data. |
| 2. Report name | `.Bind("SalesReport")` | Report resolved by name via `IReportProvider`/`ReportStorageWebExtension`. |
| 3. Prebuilt client-side model | `.Bind(Model)` | Controller builds the model (per-request report/data, or a CSP nonce). |

---

## Document Viewer

### Style 1 — Report instance (getting-started default)

The controller passes nothing; the view constructs the report. This is the style in
[getting-started.md](getting-started.md) Step 9.

```csharp
// Controllers/ReportsController.cs
using Microsoft.AspNetCore.Mvc;

public class ReportsController : Controller {
    public IActionResult Viewer() => View();
}
```

```cshtml
@* Views/Reports/Viewer.cshtml *@
@using YourApp.Reports

@* Viewer client-asset tags (CSS + JS in load order) — see getting-started.md Step 9
   for the default Direct References tags, or client-resource-integration.md for Bundled *@

@(Html.DevExpress().WebDocumentViewer("DocumentViewer")
    .Height("1000px")
    .Bind(new TestReport()))
```

`@using YourApp.Reports` is needed **only for this style**, because the view names
the report type directly. Styles 2 and 3 do not reference the report type in the view.

### Style 2 — Report name

The controller still passes nothing; the view names the report as a string, and the
registered `IReportProvider`/`ReportStorageWebExtension` resolves it. See
[report-storage.md](report-storage.md) for how to implement that resolution — this
file only shows the binding call.

```csharp
public IActionResult Viewer() => View();
```

```cshtml
@* Views/Reports/Viewer.cshtml — no @using YourApp.Reports needed *@
@* Viewer client-asset tags (CSS + JS in load order) — see getting-started.md Step 9
   for the default Direct References tags, or client-resource-integration.md for Bundled *@

@(Html.DevExpress().WebDocumentViewer("DocumentViewer")
    .Height("1000px")
    .Bind("SalesReport"))
```

To pass the name per request instead of hardcoding it, accept it in the action and
forward it through `ViewData`/a view model, then `.Bind((string)ViewData["reportName"])`.

### Style 3 — Prebuilt client-side model (Template Kit pattern)

The controller builds a `WebDocumentViewerModel` with
`IWebDocumentViewerClientSideModelGenerator` and passes it to a strongly-typed view.
Use this when the page needs per-request logic before rendering, or when a CSP nonce
must be applied ([security.md](security.md#content-security-policy-csp-with-nonce)).

```csharp
// Controllers/ReportsController.cs
using DevExpress.AspNetCore.Reporting.WebDocumentViewer;   // WebDocumentViewerController
using DevExpress.XtraReports.Web.WebDocumentViewer;        // IWebDocumentViewerClientSideModelGenerator
using Microsoft.AspNetCore.Mvc;

public class ReportsController : Controller {
    public IActionResult Viewer(
        [FromServices] IWebDocumentViewerClientSideModelGenerator modelGenerator,
        [FromQuery] string reportName = "SalesReport") {
        var model = modelGenerator.GetModel(reportName, WebDocumentViewerController.DefaultUri);
        return View(model);
    }
}
```

```cshtml
@* Views/Reports/Viewer.cshtml *@
@model DevExpress.XtraReports.Web.WebDocumentViewer.WebDocumentViewerModel

@* Viewer client-asset tags (CSS + JS in load order) — see getting-started.md Step 9
   for the default Direct References tags, or client-resource-integration.md for Bundled *@

@(Html.DevExpress().WebDocumentViewer("DocumentViewer")
    .Height("1000px")
    .Bind(Model))
```

> **`DefaultUri`.** `WebDocumentViewerController.DefaultUri` is the default route the
> infrastructure controller answers on (`/DXXRDV`). Because `DefaultUri` is a static
> field inherited by your subclass, `CustomWebDocumentViewerController.DefaultUri`
> refers to the same value — both compile and behave identically. Pass an explicit
> route string only if you gave the infrastructure controller a custom `[Route(...)]`.

> **Sync vs. async.** Use `GetModel(...)` (shown above) by default. If the app calls
> `configurator.UseAsyncEngine()` (Step 5b in [getting-started.md](getting-started.md)),
> switch to `await modelGenerator.GetModelAsync(reportName, WebDocumentViewerController.DefaultUri)`
> in an `async Task<IActionResult>` action.

---

## Report Designer

### Style 1 — Report instance

```csharp
public IActionResult Designer() => View();
```

```cshtml
@using YourApp.Reports
@* Designer client-asset tags (CSS + JS in load order) — see getting-started.md Step 9
   for the default Direct References tags, or client-resource-integration.md for Bundled *@

@(Html.DevExpress().ReportDesigner("reportDesigner")
    .Height("1000px")
    .Bind(new TestReport()))
```

### Style 2 — Report name

```csharp
public IActionResult Designer() => View();
```

```cshtml
@* Designer client-asset tags (CSS + JS in load order) — see getting-started.md Step 9
   for the default Direct References tags, or client-resource-integration.md for Bundled *@

@(Html.DevExpress().ReportDesigner("reportDesigner")
    .Height("1000px")
    .Bind("SalesReport"))
```

### Style 3 — Prebuilt client-side model (Template Kit pattern)

The controller builds a `ReportDesignerModel` with `IReportDesignerModelBuilder` and
passes it to a strongly-typed view. This is the base model-builder pattern that
[data-sources.md](data-sources.md#adding-data-sources-in-the-controller)
extends by adding pre-configured data sources.

```csharp
// Controllers/ReportsController.cs
using DevExpress.XtraReports.Web.ReportDesigner;            // ReportDesignerModel
using DevExpress.XtraReports.Web.ReportDesigner.Services;   // IReportDesignerModelBuilder
using Microsoft.AspNetCore.Mvc;

public class ReportsController : Controller {
    public IActionResult Designer(
        [FromServices] IReportDesignerModelBuilder builder,
        [FromQuery] string reportName = "SalesReport") {
        ReportDesignerModel model = builder
            .Report(reportName)
            .BuildModel();
        return View(model);
    }
}
```

```cshtml
@* Views/Reports/Designer.cshtml *@
@model DevExpress.XtraReports.Web.ReportDesigner.ReportDesignerModel

@* Designer client-asset tags (CSS + JS in load order) — see getting-started.md Step 9
   for the default Direct References tags, or client-resource-integration.md for Bundled *@

@(Html.DevExpress().ReportDesigner("reportDesigner")
    .Height("1000px")
    .Bind(Model))
```

> **Async.** With `UseAsyncEngine()`, use `await builder.Report(reportName).BuildModelAsync()`
> in an `async Task<IActionResult>` action.

---

## Razor Pages

Styles 1 and 2 need no `PageModel` — a bare `@page` view plus the `.Bind(...)` call
is enough (see [getting-started-razor-pages.md](getting-started-razor-pages.md)
Step 9). Style 3 needs a handler to build the model. In Razor Pages the view's
`@model` is the **PageModel**, so expose the built model as a property on it and bind
`Model.<Property>`.

### Viewer — Style 3

```csharp
// Pages/Reports/Viewer.cshtml.cs
using DevExpress.AspNetCore.Reporting.WebDocumentViewer;
using DevExpress.XtraReports.Web.WebDocumentViewer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace YourApp.Pages.Reports;

public class ViewerModel : PageModel {
    public WebDocumentViewerModel ReportViewerModel { get; private set; }

    public void OnGet(
        [FromServices] IWebDocumentViewerClientSideModelGenerator modelGenerator,
        [FromQuery] string reportName = "SalesReport") {
        ReportViewerModel = modelGenerator.GetModel(reportName, WebDocumentViewerController.DefaultUri);
    }
}
```

```cshtml
@* Pages/Reports/Viewer.cshtml *@
@page
@model YourApp.Pages.Reports.ViewerModel

@* Viewer client-asset tags (CSS + JS in load order) — see getting-started-razor-pages.md Step 9
   for the default Direct References tags, or client-resource-integration.md for Bundled *@

@(Html.DevExpress().WebDocumentViewer("DocumentViewer")
    .Height("1000px")
    .Bind(Model.ReportViewerModel))
```

### Designer — Style 3

```csharp
// Pages/Reports/Designer.cshtml.cs
using DevExpress.XtraReports.Web.ReportDesigner;
using DevExpress.XtraReports.Web.ReportDesigner.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace YourApp.Pages.Reports;

public class DesignerModel : PageModel {
    public ReportDesignerModel ReportDesignerModel { get; private set; }

    public void OnGet(
        [FromServices] IReportDesignerModelBuilder builder,
        [FromQuery] string reportName = "SalesReport") {
        ReportDesignerModel = builder.Report(reportName).BuildModel();
    }
}
```

```cshtml
@* Pages/Reports/Designer.cshtml *@
@page
@model YourApp.Pages.Reports.DesignerModel

@* Designer client-asset tags (CSS + JS in load order) — see getting-started-razor-pages.md Step 9
   for the default Direct References tags, or client-resource-integration.md for Bundled *@

@(Html.DevExpress().ReportDesigner("reportDesigner")
    .Height("1000px")
    .Bind(Model.ReportDesignerModel))
```

`[FromServices]`/`[FromQuery]` on an `OnGet`/`OnGetAsync` handler parameter bind
exactly as they do on an MVC action — no extra configuration.

---

## Which Style Should I Use?

- **Style 1 (instance)** — the default. Pick it when the report type is fixed and the
  view needs no per-request server logic. Simplest to read.
- **Style 2 (name)** — pick it when reports are stored/resolved by name (a designer
  with save/load, or a viewer that opens many reports by string). Requires an
  `IReportProvider`/`ReportStorageWebExtension` — see [report-storage.md](report-storage.md).
- **Style 3 (prebuilt model)** — pick it when the controller must run per-request
  logic before rendering (choose the report/data by user, inject data sources), or
  when the page enforces a **CSP nonce** — Style 3 is **required** for the CSP-nonce
  scenario in [security.md](security.md#content-security-policy-csp-with-nonce),
  because the nonce must be set on the server before the model is generated.

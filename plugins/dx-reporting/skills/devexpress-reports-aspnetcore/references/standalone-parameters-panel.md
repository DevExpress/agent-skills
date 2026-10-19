# Standalone Report Parameters Panel — ASP.NET Core

## When to Use This Reference

Use this when you need to:
- Collect report parameter values from the end user **without** showing a document preview (e.g., to email or export a report in the background)
- Add a parameter-input form that is not attached to the Document Viewer or Report Designer
- Send serialized parameter values from the client back to the server and apply them to a report instance before exporting

The Standalone Report Parameters Panel is a separate ASP.NET Core control — `Html.DevExpress().ReportParametersPanel(...)` — not just a hidden/parameter-only view of the Document Viewer. It is based on the Document Viewer's own Parameters Panel, so its client-side API is a subset of the viewer's.

If you only need to hide the parameters tab **inside** an existing Document Viewer, see [references/viewer-customization.md](viewer-customization.md) (`CustomizeElements` / `PreviewElements.RightPanel`) instead — that does not require this standalone control.

## Backend Setup (Server)

1. Install the `DevExpress.AspNetCore.Reporting` and `Microsoft.Web.LibraryManager.Build` NuGet packages (same as the Document Viewer). Add `BuildBundlerMinifier` too only if the app's Document Viewer setup uses Pattern B (Bundled) — see [client-resource-integration.md](client-resource-integration.md).

2. Register DevExpress services in `Program.cs` (same registration as the Document Viewer — reuse it if the app already has a viewer or designer):

```csharp
using DevExpress.AspNetCore;
using DevExpress.AspNetCore.Reporting;
// ...
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDevExpressControls();
builder.Services.ConfigureReportingServices(configurator => {
    configurator.ConfigureWebDocumentViewer(viewerConfigurator => {
        viewerConfigurator.UseCachedReportSourceBuilder();
    });
});
// ...
var app = builder.Build();
app.UseDevExpressControls();// MUST precede MapStaticAssets/UseStaticFiles and any other static-file handling. Keep whichever call the scaffold already has — never add a second.
```

3. The panel needs the same request-handling controller as the Document Viewer. If the app doesn't already have one, add:

```csharp
using DevExpress.AspNetCore.Reporting.WebDocumentViewer;
using DevExpress.AspNetCore.Reporting.WebDocumentViewer.Native.Services;

public class CustomWebDocumentViewerController : WebDocumentViewerController {
    public CustomWebDocumentViewerController(IWebDocumentViewerMvcControllerService controllerService)
        : base(controllerService) { }
}
```

4. **Pick one binding lane and stay in it** — the panel offers the same instance-or-name choice as the Viewer:

| lane | build the model | apply the values | resolution service |
|---|---|---|---|
| instance | `GetModel(XtraReport, …)` | `ApplyParametersStateAsync(XtraReport, string)` | not needed |
| name | `GetModel(string, …)` | `ApplyParametersStateAsync(string, string)` | required — [report-storage.md](report-storage.md#ireportprovider) |

Under instance binding `ReportInfo.ReportUrl` is the **empty string**, so pairing an instance-built model with the *name* overload fails at click time with `Missing report name resolution service…`.

5. Add a controller action that builds the panel's client-side model with `IReportParametersPanelClientSideModelGenerator`:

```csharp
using DevExpress.AspNetCore.Reporting.WebDocumentViewer;
using DevExpress.XtraReports.Web.ParametersPanel;
using Microsoft.AspNetCore.Mvc;

public IActionResult Panel(
    [FromQuery] string reportName,
    [FromServices] IReportParametersPanelClientSideModelGenerator clientSideModelGenerator) {
    reportName = string.IsNullOrEmpty(reportName) ? "SalesReport" : reportName;
    ReportParametersPanelModel parametersPanelModel =
        clientSideModelGenerator.GetModel(reportName, WebDocumentViewerController.DefaultUri);
    return View("Panel", parametersPanelModel);
}
```

## Frontend Setup (Client)

The npm packages and client-resource pattern are the same ones the Document Viewer uses — reuse them rather than duplicating. If the project doesn't already have the Document Viewer's client libraries installed, see [getting-started.md](getting-started.md) Steps 2–4 and 8–9 for the default Pattern A (Direct References) templates, or [client-resource-integration.md](client-resource-integration.md) for the Pattern B (Bundled) templates.

1. Reference `DevExpress.AspNetCore` in `_ViewImports.cshtml` (or in the view itself):

```cshtml
@using DevExpress.AspNetCore
```

2. Ensure the site-wide base libraries are linked in `_Layout.cshtml`'s `<head>` (Pattern A — Direct References, the default):

```cshtml
<link rel="stylesheet" href="~/css/thirdparty/bootstrap.min.css" />
<script src="~/js/thirdparty/jquery.min.js"></script>
<script src="~/js/thirdparty/bootstrap.min.js"></script>
```

These paths assume the app ships no Bootstrap/jQuery of its own. If it does, keep its `~/lib/...` tags and only move them into `<head>`.

Under Pattern B (Bundled) this is `~/css/thirdparty.bundle.css` + `~/js/thirdparty.bundle.js` instead.

3. Create the view with the `ReportParametersPanel` fluent builder:

```cshtml
@model DevExpress.XtraReports.Web.ParametersPanel.ReportParametersPanelModel
@{
    var panelRender = Html.DevExpress().ReportParametersPanel("ParametersPanel")
        .Height("100%")
        .Width("750px")
        .Bind(Model);
    @panelRender.RenderHtml()
}

@section Scripts {
    @* Viewer client-asset tags (the panel reuses the Document Viewer's assets), Pattern A default *@
    <link rel="stylesheet" href="~/css/dx.light.css" />
    <link rel="stylesheet" href="~/css/reporting/dx-analytics.common.css" />
    <link rel="stylesheet" href="~/css/reporting/dx-analytics.light.css" />
    <link rel="stylesheet" href="~/css/reporting/dx-webdocumentviewer.css" />
    <script src="~/js/thirdparty/knockout-latest.js"></script>
    <script src="~/js/dx.all.js"></script>
    <script src="~/js/reporting/dx-analytics-core.min.js"></script>
    <script src="~/js/reporting/dx-webdocumentviewer.min.js"></script>

    @panelRender.RenderScripts()
}
```

## Sending Parameter Values Back to the Server

The panel only collects values on the client — you still need to submit them. A common pattern: add an **Export** button in `BeforeRender`, serialize the parameter state, and post it to a controller action that applies the values and exports the report.

The handler `<script>` block goes at the **top of the view file**, above the `@{ … }` block as follows:

```cshtml
<script>
    function onBeforeRender(sender, e) {
        const panel = sender.GetParametersModel();
        panel.buttons.push({
            text: "Export",
            onClick: () => {
                const data = sender.SerializeParametersState();
                $.ajax({
                    url: "ExportWithParameters",
                    method: "POST",
                    data: { serializedParameters: data, reportUrl: "@Model.ReportInfo.ReportUrl" }
                }).then(result => alert(result.message));
            }
        });
        panel.showButtons = true;
    }
</script>

@{
    var panelRender = Html.DevExpress().ReportParametersPanel("ParametersPanel")
        .Height("100%")
        .Width("750px")
        .ClientSideEvents(configure => configure.BeforeRender("onBeforeRender"))
        .Bind(Model);
    @panelRender.RenderHtml()
}
```

On the server, apply the serialized state with `IReportParametersSerializer.ApplyParametersStateAsync`, then export as needed:

```csharp
using DevExpress.XtraReports.Web.ParametersPanel;
using Microsoft.AspNetCore.Mvc;

[HttpPost]
public async Task<IActionResult> ExportWithParameters(
    [FromServices] IReportParametersSerializer reportParametersSerializer,
    [FromServices] IWebHostEnvironment env,
    [FromForm] string serializedParameters,
    [FromForm] string reportUrl) {

    var report = await reportParametersSerializer.ApplyParametersStateAsync(reportUrl, serializedParameters);
    string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
    string fileDir = Path.Combine(env.ContentRootPath, "GeneratedReports");
    if (!Directory.Exists(fileDir)) Directory.CreateDirectory(fileDir);
    string fileName = $"{Guid.NewGuid():N}_{timestamp}.pdf";
    await report.ExportToPdfAsync(Path.Combine(fileDir, fileName));
    return Ok(new { message = "The report " + reportUrl + " is exported successfully to " + fileDir });
}
```

## Client-Side Events

> **Where the handler `<script>` block goes.** Put it at the **top of the view file**, immediately
> after `@model` / `@page` and above everything else — above the `@{ … }` block, above any
> `@(Html.DevExpress()…)` helper, above every `@section`. This is correct however the page is
> written, so you never have to work out which form yours uses.
>
> Handlers must exist before the component's **init script** runs — emitted by the helper itself, or
> by `@…RenderScripts()`. Defining them after it throws
> `Uncaught ReferenceError: <name> is not defined` and **every customization silently does nothing**,
> while the page still renders normally and the build still succeeds.
>
> The block only *declares* functions, so it does not need `dx.all.js` loaded first — the bodies run
> during initialization, after every library has loaded.
>
> Handler snippets below are **bodies only**. Paste them into that block.

`ReportParametersPanelClientSideEventsBuilder` reuses a subset of the Document Viewer's callbacks — same
name, same signature. Wire them up the same way, via `ReportParametersPanel(...).ClientSideEvents(configure => ...)`.

| Event | Description |
|---|---|
| `BeforeRender` | Fires before the panel's UI is initialized; capture its JS instance here. Generic lifecycle hook — worked example in [references/viewer-customization.md](viewer-customization.md#beforerender--configure-before-ui-initializes). |
| `OnInitializing` | Fires even earlier than `BeforeRender` — before the View Model is fetched from the server. |
| `OnServerError` | Fires on the client each time a server-side error occurs. Worked example in [references/viewer-customization.md](viewer-customization.md#error-handling-onservererror). |
| `CustomizeLocalization` | Substitute the panel's localization strings with custom ones. |
| `CustomizeParameterEditors` | Provide custom editors for report parameters — see below. |
| `CustomizeParameterLookUpSource` | Customize look-up parameter values — see below. |
| `ParametersInitialized` | Fires before parameters are shown to the user; get/modify client-side parameter values here — see below. |

### ParametersInitialized — Set Default Parameter Values

Respond when parameters are initialized on the client before being displayed to the user. This callback allows you to modify parameter values, set invisible parameter values, or filter lookup values.

#### Example: Set Default Parameter Value

```javascript
function onParamsInit(s, e) {
    e.ParametersModel.SetParameterValue("parameter1", 100);
    // Call e.Submit() if you are using the Document Viewer. The standalone Parameters Panel does not build a document, so it has no submit step of its own
}
```

```cshtml
@{
    var panelRender = Html.DevExpress().ReportParametersPanel("ParametersPanel")
        .ClientSideEvents(configure => configure.ParametersInitialized("onParamsInit"))
        .Bind(Model);
}
```

### CustomizeParameterEditors — Replace a Parameter's Editor Widget

Handle `CustomizeParameterEditors` to replace built-in parameter editors with custom ones:

```javascript
<script type="text/html" id="custom-editor">
    <div data-bind="dxNumberBox: { value: value, showSpinButtons: true, min: 1, max: 8 }"></div>
</script>

<script type="text/javascript">
    function onCustomizeParameterEditors(s, e) {
      if (e.parameter.type == "System.Int32") {
          e.info.editor = { header: 'custom-editor' };
      }
    }
</script>
```

### CustomizeParameterLookUpSource — Provide Custom Look-Up Data

`CustomizeParameterLookUpSource` occurs each time a look-up editor is created for a report parameter. Assign `e.dataSource` to a controller
route, a URL, or an in-memory array:

```javascript
var products = ["HD Video Player", "SuperHD Video Player", "SuperPlasma 50"]
    .map(function (val) { return { value: val, displayValue: val }; });

function onCustomizeParameterLookUpSource(s, e) {
    if (e.parameter.name == "parameter1") {
        e.dataSource = "/Home/GetData";
    }
    if (e.parameter.name == "parameter2") {
        e.dataSource = "https://jsonplaceholder.typicode.com/users";
    }
    if (e.parameter.name == "parameter3") {
        e.dataSource = products;
    }
}
```
### Lack of a Submit Event

The standalone Parameters Panel has no submit step of its own as the Document Viewer does. 
Instead, it collects values and hands them back via `SerializeParametersState`.

## Important Notes

- The panel's layout (which parameters appear, grouping) comes from the bound report itself — customize it either in the Report Designer's Parameters Panel editor, or at runtime with `DevExpress.XtraReports.Parameters.ParameterPanelFluentBuilder`.
- Only parameters with `Visible = true` are shown.

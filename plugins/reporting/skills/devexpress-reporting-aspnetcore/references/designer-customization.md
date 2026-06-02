# Report Designer Customization — ASP.NET Core

## When to Use This Reference

Use this when you need to:
- Hide or add toolbar/menu commands in the End-User Report Designer
- Customize the preview toolbar inside the designer
- Remove or add controls in the Toolbox
- Register custom report controls
- Configure the Report Wizard and Data Source Wizard
- Handle designer client-side events

## Toolbar and Menu Customization

Handle the `CustomizeMenuActions` client-side event to customize commands in the designer toolbar and menu.

```cshtml
@{
    var designerRender = Html.DevExpress().ReportDesigner("reportDesigner")
        .Height("1000px")
        .ClientSideEvents(ev => ev.CustomizeMenuActions("onCustomizeMenuActions"))
        .Bind("SalesReport");
}
@designerRender.RenderHtml()
```

```javascript
function onCustomizeMenuActions(s, e) {
    // Hide the "New Report" action
    var newAction = e.GetById(DevExpress.Reporting.Designer.Actions.ActionId.NewReport);
    if (newAction) newAction.visible = false;

    // Hide the "Validate Bindings" action
    var validateAction = e.GetById(DevExpress.Reporting.Designer.Actions.ActionId.ValidateBindings);
    if (validateAction) validateAction.visible = false;

    // Add a custom "Refresh" action with an SVG icon
    e.Actions.splice(15, 0, {
        container: "toolbar",
        text: "Refresh",
        imageTemplateName: "refresh",  // id of a <script type="text/html"> element
        visible: true,
        disabled: false,
        hasSeparator: false,
        hotKey: { ctrlKey: true, keyCode: "Z".charCodeAt(0) },
        clickAction: function () {
            s.GetCurrentTab().refresh();
        }
    });
}
```

**Custom icon via SVG template** (put in the Razor view, before the designer HTML):

```html
<script type="text/html" id="refresh">
    <svg version="1.1" xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24">
        <path class="dxd-icon-fill" d="M22,2v8h-3.09H14l2.94-2.94C15.68,5.79,13.93,5,12,5c-3.87,0-7,3.13-7,7H2
            C2,6.48,6.48,2,12,2c2.76,0,5.26,1.12,7.07,2.93L22,2z" />
    </svg>
</script>
```

> Use `class="dxd-icon-fill"` so the icon color adapts to the designer's color scheme automatically.

Available `DevExpress.Reporting.Designer.Actions.ActionId` values: `NewReport`, `OpenReport`, `Save`, `SaveAs`, `ValidateBindings`, `ReportWizard`.

## Customize the Preview Toolbar (Inside Designer)

Handle `Preview.CustomizeMenuActions` to modify the built-in Document Viewer toolbar inside the designer's preview mode:

```cshtml
@{
    var designerRender = Html.DevExpress().ReportDesigner("reportDesigner")
        .ClientSideEvents(configure =>
            configure.Preview(p => p.CustomizeMenuActions("onPreviewCustomizeMenuActions")))
        .Height("1000px")
        .Bind("SalesReport");
    @designerRender.RenderHtml()
}
```

```javascript
function onPreviewCustomizeMenuActions(s, e) {
    // Hide Print Page from the preview toolbar
    var printPageAction = e.GetById(DevExpress.Reporting.Viewer.ActionId.PrintPage);
    if (printPageAction) printPageAction.visible = false;

    // Add a custom action
    e.Actions.push({
        text: "Custom Command",
        imageTemplateName: "dxrd-svg-wizard-warning",
        hasSeparator: false,
        disabled: ko.observable(false),
        visible: true,
        clickAction: function () { alert("Clicked."); }
    });
}
```

## Client-Side Events Reference

```cshtml
.ClientSideEvents(ev => ev
    .CustomizeMenuActions("onCustomizeMenuActions")   // toolbar/menu
    .CustomizeToolbox("onCustomizeToolbox")           // toolbox controls
    .ReportOpened("onReportOpened")                  // after report loads
    .ReportSaved("onReportSaved")                    // after save completes
    .Preview(p => p
        .CustomizeMenuActions("onPreviewCustomizeMenuActions")
        .DocumentReady("onDocumentReady")))
```

## Toolbox Customization

### Remove a Built-In Control

Handle `CustomizeToolbox` to remove controls from the Toolbox:

```javascript
function onCustomizeToolbox(s, e) {
    var info = e.ControlsFactory.getControlInfo("XRLabel");
    info.isToolboxItem = false;
}
```

### Register a Custom Control

Build the designer model in the controller action and call `CustomControls()`:

```csharp
// Controllers/HomeController.cs
using Microsoft.AspNetCore.Mvc;
using DevExpress.XtraReports.Web.ReportDesigner;
using DevExpress.XtraReports.Web.ReportDesigner.Services;

public class HomeController : Controller {
    public IActionResult Designer(
        [FromServices] IReportDesignerModelBuilder designerModelBuilder) {
        var model = designerModelBuilder
            .Report("SalesReport")
            .CustomControls(typeof(MyCustomControl))
            .BuildModel();
        return View(model);
    }
}
```

```cshtml
@model DevExpress.XtraReports.Web.ReportDesigner.ReportDesignerModel

@{
    var designerRender = Html.DevExpress().ReportDesigner("reportDesigner")
        .Height("1000px")
        .Bind(Model);
    @designerRender.RenderHtml()
}
```

Custom control icon CSS pattern (name based on namespace + class name):
```css
.dxrd-image-mynamespace_mycustomcontrol {
    background-image: url(../images/my-control-icon.png);
    background-repeat: no-repeat;
}
```

## Report Wizard Settings

Configure wizard settings server-side in `ConfigureReportingServices`:

```csharp
builder.Services.ConfigureReportingServices(configurator => {
    configurator.ConfigureReportDesigner(designerConfigurator => {
        designerConfigurator.RegisterDataSourceWizardConfigFileConnectionStringsProvider();
    });
});
```

Or client-side via nested component or event (limit visible data source types, etc.):

```javascript
// Handle CustomizeWizard to hide specific steps
function onCustomizeWizard(s, e) {
    // e.Type is 'ReportWizard' or 'DataSourceWizard'
    // e.WizardModel contains the current wizard model
}
```

## Adding Data Sources in Controller

Build the designer model with pre-configured data sources:

```csharp
using DevExpress.DataAccess.Sql;

public IActionResult Designer([FromServices] IReportDesignerModelBuilder builder) {
    var ds = new SqlDataSource("NWindConnectionString");
    var query = SelectQueryFluentBuilder
        .AddTable("Products")
        .SelectAllColumnsFromTable()
        .Build("Products");
    ds.Queries.Add(query);
    ds.RebuildResultSchema();

    var model = builder
        .Report("SalesReport")
        .DataSources(sources => sources.Add("Northwind", ds))
        .BuildModel();
    return View(model);
}
```

## Important Notes

- All three controllers (ViewerController, DesignerController, QueryBuilderController) are required when the designer is integrated. See [getting-started.md](getting-started.md) Step 6.
- Designer report storage (`ReportStorageWebExtension`) must be registered after `AddDevExpressControls()`. See [report-storage.md](report-storage.md).

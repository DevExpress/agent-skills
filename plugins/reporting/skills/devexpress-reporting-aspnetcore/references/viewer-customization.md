# Document Viewer Customization — ASP.NET Core

## When to Use This Reference

Use this when you need to:
- Hide or add toolbar buttons in the web Document Viewer
- Configure the tab panel (parameters, search, document map, export options)
- Handle client-side events (DocumentReady, ParametersSubmitted, PreviewClick, etc.)
- Customize parameter editors
- Configure export options (hide formats, set options)
- Set up async document creation and progress bar
- Implement skeleton screen for faster perceived loading

## Toolbar Customization

Handle `CustomizeMenuActions` in the viewer's client-side events:

```cshtml
@Html.DevExpress().WebDocumentViewer("DocumentViewer")
    .Height("1000px")
    .ClientSideEvents(ev => ev
        .CustomizeMenuActions("onCustomizeMenuActions")
        .DocumentReady("onDocumentReady"))
    .Bind("SalesReport")
```

```javascript
// wwwroot/js/viewer-customization.js (referenced in _Layout or per-page)
function onCustomizeMenuActions(s, e) {
    // Hide the Print button
    var printAction = e.GetById(DevExpress.Reporting.Viewer.ActionId.Print);
    if (printAction) printAction.visible = false;

    // Hide XLS export format
    e.Actions.filter(a => a.text === "Export Document").forEach(a => {
        if (a.exportType === DevExpress.Reporting.Viewer.ExportFormatID.XLS)
            a.visible = false;
    });
}

function onDocumentReady(s, e) {
    // Navigate to last page when document is ready
    s.GoToPage(e.PageCount - 1);
}
```

Available `ActionId` values: `Print`, `ExportTo`, `Search`, `HighlightEditingFields`, `DrillDown`, `FirstPage`, `PrevPage`, `NextPage`, `LastPage`, `ZoomIn`, `ZoomOut`.

## Tab Panel Configuration

```cshtml
@Html.DevExpress().WebDocumentViewer("DocumentViewer")
    .Height("1000px")
    .TabPanelSettings(s => {
        s.Position = "Left"; // or "Right" (default)
    })
    .ClientSideEvents(ev => ev.CustomizeElements("onCustomizeElements"))
    .Bind("SalesReport")
```

```javascript
// Hide the Parameters tab
function onCustomizeElements(s, e) {
    var parametersPanel = e.GetById(DevExpress.Reporting.Viewer.PreviewElements.RightPanel);
    if (parametersPanel) parametersPanel.visible = false;
}
```

## Export Options

```cshtml
.ClientSideEvents(ev => ev.CustomizeExportOptions("onCustomizeExportOptions"))
```

```javascript
function onCustomizeExportOptions(s, e) {
    // Hide XLS format from the export menu
    e.HideFormat(DevExpress.Reporting.Viewer.ExportFormatID.XLS);
    // Hide specific export options
    // e.HideProperties(DevExpress.Reporting.Viewer.ExportFormatID.PDF, "PasswordSecurityOptions");
}
```

Available `ExportFormatID` values: `CSV`, `DOCX`, `HTML`, `IMAGE`, `MHT`, `PDF`, `RTF`, `TEXT`, `XLS`, `XLSX`.

## Client-Side Events Reference

```cshtml
.ClientSideEvents(ev => ev
    .BeforeRender("onBeforeRender")           // viewer init, capture JS reference
    .DocumentReady("onDocumentReady")         // report rendered, e.PageCount available
    .ParametersInitialized("onParamsInit")    // before parameters shown; call e.Submit() to auto-submit
    .ParametersSubmitted("onParamsSubmitted") // after Submit; modify e.Parameters
    .PreviewClick("onPreviewClick")           // brick click; e.Brick.content, e.BrickText
    .CustomizeMenuActions("onCustomizeMenuActions")
    .CustomizeExportOptions("onCustomizeExportOptions")
    .CustomizeElements("onCustomizeElements")
    .OnServerError("onServerError"))
```

Capture the viewer JS instance in `BeforeRender`:

```javascript
var viewer;
function onBeforeRender(s, e) {
    viewer = s; // now you can call viewer.GoToPage(), viewer.StartBuild(), etc.
}
```

## Parameter Editors

```cshtml
.ClientSideEvents(ev => ev.CustomizeParameterEditors("onCustomizeParameterEditors"))
```

```javascript
function onCustomizeParameterEditors(s, e) {
    if (e.parameter.name === "DateParam") {
        e.info.editor = $.extend({}, DevExpress.Analytics.Widgets.ObjectProperties.editors.date);
        e.info.editor.extendedOptions = {
            applyValueMode: "instantly"
        };
    }
}
```

## Auto-Submit Parameters

To automatically build the document when the page loads (skip the parameter input form):

```javascript
function onParamsInit(s, e) {
    // Set parameter values programmatically, then submit
    e.ParametersModel.setParameterValue("StartDate", new Date(2025, 0, 1));
    e.ParametersModel.setParameterValue("Region", "North");
    e.Submit(); // trigger document build without showing the parameter panel
}
```

## Progress Bar

```cshtml
@Html.DevExpress().WebDocumentViewer("DocumentViewer")
    .ProgressBarSettings(s => {
        s.Position = DevExpress.XtraReports.Web.WebDocumentViewer.ProgressBarPosition.TopRight;
    })
    .Bind("SalesReport")
```

## Search Panel

```cshtml
.SearchSettings(s => {
    s.SearchEnabled = true;
    // s.UseAsyncSearch = true; // recommended for large documents
})
```

## Async Document Generation (UseAsyncEngine)

For long-running reports, enable async engine:

```csharp
// Program.cs
builder.Services.ConfigureReportingServices(configurator => {
    configurator.UseAsyncEngine(); // enables IReportProviderAsync, IWebDocumentViewerReportResolverAsync
    configurator.ConfigureWebDocumentViewer(c => c.UseCachedReportSourceBuilder());
});
```

With `UseAsyncEngine`, implement `IReportProviderAsync` instead of `IReportProvider`:

```csharp
public class AsyncReportProvider : IReportProviderAsync {
    public async Task<XtraReport> GetReportAsync(string id, ReportProviderContext ctx) {
        var report = new SalesReport();
        report.DataSource = await GetDataAsync();
        return report;
    }
}
```

## Development Mode

Enable in development for verbose error output and version validation:

```csharp
if (builder.Environment.IsDevelopment())
    configurator.UseDevelopmentMode();
```

And in `appsettings.Development.json`:
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "DevExpress": "Debug"
    }
  }
}
```

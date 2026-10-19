# Document Viewer Customization

## When to Use This Reference

Use this when you need to:
- Hide, add, or reorder toolbar buttons, or look up the exact `ActionId` for a built-in command before hiding/showing it
- Filter or reconfigure export formats shown in the Export To drop-down and Export Options panel, or let designer-configured PDF/Excel export passwords reach the Viewer
- Remove entire UI panels (toolbar and tab panel)
- Handle client-side events (DocumentReady, ParametersSubmitted, PreviewClick, etc.)
- Configure the progress bar for long-running documents
- Display report date-time values in the viewer's local time zone (`UtcToLocalTime` / `ITimeZoneProvider`)

## When Not to Use This Reference
- Customize the Viewer's color scheme — see [theming.md](theming.md)
- Customize parameter editors and look-up values — worked examples live in [standalone-parameters-panel.md](standalone-parameters-panel.md#client-side-events), since these events are shared with the standalone Parameters Panel

## Basic Setup

```cshtml
@(Html.DevExpress().WebDocumentViewer("DocumentViewer")
    .Height("1000px")
    .Bind(new TestReport()))
```

## WebDocumentViewerBuilder API Reference

`Html.DevExpress().WebDocumentViewer(...)` returns a `WebDocumentViewerBuilder`. Beyond the basic `Bind`/`Height` call chain, it commonly exposes:

| Member | Description |
|---|---|
| `Bind("SalesReport")` | Loads a report by name from the registered `IReportProvider`/`ReportStorageWebExtension` and binds it to the viewer. |
| `Bind(new SalesReport())` | Binds the viewer to a report instance (`XtraReport`). |
| `Bind(Model)` | Binds a prebuilt client model (a `WebDocumentViewerModel` built in the controller via `IWebDocumentViewerClientSideModelGenerator.GetModel`/`GetModelAsync`) — used in some customization usage scenarios. See [controller-view-binding.md](controller-view-binding.md#document-viewer) (Style 3). |
| `Height(string)` / `Width(string)` | Sets the control's height/width on the page. |
| `ClientSideEvents(...)` | Wires up client-side events — see the event reference below. |
| `CssClassName(string)` | Applies a CSS class to the rendered viewer container. Required for sizing under CSP — see [security.md](security.md#content-security-policy-csp-with-nonce). |
| `Nonce(string)` | Sets the CSP nonce value for the viewer's inline scripts — see [security.md](security.md#content-security-policy-csp-with-nonce). |
| `TabPanelSettings(...)` | Configures the right tab panel (for example, position and width). |
| `ProgressBarSettings(...)` | Configures the viewer progress indicator. |
| `SearchSettings(...)` | Configures the integrated search panel behavior. |


## Setup Client-Side Events

> **Where the handler `<script>` block goes.** Put it at the **top of the view file**, immediately
> after `@model` / `@page` and above everything else — above the `@{ … }` block, above any
> `@(Html.DevExpress()…)` helper, `@…RenderScripts()`, above every `@section`. 
> 
> If the page instead splits rendering (`@viewerRender.RenderHtml()` in the body and
> `@viewerRender.RenderScripts()` inside `@section Scripts`), the `<script>` block still goes at the
> top of the file and `RenderScripts()` stays last in the section.
>
> Defining them after it throws `Uncaught ReferenceError: <name> is not defined` and **every customization silently does nothing**, while the page still renders normally and the build still succeeds.
>
> The block only *declares* functions, so it does not need `dx.all.js` loaded first — the bodies run
> during initialization, after every library has loaded.

Handler snippets below are **bodies only**. Paste them into that block.

Wire up any of the required events in one `ClientSideEvents(...)` call:

```cshtml
@model DevExpress.XtraReports.Web.WebDocumentViewer.WebDocumentViewerModel

<script>
    function onCustomizeMenuActions(s, e) { /* body from this reference */ }
    function onBeforeRender(s, e) { /* body from this reference */ }
</script>

@(Html.DevExpress().WebDocumentViewer("DocumentViewer")
    .Height("1000px")
    .ClientSideEvents(ev => ev
        .OnInitializing("onInitializing")
        .BeforeRender("onBeforeRender")
        .DocumentReady("onDocumentReady")
        .ParametersInitialized("onParamsInit")
        .ParametersReset("onParamsReset")
        .ParametersSubmitted("onParametersSubmitted")
        .PreviewClick("onPreviewClick")
        .EditingFieldChanged("onEditingFieldChanged")
        .CustomizeMenuActions("onCustomizeMenuActions")
        .CustomizeExportOptions("onCustomizeExportOptions")
        .CustomizeElements("onCustomizeElements")
        .CustomizeParameterEditors("onCustomizeParameterEditors")
        .CustomizeParameterLookUpSource("onCustomizeParameterLookUpSource")
        .CustomizeLocalization("onCustomizeLocalization")
        .OnExport("onExport")
        .OnServerError("onServerError"))
    .Bind("SalesReport"))
```

## All Available Client-Side Events — Reference Table

| Event | Description |
|---|---|
| `CustomizeElements` | Customize the Web Document Viewer's UI elements (hide/reorder panels) — see "Hide an Entire UI Panel" below. |
| `CustomizeMenuActions` | Customize the Web Document Viewer's toolbar — see "Toolbar Customization" below. |
| `CustomizeExportOptions` | Customize the available export formats and their export options — see "Customize Export Formats" below. |
| `CustomizeParameterEditors` | Provide custom editors for report parameters — worked example in [standalone-parameters-panel.md](standalone-parameters-panel.md#customizeparametereditors--replace-a-parameters-editor-widget). |
| `CustomizeParameterLookUpSource` | Customize look-up parameter values — worked example in [standalone-parameters-panel.md](standalone-parameters-panel.md#customizeparameterlookupsource--provide-custom-look-up-data). |
| `ParametersInitialized` | Fires before parameters are shown to the user; get/modify client-side parameter values, or call `e.Submit()` to auto-submit and skip the parameter panel. Worked example in [standalone-parameters-panel.md](standalone-parameters-panel.md#parametersinitialized--auto-submit-default-parameters). |
| `ParametersReset` | Fires after report parameter values are reset to their default values — see "Parameter Events" below. |
| `ParametersSubmitted` | Fires after report parameter values are submitted; modify `e.Parameters` here. Viewer-only — see "Parameter Events" below. |
| `DocumentReady` | Fires after the Web Document Viewer loads a report document; `e.PageCount` is available. |
| `PreviewClick` | Fires when a report document is clicked. |
| `EditingFieldChanged` | Fires each time an editing field's value changes. |
| `CustomizeLocalization` | Substitute the Web Document Viewer's localization strings with custom ones. |
| `OnServerError` | Fires on the client each time a server-side error occurs. |
| `BeforeRender` | Fires before the Web Document Viewer UI is initialized; capture the viewer's JS instance here. |
| `OnInitializing` | Fires even earlier than `BeforeRender` — before the View Model is fetched from the server. Use it for setup that must happen before any server round-trip. |
| `OnExport` | Fires before a print/export request is sent to the server; `e.RequestUrl`/`e.FormData` let you pass extra data. |

## Hide an Entire UI Panel (CustomizeElements)

Use `CustomizeElements` and `e.GetById(PreviewElements.X)` to remove the toolbar (`PreviewElements.Toolbar` ) or right panel (`PreviewElements.RightPanel`) from the viewer entirely:

```javascript
function onCustomizeElements(s, e) {
    var toolbarPart = e.GetById(DevExpress.Reporting.Viewer.PreviewElements.Toolbar);
    if (toolbarPart) {
        var index = e.Elements.indexOf(toolbarPart);
        if (index !== -1) e.Elements.splice(index, 1);
    }

    var rightPanelPart = e.GetById(DevExpress.Reporting.Viewer.PreviewElements.RightPanel);
    if (rightPanelPart) {
        var index = e.Elements.indexOf(rightPanelPart);
        if (index !== -1) e.Elements.splice(index, 1);
    }
}
```

## Toolbar Customization (CustomizeMenuActions)

### Hide a Built-In Toolbar Button

Use `CustomizeMenuActions` event and its `e.GetById(ActionId)` to find a built-in command, then set `visible = false`:

```javascript
function onCustomizeMenuActions(s, e) {
    var printAction = e.GetById(DevExpress.Reporting.Viewer.ActionId.Print);
    if (printAction) printAction.visible = false;
}
```

**All `DevExpress.Reporting.Viewer.ActionId` values**:

| ActionId | Description |
|---|---|
| `ActionId.Print` | Print button |
| `ActionId.PrintPage` | Print current page button |
| `ActionId.ExportTo` | Export To drop-down |
| `ActionId.Search` | Search panel toggle |
| `ActionId.FirstPage` | First page navigation |
| `ActionId.PrevPage` | Previous page navigation |
| `ActionId.NextPage` | Next page navigation |
| `ActionId.LastPage` | Last page navigation |
| `ActionId.Pagination` | The drop-down list that navigates to the selected page |
| `ActionId.MultipageToggle` | Toggle multi-page view |
| `ActionId.HighlightEditingFields` | Highlight editing fields |
| `ActionId.FullScreen` | Full screen toggle |
| `ActionId.ZoomOut` | Decrease zoom |
| `ActionId.ZoomIn` | Increase zoom |
| `ActionId.ZoomSelector` | The drop-down list with available zoom factors |

> **Verifying the hide.** Toolbar drop-downs and menus are popups that are not in the DOM until opened, so a text scan of the loaded page proves nothing — it matches unrelated elements and reports the command as still present. Open the menu (or toolbar) in the browser, take a snapshot, and confirm the command is absent. If you cannot drive the UI, report the customization as `NOT VERIFIED` rather than inferring it from page text.

### Add a Custom Toolbar Command

Handle `CustomizeMenuActions` and push an action object into `e.Actions`:

```javascript
function onCustomizeMenuActions(s, e) {
    e.Actions.push({
        text: "My Command",
        disabled: false,
        hasSeparator: false,
        hotKey: { ctrlKey: true, keyCode: "Z".charCodeAt(0) },
        imageTemplateName: "myIcon",   // id of a <script type="text/html"> SVG template
        visible: true,
        clickAction: function () { /* s.GetPreviewModel() exposes the preview API */ }
    });
}
```

Supply the icon as an SVG `<script type="text/html" id="myIcon">…</script>` template and use `class="dxd-icon-fill"` on the SVG so it follows the color scheme. Full walkthrough (runtime and design-time commands, hotkeys, custom icons): [Customize the Document Viewer Toolbar (ASP.NET Core)](https://docs.devexpress.com/XtraReports/400270).

## Customize Export Formats (CustomizeExportOptions)

### Hide Built-In Formats

Use `CustomizeExportOptions` event to hide built-in export formats from the `Export To` drop-down and the export options side panel:

```javascript
function onCustomizeExportOptions(s, e) {
    // Hide specific formats
    e.HideFormat(DevExpress.Reporting.Viewer.ExportFormatID.XLS);
    e.HideFormat(DevExpress.Reporting.Viewer.ExportFormatID.XLSX);
    e.HideFormat(DevExpress.Reporting.Viewer.ExportFormatID.CSV);
    e.HideFormat(DevExpress.Reporting.Viewer.ExportFormatID.DOCX);
    e.HideFormat(DevExpress.Reporting.Viewer.ExportFormatID.RTF);
    e.HideFormat(DevExpress.Reporting.Viewer.ExportFormatID.HTML);
    e.HideFormat(DevExpress.Reporting.Viewer.ExportFormatID.MHT);
    e.HideFormat(DevExpress.Reporting.Viewer.ExportFormatID.Image);
    e.HideFormat(DevExpress.Reporting.Viewer.ExportFormatID.Text);
    // PDF (ExportFormatID.PDF) remains visible
}
```

**All `ExportFormatID` values:** `CSV`, `DOCX`, `HTML`, `Image`, `MHT`, `PDF`, `RTF`, `Text`, `XLS`, `XLSX`. Note the exact casing.

### Add a Custom Export Format Entry

To add a custom entry to the Export To drop-down, handle `CustomizeMenuActions`, get the `ActionId.ExportTo` action with `e.GetById(...)`, and push a `{ format, text }` object onto its items. Full snippet: [Customize the Document Viewer Toolbar (ASP.NET Core) — Add a New Export Format](https://docs.devexpress.com/XtraReports/400270).

### Hide Specific Export Options

```javascript
function onCustomizeExportOptions(s, e) {
    // Hide specific options for one format...
    e.HideProperties(DevExpress.Reporting.Viewer.ExportFormatID.XLS, "ExportMode", "PageRange");
    // ...or hide every option for a format (leaves only the format itself visible).
    e.HideProperties(DevExpress.Reporting.Viewer.ExportFormatID.XLSX);
}
```

### Change a Format's Default Option Value

```javascript
function onCustomizeExportOptions(s, e) {
    var model = e.GetExportOptionsModel(DevExpress.Reporting.Viewer.ExportFormatID.CSV);
    model.separator = "|";
}
```

### Honor Designer-Configured Export Passwords

For security reasons, PDF/Excel passwords specified in the Report Designer's export options are **not** exposed to the Web Document Viewer by default — the Viewer ignores them and the exported file ends up with no password at all. Opt in explicitly:

```csharp
// Program.cs
builder.Services.ConfigureReportingServices(configurator => {
    configurator.ConfigureWebDocumentViewer(viewerConfigurator => {
        viewerConfigurator.EnablePassingExportOptionsPasswordsToClient();
    });
});
```

> Enabling this sends the configured passwords to the client as part of the export options model. Only turn it on when end users are trusted with those passwords — otherwise apply the password server-side instead.

## Customize the Tab Panel

### Specify the Tab Panel Width and Position

Use the viewer's `TabPanelSettings` nested property:

```cshtml
@(Html.DevExpress().WebDocumentViewer("DocumentViewer")
    .Height("1000px")
    .TabPanelSettings(s => {
        s.Position = "Left"; // or "Right" (default)
        s.Width = 400;       // int, panel width in pixels (default 340)
    })
    .Bind("SalesReport"))
```
### Hide the Preview Parameters Panel Tab

To hide the Preview Parameters tab and automatically submit default values to all parameters, set the `Parameter.Visible` property to `false` for all report parameters.

### Hide the Entire Export Options Panel

Handle `CustomizeExportOptions` and call the `HideExportOptionsPanel` method:

```javascript
function onCustomizeExportOptions(s, e) {
    e.HideExportOptionsPanel();
}
```

## Lifecycle Events

### DocumentReady — Navigate to Last Page on Load

Handle `DocumentReady` to access the viewer's preview model and navigate to a specific page after the document is loaded:

```javascript
function onDocumentReady(s, e) {
    var previewModel = s.GetPreviewModel();
    previewModel.GoToPage(e.PageCount - 1); // e.g. jump to the last page
}
```

### BeforeRender — Configure Before UI Initializes

Use this event for global viewer configuration applied before the UI renders. Common use cases include setting zoom levels, display modes, and other pre-render settings.

#### Example: Set Custom Zoom Level and Enable Multi-Page Mode

```javascript
function onBeforeRender(s, e) {
    e.reportPreview.zoom = 0.25;
    e.reportPreview.showMultipagePreview = true;
}
```
You can also use the following values of the DevExpress.Reporting.Viewer.ZoomAutoBy object as a function parameter: None, WholePage and PageWidth.

## Parameter Events

### ParametersSubmitted — Respond After Submission

Respond after the user submits parameters. This callback fires once parameters have been processed and are ready for the report to render with new values.

#### Example: Collapse Parameter Panel and Reset One Parameter After Submitting Parameters

```javascript
function onParametersSubmitted(s, e) {
    e.Parameters.filter(function (p) { return p.Key == "YourParameterNameHere"; })[0].Value = "SomeValue";
    var preview = s.GetPreviewModel();
    if (preview) {
        preview.tabPanel.collapsed = true;
    }
}
```

### ParametersReset — Respond When Parameters Are Reset

Fires after report parameter values are reset to their default values:

```javascript
function onParamsReset(s, e) {
    // collapses the Parameters panel after parameter values are reset
    var preview = s.GetPreviewModel();
    if (preview) {
        preview.tabPanel.collapsed = true;
    }
}
```

### ParametersInitialized, CustomizeParameterEditors, CustomizeParameterLookUpSource

These three events are shared with the standalone Report Parameters Panel — same name, same signature. For
worked examples of each, see [references/standalone-parameters-panel.md](standalone-parameters-panel.md#client-side-events).

## Error Handling (OnServerError)

Handle `OnServerError` event to respond to server-side errors on the client to show custom messages or log them:

```javascript
function onServerError(s, e) {
    console.error("Viewer server error:", e.Error.errorThrown);
}
```

## Interactivity Events

### PreviewClick — Respond to Document Area Clicks

The event args include the following:

* PageIndex - A zero-based index of the clicked page. 
* Brick - A visual brick object that is related to the clicked report control.
* Handled - Specifies whether default event handling is required.
* DefaultHandler - A function that runs when the Handled field is set to false (the default value). 
* GetBrickText() - Returns the text displayed in the clicked element.
* GetBrickValue() - Returns additional information on the brick.

The following example logs the text of the clicked brick:
```javascript
function onPreviewClick(s, e) {
    var brickText = e.GetBrickText();
    console.log("Clicked brick text:", brickText, "on page", e.PageIndex);
}
```
### EditingFieldChanged — Track Edit Field Changes

The event args include the following:
* Field - An editing field whose value has been changed.
* NewValue - The new value of an editing field.
* OldValue - The previous value of an editing field.

The following example shows how to change an editing field’s value to the previous value if the new value does not meet the required conditions:
```javascript
function onEditingFieldChanged(s, e) {
    if ((e.Field.id() === "UnitsInStock") && (e.NewValue > 100))
        e.NewValue = e.OldValue;
}
```

## Progress Bar

```cshtml
@(Html.DevExpress().WebDocumentViewer("DocumentViewer")
    .ProgressBarSettings(s => {
        s.Position = DevExpress.XtraReports.Web.WebDocumentViewer.ProgressBarPosition.TopRight;
    })
    .Bind("SalesReport"))
```

## Search Panel

```cshtml
@(Html.DevExpress().WebDocumentViewer("DocumentViewer")
    .SearchSettings(s => {
        s.SearchEnabled = true;
        // s.UseAsyncSearch = true; // recommended for large documents
    })
    .Bind("SalesReport"))
```

## Convert UTC to Local Time

Reports can display UTC date-time values in the viewer's local time zone — and convert user-entered parameter values back to UTC — with the `UtcToLocalTime()` and `LocalTimeToUtc()` expression functions. The local zone is supplied by a custom `ITimeZoneProvider` (`DevExpress.XtraReports.Services`) registered at startup (for example, resolving the client's zone from a cookie). The same provider governs the Web Document Viewer and the Report Designer's integrated Preview alike, since both render through the Web Document Viewer engine.

Full ASP.NET Core setup (provider, service registration, client cookie): [Convert Time Zones (ASP.NET Core)](https://docs.devexpress.com/XtraReports/405644). Using the functions in expressions, calculated fields, and filters: [Convert Time Zones](https://docs.devexpress.com/XtraReports/405682).

## Color Scheme

The Document Viewer supports numerous predefined color schemes and custom schemes via CSS. See [theming.md](theming.md) for how to apply a predefined scheme or build a custom one.

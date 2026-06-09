# DxDocumentViewer Customization — JavaScript-Based Document Viewer

## When to Use This Reference

Use this when you need to customize `DxDocumentViewer` (Blazor Server) or `DxWasmDocumentViewer` (Blazor WASM):
- Hide, add, or reorder toolbar commands
- Filter export formats shown in the Export To drop-down
- Remove entire UI panels (toolbar, right panel, parameter panel)
- Respond to document loading, parameter changes, or user clicks
- Provide custom parameter editors or look-up sources
- Handle server-side errors on the client
- Localize UI strings

ALL customization for `DxDocumentViewer` / `DxWasmDocumentViewer` is done in **JavaScript** via `<DxDocumentViewerCallbacks>`.

## Required Setup

### 1. JavaScript File (wwwroot/customization.js)

Place all callback functions under a window-level namespace:

```javascript
window.ViewerCustomization = {
    onCustomizeMenuActions: function (s, e) {
        // toolbar customization
    },
    onCustomizeExportOptions: function (s, e) {
        // export format filtering
    },
    onDocumentReady: function (s, e) {
        // respond to document load
    }
};
```

### 2. Register the Script (App.razor)

Use `DxResourceManager.RegisterScripts` to load the file after DevExpress scripts:

```razor
<head>
    @* ...other head content... *@
    @DxResourceManager.RegisterScripts((config) => config.Register(new DxResource("/customization.js", 900)))

</head>
```

> The `900` priority ensures your script loads after all DevExpress scripts (which use lower priority numbers).

### 3. Attach Callbacks to the Viewer Component

`DxDocumentViewer`:
```razor
<DxDocumentViewer ReportName="SalesReport" Height="1000px" Width="100%">
    <DxDocumentViewerCallbacks
        CustomizeMenuActions="ViewerCustomization.onCustomizeMenuActions"
        CustomizeExportOptions="ViewerCustomization.onCustomizeExportOptions"
        DocumentReady="ViewerCustomization.onDocumentReady" />
</DxDocumentViewer>
```

Same pattern for `DxWasmDocumentViewer`:

```razor
<DxWasmDocumentViewer ReportName="SalesReport" Height="1000px" Width="100%">
    <DxWasmDocumentViewerRequestOptions InvokeAction="DXXRDV" />
    <DxDocumentViewerCallbacks
        CustomizeMenuActions="ViewerCustomization.onCustomizeMenuActions"
        CustomizeExportOptions="ViewerCustomization.onCustomizeExportOptions"
        DocumentReady="ViewerCustomization.onDocumentReady" />
</DxWasmDocumentViewer>
```

---

## All Available Callbacks — Reference Table

| Callback | Concern | Description |
|----------|---------|-------------|
| `CustomizeMenuActions` | Toolbar | Add, hide, or reorder toolbar buttons and Export To items |
| `CustomizeExportOptions` | Export | Filter export formats and their options panels |
| `CustomizeElements` | UI structure | Hide or reorder entire UI panels (toolbar, right panel, parameter panel) |
| `CustomizeParameterEditors` | Parameters | Replace built-in parameter input widgets with custom editors |
| `CustomizeParameterLookUpSource` | Parameters | Override look-up data source for parameter editors |
| `CustomizeLocalization` | Localization | Replace default localization strings |
| `ParametersInitialized` | Parameters | Modify parameter values client-side before they are displayed |
| `ParametersReset` | Parameters | Respond when parameters are reset to defaults |
| `ParametersSubmitted` | Parameters | Respond when the user submits parameters |
| `DocumentReady` | Lifecycle | Fires after the document is loaded and rendered |
| `BeforeRender` | Lifecycle | Fires before the viewer UI is initialized (use for global config) |
| `OnInitializing` | Lifecycle | Fires before the view model is fetched from server (earliest hook) |
| `OnServerError` | Errors | Fires on the client when a server-side error occurs |
| `PreviewClick` | Interactivity | Fires when the user clicks the document area |
| `EditingFieldChanged` | Interactivity | Fires when a report editing field value changes |
| `OnExport` | Export | Fires before a print/export request, allows passing extra data to server |


All these callbacks can coexist in one `<DxDocumentViewerCallbacks>` component — they are independent concerns.

---

## Hide Entire UI Panels (CustomizeElements)

Use `CustomizeElements` JavaScript callback to remove the toolbar (`PreviewElements.Toolbar` ) or right panel (`PreviewElements.RightPanel`) from the viewer entirely:

```javascript
window.ViewerCustomization = {
    onCustomizeElements: function (s, e) {
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
};
```
---
## Toolbar Customization (CustomizeMenuActions)

### Hide a Built-in Toolbar Button

Use `CustomizeMenuActions` JavaScript callback and its `e.GetById(ActionId)` to find a built-in command, then set `visible = false`:

```javascript
window.ViewerCustomization = {
    onCustomizeMenuActions: function (s, e) {
        var printAction = e.GetById(DevExpress.Reporting.Viewer.ActionId.Print);
        if (printAction) printAction.visible = false;
    }
};
```

**Available `ActionId` values:**

| ActionId | Description |
|----------|-------------|
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

### Add a Custom Toolbar Command

Use `CustomizeMenuActions` JavaScript callback to push a new action object into `e.Actions`:

```javascript
window.ViewerCustomization = {
    onCustomizeMenuActions: function (s, e) {
        var interval;
        var selected = ko.observable(false);
        e.Actions.push({
            text: "Run Slide Show",
            imageTemplateName: "slideShow",   // SVG template ID in App.razor <head>
            visible: true,
            disabled: false,
            selected: selected,
            hasSeparator: false,
            hotKey: { ctrlKey: true, keyCode: "Z".charCodeAt(0) },
            clickAction: function () {
                if (selected()) {
                    clearInterval(interval);
                    selected(false);
                    return;
                }
                var model = s.GetPreviewModel();
                if (model) {
                    selected(true);
                    interval = setInterval(function () {
                        var pageIndex = model.GetCurrentPageIndex();
                        model.GoToPage(pageIndex + 1);
                    }, 2000);
                }
            }
        });
    }
};
```

To use a custom SVG icon, declare a script template in App.razor:

```razor
<head>
    @* ... *@
    <script type="text/html" id="slideShow">
        <svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 24 24">
            <polygon class="dxd-icon-fill" points="4,2 4,22 22,12" />
        </svg>
    </script>
</head>
```

## Customize Export Formats (CustomizeExportOptions)

### Hide Built-in Formats 

Use `CustomizeExportOptions` JavaScript callback to hide built-in export formats from the `Export To` drop-down and the export options side panel:
```javascript
window.ViewerCustomization = {
    onCustomizeExportOptions: function (s, e) {
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
};
```

**All `ExportFormatID` values:** `CSV`, `DOCX`, `HTML`, `Image`, `MHT`, `PDF`, `RTF`, `Text`, `XLS`, `XLSX`.


### Add a Custom Export Format Entry

Use `CustomizeMenuActions` JavaScript callback to inject a custom format into the Export To drop-down:

```javascript
window.ViewerCustomization = {
    onCustomizeMenuActions: function (s, e) {
        const actionExportTo = e.GetById(DevExpress.Reporting.Viewer.ActionId.ExportTo);
        const newFormat = { format: 'powerPoint', text: 'Power Point' };
        if (actionExportTo) {
            actionExportTo.events.on('propertyChanged', (args) => {
                const formats = actionExportTo.items[0].items;
                if (args.propertyName === 'items' && formats.indexOf(newFormat) === -1) {
                    formats.push(newFormat);
                }
            });
        }
    }
};
```

---

## Customize the Tab Panel

### Specify the Tab Panel Width and Position

Use the viewer's `DxDocumentViewerTabPanelSettings` nested property:

```razor
<DxDocumentViewer ReportName="TestReport" Height="calc(100vh - 130px)" Width="100%">
    <DxDocumentViewerTabPanelSettings Position=TabPanelPosition.Left Width="400" />
</DxDocumentViewer>
```

### Hide the Preview Parameters Panel Tab


To hide the Preview Parameters tab and automatically submit default values to all parameters, set the `Parameter.Visible` property to `false` for all report parameters.

### Hide the Export Panel Tab
Handle the `CustomizeExportOptions` JavaScript callback and call the   `ASPxClientCustomizeExportOptionsEventArgs.HideExportOptionsPanel` method:

```javascript
window.ViewerCustomization = {
    onCustomizeExportOptions: function (s, e) {
        e.HideExportOptionsPanel();
    }
};
```
---

## Lifecycle Events

### DocumentReady — Navigate to Last Page on Load

Handle the `DocumentReady` JavaScript callback to access the viewer's preview model and navigate to a specific page after the document is loaded:

```javascript
window.ViewerCustomization = {
    onDocumentReady: function (s, e) {
        var previewModel = s.GetPreviewModel();
        // Navigate to the exact page, for instance to the last page
        previewModel.GoToPage(e.PageCount - 1);
        // Alternative: auto-advance pages every 3 seconds
        var goToNextPage = function () {
            var pageIndex = previewModel.GetCurrentPageIndex();
            if (e.PageCount <= pageIndex)
                return;
            previewModel.GoToPage(pageIndex + 1);
            setTimeout(function () { goToNextPage(); }, 3000);
        }
        goToNextPage();
    }
};
```

### BeforeRender — Configure Before UI Initializes

Use this callback for global viewer configuration applied before the UI renders. Common use cases include setting zoom levels, display modes, and other pre-render settings.

#### Example: Set Custom Zoom Level and Enable Multi-Page Mode

Handle the `BeforeRender` JavaScript callback to set the zoom level to 25% and enable multi-page mode:

```javascript
window.ViewerCustomization = {
    onBeforeRender: function (s, e) {
        e.reportPreview.zoom = 0.25;
        e.reportPreview.showMultipagePreview = true;
    }
};
```

---

## Parameter Events

### ParametersInitialized — Modify Parameters Before Display

Respond when parameters are initialized on the client before being displayed to the user. This callback allows you to modify parameter values, set invisible parameter values, or filter lookup values.

#### Example: Set Default Parameter Value

```javascript
window.ViewerCustomization = {
    onParametersInitialized: function (s, e) {
        e.ParametersModel.SetParameterValue("parameter1", 100);
        e.Submit();
    }
};
```

### ParametersSubmitted — Respond After Submission

Respond after the user submits parameters. This callback fires once parameters have been processed and are ready for the report to render with new values.

#### Example: Collapse Parameter Panel After Submitting Parameters

```javascript
window.ViewerCustomization = {
    onParametersSubmitted: function (s, e) {
        // Collapse the Parameters panel when a user clicks the Submit button
        e.Parameters.filter(function (p) { return p.Key == "YourParameterNameHere"; })[0].Value = "SomeValue";
        var preview = s.GetPreviewModel();
        if (preview) {
            preview.tabPanel.collapsed = true;
        }
    }
};
```

### CustomizeParameterEditors — Replace Parameter Editor Widgets

Handle CustomizeParameterEditors JavaScript callback to replace built-in parameter editors with custom ones:

```javascript
window.ViewerCustomization = {
    onCustomizeParameterEditors: function (s, e) {
        if (e.parameter.type == "System.Int32") {
            e.info.editor = { header: 'custom-editor' };
        }    
    }
};
```

Custom editor template in App.razor:
```javascript
<script type ="text/html" id="custom-editor"> 
    <div data-bind="dxNumberBox: { value: value, showSpinButtons: true, min: 1, max: 8 }"> </div> 
</script> 
```

---

## Error Handling (OnServerError)

Handle `OnServerError` Javascript Callback to respond to server-side errors on the client to show custom messages or log them:

```javascript
window.ViewerCustomization = {
    onServerError: function (s, e) {
        console.error("Viewer server error:", e.Error.errorThrown);
    }
};
```

---

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
window.ViewerCustomization = {
    onPreviewClick: function (s, e) {
        var brickText = e.GetBrickText();
        console.log("Clicked brick text:", brickText, "on page", e.PageIndex);
    }
};
```


### EditingFieldChanged — Track Edit Field Changes

The event args include the following:
* Field - An editing field whose value has been changed.
* NewValue - The new value of an editing field.
* OldValue - The previous value of an editing field.

The following example shows how to change an editing field’s value to the previous value if the new value does not meet the required conditions:
```javascript
window.ViewerCustomization = {
    onEditingFieldChanged: function (s, e) {
        if ((e.Field.id() === "UnitsInStock") && (e.NewValue > 100))
            e.NewValue = e.OldValue;
    }
};
```
---

## Complete Example — Multiple Callbacks Together

```razor
@page "/viewer"
@rendermode InteractiveServer

<DxDocumentViewer ReportName="SalesReport" Height="1000px" Width="100%">
    <DxDocumentViewerTabPanelSettings Width="340" />
    <DxDocumentViewerCallbacks
        CustomizeMenuActions="ViewerCustomization.onCustomizeMenuActions"
        CustomizeExportOptions="ViewerCustomization.onCustomizeExportOptions"
        DocumentReady="ViewerCustomization.onDocumentReady"
        OnServerError="ViewerCustomization.onServerError" />
</DxDocumentViewer>
```

```javascript
// wwwroot/customization.js
window.ViewerCustomization = {
    onCustomizeMenuActions: function (s, e) {
        // Hide print button
        var printAction = e.GetById(DevExpress.Reporting.Viewer.ActionId.Print);
        if (printAction) printAction.visible = false;
    },
    onCustomizeExportOptions: function (s, e) {
        // Keep only PDF
        e.HideFormat(DevExpress.Reporting.Viewer.ExportFormatID.XLS);
        e.HideFormat(DevExpress.Reporting.Viewer.ExportFormatID.XLSX);
        e.HideFormat(DevExpress.Reporting.Viewer.ExportFormatID.DOCX);
        e.HideFormat(DevExpress.Reporting.Viewer.ExportFormatID.RTF);
        e.HideFormat(DevExpress.Reporting.Viewer.ExportFormatID.CSV);
        e.HideFormat(DevExpress.Reporting.Viewer.ExportFormatID.IMAGE);
        e.HideFormat(DevExpress.Reporting.Viewer.ExportFormatID.TEXT);
    },
    onDocumentReady: function (s, e) {
        console.log("Document ready, pages:", e.PageCount);
    },
    onServerError: function (s, e) {
        console.error("Viewer server error:", e.Error.errorThrown);
    }
};
```

---

## Critical Constraints — JS Customization

These apply when customizing `DxDocumentViewer` or `DxWasmDocumentViewer`:

1. **`CustomizeMenuActions` is for toolbar visibility only**: This callback controls which toolbar items appear and their enabled/disabled state. It does NOT control which export formats are listed in the Export To dropdown — that is controlled by `CustomizeExportOptions`. If you need to hide export formats, use `CustomizeExportOptions` and call `e.HideFormat(...)`.

2. **JS-based customization API = ASP.NET Core Reporting API**: `DxDocumentViewer` and `DxWasmDocumentViewer` are Blazor wrappers around the same HTML5 JavaScript viewer engine used in ASP.NET Core MVC. All JavaScript client-side APIs (`CustomizeMenuActions`, `CustomizeExportOptions`, `ExportFormatID` enum, `ActionId` enum) are identical between the ASP.NET Core viewer and the Blazor variants. If you find an example in ASP.NET Core documentation, the JavaScript code works identically in Blazor.

---

## Antipatterns — JS-Based Viewer

**❌ Inline `<script>` in a Blazor component for callback functions**
- **Symptom**: Script runs in static rendering but fails under Interactive Server/WASM rendering
- **Why**: Blazor re-renders do not re-execute inline scripts. DevExpress callbacks must be registered on the `window` object before the viewer initializes
- **Fix**: Place all callback functions in a file under `wwwroot/` and register it via `DxResourceManager.RegisterScripts()` in App.razor

**❌ A12: Use `CustomizeMenuActions` to filter export formats**
- **Symptom**: Export To button is hidden but formats still appear in the export options side panel
- **Why**: `CustomizeMenuActions` controls toolbar button visibility only; it does not remove formats from the export model
- **Fix**: Use `CustomizeExportOptions` with `e.HideFormat(ExportFormatID.XLS)` for format filtering. Both callbacks can coexist in `<DxDocumentViewerCallbacks>`

**❌ A13: Apply C# `OnCustomizeToolbar` or `ExportModel` to DxDocumentViewer**
- **Symptom**: No error at compile time; customization has no effect at runtime
- **Why**: `OnCustomizeToolbar` and `ExportModel` are native `DxReportViewer`-only APIs; they do not exist on `DxDocumentViewer`
- **Fix**: Use `<DxDocumentViewerCallbacks CustomizeMenuActions="...">` and `CustomizeExportOptions` for JS-based viewers
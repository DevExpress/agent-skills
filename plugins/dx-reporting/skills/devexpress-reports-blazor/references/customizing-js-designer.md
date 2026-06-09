# Customizing the JavaScript-Based Report Designer (DxReportDesigner or DxWasmReportDesigner)

## When to Use This Reference

Use this when you need to:
- Hide, add, or reorder toolbar / menu commands in the designer or its print preview
- Hide or replace whole UI panels (e.g., remove the Toolbar entirely)
- Customize the Report Wizard or Data Source Wizard (remove pages, sections, or report types)
- Restrict end-user access to data source creation in the wizard
- Configure component-level settings (parameter editing, data source panel, preview export)
- Respond to lifecycle events: report opened, saved, tab changed, server errors

This reference covers `DxReportDesigner` (Blazor Server) and `DxWasmReportDesigner` (Blazor WebAssembly). They share the same `DxReportDesignerCallbacks` and `DxReportDesignerModelSettings` nested components — examples work for both unless noted.

---

## Three Customization Approaches

| Approach | When to use |
|----------|-------------|
| **Application Settings** | Configure global settings that affect the entire application, such as registering custom functions or data connections. |
| **Page or Razor Component Settings** | Toggle specific pre-built settings without JavaScript (AllowMDI, AllowAddDataSource, wizard search box visibility, etc.) |
| **JavaScript callbacks** (`DxReportDesignerCallbacks`) | Deep customization: change visible commands, hide UI elements, modify wizard flow, respond to events |

---

## Register Custom Functions in the Expression Editor (Application Settings)

Call the CustomFunctions.Register method at application startup:

```csharp
// ...
var builder = WebApplication.CreateBuilder(args);
// ...
DevExpress.XtraReports.Expressions.CustomFunctions.Register(new CustomFormatFunction());
// ...
var app = builder.Build();
// ...
```
---

## Razor Component Settings Reference

For `DxReportDesigner`, use its nested components directly:

```razor
<DxReportDesigner ReportName="SalesReport" Height="100%" Width="100%"
                  AllowMDI="true"
                  RightToLeft ="false">
    <DxReportDesignerDataSourceSettings AllowRemoveDataSource="false" AllowAddDataSource="false" />
    <DxReportDesignerReportPreviewSettings ExportSettings="new(){UseSameTab=false}"
        ProgressBarSettings="new(){Position= DevExpress.XtraReports.Web.WebDocumentViewer.ProgressBarPosition.TopRight}">
    </DxReportDesignerReportPreviewSettings>
    <DxReportDesignerWizardSettings UseFullscreenWizard="false" EnableSqlDataSource="false" />
</DxReportDesigner>
```

For `DxWasmReportDesigner`, wrap in `DxReportDesignerModelSettings`:

```razor
<DxWasmReportDesigner ReportName="SalesReport" Height="100%" Width="100%">
    <DxWasmReportDesignerRequestOptions GetDesignerModelAction="DXXRD/GetReportDesignerModel" />
    <DxReportDesignerModelSettings AllowMDI="true" RightToLeft ="false">
        <DxReportDesignerDataSourceSettings AllowRemoveDataSource=false AllowAddDataSource=false />
        <DxReportDesignerReportPreviewSettings ExportSettings="new(){UseSameTab=false}"
            ProgressBarSettings="new(){Position= DevExpress.XtraReports.Web.WebDocumentViewer.ProgressBarPosition.TopRight}">
        </DxReportDesignerReportPreviewSettings>
        <DxReportDesignerWizardSettings UseFullscreenWizard=false EnableSqlDataSource=false />
    </DxReportDesignerModelSettings>
</DxWasmReportDesigner>
```

| Component Settings | Properties |
|-----------|---------------|
| `DxReportDesigner` and `DxWasmReportDesigner.DxReportDesignerModelSettings` | `AllowMDI`, `RightToLeft` |
| `DxReportDesignerParameterEditingSettings ` | `AllowEditParameterCollection`, `AllowEditParameterGroups`, `AllowEditParameterSeparators`, `AllowEditProperties`, `AllowReorderParameters` |
| `DxReportDesignerDataSourceSettings` | `AllowAddDataSource`, `AllowEditDataSource`, `AllowRemoveDataSource` |
| `DxReportDesignerWizardSettings` | `ReportWizardTemplatesSearchBoxVisibility`, `EnableFederationDataSource`, `EnableJsonDataSource`, `EnableMongoDBDataSource`, `EnableObjectDataSource`, `EnableSqlDataSource`, `UseFullscreenWizard`, `UseMasterDetailWizard` |
| `DxReportDesignerReportPreviewSettings` | `ExportSettings`, `ProgressBarSettings` |

---

## Register a Custom Control (Razor Component Settings)

Use the `CustomControlTypes` property to register custom controls. The designer will automatically display them and their properties in the Toolbox and the Property Grid.

```razor
@page "/designer"

<DxReportDesigner ReportName="SampleReport" Height="1000px" Width="100%" CustomControlTypes="@customTypes">
</DxReportDesigner>

@code {
    List<Type> customTypes = new List<Type> { typeof(MyControl), typeof(NumericLabel) };
}
```

---

## Enable Rich Text Editor (Application and Page Settings)
In the App.razor file, call the RegisterRichEditScripts() method to register scripts required for the inline Rich Text Editor.

```razor
@using DevExpress.Blazor.Reporting

@DxResourceManager.RegisterScripts((config) => {
        config.ConfigureReporting(x => x.RegisterRichEditScripts());
    })
```

Reference the Rich Edit stylesheet on the page where the designer is used:

```html
<link href="_content/DevExpress.Blazor.Resources/css/devexpress-richedit/dx.richedit.css" rel="stylesheet" />
```

---

## JavaScript Script Setup (Required for All JavaScript Customizations)

All JavaScript customization follows the same three-file pattern.

**1. wwwroot/customization.js** — put your code in a named namespace:

```javascript
window.DesignerCustomization = {
    onCustomizeElements: function (s, e) {
        // your code
    }
}
```

**2. App.razor** — register the script via `DxResourceManager.RegisterScripts`:

```razor
<head>
    @DxResourceManager.RegisterScripts((config) => config.Register(new DxResource("/customization.js", 900)))
</head>
```

**3. Designer.razor** — wire the handler name to the `DxReportDesignerCallbacks` property:

DxReportDesigner example:
```razor
<DxReportDesigner ReportName="SalesReport" Height="1000px" Width="100%">
    <DxReportDesignerCallbacks CustomizeMenuActions="DesignerCustomization.onCustomizeMenuActions" />
</DxReportDesigner>
```
DxWasmReportDesigner example:
```razor
<DxWasmReportDesigner ReportName="TestReport" Height="700px" Width="100%">
    <DxReportDesignerCallbacks CustomizeElements="DesignerCustomization.onCustomizeElements"/>
</DxWasmReportDesigner>
```

---

## All Javascript Callbacks: Event Reference

`DxReportDesignerCallbacks` properties — each accepts a JavaScript function name string.

The comprehensive description of each callback, its parameters, and usage examples can be found in the official DevExpress MCP documentation: [Client-Side Events in the Blazor Report Designer](https://docs.devexpress.com/XtraReports/404135).

### Customize Designer Elements and Actions

| Callback property | Description |
|-------------------|-------------|
| `CustomizeMenuActions` | Add, remove, or reorder toolbar/menu commands |
| `CustomizeElements` | Hide, move, or modify UI panels (Toolbar, etc.) |
| `CustomizeWizard` | Modify the Report Wizard or Data Source Wizard |
| `CustomizeFieldListActions` | Customize actions in the Field List panel |
| `CustomizeToolbox` | Customize the control Toolbox |

### Respond to Opening and Saving

| Callback property | Description |
|-------------------|-------------|
| `ReportOpening` | Fires before a report opens |
| `ReportOpened` | Fires after a report opens |
| `ReportSaving` | Fires before a report saves |
| `ReportSaved` | Fires after a report saves |

### Customize Save/Open Dialogs

| Callback property | Description |
|-------------------|-------------|
| `CustomizeOpenDialog` | Customize the Open Report dialog |
| `CustomizeSaveDialog` | Customize the Save dialog |
| `CustomizeSaveAsDialog` | Customize the Save As dialog |

### Lifecycle

| Callback property | Description |
|-------------------|-------------|
| `TabChanged` | Fires when the active report tab changes |
| `ExitDesigner` | Fires when the designer is closing |
| `ComponentAdded` | Fires after a control is dropped onto the design surface |
| `BeforeRender` | Fires before the designer UI initializes |
| `OnInitializing` | Fires before `BeforeRender` (before model is fetched) |
| `OnServerError` | Fires on the client when a server-side error occurs |

### Customize Parameters

| Callback property | Description |
|-------------------|-------------|
| `CustomizeParameterEditors` | Register custom editors for report parameters |
| `CustomizeParameterProperties` | Customize parameter properties, groups, separators |
| `PreviewCustomizeParameterLookUpSource` | Customize look-up values for parameters |

### Localization

| Callback property | Description |
|-------------------|-------------|
| `CustomizeLocalization` | Replace built-in localization strings |

### Preview-Specific (Designer's Built-In Document Viewer)

All preview callbacks start with the `Preview` prefix. `DxDocumentViewer` and `DxWasmDocumentViewer` use similar callbacks but without the prefix. Refer to the [customizing-js-viewer.md](customizing-js-viewer.md) reference to learn how to use these events.

| Callback property | Description |
|-------------------|-------------|
| `PreviewCustomizeMenuActions` | Add/remove/reorder toolbar commands in the preview |
| `PreviewCustomizeElements` | Hide or modify UI elements in the preview |
| `PreviewCustomizeExportOptions` | Hide export formats or change export options |
| `PreviewDocumentReady` | Fires after the report is rendered in print preview |
| `PreviewEditingFieldChanged` | Fires when an editing field value changes |
| `PreviewParametersReset` | Fires after parameters are reset to defaults |
| `PreviewClick` | Fires when the document is clicked |

---

## JavaScript Customization Scenario 1 — Hide an Entire UI Element

Use `CustomizeElements` JavaScript callback with `ReportDesignerElements` enumeration to remove whole panels:

```javascript
window.DesignerCustomization = {
    onCustomizeElements: function (s, e) {
        // Remove the Toolbar panel entirely.
        var toolbarPart = e.GetById(DevExpress.Reporting.Designer.Utils.ReportDesignerElements.Toolbar);
        var index = e.Elements.indexOf(toolbarPart);
        if (index >= 0) e.Elements.splice(index, 1);
    }
}
```

Available UI elements include: `Toolbar`, `MenuButton`, `NavigationPanel`, `RightPanel`, `Surface`, `Toolbox`.

---

## JavaScript Customization Scenario 2 — Hide and Add Toolbar and Menu Commands / Actions

Handle `CustomizeMenuActions` JavaScript callback to manipulate the `Actions` collection.

```javascript
// wwwroot/customization.js
window.DesignerCustomization = {
    onCustomizeActions: function (s, e) {
        // Hide the "New" menu command.
        var newReportAction = e.GetById(DevExpress.Reporting.Designer.Actions.ActionId.NewReport);
        if (newReportAction) newReportAction.visible = false;

        // Hide the "Validate Bindings" toolbar command.
        var validateAction = e.GetById(DevExpress.Reporting.Designer.Actions.ActionId.ValidateBindings);
        if (validateAction) validateAction.visible = false;

        // Add a custom "Refresh" toolbar command.
        e.Actions.splice(15, 0, {
            container: "toolbar",
            text: "Refresh",
            imageTemplateName: "refresh",   // must match an <svg> template id in App.razor
            visible: true,
            disabled: false,
            hasSeparator: false,
            hotKey: { ctrlKey: true, keyCode: "Z".charCodeAt(0) },
            clickAction: function () {
                s.GetCurrentTab().refresh();
            }
        });
    }
}
```

**Available ActionIds** : New, NewReportViaWizard, OpenReport, Save, SaveAs, ReportWizard, ReportWizardFullScreen, Preview, Scripts, AddDataSource, AddSqlDataSource, AddMultiQuerySqlDataSource, ValidateBindings, Exit, FullScreen, Localization. 

**Action properties:**

| Property | Description |
|----------|-------------|
| `container` | `"toolbar"` or `"menu"` |
| `text` | Tooltip text |
| `visible` | Show/hide |
| `disabled` | Enable/disable |
| `hasSeparator` | Visual separator before the command |
| `hotKey` | `{ ctrlKey, keyCode }` |
| `imageClassName` / `imageTemplateName` | Icon (CSS class or SVG template id) |
| `clickAction` | Function called on click |

---

## JavaScript Customization Scenario 3 — Respond to Report Save / Open

```javascript
window.DesignerCustomization = {
    onReportSaved: function (s, e) {
        console.log("Saved: " + e.Url);
    },
    onReportOpened: function (s, e) {
        console.log("Opened: " + e.Url);
    }
}
```

```razor
<DxReportDesigner ReportName="SalesReport" Height="1000px" Width="100%">
    <DxReportDesignerCallbacks
        ReportSaved="DesignerCustomization.onReportSaved"
        ReportOpened="DesignerCustomization.onReportOpened" />
</DxReportDesigner>
```

---

## JavaScript Customization Scenario 4 — Customize the Report Wizard

Use `CustomizeWizard` Javascript callback with the wizard's event system. Determine which wizard fired with `e.Type` (`"ReportWizard"` or `"DataSourceWizard"`).

For comprehensive examples of customizing the Report Wizard and Data Source Wizard, see the DevExpress Documentation MCP: [Customize Report and Data Source Wizards](https://docs.devexpress.com/XtraReports/405654/web-reporting/blazor-reporting/web-report-designer/customization/customize-report-and-data-source-wizards).

---

## See Also

This reference does not cover the full API of `DxReportDesignerCallbacks` or all possible customizations. For the complete list of callbacks, their parameters, and usage examples, refer to the official DevExpress documentation:

- [DxReportDesignerCallbacks Class](https://docs.devexpress.com/XtraReports/DevExpress.Blazor.Reporting.DxReportDesignerCallbacks) — full API reference
- [Customize Toolbar and Menu](https://docs.devexpress.com/XtraReports/405653)
- [Client-Side Events in the Blazor Report Designer](https://docs.devexpress.com/XtraReports/404135)
- [Customize Report and Data Source Wizards](https://docs.devexpress.com/XtraReports/405654)

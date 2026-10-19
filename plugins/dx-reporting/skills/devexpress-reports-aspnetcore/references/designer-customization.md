# Report Designer Customization — ASP.NET Core

## When to Use This Reference

Use this when you need to:
- Look up a `ReportDesignerBuilder` fluent API member beyond `Bind`/`Height`/`ClientSideEvents`
- Handle designer client-side events
- Hide or add toolbar/menu commands in the End-User Report Designer, or look up the exact `ActionId` for a built-in command before hiding/showing it
- Remove or reposition entire UI panels (Toolbar, Toolbox, Right Panel, NavigationPanel) — look up the exact `ReportDesignerElements` value first
- Customize the Preview, Parameters Panel, and other UI elements integrated inside the designer
- Remove or add controls in the Toolbox

## When Not to Use This Reference
- Rare extensibility scenarios (custom controls, custom expression functions, Report/Data Source Wizard customization) are covered concisely with links to the canonical docs in the "Advanced Extensibility" section below
- Customize the Designer's (or its Preview's) color scheme — see [theming.md](theming.md)
- Register Data Source Wizard connection/schema/Object-Data-Source services, or pre-configure a data source server-side — see [data-sources.md](data-sources.md)
- Handle Designer/Preview server-side errors or log viewer operations — see [error-handling-and-logging.md](error-handling-and-logging.md)

## Basic Setup

```cshtml
@(Html.DevExpress().ReportDesigner("reportDesigner")
    .Height("1000px")
    .Bind("SalesReport"))
```

## ReportDesignerBuilder API Reference

`Html.DevExpress().ReportDesigner(...)` returns a `ReportDesignerBuilder`. Beyond `Bind`/`Height`, it exposes:

| Member | Description |
|---|---|
| `Bind(string)` | Loads a report by name from the registered `IReportProvider`/`ReportStorageWebExtension` and binds it to the Designer. |
| `Bind(XtraReport)` | Binds the Designer to a report instance. |
| `Bind(ReportDesignerModel)` | Binds to a `ReportDesignerModel` built server-side — see [controller-view-binding.md](controller-view-binding.md#report-designer) (Style 3). |
| `Height(string)` / `Width(string)` | Sets the control's height/width on the page. |
| `Name(string)` | Sets the element's id on the page. |
| `CssClassName(string)` | Applies a CSS class to the rendered Designer container. Required for sizing under CSP — see [security.md](security.md#content-security-policy-csp-with-nonce). |
| `ClientSideEvents(Action<ReportDesignerClientSideEventsBuilder>)` | Wires up client-side events — see the Client-Side Events Reference below. |
| `DataSources(Action<IDictionary<string, object>>)` | Pre-registers data sources so they appear in the Data Source Wizard without the user configuring one from scratch — see [data-sources.md](data-sources.md#adding-data-sources-in-the-controller) for the worked example. |
| `DataSourceSettings(Action<DataSourceSettings>)` | Restricts data source actions available in the Field List panel (`AllowAddDataSource`, `AllowEditDataSource`, `AllowRemoveDataSource`). |
| `ParameterEditingSettings(Action<ReportDesignerParameterEditingSettings>)` | Configures parameter editing behavior in the Designer's Preview (`AllowEditParameterCollection`, `AllowEditParameterGroups`, `AllowEditParameterSeparators`, `AllowEditProperties`, `AllowReorderParameters`). |
| `WizardSettings(Action<WizardSettings>)` | Configures Report Wizard/Data Source Wizard defaults (`ReportWizardTemplatesSearchBoxVisibility`, `EnableFederationDataSource`, `EnableJsonDataSource`, `EnableMongoDBDataSource`, `EnableObjectDataSource`, `EnableSqlDataSource`, `UseFullscreenWizard`, `UseMasterDetailWizard`). All `Enable*` and `Use*` flags default to `true` — set them to `false` to disable these options. |

## Setup Client-Side Events

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

Wire up any of the required events in one `ClientSideEvents(...)` call:

```cshtml
<script>
    function onCustomizeMenuActions(s, e) { /* body from this reference */ }
    function onCustomizeToolbox(s, e) { /* body from this reference */ }
</script>

@(Html.DevExpress().ReportDesigner("reportDesigner")
    .ClientSideEvents(ev => ev
        .OnInitializing("onInitializing")
        .CustomizeMenuActions("onCustomizeMenuActions")
        .CustomizeElements("onCustomizeElements")
        .CustomizeToolbox("onCustomizeToolbox")
        .CustomizeWizard("onCustomizeWizard")
        .CustomizeFieldListActions("onCustomizeFieldListActions")
        .ReportOpening("onReportOpening")
        .ReportOpened("onReportOpened")
        .ReportSaving("onReportSaving")
        .ReportSaved("onReportSaved")
        .CustomizeOpenDialog("onCustomizeOpenDialog")
        .CustomizeSaveDialog("onCustomizeSaveDialog")
        .CustomizeSaveAsDialog("onCustomizeSaveAsDialog")
        .TabChanged("onTabChanged")
        .ReportTabClosing("onReportTabClosing")
        .ReportTabClosed("onReportTabClosed")
        .ExitDesigner("onExitDesigner")
        .ComponentAdded("onComponentAdded")
        .CustomizeLocalization("onCustomizeLocalization")
        .CustomizeParameterEditors("onCustomizeParameterEditors")
        .CustomizeParameterLookUpSource("onCustomizeParameterLookUpSource")
        .CustomizeParameterProperties("onCustomizeParameterProperties")
        .OnServerError("onServerError")
        .Preview(p => p
            .CustomizeMenuActions("onPreviewCustomizeMenuActions")
            .DocumentReady("onDocumentReady")))
    .Bind("SalesReport"))
```

## Client-Side Events Reference

### Report Designer

| Event | Description |
|---|---|
| `CustomizeWizard` | Customize the Report Wizard and Data Source Wizard — see "Report Wizard and Data Source Wizard" below. |
| `CustomizeElements` | Customize UI elements (hide/reorder panels) — see "Hide an Entire UI Panel" below. |
| `CustomizeMenuActions` | Customize menu/toolbar actions — see "Toolbar and Menu Customization" below. |
| `CustomizeFieldListActions` | Customize actions in the Field List panel. |
| `ReportOpening` | Fires when a report is about to be opened in the Report Designer. |
| `ReportOpened` | Fires when a report has been opened in the Report Designer. |
| `ReportSaving` | Fires when a report is about to be saved in the Report Designer. |
| `ReportSaved` | Fires when a report has been saved in the Report Designer. |
| `CustomizeOpenDialog` | Customize the **Open Report** dialog. |
| `CustomizeSaveDialog` | Customize the **Save** dialog. |
| `CustomizeSaveAsDialog` | Customize the **Save Report** dialog. |
| `TabChanged` | Fires when the active report tab changes. |
| `ReportTabClosing` | Fires when a report tab is about to be closed. |
| `ReportTabClosed` | Fires when a report tab has been closed. |
| `ExitDesigner` | Fires when the Report Designer is being closed. |
| `CustomizeToolbox` | Customize the Toolbox — see "Toolbox Customization" below. |
| `ComponentAdded` | Fires after a component has been added to the current report (dropped onto the design surface). |
| `CustomizeLocalization` | Substitute the Report Designer's localization strings with custom strings. |
| `OnServerError` | Fires on the client each time a server-side error occurs. |
| `BeforeRender` | Fires before the Report Designer's UI is initialized. |
| `OnInitializing` | Fires even earlier than `BeforeRender` — before the model is fetched from the server. |

### Report Designer's Integrated Parameters Panel

These customize the Designer's built-in Parameters Panel — for reference tables behind them, see [standalone-parameters-panel.md](standalone-parameters-panel.md), since the Parameters Panel is the same Parameters Panel used standalone.

| Event | Description |
|---|---|
| `CustomizeParameterEditors` | Add custom editors for report parameters. |
| `CustomizeParameterLookUpSource` | Customize look-up parameter values. |
| `CustomizeParameterProperties` | Customize parameters, parameter groups, parameter separators, and property editors. |

### Report Designer's Integrated Document Viewer (`.Preview(...)`)

Access these via `ReportDesignerClientSideEventsBuilder.Preview(Action<ReportDesignerPreviewClientSideEventsBuilder>)`. These customize the Designer's built-in Preview tab — for the full `ActionId`/`ExportFormatID`/`PreviewElements` reference tables behind them, see [viewer-customization.md](viewer-customization.md), since the Preview is the same Document Viewer used standalone.

| Event | Description |
|---|---|
| `PreviewClick` | Fires when a report document is clicked. |
| `CustomizeElements` | Customize UI elements in the Preview. |
| `CustomizeMenuActions` | Customize toolbar actions in the Preview. |
| `CustomizeExportOptions` | Customize export formats and their options in the Preview. |
| `DocumentReady` | Fires after a report has been switched to Print Preview. |
| `EditingFieldChanged` | Fires each time an editing field's value changes. |
| `ParametersReset` | Fires after report parameter values are reset to their default values. |
| `ParametersSubmitted` | Fires after report parameter values are submitted. |

## Client-Side Events Examples

### Toolbar and Menu Customization

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

**All `DevExpress.Reporting.Designer.Actions.ActionId` values:**

| ActionId | Description |
|---|---|
| `NewReport` | The **New** menu command — creates a new blank report. |
| `NewReportViaWizard` | The **New via Wizard** menu command. |
| `OpenReport` | The **Open** menu command. |
| `ReportWizard` | The **Design in Report Wizard** menu command — invokes the Report Wizard to edit the current report. |
| `ReportWizardFullScreen` | The **New via Wizard** menu command that runs the fullscreen Report Wizard. |
| `Preview` | The **Preview** toolbar button. |
| `Scripts` | The **Scripts** toolbar button. |
| `AddDataSource` | The drop-down list of available data sources in the Field List. |
| `AddSqlDataSource` | The **Add SQL Data Source** menu command — runs the Data Source Wizard's single-query version. |
| `AddMultiQuerySqlDataSource` | The **Add SQL Data Source** menu command — runs the Data Source Wizard's multi-query version. |
| `ValidateBindings` | The **Validate Bindings** toolbar button. |
| `Save` | The **Save** menu command. |
| `SaveAs` | The **Save As** menu command. |
| `Exit` | The **Exit** menu command. |
| `FullScreen` | The **Full Screen** toolbar command. |
| `Localization` | The **Localization** command — invokes the Localization Editor. |

> **Verifying the hide.** The menu is a popup that is not in the DOM until opened, so a text scan of the loaded page proves nothing — it matches unrelated elements and reports the command as still present. Open the menu (or toolbar) in the browser, take a snapshot, and confirm the command is absent. If you cannot drive the UI, report the customization as `NOT VERIFIED` rather than inferring it from page text.

> This is the full `ActionId` enum for the Report Designer's own toolbar/menu — if a command you need to hide/show isn't here, it isn't a built-in Designer action; double-check the command name against this table before assuming an ID exists. For the Designer's integrated Preview toolbar (a Document Viewer), use the `ActionId` table in [viewer-customization.md](viewer-customization.md#toolbar-customization-customizemenuactions) instead — see "Customize the Preview Toolbar" below.

### Hide an Entire UI Panel (CustomizeElements)

Use `CustomizeElements` and `e.GetById(ReportDesignerElements.X)` to remove a whole panel from the layout:

```javascript
function onCustomizeElements(s, e) {
    var toolbarPart = e.GetById(DevExpress.Reporting.Designer.Utils.ReportDesignerElements.Toolbar);
    var index = e.Elements.indexOf(toolbarPart);
    if (index !== -1) e.Elements.splice(index, 1);
}
```

**All `DevExpress.Reporting.Designer.Utils.ReportDesignerElements` values:**

| ReportDesignerElements | Description |
|---|---|
| `MenuButton` | The menu button in the Designer's upper-left corner. |
| `NavigationPanel` | The panel at the bottom of the Designer displaying opened report tabs. |
| `RightPanel` | The panel at the right of the Designer, containing the Field List, Report Explorer, and Properties Panel tabs. |
| `Toolbar` | The Report Designer's main Toolbar. |
| `Toolbox` | The Toolbox containing report controls. |

### Customize the Preview Toolbar (Inside Designer)

Handle `Preview.CustomizeMenuActions` to modify the built-in Document Viewer toolbar inside the designer's preview mode. The Preview is the same Document Viewer used standalone, so its `ActionId`/`ExportFormatID`/`PreviewElements` values are the ones documented in full in [viewer-customization.md](viewer-customization.md#toolbar-customization-customizemenuactions) — not repeated here.

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

### Toolbox Customization

#### Register a Custom Report Control

Register the type on the designer model builder:

```csharp
designerModelBuilder
    .Report("SalesReport")
    .CustomControls(typeof(NumericLabel))
    .BuildModel();
```

Full walkthrough (control class, toolbox icon CSS, limitations): [Custom Controls in the Web Report Designer](https://docs.devexpress.com/XtraReports/401687).

#### Remove a Built-In Control

Handle `CustomizeToolbox` to remove controls from the Toolbox:

```javascript
function onCustomizeToolbox(s, e) {
    var info = e.ControlsFactory.getControlInfo("XRLabel");
    info.isToolboxItem = false;
}
```

## Color Scheme

The Designer (and its integrated Preview) supports 8 predefined color schemes and custom schemes via CSS. See [theming.md](theming.md) for how to apply a predefined scheme or build a custom one.

## Convert UTC to Local Time in the Preview

The Designer's integrated Preview can display report date-time values in a local time zone: use the `UtcToLocalTime()` / `LocalTimeToUtc()` expression functions in the report, and register a custom `ITimeZoneProvider` (`DevExpress.XtraReports.Services`) at startup to supply the zone. See [Convert Time Zones (ASP.NET Core)](https://docs.devexpress.com/XtraReports/405644).

## Customize the Report Before It Is Previewed (PreviewReportCustomizationService)

The Designer's Preview tab renders a **live copy** of the report being edited. Implement `PreviewReportCustomizationService` to modify that copy right before it's previewed without changing the report that eventually gets saved. Override `CustomizeReport(XtraReport report)` to change the report's settings before preview, and optionally `CreateCachedReportSource(XtraReport report)` to assign a custom `CachedReportSourceWeb`.

## Advanced Extensibility

These scenarios are uncommon — reach for the linked help topics (fetch via the DevExpress MCP `devexpress_docs_search` when you need the full, version-correct flow).

**Add a custom expression function** — subclass `ReportCustomFunctionOperatorBase` and register it at startup:

```csharp
CustomFunctions.Register(new CustomFormatFunction()); 
```

To hide a built-in function from the client picker only, delete it from `DevExpress.Reporting.Designer.Widgets.reportFunctionDisplay[categoryIndex].items` in a `BeforeRender` handler. Full flow: [Custom Functions in the Expression Editor](https://docs.devexpress.com/XtraReports/403261).

**Customize the Report Wizard / Data Source Wizard** — handle the `CustomizeWizard` client-side event to add/remove wizard pages, sections, and defaults (`e.Type` is `"ReportWizard"` or `"DataSourceWizard"`; `e.Wizard.events` exposes `beforeInitialize`/`afterInitialize`/`beforePageInitialize`/`beforeSectionInitialize`):

```cshtml
.ClientSideEvents(x => x.CustomizeWizard("onCustomizeWizard"))
```

Page/section IDs come from the `FullscreenReportWizardPageId` / `FullscreenDataSourceWizardPageId` (and `…SectionId`) enums, e.g. `FullscreenReportWizardPageId.SelectReportTypePage`, `SelectDataSourcePage`, `DefineReportLayoutPage`, `SpecifyPageSettingsPage`, and `FullscreenDataSourceWizardPageId.ChooseDataSourceTypePage`, `SpecifySqlDataSourceSettingsPage`, `SpecifyJsonDataSourceSettingsPage`, `SpecifyObjectDataSourceSettingsPage`. Full examples (skip a page, filter report types): [Customize the Report Wizard and Data Source Wizard](https://docs.devexpress.com/XtraReports/401088).

## Other Services

Server-side services that support the Designer live in concern-based files:

- Data Source Wizard connection strings, database schema shaping, Object Data Source type/constructor/member filters, JSON schema discovery, controller-side data sources — [data-sources.md](data-sources.md).
- Connection-string protection, custom SQL, per-user authorization, multi-tenant SELECT filtering — [security.md](security.md).
- Designer/Preview exception handlers, operation logging, document operations — [error-handling-and-logging.md](error-handling-and-logging.md).
- Report storage and name resolution — [report-storage.md](report-storage.md).

## Important Notes

- All three controllers (ViewerController, DesignerController, QueryBuilderController) are required when the designer is integrated. See [getting-started.md](getting-started.md) Step 6.
- Designer report storage (`ReportStorageWebExtension`) must be registered after `AddDevExpressControls()`. See [report-storage.md](report-storage.md).

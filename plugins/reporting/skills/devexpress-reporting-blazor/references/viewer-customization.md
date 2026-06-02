# Viewer Customization — Blazor

## When to Use This Reference

Use this when you need to:
- Add or hide toolbar items in the native `DxReportViewer` (C# API)
- Filter export formats in the native `DxReportViewer` (`ExportModel`)
- Filter export formats in the JS-based `DxDocumentViewer` (`CustomizeExportOptions`)
- Handle report-level events (document ready, errors, parameter submission)
- Customize parameter editors
- Change the zoom level programmatically

> **Two separate APIs**: Native `DxReportViewer` uses C# event handlers (`OnCustomizeToolbar`, `ExportModel`). JS-based `DxDocumentViewer`/`DxWasmDocumentViewer` uses JavaScript callbacks via `<DxDocumentViewerCallbacks>` — the same client-side API as the ASP.NET Core HTML5 viewer.

## API Summary by Component

| Task | DxReportViewer (Native) | DxDocumentViewer (JS-based) |
|------|-------------------------|-----------------------------|
| Hide toolbar item | `OnCustomizeToolbar` → `item.Visible = false` | `CustomizeMenuActions` → `action.visible = false` |
| Filter export formats | `ExportModel.AvailableFormats.RemoveAll(...)` | `CustomizeExportOptions` → `e.HideFormat(ExportFormatID.XLS)` |
| Both at once | Two separate `OnCustomizeToolbar` operations | Two separate callbacks in `<DxDocumentViewerCallbacks>` — do NOT use `CustomizeMenuActions` to filter formats |

## Toolbar Customization

Handle the `OnCustomizeToolbar` event to add, hide, or modify toolbar items.

```razor
@using DevExpress.Blazor.Reporting
@using DevExpress.Blazor.Reporting.Models
@using DevExpress.XtraReports.UI

@inject IJSRuntime JsRuntime

<div @ref="viewerContainer" style="width: 100%; height: 1000px;">
    <DxReportViewer @ref="reportViewer"
                    Report="Report"
                    OnCustomizeToolbar="OnCustomizeToolbar" />
</div>

@code {
    DxReportViewer reportViewer;
    XtraReport Report = new SalesReport();
    ElementReference viewerContainer;

    void OnCustomizeToolbar(ToolbarModel toolbarModel) {
        // Add a custom button
        toolbarModel.AllItems.Add(new ToolbarItem {
            IconCssClass = "oi oi-fullscreen-enter",
            Text = "Full Screen",
            AdaptiveText = "Full Screen",
            AdaptivePriority = 1,
            Click = async (args) => {
                await JsRuntime.InvokeVoidAsync("document.documentElement.requestFullscreen");
            }
        });
    }
}
```

## Hide a Toolbar Item by ID

```razor
void OnCustomizeToolbar(ToolbarModel toolbarModel) {
    foreach (var item in toolbarModel.AllItems) {
        if (item.Id == "ExportTo") {
            item.Visible = false;
        }
    }
}
```

For the list of built-in toolbar item IDs, refer to `DevExpress.Blazor.Reporting.Models.ToolbarItemId`.

## Restrict Export Formats

Use `ExportModel.AvailableFormats` to limit which export formats appear in the Export To drop-down:

```razor
@using DevExpress.Blazor.Reporting

<DxReportViewer @ref="reportViewer" Report="Report" />

@code {
    DxReportViewer reportViewer;
    XtraReport Report = new SalesReport();
    static readonly string[] AllowedFormats = { "Pdf", "Image" };

    protected override void OnAfterRender(bool firstRender) {
        if (firstRender) {
            reportViewer.ExportModel.AvailableFormats
                .RemoveAll(fmt => !AllowedFormats.Contains(fmt.Name));
        }
    }
}
```

## Set Zoom Level

Use the `Zoom` property:

```razor
<DxReportViewer Report="Report" Zoom="150" />
```

Or programmatically after render:

```razor
@code {
    DxReportViewer reportViewer;

    protected override void OnAfterRender(bool firstRender) {
        if (firstRender)
            reportViewer.Zoom = 150;
    }
}
```

## Handle Client-Side Events (C# in Razor)

The native viewer exposes events as Blazor EventCallback-style properties:

```razor
<DxReportViewer @ref="reportViewer"
                Report="Report"
                OnCustomizeToolbar="OnCustomizeToolbar" />
```

## Pass Parameters Programmatically

Set parameter values before passing the report to the viewer:

```razor
@code {
    DevExpress.XtraReports.UI.XtraReport Report;
    string paramValue = "2025-01-01";

    protected override void OnInitialized() {
        var report = new SalesReport();
        report.Parameters["StartDate"].Value = DateTime.Parse(paramValue);
        report.Parameters["StartDate"].Visible = false; // hide from parameter panel
        Report = report;
    }
}
```

To reload with new parameter values, reassign the `Report` property:

```razor
void ApplyNewParameters() {
    var report = new SalesReport();
    report.Parameters["StartDate"].Value = newDate;
    report.Parameters["StartDate"].Visible = false;
    reportViewer.TabPanelModel.Tabs[0].Visible = false; // hide parameters tab
    Report = report;
}
```

## Error Handling

```razor
<DxReportViewer Report="Report" />

@code {
    // Errors are shown in the viewer's UI by default.
    // To handle server-side errors, implement IWebDocumentViewerExceptionHandler
    // and register it in DI.
}
```

## Tab Panel Customization (Native Viewer)

The native viewer's tab panel is accessed via `reportViewer.TabPanelModel`:

```razor
@code {
    DxReportViewer reportViewer;

    protected override void OnAfterRender(bool firstRender) {
        if (firstRender) {
            // Hide the Parameters tab (index 0)
            if (reportViewer.TabPanelModel.Tabs.Count > 0)
                reportViewer.TabPanelModel.Tabs[0].Visible = false;
        }
    }
}
```

## Export Format Filtering — JS-Based DxDocumentViewer

`DxDocumentViewer` and `DxWasmDocumentViewer` share the same client-side JS API as the ASP.NET Core HTML5 viewer. Use `CustomizeExportOptions` to filter formats — **not** `CustomizeMenuActions`.

```razor
<DxDocumentViewer ReportName="SalesReport" Height="1000px" Width="100%">
    <DxDocumentViewerCallbacks
        CustomizeMenuActions="onCustomizeMenuActions"
        CustomizeExportOptions="onCustomizeExportOptions" />
</DxDocumentViewer>
```

```javascript
// Correct: use CustomizeExportOptions for export format filtering
function onCustomizeExportOptions(s, e) {
    e.HideFormat(DevExpress.Reporting.Viewer.ExportFormatID.XLS);
    e.HideFormat(DevExpress.Reporting.Viewer.ExportFormatID.XLSX);
    e.HideFormat(DevExpress.Reporting.Viewer.ExportFormatID.CSV);
    e.HideFormat(DevExpress.Reporting.Viewer.ExportFormatID.DOCX);
    e.HideFormat(DevExpress.Reporting.Viewer.ExportFormatID.RTF);
    e.HideFormat(DevExpress.Reporting.Viewer.ExportFormatID.IMAGE);
    e.HideFormat(DevExpress.Reporting.Viewer.ExportFormatID.TEXT);
    // PDF (DevExpress.Reporting.Viewer.ExportFormatID.PDF) remains visible
}

// CustomizeMenuActions is for toolbar buttons only — completely separate concern
function onCustomizeMenuActions(s, e) {
    var printAction = e.GetById(DevExpress.Reporting.Viewer.ActionId.Print);
    if (printAction) printAction.visible = false;
}
```

Available `ExportFormatID` values: `CSV`, `DOCX`, `HTML`, `IMAGE`, `MHT`, `PDF`, `RTF`, `TEXT`, `XLS`, `XLSX`.

> **Never** use `CustomizeMenuActions` to hide the Export To button as a workaround for filtering formats. It hides the button but the underlying format data model is unchanged — formats may still appear in other contexts (print preview, programmatic export). Always use `CustomizeExportOptions`.

For the ASP.NET Core (MVC/Razor Pages) equivalent of these same JS APIs, see [devexpress-reporting-aspnetcore references/viewer-customization.md](../../devexpress-reporting-aspnetcore/references/viewer-customization.md).

## Key API Reference

| Member | Type | Description |
|--------|------|-------------|
| `Report` | `IReport` | The report instance to display |
| `OpenReportAsync(IReport)` | method | Async report loading |
| `Zoom` | `int` | Zoom percentage (e.g., 100, 150) |
| `OnCustomizeToolbar` | event | Customize toolbar items |
| `ExportModel` | property | Access export format list |
| `TabPanelModel` | property | Access tab panel settings |

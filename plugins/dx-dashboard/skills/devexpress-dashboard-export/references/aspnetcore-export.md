# Export — ASP.NET Core

## AspNetCoreDashboardExporter

`AspNetCoreDashboardExporter` (namespace `DevExpress.DashboardAspNetCore`) implements server-side export for ASP.NET Core. It inherits all export methods from `WebDashboardExporter` and accepts the scoped `DashboardConfigurator` from DI.

### Constructor

```csharp
new AspNetCoreDashboardExporter(DashboardConfigurator configurator, IWebHostEnvironment hostEnvironment)
```

Inject both parameters from DI (scoped `DashboardConfigurator` + singleton `IWebHostEnvironment`).

### Export Methods (inherited from WebDashboardExporter)

| Method | Description |
|--------|-------------|
| `ExportToPdf(dashboardId, stream)` | Export entire dashboard to PDF (defaults) |
| `ExportToPdf(dashboardId, stream, size, state, options)` | Export with explicit viewport size, state, and options |
| `ExportToImage(dashboardId, stream, size, state, options)` | Export entire dashboard to Image |
| `ExportToExcel(dashboardId, stream, state, options)` | Export entire dashboard data to Excel (no viewport size) |
| `ExportDashboardItemToPdf(dashboardId, itemName, stream, size, state[, options])` | Export single item to PDF |
| `ExportDashboardItemToImage(dashboardId, itemName, stream, size, state[, options])` | Export single item to Image |
| `ExportDashboardItemToExcel(dashboardId, itemName, stream[, state[, options]])` | Export single item to Excel |

> There is **no** `(dashboardId, stream, options)` overload for any format — options always follow `state` (and `size` for PDF/Image). Pass `null` for state to export with the dashboard's defaults cleared, or a deserialized `DashboardState` for "what the user sees".
>
> - `dashboardId` is the ID registered in the dashboard storage (for example `"sales"`), not the controller name.
> - `itemName` is `DashboardItem.ComponentName` (for example `"gridDashboardItem2"`), not the display name (`DashboardItem.Name`).

### Full Example — Server-Side Export Controller

```csharp
using DevExpress.DashboardAspNetCore;
using DevExpress.DashboardCommon;
using Microsoft.AspNetCore.Mvc;
using System.Drawing;

public class ExportController : Controller {
    private readonly DashboardConfigurator _configurator;
    private readonly IWebHostEnvironment _env;

    public ExportController(DashboardConfigurator configurator, IWebHostEnvironment env) {
        _configurator = configurator;
        _env = env;
    }

    // Export entire dashboard to PDF, preserving current state
    [HttpPost]
    public IActionResult ExportToPdf(string dashboardId, string dashboardStateJson) {
        var state = new DashboardState();
        state.LoadFromJson(dashboardStateJson); // extension in DevExpress.DashboardAspNetCore (DashboardStateExtensions)

        var pdfOptions = new DashboardPdfExportOptions {
            ExportFilters = true,
            DashboardStatePosition = DashboardStateExportPosition.Below
        };

        var exporter = new AspNetCoreDashboardExporter(_configurator, _env);
        using var stream = new MemoryStream();
        exporter.ExportToPdf(dashboardId, stream, new Size(1920, 1080), state, pdfOptions);
        return File(stream.ToArray(), "application/pdf", $"{dashboardId}.pdf");
    }

    // Export a single dashboard item to Excel
    [HttpPost]
    public IActionResult ExportItemToExcel(string dashboardId, string itemName) {
        var excelOptions = new DashboardExcelExportOptions {
            Format = ExcelFormat.Xlsx
        };

        var exporter = new AspNetCoreDashboardExporter(_configurator, _env);
        using var stream = new MemoryStream();
        // dashboardId: storage ID ("sales"); itemName: ComponentName ("gridDashboardItem2")
        exporter.ExportDashboardItemToExcel(dashboardId, itemName, stream, null, excelOptions);
        return File(stream.ToArray(),
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"{itemName}.xlsx");
    }
}
```

Client-side — capture state before calling the export endpoint:

```javascript
// Capture the control instance in the onBeforeRender callback
// (Razor: .OnBeforeRender("onBeforeRender") on the DashboardBuilder)
let dashboardControl;
function onBeforeRender(sender) {
    dashboardControl = sender;
}

// Later, when exporting:
async function exportToPdf() {
    const dashboardId = dashboardControl.getDashboardId();
    const stateJson = dashboardControl.getDashboardState(); // already a JSON string — do NOT JSON.stringify it

    // Form-encode the body. A JSON body does not bind to the action's plain string
    // parameters — both arrive null and the export silently returns an unfiltered
    // document. URLSearchParams sets the right Content-Type on its own.
    const response = await fetch("/Export/ExportToPdf", {
        method: "POST",
        body: new URLSearchParams({
            dashboardId: dashboardId,
            dashboardStateJson: stateJson   // key matches the action parameter, not the variable
        })
    });
    // response is the PDF file — save it however your app handles downloads.
}
```

> **If the fetch returns 404, the route is missing.** `ExportController` has no `[Route]` attribute, so it only works if `Program.cs` registers conventional routing:
>
> ```csharp
> app.MapControllerRoute(name: "default", pattern: "{controller}/{action}");
> ```
>
> Add auth headers or an antiforgery token to the `fetch` if your app requires them.

---

## Blazor

**Server-side export is identical to ASP.NET Core** — same `AspNetCoreDashboardExporter`, same controller, same options. `DxDashboard` (`DevExpress.DashboardBlazor`) wraps the JavaScript `DashboardControl` over an ASP.NET Core backend with `DashboardConfigurator`, so everything above applies unchanged. Do not write a Blazor-specific exporter.

Two caveats:

1. **The exporter lives in `DevExpress.AspNetCore.Dashboard`**, not `DevExpress.Blazor.Dashboard`. A Blazor app needs both. In Blazor WebAssembly, the exporter runs in the separate server project.
2. **Client-side state capture reaches the control through `DxJSCustomization`.** `DxDashboard` wraps the same JavaScript `DashboardControl`, so `getDashboardId()` / `getDashboardState()` behave identically — only how you obtain the instance differs. There is no Razor `.OnBeforeRender("onBeforeRender")` builder call. Instead, add a `<DxJSCustomization>` child whose `Identifier` names a `window` object holding the handlers:

```razor
<DxDashboard Endpoint="api/dashboard" style="width: 100%; height: 650px;">
    <DxJSCustomization Identifier="dashboardEvents"></DxJSCustomization>
</DxDashboard>
```

```javascript
// wwwroot/dashboard-events.js
window.dashboardEvents = {
    onBeforeRender: (args) => {
        // The control arrives as args.component — NOT as the argument itself
        window.dashboardControl = args.component;
    }
};
// When exporting, POST these to the export controller shown above:
//   window.dashboardControl.getDashboardId()
//   window.dashboardControl.getDashboardState()   // already a JSON string — do NOT JSON.stringify it
```

Register the script with `DxResourceManager.RegisterScripts()`. Custom scripts that touch `DashboardControl` must load **after** the scripts `DxDashboard` itself requires — set the position accordingly.

Export-UI option configuration (`DxDashboardExport`) is in [export-options.md](export-options.md).

---

## Enable/Disable the Export UI

Set `AllowExportDashboard` / `AllowExportDashboardItems` via the `DashboardExport` extension options (`DashboardExportOptionBuilder`) — there is no `ExportOptions` method on `DashboardBuilder`. The same extension carries the dialog's default option values ([export-options.md](export-options.md) → Set Default UI Export Options), so both are configured in one `ext.DashboardExport(...)` block:

```cshtml
@(Html.DevExpress().Dashboard("dashboardControl1")
    .Width("100%").Height("100%")
    .Extensions(ext => {
        ext.DashboardExport(opt => {
            opt.AllowExportDashboard(false);   // hide whole-dashboard export button
            opt.AllowExportDashboardItems(true);
        });
    })
)
```

---

## Custom Export Events (DashboardConfigurator)

Subscribe to these events inside the `DashboardConfigurator` factory in `Program.cs`:

### `CustomExport` — Customize Before PDF/Image Is Generated

Fires before the document is rendered. Access the underlying `XRControl` (DevExpress Reports control) for each dashboard item and customize its appearance.

```csharp
configurator.CustomExport += (object sender, CustomExportWebEventArgs e) => {
    // Get a specific item's printable control ("gridDashboardItem1" is the item's ComponentName)
    var control = e.GetPrintableControl("gridDashboardItem1");
    // Modify the XRControl properties here
    // control is an XtraReport / XRControl descendant
};
```

### `CustomizeExportDocument` — Modify the Exported Stream

Fires after the document is generated (PDF, Image, or Excel). Use to post-process the stream. Most commonly used to customize Excel output with the Spreadsheet Document API.

The event fires for every export format — check `e.ExportAction` (a `DashboardExportAction`: `ExportToPdf`, `ExportToImage`, `ExportToExcel`, `PrintPreview`) to target one format:

```csharp
using DevExpress.DashboardCommon;
using DevExpress.DashboardWeb;
using DevExpress.Spreadsheet;

configurator.CustomizeExportDocument += (object sender, CustomizeExportDocumentWebEventArgs e) => {
    if (e.ExportAction == DashboardExportAction.ExportToExcel
        && e.ExcelExportOptions.Format == ExcelFormat.Xlsx) {
        e.Stream.Position = 0; // the stream is positioned at its end after generation — reset before loading
        Workbook workbook = new Workbook();
        workbook.LoadDocument(e.Stream, DocumentFormat.Xlsx);

        Worksheet sheet = workbook.Worksheets[0];
        sheet.Cells["A1"].Value = "Custom Header";

        e.Stream.Position = 0;
        e.Stream.SetLength(0);
        workbook.SaveDocument(e.Stream, DocumentFormat.Xlsx);
    }
};
```

CSV item exports also pass through this event (`e.ExcelExportOptions.Format == ExcelFormat.Csv` → `DocumentFormat.Csv`). Full runnable sample: the DevExpress example [winforms-dashboard-customize-exported-document](https://github.com/DevExpress-Examples/winforms-dashboard-customize-exported-document) (same pattern on the desktop controls).

> **`DevExpress.Spreadsheet.Workbook` is a separate library (Spreadsheet Document API), not part of the Dashboard packages.** It is not available unless the project references the `DevExpress.Document.Processor` NuGet package. Before generating or modifying code that uses `Workbook` (or any other Spreadsheet Document API type), verify the project has the required reference.


For custom dashboard items: call `e.GetDashboardItem(itemName)` and `e.GetItemData(itemName)` to access item data.

### Add Custom Headers, Logos, and Timestamps to Exported PDF

Handle `CustomExport` to add custom content (logo image, text label, date/page number) to the exported report using the XtraReports API:

```csharp
using DevExpress.XtraReports.UI;
using DevExpress.XtraPrinting; // PageInfo enum used by XRPageInfo.PageInfo
using DevExpress.DashboardWeb;

configurator.CustomExport += (object sender, CustomExportWebEventArgs e) => {
    // e.Report is the XtraReport for the whole dashboard
    XtraReport report = e.Report;

    // Add a page header band
    PageHeaderBand header = new PageHeaderBand { HeightF = 60 };
    report.Bands.Add(header);

    // Logo image
    XRPictureBox logo = new XRPictureBox {
        ImageUrl = "/images/logo.png",
        HeightF = 50, WidthF = 200, LeftF = 0
    };
    header.Controls.Add(logo);

    // Custom text label
    XRLabel title = new XRLabel {
        Text = "Confidential — Internal Use Only",
        LeftF = 210, WidthF = 400, HeightF = 20
    };
    header.Controls.Add(title);

    // Timestamp (auto-updated at render time)
    XRPageInfo timestamp = new XRPageInfo {
        PageInfo = PageInfo.DateTime,
        Format = "Generated: {0:dd MMM yyyy HH:mm}",
        LeftF = 210, TopF = 25, WidthF = 300
    };
    header.Controls.Add(timestamp);

    // Page footer with page numbers
    PageFooterBand footer = new PageFooterBand { HeightF = 30 };
    XRPageInfo pageNum = new XRPageInfo {
        PageInfo = PageInfo.NumberOfTotal,
        Format = "Page {0} of {1}",
        WidthF = 200
    };
    footer.Controls.Add(pageNum);
    report.Bands.Add(footer);
};
```

> `XtraReport`, `PageHeaderBand`, `XRLabel`, `XRPictureBox`, `XRPageInfo` are from the `DevExpress.XtraReports.UI` namespace (`DevExpress.Reporting.Core` NuGet package needs to be installed). The `CustomExport` event fires once per export operation before the report is rendered.

---

### `BeforeExportDocument` — Hide Items from Export

Fires before export starts. Call `e.HideItem(...)` to exclude items from the exported document without affecting the dashboard UI. Overloads: `HideItem(string itemComponentName)` and `HideItem(Predicate<DashboardItem> match)`.

```csharp
configurator.BeforeExportDocument += (object sender, BeforeExportDocumentWebEventArgs e) => {
    e.HideItem("imageItem1"); // "imageItem1" is the item's COMPONENT name, not its display name
    // or: e.HideItem(item => item is ImageDashboardItem);
};
```

---

## Export on Linux / macOS / Azure

Non-Windows — Linux, macOS, Docker and Azure — the drawing engine is already Skia. Reference `DevExpress.Drawing.Skia` (same version as the other DevExpress packages); without it, export throws `DllNotFoundException`. No startup code is needed: `DrawingEngine.Skia` is the default there, and `DashboardExportSettings.CompatibilityMode` has no effect.

**Linux containers** additionally need OS libraries in the Dockerfile:

```dockerfile
RUN apt-get update && apt-get install -y libc6 libicu74 libfontconfig1
# Dashboards with Text Box items (rendered via the Office RichEdit engine) additionally need:
RUN apt-get install -y libharfbuzz0b libfreetype6
```

> **Text Box items on Linux/Docker**: every other item renders fine, but loading a Text Box throws `The type initializer for 'SkiaSharp.SKImageInfo' threw an exception` / `Unable to load shared library 'libSkiaSharp'`. Fix: install the libraries above in the image stage that actually **runs** the app (in multi-stage builds — the final/base stage, not the SDK build stage). Reference: [Use Office File API on Linux](https://docs.devexpress.com/content/OfficeFileAPI/401441/prerequisites-and-installation/use-office-file-api-on-linux?md=true).

---

## ASP.NET MVC / Web Forms

Not covered by this skill. MVC (.NET Framework) uses `WebDashboardExporter` with the static `DashboardConfigurator.Default`; Web Forms uses `ASPxDashboardExporter`. Both expose the same export methods and options as `AspNetCoreDashboardExporter`. For a full example, query the docs MCP server:
`devexpress_docs_search(technologies=["Dashboard"], question="WebDashboardExporter server-side export ASP.NET MVC")`

---

## DashboardExporter (Non-Visual, Platform-Independent)

`DashboardExporter` (namespace `DevExpress.DashboardCommon`, NuGet package `DevExpress.Dashboard.Core`) works without a web server, UI controls, or `DashboardConfigurator`. Use it for console apps, background services, scheduled exports, and emailing dashboards.

```csharp
using DevExpress.DashboardCommon;

var exporter = new DashboardExporter(); // parameterless constructor

// Supply data and connections the same way DashboardConfigurator does — via events:
exporter.ConfigureDataConnection += (object s, DashboardConfigureDataConnectionEventArgs e) => {
    /* set e.ConnectionParameters */
};
exporter.DataLoading += (object s, DataLoadingEventArgs e) => {
    /* set e.Data for Object data sources */
};
exporter.ConnectionError += (object s, DashboardExporterConnectionErrorEventArgs e) =>
    Console.WriteLine($"Error in {e.DataSourceName}: {e.Exception.Message}");

// Accepts a Dashboard object, an XML stream, or a path to the dashboard XML:
using var stream = new MemoryStream();
exporter.ExportToPdf("Data/Dashboards/sales.xml", stream,
    options: new DashboardPdfExportOptions { ExportFilters = true });
// Signature: ExportToPdf(string dashboardXmlPath, Stream outputStream, Size? dashboardSize = null,
//                        DashboardState state = null, DashboardPdfExportOptions options = null)
// Sibling overloads take a Dashboard object or an XML Stream as the first argument.
```

It exposes the same export methods (`ExportToPdf`/`ExportToImage`/`ExportToExcel`, `ExportDashboardItemTo*`) and customization events (`CustomExport`, `CustomizeExportDocument`, `BeforeExportDocument`, `CustomParameters`, `CustomFilterExpression`) as the configurator. For a complete runnable sample, see the DevExpress example "bi-dashboard-non-visual-exporter" or use MCP:
```
devexpress_docs_search(technologies=["Dashboard"], question="DashboardExporter non-visual export console")
```

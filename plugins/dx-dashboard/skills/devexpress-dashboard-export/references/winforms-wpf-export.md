# Export — WinForms and WPF

## WinForms Designer (`DashboardDesigner`)

The `DashboardDesigner` control exposes export methods and properties directly.

### Export Methods

```csharp
using DevExpress.DashboardWin;
using DevExpress.DashboardCommon;

// Entire dashboard
dashboardDesigner1.ExportToPdf(filePath, options);
dashboardDesigner1.ExportToImage(filePath, options);
dashboardDesigner1.ExportToExcel(filePath, options);

// Single dashboard item — pass the item's component name
dashboardDesigner1.ExportDashboardItemToPdf(itemName, filePath, options);
dashboardDesigner1.ExportDashboardItemToImage(itemName, filePath, options);
dashboardDesigner1.ExportDashboardItemToExcel(itemName, filePath, options);
```

### Default Export Options

Set the default options that appear pre-filled in the export dialog:

```csharp
dashboardDesigner1.PdfExportOptions.ExportFilters = true;
dashboardDesigner1.ImageExportOptions.Format = DashboardExportImageFormat.Png;
dashboardDesigner1.ExcelExportOptions.Format = ExcelFormat.Xlsx;
```

### Control Export UI

```csharp
dashboardDesigner1.AllowPrintDashboard = true;      // show Export To button in title
dashboardDesigner1.AllowPrintDashboardItems = true; // show Export To button in captions

// Show export dialog programmatically
// DashboardExportFormat values: PDF, Image, Excel (note: PDF is all-caps)
dashboardDesigner1.ShowExportDashboardDialog(DashboardExportFormat.PDF);
dashboardDesigner1.ShowExportDashboardItemDialog(itemName, DashboardExportFormat.Excel);
```

### Print Preview

```csharp
// Show print preview for the entire dashboard
dashboardDesigner1.ShowPrintPreview();

// Ribbon-style print preview
dashboardDesigner1.ShowRibbonPrintPreview();

// Print preview options
dashboardDesigner1.PrintPreviewType = DashboardPrintPreviewType.RibbonPreview; // enum: DashboardPrintPreviewType (RibbonPreview is the default)
```

### Custom Export Events

```csharp
using DevExpress.DashboardCommon;
using DevExpress.Spreadsheet; // Workbook post-processing (Spreadsheet Document API)

// Fires before PDF/Image export — customize the underlying report (see the example below)
dashboardDesigner1.CustomExport += (object sender, CustomExportEventArgs e) => {
    // e.Report is the XtraReport for the exported document
};

// Fires after export is generated — post-process the result stream.
// Check e.ExportAction (DashboardExportAction) to target one format:
dashboardDesigner1.CustomizeExportDocument += (object sender, CustomizeExportDocumentEventArgs e) => {
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
    // CSV item exports pass through this event too:
    // e.ExcelExportOptions.Format == ExcelFormat.Csv → DocumentFormat.Csv.
    // Full runnable sample (incl. Xls/Csv branches and a logo image):
    // https://github.com/DevExpress-Examples/winforms-dashboard-customize-exported-document
};

// Fires before export — hide specific items
dashboardDesigner1.BeforeExportDocument += (object sender, BeforeExportDocumentEventArgs e) => {
    e.HideItem("imageDashboardItem1"); // component name, NOT display name
    // or: e.HideItem(item => item is ImageDashboardItem);
};
```

> **`DevExpress.Spreadsheet.Workbook` is a separate library (Spreadsheet Document API), not part of the Dashboard packages.** It is not available unless the project references it. Before generating or modifying code that uses `Workbook` (or any other Spreadsheet Document API type), verify the project has the required reference:
> - **SDK-style project (PackageReference):** ensure `DevExpress.Document.Processor` is referenced; if missing, add it (same DevExpress version as the other referenced packages) before generating code.
> - **Non-SDK-style project (assembly references, .NET Framework):** verify a reference to `DevExpress.Docs.vX.Y.dll` exists (it ships with a set of dependency assemblies — full list in the Office File API *Get Started*; the `DevExpress.Document.Processor` NuGet package is also an option on .NET Framework). If the reference is missing, ask the developer to add it — do not attempt to add or fix the reference yourself, and do not proceed with `Workbook` code until it's confirmed present.

### Example: Add Custom Information to the Exported Document

Use `e.Report` (the XtraReports instance) in `CustomExport` — the same header/logo/timestamp pattern as the ASP.NET Core example in [aspnetcore-export.md](aspnetcore-export.md):

```csharp
using DevExpress.DashboardCommon;
using DevExpress.XtraReports.UI;

dashboardDesigner1.CustomExport += (object sender, CustomExportEventArgs e) => {
    XtraReport report = e.Report;
    PageHeaderBand header = new PageHeaderBand { HeightF = 40 };
    header.Controls.Add(new XRLabel {
        Text = "Confidential — Internal Use Only", WidthF = 400, HeightF = 20
    });
    report.Bands.Add(header);
};
```

---

## WinForms Viewer (`DashboardViewer`)

`DashboardViewer` exposes the same export API as `DashboardDesigner`:

```csharp
using DevExpress.DashboardWin;

dashboardViewer1.ExportToPdf(filePath, options);
dashboardViewer1.ExportToImage(filePath, options);
dashboardViewer1.ExportToExcel(filePath, options);
dashboardViewer1.ExportDashboardItemToPdf(itemName, filePath, options);

dashboardViewer1.AllowPrintDashboard = true;
dashboardViewer1.AllowPrintDashboardItems = true;

dashboardViewer1.CustomExport += ...;
dashboardViewer1.CustomizeExportDocument += ...;
dashboardViewer1.BeforeExportDocument += ...;
```

---

## WPF (`DashboardControl`)

The WPF `DashboardControl` (namespace `DevExpress.DashboardWpf`) follows the same pattern:

```csharp
using DevExpress.DashboardWpf;
using DevExpress.DashboardCommon;

// Entire dashboard
dashboardControl.ExportToPdf(filePath, options);
dashboardControl.ExportToImage(filePath, options);
dashboardControl.ExportToExcel(filePath, options);

// Single item
dashboardControl.ExportDashboardItemToPdf(itemName, filePath, options);
dashboardControl.ExportDashboardItemToImage(itemName, filePath, options);
dashboardControl.ExportDashboardItemToExcel(itemName, filePath, options);

// Export options
dashboardControl.PdfExportOptions.ExportFilters = true;
dashboardControl.ImageExportOptions.Format = DashboardExportImageFormat.Png;

// Print preview
dashboardControl.ShowPrintPreview();
dashboardControl.ShowDashboardItemPrintPreview(itemName);

// Custom export events (same as WinForms)
dashboardControl.CustomExport += ...;
dashboardControl.CustomizeExportDocument += ...;
dashboardControl.BeforeExportDocument += ...;
```

---

## DashboardExporter — Non-Visual Export for WinForms/WPF

Use `DashboardExporter` (namespace `DevExpress.DashboardCommon`, NuGet `DevExpress.Dashboard.Core`) to export without launching any UI control. Useful for Windows Services, console apps, and batch export.

```csharp
using DevExpress.DashboardCommon;

var exporter = new DashboardExporter(); // parameterless constructor

// Error diagnostics — these events are the only way to see data failures:
exporter.ConnectionError += (object s, DashboardExporterConnectionErrorEventArgs e) =>
    Console.WriteLine($"Error in {e.DataSourceName}: {e.Exception.Message}");
exporter.DataLoadingError += (object s, DataLoadingErrorEventArgs e) => {
    foreach (DataLoadingError error in e.Errors)
        Console.WriteLine($"{error.DataSourceName}: {error.Error}");
};
exporter.DashboardItemDataLoadingError += (object s, DashboardItemDataLoadingErrorEventArgs e) => {
    foreach (DashboardItemDataLoadingError error in e.Errors)
        Console.WriteLine($"{error.DashboardItemName}: {error.Error}");
};

// Accepts a Dashboard object, an XML stream, or a path to the dashboard XML:
using var stream = new FileStream("sales.pdf", FileMode.Create);
exporter.ExportToPdf("Data/Dashboards/sales.xml", stream);

// With options — note options is the LAST optional parameter:
// exporter.ExportToPdf(dashboardOrPath, stream, options: new DashboardPdfExportOptions { ExportFilters = true });
```

Same export methods and customization events (`CustomExport`, `CustomizeExportDocument`, `BeforeExportDocument`) as the UI controls. Full sample: DevExpress example "bi-dashboard-non-visual-exporter".
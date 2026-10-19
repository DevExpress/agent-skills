# Getting Started — WinForms Reporting

## NuGet Setup (.NET 8+ / .NET Framework)

```bash
dotnet add package DevExpress.Win.Reporting 
```

## Assembly References (.NET Framework)

The examples use version `v26.1`; replace it with your installed version. Assemblies are located in
`C:\Program Files\DevExpress 26.1\Components\Bin\Framework` by default.

If this folder does not exist (NuGet-only machine), ask the developer to choose one of two options:

1. Provide the folder that contains the DevExpress assemblies.
2. Use NuGet packages instead.

**Print preview (`ReportPrintTool`):**
- `DevExpress.Data.v26.1.dll`
- `DevExpress.Data.Desktop.v26.1.dll`
- `DevExpress.Drawing.v26.1.dll`
- `DevExpress.Printing.v26.1.Core.dll`
- `DevExpress.Utils.v26.1.dll`
- `DevExpress.XtraBars.v26.1.dll`
- `DevExpress.XtraEditors.v26.1.dll`
- `DevExpress.XtraLayout.v26.1.dll`
- `DevExpress.XtraTreeList.v26.1.dll`
- `DevExpress.XtraPrinting.v26.1.dll`
- `DevExpress.XtraReports.v26.1.dll`

**End-User Designer (`ReportDesignTool`)**
- `DevExpress.CodeParser.v26.1.dll`
- `DevExpress.DataAccess.v26.1.dll`
- `DevExpress.DataAccess.v26.1.UI.dll`
- `DevExpress.Diagram.v26.1.Core.dll`
- `DevExpress.Images.v26.1.dll`
- `DevExpress.Utils.v26.1.UI.dll`
- `DevExpress.XtraDiagram.v26.1.dll`
- `DevExpress.XtraGrid.v26.1.dll`
- `DevExpress.XtraReports.v26.1.CodeCompletion.dll`
- `DevExpress.XtraReports.v26.1.Extensions.dll`

**Optional — depending on report content and export targets:**
- `DevExpress.Pdf.v26.1.Core.dll`, `DevExpress.Pdf.v26.1.Drawing.dll` — export to PDF / `XRPdfContent`
- `DevExpress.Office.v26.1.Core.dll`, `DevExpress.RichEdit.v26.1.Core.dll`, `DevExpress.RichEdit.v26.1.Export.dll` — export to DOCX / `XRRichText`
- `DevExpress.Charts.v26.1.Core.dll`, `DevExpress.XtraCharts.v26.1.dll` — `XRChart`
- `DevExpress.PivotGrid.v26.1.Core.dll`, `DevExpress.XtraPivotGrid.v26.1.dll` — `XRPivotGrid`
- `DevExpress.Sparkline.v26.1.Core.dll` — `XRSparkline`
- `DevExpress.XtraGauges.v26.1.Core.dll` — `XRGauge`
- `DevExpress.Xpo.v26.1.dll` — SQL / Entity Framework data sources

## Quick Start Patterns

### Pattern 1 — Show Report Preview with ReportPrintTool

Modal dialog with built-in ribbon toolbar (Print, Export, Email):

# [C#](#tab/csharp-preview)
```csharp
using DevExpress.XtraReports.UI;
using DevExpress.LookAndFeel;

private void ShowReportPreview() {
    // Create and bind report
    var report = new SalesReport { DataSource = GetSalesData() };
    
    // Show modal preview dialog
    var tool = new ReportPrintTool(report);
    tool.ShowRibbonPreviewDialog(UserLookAndFeel.Default);
}
```

# [VB.NET](#tab/vbnet-preview)
```vb
Imports DevExpress.XtraReports.UI
Imports DevExpress.LookAndFeel

Private Sub ShowReportPreview()
    ' Create and bind report
    Dim report = New SalesReport With {.DataSource = GetSalesData()}
    
    ' Show modal preview dialog
    Dim tool = New ReportPrintTool(report)
    tool.ShowRibbonPreviewDialog(UserLookAndFeel.Default)
End Sub
```
***

### Pattern 2 — Show the Built-in End-User Report Designer Form

`ReportDesignTool.ShowRibbonDesigner()` opens the built-in Ribbon designer in a separate non-modal window; it does not embed the designer in an existing form.

Add the following method to your WinForms form:

# [C#](#tab/csharp-designer)
```csharp
using DevExpress.XtraReports.UI;

private void OpenDesigner() {
    var designer = new ReportDesignTool(new XtraReport());
    designer.ShowRibbonDesigner();
}
```

# [VB.NET](#tab/vbnet-designer)
```vb
Imports DevExpress.XtraReports.UI

Private Sub OpenDesigner()
    Dim designer = New ReportDesignTool(New XtraReport())
    designer.ShowRibbonDesigner()
End Sub
```
***

Custom report storage is optional for this example. To configure it, see [Storage & Security](storage-and-security.md).

## Next Steps

1. **Viewer customization** → See `viewer-customization.md` for preview toolbar, ribbon, parameter editors, printing commands, and dock panels
2. **Designer customization** → See `designer-customization.md` to register custom parameter types
3. **Report Storage** → See `storage-and-security.md` for database-backed reports

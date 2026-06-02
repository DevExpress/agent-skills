# Print Preview — WinForms

## When to Use This Reference

Use when showing a print preview form, embedding a `DocumentViewer` control, or customizing the print preview toolbar.

## ReportPrintTool — Quick Reference

**NuGet package**: `DevExpress.Win.Reporting`  
**Namespace**: `DevExpress.XtraReports.UI` (ReportPrintTool), `DevExpress.LookAndFeel` (UserLookAndFeel)

`ReportPrintTool` wraps a report and provides shortcut methods to show preview forms.

```csharp
// Package: DevExpress.Win.Reporting
var tool = new ReportPrintTool(report);

// Non-modal (application continues running)
tool.ShowRibbonPreview();    // modern ribbon toolbar
tool.ShowPreview();          // standard toolbar

// Modal (blocks until user closes)
tool.ShowRibbonPreviewDialog(UserLookAndFeel.Default);
tool.ShowPreviewDialog(UserLookAndFeel.Default);
```

### VB.NET

```vb
Dim tool As New ReportPrintTool(report)
tool.ShowRibbonPreviewDialog(UserLookAndFeel.Default)
```

## Access the Preview Form

```csharp
var tool = new ReportPrintTool(report);
PrintPreviewRibbonFormEx previewForm = tool.PreviewRibbonForm;
previewForm.Text = "Sales Report Preview";
previewForm.ShowDialog();
```

## Embedded DocumentViewer

Place `DocumentViewer` inside your own form:

```csharp
public class MyPreviewForm : Form {
    DocumentViewer viewer;

    public MyPreviewForm(XtraReport report) {
        viewer = new DocumentViewer { Dock = DockStyle.Fill };
        Controls.Add(viewer);

        // Assign and build document
        viewer.DocumentSource = report;
        report.CreateDocument();
    }
}
```

## Large Reports — CachedReportSource

For reports that may generate thousands of pages, use `CachedReportSource` to avoid loading the entire document into RAM:

```csharp
using DevExpress.XtraPrinting.Caching;

var storage = new MemoryDocumentStorage();
var cached = new CachedReportSource(report, storage);

viewer.DocumentSource = cached;
cached.CreateDocumentAsync();   // async build — viewer shows progress
```

Storage options:
- `MemoryDocumentStorage` — compressed in-memory
- `FileDocumentStorage(path)` — stores pages as temp files on disk
- `DbDocumentStorage` — stores pages in a database (custom implementation)

## Customize PrintControl (Low-Level)

```csharp
// Access the underlying PrintControl
PrintPreviewRibbonFormEx previewForm = tool.PreviewRibbonForm;
PrintControl printControl = previewForm.PrintControl;
printControl.UseDirectXPaint = true;     // hardware acceleration
printControl.EnableSmoothScrolling = true;
```

## Hide/Add Ribbon Buttons

```csharp
PrintPreviewRibbonFormEx form = tool.PreviewRibbonForm;
RibbonControl ribbon = form.RibbonControl;

// Hide a built-in item (e.g., "Print" button)
foreach (var item in ribbon.Items) {
    if (item.Name == "printItem")
        item.Visibility = BarItemVisibility.Never;
}
```

For advanced toolbar customization (item IDs, adding custom buttons), use DxDocs MCP:
```
devexpress_docs_search(technology="WinForms Reporting", query="customize print preview toolbar ribbon")
```

## Print Without Preview

```csharp
// Direct print to default printer
var tool = new ReportPrintTool(report);
tool.Print();               // shows printer dialog
tool.PrintDialog();         // same — shows dialog

// Silent print (no dialog)
// See references/print-api.md
```

## VB.NET — Embedded Viewer

```vb
Public Class MyPreviewForm
    Inherits Form

    Private viewer As DocumentViewer

    Public Sub New(report As XtraReport)
        viewer = New DocumentViewer()
        viewer.Dock = DockStyle.Fill
        Controls.Add(viewer)

        viewer.DocumentSource = report
        report.CreateDocument()
    End Sub
End Class
```

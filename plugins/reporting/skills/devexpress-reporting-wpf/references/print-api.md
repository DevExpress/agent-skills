# Print API — WPF (Direct Print Without Preview)

## When to Use This Reference

Use when printing a report directly to a printer from a WPF application, without showing a preview window.

## PrintHelper — Print Dialog

```csharp
using DevExpress.Xpf.Printing;

var report = new SalesReport { DataSource = GetData() };

// Show print dialog and print
PrintHelper.PrintDialog(this, report);   // shows device/page setup dialog

// Print directly (no dialog) via PrintingSystem
report.CreateDocument();
report.PrintingSystem.Print();
```

## Silent Print (No Dialog)

```csharp
var report = new SalesReport { DataSource = GetData() };
report.CreateDocument();
report.Print();    // prints to default printer without dialog
```

## Print to Specific Printer

```csharp
report.CreateDocument();
var ps = report.PrintingSystem;
ps.PageSettings.PrinterSettings.PrinterName = "HP LaserJet Pro";
ps.Print();
```

## VB.NET

```vb
Imports DevExpress.Xpf.Printing
Imports DevExpress.XtraReports.UI

Dim report As New SalesReport()
report.DataSource = GetData()
PrintHelper.PrintDialog(Me, report)

' Or silent
report.CreateDocument()
report.Print()
```

## Notes

- `CreateDocument()` must be called before `Print()` or accessing `PrintingSystem`.
- For background printing in a WPF service or scheduled task, use `ExportToPdf` + OS print command, or use `PrintingSystem.Print()` after `CreateDocument()`.
- `PrintHelper.ShowRibbonPrintPreview()` is defined in `DevExpress.Xpf.Printing` and is the recommended path for interactive printing (user sees preview, then prints from there).

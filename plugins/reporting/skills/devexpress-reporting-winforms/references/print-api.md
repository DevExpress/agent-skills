# Print API — WinForms (Silent Print, Direct Print)

## When to Use This Reference

Use when printing a report directly to a printer without showing a preview, or when programmatically controlling the print job.

## Print via ReportPrintTool

```csharp
var tool = new ReportPrintTool(report);

// Shows printer dialog and prints
tool.Print();

// Silent print to default printer (no dialog)
report.CreateDocument();
report.Print();

// Print with printer settings
var printerSettings = new System.Drawing.Printing.PrinterSettings {
    PrinterName = "HP LaserJet Pro",
    Copies = 2
};
report.CreateDocument();
report.PrintingSystem.Print(printerSettings);
```

## PrintHelper (for forms without ReportPrintTool)

```csharp
using DevExpress.XtraPrinting;

// Show print dialog and print
PrintingSystem ps = report.PrintingSystem;
report.CreateDocument();
ps.ShowPrintDialog();
ps.Print();
```

## Page Range Print

```csharp
report.CreateDocument();
report.PrintingSystem.PageSettings.PrinterSettings.FromPage = 1;
report.PrintingSystem.PageSettings.PrinterSettings.ToPage = 3;
report.PrintingSystem.Print();
```

## VB.NET

```vb
Dim tool As New ReportPrintTool(report)
tool.Print()

' Silent print
report.CreateDocument()
report.Print()
```

## Notes

- `report.CreateDocument()` must be called before `Print()` or accessing `PrintingSystem`.
- `ReportPrintTool` handles `CreateDocument` internally when calling `ShowRibbonPreview`.
- For background printing in a service/scheduler, use `ExportToPdf` to a file and send to printer programmatically, or use the `PrintingSystem` approach above.

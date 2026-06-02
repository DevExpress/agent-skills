# Getting Started with DevExpress Spreadsheet Document API (.NET Framework)

This guide covers setup and usage of the Spreadsheet Document API in .NET Framework 4.6.2+ projects.

## When to Use This Reference

Use this when you need to:
- Install the Spreadsheet Document API in a .NET Framework 4.6.2+ project
- Understand which assembly DLLs to reference when not using NuGet
- Configure PDF export without `DevExpress.Pdf.SkiaRenderer` (uses Windows GDI)
- Understand differences between .NET and .NET Framework setup

For .NET 6/7/8+ projects, see [getting-started.md](getting-started.md).

## System Requirements

- .NET Framework 4.6.2+
- Visual Studio 2019+ (recommended)
- Windows OS (PDF rendering uses Windows GDI — not cross-platform)

## Installation

### Option A: NuGet Package (Recommended)

**Package Manager Console:**
```
Install-Package DevExpress.Document.Processor
```

> On .NET Framework, `DevExpress.Pdf.SkiaRenderer` is **not** required for PDF export — Windows GDI is used automatically.

### Option B: Manual Assembly References

If you have the DevExpress Unified Component Installer, add references to these DLLs (substitute `xx.x` with your version, e.g., `25.2`):

```
DevExpress.Charts.vxx.x.Core.dll
DevExpress.Data.vxx.x.dll
DevExpress.DataAccess.vxx.x.dll
DevExpress.DataVisualization.vxx.x.Core.dll
DevExpress.Docs.vxx.x.dll
DevExpress.Drawing.vxx.x.dll
DevExpress.Office.vxx.x.Core.dll
DevExpress.Pdf.vxx.x.Core.dll
DevExpress.Printing.vxx.x.Core.dll
DevExpress.Sparkline.vxx.x.Core.dll
DevExpress.Spreadsheet.vxx.x.Core.dll
DevExpress.TreeMap.vxx.x.Core.dll
DevExpress.XtraCharts.vxx.x.dll
DevExpress.XtraTreeMap.vxx.x.dll
```

In Visual Studio: right-click **References** → **Add Reference** → browse to the DevExpress installation folder (typically `C:\Program Files (x86)\DevExpress xx.x\Components\Bin\Framework\`).

## Your First Workbook (.NET Framework)

```csharp
using DevExpress.Spreadsheet;
using System.Drawing;

namespace SpreadsheetConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            using (Workbook workbook = new Workbook())
            {
                Worksheet worksheet = workbook.Worksheets[0];

                // Set the unit of measurement
                workbook.Unit = DevExpress.Office.DocumentUnit.Point;

                workbook.BeginUpdate();
                try
                {
                    // Write data
                    worksheet.Cells["A1"].Value = "Product";
                    worksheet.Cells["B1"].Value = "Revenue";
                    worksheet.Cells["A2"].Value = "Widget A";
                    worksheet.Cells["B2"].Value = 15000;
                    worksheet.Cells["A3"].Value = "Widget B";
                    worksheet.Cells["B3"].Value = 22000;

                    // Formula
                    worksheet.Cells["B4"].FormulaInvariant = "=SUM(B2:B3)";

                    // Formatting
                    CellRange headers = worksheet.Range["A1:B1"];
                    headers.Font.Bold = true;
                    headers.FillColor = Color.FromArgb(0x44, 0x72, 0xC4);
                    headers.Font.Color = Color.White;

                    // Number format
                    worksheet.Range["B2:B4"].NumberFormat = "$#,##0";

                    // Column widths
                    worksheet.Columns["A"].AutoFit();
                    worksheet.Columns["B"].AutoFit();
                }
                finally
                {
                    workbook.EndUpdate();
                }

                // Calculate all formulas
                workbook.Calculate();

                // Save as .xlsx
                workbook.SaveDocument("Report.xlsx", DocumentFormat.Xlsx);

                // Export to PDF (uses Windows GDI on .NET Framework — no SkiaRenderer needed)
                workbook.ExportToPdf("Report.pdf");
            }

            // Open files with the default application
            System.Diagnostics.Process.Start("Report.xlsx");
            System.Diagnostics.Process.Start("Report.pdf");
        }
    }
}
```

### VB.NET Example

```vb
Imports DevExpress.Spreadsheet
Imports System.Drawing

Module Module1
    Sub Main()
        Using workbook As New Workbook()
            Dim worksheet As Worksheet = workbook.Worksheets(0)
            workbook.Unit = DevExpress.Office.DocumentUnit.Point

            workbook.BeginUpdate()
            Try
                worksheet.Cells("A1").Value = "Product"
                worksheet.Cells("B1").Value = "Revenue"
                worksheet.Cells("A2").Value = "Widget A"
                worksheet.Cells("B2").Value = 15000
                worksheet.Cells("B3").FormulaInvariant = "=SUM(B2:B2)"

                worksheet.Range("A1:B1").Font.Bold = True
                worksheet.Range("B2:B3").NumberFormat = "$#,##0"
            Finally
                workbook.EndUpdate()
            End Try

            workbook.Calculate()
            workbook.SaveDocument("Report.xlsx", DocumentFormat.Xlsx)
            workbook.ExportToPdf("Report.pdf")
        End Using

        System.Diagnostics.Process.Start("Report.xlsx")
        System.Diagnostics.Process.Start("Report.pdf")
    End Sub
End Module
```

## Key Differences from .NET (6/7/8+)

| Aspect | .NET Framework | .NET (6/7/8+) |
|--------|---------------|----------------|
| PDF rendering | Windows GDI (built-in) | Requires `DevExpress.Pdf.SkiaRenderer` |
| SkiaRenderer needed | No | Yes (for PDF export) |
| Cross-platform | No (Windows only) | Yes (Linux/macOS/Windows) |
| `Process.Start` for files | `Process.Start("file.xlsx")` | `new ProcessStartInfo { UseShellExecute = true }` |
| NuGet package | `DevExpress.Document.Processor` only | + `DevExpress.Pdf.SkiaRenderer` |

## License Registration

DevExpress validates your license automatically if the DevExpress Unified Component Installer was used. For NuGet-only installations, follow the license deployment guide at:
`https://docs.devexpress.com/GeneralInformation/116042`

## Troubleshooting (.NET Framework Specific)

- **Missing type errors**: Ensure all required DLL references are added. The most common omission is `DevExpress.Docs.vxx.x.dll`.
- **PDF export fails silently**: Verify that GDI+ is available (it is on all standard Windows installs). Check that `DevExpress.Pdf.vxx.x.Core.dll` is referenced.
- **Version mismatch**: All DevExpress DLLs must be the same version. Mixing 25.1 and 25.2 DLLs will cause `FileLoadException`.

## What to Learn Next

- [Cell Formatting](cell-formatting.md): Apply fonts, colors, borders, number formats
- [Formulas](formulas.md): Add and calculate formulas
- [Export](export.md): Configure PDF and HTML export options

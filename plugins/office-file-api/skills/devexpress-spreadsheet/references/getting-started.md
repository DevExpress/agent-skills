# Getting Started with DevExpress Spreadsheet Document API (.NET)

This guide walks you through setting up and using the Spreadsheet Document API in .NET 6/7/8+ projects.

## When to Use This Reference

Use this when you need to:
- Install the Spreadsheet Document API in a new or existing .NET project
- Configure the DevExpress NuGet feed
- Create your first workbook and save it to disk
- Load an existing spreadsheet file and read its data
- Understand which file formats are supported for load and save

For .NET Framework 4.6.2+ projects, see [getting-started-dotnet-fw.md](getting-started-dotnet-fw.md).

## System Requirements

- .NET 6.0 / 7.0 / 8.0+
- Visual Studio 2022+ (recommended) or JetBrains Rider
- DevExpress NuGet feed configured (`https://nuget.devexpress.com/api`)

## Installation

### Step 1: Add the DevExpress NuGet Feed

**Visual Studio**: Tools → NuGet Package Manager → Package Manager Settings → Package Sources → Add:
- Name: `DevExpress`
- Source: `https://nuget.devexpress.com/api`

**.NET CLI**:
```bash
dotnet nuget add source https://nuget.devexpress.com/api -n DevExpress
```

### Step 2: Install NuGet Packages

**.NET CLI:**
```bash
dotnet add package DevExpress.Document.Processor
# For PDF export and printing (cross-platform rendering):
dotnet add package DevExpress.Pdf.SkiaRenderer
```

**Package Manager Console:**
```
Install-Package DevExpress.Document.Processor
Install-Package DevExpress.Pdf.SkiaRenderer
```

### Step 3: Register Your License

DevExpress validates your license automatically if you installed via the DevExpress installer. For NuGet-only installations, follow the license deployment guide at:
`https://docs.devexpress.com/GeneralInformation/116042`

## Your First Workbook

### Create a New Workbook

```csharp
using DevExpress.Spreadsheet;
using System.Drawing;

class Program
{
    static void Main()
    {
        using (Workbook workbook = new Workbook())
        {
            // Access the first worksheet (created automatically)
            Worksheet sheet = workbook.Worksheets[0];
            sheet.Name = "My Data";

            // Batch edits for performance
            workbook.BeginUpdate();
            try
            {
                // Write values
                sheet["A1"].Value = "Name";
                sheet["B1"].Value = "Amount";
                sheet["A2"].Value = "Alice";
                sheet["B2"].Value = 1500;
                sheet["A3"].Value = "Bob";
                sheet["B3"].Value = 2300;

                // Add a formula (always use FormulaInvariant for portability)
                sheet["B4"].FormulaInvariant = "=SUM(B2:B3)";

                // Format the header row
                sheet.Range["A1:B1"].Font.Bold = true;
                sheet.Range["A1:B1"].FillColor = Color.FromArgb(68, 114, 196);
                sheet.Range["A1:B1"].Font.Color = Color.White;

                // Currency format for numbers
                sheet.Range["B2:B4"].NumberFormat = "$#,##0";

                // Auto-fit columns
                sheet.Columns.AutoFit(0, 1);
            }
            finally
            {
                workbook.EndUpdate();
            }

            // Calculate all formulas
            workbook.Calculate();

            // Save as .xlsx
            workbook.SaveDocument("MyFirstWorkbook.xlsx");
            Console.WriteLine("Workbook saved.");
        }
    }
}
```

### Open an Existing Workbook

```csharp
using DevExpress.Spreadsheet;

using (Workbook workbook = new Workbook())
{
    // Load from a file path
    workbook.LoadDocument("existing.xlsx");

    // Access a worksheet by name
    Worksheet sheet = workbook.Worksheets["Sheet1"];

    // Read a single cell value
    CellValue value = sheet["A1"].Value;
    Console.WriteLine($"A1 contains: {value}");

    // Iterate over all used cells
    CellRange usedRange = sheet.GetUsedRange();
    for (int row = usedRange.TopRowIndex; row <= usedRange.BottomRowIndex; row++)
    {
        for (int col = usedRange.LeftColumnIndex; col <= usedRange.RightColumnIndex; col++)
        {
            Console.Write($"{sheet.Cells[row, col].Value}\t");
        }
        Console.WriteLine();
    }
}
```

### Load from a Stream

```csharp
using DevExpress.Spreadsheet;
using System.IO;

using (Workbook workbook = new Workbook())
{
    using (FileStream stream = new FileStream("Document.xlsx", FileMode.Open))
    {
        workbook.LoadDocument(stream, DocumentFormat.Xlsx);
    }
    // Work with workbook...
    workbook.SaveDocument("SavedDocument.xlsx", DocumentFormat.Xlsx);
}
```

### Modify and Save

```csharp
using DevExpress.Spreadsheet;

using (Workbook workbook = new Workbook())
{
    workbook.LoadDocument("report.xlsx");
    Worksheet sheet = workbook.Worksheets[0];

    // Update a cell
    sheet["A1"].Value = "Updated Report Title";

    // Add a new worksheet
    Worksheet summary = workbook.Worksheets.Add("Summary");
    summary["A1"].Value = "Generated on:";
    summary["B1"].Value = DateTime.Now;
    summary["B1"].NumberFormat = "yyyy-MM-dd HH:mm";

    // Save (overwrites original)
    workbook.SaveDocument("report.xlsx");

    // Or save in a different format
    workbook.SaveDocument("report_v2.xlsx", DocumentFormat.Xlsx);
}
```

### VB.NET — Create a Workbook

```vb
Imports DevExpress.Spreadsheet

Module Program
    Sub Main()
        Using workbook As New Workbook()
            Dim sheet As Worksheet = workbook.Worksheets(0)
            sheet.Name = "My Data"

            sheet("A1").Value = "Name"
            sheet("B1").Value = "Amount"
            sheet("A2").Value = "Alice"
            sheet("B2").Value = 1500

            sheet("B3").FormulaInvariant = "=SUM(B2:B2)"

            workbook.SaveDocument("MyFirstWorkbook.xlsx")
        End Using
    End Sub
End Module
```

## Supported File Formats

| Format | Extension | Load | Save |
|--------|-----------|------|------|
| Excel 2007+ (OOXML) | .xlsx | Yes | Yes |
| Excel 97-2003 | .xls | Yes | Yes |
| Macro-enabled | .xlsm | Yes | Yes |
| Binary | .xlsb | Yes | Yes |
| Template | .xltx, .xltm, .xlt | Yes | Yes |
| CSV | .csv | Yes | Yes |
| Tab-delimited text | .txt | Yes | Yes |
| XML Spreadsheet 2003 | .xml | Yes | Yes |
| PDF | .pdf | No | Yes (export) |
| HTML | .html | No | Yes (export) |

## Performance Tips

- Wrap bulk edits in `workbook.BeginUpdate()` / `workbook.EndUpdate()` to suppress recalculations and notifications.
- Call `workbook.Calculate()` once after all data is written, rather than on each cell change.
- Use `sheet.Range["A1:Z1000"].Value = someValue` to fill ranges in one operation rather than looping over individual cells.

## What to Learn Next

- [Cell Formatting](cell-formatting.md): Apply fonts, colors, borders, number formats, conditional formatting
- [Formulas](formulas.md): Add and calculate formulas, user-defined functions
- [Data Import/Export](data-import-export.md): Import from DataTable, collections, databases
- [Charts](charts.md): Create and configure charts
- [Export](export.md): Export to PDF, HTML, images

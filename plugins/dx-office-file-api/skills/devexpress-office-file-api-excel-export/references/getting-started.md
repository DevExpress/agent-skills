# Getting Started with the DevExpress Excel Export Library (.NET)

This guide walks you through setting up and using the Excel Export Library in a .NET 8/9/10+ project.

## When to Use This Reference

Use this when you need to:
- Set up the Excel Export Library for the first time in a .NET project
- Understand the NuGet package requirements
- Create your first XLSX file from scratch
- Use string-based formulas (XlFormulaParser)
- See a complete working end-to-end example

## System Requirements

- .NET 8.0 / 9.0 / 10.0+
- Visual Studio 2022+ (recommended) or JetBrains Rider
- Valid DevExpress license (Office File API or Universal Subscription)

## Installation

### Step 1: Install NuGet Package

**.NET CLI:**
```bash
dotnet add package DevExpress.Document.Processor
```

**Package Manager Console:**
```
Install-Package DevExpress.Document.Processor
```

> **Note on string formulas**: If you need to parse Excel formulas from text strings (e.g., `cell.SetFormula("=SUM(B2:B10)")`), the `XlFormulaParser` class is used. It is included in `DevExpress.Document.Processor` for .NET projects — no additional package is required.

### Step 2: Add the Namespace Import

```csharp
using DevExpress.Export.Xl;
using System.IO;
```

## Key Architecture Concept: Streaming

The Excel Export Library is a **streaming API**. This means:

- Data is written to the output stream incrementally as you create rows.
- You cannot go back and modify previously written rows or cells.
- Memory usage remains low regardless of file size because only the current row is buffered.
- The document is finalized and flushed when `IXlDocument` is disposed.

The object hierarchy is: `XlExport` → `IXlExporter` → `IXlDocument` → `IXlSheet` → `IXlRow` → `IXlCell`.

Every level must be disposed (via `using`) before moving to the next element at the same level.

## Your First Excel File

### Minimal Example

```csharp
using DevExpress.Export.Xl;
using System.IO;

// Create the exporter for XLSX format
IXlExporter exporter = XlExport.CreateExporter(XlDocumentFormat.Xlsx);

using (FileStream stream = new FileStream("Hello.xlsx", FileMode.Create, FileAccess.ReadWrite))
using (IXlDocument document = exporter.CreateDocument(stream))
using (IXlSheet sheet = document.CreateSheet())
{
    sheet.Name = "Sheet1";

    using (IXlRow row = sheet.CreateRow())
    using (IXlCell cell = row.CreateCell())
    {
        cell.Value = "Hello, DevExpress!";
    }
}
// File is saved when IXlDocument is disposed
```

### Complete Example with Headers, Data, and a Formula

```csharp
using DevExpress.Export.Xl;
using System.IO;

IXlExporter exporter = XlExport.CreateExporter(XlDocumentFormat.Xlsx);

using (FileStream stream = new FileStream("SalesReport.xlsx", FileMode.Create, FileAccess.ReadWrite))
using (IXlDocument document = exporter.CreateDocument(stream))
{
    using (IXlSheet sheet = document.CreateSheet())
    {
        sheet.Name = "Sales Report";

        // Freeze header row (must be set before creating columns/rows)
        sheet.SplitPosition = new XlCellPosition(0, 1);

        // Define column widths (must be defined before rows)
        using (IXlColumn col = sheet.CreateColumn()) { col.WidthInPixels = 160; } // A
        using (IXlColumn col = sheet.CreateColumn()) { col.WidthInPixels = 100; } // B
        using (IXlColumn col = sheet.CreateColumn()) { col.WidthInPixels = 100; } // C
        using (IXlColumn col = sheet.CreateColumn())                               // D
        {
            col.WidthInPixels = 100;
            col.Formatting = new XlCellFormatting();
            col.Formatting.NumberFormat = "$#,##0.00";
        }

        // Header formatting
        XlCellFormatting headerFmt = new XlCellFormatting();
        headerFmt.Font = new XlFont();
        headerFmt.Font.Bold = true;
        headerFmt.Font.Color = XlColor.FromTheme(XlThemeColor.Light1, 0.0);
        headerFmt.Fill = XlFill.SolidFill(XlColor.FromTheme(XlThemeColor.Accent2, 0.0));
        headerFmt.Alignment = XlCellAlignment.FromHV(XlHorizontalAlignment.Center, XlVerticalAlignment.Center);

        // Write header row
        using (IXlRow row = sheet.CreateRow())
        {
            foreach (string header in new[] { "Product", "Qty", "Unit Price", "Amount" })
            {
                using (IXlCell cell = row.CreateCell())
                {
                    cell.Value = header;
                    cell.ApplyFormatting(headerFmt);
                }
            }
        }

        // Write data rows
        string[] products = { "Camembert Pierrot", "Gorgonzola Telino", "Mozzarella di Giovanni" };
        int[] quantities = { 12, 25, 10 };
        double[] prices = { 23.25, 12.99, 8.95 };

        for (int i = 0; i < products.Length; i++)
        {
            int rowNum = i + 2; // 1-based, header is row 1
            using (IXlRow row = sheet.CreateRow())
            {
                using (IXlCell cell = row.CreateCell()) { cell.Value = products[i]; }
                using (IXlCell cell = row.CreateCell()) { cell.Value = quantities[i]; }
                using (IXlCell cell = row.CreateCell()) { cell.Value = prices[i]; }
                using (IXlCell cell = row.CreateCell())
                {
                    // Amount = Qty * Unit Price using SUBTOTAL-compatible expression
                    cell.SetFormula(XlOper.Multiply(
                        XlFunc.Param(new XlCellPosition(1, row.RowIndex)),
                        XlFunc.Param(new XlCellPosition(2, row.RowIndex))));
                }
            }
        }

        // Total row
        using (IXlRow row = sheet.CreateRow())
        {
            XlCellFormatting totalFmt = new XlCellFormatting();
            totalFmt.Font = new XlFont { Bold = true };
            totalFmt.Fill = XlFill.SolidFill(XlColor.FromTheme(XlThemeColor.Accent5, 0.6));

            using (IXlCell cell = row.CreateCell())
            {
                cell.Value = "Total";
                cell.ApplyFormatting(totalFmt);
            }
            row.SkipCells(2); // skip Qty and Unit Price columns
            using (IXlCell cell = row.CreateCell())
            {
                // SUM of D2:D{last data row}
                cell.SetFormula(XlFunc.Sum(XlCellRange.FromLTRB(3, 1, 3, products.Length)));
                cell.ApplyFormatting(totalFmt);
            }
        }

        // Enable AutoFilter on the data range
        sheet.AutoFilterRange = sheet.DataRange;
    }
}
```

## Using String Formulas (XlFormulaParser)

To write formulas as plain text strings, supply `XlFormulaParser` when creating the exporter:

```csharp
using DevExpress.Export.Xl;
using DevExpress.Spreadsheet; // XlFormulaParser lives here

IXlExporter exporter = XlExport.CreateExporter(XlDocumentFormat.Xlsx, new XlFormulaParser());

using (FileStream stream = new FileStream("WithFormulas.xlsx", FileMode.Create, FileAccess.ReadWrite))
using (IXlDocument document = exporter.CreateDocument(stream))
using (IXlSheet sheet = document.CreateSheet())
{
    // Write some values in A1:A5
    for (int i = 1; i <= 5; i++)
    {
        using (IXlRow row = sheet.CreateRow())
        using (IXlCell cell = row.CreateCell())
        {
            cell.Value = i * 100;
        }
    }

    // Write a SUM formula in A6 as a text string
    using (IXlRow row = sheet.CreateRow())
    using (IXlCell cell = row.CreateCell())
    {
        cell.SetFormula("=SUM(A1:A5)");
    }
}
```

> **Important**: `XlFormulaParser` is in the `DevExpress.Spreadsheet` namespace, included in `DevExpress.Document.Processor`. Always use **invariant (English) function names** in formula strings — they are locale-independent.

## Supported Output Formats

| Format | `XlDocumentFormat` value | Notes |
|--------|--------------------------|-------|
| Excel 2007+ | `XlDocumentFormat.Xlsx` | Recommended; full feature support |
| Excel 97-2003 | `XlDocumentFormat.Xls` | Legacy; some features unavailable |
| CSV | `XlDocumentFormat.Csv` | Text only; no formatting or formulas |

## What to Learn Next

- [references/cells-and-formatting.md](cells-and-formatting.md): Apply fonts, colors, borders, number formats, merge cells, add hyperlinks, conditional formatting
- [references/advanced-features.md](advanced-features.md): Tables, formulas, sparklines, pictures, printing, grouping, data validation
- [references/getting-started-dotnet-fw.md](getting-started-dotnet-fw.md): .NET Framework 4.6.2+ setup with assembly references

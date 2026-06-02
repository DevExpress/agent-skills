---
name: devexpress-excel-export
description: Build .NET applications with the DevExpress Excel Export Library for streaming generation of Excel files with low memory footprint. Use when generating large Excel reports, writing rows sequentially, building Excel files from data grids or report engines, or when the full Spreadsheet Document API would use too much memory. Also use when someone mentions "DevExpress Excel Export", "XlExport", "IXlExporter", "IXlSheet", "DevExpress.Export.Xl", "streaming Excel generation", "low-memory Excel", or asks about generating large Excel files efficiently with DevExpress. Covers both .NET and .NET Framework.
compatibility: Requires .NET 6+ or .NET Framework 4.6.2+. NuGet packages from the DevExpress feed.
metadata:
  author: DevExpress
  version: "25.2"
  source-commit: c5a96ff6e891a1c2633c6621186093faaefabefd
---

# DevExpress Excel Export Library

The Excel Export Library is a low-level, streaming .NET API for generating Excel files (.xlsx, .xls, .csv) with a minimal memory footprint. It writes rows sequentially to an output stream and cannot read or modify existing files. Use it when you need to produce large Excel reports from databases or data grids without loading the entire document into RAM. The primary namespace is `DevExpress.Export.Xl`.

## CRITICAL: Excel Export Library vs. Spreadsheet Document API

| | Excel Export Library | Spreadsheet Document API |
|---|---|---|
| **Model** | Streaming — write rows top-to-bottom, forward only | In-memory — random access to any cell at any time |
| **Memory** | Very low — ideal for large files | Higher — entire workbook in RAM |
| **Can read files?** | No | Yes |
| **Can modify existing files?** | No | Yes |
| **Charts, Pivot Tables?** | No | Yes |
| **NuGet** | `DevExpress.Document.Processor` (shared) | `DevExpress.Document.Processor` |
| **Namespace** | `DevExpress.Export.Xl` | `DevExpress.Spreadsheet` |

**Rule**: If the developer needs to read, modify, add charts, or create pivot tables → use the Spreadsheet Document API skill instead. If they need to stream-generate a large file from data → this skill is correct.

## When to Use This Skill

Use this skill when you need to:

- Generate large Excel reports (.xlsx) with low memory consumption
- Write rows sequentially from a database reader or IEnumerable data source
- Export grid or report engine data to Excel in a streaming fashion
- Create Excel files in server-side or cloud environments where RAM is constrained
- Apply cell formatting (fonts, colors, borders, number formats) during export
- Add formulas, subtotals, or AutoFilter to generated files
- Create conditional formatting rules on streamed data
- Build Excel tables (IXlTable) with styles and calculated columns
- Insert sparklines or pictures into generated worksheets
- Configure print settings, headers/footers, and page breaks
- Add data validation rules to cell ranges

## Prerequisites & Installation

### NuGet Packages

| Package | Purpose |
|---------|---------|
| `DevExpress.Document.Processor` | Excel Export Library (streaming API) |

String formulas (parsed from text) require an additional reference — see [references/getting-started.md](references/getting-started.md).

### .NET (6/7/8+)

```bash
dotnet add package DevExpress.Document.Processor
```

### .NET Framework (4.6.2+)

Add assembly references manually — see [references/getting-started-dotnet-fw.md](references/getting-started-dotnet-fw.md).

**Important**: All DevExpress packages in a project must share the same version number. A valid DevExpress license is required.

## Before You Start — Ask the Developer

Before generating code, ask these questions to avoid rework:

### General Questions
1. **Target framework**: Are you using .NET 8+, .NET 6/7, or .NET Framework 4.x?
2. **New or existing project?**: Are you creating a new project or adding to an existing one?
3. **Hosting model**: Console app, ASP.NET Core, Blazor, MAUI, WinForms, WPF, or something else?

### Excel Export-Specific Questions
4. **Scenario check**: Are you generating a new large report (this library is correct) or do you need to read/modify an existing file (use Spreadsheet Document API instead)?
5. **Data source**: IEnumerable / DataTable / direct row-by-row writing / database reader?
6. **Excel features beyond basic cells**: formulas / tables / conditional formatting / sparklines / pictures / printing?

> **Rule**: If the developer's answer is ambiguous or missing, ask before generating code. Do not guess.

## Component Overview

The Excel Export Library provides:

- **Exporter factory**: Create an exporter targeting XLSX, XLS, or CSV (`XlExport`, `IXlExporter`)
- **Document and sheet management**: Create documents and worksheets sequentially (`IXlDocument`, `IXlSheet`)
- **Row and column writing**: Stream rows and configure columns (`IXlRow`, `IXlColumn`)
- **Cell authoring**: Set values (string, number, bool, date, error), formulas, and formatting (`IXlCell`, `XlCellFormatting`)
- **Advanced worksheet features**: Conditional formatting, tables, sparklines, pictures, data validation, print settings

### Core Entry Point

```csharp
using DevExpress.Export.Xl;
using System.IO;

// Step 1: create an exporter for the target format
IXlExporter exporter = XlExport.CreateExporter(XlDocumentFormat.Xlsx);

// Step 2: open a stream and create the document
using (FileStream stream = new FileStream("output.xlsx", FileMode.Create, FileAccess.ReadWrite))
using (IXlDocument document = exporter.CreateDocument(stream))
{
    // Step 3: create a sheet
    using (IXlSheet sheet = document.CreateSheet())
    {
        sheet.Name = "Report";

        // Step 4: write rows top-to-bottom
        using (IXlRow row = sheet.CreateRow())
        using (IXlCell cell = row.CreateCell())
        {
            cell.Value = "Hello, World!";
        }
    }
} // document is finalized and flushed here
```

## Documentation & Navigation Guide

### Getting Started (.NET)
Refer to [references/getting-started.md](references/getting-started.md)

When you need to:
- Install NuGet packages and set up a .NET project
- Understand the streaming document model
- Create your first XLSX file end-to-end
- Use string formulas (requires XlFormulaParser)
- See a complete working example with headers and data rows

### Getting Started (.NET Framework)
Refer to [references/getting-started-dotnet-fw.md](references/getting-started-dotnet-fw.md)

When you need to:
- Set up a .NET Framework 4.6.2+ project with assembly references
- Understand which DLLs are required
- Handle framework-specific API differences

### Cells and Formatting
Refer to [references/cells-and-formatting.md](references/cells-and-formatting.md)

When you need to:
- Create cells and set values (string, numeric, bool, date, error)
- Apply fonts, colors, background fills, and borders
- Set number formats (currency, date, percentage, custom)
- Align cell content (horizontal, vertical, wrap, indent)
- Apply predefined or theme-based cell styles
- Create rich text (mixed fonts within a single cell)
- Merge cells or add hyperlinks
- Apply conditional formatting rules

### Advanced Features
Refer to [references/advanced-features.md](references/advanced-features.md)

When you need to:
- Create Excel tables (IXlTable) with styles and calculated columns
- Add string, expression, or token-based cell formulas
- Create shared formulas or SUBTOTAL aggregations
- Add sparklines (line, column, win/loss) and customize them
- Insert pictures and add hyperlinks to pictures
- Configure print settings, page margins, headers/footers, page breaks, print titles
- Group rows or columns
- Enable AutoFilter on a data range
- Add data validation rules

## Quick Start Example

```csharp
using DevExpress.Export.Xl;
using System.IO;

IXlExporter exporter = XlExport.CreateExporter(XlDocumentFormat.Xlsx);

using (FileStream stream = new FileStream("SalesReport.xlsx", FileMode.Create, FileAccess.ReadWrite))
using (IXlDocument document = exporter.CreateDocument(stream))
{
    using (IXlSheet sheet = document.CreateSheet())
    {
        sheet.Name = "Sales";

        // Freeze header row
        sheet.SplitPosition = new XlCellPosition(0, 1);

        // Header formatting
        XlCellFormatting headerFmt = new XlCellFormatting();
        headerFmt.Font = new XlFont { Bold = true };
        headerFmt.Font.Color = XlColor.FromTheme(XlThemeColor.Light1, 0.0);
        headerFmt.Fill = XlFill.SolidFill(XlColor.FromTheme(XlThemeColor.Accent1, 0.0));

        // Write header row
        string[] headers = { "Product", "Q1", "Q2", "Total" };
        using (IXlRow row = sheet.CreateRow())
        {
            foreach (string h in headers)
            {
                using (IXlCell cell = row.CreateCell())
                {
                    cell.Value = h;
                    cell.ApplyFormatting(headerFmt);
                }
            }
        }

        // Write 3 data rows
        string[] products = { "Widget A", "Widget B", "Widget C" };
        int[] q1 = { 12000, 8500, 15000 };
        int[] q2 = { 14500, 9200, 11000 };

        for (int i = 0; i < 3; i++)
        {
            using (IXlRow row = sheet.CreateRow())
            {
                using (IXlCell cell = row.CreateCell()) { cell.Value = products[i]; }
                using (IXlCell cell = row.CreateCell()) { cell.Value = q1[i]; }
                using (IXlCell cell = row.CreateCell()) { cell.Value = q2[i]; }
                using (IXlCell cell = row.CreateCell())
                {
                    // Formula: =B{row}+C{row}
                    int rowNum = i + 2;
                    cell.SetFormula($"=B{rowNum}+C{rowNum}");
                }
            }
        }

        // Enable AutoFilter
        sheet.AutoFilterRange = sheet.DataRange;
    }
}
```

### What This Does
Creates `SalesReport.xlsx` with a styled header row (bold, theme-colored), three data rows with numeric Q1/Q2 values, a formula-driven Total column, a frozen header row, and AutoFilter enabled. No Microsoft Office is required.

## Key Properties & API Surface

### XlExport (static entry point)

| Method | Return Type | Description |
|--------|-------------|-------------|
| `CreateExporter(XlDocumentFormat)` | `IXlExporter` | Create an exporter for XLSX, XLS, or CSV |
| `CreateExporter(XlDocumentFormat, XlFormulaParser)` | `IXlExporter` | Create with string formula parsing support |

### IXlExporter

| Method | Return Type | Description |
|--------|-------------|-------------|
| `CreateDocument(Stream)` | `IXlDocument` | Begin writing a new document to the stream |

### IXlDocument

| Property/Method | Type | Description |
|----------------|------|-------------|
| `CreateSheet()` | `IXlSheet` | Add and begin writing a new worksheet |
| `Options` | `IDataAwareExporterOptions` | Culture, encoding, and format options |
| `Theme` | `XlDocumentTheme` | Active document theme (default: Office 2013) |

### IXlSheet

| Property/Method | Type | Description |
|----------------|------|-------------|
| `CreateRow()` | `IXlRow` | Append and begin writing the next row |
| `CreateColumn()` | `IXlColumn` | Define the next column (call before rows) |
| `Name` | `string` | Worksheet tab name |
| `SplitPosition` | `XlCellPosition` | Freeze panes anchor (set before rows/columns) |
| `AutoFilterRange` | `XlCellRange` | Range where AutoFilter is applied |
| `DataRange` | `XlCellRange` | Range of all data written so far |
| `Visible` | `bool` | Show or hide the worksheet tab |
| `PrintOptions` | `XlPrintOptions` | Print-related settings |
| `PageSetup` | `XlPageSetup` | Page orientation, paper size, scaling |
| `PageMargins` | `XlPageMargins` | Margin settings for printing |
| `HeaderFooter` | `XlHeaderFooter` | Headers and footers for printed pages |
| `PrintTitles` | `XlPrintTitles` | Rows/columns repeated on each printed page |

### IXlRow

| Property/Method | Type | Description |
|----------------|------|-------------|
| `CreateCell()` | `IXlCell` | Append the next cell in this row |
| `CreateCell(int columnIndex)` | `IXlCell` | Create a cell at a specific column index |
| `SkipCells(int count)` | `void` | Skip columns (leave them empty) |
| `ApplyFormatting(XlCellFormatting)` | `void` | Apply formatting to the entire row |
| `Formatting` | `XlCellFormatting` | Row-level default formatting |
| `HeightInPixels` | `int` | Row height in pixels |
| `IsHidden` | `bool` | Hide the row |
| `RowIndex` | `int` | Zero-based row index |
| `OutlineLevel` | `int` | Grouping level (for row grouping) |

### IXlColumn

| Property/Method | Type | Description |
|----------------|------|-------------|
| `ApplyFormatting(XlCellFormatting)` | `void` | Apply formatting to the entire column |
| `Formatting` | `XlCellFormatting` | Column-level default formatting |
| `WidthInPixels` | `int` | Column width in pixels |
| `WidthInCharacters` | `double` | Column width in character units |
| `IsHidden` | `bool` | Hide the column |
| `OutlineLevel` | `int` | Grouping level (for column grouping) |

### IXlCell

| Property/Method | Type | Description |
|----------------|------|-------------|
| `Value` | `XlVariantValue` | Cell value (string, number, bool, or error) |
| `Formatting` | `XlCellFormatting` | Cell-level formatting |
| `ApplyFormatting(XlCellFormatting)` | `void` | Apply a formatting object to this cell |
| `SetFormula(string)` | `void` | Set formula from text (requires XlFormulaParser) |
| `SetFormula(IXlFormulaParameter)` | `void` | Set formula from expression objects |
| `SetFormula(XlExpression)` | `void` | Set formula from token list |
| `SetRichText(XlRichTextString)` | `void` | Set rich-formatted text (multiple fonts) |
| `ColumnIndex` | `int` | Zero-based column index of this cell |

### XlCellFormatting

| Property | Type | Description |
|----------|------|-------------|
| `Font` | `XlFont` | Font settings (name, size, bold, italic, color) |
| `Fill` | `XlFill` | Background fill (solid color or pattern) |
| `Alignment` | `XlCellAlignment` | Horizontal/vertical alignment, wrap, indent |
| `Border` | `XlBorder` | Border line styles and colors |
| `NumberFormat` | `string` | Excel number format string |

## Common Patterns

### Pattern 1: Apply Formatting to an Entire Column

```csharp
using (IXlColumn column = sheet.CreateColumn())
{
    column.WidthInPixels = 120;
    column.Formatting = new XlCellFormatting();
    column.Formatting.NumberFormat = "$#,##0.00";
}
```

### Pattern 2: Skip to a Specific Column

```csharp
using (IXlRow row = sheet.CreateRow())
{
    row.SkipCells(2); // leave columns A and B empty
    using (IXlCell cell = row.CreateCell()) // cell is in column C
    {
        cell.Value = "Starting at C";
    }
}
```

### Pattern 3: Password-Protect the Workbook

```csharp
using (IXlDocument document = exporter.CreateDocument(stream))
{
    document.Options.EncryptionOptions = new EncryptionOptions { Password = "MyPassword" };
    // ... create sheets and rows
}
```

### Pattern 4: Subtotal Formula (no string parser needed)

```csharp
using (IXlCell cell = row.CreateCell())
{
    // SUBTOTAL(9, C2:C10) — SUM ignoring hidden rows
    cell.SetFormula(XlFunc.Subtotal(
        XlCellRange.FromLTRB(2, 1, 2, 9),
        XlSummary.Sum,
        ignoreHiddenRows: true));
}
```

## Troubleshooting

| Symptom | Cause | Solution |
|---------|-------|----------|
| `InvalidOperationException` when setting `SplitPosition` | Set after rows/columns were already created | Set `sheet.SplitPosition` as the first operation after `CreateSheet()` |
| String formula throws `NullReferenceException` or is ignored | `XlFormulaParser` not supplied | Use `XlExport.CreateExporter(format, new XlFormulaParser())` |
| `XlFormulaParser` type not found | Missing assembly reference | Reference `DevExpress.Spreadsheet.vXX.X.Core.dll` (.NET FW) or ensure `DevExpress.Document.Processor` is installed (.NET) |
| Cells appear out of order | Cells must be written left-to-right within a row | Use `row.CreateCell(columnIndex)` to skip ahead, or `row.SkipCells(n)` |
| File is corrupt or truncated | `IXlDocument` or `IXlExporter` not disposed | Wrap all levels in `using` blocks; the document is finalized on `Dispose()` |
| Version mismatch build error | Mixed DevExpress package versions | Ensure all DX packages use the exact same version (e.g., all 25.2.x) |
| License error at runtime | Missing DevExpress license | Register license per installation guide; check license file deployment |

## Constraints & Rules

CRITICAL — follow these rules in every interaction:

1. **Streaming only**: This API writes forward. You cannot go back to modify a previously written row or cell.
2. **Dispose order matters**: Dispose `IXlCell` before creating the next cell, `IXlRow` before the next row, `IXlSheet` before the next sheet, and `IXlDocument` to finalize the file. Always use nested `using` blocks.
3. **Column declarations**: Call `sheet.CreateColumn()` for all columns before calling `sheet.CreateRow()`. Column order is sequential.
4. **Freeze panes**: Set `sheet.SplitPosition` before creating any columns or rows.
5. **String formulas**: Require `XlFormulaParser` passed to `XlExport.CreateExporter`. Prefer `XlFunc`/`XlOper` expression objects when parser is not available.
6. **NuGet packages**: Use `DevExpress.Document.Processor` for .NET. Do not guess other package names.
7. **Namespace imports**: Always include `using DevExpress.Export.Xl;` and `using System.IO;`.
8. **Version consistency**: All DevExpress packages must use the same version.
9. **No read/modify**: Never suggest using this library to open or modify existing files — redirect to Spreadsheet Document API.
10. **Framework detection**: Check .csproj for target framework before writing code. .NET Framework requires manual DLL references.

## Using DevExpress Documentation MCP

If the DxDocs MCP server is available, use it to supplement this skill:

- **Search**: Use `devexpress_docs_search` with technology "Excel Export Library" and your question.
- **Fetch**: Use `devexpress_docs_get_content` with a documentation URL to get full article content.

**When to use MCP vs. built-in references:**
- **Built-in references**: Getting started, common patterns, key properties, troubleshooting.
- **MCP search**: Advanced scenarios not covered here, version-specific changes, uncommon features.
- **Always MCP for**: Exact enum values, event signatures, or method overloads when uncertain.

---

## Next Steps

Start with **[Getting Started](references/getting-started.md)** to install and configure the Excel Export Library, then explore specific features through the navigation guide above.

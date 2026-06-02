# PDF & Image Export — DevExpress Spreadsheet Document API

Export workbooks and worksheets to PDF, HTML, and images; configure print settings and page layout.

## When to Use This Reference

Use this when you need to:
- Export a workbook or specific worksheet to PDF
- Configure PDF page size, margins, orientation, and headers/footers
- Export a worksheet to HTML
- Render a worksheet or cell range as a PNG/BMP/JPEG image
- Configure print settings (print area, page breaks, print titles, scale)
- Set up headers and footers for printed/exported pages

## Key Classes and Types

| Class/Interface | Purpose |
|----------------|---------|
| `Workbook.ExportToPdf(path)` | Export all worksheets to a PDF file |
| `Worksheet.ActiveView` | Sheet-level view settings (orientation, paper size, print options) |
| `WorksheetPrintOptions` | Print options: FitToPage, FitToWidth, GridLines, PrintArea, Margins |
| `WorksheetHeaderFooterOptions` | Header and footer text accessed via `Worksheet.HeaderFooterOptions` |
| `PageOrientation` | Enum: `Portrait` or `Landscape` |
| `DXPaperKind` | Enum for paper sizes (A4, Letter, etc.) — in `DevExpress.Drawing.Printing` |

## PDF Export

### Export an Entire Workbook to PDF

```csharp
using DevExpress.Spreadsheet;

using (Workbook workbook = new Workbook())
{
    workbook.LoadDocument("report.xlsx");

    // Simple export: all sheets, default settings
    workbook.ExportToPdf("report.pdf");
}
```

### Export with Custom Options

```csharp
using DevExpress.Spreadsheet;

using (Workbook workbook = new Workbook())
{
    workbook.LoadDocument("report.xlsx");

    // Access export options via the event
    workbook.BeforeExport += (sender, e) =>
    {
        if (e.DocumentFormat == DocumentFormat.Pdf)
        {
            var pdfOptions = (DevExpress.XtraSpreadsheet.Export.PdfDocumentExporterOptions)e.Options;
            pdfOptions.PageRange = "1"; // Export only page 1
        }
    };

    workbook.ExportToPdf("report.pdf");
}
```

### Export a Single Worksheet to PDF

```csharp
using DevExpress.Spreadsheet;

using (Workbook workbook = new Workbook())
{
    workbook.LoadDocument("data.xlsx");
    Worksheet sheet = workbook.Worksheets["Sales"];

    // Make the target worksheet active before exporting a single sheet
    workbook.Worksheets.ActiveWorksheet = sheet;

    workbook.ExportToPdf("sales.pdf");
}
```

### Export to PDF Stream

```csharp
using DevExpress.Spreadsheet;
using System.IO;

using (Workbook workbook = new Workbook())
{
    workbook.LoadDocument("report.xlsx");

    using (MemoryStream pdfStream = new MemoryStream())
    {
        workbook.ExportToPdf(pdfStream);
        byte[] pdfBytes = pdfStream.ToArray();
        // e.g., return as HTTP response, send via email, etc.
    }
}
```

## HTML Export

```csharp
using DevExpress.Spreadsheet;

using (Workbook workbook = new Workbook())
{
    workbook.LoadDocument("report.xlsx");
    Worksheet sheet = workbook.Worksheets[0];

    // Export the worksheet to HTML
    workbook.SaveDocument("output.html", DocumentFormat.Html);
}
```

## Image Export

### Export a Worksheet to an Image

```csharp
using DevExpress.Spreadsheet;

using (Workbook workbook = new Workbook())
{
    workbook.LoadDocument("report.xlsx");
    Worksheet sheet = workbook.Worksheets[0];

    // Export the entire used area to a PNG image
    sheet.ExportToImage("worksheet.png");
}
```

### Export a Cell Range to an Image

```csharp
using DevExpress.Spreadsheet;

using (Workbook workbook = new Workbook())
{
    workbook.LoadDocument("report.xlsx");
    Worksheet sheet = workbook.Worksheets[0];

    // Export a specific range
    CellRange range = sheet.Range["A1:F20"];
    range.ExportToImage("range.png");
}
```

## Page Setup & Print Configuration

Configure page layout before printing or exporting to PDF:

```csharp
using DevExpress.Spreadsheet;
using DevExpress.Drawing.Printing;

Worksheet sheet = workbook.Worksheets[0];

// Page orientation and paper size — on the ActiveView (SheetView)
sheet.ActiveView.Orientation = PageOrientation.Landscape;
sheet.ActiveView.PaperKind = DXPaperKind.A4;

// Print options — accessed via ActiveView.PrintOptions
WorksheetPrintOptions printOptions = sheet.ActiveView.PrintOptions;

// Fit to page(s)
printOptions.FitToPage = true;
printOptions.FitToWidth = 1;  // All columns fit on 1 page wide
printOptions.FitToHeight = 0; // Any number of pages tall

// Or set a scale percentage (overrides FitToPage)
// sheet.ActiveView.Scale = 75; // 75%

// Gridlines and headings
printOptions.PrintGridlines = false;
printOptions.PrintHeadings = false;

// Margins (in inches by default — access via SheetView)
sheet.ActiveView.Margins.Left = 0.5f;
sheet.ActiveView.Margins.Right = 0.5f;
sheet.ActiveView.Margins.Top = 0.75f;
sheet.ActiveView.Margins.Bottom = 0.75f;

// Center on page
sheet.ActiveView.CenterHorizontally = true;
sheet.ActiveView.CenterVertically = false;

// Print area
sheet.PrintArea = sheet.Range["A1:F50"];

// Print titles (repeat rows/columns on each printed page)
sheet.PrintTitles.SetRows(sheet.Range["1:1"]);     // Repeat row 1
sheet.PrintTitles.SetColumns(sheet.Range["A:A"]); // Repeat column A
```

## Headers and Footers

```csharp
using DevExpress.Spreadsheet;

Worksheet sheet = workbook.Worksheets[0];

// Headers and footers are accessed via Worksheet.HeaderFooterOptions
WorksheetHeaderFooterOptions hf = sheet.HeaderFooterOptions;

// Header: &L = left section, &C = center, &R = right
hf.OddHeader = "&LConfidential&C&[File]&RPage &[Page] of &[Pages]";

// Footer with date
hf.OddFooter = "&LPrinted: &[Date]&RPrepared by Finance Team";

// Different first page header
hf.DifferentFirst = true;
hf.FirstHeader = "&CTitle Page";
hf.FirstFooter = "";

// Align with margins
hf.AlignWithMargins = true;
```

Common header/footer codes:

| Code | Inserts |
|------|---------|
| `&[Page]` | Current page number |
| `&[Pages]` | Total page count |
| `&[Date]` | Current date |
| `&[Time]` | Current time |
| `&[File]` | File name |
| `&[Tab]` | Sheet (tab) name |
| `&L` / `&C` / `&R` | Left / Center / Right section |
| `&B` / `&I` / `&U` | Bold / Italic / Underline |
| `&nn` | Font size (e.g., `&14`) |

## Page Breaks

```csharp
// Insert a horizontal page break before row 30 (0-based)
sheet.HorizontalPageBreaks.Add(29);

// Insert a vertical page break before column G (index 6)
sheet.VerticalPageBreaks.Add(6);

// Remove a page break
sheet.HorizontalPageBreaks.Remove(29);

// Clear all page breaks
sheet.HorizontalPageBreaks.Clear();
sheet.VerticalPageBreaks.Clear();
```

## Platform Notes

| Feature | .NET (6/7/8+) | .NET Framework |
|---------|--------------|----------------|
| `ExportToPdf` | Requires `DevExpress.Pdf.SkiaRenderer` | Built-in (Windows GDI) |
| `ExportToImage` | Requires `DevExpress.Pdf.SkiaRenderer` | Built-in |
| Cross-platform rendering | Yes (Skia) | Windows only |

## Troubleshooting

- **PDF is blank or empty**: Ensure `sheet.PrintArea` is set or the worksheet has data in `GetUsedRange()`. Call `workbook.Calculate()` before exporting to ensure formula results are rendered.
- **Skia not found**: Add `DevExpress.Pdf.SkiaRenderer` NuGet package to your .NET project.
- **Image export produces a tiny image**: The image size depends on the range or used range dimensions. Set DPI using export options if available, or ensure the source range covers the intended area.
- **Headers/footers not appearing in exported PDF**: Verify `hf.OddHeader` / `hf.OddFooter` are set on the worksheet's `PrintOptions.HeaderFooter`, not on a chart sheet.
- **Landscape orientation not applied**: Set `ps.Orientation = PageOrientation.Landscape` before calling `ExportToPdf`. Changes take effect at export time.

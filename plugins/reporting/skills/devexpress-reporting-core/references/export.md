# Export API — ExportTo*** Methods and ExportOptions

## When to Use This Reference

Use when exporting a report to PDF, Excel, Word, CSV, HTML, image, or other formats using the `ExportTo***` methods, and when configuring export options classes.

## All Export Methods

Every export method is available in sync and async variants. Sync is fine for desktop apps; use async in web/API contexts.

| Format | Method | Options Class |
|--------|--------|---------------|
| PDF | `ExportToPdf` / `ExportToPdfAsync` | `PdfExportOptions` |
| XLSX | `ExportToXlsx` / `ExportToXlsxAsync` | `XlsxExportOptions` |
| XLS | `ExportToXls` / `ExportToXlsAsync` | `XlsExportOptions` |
| DOCX | `ExportToDocx` / `ExportToDocxAsync` | `DocxExportOptions` |
| RTF | `ExportToRtf` / `ExportToRtfAsync` | `RtfExportOptions` |
| HTML | `ExportToHtml` / `ExportToHtmlAsync` | `HtmlExportOptions` |
| MHT | `ExportToMht` / `ExportToMhtAsync` | `MhtExportOptions` |
| Image | `ExportToImage` / `ExportToImageAsync` | `ImageExportOptions` |
| CSV | `ExportToCsv` / `ExportToCsvAsync` | `CsvExportOptions` |
| Text | `ExportToText` / `ExportToTextAsync` | `TextExportOptions` |

## Export to File

```csharp
report.ExportToPdf("output.pdf");
report.ExportToXlsx("output.xlsx");
report.ExportToDocx("output.docx");
report.ExportToCsv("output.csv");
```

## Export to Stream (Web/API)

```csharp
using var ms = new MemoryStream();
report.ExportToPdf(ms);
ms.Position = 0;
// return ms as file response

// ASP.NET Core controller:
using var ms = new MemoryStream();
await report.ExportToPdfAsync(ms);
return File(ms.ToArray(), "application/pdf", "report.pdf");
```

## PdfExportOptions

```csharp
var opts = new PdfExportOptions {
    PageRange = "1-3,5",                      // specific pages
    PdfACompatibility = PdfACompatibility.PdfA2b,
    ImageQuality = PdfJpegImageQuality.Highest,
    DocumentOptions = {
        Author = "DevExpress",
        Title = "Sales Report",
        Subject = "Monthly Sales"
    }
};
report.ExportToPdf("output.pdf", opts);
```

Key properties: `PageRange`, `PdfACompatibility`, `ImageQuality`, `ConvertImagesToJpeg`, `DocumentOptions` (Author, Title, Subject, Creator), `EncryptionOptions` (password protection), `SignatureOptions`.

## XlsxExportOptions

```csharp
var opts = new XlsxExportOptions {
    ExportMode = XlsxExportMode.SingleFile,
    SheetName = "Products",
    ShowGridLines = true,
    RightToLeftDocument = false,
    TextExportMode = TextExportMode.Value    // export raw values, not formatted text
};
report.ExportToXlsx("output.xlsx", opts);
```

Key properties: `ExportMode` (SingleFile, SingleFilePageByPage, DifferentFiles), `SheetName`, `ShowGridLines`, `TextExportMode`, `SuppressEmptyRows`, `AllowSpannedColumnsOnly`.

## XlsExportOptions

```csharp
var opts = new XlsExportOptions {
    ExportMode = XlsExportMode.SingleFile,
    TextExportMode = TextExportMode.Value
};
report.ExportToXls("output.xls", opts);
```

## DocxExportOptions

```csharp
var opts = new DocxExportOptions {
    ExportPageBreaks = true,
    PageRange = "1-5",
    RasterizeImages = false
};
report.ExportToDocx("output.docx", opts);
```

## CsvExportOptions

```csharp
var opts = new CsvExportOptions {
    Separator = ",",
    TextExportMode = TextExportMode.Value,
    Encoding = Encoding.UTF8,
    SkipEmptyRows = true,
    SkipEmptyColumns = true
};
report.ExportToCsv("output.csv", opts);
```

## HtmlExportOptions

```csharp
var opts = new HtmlExportOptions {
    ExportMode = HtmlExportMode.SingleFile,
    Title = "Report",
    EmbedImagesInHTML = true,
    PageRange = "1"
};
report.ExportToHtml("output.html", opts);
```

## ImageExportOptions

```csharp
var opts = new ImageExportOptions {
    Format = ImageFormat.Png,
    Resolution = 150,
    PageRange = "1"
};
report.ExportToImage("output.png", opts);
```

Key `Format` values: `ImageFormat.Png`, `ImageFormat.Jpeg`, `ImageFormat.Tiff`, `ImageFormat.Bmp`, `ImageFormat.Gif`.

## Large Report Export — PdfStreamingExporter

For reports with thousands of pages, use `PdfStreamingExporter` to avoid loading the entire document into memory:

```csharp
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Caching;

var storage = new MemoryDocumentStorage();
var cachedSource = new CachedReportSource(report, storage);
await cachedSource.CreateDocumentAsync();

using var fs = new FileStream("large.pdf", FileMode.Create);
var exporter = new PdfStreamingExporter(cachedSource);
await exporter.ExportAsync(fs);
```

## Default ExportOptions on Report

Set defaults that apply when no options object is passed:

```csharp
report.ExportOptions.Pdf.PdfACompatibility = PdfACompatibility.PdfA1b;
report.ExportOptions.Xlsx.SheetName = "Data";
```

## VB.NET

```vb
' Export to file
report.ExportToPdf("output.pdf")

' Export to stream with options
Dim opts As New PdfExportOptions()
opts.PageRange = "1-3"
opts.PdfACompatibility = PdfACompatibility.PdfA2b

Using ms As New MemoryStream()
    report.ExportToPdf(ms, opts)
    ' use ms...
End Using
```

# Export — DevExpress Word Processing Document API

The Word Processing Document API can export documents to PDF, HTML, RTF, ODT, plain text, and series of page images. PDF export supports advanced options including PDF/A compliance, password security, digital signature metadata, and accessibility (PDF/UA). HTML export supports embedded or external images. Printing is supported on Windows and via CUPS on Linux/macOS.

## When to Use This Reference

Use this when you need to:
- Export a Word document to PDF with custom options (page range, PDF/A, password, author)
- Export to HTML with embedded images or external image files
- Save in RTF, ODT, plain text, MHT, or WordML formats
- Export document pages as a series of PNG/JPG images
- Print a document using the default printer or custom `PrinterSettings`
- Configure printing options (background color, comment display)
- Handle `BeforeExport` to customize format-specific export options

## PDF Export

### Basic Export to PDF

```csharp
using DevExpress.XtraRichEdit;

using (var server = new RichEditDocumentServer())
{
    server.LoadDocument("input.docx", DocumentFormat.Docx);
    server.ExportToPdf("output.pdf");
}
```

### Export to PDF with Options

```csharp
using DevExpress.XtraRichEdit;
using DevExpress.XtraPrinting;
using System.IO;

using (var server = new RichEditDocumentServer())
{
    server.LoadDocument("input.docx", DocumentFormat.Docx);

    PdfExportOptions options = new PdfExportOptions();

    // Document metadata
    options.DocumentOptions.Author = "My Application";
    options.DocumentOptions.Title = "Exported Report";

    // Image quality (smaller file)
    options.ConvertImagesToJpeg = true;
    options.ImageQuality = PdfJpegImageQuality.High;

    // Page range: export pages 1, 3, and 5-12
    options.PageRange = "1,3,5-12";

    // Compression
    options.Compressed = true;

    // Export to file
    server.ExportToPdf("output.pdf", options);

    // Or export to a stream
    using (FileStream stream = new FileStream("output.pdf", FileMode.Create))
    {
        server.ExportToPdf(stream, options);
    }
}
```

### PDF/A Compliance

```csharp
PdfExportOptions options = new PdfExportOptions();
options.PdfACompatibility = PdfACompatibility.PdfA2b; // ISO 19005-2:2011

// Validate options before export
if (options.Validate() == null) // null = no validation errors
{
    server.ExportToPdf("output_pdfa.pdf", options);
}
```

Available values: `PdfACompatibility.None` (default), `PdfA1a`, `PdfA1b`, `PdfA2a`, `PdfA2b`, `PdfA3a`, `PdfA3b`.

> **Limitations**: PDF/A does not support password security, `NeverEmbeddedFonts`, or `ShowPrintDialogOnOpen`. Transparent images are not supported in PDF/A-1.

### PDF/UA (Accessibility)

```csharp
options.PdfUACompatibility = PdfUACompatibility.PdfUA1;  // ISO 14289-1
options.PdfUACompatibility = PdfUACompatibility.PdfUA2;  // ISO 14289-2 (v26.1+)
```

### Password Security

```csharp
PdfExportOptions options = new PdfExportOptions();
options.PasswordSecurityOptions.OpenPassword = "open123";
options.PasswordSecurityOptions.EncryptionLevel = PdfEncryptionAlgorithm.AES256;
options.PasswordSecurityOptions.PermissionsOptions.PrintingPermissions = PdfPrintingPermissions.LowResolution;
options.PasswordSecurityOptions.PermissionsOptions.EnableCopying = false;
```

### Page Background Color

```csharp
using System.Drawing;

server.Options.Printing.EnablePageBackgroundOnPrint = true;
server.Document.SetPageBackground(Color.LightYellow);
server.ExportToPdf("output.pdf");
```

## HTML Export

### Export to HTML String

```csharp
// Access HTML content directly via the Document property
string htmlContent = server.Document.HtmlText;
File.WriteAllText("output.html", htmlContent);
```

### Export to HTML with Options via BeforeExport

```csharp
using DevExpress.XtraRichEdit.Export;

server.BeforeExport += (sender, e) =>
{
    if (e.DocumentFormat == DocumentFormat.Html)
    {
        HtmlDocumentExporterOptions htmlOptions = e.Options as HtmlDocumentExporterOptions;
        htmlOptions.EmbedImages = true;           // Embed images as base64 data URIs
        htmlOptions.CssPropertiesExportType = CssPropertiesExportType.Style; // Inline CSS
        htmlOptions.UseFontSubstitution = false;
    }
};

server.SaveDocument("output.html", DocumentFormat.Html);
```

### Export Images to Separate Files

```csharp
server.BeforeExport += (sender, e) =>
{
    if (e.DocumentFormat == DocumentFormat.Html)
    {
        HtmlDocumentExporterOptions opts = e.Options as HtmlDocumentExporterOptions;
        opts.EmbedImages = false;
        opts.ExportImageFolder = "output_images"; // Relative folder for image files
    }
};
server.SaveDocument("output.html", DocumentFormat.Html);
```

## RTF Export

```csharp
// Save as RTF
server.SaveDocument("output.rtf", DocumentFormat.Rtf);

// Access RTF content as a string
string rtfContent = server.Document.RtfText;
```

Configure RTF-specific options in `BeforeExport`:
```csharp
server.BeforeExport += (sender, e) =>
{
    if (e.DocumentFormat == DocumentFormat.Rtf)
    {
        RtfDocumentExporterOptions rtfOptions = e.Options as RtfDocumentExporterOptions;
        // Configure as needed
    }
};
```

## ODT Export

```csharp
server.SaveDocument("output.odt", DocumentFormat.OpenDocument);

// Access ODT bytes
byte[] odtBytes = server.Document.OpenDocumentBytes;
```

## Plain Text Export

```csharp
server.SaveDocument("output.txt", DocumentFormat.PlainText);

// Access as string
string plainText = server.Document.Text;
```

Configure in `BeforeExport`:
```csharp
server.BeforeExport += (sender, e) =>
{
    if (e.DocumentFormat == DocumentFormat.PlainText)
    {
        PlainTextDocumentExporterOptions opts = e.Options as PlainTextDocumentExporterOptions;
        opts.ExportHiddenText = false;
    }
};
```

## DOCX-Specific Export Options

```csharp
server.BeforeExport += (sender, e) =>
{
    if (e.DocumentFormat == DocumentFormat.Docx)
    {
        OpenXmlDocumentExporterOptions docxOptions = e.Options as OpenXmlDocumentExporterOptions;
        docxOptions.ExportedDocumentProperties =
            DocumentPropertyNames.Title |
            DocumentPropertyNames.LastModifiedBy |
            DocumentPropertyNames.Modified;
    }
};
server.SaveDocument("output.docx", DocumentFormat.Docx);
```

## Export Pages as Images

Export each document page as a raster image (PNG, JPEG, etc.):

```csharp
using DevExpress.XtraRichEdit;

using (var server = new RichEditDocumentServer())
{
    server.LoadDocument("input.docx", DocumentFormat.Docx);

    // Export all pages to a folder
    // The API uses the DocumentLayout for image rendering
    // Use the async API for image series export:
    // server.ExportToImages(...)  -- check MCP/docs for exact overload
}
```

> For image series export, use `RichEditDocumentServer` with the layout engine. Consult the DevExpress documentation (`devexpress_docs_search` with "export to images Word Processing") for the exact `ExportToImages` overload available in your version.

> **Non-Windows**: image export renders PDF/document page content, so on Linux, macOS, Docker, or cloud hosts it requires the `DevExpress.Pdf.SkiaRenderer` package in addition to `DevExpress.Drawing.Skia`. The Skia drawing engine is enabled automatically on non-Windows platforms. See [Non-Windows Platform Support](getting-started.md#non-windows-platform-support-linux-macos-docker-cloud) in Getting Started.

## Printing

### Print with Default Printer

```csharp
using (var server = new RichEditDocumentServer())
{
    server.LoadDocument("input.docx", DocumentFormat.Docx);
    server.Print(); // Uses system default printer
}
```

### Print with Custom Settings

```csharp
using System.Drawing.Printing;

using (var server = new RichEditDocumentServer())
{
    server.LoadDocument("input.docx", DocumentFormat.Docx);

    PrinterSettings settings = new PrinterSettings();
    settings.PrinterName = "My Printer";
    settings.Copies = 2;
    settings.FromPage = 1;
    settings.ToPage = 5;

    server.Print(settings);
}
```

### Print Options

```csharp
PrintingOptions printOptions = server.Options.Printing;
printOptions.EnableCommentBackgroundOnPrint = false;
printOptions.EnablePageBackgroundOnPrint = true;
```

### Print on Linux/macOS (CUPS)

```csharp
using DevExpress.Drawing.Printing;

// Use DXPrinterSettings instead of System.Drawing.Printing.PrinterSettings
DXPrinterSettings dxSettings = new DXPrinterSettings();
dxSettings.PrinterName = "CUPS_Printer_Name";
server.Print(dxSettings);
```

> Requires the `libcups2` package to be installed on the system.

## BeforeImport — Customize Load Options

```csharp
using DevExpress.XtraRichEdit.Import;

server.BeforeImport += (sender, e) =>
{
    if (e.DocumentFormat == DocumentFormat.PlainText)
    {
        ((PlainTextDocumentImporterOptions)e.Options).AutoDetectEncoding = true;
    }
    if (e.DocumentFormat == DocumentFormat.Html)
    {
        ((HtmlDocumentImporterOptions)e.Options).AsyncImageLoading = false;
    }
};
server.LoadDocument("input.html", DocumentFormat.Html);
```

## Troubleshooting

| Symptom | Cause | Solution |
|---------|-------|----------|
| `No usable version of ICU libraries` on Linux | Missing ICU for PDF | Set `DXEXPORT_ICU_VERSION_OVERRIDE=65.1` env var or install `libicu` |
| PDF/A export fails validation | Incompatible options (e.g., password + PDF/A) | Call `options.Validate()` before export; disable conflicting options |
| Exported PDF has no hyperlinks in Adobe Reader | Adobe Reader setting | Enable "Create links from URLs" in Adobe Reader Preferences > General |
| HTML images missing | `EmbedImages = false` with no output folder | Set `opts.ExportImageFolder` or set `EmbedImages = true` |
| Print produces wrong page size | `PrinterSettings.DefaultPageSettings.Landscape` ignored | Set page size via `Section.Page.PaperKind` and `Section.Page.Landscape` instead |
| WMF images not exported to PDF | WMF not supported in PDF export | Convert WMF to EMF or EMF+ before saving the document |

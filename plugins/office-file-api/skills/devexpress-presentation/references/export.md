# Export, Merge, Split, and Print — DevExpress Presentation API

This reference covers exporting presentations to PDF (with options), merging multiple presentations into one, splitting a presentation into parts, and printing.

## When to Use This Reference

Use this when you need to:
- Export a .pptx presentation to a PDF file or stream
- Configure PDF export options (password security, image optimization, digital signatures, attachments)
- Merge slides from multiple presentations into a single .pptx
- Split a presentation into separate files by extracting slide subsets
- Print a presentation to the default or a specified printer
- Copy individual slides from one presentation to another

## Key Classes and Types

| Class/Interface | Purpose |
|----------------|---------|
| `Presentation.ExportToPdf` | Export the presentation to PDF |
| `PdfExportOptions` | Configure PDF export (namespace: `DevExpress.Docs.Presentation.Export`) |
| `EncryptionOptions` | Password protection and permissions for exported PDF |
| `SignatureOptions` | Digital signature settings for exported PDF |
| `PrintOptions` | Print settings (namespace: `DevExpress.Docs.Presentation.Printing`) |
| `SlideCollection.Move` | Reorder slides |
| `Slide.Clone()` | Duplicate a slide for cross-presentation copying |

## PDF Export

### Basic Export

```csharp
using DevExpress.Docs.Presentation;

// Export to a file path
presentation.ExportToPdf("output.pdf");

// Export to a stream
using FileStream pdfStream = new FileStream("output.pdf", FileMode.Create, FileAccess.ReadWrite);
presentation.ExportToPdf(pdfStream);
```

### Export with Options

```csharp
using DevExpress.Docs.Presentation;
using DevExpress.Docs.Presentation.Export;
using DevExpress.Docs.Pdf;

var options = new PdfExportOptions();

// Image optimization
options.ConvertImagesToJpeg = true;
options.ImageQuality = ImageQuality.Medium;
options.RasterizeImages = true;
options.RasterizationResolution = 150;  // DPI

using FileStream pdfStream = new FileStream("output.pdf", FileMode.Create, FileAccess.ReadWrite);
presentation.ExportToPdf(pdfStream, options);
```

### PDF Security (Password Protection)

```csharp
using DevExpress.Docs.Presentation.Export;
using DevExpress.Docs.Pdf;

var options = new PdfExportOptions();

// User password restricts opening the document
// Owner password restricts operations (modification, printing, etc.)
options.EncryptionOptions = new EncryptionOptions(
    userPassword: "view123",
    ownerPassword: "admin456"
);

// Optionally restrict permissions
options.EncryptionOptions.PrintPermissions = PdfDocumentPrintPermissions.NotAllowed;
options.EncryptionOptions.ModificationPermissions = PdfDocumentModificationPermissions.NotAllowed;

using FileStream pdfStream = new FileStream("secured.pdf", FileMode.Create, FileAccess.ReadWrite);
presentation.ExportToPdf(pdfStream, options);
```

### PDF Attachments

```csharp
using DevExpress.Docs.Presentation.Export;
using DevExpress.Docs.Pdf;

var options = new PdfExportOptions();
options.Attachments.Add(new Attachment(
    File.ReadAllBytes("data.xlsx"),
    "data.xlsx",
    "Source data spreadsheet"
));

using FileStream pdfStream = new FileStream("with-attachment.pdf", FileMode.Create, FileAccess.ReadWrite);
presentation.ExportToPdf(pdfStream, options);
```

### PDF Document Properties

```csharp
using DevExpress.Docs.Presentation.Export;

var options = new PdfExportOptions();
options.DocumentOptions.Title = "Q4 Report";
options.DocumentOptions.Author = "DevExpress";
options.DocumentOptions.Subject = "Quarterly Sales";
options.DocumentOptions.Keywords = "sales, Q4, 2025";

using FileStream pdfStream = new FileStream("output.pdf", FileMode.Create, FileAccess.ReadWrite);
presentation.ExportToPdf(pdfStream, options);
```

### PDF Export Limitations

Be aware of the following known limitations in v25.2:
- All presentation content is exported **as images** — text in exported PDFs is not selectable.
- The following elements are **not exported** to PDF: SmartArt diagrams, Charts, Comments, `FillOverlayEffect`.
- `PdfExportOptions.NonEmbeddedFonts` has no effect (content is rasterized).
- PDF/A and PDF/UA compliance modes are not available.
- Certain combinations of `ModificationPermissions` and `InteractivityPermissions` are not supported.

## Merging Presentations

### Append All Slides from One Presentation to Another

```csharp
using DevExpress.Docs.Presentation;

Presentation target = new Presentation(File.ReadAllBytes("target.pptx"));
Presentation source = new Presentation(File.ReadAllBytes("source.pptx"));

// Append all slides from source to target
foreach (Slide slide in source.Slides) {
    target.Slides.Add(slide);
}

// Save the merged result
using FileStream outStream = new FileStream("merged.pptx", FileMode.Create);
target.SaveDocument(outStream);
```

**Notes on merging:**
- Merged slides keep their original formatting and styles.
- Slide masters used by source slides are automatically transferred to the target.
- If the target slide size is smaller, some shapes may fall outside the visible area but remain in the file.

### Insert a Single Slide from Another Presentation

```csharp
Presentation source = new Presentation(File.ReadAllBytes("source.pptx"));
Presentation target = new Presentation(File.ReadAllBytes("target.pptx"));

// Clone the slide before inserting to avoid modifying the source
Slide slideToInsert = source.Slides[0].Clone();

// Insert at the beginning of the target
target.Slides.Insert(0, slideToInsert);

using FileStream outStream = new FileStream("result.pptx", FileMode.Create);
target.SaveDocument(outStream);
```

### Merge Multiple Presentations

```csharp
string[] files = { "part1.pptx", "part2.pptx", "part3.pptx" };
Presentation merged = new Presentation();
merged.Slides.Clear();
// Match slide size from first file
merged.SlideSize = new Presentation(File.ReadAllBytes(files[0])).SlideSize;

foreach (string file in files) {
    Presentation part = new Presentation(File.ReadAllBytes(file));
    foreach (Slide slide in part.Slides) {
        merged.Slides.Add(slide);
    }
}

using FileStream outStream = new FileStream("final-merged.pptx", FileMode.Create);
merged.SaveDocument(outStream);
```

## Splitting Presentations

### Extract Slides into a New Presentation

```csharp
using DevExpress.Docs.Presentation;

Presentation original = new Presentation(File.ReadAllBytes("large-presentation.pptx"));

// Create a new presentation to hold extracted slides
Presentation part1 = new Presentation();
part1.Slides.Clear();
part1.SlideSize = original.SlideSize;  // Match slide dimensions

// Move slides 0-2 to the new presentation
// Note: removing from original shifts indices — iterate carefully
List<Slide> slidesToMove = new List<Slide>();
for (int i = 0; i < Math.Min(3, original.Slides.Count); i++) {
    slidesToMove.Add(original.Slides[i].Clone());
}

foreach (Slide s in slidesToMove) {
    part1.Slides.Add(s);
}

// Save both parts
using FileStream f1 = new FileStream("part1.pptx", FileMode.Create);
part1.SaveDocument(f1);

// Remove those slides from the original if desired
for (int i = Math.Min(3, original.Slides.Count) - 1; i >= 0; i--) {
    original.Slides.RemoveAt(i);
}

using FileStream f2 = new FileStream("remaining.pptx", FileMode.Create);
original.SaveDocument(f2);
```

## Printing

### Print with Default Printer

```csharp
using DevExpress.Docs.Presentation;
using DevExpress.Docs.Presentation.Printing;

using var presentation = new Presentation(File.ReadAllBytes("input.pptx"));
presentation.Print();
```

### Print with Options

```csharp
using DevExpress.Docs.Presentation;
using DevExpress.Docs.Presentation.Printing;

using var presentation = new Presentation(File.ReadAllBytes("input.pptx"));

PrintOptions options = new PrintOptions();
options.PrintHiddenSlides = true;
options.PrinterSettings.Copies = 2;
options.PrinterSettings.PageRange = "1-3";  // Print only slides 1 through 3

presentation.Print(options);
```

**Note:** Printing in non-Windows environments (macOS, Linux) requires the `libcups2` package (CUPS). Ensure it is installed separately. The `DevExpress.Pdf.SkiaRenderer` NuGet package is required for cross-platform print rendering.

## Configuration Options

| Option | Class | Description |
|--------|-------|-------------|
| `ConvertImagesToJpeg` | `PdfExportOptions` | Convert bitmaps to JPEG to reduce file size |
| `ImageQuality` | `PdfExportOptions` | Quality level for JPEG conversion (`Low`, `Medium`, `High`) |
| `RasterizeImages` | `PdfExportOptions` | Rasterize vector images |
| `RasterizationResolution` | `PdfExportOptions` | DPI for rasterization (default: 96) |
| `EncryptionOptions` | `PdfExportOptions` | Password and permission settings |
| `SignatureOptions` | `PdfExportOptions` | Digital signature settings |
| `Attachments` | `PdfExportOptions` | List of files to attach to the PDF |
| `DocumentOptions` | `PdfExportOptions` | PDF metadata (title, author, subject, keywords) |
| `PrintHiddenSlides` | `PrintOptions` | Whether to print hidden slides |
| `PrinterSettings.Copies` | `PrintOptions` | Number of copies |
| `PrinterSettings.PageRange` | `PrintOptions` | Page range string (e.g., `"1-3,5"`) |

## Troubleshooting

- **PDF export is blank or missing content**: Verify `DevExpress.Pdf.SkiaRenderer` is installed. Missing this package causes silent rendering failure.
- **Charts and SmartArt missing in PDF**: Known limitation — these elements are not exported. Use screenshots or pre-rendered images as a workaround.
- **Merged slide shapes are off-screen**: Target presentation slide size is different from source. Set `target.SlideSize` to match the source before merging.
- **Printing fails on Linux/macOS**: Install `libcups2` and ensure a CUPS printer is configured.
- **Wrong slide count after splitting with RemoveAt in loop**: When removing by index in a forward loop, index shifts cause elements to be skipped. Iterate backwards or collect slides to remove first.

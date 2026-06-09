# Document Manipulation — DevExpress PDF Document API

Document manipulation covers operations on existing PDF files: merging multiple PDFs, organizing pages (add, insert, copy, delete, rotate, resize), converting to PDF/A, and optimizing file size.

## When to Use This Reference

Use this when you need to:
- Merge two or more PDF files into a single document
- Extract specific pages from a PDF into a new file
- Copy pages from one document to another
- Delete or reorder pages
- Rotate or resize pages
- Scale, rotate, or offset page content
- Convert an existing PDF to PDF/A-2b or PDF/A-3b
- Reduce file size using object streams

## Key Classes and Types

| Class/Method | Purpose |
|-------------|---------|
| `PdfDocumentProcessor` | Main processor — load, save, and operate on documents |
| `PdfDocumentProcessor.AppendDocument()` | Append another PDF's pages (with bookmarks, forms, attachments) |
| `PdfDocumentProcessor.AddNewPage()` | Append a blank page |
| `PdfDocumentProcessor.InsertNewPage()` | Insert a blank page at a position |
| `PdfDocumentProcessor.DeletePage()` | Delete a page by 1-based number |
| `PdfDocument.Pages` | `IList<PdfPage>` — access and manipulate the page collection |
| `PdfPage.Rotate` | Page rotation angle (0, 90, 180, 270) |
| `PdfPage.Resize()` | Resize a page while preserving content aspect ratio |
| `PdfPage.ScaleContent()` | Scale page content by X/Y factors |
| `PdfPage.RotateContent()` | Rotate page content around a point |
| `PdfPage.OffsetContent()` | Shift page content without changing page size |
| `PdfPageFacade.ClearContent()` | Remove content from page areas |
| `PdfSaveOptions.UseObjectStreams` | Enable object streams for smaller file size |

## Merging PDFs

`AppendDocument` copies all pages and additional content (bookmarks, forms, hyperlinks, attachments) from the source document into the current empty document.

```csharp
using DevExpress.Pdf;

using (PdfDocumentProcessor processor = new PdfDocumentProcessor())
{
    // CreateEmptyDocument writes to disk immediately — no explicit Save needed for merge
    processor.CreateEmptyDocument("merged.pdf");
    processor.AppendDocument("document1.pdf");
    processor.AppendDocument("document2.pdf");
    processor.AppendDocument("document3.pdf");
    // Disposing the processor finalizes and closes the file
}
```

### Merge with Reduced File Size

```csharp
using (PdfDocumentProcessor processor = new PdfDocumentProcessor())
{
    var saveOptions = new PdfSaveOptions { UseObjectStreams = true };
    processor.CreateEmptyDocument("merged_optimized.pdf", saveOptions);
    processor.AppendDocument("file1.pdf");
    processor.AppendDocument("file2.pdf");
}
```

## Page Operations

### Add and Insert Pages

```csharp
using (PdfDocumentProcessor processor = new PdfDocumentProcessor())
{
    processor.LoadDocument("input.pdf");

    // Append a blank A4 page at the end
    processor.AddNewPage(new PdfRectangle(0, 0, 595, 842));  // A4 in points

    // Insert a blank Letter page before page 2 (1-based index)
    processor.InsertNewPage(2, new PdfRectangle(0, 0, 612, 792));

    processor.SaveDocument("output.pdf");
}
```

### Delete a Page

```csharp
using (PdfDocumentProcessor processor = new PdfDocumentProcessor())
{
    processor.LoadDocument("input.pdf");
    processor.DeletePage(3);  // Delete page 3 (1-based)
    processor.SaveDocument("output.pdf");
}
```

### Rotate Pages

```csharp
using (PdfDocumentProcessor processor = new PdfDocumentProcessor())
{
    processor.LoadDocument("input.pdf");

    // Rotate all pages 90 degrees
    foreach (PdfPage page in processor.Document.Pages)
        page.Rotate = 90;

    // Or rotate a specific page by its index (0-based)
    processor.Document.Pages[0].Rotate = 180;

    processor.SaveDocument("rotated.pdf");
}
```

### Copy Pages Between Documents

```csharp
using (PdfDocumentProcessor source = new PdfDocumentProcessor())
using (PdfDocumentProcessor target = new PdfDocumentProcessor())
{
    source.LoadDocument("source.pdf");
    target.LoadDocument("target.pdf");

    // Copy the first page of source into target at position 1 (0-based)
    PdfPage pageToCopy = source.Document.Pages[0];
    target.Document.Pages.Insert(0, pageToCopy);

    target.SaveDocument("target_with_copy.pdf");
}
```

### Extract Specific Pages to a New Document

```csharp
using (PdfDocumentProcessor result = new PdfDocumentProcessor())
using (PdfDocumentProcessor source = new PdfDocumentProcessor())
{
    source.LoadDocument("source.pdf");
    result.CreateEmptyDocument("extracted.pdf");

    // Extract pages 2 and 4 (0-based index: 1 and 3)
    result.Document.Pages.Add(source.Document.Pages[1]);
    result.Document.Pages.Add(source.Document.Pages[3]);
}
```

## Page Content Transformations

### Resize a Page

Resize scales the page dimensions; content retains its aspect ratio and is aligned within the new size.

```csharp
using (PdfDocumentProcessor processor = new PdfDocumentProcessor())
{
    processor.LoadDocument("input.pdf");
    PdfPage page = processor.Document.Pages[0];

    // Resize to A4 (595 x 842 points), centered
    page.Resize(new PdfRectangle(0, 0, 595, 842),
        PdfContentHorizontalAlignment.Center,
        PdfContentVerticalAlignment.Center);

    processor.SaveDocument("resized.pdf");
}
```

### Scale Page Content

```csharp
PdfPage page = processor.Document.Pages[0];
// Scale to 80% horizontally and vertically
page.ScaleContent(0.8, 0.8);
// Flip horizontally
page.ScaleContent(-1.0, 1.0);
```

### Rotate Page Content

```csharp
PdfPage page = processor.Document.Pages[0];
// Rotate content 45 degrees around the point (306, 396) — page center for Letter
page.RotateContent(45, 306, 396);
```

### Offset Page Content

```csharp
PdfPage page = processor.Document.Pages[0];
// Shift content 50 points right and 100 points up
page.OffsetContent(50, 100);
```

## PDF/A Conversion

Convert an existing PDF to PDF/A-2b or PDF/A-3b:

```csharp
using (PdfDocumentProcessor processor = new PdfDocumentProcessor())
{
    processor.LoadDocument("input.pdf");

    processor.SaveDocument("archive.pdf", new PdfSaveOptions
    {
        // Use PdfCompatibility via CreationOptions on conversion — see note below
    });
}
```

> **Note**: For PDF/A conversion of existing documents, use `PdfDocumentProcessor.CreateEmptyDocument` with `PdfCreationOptions.Compatibility` set to the target level, then `AppendDocument` to pull in the existing content. Alternatively, check the DevExpress documentation for the `ConvertToPdfA` method if available in your version.

## File Size Optimization

Enable object streams to compress cross-reference data and reduce file size, especially effective for merged documents:

```csharp
processor.SaveDocument("optimized.pdf", new PdfSaveOptions
{
    UseObjectStreams = true
});
```

## Troubleshooting

- **`AppendDocument` loses bookmarks**: This should not happen — `AppendDocument` copies all additional content. If bookmarks are missing, verify the source document has bookmarks using a PDF viewer.
- **`DeletePage` throws on single-page document**: A PDF must have at least one page. Check `processor.Document.Pages.Count > 1` before deleting.
- **Page rotation not visible in some viewers**: Some viewers ignore the `Rotate` property during display but respect it for printing. Test with Adobe Reader for authoritative behavior.
- **Content disappears after `OffsetContent`**: Content shifted outside the crop box becomes invisible. Adjust crop box or use smaller offsets.

# Annotations — DevExpress PDF Document API

PDF annotations allow you to mark up document content (highlights, underlines, sticky notes, stamps) or redact sensitive content. The PDF Document API supports adding, editing, flattening, and removing annotations using the `PdfDocumentFacade` and page-level facade classes.

## When to Use This Reference

Use this when you need to:
- Add text markup annotations (highlight, underline, strikeout, squiggly)
- Add sticky note (text) annotations
- Add redaction annotations and apply (permanently remove) content
- Edit annotation properties (author, color, subject, contents)
- Add comments or review status to annotations
- Flatten annotations (bake them into static page content)
- Remove annotations from pages

## Key Classes and Types

| Class | Purpose |
|-------|---------|
| `PdfDocumentFacade` | High-level document accessor — obtained via `processor.DocumentFacade` |
| `PdfPageFacade` | Per-page access — obtained via `facade.Pages[index]` |
| `PdfPageFacade.Annotations` | Collection of all annotation facades on the page |
| `PdfAnnotationFacade` | Base class for all annotation facades |
| `PdfMarkupAnnotationFacade` | Base for markup and sticky note annotations |
| `PdfLinkAnnotationFacade` | Hyperlink annotation |
| `PdfRedactAnnotationFacade` | Redaction annotation (mark for removal) |
| `PdfTextMarkupAnnotationData` | Data returned by `AddTextMarkupAnnotation` |
| `PdfTextAnnotationData` | Data returned by `AddTextAnnotation` (sticky note) |
| `PdfTextMarkupAnnotationType` | Enum: `Highlight`, `Underline`, `StrikeOut`, `Squiggly` |
| `PdfRGBColor` | Color in RGB (values 0.0–1.0) |

## Add Markup Annotations (via PdfDocumentProcessor)

Use `PdfDocumentProcessor.AddTextMarkupAnnotation` to add highlight, underline, strikeout, or squiggly line annotations, and `AddTextAnnotation` for sticky notes.

```csharp
using DevExpress.Pdf;

using (PdfDocumentProcessor processor = new PdfDocumentProcessor())
{
    processor.LoadDocument("document.pdf");

    // Add a highlight annotation on page 1 (1-based)
    // Bounds in page coordinate system (origin bottom-left, Y up, units = points)
    PdfTextMarkupAnnotationData highlight = processor.AddTextMarkupAnnotation(
        pageNumber: 1,
        rect: new PdfRectangle(90, 100, 400, 120),
        annotationType: PdfTextMarkupAnnotationType.Highlight);

    if (highlight != null)
    {
        highlight.Author = "Jane Reviewer";
        highlight.Contents = "This section needs attention.";
        highlight.Color = new PdfRGBColor(1.0, 0.9, 0.0);  // Yellow
    }

    // Add an underline annotation
    PdfTextMarkupAnnotationData underline = processor.AddTextMarkupAnnotation(
        1,
        new PdfRectangle(90, 140, 350, 155),
        PdfTextMarkupAnnotationType.Underline);

    if (underline != null)
        underline.Color = new PdfRGBColor(0.0, 0.0, 1.0); // Blue

    // Add a sticky note annotation
    PdfTextAnnotationData stickyNote = processor.AddTextAnnotation(
        pageNumber: 1,
        point: new PdfPoint(50, 300));

    stickyNote.Author = "John Editor";
    stickyNote.Contents = "Please verify this figure.";
    stickyNote.Color = new PdfRGBColor(1.0, 1.0, 0.0);
    stickyNote.IconName = PdfTextAnnotationIconName.Note;

    processor.SaveDocument("annotated.pdf");
}
```

## Read and Edit Existing Annotations

Use `PdfDocumentFacade.Pages` to access page annotation facades and modify their properties:

```csharp
using (PdfDocumentProcessor processor = new PdfDocumentProcessor())
{
    processor.LoadDocument("annotated.pdf");
    PdfDocumentFacade facade = processor.DocumentFacade;

    // Access the first page
    PdfPageFacade page = facade.Pages[0];

    foreach (PdfAnnotationFacade annotation in page.Annotations)
    {
        // Cast to markup annotation to access extended properties
        if (annotation is PdfMarkupAnnotationFacade markup)
        {
            Console.WriteLine($"Type: {markup.GetType().Name}");
            Console.WriteLine($"Author: {markup.Author}");
            Console.WriteLine($"Contents: {markup.Contents}");

            // Modify the annotation
            markup.Contents = "[Reviewed] " + markup.Contents;
        }
    }

    processor.SaveDocument("edited_annotations.pdf");
}
```

## Add Comments to Annotations

```csharp
PdfMarkupAnnotationFacade markup = page.Annotations
    .OfType<PdfMarkupAnnotationFacade>()
    .First();

// Add a reply comment
PdfMarkupAnnotationComment comment = markup.AddReply("Alice", "I agree with this comment.");

// Add a nested reply
comment.AddReply("Bob", "Thanks for the confirmation.");

// Add a review status
markup.AddReview("Carol", PdfAnnotationReviewState.Accepted);
```

## Flatten Annotations

Flattening converts annotation content into static page content (non-interactive, cannot be deleted or edited).

```csharp
using (PdfDocumentProcessor processor = new PdfDocumentProcessor())
{
    processor.LoadDocument("annotated.pdf");
    PdfDocumentFacade facade = processor.DocumentFacade;
    PdfPageFacade page = facade.Pages[0];

    // Flatten all annotations on the page
    page.FlattenAnnotations();

    // Or flatten a specific annotation:
    // page.Annotations[0].Flatten();

    processor.SaveDocument("flattened.pdf");
}
```

## Remove Annotations

```csharp
using (PdfDocumentProcessor processor = new PdfDocumentProcessor())
{
    processor.LoadDocument("annotated.pdf");
    PdfDocumentFacade facade = processor.DocumentFacade;
    PdfPageFacade page = facade.Pages[0];

    // Remove a specific annotation by index
    PdfAnnotationFacade toRemove = page.Annotations[0];
    toRemove.Remove();

    // Remove all annotations on the page
    // foreach (var ann in page.Annotations.ToList()) ann.Remove();

    processor.SaveDocument("no_annotations.pdf");
}
```

## Redaction Annotations

Redaction annotations mark areas for permanent content removal. Apply them to remove sensitive information.

```csharp
using (PdfDocumentProcessor processor = new PdfDocumentProcessor())
{
    processor.LoadDocument("sensitive.pdf");
    PdfDocumentFacade facade = processor.DocumentFacade;
    PdfPageFacade page = facade.Pages[0];

    // Add a redaction annotation over a sensitive area
    // (Use page coordinates: origin at bottom-left, Y up)
    PdfRedactAnnotationFacade redact = page.AddRedactAnnotation(
        new PdfRectangle(100, 700, 400, 720));

    redact.FillColor = new PdfRGBColor(0, 0, 0);     // Black fill after redaction
    redact.BorderColor = new PdfRGBColor(1, 0, 0);   // Red border before applying

    // Apply the redaction — permanently removes content under the annotation
    // and burns in the fill color
    facade.ApplyRedactAnnotations();

    processor.SaveDocument("redacted.pdf");
}
```

> **Warning**: `ApplyRedactAnnotations` permanently removes content from the document. This operation cannot be undone. Always work on a copy of the original document.

## Annotation Types Summary

| Annotation Type | Class | How to Add |
|----------------|-------|-----------|
| Highlight | `PdfMarkupAnnotationFacade` | `processor.AddTextMarkupAnnotation(..., Highlight)` |
| Underline | `PdfMarkupAnnotationFacade` | `processor.AddTextMarkupAnnotation(..., Underline)` |
| Strikeout | `PdfMarkupAnnotationFacade` | `processor.AddTextMarkupAnnotation(..., StrikeOut)` |
| Squiggly | `PdfMarkupAnnotationFacade` | `processor.AddTextMarkupAnnotation(..., Squiggly)` |
| Sticky Note | `PdfMarkupAnnotationFacade` | `processor.AddTextAnnotation(pageNum, point)` |
| Link | `PdfLinkAnnotationFacade` | Via `PdfGraphics.AddLinkToUri` or `AddLinkToPage` |
| Redaction | `PdfRedactAnnotationFacade` | `page.AddRedactAnnotation(rect)` |

## Troubleshooting

- **`AddTextMarkupAnnotation` returns `null`**: The specified rectangle may not overlap any text content on the page. Check the bounds are in page coordinates and overlap text.
- **Annotations not visible in viewer**: Some viewers require the PDF to be saved and reopened. Verify the annotation is saved by calling `SaveDocument` before opening.
- **Flattening redaction annotations does not remove content**: Use `ApplyRedactAnnotations` (not `FlattenAnnotations`) to permanently apply redactions. Flatten only converts annotation visuals; it does not remove underlying content.
- **Coordinate confusion**: Annotation bounds use the page coordinate system (origin bottom-left). If the page is rotated, adjust accordingly. Compare your coordinates with the page's `CropBox` dimensions.
- **Cannot edit annotation after flattening**: Flattening is irreversible. Work on a copy before flattening.

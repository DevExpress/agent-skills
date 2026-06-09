# PDF Extensions — DevExpress AI-Powered Extensions

The PDF extensions enable AI-powered translation, summarization, and contextual Q&A on PDF files processed by `PdfDocumentProcessor`. Unlike the Word and Presentation extensions, PDF translation and summarization **return translated/summarized text as a `string`** rather than modifying the PDF in-place. The developer is responsible for rendering the returned text onto a new or existing page using the PDF Graphics API.

## When to Use This Reference

Use this when you need to:
- Translate an entire PDF document and receive the translation as a text string
- Translate a specific page region (defined by `PdfDocumentArea` coordinates) and receive the translated text
- Summarize the text content of a PDF document
- Ask a contextual question about a PDF document using RAG (retrieval-augmented generation)
- Understand how to define page regions with `PdfDocumentArea` and `PdfDocumentPosition`
- Render AI-generated text onto a PDF page using `PdfGraphics`

## Key Classes and Types

| Class/Interface | Namespace | Purpose |
|----------------|-----------|---------|
| `IAIDocProcessingService` | `DevExpress.AIIntegration.Docs` | Main service interface with all AI extension methods |
| `PdfDocumentProcessor` | `DevExpress.Pdf` | Loads, edits, and saves PDF documents |
| `PdfDocumentArea` | `DevExpress.Pdf` | Defines a rectangular region within a PDF page |
| `PdfDocumentPosition` | `DevExpress.Pdf` | Position within a PDF page (page number + point) |
| `PdfPage` | `DevExpress.Pdf` | Represents a single page in the PDF document |
| `PdfRectangle` | `DevExpress.Pdf` | Rectangle used to define page bounds |
| `PdfGraphics` | `DevExpress.Pdf` | Graphics context for drawing text/images onto PDF pages |
| `SummarizationMode` | `DevExpress.AIIntegration.Docs` | `Abstractive` or `Extractive` |
| `RagOptions` | `DevExpress.AIIntegration.Docs` | Fine-tunes RAG behavior for `AskAIAsync` |

## Required `using` Directives

```csharp
using DevExpress.AIIntegration;
using DevExpress.AIIntegration.Docs;
using DevExpress.Pdf;
using System.Globalization;
using System.Threading;
```

## Important Behavioral Differences from Word/Presentation

| Behavior | Word / Presentation | PDF |
|----------|--------------------|----|
| `TranslateAsync` modifies document in-place | Yes | **No** — returns `Task<string>` |
| `SummarizeAsync` return value | `Task<string>` | `Task<string>` |
| Works with images, annotations, comments | Yes | **No** — text content only |
| Field values processed | Yes | **No** |

## Common Scenarios

### Translate an Entire PDF Document

`TranslateAsync(PdfDocumentProcessor, CultureInfo)` extracts all text from the PDF and returns the translated content as a string.

```csharp
using var pdf = new PdfDocumentProcessor();
pdf.LoadDocument("Documents/input.pdf");

string translatedText = await docService.TranslateAsync(
    pdf,
    new CultureInfo("de-DE"));

Console.WriteLine(translatedText);
// Write the translated text to a file or render it onto a new PDF page
File.WriteAllText("Documents/translated_text.txt", translatedText);

pdf.SaveDocument("Documents/output.pdf");
```

### Translate a Specific Page Region (PdfDocumentArea)

Use `PdfDocumentArea.Create(PdfDocumentPosition, PdfDocumentPosition)` to define the region. Page numbers are **1-based**. Coordinates are in PDF world units (points, measured from bottom-left).

```csharp
using var pdf = new PdfDocumentProcessor();
pdf.LoadDocument("Documents/input.pdf");

// Get the bounding box of the first page (index 0 in the Pages collection = page 1)
var cropBox = pdf.Document.Pages[0].CropBox;

// Create positions: page number is 1-based
PdfDocumentPosition topLeft     = new PdfDocumentPosition(1, cropBox.TopLeft);
PdfDocumentPosition bottomRight = new PdfDocumentPosition(1, cropBox.BottomRight);

var pageArea = PdfDocumentArea.Create(topLeft, bottomRight);

// Translate the page area to Spanish — returns the translated text as a string
string translatedText = await docService.TranslateAsync(
    pdf,
    pageArea,
    new CultureInfo("es-ES"));

// Add a new page and render the translated text onto it
PdfPage newPage = pdf.InsertNewPage(1, PdfPaperSize.Letter);
RenderTextOnPage(pdf, newPage, newPage.CropBox, translatedText);

pdf.SaveDocument("Documents/output.pdf");
```

### Render Text onto a PDF Page

The following helper renders a text string onto a page using `PdfGraphics`. Call this after obtaining translated or summarized text.

```csharp
using DevExpress.Drawing;
using System.Drawing;

void RenderTextOnPage(PdfDocumentProcessor processor, PdfPage page, PdfRectangle pageSize, string text)
{
    using (PdfGraphics graphics = processor.CreateGraphicsWorldSystem())
    {
        using (var brush = new DXSolidBrush(Color.Black))
        {
            DXFont font = new DXFont("Segoe UI", 12, DXFontStyle.Regular);

            RectangleF rect = new RectangleF(
                10f,
                10f,
                (float)pageSize.Width - 20f,
                (float)pageSize.Height - 20f);

            graphics.DrawString(text, font, brush, rect);
            graphics.AddToPageForeground(page);
        }
    }
}
```

> **Note**: `PdfGraphics` coordinates are in PDF world units (points). The `AddToPageForeground` call composites the graphics onto the page.

### Summarize a PDF Document

`SummarizeAsync` returns the summary as a string. It does not modify the PDF.

```csharp
using var pdf = new PdfDocumentProcessor();
pdf.LoadDocument("Documents/input.pdf");

string summary = await docService.SummarizeAsync(
    pdf,
    SummarizationMode.Extractive,
    CancellationToken.None);

Console.WriteLine("Summary:");
Console.WriteLine(summary);

// Optionally insert the summary as a new first page
PdfPage summaryPage = pdf.InsertNewPage(1, PdfPaperSize.Letter);
RenderTextOnPage(pdf, summaryPage, summaryPage.CropBox, "Summary:\n" + summary);

pdf.SaveDocument("Documents/summarized.pdf");
```

**SummarizationMode options:**
- `SummarizationMode.Extractive` — selects key sentences from the original text verbatim
- `SummarizationMode.Abstractive` — rephrases content in a new concise form (AI generates new sentences)

### Ask a Contextual Question (RAG)

`AskAIAsync` uses retrieval-augmented generation to answer a natural language question about the PDF content. Returns a `string`. Requires an embedding generator and vector store registered in the AI container.

```csharp
using var pdf = new PdfDocumentProcessor();
using (var fs = File.OpenRead("Documents/input.pdf"))
{
    pdf.LoadDocument(fs, true);
}

string answer = await docService.AskAIAsync(
    pdf,
    "What terms does this document define?");

Console.WriteLine(answer);
```

**With RagOptions:**

```csharp
var ragOptions = new RagOptions
{
    ChunkSize = 1000,
    AugmentationChunkCount = 5,
    RebuildEmbeddings = true
};

string answer = await docService.AskAIAsync(
    pdf,
    "Summarize the liability section of this contract.",
    ragOptions);
```

## Working with PdfDocumentArea

A `PdfDocumentArea` requires two `PdfDocumentPosition` values (start and end of the region). Each `PdfDocumentPosition` takes:
- `pageNumber` (int, **1-based**)
- A point on that page (in PDF world coordinates, origin at bottom-left)

```csharp
// Target the top half of page 2
var page = pdf.Document.Pages[1];  // index 1 = page 2
var box = page.CropBox;

// Mid-height point
var midPoint = new PdfPoint(box.Left, box.Top - (box.Height / 2));

var area = PdfDocumentArea.Create(
    new PdfDocumentPosition(2, new PdfPoint(box.Left, box.Top)),   // top-left of page 2
    new PdfDocumentPosition(2, midPoint));                          // mid-left of page 2

string text = await docService.TranslateAsync(pdf, area, new CultureInfo("fr-FR"));
```

## Loading PDF Documents

```csharp
// Load from file path
using var pdf = new PdfDocumentProcessor();
pdf.LoadDocument("input.pdf");

// Load from stream (pass true to leave stream open)
using (var fs = File.OpenRead("input.pdf"))
{
    pdf.LoadDocument(fs, true);
    fs.Close();
}
```

## Saving PDF Documents

```csharp
// Save to file path
pdf.SaveDocument("output.pdf");

// Save with optimization options
var saveOptions = new PdfSaveOptions { UseObjectStreams = true };
pdf.SaveDocument("output_optimized.pdf");
```

## Troubleshooting

- **`TranslateAsync` returns an empty string**: The PDF may contain only images or scanned content (no extractable text). AI PDF extensions process text content only — images, annotations, comments, and field values are ignored.
- **Page numbering confusion**: `pdf.Document.Pages[0]` is the first page, but `PdfDocumentPosition` takes a **1-based** page number. `Pages[0]` corresponds to page number `1`.
- **Text is rendered off the page**: Check that your `RectangleF` coordinates are within the `CropBox` bounds. PDF world coordinates have the origin at the bottom-left of the page.
- **`AskAIAsync` returns vague or generic answers**: Ensure an embedding generator and in-memory (or persistent) vector store are registered in the AI container before calling `AskAIAsync`. RAG cannot function without them.
- **Large PDFs time out**: Process specific `PdfDocumentArea` regions rather than the whole document, or increase the `CancellationToken` timeout.

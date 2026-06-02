# Document Generation — DevExpress PDF Document API

The document generation API lets you create new PDF files from scratch by combining an empty document with a graphics drawing context. Pages are constructed by drawing content into a `PdfGraphics` object and then rendering that graphics as a page.

## When to Use This Reference

Use this when you need to:
- Create a new PDF file from scratch (no existing template)
- Add one or more pages with text, images, and shapes
- Control PDF/A compatibility mode (PDF/A-1b, PDF/A-2b, PDF/A-3b)
- Control font embedding behavior
- Generate documents with custom page sizes

## Key Classes and Types

| Class/Interface | Purpose |
|----------------|---------|
| `PdfDocumentProcessor` | Main entry point — creates documents, manages pages |
| `PdfGraphics` | Drawing context — text, images, shapes, links |
| `PdfCreationOptions` | Options for new documents: PDF/A mode, font embedding |
| `PdfCompatibility` | Enum: `Pdf` (default), `PdfA1b`, `PdfA2b`, `PdfA3b` |
| `PdfPaperSize` | Enum: `Letter`, `A4`, `A3`, `Legal`, etc. |
| `PdfRectangle` | Custom page size (in points, 1 pt = 1/72 inch) |
| `DXFont` | Cross-platform font (from `DevExpress.Drawing`) |
| `DXSolidBrush` | Solid color brush for drawing |

## Basic Usage

```csharp
using DevExpress.Drawing;
using DevExpress.Pdf;
using System.Drawing;

using (PdfDocumentProcessor processor = new PdfDocumentProcessor())
{
    // Create an empty document at the specified path
    processor.CreateEmptyDocument("Result.pdf");

    // Create graphics context (page coordinate system)
    using (PdfGraphics graph = processor.CreateGraphicsPageSystem())
    {
        // Draw a title
        using (DXFont font = new DXFont("Times New Roman", 32, DXFontStyle.Bold))
            graph.DrawString("PDF Document Processor", font,
                new DXSolidBrush(Color.Black), 180, 150);

        // Draw a subtitle
        using (DXFont font2 = new DXFont("Arial", 20))
            graph.DrawString("Display, Print and Export PDF Documents",
                font2, new DXSolidBrush(Color.Black), 168, 230);

        // Render the graphics as a Letter-size page
        processor.RenderNewPage(PdfPaperSize.Letter, graph);
    }
}
```

## Common Scenarios

### Multi-Page Document

Each call to `RenderNewPage` produces one page. Create a new `PdfGraphics` for each page.

```csharp
using (PdfDocumentProcessor processor = new PdfDocumentProcessor())
{
    processor.CreateEmptyDocument("MultiPage.pdf");

    for (int i = 1; i <= 3; i++)
    {
        using (PdfGraphics graph = processor.CreateGraphicsPageSystem())
        {
            using (DXFont font = new DXFont("Arial", 16))
                graph.DrawString($"Page {i} of 3", font,
                    new DXSolidBrush(Color.Black), 50, 50);

            processor.RenderNewPage(PdfPaperSize.A4, graph);
        }
    }
}
```

### Custom Page Size

Use `PdfRectangle` to specify dimensions in points (1 point = 1/72 inch).

```csharp
// A custom 4x6 inch page (288 x 432 points)
PdfRectangle customSize = new PdfRectangle(0, 0, 288, 432);

using (PdfDocumentProcessor processor = new PdfDocumentProcessor())
{
    processor.CreateEmptyDocument("CustomSize.pdf");
    using (PdfGraphics graph = processor.CreateGraphicsPageSystem())
    {
        using (DXFont font = new DXFont("Arial", 14))
            graph.DrawString("4x6 inch page", font,
                new DXSolidBrush(Color.Black), 20, 20);
        processor.RenderNewPage(customSize, graph);
    }
}
```

### Save to Stream (for ASP.NET Core / Blazor)

```csharp
using System.IO;

using (PdfDocumentProcessor processor = new PdfDocumentProcessor())
{
    using MemoryStream stream = new MemoryStream();
    processor.CreateEmptyDocument(stream);

    using (PdfGraphics graph = processor.CreateGraphicsPageSystem())
    {
        using (DXFont font = new DXFont("Arial", 12))
            graph.DrawString("Generated PDF", font,
                new DXSolidBrush(Color.Black), 50, 50);
        processor.RenderNewPage(PdfPaperSize.A4, graph);
    }

    // stream now contains the PDF bytes
    byte[] pdfBytes = stream.ToArray();
}
```

### PDF/A Compliance

```csharp
using (PdfDocumentProcessor processor = new PdfDocumentProcessor())
{
    processor.CreateEmptyDocument("Archive.pdf", new PdfCreationOptions
    {
        Compatibility = PdfCompatibility.PdfA2b
    });

    using (PdfGraphics graph = processor.CreateGraphicsPageSystem())
    {
        using (DXFont font = new DXFont("Arial", 12))
            graph.DrawString("Archival-quality document", font,
                new DXSolidBrush(Color.Black), 50, 50);
        processor.RenderNewPage(PdfPaperSize.A4, graph);
    }
}
```

| PDF/A Level | Standard | Transparency | Attachments | Encryption |
|-------------|----------|-------------|-------------|-----------|
| `PdfA1b` | ISO 19005-1 | Not allowed | Not allowed | Not allowed |
| `PdfA2b` | ISO 19005-2 | Allowed | Not allowed | Not allowed |
| `PdfA3b` | ISO 19005-3 | Allowed | Allowed | Not allowed |

### Control Font Embedding

By default, all fonts are embedded. To exclude specific families:

```csharp
var options = new PdfCreationOptions();
options.NotEmbeddedFontFamilies.Add("Arial");
options.NotEmbeddedFontFamilies.Add("Times New Roman");
// or disable all embedding:
// options.DisableEmbeddingAllFonts = true;

processor.CreateEmptyDocument("NoEmbeddedFonts.pdf", options);
```

### World Coordinate System

Use `CreateGraphicsWorldSystem()` when you prefer the top-left origin with Y increasing downward (like GDI+/screen coordinates). Default DPI is 96.

```csharp
using (PdfGraphics graph = processor.CreateGraphicsWorldSystem())
{
    // (0,0) is top-left. Y increases downward. 96 px = 1 inch.
    using (DXFont font = new DXFont("Arial", 12))
        graph.DrawString("World coordinate text", font,
            new DXSolidBrush(Color.Black), 50, 50);
    processor.RenderNewPage(PdfPaperSize.Letter, graph);
}
```

## Configuration Options

| Option | Default | Description |
|--------|---------|-------------|
| `PdfCreationOptions.Compatibility` | `PdfCompatibility.Pdf` | PDF/A conformance level |
| `PdfCreationOptions.DisableEmbeddingAllFonts` | `false` | Skip font embedding entirely |
| `PdfCreationOptions.NotEmbeddedFontFamilies` | empty | Font families to exclude from embedding |

## Troubleshooting

- **Empty PDF saved but no pages**: You must call `RenderNewPage` for each page. Calling `CreateEmptyDocument` alone produces a 0-page document.
- **Graphics appear mirrored/upside-down**: You are likely mixing coordinate systems. In page system (Y up), positive Y moves content toward the top of the visible page, which means larger Y = higher on page. In world system (Y down), larger Y = lower on page.
- **ICU error on Linux**: Set `export DXEXPORT_ICU_VERSION_OVERRIDE=65.1` (or the version matching your system's ICU install) before running the application.
- **PDF/A validation fails**: Ensure you are not using transparency (gradients, alpha) in PDF/A-1b, and all fonts are embedded. Use a PDF/A validator tool to identify violations.

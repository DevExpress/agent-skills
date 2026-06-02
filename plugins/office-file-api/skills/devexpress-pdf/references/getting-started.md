# Getting Started with DevExpress PDF Document API (.NET 6+)

This guide walks you through setting up and using the PDF Document API for the first time in a .NET 6/7/8+ application.

## When to Use This Reference

Use this when you need to:
- Set up the PDF Document API in a new or existing .NET 6+ project
- Install and configure required NuGet packages
- Create your first PDF document from scratch
- Load and modify an existing PDF file
- Understand which packages are needed for rendering and export

## System Requirements

- .NET 6.0 / 7.0 / 8.0+
- Visual Studio 2022+ (recommended) or JetBrains Rider 2022+
- DevExpress NuGet feed configured: `https://nuget.devexpress.com/api`
- A valid DevExpress license

## Installation

### Step 1: Add the DevExpress NuGet Feed

In Visual Studio: **Tools > NuGet Package Manager > Package Sources** — add `https://nuget.devexpress.com/api` with your DevExpress credentials.

Or create/edit `nuget.config` in your solution root:

```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="DevExpress" value="https://nuget.devexpress.com/api" />
  </packageSources>
</configuration>
```

### Step 2: Install NuGet Packages

**.NET CLI:**
```bash
dotnet add package DevExpress.Document.Processor
dotnet add package DevExpress.Pdf.SkiaRenderer
```

**Package Manager Console:**
```
Install-Package DevExpress.Document.Processor
Install-Package DevExpress.Pdf.SkiaRenderer
```

`DevExpress.Pdf.SkiaRenderer` provides cross-platform PDF rendering (used internally for image export and cross-platform printing). It is required on .NET 6+ for most rendering operations.

### Step 3: Add Namespace Imports

```csharp
using DevExpress.Pdf;        // PdfDocumentProcessor, PdfPage, PdfGraphics, etc.
using DevExpress.Drawing;    // DXFont, DXSolidBrush, DXBrushes, DXFontStyle
using System.Drawing;        // Color, RectangleF, PointF
```

## Create a New PDF Document

```csharp
using DevExpress.Drawing;
using DevExpress.Pdf;
using System.Drawing;

using (PdfDocumentProcessor processor = new PdfDocumentProcessor())
{
    // Create an empty PDF document
    processor.CreateEmptyDocument("MyDocument.pdf");

    // Create a graphics context (page coordinate system: origin at bottom-left)
    using (PdfGraphics graph = processor.CreateGraphicsPageSystem())
    {
        // Draw a title string
        using (DXFont titleFont = new DXFont("Arial", 24, DXFontStyle.Bold))
            graph.DrawString("Hello, DevExpress PDF!", titleFont,
                new DXSolidBrush(Color.Black), 50, 50);

        // Draw a subtitle
        using (DXFont bodyFont = new DXFont("Arial", 12))
            graph.DrawString("Created with the DevExpress PDF Document API.",
                bodyFont, new DXSolidBrush(Color.DarkGray), 50, 110);

        // Draw a rectangle
        graph.DrawRectangle(new DXPen(Color.Navy, 2), new RectangleF(40, 40, 520, 200));

        // Render as a Letter-size page and append to the document
        processor.RenderNewPage(PdfPaperSize.Letter, graph);
    }
}
// File is saved and closed when the processor is disposed
```

## Open and Modify an Existing PDF

```csharp
using DevExpress.Pdf;

using (PdfDocumentProcessor processor = new PdfDocumentProcessor())
{
    // Load an existing PDF
    processor.LoadDocument("input.pdf");

    // Rotate all pages by 90 degrees (cumulative)
    int angle = 0;
    foreach (PdfPage page in processor.Document.Pages)
    {
        angle = (angle + 90) % 360;
        page.Rotate = angle;
    }

    // Save the result
    processor.SaveDocument("rotated.pdf");
}
```

## Load a Password-Protected PDF

```csharp
using (PdfDocumentProcessor processor = new PdfDocumentProcessor())
{
    processor.LoadDocument("protected.pdf", "userPassword");
    processor.SaveDocument("unprotected_copy.pdf");
}
```

## Create a PDF/A-Compliant Document

```csharp
using (PdfDocumentProcessor processor = new PdfDocumentProcessor())
{
    var options = new PdfCreationOptions
    {
        Compatibility = PdfCompatibility.PdfA2b  // or PdfA1b, PdfA3b
    };
    processor.CreateEmptyDocument("archive.pdf", options);

    using (PdfGraphics graph = processor.CreateGraphicsPageSystem())
    {
        using (DXFont font = new DXFont("Arial", 12))
            graph.DrawString("PDF/A-2b compliant content", font,
                new DXSolidBrush(System.Drawing.Color.Black), 50, 50);
        processor.RenderNewPage(PdfPaperSize.A4, graph);
    }
}
```

**PDF/A limitations:**
- All fonts must be embedded (default behavior)
- PDF/A-1b does not allow transparency
- PDF/A-1b and PDF/A-2b do not support file attachments
- Encryption is not permitted in PDF/A documents

## Coordinate Systems

The PDF Document API uses three coordinate systems:

| System | Origin | Y direction | DPI | Used by |
|--------|--------|-------------|-----|---------|
| **World** | Top-left | Downward | 96 (default) | `CreateGraphicsWorldSystem()` |
| **Page** | Bottom-left of crop box | Upward | 72 | `CreateGraphicsPageSystem()` |
| **User** | Flexible (PDF internal) | Upward | 72 | Low-level document model API |

For most new-document scenarios, use `CreateGraphicsPageSystem()` with the page coordinate system. When adding overlays to existing pages, use `CreateGraphicsPageSystem()` and call `AddToPageForeground()` or `AddToPageBackground()`.

## What to Learn Next

- [references/document-generation.md](document-generation.md): Font embedding, PDF/A, graphics-based document creation patterns
- [references/pdf-graphics.md](pdf-graphics.md): Complete graphics API — text, images, shapes, links, transforms
- [references/document-manipulation.md](document-manipulation.md): Merging, page operations, conversion, optimization
- [references/document-security.md](document-security.md): Password protection and digital signatures

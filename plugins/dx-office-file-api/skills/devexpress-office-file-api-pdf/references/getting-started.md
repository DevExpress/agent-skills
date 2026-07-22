# Getting Started with DevExpress PDF Document API (.NET 8+)

This guide walks you through setting up and using the PDF Document API for the first time in a .NET 8/9/10+ application.

## When to Use This Reference

Use this when you need to:
- Set up the PDF Document API in a new or existing .NET 8+ project
- Install and configure required NuGet packages
- Create your first PDF document from scratch
- Load and modify an existing PDF file
- Understand which packages are needed for rendering and export

## System Requirements

- .NET 8.0 / 9.0 / 10.0+
- Visual Studio 2022+ (recommended) or JetBrains Rider 2022+
- A valid DevExpress license

## Installation

### Step 1: Install NuGet Packages

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

`DevExpress.Pdf.SkiaRenderer` provides cross-platform PDF rendering (used internally for image export and cross-platform printing). It is required on .NET 8+ for most rendering operations.

## Non-Windows Platform Support (Linux, macOS, Docker, Cloud)

The library uses a platform-specific drawing engine: GDI+ on Windows, SkiaSharp elsewhere. **The SkiaSharp-based engine is enabled automatically on non-Windows platforms.** Enable `Settings.DrawingEngine` at app startup only to force Skia *on Windows* (e.g., to work around the 10K GDI-handle limit).

Beyond `DevExpress.Pdf.SkiaRenderer` (Step 1), also add the Skia-based drawing engine package:

```bash
dotnet add package DevExpress.Drawing.Skia
```

### Non-Windows Troubleshooting

| Issue | Cause | Solution |
|-------|-------|----------|
| `System.DllNotFoundException` referencing `DevExpress.Drawing.*.Skia`, `DevExpress.Pdf.*.Skia`, or a SkiaSharp/HarfBuzz assembly | `DevExpress.Drawing.Skia` or `DevExpress.Pdf.SkiaRenderer` package missing, or (if referencing DevExpress assemblies directly instead of via NuGet) the SkiaSharp native asset packages for your OS aren't referenced | Add the `DevExpress.Drawing.Skia` and `DevExpress.Pdf.SkiaRenderer` NuGet packages — normal NuGet restores handle native assets automatically. If the exception persists, explicitly add `SkiaSharp`, `SkiaSharp.HarfBuzz`, and the native asset package matching your target platform: `SkiaSharp.NativeAssets.Linux` (also add `HarfBuzzSharp.NativeAssets.Linux` on Linux), `SkiaSharp.NativeAssets.macOS`, or `SkiaSharp.NativeAssets.WebAssembly`. See the [DevExpress.Drawing troubleshooting guide](https://docs.devexpress.com/CoreLibraries/404254/devexpress-drawing-library/troubleshooting). |
| `System.TypeInitializationException` on Linux/Docker | Missing native libraries | Install required libraries: `apt-get install -y libc6 libicu-dev libfontconfig1` (Debian/Ubuntu) or `yum install -y glibc-devel libicu fontconfig` (RHEL/CentOS). On .NET 8+, the `Microsoft.ICU.ICU4C.Runtime` package can supply ICU instead (not available for .NET Framework). |
| Fonts missing or rendered incorrectly on non-Windows | System fonts unavailable | Register fonts explicitly at runtime via [DXFontRepository](https://docs.devexpress.com/CoreLibraries/404255/devexpress-drawing-library/use-font-repository-to-add-custom-fonts): `DXFontRepository.Instance.AddFont(...)`. |
| Printing on Linux/macOS | GDI/XPS printing pipeline is Windows-only | Use `PdfPrinterSettings.DXSettings` with [CUPS](https://en.wikipedia.org/wiki/Common_UNIX_Printing_System) (install `libcups2` separately) — see [Printing in PDF Document API](https://docs.devexpress.com/OfficeFileAPI/404300). |

Platform-specific guides:
- macOS: https://docs.devexpress.com/OfficeFileAPI/401532
- Linux: https://docs.devexpress.com/OfficeFileAPI/401441
- Docker: https://docs.devexpress.com/OfficeFileAPI/401528

### Step 2: Add Namespace Imports

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

# Getting Started with DevExpress PDF Document API (.NET Framework)

This guide covers setting up the PDF Document API in .NET Framework 4.6.2+ projects. Key differences from .NET 6+ include the rendering engine (GDI+ instead of Skia) and assembly reference requirements.

## When to Use This Reference

Use this when you need to:
- Set up the PDF Document API in a .NET Framework 4.6.2+ project
- Understand which assemblies to reference manually (installer path) vs. via NuGet
- Know .NET Framework-specific rendering behavior (GDI+ engine)
- Understand WinForms or WPF integration on .NET Framework

## System Requirements

- .NET Framework 4.6.2 or higher
- Visual Studio 2019 or 2022
- DevExpress Unified Component Installer (optional — provides assemblies directly) OR DevExpress NuGet feed
- A valid DevExpress license

## Installation

### Option A: NuGet Package (Recommended)

```
Install-Package DevExpress.Document.Processor
```

**Do not** install `DevExpress.Pdf.SkiaRenderer` on .NET Framework — it is not required. GDI+ is used automatically for rendering.

### Option B: Manual Assembly References

If you have the DevExpress Unified Component Installer, add references to:

| Assembly | Purpose |
|----------|---------|
| `DevExpress.Data.v25.2.dll` | Base data layer |
| `DevExpress.Docs.v25.2.dll` | Document processing core |
| `DevExpress.Drawing.v25.2.dll` | Cross-platform drawing abstractions |
| `DevExpress.Office.v25.2.Core.dll` | Office file API core |
| `DevExpress.Pdf.v25.2.Core.dll` | PDF processing engine |
| `DevExpress.Pdf.v25.2.Drawing.dll` | PDF graphics and rendering |

Default install path: `%ProgramFiles%\DevExpress 25.2\Components\Bin\Framework\`

## Code Example — Create a New PDF

The API surface is identical to .NET 6+. The difference is in rendering internals.

```csharp
using DevExpress.Drawing;
using DevExpress.Pdf;
using System.Drawing;

static void Main(string[] args)
{
    using (PdfDocumentProcessor processor = new PdfDocumentProcessor())
    {
        processor.CreateEmptyDocument("Output.pdf");

        using (PdfGraphics graph = processor.CreateGraphicsPageSystem())
        {
            using (DXFont titleFont = new DXFont("Times New Roman", 28, DXFontStyle.Bold))
                graph.DrawString("Hello from .NET Framework!", titleFont,
                    (DXSolidBrush)DXBrushes.Black, 80, 100);

            using (DXFont bodyFont = new DXFont("Arial", 12))
                graph.DrawString("Generated with DevExpress PDF Document API on .NET Framework.",
                    bodyFont, (DXSolidBrush)DXBrushes.Black, 80, 160);

            processor.RenderNewPage(PdfPaperSize.Letter, graph);
        }
    }
}
```

## Code Example — Load and Save

```csharp
using DevExpress.Pdf;

using (PdfDocumentProcessor processor = new PdfDocumentProcessor())
{
    processor.LoadDocument(@"..\..\docs\Document.pdf");

    // Rotate each page by 90 degrees (cumulative)
    int angle = 0;
    foreach (PdfPage page in processor.Document.Pages)
    {
        angle = (angle + 90) % 360;
        page.Rotate = angle;
    }

    processor.SaveDocument(@"..\..\docs\Rotated.pdf");
}
```

## .NET Framework vs. .NET 6+ Differences

| Feature | .NET Framework | .NET 6+ |
|---------|---------------|---------|
| Rendering engine | GDI+ (Windows only) | Skia (cross-platform) |
| Required renderer package | None | `DevExpress.Pdf.SkiaRenderer` |
| Printing | GDI/XPS pipeline (Windows) | CUPS (Linux/macOS via libcups2) |
| Cross-platform support | Windows only | Windows, Linux, macOS |
| PDF Graphics API | Same (`PdfGraphics`) | Same (`PdfGraphics`) |
| `DXFont` / `DXSolidBrush` | `DevExpress.Drawing` assembly | Same |

## Printing on .NET Framework

On .NET Framework, printing uses the Windows printing infrastructure (GDI/XPS). Call `PdfDocumentProcessor.Print` with `PdfPrinterSettings`:

```csharp
using DevExpress.Pdf;
using System.Drawing.Printing;

using (PdfDocumentProcessor processor = new PdfDocumentProcessor())
{
    processor.LoadDocument("Document.pdf");

    PdfPrinterSettings settings = new PdfPrinterSettings();
    settings.Settings.PrinterName = "Microsoft Print to PDF";
    settings.PageNumbers = new int[] { 1, 2, 3 };
    settings.ScaleMode = PdfPrintScaleMode.CustomScale;
    settings.Scale = 90;

    processor.Print(settings);
}
```

## Troubleshooting

- **Missing `DevExpress.Drawing` types**: Add a reference to `DevExpress.Drawing.v25.2.dll` from the DevExpress install directory, or ensure `DevExpress.Document.Processor` NuGet package is installed (it pulls in all required dependencies).
- **GDI+ rendering issues in print**: Set `processor.RenderingEngine = PdfRenderingEngine.Gdi` to force GDI+ instead of DirectX. Note that GDI+ does not support text stroke rendering, transparency, or blend modes.
- **Font not embedded**: By default, fonts are embedded. To skip specific families, use `PdfCreationOptions.NotEmbeddedFontFamilies`.
- **Version mismatch**: All DevExpress assemblies and NuGet packages must use the exact same version number (e.g., all 25.2.x).

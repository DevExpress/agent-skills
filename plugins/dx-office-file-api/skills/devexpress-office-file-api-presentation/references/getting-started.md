# Getting Started with DevExpress Presentation API

This guide walks you through setting up the DevExpress Presentation API and creating your first PowerPoint presentation in code.

## When to Use This Reference

Use this when you need to:
- Install the Presentation API NuGet package for the first time
- Understand assembly references for .NET Framework projects
- Create a new .pptx presentation from scratch
- Load an existing presentation from a file, stream, or byte array
- Save a presentation to a file or stream
- See a complete working end-to-end example

## System Requirements

- .NET 8.0 / 9.0 / 10.0+ or .NET Framework 4.6.2+
- Visual Studio 2022+ (recommended) or JetBrains Rider
- A valid DevExpress license

## Installation

### Step 1: Install NuGet Packages

**.NET CLI:**
```bash
dotnet add package DevExpress.Docs.Presentation
```

**Package Manager Console:**
```
Install-Package DevExpress.Docs.Presentation
```

### Step 2: .NET Framework — Manual Assembly References (Alternative)

If you installed DevExpress via the Unified Component Installer and prefer assembly references instead of NuGet, add:

- `DevExpress.Docs.Presentation.vXX.X.dll`
- `DevExpress.Data.vXX.X.dll`
- `DevExpress.Drawing.vXX.X.dll`
- `DevExpress.Printing.vXX.X.Core.dll`
- `DevExpress.Office.vXX.X.Core.dll`
- `DevExpress.Docs.Core.vXX.X.dll`
- `DevExpress.Pdf.vXX.X.Core.dll`

### Step 3: Register Your License

```csharp
// For NuGet-only installations, follow the DevExpress license deployment guide.
// DevExpress license is validated automatically when installed via the DevExpress installer.
// See: https://docs.devexpress.com/GeneralInformation/116042
```

## Non-Windows Platform Support (Linux, macOS, Docker, Cloud)

The library uses a platform-specific drawing engine: GDI+ on Windows, SkiaSharp elsewhere. **The SkiaSharp-based engine is enabled automatically on non-Windows platforms.** Enable `Settings.DrawingEngine` at app startup only to force Skia *on Windows* (e.g., to work around the 10K GDI-handle limit).

Add the Skia-based drawing engine package:

```bash
dotnet add package DevExpress.Drawing.Skia
```

### Non-Windows Troubleshooting

| Issue | Cause | Solution |
|-------|-------|----------|
| `System.DllNotFoundException` referencing `DevExpress.Drawing.*.Skia` or a SkiaSharp/HarfBuzz assembly | `DevExpress.Drawing.Skia` package missing, or (if referencing DevExpress.Drawing assemblies directly instead of via NuGet) the SkiaSharp native asset packages for your OS aren't referenced | Add the `DevExpress.Drawing.Skia` NuGet package — normal NuGet restores handle native assets automatically. If the exception persists, explicitly add `SkiaSharp`, `SkiaSharp.HarfBuzz`, and the native asset package matching your target platform: `SkiaSharp.NativeAssets.Linux` (also add `HarfBuzzSharp.NativeAssets.Linux` on Linux), `SkiaSharp.NativeAssets.macOS`, or `SkiaSharp.NativeAssets.WebAssembly`. See the [DevExpress.Drawing troubleshooting guide](https://docs.devexpress.com/CoreLibraries/404254/devexpress-drawing-library/troubleshooting). |
| `System.TypeInitializationException` on Linux/Docker | Missing native libraries | Install required libraries: `apt-get install -y libc6 libicu-dev libfontconfig1` (Debian/Ubuntu) or `yum install -y glibc-devel libicu fontconfig` (RHEL/CentOS). On .NET 8+, the `Microsoft.ICU.ICU4C.Runtime` package can supply ICU instead (not available for .NET Framework). |

Platform-specific guides:
- macOS: https://docs.devexpress.com/OfficeFileAPI/401532
- Linux: https://docs.devexpress.com/OfficeFileAPI/401441
- Docker: https://docs.devexpress.com/OfficeFileAPI/401528

## Your First Presentation

### Create a New Presentation

A `Presentation` created with the default constructor starts with one empty slide using the `Title` layout.

```csharp
using DevExpress.Docs.Presentation;

// Create an empty presentation (one default slide)
Presentation presentation = new Presentation();
```

### Load an Existing Presentation

**From a file stream:**
```csharp
using DevExpress.Docs.Presentation;
using System.IO;

using FileStream fs = File.OpenRead(@"C:\Documents\MyPresentation.pptx");
Presentation presentation = new Presentation(fs);
```

**From a byte array:**
```csharp
using DevExpress.Docs.Presentation;
using System.IO;

Presentation presentation = new Presentation(File.ReadAllBytes(@"C:\Documents\MyPresentation.pptx"));
```

**Supported input formats:** PPTX, PPTM, POTX, POTM.

If the file is not a recognized format, a `PresentationUnsupportedFormatException` is thrown.

### Save a Presentation

```csharp
using System.IO;

// Save to a file
FileStream outputStream = new FileStream(@"C:\Documents\output.pptx", FileMode.Create);
presentation.SaveDocument(outputStream);
outputStream.Dispose();

// Save with explicit format
presentation.SaveDocument(outputStream, DocumentFormat.Pptx);
```

The default save format is PPTX. Use the `DocumentFormat` enum to specify a different format.

## Complete First Presentation Example

This example creates a presentation with a styled background, a title slide, and a content slide, then saves it as .pptx.

```csharp
using DevExpress.Docs.Office;
using DevExpress.Docs.Presentation;
using System.Drawing;
using System.IO;

// Step 1: Create the presentation
Presentation presentation = new Presentation();

// Step 2: Remove the default blank slide
presentation.Slides.Clear();

// Step 3: Configure the slide master background
SlideMaster master = presentation.SlideMasters[0];
master.Background = new CustomSlideBackground(new SolidFill(Color.FromArgb(240, 246, 255)));

// Step 4: Add a title slide using the Title layout
Slide titleSlide = new Slide(master.Layouts.Get(SlideLayoutType.Title));
foreach (Shape shape in titleSlide.Shapes) {
    if (shape.PlaceholderSettings.Type is PlaceholderType.CenteredTitle)
        shape.TextArea = new TextArea("Quarterly Report");
    if (shape.PlaceholderSettings.Type is PlaceholderType.Subtitle)
        shape.TextArea = new TextArea("Q4 2025 — DevExpress Presentation API");
}
presentation.Slides.Add(titleSlide);

// Step 5: Add a content slide using the Object layout
Slide contentSlide = new Slide(master.Layouts.GetOrCreate(SlideLayoutType.Object));
foreach (Shape shape in contentSlide.Shapes) {
    if (shape.PlaceholderSettings.Type is PlaceholderType.Title)
        shape.TextArea = new TextArea("Revenue Highlights");
    if (shape.PlaceholderSettings.Type is PlaceholderType.Body) {
        TextArea body = new TextArea();
        body.Paragraphs.Clear();
        body.Paragraphs.Add(new TextParagraph("Total revenue: $4.2M (+18% YoY)"));
        body.Paragraphs.Add(new TextParagraph("New customers: 340"));
        body.Paragraphs.Add(new TextParagraph("Churn rate: 2.1%"));
        shape.TextArea = body;
    }
}
presentation.Slides.Add(contentSlide);

// Step 6: Add footer with slide numbers to all slides
presentation.HeaderFooterManager.AddSlideNumberPlaceholder(presentation.Slides);
presentation.HeaderFooterManager.AddFooterPlaceholder(presentation.Slides, "Confidential");

// Step 7: Save as .pptx
using FileStream outStream = new FileStream("quarterly-report.pptx", FileMode.Create);
presentation.SaveDocument(outStream);

Console.WriteLine("Presentation saved: quarterly-report.pptx");
```

### What This Produces

A two-slide presentation:
- Slide 1: Title "Quarterly Report" with subtitle.
- Slide 2: Title "Revenue Highlights" with three bullet points.
- All slides share the light blue master background and display slide numbers and a "Confidential" footer.

## Key Notes

- A newly created `Presentation` always contains exactly **one** default slide master. You cannot have an empty `SlideMasters` collection when saving.
- `Layouts.Get(SlideLayoutType)` returns an existing layout or throws if not found. Use `Layouts.GetOrCreate(SlideLayoutType)` to create it if it doesn't exist.
- Shape positions use document units (1/300 inch). A widescreen slide (default) is approximately 4000 x 2250 units.
- Always `Dispose()` output streams after saving to avoid file locks.

## What to Learn Next

- [slides-and-shapes.md](slides-and-shapes.md): Slide management, all shape types, text formatting, tables, and backgrounds.
- [export.md](export.md): PDF export with options, merge/split presentations, and printing.
- [advanced-features.md](advanced-features.md): Speaker notes, headers/footers, text search/replace, themes, view settings, document properties.

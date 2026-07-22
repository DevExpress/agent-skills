# Getting Started with DevExpress Barcode Generation API

This guide walks you through setting up and generating your first barcode image in a .NET application.

## When to Use This Reference

Use this when you need to:
- Set up the Barcode Generation API for the first time
- Install the NuGet package and configure your project
- Understand the basic barcode generation workflow
- Create and save a QR Code barcode as a PNG image
- See a complete, compilable working example

## System Requirements

- .NET 8.0 / 9.0 / 10.0+ or .NET Framework 4.6.2+
- Visual Studio 2022+ (recommended) or JetBrains Rider
- A valid DevExpress license

## Installation

### Step 1: Install NuGet Packages

**.NET CLI:**
```bash
dotnet add package DevExpress.Document.Processor
```

**Package Manager Console:**
```
Install-Package DevExpress.Document.Processor
```

The `DevExpress.Document.Processor` package includes the `DevExpress.Docs.Barcode` namespace used by the modern Barcode Generation API.

## Non-Windows Platform Support (Linux, macOS, Docker, Cloud)

Barcode image export (`ExportToImage`, `DXImage`) uses a platform-specific drawing engine: GDI+ on Windows, SkiaSharp elsewhere. **The SkiaSharp-based engine is enabled automatically on non-Windows platforms.** Enable `Settings.DrawingEngine` at app startup only to force Skia *on Windows* (e.g., to work around the 10K GDI-handle limit).

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

### Step 2: Add Using Directives

Add the following `using` directives at the top of your file:

```csharp
using DevExpress.Docs.Barcode;
using DevExpress.Drawing;
using System.IO;
```

## Typical Barcode Generation Workflow

The Barcode Generation API follows a consistent four-step workflow for all barcode types:

1. **Choose symbology** — Select the right barcode type for your use case (QR Code, Code 128, EAN-13, Data Matrix, etc.)
2. **Configure common options** — Set colors, layout, DPI, module size via `BarcodeOptions` properties
3. **Set symbology-specific options** — Configure type-specific settings (e.g., `QRCodeOptions.ErrorCorrectionLevel`)
4. **Export the barcode image** — Write to a file stream or get an in-memory `DXImage`

## Your First Barcode — QR Code

The following example generates a QR Code and saves it as a PNG file:

```csharp
using DevExpress.Docs.Barcode;
using DevExpress.Drawing;
using System.IO;

// Step 1 & 2: Choose symbology and configure common options
var qrOptions = new QRCodeOptions();
qrOptions.Dpi = 96;
qrOptions.ModuleSize = 2f;
qrOptions.ShowText = false;

// Step 3: Configure QR Code-specific options
qrOptions.CompactionMode = QRCodeCompactionMode.Byte;
qrOptions.ErrorCorrectionLevel = QRCodeErrorCorrectionLevel.Q;

// Step 4: Export the barcode image
using var output = new FileStream("BarCodeImage.png", FileMode.Create, FileAccess.Write);
using var generator = new BarcodeGenerator(qrOptions);
generator.Export("https://www.devexpress.com", output, DXImageFormat.Png);
```

Run the project. The image is saved as `BarCodeImage.png` in the working directory.

## Understanding the Key Parameters

| Parameter | What it does |
|-----------|-------------|
| `Dpi` | Output resolution in dots per inch. Use 96 for screen, 300+ for print. |
| `ModuleSize` | Width (in units) of the narrowest bar or module. Increase for larger barcodes. |
| `ShowText` | Set to `true` to display human-readable text below the barcode. |
| `CompactionMode` | Controls how the data is encoded. `Byte` is the most general-purpose mode. |
| `ErrorCorrectionLevel` | Amount of data redundancy for recovery from damage. Q = 25% correction. |

## Output Formats

The `DXImageFormat` enum provides all supported raster image formats:

| Format | Enum Value | Use Case |
|--------|-----------|---------|
| PNG | `DXImageFormat.Png` | Web, documents, general use |
| BMP | `DXImageFormat.Bmp` | Windows bitmap, no compression |
| JPEG | `DXImageFormat.Jpeg` | Photos; avoid for barcodes (lossy) |
| TIFF | `DXImageFormat.Tiff` | Print workflows, archiving |
| GIF | `DXImageFormat.Gif` | Legacy web use |

To export as a vector PDF (preserving crisp rendering at any size):

```csharp
using var pdfStream = new FileStream("barcode.pdf", FileMode.Create, FileAccess.Write);
using var generator = new BarcodeGenerator(options);
generator.ExportToPdf("https://www.devexpress.com", pdfStream);
```

## Get Barcode as In-Memory Image

Use `ExportToImage` to get a `DXImage` object without writing to disk:

```csharp
using var generator = new BarcodeGenerator(qrOptions);
DXImage image = generator.ExportToImage("https://www.devexpress.com", DXImageFormat.Png);
// Use 'image' in your application: display in UI, embed in a document, etc.
```

## What to Learn Next

- [Barcode Types](barcode-types.md): All 30+ supported symbologies with code examples — choose the right type for your scenario.
- [Barcode Options & Export](barcode-options.md): Full reference for all configurable properties, customization patterns, and export methods.

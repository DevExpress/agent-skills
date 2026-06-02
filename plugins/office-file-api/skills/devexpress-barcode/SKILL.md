---
name: devexpress-barcode
description: Build .NET applications with the DevExpress Barcode Generation API for generating barcode images programmatically. Use when generating QR codes, Data Matrix, Code 128, EAN, UPC, PDF417, Aztec, GS1 barcodes, or any 1D/2D barcode type. Also use when someone mentions "DevExpress Barcode", "BarCode", "DevExpress.BarCodes", "generate QR code C#", "barcode image .NET", "create barcode programmatically", or asks about any barcode generation with DevExpress. Covers both .NET and .NET Framework.
compatibility: Requires .NET 6+ or .NET Framework 4.6.2+. NuGet packages from the DevExpress feed.
metadata:
  author: DevExpress
  version: "25.2"
  source-commit: c5a96ff6e891a1c2633c6621186093faaefabefd
---

# DevExpress Barcode Generation API

The Barcode Generation API is a non-visual .NET library for generating barcode images programmatically. It supports over 30 barcode symbologies — 1D and 2D — and can export barcode images to PNG, BMP, JPEG, TIFF, GIF, and PDF formats. The primary namespace is `DevExpress.Docs.Barcode`; a legacy namespace `DevExpress.BarCodes` is also available but is not recommended for new development.

## When to Use This Skill

Use this skill when you need to:

- Generate QR Code images from URLs, text, or binary data
- Generate Data Matrix barcodes for product labeling or manufacturing
- Generate PDF417 barcodes for transport, ID cards, and inventory
- Generate Aztec Code barcodes for ticketing and logistics
- Generate EAN-13, EAN-8, UPC-A, or UPC-E barcodes for retail
- Generate GS1 QR Code or GS1 Data Matrix for GS1-compliant workflows
- Generate EPC QR Codes for SEPA credit transfers
- Generate Code 128, Code 39, or Code 93 linear barcodes
- Export barcode images to PNG, BMP, JPEG, TIFF, GIF, or PDF
- Customize barcode appearance (colors, module size, DPI, quiet zone, border)
- Display or embed barcodes in ASP.NET, Blazor, WinForms, WPF, or MAUI apps

## Prerequisites & Installation

### NuGet Packages

| Package | Purpose |
|---------|---------|
| `DevExpress.Document.Processor` | Core barcode generation (includes `DevExpress.Docs.Barcode`) |

### .NET (6/7/8+)

```bash
dotnet add package DevExpress.Document.Processor
```

### .NET Framework (4.6.2+)

```
Install-Package DevExpress.Document.Processor
```

**Important**: All DevExpress packages in a project must share the same version number. A valid DevExpress license is required.

## Before You Start — Ask the Developer

Before generating code, ask these questions to avoid rework:

### General Questions
1. **Target framework**: Are you using .NET 8+, .NET 6/7, or .NET Framework 4.x?
2. **New or existing project?**: Are you creating a new project or adding to an existing one?
3. **Hosting model**: Console app, ASP.NET Core, Blazor, MAUI, WinForms, WPF, or something else?

### Barcode-Specific Questions
4. **Barcode type**: QR Code / Data Matrix / Code 128 / EAN-13 / UPC-A / PDF417 / Aztec / GS1 / other?
5. **Output format**: Save as PNG/BMP/JPEG/TIFF image file / get as Stream / embed in Word/Excel/PDF document?
6. **Special requirements**: GS1 encoding / EPC QR Code / quiet zone size / module size / colors / human-readable text?

> **Rule**: If the developer's answer is ambiguous or missing, ask before generating code. Do not guess.

## Component Overview

The Barcode Generation API provides:

- **Barcode creation**: Instantiate `BarcodeGenerator` with a symbology-specific options object (`QRCodeOptions`, `Code128Options`, `DataMatrixOptions`, etc.)
- **Common options**: Configure appearance and layout via `BarcodeOptions` properties (colors, DPI, module size, rotation, border, text)
- **Symbology-specific options**: Each barcode type exposes its own options class with symbology-specific properties
- **Export**: Write to `Stream` as image or PDF, or get a `DXImage` object, via `BarcodeGenerator.Export()`, `ExportToImage()`, `ExportToPdf()`
- **Fluent API**: Some symbologies expose `XxxOptionsBuilder` classes for a builder-style configuration pattern — check barcode-options.md for confirmed availability per type

### Core Entry Point

```csharp
using DevExpress.Docs.Barcode;
using DevExpress.Drawing;
using System.IO;

// 1. Choose symbology and configure options
var options = new QRCodeOptions();
options.Dpi = 96;
options.ModuleSize = 2f;
options.ShowText = false;
options.ErrorCorrectionLevel = QRCodeErrorCorrectionLevel.Q;

// 2. Generate and export
using var stream = new FileStream("qrcode.png", FileMode.Create, FileAccess.Write);
using var generator = new BarcodeGenerator(options);
generator.Export("https://www.devexpress.com", stream, DXImageFormat.Png);
```

## Documentation & Navigation Guide

### Getting Started
Refer to [references/getting-started.md](references/getting-started.md)

When you need to:
- Set up the Barcode Generation API for the first time
- Install and configure the NuGet package
- Generate your first QR Code barcode image
- See a complete step-by-step working example

### Barcode Types
Refer to [references/barcode-types.md](references/barcode-types.md)

When you need to:
- Choose the right barcode symbology for your use case
- See all supported 1D and 2D barcode types
- Find the options class name for a specific barcode type
- See code examples for QR Code, Data Matrix, PDF417, Code 128
- Understand GS1, EPC, and postal barcode specifics

### Barcode Options & Export
Refer to [references/barcode-options.md](references/barcode-options.md)

When you need to:
- Configure colors, module size, DPI, rotation, border, quiet zone
- Show or hide human-readable text below/above the barcode
- Save a barcode as a PNG, BMP, JPEG, TIFF, GIF, or PDF file
- Get a barcode as a `Stream` or `DXImage`
- Embed a barcode image in a Word Processing, Spreadsheet, or Presentation document
- Understand the difference between `ExportToImage()` and `Export(stream)`
- Understand all configurable `BarcodeOptions` properties

## Quick Start Example

A complete example — generate a QR Code and save it as PNG:

```csharp
using DevExpress.Docs.Barcode;
using DevExpress.Drawing;
using System.IO;

// Configure QR Code options
var qrOptions = new QRCodeOptions();
qrOptions.Dpi = 96;
qrOptions.ModuleSize = 2f;
qrOptions.ShowText = false;
qrOptions.CompactionMode = QRCodeCompactionMode.Byte;
qrOptions.ErrorCorrectionLevel = QRCodeErrorCorrectionLevel.Q;

// Export to PNG
using var output = new FileStream("qrcode.png", FileMode.Create, FileAccess.Write);
using var generator = new BarcodeGenerator(qrOptions);
generator.Export("https://www.devexpress.com", output, DXImageFormat.Png);
```

### What This Does
Creates a QR Code encoding the URL `https://www.devexpress.com` and saves it as `qrcode.png` in the working directory. The `ModuleSize` controls the size of each QR module in pixels; `ErrorCorrectionLevel.Q` provides 25% error correction capacity.

## Key Properties & API Surface

### BarcodeGenerator

| Property/Method | Type | Description |
|----------------|------|-------------|
| `BarcodeGenerator(BarcodeOptions)` | ctor | Creates a generator with the specified options object |
| `Export(string, Stream, DXImageFormat)` | `void` | Exports barcode as image to a stream |
| `ExportToImage(string, DXImageFormat)` | `DXImage` | Returns a `DXImage` object (in-memory) |
| `ExportToPdf(string, Stream)` | `void` | Exports barcode as a vector PDF to a stream |
| `Options` | `BarcodeOptions` | The current options object |
| `Dispose()` | `void` | Releases resources; use `using` statement |

### BarcodeOptions (common properties, all symbologies)

| Property | Type | Description |
|----------|------|-------------|
| `BackColor` | `Color` | Barcode background color |
| `ForeColor` | `Color` | Bar / module foreground color |
| `BorderColor` | `Color` | Border color |
| `BorderStyle` | `BorderStyle` | Border style (None, Center, etc.) |
| `BorderDashStyle` | `BorderDashStyle` | Border dash style |
| `BorderWidth` | `float` | Border thickness |
| `RotationAngle` | `float` | Rotation in degrees (0, 90, 180, 270) |
| `Dpi` | `float` | Output resolution in dots per inch |
| `ModuleSize` | `float` | Width of the narrowest bar/module |
| `ShowText` | `bool` | Whether to show human-readable text |
| `TextFont` | `DXFont` | Font for the human-readable text |
| `CodeTextHorizontalAlignment` | `DXStringAlignment` | Horizontal text alignment |
| `CodeTextVerticalAlignment` | `DXStringAlignment` | Vertical text alignment |
| `Padding` | `Padding` | Internal padding around the barcode |

### QRCodeOptions (symbology-specific)

| Property | Type | Description |
|----------|------|-------------|
| `CompactionMode` | `QRCodeCompactionMode` | Data compaction mode (Auto, Byte, Numeric, Alphanumeric) |
| `ErrorCorrectionLevel` | `QRCodeErrorCorrectionLevel` | Error correction (L=7%, M=15%, Q=25%, H=30%) |
| `Version` | `QRCodeVersion` | QR Code version (1-40 or Auto) |
| `IncludeQuietZone` | `bool` | Whether to include the quiet zone around the symbol |
| `Logo` | `DXImage` | Embedded logo image in the QR Code center |

## Common Patterns

### Save Barcode to File (PNG)

```csharp
using var stream = new FileStream("barcode.png", FileMode.Create, FileAccess.Write);
using var generator = new BarcodeGenerator(options);
generator.Export("data to encode", stream, DXImageFormat.Png);
```

### Get Barcode as DXImage (in-memory)

```csharp
using var generator = new BarcodeGenerator(options);
DXImage image = generator.ExportToImage("data to encode", DXImageFormat.Png);
// Use image in your application (e.g., display in UI or embed in a document)
```

### Export Barcode to PDF

```csharp
using var pdfStream = new FileStream("barcode.pdf", FileMode.Create, FileAccess.Write);
using var generator = new BarcodeGenerator(options);
generator.ExportToPdf("data to encode", pdfStream);
```

### Customize Colors and Border

```csharp
var options = new QRCodeOptions();
options.BackColor = DXColor.LightGray;
options.ForeColor = Color.DarkGreen;
options.BorderColor = DXColor.DarkCyan;
options.BorderStyle = BorderStyle.Center;
options.BorderDashStyle = BorderDashStyle.DashDot;
options.BorderWidth = 2f;
options.Dpi = 96;
options.ModuleSize = 3f;
options.ShowText = true;
options.TextFont = new DXFont("Segoe UI", 12f);
```

### Configure Options Directly (Recommended)

`QRCodeOptionsBuilder` does not exist in v25.2. Always configure options by assigning properties directly:

```csharp
var options = new QRCodeOptions();
options.ErrorCorrectionLevel = QRCodeErrorCorrectionLevel.H;
options.CompactionMode = QRCodeCompactionMode.Auto;
options.ModuleSize = 3f;
options.Dpi = 96;
options.ShowText = false;

using var stream = new FileStream("qrcode.png", FileMode.Create, FileAccess.Write);
using var generator = new BarcodeGenerator(options);
generator.Export("https://example.com", stream, DXImageFormat.Png);
```

## Version-Specific Notes

### Micro QR Code
> This barcode type requires DevExpress v26.1+. Reference to be added in a future update.

## Troubleshooting

| Symptom | Cause | Solution |
|---------|-------|----------|
| `"There are invalid characters in the text"` | Input contains characters not supported by the symbology | Check allowed character ranges in the barcode specification; use a different compaction mode or symbology |
| Barcode is too dense / not readable by scanner | Module size too small for printer/screen DPI | Increase `ModuleSize`; ensure `ModuleSize * Dpi` yields an integer pixel count |
| Scanner reads the barcode incorrectly | Encoding mismatch between generator and scanner | Check the scanner's expected encoding; use `QRCodeCompactionMode.Byte` with explicit `System.Text.Encoding` |
| Barcode appears on screen but scanner won't read it | Screen DPI too low; scanner not configured for this symbology | Export to high-DPI image; configure scanner for the correct symbology |
| Build error: missing assembly | NuGet package not installed or version mismatch | Run `dotnet add package DevExpress.Document.Processor` and ensure all DX packages share the same version |
| License error at runtime | Missing or invalid DevExpress license | Register your license key per the DevExpress installation guide |

## Constraints & Rules

CRITICAL — follow these rules in every interaction:

1. **Build verification**: After making changes, verify the project builds with `dotnet build`. Check for errors before reporting success.
2. **NuGet packages**: Use `DevExpress.Document.Processor`. Do not guess other package names.
3. **Namespace imports**: Always include `using DevExpress.Docs.Barcode;` and `using DevExpress.Drawing;`.
4. **Version consistency**: All DevExpress packages must use the same version. Do not mix.
5. **License**: DevExpress requires a valid license. Remind the developer if they hit license-related build errors.
6. **No destructive changes**: Preserve existing code structure. Only add or modify what is necessary.
7. **Framework detection**: Check the project's .csproj for target framework before writing code.
8. **Correct namespace**: Use `DevExpress.Docs.Barcode` (modern API), not `DevExpress.BarCodes` (legacy). Both exist but the `DevExpress.BarCodes` namespace is the legacy API.

## Using DevExpress Documentation MCP

If the DxDocs MCP server is available, use it to supplement this skill:

- **Search**: Use `devexpress_docs_search` with the technology "Barcode" and your question.
- **Fetch**: Use `devexpress_docs_get_content` with a documentation URL to get full article content.

**When to use MCP vs. built-in references:**
- **Built-in references**: Getting started, common patterns, key properties, troubleshooting.
- **MCP search**: Advanced scenarios not covered here, version-specific changes, uncommon features.
- **Always MCP for**: Exact method signatures, enum values, or edge cases when you are not 100% certain.

---

## Next Steps

Start with **[Getting Started](references/getting-started.md)** to install and configure the Barcode Generation API, then explore **[Barcode Types](references/barcode-types.md)** to choose the right symbology.

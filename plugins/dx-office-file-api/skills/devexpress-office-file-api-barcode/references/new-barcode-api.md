# New Barcode Generation API — DevExpress.Docs.Barcode

**v26.1+** — Modern barcode API in the `DevExpress.Docs.Barcode` namespace (NuGet: `DevExpress.Docs.Barcode`).

## When You Need to:
- Generate barcode images for any supported symbology with the new API
- Use the fluent builder pattern to configure barcodes in a type-safe, chainable way
- Export barcodes asynchronously to streams or get `DXImage` objects
- Migrate from the legacy `DevExpress.BarCodes` namespace
- Use Micro QR Code (v26.1+ only)

## Installation

```bash
dotnet add package DevExpress.Docs.Barcode
```

Or install the broader package (includes barcode + Office File API):
```bash
dotnet add package DevExpress.Document.Processor
```

## Basic Pattern

```csharp
using System.IO;
using DevExpress.Docs.Barcode;
using DevExpress.Drawing;

// 1. Create options for the desired symbology
var qrOptions = new QRCodeOptions();
qrOptions.Dpi = 96;
qrOptions.ModuleSize = 2f;
qrOptions.ShowText = false;
qrOptions.CompactionMode = QRCodeCompactionMode.Byte;
qrOptions.ErrorCorrectionLevel = QRCodeErrorCorrectionLevel.Q;

// 2. Create BarcodeGenerator with the options
// 3. Export to a stream
using var output = new FileStream("BarCodeImage.png", FileMode.Create, FileAccess.Write);
using var generator = new BarcodeGenerator(qrOptions);
generator.Export("https://www.devexpress.com", output, DXImageFormat.Png);
```

```vb
Imports System.IO
Imports DevExpress.Docs.Barcode
Imports DevExpress.Drawing

Dim qrOptions As New QRCodeOptions()
qrOptions.Dpi = 96
qrOptions.ModuleSize = 2F
qrOptions.ShowText = False
qrOptions.CompactionMode = QRCodeCompactionMode.Byte
qrOptions.ErrorCorrectionLevel = QRCodeErrorCorrectionLevel.Q

Using output As New FileStream("BarCodeImage.png", FileMode.Create, FileAccess.Write)
    Using generator As New BarcodeGenerator(qrOptions)
        generator.Export("https://www.devexpress.com", output, DXImageFormat.Png)
    End Using
End Using
```

## Fluent Builder Pattern

Each options class has a corresponding `XxxOptionsBuilder` for fluent configuration:

```csharp
using DevExpress.Docs.Barcode;
using DevExpress.Drawing;

var aztecOptions = AztecCodeOptionsBuilder.Create()
    .WithCompactionMode(AztecCodeCompactionMode.Text)
    .WithErrorCorrectionLevel(AztecCodeErrorCorrectionLevel.Level1)
    .WithVersion(AztecCodeVersion.Version45x45)
    .Build();

using var stream = new FileStream("aztec.png", FileMode.Create);
using var generator = new BarcodeGenerator(aztecOptions);
generator.Export("0123-456789", stream, DXImageFormat.Png);
```

```vb
Imports DevExpress.Docs.Barcode
Imports DevExpress.Drawing

Dim aztecOptions = AztecCodeOptionsBuilder.Create() _
    .WithCompactionMode(AztecCodeCompactionMode.Text) _
    .WithErrorCorrectionLevel(AztecCodeErrorCorrectionLevel.Level1) _
    .WithVersion(AztecCodeVersion.Version45x45) _
    .Build()

Using stream As New FileStream("aztec.png", FileMode.Create),
      generator As New BarcodeGenerator(aztecOptions)
    generator.Export("0123-456789", stream, DXImageFormat.Png)
End Using
```

## Async Export

```csharp
using var output = new FileStream("output.png", FileMode.Create);
using var generator = new BarcodeGenerator(new QRCodeOptions());

// Export to stream (async)
await generator.ExportAsync("https://example.com", output, DXImageFormat.Png);

// Or get a DXImage object (async)
DXImage image = await generator.ExportToImageAsync("https://example.com", DXImageFormat.Png);
image.Save("output.png", DXImageFormat.Png);
```

## Export to DXImage (Sync)

```csharp
using var generator = new BarcodeGenerator(new QRCodeOptions());
DXImage image = generator.ExportToImage("https://example.com", DXImageFormat.Png);

// Use image in a document, UI, or save to file
image.Save("barcode.png", DXImageFormat.Png);
```

## Export to PDF

```csharp
using var output = new FileStream("barcode.pdf", FileMode.Create);
using var generator = new BarcodeGenerator(new Code128Options());
generator.ExportToPdf("12345678", output);
```

## Micro QR Code (v26.1+)

Micro QR Code produces a compact 2D barcode for small labels.

```csharp
using DevExpress.Docs.Barcode;
using DevExpress.Drawing;

var microQrOptions = new MicroQRCodeOptions();
// Or use the fluent builder:
// var microQrOptions = MicroQRCodeOptionsBuilder.Create().Build();

using var output = new FileStream("micro_qr.png", FileMode.Create);
using var generator = new BarcodeGenerator(microQrOptions);
generator.Export("ABC-123", output, DXImageFormat.Png);
```

## Common Options Properties

All options classes inherit common properties from `BarcodeOptions`:

| Property | Type | Description |
|----------|------|-------------|
| `Dpi` | `float` | Output resolution (default: 96) |
| `ModuleSize` | `float` | Width/height of the smallest barcode element in pixels |
| `ShowText` | `bool` | Show human-readable text below the barcode |
| `Padding` | `Padding` | Quiet zone padding |
| `ForeColor` | `Color` | Barcode foreground color (default: Black) |
| `BackColor` | `Color` | Background color (default: White) |

## Key API

| Member | Description |
|--------|-------------|
| `BarcodeGenerator(BarcodeOptions)` | Constructor — accepts any `XxxOptions` instance |
| `BarcodeGenerator.Export(text, stream, format)` | Export barcode image to stream |
| `BarcodeGenerator.ExportAsync(text, stream, format)` | Async export to stream |
| `BarcodeGenerator.ExportToImage(text, format)` | Returns `DXImage` |
| `BarcodeGenerator.ExportToImageAsync(text, format)` | Async — returns `Task<DXImage>` |
| `BarcodeGenerator.ExportToPdf(text, stream)` | Export to PDF stream |
| `XxxOptionsBuilder.Create()` | Start fluent builder chain |
| `BarcodeOptionsBuilder.WithBackColor(Color)` | Common builder property |
| `BarcodeOptionsBuilder.WithShowText(bool)` | Common builder property |
| `BarcodeOptionsBuilder.Build()` | Produce the `BarcodeOptions` instance |

## Migration from DevExpress.BarCodes

Replace `using DevExpress.BarCodes;` with `using DevExpress.Docs.Barcode;` and update class names:

| Legacy (`DevExpress.BarCodes`) | New (`DevExpress.Docs.Barcode`) |
|-------------------------------|--------------------------------|
| `BarCode` + `Symbology.QRCode` | `QRCodeOptions` + `BarcodeGenerator` |
| `barCode.Save(path, format)` | `generator.Export(text, stream, format)` |
| `Options.QRCode.ErrorLevel` | `QRCodeOptions.ErrorCorrectionLevel` |
| `barCode.Module` | `options.ModuleSize` |
| `barCode.Margins` | `options.Margin` |
| `barCode.Paddings` | `options.Padding` |

For the full type mapping table, see [migrate-from-legacy-barcode-generation-api](https://docs.devexpress.com/OfficeFileAPI/405767).

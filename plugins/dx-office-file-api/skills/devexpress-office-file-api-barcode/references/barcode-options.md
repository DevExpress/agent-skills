# Barcode Options & Export — DevExpress Barcode Generation API

This reference covers all configurable barcode options, common customization patterns, and the full set of export methods available in `DevExpress.Docs.Barcode`.

## When to Use This Reference

Use this when you need to:
- Configure barcode appearance (colors, module size, DPI, rotation, border)
- Show or hide human-readable text (and change its font or alignment)
- Set the quiet zone or padding around the barcode
- Save a barcode to a PNG, BMP, JPEG, TIFF, GIF, or PDF file
- Get a barcode as a `Stream` or an in-memory `DXImage` object
- Understand all properties available on `BarcodeOptions`

## Key Classes and Types

| Class/Interface | Purpose |
|----------------|---------|
| `BarcodeGenerator` | Main entry point — creates, exports, and prints barcode images |
| `BarcodeOptions` | Abstract base class with all common options; inherited by every `XxxOptions` class |
| `QRCodeOptions` | Symbology-specific options for QR Code |
| `DataMatrixOptions` | Symbology-specific options for Data Matrix |
| `PDF417Options` | Symbology-specific options for PDF417 |
| `Code128Options` | Symbology-specific options for Code 128 |
| `DXImageFormat` | Enum specifying output image format (Png, Bmp, Jpeg, Tiff, Gif) |
| `DXImage` | DevExpress cross-platform image type returned by `ExportToImage` |
| `DXColor` | DevExpress color utilities (`DXColor.White`, `DXColor.LightGray`, etc.) |
| `DXFont` | DevExpress font type used for `TextFont` property |

## BarcodeOptions — All Common Properties

These properties are available on every symbology-specific options class (they are inherited from `BarcodeOptions`).

### Appearance

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `BackColor` | `Color` | White | Barcode background color |
| `ForeColor` | `Color` | Black | Bar / module foreground color |
| `BorderColor` | `Color` | Black | Border color |
| `BorderStyle` | `BorderStyle` | None | Border style around the barcode (`None`, `Center`, etc.) |
| `BorderDashStyle` | `BorderDashStyle` | Solid | Border line pattern (`Solid`, `Dash`, `DashDot`, etc.) |
| `BorderWidth` | `float` | 0 | Border thickness in units |

### Layout

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `RotationAngle` | `float` | 0 | Rotation in degrees: 0, 90, 180, or 270 |
| `Padding` | `Padding` | (0) | Internal padding between symbol and image edge |

### Output

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `Dpi` | `float` | 96 | Output resolution in dots per inch; use 300+ for print |
| `ModuleSize` | `float` | auto | Width of the narrowest bar or module; controls overall barcode size |

### Text (Human-Readable)

| Property | Type | Default | Description |
|----------|------|---------|-------------|
| `ShowText` | `bool` | `true` | Whether to display human-readable text |
| `TextFont` | `DXFont` | System font | Font used to render the text label |
| `CodeTextHorizontalAlignment` | `DXStringAlignment` | Center | Horizontal alignment of the text label |
| `CodeTextVerticalAlignment` | `DXStringAlignment` | Near | Vertical alignment of the text label |

## BarcodeGenerator — Export Methods

| Method | Description |
|--------|-------------|
| `Export(string text, Stream stream, DXImageFormat format)` | Export barcode as raster image to a stream |
| `Export(string text, Encoding encoding, Stream stream, DXImageFormat format)` | Export with explicit text encoding |
| `ExportToImage(string text, DXImageFormat format)` | Return barcode as an in-memory `DXImage` |
| `ExportToImage(string text, Encoding encoding, DXImageFormat format)` | Return `DXImage` with explicit encoding |
| `ExportToPdf(string text, Stream stream)` | Export barcode as vector PDF to a stream |

All methods accept a `string text` parameter — the data to encode into the barcode.

## Common Customization Patterns

### Basic — Minimal QR Code

```csharp
using DevExpress.Docs.Barcode;
using DevExpress.Drawing;
using System.IO;

var options = new QRCodeOptions();
options.Dpi = 96;
options.ModuleSize = 2f;
options.ShowText = false;
options.ErrorCorrectionLevel = QRCodeErrorCorrectionLevel.Q;

using var stream = new FileStream("qrcode.png", FileMode.Create, FileAccess.Write);
using var generator = new BarcodeGenerator(options);
generator.Export("https://example.com", stream, DXImageFormat.Png);
```

### Custom Colors and Border

```csharp
using DevExpress.Docs.Barcode;
using DevExpress.Drawing;
using System.IO;
using System.Drawing;

var options = new QRCodeOptions();
options.BackColor = DXColor.LightGray;
options.ForeColor = Color.DarkGreen;
options.BorderColor = DXColor.DarkCyan;
options.BorderStyle = BorderStyle.Center;
options.BorderDashStyle = BorderDashStyle.DashDot;
options.BorderWidth = 2f;
options.Dpi = 96;
options.ModuleSize = 3f;
options.ErrorCorrectionLevel = QRCodeErrorCorrectionLevel.Q;

using var stream = new FileStream("custom.png", FileMode.Create, FileAccess.Write);
using var generator = new BarcodeGenerator(options);
generator.Export("https://example.com", stream, DXImageFormat.Png);
```

### Show Human-Readable Text

```csharp
var options = new Code128Options();
options.ShowText = true;
options.TextFont = new DXFont("Segoe UI", 10f);
options.CodeTextHorizontalAlignment = DXStringAlignment.Center;
options.CodeTextVerticalAlignment = DXStringAlignment.Far;
options.ModuleSize = 2f;
options.Dpi = 96;
```

### Rotated Barcode (90 degrees)

```csharp
var options = new Code128Options();
options.RotationAngle = 90;
options.ModuleSize = 2f;
options.ShowText = false;
```

### Save to PNG File

```csharp
using var stream = new FileStream("barcode.png", FileMode.Create, FileAccess.Write);
using var generator = new BarcodeGenerator(options);
generator.Export("data", stream, DXImageFormat.Png);
```

### Save to TIFF File (for print workflows)

```csharp
using var stream = new FileStream("barcode.tiff", FileMode.Create, FileAccess.Write);
using var generator = new BarcodeGenerator(options);
generator.Export("data", stream, DXImageFormat.Tiff);
```

### Get Barcode as Stream (no file)

```csharp
var memStream = new MemoryStream();
using var generator = new BarcodeGenerator(options);
generator.Export("data", memStream, DXImageFormat.Png);
memStream.Position = 0;
// Use memStream: send in HTTP response, embed in document, etc.
```

### Export to PDF (vector)

```csharp
using var pdfStream = new FileStream("barcode.pdf", FileMode.Create, FileAccess.Write);
using var generator = new BarcodeGenerator(options);
generator.ExportToPdf("data", pdfStream);
```

### Get DXImage Object (in-memory)

```csharp
using var generator = new BarcodeGenerator(options);
DXImage image = generator.ExportToImage("data", DXImageFormat.Png);
// image can be used wherever a DXImage is accepted
```

### High-Resolution for Printing (300 DPI)

```csharp
var options = new Code128Options();
options.Dpi = 300;
options.ModuleSize = 1f;   // 1 unit at 300 DPI = 300 pixels per unit
options.ShowText = false;

using var stream = new FileStream("barcode_print.png", FileMode.Create, FileAccess.Write);
using var generator = new BarcodeGenerator(options);
generator.Export("PRINT-123456", stream, DXImageFormat.Png);
```

### QR Code with Logo

```csharp
using DevExpress.Docs.Barcode;
using DevExpress.Drawing;
using System.IO;

var options = new QRCodeOptions();
options.ErrorCorrectionLevel = QRCodeErrorCorrectionLevel.H;  // H required for logos
options.CompactionMode = QRCodeCompactionMode.Auto;
options.IncludeQuietZone = true;
options.ModuleSize = 4f;

using var logoStream = new FileStream("logo.png", FileMode.Open);
options.Logo = DXImage.FromStream(logoStream);

using var outputStream = new FileStream("qr_with_logo.png", FileMode.Create, FileAccess.Write);
using var generator = new BarcodeGenerator(options);
generator.Export("https://example.com", outputStream, DXImageFormat.Png);
```

> When embedding a logo, always use `ErrorCorrectionLevel.H` (30% correction) so the QR Code remains scannable despite the logo covering part of the symbol.

### ExportToImage vs Export(stream) — When to Use Which

Both methods produce a raster image of the barcode, but serve different downstream needs:

| Scenario | Method | Returns |
|----------|--------|---------|
| Save to file | `Export(text, fileStream, format)` | `void` |
| Embed into Word / Presentation / PDF | `Export(text, memoryStream, format)` → wrap stream | `void` + `MemoryStream` |
| Use as a `DXImage` in DevExpress UI controls | `ExportToImage(text, format)` | `DXImage` |
| Pass to `DocumentImageSource.FromStream()` | Either path works | `MemoryStream` |

Use `ExportToImage()` when the code you're calling expects a `DXImage` object directly. Use `Export(stream)` when you need a `MemoryStream` to pass to another API.

## Embedding a Barcode in a Word / Spreadsheet / Presentation Document

To insert a barcode image into a DevExpress Word Processing, Spreadsheet, or Presentation document, generate the barcode into a `MemoryStream` and then use the document's image insertion API.

### Barcode → Word Processing Document

```csharp
using DevExpress.Docs.Barcode;
using DevExpress.Drawing;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;
using System.IO;

// 1. Generate barcode into a MemoryStream
var options = new QRCodeOptions();
options.Dpi = 96;
options.ModuleSize = 3f;
options.ShowText = false;

var memStream = new MemoryStream();
using var generator = new BarcodeGenerator(options);
generator.Export("https://example.com", memStream, DXImageFormat.Png);
memStream.Position = 0;

// 2. Insert into the Word document
using var server = new RichEditDocumentServer();
server.LoadDocument("document.docx");
Document doc = server.Document;

DocumentImageSource imageSource = DocumentImageSource.FromStream(memStream);
doc.Images.Insert(doc.Range.End, imageSource);

server.SaveDocument("output.docx", DocumentFormat.Docx);
```

### Barcode → Spreadsheet Document

```csharp
using DevExpress.Docs.Barcode;
using DevExpress.Drawing;
using DevExpress.Spreadsheet;
using System.IO;

var memStream = new MemoryStream();
using var generator = new BarcodeGenerator(new QRCodeOptions { Dpi = 96, ModuleSize = 3f });
generator.Export("https://example.com", memStream, DXImageFormat.Png);
memStream.Position = 0;

using var workbook = new Workbook();
Worksheet sheet = workbook.Worksheets[0];
sheet.Pictures.AddPicture(memStream, sheet.Cells["B2"]);

workbook.SaveDocument("output.xlsx");
```

> **Note**: When using the modern `BarcodeGenerator` API, always generate into a `MemoryStream` and reset `Position = 0` before passing the stream to the document API. Do not use the legacy `DevExpress.BarCodes.BarCode.BarCodeImage` property — it is deprecated and not available in `DevExpress.Docs.Barcode`.

## Image Format Reference

| `DXImageFormat` Value | Extension | Notes |
|----------------------|-----------|-------|
| `DXImageFormat.Png` | `.png` | Lossless, best general choice |
| `DXImageFormat.Bmp` | `.bmp` | Uncompressed, large files |
| `DXImageFormat.Jpeg` | `.jpg` | Lossy — avoid for barcodes |
| `DXImageFormat.Tiff` | `.tiff` | Lossless, preferred for archiving and print |
| `DXImageFormat.Gif` | `.gif` | Limited palette; legacy use |
| `DXImageFormat.Emf` | `.emf` | Vector (Windows metafile); passed to `ExportToImage` |

For vector output use `ExportToPdf()` (cross-platform) or `DXImageFormat.Emf` (Windows only).

## Troubleshooting

- **Barcode too small**: Increase `ModuleSize`. For screen use, try values between 2 and 5. For print at 300 DPI, try 0.01–0.03 inches.
- **Rounding errors in printed barcodes**: Ensure `ModuleSize * Dpi` yields an integer. For example, at 300 DPI use `0.01333` (4 pixels) or `0.01667` (5 pixels) rather than `0.015`.
- **Text appears cut off**: Increase `Padding` to add space around the symbol.
- **Wrong color in exported image**: Verify `DXColor` vs `System.Drawing.Color` — both work but use consistent imports.
- **Logo makes QR Code unreadable**: Switch `ErrorCorrectionLevel` to `H` and reduce the logo to 30% of QR symbol area.

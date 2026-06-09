# Barcode Types — DevExpress Barcode Generation API

The DevExpress Barcode Generation API supports over 30 barcode symbologies. Each type is represented by a dedicated options class in the `DevExpress.Docs.Barcode` namespace.

## When to Use This Reference

Use this when you need to:
- Choose the right barcode type for your use case
- Find the `XxxOptions` class name for a specific symbology
- See code examples for the most common barcode types
- Understand 2D, linear (1D), GS1, and postal barcode families
- Look up the quick-reference table of all supported types

## Quick-Reference Table: All Supported Types

| Barcode Type | Category | Options Class | Use Case |
|-------------|----------|--------------|---------|
| QR Code | 2D | `QRCodeOptions` | URLs, text, binary; general-purpose |
| GS1 QR Code | 2D / GS1 | `QRCodeGS1Options` | GS1 General Specification |
| EPC QR Code | 2D / Payment | `QRCodeEPCOptions` | SEPA credit transfers |
| Data Matrix (ECC200) | 2D | `DataMatrixOptions` | Manufacturing, labeling, small items |
| GS1 Data Matrix | 2D / GS1 | `DataMatrixGS1Options` | GS1-compliant Data Matrix |
| PDF417 | 2D | `PDF417Options` | Transport, ID cards, large data sets |
| Aztec Code | 2D | `AztecCodeOptions` | Ticketing, logistics, no quiet zone needed |
| Micro QR Code | 2D | `MicroQRCodeOptions` | Small labels (v26.1+) |
| Intelligent Mail | 2D / Postal | `IntelligentMailOptions` | USPS mail sorting |
| Code 128 | Linear | `Code128Options` | Alphanumeric; shipping, inventory |
| Code 39 (USD-3) | Linear | `Code39Options` | Uppercase alphanumeric; general use |
| Code 39 Extended | Linear | `Code39ExtendedOptions` | Full ASCII |
| Code 93 | Linear | `Code93Options` | Compact alphanumeric |
| Code 93 Extended | Linear | `Code93ExtendedOptions` | Full ASCII, compact |
| Codabar | Linear | `CodabarOptions` | Medical, libraries, blood banks |
| Code 11 (USD-8) | Linear | `Code11Options` | Telecom equipment |
| Interleaved 2 of 5 | Linear | `Interleaved2of5Options` | Numeric; warehousing |
| Industrial 2 of 5 | Linear | `Industrial2of5Options` | Numeric; older industrial use |
| Matrix 2 of 5 | Linear | `Matrix2of5Options` | Numeric; warehouse, air cargo |
| MSI-Plessey | Linear | `CodeMSIOptions` | Retail shelf labels |
| PostNet | Linear | `PostNetOptions` | USPS postal routing |
| EAN-13 | Linear / GS1 | `EAN13Options` | Retail product identification (global) |
| EAN-8 | Linear / GS1 | `EAN8Options` | Small retail items (8-digit EAN) |
| EAN-128 (UCC) | Linear / GS1 | `EAN128Options` | GS1-compliant serial shipping |
| UPC-A | Linear / GS1 | `UPCAOptions` | US retail (12-digit) |
| UPC-E0 | Linear / GS1 | `UPCE0Options` | Compact US retail (6-digit) |
| UPC-E1 | Linear / GS1 | `UPCE1Options` | Compact US retail (alternate) |
| UPC Supplemental 2 | Linear / GS1 | `UPCSupplemental2Options` | Periodical add-on (2-digit) |
| UPC Supplemental 5 | Linear / GS1 | `UPCSupplemental5Options` | Book price add-on (5-digit) |
| GS1 DataBar | Linear / GS1 | `DataBarOptions` | POS, coupons, small items |
| SSCC-18 | Linear / GS1 | `SSCCOptions` | Serial shipping container code |
| ITF-14 | Linear / GS1 | `Interleaved2of5Options` | Retail outer packaging (14-digit) |

---

## 2D Barcodes

2D barcodes encode data in both horizontal and vertical directions, allowing high data density in a compact symbol.

### QR Code

QR Code (Quick Response) is the most widely used 2D barcode. It encodes text, URLs, or binary data and is readable by smartphone cameras and dedicated scanners.

```csharp
using DevExpress.Docs.Barcode;
using DevExpress.Drawing;
using System.IO;

var qrOptions = new QRCodeOptions();
qrOptions.Dpi = 96;
qrOptions.ModuleSize = 2f;
qrOptions.ShowText = false;
qrOptions.CompactionMode = QRCodeCompactionMode.Byte;
qrOptions.ErrorCorrectionLevel = QRCodeErrorCorrectionLevel.Q;
qrOptions.IncludeQuietZone = true;

using var stream = new FileStream("qrcode.png", FileMode.Create, FileAccess.Write);
using var generator = new BarcodeGenerator(qrOptions);
generator.Export("https://www.devexpress.com", stream, DXImageFormat.Png);
```

**QR Code symbology-specific options:**

| Property | Values | Description |
|----------|--------|-------------|
| `CompactionMode` | `Auto`, `Byte`, `Numeric`, `Alphanumeric` | Data encoding mode |
| `ErrorCorrectionLevel` | `L` (7%), `M` (15%), `Q` (25%), `H` (30%) | Error correction capacity |
| `Version` | `Auto`, `Version1` to `Version40` | Symbol size |
| `IncludeQuietZone` | `bool` | Whether to include blank border |
| `Logo` | `DXImage` | Embedded image in QR center |

### Data Matrix (ECC200)

Data Matrix is a compact 2D symbology widely used in manufacturing, electronics labeling, medical devices, and postal services. It uses an L-shaped finder pattern.

```csharp
using DevExpress.Docs.Barcode;
using DevExpress.Drawing;
using System.IO;

var dmOptions = new DataMatrixOptions();
dmOptions.ModuleSize = 5;
dmOptions.Padding = new System.Drawing.Padding(5);
dmOptions.BorderWidth = 1;
dmOptions.CompactionMode = DataMatrixCompactionMode.ASCII;

using var stream = new FileStream("datamatrix.png", FileMode.Create, FileAccess.Write);
using var generator = new BarcodeGenerator(dmOptions);
generator.Export("ABC1234567890", stream, DXImageFormat.Png);
```

**Data Matrix symbology-specific options:**

| Property | Description |
|----------|-------------|
| `CompactionMode` | Data encoding mode (ASCII, C40, Text, Binary, etc.) |
| `MatrixSize` | Symbol size (Auto or a specific `DataMatrixSize` value) |

### PDF417

PDF417 (Portable Data File) is a stacked 2D barcode used for transport documents, ID cards, and high-capacity data storage. It supports 3 to 90 rows.

```csharp
using DevExpress.Docs.Barcode;
using DevExpress.Drawing;
using System.IO;
using System.Drawing;

var pdf417Options = new PDF417Options();
pdf417Options.Columns = 3;
pdf417Options.TruncateSymbol = false;
pdf417Options.ErrorCorrectionLevel = PDF417ErrorCorrectionLevel.Level2;
pdf417Options.BackColor = Color.White;
pdf417Options.ShowText = false;
pdf417Options.Padding = new System.Drawing.Padding(5);
pdf417Options.BorderWidth = 1;
pdf417Options.BorderColor = Color.Blue;

using var stream = new FileStream("pdf417.png", FileMode.Create, FileAccess.Write);
using var generator = new BarcodeGenerator(pdf417Options);
generator.Export("PDF417 DEMO 1234567890", stream, DXImageFormat.Png);
```

**PDF417 symbology-specific options:**

| Property | Description |
|----------|-------------|
| `CompactionMode` | Data compaction (Binary, Text, Numeric) |
| `ErrorCorrectionLevel` | Error correction level (Level0 to Level8) |
| `Rows` | Number of rows (auto or specified) |
| `Columns` | Number of columns (auto or 1-30) |
| `TruncateSymbol` | Whether to use truncated (compact) form |
| `YToXRatio` | Height-to-width ratio of rows |

### Aztec Code

Aztec Code is a matrix barcode that does not require a quiet zone, making it ideal for space-constrained applications like transport tickets.

```csharp
using DevExpress.Docs.Barcode;
using DevExpress.Drawing;
using System.IO;

var aztecOptions = new AztecCodeOptions();
aztecOptions.CompactionMode = AztecCodeCompactionMode.Binary;
aztecOptions.ErrorCorrectionLevel = AztecCodeErrorCorrectionLevel.Level2;
aztecOptions.ModuleSize = 4;

using var stream = new FileStream("aztec.png", FileMode.Create, FileAccess.Write);
using var generator = new BarcodeGenerator(aztecOptions);
generator.Export("Aztec Code Data 12345", stream, DXImageFormat.Png);
```

### GS1 QR Code

GS1 QR Code is a QR Code variant conforming to the GS1 General Specification for supply chain and trade item applications.

```csharp
var gs1QrOptions = new QRCodeGS1Options();
gs1QrOptions.FNC1Substitute = "#";    // FNC1 separator character
gs1QrOptions.ErrorCorrectionLevel = QRCodeErrorCorrectionLevel.M;
gs1QrOptions.ModuleSize = 3f;

using var stream = new FileStream("gs1qr.png", FileMode.Create, FileAccess.Write);
using var generator = new BarcodeGenerator(gs1QrOptions);
generator.Export("(01)00123456789012(17)250101(10)ABC123", stream, DXImageFormat.Png);
```

### EPC QR Code

EPC QR Code initiates SEPA credit transfers. It uses the `QRCodeEPCOptions` class which exposes payment-specific fields.

```csharp
var epcOptions = new QRCodeEPCOptions();
epcOptions.BeneficiaryName = "Example GmbH";
epcOptions.IBAN = "DE89370400440532013000";
epcOptions.BIC = "COBADEFFXXX";
epcOptions.TransferAmount = 100.00m;
epcOptions.RemittanceInformation = "Invoice 2025-001";
epcOptions.ErrorCorrectionLevel = QRCodeErrorCorrectionLevel.M;

using var stream = new FileStream("epc_qr.png", FileMode.Create, FileAccess.Write);
using var generator = new BarcodeGenerator(epcOptions);
generator.Export(string.Empty, stream, DXImageFormat.Png);
```

### Micro QR Code

> This barcode type requires DevExpress v26.1+. Reference to be added in a future update.

---

## Linear (1D) Barcodes

### Code 128

Code 128 is a high-density alphanumeric symbology supporting the full ASCII character set. It is the most commonly used linear barcode in shipping and inventory.

```csharp
using DevExpress.Docs.Barcode;
using DevExpress.Drawing;
using System.IO;
using System.Drawing;

var code128Options = new Code128Options();
code128Options.ShowText = true;
code128Options.Charset = Code128CharacterSet.CharsetB;
code128Options.AddLeadingZero = true;
code128Options.Padding = new System.Drawing.Padding(5);
code128Options.BorderWidth = 1;

using var stream = new FileStream("code128.png", FileMode.Create, FileAccess.Write);
using var generator = new BarcodeGenerator(code128Options);
generator.Export("Code128 Sample", stream, DXImageFormat.Png);
```

**Code 128 symbology-specific options:**

| Property | Description |
|----------|-------------|
| `Charset` | Character set (Auto, CharsetA, CharsetB, CharsetC) |
| `AddLeadingZero` | Prepend a leading zero to odd-length data |
| `FNC1Substitute` | Placeholder character for FNC1 |
| `FNC2Substitute` | Placeholder character for FNC2 |
| `FNC3Substitute` | Placeholder character for FNC3 |
| `FNC4Substitute` | Placeholder character for FNC4 |

---

## GS1 Barcode Family

GS1 barcodes encode data using standardized GS1 Application Identifiers (AIs). Use them in supply chain, retail, and healthcare workflows.

- **EAN-13 / EAN-8**: Standard retail barcodes. Options classes: `EAN13Options`, `EAN8Options`. No symbology-specific options (data must be exactly 12 or 7 digits; check digit is appended automatically).
- **EAN-128 (UCC)**: GS1-compliant serial shipping. Options class: `EAN128Options`. Supports `FNC1Substitute` for AI separation.
- **UPC-A**: 12-digit US retail. Options class: `UPCAOptions`.
- **UPC-E0 / UPC-E1**: Compact 6-digit UPC. Options classes: `UPCE0Options`, `UPCE1Options`.
- **GS1 DataBar**: Encodes 14-digit GTINs; supported variants (Omnidirectional, Stacked, Truncated, Limited, Expanded) via `DataBarOptions.DataBarType`.
- **SSCC-18**: Serial Shipping Container Code. Options class: `SSCCOptions`.
- **GS1 Data Matrix**: GS1-compliant Data Matrix. Options class: `DataMatrixGS1Options`. Supports `FNC1Substitute`.
- **ITF-14**: 14-digit barcode for retail outer packaging; uses `Interleaved2of5Options`.

### EAN-13 Example

```csharp
var ean13Options = new EAN13Options();
ean13Options.ShowText = true;
ean13Options.ModuleSize = 2f;
ean13Options.Dpi = 96;

using var stream = new FileStream("ean13.png", FileMode.Create, FileAccess.Write);
using var generator = new BarcodeGenerator(ean13Options);
// Provide exactly 12 digits; check digit is added automatically
generator.Export("590123412345", stream, DXImageFormat.Png);
```

---

## Postal Barcodes

- **PostNet** (`PostNetOptions`): USPS postal routing barcode for ZIP codes.
- **Intelligent Mail** (`IntelligentMailOptions`): USPS Intelligent Mail barcode (IMb) for mail piece sorting and tracking.

### PostNet Example

```csharp
var postnetOptions = new PostNetOptions();
postnetOptions.ShowText = true;

using var stream = new FileStream("postnet.png", FileMode.Create, FileAccess.Write);
using var generator = new BarcodeGenerator(postnetOptions);
generator.Export("123456789", stream, DXImageFormat.Png);
```

---

## Choosing the Right Barcode Type

| Scenario | Recommended Type |
|----------|----------------|
| URL, general text, binary data | QR Code |
| SEPA bank transfer | EPC QR Code |
| GS1 supply chain (trade item) | GS1 QR Code or GS1 Data Matrix |
| Small label (manufacturing) | Data Matrix (ECC200) |
| Transport ticket | Aztec Code |
| High-capacity document | PDF417 |
| Alphanumeric product code | Code 128 |
| US retail (grocery) | UPC-A or EAN-13 |
| Global retail | EAN-13 |
| USPS postal mail | PostNet or Intelligent Mail |
| Shipping container | EAN-128 or SSCC-18 |

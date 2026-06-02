# Barcode Controls

## When to Use This Reference

Use when displaying barcodes of 35 supported types (1D/2D): `XRBarCode`.

## XRBarCode — Barcode Display

Displays 35 supported barcode types:

```csharp
var barcode = new XRBarCode();
detail.Controls.Add(barcode);
barcode.BoundsF = new RectangleF(0, 0, 120, 60);
barcode.Symbology = new QRCodeGenerator { Version = 2, ErrorCorrectionLevel = QRCodeErrorCorrectionLevel.Q };
barcode.AutoModule = true;  // auto-fit bar width to control size
barcode.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "[SKU]"));
```

**Main content properties**: 
- `Text` (data field or expression to encode)
- `BinaryData` (for encoding raw byte arrays)

**Other key properties**:
- `Symbology` — a generator object that defines the barcode type and its specific options.
- `AutoModule` — when `true`, automatically calculates bar width from control dimensions. Recommended for most scenarios.
- `Module` — fixed narrow bar width in report units (used when `AutoModule = false`).
- `TargetDeviceDpi` — adjust bar density for the target printer's DPI to ensure scan accuracy.

**1D barcodes (linear)**: `Codabar`, `Code11`, `Code128`, `Code39` (USD-3), `Code39Extended`, `Code93`, `Code93Extended`, `DeutschePostLeitcode`, `DeutschePostIdentcode`, `EAN8`, `EAN13`, `GS1-128` (EAN-128/UCC), `GS1DataBar`, `Industrial2of5`, `IntelligentMailPackage`, `Interleaved2of5`, `Matrix2of5`, `MSIPlessey`, `Pharmacode`, `PostNet`, `SSCC18`, `ITF14`, `UPCSupplemental2`, `UPCSupplemental5`, `UPCA`, `UPCE0`, `UPCE1`.

**2D barcodes (matrix)**: `AztecCode`, `DataMatrix` (ECC200), `GS1DataMatrix`, `IntelligentMail`, `PDF417`, `QRCode`, `MicroQRCode`, `GS1QRCode`.

Each barcode type corresponds to a specific symbology class generator with its own set of options. Use DevExpress MCP (dxdocs) documentation to find a proper class generator for the required symbology and its options.

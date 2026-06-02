// DevExpress Barcode Generation API — Quick Start
// Generates a QR Code barcode from a URL string and saves it as "qrcode.png"
//
// Prerequisites:
//   dotnet add package DevExpress.Document.Processor
//
// Namespaces:
//   DevExpress.Docs.Barcode  — modern barcode generation API
//   DevExpress.Drawing       — DXImageFormat, DXColor, DXFont

using DevExpress.Docs.Barcode;
using DevExpress.Drawing;
using System.IO;

// The URL (or any string) to encode into the QR Code
string urlToEncode = "https://www.devexpress.com";

// Step 1: Create a QRCodeOptions object and configure common options
var qrOptions = new QRCodeOptions();
qrOptions.Dpi = 96;           // 96 DPI for screen; use 300+ for print
qrOptions.ModuleSize = 2f;    // Width of each QR module in pixels (at the given DPI)
qrOptions.ShowText = false;   // Do not show human-readable text below the symbol

// Step 2: Configure QR Code-specific options
qrOptions.CompactionMode = QRCodeCompactionMode.Byte;          // Byte mode supports all UTF-8 data
qrOptions.ErrorCorrectionLevel = QRCodeErrorCorrectionLevel.Q; // Q = 25% error correction capacity
qrOptions.IncludeQuietZone = true;                             // Include blank border around symbol

// Step 3: Create a BarcodeGenerator with the options and export to PNG
using var outputStream = new FileStream("qrcode.png", FileMode.Create, FileAccess.Write);
using var generator = new BarcodeGenerator(qrOptions);
generator.Export(urlToEncode, outputStream, DXImageFormat.Png);

Console.WriteLine("QR Code saved to: qrcode.png");

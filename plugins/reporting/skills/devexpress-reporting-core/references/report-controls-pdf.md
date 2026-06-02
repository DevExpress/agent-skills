# PDF Content and Signature Controls

## When to Use This Reference

Use when rendering PDF files or digital signatures in reports: `XRPdfContent`, `XRPdfSignature`.

## XRPdfContent — Embed PDF Pages

Renders an existing PDF file's pages inside a report:

```csharp
var pdfContent = new XRPdfContent();
// Option 1: embed binary data (stored in .repx layout — no external dependency at render time)
pdfContent.Source = System.IO.File.ReadAllBytes("MasterDetailReport.pdf");
// Option 2: reference by path/URL (resolved at render time; takes precedence over Source)
pdfContent.SourceUrl = "template.pdf";
// Option 3: bind to a data field containing byte[] PDF data
pdfContent.ExpressionBindings.Add(new ExpressionBinding("Source", "[PdfData]"));

// Default mode: each PDF page = one report page
detailBand.Controls.Add(pdfContent);

// Embedded mode: PDF content overlaid with other report controls on the same page
pdfContent.GenerateOwnPages = false;  // must be false to embed
```

**Main properties**: 
- `Source` (accepts PDF data as a byte array, ideal for embedding PDF content directly into the report layout without external dependencies)
- `SourceUrl` (string path or URL to the PDF file; takes precedence over `Source` when both are set)

**Other key properties**: 
- `GenerateOwnPages` (default `true` = each PDF page becomes a separate report page; `false` = embeds into band)
- `PageRange` (select specific pages when embedding)

**Limitations**:
- Cannot be added to: `TopMarginBand`/`BottomMarginBand`, `PageHeaderBand`/`PageFooterBand`, `GroupHeaderBand`/`GroupFooterBand` with `RepeatEveryPage`, or vertical bands.
- RTF export: multiple page sizes cropped — use DOCX Single Page export instead.
- Linux/Azure: requires `DevExpress.Pdf.SkiaRenderer` NuGet package (+ `SkiaSharp.NativeAssets.Linux` on Linux).

## XRPdfSignature — Digital Signature Field

Visualizes a digital signature field in PDF export:

```csharp
var sig = new XRPdfSignature();
sig.BoundsF = new RectangleF(0, 0, 200, 60);
detailBand.Controls.Add(sig);
sig.SignatureOptions.ImageDisplayMode    = DevExpress.XtraPrinting.SignatureImageDisplayMode.Show;
sig.SignatureOptions.ShowCaptions        = true;
sig.SignatureOptions.ShowCertificateName = true;
sig.SignatureOptions.ShowLocation        = true;
sig.SignatureOptions.ShowSignatureDate   = true;
sig.SignatureOptions.ShowSignatureReason = true;
sig.SignatureOptions.DisplayDocumentSignature = true;  // this control renders the doc signature

// Wire up the actual certificate on the report's PDF export options:
var signatureOptions = report.ExportOptions.Pdf.SignatureOptions;
var certificate = new System.Security.Cryptography.X509Certificates.X509Certificate2(@"Data\AndrewFuller.pfx", "AndrewFullerPassword");
signatureOptions.Certificate = certificate;
signatureOptions.ImageSource = ImageSource.FromFile(@"Images\signature.png");
signatureOptions.Reason = "Approved";
signatureOptions.Location = "USA";
signatureOptions.ContactInfo = "andrew.fuller@example.com";
report.ExportToPdf("ReportWithSignature.pdf");
```

**Key properties on `SignatureOptions**:
- `DisplayDocumentSignature` (first control `true` → renders doc signature; additional controls `false` → blank form fields for user input)
- `ImageDisplayMode` (Show / Hide / ShowCertificateNameAsImage)
- `ShowCaptions`, `ShowCertificateName`, `ShowDistinguishedName`, `ShowLocation`, `ShowSignatureDate`, `ShowSignatureReason`

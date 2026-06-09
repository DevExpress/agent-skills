# Metadata and Attachments — New PDF Document API

> **CTP Warning**: `DevExpress.Docs.Pdf` is a Community Technology Preview. Do not use in mission-critical production applications.

## When You Need to:
- Read or write standard PDF document properties (Title, Author, Subject, Keywords)
- Read or write XMP metadata schemas (Dublin Core, PDF, XMP rights)
- Embed file attachments in a PDF document
- Attach a ZUGFeRD/Factur-X electronic invoice XML to a PDF

## Document Metadata

Access standard PDF metadata via `PdfDocument.Metadata`:

```csharp
using DevExpress.Docs.Pdf;
using System.IO;

using (PdfDocument document = new PdfDocument(File.OpenRead("input.pdf"))) {

    // Read/write standard document info via DocumentInfo
    DocumentInfo info = document.Metadata.DocumentInfo;
    info.Title = "Annual Report 2026";
    info.Author = "Finance Team";
    info.Subject = "Q4 Financial Summary";
    info.Keywords = "finance, annual report, 2026";

    document.Save(File.Create("output.pdf"));
}
```

## XMP Metadata

XMP metadata provides richer, schema-based metadata stored as XML in the PDF:

```csharp
using DevExpress.Docs.Pdf;
using System.IO;

using (PdfDocument document = new PdfDocument(File.OpenRead("input.pdf"))) {

    // Access XMP metadata
    XmpMetadata xmp = document.Metadata.Xmp;

    // Synchronize standard info into XMP (InfoToXmp direction)
    document.Metadata.Synchronize(MetadataSyncMode.InfoToXmp);

    document.Save(File.Create("output.pdf"));
}
```

### Migration Note

| Legacy (`DevExpress.Pdf`) | New (`DevExpress.Docs.Pdf`) |
|--------------------------|----------------------------|
| `PdfMetadata` | `DocumentMetadata` — access via `PdfDocument.Metadata` |
| `PdfMetadata.Title` / `.Author` etc | `DocumentMetadata.DocumentInfo.Title` / `.Author` etc |
| `DevExpress.Pdf.Xmp.XmpMetadata` | `DevExpress.Docs.Pdf.XmpMetadata` — access via `DocumentMetadata.Xmp` |
| `DevExpress.Pdf.Xmp.XmpSimpleNode` | `XmpValue` |
| `DevExpress.Pdf.Xmp.XmpArray` | `XmpBoolArray` / `XmpDateArray` / `XmpStringArray` |

## File Attachments

Embed files inside a PDF using the `Attachment` class:

```csharp
using DevExpress.Docs.Pdf;
using System.IO;

using (PdfDocument document = new PdfDocument(File.OpenRead("input.pdf"))) {

    // Create an attachment from a file
    byte[] excelData = File.ReadAllBytes("data.xlsx");
    var attachment = new Attachment("data.xlsx", excelData);
    attachment.Description = "Source spreadsheet";
    attachment.MimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    attachment.Relationship = AssociatedFileRelationship.Supplement;

    document.Attachments.Add(attachment);

    document.Save(File.Create("output.pdf"));
}
```

```vb
Imports DevExpress.Docs.Pdf
Imports System.IO

Using document As New PdfDocument(File.OpenRead("input.pdf"))
    Dim excelData As Byte() = File.ReadAllBytes("data.xlsx")
    Dim attachment As New Attachment("data.xlsx", excelData)
    attachment.Description = "Source spreadsheet"
    attachment.MimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
    attachment.Relationship = AssociatedFileRelationship.Supplement

    document.Attachments.Add(attachment)
    document.Save(File.Create("output.pdf"))
End Using
```

### Attachment Properties

| Property | Type | Description |
|----------|------|-------------|
| `Data` | `byte[]` | Attachment file bytes |
| `FileName` | `string` | File name shown in PDF viewer |
| `Description` | `string` | Human-readable description |
| `MimeType` | `string` | MIME type (e.g., `"application/pdf"`) |
| `CreationDate` | `DateTimeOffset?` | When the file was created |
| `ModificationDate` | `DateTimeOffset?` | When the file was last modified |
| `Relationship` | `AssociatedFileRelationship` | File association type (`Supplement`, `Source`, `Data`, `Alternative`, `Unspecified`) |

## ZUGFeRD / Factur-X Electronic Invoicing

Attach a structured invoice XML to create a hybrid PDF/A-3 invoice (ZUGFeRD/Factur-X):

```csharp
using DevExpress.Docs.Pdf;
using System.IO;

using (PdfDocument document = new PdfDocument(File.OpenRead("invoice.pdf"))) {
    // Attach ZUGFeRD XML — simplest overload (auto-detects version/conformance)
    byte[] invoiceXml = File.ReadAllBytes("invoice.xml");
    document.AttachZugferdInvoice(invoiceXml);

    document.Save(File.Create("zugferd_invoice.pdf"));
}
```

**Overloads:**
- `AttachZugferdInvoice(byte[] invoiceData)` — attach from byte array (auto-detect)
- `AttachZugferdInvoice(Stream invoiceStream)` — attach from stream (auto-detect)
- `AttachZugferdInvoice(Stream, ZugferdVersion, ZugferdConformanceLevel)` — explicit version/conformance

> **Note:** `AttachZugferdInvoice` does not convert the document to PDF/A-3b. Ensure the source document is PDF/A-3b compliant before attaching the invoice.

## Key API

| Member | Description |
|--------|-------------|
| `PdfDocument.Metadata` | `DocumentMetadata` — PDF metadata container |
| `DocumentMetadata.DocumentInfo` | `DocumentInfo` — standard PDF doc properties (Title, Author, Subject, Keywords) |
| `DocumentMetadata.Xmp` | `XmpMetadata` — XMP metadata object |
| `DocumentMetadata.Synchronize(MetadataSyncMode)` | Synchronize between DocumentInfo and XMP (`InfoToXmp`, `XmpToInfo`, `Auto`) |
| `PdfDocument.Attachments` | `AttachmentCollection` — add/remove file attachments |
| `Attachment(string fileName, byte[] data)` | Constructor |
| `Attachment.Description` | Human-readable description |
| `Attachment.Data` | File content as byte array |
| `Attachment.FileName` | Attachment file name |
| `Attachment.MimeType` | MIME type string |
| `Attachment.Relationship` | `AssociatedFileRelationship` enum |
| `PdfDocument.AttachZugferdInvoice(byte[])` | Attach a ZUGFeRD/Factur-X XML invoice |

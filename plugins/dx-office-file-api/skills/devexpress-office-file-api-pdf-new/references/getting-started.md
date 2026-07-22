# Getting Started with DevExpress New PDF Document API (CTP)

> **CTP Warning**: `DevExpress.Docs.Pdf` is a Community Technology Preview. Do not use in mission-critical production applications.

## System Requirements

- .NET 8.0 / 9.0 / 10.0+ or .NET Framework 4.7.2+
- Visual Studio 2022+ or JetBrains Rider
- A valid DevExpress license

## Installation

### Step 1: Install the Package

**.NET CLI:**
```bash
dotnet add package DevExpress.Docs.Pdf
```

**Package Manager Console:**
```
Install-Package DevExpress.Docs.Pdf
```

### .NET Framework DLL References (alternative to NuGet)

- `DevExpress.Docs.Pdf.v26.1.dll`
- `DevExpress.Data.v26.1.dll`
- `DevExpress.Drawing.v26.1.dll`
- `DevExpress.Printing.v26.1.Core.dll`
- `DevExpress.Office.v26.1.Core.dll`
- `DevExpress.Docs.Core.v26.1.dll`

## Key Namespaces

| Namespace | Contents |
|-----------|----------|
| `DevExpress.Docs.Pdf` | `PdfDocument`, `Page`, `TextFragment`, `ImageFragment`, `PathFragment`, `TextFont`, `TextFontStyle`, all annotations and fields |
| `DevExpress.Drawing` | `DXImage`, `DXFont`, `DXPaperKind` |
| `DevExpress.Drawing.Printing` | `DXPaperKind`, `DXPrinterSettings` |
| `DevExpress.Docs.Pdf.Printing` | `PrintOptions` |

## Coordinate System

| Parameter | Value |
|-----------|-------|
| Origin | Bottom-left corner of page |
| Units | Points (72 pt = 1 inch) |
| Y axis | Increases upward |
| A4 size | 595.28 × 841.89 pts |
| Letter size | 612 × 792 pts |

**Practical tip**: For A4, `Location = new PointF(50, 770)` places content ~1 inch from the top.

## Create Your First PDF Document

```csharp
using DevExpress.Docs.Pdf;
using DevExpress.Drawing;
using DevExpress.Drawing.Printing;
using System.Drawing;
using System.IO;

namespace DxPdfGetStarted;

public class Program {
    public static void Main(string[] _) {
        using (PdfDocument pdfDocument = new PdfDocument()) {
            // Add an A4 page
            Page page = pdfDocument.Pages.Add(DXPaperKind.A4);

            // Add a title
            page.AddFragment(new TextFragment {
                Text = "Quarterly Sales Report",
                Location = new PointF(50, 770),
                Font = new TextFont("Arial", TextFontStyle.Bold),
                FontSize = 24
            });

            // Add body text
            page.AddFragment(new TextFragment {
                Text = "This report summarizes sales data for Q1 2026.",
                Location = new PointF(50, 730),
                Font = new TextFont("Arial"),
                FontSize = 12
            });

            // Add an image
            using (var logoStream = File.OpenRead("logo.png")) {
                DXImage logo = DXImage.FromStream(logoStream);
                page.AddImageFragment(new ImageFragment(logo) {
                    Location = new PointF(50, 600),
                    Size = new SizeF(200, 100)
                });
            }

            // Add a second page
            Page page2 = pdfDocument.Pages.Add(DXPaperKind.A4);
            page2.AddFragment(new TextFragment {
                Text = "North America: $1,250,000",
                Location = new PointF(50, 730),
                Font = new TextFont("Arial"),
                FontSize = 12
            });

            // Save
            using (FileStream fs = File.Create("SalesReport.pdf"))
                pdfDocument.Save(fs);
        }
    }
}
```

### VB.NET equivalent

```vb
Imports DevExpress.Docs.Pdf
Imports DevExpress.Drawing
Imports DevExpress.Drawing.Printing
Imports System.Drawing
Imports System.IO

Namespace DxPdfGetStarted
    Public Class Program
        Public Shared Sub Main(ByVal underscore() As String)
            Using pdfDocument As New PdfDocument()
                Dim page As Page = pdfDocument.Pages.Add(DXPaperKind.A4)

                page.AddFragment(New TextFragment With {
                    .Text = "Quarterly Sales Report",
                    .Location = New PointF(50, 770),
                    .Font = New TextFont("Arial", TextFontStyle.Bold),
                    .FontSize = 24
                })

                Using fs As FileStream = File.Create("SalesReport.pdf")
                    pdfDocument.Save(fs)
                End Using
            End Using
        End Sub
    End Class
End Namespace
```

## Load an Existing PDF Document

```csharp
using DevExpress.Docs.Pdf;
using System.IO;

// Open for reading and writing
using (PdfDocument doc = new PdfDocument(
    new FileStream("input.pdf", FileMode.Open, FileAccess.ReadWrite))) {
    // Access pages
    int pageCount = doc.Pages.Count;
    Page firstPage = doc.Pages[0];
    // ... modify document ...
    doc.Save(new FileStream("output.pdf", FileMode.Create));
}
```

## Open a Password-Protected Document

```csharp
using DevExpress.Docs.Pdf;

using (PdfDocument doc = new PdfDocument(
    File.OpenRead("protected.pdf"),
    new LoadOptions { Password = "userPassword" })) {
    // Work with the document
    doc.RemoveEncryption();
    doc.Save(new FileStream("unprotected.pdf", FileMode.Create));
}
```

## What to Learn Next

- [Add content (text, images, shapes)](add-content.md) — populate your pages
- [Organize pages](organize-pages.md) — manage the page collection
- [Interactive forms](form-fields.md) — create fillable PDFs
- [Security](security.md) — encrypt and set permissions
- [Structure tree](structure-tree.md) — build tagged/accessible PDFs
- [Migration](migration.md) — migrate from `PdfDocumentProcessor`

# Organize Pages in PDF Documents

> **CTP Warning**: `DevExpress.Docs.Pdf` is a Community Technology Preview. Do not use in mission-critical production applications.

## When you need to:
- Add new pages (at end or at a specific position)
- Clone, reorder, or copy pages within a document
- Copy pages from one document to another
- Merge PDF documents by appending pages
- Rotate or resize pages
- Rotate, scale, or offset page content
- Remove pages

## Add Pages

```csharp
using DevExpress.Docs.Pdf;
using DevExpress.Drawing.Printing;
using System.IO;

using (PdfDocument pdfDocument = new PdfDocument(
    new FileStream("Document.pdf", FileMode.Open, FileAccess.ReadWrite))) {
    // Add a page at the end
    pdfDocument.Pages.Add(DXPaperKind.A4);
    // Insert a page at index 1
    pdfDocument.Pages.Insert(1, new Page(DXPaperKind.Letter));

    pdfDocument.Save(new FileStream("Document_upd.pdf", FileMode.Create, FileAccess.Write));
}
```

```vb
Imports DevExpress.Docs.Pdf
Imports DevExpress.Drawing.Printing
Imports System.IO

Using pdfDocument As New PdfDocument(
    New FileStream("Document.pdf", FileMode.Open, FileAccess.ReadWrite))
    pdfDocument.Pages.Add(DXPaperKind.A4)
    pdfDocument.Pages.Insert(1, New Page(DXPaperKind.Letter))
    pdfDocument.Save(New FileStream("Document_upd.pdf", FileMode.Create, FileAccess.Write))
End Using
```

## Clone and Reorder Pages

### Clone a Page

```csharp
using (PdfDocument pdfDocument = new PdfDocument(
    new FileStream("Document.pdf", FileMode.OpenOrCreate, FileAccess.ReadWrite))) {
    // Clone page 0 and append the copy
    Page extracted = pdfDocument.Pages[0].Clone();
    pdfDocument.Pages.Add(extracted);
    pdfDocument.Save(new FileStream("Result.pdf", FileMode.Create, FileAccess.Write));
}
```

### Reorder — Move First Page to End

```csharp
using (PdfDocument pdfDocument = new PdfDocument(
    new FileStream("Document.pdf", FileMode.OpenOrCreate, FileAccess.ReadWrite))) {
    Page firstPage = pdfDocument.Pages[0];
    pdfDocument.Pages.Add(firstPage);    // moves it (removes from old position)
    pdfDocument.Save(new FileStream("Result.pdf", FileMode.Create, FileAccess.Write));
}
```

## Copy Pages from Another Document

```csharp
using DevExpress.Docs.Pdf;
using System.IO;

using (PdfDocument source = new PdfDocument(
    new FileStream("Source.pdf", FileMode.OpenOrCreate, FileAccess.ReadWrite))) {
    using (PdfDocument target = new PdfDocument(
        new FileStream("Target.pdf", FileMode.OpenOrCreate, FileAccess.ReadWrite))) {
        // Copy page 0 from source into target at index 3
        target.Pages.Insert(3, source.Pages[0].Clone());
        target.Save(new FileStream("Result.pdf", FileMode.Create, FileAccess.Write));
    }
}
```

```vb
Imports DevExpress.Docs.Pdf
Imports System.IO

Using source As New PdfDocument(New FileStream("Source.pdf", FileMode.OpenOrCreate, FileAccess.ReadWrite))
    Using target As New PdfDocument(New FileStream("Target.pdf", FileMode.OpenOrCreate, FileAccess.ReadWrite))
        target.Pages.Insert(3, source.Pages(0).Clone())
        target.Save(New FileStream("Result.pdf", FileMode.Create, FileAccess.Write))
    End Using
End Using
```

## Merge Documents

Use `PdfDocument.AppendDocument` to append all pages from another document.

```csharp
using (PdfDocument main = new PdfDocument(File.OpenRead("main.pdf")))
using (PdfDocument appendage = new PdfDocument(File.OpenRead("appendage.pdf"))) {
    main.AppendDocument(appendage);
    main.Save(new FileStream("merged.pdf", FileMode.Create, FileAccess.Write));
}
```

## Rotate and Resize Pages

```csharp
using DevExpress.Docs.Pdf;
using System.Drawing;
using System.IO;

using (PdfDocument pdfDocument = new PdfDocument(
    new FileStream("Document.pdf", FileMode.OpenOrCreate, FileAccess.ReadWrite))) {
    Page page = pdfDocument.Pages[0];
    // Rotate page 90° clockwise
    page.Rotation = PageRotationAngle.Clockwise90;
    // Resize to A4
    page.Resize(
        new RectangleF(0, 0, 595.28f, 841.89f),
        PageContentHorizontalAlignment.Center,
        PageContentVerticalAlignment.Center);
    pdfDocument.Save(new FileStream("Result.pdf", FileMode.Create, FileAccess.Write));
}
```

```vb
Imports DevExpress.Docs.Pdf
Imports System.Drawing
Imports System.IO

Using pdfDocument As New PdfDocument(New FileStream("Document.pdf", FileMode.OpenOrCreate, FileAccess.ReadWrite))
    Dim page As Page = pdfDocument.Pages(0)
    page.Rotation = PageRotationAngle.Clockwise90
    page.Resize(
        New RectangleF(0, 0, 595.28F, 841.89F),
        PageContentHorizontalAlignment.Center,
        PageContentVerticalAlignment.Center)
    pdfDocument.Save(New FileStream("Result.pdf", FileMode.Create, FileAccess.Write))
End Using
```

### PageRotationAngle Values

| Value | Description |
|-------|-------------|
| `None` | No rotation |
| `Clockwise90` | 90° clockwise |
| `Clockwise180` | 180° |
| `Clockwise270` | 270° clockwise |

## Transform Page Content

### Rotate Content

```csharp
// Rotate content 270° around point (300, 300)
page.RotateContent(300, 300, 270);
```

### Scale Content

```csharp
// Scale content to 50% on both axes
page.ScaleContent(0.5, 0.5);
```

### Offset Content

```csharp
// Move content 50 pts right and 50 pts up
page.OffsetContent(50, 50);
```

Full example — rotate content:

```csharp
using (PdfDocument pdfDocument = new PdfDocument(
    new FileStream("Document.pdf", FileMode.OpenOrCreate, FileAccess.ReadWrite))) {
    Page page = pdfDocument.Pages[0];
    page.RotateContent(300, 300, 270);
    pdfDocument.Save(new FileStream("Result.pdf", FileMode.Create, FileAccess.Write));
}
```

## Remove Pages

```csharp
using (PdfDocument pdfDocument = new PdfDocument(
    new FileStream("Document.pdf", FileMode.OpenOrCreate, FileAccess.ReadWrite))) {
    // Remove by index
    pdfDocument.Pages.RemoveAt(0);
    pdfDocument.Save(new FileStream("Result.pdf", FileMode.Create, FileAccess.Write));
}
```

```vb
Using pdfDocument As New PdfDocument(New FileStream("Document.pdf", FileMode.OpenOrCreate, FileAccess.ReadWrite))
    pdfDocument.Pages.RemoveAt(0)
    pdfDocument.Save(New FileStream("Result.pdf", FileMode.Create, FileAccess.Write))
End Using
```

## PageCollection API Reference

| Member | Description |
|--------|-------------|
| `Add(DXPaperKind)` | Append a new blank page |
| `Add(Page)` | Append an existing `Page` object |
| `Insert(int, Page)` | Insert at a specific index |
| `Remove(Page)` | Remove a specific page object |
| `RemoveAt(int)` | Remove by index |
| `Count` | Total page count |
| `this[int]` | Access page by index |

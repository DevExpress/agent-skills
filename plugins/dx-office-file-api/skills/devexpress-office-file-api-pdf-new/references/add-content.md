# Add Content to PDF Pages

> **CTP Warning**: `DevExpress.Docs.Pdf` is a Community Technology Preview. Do not use in mission-critical production applications.

## When you need to:
- Add single-line or formatted text to a PDF page
- Add multiline/wrapped text (paragraph)
- Add images (BMP, JPEG, PNG, EMF, TIFF, SVG)
- Draw shapes and lines using `PathFragment`
- Create a reusable page template (`FormTemplate` / `FormFragment`)

## Coordinate System

| Parameter | Value |
|-----------|-------|
| Origin | Bottom-left corner of page |
| Units | Points (72 pt = 1 inch) |
| Y axis | Increases upward |
| Rotation | Degrees, positive = clockwise |

For A4 (841.89 pt tall): `Location = new PointF(50, 770)` places content ~1 inch from the top.

## Add Text — Single Line

Use `Page.AddFragment(TextFragment)` to add a formatted text line.

```csharp
using DevExpress.Docs.Pdf;
using DevExpress.Drawing;
using DevExpress.Drawing.Printing;
using System.Drawing;
using System.IO;

using (PdfDocument pdfDocument = new PdfDocument()) {
    var page = pdfDocument.Pages.Add(DXPaperKind.A4);
    var text = new TextFragment() {
        Text = "The PDF Document API",
        Location = new PointF(10, 740),
        Font = new TextFont("Courier New"),
    };
    page.AddFragment(text);
    pdfDocument.Save(new FileStream("Result.pdf", FileMode.Create, FileAccess.Write));
}
```

```vb
Imports DevExpress.Docs.Pdf
Imports DevExpress.Drawing
Imports DevExpress.Drawing.Printing
Imports System.Drawing
Imports System.IO

Using pdfDocument As New PdfDocument()
    Dim page = pdfDocument.Pages.Add(DXPaperKind.A4)
    Dim text As New TextFragment() With {
        .Text = "The PDF Document API",
        .Location = New PointF(10, 740),
        .Font = New TextFont("Courier New")
    }
    page.AddFragment(text)
    pdfDocument.Save(New FileStream("Result.pdf", FileMode.Create, FileAccess.Write))
End Using
```

### TextFragment Key Properties

| Property | Type | Description |
|----------|------|-------------|
| `Text` | `string` | Text content |
| `Location` | `PointF` | Bottom-left anchor in PDF coordinates |
| `Font` | `TextFont` | Font family and style: `new TextFont("Arial", TextFontStyle.Bold)` |
| `FontSize` | `float` | Override font size |
| `Bold` | `bool` | Bold formatting |
| `ForegroundFill` | `Fill` | Fill with opacity: `new SolidFill(PdfColor.Red, 0.2f)` |
| `RotationAngle` | `double` | Clockwise rotation in degrees |

## Add Text — Multiline (ParagraphFragment)

Use `ParagraphFragment` for wrapped text with alignment control.

```csharp
using DevExpress.Docs.Pdf;
using DevExpress.Drawing;
using DevExpress.Drawing.Printing;
using System.Drawing;
using System.IO;

using (PdfDocument pdfDocument = new PdfDocument()) {
    var page = pdfDocument.Pages.Add(DXPaperKind.A4);
    var multiLine = new ParagraphFragment() {
        Text = "The PDF Document API is a non-visual .NET library that allows you to " +
               "generate, convert, merge, split, edit, password-protect, and digitally " +
               "sign PDF files.",
        Location = new PointF(10, 740),
        Font = new DXFont("Courier New", 11, DXFontStyle.Regular),
        Width = 500,
        Height = 500,
        StringFormat = new DXStringFormat() {
            Alignment = DXStringAlignment.Center,
            LineAlignment = DXStringAlignment.Near,
        },
    };
    page.Fragments.Add(multiLine);
    pdfDocument.Save(new FileStream("Result.pdf", FileMode.Create, FileAccess.Write));
}
```

```vb
Imports DevExpress.Docs.Pdf
Imports DevExpress.Drawing
Imports DevExpress.Drawing.Printing
Imports System.Drawing
Imports System.IO

Using pdfDocument As New PdfDocument()
    Dim page = pdfDocument.Pages.Add(DXPaperKind.A4)
    Dim multiLine As New ParagraphFragment() With {
        .Text = "The PDF Document API is a non-visual .NET library...",
        .Location = New PointF(10, 740),
        .Font = New DXFont("Courier New", 11, DXFontStyle.Regular),
        .Width = 500,
        .Height = 500,
        .StringFormat = New DXStringFormat() With {
            .Alignment = DXStringAlignment.Center,
            .LineAlignment = DXStringAlignment.Near
        }
    }
    page.Fragments.Add(multiLine)
    pdfDocument.Save(New FileStream("Result.pdf", FileMode.Create, FileAccess.Write))
End Using
```

## Add Images

Use `ImageFragment` to add images. Supported formats: BMP, JPEG, PNG, EMF, EMF+, TIFF, GIF, SVG.

```csharp
using DevExpress.Docs.Pdf;
using DevExpress.Drawing;
using DevExpress.Drawing.Printing;
using System.Drawing;
using System.IO;

using (PdfDocument pdfDocument = new PdfDocument()) {
    var page = pdfDocument.Pages.Add(DXPaperKind.A4);
    var image = new ImageFragment {
        Image = DXImage.FromStream(new FileStream("logo.png", FileMode.Open, FileAccess.Read)),
        Location = new PointF(100, 100),
        SkewX = 20,
    };
    page.Fragments.Add(image);
    pdfDocument.Save(new FileStream("Result.pdf", FileMode.Create, FileAccess.Write));
}
```

```vb
Imports DevExpress.Docs.Pdf
Imports DevExpress.Drawing
Imports DevExpress.Drawing.Printing
Imports System.Drawing
Imports System.IO

Using pdfDocument As New PdfDocument()
    Dim page = pdfDocument.Pages.Add(DXPaperKind.A4)
    Dim image As New ImageFragment With {
        .Image = DXImage.FromStream(New FileStream("logo.png", FileMode.Open, FileAccess.Read)),
        .Location = New PointF(100, 100),
        .SkewX = 20
    }
    page.Fragments.Add(image)
    pdfDocument.Save(New FileStream("Result.pdf", FileMode.Create, FileAccess.Write))
End Using
```

**Alternative**: `DXImage.FromFile("logo.png")` or `new ImageFragment(dxImage) { Location = ..., Size = new SizeF(200, 100) }`.

## Add Shapes (PathFragment)

### Rectangle

```csharp
var rectangle = PathFragment.Rectangle(new RectangleF(100, 100, 200, 200));
rectangle.Fill = Fill.CreateSolid(PdfColor.Red);
rectangle.Outline = Outline.Create(Fill.CreateSolid(PdfColor.Blue), 5);
page.AddFragment(rectangle);
```

### Custom Shapes (Triangle + Bézier Curve)

```csharp
using DevExpress.Docs.Pdf;
using DevExpress.Drawing;
using DevExpress.Drawing.Printing;
using System.Drawing;
using System.IO;

using (PdfDocument pdfDocument = new PdfDocument()) {
    var page = pdfDocument.Pages.Add(DXPaperKind.A4);

    // Triangle
    GraphicsPath triangle = new GraphicsPath(new PointF(410, 490));
    triangle.AppendLineSegment(new PointF(480, 460));
    triangle.AppendLineSegment(new PointF(550, 490));
    triangle.Closed = true;

    page.Fragments.Add(new PathFragment() {
        Paths = { triangle },
        Fill = Fill.CreateSolid(PdfColor.LightGray),
        Outline = Outline.Create(Fill.CreateSolid(PdfColor.Black), 1),
        Action = PathShapeAction.FillAndStroke
    });

    // Bézier curve
    GraphicsPath bezier = new GraphicsPath(new PointF(210, 375));
    bezier.AppendBezierSegment(new PointF(210, 320), new PointF(350, 320), new PointF(350, 375));
    bezier.AppendBezierSegment(new PointF(350, 430), new PointF(210, 430), new PointF(210, 375));

    page.Fragments.Add(new PathFragment() {
        Paths = { bezier },
        Fill = Fill.CreateSolid(PdfColor.Red),
        Outline = Outline.Create(Fill.CreateSolid(PdfColor.Black), 1),
        Action = PathShapeAction.FillAndStroke
    });

    pdfDocument.Save(new FileStream("Result.pdf", FileMode.Create, FileAccess.Write));
}
```

### PathFragment Key Members

| Member | Description |
|--------|-------------|
| `Paths` | Collection of `GraphicsPath` objects |
| `Fill` | Fill: `Fill.CreateSolid(PdfColor.Red)` |
| `Outline` | Outline: `Outline.Create(Fill.CreateSolid(PdfColor.Black), 1)` |
| `Action` | `PathShapeAction.Fill`, `Stroke`, or `FillAndStroke` |
| `PathFragment.Rectangle(RectangleF)` | Static factory for rectangles |

### GraphicsPath Methods

| Method | Description |
|--------|-------------|
| `new GraphicsPath(PointF startPoint)` | Create path with start point |
| `AppendLineSegment(PointF)` | Add a straight line segment |
| `AppendBezierSegment(PointF, PointF, PointF)` | Add a Bézier curve |
| `Closed` | Close the path (connect end to start) |

## Add Reusable Templates (FormTemplate / FormFragment)

Create a `FormTemplate` once, then place it on multiple pages using `FormFragment`.

```csharp
using DevExpress.Docs.Pdf;
using DevExpress.Drawing;
using DevExpress.Drawing.Printing;
using System;
using System.Drawing;
using System.IO;

using (PdfDocument pdfDocument = new PdfDocument()) {
    var page = pdfDocument.Pages.Add(DXPaperKind.A4);

    // Create template
    var headerTemplate = new FormTemplate();
    headerTemplate.Bounds = new RectangleF(0, 0, (float)page.Width, 150);
    headerTemplate.AddTextFragment("Company Name: DevExpress", 50, 160);
    headerTemplate.AddTextFragment("Invoice Date: " + DateTime.Now.ToShortDateString(), 50, 120);

    // Add separator line
    GraphicsPath headerLine = new GraphicsPath(new PointF(0, 110));
    headerLine.AppendLineSegment(new PointF((float)headerTemplate.Bounds.Width, 110));
    headerTemplate.Fragments.Add(new PathFragment {
        Fill = Fill.CreateSolid(PdfColor.LightGray),
        Paths = new GraphicsPath[] { headerLine }
    });

    // Place template on page 1
    var formFragment1 = new FormFragment(headerTemplate);
    formFragment1.Location = new PointF(50, (float)page.Height - 160);
    page.AddFragment(formFragment1);

    // Place same template on page 2 at a different position
    var page2 = pdfDocument.Pages.Add(DXPaperKind.A4);
    var formFragment2 = new FormFragment(headerTemplate);
    formFragment2.Location = new PointF(100, (float)page2.Height - 200);
    page2.AddFragment(formFragment2);

    pdfDocument.Save(new FileStream("Result.pdf", FileMode.Create, FileAccess.Write));
}
```

```vb
Imports DevExpress.Docs.Pdf
Imports DevExpress.Drawing
Imports DevExpress.Drawing.Printing
Imports System
Imports System.Drawing
Imports System.IO

Using pdfDocument As New PdfDocument()
    Dim page = pdfDocument.Pages.Add(DXPaperKind.A4)

    Dim headerTemplate As New FormTemplate()
    headerTemplate.Bounds = New RectangleF(0, 0, CSng(page.Width), 150)
    headerTemplate.AddTextFragment("Company Name: DevExpress", 50, 160)
    headerTemplate.AddTextFragment("Invoice Date: " & DateTime.Now.ToShortDateString(), 50, 120)

    Dim headerLine As New GraphicsPath(New PointF(0, 110))
    headerLine.AppendLineSegment(New PointF(CSng(headerTemplate.Bounds.Width), 110))
    headerTemplate.Fragments.Add(New PathFragment With {
        .Fill = Fill.CreateSolid(PdfColor.LightGray),
        .Paths = {headerLine}
    })

    Dim formFragment1 As New FormFragment(headerTemplate)
    formFragment1.Location = New PointF(50, CSng(page.Height) - 160)
    page.AddFragment(formFragment1)

    Dim page2 = pdfDocument.Pages.Add(DXPaperKind.A4)
    Dim formFragment2 As New FormFragment(headerTemplate)
    formFragment2.Location = New PointF(100, CSng(page2.Height) - 200)
    page2.AddFragment(formFragment2)

    pdfDocument.Save(New FileStream("Result.pdf", FileMode.Create, FileAccess.Write))
End Using
```

> **Tip**: The template content is clipped to `FormTemplate.Bounds`. Ensure all content fits within the defined bounds.

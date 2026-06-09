# PDF Graphics API — DevExpress PDF Document API

The `PdfGraphics` class provides a drawing surface for placing text, images, shapes, and links on PDF pages. It works with both new pages (via `RenderNewPage`) and existing pages (via `AddToPageForeground` / `AddToPageBackground`).

## When to Use This Reference

Use this when you need to:
- Draw text strings with specific fonts, colors, and formatting
- Measure text before drawing (to calculate layout)
- Draw images (bitmaps) on a page
- Draw shapes: rectangles, ellipses, lines, polygons, Bezier curves, custom paths
- Add hyperlinks (URI or page destination) to a page area
- Apply geometric transforms: scale, rotate, translate
- Save and restore the graphics state
- Stamp graphics onto an existing page foreground or background

## Key Classes and Types

| Class | Purpose |
|-------|---------|
| `PdfGraphics` | Main drawing surface — obtained from `PdfDocumentProcessor` |
| `DXFont` | Cross-platform font descriptor (`DevExpress.Drawing` namespace) |
| `DXFontStyle` | Enum: `Regular`, `Bold`, `Italic`, `Underline`, `Strikeout` |
| `DXSolidBrush` | Solid color fill brush |
| `DXPen` | Pen for stroke drawing (color + width) |
| `DXBrushes` | Static color brushes (`DXBrushes.Black`, `DXBrushes.Red`, etc.) |
| `PdfStringFormat` | Text alignment and line spacing: `GenericDefault`, `GenericTypographic` |
| `PdfStringAlignment` | Enum: `Near`, `Center`, `Far` |

## Drawing Text

### Draw at a Point

```csharp
using DevExpress.Drawing;
using DevExpress.Pdf;
using System.Drawing;

using (DXFont font = new DXFont("Arial", 14, DXFontStyle.Bold))
{
    graph.DrawString("Hello World", font,
        new DXSolidBrush(Color.Black), 50f, 100f);
}
```

### Draw in a Rectangle with Alignment

```csharp
using (DXFont font = new DXFont("Arial", 12))
{
    var rect = new RectangleF(50, 50, 400, 200);
    PdfStringFormat format = PdfStringFormat.GenericTypographic;
    format.Alignment = PdfStringAlignment.Center;
    format.LineAlignment = PdfStringAlignment.Center;

    graph.DrawString("Centered text in a box", font,
        new DXSolidBrush(Color.DarkSlateBlue), rect, format);
}
```

### Measure Text Before Drawing

```csharp
using (DXFont font = new DXFont("Arial", 12))
{
    SizeF textSize = graph.MeasureString("My text", font);
    // Use textSize.Width and textSize.Height for layout calculations
    float x = (pageWidth - textSize.Width) / 2f; // center horizontally
    graph.DrawString("My text", font, new DXSolidBrush(Color.Black), x, 50f);
}
```

## Drawing Images

```csharp
using System.Drawing;

// Load an image from disk
using (Image img = Image.FromFile("logo.png"))
{
    // Draw image into a specific rectangle on the page
    graph.DrawImage(img, new RectangleF(50, 50, 200, 100));
}
```

## Drawing Shapes

### Rectangle

```csharp
// Draw outline
graph.DrawRectangle(new DXPen(Color.Navy, 2), new RectangleF(50, 50, 300, 150));

// Fill solid
graph.FillRectangle(new DXSolidBrush(Color.LightBlue), new RectangleF(50, 50, 300, 150));
```

### Ellipse / Circle

```csharp
// Draw outline
graph.DrawEllipse(new DXPen(Color.Red, 1.5f), new RectangleF(100, 100, 150, 100));

// Fill solid
graph.FillEllipse(new DXSolidBrush(Color.LightGreen), new RectangleF(100, 100, 150, 100));
```

### Lines

```csharp
// Single line
graph.DrawLine(new DXPen(Color.Black, 1), 50f, 200f, 400f, 200f);

// Multiple connected line segments
var points = new PointF[] {
    new PointF(50, 50),
    new PointF(150, 150),
    new PointF(250, 80)
};
graph.DrawLines(new DXPen(Color.DarkGreen, 1), points);
```

### Polygon

```csharp
var vertices = new PointF[] {
    new PointF(200, 50),
    new PointF(300, 150),
    new PointF(100, 150)
};
graph.DrawPolygon(new DXPen(Color.Purple, 2), vertices);
graph.FillPolygon(new DXSolidBrush(Color.Lavender), vertices);
```

### Bezier Curves

```csharp
// Four control points define one cubic Bezier segment
graph.DrawBezier(new DXPen(Color.DarkRed, 1.5f),
    new PointF(50, 200),    // start
    new PointF(150, 50),    // control 1
    new PointF(250, 350),   // control 2
    new PointF(350, 200));  // end
```

### Shape Summary Table

| Shape | Draw (outline) | Fill |
|-------|---------------|------|
| Rectangle | `DrawRectangle(pen, rect)` | `FillRectangle(brush, rect)` |
| Ellipse | `DrawEllipse(pen, rect)` | `FillEllipse(brush, rect)` |
| Line | `DrawLine(pen, x1, y1, x2, y2)` | — |
| Multiple lines | `DrawLines(pen, points[])` | — |
| Polygon | `DrawPolygon(pen, points[])` | `FillPolygon(brush, points[])` |
| Bezier curve | `DrawBezier(pen, p1, p2, p3, p4)` | — |
| Multiple Beziers | `DrawBeziers(pen, points[])` | — |
| Path | `DrawPath(pen, path)` | `FillPath(brush, path)` |

## Adding Hyperlinks

```csharp
using System;

// Link to a URI
graph.AddLinkToUri(
    new RectangleF(50, 300, 200, 20),
    new Uri("https://www.devexpress.com"));

// Link to a specific page and position (page number is 1-based)
graph.AddLinkToPage(
    new RectangleF(50, 340, 150, 20),  // clickable area
    pageNumber: 2,                      // destination page
    x: 0, y: 0);                       // destination position on that page
```

## Transforms

Transforms affect all subsequent drawing operations. Always save and restore state when applying temporary transforms.

```csharp
// Save the current state
graph.SaveGraphicsState();

// Apply transforms
graph.TranslateTransform(200f, 400f);   // move origin
graph.RotateTransform(45f);             // rotate 45 degrees clockwise
graph.ScaleTransform(1.5f, 1.5f);      // scale 150%

// Draw in the transformed space
using (DXFont font = new DXFont("Arial", 12))
    graph.DrawString("Rotated Text", font, new DXSolidBrush(Color.Red), 0f, 0f);

// Restore the original state
graph.RestoreGraphicsState();
```

## Clipping

```csharp
graph.SaveGraphicsState();

// Only draw within the clip rectangle
graph.IntersectClip(new RectangleF(100, 100, 200, 150));

// Content outside the clip rect will be invisible
graph.FillRectangle(new DXSolidBrush(Color.LightYellow), new RectangleF(0, 0, 500, 400));

graph.RestoreGraphicsState();
```

## Adding Graphics to an Existing Page

Use `AddToPageForeground` or `AddToPageBackground` to stamp graphics onto an existing PDF page (watermarks, stamps, overlays):

```csharp
using (PdfDocumentProcessor processor = new PdfDocumentProcessor())
{
    processor.LoadDocument("input.pdf");
    PdfPage firstPage = processor.Document.Pages[0];

    using (PdfGraphics graphics = processor.CreateGraphicsPageSystem())
    {
        // Add a "DRAFT" watermark
        graphics.SaveGraphicsState();
        graphics.TranslateTransform(300f, 400f);
        graphics.RotateTransform(45f);
        using (DXFont font = new DXFont("Arial Black", 48, DXFontStyle.Bold))
        using (DXSolidBrush brush = new DXSolidBrush(Color.FromArgb(80, Color.Red)))
            graphics.DrawString("DRAFT", font, brush, -100f, -25f);
        graphics.RestoreGraphicsState();

        // Stamp onto the foreground (on top of existing content)
        graphics.AddToPageForeground(firstPage);
        // -- OR --
        // graphics.AddToPageBackground(firstPage);  // behind existing content
    }

    processor.SaveDocument("output.pdf");
}
```

## Configuration Options

| Property/Method | Description |
|----------------|-------------|
| `PdfStringFormat.Alignment` | Horizontal text alignment within bounding rect |
| `PdfStringFormat.LineAlignment` | Vertical text alignment within bounding rect |
| `DXFont(name, size, style)` | Font name, point size, and style flags |
| `DXPen(color, width)` | Stroke color and width |

## Troubleshooting

- **Text is invisible**: Ensure the brush color contrasts with the background. Check that the Y coordinate is within the visible page area.
- **DrawString uses page-system Y (origin = bottom-left)**: Positive Y moves text toward the top. A Y of 0 is the bottom edge of the page.
- **Graphics state lost after `RenderNewPage`**: Graphics state is local to each `PdfGraphics` instance. Create a fresh instance for each page.
- **Image appears distorted**: Pass the correct aspect-ratio `RectangleF` to `DrawImage`, or calculate width/height from the original image dimensions.
- **`DXFont` not found**: Ensure you have `using DevExpress.Drawing;` and the `DevExpress.Document.Processor` NuGet package installed.

# Shapes and Images — Word Processing Document API

## When to Use This Reference

Use this when you need to insert shapes, pictures, or text boxes into a Word document, group shapes, add charts, or control how drawing objects are positioned and sized.

## Access Drawing Objects

All drawing objects (shapes, pictures, text boxes, charts) live in `Document.Shapes` (`ShapeCollection`). Use `Shape.Type` to distinguish between them.

```csharp
using DevExpress.XtraRichEdit.API.Native;

foreach (Shape s in doc.Shapes)
{
    Console.WriteLine($"Type: {s.Type}, Name: {s.Name}");
}
```

## Insert a Shape

Use `ShapeCollection.InsertShape` with a `ShapeGeometryPreset` value. Pass a `RectangleF` to set the initial position and size (units match `Document.Unit`).

```csharp
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;
using System.Drawing;

using (var server = new RichEditDocumentServer())
{
    Document document = server.Document;
    document.Unit = DevExpress.Office.DocumentUnit.Inch;

    // Insert a rectangle at (1.5", 1") with size 2" x 1.5"
    Shape rectangle = document.Shapes.InsertShape(
        document.Range.Start,
        ShapeGeometryPreset.Rectangle,
        new RectangleF(1.5f, 1f, 2f, 1.5f));

    // Fill and border
    rectangle.Fill.SetSolidFill(Color.FromArgb(0xFF, 0xEE, 0xAD));
    rectangle.Line.Color = Color.FromArgb(0x4D, 0x64, 0x8D);
    rectangle.Line.Thickness = 6;

    server.SaveDocument("output.docx", DocumentFormat.Docx);
}
```

## Insert a Picture

```csharp
using DevExpress.XtraRichEdit.API.Native;
using System.Drawing;

// Insert a picture from a file
Shape picture = document.Shapes.InsertPicture(
    document.Range.Start,
    DocumentImageSource.FromFile("Dog.png"));

// Optional: round the corners
picture.PictureFormat.Preset = ShapeGeometryPreset.RoundedRectangle;
picture.Line.Color = Color.Black;
picture.Line.Thickness = 3;

// Align within the page
picture.VerticalAlignment = ShapeVerticalAlignment.Top;
picture.HorizontalAlignment = ShapeHorizontalAlignment.Center;
```

Scale all pictures in a document:

```csharp
foreach (Shape s in document.Shapes)
{
    if (s.Type == ShapeType.Picture)
    {
        s.ScaleX = 0.8f;
        s.ScaleY = 0.8f;
    }
}
```

## Insert a Text Box

```csharp
using DevExpress.XtraRichEdit.API.Native;
using System.Drawing;

document.Unit = DevExpress.Office.DocumentUnit.Inch;

// Create a text box at (1.5", 1") with size 1.5" x 0.5"
Shape myTextBox = document.Shapes.InsertTextBox(
    document.Range.Start,
    new RectangleF(1.5f, 1f, 1.5f, 0.5f));

myTextBox.Fill.Color = Color.WhiteSmoke;
myTextBox.Line.Color = Color.Black;
myTextBox.Line.Thickness = 1;

// Access and populate the text box content
SubDocument textBoxDocument = myTextBox.ShapeFormat.TextBox.Document;
textBoxDocument.AppendText("Text box");
CharacterProperties cp = textBoxDocument.BeginUpdateCharacters(textBoxDocument.Range.Start, 4);
cp.ForeColor = Color.Orange;
cp.FontSize = 24;
textBoxDocument.EndUpdateCharacters(cp);
```

Identify a text box programmatically: `s.Type == ShapeType.Shape && s.ShapeFormat.HasText == true`.

## Group Shapes

Use `ShapeCollection.InsertGroup` to create a new group. Add shapes to the group via `Shape.GroupItems`. Note: you cannot group shapes that already exist in the document — build the group from scratch.

```csharp
// Ungroup an existing group
Shape group = document.Shapes[0]; // assume it's a group
group.GroupItems.Ungroup();
```

## Insert a Chart

Charts require activating cross-platform chart support before creating a `RichEditDocumentServer`. Add NuGet packages for `DevExpress.Spreadsheet.Core` and charts assemblies.

```csharp
using DevExpress.Office.Services;
using DevExpress.XtraSpreadsheet.Services;
using DevExpress.XtraRichEdit.API.Native;
using DevExpress.Spreadsheet.Charts;
using DevExpress.Spreadsheet;

// Call ONCE before creating RichEditDocumentServer
OfficeCharts.Instance.ActivateCrossPlatformCharts();

using (var server = new RichEditDocumentServer())
{
    Document document = server.Document;
    document.Unit = DevExpress.Office.DocumentUnit.Inch;

    Shape chartShape = document.Shapes.InsertChart(
        document.Range.Start,
        DevExpress.XtraRichEdit.API.Native.ChartType.ColumnClustered);

    chartShape.Size = new System.Drawing.SizeF(6, 4);

    // Populate the embedded worksheet with data
    Worksheet worksheet = (Worksheet)chartShape.ChartFormat.Worksheet;
    worksheet["B2"].Value = "Category";
    worksheet["C2"].Value = "Value";
    worksheet["B3"].Value = "Alpha"; worksheet["C3"].Value = 10;
    worksheet["B4"].Value = "Beta";  worksheet["C4"].Value = 20;

    ChartObject chart = (ChartObject)chartShape.ChartFormat.Chart;
    chart.SelectData(worksheet["B2:C4"]);
    chart.Title.Visible = true;
    chart.Title.SetValue("Sample Chart");

    server.SaveDocument("output.docx", DocumentFormat.Docx);
}
```

## Position and Size Shapes

| Property | Description |
|---------|-------------|
| `Shape.HorizontalAlignment` | Align horizontally relative to `RelativeHorizontalPosition` |
| `Shape.VerticalAlignment` | Align vertically relative to `RelativeVerticalPosition` |
| `Shape.OffsetX` / `Shape.OffsetY` | Absolute offset (active when alignment is `None`) |
| `Shape.Width` / `Shape.Height` | Absolute size in current `Document.Unit` |
| `Shape.ScaleX` / `Shape.ScaleY` | Scale relative to original size |
| `Shape.RotationAngle` | Rotation in degrees |
| `Shape.LockAspectRatio` | Preserve proportions on resize |
| `Shape.TextWrapping` | `TextWrappingType.Square`, `Inline`, `InFrontOfText`, etc. |
| `Shape.AltText` | Accessibility alternative text |
| `Shape.Decorative` | Mark shape as decorative (excluded from accessible PDF export) |

## Remove Shapes

```csharp
// Remove by reference
document.Shapes.Remove(document.Shapes[0]);

// Remove all charts
for (int i = document.Shapes.Count - 1; i >= 0; i--)
{
    if (document.Shapes[i].Type == ShapeType.Chart)
        document.Shapes.Remove(document.Shapes[i]);
}
```

## Key Classes and Types

| Class/Enum | Purpose |
|-----------|---------|
| `ShapeCollection` | `Document.Shapes` — collection of all drawing objects |
| `Shape` | Single drawing object (shape, picture, text box, chart, group) |
| `ShapeType` | Enum: `Shape`, `Picture`, `Chart`, `OleObject`, `ActiveX` |
| `ShapeGeometryPreset` | Enum of preset geometries (Rectangle, Ellipse, Arrow, etc.) |
| `DocumentImageSource` | Factory for creating image sources (`FromFile`, `FromStream`, `FromUri`) |
| `ShapeFormat` | Accessed via `Shape.ShapeFormat`; exposes `TextBox`, `HasText` |
| `TextBox` | Exposes `Document` (a `SubDocument`) for text box content |
| `ChartFormat` | Accessed via `Shape.ChartFormat`; exposes `Chart` and `Worksheet` |
| `ChartType` (RichEdit) | `DevExpress.XtraRichEdit.API.Native.ChartType` — chart types for `InsertChart` |
| `GroupShapeCollection` | `Shape.GroupItems` — items in a shape group |

# Slides and Shapes — DevExpress Presentation API

This reference covers everything related to slides (add, remove, reorder, copy), shapes (all types, position, fill, outline, text), tables, slide backgrounds, and slide masters and layouts.

## When to Use This Reference

Use this when you need to:
- Add, remove, reorder, clone, or copy slides
- Create slides from slide master layouts
- Add geometric shapes, picture shapes, connectors, or groups to a slide
- Position and size shapes on a slide
- Apply fill (solid, gradient, pattern, picture) and outline styles to shapes
- Add and format text inside shapes (paragraphs, runs, font, color, bullets)
- Create tables and work with rows, columns, cells, styles, and borders
- Set slide backgrounds (solid color, gradient, pattern, picture, or theme)
- Configure slide masters and layouts for consistent design across slides

## Key Classes and Types

| Class/Interface | Purpose |
|----------------|---------|
| `Slide` | A single slide in the presentation |
| `SlideCollection` | Collection of slides with add/remove/move methods |
| `SlideMaster` | Top-level template that cascades settings to all descendant slides |
| `SlideLayout` | Per-layout template with content placeholders |
| `SlideLayoutType` | Enum listing built-in layout presets |
| `Shape` | Standard shape (geometry preset or custom) |
| `PictureShape` | Shape containing an image |
| `GroupShape` | Container grouping multiple shapes |
| `ConnectorShape` | Line connecting two shapes |
| `ShapeType` | Enum listing built-in shape presets (Rectangle, Star12, Heart, etc.) |
| `TextArea` | Text content container attached to a shape |
| `TextParagraph` | A paragraph within a TextArea |
| `TextRun` | A run of text with uniform formatting within a paragraph |
| `TextProperties` | Font settings (fill/color, size, bold, italic, underline) |
| `TextParagraphProperties` | Paragraph-level settings (alignment, indent, bullets) — in `DevExpress.Docs.Office` |
| `Table` | Table shape (rows x columns) |
| `TableCell` | Individual cell in a table |
| `TableRow` | A row in a table |
| `TableColumn` | A column in a table |
| `ThemedTableStyle` | Theme-based table style |
| `TableStyleType` | Enum of predefined table styles |
| `CustomSlideBackground` | Custom background with any fill type |
| `ThemedSlideBackground` | Background using a theme fill |
| `SolidFill` | Solid color fill |
| `GradientFill` | Gradient fill |
| `PatternFill` | Pattern fill |
| `PictureFill` | Image fill |
| `LineStyle` | Outline/border configuration |

## Slide Management

### Add a Slide

```csharp
using DevExpress.Docs.Presentation;

Presentation presentation = new Presentation();

// Add a blank slide
Slide blankSlide = new Slide(new SlideLayout(SlideLayoutType.Blank, "blank"));
presentation.Slides.Add(blankSlide);

// Add a slide based on the default master's Title layout
SlideMaster master = presentation.SlideMasters[0];
Slide titleSlide = new Slide(master.Layouts.Get(SlideLayoutType.Title));
presentation.Slides.Add(titleSlide);

// Insert at a specific position
Slide insertedSlide = new Slide(master.Layouts.GetOrCreate(SlideLayoutType.Object));
presentation.Slides.Insert(1, insertedSlide);
```

### Access and Remove Slides

```csharp
// Access by index
Slide firstSlide = presentation.Slides[0];

// Remove specific slide
presentation.Slides.Remove(firstSlide);

// Remove by index
presentation.Slides.RemoveAt(0);

// Clear all slides
presentation.Slides.Clear();
```

### Reorder Slides

```csharp
// Move slide to position 1 (0-based)
presentation.Slides.Move(slide, 1);

// Move by index: move slide at index 3 to index 1
presentation.Slides.Move(3, 1);
```

### Clone and Copy Slides

```csharp
// Clone within the same presentation
Slide cloned = presentation.Slides[0].Clone();
presentation.Slides.Add(cloned);

// Copy from another presentation (merge)
Presentation source = new Presentation(File.ReadAllBytes("source.pptx"));
Presentation target = new Presentation(File.ReadAllBytes("target.pptx"));

foreach (Slide slide in source.Slides) {
    target.Slides.Add(slide);
}
```

### Manage Slide Visibility

```csharp
// Hide a slide (it stays in file but is skipped during presentation)
slide.Visible = false;

// Remove all hidden slides
foreach (Slide s in presentation.Slides.ToList()) {
    if (!s.Visible)
        presentation.Slides.Remove(s);
}
```

## Slide Size and Orientation

```csharp
// Use a predefined size (default is Widescreen 16:9)
presentation.SlideSize = new SlideSize(SlideSizeType.A4Paper);

// With orientation
presentation.SlideSize = new SlideSize(SlideSizeType.A4Paper, SlideOrientation.Portrait);

// Custom size (in document units, 1/300 inch)
presentation.SlideSize = new SlideSize(1600, 900);

// Read current size
float width = presentation.SlideSize.Width;
float height = presentation.SlideSize.Height;
SlideOrientation orientation = presentation.SlideSize.Orientation;
```

Available predefined sizes via `SlideSizeType`: `Widescreen` (default, 16:9), `OnScreen` (4:3), `A4Paper`, `A3Paper`, `LetterPaper`, `LedgerPaper`, `Banner`, `Overhead`, `Slide35mm`, `OnScreen16x10`, `OnScreen16x9`, `B4IsoPaper`, `B5IsoPaper`.

## Slide Masters and Layouts

### Configure the Default Slide Master

```csharp
SlideMaster master = presentation.SlideMasters[0];
master.Name = "Corporate";
master.Background = new CustomSlideBackground(new SolidFill(Color.FromArgb(30, 60, 120)));

// Add a shape visible on all descendant slides
Shape logo = new Shape(ShapeType.Rectangle) {
    X = 3800, Y = 50, Width = 150, Height = 50,
    Fill = new SolidFill(Color.White)
};
master.Shapes.Add(logo);
```

### Add a Custom Slide Master

```csharp
SlideMaster newMaster = new SlideMaster("AlternateMaster");
newMaster.Background = new CustomSlideBackground(new SolidFill(Color.DarkGreen));
// Replace the default master
presentation.SlideMasters.ResetTo(newMaster);
// Or add alongside existing master
presentation.SlideMasters.Add(newMaster);
```

### Work with Layouts

Built-in layout types (via `SlideLayoutType`): `Title`, `Object`, `Blank`, `SectionHeader`, `TwoObjects`, `TwoTextsAndTwoObjects`, `TitleOnly`, `TitleObjectAndCaption`, `PictureAndCaption`, `VerticalText`, `VerticalTitleAndText`, `Custom`.

```csharp
// Get an existing layout (throws if not found)
SlideLayout titleLayout = master.Layouts.Get(SlideLayoutType.Title);

// Get or create a layout
SlideLayout objectLayout = master.Layouts.GetOrCreate(SlideLayoutType.Object);

// Create and add a custom layout
SlideLayout customLayout = new SlideLayout(SlideLayoutType.Custom, "MyLayout");
customLayout.Background = new CustomSlideBackground(new SolidFill(Color.LightYellow));
master.Layouts.Add(customLayout);
```

### Access Placeholder Shapes

Placeholder shapes are available through `slide.Shapes`. Use `shape.PlaceholderSettings.Type` to identify them.

```csharp
Slide slide = new Slide(master.Layouts.Get(SlideLayoutType.Title));
foreach (Shape shape in slide.Shapes) {
    if (shape.PlaceholderSettings.Type is PlaceholderType.CenteredTitle)
        shape.TextArea = new TextArea("Presentation Title");
    if (shape.PlaceholderSettings.Type is PlaceholderType.Subtitle)
        shape.TextArea = new TextArea("Subtitle text here");
}
presentation.Slides.Add(slide);
```

Available `PlaceholderType` values: `CenteredTitle`, `Title`, `Subtitle`, `Body`, `Object`, `Picture`, `DateAndTime`, `Footer`, `SlideNumber`.

### Remove Unused Placeholders

A layout's placeholder shapes (e.g., Title + Subtitle for the Title layout) are all added to a new slide, whether or not you populate them. If you don't assign a `TextArea` to a placeholder, it stays on the slide empty — PowerPoint's editing view then shows a "Click to add title"/"Click to add text" frame that overlaps your custom content. This is mostly a PowerPoint-authoring concern: slide show mode and PDF/image export are largely unaffected.

To avoid this, remove any placeholder shape you don't use. Iterate a materialized copy of `slide.Shapes` (`.ToList()`) since you're removing from the same collection you're enumerating:

```csharp
Slide slide = new Slide(master.Layouts.GetOrCreate(SlideLayoutType.Object));
foreach (Shape shape in slide.Shapes.ToList()) {
    if (shape.PlaceholderSettings.Type is PlaceholderType.Title) {
        shape.TextArea = new TextArea("Build Status");
    }
    if (shape.PlaceholderSettings.Type is PlaceholderType.Body) {
        // Replace the Body placeholder with a table instead of populating it
        RectangleF rect = presentation.GetActualShapeBounds(slide, shape);
        slide.Shapes.Remove(shape);

        Table table = new Table(5, 5, rect.X, rect.Y, rect.Width, rect.Height);
        slide.Shapes.Add(table);
    }
}
```

Use `Presentation.GetActualShapeBounds(Slide, Shape)` to read a placeholder's effective bounds before removing it, so replacement content (a table, a custom shape) lines up with where the placeholder was.

## Shapes

### Add a Preset Shape

Shapes use document units (1/300 inch). A default widescreen slide is ~4000 x 2250 units.

```csharp
using DevExpress.Docs.Office;
using DevExpress.Docs.Presentation;
using System.Drawing;

// Shape(ShapeType, x, y, width, height) constructor
Shape rect = new Shape(ShapeType.Rectangle, 100, 100, 1500, 600);
rect.Fill = new SolidFill(Color.SteelBlue);
rect.Outline = new LineStyle { Fill = new SolidFill(Color.DarkBlue), Width = 3 };
slide.Shapes.Add(rect);

// Star shape
Shape star = new Shape(ShapeType.Star12);
star.Fill = new SolidFill(Color.Gold);
star.Outline = new LineStyle { Fill = new SolidFill(Color.DarkOrange), Width = 4 };
star.X = 500; star.Y = 500; star.Width = 800; star.Height = 800;
slide.Shapes.Add(star);
```

Common `ShapeType` values: `Rectangle`, `RoundedRectangle`, `Ellipse`, `Triangle`, `RightTriangle`, `Pentagon`, `Hexagon`, `Star4`, `Star5`, `Star6`, `Star8`, `Star10`, `Star12`, `Heart`, `Diamond`, `Arrow`, `TextBox`.

### Add a Picture Shape

```csharp
using DevExpress.Drawing;

PictureShape picture = new PictureShape();
using Stream imgStream = new FileStream("logo.png", FileMode.Open, FileAccess.Read);
picture.Image = DXImage.FromStream(imgStream);
picture.X = 200; picture.Y = 200;
picture.Width = 700; picture.Height = 500;
picture.Outline = new LineStyle { Fill = new SolidFill(Color.Gray), Width = 2 };
slide.Shapes.Add(picture);
```

### Group Shapes

```csharp
Shape s1 = new Shape(ShapeType.Rectangle) { X = 100, Y = 100, Width = 500, Height = 500, Fill = new SolidFill(Color.Blue) };
Shape s2 = new Shape(ShapeType.Ellipse) { X = 700, Y = 100, Width = 500, Height = 500, Fill = new SolidFill(Color.Red) };

GroupShape group = new GroupShape();
group.X = 100; group.Y = 100; group.Width = 1200; group.Height = 500;
group.Fill = new SolidFill(Color.LightGray);
group.Shapes.Add(s1);
group.Shapes.Add(s2);
slide.Shapes.Add(group);
```

### Reorder Shapes (Z-Order)

```csharp
slide.Shapes.BringToFront(shape);
slide.Shapes.BringForward(shape);
slide.Shapes.SendToBack(shape);
slide.Shapes.SendBackward(shape);
slide.Shapes.Move(shape, targetIndex: 0);
```

### Remove Shapes

```csharp
slide.Shapes.Remove(shape);
slide.Shapes.RemoveAt(0);
slide.Shapes.Clear();
```

### Apply Effects to Shapes

```csharp
ShapeEffectProperties effects = new ShapeEffectProperties();
effects.OuterShadow = new OuterShadowEffect {
    BlurRadius = 50,
    Color = new OfficeColor(Color.Gray),
    HorizontalScale = 120,
    VerticalScale = 120
};
shape.Effects = effects;
```

## Shape Text Formatting

### Basic Text Assignment

```csharp
// Assign plain text
shape.TextArea = new TextArea("Hello, World!");

// Multiple paragraphs via Text property (use \r\n as separator)
shape.TextArea = new TextArea() { Text = "Line 1\r\nLine 2\r\nLine 3" };
```

### Paragraphs and Runs with Formatting

```csharp
TextArea textArea = new TextArea();
textArea.Paragraphs.Clear(); // Remove the default empty paragraph

// Paragraph with a single run
TextParagraph para = new TextParagraph();
para.Runs.Add(new TextRun {
    Text = "Bold Red Text",
    TextProperties = new TextProperties {
        Fill = new SolidFill(Color.Red),
        Bold = true,
        FontSize = 24
    }
});
textArea.Paragraphs.Add(para);

// Mixed-format paragraph
TextParagraph para2 = new TextParagraph();
para2.Runs.Add(new TextRun { Text = "Normal " });
para2.Runs.Add(new TextRun { Text = "Bold", TextProperties = new TextProperties { Bold = true } });
para2.Runs.Add(new TextRun { Text = " Italic", TextProperties = new TextProperties { Italic = true } });
textArea.Paragraphs.Add(para2);

shape.TextArea = textArea;
```

### Paragraph-Level Formatting

```csharp
// Set alignment for a specific paragraph
TextParagraph para = new TextParagraph("Centered text");
para.Properties = new TextParagraphProperties {
    Alignment = TextParagraphAlignment.Center
};

// Set formatting for all paragraphs via TextParagraphProperties
textArea.ParagraphProperties = new TextParagraphProperties {
    TextProperties = new TextProperties { FontSize = 18, Fill = new SolidFill(Color.DarkBlue) }
};
```

### Bullet Lists

```csharp
// Character bullet
TextParagraph charBullet = new TextParagraph("Item with bullet");
charBullet.Properties = new TextParagraphProperties {
    ListBullet = new CharListBullet('•')
};

// Numbered list
TextParagraph numbered = new TextParagraph("First item");
numbered.Properties = new TextParagraphProperties {
    ListBullet = new NumberingListBullet(NumberingListBulletFormat.WideBlackCircledNumber, startNumber: 1)
};

textArea.Paragraphs.Add(charBullet);
textArea.Paragraphs.Add(numbered);
```

### Find, Replace, Remove Text

```csharp
// Find all occurrences
IList<TextRange> ranges = shape.TextArea.FindText("DevExpress");
IList<TextRange> caseRanges = shape.TextArea.FindText("DevExpress",
    new TextSearchOptions { MatchCase = true, WholeWordOnly = true });

// Replace text
shape.TextArea.ReplaceText("old text", "new text");
shape.TextArea.ReplaceText("old", "new", new TextSearchOptions { MatchCase = true });

// Apply formatting to found ranges
foreach (TextRange range in ranges) {
    shape.TextArea.ModifyTextProperties(range, new TextProperties { Fill = new SolidFill(Color.Blue) });
}

// Remove text
shape.TextArea.RemoveText(shape.TextArea.FindText("remove this"));
```

## Tables

### Create and Add a Table

```csharp
using DevExpress.Docs.Office;
using DevExpress.Docs.Presentation;

// Table(rowCount, columnCount, x, y, width, height)
Table table = new Table(3, 4, 100f, 200f, 3500f, 1200f);
slide.Shapes.Add(table);

// Set cell text using [rowIndex, columnIndex] indexer
table[0, 0].TextArea.Text = "Product";
table[0, 1].TextArea.Text = "Q1";
table[0, 2].TextArea.Text = "Q2";
table[0, 3].TextArea.Text = "Total";

table[1, 0].TextArea.Text = "Widget A";
table[1, 1].TextArea.Text = "15,000";
table[1, 2].TextArea.Text = "18,500";
table[1, 3].TextArea.Text = "33,500";

// Apply a theme style
table.Style = new ThemedTableStyle(TableStyleType.LightStyle1Accent4);
table.HasHeaderRow = true;
table.HasBandedRows = true;
```

### Insert and Remove Rows/Columns

```csharp
// Insert a new column at position 0
table.Columns.Insert(0, new TableColumn(width: 500));
table[0, 0].TextArea.Text = "ID";

// Insert a new row at the top
table.Rows.Insert(0, new TableRow());

// Remove column and row
table.Columns.RemoveAt(0);
table.Rows.RemoveAt(0);
```

### Format Table Cells

```csharp
TableCell cell = table[1, 2];

// Fill and text
cell.Fill = new SolidFill(Color.AliceBlue);
cell.TextArea.Level1ParagraphProperties.TextProperties.Fill = new SolidFill(Color.DarkBlue);
cell.TextArea.Level1ParagraphProperties.Alignment = TextParagraphAlignment.Center;

// Cell borders
cell.TopBorder = new LineStyle { Width = 4, Fill = new SolidFill(Color.Red) };
cell.BottomBorder = new LineStyle { Width = 4, Fill = new SolidFill(Color.Blue) };
cell.LeftBorder = new LineStyle { Width = 2, Fill = new SolidFill(Color.Gray) };
cell.RightBorder = new LineStyle { Width = 2, Fill = new SolidFill(Color.Gray) };
```

### Merge and Split Cells

```csharp
// Merge cells: use table.MergeCells(startCell, endCell)
table.MergeCells(table[0, 0], table[0, 1]); // merge cells [0,0] and [0,1] in row 0
// Split: table[rowIndex, colIndex].Split(rowCount, columnCount)
table[0, 0].Split(rowCount: 1, columnCount: 2);
```

### Extract Data from a Table

```csharp
var sb = new System.Text.StringBuilder();
for (int r = 0; r < table.Rows.Count; r++) {
    for (int c = 0; c < table.Columns.Count; c++) {
        string text = table[r, c].TextArea?.Text ?? string.Empty;
        sb.Append(text);
        if (c < table.Columns.Count - 1) sb.Append('\t');
    }
    sb.AppendLine();
}
string tableData = sb.ToString();
```

## Slide Backgrounds

### Solid Color Background

```csharp
slide.Background = new CustomSlideBackground(new SolidFill(Color.LightCyan));
```

### Gradient Background

```csharp
using DevExpress.Docs.Office;

GradientFill gradient = new GradientFill();
gradient.GradientType = GradientType.Linear;
gradient.Angle = 90;
// Verify: GradientFill.GradientStops API for adding stops
slide.Background = new CustomSlideBackground(gradient);
```

### Pattern Background

```csharp
PatternFill pattern = new PatternFill(FillPatternType.Vertical, Color.White, Color.LightBlue);
slide.Background = new CustomSlideBackground(pattern);
```

### Picture Background

```csharp
using DevExpress.Drawing;

using Stream stream = new FileStream("background.jpg", FileMode.Open, FileAccess.Read);
PictureFill picture = new PictureFill(DXImage.FromStream(stream));
picture.Stretch = true;
slide.Background = new CustomSlideBackground(picture);
```

### Theme Background

```csharp
// Apply a fill from the theme (fill index 1..999)
slide.Background = new ThemedSlideBackground(2) { Color = new OfficeColor(Color.RoyalBlue) };
```

### Inherit Background from Master

To apply a consistent background to all slides, set the background on the `SlideMaster` or `SlideLayout` instead of individual slides:

```csharp
presentation.SlideMasters[0].Background = new CustomSlideBackground(new SolidFill(Color.Navy));
```

## Troubleshooting

- **Shape not visible**: Check that X, Y, Width, Height place the shape within the slide bounds. Use `Presentation.GetActualShapeBounds(slide, shape)` to inspect inherited bounds from layout.
- **Text not displaying**: Ensure `shape.TextArea` is not null. A new shape has no `TextArea` by default — assign one before reading.
- **Table index out of range**: Check `table.Rows.Count` and `table.Columns.Count` before accessing cells.
- **Background not applying**: Setting a background on an individual slide overrides the master. Setting it on the master applies to all slides that do not have their own background.
- **Shapes not in expected order**: `Shapes` collection order controls z-index stacking, not visual position. Use `BringToFront` / `SendToBack` to adjust.

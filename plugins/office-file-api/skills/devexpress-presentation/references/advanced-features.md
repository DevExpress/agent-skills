# Advanced Features — DevExpress Presentation API

This reference covers speaker notes, headers and footers, text search/replace across the full presentation, content extraction, theme customization, view settings, and document properties.

## When to Use This Reference

Use this when you need to:
- Add, edit, or remove speaker notes on individual slides
- Configure the Notes Master layout
- Add footer text, date/time, and slide numbers to slides
- Edit or remove footer elements from individual slides or slide masters
- Search for text across the entire presentation or a specific slide
- Replace or remove text programmatically
- Extract text or images from slides
- Customize the presentation theme (color scheme, font scheme)
- Configure view settings (active view, grid spacing, normal view panels)
- Read or set document properties (built-in or custom metadata)

## Key Classes and Types

| Class/Interface | Purpose |
|----------------|---------|
| `NotesSlide` | Represents speaker notes for a slide |
| `NotesMaster` | Shared layout for all notes (visual settings, background) |
| `HeaderFooterManager` | Adds footer text, date, and slide numbers to slides |
| `TextSearchOptions` | Options for text search (case sensitivity, whole word) |
| `TextSearchInfo` | Search result: contains matched TextRange and parent TextArea |
| `TextRange` | A range of characters within a TextArea |
| `TextProperties` | Text formatting (fill/color, bold, italic, font size, highlight) |
| `Theme` | Presentation theme (color scheme, font scheme, effects) |
| `ThemeColorScheme` | Color scheme within a theme |
| `ThemeFontScheme` | Font scheme (major/minor fonts) within a theme |
| `ViewProperties` | View settings (active view type, grid spacing) |
| `NormalViewProperties` | Settings specific to the Normal View (pane sizes, single view) |
| `CommonSlideViewProperties` | Settings for slide and notes regions (drawing guides, scale) |
| `DrawingGuide` | A guide line in the slide view |
| `DocumentProperties` | Built-in and custom metadata for the presentation |
| `DocumentCustomProperty` | A single custom document property |

## Speaker Notes

### Create Notes Master

Before adding notes, create a `NotesMaster` to define the shared visual parameters for all notes.

```csharp
using DevExpress.Docs.Presentation;

Presentation presentation = new Presentation(File.ReadAllBytes("input.pptx"));

// Create a notes master if one does not exist
if (presentation.NotesMaster == null) {
    // Verify: NotesMaster creation API — check NotesMaster constructor availability
    // presentation.NotesMaster = new NotesMaster();
}
```

### Add a Note to a Slide

```csharp
Slide slide = presentation.Slides[0];

// Create a NotesSlide and assign it
NotesSlide note = new NotesSlide();
note.TextArea.Text = "This is the speaker note for slide 1. Remember to pause here.";
slide.Notes = note;
```

### Edit Note Text

```csharp
// Access existing note
NotesSlide note = presentation.Slides[0].Notes;
if (note != null) {
    note.TextArea.Text = "Updated speaker note content.";
}
```

### Remove Notes

```csharp
// Remove from a specific slide
presentation.Slides[0].Notes = null;

// Remove from all slides
foreach (Slide slide in presentation.Slides) {
    slide.Notes = null;
}
```

### Extract Note Text

```csharp
// Extract note body text from a specific slide
string noteText = "";
NotesSlide notes = presentation.Slides[0].Notes;
if (notes != null) {
    foreach (var noteShape in notes.Shapes) {
        if (noteShape is Shape textShape && textShape.TextArea != null) {
            string shapeText = textShape.TextArea.Text;
            if (textShape.PlaceholderSettings?.Type == PlaceholderType.SlideNumber
                || string.IsNullOrWhiteSpace(shapeText))
                continue;
            noteText += shapeText + "\r\n";
        }
    }
}
```

## Headers and Footers

### Add Footer to All Slides

```csharp
using DevExpress.Docs.Presentation;

HeaderFooterManager hfm = presentation.HeaderFooterManager;

// Add footer text to all slides
hfm.AddFooterPlaceholder(presentation.Slides, "DevExpress Confidential");

// Add slide numbers to all slides
hfm.AddSlideNumberPlaceholder(presentation.Slides);

// Add date/time to all slides
hfm.AddDateTimePlaceholder(presentation.Slides, DateTime.Now.ToString("yyyy-MM-dd"));
```

### Add Footer to a Specific Slide

```csharp
Slide firstSlide = presentation.Slides[0];
HeaderFooterManager hfm = presentation.HeaderFooterManager;

hfm.AddFooterPlaceholder(firstSlide, "Page footer text");
hfm.AddSlideNumberPlaceholder(firstSlide);
hfm.AddDateTimePlaceholder(firstSlide, "April 2025");
```

### Edit Footer Content

```csharp
// Access the footer placeholder on a slide
Shape footerShape = slide.ActualFooterPlaceholder;
if (footerShape != null) {
    footerShape.TextArea.Text = "Updated Footer Text";
}

// Access date/time placeholder
Shape dateShape = slide.ActualDateTimePlaceholder;
if (dateShape != null) {
    dateShape.TextArea.Text = DateTime.Now.ToString("dddd, MMMM d, yyyy");
}

// Style the footer text
slide.ActualFooterPlaceholder.TextArea.ParagraphProperties = new ParagraphProperties {
    TextProperties = new TextProperties {
        Fill = new SolidFill(Color.DarkGray),
        FontSize = 10
    }
};
```

### Configure Footer in Slide Master

Set footer content at the master level to apply to all descendant slides:

```csharp
SlideMaster master = presentation.SlideMasters[0];
master.ActualFooterPlaceholder.TextArea.Text = "Company Confidential";
master.ActualDateTimePlaceholder.TextArea.Text = DateTime.Now.ToString("dddd dd MMMM yyyy");
```

### Remove Footer Elements

```csharp
// Remove the footer placeholder from a slide
while (slide.ActualFooterPlaceholder != null) {
    slide.Shapes.Remove(slide.ActualFooterPlaceholder);
}

// Remove slide number placeholder
while (slide.ActualSlideNumberPlaceholder != null) {
    slide.Shapes.Remove(slide.ActualSlideNumberPlaceholder);
}
```

## Text Search, Replace, and Remove

Text operations work at three levels: `TextArea` (shape level), `Slide` level, and `Presentation` level.

### Search Text Across the Entire Presentation

```csharp
using DevExpress.Docs.Presentation;

// Find all occurrences across all slides, shapes, notes, and tables
IList<TextSearchInfo> results = presentation.FindText("DevExpress");

// Case-sensitive, whole-word search
IList<TextSearchInfo> exactResults = presentation.FindText("API",
    new TextSearchOptions { MatchCase = true, WholeWordOnly = true });

Console.WriteLine($"Found {results.Count} occurrence(s).");
```

### Replace Text Across the Entire Presentation

```csharp
// Replace across all slides
presentation.ReplaceText("old company name", "New Company Name");

// Case-sensitive replace
presentation.ReplaceText("v25.1", "v25.2",
    new TextSearchOptions { MatchCase = true });
```

### Replace Text in a Specific Slide

```csharp
Slide slide = presentation.Slides[0];
slide.ReplaceText("placeholder text", "actual content");
```

### Apply Formatting to Found Text

```csharp
// Highlight all occurrences of "important" in red
IList<TextSearchInfo> matches = presentation.FindText("important",
    new TextSearchOptions { MatchCase = false });
presentation.ModifyTextProperties(matches,
    new TextProperties { Fill = new SolidFill(Color.Red), Bold = true });
```

### Remove Text

```csharp
// Find and remove across the presentation
IList<TextSearchInfo> toRemove = presentation.FindText("[DRAFT]");
presentation.RemoveText(toRemove);

// On a specific slide
Slide slide = presentation.Slides[0];
IList<TextSearchInfo> slideMatches = slide.FindText("CONFIDENTIAL");
slide.RemoveText(slideMatches);
```

## Content Extraction

### Extract Text from All Slides

```csharp
using System.Linq;

var allText = new System.Text.StringBuilder();

foreach (Slide slide in presentation.Slides) {
    // Sort shapes top-to-bottom, left-to-right for reading order
    var sortedShapes = slide.Shapes
        .Where(s => s is Shape shape && shape.TextArea != null)
        .OrderBy(s => s.Y)
        .ThenBy(s => s.X);

    foreach (var shapeBase in sortedShapes) {
        if (shapeBase is Shape textShape) {
            string text = textShape.TextArea.Text;
            if (textShape.PlaceholderSettings?.Type == PlaceholderType.SlideNumber
                || string.IsNullOrWhiteSpace(text))
                continue;
            allText.AppendLine(text);
        }
    }
}
string extracted = allText.ToString();
```

### Extract Text from a Specific Shape

```csharp
// Find a shape by name
Shape target = presentation.Slides[0].Shapes.Find<Shape>(s => s.Name == "TextBox 1");
if (target != null) {
    string text = target.TextArea.Text;
    string firstParagraph = target.TextArea.Paragraphs[0].Text;
}
```

### Extract Images from Slides

```csharp
using DevExpress.Drawing;

int slideIndex = 0;
foreach (Slide slide in presentation.Slides) {
    var pictures = slide.Shapes
        .Where(s => s is PictureShape)
        .OrderBy(s => s.Y).ThenBy(s => s.X)
        .Cast<PictureShape>();

    int imgIndex = 0;
    foreach (PictureShape pic in pictures) {
        pic.Image.Save($"slide{slideIndex}_img{imgIndex}.png", DXImageFormat.Png);
        imgIndex++;
    }
    slideIndex++;
}
```

## Themes

Themes define the visual identity of a presentation: color scheme, font scheme, and effect settings. Each `SlideMaster` has an associated `Theme`.

### Access the Theme

```csharp
// Access via the slide master
Theme theme = presentation.SlideMasters[0].Theme;
ThemeColorScheme colorScheme = theme.ColorScheme;
ThemeFontScheme fontScheme = theme.FontScheme;
```

### Customize the Color Scheme

```csharp
// Verify: ThemeColorScheme property names from API documentation
// Color scheme properties include: Dark1, Dark2, Light1, Light2, Accent1..Accent6, Hyperlink, FollowedHyperlink
// Example (verify exact property names):
// theme.ColorScheme.Accent1 = new OfficeColor(Color.DarkBlue);
// theme.ColorScheme.Accent2 = new OfficeColor(Color.DarkGreen);
```

> **Note:** For exact `ThemeColorScheme` property names and `ThemeFontScheme` API, use the DxDocs MCP tool or check the `apidoc/DevExpress.Docs.Presentation/ThemeColorScheme` reference file.

### Apply a Theme to a Slide Master

Each `SlideMaster` carries its own `Theme`. Assign a modified theme or create a new one:

```csharp
// Verify: Theme constructor and assignment API
// theme = new Theme();
// presentation.SlideMasters[0].Theme = theme;
```

## View Settings

### Set the Active View

```csharp
using DevExpress.Docs.Presentation;

// Set the default view that opens when the file is opened
presentation.ViewProperties.ActiveViewType = ViewType.NormalSlideView;
// Other options: SlideThumbnailView, SlideSorterView, SlideMasterView,
//                NotesView, NotesMasterView, HandoutView, OutlineView
```

### Adjust Grid Spacing

```csharp
// Set grid spacing for all views (except SlideSorterView)
presentation.ViewProperties.GridSpacing = 500;
```

### Customize the Normal View

```csharp
// Collapse the thumbnails pane
NormalViewRestoredProperties collapsed = new NormalViewRestoredProperties(0.0, false);
presentation.ViewProperties.NormalViewProperties.RestoredLeft = collapsed;

// Display a single slide view (hide thumbnails pane)
presentation.ViewProperties.NormalViewProperties.UseSingleView = true;
```

### Add Drawing Guides

```csharp
// Add a vertical drawing guide at position 150
DrawingGuide verticalGuide = new DrawingGuide(Direction.Vertical, 150);
presentation.ViewProperties.SlideViewProperties.DrawingGuides.Add(verticalGuide);

// Add a horizontal drawing guide
DrawingGuide horizontalGuide = new DrawingGuide(Direction.Horizontal, 300);
presentation.ViewProperties.SlideViewProperties.DrawingGuides.Add(horizontalGuide);
```

## Document Properties

### Read and Set Built-In Properties

```csharp
DocumentProperties props = presentation.DocumentProperties;

// Read
string title = props.Title;
string author = props.Author;
int slideCount = props.Slides;  // Read-only

// Set
props.Title = "Annual Report 2025";
props.Author = "Finance Team";
props.Subject = "Financial Performance";
props.Company = "DevExpress";
props.Keywords = "finance, annual, 2025";
props.Category = "Reports";
props.Description = "Year-end financial review";
```

Available built-in properties: `Title`, `Author`, `Subject`, `Company`, `Category`, `Keywords`, `Description`, `Application`, `ContentStatus`, `DocumentRevision`, `DocumentVersion`. Read-only: `Slides`, `HiddenSlides`, `Notes`, `Security`, `PresentationFormat`. Auto-updated: `Created`, `Modified`, `LastModifiedBy`, `Printed`.

### Add Custom Properties

```csharp
DocumentProperties props = presentation.DocumentProperties;

// Add custom properties (supported types: string, bool, int, double, DateTime)
props.CustomProperties.Add("Department", "Engineering");
props.CustomProperties.Add("ApprovalStatus", true);
props.CustomProperties.Add("ReviewCycle", 3);
props.CustomProperties.Add("LastReviewDate", DateTime.Today);
props.CustomProperties.Add("BudgetAmount", 125000.50);
```

### Read Custom Properties

```csharp
foreach (var kvp in presentation.DocumentProperties.CustomProperties) {
    Console.WriteLine($"{kvp.Key}: {kvp.Value.StringValue}");
}
```

## Troubleshooting

- **Notes not visible after adding**: The `NotesMaster` must exist in the presentation. Check whether `presentation.NotesMaster` is null and create it if needed.
- **Footer not appearing on slides**: Call `HeaderFooterManager.Add...Placeholder` on the slide or pass the slide collection. Setting footer text on `ActualFooterPlaceholder` only works after the placeholder has been added.
- **`FindText` returns empty results**: The search is case-sensitive by default only when `MatchCase = true`. Omit `TextSearchOptions` for a case-insensitive search.
- **Theme changes not visible**: Theme properties cascade from master to slides. Changes apply when the presentation is opened in a viewer. Some properties may require the viewer to re-render.
- **Custom property not found**: Use the exact key string used during `Add`. Keys are case-sensitive.
- **View settings ignored by viewer**: View settings (`ViewProperties`) are hints stored in the file. Some viewers (e.g., Google Slides, LibreOffice) may ignore them.

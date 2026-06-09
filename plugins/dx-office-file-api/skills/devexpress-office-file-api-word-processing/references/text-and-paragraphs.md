# Text and Paragraphs — DevExpress Word Processing Document API

The Word Processing Document API provides full control over text content, character formatting, paragraph formatting, document styles, and lists. All changes use a Begin/End update pattern that batches modifications efficiently.

## When to Use This Reference

Use this when you need to:
- Add or insert text and paragraphs programmatically
- Apply character formatting (font, size, color, bold, italic, underline, subscript, etc.)
- Apply paragraph formatting (alignment, indents, spacing, tab stops, keep-with-next)
- Set document-wide default character and paragraph formatting
- Create paragraph styles, character styles, or linked styles
- Apply existing styles from a loaded document
- Set list formatting (bulleted, numbered, multilevel)
- Add hyperlinks, bookmarks, or comments

## Key Classes and Types

| Class/Interface | Purpose |
|----------------|---------|
| `Document` | Root document object; implements `ISubDocument` |
| `Paragraph` | A single paragraph; has `Range`, `Style`, and index in `Document.Paragraphs` |
| `ParagraphCollection` | Zero-indexed collection; use `Append()` to add paragraphs |
| `CharacterProperties` | Character format session object (font, size, color, bold, italic, etc.) |
| `ParagraphProperties` | Paragraph format session object (alignment, indents, spacing, tabs) |
| `ParagraphStyle` | Reusable paragraph style with character + paragraph settings |
| `CharacterStyle` | Reusable character-only style |
| `Units` | Static helper: `InchesToDocumentsF()`, `CentimetersToDocumentsF()`, etc. |

## Adding Text and Paragraphs

```csharp
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;

using (var server = new RichEditDocumentServer())
{
    Document doc = server.Document;

    // AppendText adds text at the end of the document
    doc.AppendText("First paragraph content");

    // Append creates a new paragraph at the end
    Paragraph para2 = doc.Paragraphs.Append();
    doc.InsertText(para2.Range.Start, "Second paragraph content");

    // Insert text at a specific position
    DocumentPosition pos = doc.Paragraphs[0].Range.Start;
    doc.InsertText(pos, "Prefix: ");

    server.SaveDocument("output.docx", DocumentFormat.Docx);
}
```

## Character Formatting

Use `BeginUpdateCharacters` / `EndUpdateCharacters` to format a range:

```csharp
using System.Drawing;

Document doc = server.Document;
doc.AppendText("Title of the Document");

// Format the first paragraph's characters
DocumentRange range = doc.Paragraphs[0].Range;
CharacterProperties cp = doc.BeginUpdateCharacters(range);

cp.FontName = "Helvetica";
cp.FontSize = 20;
cp.Bold = true;
cp.ForeColor = Color.DarkBlue;
cp.Italic = false;
cp.Underline = UnderlineType.Single;
cp.StrikeoutType = StrikeoutType.None;

doc.EndUpdateCharacters(cp);
```

### Available Character Properties

| Property | Type | Description |
|----------|------|-------------|
| `FontName` | `string` | Font family name |
| `FontSize` | `float` | Size in points |
| `Bold` | `bool` | Bold weight |
| `Italic` | `bool` | Italic style |
| `ForeColor` | `Color` | Text color |
| `BackColor` | `Color` | Text highlight/background color |
| `Underline` | `UnderlineType` | Underline style (None, Single, Double, etc.) |
| `StrikeoutType` | `StrikeoutType` | Strikethrough (None, Single, Double) |
| `Subscript` / `Superscript` | `bool` | Subscript or superscript |
| `AllCaps` | `bool` | Display all characters in capitals |
| `SmallCaps` | `bool` | Display lowercase as small capitals |
| `Hidden` | `bool` | Hide the character run |
| `Style` | `CharacterStyle` | Apply a character style |

### Set Default Character Formatting

```csharp
// Applied to all new text in the document
doc.DefaultCharacterProperties.FontName = "Arial";
doc.DefaultCharacterProperties.FontSize = 12;
doc.DefaultCharacterProperties.ForeColor = Color.Black;
```

## Paragraph Formatting

Use `BeginUpdateParagraphs` / `EndUpdateParagraphs`:

```csharp
using DevExpress.Office.Utils;

DocumentRange titleRange = doc.Paragraphs[0].Range;
ParagraphProperties pp = doc.BeginUpdateParagraphs(titleRange);

pp.Alignment = ParagraphAlignment.Center;
pp.LeftIndent = Units.InchesToDocumentsF(0.5f);
pp.RightIndent = Units.InchesToDocumentsF(0.5f);
pp.SpacingBefore = Units.InchesToDocumentsF(0.1f);
pp.SpacingAfter = Units.InchesToDocumentsF(0.2f);
pp.LineSpacingType = ParagraphLineSpacing.Multiple;
pp.LineSpacingMultiplier = 1.5f;
pp.KeepWithNext = true;
pp.PageBreakBefore = false;

doc.EndUpdateParagraphs(pp);
```

### Tab Stops

```csharp
ParagraphProperties pp = doc.BeginUpdateParagraphs(doc.Paragraphs[0].Range);

TabInfoCollection tabs = pp.BeginUpdateTabs(true); // true = clear existing tabs
TabInfo tab = new TabInfo();
tab.Alignment = TabAlignmentType.Center;
tab.Position = Units.InchesToDocumentsF(2.5f);
tabs.Add(tab);
pp.EndUpdateTabs(tabs);

doc.EndUpdateParagraphs(pp);
```

### Set Default Paragraph Formatting

```csharp
doc.DefaultParagraphProperties.Alignment = ParagraphAlignment.Justify;
```

## Document Styles

`RichEditDocumentServer` has no predefined styles in a new blank document. Load a template or create styles manually. When loading an existing document, its styles are available via `Document.ParagraphStyles` and `Document.CharacterStyles`.

### Create and Apply a Paragraph Style

```csharp
doc.BeginUpdate();

ParagraphStyle titleStyle = doc.ParagraphStyles.CreateNew();
titleStyle.Name = "MyHeading1";
titleStyle.FontName = "Segoe UI";
titleStyle.FontSize = 18;
titleStyle.Bold = true;
titleStyle.ForeColor = Color.DarkBlue;
titleStyle.Alignment = ParagraphAlignment.Left;
titleStyle.SpacingAfter = Units.InchesToDocumentsF(0.15f);
titleStyle.OutlineLevel = 1; // Heading level (for TOC)

doc.ParagraphStyles.Add(titleStyle);
doc.EndUpdate();

// Apply to paragraph
doc.Paragraphs[0].Style = titleStyle;
```

### Apply an Existing Style (from a Loaded Document)

```csharp
// Apply a paragraph style by name
doc.Paragraphs[1].Style = doc.ParagraphStyles["Heading 2"];

// Apply a character style to a range
CharacterProperties cp = doc.BeginUpdateCharacters(doc.Paragraphs[2].Range);
cp.Style = doc.CharacterStyles["Strong"];
doc.EndUpdateCharacters(cp);
```

### Create a Linked Style

A linked style combines paragraph and character formatting. Applied to a `Paragraph` it uses both; applied to a `DocumentRange` it uses character formatting only.

```csharp
doc.BeginUpdate();

ParagraphStyle paraStyle = doc.ParagraphStyles.CreateNew();
paraStyle.Name = "MyAnnotation";
paraStyle.Alignment = ParagraphAlignment.Right;
paraStyle.LineSpacingMultiplier = 1.5f;
doc.ParagraphStyles.Add(paraStyle);

CharacterStyle charStyle = doc.CharacterStyles.CreateNew();
charStyle.Name = "MyAnnotationChar";
charStyle.Italic = true;
charStyle.FontSize = 11;
charStyle.ForeColor = Color.Gray;
charStyle.LinkedStyle = paraStyle; // Link to the paragraph style
doc.CharacterStyles.Add(charStyle);

doc.EndUpdate();

// Apply as paragraph style
doc.Paragraphs[0].Style = paraStyle;

// Apply as character style (only character formatting used)
CharacterProperties cp = doc.BeginUpdateCharacters(doc.Paragraphs[1].Range);
cp.Style = charStyle;
doc.EndUpdateCharacters(cp);
```

## Reset Formatting

```csharp
// Reset character formatting on a range to the Normal style
CharacterProperties cp = doc.BeginUpdateCharacters(doc.Paragraphs[0].Range);
cp.Reset(); // All properties reset
doc.EndUpdateCharacters(cp);

// Reset only specific properties using mask
// cp.Reset(CharacterPropertiesMask.FontName | CharacterPropertiesMask.FontSize);
```

## Lists

Lists use `ListLevelProperties` accessed through paragraph numbering. Refer to the DevExpress documentation for detailed list API — the pattern uses `Document.AbstractNumberingLists` and `Document.NumberingLists`.

Common shortcut — apply built-in list styles via paragraph style:

```csharp
// If the document was loaded with built-in styles:
doc.Paragraphs[2].Style = doc.ParagraphStyles["List Bullet"];
doc.Paragraphs[3].Style = doc.ParagraphStyles["List Number"];
```

## Hyperlinks

```csharp
// Add a hyperlink to found text
DocumentRange[] found = doc.FindAll("Click here", SearchOptions.None);
if (found.Length > 0)
{
    Hyperlink link = doc.Hyperlinks.Create(found[0]);
    link.NavigateUri = "https://www.devexpress.com";
    link.ToolTip = "DevExpress website";
}
```

## Bookmarks

```csharp
// Create a bookmark at the start of the document
DocumentRange startRange = doc.CreateRange(doc.Range.Start, 1);
doc.Bookmarks.Create(startRange, "DocumentTop");

// Create a hyperlink anchored to the bookmark
DocumentRange[] targetRanges = doc.FindAll("Back to Top", SearchOptions.None);
if (targetRanges.Length > 0)
{
    Hyperlink link = doc.Hyperlinks.Create(targetRanges[0]);
    link.Anchor = "DocumentTop"; // References the bookmark name
}
```

## Comments

```csharp
// Insert a comment on the first paragraph
DocumentRange commentRange = doc.Paragraphs[0].Range;
doc.Comments.Create(commentRange, "ReviewerName", "This paragraph needs revision.");
```

## Troubleshooting

- **Formatting not applied**: Ensure `EndUpdateCharacters` or `EndUpdateParagraphs` is always called — even if an exception occurs (use try/finally).
- **`ParagraphStyles["Heading 1"]` returns null**: The document was created blank; load a `.docx` template that contains the style or create the style manually.
- **Style changes don't update existing paragraphs**: Modifying a `ParagraphStyle` after `Add()` automatically updates all paragraphs that use that style.
- **`Units` class not found**: Add `using DevExpress.Office.Utils;`.
- **Tab stops lost on save**: Ensure `EndUpdateTabs` is called before `EndUpdateParagraphs`.

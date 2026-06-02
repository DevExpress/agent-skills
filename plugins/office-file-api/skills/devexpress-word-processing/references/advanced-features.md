# Advanced Features — DevExpress Word Processing Document API

This reference covers fields, content controls, custom XML parts, track changes, document comparison, merge/split, search/replace with regex, watermarks, hyphenation, and VBA macro handling.

## When to Use This Reference

Use this when you need to:
- Insert and update document fields (TOC, PAGE, NUMPAGES, DATE, IF, HYPERLINK, MERGEFIELD, etc.)
- Work with content controls (plain text, rich text, dropdown, checkbox, date picker)
- Read or write custom XML parts embedded in a DOCX
- Enable Track Changes and access, accept, or reject revisions
- Compare two documents and produce a revision-marked output
- Merge multiple Word documents into one or split a document by sections
- Search and replace text using regex patterns
- Add text or image watermarks to a document
- Configure hyphenation settings
- Preserve or remove VBA macro modules when loading DOCM files

## Fields

Fields are dynamic placeholders that can be calculated, updated, or converted to plain text. Use `Document.Fields.Create()` to insert a field by its field code string.

### Insert a Field

```csharp
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;

using (var server = new RichEditDocumentServer())
{
    Document doc = server.Document;
    doc.AppendText("Page: ");

    // Insert PAGE field
    doc.Fields.Create(doc.Paragraphs[0].Range.End, "PAGE");

    // Insert NUMPAGES field
    doc.AppendText(" of ");
    doc.Fields.Create(doc.Range.End, "NUMPAGES");

    // Insert DATE field with format switch
    Paragraph datePara = doc.Paragraphs.Append();
    doc.Fields.Create(datePara.Range.Start, @"DATE \@ ""MMMM d, yyyy""");

    // Insert TOC field (Table of Contents)
    Paragraph tocPara = doc.Paragraphs.Append();
    doc.Fields.Create(tocPara.Range.Start, "TOC \\o \"1-3\" \\h \\z \\u");

    // Update all fields
    doc.Fields.UpdateAllFields();

    server.SaveDocument("fields.docx", DocumentFormat.Docx);
}
```

### Supported Field Codes (partial list)

| Field Code | Description |
|-----------|-------------|
| `PAGE` | Current page number |
| `NUMPAGES` | Total page count |
| `DATE \@ "format"` | Current date/time |
| `TOC` | Table of contents (with heading levels, hyperlinks) |
| `HYPERLINK "url"` | Hyperlink |
| `MERGEFIELD FieldName` | Mail merge placeholder |
| `DOCVARIABLE VarName` | Dynamically computed content |
| `INCLUDEPICTURE "path"` | Inline image |
| `IF condition "true" "false"` | Conditional field |
| `SEQ Identifier` | Sequential numbering |
| `STYLEREF StyleName` | Reference a styled paragraph |
| `SYMBOL number \f "FontName"` | Insert a symbol character |
| `DOCPROPERTY PropertyName` | Insert a document property value |
| `REF BookmarkName` | Reference content at a bookmark |

### Update a Specific Field

```csharp
// Update a single field
Field tocField = doc.Fields[0];
tocField.Update();

// Update all fields in the document
doc.Fields.UpdateAllFields();
```

### Convert a Field to Static Text

```csharp
Field field = doc.Fields[0];
field.ResultRange; // DocumentRange containing the field result
doc.Fields.Remove(field); // Removes the field, leaving the result text
```

## Content Controls

Content controls are interactive or structured containers in a document.

```csharp
// Access content controls (requires a document with content controls)
foreach (ContentControl cc in doc.ContentControls)
{
    Console.WriteLine($"Tag: {cc.Tag}, Type: {cc.Type}");
    if (cc.Type == ContentControlType.PlainText)
    {
        string value = doc.GetText(cc.ContentRange);
        Console.WriteLine($"Value: {value}");
    }
}
```

> For creating new content controls programmatically, use `Document.ContentControls.Create()`. Consult the DxDocs MCP for the exact overload and `ContentControlType` values.

## Custom XML Parts

DOCX files can contain custom XML data. The Word Processing Document API allows reading, writing, and removing these parts.

```csharp
// Access custom XML parts in a loaded document
foreach (CustomXmlPart part in doc.CustomXmlParts)
{
    Console.WriteLine(part.XmlData);
}

// Add a new custom XML part
string xml = "<root><item>Value</item></root>";
doc.CustomXmlParts.Add(xml);

// Remove a custom XML part
CustomXmlPart partToRemove = doc.CustomXmlParts[0];
doc.CustomXmlParts.Remove(partToRemove);
```

## Track Changes

### Enable Track Changes

```csharp
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;

using (var server = new RichEditDocumentServer())
{
    server.LoadDocument("input.docx", DocumentFormat.Docx);
    Document doc = server.Document;

    // Enable tracking
    DocumentTrackChangesOptions trackOptions = doc.TrackChanges;
    trackOptions.Enabled = true;
    trackOptions.TrackFormatting = true;
    trackOptions.TrackMoves = true;

    // Any modifications made here are recorded as revisions
    CharacterProperties cp = doc.BeginUpdateCharacters(doc.Paragraphs[0].Range);
    cp.Bold = true;
    doc.EndUpdateCharacters(cp);

    server.SaveDocument("tracked.docx", DocumentFormat.Docx);
}
```

### Change Display Options for Revisions

```csharp
TrackChangesOptions displayOpts = server.Options.Annotations.TrackChanges;
displayOpts.DisplayForReviewMode = DisplayForReviewMode.SimpleMarkup;
displayOpts.DisplayFormatting = DisplayFormatting.ColorOnly;
displayOpts.FormattingColor = RevisionColor.ClassicBlue;
displayOpts.DisplayInsertionStyle = DisplayInsertionStyle.Underline;
displayOpts.InsertionColor = RevisionColor.DarkRed;

// Hide all revision marks in PDF export
displayOpts.DisplayForReviewMode = DisplayForReviewMode.NoMarkup;
```

### Access Revisions

```csharp
RevisionCollection revisions = doc.Revisions;

// Get revisions from the main body
var bodyRevisions = revisions.Get(doc);

// Get revisions from a header
SubDocument header = doc.Sections[0].BeginUpdateHeader(HeaderFooterType.First);
var headerRevisions = revisions.Get(header);
doc.Sections[0].EndUpdateHeader(header);

// Get revisions in a specific range
var rangeRevisions = revisions.Get(doc.Paragraphs[0].Range);
```

### Accept and Reject Revisions

```csharp
// Accept all revisions in the document
doc.Revisions.AcceptAll(doc);

// Reject all revisions in the document
doc.Revisions.RejectAll(doc);

// Accept or reject by a predicate (e.g., by author)
var byAuthor = revisions.Get(doc).Where(r => r.Author == "Janet Leverling");
foreach (Revision revision in byAuthor)
    revision.Accept();
    // or: revision.Reject();

// Reject all revisions in a specific section header
SubDocument hdr = doc.Sections[0].BeginUpdateHeader(HeaderFooterType.Odd);
doc.Revisions.RejectAll(hdr);
doc.Sections[0].EndUpdateHeader(hdr);
```

## Compare Documents

```csharp
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;

using (var server1 = new RichEditDocumentServer())
using (var server2 = new RichEditDocumentServer())
{
    server1.LoadDocument("original.docx", DocumentFormat.Docx);
    server2.LoadDocument("revised.docx", DocumentFormat.Docx);

    // Both documents must NOT have existing revisions
    Document original = server1.Document;
    Document revised = server2.Document;

    // Compare — returns a new SubDocument with revision marks
    SubDocument result = RichEditDocumentServerExtensions.Compare(original, revised);

    // Save result to a new server
    using (var resultServer = new RichEditDocumentServer())
    {
        resultServer.Document.AppendDocumentContent(result.Range);
        resultServer.SaveDocument("comparison.docx", DocumentFormat.Docx);
    }
}
```

### Comparison Options

```csharp
CompareDocumentOptions compareOpts = new CompareDocumentOptions();
compareOpts.AuthorName = "Reviewer";
compareOpts.IgnoreFormatting = true;
compareOpts.IgnoreSpaces = false;
// compareOpts.ComparisonLevel = ComparisonLevel.Character; // if available

SubDocument result = RichEditDocumentServerExtensions.Compare(original, revised, ComparisonTargetType.NewDocument, compareOpts);
```

> **Limitation**: Comments, tables, math equations, fields, and content controls are ignored when comparing.

## Merge Documents

```csharp
using (var server = new RichEditDocumentServer())
{
    server.LoadDocument("document1.docx", DocumentFormat.Docx);

    // Append another document at the end
    server.Document.AppendDocumentContent("document2.docx");

    // Or insert at a specific position
    // server.Document.InsertDocumentContent(position, "document2.docx");

    // Control formatting behavior
    server.Document.AppendDocumentContent("document3.docx", InsertOptions.KeepSourceFormatting);
    // Options: KeepSourceFormatting, UseDestinationStyles, KeepTextOnly

    server.SaveDocument("merged.docx", DocumentFormat.Docx);
}
```

### Merge with Section Boundaries

To preserve different page layouts (margins, paper size, headers/footers), insert each document into a new section. See the `merge-documents.md` article for the full `SectionsMerger` helper pattern.

```csharp
// Append a section break before inserting the next document
Section newSection = server.Document.AppendSection();
newSection.StartType = SectionStartType.NextPage;
server.Document.AppendDocumentContent("next_doc.docx", InsertOptions.KeepSourceFormatting);
```

## Split Documents

Split by extracting content from specific ranges and saving to new files:

```csharp
using (var sourceServer = new RichEditDocumentServer())
{
    sourceServer.LoadDocument("long_doc.docx", DocumentFormat.Docx);
    Document source = sourceServer.Document;

    // Split at a specific section boundary
    foreach (Section section in source.Sections)
    {
        using (var partServer = new RichEditDocumentServer())
        {
            partServer.Document.AppendDocumentContent(section.Range);
            partServer.SaveDocument($"part_{section.Index}.docx", DocumentFormat.Docx);
        }
    }
}
```

## Search and Replace

### Basic Search

```csharp
// Find all occurrences (case-insensitive)
DocumentRange[] found = doc.FindAll("old text", SearchOptions.None);

// Case-sensitive search
DocumentRange[] exact = doc.FindAll("OldText", SearchOptions.CaseSensitive);

// Whole word search
DocumentRange[] words = doc.FindAll("word", SearchOptions.WholeWord);
```

### Regex Search

```csharp
using System.Text.RegularExpressions;

// Find all 6-letter words
Regex regex = new Regex(@"\b\w{6}\b");
DocumentRange[] matches = doc.FindAll(regex);

foreach (DocumentRange match in matches)
{
    string matched = doc.GetText(match);
    Console.WriteLine($"Found: {matched}");
}
```

### Search and Replace

```csharp
// Replace text manually (FindAll + delete + insert)
DocumentRange[] occurrences = doc.FindAll("placeholder", SearchOptions.None);
// Iterate in reverse order to preserve positions
for (int i = occurrences.Length - 1; i >= 0; i--)
{
    doc.Delete(occurrences[i]);
    doc.InsertText(occurrences[i].Start, "replacement");
}
```

## Watermarks

```csharp
using System.Drawing;

// Add a text watermark to all pages (via a floating shape in the header)
Section section = doc.Sections[0];
SubDocument header = section.BeginUpdateHeader(HeaderFooterType.Odd);

Shape watermark = header.Shapes.InsertTextBox(
    header.Range.Start,
    Units.InchesToDocumentsF(2f),
    Units.InchesToDocumentsF(1f)
);
watermark.TextWrapping = TextWrappingType.InFrontOfText;
watermark.HorizontalAlignment = ShapeHorizontalAlignment.Center;
watermark.VerticalAlignment = ShapeVerticalAlignment.Center;
watermark.Line.LineStyle = ShapeLineStyle.None;

// Set watermark text
SubDocument textBoxDoc = watermark.TextBox.Document;
textBoxDoc.AppendText("DRAFT");
CharacterProperties cp = textBoxDoc.BeginUpdateCharacters(textBoxDoc.Range);
cp.FontSize = 72;
cp.ForeColor = Color.FromArgb(128, Color.Red); // Semi-transparent
cp.Bold = true;
textBoxDoc.EndUpdateCharacters(cp);

section.EndUpdateHeader(header);
```

## Hyphenation

```csharp
// Enable automatic hyphenation for the document
doc.Hyphenation = true;
doc.HyphenateCaps = false;

// Disable hyphenation for a specific paragraph
ParagraphProperties pp = doc.BeginUpdateParagraphs(doc.Paragraphs[0].Range);
pp.SuppressHyphenation = true;
doc.EndUpdateParagraphs(pp);
```

## VBA Macros

When loading a DOCM file, the VBA project is preserved. You can delete VBA modules but cannot add new ones.

```csharp
using (var server = new RichEditDocumentServer())
{
    server.LoadDocument("macro-enabled.docm", DocumentFormat.Docm);
    Document doc = server.Document;

    // Access VBA project (if present)
    if (doc.VbaProject != null)
    {
        // List modules
        foreach (var module in doc.VbaProject.Modules)
        {
            Console.WriteLine($"Module: {module.Name}");
        }

        // Remove a module (cannot add new modules)
        // doc.VbaProject.Modules.Remove(module);
    }

    // Save back as DOCM to preserve macros
    server.SaveDocument("output.docm", DocumentFormat.Docm);
}
```

## Document Properties

```csharp
// Built-in document properties
doc.DocumentProperties.Title = "My Report";
doc.DocumentProperties.Author = "My Application";
doc.DocumentProperties.Company = "Acme Corp";
doc.DocumentProperties.Keywords = "report, quarterly";

// Custom document properties
doc.CustomDocumentProperties.Add("ProjectCode", "PROJ-2025-001");
```

## Troubleshooting

- **`Compare` throws exception**: Both input documents must have zero existing tracked revisions. Call `doc.Revisions.AcceptAll(doc)` first.
- **Field result is `{FIELD}` placeholder**: Call `doc.Fields.UpdateAllFields()` after inserting fields.
- **Regex search returns no results**: Verify the pattern with `System.Text.RegularExpressions.Regex` independently before passing to `FindAll`.
- **Watermark appears only on page 1**: The watermark must be in the section's header; if the document has multiple sections, repeat for each section.
- **VBA modules cannot be added**: This is a known limitation — you can only delete modules; use a pre-built DOCM template if you need macros.
- **Merge loses header/footer**: Headers/footers belong to sections. When using `AppendDocumentContent`, copy headers/footers manually using `BeginUpdateHeader`/`EndUpdateHeader` pattern (see merge-documents.md).

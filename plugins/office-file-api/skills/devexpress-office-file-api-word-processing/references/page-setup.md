# Page Setup — Word Processing Document API

## When to Use This Reference

Use this when you need to set page size, orientation, margins, page numbering, columns, or section breaks in a Word document. All page-layout settings belong to a `Section`; a document contains at least one section.

## Access and Modify Page Settings

Use `Document.Sections[n].Page` to get the `SectionPage` object. Set `PaperKind` (from `DXPaperKind`) and `Landscape` to control size and orientation.

```csharp
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;
using DevExpress.Drawing.Printing;
using DevExpress.Utils;

using (var server = new RichEditDocumentServer())
{
    server.LoadDocument("Documents/Grimm.docx");
    Document document = server.Document;

    // Set page size to A6 in landscape with a 2-inch left margin
    document.Sections[0].Page.PaperKind = DXPaperKind.A6;
    document.Sections[0].Page.Landscape = true;
    document.Sections[0].Margins.Left = Units.InchesToDocumentsF(2.0f);

    server.SaveDocument("output.docx", DocumentFormat.Docx);
}
```

For a custom size, set `SectionPage.Width` and `SectionPage.Height` directly (values in `Document.Unit`).

## Set Page Margins

`Section.Margins` returns a `SectionMargins` object. All values are in the units set by `Document.Unit` (default: document units, 1/300 inch). Use `Units.InchesToDocumentsF` to convert.

```csharp
using DevExpress.Office.Utils;

Section firstSection = server.Document.Sections[0];
var margins = firstSection.Margins;
margins.Left   = Units.InchesToDocumentsF(0.5f);
margins.Top    = Units.InchesToDocumentsF(0.7f);
margins.Right  = Units.InchesToDocumentsF(0.5f);
margins.Bottom = Units.InchesToDocumentsF(1.5f);
```

Enable mirrored margins (for double-sided printing) via `Document.MarginType`. Add gutter space via `SectionMargins.Gutter`.

## Work with Sections

```csharp
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;
using DevExpress.Office;

using (var server = new RichEditDocumentServer())
{
    server.LoadDocument("input.docx", DocumentFormat.Docx);
    Document document = server.Document;

    // Append a new section (starts on the next page)
    Section newSection = document.AppendSection();
    newSection.StartType = SectionStartType.NextPage;

    // Insert a section at a specific position
    // document.InsertSection(somePosition);

    // Insert a page break within a section
    var splitPos = document.Sections[0].Paragraphs[2].Range.End;
    document.InsertText(splitPos, Characters.PageBreak.ToString());

    server.SaveDocument("output.docx", DocumentFormat.Docx);
}
```

### Section Start Types

| `SectionStartType` value | Description |
|--------------------------|-------------|
| `NextPage` | New section begins on the next page |
| `Continuous` | New section begins on the same page |
| `EvenPage` | New section begins on the next even page |
| `OddPage` | New section begins on the next odd page |
| `Column` | New section begins in the next column |

## Page Numbering

Each section controls its own page numbering sequence via `Section.PageNumbering`.

```csharp
Section section = document.Sections[0];
section.PageNumbering.ContinueNumbering = false;
section.PageNumbering.FirstPageNumber = 1;
section.PageNumbering.NumberingFormat = NumberingFormat.Decimal;

// Apply updated numbers to PAGE / NUMPAGES fields
document.Fields.UpdateAllFields();
```

## Multi-Column Layout

```csharp
// Create three uniform columns in a section
Section section = document.Sections[0];
section.Columns.CreateUniformColumns(document, 3, Units.InchesToDocumentsF(0.2f));
```

## Key Classes and Types

| Class/Property | Purpose |
|---------------|---------|
| `Document.Sections` | `SectionCollection` — all sections in the document |
| `Document.AppendSection()` | Appends a new section; returns the new `Section` |
| `Document.InsertSection(pos)` | Inserts a section at the given position |
| `Section.Page` | `SectionPage` — paper kind, landscape flag, width, height |
| `SectionPage.PaperKind` | `DXPaperKind` enum (Letter, A4, A6, Legal, etc.) |
| `SectionPage.Landscape` | `true` for landscape orientation |
| `SectionPage.Width` / `.Height` | Custom page size in current `Document.Unit` |
| `Section.Margins` | `SectionMargins` — Left, Right, Top, Bottom, Gutter |
| `SectionMargins.GutterPosition` | Left or right gutter for bound documents |
| `Document.MarginType` | `Normal` or `Mirror` for double-sided printing |
| `Section.StartType` | `SectionStartType` — where the section starts |
| `Section.PageNumbering` | `SectionPageNumbering` — numbering start, format, continuation |
| `SectionPageNumbering.NumberingFormat` | `NumberingFormat` enum (Decimal, LowerRoman, etc.) |
| `Section.Columns` | `SectionColumns` — multi-column layout |
| `SectionColumns.CreateUniformColumns` | Create equal-width columns with a specified spacing |
| `Section.PageBorders` | `SectionPageBorders` — page border styling per section |
| `Section.ProtectedForForms` | Lock this section; use with `Document.Protect` |
| `Characters.PageBreak` | String constant for inserting a manual page break |
| `Units.InchesToDocumentsF(float)` | Convert inches to document units (1/300 inch) |
| `DXPaperKind` | Enum from `DevExpress.Drawing.Printing` (A4, Letter, A6, etc.) |

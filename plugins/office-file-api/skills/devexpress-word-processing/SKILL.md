---
name: devexpress-word-processing
description: Build .NET applications with the DevExpress Word Processing Document API for creating, reading, modifying, and exporting Word documents programmatically without Microsoft Office. Use when working with .docx, .doc, .rtf, .odt, .txt documents, paragraphs, tables, styles, mail merge, headers/footers, fields, track changes, or document conversion. Also use when someone mentions "DevExpress Word", "Word Processing Document API", "RichEditDocumentServer", "DevExpress.XtraRichEdit", "create Word document in C#", "docx automation", "mail merge .NET", or asks about any Word/document processing with DevExpress. Covers both .NET and .NET Framework.
compatibility: Requires .NET 6+ or .NET Framework 4.6.2+. NuGet packages from the DevExpress feed. DevExpress.Pdf.SkiaRenderer required for PDF export on .NET 6+.
metadata:
  author: DevExpress
  version: "25.2"
  source-commit: c5a96ff6e891a1c2633c6621186093faaefabefd
---

# DevExpress Word Processing Document API

The Word Processing Document API is a non-visual .NET library for creating, loading, editing, and exporting Word processing documents programmatically — without requiring Microsoft Office. It supports .docx, .doc, .rtf, .odt, .txt, .html, .mht, and WordML formats and can export to PDF, HTML, and image series. The primary entry point is `RichEditDocumentServer`, a server-side class suitable for console apps, ASP.NET Core, Blazor, MAUI, and background services.

## When to Use This Skill

Use this skill when you need to:

- Create Word documents (.docx, .rtf, .odt) programmatically without Microsoft Office
- Load and modify existing .docx / .doc / .rtf files in .NET
- Apply character and paragraph formatting, styles, or linked styles
- Build tables, lists, hyperlinks, and bookmarks in code
- Add headers, footers, footnotes, endnotes, or watermarks
- Perform mail merge — generate personalized letters, invoices, or reports from a data source
- Search and replace text, including regex-based search
- Export Word documents to PDF, HTML, or a series of page images
- Compare two documents and produce a revision-marked result
- Accept or reject tracked changes programmatically
- Merge or split Word documents
- Work with fields (MERGEFIELD, TOC, HYPERLINK, PAGE, DATE, IF, etc.)
- Protect documents with passwords or restrict editing permissions

## Prerequisites & Installation

### NuGet Packages

| Package | Purpose |
|---------|---------|
| `DevExpress.Document.Processor` | Core Word processing (create, load, edit, save, mail merge) |
| `DevExpress.Pdf.SkiaRenderer` | Required for PDF export and printing on .NET 6+ (cross-platform) |

### .NET (6/7/8+)

```bash
dotnet add package DevExpress.Document.Processor
dotnet add package DevExpress.Pdf.SkiaRenderer  # For PDF export
```

### .NET Framework (4.6.2+)

```
Install-Package DevExpress.Document.Processor
```

Alternatively, reference these assemblies from the DevExpress Unified Installer:
`DevExpress.Data`, `DevExpress.Drawing`, `DevExpress.Office.Core`, `DevExpress.RichEdit.Core`, `DevExpress.Printing.Core`, `DevExpress.Pdf.Core`.

**Important**: All DevExpress packages in a project must share the same version number. A valid DevExpress license is required.

## Before You Start — Ask the Developer

Before generating code, ask these questions to avoid rework:

### General Questions
1. **Target framework**: Are you using .NET 8+, .NET 6/7, or .NET Framework 4.x?
2. **New or existing project?**: Are you creating a new project or adding to an existing one?
3. **Hosting model**: Console app, ASP.NET Core, Blazor, MAUI, WinForms, WPF, or something else?

### Word Processing–Specific Questions
4. **Operation type**: Create new / read existing / modify / convert / mail merge?
5. **Features needed**: Paragraphs & styles / tables / fields / headers-footers / mail merge / search-replace / track changes / shapes & images?
6. **Output format**: .docx / .rtf / .odt / PDF / HTML / image series?

> **Rule**: If the developer's answer is ambiguous or missing, ask before generating code. Do not guess.

## Component Overview

The Word Processing Document API provides:

- **Document lifecycle**: Create, load, save, and dispose of documents (`RichEditDocumentServer`, `Document`)
- **Content authoring**: Paragraphs, text runs, tables, lists, shapes, images, comments (`Document`, `Paragraph`, `Table`, `Shape`)
- **Formatting**: Character properties, paragraph properties, styles, linked styles, theme fonts (`CharacterProperties`, `ParagraphProperties`, `ParagraphStyle`, `CharacterStyle`)
- **Fields & merge**: 25+ field types, mail merge with plain and master-detail data sources (`Field`, `MailMergeOptions`)
- **Output**: PDF export, HTML export/import, RTF, ODT, printing, image series (`RichEditDocumentServer.ExportToPdf`, `SaveDocument`)
- **Review features**: Track changes, compare documents, accept/reject revisions (`Document.TrackChanges`, `Document.Revisions`)

### Core Entry Point

```csharp
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;

// Create a new document
using (var server = new RichEditDocumentServer())
{
    Document doc = server.Document;
    doc.AppendText("Hello, World!");
    server.SaveDocument("output.docx", DocumentFormat.Docx);
}

// Load and modify an existing document
using (var server = new RichEditDocumentServer())
{
    server.LoadDocument("input.docx", DocumentFormat.Docx);
    Document doc = server.Document;
    // ... make changes ...
    server.SaveDocument("output.docx", DocumentFormat.Docx);
}
```

## Documentation & Navigation Guide

### Getting Started
Refer to [references/getting-started.md](references/getting-started.md)

When you need to:
- Set up the Word Processing Document API for the first time
- Understand NuGet packages and assembly references for .NET
- Create your first Word document
- Load, modify, and save documents in different formats
- See a complete working example

### Getting Started — .NET Framework
Refer to [references/getting-started-dotnet-fw.md](references/getting-started-dotnet-fw.md)

When you need to:
- Target .NET Framework 4.6.2+ specifically
- Reference DevExpress assemblies instead of NuGet packages
- Understand .NET Framework–specific differences and limitations

### Text and Paragraphs
Refer to [references/text-and-paragraphs.md](references/text-and-paragraphs.md)

When you need to:
- Add, insert, or delete text and paragraphs
- Apply direct character formatting (font, size, color, bold, italic, underline)
- Apply paragraph formatting (alignment, indents, spacing, tab stops)
- Create and apply paragraph and character styles, including linked styles
- Set default document formatting
- Work with lists (bulleted, numbered, multilevel)
- Add hyperlinks, bookmarks, and comments

### Tables
Refer to [references/tables.md](references/tables.md)

When you need to:
- Create tables in a document
- Add, remove, or resize rows and columns
- Apply table styles and cell formatting
- Merge or split cells
- Set fixed-width columns or AutoFit behavior
- Configure repeat header rows across pages

### Mail Merge
Refer to [references/mail-merge.md](references/mail-merge.md)

When you need to:
- Create or load a mail merge template
- Add MERGEFIELD, DOCVARIABLE, or INCLUDEPICTURE fields to a template
- Connect a data source (DataTable, DataSet, collection, database)
- Execute a mail merge and save or stream the result
- Build master-detail reports with table regions
- Insert images from a database during merge

### Export
Refer to [references/export.md](references/export.md)

When you need to:
- Export a Word document to PDF with options (page range, PDF/A, password)
- Export to HTML with embedded or external images
- Save as RTF, ODT, plain text, or other formats
- Export document pages as images
- Print a document with the default or custom printer settings
- Configure print options (page background, comment display)

### Advanced Features
Refer to [references/advanced-features.md](references/advanced-features.md)

When you need to:
- Work with fields (TOC, PAGE, NUMPAGES, DATE, IF, HYPERLINK, MERGEFIELD, etc.)
- Use content controls or custom XML parts
- Enable and manage track changes
- Compare two documents and produce a diff
- Merge multiple documents or split a document by sections
- Search and replace text, including regex patterns
- Add watermarks, hyphenation settings, or VBA macro handling

### Digital Signing
> ⚠️ This feature requires DevExpress v26.1+. Reference to be added in a future update.

## Quick Start Example

```csharp
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;
using System.Drawing;

using (var server = new RichEditDocumentServer())
{
    Document doc = server.Document;

    // Add a heading paragraph
    doc.BeginUpdate();
    Paragraph heading = doc.Paragraphs.Append();
    doc.InsertText(heading.Range.Start, "Getting Started with Word Processing API");
    doc.EndUpdate();

    // Apply heading style (bold, large font)
    CharacterProperties headingCp = doc.BeginUpdateCharacters(heading.Range);
    headingCp.Bold = true;
    headingCp.FontSize = 18;
    headingCp.ForeColor = Color.DarkBlue;
    doc.EndUpdateCharacters(headingCp);

    ParagraphProperties headingPp = doc.BeginUpdateParagraphs(heading.Range);
    headingPp.Alignment = ParagraphAlignment.Center;
    headingPp.SpacingAfter = Units.InchesToDocumentsF(0.2f);
    doc.EndUpdateParagraphs(headingPp);

    // Add a body paragraph
    Paragraph body = doc.Paragraphs.Append();
    doc.InsertText(body.Range.Start, "This document was created with DevExpress Word Processing Document API.");

    server.SaveDocument("QuickStart.docx", DocumentFormat.Docx);
}
```

### What This Does
Creates a `QuickStart.docx` with a centered dark-blue bold heading and a body paragraph. The file is saved to the working directory and can be opened in any Word-compatible application.

## Key Properties & API Surface

### RichEditDocumentServer

| Property/Method | Type | Description |
|----------------|------|-------------|
| `Document` | `Document` | The document object — main access point for all content |
| `LoadDocument(path)` | `void` | Load from file; format auto-detected or explicitly specified |
| `LoadDocument(stream, format)` | `void` | Load from stream with explicit format |
| `LoadDocumentTemplate(path)` | `void` | Load as template (prevents accidental overwrite) |
| `SaveDocument(path, format)` | `void` | Save to file in the specified format |
| `ExportToPdf(path)` | `void` | Export to PDF |
| `ExportToPdf(stream, options)` | `void` | Export to PDF stream with `PdfExportOptions` |
| `Print()` | `void` | Print with default printer |
| `Print(printerSettings)` | `void` | Print with custom `PrinterSettings` |
| `Options` | `RichEditControlOptions` | Access document capabilities, printing, annotations options |
| `BeforeImport` | event | Customize import options per format |
| `BeforeExport` | event | Customize export options per format |
| `CalculateDocumentVariable` | event | Supply values for DOCVARIABLE fields |

### Document (ISubDocument)

| Property/Method | Type | Description |
|----------------|------|-------------|
| `Paragraphs` | `ParagraphCollection` | All paragraphs in the document |
| `Sections` | `SectionCollection` | Document sections (page layout) |
| `Tables` | `TableCollection` | All tables |
| `Fields` | `FieldCollection` | All fields |
| `Bookmarks` | `BookmarkCollection` | All bookmarks |
| `Hyperlinks` | `HyperlinkCollection` | All hyperlinks |
| `Shapes` | `ShapeCollection` | Inline and floating shapes/images |
| `TrackChanges` | `DocumentTrackChangesOptions` | Track changes settings |
| `Revisions` | `RevisionCollection` | All tracked revisions |
| `AppendText(text)` | `DocumentPosition` | Append text at end |
| `InsertText(pos, text)` | `DocumentPosition` | Insert text at position |
| `BeginUpdateCharacters(range)` | `CharacterProperties` | Start character format session |
| `EndUpdateCharacters(cp)` | `void` | Commit character format session |
| `BeginUpdateParagraphs(range)` | `ParagraphProperties` | Start paragraph format session |
| `EndUpdateParagraphs(pp)` | `void` | Commit paragraph format session |
| `FindAll(text, options)` | `DocumentRange[]` | Search text |
| `AppendDocumentContent(path)` | `DocumentRange` | Append content from another file |
| `InsertDocumentContent(pos, path)` | `DocumentRange` | Insert content at position |
| `SaveDocument(path, format)` | `void` | Save document from `Document` instance |

## Common Patterns

### Load, Modify, Save

```csharp
using (var server = new RichEditDocumentServer())
{
    server.LoadDocument("input.docx", DocumentFormat.Docx);
    Document doc = server.Document;

    // Modify first paragraph text color
    CharacterProperties cp = doc.BeginUpdateCharacters(doc.Paragraphs[0].Range);
    cp.ForeColor = Color.DarkRed;
    doc.EndUpdateCharacters(cp);

    server.SaveDocument("output.docx", DocumentFormat.Docx);
}
```

### Search and Replace

```csharp
DocumentRange[] found = doc.FindAll("OldText", SearchOptions.None);
foreach (DocumentRange range in found)
    doc.InsertText(range.Start, "NewText");
    // or: doc.Delete(range); doc.InsertText(range.Start, "NewText");
```

### Export to PDF

```csharp
using DevExpress.XtraPrinting;

using (var server = new RichEditDocumentServer())
{
    server.LoadDocument("input.docx", DocumentFormat.Docx);
    PdfExportOptions options = new PdfExportOptions();
    options.DocumentOptions.Author = "My App";
    server.ExportToPdf("output.pdf", options);
}
```

## Troubleshooting

| Symptom | Cause | Solution |
|---------|-------|----------|
| `FileNotFoundException` for SkiaSharp | Missing PDF renderer package on .NET 6+ | Add `DevExpress.Pdf.SkiaRenderer` NuGet package |
| `No usable version of ICU libraries` on Linux | Missing ICU library for PDF export | Set env var `DXEXPORT_ICU_VERSION_OVERRIDE=65.1` or install `libicu` |
| Document saves with wrong format | `DocumentFormat.Undefined` passed to `SaveDocument` | Always specify an explicit `DocumentFormat` enum value |
| Styles not available in new document | `RichEditDocumentServer` has no predefined styles | Create styles via `ParagraphStyles.CreateNew()` or load a template with styles |
| Mail merge produces blank fields | Field names don't match data source column names | Verify `MERGEFIELD` names match column names exactly (case-sensitive) |
| `Compare` throws exception on input documents | Input documents contain existing revisions | Accept or reject all revisions in both documents before calling `Compare` |
| Version mismatch build error | Mixed DevExpress NuGet package versions | Ensure all DX packages in the project use the exact same version |
| License error at runtime | Missing or invalid DevExpress license | Register your license key per the DevExpress installation guide |
| `NullReferenceException` on `Document` | Accessing `Document` before `LoadDocument` completes | Subscribe to `DocumentLoaded` event for safe post-load access |

## Constraints & Rules

CRITICAL — follow these rules in every interaction:

1. **Server-side only**: Always use `RichEditDocumentServer`, never `RichEditControl` (UI-only, requires WinForms/WPF).
2. **Build verification**: After making changes, verify the project builds with `dotnet build`. Check for errors before reporting success.
3. **NuGet packages**: Use `DevExpress.Document.Processor` (and `DevExpress.Pdf.SkiaRenderer` for PDF). Do not guess other package names.
4. **Namespace imports**: Always include `using DevExpress.XtraRichEdit;` and `using DevExpress.XtraRichEdit.API.Native;` plus others as needed.
5. **Version consistency**: All DevExpress packages must use the same version. Do not mix.
6. **License**: DevExpress requires a valid license. Remind the developer if they hit license-related errors.
7. **No destructive changes**: Preserve existing code structure. Only add or modify what is necessary.
8. **Framework detection**: Check the `.csproj` for target framework before writing code. Adapt for .NET vs .NET Framework.
9. **Format constant**: Never use `DocumentFormat.Undefined` for saving; always specify the target format explicitly.

## Using DevExpress Documentation MCP

If the DxDocs MCP server is available, use it to supplement this skill:

- **Search**: Use `devexpress_docs_search` with technology "Word Processing" and your question.
- **Fetch**: Use `devexpress_docs_get_content` with a documentation URL to get full article content.

**When to use MCP vs. built-in references:**
- **Built-in references**: Getting started, common patterns, key properties, troubleshooting.
- **MCP search**: Advanced scenarios not covered here, version-specific changes, uncommon features, or when the developer asks about something outside this skill's references.
- **Always MCP for**: Exact method signatures, event args, or enum values when you are not 100% certain.

---

## Next Steps

Start with **[Getting Started](references/getting-started.md)** to install and configure the Word Processing Document API, then explore specific features through the navigation guide above.

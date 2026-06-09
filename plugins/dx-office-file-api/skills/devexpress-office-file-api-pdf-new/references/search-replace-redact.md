# Find, Edit, and Redact Text

> **CTP Warning**: `DevExpress.Docs.Pdf` is a Community Technology Preview. Do not use in mission-critical production applications.

## When you need to:
- Find text in a PDF document (all pages or a page range)
- Format (bold, recolor) matched text fragments
- Permanently redact matched text
- Remove matched text
- Clear content from rectangular regions

## Search Result Structure

`PdfDocument.FindText` returns `IEnumerable<TextSearchInfo>`, one object per page.

Each `TextSearchInfo` has:
- **`Matches`** — list of `TextMatch` objects with `MatchFragments` (rectangles) — use for redaction/drawing
- **`Groups`** — list of `TextMatchGroup` with a `Fragment` (`TextFragment`) — use for formatting
- **`PageIndex`** — page index where matches were found

## Find Text

```csharp
using DevExpress.Docs.Pdf;
using System.Collections.Generic;
using System.IO;

using (PdfDocument pdfDocument = new PdfDocument(File.OpenRead("Document.pdf"))) {
    // Search pages 0–2, case-sensitive, whole-word
    IEnumerable<TextSearchInfo> results =
        pdfDocument.FindText("DevExpress",
            new TextSearchOptions(matchCase: true, wholeWordOnly: true), 0, 2);

    foreach (TextSearchInfo searchResult in results) {
        // searchResult.PageIndex, searchResult.Matches, searchResult.Groups
    }

    pdfDocument.Save(new FileStream("Document_upd.pdf", FileMode.Create, FileAccess.Write));
}
```

```vb
Imports DevExpress.Docs.Pdf
Imports System.Collections.Generic
Imports System.IO

Using pdfDocument As New PdfDocument(File.OpenRead("Document.pdf"))
    Dim results As IEnumerable(Of TextSearchInfo) =
        pdfDocument.FindText("DevExpress",
            New TextSearchOptions(True, True), 0, 2)

    For Each searchResult As TextSearchInfo In results
        ' Process results
    Next

    pdfDocument.Save(New FileStream("Document_upd.pdf", FileMode.Create, FileAccess.Write))
End Using
```

## Format Search Results

Use `TextSearchInfo.Groups` to access the underlying `TextFragment` objects and change their formatting. Call `fragment.Split(group, ...)` to split out matched vs. unmatched parts, then apply formatting.

```csharp
using DevExpress.Docs.Pdf;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

using (PdfDocument pdfDocument = new PdfDocument(File.OpenRead("Document.pdf"))) {
    IEnumerable<TextSearchInfo> results =
        pdfDocument.FindText("DevExpress",
            new TextSearchOptions(matchCase: true, wholeWordOnly: true), 0, 2);

    foreach (TextSearchInfo searchResult in results) {
        foreach (var group in searchResult.Groups) {
            // Split the fragment at match boundaries
            var newFragments = group.Fragment.Split(group,
                out TextFragment[] matched,
                out TextFragment[] notMatched);

            // Apply formatting to matched parts
            foreach (var matchedFragment in matched) {
                matchedFragment.Font = new TextFont("Arial", TextFontStyle.Bold);
                matchedFragment.ForegroundFill = Fill.CreateSolid(PdfColor.Red);
            }

            pdfDocument.Pages[searchResult.PageIndex]
                .Fragments.Replace(group.Fragment, newFragments);
        }
    }

    pdfDocument.Save(new FileStream("Result.pdf", FileMode.Create, FileAccess.Write));
}
```

```vb
Imports DevExpress.Docs.Pdf
Imports System.Collections.Generic
Imports System.Drawing
Imports System.IO

Using pdfDocument As New PdfDocument(File.OpenRead("Document.pdf"))
    Dim results As IEnumerable(Of TextSearchInfo) =
        pdfDocument.FindText("DevExpress", New TextSearchOptions(matchCase:=True, wholeWordOnly:=True), 0, 2)

    For Each searchResult As TextSearchInfo In results
        For Each group In searchResult.Groups
            Dim matched As TextFragment() = Nothing
            Dim notMatched As TextFragment() = Nothing
            Dim newFragments As TextFragment() =
                group.Fragment.Split(group, matched, notMatched)

            For Each matchedFragment In matched
                matchedFragment.Font = New TextFont("Arial", TextFontStyle.Bold)
                matchedFragment.ForegroundFill = Fill.CreateSolid(PdfColor.Red)
            Next

            pdfDocument.Pages(searchResult.PageIndex) _
                .Fragments.Replace(group.Fragment, newFragments)
        Next
    Next

    pdfDocument.Save(New FileStream("Result.pdf", FileMode.Create, FileAccess.Write))
End Using
```

## Redact Search Results

> **Warning**: `ApplyRedaction` permanently removes underlying content. This operation cannot be undone. Save to a new file.

Create `RedactionAnnotation` objects from match rectangles, then call `ApplyRedaction`.

```csharp
using DevExpress.Docs.Pdf;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

using (PdfDocument pdfDocument = new PdfDocument(
    new FileStream("Document.pdf", FileMode.Open, FileAccess.ReadWrite))) {
    IEnumerable<TextSearchInfo> results =
        pdfDocument.FindText("CONFIDENTIAL",
            new TextSearchOptions(true, false), 0, pdfDocument.Pages.Count - 1);

    foreach (var result in results) {
        var annotations = new List<RedactionAnnotation>();
        foreach (var match in result.Matches) {
            var rectangles = match.MatchFragments.Select(f => f.Rectangle).ToArray();
            annotations.Add(new RedactionAnnotation(RectangleF.Empty) {
                Geometry = new RedactionGeometry(rectangles),
                FillColor = PdfColor.Black,
                Color = PdfColor.Red,
                OverlayText = "REDACTED",
                TextJustification = TextJustification.LeftJustified,
                RepeatText = true,
                TextAppearance = new TextAppearance() {
                    FontSize = 0,
                    Fill = SolidFill.White,
                }
            });
        }
        pdfDocument.ApplyRedaction(result.PageIndex, annotations.ToArray());
    }

    pdfDocument.Save(new FileStream("Document_redacted.pdf", FileMode.Create, FileAccess.Write));
}
```

```vb
Imports DevExpress.Docs.Pdf
Imports System.Collections.Generic
Imports System.Drawing
Imports System.IO
Imports System.Linq

Using pdfDocument As New PdfDocument(
    New FileStream("Document.pdf", FileMode.Open, FileAccess.ReadWrite))

    Dim results = pdfDocument.FindText("CONFIDENTIAL",
        New TextSearchOptions(True, False), 0, pdfDocument.Pages.Count - 1)

    For Each result In results
        Dim annotations As New List(Of RedactionAnnotation)()
        For Each match In result.Matches
            Dim rectangles = match.MatchFragments.Select(Function(f) f.Rectangle).ToArray()
            Dim annotation As New RedactionAnnotation(RectangleF.Empty) With {
                .Geometry = New RedactionGeometry(rectangles),
                .FillColor = PdfColor.Black,
                .Color = PdfColor.Red,
                .OverlayText = "REDACTED",
                .TextJustification = TextJustification.LeftJustified,
                .RepeatText = True,
                .TextAppearance = New TextAppearance() With {
                    .FontSize = 0,
                    .Fill = SolidFill.White
                }
            }
            annotations.Add(annotation)
        Next
        pdfDocument.ApplyRedaction(result.PageIndex, annotations.ToArray())
    Next

    pdfDocument.Save(New FileStream("Document_redacted.pdf", FileMode.Create, FileAccess.Write))
End Using
```

### Add Redaction Without Applying

To mark redactions for review without applying them permanently:

```csharp
// Add annotation to page without applying
doc.Pages[result.PageIndex].Annotations.Add(redactionAnnotation);
// Later, apply when ready:
// doc.ApplyRedaction(pageIndex, annotations.ToArray());
```

## Remove Found Text

`RemoveText` removes text permanently from the document.

```csharp
using (PdfDocument pdfDocument = new PdfDocument(
    new FileStream("Document.pdf", FileMode.Open, FileAccess.ReadWrite))) {
    IEnumerable<TextSearchInfo> results =
        pdfDocument.FindText("DevExpress",
            new TextSearchOptions(true, true), 0, 2);

    pdfDocument.RemoveText(results);

    pdfDocument.Save(new FileStream("Result.pdf", FileMode.Create, FileAccess.Write));
}
```

```vb
Using pdfDocument As New PdfDocument(
    New FileStream("Document.pdf", FileMode.Open, FileAccess.ReadWrite))
    Dim results = pdfDocument.FindText("DevExpress",
        New TextSearchOptions(True, True), 0, 2)

    pdfDocument.RemoveText(results)

    pdfDocument.Save(New FileStream("Result.pdf", FileMode.Create, FileAccess.Write))
End Using
```

## API Reference

| Member | Description |
|--------|-------------|
| `PdfDocument.FindText(string, TextSearchOptions, int, int)` | Returns `IEnumerable<TextSearchInfo>` across page range |
| `TextSearchOptions(bool matchCase, bool wholeWordOnly)` | Search configuration |
| `TextSearchInfo.PageIndex` | Page where matches were found |
| `TextSearchInfo.Matches` | List of `TextMatch` — has `MatchFragments` with rectangles |
| `TextSearchInfo.Groups` | List of `TextMatchGroup` — has `Fragment` for formatting |
| `TextFragment.Split(group, out matched, out notMatched)` | Split fragment at match boundary |
| `FragmentCollection.Replace(old, newFragments[])` | Replace a fragment with an array |
| `RedactionAnnotation(RectangleF)` | Create a redaction annotation |
| `RedactionGeometry(RectangleF[])` | Define the region to redact |
| `PdfDocument.ApplyRedaction(pageIndex, annotations[])` | **Permanently** apply redactions |
| `PdfDocument.RemoveText(IEnumerable<TextSearchInfo>)` | Remove found text permanently |
| `PdfDocument.ClearContent(int, List<OrientedRectangle>, ...)` | Clear content in rectangular regions |

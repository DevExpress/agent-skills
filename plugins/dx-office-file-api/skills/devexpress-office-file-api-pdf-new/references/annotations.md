# Annotations — New PDF Document API

## When to Use This Reference
Add, edit, search, or remove annotations (sticky notes, highlights, watermarks, link annotations, rubber stamps, drawing annotations, comments, reviews). For redaction annotations, see `search-replace-redact.md`.

## Annotation Types

**Markup** (attached to page content): `TextAnnotation`, `TextMarkupAnnotation` (highlight/underline/strikeout/squiggly), `FreeTextAnnotation`, `CaretAnnotation`, `CircleAnnotation`, `SquareAnnotation`, `LineAnnotation`, `PolylineAnnotation`, `PolygonAnnotation`, `InkAnnotation`, `RubberStampAnnotation`, `FileAttachmentAnnotation`, `RedactionAnnotation`

**Non-markup**: `LinkAnnotation`, `WatermarkAnnotation`, `PopupAnnotation`

Access via `page.Annotations` (`AnnotationCollection`); get pages via `document.Pages`.

---

## Create an Annotation

```csharp
using DevExpress.Docs.Pdf;
using System.Drawing;
using System.IO;

using var input = File.OpenRead("Invoice.pdf");
using var document = new PdfDocument(input, new LoadOptions());

document.Pages[0].Annotations.Add(new TextAnnotation(new RectangleF(85, 405, 20, 20)) {
    Color = PdfColor.Yellow,
    Title = "Brian Zetc",
    Content = "This is an excellent overview."
});

document.Save(new FileStream("Invoice2.pdf", FileMode.Create, FileAccess.Write));
```

## Create a Watermark Annotation

```csharp
Page page = document.Pages[0];
FormTemplate form = new() { Bounds = page.MediaBox };
form.AddTextFragment(new TextFragment {
    Text = "Watermark",
    RotationAngle = 45,
    ForegroundFill = new SolidFill(PdfColor.Red),
    Location = new PointF(200, 100),
    Font = new TextFont("SegoeUI", TextFontStyle.Regular),
    FontSize = 100,
});

page.Annotations.Add(new WatermarkAnnotation(page.MediaBox) {
    Appearance = new AnnotationAppearances {
        Normal = new AnnotationAppearance { DefaultForm = form }
    }
});
```

## Search and Edit Annotations

```csharp
foreach (var page in document.Pages) {
    foreach (var annotation in page.Annotations) {
        if (annotation is TextAnnotation ta &&
            ta.Title?.IndexOf("Brian Zetc", StringComparison.OrdinalIgnoreCase) >= 0) {
            ta.Content = "Resolved";
            ta.Color = PdfColor.Green;
    }
}
```

## Remove Annotations

Remove replies before removing the annotation itself:

```csharp
for (int i = 0; i < document.Pages.Count; i++) {
    var markups = document.Pages[i].Annotations
        .OfType<MarkupAnnotation>()
        .Where(a => a.Title == "Brian Zetc")
        .ToList();

    foreach (var ann in markups) {
        foreach (var reply in document.Pages[i].Annotations
            .OfType<TextAnnotation>()
            .Where(a => a.InReplyTo == ann).ToList())
            document.Pages[i].Annotations.Remove(reply);

        document.Pages[i].Annotations.Remove(ann);
    }
}
```

## Customize Appearance

```csharp
document.Pages[0].Annotations.Add(new FreeTextAnnotation(new RectangleF(310, 50, 370, 50)) {
    Color = PdfColor.LightBlue,
    BorderEffect = new AnnotationBorderEffect {
        Style = AnnotationBorderEffectStyle.CloudyEffect, Intensity = 1 },
    Border = new AnnotationBorder {
    Border = new AnnotationBorder {
        Width = 2,
        Style = new LineStyle { DashPattern = new double[] { 1.5, 2.0 }, DashPhase = 2.5d },
        HorizontalCornerRadius = 5, VerticalCornerRadius = 5 },
    OutlineFill = new SolidFill(PdfColor.LightSalmon),
    Title = "Brian Zetc",
    Content = "A free text annotation"
});
```

---

## Link Annotations

### URL Link

```csharp
page.Fragments.Add(new TextFragment {
    Text = "The DevExpress PDF Viewer",
    Location = new PointF(60, 550),
    Font = new TextFont("Arial", TextFontStyle.Regular),
    ForegroundFill = new SolidFill(PdfColor.Red)
});

page.Annotations.Add(new LinkAnnotation(new RectangleF(50, 530, 180, 50)) {
    Action = new UriAction { Uri = "https://www.devexpress.com" },
    HighlightingMode = AnnotationHighlightingMode.Invert
});
```

### Page Link (same document)

```csharp
page.Annotations.Add(new LinkAnnotation(new RectangleF(50, 530, 180, 50)) {
    Action = new GoToAction { Destination = new FitDestination { PageIndex = 2 } },
    HighlightingMode = AnnotationHighlightingMode.Push
});
```

### Link to Another PDF

```csharp
page.Annotations.Add(new LinkAnnotation(new RectangleF(50, 530, 180, 50)) {
    Action = new RemoteGoToAction {
        Destination = new FitDestination { PageIndex = 1 },
        FileSpecification = new FileSpecification { FileName = "Invoice.pdf" },
        OpenInNewWindow = true
    }
});
```

### Wrap Existing Text in a Link

```csharp
var searchResults = document.FindText("PDF Viewer", 0, 0).ToArray();
Page page = document.Pages[searchResults[0].PageIndex];

RectangleF textRect = searchResults[0].Groups[0].Matches[0].MatchFragment.Rectangle.BoundingBox;
page.Annotations.Add(new LinkAnnotation(textRect) {
    Action = new UriAction { Uri = "https://www.devexpress.com" }
});
```

---

## Markup Annotations

### Text Highlight

```csharp
foreach (var match in document.FindText("external PDF Viewer").SelectMany(x => x.Matches)) {
    var rects = match.MatchFragments.Select(x => x.Rectangle).ToList();
    document.Pages[match.PageIndex].Annotations.Add(new TextMarkupAnnotation(rects) {
        Color = PdfColor.Yellow,
        Title = "Brian Zetc",
        Content = "This is a highlight annotation.",
        Type = TextMarkupAnnotationType.Highlight
    });
}
```

`TextMarkupAnnotationType` values: `Highlight`, `Underline`, `StrikeOut`, `Squiggly`.

### Circle Drawing Annotation

```csharp
Page page = document.Pages[0];
RectangleF rect = new RectangleF(100, 100, 150, 150);

page.Annotations.Add(new CircleAnnotation(rect) {
    Color = PdfColor.Red,
    Title = "Brian Zetc",
    Content = "This is a circle annotation."
});
```

### Rubber Stamp

```csharp
// Static stamp
document.Pages[0].Annotations.Add(new RubberStampAnnotation(new RectangleF(10, 550, 240, 50)) {
    Title = "Jesse Faden",
    IconName = RubberStampAnnotationIconName.TopSecret,
    CreationDate = DateTime.Now,
    Subject = "Approval",
    Content = "This document has been approved."
});

// Dynamic stamp (shows author + date)
document.Pages[0].Annotations.Add(
    new RubberStampAnnotation(new RectangleF(10, 550, 240, 50), RubberStampAnnotationIconName.DReviewed) {
        Title = "Jesse Faden", CreationDate = DateTime.Now
    });

// Custom stamp from another PDF page
var stamp = new RubberStampAnnotation(new RectangleF(10, 550, 240, 50));
using var iconStream = File.OpenRead("document.pdf");
stamp.SetCustomIcon(iconStream, 3);
document.Pages[0].Annotations.Add(stamp);
```

---

## Comments and Reviews

```csharp
// Add a reply (comment) to an annotation
TextAnnotation ann = /* find annotation */;
ann.AddReply(document.Pages[0], "Cardle Anita L", "Thank you!");

// Add a review
annotation.AddReview(document.Pages[0], "Cardle Anita L", ReviewStatus.Accepted);
// ReviewStatus values: None, Accepted, Rejected, Cancelled, Completed, AsIs

// Get review history
var history = annotation.GetReviewHistory(page);

// Clear all reviews
annotation.ClearReviews(page);
```

---

## Key Classes and Types

| Class / Enum | Purpose |
|---|---|
| `Page.Annotations` | `AnnotationCollection` — all annotations on a page |
| `TextAnnotation` | Sticky note (pop-up comment) |
| `TextMarkupAnnotation` | Highlight, underline, strikeout, squiggly |
| `FreeTextAnnotation` | Text box / callout on the page surface |
| `CircleAnnotation`, `SquareAnnotation` | Shape drawing annotations |
| `RubberStampAnnotation` | Approval/review stamp; `SetCustomIcon(stream, pageIndex)` |
| `WatermarkAnnotation` | Fixed-position watermark with `AnnotationAppearances` |
| `LinkAnnotation` | Clickable region; `Action` = `UriAction` / `GoToAction` / `RemoteGoToAction` |
| `AnnotationBorder`, `AnnotationBorderEffect` | Border width, dash, rounded corners, cloudy effect |
| `MarkupAnnotation.AddReply` | Thread a comment reply |
| `MarkupAnnotation.AddReview` | Attach a `ReviewStatus` review |
| `MarkupAnnotation.InReplyTo` | Link to the parent annotation (for threaded replies) |

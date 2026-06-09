# Migrate from PdfDocumentProcessor to PdfDocument

> **CTP Warning**: `DevExpress.Docs.Pdf` is a Community Technology Preview. Do not use in mission-critical production applications.

## Overview

The new `DevExpress.Docs.Pdf` namespace replaces the legacy `DevExpress.Pdf` namespace. Both are available side by side — you can migrate incrementally.

| | Legacy | New |
|-|--------|-----|
| NuGet package | `DevExpress.Document.Processor` | `DevExpress.Docs.Pdf` |
| Namespace | `DevExpress.Pdf` | `DevExpress.Docs.Pdf` |
| Entry point | `PdfDocumentProcessor` | `PdfDocument` |

## Migration Steps

1. Add the `DevExpress.Docs.Pdf` NuGet package:
   ```bash
   dotnet add package DevExpress.Docs.Pdf
   ```

2. Optionally remove `DevExpress.Document.Processor` (if no other features from it are used).

3. Replace the namespace import:
   ```csharp
   // Before:
   // using DevExpress.Pdf;
   
   // After:
   using DevExpress.Docs.Pdf;
   ```

4. Replace types using the mapping tables below.

5. Test: open/save documents, edit content, verify output.

## Before/After Example

### Legacy API

```csharp
using DevExpress.Pdf;
using System.Drawing;
using System.Linq;

using (PdfDocumentProcessor processor = new PdfDocumentProcessor()) {
    processor.LoadDocument("Result.pdf");

    // Annotations
    PdfPageFacade pageFacade = processor.DocumentFacade.Pages[0];
    var freeTextAnnotations = pageFacade.Annotations
        .Where(a => a.Type == PdfAnnotationType.FreeText);
    foreach (PdfFreeTextAnnotationFacade freeText in freeTextAnnotations) {
        freeText.InteriorColor = new PdfRGBColor(0.36, 0.54, 0.66);
        freeText.BorderWidth = 0.5;
        freeText.TextJustification = PdfTextJustification.Centered;
    }

    // Watermark (using PdfGraphics)
    using (DXSolidBrush brush = new DXSolidBrush(Color.FromArgb(63, Color.Black))) {
        using (PdfGraphics graphics = processor.CreateGraphicsPageSystem()) {
            graphics.DrawString("WATERMARK", new DXFont("Arial", 48), brush,
                new RectangleF(100, 300, 400, 100), PdfStringFormat.GenericTypographic);
            graphics.AddToPageForeground(processor.Document.Pages[0]);
        }
    }

    processor.SaveDocument("Result_1.pdf");
}
```

### New API

```csharp
using DevExpress.Docs.Pdf;
using System.Drawing;

using (PdfDocument document = new PdfDocument(
    File.OpenRead("Result.pdf"), new LoadOptions())) {

    // Annotations — direct typed access
    Page page = document.Pages[0];
    var freeTextAnnotations = page.Annotations.OfType<FreeTextAnnotation>();
    foreach (FreeTextAnnotation freeText in freeTextAnnotations) {
        freeText.Color = PdfColor.DarkSlateGray;
        freeText.Border = new AnnotationBorder { Width = 0.5 };
        freeText.TextJustification = TextJustification.Centered;
    }

    // Watermark — TextFragment on every page
    TextFragment watermark = new TextFragment {
        Text = "WATERMARK",
        Location = new PointF(150, 400),
        FontSize = 72,
        RotationAngle = 45,
        ForegroundFill = new SolidFill(PdfColor.Red, 0.2f)
    };
    for (int i = 0; i < document.Pages.Count; i++)
        document.Pages[i].Fragments.Add(watermark);

    document.Save(new FileStream("Result_1.pdf", FileMode.Create, FileAccess.Write));
}
```

## Type Mapping Tables

### Core Document Types

| Legacy (`DevExpress.Pdf`) | New (`DevExpress.Docs.Pdf`) |
|--------------------------|----------------------------|
| `PdfDocumentProcessor` | `PdfDocument` |
| `PdfDocumentFacade` | `PdfDocument` |
| `PdfPageFacade` | `Page` |
| `PdfPage` | `Page` |

### Annotation Types

| Legacy | New |
|--------|-----|
| `PdfAnnotation` | `BaseAnnotation` |
| `PdfAnnotationFacade` | `BaseAnnotation` |
| `PdfFreeTextAnnotationFacade` | `FreeTextAnnotation` |
| `PdfTextAnnotationFacade` | `TextAnnotation` |
| `PdfMarkupAnnotationFacade` | `MarkupAnnotation` |
| `PdfCircleAnnotationFacade` | `CircleAnnotation` |
| `PdfSquareAnnotationFacade` | `SquareAnnotation` |
| `PdfLineAnnotationFacade` | `LineAnnotation` |
| `PdfInkAnnotationFacade` | `InkAnnotation` |
| `PdfRedactAnnotationFacade` | `RedactionAnnotation` |
| `PdfCaretAnnotationFacade` | `CaretAnnotation` |
| `PdfRubberStampAnnotationFacade` | `RubberStampAnnotation` |
| `PdfFileAttachmentAnnotationFacade` | `FileAttachmentAnnotation` |
| `PdfPathAnnotationFacade` | `PathAnnotation` |
| `PdfPolygonAnnotationFacade` | `PolygonAnnotation` |
| `PdfPolyLineAnnotationFacade` | `PolylineAnnotation` |
| `PdfPopupAnnotation` | `PopupAnnotation` |
| `PdfLinkAnnotationFacade` | `LinkAnnotation` |
| `PdfAnnotationType` enum | Use `is` operator: `annotation is FreeTextAnnotation` |

### Widget Annotations

| Legacy | New |
|--------|-----|
| `PdfWidgetAnnotation` | `WidgetAnnotation` |
| `PdfWidgetFacade` | `WidgetAnnotation` |
| `PdfComboBoxWidgetFacade` | `ComboBoxWidgetAnnotation` |
| `PdfListBoxWidgetFacade` | `ListBoxWidgetAnnotation` |
| `PdfSignatureWidgetFacade` | `SignatureWidgetAnnotation` |
| `PdfCheckBoxWidgetFacade` | `CheckBoxWidgetAnnotation` |
| `PdfRadioButtonWidgetFacade` | `RadioGroupItemWidgetAnnotation` |
| `PdfTextWidgetFacade` | `TextBoxWidgetAnnotation` |

### Form Fields

| Legacy | New |
|--------|-----|
| `PdfAcroFormCheckBoxField` | `CheckBoxField` |
| `PdfAcroFormComboBoxField` | `ComboBoxField` |
| `PdfAcroFormGroupField` | `GroupField` |
| `PdfAcroFormListBoxField` | `ListBoxField` |
| `PdfAcroFormRadioGroupField` | `RadioGroupField` |
| `PdfAcroFormSignatureField` | `SignatureField` |
| `PdfAcroFormTextBoxField` | `TextBoxField` |

### Content Operations

| Legacy | New |
|--------|-----|
| `PdfGraphics.DrawString` | `Page.AddTextFragment(TextFragment)` |
| `PdfGraphics.DrawImage` | `Page.AddImageFragment(ImageFragment)` |
| `PdfGraphics.DrawRectangle` | `Page.AddPathFragment(PathFragment)` |
| `PdfGraphics.DrawLine` | `Page.AddPathFragment(PathFragment)` |
| `PdfGraphics.DrawBezier` | `Page.AddPathFragment(PathFragment)` |
| `PdfPageFacade.ClearContent` | `PdfDocument.ClearContent(int, List<OrientedRectangle>, ...)` |

### Encryption

| Legacy | New |
|--------|-----|
| `PdfEncryptionOptions` | `EncryptionOptions` |
| `PdfEncryptionOptions.InteractivityPermissions` | `PdfDocument.InteractivityPermissions` |
| `PdfEncryptionOptions.ModificationPermissions` | `PdfDocument.ModificationPermissions` |
| `PdfEncryptionOptions.DataExtractionPermissions` | `PdfDocument.DataExtractionPermissions` |
| `PdfEncryptionOptions.PrintingPermissions` | `PdfDocument.PrintPermissions` |

### Color, Font, and Style Types

| Legacy | New |
|--------|-----|
| `PdfRGBColor` | `PdfColor` |
| `PdfColor` | `PdfColor` |
| `PdfColorSpace` | `ColorSpace` |
| `PdfTextJustification` | `TextJustification` |
| `PdfFont` | `TextFont` (from `DevExpress.Docs.Office`) |

### Document Metadata

| Legacy | New |
|--------|-----|
| `DevExpress.Pdf.PdfMetadata` | `DevExpress.Docs.Pdf.DocumentMetadata` |
| `DevExpress.Pdf.Xmp.XmpMetadata` | `DevExpress.Docs.Pdf.XmpMetadata` |
| `DevExpress.Pdf.Xmp.XmpSimpleNode` | `DevExpress.Docs.Pdf.XmpValue` |
| `DevExpress.Pdf.Xmp.XmpArray` | `XmpBoolArray` / `XmpDateArray` / `XmpStringArray` |

## Coexistence

Both namespaces can coexist in one project. Use aliases to avoid ambiguity:

```csharp
using NewPdf = DevExpress.Docs.Pdf;
using LegacyPdf = DevExpress.Pdf;
```

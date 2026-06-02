# Content Extraction — DevExpress PDF Document API

The PDF Document API provides tools to extract text (with word and character coordinates), search for text strings across pages, and extract embedded images from PDF documents — without requiring Acrobat.

## When to Use This Reference

Use this when you need to:
- Extract all text from specific pages or the entire document
- Get word bounding boxes in page coordinates (for highlighting, indexing, etc.)
- Search for a text string and get the positions of matches
- Extract embedded images from PDF pages
- Build a text index or content-based analysis of a PDF

## Key Classes and Types

| Class | Purpose |
|-------|---------|
| `PdfDocumentProcessor.GetText(pageNumber)` | Get all words on a page as a list of `PdfWord` objects |
| `PdfDocumentProcessor.FindText(searchText)` | Search all pages for a text string |
| `PdfDocumentProcessor.GetDXImages(pageNumber)` | Extract images from a page |
| `PdfWord` | Represents a word: text, bounding rectangle in page coordinates |
| `PdfTextSearchResults` | Results of a `FindText` call — per-page word matches |
| `PdfTextSearchResult` | Words on a specific page that match the search |
| `PdfTextFindOptions` | Options for `FindText`: case sensitivity, whole word |

All coordinates returned by these methods use the **page coordinate system** (origin at bottom-left of the crop box, Y increases upward, units in points).

## Extract Text from a Page

```csharp
using DevExpress.Pdf;

using (PdfDocumentProcessor processor = new PdfDocumentProcessor())
{
    processor.LoadDocument("document.pdf");

    // GetText returns words on page 1 (1-based page number)
    IList<PdfWord> words = processor.GetText(1);

    foreach (PdfWord word in words)
    {
        Console.WriteLine($"Word: \"{word.Text}\"");

        // Bounding box in page coordinates (points, origin bottom-left)
        PdfOrientedRect bounds = word.Bounds;
        Console.WriteLine($"  Position: X={bounds.Left:F1}, Y={bounds.Bottom:F1}");
        Console.WriteLine($"  Size: W={bounds.Width:F1}, H={bounds.Height:F1}");
    }
}
```

### Concatenate All Text on a Page

```csharp
using (PdfDocumentProcessor processor = new PdfDocumentProcessor())
{
    processor.LoadDocument("document.pdf");

    for (int pageNum = 1; pageNum <= processor.Document.Pages.Count; pageNum++)
    {
        IList<PdfWord> words = processor.GetText(pageNum);
        string pageText = string.Join(" ", words.Select(w => w.Text));
        Console.WriteLine($"--- Page {pageNum} ---");
        Console.WriteLine(pageText);
    }
}
```

## Search for Text

```csharp
using DevExpress.Pdf;

using (PdfDocumentProcessor processor = new PdfDocumentProcessor())
{
    processor.LoadDocument("document.pdf");

    // Search for the phrase across all pages
    PdfTextSearchResults results = processor.FindText("invoice total");

    if (results == null)
    {
        Console.WriteLine("Text not found.");
        return;
    }

    // Iterate over pages that contain matches
    foreach (PdfTextSearchResult pageResult in results)
    {
        Console.WriteLine($"Found on page {pageResult.PageNumber}:");
        foreach (PdfWord word in pageResult.Words)
        {
            Console.WriteLine($"  \"{word.Text}\" at bounds: {word.Bounds}");
        }
    }
}
```

### Search with Options (Case-Insensitive, Whole Word)

```csharp
PdfTextFindOptions options = new PdfTextFindOptions
{
    CaseSensitive = false,
    WholeWord = true
};

PdfTextSearchResults results = processor.FindText("Total", options);
```

### Highlight Search Results

After finding text, convert the page coordinate bounds to world coordinates to draw highlights with `PdfGraphics.AddToPageForeground`:

```csharp
using DevExpress.Drawing;
using System.Drawing;

using (PdfDocumentProcessor processor = new PdfDocumentProcessor())
{
    processor.LoadDocument("document.pdf");
    PdfTextSearchResults results = processor.FindText("important");

    if (results != null)
    {
        foreach (PdfTextSearchResult pageResult in results)
        {
            // Get the actual page object
            PdfPage page = processor.Document.Pages[pageResult.PageNumber - 1];

            using (PdfGraphics graphics = processor.CreateGraphicsPageSystem())
            {
                foreach (PdfWord word in pageResult.Words)
                {
                    // Convert page coordinates to world coordinates for drawing
                    // Page system: origin bottom-left, Y up (points)
                    // Page bounds give us the crop box dimensions
                    double pageHeight = page.CropBox.Height;

                    // In page coordinate system, Y from bottom; invert for drawing
                    RectangleF highlightRect = new RectangleF(
                        (float)word.Bounds.Left,
                        (float)(pageHeight - word.Bounds.Top),
                        (float)word.Bounds.Width,
                        (float)word.Bounds.Height);

                    graphics.FillRectangle(
                        new DXSolidBrush(Color.FromArgb(128, Color.Yellow)),
                        highlightRect);
                }

                graphics.AddToPageForeground(page);
            }
        }

        processor.SaveDocument("highlighted.pdf");
    }
}
```

> **Coordinate note**: `GetText` and `FindText` return coordinates in the page coordinate system (Y from bottom-left, upward). To draw highlights using `CreateGraphicsPageSystem`, you may need to invert the Y axis depending on your specific page rotation and crop box. Refer to the DevExpress coordinate systems documentation for the exact conversion formula.

## Extract Images

```csharp
using DevExpress.Drawing;
using DevExpress.Pdf;
using System.IO;

using (PdfDocumentProcessor processor = new PdfDocumentProcessor())
{
    processor.LoadDocument("document.pdf");

    for (int pageNum = 1; pageNum <= processor.Document.Pages.Count; pageNum++)
    {
        // Returns a list of DXImage objects extracted from the page
        IList<DXImage> images = processor.GetDXImages(pageNum);

        int imgIndex = 0;
        foreach (DXImage image in images)
        {
            string outputPath = $"page{pageNum}_image{imgIndex}.png";
            image.Save(outputPath);
            Console.WriteLine($"Saved: {outputPath} ({image.Width}x{image.Height})");
            imgIndex++;
        }
    }
}
```

## PdfWord Members

| Member | Type | Description |
|--------|------|-------------|
| `Text` | `string` | The word's text content |
| `Bounds` | `PdfOrientedRect` | Bounding rectangle in page coordinates |
| `Bounds.Left` | `double` | Left edge (points from left of crop box) |
| `Bounds.Bottom` | `double` | Bottom edge (points from bottom of crop box) |
| `Bounds.Width` | `double` | Width in points |
| `Bounds.Height` | `double` | Height in points |

## Troubleshooting

- **`GetText` returns empty list**: The page may use image-based text (scanned document). The PDF Document API extracts only programmatic text, not OCR content. Use an OCR library separately for scanned PDFs.
- **Word coordinates seem wrong**: Coordinates are in the page coordinate system (origin = bottom-left of crop box). If the page is rotated, coordinates rotate accordingly. Convert to world coordinates for drawing.
- **`FindText` returns `null`**: The exact text was not found. Try case-insensitive search with `PdfTextFindOptions.CaseSensitive = false`. Hyphenated or ligature text may split differently than expected.
- **`GetDXImages` returns empty**: Some PDFs render images via patterns or XObjects rather than embedded images. Not all visual images are extractable as discrete image resources.
- **Images are low resolution**: The extracted resolution matches what is embedded in the PDF, not the displayed resolution. The PDF may use upscaled low-resolution images internally.

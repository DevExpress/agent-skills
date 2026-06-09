// DevExpress PDF Document API — Quick Start Console Application
// Creates a new PDF with a title string and a colored rectangle, then saves it.
//
// NuGet packages required:
//   dotnet add package DevExpress.Document.Processor
//   dotnet add package DevExpress.Pdf.SkiaRenderer   (.NET 6+ only)
//
// Namespaces: DevExpress.Pdf, DevExpress.Drawing

using DevExpress.Drawing;
using DevExpress.Pdf;
using System.Drawing;

// Output path — saves to the current working directory
string outputPath = "QuickStart.pdf";

using (PdfDocumentProcessor processor = new PdfDocumentProcessor())
{
    // Step 1: Create an empty PDF document at the output path.
    // This creates the file on disk immediately (no in-memory buffer).
    processor.CreateEmptyDocument(outputPath);

    // Step 2: Create a graphics context in the page coordinate system.
    //   - Origin (0,0) is at the bottom-left of the page crop box.
    //   - Y increases upward.
    //   - Units are points (1 point = 1/72 inch).
    // Always dispose PdfGraphics after calling RenderNewPage.
    using (PdfGraphics graph = processor.CreateGraphicsPageSystem())
    {
        // Step 3: Draw a filled rectangle (banner background).
        // RectangleF(x, y, width, height) — in page coordinates.
        // Y=700 puts the banner near the top of a Letter page (792 pt tall).
        graph.FillRectangle(
            new DXSolidBrush(Color.FromArgb(68, 114, 196)),  // DevExpress blue
            new RectangleF(30f, 700f, 550f, 60f));

        // Step 4: Draw a title string inside the banner (white text).
        // DXFont(familyName, emSize) — emSize is in points.
        using (DXFont titleFont = new DXFont("Arial", 22, DXFontStyle.Bold))
        {
            graph.DrawString(
                "DevExpress PDF Document API",
                titleFont,
                new DXSolidBrush(Color.White),
                45f,   // X — left margin inside banner
                712f); // Y — baseline from bottom; ~midpoint of the banner
        }

        // Step 5: Draw a body paragraph below the banner.
        using (DXFont bodyFont = new DXFont("Arial", 12))
        {
            graph.DrawString(
                "This PDF was created programmatically without Adobe Acrobat " +
                "using the DevExpress PDF Document API (v25.2).",
                bodyFont,
                new DXSolidBrush(Color.Black),
                30f,   // X
                660f); // Y — below the banner
        }

        // Step 6: Draw a rectangle border around the body area.
        graph.DrawRectangle(
            new DXPen(Color.FromArgb(68, 114, 196), 1.5f),
            new RectangleF(30f, 30f, 550f, 720f));

        // Step 7: Render all drawn content as a new Letter-size page.
        // PdfPaperSize.Letter = 612 x 792 points (8.5 x 11 inches).
        // RenderNewPage appends the page to the document.
        processor.RenderNewPage(PdfPaperSize.Letter, graph);
    }

    // The document is saved and closed when PdfDocumentProcessor is disposed.
}

Console.WriteLine($"PDF created successfully: {Path.GetFullPath(outputPath)}");

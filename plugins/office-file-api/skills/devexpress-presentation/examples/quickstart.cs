// DevExpress Presentation API — Quick Start Example
// Requires NuGet package: DevExpress.Document.Processor
// Optional for PDF export: DevExpress.Pdf.SkiaRenderer
//
// This console application:
//   1. Creates a new PPTX presentation
//   2. Configures a slide master background
//   3. Adds a title slide with a centered title shape and subtitle
//   4. Adds a content slide with a text shape saying "Hello DevExpress Presentation API!"
//   5. Adds a shape with formatted text (bold, colored run)
//   6. Exports the result to output.pptx

using DevExpress.Docs.Presentation;
using System.Drawing;
using System.IO;

namespace PresentationQuickStart;

public class Program
{
    public static void Main(string[] args)
    {
        // ---------------------------------------------------------------
        // Step 1: Create a new presentation
        // A default presentation starts with one empty slide (Title layout).
        // ---------------------------------------------------------------
        Presentation presentation = new Presentation();

        // Remove the default slide so we control exactly what slides appear.
        presentation.Slides.Clear();

        // ---------------------------------------------------------------
        // Step 2: Configure the slide master
        // The slide master defines appearance inherited by all slides.
        // ---------------------------------------------------------------
        SlideMaster master = presentation.SlideMasters[0];
        master.Background = new CustomSlideBackground(
            new SolidFill(Color.FromArgb(230, 240, 255))  // Light blue background
        );

        // ---------------------------------------------------------------
        // Step 3: Add a title slide
        // Use the built-in Title layout (CenteredTitle + Subtitle placeholders).
        // ---------------------------------------------------------------
        Slide titleSlide = new Slide(master.Layouts.Get(SlideLayoutType.Title));

        foreach (Shape shape in titleSlide.Shapes)
        {
            if (shape.PlaceholderSettings.Type is PlaceholderType.CenteredTitle)
            {
                shape.TextArea = new TextArea("DevExpress Presentation API");
            }
            if (shape.PlaceholderSettings.Type is PlaceholderType.Subtitle)
            {
                shape.TextArea = new TextArea("Quick Start Example — .NET Console App");
            }
        }

        presentation.Slides.Add(titleSlide);

        // ---------------------------------------------------------------
        // Step 4: Add a content slide with a text shape
        // Uses a blank layout so we can position shapes freely.
        // ---------------------------------------------------------------
        Slide contentSlide = new Slide(new SlideLayout(SlideLayoutType.Blank, "blank"));
        presentation.Slides.Add(contentSlide);

        // Add the primary text shape with the required greeting
        // Position: x=200, y=800, width=3600, height=600 (document units: 1/300 inch)
        Shape helloShape = new Shape(ShapeType.Rectangle, 200f, 800f, 3600f, 600f);
        helloShape.Fill = new SolidFill(Color.FromArgb(70, 130, 180));       // Steel blue fill
        helloShape.Outline = new LineStyle
        {
            Fill = new SolidFill(Color.FromArgb(30, 80, 130)),
            Width = 3
        };

        // Build the TextArea with a paragraph containing a single run
        TextArea helloTextArea = new TextArea();
        helloTextArea.Paragraphs.Clear();

        TextParagraph helloParagraph = new TextParagraph();
        helloParagraph.Properties = new ParagraphProperties
        {
            Alignment = TextParagraphAlignment.Center
        };
        helloParagraph.Runs.Add(new TextRun
        {
            Text = "Hello DevExpress Presentation API!",
            TextProperties = new TextProperties
            {
                Fill = new SolidFill(Color.White),
                Bold = true,
                FontSize = 28
            }
        });
        helloTextArea.Paragraphs.Add(helloParagraph);
        helloShape.TextArea = helloTextArea;
        contentSlide.Shapes.Add(helloShape);

        // ---------------------------------------------------------------
        // Step 5: Add a second shape with mixed-format text
        // Demonstrates paragraph runs with different formatting.
        // ---------------------------------------------------------------
        Shape infoShape = new Shape(ShapeType.RoundedRectangle, 200f, 1600f, 3600f, 450f);
        infoShape.Fill = new SolidFill(Color.FromArgb(245, 248, 255));
        infoShape.Outline = new LineStyle
        {
            Fill = new SolidFill(Color.FromArgb(180, 200, 230)),
            Width = 2
        };

        TextArea infoTextArea = new TextArea();
        infoTextArea.Paragraphs.Clear();

        // First paragraph: plain text
        TextParagraph infoPara1 = new TextParagraph();
        infoPara1.Runs.Add(new TextRun { Text = "Created with " });
        infoPara1.Runs.Add(new TextRun
        {
            Text = "DevExpress.Document.Processor",
            TextProperties = new TextProperties
            {
                Fill = new SolidFill(Color.FromArgb(0, 80, 160)),
                Bold = true
            }
        });
        infoTextArea.Paragraphs.Add(infoPara1);

        // Second paragraph: italic note
        TextParagraph infoPara2 = new TextParagraph();
        infoPara2.Runs.Add(new TextRun
        {
            Text = "No Microsoft Office required.",
            TextProperties = new TextProperties
            {
                Italic = true,
                Fill = new SolidFill(Color.FromArgb(80, 80, 80))
            }
        });
        infoTextArea.Paragraphs.Add(infoPara2);

        infoShape.TextArea = infoTextArea;
        contentSlide.Shapes.Add(infoShape);

        // ---------------------------------------------------------------
        // Step 6: Add slide numbers and a footer to all slides
        // ---------------------------------------------------------------
        presentation.HeaderFooterManager.AddSlideNumberPlaceholder(presentation.Slides);
        presentation.HeaderFooterManager.AddFooterPlaceholder(
            presentation.Slides,
            "DevExpress Presentation API Quick Start"
        );

        // ---------------------------------------------------------------
        // Step 7: Save as output.pptx
        // ---------------------------------------------------------------
        string outputPath = "output.pptx";
        using (FileStream outStream = new FileStream(outputPath, FileMode.Create, FileAccess.ReadWrite))
        {
            presentation.SaveDocument(outStream);
        }

        Console.WriteLine($"Presentation saved to: {Path.GetFullPath(outputPath)}");
        Console.WriteLine($"Slides: {presentation.Slides.Count}");
        Console.WriteLine("Open output.pptx in PowerPoint or any compatible viewer.");

        // ---------------------------------------------------------------
        // Optional: Export to PDF (requires DevExpress.Pdf.SkiaRenderer)
        // ---------------------------------------------------------------
        // string pdfPath = "output.pdf";
        // presentation.ExportToPdf(pdfPath);
        // Console.WriteLine($"PDF exported to: {Path.GetFullPath(pdfPath)}");
    }
}

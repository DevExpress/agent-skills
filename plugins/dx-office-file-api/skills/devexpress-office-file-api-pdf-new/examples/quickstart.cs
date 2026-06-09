// DevExpress.Docs.Pdf — New PDF Document API (CTP) Quick Start Example
// NuGet: dotnet add package DevExpress.Docs.Pdf
// NOTE: This is a Community Technology Preview. Do not use in mission-critical production applications.

using DevExpress.Docs.Pdf;
using DevExpress.Docs.Office;
using DevExpress.Drawing;
using DevExpress.Drawing.Printing;
using System.Drawing;
using System.IO;

namespace DxNewPdfQuickStart;

public class Program
{
    public static void Main(string[] _)
    {
        // ---------------------------------------------------------------
        // Example 1: Create a new multi-page PDF document
        // ---------------------------------------------------------------
        CreateNewDocument();

        // ---------------------------------------------------------------
        // Example 2: Load an existing PDF and add a watermark to every page
        // ---------------------------------------------------------------
        // AddWatermark("input.pdf", "output_watermarked.pdf");

        // ---------------------------------------------------------------
        // Example 3: Encrypt a PDF with AES-256
        // ---------------------------------------------------------------
        // EncryptDocument("input.pdf", "output_secured.pdf");
    }

    static void CreateNewDocument()
    {
        using (PdfDocument pdfDocument = new PdfDocument())
        {
            // Page 1: title + body text + image
            Page page1 = pdfDocument.Pages.Add(DXPaperKind.A4);

            // Title
            page1.AddFragment(new TextFragment
            {
                Text = "Quarterly Sales Report",
                Location = new PointF(50, 770),
                Font = new TextFont("Arial", TextFontStyle.Bold),
                FontSize = 24
            });

            // Body text
            page1.AddFragment(new TextFragment
            {
                Text = "This report summarizes sales data for Q1 2026.",
                Location = new PointF(50, 730),
                Font = new TextFont("Arial"),
                FontSize = 12
            });

            // Multiline paragraph
            var paragraph = new ParagraphFragment
            {
                Text = "Q1 results exceeded targets across all regions. " +
                       "North America led with 35% growth YoY, followed by EMEA at 22%.",
                Location = new PointF(50, 690),
                Font = new DXFont("Arial", 11),
                Width = 495,
                Height = 100,
                StringFormat = new DXStringFormat
                {
                    Alignment = DXStringAlignment.Near,
                    LineAlignment = DXStringAlignment.Near
                }
            };
            page1.Fragments.Add(paragraph);

            // Logo image (comment out if file doesn't exist)
            // DXImage logo = DXImage.FromFile("logo.png");
            // page1.AddImageFragment(new ImageFragment(logo)
            // {
            //     Location = new PointF(400, 750),
            //     Size = new SizeF(145, 40)
            // });

            // Decorative rectangle
            var rect = PathFragment.Rectangle(new RectangleF(50, 650, 495, 2));
            rect.Fill = Fill.CreateSolid(new PdfColor(0, 0, 139));
            page1.Fragments.Add(rect);

            // Page 2: data table rows
            Page page2 = pdfDocument.Pages.Add(DXPaperKind.A4);

            page2.AddFragment(new TextFragment
            {
                Text = "Regional Breakdown",
                Location = new PointF(50, 780),
                Font = new TextFont("Arial", TextFontStyle.Bold),
                FontSize = 16
            });

            string[] regions = { "North America", "EMEA", "APAC", "LATAM" };
            string[] values  = { "$1,250,000",     "$980,000", "$740,000", "$310,000" };
            float y = 740;
            for (int i = 0; i < regions.Length; i++)
            {
                page2.AddFragment(new TextFragment
                {
                    Text = regions[i],
                    Location = new PointF(50, y),
                    Font = new TextFont("Arial"),
                    FontSize = 12
                });
                page2.AddFragment(new TextFragment
                {
                    Text = values[i],
                    Location = new PointF(300, y),
                    Font = new TextFont("Arial"),
                    FontSize = 12
                });
                y -= 25;
            }

            // Save
            using (FileStream fs = File.Create("SalesReport.pdf"))
                pdfDocument.Save(fs);
        }

        Console.WriteLine("Created: SalesReport.pdf");
    }

    static void AddWatermark(string inputPath, string outputPath)
    {
        using (PdfDocument doc = new PdfDocument(File.OpenRead(inputPath)))
        {
            TextFragment watermark = new TextFragment
            {
                Text = "DRAFT",
                Location = new PointF(150, 400),
                FontSize = 72,
                RotationAngle = 45,
                ForegroundFill = new SolidFill(PdfColor.Red, 0.2f)
            };

            for (int i = 0; i < doc.Pages.Count; i++)
                doc.Pages[i].Fragments.Add(watermark);

            doc.Save(new FileStream(outputPath, FileMode.Create, FileAccess.Write));
        }

        Console.WriteLine($"Watermarked: {outputPath}");
    }

    static void EncryptDocument(string inputPath, string outputPath)
    {
        using (PdfDocument doc = new PdfDocument(File.OpenRead(inputPath)))
        {
            var opts = new EncryptionOptions("ownerPass", "userPass")
            {
                Algorithm = EncryptionAlgorithm.AES256,
                PrintPermissions = DocumentPrintPermissions.LowQuality,
                ModificationPermissions = DocumentModificationPermissions.NotAllowed
            };
            doc.Encrypt(opts);
            doc.Save(new FileStream(outputPath, FileMode.Create, FileAccess.Write));
        }

        Console.WriteLine($"Encrypted: {outputPath}");
    }
}

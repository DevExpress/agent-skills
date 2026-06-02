// DevExpress Word Processing Document API — Quick Start Console Application
// Creates a Word document with a heading paragraph, a body paragraph,
// a formatted 3x3 table, and saves as QuickStartDocument.docx.
//
// Requirements:
//   dotnet add package DevExpress.Document.Processor
//   dotnet add package DevExpress.Pdf.SkiaRenderer  (for PDF export)
//
// Namespaces used:
//   DevExpress.XtraRichEdit
//   DevExpress.XtraRichEdit.API.Native
//   DevExpress.Office.Utils
//   System.Drawing

using System;
using System.Drawing;
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;
using DevExpress.Office.Utils;

class Program
{
    static void Main(string[] args)
    {
        string outputPath = "QuickStartDocument.docx";

        using (var server = new RichEditDocumentServer())
        {
            Document doc = server.Document;

            // ----------------------------------------------------------------
            // 1. Heading Paragraph
            // ----------------------------------------------------------------
            doc.AppendText("Word Processing Document API — Quick Start");

            // Apply character formatting to the heading
            CharacterProperties headingCp = doc.BeginUpdateCharacters(doc.Paragraphs[0].Range);
            headingCp.FontName = "Segoe UI";
            headingCp.FontSize = 22;
            headingCp.Bold = true;
            headingCp.ForeColor = Color.FromArgb(31, 73, 125); // Dark blue
            doc.EndUpdateCharacters(headingCp);

            // Apply paragraph formatting to the heading
            ParagraphProperties headingPp = doc.BeginUpdateParagraphs(doc.Paragraphs[0].Range);
            headingPp.Alignment = ParagraphAlignment.Center;
            headingPp.SpacingAfter = Units.InchesToDocumentsF(0.2f);
            headingPp.SpacingBefore = Units.InchesToDocumentsF(0.1f);
            doc.EndUpdateParagraphs(headingPp);

            // ----------------------------------------------------------------
            // 2. Body Paragraph
            // ----------------------------------------------------------------
            Paragraph bodyPara = doc.Paragraphs.Append();
            doc.InsertText(bodyPara.Range.Start,
                "This document was created programmatically with the DevExpress " +
                "Word Processing Document API. It demonstrates paragraphs, text " +
                "formatting, and table creation without Microsoft Office.");

            // Apply character formatting to the body paragraph
            CharacterProperties bodyCp = doc.BeginUpdateCharacters(bodyPara.Range);
            bodyCp.FontName = "Calibri";
            bodyCp.FontSize = 11;
            bodyCp.ForeColor = Color.FromArgb(64, 64, 64); // Dark gray
            doc.EndUpdateCharacters(bodyCp);

            // Apply paragraph formatting to the body paragraph
            ParagraphProperties bodyPp = doc.BeginUpdateParagraphs(bodyPara.Range);
            bodyPp.Alignment = ParagraphAlignment.Justify;
            bodyPp.SpacingAfter = Units.InchesToDocumentsF(0.15f);
            bodyPp.LineSpacingType = ParagraphLineSpacing.Multiple;
            bodyPp.LineSpacingMultiplier = 1.15f;
            doc.EndUpdateParagraphs(bodyPp);

            // ----------------------------------------------------------------
            // 3. Section Label Before the Table
            // ----------------------------------------------------------------
            Paragraph labelPara = doc.Paragraphs.Append();
            doc.InsertText(labelPara.Range.Start, "Quarterly Sales Summary");

            CharacterProperties labelCp = doc.BeginUpdateCharacters(labelPara.Range);
            labelCp.FontName = "Segoe UI";
            labelCp.FontSize = 13;
            labelCp.Bold = true;
            labelCp.ForeColor = Color.FromArgb(31, 73, 125);
            doc.EndUpdateCharacters(labelCp);

            ParagraphProperties labelPp = doc.BeginUpdateParagraphs(labelPara.Range);
            labelPp.SpacingBefore = Units.InchesToDocumentsF(0.1f);
            labelPp.SpacingAfter = Units.InchesToDocumentsF(0.05f);
            doc.EndUpdateParagraphs(labelPp);

            // ----------------------------------------------------------------
            // 4. Table — 3 rows x 3 columns
            // ----------------------------------------------------------------
            // Append an empty paragraph then create the table after it
            doc.Paragraphs.Append();
            DocumentPosition tablePos = doc.Paragraphs[doc.Paragraphs.Count - 1].Range.Start;

            Table table = doc.Tables.Create(tablePos, 3, 3);

            // Set fixed column widths and table layout
            table.BeginUpdate();
            table.TableLayout = TableLayoutType.Fixed;
            table.PreferredWidthType = WidthType.Fixed;
            table.PreferredWidth = Units.InchesToDocumentsF(5.5f);
            table.TableAlignment = TableRowAlignment.Left;

            foreach (TableRow row in table.Rows)
            {
                row.Cells[0].PreferredWidthType = WidthType.Fixed;
                row.Cells[0].PreferredWidth = Units.InchesToDocumentsF(2.5f);
                row.Cells[1].PreferredWidthType = WidthType.Fixed;
                row.Cells[1].PreferredWidth = Units.InchesToDocumentsF(1.5f);
                row.Cells[2].PreferredWidthType = WidthType.Fixed;
                row.Cells[2].PreferredWidth = Units.InchesToDocumentsF(1.5f);
            }
            table.EndUpdate();

            // Define header data
            string[] headers = { "Product", "Q1 Sales", "Q2 Sales" };

            // Define data rows
            string[][] data =
            {
                new[] { "Widget A", "$15,000", "$18,500" },
                new[] { "Widget B", "$22,000", "$19,800" },
            };

            // --- Populate and format ---
            table.BeginUpdate();

            // Header row (row 0)
            for (int col = 0; col < headers.Length; col++)
            {
                TableCell headerCell = table[0, col];
                doc.InsertText(headerCell.Range.Start, headers[col]);

                // Dark blue background
                headerCell.BackgroundColor = Color.FromArgb(31, 73, 125);
                headerCell.VerticalAlignment = TableCellVerticalAlignment.Center;

                // Bold white text, centered
                CharacterProperties hcp = doc.BeginUpdateCharacters(headerCell.Range);
                hcp.FontName = "Calibri";
                hcp.FontSize = 11;
                hcp.Bold = true;
                hcp.ForeColor = Color.White;
                doc.EndUpdateCharacters(hcp);

                ParagraphProperties hpp = doc.BeginUpdateParagraphs(headerCell.Range);
                hpp.Alignment = ParagraphAlignment.Center;
                doc.EndUpdateParagraphs(hpp);
            }

            // Data rows (rows 1–2)
            for (int row = 0; row < data.Length; row++)
            {
                for (int col = 0; col < data[row].Length; col++)
                {
                    TableCell cell = table[row + 1, col];
                    doc.InsertText(cell.Range.Start, data[row][col]);

                    // Alternate row shading on even data rows
                    if (row % 2 == 1)
                        cell.BackgroundColor = Color.FromArgb(217, 225, 242); // Light blue

                    CharacterProperties dcp = doc.BeginUpdateCharacters(cell.Range);
                    dcp.FontName = "Calibri";
                    dcp.FontSize = 11;
                    dcp.ForeColor = Color.FromArgb(64, 64, 64);
                    doc.EndUpdateCharacters(dcp);

                    ParagraphProperties dpp = doc.BeginUpdateParagraphs(cell.Range);
                    dpp.Alignment = (col == 0) ? ParagraphAlignment.Left : ParagraphAlignment.Right;
                    doc.EndUpdateParagraphs(dpp);
                }
            }

            // Apply single-line borders to all cells
            foreach (TableRow row in table.Rows)
            {
                foreach (TableCell cell in row.Cells)
                {
                    cell.Borders.Left.LineStyle = TableBorderLineStyle.Single;
                    cell.Borders.Left.LineThickness = 1;
                    cell.Borders.Right.LineStyle = TableBorderLineStyle.Single;
                    cell.Borders.Right.LineThickness = 1;
                    cell.Borders.Top.LineStyle = TableBorderLineStyle.Single;
                    cell.Borders.Top.LineThickness = 1;
                    cell.Borders.Bottom.LineStyle = TableBorderLineStyle.Single;
                    cell.Borders.Bottom.LineThickness = 1;
                }
            }

            table.EndUpdate();

            // ----------------------------------------------------------------
            // 5. Save as DOCX
            // ----------------------------------------------------------------
            server.SaveDocument(outputPath, DocumentFormat.Docx);
        }

        Console.WriteLine($"Document created successfully: {outputPath}");

        // Optional: open the file (Windows / macOS / Linux with default viewer)
        // System.Diagnostics.Process.Start(
        //     new System.Diagnostics.ProcessStartInfo(outputPath) { UseShellExecute = true });
    }
}

// DevExpress Spreadsheet Document API — Quickstart Example
// NuGet: DevExpress.Document.Processor
// NuGet: DevExpress.Pdf.SkiaRenderer  (required for PDF export on .NET 6/7/8+)
//
// Usage:
//   dotnet add package DevExpress.Document.Processor
//   dotnet add package DevExpress.Pdf.SkiaRenderer
//   Then run this program.

using DevExpress.Spreadsheet;
using System;
using System.Drawing;

class QuickstartDemo
{
    static void Main()
    {
        string xlsxPath = "QuarterlyReport.xlsx";

        using (Workbook workbook = new Workbook())
        {
            Worksheet sheet = workbook.Worksheets[0];
            sheet.Name = "Q1 Report";

            // Batch all edits for better performance
            workbook.BeginUpdate();
            try
            {
                // ── Headers ──────────────────────────────────────────────────
                sheet["A1"].Value = "Product";
                sheet["B1"].Value = "Unit Price";
                sheet["C1"].Value = "Units Sold";
                sheet["D1"].Value = "Revenue";
                sheet["E1"].Value = "Target";
                sheet["F1"].Value = "vs Target";

                // Style the header row
                CellRange headerRow = sheet.Range["A1:F1"];
                Formatting headerFmt = headerRow.BeginUpdateFormatting();
                headerFmt.Font.Bold = true;
                headerFmt.Font.Size = 12;
                headerFmt.Font.Color = Color.White;
                headerFmt.Fill.BackgroundColor = Color.FromArgb(68, 114, 196);
                headerFmt.Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
                headerFmt.Alignment.Vertical = SpreadsheetVerticalAlignment.Center;
                headerFmt.Borders.SetOutsideBorders(Color.White, BorderLineStyle.Thin);
                headerRow.EndUpdateFormatting(headerFmt);
                sheet.Rows[0].Height = 28;

                // ── Data ─────────────────────────────────────────────────────
                object[,] data =
                {
                    { "Widget A",   29.99, 1250, null, 35000 },
                    { "Widget B",   49.99, 870,  null, 40000 },
                    { "Gadget X",   99.00, 430,  null, 50000 },
                    { "Gadget Pro", 149.99, 210,  null, 35000 },
                    { "Accessory Z", 9.99, 3800, null, 30000 },
                };

                int dataStartRow = 1; // 0-based index (row 2 in Excel)
                int dataRowCount = data.GetLength(0);

                for (int i = 0; i < dataRowCount; i++)
                {
                    int rowIdx = dataStartRow + i;
                    sheet.Cells[rowIdx, 0].Value = (string)data[i, 0];  // Product
                    sheet.Cells[rowIdx, 1].Value = (double)data[i, 1];  // Unit Price
                    sheet.Cells[rowIdx, 2].Value = (int)data[i, 2];     // Units Sold
                    // Column D (Revenue) = Unit Price * Units Sold — formula
                    sheet.Cells[rowIdx, 3].FormulaInvariant = $"=B{rowIdx + 1}*C{rowIdx + 1}";
                    sheet.Cells[rowIdx, 4].Value = (int)data[i, 4];     // Target
                    // Column F = Revenue vs Target (percentage difference)
                    sheet.Cells[rowIdx, 5].FormulaInvariant = $"=(D{rowIdx + 1}-E{rowIdx + 1})/E{rowIdx + 1}";
                }

                // ── Totals Row ───────────────────────────────────────────────
                int totalsRow = dataStartRow + dataRowCount; // 0-based
                sheet.Cells[totalsRow, 0].Value = "TOTAL";
                sheet.Cells[totalsRow, 0].Font.Bold = true;
                sheet.Cells[totalsRow, 3].FormulaInvariant = $"=SUM(D{dataStartRow + 1}:D{totalsRow})";
                sheet.Cells[totalsRow, 4].FormulaInvariant = $"=SUM(E{dataStartRow + 1}:E{totalsRow})";
                sheet.Cells[totalsRow, 5].FormulaInvariant = $"=(D{totalsRow + 1}-E{totalsRow + 1})/E{totalsRow + 1}";

                // Style the totals row
                CellRange totalsRange = sheet.Range.FromLTRB(0, totalsRow, 5, totalsRow);
                Formatting totalsFmt = totalsRange.BeginUpdateFormatting();
                totalsFmt.Font.Bold = true;
                totalsFmt.Fill.BackgroundColor = Color.FromArgb(217, 225, 242);
                totalsFmt.Borders.TopBorder.LineStyle = BorderLineStyle.Double;
                totalsFmt.Borders.TopBorder.Color = Color.FromArgb(68, 114, 196);
                totalsRange.EndUpdateFormatting(totalsFmt);

                // ── Number Formats ───────────────────────────────────────────
                // Unit Price column (B): currency with 2 decimals
                sheet.Columns["B"].NumberFormat = "$#,##0.00";

                // Units Sold column (C): integer with thousands separator
                sheet.Columns["C"].NumberFormat = "#,##0";

                // Revenue and Target columns (D, E): currency
                sheet.Range["D2:E7"].NumberFormat = "$#,##0";

                // vs Target column (F): percentage with 1 decimal
                sheet.Range["F2:F7"].NumberFormat = "0.0%;[Red]-0.0%";

                // ── Alternating Row Colors ───────────────────────────────────
                for (int i = 0; i < dataRowCount; i++)
                {
                    if (i % 2 == 1)
                    {
                        int rowIdx = dataStartRow + i;
                        sheet.Range.FromLTRB(0, rowIdx, 5, rowIdx).FillColor =
                            Color.FromArgb(221, 235, 247);
                    }
                }

                // ── Data Borders ─────────────────────────────────────────────
                CellRange dataRange = sheet.Range.FromLTRB(0, dataStartRow, 5, totalsRow);
                dataRange.Borders.SetInsideBorders(Color.LightGray, BorderLineStyle.Thin);
                dataRange.Borders.SetOutsideBorders(Color.FromArgb(68, 114, 196), BorderLineStyle.Medium);

                // ── Column Widths ────────────────────────────────────────────
                sheet.Columns["A"].Width = 18;
                sheet.Columns.AutoFit(1, 5); // Auto-fit B through F
            }
            finally
            {
                workbook.EndUpdate();
            }

            // Calculate all formulas before saving
            workbook.Calculate();

            // ── Save as .xlsx ────────────────────────────────────────────────
            workbook.SaveDocument(xlsxPath, DocumentFormat.Xlsx);
            Console.WriteLine($"Saved: {xlsxPath}");

            // ── Export to PDF ────────────────────────────────────────────────
            // Requires DevExpress.Pdf.SkiaRenderer on .NET 6/7/8+
            // On .NET Framework, Windows GDI is used automatically
            try
            {
                // Configure page setup for a clean PDF
                sheet.ActiveView.Orientation = PageOrientation.Landscape;
                var printOpts = sheet.ActiveView.PrintOptions;
                printOpts.FitToPage = true;
                printOpts.FitToWidth = 1;
                printOpts.FitToHeight = 0;
                sheet.ActiveView.CenterHorizontally = true;
                sheet.HeaderFooterOptions.OddHeader = "&CQuarterly Report";
                sheet.HeaderFooterOptions.OddFooter = "&LGenerated: &[Date]&RPage &[Page] of &[Pages]";

                string pdfPath = System.IO.Path.ChangeExtension(xlsxPath, ".pdf");
                workbook.ExportToPdf(pdfPath);
                Console.WriteLine($"Exported: {pdfPath}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"PDF export skipped: {ex.Message}");
                Console.WriteLine("Add the DevExpress.Pdf.SkiaRenderer NuGet package for PDF export on .NET.");
            }
        }

        Console.WriteLine("Done. Open QuarterlyReport.xlsx to view the result.");
    }
}

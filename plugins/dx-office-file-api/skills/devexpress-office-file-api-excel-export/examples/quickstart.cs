// DevExpress Excel Export Library — Quickstart Example
// NuGet: DevExpress.Document.Processor
// Target: .NET 8+ or .NET Framework 4.6.2+
//
// This console application creates "QuickStart.xlsx" containing:
//   - Three bold, styled header cells (Product, Units Sold, Revenue)
//   - Five data rows with numeric values
//   - A SUM formula in the totals row for each numeric column
//   - Column width settings for readability

using DevExpress.Export.Xl;
using System;
using System.IO;

namespace DevExpressExcelExportQuickstart
{
    class Program
    {
        static void Main(string[] args)
        {
            string outputPath = "QuickStart.xlsx";

            // Step 1: Create an exporter for XLSX format.
            // Pass XlFormulaParser if you want to write string-based formulas.
            IXlExporter exporter = XlExport.CreateExporter(XlDocumentFormat.Xlsx);

            // Step 2: Open the output stream and create the document.
            // IXlDocument is finalized and flushed when disposed.
            using (FileStream stream = new FileStream(outputPath, FileMode.Create, FileAccess.ReadWrite))
            using (IXlDocument document = exporter.CreateDocument(stream))
            {
                // Step 3: Create a worksheet.
                using (IXlSheet sheet = document.CreateSheet())
                {
                    sheet.Name = "Sales";

                    // Step 4: Freeze the header row.
                    // Must be set before any columns or rows are created.
                    sheet.SplitPosition = new XlCellPosition(0, 1); // freeze row 1

                    // Step 5: Define column widths.
                    // Columns must be defined before rows are written.
                    using (IXlColumn col = sheet.CreateColumn()) { col.WidthInPixels = 180; } // A: Product
                    using (IXlColumn col = sheet.CreateColumn()) { col.WidthInPixels = 120; } // B: Units Sold
                    using (IXlColumn col = sheet.CreateColumn())                               // C: Revenue
                    {
                        col.WidthInPixels = 130;
                        // Apply a currency number format to the entire column
                        col.Formatting = new XlCellFormatting();
                        col.Formatting.NumberFormat = "$#,##0.00";
                    }

                    // Step 6: Build the header formatting object.
                    XlCellFormatting headerFmt = new XlCellFormatting();
                    headerFmt.Font = new XlFont();
                    headerFmt.Font.Bold = true;
                    headerFmt.Font.Color = XlColor.FromTheme(XlThemeColor.Light1, 0.0); // white text
                    headerFmt.Font.SchemeStyle = XlFontSchemeStyles.None;
                    headerFmt.Fill = XlFill.SolidFill(XlColor.FromTheme(XlThemeColor.Accent1, 0.0));
                    headerFmt.Alignment = XlCellAlignment.FromHV(
                        XlHorizontalAlignment.Center,
                        XlVerticalAlignment.Center);
                    headerFmt.Border = XlBorder.OutlineBorders(
                        XlColor.FromArgb(0x70, 0x70, 0x70),
                        XlBorderLineStyle.Thin);

                    // Step 7: Write the header row.
                    using (IXlRow row = sheet.CreateRow())
                    {
                        row.HeightInPixels = 28;

                        using (IXlCell cell = row.CreateCell())
                        {
                            cell.Value = "Product";
                            cell.ApplyFormatting(headerFmt);
                        }
                        using (IXlCell cell = row.CreateCell())
                        {
                            cell.Value = "Units Sold";
                            cell.ApplyFormatting(headerFmt);
                        }
                        using (IXlCell cell = row.CreateCell())
                        {
                            cell.Value = "Revenue";
                            cell.ApplyFormatting(headerFmt);
                        }
                    }

                    // Step 8: Write five data rows.
                    string[] products  = { "Widget Alpha", "Widget Beta", "Widget Gamma", "Widget Delta", "Widget Epsilon" };
                    int[]    unitsSold = { 120, 85, 200, 60, 145 };
                    double[] revenue   = { 14400.00, 8925.75, 19800.00, 5400.00, 16820.50 };

                    XlCellFormatting dataFmt = new XlCellFormatting();
                    dataFmt.Font = new XlFont();
                    dataFmt.Font.SchemeStyle = XlFontSchemeStyles.None;
                    dataFmt.Border = XlBorder.OutlineBorders(
                        XlColor.FromArgb(0xCC, 0xCC, 0xCC),
                        XlBorderLineStyle.Thin);

                    // Alternating row fill colors
                    XlFill evenRowFill = XlFill.SolidFill(XlColor.FromArgb(0xF2, 0xF7, 0xFF));
                    XlFill oddRowFill  = XlFill.NoFill();

                    for (int i = 0; i < products.Length; i++)
                    {
                        using (IXlRow row = sheet.CreateRow())
                        {
                            // Apply alternating fill
                            XlCellFormatting rowFmt = new XlCellFormatting();
                            rowFmt.CopyFrom(dataFmt);
                            rowFmt.Fill = (i % 2 == 0) ? evenRowFill : oddRowFill;

                            using (IXlCell cell = row.CreateCell())
                            {
                                cell.Value = products[i];
                                cell.ApplyFormatting(rowFmt);
                            }
                            using (IXlCell cell = row.CreateCell())
                            {
                                cell.Value = unitsSold[i];
                                cell.ApplyFormatting(rowFmt);
                            }
                            using (IXlCell cell = row.CreateCell())
                            {
                                cell.Value = revenue[i];
                                cell.ApplyFormatting(rowFmt); // number format inherited from column
                            }
                        }
                    }

                    // Step 9: Write a totals row with SUM formulas.
                    // Use XlFunc.Sum (no XlFormulaParser required).
                    XlCellFormatting totalFmt = new XlCellFormatting();
                    totalFmt.Font = new XlFont();
                    totalFmt.Font.Bold = true;
                    totalFmt.Font.SchemeStyle = XlFontSchemeStyles.None;
                    totalFmt.Fill = XlFill.SolidFill(XlColor.FromTheme(XlThemeColor.Accent1, 0.6));
                    totalFmt.Border = XlBorder.OutlineBorders(
                        XlColor.FromArgb(0x70, 0x70, 0x70),
                        XlBorderLineStyle.Thin);

                    using (IXlRow row = sheet.CreateRow())
                    {
                        // Column A: label
                        using (IXlCell cell = row.CreateCell())
                        {
                            cell.Value = "TOTAL";
                            cell.ApplyFormatting(totalFmt);
                            cell.ApplyFormatting(XlCellAlignment.FromHV(
                                XlHorizontalAlignment.Right,
                                XlVerticalAlignment.Bottom));
                        }

                        // Column B: SUM of Units Sold (B2:B6, 0-based column index 1, rows 1..5)
                        using (IXlCell cell = row.CreateCell())
                        {
                            cell.SetFormula(XlFunc.Sum(
                                XlCellRange.FromLTRB(1, 1, 1, products.Length)));
                            cell.ApplyFormatting(totalFmt);
                        }

                        // Column C: SUM of Revenue (C2:C6, 0-based column index 2, rows 1..5)
                        using (IXlCell cell = row.CreateCell())
                        {
                            cell.SetFormula(XlFunc.Sum(
                                XlCellRange.FromLTRB(2, 1, 2, products.Length)));
                            cell.ApplyFormatting(totalFmt);
                            // Currency format is inherited from the column definition
                        }
                    }

                    // Step 10: Enable AutoFilter on the header row.
                    sheet.AutoFilterRange = XlCellRange.FromLTRB(0, 0, 2, 0);
                }
            } // IXlDocument disposed here — file is finalized and written to disk

            Console.WriteLine($"File created: {Path.GetFullPath(outputPath)}");
        }
    }
}

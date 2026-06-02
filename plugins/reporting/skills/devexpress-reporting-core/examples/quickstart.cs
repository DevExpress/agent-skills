// DevExpress XtraReports — Core Runtime API Quickstart
// Demonstrates: data class, report with header/grouping/detail/footer, export to PDF and XLSX

using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using DevExpress.Drawing;              // DXMargins, DXFont, DXFontStyle
using DevExpress.Drawing.Printing;     // DXPaperKind
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Parameters;
using DevExpress.XtraPrinting;

// --- Data model ---
// Place model classes in a separate file (e.g., Model/Product.cs) in real projects.
public class Product
{
    public string Name { get; set; }
    public string Category { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}

// --- Report class ---
public class ProductCatalogReport : XtraReport
{
    public ProductCatalogReport()
    {
        Name = "ProductCatalogReport";
        PaperKind = DXPaperKind.A4;
        Margins = new DXMargins(40, 40, 30, 30);  // Left, Right, Top, Bottom
        ReportUnit = ReportUnit.HundredthsOfAnInch;

        BuildBands();
    }

    private void BuildBands()
    {
        // Derive available width from page dimensions.
        float availableWidth = PageWidthF - Margins.Left - Margins.Right;

        // Column proportions (3 : 1 : 1)
        float nameW  = availableWidth * 0.60f;
        float priceW = availableWidth * 0.20f;
        float qtyW   = availableWidth - nameW - priceW; // remainder avoids floating-point drift

        var boldSmall = new DXFont("Arial", 9f, DXFontStyle.Bold);

        // Report header — title
        var reportHeader = new ReportHeaderBand();
        Bands.Add(reportHeader);
        reportHeader.HeightF = 50;
        var titleLabel = new XRLabel
        {
            Text = "Product Catalog",
            Font = new DXFont("Arial", 16f, DXFontStyle.Bold),
            TextAlignment = TextAlignment.MiddleCenter
        };
        reportHeader.Controls.Add(titleLabel);
        titleLabel.BoundsF = new RectangleF(0, 0, availableWidth, 50);

        // Page header — column labels
        var pageHeader = new PageHeaderBand();
        Bands.Add(pageHeader);
        pageHeader.HeightF = 22;
        var headerTable = new XRTable();
        pageHeader.Controls.Add(headerTable);
        headerTable.SizeF = new SizeF(availableWidth, 22);
        headerTable.BeginInit();
        var headerRow = new XRTableRow();        
        headerTable.Rows.Add(headerRow);
        headerRow.Cells.Add(new XRTableCell { Text = "Product", WidthF = nameW,  Font = boldSmall });
        headerRow.Cells.Add(new XRTableCell { Text = "Price",   WidthF = priceW, Font = boldSmall, TextAlignment = TextAlignment.MiddleRight });
        headerRow.Cells.Add(new XRTableCell { Text = "Qty",     WidthF = qtyW,   Font = boldSmall, TextAlignment = TextAlignment.MiddleRight });
        headerTable.EndInit();

        // Group header — category name
        var groupHeader = new GroupHeaderBand
        {
            GroupFields = { new GroupField("Category", XRColumnSortOrder.Ascending) }
        };
        Bands.Add(groupHeader);
        groupHeader.HeightF = 22;
        var catLabel = new XRLabel
        {
            Font = boldSmall,
            ForeColor = Color.DarkBlue
        };
        catLabel.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "[Category]"));
        groupHeader.Controls.Add(catLabel);
        catLabel.BoundsF = new RectangleF(0, 0, availableWidth * 0.40f, 22);

        // Detail band — multi-column table
        var detail = new DetailBand();
        Bands.Add(detail);
        detail.HeightF = 20;
        var dataTable = new XRTable();
        detail.Controls.Add(dataTable);
        dataTable.SizeF = new SizeF(availableWidth, 20);
        dataTable.BeginInit();
        var dataRow = new XRTableRow();
        dataTable.Rows.Add(dataRow);
        var nameCell  = new XRTableCell { WidthF = nameW };
        var priceCell = new XRTableCell { WidthF = priceW, TextAlignment = TextAlignment.MiddleRight };
        var qtyCell   = new XRTableCell { WidthF = qtyW,   TextAlignment = TextAlignment.MiddleRight };
        nameCell.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "[Name]"));
        priceCell.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "[Price]"));
        priceCell.TextFormatString = "{0:C2}";
        qtyCell.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "[Quantity]"));
        dataRow.Cells.Add(nameCell);
        dataRow.Cells.Add(priceCell);
        dataRow.Cells.Add(qtyCell);
        dataTable.EndInit();

        // Group footer — subtotal per category
        var groupFooter = new GroupFooterBand();
        Bands.Add(groupFooter);
        groupFooter.HeightF = 22;
        var subtotalLabel = new XRLabel
        {
            TextAlignment = TextAlignment.MiddleRight,
            Font = boldSmall,
            TextFormatString = "Category total: {0:C2}",
            Summary = new XRSummary { Running = SummaryRunning.Group }
        };
        subtotalLabel.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "sumSum([Price] * [Quantity])"));
        groupFooter.Controls.Add(subtotalLabel);
        subtotalLabel.BoundsF = new RectangleF(0, 0, availableWidth, 22);

        // Report footer — grand total
        var reportFooter = new ReportFooterBand();
        Bands.Add(reportFooter);
        reportFooter.HeightF = 30;
        var grandTotalLabel = new XRLabel
        {
            TextAlignment = TextAlignment.MiddleRight,
            Font = new DXFont("Arial", 10f, DXFontStyle.Bold),
            ForeColor = Color.DarkGreen,
            TextFormatString = "Grand total: {0:C2}",
            Summary = new XRSummary { Running = SummaryRunning.Report }
        };
        grandTotalLabel.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "sumSum([Price] * [Quantity])"));
        reportFooter.Controls.Add(grandTotalLabel);
        grandTotalLabel.BoundsF = new RectangleF(0, 0, availableWidth, 30);

        // Page footer — page number    
        var pageFooter = new PageFooterBand();
        Bands.Add(pageFooter);
        pageFooter.HeightF = 20;
        var pageNum = new XRPageInfo
        {
            PageInfo = PageInfo.NumberOfTotal,
            TextAlignment = TextAlignment.MiddleRight
        };
        pageFooter.Controls.Add(pageNum);
        pageNum.BoundsF = new RectangleF(availableWidth * 0.76f, 0, availableWidth * 0.24f, 20);
    }
}

// --- Usage ---
public static class Program
{
    public static void Main()
    {
        var data = new List<Product>
        {
            new Product { Name = "Widget A", Category = "Tools", Price = 9.99m, Quantity = 50 },
            new Product { Name = "Widget B", Category = "Tools", Price = 14.99m, Quantity = 30 },
            new Product { Name = "Bolt Pack", Category = "Hardware", Price = 4.99m, Quantity = 200 },
            new Product { Name = "Nut Pack", Category = "Hardware", Price = 3.49m, Quantity = 150 },
        };

        var report = new ProductCatalogReport { DataSource = data };

        // Export to PDF
        report.ExportToPdf("catalog.pdf");

        // Export to XLSX with options
        var xlsxOptions = new XlsxExportOptions
        {
            SheetName = "Products",
            TextExportMode = TextExportMode.Value
        };
        report.ExportToXlsx("catalog.xlsx", xlsxOptions);

        // Export to stream (for web/API use)
        using var ms = new MemoryStream();
        report.ExportToPdf(ms);
        // ms is ready for return File(ms.ToArray(), "application/pdf", "catalog.pdf")
    }
}

// DevExpress WinForms Reporting — Quickstart
// Demonstrates: ReportPrintTool (preview), ReportDesignTool (designer), DocumentViewer (embedded)
// Package (Reporting subscription): DevExpress.Win.Reporting

using System;
using System.Windows.Forms;
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.UserDesigner;
using DevExpress.LookAndFeel;
using DevExpress.XtraPrinting.Caching;

// ------------------------------------------------------------------
// 1. Show Ribbon Print Preview (modal)
// ------------------------------------------------------------------
public class PrintPreviewExample
{
    public static void ShowPreview()
    {
        var report = new XtraReport { DataSource = SampleData.GetProducts() };

        var detail = new DetailBand { HeightF = 25 };
        var label = new XRLabel { BoundsF = new System.Drawing.RectangleF(0, 2, 300, 20) };
        label.ExpressionBindings.Add(new ExpressionBinding("Text", "[Name]"));
        detail.Controls.Add(label);
        report.Bands.Add(detail);

        var tool = new ReportPrintTool(report);
        tool.ShowRibbonPreviewDialog(UserLookAndFeel.Default);
    }
}

// ------------------------------------------------------------------
// 2. Launch Ribbon Designer (modal) — Reporting license
// ------------------------------------------------------------------
public class DesignerExample
{
    public static void ShowDesigner()
    {
        var report = new XtraReport();
        report.LoadLayoutFromXml("layout.repx"); // optional: start from saved layout
        report.DataSource = SampleData.GetProducts(); // re-assign after load

        var tool = new ReportDesignTool(report);
        tool.ShowRibbonDesignerDialog(UserLookAndFeel.Default);

        // Save after designer closes
        report.SaveLayoutToXml("layout.repx");
    }
}

// ------------------------------------------------------------------
// 3. Embedded DocumentViewer in a WinForms Form
// ------------------------------------------------------------------
public class ReportViewerForm : Form
{
    private DocumentViewer viewer;

    public ReportViewerForm()
    {
        Text = "Report Viewer";
        Size = new System.Drawing.Size(1000, 700);

        viewer = new DocumentViewer { Dock = DockStyle.Fill };
        Controls.Add(viewer);

        Load += ReportViewerForm_Load;
    }

    private void ReportViewerForm_Load(object sender, EventArgs e)
    {
        var report = new XtraReport { DataSource = SampleData.GetProducts() };

        var detail = new DetailBand { HeightF = 25 };
        var label = new XRLabel { BoundsF = new System.Drawing.RectangleF(0, 2, 300, 20) };
        label.ExpressionBindings.Add(new ExpressionBinding("Text", "[Name]"));
        detail.Controls.Add(label);
        report.Bands.Add(detail);

        viewer.DocumentSource = report;
        report.CreateDocument();
    }
}

// ------------------------------------------------------------------
// 4. Embedded Designer Component (WinForms license)
// ------------------------------------------------------------------
public class EmbeddedDesignerForm : XRDesignRibbonForm
{
    public EmbeddedDesignerForm(XtraReport report)
    {
        OpenReport(report);

        // Optional: configure before showing
        DesignMdiController.DefaultReportSettings.ShowGrid = true;
    }
}

// ------------------------------------------------------------------
// Sample data helper
// ------------------------------------------------------------------
public static class SampleData
{
    public static System.Collections.Generic.List<Product> GetProducts() =>
        new System.Collections.Generic.List<Product>
        {
            new Product { Name = "Widget A", Price = 9.99m },
            new Product { Name = "Widget B", Price = 14.99m },
        };

    public class Product
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
    }
}

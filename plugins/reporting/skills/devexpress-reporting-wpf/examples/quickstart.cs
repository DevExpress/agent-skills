// DevExpress WPF Reporting — Quickstart (C#)
// Demonstrates: PrintHelper (modal preview), DocumentPreviewControl + MVVM, ReportDesigner

using System;
using System.Windows;
using DevExpress.Xpf.Printing;
using DevExpress.Xpf.Reports.UserDesigner;
using DevExpress.XtraReports.UI;
using DevExpress.XtraPrinting.Caching;
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;

// ------------------------------------------------------------------
// 1. PrintHelper — Ribbon preview (simplest, no XAML changes)
// ------------------------------------------------------------------
public partial class MainWindow : Window {
    private void btnPreview_Click(object sender, RoutedEventArgs e) {
        var report = new SalesReport { DataSource = SampleData.Get() };
        PrintHelper.ShowRibbonPrintPreviewDialog(this, report);
    }

    private void btnDesign_Click(object sender, RoutedEventArgs e) {
        // Requires WPF subscription
        var designerWindow = new DesignerWindow();
        designerWindow.Show();
    }
}

// ------------------------------------------------------------------
// 2. MVVM ViewModel for DocumentPreviewControl
//
// XAML (in MainWindow.xaml):
//   <Window DataContext="{dxmvvm:ViewModelSource Type=local:ReportViewModel}">
//       <dxp:DocumentPreviewControl RequestDocumentCreation="True"
//                                   DocumentSource="{Binding Report}" />
//   </Window>
// ------------------------------------------------------------------
[POCOViewModel]
public class ReportViewModel : ViewModelBase {
    public virtual XtraReport Report { get; protected set; }

    protected override void OnInitialized() {
        LoadReport();
    }

    public void LoadReport() {
        Report = new SalesReport { DataSource = SampleData.Get() };
    }
}

// ------------------------------------------------------------------
// 3. ViewModel — filtered report with parameter
// ------------------------------------------------------------------
[POCOViewModel]
public class FilteredReportViewModel : ViewModelBase {
    public virtual XtraReport Report { get; protected set; }

    public void ShowForCategory(string category) {
        var report = new SalesReport();
        report.Parameters["Category"].Value = category;
        report.Parameters["Category"].Visible = false;
        Report = report;
    }
}

// ------------------------------------------------------------------
// 4. ReportDesigner Window (WPF license required)
//
// XAML (DesignerWindow.xaml):
//   <Window xmlns:dxrud="http://schemas.devexpress.com/winfx/2008/xaml/reports/userdesigner">
//       <dxrud:ReportDesigner x:Name="reportDesigner" />
//   </Window>
// ------------------------------------------------------------------
public partial class DesignerWindow : Window {
    private void Window_Loaded(object sender, RoutedEventArgs e) {
        // reportDesigner is declared in XAML
        reportDesigner.OpenDocument(new SalesReport());
    }
}

// ------------------------------------------------------------------
// 5. Large report with CachedReportSource
// ------------------------------------------------------------------
public partial class LargeReportWindow : Window {
    private void Window_Loaded(object sender, RoutedEventArgs e) {
        var report = new SalesReport { DataSource = SampleData.GetLarge() };
        var cached = new CachedReportSource(report, new MemoryDocumentStorage());

        // documentPreview declared in XAML as:
        // <dxp:DocumentPreviewControl x:Name="documentPreview" />
        documentPreview.DocumentSource = cached;
        cached.CreateDocumentAsync();   // async — UI stays responsive
    }
}

// ------------------------------------------------------------------
// Sample data / report stubs
// ------------------------------------------------------------------
public class SalesReport : XtraReport {
    public SalesReport() {
        var detail = new DetailBand { HeightF = 20 };
        var label = new XRLabel { BoundsF = new System.Drawing.RectangleF(0, 1, 300, 18) };
        label.ExpressionBindings.Add(new ExpressionBinding("Text", "[Name]"));
        detail.Controls.Add(label);
        Bands.Add(detail);
    }
}

public static class SampleData {
    public record Product(string Name, decimal Price);

    public static System.Collections.Generic.List<Product> Get() => new() {
        new("Widget A", 9.99m),
        new("Widget B", 14.99m),
    };

    public static System.Collections.Generic.List<Product> GetLarge() {
        var list = new System.Collections.Generic.List<Product>();
        for (int i = 0; i < 1000; i++)
            list.Add(new($"Product {i}", i * 1.5m));
        return list;
    }
}

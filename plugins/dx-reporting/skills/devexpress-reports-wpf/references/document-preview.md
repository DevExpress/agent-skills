# Document Preview — WPF

## When to Use This Reference

Use when embedding `DevExpress.Xpf.Printing.DocumentPreviewControl` in a WPF window, binding it to a report in MVVM, and customizing the toolbar.

## XAML Namespaces

The XAML snippets below use the `dxp:`, `dx:`, and `dxmvvm:` prefixes. Declare them on the root
element — see `references/getting-started.md` → "XAML Namespace Declarations" for the full list:

```xaml
xmlns:dxp="http://schemas.devexpress.com/winfx/2008/xaml/printing"
xmlns:dx="http://schemas.devexpress.com/winfx/2008/xaml/core"
xmlns:dxmvvm="http://schemas.devexpress.com/winfx/2008/xaml/mvvm"
```

## DocumentPreviewControl Basics

```xaml
<dxp:DocumentPreviewControl x:Name="documentPreview"
                            RequestDocumentCreation="True"
                            DocumentSource="{Binding Report}"
                            CommandBarStyle="Ribbon" />
```

Key XAML properties:

| Property | Values | Description |
|----------|--------|-------------|
| `DevExpress.Xpf.Printing.DocumentPreviewControl.DocumentSource` | `DevExpress.XtraReports.UI.XtraReport`, `DevExpress.XtraPrinting.Caching.CachedReportSource` | The report to display |
| `DevExpress.Xpf.Printing.DocumentPreviewControl.RequestDocumentCreation` | `True` / `False` | Auto-build document on `DocumentSource` assignment |
| `DevExpress.Xpf.DocumentViewer.DocumentViewerControl.CommandBarStyle` | `Ribbon`, `Bars`, `None` | Toolbar type |

## Assign Report in Code-Behind

```csharp
// Simple assignment — RequestDocumentCreation="True" handles document build
documentPreview.DocumentSource = new SalesReport { DataSource = GetData() };

// Manual control
var report = new SalesReport { DataSource = GetData() };
documentPreview.DocumentSource = report;
report.CreateDocument(true);               // build synchronously on the current thread
// or:
await report.CreateDocumentAsync();    // async — preferred for large reports
```

## MVVM Binding

# [C#](#tab/csharp-doc-mvvm)
```csharp
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.XtraReports.UI;

[POCOViewModel]
public class ReportViewModel : ViewModelBase {
    public virtual XtraReport Report { get; protected set; }

    public void LoadSalesReport() {
        Report = new SalesReport { DataSource = GetSalesData() };
    }
}
```

# [VB.NET](#tab/vbnet-doc-mvvm)
```vb
Imports DevExpress.Mvvm
Imports DevExpress.Mvvm.DataAnnotations
Imports DevExpress.XtraReports.UI

<POCOViewModel>
Public Class ReportViewModel
    Inherits ViewModelBase
    
    Public Overridable Property Report As XtraReport

    Public Sub LoadSalesReport()
        Report = New SalesReport With {.DataSource = GetSalesData()}
    End Sub
End Class
```
***

### Binding Report in XAML with ViewModel

```xaml
<dx:ThemedWindow DataContext="{dxmvvm:ViewModelSource Type=local:ReportViewModel}"
                Loaded="Window_Loaded">
    <dxp:DocumentPreviewControl RequestDocumentCreation="True"
                               DocumentSource="{Binding Report}" />
</dx:ThemedWindow>
```

Code-behind:

# [C#](#tab/csharp-doc-xaml)
```csharp
private void Window_Loaded(object sender, RoutedEventArgs e) {
    ((ReportViewModel)DataContext).LoadSalesReport();
}
```

# [VB.NET](#tab/vbnet-doc-xaml)
```vb
Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
    DirectCast(DataContext, ReportViewModel).LoadSalesReport()
End Sub
```
***

### Without DevExpress MVVM Framework

```csharp
public class ReportViewModel : INotifyPropertyChanged {
    private XtraReport _report;
    public XtraReport Report {
        get => _report;
        set { _report = value; OnPropertyChanged(nameof(Report)); }
    }

    public event PropertyChangedEventHandler PropertyChanged;
    void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
```

## Passing Parameters from ViewModel

# [C#](#tab/csharp-doc-params)
```csharp
[POCOViewModel]
public class FilteredReportViewModel : ViewModelBase {
    public virtual XtraReport Report { get; protected set; }

    public void ApplyFilter(string category) {
        var report = new ProductReport();
        report.Parameters["Category"].Value = category;
        report.Parameters["Category"].Visible = false;  // hide from UI
        Report = report;
    }
}
```

# [VB.NET](#tab/vbnet-doc-params)
```vb
<POCOViewModel>
Public Class FilteredReportViewModel
    Inherits ViewModelBase
    
    Public Overridable Property Report As XtraReport

    Public Sub ApplyFilter(category As String)
        Dim report = New ProductReport()
        report.Parameters("Category").Value = category
        report.Parameters("Category").Visible = False
        Report = report
    End Sub
End Class
```
***

## Customize Toolbar

> **🔴 Note**: Advanced toolbar customization (adding/removing buttons, custom ribbon pages) requires **WPF subscription**. Changing `CommandBarStyle` (Ribbon/Bars/None) works with **Reporting subscription**.

```xaml
<!-- Hide toolbar completely (Reporting OK) -->
<dxp:DocumentPreviewControl CommandBarStyle="None" DocumentSource="{Binding Report}" />

<!-- Use classic Bars toolbar instead of Ribbon (Reporting OK) -->
<dxp:DocumentPreviewControl CommandBarStyle="Bars" DocumentSource="{Binding Report}" />
```

For advanced toolbar customization (adding/removing specific buttons, adding custom ribbon pages), use DxDocs MCP:
```
devexpress_docs_search(technology="WPF Reporting", query="DocumentPreviewControl customize toolbar ribbon")
```

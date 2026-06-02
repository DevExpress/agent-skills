# Document Preview — WPF

## When to Use This Reference

Use when embedding `DocumentPreviewControl` in a WPF window, binding it to a report in MVVM, customizing the toolbar, or handling large reports with `CachedReportSource`.

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
| `DocumentSource` | `XtraReport`, `CachedReportSource` | The report to display |
| `RequestDocumentCreation` | `True` / `False` | Auto-build document on `DocumentSource` assignment |
| `CommandBarStyle` | `Ribbon`, `Bars`, `None` | Toolbar type |

## Assign Report in Code-Behind

```csharp
// Simple assignment — RequestDocumentCreation="True" handles document build
documentPreview.DocumentSource = new SalesReport { DataSource = GetData() };

// Manual control
var report = new SalesReport { DataSource = GetData() };
documentPreview.DocumentSource = report;
report.CreateDocument();        // explicit build — use in desktop apps
// or:
await report.CreateDocumentAsync();   // async — preferred for large reports
```

## MVVM Binding

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

```xaml
<dx:ThemedWindow DataContext="{dxmvvm:ViewModelSource Type=local:ReportViewModel}"
                Loaded="Window_Loaded">
    <dxp:DocumentPreviewControl RequestDocumentCreation="True"
                               DocumentSource="{Binding Report}" />
</dx:ThemedWindow>
```

```csharp
private void Window_Loaded(object sender, RoutedEventArgs e) {
    ((ReportViewModel)DataContext).LoadSalesReport();
}
```

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

## Large Reports — CachedReportSource

```csharp
using DevExpress.Xpf.Printing;
using DevExpress.XtraPrinting.Caching;

private void Window_Loaded(object sender, RoutedEventArgs e) {
    var report = new LargeReport { DataSource = GetData() };
    var storage = new MemoryDocumentStorage();
    var cached = new CachedReportSource(report, storage);

    documentPreview.DocumentSource = cached;
    cached.CreateDocumentAsync();   // non-blocking — preview shows progress bar
}
```

Storage options:
- `MemoryDocumentStorage` — compressed pages in RAM
- `FileDocumentStorage(path)` — pages stored in temp files
- `DbDocumentStorage` — custom database-backed storage

## Passing Parameters from ViewModel

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

## VB.NET

```vb
Imports DevExpress.Mvvm
Imports DevExpress.Mvvm.DataAnnotations
Imports DevExpress.XtraReports.UI

<POCOViewModel>
Public Class ReportViewModel
    Inherits ViewModelBase

    Public Overridable Property Report As XtraReport

    Public Sub LoadReport()
        Report = New SalesReport() With {.DataSource = GetData()}
    End Sub
End Class
```

```xaml
<dxp:DocumentPreviewControl RequestDocumentCreation="True"
                           DocumentSource="{Binding Report}" />
```

## Customize Toolbar

```xaml
<!-- Hide toolbar completely -->
<dxp:DocumentPreviewControl CommandBarStyle="None" DocumentSource="{Binding Report}" />

<!-- Use classic Bars toolbar instead of Ribbon -->
<dxp:DocumentPreviewControl CommandBarStyle="Bars" DocumentSource="{Binding Report}" />
```

For advanced toolbar customization (adding/removing specific buttons, adding custom ribbon pages), use DxDocs MCP:
```
devexpress_docs_search(technology="WPF Reporting", query="DocumentPreviewControl customize toolbar ribbon")
```

# Getting Started — WPF Reporting (.NET 8+)

## When to Use This Reference

Use when setting up a new .NET 8+ WPF project with DevExpress reporting.

## NuGet Setup

```bash
dotnet add package DevExpress.Wpf.Reporting
dotnet add package DevExpress.Mvvm          # optional — for POCO ViewModel helpers
```

## Assembly References (.NET Framework without NuGet)

Minimum for `DevExpress.Xpf.Printing.DocumentPreviewControl` and `DevExpress.Xpf.Printing.PrintHelper`:
- `DevExpress.Data.v<XX.X>.dll`
- `DevExpress.Data.Desktop.v<XX.X>.dll`
- `DevExpress.Rich.v<XX.X>.Core.dll`
- `DevExpress.Office.v<XX.X>.Core.dll`
- `DevExpress.Pdf.v<XX.X>.Core.dll`
- `DevExpress.Drawing.v<XX.X>.dll`
- `DevExpress.Mvvm.v<XX.X>.dll`
- `DevExpress.Printing.v<XX.X>.Core.dll`
- `DevExpress.Xpf.Core.v<XX.X>.dll`
- `DevExpress.Xpf.DocumentViewer.v<XX.X>.Core.dll`
- `DevExpress.Xpf.LayoutControl.v<XX.X>.dll`
- `DevExpress.Xpf.Docking.v<XX.X>.dll`
- `DevExpress.Xpf.Grid.v<XX.X>.Core.dll`
- `DevExpress.Xpf.Grid.v<XX.X>.dll`
- `DevExpress.Xpf.Printing.v<XX.X>.dll`
- `DevExpress.Xpf.Ribbon.v<XX.X>.dll`

For `DevExpress.Xpf.Reports.UserDesigner.ReportDesigner`:
- `DevExpress.Data.v<XX.X>.dll`
- `DevExpress.Data.Desktop.v<XX.X>.dll`
- `DevExpress.DataAccess.v<XX.X>.dll`
- `DevExpress.Rich.v<XX.X>.Core.dll`
- `DevExpress.Office.v<XX.X>.Core.dll`
- `DevExpress.Pdf.v<XX.X>.Core.dll`
- `DevExpress.Drawing.v<XX.X>.dll`
- `DevExpress.Mvvm.v<XX.X>.dll`
- `DevExpress.Printing.v<XX.X>.Core.dll`
- `DevExpress.Charts.v<XX.X>.Core.dll`
- `DevExpress.XtraCharts.v<XX.X>.dll`
- `DevExpress.XtraReports.v<XX.X>.dll`
- `DevExpress.Xpf.Core.v<XX.X>.dll`
- `DevExpress.Xpf.DocumentViewer.v<XX.X>.Core.dll`
- `DevExpress.Xpf.LayoutControl.v<XX.X>.dll`
- `DevExpress.Xpf.Docking.v<XX.X>.dll`
- `DevExpress.Xpf.Grid.v<XX.X>.Core.dll`
- `DevExpress.Xpf.Grid.v<XX.X>.dll`
- `DevExpress.Xpf.Printing.v<XX.X>.dll`
- `DevExpress.Xpf.Ribbon.v<XX.X>.dll`
- `DevExpress.Xpf.ReportDesigner.v<XX.X>.dll`
- `DevExpress.Xpf.DataAccess.v<XX.X>.dll`
- `DevExpress.Xpf.RichEdit.v<XX.X>.dll`
- `DevExpress.Xpf.PivotGrid.v<XX.X>.dll`
- `DevExpress.Xpf.PropertyGrid.v<XX.X>.dll`
- `DevExpress.Xpf.ExpressionEditor.v<XX.X>.dll`
- `DevExpress.CodeParser.v<XX.X>.dll`

## Required Usings

# [C#](#tab/csharp-usings)
```csharp
using DevExpress.Xpf.Printing;                    // DocumentPreviewControl, PrintHelper
using DevExpress.Xpf.Reports.UserDesigner;        // ReportDesigner (WPF license)
using DevExpress.XtraReports.UI;                  // XtraReport
using DevExpress.XtraPrinting.Caching;            // CachedReportSource
using DevExpress.Mvvm;                            // ViewModelBase, POCOViewModel (optional)
```

# [VB.NET](#tab/vbnet-usings)
```vb
Imports DevExpress.Xpf.Printing                    ' DocumentPreviewControl, PrintHelper
Imports DevExpress.Xpf.Reports.UserDesigner        ' ReportDesigner (WPF license)
Imports DevExpress.XtraReports.UI                  ' XtraReport
Imports DevExpress.XtraPrinting.Caching            ' CachedReportSource
Imports DevExpress.Mvvm                            ' ViewModelBase, POCOViewModel (optional)
```
***

## XAML Namespace Declarations

```xaml
<Window
  xmlns:dx="http://schemas.devexpress.com/winfx/2008/xaml/core"
  xmlns:dxp="http://schemas.devexpress.com/winfx/2008/xaml/printing"
  xmlns:dxrud="http://schemas.devexpress.com/winfx/2008/xaml/reports/userdesigner"
  xmlns:dxmvvm="http://schemas.devexpress.com/winfx/2008/xaml/mvvm"
  xmlns:dxda="http://schemas.devexpress.com/winfx/2008/xaml/dataaccess">
    <!-- content here -->
</Window>
```

| Prefix | Purpose | Used For |
|--------|---------|----------|
| `dx:` | DevExpress core | `ThemedWindow`, messaging |
| `dxp:` | Printing/preview | `DevExpress.Xpf.Printing.DocumentPreviewControl`, `DevExpress.Xpf.Printing.PrintHelper` |
| `dxrud:` | Report designer | `DevExpress.Xpf.Reports.UserDesigner.ReportDesigner` WPF control |
| `dxmvvm:` | MVVM behaviors | `ViewModelSource`, `Interaction.Behaviors` |

## Minimal Preview — PrintHelper

# [C#](#tab/csharp-printhelper)
```csharp
// MainWindow.xaml.cs
private void btnPreview_Click(object sender, RoutedEventArgs e) {
    var report = new SalesReport { DataSource = GetData() };

    // Modal preview dialog
    PrintHelper.ShowRibbonPrintPreviewDialog(this, report);

    // Or non-modal preview
    PrintHelper.ShowRibbonPrintPreview(this, report);
}
```

# [VB.NET](#tab/vbnet-printhelper)
```vb
' MainWindow.xaml.vb
Private Sub btnPreview_Click(sender As Object, e As RoutedEventArgs)
    Dim report = New SalesReport With {.DataSource = GetData()}

    ' Modal preview dialog
    PrintHelper.ShowRibbonPrintPreviewDialog(Me, report)

    ' Or non-modal preview
    PrintHelper.ShowRibbonPrintPreview(Me, report)
End Sub
```
***

## Minimal Preview — DocumentPreviewControl

```xaml
<!-- MainWindow.xaml -->
<Window xmlns:dxp="http://schemas.devexpress.com/winfx/2008/xaml/printing"
        x:Class="MyApp.MainWindow">
    <dxp:DocumentPreviewControl x:Name="preview"
                               RequestDocumentCreation="True" />
</Window>
```

# [C#](#tab/csharp-preview)
```csharp
// MainWindow.xaml.cs
private void Window_Loaded(object sender, RoutedEventArgs e) {
    preview.DocumentSource = new SalesReport { DataSource = GetData() };
}
```

# [VB.NET](#tab/vbnet-preview)
```vb
' MainWindow.xaml.vb
Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
    preview.DocumentSource = New SalesReport With {.DataSource = GetData()}
End Sub
```
***

## Minimal Designer — Built-in End-User Report Designer Form

**Reporting subscription is sufficient** to open the built-in End-User Report Designer form with `ReportDesigner.ShowWindow`. It opens a separate window; it does not embed the designer in your XAML window.

Add the following method to your WPF `Window` class and pass an existing `XtraReport`. `this` / `Me` supplies the owner window. Keep the report available while the designer window is open.

# [C#](#tab/csharp-designer-window)
```csharp
using DevExpress.Xpf.Reports.UserDesigner;
using DevExpress.XtraReports.UI;

private void ShowReportDesigner(XtraReport report) {
    var designer = new ReportDesigner();
    designer.OpenDocument(report);
    designer.ShowWindow(this);
}
```

# [VB.NET](#tab/vbnet-designer-window)
```vb
Imports DevExpress.Xpf.Reports.UserDesigner
Imports DevExpress.XtraReports.UI

Private Sub ShowReportDesigner(report As XtraReport)
    Dim designer = New ReportDesigner()
    designer.OpenDocument(report)
    designer.ShowWindow(Me)
End Sub
```
***

See [ReportDesigner](https://docs.devexpress.com/WPF/DevExpress.Xpf.Reports.UserDesigner.ReportDesigner) for subscription requirements and [ShowWindow(FrameworkElement)](https://docs.devexpress.com/WPF/DevExpress.Xpf.Reports.UserDesigner.ReportDesigner.ShowWindow(System.Windows.FrameworkElement)) for the owner parameter.

## Minimal Designer — Embedded Control

**Requires a WPF/Universal/DXperience subscription** to host the `ReportDesigner` control in your own XAML window.

```xaml
<Window xmlns:dxrud="http://schemas.devexpress.com/winfx/2008/xaml/reports/userdesigner"
        x:Class="MyApp.DesignerWindow">
    <dxrud:ReportDesigner x:Name="reportDesigner" />
</Window>
```

# [C#](#tab/csharp-designer)
```csharp
private void Window_Loaded(object sender, RoutedEventArgs e) {
    reportDesigner.OpenDocument(new SalesReport());
}
```

# [VB.NET](#tab/vbnet-designer)
```vb
Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
    reportDesigner.OpenDocument(New SalesReport())
End Sub
```
***
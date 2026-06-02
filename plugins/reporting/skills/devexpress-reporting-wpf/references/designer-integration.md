# Designer Integration — WPF

## When to Use This Reference

Use when embedding the `ReportDesigner` WPF control, opening and managing documents in it, or controlling the designer from a ViewModel.

> **WPF subscription required** for the `ReportDesigner` control. Use `DocumentPreviewControl` for preview with a Reporting subscription.

## Basic Integration

```xaml
<Window xmlns:dxrud="http://schemas.devexpress.com/winfx/2008/xaml/reports/userdesigner"
        x:Class="MyApp.DesignerWindow"
        Title="Report Designer" Height="700" Width="1100">
    <dxrud:ReportDesigner x:Name="reportDesigner" />
</Window>
```

```csharp
private void Window_Loaded(object sender, RoutedEventArgs e) {
    reportDesigner.OpenDocument(new SalesReport());
}
```

### VB.NET

```vb
Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
    reportDesigner.OpenDocument(New SalesReport())
End Sub
```

## Key Methods

```csharp
// Open an existing report
reportDesigner.OpenDocument(new SalesReport());

// Open from stream (.repx)
using var stream = File.OpenRead("layout.repx");
reportDesigner.OpenDocument(stream);

// Create a new blank document
reportDesigner.NewDocument();

// Access open documents
foreach (var doc in reportDesigner.Documents)
    Console.WriteLine(doc.Caption);

// Get the active document
var activeDoc = reportDesigner.ActiveDocument;

// Switch between Design and Preview view
activeDoc.ViewKind = ReportDesignerDocumentViewKind.Preview;
activeDoc.ViewKind = ReportDesignerDocumentViewKind.Designer;
```

## Modern Ribbon Style

```xaml
<dxrud:ReportDesigner x:Name="reportDesigner"
                     UseOfficeInspiredRibbonStyle="True" />
```

## MVVM — Service Pattern

To control the designer from a ViewModel (without code-behind), implement a service behavior.

### 1. Define the Service Interface

```csharp
using DevExpress.Xpf.Reports.UserDesigner;

public interface IReportDesignerService {
    ReportDesignerDocument NewReport(XtraReport report = null);
    ReportDesignerDocument Open();
    ReportDesignerDocument Open(System.IO.Stream stream);
    ReportDesignerDocument ActiveDocument { get; }
}
```

### 2. Implement the Service

```csharp
using DevExpress.Mvvm.UI;
using DevExpress.Mvvm.UI.Interactivity;

public class ReportDesignerService : ServiceBase, IReportDesignerService {
    ReportDesigner Designer => (ReportDesigner)AssociatedObject;

    public ReportDesignerDocument ActiveDocument => Designer.ActiveDocument;

    public ReportDesignerDocument NewReport(XtraReport report = null) {
        Func<XtraReport> factory = report != null ? () => report : (Func<XtraReport>)null;
        return Designer.NewDocument(factory);
    }

    public ReportDesignerDocument Open() => Designer.OpenDocument();
    public ReportDesignerDocument Open(System.IO.Stream stream) => Designer.OpenDocument(stream);
}
```

### 3. Register via XAML

```xaml
<dxrud:ReportDesigner x:Name="reportDesigner">
    <dxmvvm:Interaction.Behaviors>
        <local:ReportDesignerService />
    </dxmvvm:Interaction.Behaviors>
</dxrud:ReportDesigner>
```

### 4. Use from ViewModel

```csharp
[POCOViewModel]
public class DesignerViewModel : ViewModelBase {
    IReportDesignerService DesignerService =>
        ServiceContainer.GetService<IReportDesignerService>();

    [Command]
    public void NewReport() => DesignerService.NewReport();
    public bool CanNewReport() => DesignerService != null;

    [Command]
    public void OpenReport() => DesignerService.Open();
    public bool CanOpenReport() => DesignerService != null;
}
```

## Deep Customization

For deep designer customization — wizard step customization, custom toolbox categories, custom report controls, data source wizard — these topics are not fully covered in this reference. Use DxDocs MCP:

```
devexpress_docs_search(technology="WPF Reporting", query="end-user report designer customization WPF")
devexpress_docs_search(technology="WPF Reporting", query="report designer wizard WPF")
devexpress_docs_search(technology="WPF Reporting", query="toolbox categories WPF report designer")
devexpress_docs_get_content(url="<article URL from search>")
```

## VB.NET — Basic Integration

```vb
Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
    reportDesigner.OpenDocument(New SalesReport())
End Sub
```

```vb
' Access active document
Dim activeDoc As ReportDesignerDocument = reportDesigner.ActiveDocument

' Switch to preview
activeDoc.ViewKind = ReportDesignerDocumentViewKind.Preview
```

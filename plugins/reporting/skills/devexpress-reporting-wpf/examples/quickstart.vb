' DevExpress WPF Reporting — Quickstart (VB.NET)
' Demonstrates: PrintHelper preview, MVVM ViewModel, ReportDesigner

Imports System
Imports System.Windows
Imports DevExpress.Xpf.Printing
Imports DevExpress.Xpf.Reports.UserDesigner
Imports DevExpress.XtraReports.UI
Imports DevExpress.Mvvm
Imports DevExpress.Mvvm.DataAnnotations

' 1. PrintHelper — simplest approach
Partial Public Class MainWindow
    Inherits Window

    Private Sub btnPreview_Click(sender As Object, e As RoutedEventArgs)
        Dim report As New SalesReport()
        report.DataSource = SampleData.GetProducts()
        PrintHelper.ShowRibbonPrintPreviewDialog(Me, report)
    End Sub
End Class

' 2. MVVM ViewModel for DocumentPreviewControl
'
' XAML:
'   <Window DataContext="{dxmvvm:ViewModelSource Type=local:ReportViewModel}">
'       <dxp:DocumentPreviewControl RequestDocumentCreation="True"
'                                   DocumentSource="{Binding Report}" />
'   </Window>

<POCOViewModel>
Public Class ReportViewModel
    Inherits ViewModelBase

    Public Overridable Property Report As XtraReport

    Protected Overrides Sub OnInitialized()
        LoadReport()
    End Sub

    Public Sub LoadReport()
        Report = New SalesReport() With {.DataSource = SampleData.GetProducts()}
    End Sub
End Class

' 3. ViewModel with parameter
<POCOViewModel>
Public Class FilteredReportViewModel
    Inherits ViewModelBase

    Public Overridable Property Report As XtraReport

    Public Sub ShowForCategory(category As String)
        Dim report As New SalesReport()
        report.Parameters("Category").Value = category
        report.Parameters("Category").Visible = False
        Me.Report = report
    End Sub
End Class

' 4. Designer window (WPF license required)
'
' XAML:
'   <Window xmlns:dxrud="http://schemas.devexpress.com/winfx/2008/xaml/reports/userdesigner">
'       <dxrud:ReportDesigner x:Name="reportDesigner" />
'   </Window>

Partial Public Class DesignerWindow
    Inherits Window

    Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
        reportDesigner.OpenDocument(New SalesReport())
    End Sub
End Class

' Sample report and data
Public Class SalesReport
    Inherits XtraReport

    Public Sub New()
        Dim detail As New DetailBand()
        detail.HeightF = 20

        Dim label As New XRLabel()
        label.BoundsF = New System.Drawing.RectangleF(0, 1, 300, 18)
        label.ExpressionBindings.Add(New ExpressionBinding("Text", "[Name]"))
        detail.Controls.Add(label)
        Bands.Add(detail)
    End Sub
End Class

Public Module SampleData
    Public Class Product
        Public Property Name As String
        Public Property Price As Decimal
    End Class

    Public Function GetProducts() As System.Collections.Generic.List(Of Product)
        Return New System.Collections.Generic.List(Of Product) From {
            New Product() With {.Name = "Widget A", .Price = 9.99D},
            New Product() With {.Name = "Widget B", .Price = 14.99D}
        }
    End Function
End Module

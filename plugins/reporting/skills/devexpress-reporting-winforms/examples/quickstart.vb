' DevExpress WinForms Reporting — Quickstart (VB.NET)
' Demonstrates: Print preview, designer launch, embedded viewer

Imports System
Imports System.Windows.Forms
Imports DevExpress.XtraReports.UI
Imports DevExpress.XtraReports.UserDesigner
Imports DevExpress.LookAndFeel

' 1. Ribbon Print Preview (modal)
Public Class PrintPreviewExample
    Public Shared Sub ShowPreview()
        Dim report As New XtraReport()
        report.DataSource = SampleData.GetProducts()

        Dim detail As New DetailBand()
        detail.HeightF = 25

        Dim label As New XRLabel()
        label.BoundsF = New System.Drawing.RectangleF(0, 2, 300, 20)
        label.ExpressionBindings.Add(New ExpressionBinding("Text", "[Name]"))
        detail.Controls.Add(label)
        report.Bands.Add(detail)

        Dim tool As New ReportPrintTool(report)
        tool.ShowRibbonPreviewDialog(UserLookAndFeel.Default)
    End Sub
End Class

' 2. Ribbon Designer (modal) — Reporting license
Public Class DesignerExample
    Public Shared Sub ShowDesigner()
        Dim report As New XtraReport()
        ' Optional: load existing layout
        ' report.LoadLayoutFromXml("layout.repx")
        ' report.DataSource = SampleData.GetProducts()

        Dim tool As New ReportDesignTool(report)
        tool.ShowRibbonDesignerDialog(UserLookAndFeel.Default)

        report.SaveLayoutToXml("layout.repx")
    End Sub
End Class

' 3. Embedded DocumentViewer
Public Class ReportViewerForm
    Inherits Form

    Private viewer As DocumentViewer

    Public Sub New()
        Me.Text = "Report Viewer"
        Me.Size = New System.Drawing.Size(1000, 700)

        viewer = New DocumentViewer()
        viewer.Dock = DockStyle.Fill
        Controls.Add(viewer)
    End Sub

    Private Sub ReportViewerForm_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim report As New XtraReport()
        report.DataSource = SampleData.GetProducts()

        Dim detail As New DetailBand()
        detail.HeightF = 25

        Dim label As New XRLabel()
        label.BoundsF = New System.Drawing.RectangleF(0, 2, 300, 20)
        label.ExpressionBindings.Add(New ExpressionBinding("Text", "[Name]"))
        detail.Controls.Add(label)
        report.Bands.Add(detail)

        viewer.DocumentSource = report
        report.CreateDocument()
    End Sub
End Class

' Sample data
Public Module SampleData
    Public Class Product
        Public Property Name As String
        Public Property Price As Decimal
    End Class

    Public Function GetProducts() As System.Collections.Generic.List(Of Product)
        Dim list As New System.Collections.Generic.List(Of Product)()
        list.Add(New Product() With {.Name = "Widget A", .Price = 9.99D})
        list.Add(New Product() With {.Name = "Widget B", .Price = 14.99D})
        Return list
    End Function
End Module

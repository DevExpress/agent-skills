' DevExpress XtraReports — Core Runtime API Quickstart (VB.NET)
' Demonstrates: data class, grouped report with XRTable layout, export to PDF

Imports System
Imports System.Collections.Generic
Imports System.Drawing
Imports DevExpress.Drawing              ' DXMargins, DXFont, DXFontStyle
Imports DevExpress.Drawing.Printing     ' DXPaperKind
Imports DevExpress.XtraReports.UI
Imports DevExpress.XtraPrinting

' Data model
' Place model classes in a separate file in real projects (see Constraint 10).
Public Class Product
    Public Property Name As String
    Public Property Category As String
    Public Property Price As Decimal
    Public Property Quantity As Integer
End Class

' Report class
Public Class ProductCatalogReport
    Inherits XtraReport

    Public Sub New()
        Me.Name = "ProductCatalogReport"
        Me.PaperKind = DXPaperKind.A4
        Me.Margins = New DXMargins(40, 40, 30, 30)  ' Left, Right, Top, Bottom
        BuildBands()
    End Sub

    Private Sub BuildBands()
        Dim availableWidth As Single = PageWidthF - Margins.Left - Margins.Right

        ' Column proportions (3 : 1 : 1)
        Dim nameW  As Single = availableWidth * 0.6F
        Dim priceW As Single = availableWidth * 0.2F
        Dim qtyW   As Single = availableWidth - nameW - priceW

        Dim boldSmall As New DXFont("Arial", 9F, DXFontStyle.Bold)

        ' Report header
        Dim reportHeader As New ReportHeaderBand()
        Bands.Add(reportHeader)
        reportHeader.HeightF = 50

        Dim titleLabel As New XRLabel()
        titleLabel.Text = "Product Catalog"
        titleLabel.Font = New DXFont("Arial", 16F, DXFontStyle.Bold)
        titleLabel.TextAlignment = TextAlignment.MiddleCenter
        reportHeader.Controls.Add(titleLabel)
        titleLabel.BoundsF = New RectangleF(0, 5, availableWidth, 40)

        ' Group header — sort and display category
        Dim groupHeader As New GroupHeaderBand()
        groupHeader.GroupFields.Add(New GroupField("Category", XRColumnSortOrder.Ascending))
        Bands.Add(groupHeader)
        groupHeader.HeightF = 22

        Dim catLabel As New XRLabel()
        catLabel.Font = boldSmall
        catLabel.ForeColor = Color.DarkBlue
        catLabel.ExpressionBindings.Add(New ExpressionBinding("BeforePrint", "Text", "[Category]"))
        groupHeader.Controls.Add(catLabel)
        catLabel.BoundsF = New RectangleF(0, 2, availableWidth * 0.4F, 18)

        ' Detail band 
        Dim detail As New DetailBand()
        Bands.Add(detail)
        detail.HeightF = 20

        Dim dataTable As New XRTable()
        detail.Controls.Add(dataTable)
        dataTable.BeginInit()
        Dim dataRow As New XRTableRow()
        Dim nameCell  As New XRTableCell() With { .WidthF = nameW }
        Dim priceCell As New XRTableCell() With { .WidthF = priceW, .TextAlignment = TextAlignment.MiddleRight, .TextFormatString = "{0:C2}" }
        Dim qtyCell   As New XRTableCell() With { .WidthF = qtyW,   .TextAlignment = TextAlignment.MiddleRight }
        nameCell.ExpressionBindings.Add(New ExpressionBinding("BeforePrint", "Text", "[Name]"))
        priceCell.ExpressionBindings.Add(New ExpressionBinding("BeforePrint", "Text", "[Price]"))
        qtyCell.ExpressionBindings.Add(New ExpressionBinding("BeforePrint", "Text", "[Quantity]"))
        dataRow.Cells.Add(nameCell)
        dataRow.Cells.Add(priceCell)
        dataRow.Cells.Add(qtyCell)
        dataTable.Rows.Add(dataRow)
        dataTable.SizeF = New SizeF(availableWidth, 18)
        dataTable.EndInit()

        ' Group footer — subtotal
        Dim groupFooter As New GroupFooterBand()
        Bands.Add(groupFooter)
        groupFooter.HeightF = 22

        Dim subtotalLabel As New XRLabel()
        subtotalLabel.TextAlignment = TextAlignment.MiddleRight
        subtotalLabel.Font = boldSmall
        subtotalLabel.TextFormatString = "Category total: {0:C2}"
        subtotalLabel.Summary = New XRSummary() With { .Running = SummaryRunning.Group }
        subtotalLabel.ExpressionBindings.Add(New ExpressionBinding("BeforePrint", "Text", "sumSum([Price] * [Quantity])"))
        groupFooter.Controls.Add(subtotalLabel)
        subtotalLabel.BoundsF = New RectangleF(0, 2, availableWidth, 18)

        ' Page footer — page number
        Dim pageFooter As New PageFooterBand()
        Bands.Add(pageFooter)
        pageFooter.HeightF = 20

        Dim pageNum As New XRPageInfo()
        pageNum.PageInfo = PageInfo.NumberOfTotal
        pageNum.TextAlignment = TextAlignment.MiddleRight
        pageFooter.Controls.Add(pageNum)
        pageNum.BoundsF = New RectangleF(availableWidth * 0.76F, 2, availableWidth * 0.24F, 16)
    End Sub
End Class

' Usage
Module Program
    Sub Main()
        Dim data As New List(Of Product)()
        data.Add(New Product() With {.Name = "Widget A", .Category = "Tools", .Price = 9.99D, .Quantity = 50})
        data.Add(New Product() With {.Name = "Widget B", .Category = "Tools", .Price = 14.99D, .Quantity = 30})
        data.Add(New Product() With {.Name = "Bolt Pack", .Category = "Hardware", .Price = 4.99D, .Quantity = 200})

        Dim report As New ProductCatalogReport()
        report.DataSource = data

        report.ExportToPdf("catalog.pdf")
        report.ExportToXlsx("catalog.xlsx")
    End Sub
End Module

# Report Types

## When to Use This Reference

Use when building a specific report pattern: table report, grouped report, master-detail, label report, or inherited report.

## Table Report

Classic tabular layout with a column header row and a detail row using `XRTable`:

```csharp
var report = new XtraReport { DataSource = products };
var availableWidth =  report.PageWidthF - report.Margins.Left - report.Margins.Right;

// Page header with column labels
var pageHeader = new PageHeaderBand();
report.Bands.Add(pageHeader);
pageHeader.HeightF = 22;
var headerTable = new XRTable();
pageHeader.Controls.Add(headerTable);
headerTable.BeginInit();
headerTable.BoundsF = new RectangleF(0, 0, availableWidth, 22);
var headerRow = new XRTableRow();
headerRow.Cells.Add(new XRTableCell { Text = "Product", WidthF = availableWidth * 0.50f });
headerRow.Cells.Add(new XRTableCell { Text = "Price", WidthF =  availableWidth * 0.25f });
headerRow.Cells.Add(new XRTableCell { Text = "Category", WidthF =  availableWidth * 0.25f });
headerTable.Rows.Add(headerRow);
headerTable.EndInit();

// Detail band with data row
var detail = new DetailBand();
report.Bands.Add(detail);
detail.HeightF = 22;

var dataTable = new XRTable();
detail.Controls.Add(dataTable);
dataTable.BoundsF = new RectangleF(0, 0, availableWidth, 22);

dataTable.BeginInit();
var dataRow = new XRTableRow();

var nameCell = new XRTableCell { WidthF = availableWidth * 0.50f };
nameCell.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "[Name]"));

var priceCell = new XRTableCell { WidthF = availableWidth * 0.25f };
priceCell.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "[Price]"));
priceCell.TextFormatString = "{0:C2}";

var catCell = new XRTableCell { WidthF = availableWidth * 0.25f };
catCell.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "[Category]"));

dataRow.Cells.Add(nameCell);
dataRow.Cells.Add(priceCell);
dataRow.Cells.Add(catCell);
dataTable.Rows.Add(dataRow);
dataTable.EndInit();

```

## Grouped Report

```csharp
var report = new XtraReport { DataSource = products };
var availableWidth =  report.PageWidthF - report.Margins.Left - report.Margins.Right;

var groupHeader = new GroupHeaderBand {
    GroupFields = { new GroupField("Category", XRColumnSortOrder.Ascending) }
};
report.Bands.Add(groupHeader);
groupHeader.HeightF = 30;
var groupLabel = new XRLabel();
groupHeader.Controls.Add(groupLabel);
groupLabel.BoundsF = new RectangleF(0, 0, availableWidth, 30);
groupLabel.Font = new DevExpress.Drawing.DXFont("Arial", 14f, DevExpress.Drawing.DXFontStyle.Bold);
groupLabel.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "[Category]"));

var detail = new DetailBand();
report.Bands.Add(detail);
detail.HeightF = 22;
var nameLabel = new XRLabel();
detail.Controls.Add(nameLabel);
nameLabel.BoundsF = new RectangleF(20, 0, availableWidth - 20, 22);
nameLabel.Font = new DevExpress.Drawing.DXFont("Arial", 10f);
nameLabel.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "[Name]"));

var groupFooter = new GroupFooterBand();
report.Bands.Add(groupFooter);
groupFooter.HeightF = 22;
var sumLabel = new XRLabel();
groupFooter.Controls.Add(sumLabel);
sumLabel.BoundsF = new RectangleF(150, 0, availableWidth - 150, 22);
sumLabel.Font = new DevExpress.Drawing.DXFont("Arial", 12f);
sumLabel.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "sumSum([Price])"));
sumLabel.TextFormatString = "Subtotal: {0:C2}";
XRSummary summary = new XRSummary();
summary.Running = SummaryRunning.Group;
sumLabel.Summary = summary; 
```

## Master-Detail Report

Two approaches:

### Approach 1 — DetailReportBand (preferred for collections)

```csharp
// Outer report: list of customers
report.DataSource = customers;   // List<Customer> where each Customer has Orders property
var availableWidth =  report.PageWidthF - report.Margins.Left - report.Margins.Right;

var masterDetail = new DetailBand();
report.Bands.Add(masterDetail);
masterDetail.HeightF = 22;
var custLabel = new XRLabel();
masterDetail.Controls.Add(custLabel);
custLabel.BoundsF = new RectangleF(0, 0, availableWidth, 22);
custLabel.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "[CompanyName]"));

// Inner report: orders for each customer
var nestedDetailReport = new DetailReportBand {
    DataSource = customers,
    DataMember = "Orders"    // navigation property
};
masterDetail.Bands.Add(nestedDetailReport);
var nestedDetail = new DetailBand();
nestedDetailReport.Bands.Add(nestedDetail);
nestedDetail.HeightF = 20;
var orderLabel = new XRLabel();
nestedDetail.Controls.Add(orderLabel);
orderLabel.BoundsF = new RectangleF(20, 0, availableWidth - 20, 20);
orderLabel.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "[OrderDate]"));

```

### Approach 2 — XRSubreport (for separate report definitions)

```csharp
var availableWidth =  report.PageWidthF - report.Margins.Left - report.Margins.Right;

var subreport = new XRSubreport();
detail.Controls.Add(subreport);
subreport.GenerateOwnPages = true; // if true, subreport starts on a new page; if false, subreport content is rendered inline within the master report's detail band
subreport.ParameterBindings.Add(new ParameterBinding("subreportCategory", null, "Products.CategoryID")); // optionally, pass category ID to subreport
subreport.BoundsF = new RectangleF(0, 0, availableWidth, 100);
subreport.ReportSource = new OrderDetailReport(); // OrderDetailReport is a separate XtraReport class with its own data source and layout, designed to show orders for a given category 
```

## Label Report (Badges / Mailing Labels)

```csharp
var report = new XtraReport {
    DataSource = contacts,
    PaperKind = DevExpress.Drawing.Printing.DXPaperKind.Letter,
    Margins = new DevExpress.Drawing.DXMargins(20, 20, 20, 20)
};
var availableWidth =  report.PageWidthF - report.Margins.Left - report.Margins.Right;

var detail = new DetailBand();
report.Bands.Add(detail);
detail.HeightF = 40;  // label height
detail.MultiColumn = new MultiColumn {
    ColumnCount = 3,
    Mode = MultiColumnMode.UseColumnCount,
    ColumnSpacing = 5,
    Layout = ColumnLayout.DownThenAcross
};

var nameLabel = new XRLabel();
detail.Controls.Add(nameLabel);
nameLabel.BoundsF = new RectangleF(0, 0, availableWidth / 3.0f, 18);
nameLabel.Font = new DevExpress.Drawing.DXFont("Arial", 9, DevExpress.Drawing.DXFontStyle.Bold);
nameLabel.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "[FullName]"));

var addrLabel = new XRLabel();
detail.Controls.Add(addrLabel);
addrLabel.BoundsF = new RectangleF(0, 20, availableWidth / 3.0f , 18);
addrLabel.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "[Address]"));

```

## Inherited Report

Create a base layout and override only what changes:

```csharp
// Base report
public class BaseReport : XtraReport {
    protected XRLabel TitleLabel;

    public BaseReport() {
        var availableWidth =  PageWidthF - Margins.Left - Margins.Right;
        var header = new ReportHeaderBand();
        Bands.Add(header);
        header.HeightF = 40;
        TitleLabel = new XRLabel();
        header.Controls.Add(TitleLabel);
        TitleLabel.BoundsF = new RectangleF(0, 0, availableWidth, 40);
        TitleLabel.Font = new DevExpress.Drawing.DXFont("Arial", 16, DevExpress.Drawing.DXFontStyle.Bold);
    }
}

// Derived report
public class SalesReport : BaseReport {
    public SalesReport() {
        TitleLabel.Text = "Sales Report";
        DataSource = GetSalesData();

        var detail = new DetailBand();
        Bands.Add(detail);
        detail.HeightF = 20;
        var label = new XRLabel();        
        detail.Controls.Add(label);
        label.BoundsF = new RectangleF(0, 0, PageWidthF - Margins.Left - Margins.Right, 20);
        label.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "[Product]"));
    }
}
```

## VB.NET — Grouped Report

```vb
Dim groupHeader As New GroupHeaderBand()
groupHeader.GroupFields.Add(New GroupField("Category", XRColumnSortOrder.Ascending))
report.Bands.Add(groupHeader)
groupHeader.HeightF = 22

Dim groupLabel As New XRLabel()
groupLabel.Font = New Font("Arial", 9, FontStyle.Bold)
groupLabel.ExpressionBindings.Add(New ExpressionBinding("Text", "[Category]"))
groupHeader.Controls.Add(groupLabel)
groupLabel.BoundsF = New RectangleF(0, 0, 200, 20)

Dim detail As New DetailBand()
report.Bands.Add(detail)
detail.HeightF = 20
```

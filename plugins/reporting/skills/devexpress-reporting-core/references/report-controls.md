# Report Controls

## When to Use This Reference

Use when adding controls to report bands: text labels, tables, images, barcodes, charts, page info, shapes, subreports.

## Control Categories

### Text

**XRLabel** — most common text/field control useful for standalone text:
```csharp
var label = new XRLabel();
detail.Controls.Add(label);
label.LocationF = new System.Drawing.PointF(0, 0);
label.SizeF = new System.Drawing.SizeF(150, 25);
label.Font = new DevExpress.Drawing.DXFont("Arial", 10f, DevExpress.Drawing.DXFontStyle.Bold);
label.ForeColor = Color.Black;
label.TextFormatString = "{0:C2}"; // format string for numeric/date fields
label.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "[ProductName]"));
```

Key properties: `Text`, `TextFormatString`, `Font`, `ForeColor`, `BackColor`, `Borders`, `TextAlignment`, `Multiline`, `WordWrap`, `CanGrow`, `CanShrink`.

**Tabular layout**: When displaying two or more fields side-by-side as columns inside a band, use `XRTable` / `XRTableRow` / `XRTableCell`. See the Table section below and the SKILL.md **Antipatterns** section (AP3).
<!-- Addresses: 2026-05-19-xlabel-instead-of-xrtable.md -->


**XRRichText** — HTML/RTF/DOCX formatted content:
```csharp
var richText = new XRRichText();
detail.Controls.Add(richText);
richText.BoundsF = new RectangleF(0, 0, 300, 100);
richText.SerializationFormat = RichTextSerializationFormat.Rtf;
richText.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Rtf", "[Notes]"));
```

**XRCharacterComb** — fixed-cell character grid (for forms):
```csharp
var comb = new XRCharacterComb();
detail.Controls.Add(comb);
comb.BoundsF = new RectangleF(0, 0, 200, 25);
comb.CellWidth = 15;
comb.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "[Code]"));
```

### Table

**XRTable / XRTableRow / XRTableCell** — structured grid layout:
```csharp
// Creates a table and adds it to the detail band.
// Use XRTable whenever two or more data-bound fields appear side-by-side as columns.
var detail = new DetailBand();
report.Bands.Add(detail);
detail.HeightF = 30;

var table = new XRTable();
detail.Controls.Add(table);

table.BeginInit();

var row = new XRTableRow();
table.Rows.Add(row);

var nameCell = new XRTableCell { WidthF = 450 };   // absolute column width in pixels
var priceCell = new XRTableCell { WidthF = 200 };  // sum of WidthF values must equal table SizeF.Width
row.Cells.Add(nameCell);
row.Cells.Add(priceCell);

nameCell.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "[ProductName]"));
priceCell.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "[UnitPrice]"));

table.SizeF = new SizeF(650, 25);  // must equal sum of cell WidthF values (450 + 200)
table.EndInit();
```
Use `WidthF` on cells to set absolute column widths in pixels. The sum of all `WidthF` values in a row must equal the table's `SizeF.Width`. Use `RowSpan` to merge cells vertically.
<!-- Addresses: 2026-05-19-xrtablecell-weight-vs-widthf.md -->

### Images

**XRPictureBox** — static or data-bound image:
```csharp
// Static image
var pic = new XRPictureBox();
detail.Controls.Add(pic);
pic.BoundsF = new RectangleF(0, 0, 80, 60);
pic.ImageSource = ImageSource.FromFile("logo.png");
pic.Sizing = ImageSizeMode.ZoomImage;

// Data-bound image (byte[] field)
var pic2 = new XRPictureBox();
detail.Controls.Add(pic2);
pic2.BoundsF = new RectangleF(0, 0, 60, 60);
pic2.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "ImageSource", "[Photo]"));
```

### Barcode

**XRBarCode**:
```csharp
var barcode = new XRBarCode();
detail.Controls.Add(barcode);
barcode.BoundsF = new RectangleF(0, 0, 120, 60);
barcode.Symbology = new QRCode { Version = 2 };
barcode.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "[SKU]"));
```

Common symbologies: `QRCode`, `Code128`, `EAN13`, `PDF417`, `DataMatrix`.

### Checkbox

**XRCheckBox**:
```csharp
var check = new XRCheckBox();
detail.Controls.Add(check);
check.BoundsF = new RectangleF(0, 0, 20, 20);
check.Text = "Active";
check.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "CheckBoxState", "[IsActive]"));
```

### Page Info

**XRPageInfo** — page number, date, time:
```csharp
var pageNum = new XRPageInfo();
pageFooter.Controls.Add(pageNum);
pageNum.PageInfo = DevExpress.XtraPrinting.PageInfo.NumberOfTotal;  // "1 of 5"
pageNum.BoundsF = new RectangleF(300, 0, 100, 18);
pageNum.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleRight;
// PageInfo.Number, PageInfo.Total, PageInfo.NumberOfTotal, PageInfo.DateTime
```

### Data Visualization

**XRChart** — embeds a DevExpress Chart control. The XRChart control is implemented based on the ChartControl for WinForms.
```csharp
var chart = new XRChart();
detail.Controls.Add(chart);
chart.BoundsF = new RectangleF(0, 0, 400, 200);
// Configure via chart.Series, chart.DataSource, etc.
// See DxDocs MCP for advanced chart configuration.
// Refer to the Chart Control (WinForms) help topics for information on possible configurations.
```

**XRCrossTab** — pivot table (use DxDocs MCP for cross-tab setup — configuration is complex).

**XRSparkline** — compact inline chart.

**XRGauge** — dial/linear gauge.

### Layout

**XRPanel** — container for grouping controls:
```csharp
var panel = new XRPanel();
detail.Controls.Add(panel);
panel.BoundsF = new RectangleF(0, 0, 300, 50);
panel.Controls.Add(label1);
panel.Controls.Add(label2);
```

**XRPageBreak** — force a new page:
```csharp
var pageBreak = new XRPageBreak();
detail.Controls.Add(pageBreak);
pageBreak.LocationF = new PointF(0, 25);
```

**XRLine** — horizontal or vertical rule:
```csharp
var line = new XRLine();
detail.Controls.Add(line);
line.LineDirection = DevExpress.XtraReports.UI.LineDirection.Vertical;
line.LineStyle = DevExpress.Drawing.DXDashStyle.Dash;
line.LineWidth = 1F;
line.LocationF = new System.Drawing.PointF(0, 0);
line.SizeF = new System.Drawing.SizeF(100F, 25F);
```

### Special

**XRSubreport** — embed another report:
```csharp
var sub = new XRSubreport();
detail.Controls.Add(sub);
sub.BoundsF = new RectangleF(0, 0, 400, 100);
sub.ReportSource = new SubReport();
```

**XRTableOfContents** — auto-generated TOC (uses bookmarks).

**XRPdfContent** — embed a PDF file as report content.

**XRPdfSignature** — digital PDF signature field.

## Common Control Properties

| Property | Type | Description |
|----------|------|-------------|
| `BoundsF` | `RectangleF` | Position and size (x, y, width, height in report units) |
| `LocationF` | `PointF` | Position only in report units |
| `SizeF` | `SizeF` | Size only in report units |
| `Visible` | `bool` | Show/hide control |
| `Font` | `DXFont` | Text font |
| `ForeColor` | `Color` | Text color |
| `BackColor` | `Color` | Background (Transparent by default) |
| `Borders` | `BorderSide` | Which borders to draw |
| `BorderWidth` | `float` | Border width in pixels |
| `BorderColor` | `Color` | Border color |
| `Padding` | `PaddingInfo` | Inner padding |
| `TextAlignment` | `TextAlignment` | Text alignment within the control |
| `CanGrow` | `bool` | Expand height for long content |
| `CanShrink` | `bool` | Shrink height when content is short |

## ExpressionBinding

Bind any control property to a data field or expression:

```csharp
control.ExpressionBindings.Add(new ExpressionBinding(eventName,propertyName, expression));
// eventName : "BeforePrint" or "PrintOnPage"
// propertyName: "Text", "Visible", "BackColor", "Image", etc.
// expression: "[FieldName]", "[Price] * [Qty]", "Iif([Status]='Active', True, False)", etc. See expressions.md
```

# Table Controls

## When to Use This Reference

Use when creating structured grid layouts with multiple columns: `XRTable`, `XRTableRow`, `XRTableCell`.

## XRTable / XRTableRow / XRTableCell — Structured Grid Layout

Structured grid layout for displaying data in columns:

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

**XRTableCell** is derived from `XRLabel` and inherits its key properties and methods.

**Main content property of XRTableCell**: `Text`.

**Key properties**:
- `WidthF` on cells — set absolute column widths in pixels. The sum of all `WidthF` values in a row must equal the table's `SizeF.Width`.
- `RowSpan` — merge cells vertically.

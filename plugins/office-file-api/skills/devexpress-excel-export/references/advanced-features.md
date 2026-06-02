# Advanced Features — DevExpress Excel Export Library

This reference covers the advanced capabilities of the Excel Export Library beyond basic cell writing: tables, formulas, sparklines, pictures, printing, row/column grouping, AutoFilter, and data validation.

## When to Use This Reference

Use this when you need to:
- Create Excel tables (IXlTable) with built-in styles, banding, and calculated columns
- Write cell formulas using string parsing, expression objects, or low-level tokens
- Create shared formulas (applied across a range at once)
- Add SUBTOTAL aggregation rows
- Insert sparklines (line, column, win/loss charts) and customize their appearance
- Embed pictures in a worksheet and add hyperlinks to them
- Configure print settings: page orientation, margins, headers/footers, page breaks, print titles
- Group rows or columns with outline levels
- Enable AutoFilter on a data range with optional pre-configured filter criteria
- Add data validation rules to cell ranges

## Tables (IXlTable)

Excel tables provide built-in banding, sorting, and filtering UI. Create a table after writing the data rows.

### Key Classes

| Class/Interface | Purpose |
|----------------|---------|
| `IXlTable` | A table in a worksheet |
| `IXlTableColumn` | A column within a table |
| `XlTableStyleId` | Enum of built-in table style names |

### Create a Table

```csharp
// Tables in Excel Export Library are row-based (streaming API).
// Call BeginTable on the header row, EndTable on the last data row.

IXlTable table;

// 1. Write the header row and begin the table
using (IXlRow headerRow = sheet.CreateRow())
{
    table = headerRow.BeginTable(new[] { "Product", "Q1", "Q2", "Total" }, hasHeaderRow: true);
    table.Style.Name = XlTableStyleId.TableStyleMedium2;
    table.Columns[3].TotalRowFunction = XlTotalRowFunction.Sum;
    table.Columns[3].TotalRowLabel = "Total";
}

// 2. Write data rows
using (IXlRow row = sheet.CreateRow())
{
    using (IXlCell cell = row.CreateCell()) { cell.Value = "Widget A"; }
    using (IXlCell cell = row.CreateCell()) { cell.Value = 15000; }
    using (IXlCell cell = row.CreateCell()) { cell.Value = 18500; }
    using (IXlCell cell = row.CreateCell()) { cell.Value = 33500; }
}

// 3. End the table on the last row
using (IXlRow lastRow = sheet.CreateRow())
{
    using (IXlCell cell = lastRow.CreateCell()) { cell.Value = "Widget B"; }
    using (IXlCell cell = lastRow.CreateCell()) { cell.Value = 22000; }
    using (IXlCell cell = lastRow.CreateCell()) { cell.Value = 19800; }
    using (IXlCell cell = lastRow.CreateCell()) { cell.Value = 41800; }
    lastRow.EndTable(table, hasTotalRow: true);
}
```

### Calculated Columns

```csharp
// Column D gets a formula automatically applied to all data rows
table.Columns[3].Formula = "=[Qty]*[Unit Price]";
// [ColumnName] syntax refers to other table columns by name
```

## Formulas

Three methods are available for writing formulas, ordered from simplest to most flexible.

### Method 1: String Formula (requires XlFormulaParser)

```csharp
// Exporter must be created with XlFormulaParser
IXlExporter exporter = XlExport.CreateExporter(XlDocumentFormat.Xlsx, new XlFormulaParser());

// Then in a cell:
cell.SetFormula("=IF(B2>100, B2*0.9, B2)");
cell.SetFormula("=SUM(C2:C20)");
cell.SetFormula("=VLOOKUP(A2, Sheet2!A:B, 2, FALSE)");
```

Always use **invariant (English) function names** — they are locale-independent.

### Method 2: Expression Objects (no parser needed)

Build formulas from `XlFunc` (functions), `XlOper` (operators), and `XlFunc.Param` (constants):

```csharp
// =SUM($D$2:$D$10) + 100
IXlFormulaParameter sumRange = XlFunc.Sum(
    XlCellRange.FromLTRB(3, 1, 3, 9).AsAbsolute()); // $D$2:$D$10
IXlFormulaParameter fee = XlFunc.Param(100);
cell.SetFormula(XlOper.Add(sumRange, fee));

// =IF(B2>15, C2*B2*0.8, C2*B2)
// Use cell row/column index from the current cell
IXlFormulaParameter qty = XlFunc.Param(new XlCellPosition(1, row.RowIndex));  // B (col=1)
IXlFormulaParameter price = XlFunc.Param(new XlCellPosition(2, row.RowIndex)); // C (col=2)
cell.SetFormula(XlFunc.If(
    XlOper.Greater(qty, XlFunc.Param(15)),
    XlOper.Multiply(XlOper.Multiply(price, qty), XlFunc.Param(0.8)),
    XlOper.Multiply(price, qty)));
```

### Method 3: SUBTOTAL (no parser needed)

```csharp
// SUBTOTAL(9, C2:C10) — SUM that ignores hidden rows
cell.SetFormula(XlFunc.Subtotal(
    XlCellRange.FromLTRB(2, 1, 2, 9),  // C2:C10 (0-based column 2)
    XlSummary.Sum,
    ignoreHiddenRows: true));
```

### Shared Formulas

Apply the same formula pattern to a range of cells (efficient storage):

```csharp
// The formula in the first cell serves as the template
// Excel adjusts relative references for each subsequent cell automatically
using (IXlCell cell = row.CreateCell())
{
    cell.SetSharedFormula("=B2*C2", XlCellRange.FromLTRB(3, 1, 3, 10));
}
// Subsequent cells in the range reference the shared formula
// by calling SetSharedFormula with only the range (no formula string)
```

## Sparklines

Sparklines are small in-cell charts placed in a single column alongside data rows.

```csharp
// Data in columns A-D (rows 1-5), sparklines in column E
XlSparklineGroup sparklineGroup = new XlSparklineGroup(
    XlCellRange.FromLTRB(0, 1, 3, 5),  // data range (A2:D6)
    XlCellRange.FromLTRB(4, 1, 4, 5)); // sparkline location (E2:E6)

// Type: Line, Column, or WinLoss
sparklineGroup.SparklineType = XlSparklineType.Line;

// Customize appearance
sparklineGroup.ShowMarkers = true;
sparklineGroup.HighlightHighest = true;
sparklineGroup.HighlightLowest = true;
sparklineGroup.SeriesColor = XlColor.FromTheme(XlThemeColor.Accent1, 0.0);
sparklineGroup.HighColor = XlColor.FromArgb(0x00, 0xB0, 0x50);  // green for high
sparklineGroup.LowColor = XlColor.FromArgb(0xFF, 0x00, 0x00);   // red for low

// Axis settings
sparklineGroup.MinScalingType = XlSparklineAxisScaling.Fixed;
sparklineGroup.MinAxisValue = 0;
sparklineGroup.MaxScalingType = XlSparklineAxisScaling.Group;   // same scale for all

sheet.SparklineGroups.Add(sparklineGroup);
```

## Pictures

Embed images in a worksheet by specifying a cell anchor.

```csharp
// Load image data
byte[] imageData = File.ReadAllBytes("logo.png");

// Create picture anchored to cells B2:D5
IXlPicture picture = sheet.CreatePicture();
picture.SetImage(imageData, XlImageFormat.Png);
picture.SetTwoCellAnchor(
    new XlAnchorPoint(1, 1, 0, 0),   // top-left: column B (1), row 2 (1), zero offsets
    new XlAnchorPoint(3, 4, 0, 0));  // bottom-right: column D (3), row 5 (4)

// Optional: add a hyperlink to the picture
XlPictureHyperlink picLink = new XlPictureHyperlink();
picLink.TargetUri = "https://www.devexpress.com";
picture.HyperlinkClick = picLink;
```

## Printing

Configure page layout and print settings through `IXlSheet` properties.

```csharp
// Page orientation and paper size
sheet.PageSetup = new XlPageSetup();
sheet.PageSetup.PaperKind = XlPaperKind.A4;
sheet.PageSetup.PageOrientation = XlPageOrientation.Landscape;
sheet.PageSetup.FitToPage = true;
sheet.PageSetup.FitToWidth = 1;  // fit all columns to 1 page wide
sheet.PageSetup.FitToHeight = 0; // unlimited pages tall

// Page margins (in inches)
sheet.PageMargins = new XlPageMargins();
sheet.PageMargins.Left = 0.5;
sheet.PageMargins.Right = 0.5;
sheet.PageMargins.Top = 0.75;
sheet.PageMargins.Bottom = 0.75;
sheet.PageMargins.Header = 0.3;
sheet.PageMargins.Footer = 0.3;

// Print options
sheet.PrintOptions = new XlPrintOptions();
sheet.PrintOptions.PrintGridLines = false;
sheet.PrintOptions.BlackAndWhite = false;
sheet.PrintOptions.CenterHorizontally = true;

// Headers and footers
sheet.HeaderFooter = new XlHeaderFooter();
sheet.HeaderFooter.OddHeader = "&CMonthly Sales Report";   // centered header
sheet.HeaderFooter.OddFooter = "&LConfidential&RPage &P of &N"; // left + right footer
// &L=left, &C=center, &R=right, &P=page number, &N=total pages, &D=date, &T=time

// Print titles (repeat rows/columns on each page)
sheet.PrintTitles = new XlPrintTitles();
sheet.PrintTitles.SetRows(0, 0);     // repeat row 1 (index 0) on every page
sheet.PrintTitles.SetColumns(0, 0);  // repeat column A (index 0) on every page

// Manual page breaks (0-based row/column indices)
sheet.RowPageBreaks.Add(24);    // insert a horizontal break before row 25 (0-based index 24)
sheet.ColumnPageBreaks.Add(5);  // insert a vertical break before column F (0-based index 5)
```

## Row and Column Grouping

Group rows or columns to create a collapsible outline.

```csharp
// Group rows 2-6 (0-based indices 1-5) at outline level 1
using (IXlRow row = sheet.CreateRow())
{
    row.OutlineLevel = 1; // set this on each row in the group
    // ... write cells
}

// Group columns B-D (indices 1-3) at outline level 1
using (IXlColumn col = sheet.CreateColumn()) { col.OutlineLevel = 1; }
using (IXlColumn col = sheet.CreateColumn()) { col.OutlineLevel = 1; }
using (IXlColumn col = sheet.CreateColumn()) { col.OutlineLevel = 1; }
```

## AutoFilter

Enable the AutoFilter dropdown buttons on a data range:

```csharp
// Enable on all data written so far
sheet.AutoFilterRange = sheet.DataRange;

// Or specify a fixed range
sheet.AutoFilterRange = XlCellRange.FromLTRB(0, 0, 3, 0); // header row only — Excel infers the data range
```

Pre-configure a filter criterion programmatically:

```csharp
// Filter column B (index 1) to show only values > 1000
XlFilterColumn filterCol = new XlFilterColumn();
filterCol.ColumnIndex = 1; // 0-based, relative to AutoFilterRange start
XlCustomFilters customFilter = new XlCustomFilters();
customFilter.Filter1 = new XlCustomFilterCriteria(XlFilterOperator.GreaterThan, "1000");
filterCol.FilterCriteria = customFilter;
sheet.AutoFilterColumns.Add(filterCol);
```

## Data Validation

Restrict cell input to a defined set of values or a range.

```csharp
// Drop-down list validation on C2:C100
XlDataValidation validation = new XlDataValidation();
validation.Ranges.Add(XlCellRange.FromLTRB(2, 1, 2, 99)); // column C, rows 2-100

validation.ValidationType = XlDataValidationType.List;
validation.Formula1 = "\"Pending,Approved,Rejected\""; // comma-separated list (no spaces)
// Or reference a range: validation.Formula1 = "Sheet2!$A$1:$A$3";

validation.ShowDropDown = false; // false = show dropdown arrow (confusingly named)
validation.ShowErrorAlert = true;
validation.ErrorAlertStyle = XlDataValidationAlertStyle.Stop;
validation.ErrorTitle = "Invalid Input";
validation.ErrorMessage = "Please select a value from the list.";

validation.ShowInputMessage = true;
validation.PromptTitle = "Status";
validation.Prompt = "Choose the approval status.";

sheet.DataValidations.Add(validation);
```

### Numeric Range Validation

```csharp
XlDataValidation numValidation = new XlDataValidation();
numValidation.Ranges.Add(XlCellRange.FromLTRB(1, 1, 1, 99)); // column B, rows 2-100
numValidation.ValidationType = XlDataValidationType.Decimal;
numValidation.ValidationOperator = XlDataValidationOperator.Between;
numValidation.Formula1 = "0";
numValidation.Formula2 = "100";
numValidation.ShowErrorAlert = true;
numValidation.ErrorAlertStyle = XlDataValidationAlertStyle.Warning;
numValidation.ErrorMessage = "Value must be between 0 and 100.";
sheet.DataValidations.Add(numValidation);
```

## Configuration Summary

| Feature | Key property / method | Notes |
|---------|----------------------|-------|
| Table | `headerRow.BeginTable(string[], bool)` / `lastRow.EndTable(IXlTable, bool)` | Row-based streaming API |
| Table style | `table.Style.Name = XlTableStyleId.*` | 60+ built-in styles available |
| Totals row | `table.ShowTotalsRow = true` | Then set `table.Columns[n].TotalRowFunction` |
| String formula | `cell.SetFormula(string)` | Requires `XlFormulaParser` in exporter |
| Expression formula | `cell.SetFormula(IXlFormulaParameter)` | Use `XlFunc` and `XlOper` |
| Sparkline type | `sparklineGroup.SparklineType` | `Line`, `Column`, `WinLoss` |
| Picture format | `picture.SetImage(bytes, XlImageFormat.Png)` | Also supports Jpeg, Bmp |
| Page orientation | `sheet.PageSetup.PageOrientation` | `Portrait` or `Landscape` |
| Page break (row) | `sheet.RowPageBreaks.Add(rowIndex)` | 0-based row index before which the break is inserted |
| Page break (column) | `sheet.ColumnPageBreaks.Add(colIndex)` | 0-based column index before which the break is inserted |
| Row group level | `row.OutlineLevel` | 1–7; set on each row in the group |
| AutoFilter | `sheet.AutoFilterRange` | Assign `sheet.DataRange` or a fixed range |
| Data validation type | `validation.ValidationType` | `List`, `Decimal`, `Integer`, `Date`, `TextLength`, `Custom` |

## Troubleshooting

- **Table creation fails or produces corrupt file**: Ensure the table range includes the header row and all data rows. The range must not overlap another table.
- **Calculated column formula not applied**: The formula uses structured reference syntax `=[ColumnName]`. Column names match the header cell values exactly.
- **Sparklines not visible**: Call `sheet.SparklineGroups.Add(sparklineGroup)` after fully configuring the group. The data range and location range must have the same number of rows.
- **Picture anchor out of bounds**: `XlAnchorPoint(column, row, colOffset, rowOffset)` uses 0-based column and row indices. The columns/rows must have been defined (via `CreateColumn`) or exist by default.
- **Print titles cause corrupt output**: Set `sheet.PrintTitles` before creating any rows (or at least before finalizing the sheet). Row and column indices are 0-based.
- **Data validation list not showing**: Verify `ShowDropDown = false` (this property name is inverted from its meaning — `false` shows the dropdown).
- **AutoFilter filter criterion ignored on open**: Some pre-configured filter types may not be honored by all Excel versions. Test with the target Excel version.

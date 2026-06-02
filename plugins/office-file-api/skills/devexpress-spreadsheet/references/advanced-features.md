# Advanced Features — DevExpress Spreadsheet Document API

Tables (ListObjects), data validation, sparklines, shapes, pictures, hyperlinks, comments, mail merge, named ranges, and data binding.

## When to Use This Reference

Use this when you need to:
- Create and manage Tables (ListObjects) with structured references and auto-styles
- Add data validation rules (dropdown lists, number ranges, date constraints, custom formulas)
- Insert sparklines (mini-charts within cells)
- Add shapes (rectangles, ellipses, callouts, etc.) to worksheets
- Insert and modify pictures (from file, stream, or bytes)
- Add hyperlinks to cells or pictures
- Add simple notes or threaded comments to cells
- Perform mail merge from a collection or DataTable data source
- Work with workbook-level or worksheet-level named ranges
- Bind worksheet ranges to external data sources

## Tables (ListObjects)

### Create a Table

```csharp
using DevExpress.Spreadsheet;

Worksheet sheet = workbook.Worksheets[0];

// Define data first
sheet["A1"].Value = "Product"; sheet["B1"].Value = "Price"; sheet["C1"].Value = "Stock";
sheet["A2"].Value = "Widget A"; sheet["B2"].Value = 29.99; sheet["C2"].Value = 100;
sheet["A3"].Value = "Widget B"; sheet["B3"].Value = 49.99; sheet["C3"].Value = 50;

// Create a table from the data range (includes header row)
Table table = sheet.Tables.Add(sheet.Range["A1:C3"], hasHeaders: true);
table.Name = "ProductTable";

// Apply a built-in table style
table.Style = workbook.TableStyles["TableStyleMedium9"];

// Show banded rows and filter button
table.ShowRowStripes = true;
table.ShowAutoFilterButton = true;
```

### Work with Table Columns

```csharp
// Access a table column
TableColumn priceCol = table.Columns["Price"];

// Set a totals row formula for the column
table.ShowTotals = true;
priceCol.TotalsRowFunction = TotalsRowFunction.Average;

// Add a calculated column using structured references
TableColumn taxCol = table.Columns.Add();
taxCol.Name = "Tax";
taxCol.Formula = "=[Price]*0.1"; // Structured reference formula
```

### Convert Table to Range

```csharp
// Remove the table structure but keep the data and formatting
table.ConvertToRange();
```

## Data Validation

### Dropdown List Validation

```csharp
using DevExpress.Spreadsheet;

Worksheet sheet = workbook.Worksheets[0];

// Add a dropdown list from a fixed set of values
DataValidation validation = sheet.DataValidations.Add(
    sheet.Range["B2:B100"],
    DataValidationType.List,
    DataValidationOperator.Between,
    "\"North,South,East,West\"",
    null);

validation.ShowDropDown = true;
validation.ShowInputMessage = true;
validation.InputTitle = "Select Region";
validation.InputMessage = "Choose from the list.";
validation.ShowErrorMessage = true;
validation.ErrorStyle = DataValidationErrorStyle.Stop;
validation.ErrorTitle = "Invalid Entry";
validation.ErrorMessage = "Please select a valid region.";
```

### Numeric Range Validation

```csharp
// Allow only whole numbers between 1 and 100
DataValidation numValidation = sheet.DataValidations.Add(
    sheet.Range["C2:C100"],
    DataValidationType.WholeNumber,
    DataValidationOperator.Between,
    "1",
    "100");

numValidation.ShowErrorMessage = true;
numValidation.ErrorStyle = DataValidationErrorStyle.Warning;
numValidation.ErrorMessage = "Value must be between 1 and 100.";
```

### Date Validation

```csharp
// Allow only dates from today onward
DataValidation dateValidation = sheet.DataValidations.Add(
    sheet.Range["D2:D100"],
    DataValidationType.Date,
    DataValidationOperator.GreaterThanOrEqual,
    "TODAY()",
    null);
```

### Custom Formula Validation

```csharp
// Require the value in B2 to be unique (not in B1:B1)
DataValidation customValidation = sheet.DataValidations.Add(
    sheet.Range["B2:B100"],
    DataValidationType.Custom,
    DataValidationOperator.Between,
    "=COUNTIF($B$2:$B$100,B2)=1",
    null);
```

### Validate and Find Invalid Cells

```csharp
// Check if a specific cell passes all validation rules
bool isValid = sheet.DataValidations.Validate(sheet.Cells["B5"]);

// Get all cells that fail validation
IList<Cell> invalidCells = sheet.DataValidations.GetInvalidCells();
foreach (Cell c in invalidCells)
    Console.WriteLine($"Invalid: {c.GetReferenceA1()}");
```

## Sparklines

Sparklines are mini-charts that fit inside a single cell:

```csharp
using DevExpress.Spreadsheet;

Worksheet sheet = workbook.Worksheets[0];

// Data range: A1:D4 (4 rows of data, each row produces one sparkline)
// Sparkline location: column E (one cell per row)
SparklineGroup group = sheet.SparklineGroups.Add(
    SparklineGroupType.Line,          // Type: Line, Column, WinLoss
    sheet.Range["A2:D5"],             // Data range
    sheet.Range["E2:E5"]);            // Location (one cell per sparkline)

// Style the sparklines
group.MarkersVisible = true;
group.HighPointColor = System.Drawing.Color.Green;
group.LowPointColor = System.Drawing.Color.Red;
group.LineWeight = 1.5;
```

## Shapes

```csharp
using DevExpress.Spreadsheet;
using System.Drawing;

Worksheet sheet = workbook.Worksheets[0];

// Set unit to points before adding shapes
workbook.Unit = DevExpress.Office.DocumentUnit.Point;

// Add a rectangle shape at (left=50, top=50, width=200, height=100) in points
Shape shape = sheet.Shapes.AddShape(ShapeGeometryPreset.Rectangle, 50, 50, 200, 100);

// Add text to the shape
shape.ShapeText.Characters("Important Note").Font.Bold = true;

// Style the shape
shape.Fill.SetSolidFill(Color.LightBlue);
shape.Outline.SetSolidFill(Color.DarkBlue);
shape.Outline.Width = 2;
```

## Pictures

### Insert a Picture from File

```csharp
using DevExpress.Spreadsheet;

Worksheet sheet = workbook.Worksheets[0];

// Insert anchored to the top-left corner of a cell
Picture picture = sheet.Pictures.AddPicture("logo.png", sheet.Cells["A1"]);

// Insert at a specific position (left, top, width, height) in the workbook's unit
workbook.Unit = DevExpress.Office.DocumentUnit.Millimeter;
Picture sized = sheet.Pictures.AddPicture("logo.png", 10, 10, 80, 30, lockAspectRatio: true);
```

### Insert a Picture from Stream

```csharp
using System.IO;

// The overload that accepts a stream requires position + size parameters
workbook.Unit = DevExpress.Office.DocumentUnit.Point;
using (FileStream imageStream = File.OpenRead("logo.png"))
{
    Picture picture = sheet.Pictures.AddPicture(imageStream, 50, 50, 150, 100);
}
```

### Modify a Picture

```csharp
Picture pic = sheet.Pictures[0];

// Resize by specifying top-left/bottom-right cells
pic.TopLeftCell = sheet.Cells["B2"];
pic.BottomRightCell = sheet.Cells["F10"];

// Or set exact position (in the workbook's unit)
pic.Top = 50;
pic.Left = 100;
pic.Width = 200;
pic.Height = 150;
```

## Hyperlinks

### Add a Hyperlink to a Cell

```csharp
using DevExpress.Spreadsheet;

Worksheet sheet = workbook.Worksheets[0];

// URL hyperlink
Hyperlink link = sheet.Hyperlinks.Add(sheet.Cells["A1"],
    "https://www.devexpress.com", isExternal: true, "DevExpress");

// ScreenTip tooltip
link.ScreenTip = "Visit DevExpress website";

// Internal hyperlink (to another cell in the workbook)
Hyperlink internalLink = sheet.Hyperlinks.Add(sheet.Cells["B2"],
    "Sheet2!A1", isExternal: false, "Go to Sheet2");

// Email hyperlink
Hyperlink emailLink = sheet.Hyperlinks.Add(sheet.Cells["C3"],
    "mailto:support@devexpress.com", isExternal: true, "Contact Support");
```

## Comments (Simple Notes)

```csharp
using DevExpress.Spreadsheet;

Worksheet sheet = workbook.Worksheets[0];

// Add a simple note (legacy comment) to a cell
Comment comment = sheet.Comments.Add(sheet.Cells["C3"], "Author Name",
    "This value needs verification.");

// Access and modify existing comment
comment.Text = "Updated: verified on 2026-04-13";
comment.Visible = false; // Hidden by default
```

## Named Ranges

### Workbook-Scoped Named Ranges

```csharp
using DevExpress.Spreadsheet;

// Add a named range
workbook.DefinedNames.Add("SalesData", "Sheet1!$B$2:$B$100");

// Add a named constant
workbook.DefinedNames.Add("VATRate", "0.20");

// Use in formula
sheet["D2"].FormulaInvariant = "=B2*(1+VATRate)";

// Get and update
DefinedName name = workbook.DefinedNames.GetDefinedName("VATRate");
name.RefersTo = "0.25";

// Remove
workbook.DefinedNames.Remove("VATRate");
```

### Worksheet-Scoped Named Ranges

```csharp
// Worksheet-level names (visible only within the sheet)
sheet.DefinedNames.Add("LocalSales", "Sheet1!$C$2:$C$50");
```

## Mail Merge

Mail merge generates one or more workbooks from a template and a data source:

```csharp
using DevExpress.Spreadsheet;

Workbook workbook = new Workbook();
Worksheet template = workbook.Worksheets[0];

// Build a mail merge template using FIELD() formula markers
template.Cells["A1"].Value = "Name:";
template.Cells["B1"].Formula = "=FIELD(\"FirstName\")&\" \"&FIELD(\"LastName\")";
template.Cells["A2"].Value = "Email:";
template.Cells["B2"].Formula = "FIELD(\"Email\")";
template.Cells["A3"].Value = "Amount:";
template.Cells["B3"].Formula = "FIELD(\"Amount\")";
template.Cells["B3"].NumberFormat = "$#,##0.00";

// Define the detail range (required)
CellRange detailRange = template.Range["A1:B3"];
detailRange.Name = "DETAILRANGE";

// Set merge mode: "Worksheets" = one sheet per record
workbook.DefinedNames.Add("MAILMERGEMODE", "=\"Worksheets\"");

// Bind the data source (any IEnumerable or DataTable)
workbook.MailMergeDataSource = GetCustomerList();

// Execute the mail merge
IList<Workbook> results = workbook.GenerateMailMergeDocuments();

// Save the first result
results[0].SaveDocument("merged_output.xlsx");
```

### Mail Merge Modes

| `MAILMERGEMODE` value | Description |
|----------------------|-------------|
| `"Worksheets"` | One worksheet per record, all in one workbook |
| `"Documents"` | One separate workbook per record |
| `"OneWorksheet"` | All records on a single worksheet |

## Data Binding

Bind a worksheet range to a live data source:

```csharp
using DevExpress.Spreadsheet;

Worksheet sheet = workbook.Worksheets[0];

// Bind an IEnumerable<T> or DataTable to a range
ExternalDataSourceOptions options = new ExternalDataSourceOptions();
options.ImportHeaders = true;

WorksheetDataBinding binding = sheet.DataBindings.BindToDataSource(
    myDataSource,
    sheet.Range["A1:E100"],
    options);

// Remove binding when no longer needed
sheet.DataBindings.Remove(binding);
```

## Troubleshooting

- **Table formula structured references not working**: Ensure the cell is inside the table range. Structured references like `=[Column]` only resolve within the table context.
- **Dropdown validation list not appearing**: Wrap the list values in escaped double quotes: `"\"North,South,East,West\""`. Do not use a range reference if you want literal values.
- **Sparkline group has wrong number of cells**: The data range row count must match the location range row count (one sparkline per row).
- **Mail merge produces no output**: Verify that `DETAILRANGE` is defined as a named range in the template. It is required for the merge engine to locate the repeating block.
- **Hyperlink navigates to wrong location**: For internal links, use the format `"SheetName!A1"` (with the exact sheet name). Sheet names with spaces must be quoted: `"'My Sheet'!A1"`.
- **Comment not visible**: Comments are hidden by default. Set `comment.Visible = true` to display, or open in Excel and right-click → Show Comment.

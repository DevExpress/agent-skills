# Cell Formatting — DevExpress Spreadsheet Document API

Apply visual formatting to cells and ranges including fonts, colors, borders, number formats, alignment, styles, and conditional formatting.

## When to Use This Reference

Use this when you need to:
- Apply fonts (bold, italic, size, color, family) to cells or ranges
- Set cell background colors (fill)
- Add borders to cells or ranges
- Format numbers (dates, currencies, percentages, custom formats)
- Align cell content (horizontal, vertical, wrap text, indent, rotation)
- Merge or unmerge cells
- Apply conditional formatting rules (data bars, color scales, icon sets, cell-is rules)
- Adjust row height and column width
- Use `BeginUpdateFormatting`/`EndUpdateFormatting` for efficient range formatting

## Key Classes and Types

| Class/Interface | Purpose |
|----------------|---------|
| `Cell` | Individual cell with direct formatting properties |
| `CellRange` | Range of cells; supports batch formatting |
| `Formatting` | Snapshot object returned by `BeginUpdateFormatting` for batch edits |
| `SpreadsheetFont` | Font properties (Name, Size, Bold, Italic, Color, Underline) |
| `Borders` | Border styles for all four sides plus diagonal |
| `Alignment` | Horizontal/vertical alignment, wrap text, indent, rotation angle |
| `ConditionalFormattingCollection` | Conditional formatting rules on a worksheet |

## Basic Formatting

### Font Styling

```csharp
using DevExpress.Spreadsheet;
using System.Drawing;

Worksheet sheet = workbook.Worksheets[0];

// Single cell — direct property access
Cell cell = sheet["A1"];
cell.Value = "Bold Title";
cell.Font.Bold = true;
cell.Font.Size = 14;
cell.Font.Name = "Calibri";
cell.Font.Color = Color.DarkBlue;
cell.Font.Italic = true;
cell.Font.Underline = UnderlineType.Single;

// Range of cells — direct property access (each cell updated individually)
CellRange range = sheet.Range["B2:D2"];
range.Font.Bold = true;
range.Font.Size = 11;
```

### Efficient Batch Range Formatting

For ranges, use `BeginUpdateFormatting`/`EndUpdateFormatting` — this applies all changes in a single pass:

```csharp
CellRange range = sheet.Range["A1:F10"];
Formatting fmt = range.BeginUpdateFormatting();

fmt.Font.Name = "Calibri";
fmt.Font.Bold = true;
fmt.Font.Size = 12;
fmt.Font.Color = Color.DarkBlue;
fmt.Fill.BackgroundColor = Color.LightYellow;
fmt.Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
fmt.Alignment.Vertical = SpreadsheetVerticalAlignment.Center;

range.EndUpdateFormatting(fmt);
```

### Background Color

```csharp
// Single cell — FillColor shorthand
sheet["A1"].FillColor = Color.LightYellow;

// Range with RGB
sheet.Range["A1:D1"].FillColor = Color.FromArgb(68, 114, 196);

// Via Formatting object (inside BeginUpdateFormatting block)
fmt.Fill.BackgroundColor = Color.LightSkyBlue;

// Remove fill (transparent)
sheet["A1"].FillColor = Color.Empty;
```

### Borders

```csharp
CellRange range = sheet.Range["A1:D5"];

// All borders (inside and outside)
range.Borders.SetAllBorders(Color.Black, BorderLineStyle.Thin);

// Outside border only (thicker outline)
range.Borders.SetOutsideBorders(Color.Black, BorderLineStyle.Medium);

// Inside borders only (grid lines)
range.Borders.SetInsideBorders(Color.Gray, BorderLineStyle.Hair);

// Specific sides
range.Borders.BottomBorder.LineStyle = BorderLineStyle.Double;
range.Borders.BottomBorder.Color = Color.DarkBlue;

// Remove borders
range.Borders.SetAllBorders(Color.Empty, BorderLineStyle.None);
```

### Alignment

```csharp
Cell cell = sheet["A1"];
cell.Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
cell.Alignment.Vertical = SpreadsheetVerticalAlignment.Center;
cell.Alignment.WrapText = true;
cell.Alignment.Indent = 2;
cell.Alignment.RotationAngle = 45; // Rotate text 45 degrees
```

### Merge Cells

```csharp
// Merge a range (top-left cell retains its value)
sheet.MergeCells(sheet.Range["A1:D1"]);

// Unmerge
sheet.UnMergeCells(sheet.Range["A1:D1"]);
```

## Number Formats

Number formats use Excel format codes (not .NET format strings):

```csharp
// Currency
sheet["A1"].NumberFormat = "$#,##0.00";

// Percentage
sheet["A2"].NumberFormat = "0.00%";

// Date
sheet["A3"].NumberFormat = "yyyy-MM-dd";
sheet["A3"].Value = DateTime.Now;

// Time
sheet["A4"].NumberFormat = "HH:mm:ss";

// Thousands separator (no decimals)
sheet["A5"].NumberFormat = "#,##0";

// Negative numbers in red parentheses
sheet["A6"].NumberFormat = "#,##0;[Red](#,##0)";

// Text format (prevents auto-conversion of leading zeros)
sheet["A7"].NumberFormat = "@";
sheet["A7"].Value = "001234"; // Kept as text

// Accounting format
sheet["A8"].NumberFormat = "_(* #,##0.00_);_(* (#,##0.00);_(* \"-\"??_);_(@_)";
```

## Row and Column Sizing

```csharp
// Set column width (in characters)
sheet.Columns["A"].Width = 25;

// Set column width in characters (more precise)
sheet.Columns["B"].WidthInCharacters = 15;

// Set row height (in points)
sheet.Rows[0].Height = 30;

// Auto-fit column width to content
sheet.Columns["A"].AutoFit();

// Auto-fit a range of columns (0-based index)
sheet.Columns.AutoFit(0, 5); // Columns A through F

// Auto-fit rows
sheet.Rows.AutoFit(0, 10); // Rows 1 through 11 (0-based)
```

## Conditional Formatting

```csharp
ConditionalFormattingCollection cf = sheet.ConditionalFormattings;

// Cells > 1000 → green background
var rule = cf.AddCellIs(sheet.Range["B2:B100"],
    ConditionalFormattingOperator.GreaterThan, "1000");
rule.Formatting.Fill.BackgroundColor = Color.LightGreen;

// Cells between 500 and 1000 → yellow
var rule2 = cf.AddCellIs(sheet.Range["B2:B100"],
    ConditionalFormattingOperator.Between, "500", "1000");
rule2.Formatting.Fill.BackgroundColor = Color.LightYellow;

// Data bars
cf.AddDataBar(sheet.Range["C2:C20"], Color.CornflowerBlue);

// Two-color scale (red → green)
cf.AddColorScale2(sheet.Range["D2:D20"], Color.Red, Color.Green);

// Three-color scale (red → yellow → green)
cf.AddColorScale3(sheet.Range["E2:E20"], Color.Red, Color.Yellow, Color.Green);

// Icon set
cf.AddIconSet(sheet.Range["F2:F20"], IconSetType.Arrows3);

// Top/bottom ranked values — highlight top 10
var topRule = cf.AddTopBottom(sheet.Range["G2:G20"],
    ConditionalFormattingTopBottomType.Top, 10, false);
topRule.Formatting.Fill.BackgroundColor = Color.LightBlue;
```

## Common Scenarios

### Create a Styled Header Row

```csharp
CellRange headers = sheet.Range["A1:F1"];
Formatting fmt = headers.BeginUpdateFormatting();
fmt.Font.Bold = true;
fmt.Font.Size = 12;
fmt.Font.Color = Color.White;
fmt.Fill.BackgroundColor = Color.FromArgb(68, 114, 196);
fmt.Alignment.Horizontal = SpreadsheetHorizontalAlignment.Center;
fmt.Borders.SetAllBorders(Color.White, BorderLineStyle.Thin);
headers.EndUpdateFormatting(fmt);
sheet.Rows[0].Height = 25;
```

### Alternating Row Colors (Zebra Striping)

```csharp
int lastRow = sheet.GetUsedRange().BottomRowIndex;
for (int row = 1; row <= lastRow; row++)
{
    if (row % 2 == 0)
    {
        // Range.FromLTRB(leftCol, topRow, rightCol, bottomRow) — 0-based
        sheet.Range.FromLTRB(0, row, 5, row).FillColor =
            Color.FromArgb(221, 235, 247);
    }
}
```

### Apply a Built-in Cell Style

```csharp
// Apply a named style from the workbook's style collection
sheet["A1"].Style = workbook.Styles["Good"];
sheet["A2"].Style = workbook.Styles["Bad"];
sheet["A3"].Style = workbook.Styles["Neutral"];
sheet["A4"].Style = workbook.Styles["Heading 1"];
```

### Create or Modify a Custom Style

```csharp
// Create a new named style
Style myStyle = workbook.Styles.Add("MyStyle");
myStyle.Font.Bold = true;
myStyle.Font.Color = Color.DarkGreen;
myStyle.Fill.BackgroundColor = Color.LightGreen;
myStyle.NumberFormat = "$#,##0.00";

// Apply it
sheet.Range["B2:B20"].Style = myStyle;
```

## Troubleshooting

- **Color does not appear**: Use `System.Drawing.Color` (not `DevExpress.Utils.DXColor` unless you are intentionally using the DX color type). Both are accepted by the API.
- **Number format shows literal text**: Format strings use Excel codes, not .NET strings. Use `"#,##0.00"` not `"{0:N2}"`.
- **Merged cells lose data**: Only the top-left cell of a merged range retains its value. Write data before merging, or write directly to the top-left cell reference.
- **AutoFit after formula load**: Call `workbook.Calculate()` before `AutoFit()` to ensure formula results are computed and column widths are accurate.
- **`BeginUpdateFormatting` not applied**: Ensure `EndUpdateFormatting(fmt)` is always called — even in a `finally` block — to apply the formatting snapshot.

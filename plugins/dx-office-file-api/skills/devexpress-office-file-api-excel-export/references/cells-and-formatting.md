# Cells and Formatting — DevExpress Excel Export Library

Cells are the fundamental data unit in a worksheet. Each cell holds a value, an optional formula, and formatting attributes. In the Excel Export Library, cells are written sequentially within a row using `IXlRow.CreateCell()`.

## When to Use This Reference

Use this when you need to:
- Create cells and set values of various types (string, number, bool, date, error)
- Apply fonts (name, size, bold, italic, color)
- Set cell background colors and fill patterns
- Add borders (inner, outer, individual sides)
- Configure cell alignment (horizontal, vertical, text wrap, indent)
- Specify number formats (currency, date, percentage, custom)
- Create rich text with mixed fonts in a single cell
- Merge cells or split merged cells
- Add hyperlinks to cells
- Apply conditional formatting rules

## Key Classes and Types

| Class/Interface | Purpose |
|----------------|---------|
| `IXlCell` | A cell in a worksheet; holds value, formula, and formatting |
| `XlVariantValue` | The typed value stored in a cell (string, number, bool, error) |
| `XlCellFormatting` | Container for all formatting attributes: font, fill, alignment, border, number format |
| `XlFont` | Font settings (name, size, bold, italic, underline, color, scheme) |
| `XlFill` | Background fill (solid color, pattern, or gradient) |
| `XlCellAlignment` | Horizontal/vertical alignment, text wrap, indent, rotation |
| `XlBorder` | Cell border lines (style and color per side) |
| `XlNumberFormat` | Number format string applied to a cell value |
| `XlColor` | Color value: RGB, ARGB, system color, or theme color |
| `XlRichTextString` | Rich-formatted text composed of runs with individual fonts |
| `XlRichTextRun` | A segment of rich text with its own font settings |
| `XlHyperlink` | A hyperlink associated with a cell or cell range |
| `XlConditionalFormatting` | Conditional formatting rule container |

## Basic Usage

```csharp
using DevExpress.Export.Xl;

// Build a formatting object once and reuse it
XlCellFormatting fmt = new XlCellFormatting();
fmt.Font = new XlFont();
fmt.Font.Bold = true;
fmt.Font.Size = 12;
fmt.Font.Color = XlColor.FromArgb(0x33, 0x33, 0x33);
fmt.Fill = XlFill.SolidFill(XlColor.FromArgb(0xFF, 0xF0, 0xC0));
fmt.Alignment = XlCellAlignment.FromHV(XlHorizontalAlignment.Center, XlVerticalAlignment.Center);
fmt.Border = XlBorder.OutlineBorders(XlColor.FromArgb(0x80, 0x80, 0x80), XlBorderLineStyle.Thin);
fmt.NumberFormat = "#,##0.00";

using (IXlRow row = sheet.CreateRow())
{
    using (IXlCell cell = row.CreateCell())
    {
        cell.Value = 1234.56;
        cell.ApplyFormatting(fmt);
    }
}
```

## Common Scenarios

### Setting Cell Values by Type

```csharp
using (IXlRow row = sheet.CreateRow())
{
    using (IXlCell cell = row.CreateCell()) { cell.Value = "Text string"; }
    using (IXlCell cell = row.CreateCell()) { cell.Value = 42.5; }          // numeric
    using (IXlCell cell = row.CreateCell()) { cell.Value = true; }          // boolean
    using (IXlCell cell = row.CreateCell()) { cell.Value = DateTime.Today; } // date
    using (IXlCell cell = row.CreateCell()) { cell.Value = XlVariantValue.ErrorName; } // #NAME?
}
```

Date values are stored as numbers in Excel. To display them correctly, apply a date number format:

```csharp
XlCellFormatting dateFmt = new XlCellFormatting();
dateFmt.NumberFormat = "yyyy-mm-dd";

using (IXlCell cell = row.CreateCell())
{
    cell.Value = DateTime.Today;
    cell.ApplyFormatting(dateFmt);
}
```

### Font Settings

```csharp
XlCellFormatting fmt = new XlCellFormatting();
fmt.Font = new XlFont();
fmt.Font.Name = "Calibri";
fmt.Font.SchemeStyle = XlFontSchemeStyles.None; // required when setting a specific font name
fmt.Font.Size = 14;
fmt.Font.Bold = true;
fmt.Font.Italic = true;
fmt.Font.Underline = XlUnderlineType.Single;
fmt.Font.Color = XlColor.FromArgb(0x1F, 0x49, 0x7D);
```

Use theme fonts instead of a named font:

```csharp
fmt.Font = XlFont.BodyFont();    // theme body font
fmt.Font = XlFont.HeadingsFont(); // theme heading font
```

### Background Fill

```csharp
// Solid color fill
fmt.Fill = XlFill.SolidFill(XlColor.FromArgb(0xDD, 0xEA, 0xF1));

// Theme color fill (Accent2, no tint)
fmt.Fill = XlFill.SolidFill(XlColor.FromTheme(XlThemeColor.Accent2, 0.0));

// Theme color fill, 40% lighter
fmt.Fill = XlFill.SolidFill(XlColor.FromTheme(XlThemeColor.Accent3, 0.4));
```

### Borders

```csharp
// All four sides — same style and color
fmt.Border = XlBorder.OutlineBorders(XlColor.FromArgb(0x70, 0x70, 0x70), XlBorderLineStyle.Thin);

// Individual sides
fmt.Border = new XlBorder();
fmt.Border.BottomColor = XlColor.FromArgb(0x00, 0x70, 0xC0);
fmt.Border.BottomLineStyle = XlBorderLineStyle.Medium;
fmt.Border.TopLineStyle = XlBorderLineStyle.None;
```

### Alignment

```csharp
// Horizontal and vertical in one call
fmt.Alignment = XlCellAlignment.FromHV(XlHorizontalAlignment.Right, XlVerticalAlignment.Center);

// With text wrap
fmt.Alignment = new XlCellAlignment();
fmt.Alignment.HorizontalAlignment = XlHorizontalAlignment.Left;
fmt.Alignment.VerticalAlignment = XlVerticalAlignment.Top;
fmt.Alignment.WrapText = true;
fmt.Alignment.Indent = 1;
```

### Number Formats

```csharp
// Common format strings
fmt.NumberFormat = "#,##0";          // integer with thousands separator
fmt.NumberFormat = "#,##0.00";       // 2 decimal places
fmt.NumberFormat = "$#,##0.00";      // US currency
fmt.NumberFormat = "0.00%";          // percentage
fmt.NumberFormat = "yyyy-mm-dd";     // ISO date
fmt.NumberFormat = "dd/mm/yyyy";     // European date
fmt.NumberFormat = "h:mm AM/PM";     // time with AM/PM
fmt.NumberFormat = @"_([$$-409]* #,##0.00_);_([$$-409]* \(#,##0.00\);_([$$-409]* ""-""??_);_(@_)"; // accounting
```

### Predefined Style-Like Formatting

```csharp
// Use built-in style presets (matches Excel's named styles)
cell.ApplyFormatting(XlCellFormatting.Good);
cell.ApplyFormatting(XlCellFormatting.Bad);
cell.ApplyFormatting(XlCellFormatting.Neutral);
cell.ApplyFormatting(XlCellFormatting.Heading1);
cell.ApplyFormatting(XlCellFormatting.Total);
cell.ApplyFormatting(XlCellFormatting.Calculation);
```

### Themed Formatting

```csharp
// Apply theme-based formatting (background + auto font color)
cell.ApplyFormatting(XlCellFormatting.Themed(XlThemeColor.Accent1, 0.0));  // dark accent
cell.ApplyFormatting(XlCellFormatting.Themed(XlThemeColor.Accent2, 0.6));  // light accent (60% tint)
```

### Rich Text (Mixed Fonts in One Cell)

```csharp
XlRichTextString richText = new XlRichTextString();
richText.Runs.Add(new XlRichTextRun("Bold part: ",
    XlFont.CustomFont("Calibri", 12.0, XlColor.FromArgb(0x00, 0x00, 0x00)) ));
richText.Runs.Add(new XlRichTextRun("colored text",
    XlFont.CustomFont("Calibri", 12.0, XlColor.FromArgb(0xC0, 0x00, 0x00))));

using (IXlCell cell = row.CreateCell())
{
    cell.SetRichText(richText);
}
```

> `XlFont.CustomFont(name, size, color)` creates a fully specified font. Set `SchemeStyle = XlFontSchemeStyles.None` if setting a specific font name through the constructor-less path.

### Merging Cells

```csharp
// Merge A1:C1 — set the value in the first cell of the merged range
sheet.MergedCells.Add(XlCellRange.FromLTRB(0, 0, 2, 0)); // left=0, top=0, right=2, bottom=0

using (IXlRow row = sheet.CreateRow())
{
    using (IXlCell cell = row.CreateCell()) // A1 — holds the value
    {
        cell.Value = "Merged Header";
        cell.ApplyFormatting(XlCellAlignment.FromHV(XlHorizontalAlignment.Center, XlVerticalAlignment.Center));
    }
    row.SkipCells(2); // B1 and C1 are part of the merge — skip them
}
```

### Hyperlinks

```csharp
// URL hyperlink on a cell
XlHyperlink link = new XlHyperlink();
link.Range = XlCellRange.FromLTRB(0, row.RowIndex, 0, row.RowIndex);
link.TargetUri = "https://www.devexpress.com";
link.Tooltip = "Visit DevExpress";
sheet.Hyperlinks.Add(link);

// The cell itself still needs a value
using (IXlCell cell = row.CreateCell())
{
    cell.Value = "DevExpress Website";
}
```

## Conditional Formatting

Conditional formatting applies automatic formatting to cells whose values satisfy a rule. Rules are attached to the sheet after rows are written.

```csharp
// Highlight cells greater than 10000 with a green fill
XlConditionalFormatting cf = new XlConditionalFormatting();
cf.Ranges.Add(XlCellRange.FromLTRB(2, 1, 2, dataRowCount)); // column C, rows 2..N

XlCondFmtRuleCellIs rule = new XlCondFmtRuleCellIs();
rule.Operator = XlCondFmtOperator.GreaterThan;
rule.Value = "10000";
rule.Formatting = XlCellFormatting.Good; // green
cf.Rules.Add(rule);

sheet.ConditionalFormattings.Add(cf);
```

### Conditional Formatting Rule Types

| Rule class | Use case |
|-----------|---------|
| `XlCondFmtRuleCellIs` | Values meeting a relational operator (greater than, between, etc.) |
| `XlCondFmtRuleAboveAverage` | Values above or below the range average |
| `XlCondFmtRuleBlanks` | Blank or non-blank cells |
| `XlCondFmtRuleDuplicates` | Duplicate values |
| `XlCondFmtRuleUnique` | Unique values |
| `XlCondFmtRuleExpression` | Formula-based rule |
| `XlCondFmtRuleTop10` | Top/bottom N values or percentages |
| `XlCondFmtRuleSpecificText` | Cells containing, beginning with, or ending with text |
| `XlCondFmtRuleTimePeriod` | Date values within a time period (today, last week, etc.) |
| `XlCondFmtRuleDataBar` | Data bar visualization |
| `XlCondFmtRuleIconSet` | Icon set visualization |
| `XlCondFmtRuleColorScale` | Two- or three-color gradient scale |

### Data Bar Example

```csharp
XlConditionalFormatting cf = new XlConditionalFormatting();
cf.Ranges.Add(XlCellRange.FromLTRB(1, 1, 1, 10)); // column B

XlCondFmtRuleDataBar dataBarRule = new XlCondFmtRuleDataBar();
dataBarRule.Color = XlColor.FromTheme(XlThemeColor.Accent1, 0.0);
cf.Rules.Add(dataBarRule);

sheet.ConditionalFormattings.Add(cf);
```

## Configuration Options

| Property | Default | Description |
|----------|---------|-------------|
| `XlFont.Bold` | `false` | Bold text |
| `XlFont.Italic` | `false` | Italic text |
| `XlFont.Size` | `11` | Font size in points |
| `XlFont.SchemeStyle` | `XlFontSchemeStyles.Minor` | Set to `None` when specifying a custom font name |
| `XlCellAlignment.WrapText` | `false` | Wrap long text within the cell |
| `XlCellAlignment.Indent` | `0` | Number of indent steps |
| `XlBorderLineStyle` | `None` | Border line style (Thin, Medium, Thick, Dashed, etc.) |

## Troubleshooting

- **Custom font not applied**: If you set `Font.Name` to a specific font, also set `Font.SchemeStyle = XlFontSchemeStyles.None`. Otherwise the theme font overrides the name.
- **Date displayed as a number**: Apply a date number format string (e.g., `"yyyy-mm-dd"`) to the cell or column.
- **Merged cells appear empty**: Set the value on the top-left cell of the merge range only. Skip the remaining cells with `row.SkipCells()`.
- **Conditional formatting not visible**: The `XlConditionalFormatting` object must be added to `sheet.ConditionalFormattings` after the rules and ranges are configured. Call `sheet.ConditionalFormattings.Add(cf)`.
- **Hyperlink not clickable**: Ensure the `Range` property of `XlHyperlink` matches the actual cell position and the hyperlink is added to `sheet.Hyperlinks`.

# Tables — DevExpress Word Processing Document API

The Word Processing Document API provides comprehensive support for creating, modifying, styling, and manipulating tables in Word documents. Tables support complex formatting including cell merging, fixed-width columns, vertical alignment, borders, shading, and repeat header rows.

## When to Use This Reference

Use this when you need to:
- Create a table in a document at a specific position or at the end
- Add, remove, or iterate rows and columns
- Set cell content, borders, background color, or padding
- Merge cells horizontally or vertically
- Apply table styles (from a loaded document with styles)
- Set table width behavior (fixed width, AutoFit contents, AutoFit window)
- Configure repeat header rows for multi-page tables
- Control text wrapping around tables

## Key Classes and Types

| Class/Interface | Purpose |
|----------------|---------|
| `Table` | Represents a table; access via `Document.Tables` or `table[row, col]` indexer |
| `TableRow` | A row in a table; access via `Table.Rows` |
| `TableCell` | A cell in a row; access via `TableRow.Cells[i]` or `table[row, col]` |
| `TableBorderLineStyle` | Border style enum (Single, Double, Dotted, Dashed, None, etc.) |
| `TableCellVerticalAlignment` | Top, Center, Bottom |
| `WidthType` | Fixed, Auto, FiftiethsOfPercent |
| `TableLayoutType` | Fixed, Autofit |
| `Units` | Static helper: `InchesToDocumentsF()`, `CentimetersToDocumentsF()`, etc. |

## Create a Table

```csharp
using DevExpress.XtraRichEdit;
using DevExpress.XtraRichEdit.API.Native;
using DevExpress.Office.Utils;

using (var server = new RichEditDocumentServer())
{
    Document doc = server.Document;
    doc.BeginUpdate();

    // Create a 3-row x 3-column table at the start of the document
    // Parameters: position, rowCount, columnCount
    Table table = doc.Tables.Create(doc.Range.Start, 3, 3);

    // Set fixed column widths
    table.BeginUpdate();
    table.TableLayout = TableLayoutType.Fixed;
    table.PreferredWidthType = WidthType.Fixed;
    table.PreferredWidth = Units.InchesToDocumentsF(5f);

    foreach (TableRow row in table.Rows)
    {
        row.Cells[0].PreferredWidthType = WidthType.Fixed;
        row.Cells[0].PreferredWidth = Units.InchesToDocumentsF(2f);
        row.Cells[1].PreferredWidthType = WidthType.Fixed;
        row.Cells[1].PreferredWidth = Units.InchesToDocumentsF(1.5f);
        row.Cells[2].PreferredWidthType = WidthType.Fixed;
        row.Cells[2].PreferredWidth = Units.InchesToDocumentsF(1.5f);
    }
    table.EndUpdate();

    doc.EndUpdate();
    server.SaveDocument("table.docx", DocumentFormat.Docx);
}
```

## Set Cell Content

```csharp
// Use table[row, col] indexer or table.Rows[r].Cells[c]
for (int row = 0; row < table.Rows.Count; row++)
{
    for (int col = 0; col < table.Rows[row].Cells.Count; col++)
    {
        TableCell cell = table[row, col]; // Equivalent to table.Rows[row].Cells[col]
        doc.InsertText(cell.Range.Start, $"Row {row + 1}, Col {col + 1}");
    }
}
```

## Cell Formatting — Borders, Background, Alignment

Properties are set directly on the `TableCell` or `Table` object (no BeginUpdateCells pattern):

```csharp
using System.Drawing;

table.BeginUpdate();

// --- Cell borders ---
foreach (TableRow row in table.Rows)
{
    foreach (TableCell cell in row.Cells)
    {
        cell.Borders.Left.LineStyle = TableBorderLineStyle.Single;
        cell.Borders.Left.LineThickness = 1;
        cell.Borders.Right.LineStyle = TableBorderLineStyle.Single;
        cell.Borders.Right.LineThickness = 1;
        cell.Borders.Top.LineStyle = TableBorderLineStyle.Single;
        cell.Borders.Top.LineThickness = 1;
        cell.Borders.Bottom.LineStyle = TableBorderLineStyle.Single;
        cell.Borders.Bottom.LineThickness = 1;
    }
}

// --- Header row: dark blue background ---
foreach (TableCell cell in table.Rows[0].Cells)
{
    cell.BackgroundColor = Color.FromArgb(31, 73, 125);
    cell.VerticalAlignment = TableCellVerticalAlignment.Center;

    // Make header text bold + white
    CharacterProperties cp = doc.BeginUpdateCharacters(cell.Range);
    cp.Bold = true;
    cp.ForeColor = Color.White;
    doc.EndUpdateCharacters(cp);

    ParagraphProperties pp = doc.BeginUpdateParagraphs(cell.Range);
    pp.Alignment = ParagraphAlignment.Center;
    doc.EndUpdateParagraphs(pp);
}

// --- Alternate row shading ---
for (int r = 1; r < table.Rows.Count; r++)
{
    if (r % 2 == 0)
    {
        foreach (TableCell cell in table.Rows[r].Cells)
        {
            cell.BackgroundColor = Color.FromArgb(217, 225, 242); // Light blue
        }
    }
}

table.EndUpdate();
```

## Table-Level Borders

```csharp
table.BeginUpdate();

// Set inside borders
table.Borders.InsideHorizontalBorder.LineStyle = TableBorderLineStyle.Single;
table.Borders.InsideHorizontalBorder.LineThickness = 1;
table.Borders.InsideVerticalBorder.LineStyle = TableBorderLineStyle.Single;
table.Borders.InsideVerticalBorder.LineThickness = 1;

// Set outside border (heavier)
table.Borders.Left.LineStyle = TableBorderLineStyle.Double;
table.Borders.Right.LineStyle = TableBorderLineStyle.Double;
table.Borders.Top.LineStyle = TableBorderLineStyle.Double;
table.Borders.Bottom.LineStyle = TableBorderLineStyle.Double;

table.EndUpdate();
```

## Cell Vertical Alignment

```csharp
table.BeginUpdate();
table.Rows[1].Cells[0].VerticalAlignment = TableCellVerticalAlignment.Center;
table.Rows[1].Cells[1].VerticalAlignment = TableCellVerticalAlignment.Bottom;
table.EndUpdate();
```

## Row Height

```csharp
table.BeginUpdate();
table.Rows[0].HeightType = HeightType.Exact;
table.Rows[0].Height = Units.InchesToDocumentsF(0.4f);
table.EndUpdate();
```

## Table Alignment

```csharp
table.BeginUpdate();
table.TableAlignment = TableRowAlignment.Center;
table.EndUpdate();
```

## Merge Cells

```csharp
table.BeginUpdate();

// Horizontal merge: merge cells in row 0 from col 0 to col 2
table.MergeCells(table[0, 0], table[0, 2]);

// Vertical merge: merge rows 1 and 2 in column 0
table.MergeCells(table[1, 0], table[2, 0]);

table.EndUpdate();
```

> **Note**: After merging, cell indices change. Use `table.Rows[r].Cells.Count` to check the current count before accessing cells.

## Repeat Header Row

```csharp
table.BeginUpdate();
table.Rows[0].RepeatAsHeaderRow = true;
table.EndUpdate();
```

## Add or Remove Rows

```csharp
table.BeginUpdate();

// Insert a row after index 2
table.Rows.InsertAfter(2);

// Insert a row before index 0
table.Rows.InsertBefore(0);

// Delete row at index 3
table.Rows.Delete(3);

table.EndUpdate();
```

## Apply a Table Style

If the document was loaded from a `.docx` file that contains table styles:

```csharp
table.Style = doc.TableStyles["Table Grid"];
```

> **Note**: A blank `RichEditDocumentServer` document has no predefined table styles. Load a `.docx` template with styles first, or use direct cell formatting.

## Iterate Table Cells

```csharp
foreach (TableRow row in table.Rows)
{
    foreach (TableCell cell in row.Cells)
    {
        string text = doc.GetText(cell.Range);
        Console.WriteLine($"  Cell: '{text}'");
    }
}
```

## Access Tables in a Loaded Document

```csharp
using (var server = new RichEditDocumentServer())
{
    server.LoadDocument("report.docx", DocumentFormat.Docx);
    Document doc = server.Document;

    if (doc.Tables.Count > 0)
    {
        Table firstTable = doc.Tables[0];
        Console.WriteLine($"Table: {firstTable.Rows.Count} rows");

        // Read the first cell's text
        string cellText = doc.GetText(firstTable[0, 0].Range);
        Console.WriteLine($"First cell: '{cellText}'");
    }
}
```

## Configuration Options

| Property | Location | Description |
|----------|----------|-------------|
| `TableLayout` | `Table` | `Fixed` or `Autofit` |
| `PreferredWidthType` | `Table` or `TableCell` | `Fixed`, `Auto`, `FiftiethsOfPercent` |
| `PreferredWidth` | `Table` or `TableCell` | Width in document units or percent |
| `TableAlignment` | `Table` | `Left`, `Center`, `Right` |
| `TableCellSpacing` | `Table` | Space between cells |
| `BackgroundColor` | `TableCell` | Cell background color |
| `VerticalAlignment` | `TableCell` | `Top`, `Center`, `Bottom` |
| `HeightType` | `TableRow` | `Auto`, `Exact`, `AtLeast` |
| `Height` | `TableRow` | Row height in document units |
| `RepeatAsHeaderRow` | `TableRow` | Repeat row as header on each page |

## Troubleshooting

- **Column widths not respected**: Set `table.TableLayout = TableLayoutType.Fixed` and `table.PreferredWidthType = WidthType.Fixed`, then set `cell.PreferredWidthType = WidthType.Fixed` on each cell.
- **Cell background not saved**: Ensure `table.BeginUpdate()` / `table.EndUpdate()` surrounds your changes, and the document is saved with `DocumentFormat.Docx` (not plain text or RTF, which have limited color support).
- **Merged cell index out of range**: After merging cells, re-access them by iterating `row.Cells` rather than using a fixed index.
- **Table style not found**: Load a `.docx` template containing the target style, or use direct border/background formatting.
- **`Units` class not found**: Add `using DevExpress.Office.Utils;`.

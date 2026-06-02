# Pivot Tables — DevExpress Spreadsheet Document API

Create and configure pivot tables to summarize, analyze, and present worksheet data.

## When to Use This Reference

Use this when you need to:
- Create a pivot table from a worksheet data range
- Add row fields, column fields, data fields, and filter (page) fields
- Apply a pivot table style
- Refresh a pivot table after the source data changes
- Sort, filter, or group pivot table items

## Key Classes and Types

| Class/Interface | Purpose |
|----------------|---------|
| `PivotTableCollection` | All pivot tables on a worksheet (`Worksheet.PivotTables`) |
| `PivotTable` | A pivot table report |
| `PivotField` | A field in the pivot table corresponding to a source data column |
| `PivotFieldCollection` | Collection of all pivot fields (`PivotTable.Fields`) |
| `PivotRowColumnField` | A field placed in the row or column area |
| `PivotDataField` | A data (value) field with its summary function |
| `PivotPageField` | A report filter (page) field |
| `PivotTableStyleCollection` | Built-in and custom pivot table styles |

## Pivot Table Structure

A pivot table is organized into four areas:

| Area | Description |
|------|-------------|
| **Row Area** | Fields whose values become row labels |
| **Column Area** | Fields whose values become column headers |
| **Data Area** | Fields summarized with functions (Sum, Count, Average, Min, Max, etc.) |
| **Filter Area** | Fields used to filter the entire report |

## Create a Pivot Table

```csharp
using DevExpress.Spreadsheet;

// Assume worksheet "Data" has: Product | Region | Quarter | Sales
Worksheet dataSheet = workbook.Worksheets["Data"];
Worksheet reportSheet = workbook.Worksheets.Add("Pivot Report");

// Define the source data range (including headers)
CellRange sourceRange = dataSheet.GetUsedRange();

// Add a pivot table to the report sheet, anchored at cell A3
PivotTable pt = reportSheet.PivotTables.Add(sourceRange, reportSheet["A3"]);
pt.Name = "SalesSummary";
```

## Add Fields to Areas

```csharp
// Fields are referenced by their 0-based index in the source data columns:
// Column 0 = Product, Column 1 = Region, Column 2 = Quarter, Column 3 = Sales

// Add "Product" as a row field
pt.RowFields.Add(pt.Fields[0]); // Product

// Add "Quarter" as a column field
pt.ColumnFields.Add(pt.Fields[2]); // Quarter

// Add "Sales" as a data field (default aggregation: Sum)
PivotDataField dataField = pt.DataFields.Add(pt.Fields[3]); // Sales
dataField.Name = "Total Sales";
dataField.SummarizeValuesBy = ConsolidationFunction.Sum;
dataField.NumberFormat = "$#,##0";

// Add "Region" as a report filter (page) field
pt.PageFields.Add(pt.Fields[1]); // Region
```

## Supported Summary Functions

| `ConsolidationFunction` Value | Description |
|-------------------------------|-------------|
| `Sum` | Sum of values (default for numeric data) |
| `Count` | Count of non-empty cells (default for text) |
| `Average` | Arithmetic mean |
| `Min` | Minimum value |
| `Max` | Maximum value |
| `Product` | Product of all values |
| `CountNums` | Count of numeric values |
| `StdDev` | Standard deviation (sample) |
| `StdDevP` | Standard deviation (population) |
| `Var` | Variance (sample) |
| `VarP` | Variance (population) |

## Apply a Pivot Table Style

```csharp
// Apply a built-in style by name
pt.StyleName = "PivotStyleMedium9";

// Show/hide banded rows and columns
pt.ShowRowStripes = true;
pt.ShowColumnStripes = false;
pt.ShowRowHeaders = true;
pt.ShowColumnHeaders = true;
```

Common built-in style names: `"PivotStyleLight1"` through `"PivotStyleLight28"`, `"PivotStyleMedium1"` through `"PivotStyleMedium28"`, `"PivotStyleDark1"` through `"PivotStyleDark28"`.

## Sort and Filter Pivot Fields

```csharp
// Sort a row field's items alphabetically
PivotField productField = pt.Fields[0];
productField.SortType = PivotFieldSortType.Ascending;

// Hide specific items in a field (filter them out)
foreach (PivotItem item in productField.Items)
{
    if (item.Value == "Widget C")
        item.Hidden = true; // Exclude "Widget C" from the report
}

// Set the selected item for a page (filter) field
PivotField regionField = pt.Fields[1];
PivotPageField pageField = pt.PageFields[0];
// Select "North" region
foreach (PivotItem item in regionField.Items)
{
    item.Hidden = (item.Value != "North");
}
```

## Group Pivot Field Items

```csharp
// Group a date field by month and year
PivotField dateField = pt.Fields[2]; // e.g., "Date" column
dateField.GroupBy(PivotFieldGroupByType.Months);

// Group numeric field into ranges
PivotField salesField = pt.Fields[3];
salesField.GroupRange(0, 50000, 10000); // From 0 to 50000, step 10000
```

## Refresh a Pivot Table

After the source data changes, call `Refresh()` to update the pivot table:

```csharp
// Update source data
dataSheet["E2"].Value = 99999;

// Refresh to reflect new data
pt.Refresh();
```

## Complete Example

```csharp
using DevExpress.Spreadsheet;

using (Workbook workbook = new Workbook())
{
    // Create source data
    Worksheet data = workbook.Worksheets[0];
    data.Name = "Data";

    data["A1"].Value = "Product"; data["B1"].Value = "Region";
    data["C1"].Value = "Quarter"; data["D1"].Value = "Sales";

    data["A2"].Value = "Widget A"; data["B2"].Value = "North";
    data["C2"].Value = "Q1";       data["D2"].Value = 15000;
    data["A3"].Value = "Widget A"; data["B3"].Value = "South";
    data["C3"].Value = "Q1";       data["D3"].Value = 12000;
    data["A4"].Value = "Widget B"; data["B4"].Value = "North";
    data["C4"].Value = "Q2";       data["D4"].Value = 22000;
    data["A5"].Value = "Widget B"; data["B5"].Value = "South";
    data["C5"].Value = "Q2";       data["D5"].Value = 18000;

    // Create pivot table sheet
    Worksheet report = workbook.Worksheets.Add("Pivot");

    CellRange source = data.GetUsedRange();
    PivotTable pt = report.PivotTables.Add(source, report["A3"]);
    pt.Name = "SalesReport";

    // Configure fields
    pt.RowFields.Add(pt.Fields[0]);    // Product → rows
    pt.ColumnFields.Add(pt.Fields[2]); // Quarter → columns
    pt.PageFields.Add(pt.Fields[1]);   // Region → filter

    PivotDataField df = pt.DataFields.Add(pt.Fields[3]); // Sales → values
    df.Name = "Total Sales";
    df.SummarizeValuesBy = ConsolidationFunction.Sum;
    df.NumberFormat = "$#,##0";

    // Style
    pt.StyleName = "PivotStyleMedium9";
    pt.ShowRowStripes = true;

    workbook.SaveDocument("pivot.xlsx");
}
```

## Troubleshooting

- **Pivot table is empty after creation**: Fields must be added to at least the Row and Data areas. Verify that `pt.RowFields.Add(...)` and `pt.DataFields.Add(...)` are called.
- **`Refresh()` does not update data**: Ensure the source range still covers the new data rows. The source range is fixed at creation time; to expand it, re-create the pivot table with the new range.
- **Field index is wrong**: Field index corresponds to the 0-based column index in the source range. Column A = 0, Column B = 1, etc.
- **Style name not found**: Use exact style name strings (case-sensitive). Check the DevExpress documentation for the full list of built-in pivot style names.

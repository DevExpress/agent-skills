# Data Import & Export — DevExpress Spreadsheet Document API

Import data from arrays, collections, and DataTables into worksheets; export worksheet data back to DataTables; and handle format-specific CSV/TXT settings.

## When to Use This Reference

Use this when you need to:
- Import data from a `DataTable` into a worksheet
- Import from a `List<T>` or `IEnumerable` of custom objects
- Import from a one-dimensional or two-dimensional array
- Export a worksheet range to a `DataTable` (with optional custom converters)
- Configure CSV encoding, delimiters, or other format-specific import options
- Handle `BeforeImport`/`BeforeExport` events to set format options programmatically

## Key Classes and Types

| Class/Interface | Purpose |
|----------------|---------|
| `WorksheetExtensions.Import` | Extension methods for importing data into a worksheet |
| `DataSourceImportOptions` | Options for importing from typed collections (property mapping, etc.) |
| `WorksheetExtensions.CreateDataTable` | Create a typed `DataTable` matching a worksheet range |
| `WorksheetExtensions.CreateDataTableExporter` | Create an exporter for converting worksheet data to `DataTable` |
| `DataTableExporter` | Exports cells to a `DataTable` with conversion hooks |
| `DataTableExportOptions` | Export options including custom converters |
| `ICellValueToColumnTypeConverter` | Interface for custom cell → column type converters |
| `IDataValueConverter` | Interface for custom value converters during import |
| `CsvDocumentImporterOptions` | CSV-specific import settings |
| `CsvDocumentExporterOptions` | CSV-specific export settings |

## Import Data into a Worksheet

### From a DataTable

```csharp
using DevExpress.Spreadsheet;
using System.Data;

using (Workbook workbook = new Workbook())
{
    Worksheet sheet = workbook.Worksheets[0];

    // DataTable with header row
    DataTable table = GetDataTable(); // your data

    // Import: addHeader=true inserts column names as the first row
    sheet.Import(table, addHeader: true, firstRowIndex: 0, firstColumnIndex: 0);

    // Auto-fit all populated columns
    sheet.Columns.AutoFit(0, sheet.GetUsedRange().ColumnCount - 1);

    workbook.SaveDocument("output.xlsx");
}
```

### From a List of Custom Objects

```csharp
using DevExpress.Spreadsheet;
using System.Collections.Generic;

public class Product
{
    public string Name { get; set; }
    public string Category { get; set; }
    public decimal Price { get; set; }
}

var products = new List<Product>
{
    new Product { Name = "Widget A", Category = "Hardware", Price = 29.99m },
    new Product { Name = "Widget B", Category = "Software", Price = 99.00m },
};

using (Workbook workbook = new Workbook())
{
    Worksheet sheet = workbook.Worksheets[0];

    // Import all properties; row 0, column 0
    sheet.Import(products, 0, 0);

    // Import only specific properties
    var options = new DataSourceImportOptions
    {
        PropertyNames = new[] { "Name", "Price" }
    };
    sheet.Import(products, 5, 0, options);

    sheet.Columns.AutoFit(0, 2);
    workbook.SaveDocument("products.xlsx");
}
```

### From a One-Dimensional Array

```csharp
string[] headers = { "Product", "Q1", "Q2", "Q3", "Q4" };

// isVertical: false = horizontal, true = vertical
sheet.Import(headers, isVertical: false, firstRowIndex: 0, firstColumnIndex: 0);
```

### From a Two-Dimensional Array

```csharp
object[,] data =
{
    { "Widget A", 1500, 1800, 2100 },
    { "Widget B", 2200, 1900, 2500 },
    { "Widget C", 800,  950,  1100 },
};

sheet.Import(data, firstRowIndex: 1, firstColumnIndex: 0);
```

### With a Custom Converter

Implement `IDataValueConverter` to transform values during import:

```csharp
using DevExpress.Spreadsheet;
using System.Collections.Generic;

public class MyConverter : IDataValueConverter
{
    public bool TryConvert(object value, int columnIndex, out CellValue result)
    {
        if (value is bool b)
        {
            result = b ? "Yes" : "No"; // Convert bool to text
            return true;
        }
        result = CellValue.TryCreateFromObject(value);
        return true;
    }
}

// Use the converter
var importOptions = new DataSourceImportOptions
{
    Converter = new MyConverter()
};
sheet.Import(myList, 0, 0, importOptions);
```

## Export Worksheet Data to DataTable

### Basic Export

```csharp
using DevExpress.Spreadsheet;
using DevExpress.Spreadsheet.Export;
using System.Data;

using (Workbook workbook = new Workbook())
{
    workbook.LoadDocument("data.xlsx");
    Worksheet sheet = workbook.Worksheets[0];

    // Define the range to export
    CellRange range = sheet.GetUsedRange();

    // Create a DataTable with column types inferred from cell content
    DataTable dataTable = sheet.CreateDataTable(range, hasHeaders: true);

    // Create and run the exporter
    DataTableExporter exporter = sheet.CreateDataTableExporter(range, dataTable, hasHeaders: true);
    exporter.Export();

    // Use the DataTable
    foreach (DataRow row in dataTable.Rows)
    {
        Console.WriteLine($"{row[0]}: {row[1]}");
    }
}
```

### Export with Custom Converter

```csharp
using DevExpress.Spreadsheet;
using DevExpress.Spreadsheet.Export;
using System.Data;

public class DateToStringConverter : ICellValueToColumnTypeConverter
{
    public bool Convert(Cell cell, CellValue value, Type targetType, out object result)
    {
        if (value.IsDateTime)
        {
            result = value.DateTimeValue.ToString("MMMM yyyy");
            return true;
        }
        result = null;
        return false;
    }
}

CellRange range = sheet.Tables[0].Range;
DataTable dt = sheet.CreateDataTable(range, true);

// Override type for a specific column
dt.Columns["Date"].DataType = typeof(string);

DataTableExporter exporter = sheet.CreateDataTableExporter(range, dt, true);
exporter.Options.CustomConverters.Add("Date", new DateToStringConverter());
exporter.Options.ConvertEmptyCells = true;

// Handle conversion errors
exporter.CellValueConversionError += (sender, e) =>
{
    Console.WriteLine($"Error in cell {e.Cell.GetReferenceA1()}");
    e.DataTableValue = null;
    e.Action = DataTableExporterAction.Continue;
};

exporter.Export();
```

## Format-Specific Import/Export Options

Handle `Workbook.BeforeImport` to set options per format:

```csharp
workbook.BeforeImport += (sender, e) =>
{
    if (e.DocumentFormat == DocumentFormat.Csv)
    {
        var options = (CsvDocumentImporterOptions)e.Options;
        options.Delimiter = ';';             // Use semicolon as separator
        options.Encoding = Encoding.UTF8;    // Specify encoding
        options.TrimBlanks = true;           // Trim whitespace
        options.AutoDetectEncoding = false;
    }
    if (e.DocumentFormat == DocumentFormat.Text)
    {
        var options = (TxtDocumentImporterOptions)e.Options;
        options.AutoDetectEncoding = true;
    }
};

workbook.LoadDocument("data.csv", DocumentFormat.Csv);
```

Handle `Workbook.BeforeExport` for export settings:

```csharp
workbook.BeforeExport += (sender, e) =>
{
    if (e.DocumentFormat == DocumentFormat.Csv)
    {
        var options = (CsvDocumentExporterOptions)e.Options;
        options.Delimiter = ',';
        options.WorksheetName = "Sheet1"; // Export only this sheet
        options.SkipHiddenColumns = true;
        options.FormulaExportMode = DevExpress.XtraSpreadsheet.Export.FormulaExportMode.CalculatedValue;
    }
};

workbook.SaveDocument("output.csv", DocumentFormat.Csv);
```

## Supported Import Sources

| Source | Method Signature |
|--------|-----------------|
| `DataTable` | `sheet.Import(DataTable, bool addHeader, int firstRow, int firstCol)` |
| `IList<T>` / `List<T>` | `sheet.Import(IList<T>, int firstRow, int firstCol)` |
| `IList<T>` with options | `sheet.Import(IList<T>, int firstRow, int firstCol, DataSourceImportOptions)` |
| 1-D array | `sheet.Import(Array, bool isVertical, int firstRow, int firstCol)` |
| 2-D array | `sheet.Import(object[,], int firstRow, int firstCol)` |

## Troubleshooting

- **Headers not imported**: Pass `addHeader: true` to `sheet.Import(dataTable, addHeader: true, ...)`.
- **Wrong column data types in DataTable**: `CreateDataTable` infers types from the first data row. Override individual column types after creation: `dt.Columns["Amount"].DataType = typeof(decimal)`.
- **CSV imports all data into one column**: The delimiter does not match. Set the correct `Delimiter` in the `BeforeImport` event handler.
- **`WorksheetExtensions` not found**: Add a reference to `DevExpress.Docs.vxx.x.dll` (or ensure `DevExpress.Document.Processor` NuGet package is installed). These extension methods live in `DevExpress.Docs.dll`.
- **Import overwrites existing data**: `Import` starts at the `firstRowIndex`/`firstColumnIndex` you provide. Use `sheet.GetUsedRange().BottomRowIndex + 1` to append below existing data.

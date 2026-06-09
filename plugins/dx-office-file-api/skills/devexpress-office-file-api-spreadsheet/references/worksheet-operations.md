# Worksheet Operations — DevExpress Spreadsheet Document API

Add, remove, rename, copy, move, and navigate worksheets in a workbook.

## When to Use This Reference

Use this when you need to:
- Add a new worksheet to a workbook (by name or at a specific position)
- Remove a worksheet by name or index
- Rename a worksheet
- Copy a worksheet within the same workbook
- Move a worksheet to a different position
- Set the active (selected) worksheet
- Show or hide worksheets
- Iterate over all worksheets

## Key Classes and Types

| Class/Member | Purpose |
|-------------|---------|
| `Workbook.Worksheets` | `WorksheetCollection` — access, add, remove worksheets |
| `WorksheetCollection.Add(name)` | Append a new worksheet (optional name) |
| `WorksheetCollection.Insert(index, name)` | Insert a worksheet at a specific position |
| `WorksheetCollection.Remove(sheet)` | Remove a worksheet by reference |
| `WorksheetCollection.RemoveAt(index)` | Remove a worksheet by index |
| `WorksheetCollection.ActiveWorksheet` | Get or set the active (visible) worksheet |
| `Worksheet.Name` | Get or set the worksheet tab name |
| `Worksheet.CopyFrom(source)` | Copy all content and formatting from another worksheet |
| `Worksheet.Move(index)` | Move the worksheet to a zero-based position in the collection |
| `Worksheet.Index` | Zero-based position in the workbook |
| `Worksheet.Visible` | Show (`true`) or hide (`false`) a worksheet |
| `Worksheet.VisibilityType` | Fine-grained visibility: `Visible`, `Hidden`, `VeryHidden` |

## Add a Worksheet

```csharp
using DevExpress.Spreadsheet;

using (Workbook workbook = new Workbook())
{
    // Append a worksheet with an auto-generated name ("Sheet2", "Sheet3", ...)
    workbook.Worksheets.Add();

    // Append a worksheet with a specific name
    workbook.Worksheets.Add("Summary");

    // Insert a worksheet at position 1 (zero-based)
    workbook.Worksheets.Insert(1, "Details");

    workbook.SaveDocument("output.xlsx");
}
```

## Remove a Worksheet

```csharp
// Remove by name
workbook.Worksheets.Remove(workbook.Worksheets["Sheet2"]);

// Remove by zero-based index
workbook.Worksheets.RemoveAt(0);
```

> A workbook must always contain at least one visible worksheet. Attempting to remove the last visible worksheet throws an exception.

## Rename a Worksheet

```csharp
// Rename the second worksheet
workbook.Worksheets[1].Name = "Q1 Sales";

// Rename using a variable
Worksheet sheet = workbook.Worksheets["Sheet1"];
sheet.Name = "January";
```

Worksheet names must be unique within the workbook and may not contain the characters `/ \ ? * [ ]`. The maximum length is 31 characters.

## Copy a Worksheet

```csharp
// Add a new empty worksheet, then copy content and formatting from the source
workbook.Worksheets.Add("Sheet1_Copy");
workbook.Worksheets["Sheet1_Copy"].CopyFrom(workbook.Worksheets["Sheet1"]);
```

`CopyFrom` copies all cell values, formulas, formatting, and worksheet settings from the source sheet.

## Move a Worksheet

```csharp
// Move the first worksheet to the last position
workbook.Worksheets[0].Move(workbook.Worksheets.Count - 1);

// Move "Summary" to the front
workbook.Worksheets["Summary"].Move(0);
```

The `Move` method takes a zero-based destination index.

## Set the Active Worksheet

```csharp
// Activate by name
workbook.Worksheets.ActiveWorksheet = workbook.Worksheets["Sheet2"];

// Activate by index
workbook.Worksheets.ActiveWorksheet = workbook.Worksheets[2];
```

The active worksheet is the tab that is selected when the file opens in Excel.

## Access Worksheets

```csharp
// By zero-based index
Worksheet first = workbook.Worksheets[0];

// By name
Worksheet sheet = workbook.Worksheets["Sales"];

// Iterate over all worksheets
foreach (Worksheet ws in workbook.Worksheets)
    Console.WriteLine(ws.Name);

// Check whether a named worksheet exists
bool exists = workbook.Worksheets.Contains("Summary");
```

## Show and Hide Worksheets

```csharp
// Hide a worksheet (user can unhide it via Excel UI)
workbook.Worksheets["Sheet3"].Visible = false;

// Hide a worksheet so it cannot be unhidden via Excel UI
workbook.Worksheets["Sheet2"].VisibilityType = WorksheetVisibilityType.VeryHidden;

// Show a hidden worksheet
workbook.Worksheets["Sheet2"].Visible = true;
```

## Troubleshooting

- **`ArgumentException` on remove**: Ensure the workbook has more than one visible worksheet before removing. At least one must remain visible.
- **Name conflict on add**: Worksheet names must be unique. Check with `workbook.Worksheets.Contains(name)` before adding.
- **`CopyFrom` does not copy charts**: `CopyFrom` copies cell data and formatting. Charts and some graphic objects may not transfer — add them after copying if needed.
- **`VeryHidden` worksheet not accessible in Excel**: That is intentional. Use `sheet.Visible = true` programmatically to restore it.

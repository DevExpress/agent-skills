# Formulas & Calculations — DevExpress Spreadsheet Document API

Add, calculate, and manage formulas in worksheets — including built-in functions, named ranges, array formulas, and user-defined functions.

## When to Use This Reference

Use this when you need to:
- Add formulas to cells using `FormulaInvariant` (portable, English-language syntax)
- Force formula recalculation programmatically
- Work with named ranges in formulas
- Create array formulas or shared formulas
- Register custom/user-defined functions (UDFs)
- Parse and analyze formula expression trees using `FormulaEngine`
- Configure the calculation engine (mode, threading, iterative)

## Key Classes and Types

| Class/Interface | Purpose |
|----------------|---------|
| `Cell.FormulaInvariant` | Set/get a formula using invariant (English) syntax — always use this |
| `Cell.Formula` | Set/get a formula in the current culture's locale — avoid for portability |
| `Workbook.Calculate()` | Recalculate all cells marked for calculation |
| `Workbook.CalculateFull()` | Force-recalculate all cells regardless of state |
| `DefinedNameCollection` | Named ranges/constants at workbook or worksheet scope |
| `ICustomFunction` | Interface for implementing user-defined functions |
| `CustomFunctionCollection` | Collection for registering UDFs (`Workbook.CustomFunctions`) |
| `FormulaEngine` | Parse, evaluate, and analyze formula expressions |
| `ParsedExpression` | Result of parsing a formula; exposes an expression tree |

## Basic Formula Usage

### Always Use `FormulaInvariant`

```csharp
using DevExpress.Spreadsheet;

Worksheet sheet = workbook.Worksheets[0];

// Use FormulaInvariant for portable, culture-independent formulas
sheet["B4"].FormulaInvariant = "=SUM(B2:B3)";
sheet["C4"].FormulaInvariant = "=AVERAGE(C2:C3)";
sheet["D4"].FormulaInvariant = "=IF(B4>10000, \"High\", \"Low\")";
sheet["E4"].FormulaInvariant = "=VLOOKUP(A4, $A$2:$D$10, 2, FALSE)";
sheet["F4"].FormulaInvariant = "=IFERROR(B4/C4, 0)";

// Read back the formula (also invariant)
string formula = sheet["B4"].FormulaInvariant;
Console.WriteLine(formula); // "=SUM(B2:B3)"

// Read the calculated value
double result = (double)sheet["B4"].Value;
```

### Trigger Calculation

```csharp
// Calculate all cells marked for recalculation (respects manual mode)
workbook.Calculate();

// Force-recalculate all cells (any mode)
workbook.CalculateFull();

// Recalculate a specific worksheet
workbook.Worksheets[0].Calculate();

// Recalculate a specific range
sheet.Range["B2:B100"].Calculate();
```

### Calculation Mode

By default, `Workbook` uses manual calculation — formulas are only calculated on demand.
To use the mode stored in the document:

```csharp
workbook.Options.CalculationMode = WorkbookCalculationMode.UseDocumentSettings;
```

To force automatic recalculation on every cell change:

```csharp
workbook.DocumentSettings.Calculation.Mode = CalculationMode.Automatic;
```

## Supported Function Categories

The API supports 400+ Excel-compatible functions:

| Category | Example Functions |
|----------|------------------|
| Mathematical | `SUM`, `SUMIF`, `SUMPRODUCT`, `ROUND`, `ABS`, `MOD`, `POWER`, `SQRT`, `RAND` |
| Statistical | `AVERAGE`, `COUNT`, `COUNTA`, `MIN`, `MAX`, `MEDIAN`, `STDEV`, `PERCENTILE` |
| Date & Time | `TODAY`, `NOW`, `DATE`, `YEAR`, `MONTH`, `DAY`, `DATEDIF`, `WORKDAY`, `NETWORKDAYS` |
| Text | `CONCATENATE`, `LEFT`, `RIGHT`, `MID`, `LEN`, `TRIM`, `UPPER`, `LOWER`, `TEXT`, `SUBSTITUTE` |
| Logical | `IF`, `AND`, `OR`, `NOT`, `IFERROR`, `IFS`, `SWITCH` |
| Lookup | `VLOOKUP`, `HLOOKUP`, `INDEX`, `MATCH`, `OFFSET`, `CHOOSE`, `XLOOKUP` |
| Financial | `PMT`, `PV`, `FV`, `NPV`, `IRR`, `RATE`, `SLN`, `DB` |
| Engineering | `CONVERT`, `BIN2DEC`, `DEC2BIN`, `IMSUM`, `COMPLEX` |
| Information | `ISNUMBER`, `ISTEXT`, `ISBLANK`, `ISERROR`, `CELL`, `TYPE` |
| Database | `DSUM`, `DAVERAGE`, `DCOUNT`, `DMAX`, `DMIN` |
| Web | `WEBSERVICE`, `FILTERXML` |

## Named Ranges (Defined Names)

### Create Named Ranges

```csharp
// Workbook-scoped named range
workbook.DefinedNames.Add("SalesData", "Sheet1!$B$2:$B$100");

// Workbook-scoped constant
workbook.DefinedNames.Add("TaxRate", "0.085");

// Worksheet-scoped named range
sheet.DefinedNames.Add("Headers", "Sheet1!$A$1:$F$1");

// Use in a formula
sheet["D4"].FormulaInvariant = "=SUM(SalesData)";
sheet["E4"].FormulaInvariant = "=B4*(1+TaxRate)";
```

### Get and Update Named Ranges

```csharp
// Get a defined name by name
DefinedName name = workbook.DefinedNames.GetDefinedName("TaxRate");
if (name != null)
{
    Console.WriteLine(name.RefersTo); // e.g., "0.085"
    name.RefersTo = "0.09";           // Update the value
}
```

## Array Formulas

Array formulas calculate across multiple cells simultaneously. The API has two types:

**Legacy array formulas** (`CellRange.ArrayFormula`): entered with Ctrl+Shift+Enter equivalent.

```csharp
// Enter a legacy array formula in a range (= localized syntax, be aware of culture)
CellRange arrayRange = sheet.Range["C2:C10"];
arrayRange.ArrayFormula = "=A2:A10*B2:B10"; // Multiplies each corresponding row

// List all legacy array formulas in the worksheet
foreach (ArrayFormula af in sheet.ArrayFormulas)
    Console.WriteLine($"{af.Range.GetReferenceA1()}: {af.Formula}");
```

**Dynamic array formulas** (`CellRange.DynamicArrayFormulaInvariant`): Excel 365-style spill formulas.

```csharp
// Enter a dynamic array formula (invariant, portable syntax)
sheet["A1"].DynamicArrayFormulaInvariant = "={\"Red\",\"Green\",\"Orange\",\"Blue\"}";

// Or use the collection
sheet.DynamicArrayFormulas.Add(sheet["A2"], "=LEN(A1:D1)");
```

## Shared Formulas

Shared formulas are one formula definition applied to a range (more efficient for large ranges):

```csharp
// Enter a formula that uses relative references across a range
sheet.Range["D2:D100"].FormulaInvariant = "=B2+C2";
// The engine automatically creates a shared formula internally
```

## User-Defined Functions (UDF)

### Implement a UDF

```csharp
using DevExpress.Spreadsheet;
using DevExpress.Spreadsheet.Functions;
using System.Collections.Generic;
using System.Globalization;

public class TaxFunction : ICustomFunction
{
    private readonly ParameterInfo[] parameters = new[]
    {
        new ParameterInfo(ParameterType.Value, ParameterAttributes.Required), // amount
        new ParameterInfo(ParameterType.Value, ParameterAttributes.Optional)  // rate (optional)
    };

    public string Name => "CALCTAX";
    ParameterInfo[] IFunction.Parameters => parameters;
    ParameterType IFunction.ReturnType => ParameterType.Value;
    bool IFunction.Volatile => false;

    ParameterValue IFunction.Evaluate(IList<ParameterValue> parameters, EvaluationContext context)
    {
        double amount = parameters[0].NumericValue;
        double rate = parameters.Count == 2 ? parameters[1].NumericValue : 0.085;
        return amount * rate;
    }

    string IFunction.GetName(CultureInfo culture) => Name;
}
```

### Register and Use a UDF

```csharp
// Register BEFORE loading any document that uses the function
Workbook workbook = new Workbook();
workbook.CustomFunctions.Add(new TaxFunction());

// Use in a formula
Worksheet sheet = workbook.Worksheets[0];
sheet["A1"].Value = 1000;
sheet["B1"].FormulaInvariant = "=CALCTAX(A1)";          // Uses default rate 8.5%
sheet["C1"].FormulaInvariant = "=CALCTAX(A1, 0.1)";     // Uses 10%

workbook.Calculate();
Console.WriteLine(sheet["B1"].Value); // 85
```

> **Important**: Add custom functions to `workbook.CustomFunctions` **before** loading a document that contains those functions, or the formula will show `#NAME!`.

## Formula Engine (Advanced)

Parse and analyze formula expression trees:

```csharp
using DevExpress.Spreadsheet;
using DevExpress.Spreadsheet.Formulas;

// Access the formula engine
FormulaEngine engine = workbook.FormulaEngine;

// Parse a formula string into an expression tree
ExpressionContext context = new ExpressionContext(sheet, sheet["A1"]);
ParsedExpression parsed = engine.Parse("=SUM(A1:A10)+B1", context);

// Get all cell ranges referenced in the formula
IList<CellRange> refs = parsed.GetRanges(context);
foreach (CellRange r in refs)
    Console.WriteLine(r.GetReferenceA1()); // A1:A10, B1

// Rebuild the formula string from the expression tree
string formulaText = parsed.ToString(context);

// Evaluate a formula string against a context
CellValue result = engine.Evaluate("=SUM(A1:A5)", context);
Console.WriteLine(result.NumericValue);
```

## Configuration Options

| Property | Default | Description |
|----------|---------|-------------|
| `DocumentSettings.Calculation.Mode` | `CalculationMode.Manual` | When formulas are recalculated |
| `DocumentSettings.Calculation.RecalculateBeforeSaving` | `true` | Recalculate before saving the file |
| `DocumentSettings.Calculation.Iterative` | `false` | Allow circular references to iterate |
| `DocumentSettings.Calculation.EnableMultiThreading` | `true` | Use multiple threads for calculation |
| `Options.CalculationEngineType` | `Recursive` (Workbook) | Chain-based (UI) or Recursive (server) |

## Troubleshooting

- **Formula shows as text in a cell**: The cell's `NumberFormat` may be set to `"@"` (text). Clear it and re-enter the formula.
- **`#NAME!` error**: The function name is not recognized. Check spelling, or register your UDF before loading the document.
- **`#REF!` error**: A cell reference in the formula is invalid (deleted rows/columns or out-of-range). Verify all referenced ranges still exist.
- **Formula not recalculated**: The default `WorkbookCalculationMode` is `Manual`. Call `workbook.Calculate()` or `workbook.CalculateFull()` explicitly.
- **UDF returns wrong result**: Ensure `IFunction.Volatile` returns `true` if your function depends on external state; otherwise, cached values may be returned.
- **Circular reference error**: Set `DocumentSettings.Calculation.Iterative = true` to allow iterative calculation of circular dependencies.

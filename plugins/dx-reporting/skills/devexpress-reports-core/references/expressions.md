# Expressions and Calculated Fields

## When to Use This Reference

Use when binding control properties to computed values, adding calculated fields to the report data, or applying conditional formatting via expressions.

## Expression Syntax

DevExpress expressions are similar to SQL expressions with C#-style functions.

```
[FieldName]                      -- data field reference
[Price] * [Quantity]             -- arithmetic
Iif([Status] = 'Active', 1, 0)   -- conditional
[Name] + ' ' + [Surname]         -- string concat
?ParameterName                   -- report parameter reference
```

## ExpressionBinding

Bind any control property to an expression:

```csharp
// Text from a data field
label.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "[ProductName]"));

// Computed value
priceLabel.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "[UnitPrice] * [Quantity]"));

// Conditional visibility
control.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Visible", "[Price] > 100"));

// Conditional color
label.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "ForeColor", "Iif([Stock] = 0, 'Red', 'Black')"));

// Date formatting — use TextFormatString property instead of expressions
label.TextFormatString = "{0:MM/dd/yyyy}";
```

## Calculated Fields

Calculated fields act as virtual data fields available in field binding and filter strings:

```csharp
var totalField = new CalculatedField {
    Name = "TotalPrice",
    FieldType = FieldType.Decimal,
    Expression = "[UnitPrice] * [Quantity]"
};
report.CalculatedFields.Add(totalField);

// Use in binding
totalLabel.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "[TotalPrice]"));

// Use in filter if necessary
report.FilterString = "[TotalPrice] > 50";
```

### VB.NET

```vb
Dim totalField As New CalculatedField()
totalField.Name = "TotalPrice"
totalField.FieldType = FieldType.Decimal
totalField.Expression = "[UnitPrice] * [Quantity]"
report.CalculatedFields.Add(totalField)
```

## Summary Functions

Use summary functions in `GroupFooterBand` or `ReportFooterBand` controls:

| Function | Expression Syntax | Description |
|----------|------------------|-------------|
| Sum | `sumSum([Price])` | Sum of field |
| Count | `sumCount([Id])` | Count of records |
| Average | `sumAvg([Price])` | Average |
| Min | `sumMin([Price])` | Minimum value |
| Max | `sumMax([Price])` | Maximum value |
| Running sum | `sumRunningSum([Price])` | Cumulative total |

```csharp
// Create an XRSummary object. 
XRSummary summary = new XRSummary();
summary.Running = SummaryRunning.Group;
summary.IgnoreNullValues = true;
summary.TreatStringsAsNumerics = true;
summary.FormatString = "{0:c2}";

var totalLabel = new XRLabel();
groupFooter.Controls.Add(totalLabel);
totalLabel.BoundsF = new RectangleF(200, 0, 100, 20);
totalLabel.Summary = summary;
totalLabel.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "sumSum([Price])"));
```

## Common Iif Patterns

```
// Conditional text
Iif([Status] = 'Paid', 'Paid', 'Outstanding')

// Conditional color (use string color name or #RRGGBB)
Iif([Balance] < 0, 'Red', 'Black')

// Nested conditional
Iif([Score] >= 90, 'A', Iif([Score] >= 80, 'B', 'C'))
```

---

## Function Reference

> Function names differ from C#/LINQ/SQL equivalents — look up each function here before using it. See the SKILL.md **Antipatterns** section (AP2).

### String Functions

| Function | Description |
|----------|-------------|
| `FormatString(format, value)` | Formats a value using a .NET format spec: `FormatString('{0:N0}', [Area])`, `FormatString('{0:C2}', [Price])`, `FormatString('{0:MM/dd/yyyy}', [Date])` |
| `Concat(str1, str2, ...)` | Concatenates strings (null-safe; preferred over `+`) |
| `Substring(string, start, length)` | Returns a substring |
| `Len(string)` | Returns the length of a string |
| `Upper(string)` | Converts to uppercase |
| `Lower(string)` | Converts to lowercase |
| `Trim(string)` | Removes leading and trailing whitespace |
| `Replace(string, old, new)` | Replaces all occurrences of a substring |
| `Contains(string, substring)` | Returns `True` if the string contains the substring |
| `StartsWith(string, substring)` | Returns `True` if the string starts with the substring |
| `EndsWith(string, substring)` | Returns `True` if the string ends with the substring |
| `Insert(string, pos, insert)` | Inserts a string at the specified position |
| `Remove(string, start[, length])` | Removes characters from a string |
| `CharIndex(str1, str2[, start[, len]])` | Returns the index of the first occurrence of one string in another |
| `PadLeft(string, length[, char])` | Pads the string on the left |
| `PadRight(string, length[, char])` | Pads the string on the right |
| `Reverse(string)` | Reverses the character order |
| `Ascii(string)` | Returns the ASCII code of the first character |
| `Char(number)` | Converts a number to a Unicode character |

### Logical Functions

| Function | Description |
|----------|-------------|
| `Iif(expr, trueVal, ..., falseVal)` | Multi-branch conditional (supports chaining) |
| `IsNull(value[, value2])` | Returns `True`/`False` (1 arg) or the first non-null value (2 args) |
| `IsNullOrEmpty(string)` | Returns `True` if the value is null or empty string |
| `InRange(value, from, to)` | Returns `True` if a numeric value is within the specified range |

### Math Functions

| Function | Description |
|----------|-------------|
| `Abs(value)` | Absolute value |
| `Ceiling(value)` | Smallest integer ≥ value |
| `Floor(value)` | Largest integer ≤ value |
| `Round(value[, precision])` | Rounds to nearest integer or decimal places |
| `Sqr(value)` | Square root |
| `Power(value, power)` | Raises value to the specified power |
| `Rnd()` | Random number between 0 (inclusive) and 1 (exclusive) |
| `Max(value1, value2)` | Larger of two numeric values |
| `Min(value1, value2)` | Smaller of two numeric values |
| `Sign(value)` | Returns +1, 0, or -1 |
| `Log(value[, base])` | Natural log, or log to specified base |
| `Log10(value)` | Base-10 logarithm |
| `Exp(value)` | Exponential (eˣ) |
| `ToDecimal(value)` | Converts to decimal |
| `ToDouble(value)` | Converts to double |
| `ToInt(value)` | Converts to integer |

### Date and Time Functions

| Function | Description |
|----------|-------------|
| `Now()` | Current date and time |
| `Today()` | Current date with time set to 00:00:00 |
| `UtcNow()` | Current date and time in UTC |
| `GetDate(dateTime)` | Returns the date-only part of a DateTime |
| `GetYear(date)` | Year component |
| `GetMonth(date)` | Month number (1–12) |
| `GetDay(date)` | Day of the month |
| `GetDayOfWeek(date)` | Day of the week as integer |
| `GetDayOfYear(date)` | Day of the year (1–366) |
| `GetHour(time)` | Hour (0–23) |
| `GetMinute(time)` | Minute (0–59) |
| `GetSecond(time)` | Second (0–59) |
| `AddDays(date, count)` | Adds days |
| `AddMonths(date, count)` | Adds months |
| `AddYears(date, count)` | Adds years |
| `AddHours(dateTime, count)` | Adds hours |
| `AddMinutes(dateTime, count)` | Adds minutes |
| `AddSeconds(dateTime, count)` | Adds seconds |
| `DateDiffDay(start, end)` | Number of day boundaries crossed |
| `DateDiffMonth(start, end)` | Number of month boundaries crossed |
| `DateDiffYear(start, end)` | Number of year boundaries crossed |
| `DateDiffHour(start, end)` | Number of hour boundaries crossed |
| `DateDiffMinute(start, end)` | Number of minute boundaries crossed |
| `DateDiffSecond(start, end)` | Number of second boundaries crossed |
| `DateTimeFromParts(year, month, day, ...)` | Constructs a DateTime from components |
| `InDateRange(date, from, to)` | Returns `True` if the date is within the range |
| `IsThisWeek(date)` | Returns `True` if the date is in the current week |
| `IsThisMonth(date)` | Returns `True` if the date is in the current month |
| `IsThisYear(date)` | Returns `True` if the date is in the current year |
| `IsLastMonth(date)` | Returns `True` if the date is in the previous month |
| `IsLastYear(date)` | Returns `True` if the date is in the previous year |
| `IsSameDay(date1, date2)` | Returns `True` if two dates fall on the same day |

### Aggregate Functions

These functions operate over a collection (e.g., a detail band data source) when used in a calculated field or filter:

| Function | Description |
|----------|-------------|
| `Avg(value)` | Average of values in the collection |
| `Count()` | Number of objects in the collection |
| `Exists()` | Returns `True` if any object exists in the collection |
| `Join()` | Concatenates all expression values into a single string |
| `Max(value)` | Maximum value in the collection |
| `Min(value)` | Minimum value in the collection |
| `Sum(value)` | Sum of all expression values in the collection |
| `Single()` | Returns the object if it is the only element in the collection |

> **Note**: These aggregate functions (`Sum`, `Count`, etc.) operate on collections in filter expressions. For summary bands (`GroupFooterBand`, `ReportFooterBand`) use the `sum*()` family instead: `sumSum([Field])`, `sumCount()`, `sumAvg([Field])`, `sumMax([Field])`, `sumMin([Field])`, `sumRunningSum([Field])`. See the **Summary Functions** section above.

### Reporting Functions

| Function | Description |
|----------|-------------|
| `FormatString(format, value)` | Formats a value — **the correct way to apply number/date formats in expressions** |
| `Iif(condition, trueVal, falseVal)` | Conditional (also in Logical Functions) |
| `Rgb(r, g, b)` | Returns a color string from RGB values |
| `Argb(alpha, r, g, b)` | Returns a color string from ARGB values |
| `GetDisplayText(?paramName)` | Returns the display text of a parameter's lookup value |
| `CurrentRowIndexInGroup()` | Zero-based index of the current row within its group |
| `GroupIndex(level)` | Index of the parent group row at the specified nesting level |
| `NextRowColumnValue(columnName)` | Value from the specified column in the next row |
| `PrevRowColumnValue(columnName)` | Value from the specified column in the previous row |
| `ConvertDataToEPC(...)` | Converts data into a formatted string for EPC QR codes |

---

## Notes

- Expression property names in `ExpressionBinding` are case-sensitive (`"Text"`, `"Visible"`, `"ForeColor"`).
- `TextFormatString` uses `{0:formatSpec}` syntax — it is separate from `ExpressionBinding` and applied after the value is resolved.
- For complex calculated logic, prefer `CalculatedField` over inline expressions in bindings — it can be reused across multiple controls and in the filter string.

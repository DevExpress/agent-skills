## Mistake 1

### Task context
The user asked to create an XtraReport in code that groups data by a field and exports to PDF; the agent needed to format a numeric field using an expression binding.

### What the skill said (or didn't say)
The skill's **Common Patterns** section showed expression binding examples but never listed the available expression functions. The skill stated:

> "If a scenario is not covered in the main skill body, read the relevant reference file or use MCP servers before writing code — do not guess API signatures."

The skill references a separate `references/` directory and links to `Functions in Expressions` docs, but does not inline the function list in the main skill body.

### What you did wrong
Used the non-existent expression function `Format('{0:N0}', [Area])` in an `ExpressionBinding`. No such function exists in the DevExpress expression language.

```csharp
// Wrong
cellArea.ExpressionBindings.Add(
	new ExpressionBinding("Text", "Format('{0:N0}', [Area])"));
```

### Why you made the mistake
The skill was **silent on the exact list of expression functions** in the main body. The correct function `FormatString` was only discoverable via dxdocs lookup. The agent guessed based on general .NET knowledge (`String.Format`) instead of consulting dxdocs first.

### What the correct behavior should have been
Before writing any expression string, consult dxdocs for the expression function list. The correct function is `FormatString(format, value)`:

```csharp
// Correct
cellArea.ExpressionBindings.Add(
	new ExpressionBinding("Text", "FormatString('{0:N0}', [Area])"));
```

### Proposed skill fix
**New rule** — Add a CRITICAL hard-stop in the **Common Patterns / Expression binding** section:

```markdown
> **Expression function names are NOT .NET method names.**
> Do not guess expression function names from general C# knowledge.
> Always look up the exact function in `expressions.md` and dxdocs (`Functions in Expressions`) before writing any expression string.
> The most commonly needed functions are:
> - `sumCount()`, `sumSum()`, `sumAvg()` — summary functions (only valid in Summary Expression Editor context)
> - `Iif(condition, trueVal, falseVal)` — conditional
> - `Now()`, `Today()` — date functions
```


Add a list of available functions to `expressions.md`:


## Aggregate Functions

| Function | Description |
| :-- | :-- |
| `Avg(Value)` | Returns the average of values in a collection |
| `Count()` | Returns the number of objects in a collection |
| `Exists()` | Determines whether the object exists in a collection |
| `Join()` | Concatenates all expression values in a collection into a single string |
| `Max(Value)` | Returns the maximum expression value in a collection |
| `Min(Value)` | Returns the minimum expression value in a collection |
| `Single()` | Returns an object if it is the only element in a collection |
| `Single(Expression)` | Returns the expression value if only one object in a collection meets the condition |
| `Sum(Value)` | Returns the sum of all expression values in a collection |


***

## Date and Time Functions

| Function | Description |
| :-- | :-- |
| `Now()` | Returns the current date and time |
| `Today()` | Returns the current date with time set to 00:00:00 |
| `UtcNow()` | Returns the current date and time in UTC |
| `UtcToLocalTime(DateTime)` | Converts a UTC date-time to local time zone |
| `LocalTimeToUtc(DateTime)` | Converts a local date-time to UTC |
| `AddTicks(DateTime, Count)` | Adds the specified number of 100-ns ticks to a date |
| `AddMilliSeconds(Time, Count)` | Adds milliseconds to a date/time value |
| `AddSeconds(Time, Count)` | Adds seconds to a date/time value |
| `AddMinutes(Time, Count)` | Adds minutes to a date/time value |
| `AddHours(Time, Count)` | Adds hours to a date/time value |
| `AddDays(Date, Count)` | Adds days to a date value |
| `AddMonths(Date, Count)` | Adds months to a date value |
| `AddYears(Date, Count)` | Adds years to a date value |
| `AddTimeSpan(DateTime, TimeSpan)` | Adds a TimeSpan to a date value |
| `DateDiffDay(Start, End)` | Returns the number of day boundaries between two dates |
| `DateDiffMonth(Start, End)` | Returns the number of month boundaries between two dates |
| `DateDiffYear(Start, End)` | Returns the number of year boundaries between two dates |
| `DateDiffMilliSecond(Start, End)` | Returns the number of millisecond boundaries between two dates/times |
| `DateDiffSecond(Start, End)` | Returns the number of second boundaries between two dates/times |
| `DateDiffMinute(Start, End)` | Returns the number of minute boundaries between two dates/times |
| `DateDiffHour(Start, End)` | Returns the number of hour boundaries between two dates/times |
| `DateDiffTick(Start, End)` | Returns the number of tick boundaries between two dates |
| `GetDate(Date)` | Returns the date part of a DateTime value |
| `GetYear(Date)` | Returns the year of a date |
| `GetMonth(Date)` | Returns the month number of a date |
| `GetDay(Date)` | Returns the day of the month |
| `GetDayOfYear(Date)` | Returns the day of the year (1–366) |
| `GetDayOfWeek(Date)` | Returns the day of the week as an integer |
| `GetHour(Time)` | Returns the hours value (0–23) of a date/time |
| `GetMinute(Time)` | Returns the minutes value (0–59) of a date/time |
| `GetSecond(Time)` | Returns the seconds value (0–59) of a date/time |
| `GetMilliSecond(Time)` | Returns the milliseconds value (0–999) of a date/time |
| `GetTimeOfDay(DateTime)` | Returns the time part as ticks elapsed since midnight |
| `DateTimeFromParts(Year, Month, Day, ...)` | Constructs a DateTime from year, month, day (and optionally hour, minute, second, ms) |
| `DateOnlyFromParts(Year, Month, Day)` | Constructs a DateOnly value from year, month, and day |
| `TimeOnlyFromParts(Hour, Minute, ...)` | Constructs a TimeOnly value from hour, minute (and optionally second, ms) |
| `InDateRange(Date, From, To)` | Returns `True` if a date falls within the specified range |
| `IsThisWeek(Date)` | Returns `True` if the date is in the current week |
| `IsThisMonth(Date)` | Returns `True` if the date is in the current month |
| `IsThisYear(Date)` | Returns `True` if the date is in the current year |
| `IsLastMonth(Date)` | Returns `True` if the date is in the previous month |
| `IsLastYear(Date)` | Returns `True` if the date is in the previous year |
| `IsNextMonth(Date)` | Returns `True` if the date is in the next month |
| `IsNextYear(Date)` | Returns `True` if the date is in the next year |
| `IsYearToDate(Date)` | Returns `True` if the date is from Jan 1 of this year to today |
| `IsSameDay(Date1, Date2)` | Returns `True` if two dates fall on the same day |
| `IsJanuary(Date)` – `IsDecember(Date)` | Returns `True` if the date falls within the specified month |
| `LocalDateTimeNow()` | Returns the current date and time |
| `LocalDateTimeToday()` | Returns the start of the current day |
| `LocalDateTimeTomorrow()` | Returns the start of tomorrow |
| `LocalDateTimeYesterday()` | Returns the start of yesterday |
| `LocalDateTimeDayAfterTomorrow()` | Returns the start of the day two days from now |
| `LocalDateTimeThisWeek()` | Returns the start of the current week |
| `LocalDateTimeLastWeek()` | Returns the start of the previous week |
| `LocalDateTimeNextWeek()` | Returns the start of the next week |
| `LocalDateTimeTwoWeeksAway()` | Returns the start of the week after next |
| `LocalDateTimeThisMonth()` | Returns the first day of the current month |
| `LocalDateTimeLastMonth()` | Returns the first day of the previous month |
| `LocalDateTimeNextMonth()` | Returns the first day of the next month |
| `LocalDateTimeTwoMonthsAway()` | Returns the first day of the month after next |
| `LocalDateTimeThisYear()` | Returns the first day of the current year |
| `LocalDateTimeLastYear()` | Returns the first day of the previous year |
| `LocalDateTimeNextYear()` | Returns the first day of the next year |
| `LocalDateTimeTwoYearsAway()` | Returns the first day of the year after next |
| `LocalDateTimeYearBeforeToday()` | Returns the date one year ago |
| `AfterMidday(Time)` | Returns `True` if the time is after 12:00 PM |
| `BeforeMidday(Time)` | Returns `True` if the time is before 12:00 PM |
| `IsAfternoon(Time)` | Returns `True` if the time is between 12:00 PM and 6:00 PM |
| `IsEvening(Time)` | Returns `True` if the time is between 6:00 PM and 9:00 PM |
| `IsMorning(Time)` | Returns `True` if the time is between 6:00 AM and 12:00 PM |
| `IsNight(Time)` | Returns `True` if the time is between 9:00 PM and 9:00 AM |
| `IsLunchTime(Time)` | Returns `True` if the time falls within lunch time |
| `IsFreeTime(Time)` | Returns `True` if the time falls within free time |
| `IsWorkTime(Time)` | Returns `True` if the time falls within work time |
| `IsLastHour(Time)` | Returns `True` if the time falls within the last hour |
| `IsNextHour(Time)` | Returns `True` if the time falls within the next hour |
| `IsThisHour(Time)` | Returns `True` if the time falls within the current hour |
| `IsSameHour(Time)` | Returns `True` if the time falls within the same hour |
| `IsSameTime(Time)` | Returns `True` if the time matches the same hour and minute |


***

## Logical Functions

| Function | Description |
| :-- | :-- |
| `Iif(Expr, TrueVal, ..., FalseVal)` | Returns one of several values based on logical expressions (multi-branch conditional) |
| `InRange(Value, From, To)` | Returns `True` if a numeric value is within the specified range |
| `IsNull(Value [, Value2])` | Returns `True`/`False` (1 arg) or the first non-null value (2 args) |
| `IsNullOrEmpty(String)` | Returns `True` if the value is null or an empty string |


***

## Math Functions

| Function | Description |
| :-- | :-- |
| `Abs(Value)` | Returns the absolute value |
| `Acos(Value)` | Returns the arccosine (in radians) |
| `Asin(Value)` | Returns the arcsine (in radians) |
| `Atn(Value)` | Returns the arctangent (in radians) |
| `Atn2(Value1, Value2)` | Returns the arctangent of the quotient of two values |
| `BigMul(Value1, Value2)` | Calculates the full product of two integers |
| `Ceiling(Value)` | Returns the smallest integer ≥ the specified value |
| `Cos(Value)` | Returns the cosine of an angle in radians |
| `Cosh(Value)` | Returns the hyperbolic cosine |
| `Exp(Value)` | Returns the exponential value (e^x) |
| `Floor(Value)` | Returns the largest integer ≤ the specified value |
| `Log(Value)` | Returns the natural logarithm |
| `Log(Value, Base)` | Returns the logarithm to the specified base |
| `Log10(Value)` | Returns the base-10 logarithm |
| `Max(Value1, Value2)` | Returns the larger of two numeric values |
| `Min(Value1, Value2)` | Returns the smaller of two numeric values |
| `Power(Value, Power)` | Raises a value to the specified power |
| `Rnd()` | Returns a random number between 0 (inclusive) and 1 (exclusive) |
| `Round(Value [, Precision])` | Rounds a value to the nearest integer or specified decimal places |
| `Sign(Value)` | Returns +1, 0, or -1 based on the sign of the value |
| `Sin(Value)` | Returns the sine of an angle in radians |
| `Sinh(Value)` | Returns the hyperbolic sine |
| `Sqr(Value)` | Returns the square root |
| `Tan(Value)` | Returns the tangent of an angle in radians |
| `Tanh(Value)` | Returns the hyperbolic tangent |
| `ToDecimal(Value)` | Converts to decimal |
| `ToDouble(Value)` | Converts to double |
| `ToFloat(Value)` | Converts to float |
| `ToInt(Value)` | Converts to integer |
| `ToLong(Value)` | Converts to long integer |


***

## String Functions

| Function | Description |
| :-- | :-- |
| `Ascii(String)` | Returns the ASCII code of the first character |
| `Char(Number)` | Converts a number to a Unicode character |
| `CharIndex(String1, String2 [, Start [, Length]])` | Returns the index of the first occurrence of one string in another |
| `Concat(String1, ..., StringN)` | Concatenates multiple strings |
| `Contains(String, SubString)` | Returns `True` if a string contains the specified substring |
| `EndsWith(String, SubString)` | Returns `True` if a string ends with the specified substring |
| `Insert(String, Position, InsertString)` | Inserts a string at the specified index position |
| `Len(String)` | Returns the length of a string |
| `Lower(String)` | Converts a string to lowercase |
| `PadLeft(String, Length [, Char])` | Pads a string on the left to a specified total length |
| `PadRight(String, Length [, Char])` | Pads a string on the right to a specified total length |
| `Remove(String, Start [, Length])` | Removes characters from a string starting at a position |
| `Replace(String, OldSubStr, NewSubStr)` | Replaces all occurrences of a substring |
| `Reverse(String)` | Reverses the character order of a string |
| `StartsWith(String, SubString)` | Returns `True` if a string begins with the specified substring |


***

## Reporting Functions

| Function | Description |
| :-- | :-- |
| `Argb(Alpha, Red, Green, Blue)` | Returns a color string from ARGB channel values |
| `Rgb(Red, Green, Blue)` | Returns a color string from RGB channel values |
| `ConvertDataToEPC(...)` | Converts data into a formatted string for EPC QR codes |
| `GetDisplayText(?parameterName)` | Returns the display text of a parameter's lookup value |
| `CurrentRowIndexInGroup()` | Returns the current row's index within its group |
| `GroupIndex(Level)` | Returns the index of the parent group row at the specified nesting level |
| `NextRowColumnValue(ColumnName)` | Returns the value from the specified column in the next row |
| `PrevRowColumnValue(ColumnName)` | Returns the value from the specified column in the previous row |


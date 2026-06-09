# Report Parameters

## When to Use This Reference

Use when adding input parameters to reports: simple value parameters, multi-value, cascading, date ranges, and applying parameters to filter strings.

## Define a Parameter

```csharp
using DevExpress.XtraReports.Parameters;

var dateParameter = new Parameter() {
    Name = "date",
    Description = "Date:",
    Type = typeof(System.DateTime),
    ExpressionBindings = { new BasicExpressionBinding("Value", "Now()") }
};
report.Parameters.Add(dateParameter);
```

Apply the parameter in a filter string:

```csharp
report.FilterString = "[OrderDate] >= ?StartDate And [OrderDate] <= ?EndDate";
```

The `?ParameterName` syntax references a parameter in filter expressions.

## Parameter Types

| Use Case | `Type` |
|----------|--------|
| Text/string | `typeof(string)` |
| Integer | `typeof(int)` |
| Decimal | `typeof(decimal)` |
| DateTime | `typeof(DateTime)` |
| Boolean | `typeof(bool)` |
| Enum/list | `typeof(string)` with `LookUpSettings` |
| Custom type | Any serializable type |

## Hide a Parameter from the UI

```csharp
param.Visible = false;
```

Useful when setting a parameter programmatically before display.

## Set Parameter Value at Runtime

```csharp
report.Parameters["StartDate"].Value = new DateTime(2025, 1, 1);
report.Parameters["EndDate"].Value = new DateTime(2025, 12, 31);
```

## Multi-Value Parameter

```csharp
var param = new Parameter {
    Name = "Categories",
    Type = typeof(string),
    MultiValue = true,
    Value = new[] { "Tools", "Parts" }
};
report.Parameters.Add(param);
// Filter: "[Category] In (?Categories)"
```

## Static Lookup Values

```csharp
var param = new Parameter {
    Name = "Status",
    Type = typeof(string),
    Value = "Active"
};

var lookUp = new StaticListLookUpSettings();
lookUp.LookUpValues.Add(new LookUpValue("Active", "Active"));
lookUp.LookUpValues.Add(new LookUpValue("Inactive", "Inactive"));
lookUp.LookUpValues.Add(new LookUpValue("All", "All"));

param.LookUpSettings = lookUp;
report.Parameters.Add(param);
```

## Dynamic Lookup (data-bound dropdown)

```csharp
var param = new Parameter { Name = "CategoryId", Type = typeof(int) };

var lookUp = new DynamicListLookUpSettings {
    DataSource = categoryList,
    ValueMember = "Id",
    DisplayMember = "Name"
};
param.LookUpSettings = lookUp;
report.Parameters.Add(param);
```

## Cascading Parameters

Create two parameters where the second depends on the first. Connect them using `DynamicListLookUpSettings` with a `FilterString` that references the first parameter:

```csharp
var regionParam = new Parameter { Name = "Region", Type = typeof(string) };
var cityParam = new Parameter { Name = "City", Type = typeof(string) };

var cityLookUp = new DynamicListLookUpSettings {
    DataSource = cityList,
    ValueMember = "CityName",
    DisplayMember = "CityName",
    FilterString = "[Region] = ?Region"   // depends on Region parameter
};
cityParam.LookUpSettings = cityLookUp;

report.Parameters.Add(regionParam);
report.Parameters.Add(cityParam);
```

## Using Parameters in Expressions

Reference a parameter in an expression binding using `?ParameterName` or `Parameters.ParameterName`:

```csharp
// In filter string
report.FilterString = "[Price] >= ?MinPrice";

// In ExpressionBinding
label.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text",
    "Iif([Price] >= ?MinPrice, 'In Range', 'Below Min')"));
```

## VB.NET

```vb
Imports DevExpress.XtraReports.Parameters

Dim param As New Parameter()
param.Name = "StartDate"
param.Description = "Start Date"
param.Type = GetType(DateTime)
param.Value = DateTime.Today.AddMonths(-1)
report.Parameters.Add(param)

report.FilterString = "[OrderDate] >= ?StartDate"
```

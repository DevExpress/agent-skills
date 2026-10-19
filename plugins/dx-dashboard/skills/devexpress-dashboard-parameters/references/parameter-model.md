# Parameter Model — Definition & Binding

This reference covers how to **create** and **bind** parameters. It is platform-agnostic — the same code works on WinForms, WPF, and ASP.NET Core.

## Namespaces Required

```csharp
using DevExpress.DashboardCommon;   // Dashboard, DashboardParameter, StaticListLookUpSettings, DynamicListLookUpSettings, DashboardSqlDataSource
using DevExpress.DataAccess;        // Expression  ← NOT DevExpress.DataAccess.Sql
using DevExpress.DataAccess.Sql;    // CustomSqlQuery, SelectQueryFluentBuilder, QueryParameter
```

---

## DashboardParameter Constructors

There are exactly two argument-taking constructors, plus a parameterless one. **No arguments are optional.**

```csharp
public DashboardParameter(string name, Type type, object value);

public DashboardParameter(
    string name,                            // unique identifier; used as ?name and [Parameters.name]
    Type type,                              // typeof(string), typeof(int), typeof(DateTime), ...
    object value,                           // current value (also what the Reset button restores)
    string description,                     // label in the Dashboard Parameters dialog
    bool visible,                           // show/hide in the dialog
    ParameterLookUpSettings lookupsettings); // StaticListLookUpSettings, DynamicListLookUpSettings, or null
```

Named arguments must use the real parameter names — `value`, `description`, `lookupsettings`:

```csharp
var yearParam = new DashboardParameter(
    name: "yearParameter",
    type: typeof(string),
    value: "2024",
    description: "Select Year:",
    visible: true,
    lookupsettings: new StaticListLookUpSettings {
        Values = new string[] { "2022", "2023", "2024" }
    });

dashboard.Parameters.Add(yearParam);
```

### Parameterless Constructor + Property Setters

Equally valid, and the style DevExpress's own examples use. Required for `AllowMultiselect`, `AllowNull`, and `SelectAllValues` — they have no constructor slot:

```csharp
using System.Collections.Generic;   // List<T>

DashboardParameter stateParam = new DashboardParameter();
stateParam.Name = "parameterState";
stateParam.Type = typeof(string);
stateParam.AllowMultiselect = true;
stateParam.LookUpSettings = new StaticListLookUpSettings {
    Values = new string[] { "Alabama", "Ohio", "Utah" }
};
stateParam.Value = new List<string> { "Ohio", "Utah" };   // multi-value initial value

dashboard.Parameters.Add(stateParam);
```

### Properties on the Model Object

| Property | Type | Notes |
|----------|------|-------|
| `Name` | `string` | Unique within the dashboard; case-sensitive |
| `Type` | `Type` | CLR type of the value |
| `Value` | `object` | Current value. **This class has no `DefaultValue` property.** |
| `Description` | `string` | Label in the Dashboard Parameters dialog |
| `Visible` | `bool` | `false` hides it from the dialog (still usable in queries/filters) |
| `AllowMultiselect` | `bool` | Enables multiple selection |
| `AllowNull` | `bool` | Permits a null value |
| `SelectAllValues` | `bool` | Pre-selects every look-up value on load. In effect only when `AllowMultiselect` is `true`; restricted on dynamic look-ups |
| `LookUpSettings` | `ParameterLookUpSettings` | Static list, dynamic list, or `null` |

---

## Look-Up Settings

### No Look-Up (Free-Form Input)

Pass `null` — only the three-argument and six-argument overloads exist:

```csharp
var targetParam = new DashboardParameter(
    "targetProfit", typeof(decimal), 100000m,
    "Target Profit:", true, null);
dashboard.Parameters.Add(targetParam);
```

### StaticListLookUpSettings (Hard-Coded Values)

Use a static list when you hard-code the values yourself. Use a dynamic list when they come from a data source.

`Values` is a `string[]` even when the parameter's `Type` is `int` or `DateTime` — the strings are converted at runtime.

```csharp
var categoryParam = new DashboardParameter(
    "categoryParameter", typeof(string), "Beverages",
    "Select Category:", true,
    new StaticListLookUpSettings {
        Values = new string[] { "Beverages", "Condiments", "Produce" }
    });

dashboard.Parameters.Add(categoryParam);
```

### DynamicListLookUpSettings (Values From a Query)

Use it when values come from data. `DataMember` names an existing query (for SQL and Entity Framework sources), and `ValueMember` a column of its result. OLAP data sources cannot supply parameter values.

```csharp
var dataSource = (DashboardSqlDataSource)dashboard.DataSources[0];

var settings = new DynamicListLookUpSettings {
    DataSource = dataSource,         // the data source object
    DataMember = "Categories",       // the QUERY name (not the database table name, unless they match)
    ValueMember = "CategoryID",      // column stored in the parameter value
    DisplayMember = "CategoryName",  // column shown to the user (optional)
    SortOrder = DimensionSortOrder.Ascending
};

var categoryParam = new DashboardParameter(
    "categoryParameter", typeof(int), 1,
    "Select Category:", true, settings);

dashboard.Parameters.Add(categoryParam);
```

### Cascading Parameters Are Out of Scope

This skill does not cover cascading parameters (one parameter filtering another's look-up list). Do not write a cascading pattern from memory.

1. Fetch `devexpress_docs_get_content(url="https://docs.devexpress.com/Dashboard/404884/winforms-dashboard/winforms-designer/create-dashboards-in-the-winforms-designer/data-analysis/dashboard-parameters/create-cascading-parameters")`.
2. Follow its **Create Cascading Parameters in Code** section only. The code uses `DevExpress.DashboardCommon` classes, so it applies to every platform, the web included.

If the docs tool is not available, tell the user that cascading parameters are not covered and stop.

---

## Reading Look-Up Values Back in Code

**No dashboard equivalent exists for the Reports API's `GetLookUpValues`.** Read `LookUpSettings` and branch on its concrete type — it is declared as the abstract base `ParameterLookUpSettings`:

```csharp
using DevExpress.DashboardCommon;

// Look up by name, and null-check the result before using it.
DashboardParameter parameter = dashboard.Parameters["categoryParameter"];
if (parameter == null) return;

switch (parameter.LookUpSettings) {
    case StaticListLookUpSettings staticSettings:
        // Values is a string[] — the list is known without touching data.
        foreach (string value in staticSettings.Values)
            Console.WriteLine(value);
        break;

    case DynamicListLookUpSettings dynamicSettings:
        // Nothing is materialized here. These members only describe WHERE the values
        // come from: DataSource (IDashboardDataSource), DataSourceName, DataMember,
        // ValueMember, DisplayMember, SortByMember, SortOrder. The values themselves
        // are not on the settings object — query the data source to get them.
        break;

    case null:
        break;   // free-form input — no look-up list exists
}
```

`StaticListLookUpSettings` **contains** its values; `DynamicListLookUpSettings` only **points at** them. Code that walks parameters must handle both shapes, and must not assume a dynamic list can be read without a data round-trip.


Three ways to name the data source, all valid and none needing a cast. `dynamicSettings.DataSourceName` is a string and works even when `DataSource` is null. `DataSource` itself is declared as `IDashboardDataSource`, which exposes both `Name` (e.g. `"Northwind Source"`) and `ComponentName` (the XML identifier, e.g. `"dashboardSqlDataSource1"`) — inherited from `IDashboardComponent`, which is why they do not appear on the `IDashboardDataSource` documentation page. `Name` and `ComponentName` are different strings, so pass the one the target API expects.

---

## Binding Parameters to Filters

Three levels, each with its own identifier vocabulary:

| Level | Property | Identifiers are | Scope |
|-------|----------|-----------------|-------|
| Data source | `dataSource.Filter` | data source **field names** | everything bound to that data source |
| Dashboard item | `item.FilterString` | data item **`UniqueId`s** | one item, after data is fetched |
| Query | `query.Parameters` + placeholder | SQL/query **parameter names** | server-side, before data is fetched |

You can combine all three levels: the query narrows what is fetched, the data source filter narrows what every item sees, and an item filter string narrows one item further.

### Option 1: Data Source Filter (All Items at Once)

Identifiers are the data source's own field names — no `UniqueId` required:

```csharp
using System.Collections.Generic;
using DevExpress.DashboardCommon;

DashboardParameter stateParam = new DashboardParameter();
stateParam.Name = "parameterState";
stateParam.Type = typeof(string);
stateParam.AllowMultiselect = true;
stateParam.LookUpSettings = new StaticListLookUpSettings {
    Values = new string[] { "Alabama", "Ohio", "Utah" }
};
stateParam.Value = new List<string> { "Ohio", "Utah" };
dashboard.Parameters.Add(stateParam);

// [State] is a data source field; In (?p) because the parameter is multi-value.
dashboard.DataSources[0].Filter = "[State] In (?parameterState)";
```

To filter only one item, use an item filter string (Option 2) instead.

### Option 2: Filter String (Client-Side Item Filtering)

Applies to a single dashboard item **after** data is fetched. Identifiers are **data item `UniqueId`s**, not database field names — assign the `UniqueId` first.

```csharp
GridDashboardItem grid = (GridDashboardItem)dashboard.Items[0];
var categoryDimension = ((GridDimensionColumn)grid.Columns[0]).Dimension;
categoryDimension.UniqueId = "categoryColumn";

grid.FilterString = "[categoryColumn] = ?categoryParameter";

// Multi-value parameters use the IN operator:
grid.FilterString = "[categoryColumn] in (?categoryParameter)";
```

**Build the item from scratch** (`dataSource` already exists):

```csharp
using DevExpress.DashboardCommon;

Dashboard dashboard = new Dashboard();

var categoryParam = new DashboardParameter(
    "categoryParameter", typeof(string), "Beverages",
    "Select Category:", true,
    new StaticListLookUpSettings {
        Values = new string[] { "Beverages", "Condiments", "Produce" }
    });
dashboard.Parameters.Add(categoryParam);

// GridDashboardItem has a parameterless constructor only.
GridDashboardItem grid = new GridDashboardItem();
grid.ComponentName = "gridDashboardItem1";   // component name, not the caption
grid.DataSource = dataSource;
grid.DataMember = "OrdersQuery";             // the query name on that data source

var categoryDimension = new Dimension("CategoryName");
categoryDimension.UniqueId = "categoryColumn";
grid.Columns.Add(new GridDimensionColumn(categoryDimension));
grid.Columns.Add(new GridMeasureColumn(new Measure("UnitPrice", SummaryType.Sum)));

grid.FilterString = "[categoryColumn] = ?categoryParameter";
dashboard.Items.Add(grid);
```

### Option 3: SQL Query Parameter (Server-Side Filtering)

A query parameter filters data **before** it reaches the client. Prefer it for large SQL datasets.

**How the three names relate:**
1. The SQL text contains the provider placeholder — `@CategoryParam` for SQL Server (`:name` for Oracle).
2. The `QueryParameter` name must match that placeholder.
3. The `Expression` references the dashboard parameter: `[Parameters.categoryParameter]` (the short form `?categoryParameter` is also valid).

```csharp
using DevExpress.DashboardCommon;
using DevExpress.DataAccess;
using DevExpress.DataAccess.Sql;

// ...
// Web: custom SQL queries are disabled by default.
CustomSqlQuery query = new CustomSqlQuery("OrdersQuery",
    "SELECT * FROM Orders WHERE Category = @CategoryParam");

query.Parameters.Add(new QueryParameter(
    "CategoryParam",                                  // matches @CategoryParam above
    typeof(Expression),                               // the type must be Expression
    new Expression("[Parameters.categoryParameter]")  // references the dashboard parameter
));

dataSource.Queries.Add(query);
```

A `SelectQuery` built with the fluent builder uses a filter string instead of raw SQL:

```csharp
SelectQuery productsQuery = SelectQueryFluentBuilder
    .AddTable("Products")
    .SelectAllColumns()
    .Filter("[Products.CategoryID] = ?categoryQueryParam")
    .Build("Products");
productsQuery.Parameters.Add(new QueryParameter(
    "categoryQueryParam", typeof(Expression), new Expression("?categoryParameter")));
dataSource.Queries.Add(productsQuery);
```

**Stored procedures** work the same way — `StoredProcQuery` exposes the same `Parameters` collection; the stored-procedure parameter API is identical:

```csharp
using DevExpress.DashboardCommon;
using DevExpress.DataAccess;        // Expression
using DevExpress.DataAccess.Sql;    // StoredProcQuery, QueryParameter

// Constructor is (queryName, storedProcName). queryName is what the dashboard
// shows as the data member; storedProcName is the procedure in the database.
StoredProcQuery spQuery = new StoredProcQuery("SalesByYear", "GetSalesByYear");

// The QueryParameter name must match the procedure's parameter (@Year here).
spQuery.Parameters.Add(new QueryParameter(
    "@Year", typeof(Expression), new Expression("[Parameters.yearParameter]")));

dataSource.Queries.Add(spQuery);
```

Do **not** hard-code the value (`new QueryParameter("@Year", typeof(int), 2024)`) when a dashboard parameter should drive it — that pins the procedure to one value and the parameter never reaches it.

**A multi-value parameter into a stored procedure — collapse the list with `JOIN`.** A procedure parameter takes one scalar, so the list must be joined into a single string and split inside the procedure:

```csharp
// JOIN(?orderIDs) turns the selected values into one comma-separated string.
spQuery.Parameters.Add(new QueryParameter(
    "@data", typeof(Expression), new Expression("JOIN(?orderIDs)")));
```

The procedure then splits `@data` (`STRING_SPLIT` on SQL Server, or an equivalent) and filters on the result.


---

## Using Parameters in Calculated Fields

A calculated field belongs to the **data source**, and its `Expression` is a `string`:

```csharp
using DevExpress.DashboardCommon;

var field = new CalculatedField();
field.DataMember = "OrdersQuery";        // the query the field is calculated for
field.Name = "AdjustedProfit";           // the field name used by data items
field.Expression = "[Profit] * [Parameters.discountMultiplier]";
dataSource.CalculatedFields.Add(field);

// Use it like any other field:
grid.Columns.Add(new GridMeasureColumn(new Measure("AdjustedProfit")));
```

---

## Using Parameters in Conditional Formatting (Format Rules)

Conditional formatting is expressed as a **format rule** whose condition is a `FormatConditionExpression` with a `string` expression:

```csharp
using DevExpress.DashboardCommon;

var profitMeasure = new Measure("Profit", SummaryType.Sum);
grid.Columns.Add(new GridMeasureColumn(profitMeasure));

var rule = new GridItemFormatRule();
rule.DataItem = profitMeasure;       // the data item the rule is evaluated for
rule.ApplyToRow = true;
rule.Condition = new FormatConditionExpression {
    Expression = "[Profit] > [Parameters.targetProfit]"
};
grid.FormatRules.Add(rule);
```

---

## Multi-Value Parameters

```csharp
using System.Collections.Generic;
using DevExpress.DashboardCommon;

var categoryParam = new DashboardParameter(
    "categoryParameter", typeof(string),
    new List<string> { "Beverages", "Produce" },   // multiple initial values
    "Select Categories:", true,
    new DynamicListLookUpSettings {
        DataSource = dataSource, DataMember = "Categories", ValueMember = "CategoryName"
    });
categoryParam.AllowMultiselect = true;

dashboard.Parameters.Add(categoryParam);

// In a filter string use IN, not =:
grid.FilterString = "[categoryColumn] in (?categoryParameter)";
```

The value is a **list**, so a scalar `=` comparison matches nothing. In a query, filter with the criteria-language `In` form, which accepts the whole list:

```csharp
SelectQuery categoriesQuery = SelectQueryFluentBuilder
    .AddTable("Products")
    .SelectAllColumns()
    .Filter("[Products.CategoryName] In (?categoryQueryParam)")
    .Build("ProductsByCategory");
categoriesQuery.Parameters.Add(new QueryParameter(
    "categoryQueryParam", typeof(Expression), new Expression("?categoryParameter")));
dataSource.Queries.Add(categoriesQuery);
```

A raw `CustomSqlQuery` placeholder binds one scalar, so `WHERE CategoryName IN (@CategoryParam)` matches one value, not the list. Custom SQL and stored procedures are the only targets with this limit; `SelectQuery` filters, data source `Filter`, and item `FilterString` take the list. Either `JOIN` the list into a string and split it in SQL (SQL Server, MySQL, Oracle), or filter above the query.

**`SelectAllValues` is built for static lists.** It pre-selects every value on load, works only with `AllowMultiselect = true`, and on a dynamic look-up is advisable only when the expression using the parameter targets the same data source as the list — never for custom SQL or stored procedures.

At runtime on WinForms, a multi-value parameter is set through `SelectedValues` (not `SelectedValue`) — see [winforms-wpf-runtime.md](winforms-wpf-runtime.md).

---

## Hidden Parameters (Visible = false)

A hidden parameter never appears in the dialog but is still usable in queries, filters, and expressions — the usual carrier for a user id injected server-side:

```csharp
var hiddenParam = new DashboardParameter(
    "internalUserId", typeof(string), string.Empty,
    "User ID (hidden)", false, null);   // visible: false, no look-up
dashboard.Parameters.Add(hiddenParam);

CustomSqlQuery query = new CustomSqlQuery("OrdersQuery",
    "SELECT * FROM Orders WHERE UserId = @UserId");
query.Parameters.Add(new QueryParameter(
    "UserId", typeof(Expression), new Expression("[Parameters.internalUserId]")));
```

Set the value in `CustomParameters`. Hiding the parameter does not protect it — users can still read hidden values.

Hiding a parameter does not stop the Dashboard Parameters dialog from appearing for the *other* parameters — see [web-parameters.md](web-parameters.md) for suppressing the dialog entirely.

---

## Relative Dates ("Always Today", "Start of Month")

A `DashboardParameter`'s value is a **stored literal**, not an expression. Date parameters saved as `01/26/2026` stay `01/26/2026` on every future load.

**The trap:** `Value` is typed `object`, so assigning an `Expression` compiles *and* is accepted at runtime — but it is **silently discarded when the dashboard is saved**:

```csharp
var p = new DashboardParameter("fromDate", typeof(DateTime), DateTime.Today);
p.Value = new Expression("Today()");   // accepted — no exception
dashboard.Parameters.Add(p);
dashboard.SaveToXml(stream);
```

The saved node has no `Value` attribute, so reloading yields the `DateTime` default (displayed as **01-Jan-00**), not today:

```xml
<Parameter Name="fromDate" Type="System.DateTime" />
```

Literal dates round-trip correctly — `DateTime.Today`, `DateTime.Now`, and fixed dates all keep their `Value` attribute. So a date parameter reading 01-Jan-00 on the server is the signature of a `<Parameter>` node with no `Value`; look at the saved XML, not at the code that set it.

Two constructs are easy to confuse here, and only one of them understands relative dates:

| | Relative dates? | Binds to |
|---|---|---|
| **Date Filter dashboard item** | Yes — intervals (Last 3 Months), presets (Month-to-Date), and custom `DateTimePeriod` objects | A **data field** (a dimension) |
| **Date parameter** | No — a stored literal | Queries, filter strings, calculated fields, conditional formatting |

The Date Filter item binds to a dimension, **not to a parameter**, so it cannot drive a dashboard parameter and a calculated field will not bridge the two. If a stored procedure needs the date range, that range must come from parameters.

To get a date parameter that tracks the current date, compute the value at load time:

```csharp
using System;
using System.Linq;
using DevExpress.DashboardWeb;      // CustomParametersWebEventArgs

// ASP.NET Core / Blazor — recomputed before each data request.
// e.Parameters holds DevExpress.Data.IParameter, so use var for the element.
configurator.CustomParameters += (object sender, CustomParametersWebEventArgs e) => {
    var fromDate = e.Parameters.FirstOrDefault(p => p.Name == "fromDate");
    if (fromDate != null)
        fromDate.Value = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);  // start of month

    var toDate = e.Parameters.FirstOrDefault(p => p.Name == "toDate");
    if (toDate != null)
        toDate.Value = DateTime.Today;
};
```

On the desktop, apply the same values through `SetInitialDashboardState`, or through `CustomParameters` to recompute before every query — see [winforms-wpf-runtime.md](winforms-wpf-runtime.md).

Trade-off: `CustomParameters` changes the value the **queries** receive, not what the Dashboard Parameters dialog displays. When the displayed value must match, set dashboard state as well.

---

## Common Issues & Solutions

| Issue | Cause | Fix |
|-------|-------|-----|
| Date parameter keeps a stale date instead of tracking today | Parameter defaults are stored literals | Compute the value in `CustomParameters` / `SetInitialDashboardState` (see above) |
| `CS0246: 'Expression' could not be found` | Wrong namespace | `using DevExpress.DataAccess;` |
| `CS7036: no argument given for 'lookupsettings'` | 5-argument constructor call | Pass `null` as the sixth argument |
| `CS1739: no parameter named 'defaultValue'` | Wrong named arguments | `value:`, `description:`, `lookupsettings:` |
| `CS0029: cannot convert 'int[]' to 'string[]'` | Non-string static values | `StaticListLookUpSettings.Values` is `string[]` |
| `CS1729: GridDashboardItem does not contain a constructor that takes 1 arguments` | `new GridDashboardItem("name")` | Use `new GridDashboardItem()` and set `ComponentName` |
| `IndexOutOfRangeException` on `grid.Columns[0]` | The item was just created and has no columns | Add columns before indexing them |
| Filter string doesn't work | Identifier is a database field, or `UniqueId` unset | Assign `UniqueId` and reference that |
| SQL query ignores the parameter | Missing placeholder or missing `QueryParameter` | Both are required (Option 3) |
| Parameter shows in the dialog but filters nothing | Not bound anywhere | Bind it to a filter string or a query parameter |

---

## Related References

- [winforms-wpf-runtime.md](winforms-wpf-runtime.md) — setting values on the desktop
- [web-parameters.md](web-parameters.md) — setting values in ASP.NET Core and JavaScript
- [parameter-export.md](parameter-export.md) — exporting with the current parameter state

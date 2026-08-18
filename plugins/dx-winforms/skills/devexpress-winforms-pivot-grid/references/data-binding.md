# Data Binding — WinForms PivotGridControl (DevExpress v26.1)

## Overview

The `PivotGridControl` supports two binding layers:

1. **Control-level** — assign a data source via `DataSource` / `DataMember`.
2. **Field-level** — each `PivotGridField` has a `DataBinding` property that determines what data it visualizes.

---

## Supported Data Sources

| Source type | Notes |
|---|---|
| `IList` / `IEnumerable` (e.g. `List<T>`) | In-memory, Optimized engine |
| `DataTable` / `DataSet` | Use `DataMember` to select the table |
| `BindingSource` | Wraps any of the above |
| Entity Framework / EF Core | `DbContext.DbSet<T>` as `IQueryable` (loads into memory) |
| EF Core / LINQ **Server Mode** | `EntityServerModeSource` / `LinqServerModeSource` — server-side aggregation for large data (see *Server Mode* below) |
| `ExcelDataSource` | `DevExpress.DataAccess.Excel` |
| LINQ to SQL | `DataContext.Table<T>` |
| XPO `XPCollection` | DevExpress ORM |
| OLAP Cube (SSAS / XML/A) | Set via `OLAPConnectionString` instead of `DataSource` |

---

## Assigning the Data Source

```csharp
// In-memory list
pivotGridControl1.DataSource = salesList;      // List<SaleRecord>

// DataSet with multiple tables
pivotGridControl1.DataSource = myDataSet;
pivotGridControl1.DataMember  = "Orders";

// BindingSource
bindingSource1.DataSource = myList;
pivotGridControl1.DataSource = bindingSource1;
```

---

## Field Data Binding (DataSourceColumnBinding)

`DataSourceColumnBinding` maps a field to a column name in the data source:

```csharp
var field = new PivotGridField
{
    Caption   = "Category",
    Area      = PivotArea.RowArea,
    AreaIndex = 0
};
field.DataBinding = new DataSourceColumnBinding("CategoryName");
pivotGridControl1.Fields.Add(field);
```

### With GroupInterval (date/numeric/alphabetical splitting)

```csharp
var fieldYear = new PivotGridField
{
    Caption   = "Year",
    Area      = PivotArea.ColumnArea,
    AreaIndex = 0
};
fieldYear.DataBinding = new DataSourceColumnBinding("OrderDate")
{
    GroupInterval = PivotGroupInterval.DateYear
};

var fieldQuarter = new PivotGridField
{
    Caption   = "Quarter",
    Area      = PivotArea.ColumnArea,
    AreaIndex = 1
};
fieldQuarter.DataBinding = new DataSourceColumnBinding("OrderDate")
{
    GroupInterval = PivotGroupInterval.DateQuarter
};

pivotGridControl1.Fields.AddRange(new[] { fieldYear, fieldQuarter });
```

### Common GroupInterval values

| Value | Splits by |
|---|---|
| `DateYear` | Year |
| `DateQuarter` | Quarter (Q1–Q4) |
| `DateMonth` | Month (1–12) |
| `DateWeekOfYear` | Week number |
| `DateDayOfWeek` | Day of week |
| `DateDay` | Day of month |
| `Alphabetical` | First letter |
| `Numeric` | Numeric ranges — set the bucket width via the binding's `GroupIntervalNumericRange` (default `10`) |
| `DayAge` | Age in days from today |
| `Default` | No grouping (raw values) |

---

## AddDataSourceColumn Shorthand

For quick setup without creating fields explicitly:

```csharp
// Returns the created PivotGridField
PivotGridField f = pivotGridControl1.Fields.AddDataSourceColumn("Sales", PivotArea.DataArea);
f.Caption = "Total Sales";
```

---

## Calculated Bindings (Optimized Mode only)

Use `ExpressionDataBinding` to define a field via an expression over other columns:

```csharp
var fieldProfit = new PivotGridField
{
    Caption   = "Profit",
    Area      = PivotArea.DataArea,
    AreaIndex = 1
};
// Criterion-style expression:
fieldProfit.DataBinding = new ExpressionDataBinding("[Revenue] - [Cost]");
pivotGridControl1.Fields.Add(fieldProfit);
```

Requires `OptionsData.DataProcessingEngine = PivotDataProcessingEngine.Optimized`.

---

## Window Calculation Bindings (Optimized Mode only)

Use instead of raw `DataSourceColumnBinding` to display relative values:

```csharp
// Show percent of column grand total
var fieldPct = new PivotGridField
{
    Caption   = "% of Grand Total",
    Area      = PivotArea.DataArea,
    AreaIndex = 1
};
// Source is a DataBindingBase (e.g. a DataSourceColumnBinding), NOT a field.
// The second arg is a CalculationPartitioningCriteria that defines the window.
fieldPct.DataBinding = new PercentOfTotalBinding(
    new DataSourceColumnBinding("Sales"),               // source data binding
    CalculationPartitioningCriteria.ColumnValue         // window = same column value
);
pivotGridControl1.Fields.Add(fieldPct);
```

Other binding classes: `RunningTotalBinding`, `DifferenceBinding`, `RankBinding`, `MovingCalculationBinding`.

---

## OLAP Cube Binding

Bind to a Microsoft Analysis Services cube:

```csharp
// Use the OLAPConnectionString property instead of DataSource
pivotGridControl1.OLAPConnectionString =
    "provider=MSOLAP;data source=localhost;" +
    "initial catalog=AdventureWorksDW;cube name=Adventure Works;";

// Fields use OLAP member paths in DataSourceColumnBinding
var fieldCountry = new PivotGridField
{
    Caption   = "Country",
    Area      = PivotArea.RowArea,
    AreaIndex = 0
};
fieldCountry.DataBinding = new DataSourceColumnBinding("[Customer].[Country].[Country]");

// Calculated measure in OLAP with MDX expression
var fieldCalc = new PivotGridField
{
    Caption   = "EUR Sales",
    Area      = PivotArea.DataArea,
    AreaIndex = 1
};
fieldCalc.DataBinding = new OLAPExpressionBinding(
    "[Measures].[Internet Sales Amount] * 0.87"
);
```

---

## Server Mode (Large Data Sets)

For large databases, prefer **server mode** over loading the whole `DbSet`/`IQueryable` into memory: the `PivotGridControl` pushes grouping and aggregation down to the database, so only summary results are fetched. This is the scalable alternative to the in-memory Optimized engine (`OptionsData.DataProcessingEngine = PivotDataProcessingEngine.Optimized`, optionally with `OptionsBehavior.UseAsyncMode = true`) — use the engine/async options for in-memory data, and server mode when the data lives in a database.

Bind through a server-mode source instead of assigning the `DbSet` directly:

```csharp
using DevExpress.Data.Linq;   // EntityServerModeSource

var serverSource = new EntityServerModeSource {
    ElementType     = typeof(ClothingProduct),     // the entity type
    QueryableSource = context.ClothingProduct,     // the EF Core DbSet / IQueryable
    KeyExpression   = "Id"                          // unique key field
};
pivotGridControl1.DataSource = serverSource;        // the grid now runs queries server-side
```

- **EF / EF Core** → `EntityServerModeSource` (namespace `DevExpress.Data.Linq`).
- **LINQ to SQL** → `LinqServerModeSource`.
- The grid automatically operates in server mode once bound to a server-mode source. Field `DataBinding`, areas, grouping, and summaries work the same; aggregation runs in SQL.

> When asked to handle a large data set, recommend server mode (`EntityServerModeSource`) for database-backed data — not only the in-memory `Optimized` engine + `UseAsyncMode`.

---

## Full Code Example (List<T>)

```csharp
using DevExpress.XtraEditors;
using DevExpress.XtraPivotGrid;

public partial class Form1 : XtraForm
{
    public Form1()
    {
        InitializeComponent();

        pivotGridControl1.OptionsData.DataProcessingEngine =
            PivotDataProcessingEngine.Optimized;
        pivotGridControl1.DataSource = SaleRepository.GetAll();

        pivotGridControl1.BeginUpdate();

        // Row: Category hierarchy
        var fCat = new PivotGridField { Caption = "Category", Area = PivotArea.RowArea, AreaIndex = 0 };
        fCat.DataBinding = new DataSourceColumnBinding("Category");

        // Column: Year / Quarter
        var fYear = new PivotGridField { Caption = "Year", Area = PivotArea.ColumnArea, AreaIndex = 0 };
        fYear.DataBinding = new DataSourceColumnBinding("OrderDate") { GroupInterval = PivotGroupInterval.DateYear };

        var fQtr = new PivotGridField { Caption = "Qtr", Area = PivotArea.ColumnArea, AreaIndex = 1 };
        fQtr.DataBinding = new DataSourceColumnBinding("OrderDate") { GroupInterval = PivotGroupInterval.DateQuarter };

        // Data: Sales
        var fSales = new PivotGridField { Caption = "Sales", Area = PivotArea.DataArea, AreaIndex = 0 };
        fSales.DataBinding = new DataSourceColumnBinding("Amount");
        fSales.CellFormat.FormatType   = DevExpress.Utils.FormatType.Numeric;
        fSales.CellFormat.FormatString = "c2";

        pivotGridControl1.Fields.AddRange(new[] { fCat, fYear, fQtr, fSales });
        pivotGridControl1.EndUpdate();
    }
}
```

---

## Key API Reference

| Member | Description |
|---|---|
| `PivotGridControl.DataSource` | Control-level data source |
| `PivotGridControl.DataMember` | Table/member name within the source |
| `PivotGridControl.OLAPConnectionString` | OLAP cube connection |
| `PivotGridField.DataBinding` | Field-level binding (`DataSourceColumnBinding`, `ExpressionDataBinding`, `OLAPExpressionBinding`, …) |
| `DataSourceColumnBinding(columnName)` | Bind to a data source column |
| `DataSourceColumnBinding.GroupInterval` | Date/numeric/alphabetical split (use this in Optimized/Server mode) |
| `DataSourceColumnBinding.GroupIntervalNumericRange` | Bucket width when `GroupInterval = Numeric` (or an age interval); default `10` |
| `ExpressionDataBinding(expression)` | Calculated field using a criterion expression |
| `OLAPExpressionBinding(mdx)` | MDX-based calculated OLAP measure |
| `PercentOfTotalBinding` / `RunningTotalBinding` / … | Window calculation bindings |
| `PivotGroupInterval` enum | All available group intervals |
| `PivotGridField.CellFormat` | `FormatInfo` object — format type and format string |

---

## Source

- Data Binding overview: https://docs.devexpress.com/content/WindowsForms/1842?md=true
- Bind to Data Columns: https://docs.devexpress.com/content/WindowsForms/401376?md=true
- Bind to Calculated Expressions: https://docs.devexpress.com/content/WindowsForms/1799?md=true
- Window Calculations: https://docs.devexpress.com/content/WindowsForms/403904?md=true
- OLAP binding: https://docs.devexpress.com/content/WindowsForms/12006?md=true

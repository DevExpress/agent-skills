# Data Binding — DevExpress WPF Pivot Grid

Unlike `GridControl` (which uses `ItemsSource`), `PivotGridControl` binds via the **`DataSource`** property. The control supports four processing modes — **In-Memory (Optimized)**, **Server Mode**, **OLAP**, and **Asynchronous** — chosen based on data volume and source type.

## When to Use This Reference

Use this when you need to:

- Bind to a `DataTable` / `DataSet` (ADO.NET)
- Bind to in-memory collections (`List<T>`, `IEnumerable<T>`, `ObservableCollection<T>`)
- Bind to Entity Framework / Entity Framework Core
- Bind to a Microsoft Analysis Services OLAP cube
- Use Server Mode for very large data sets
- Use Asynchronous Mode for background-thread aggregation
- Use the Items Source Configuration Wizard at design time

## Key Properties

| Property | Type | Description |
|---|---|---|
| `PivotGridControl.DataSource` | `object` | The bound data source. Accepts `DataTable`, `IListSource`, `IEnumerable`, server-mode source, OLAP source. |
| `PivotGridControl.DataProcessingEngine` | `DataProcessingEngine` | `Optimized` (default, recommended) or `Legacy`. |
| `PivotGridField.DataBinding` | `DataBinding` | `DataSourceColumnBinding`, `ExpressionDataBinding`, or one of the window-calculation bindings (`RunningTotalBinding`, `DifferenceBinding`, `RankBinding`, `PercentOfTotalBinding`, `MovingCalculationBinding`, `WindowExpressionBinding`). |
| `DataSourceColumnBinding.ColumnName` | `string` | Name of the data source column. |
| `DataSourceColumnBinding.GroupInterval` | `FieldGroupInterval` | Grouping at the data layer (date interval, alphabetical, numeric range). |

## In-Memory (Optimized Engine)

Default for `DataTable` and `List<T>`. Aggregation happens in-process; fast up to ~1M rows.

```csharp
public class Sale {
    public string Country { get; set; } = "";
    public DateTime OrderDate { get; set; }
    public decimal Amount { get; set; }
}

pivotGridControl1.DataProcessingEngine = DataProcessingEngine.Optimized;
pivotGridControl1.DataSource = sales;   // List<Sale>
```

Source: `articles/controls-and-libraries/pivot-grid/binding-to-data.md` (root) and `xref:404337` (In-Memory Mode).

## ADO.NET — DataTable from OLE DB / SQL

```csharp
using System.Data;
using System.Data.OleDb;

var conn = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=NWIND.MDB");
var adapter = new OleDbDataAdapter("SELECT * FROM SalesPerson", conn);
var ds = new DataSet();
adapter.Fill(ds, "SalesPerson");

pivotGridControl1.DataSource = ds.Tables["SalesPerson"];
```

For SQL Server / other providers, swap `OleDbConnection` / `OleDbDataAdapter` for the provider-specific classes (`SqlConnection`, `SqlDataAdapter`, etc.). The pattern is identical.

Source: `articles/controls-and-libraries/pivot-grid/getting-started/NET-Core/lesson-1-bind-a-pivot-grid-to-an-mdb-database-net.md`.

## Entity Framework Core

```csharp
using Microsoft.EntityFrameworkCore;

using var db = new NorthwindContext();
db.Sales.Load();
pivotGridControl1.DataSource = db.Sales.Local.ToObservableCollection();
```

For larger sets, prefer Server Mode (below) over loading all rows into memory.

## Server Mode (Very Large Data)

When the underlying data set has many millions of rows, push aggregation to the server. Use one of the server-mode source classes (the same family as for `GridControl`):

| Data Access Technology | Server Mode Source | Instant-Feedback Source |
|---|---|---|
| Entity Framework 4+ / Core | `EntityServerModeSource` (`DevExpress.Data.Linq`) | `EntityInstantFeedbackSource` |
| LINQ to SQL | `LinqServerModeSource` | `LinqInstantFeedbackSource` |
| eXpress Persistent Objects | `XPServerCollectionSource` (`DevExpress.Xpo`) | `XPInstantFeedbackSource` |
| OData v4 | `ODataServerModeSource` | `ODataInstantFeedbackSource` |

```csharp
using DevExpress.Data.Linq;

var source = new EntityServerModeSource {
    ElementType = typeof(Sale),
    QueryableSource = db.Sales,
    KeyExpression = "Id",
};

pivotGridControl1.DataSource = source;
```

The control queries the server for aggregated cell values — only the cells visible in the pivot are computed.

Source: `articles/controls-and-libraries/pivot-grid.md` § Binding to Data → Server Mode (`xref:18010`).

## OLAP (Microsoft Analysis Services Cube)

Use OLAP mode when data is already modeled as a cube on Microsoft Analysis Services. The Pivot Grid delegates all data operations (summarization, grouping, sorting, filtering) to the OLAP server.

### Setup

```csharp
// 1. Specify the OLAP data provider
pivotGridControl1.OlapDataProvider = OlapDataProvider.Adomd;

// 2. Specify connection settings
pivotGridControl1.OlapConnectionString =
    "Provider=msolap;Data Source=localhost;" +
    "Initial Catalog=Adventure Works DW;" +
    "Cube Name=Adventure Works;Query Timeout=100;";
```

Required connection string parameters: **Provider**, **Data Source**, **Initial Catalog**, **Cube Name**.

### Bind Fields to Cube Members

Use `DataSourceColumnBinding.ColumnName` with the **full name** of the cube measure or dimension:

```csharp
var countryField = pivotGridControl1.Fields.Add();
countryField.Caption = "Country";
countryField.Area = FieldArea.RowArea;
countryField.DataBinding = new DataSourceColumnBinding(
    "[Customer].[Customer Geography].[Country]");

var salesField = pivotGridControl1.Fields.Add();
salesField.Caption = "Sales";
salesField.Area = FieldArea.DataArea;
salesField.DataBinding = new DataSourceColumnBinding(
    "[Measures].[Sales Amount]");
```

**Name format**:
- **Dimensions**: `[DimensionName].[HierarchyName].[LevelName]` — bracketed and dot-separated.
- **Measures**: `[Measures].[MeasureName]`.

### Discover Available Fields

```csharp
var fieldList = pivotGridControl1.GetFieldList();   // List of available cube members
pivotGridControl1.RetrieveFields();                  // Auto-create PivotGridField objects for all available fields
```

### .NET 5+ Limitation

> **You cannot bind the Pivot Grid to data at design time in .NET 5+ projects.** Use code-only setup (above) on modern .NET. Design-time wizards work only on .NET Framework. See `articles/controls-and-libraries/pivot-grid/getting-started/NET-Framework/lesson-2-bind-a-pivot-grid-to-an-olap-cube.md` for .NET Framework design-time path.

Source: `articles/controls-and-libraries/pivot-grid/binding-to-data/olap-data-source/binding-to-olap-data-sources.md` (`xref:8015`).

## Asynchronous Mode

For large datasets, perform data retrieval, sorting, grouping, filtering, and summary calculation on a **background thread** so the rest of the application stays responsive while the Pivot Grid processes.

### Enable Async for End-User Operations

```xaml
<dxpg:PivotGridControl UseAsyncMode="True"/>
```

With `UseAsyncMode="True"`, all data operations triggered by UI actions (filter dropdown, drag a field, sort) run asynchronously. The control disables its UI and shows a loading panel until the operation completes; the rest of the app stays responsive.

### Async in Code

Methods ending in `Async` run asynchronously regardless of the `UseAsyncMode` setting:

```csharp
pivotGridControl1.BeginUpdate();
try {
    AddField("Country", FieldArea.RowArea, "Country");
    AddField("Year", FieldArea.ColumnArea, "OrderDate",
             interval: FieldGroupInterval.DateYear);
    AddField("Sales", FieldArea.DataArea, "Amount");
} finally {
    await pivotGridControl1.EndUpdateAsync();    // Apply changes asynchronously
}
```

> Always wrap `BeginUpdate` / `EndUpdateAsync` in `try` / `finally` so the async update fires even if an exception occurs.

### Async Lifecycle Events

| Event | When |
|---|---|
| `PivotGridControl.AsyncOperationStarting` | Operation begins on background thread |
| `PivotGridControl.AsyncOperationCompleted` | Operation completed |

```csharp
pivotGridControl1.AsyncOperationStarting += (s, e) => statusBar.Text = "Calculating...";
pivotGridControl1.AsyncOperationCompleted += (s, e) => statusBar.Text = "Ready";
```

### Concurrency Constraint

> You cannot start a data-aware operation in code while another operation is running in a background thread. The Pivot Grid blocks API access while async operations are in progress.

### Summary Event Handling in Async Mode

In async mode, summary-calculation events (`CustomCellValue`, `CustomSummary`, etc.) fire on the **background thread**, not the UI thread. Do NOT touch UI elements from these handlers — use `Dispatcher.Invoke` if you must.

Source: `articles/controls-and-libraries/pivot-grid/binding-to-data/asynchronous-mode.md` (`xref:9776`).

## Items Source Configuration Wizard (Design Time)

The Quick Actions smart tag on `PivotGridControl` opens the **Items Source Configuration Wizard**, which scaffolds binding code for:

- ADO.NET DataSets (`Connect to Database`)
- SQL databases (any ADO.NET provider)
- OLAP cubes
- Microsoft Excel files
- LINQ to SQL
- Entity Framework
- OData

The wizard generates the connection plumbing + field setup. Use this path when you prefer designer-driven scaffolding.

Source: `articles/controls-and-libraries/pivot-grid.md` references the wizard at `xref:115629`.

## Calculated Expression Fields

A field can be bound to a calculated expression instead of a raw column:

```csharp
var profitField = pivotGridControl1.Fields.Add();
profitField.Caption = "Profit";
profitField.Area = FieldArea.DataArea;
profitField.DataBinding = new ExpressionDataBinding { Expression = "[Revenue] - [Cost]" };
```

The expression uses [Criteria Language Syntax](https://docs.microsoft.com/) — same as `GridControl` filters. Field names go in square brackets.

Source: `articles/controls-and-libraries/pivot-grid/fundamentals/fields.md` § Bind Pivot Grid Fields to Data → Calculated Expressions (`xref:8025`).

## Window Calculations

Window calculations (running totals, percent-of-parent, rank, moving averages, arbitrary window expressions) use one of the window-calculation binding classes — `RunningTotalBinding`, `DifferenceBinding`, `RankBinding`, `PercentOfTotalBinding`, `MovingCalculationBinding`, `WindowExpressionBinding`. All inherit from `DirectedCalculationBinding`. See [data-shaping.md](data-shaping.md) § Window Calculations and `xref:403913` in the docs.

## Choosing the Right Mode

| Data shape | Mode | DataSource type |
|---|---|---|
| `< 10K rows`, POCO collection | In-Memory / Optimized | `List<T>` or `ObservableCollection<T>` |
| `< 1M rows`, `DataTable` | In-Memory / Optimized | `DataTable` |
| `> 1M rows`, EF / SQL backend | Server Mode | `EntityServerModeSource`, `LinqServerModeSource` |
| Already in an Analysis Services cube | OLAP | OLAP connection string |
| Slow data fetch, UI must stay responsive | Asynchronous | (asynchronous source class) |

## Common Issues

- **Pivot is empty after `DataSource = ...`** — fields haven't been added yet. `DataSource` alone doesn't show data; you must also add at least one field to each of Row, Column, Data areas.
- **`OleDbConnection` throws "provider is not registered"** — install the Microsoft Access Database Engine Redistributable (x86 / x64 must match the process architecture).
- **Server-mode source is slow** — check the underlying query plan and indexes. The grid only requests aggregated values for visible cells, but the server must compute them efficiently.

## Source Material

- `articles/controls-and-libraries/pivot-grid.md` (root, sections § Binding to Data)
- `articles/controls-and-libraries/pivot-grid/binding-to-data.md`
- `articles/controls-and-libraries/pivot-grid/getting-started/NET-Core/lesson-1-bind-a-pivot-grid-to-an-mdb-database-net.md`
- `articles/controls-and-libraries/pivot-grid/getting-started/NET-Core/lesson-2-bind-a-pivot-grid-to-an-olap-cube-net.md`
- `articles/controls-and-libraries/pivot-grid/fundamentals/fields.md`

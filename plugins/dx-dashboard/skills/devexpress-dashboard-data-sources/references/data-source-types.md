# Data Source Types — DevExpress Dashboard

## When to Use This Reference

Use this when you need code examples for a specific data source type. Registration in the examples uses the ASP.NET Core pattern (`DashboardConfigurator` as a scoped service); desktop-specific notes are marked inline where they differ. For the desktop (WinForms/WPF) registration pattern, see [winforms-wpf-data-sources.md](winforms-wpf-data-sources.md).

The general pattern for every type is:

```csharp
// 1. Create the data source instance
DashboardXxxDataSource ds = new DashboardXxxDataSource("Display Name", ...);

// 2. Configure it (type-specific properties and queries)

// 3. Register in storage — "myId" is the STORAGE id the dashboard XML refers to.
//    It is NOT the same string as DashboardObjectDataSource.DataId (matched in DataLoading)
//    or ConnectionName (matched in ConfigureDataConnection).
DataSourceInMemoryStorage storage = new DataSourceInMemoryStorage();
storage.RegisterDataSource("myId", ds.SaveToXml());

// 4. Set storage on configurator
configurator.SetDataSourceStorage(storage);
```

---

## SQL Data Source (`DashboardSqlDataSource`)

**Prerequisite (.NET 8+)**: the ADO.NET driver for the database behind the connection string — `Microsoft.Data.SqlClient` for `XpoProvider=MSSqlServer`, `Microsoft.Data.Sqlite` for the `XpoProvider=SQLite` connections in the samples below. XPO loads it by reflection on the first data request, so a missing package builds cleanly and fails at runtime. Read the `.csproj` and add it before generating code (Rule 5). Connection string format per provider: https://docs.devexpress.com/content/CoreLibraries/403639/devexpress-data-library/data-sources/use-the-sql-data-source/connection-strings-for-xpo-providers?md=true

### Using SelectQueryFluentBuilder

```csharp
using DevExpress.DashboardCommon;
using DevExpress.DataAccess.Sql;

// 2nd argument = a ConnectionStrings key from appsettings.json ( "NWindConnectionString" in this sample — use the project's own)
DashboardSqlDataSource sqlDataSource = new DashboardSqlDataSource("SQL Data Source", "NWindConnectionString");
// Optional: Server is the default and keeps aggregation in the database. Client pulls the
// whole query result into memory — uncomment only if you need client-side processing.
// sqlDataSource.DataProcessingMode = DataProcessingMode.Client;

SelectQuery query = SelectQueryFluentBuilder
    .AddTable("Categories")
    .Join("Products", "CategoryID")
    .SelectAllColumnsFromTable()
    .Build("Products_Categories");
sqlDataSource.Queries.Add(query);

dataSourceStorage.RegisterDataSource("sqlDataSource", sqlDataSource.SaveToXml());
```

### SelectQueryFluentBuilder — Join Types

The default join is `Inner`. Use the overload that takes `SqlJoinType` for other join types:

```csharp
using DevExpress.DataAccess.Sql;

// Left outer join
SelectQuery query = SelectQueryFluentBuilder
    .AddTable("Categories")
    .Join("Products", SqlJoinType.LeftOuter, "CategoryID")
    .SelectAllColumnsFromTable()
    .Build("CategoriesWithProducts");

// Join where parent and child column names differ
SelectQuery query2 = SelectQueryFluentBuilder
    .AddTable("Orders")
    .Join("Customers", SqlJoinType.Inner,
        parentColumnName: "CustomerID",
        nestedColumnName: "ID")      // different name in the joined table
    .SelectAllColumnsFromTable()
    .Build("OrdersWithCustomers");

// Multi-column join condition
SelectQuery query3 = SelectQueryFluentBuilder
    .AddTable("Employees")
    .Join("Territories", SqlJoinType.Inner,
        keyColumns: new[] {
            new RelationColumnInfo("City", "City"),
            new RelationColumnInfo("Region", "Region")
        })
    .SelectAllColumnsFromTable()
    .Build("EmployeeTerritories");
```

`SqlJoinType` values: `Inner`, `LeftOuter`, `RightOuter`, `FullOuter`

### Stored Procedure Query (`StoredProcQuery`)

```csharp
using DevExpress.DashboardCommon;
using DevExpress.DataAccess.Sql;

// 2nd argument = a ConnectionStrings key/name from appsettings.json / app.config ( "NWindConnectionString" in this sample — use the project's own)
DashboardSqlDataSource sqlDataSource = new DashboardSqlDataSource("SQL Data Source", "NWindConnectionString");

StoredProcQuery spQuery = new StoredProcQuery {
    Name = "CustOrdersDetail",
    StoredProcName = "CustOrdersDetail"
};
spQuery.Parameters.Add(new QueryParameter("@OrderID", typeof(int), 10248));
sqlDataSource.Queries.Add(spQuery);

dataSourceStorage.RegisterDataSource("sqlDataSource", sqlDataSource.SaveToXml());
```

`Name` is the query display name used within the dashboard; `StoredProcName` is the actual stored procedure name in the database.

### Using CustomSqlQuery (raw SQL string)

Custom SQL execution is **disabled by default** on the web. Enable it on the configurator (ASP.NET Core / MVC):

```csharp
using DevExpress.DataAccess.Sql;

// SECURITY GATE (web): allow the server to execute custom SQL queries
configurator.AllowExecutingCustomSql = true;

DashboardSqlDataSource sqlDataSource = new DashboardSqlDataSource("SQL Data Source", "NWindConnectionString");
CustomSqlQuery customQuery = new CustomSqlQuery {
    Name = "CustomQuery",
    Sql = "SELECT CategoryName, ProductName, UnitPrice FROM Products INNER JOIN Categories ON Products.CategoryID = Categories.CategoryID"
};
sqlDataSource.Queries.Add(customQuery);

dataSourceStorage.RegisterDataSource("sqlDataSource", sqlDataSource.SaveToXml());
```

To additionally let end users **type** SQL in the Data Source Wizard, enable the wizard option (ASP.NET Core Razor builder): `.Extensions(ext => ext.DataSourceWizard(w => w.EnableCustomSql(true)))`. On the desktop (WinForms/WPF), the equivalent wizard-level gate is `SqlWizardSettings.EnableCustomSql` (`DevExpress.DataAccess.UI.Wizard`).

> **Two different gates — do not confuse them.**
>
> | Member | Kind | Default | Applies to |
> |---|---|---|---|
> | `DashboardConfigurator.AllowExecutingCustomSql` | member of the configurator | `false` | web only |
> | `SqlDataSource.AllowCustomSqlQueries` | **static** | `true` | all platforms — already permissive; you would only set it to *disable* custom SQL |
>
> Since `AllowCustomSqlQueries` is static, write it on the type (`SqlDataSource.AllowCustomSqlQueries = false;`). Writing it through an instance (`sqlDataSource.AllowCustomSqlQueries`) is a compile error, CS0176.

> Leave `SqlDataSource.DisableCustomQueryValidation` at its default (`false`) to retain the built-in validation: only SELECT statements are accepted. Do not emit a line setting it.

### Custom SQL Validation (`ValidateCustomSqlQuery` event)

The built-in validator rejects any custom query that is not purely SELECT (note: this validation became stricter in recent versions — previously-working queries with comments or extra statements can start failing after an upgrade). The `ValidateCustomSqlQuery` event fires on every SQL request and lets you implement your own validation logic:

```csharp
configurator.ValidateCustomSqlQuery += (object sender, ValidateDashboardCustomSqlQueryWebEventArgs e) => {
    // e.DataSourceName          — display name of the data source
    // e.DataSourceComponentName — component name of the data source
    // e.CustomSqlQuery          — the query being validated
    // e.Valid                   — set by default validation; override to accept/reject
    // e.ExceptionMessage        — message shown when invalid
    if (!e.Valid && e.CustomSqlQuery.Sql.Contains("--")) {
        // example: custom handling for queries the default validator rejected
        e.ExceptionMessage = "Remove SQL comments from the query.";
    }
};
```

Default validation only runs while `SqlDataSource.DisableCustomQueryValidation` is `false`. The static `SqlDataSource.ValidateCustomSqlQueryGlobal` event provides the same hook outside the web configurator.

### Restricting Visible Tables/Views/Procedures (`IDBSchemaProviderEx`)

To limit which tables, views, and stored procedures end users can see in the Data Source Wizard (independent of `AllowExecutingCustomSql`), implement `IDBSchemaProviderEx` and register it with `SetDBSchemaProvider`. Wrap the built-in `DBSchemaProviderEx` and filter its results — do not reimplement schema loading from scratch:

```csharp
using System.Linq;
using DevExpress.DataAccess.Sql;
using DevExpress.Xpo.DB;

public class LimitDBSchemaProvider : IDBSchemaProviderEx {
    private readonly DBSchemaProviderEx provider = new DBSchemaProviderEx();

    public DBTable[] GetTables(SqlDataConnection connection, params string[] tableList) {
        return provider.GetTables(connection, tableList).Where(t => t.Name.StartsWith("C")).ToArray();
    }

    public DBTable[] GetViews(SqlDataConnection connection, params string[] viewList) {
        return provider.GetViews(connection, viewList).Where(v => v.Name.StartsWith("Sales")).ToArray();
    }

    public DBStoredProcedure[] GetProcedures(SqlDataConnection connection, params string[] procedureList) {
        return provider.GetProcedures(connection, procedureList).Where(p => p.Arguments.Count == 0).ToArray();
    }

    public void LoadColumns(SqlDataConnection connection, params DBTable[] tables) {
        provider.LoadColumns(connection, tables);
    }
}
```

Register the provider — the API differs by platform:

```csharp
// ASP.NET Core — inside the DashboardConfigurator factory in Program.cs
configurator.SetDBSchemaProvider(new LimitDBSchemaProvider());

// WinForms Designer
dashboardDesigner1.CustomDBSchemaProviderEx = new LimitDBSchemaProvider();

// WinForms Viewer
dashboardViewer1.CustomDBSchemaProviderEx = new LimitDBSchemaProvider();
```

> **WPF**: `DashboardControl` is a viewer-only control — it has no Data Source Wizard and does not expose `CustomDBSchemaProviderEx`. Schema restriction applies only when using the WinForms designer or the web wizard.

This restricts what appears in the wizard's table/view/procedure picker; it does not by itself restrict what a `CustomSqlQuery` can execute — combine with `AllowExecutingCustomSql`/`ValidateCustomSqlQuery` above if raw SQL is also enabled.

### Connection string in appsettings.json

```json
{
  "ConnectionStrings": {
    "NWindConnectionString": "XpoProvider=SQLite;Data Source=Data/nwind.db"
  }
}
```

The "NWindConnectionString" JSON key/name is exactly the string passed as the data source's second constructor argument. Pass connection string keys/names to the configurator via `DashboardConnectionStringsProvider` (see [connection-strings.md](connection-strings.md)).

---

## Object Data Source (`DashboardObjectDataSource`)

Use for in-memory .NET objects (List, IEnumerable, DataTable). Data is supplied at runtime via the `DataLoading` event.

```csharp
using DevExpress.DashboardCommon;
using DevExpress.DashboardWeb;

// Create data source and set a unique ID
DashboardObjectDataSource objDataSource = new DashboardObjectDataSource("Object Data Source");
objDataSource.DataId = "objectDataSource";

// Subscribe to DataLoading to provide data at runtime
configurator.DataLoading += (object sender, DataLoadingWebEventArgs e) => {
    if (e.DataId == "objectDataSource") {
        e.Data = Invoices.CreateData(); // returns List<Invoices> or any IEnumerable
    }
};

dataSourceStorage.RegisterDataSource("objDataSource", objDataSource.SaveToXml());
```

Supplying data this way serializes nothing about your class, so no type trust is involved (Rule 9). Keep giving the source its data — `DataLoading` on web, `DataSource = MyStore.All` on desktop. Pointing `DataSource` at a `Type` instead makes DevExpress resolve `DataMember` on that type, and `DataMember` must name a property or a method — `static` is fine, a field is not. Name a field and the lookup throws "missing member", leaving the item with no data.

A source that does hold a `Type` stores the type name in its XML, where Safe Deserialization resolves it. Trust that type at startup — `DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(Order))`, on web as well as desktop — or the item stays empty. You meet this in a dashboard XML that already carries `<DataSource Type="…">`; it is not a route to pick.

---

## Entity Framework Data Source (`DashboardEFDataSource`)

Supports Entity Framework 5+ and EF Core 1+.

```csharp
using DevExpress.DashboardCommon;
using DevExpress.DataAccess.EntityFramework;

DashboardEFDataSource efDataSource = new DashboardEFDataSource("EF Data Source");
efDataSource.ConnectionParameters = new EFConnectionParameters(typeof(OrdersContext));

dataSourceStorage.RegisterDataSource("efDataSource", efDataSource.SaveToXml());
```

`OrdersContext` is a class that inherits `DbContext` from Microsoft.EntityFrameworkCore.

### Resolving the `DbContext` from ASP.NET Core DI (`IEFContextProvider`)

The pattern above lets the dashboard construct `OrdersContext` itself. To resolve the context from the app's DI container instead (so it participates in the app's configured lifetime, interceptors, etc.), implement `IEFContextProvider` and register it with `SetEFContextProvider`:

```csharp
using DevExpress.Data.Entity;

public class CustomEFContextProvider : IEFContextProvider, IDisposable {
    IServiceScope scope;
    public CustomEFContextProvider(IServiceProvider provider) {
        this.scope = provider.CreateScope();
    }
    public object GetContext(string connectionName, Type contextType) {
        // connectionName is the DashboardEFDataSource's ConnectionName (2nd ctor argument), not the class name
        if (connectionName == "efDataConnection") {
            return scope.ServiceProvider.GetRequiredService(contextType);
        }
        return null;
    }
    public void Dispose() => scope.Dispose();
}
```

```csharp
using DevExpress.DashboardCommon;
using DevExpress.DataAccess.EntityFramework; // EFConnectionParameters

// Program.cs — inside the DashboardConfigurator factory
DashboardEFDataSource efDataSource = new DashboardEFDataSource("EF Data Source", "efDataConnection",
    new EFConnectionParameters(typeof(OrdersContext)));
dataSourceStorage.RegisterDataSource("efDataSource", efDataSource.SaveToXml());

configurator.SetEFContextProvider(new CustomEFContextProvider(serviceProvider));
```

```csharp
using Microsoft.EntityFrameworkCore; // required for UseSqlite / UseSqlServer extension methods

// Elsewhere in Program.cs — register the context itself; Transient avoids holding a long-lived
// scoped DbContext across the DashboardConfigurator's own scope
builder.Services.AddDbContext<OrdersContext>(
    options => options.UseSqlite("Data Source=file:Data/nwind.db"), ServiceLifetime.Transient);
```

---

## JSON Data Source (`DashboardJsonDataSource`)

**Prerequisite (.NET Framework)**: requires the `System.Text.Json` NuGet package — read the `.csproj` and add it if missing before generating code (Rule 5).

**Desktop apps**: If loading from a local JSON file, make sure the file is copied to the output directory. Set the file's `CopyToOutputDirectory` property to `Always` in Visual Studio, or add this to the `.csproj`:
```xml
<None Update="Data\customers.json">
  <CopyToOutputDirectory>Always</CopyToOutputDirectory>
</None>
```

### From a JSON string (inline)

```csharp
using DevExpress.DashboardCommon;
using DevExpress.DataAccess.Json;

DashboardJsonDataSource jsonDs = new DashboardJsonDataSource("JSON Data Source (String)");
string json = "{\"Customers\":[{\"Id\":\"ALFKI\",\"CompanyName\":\"Alfreds Futterkiste\"}]}";
jsonDs.JsonSource = new CustomJsonSource(json);
jsonDs.RootElement = "Customers";

dataSourceStorage.RegisterDataSource("jsonDataSourceString", jsonDs.SaveToXml());
```

### From a URL (connection string in appsettings.json)

```csharp
DashboardJsonDataSource jsonDs = new DashboardJsonDataSource("JSON Data Source (URL)");
jsonDs.ConnectionName = "jsonUrlConnection"; // matches key in appsettings.json ConnectionStrings
jsonDs.RootElement = "Customers";

dataSourceStorage.RegisterDataSource("jsonDataSourceUrl", jsonDs.SaveToXml());
```

```json
{
  "ConnectionStrings": {
    "jsonUrlConnection": "uri=https://raw.githubusercontent.com/DevExpress-Examples/DataSources/master/JSON/customers.json"
  }
}
```

### From a file (via ConfigureDataConnection event)

```csharp
using DevExpress.DashboardCommon;
using DevExpress.DataAccess.Json;

DashboardJsonDataSource jsonDs = new DashboardJsonDataSource("JSON Data Source (File)");
jsonDs.ConnectionName = "jsonFileConnection";
jsonDs.RootElement = "Customers";

configurator.ConfigureDataConnection += (object sender, ConfigureDataConnectionWebEventArgs e) => {
    if (e.ConnectionName == "jsonFileConnection") {
        Uri fileUri = new Uri(fileProvider.GetFileInfo("Data/customers.json").PhysicalPath,
            UriKind.RelativeOrAbsolute);
        JsonSourceConnectionParameters jsonParams = new JsonSourceConnectionParameters();
        jsonParams.JsonSource = new UriJsonSource(fileUri);
        e.ConnectionParameters = jsonParams;
    }
};

dataSourceStorage.RegisterDataSource("jsonDataSourceFile", jsonDs.SaveToXml());
```

---

## MongoDB Data Source (`DashboardMongoDBDataSource`)

**Prerequisite**: requires the `MongoDB.Driver` NuGet package — read the `.csproj` and add it if missing before generating code (Rule 5).

```csharp
using DevExpress.DashboardCommon;
using DevExpress.DataAccess.MongoDB;
using DevExpress.DataAccess.ConnectionParameters;

DashboardMongoDBDataSource mongoDs = new DashboardMongoDBDataSource(
    "MongoDB Data Source", "mongoDataSourceConnection");

// Add a query for a collection
MongoDBQuery query = new MongoDBQuery();
query.DatabaseName = "Northwind";
query.CollectionName = "Orders";
mongoDs.Queries.Add(query);

dataSourceStorage.RegisterDataSource("mongoDataSource", mongoDs.SaveToXml());

// Provide connection parameters at runtime
configurator.ConfigureDataConnection += (object sender, ConfigureDataConnectionWebEventArgs e) => {
    if (e.ConnectionName == "mongoDataSourceConnection") {
        // Option A: connection string
        e.ConnectionParameters = new MongoDBCustomConnectionParameters("mongodb://localhost:27017");

        // Option B: structured parameters (without authentication)
        // e.ConnectionParameters = new MongoDBConnectionParameters("localhost", false, 27017);
    }
};
```

---

## Excel Data Source (`DashboardExcelDataSource`)

Supports: `.xls`, `.xlsx`, `.csv`

**Desktop apps**: If loading from a local file, make sure the file is copied to the output directory. Set `CopyToOutputDirectory` to `Always` in Visual Studio, or add to the `.csproj`:
```xml
<None Update="Data\Sales.xlsx">
  <CopyToOutputDirectory>Always</CopyToOutputDirectory>
</None>
```

```csharp
using DevExpress.DashboardCommon;
using DevExpress.DataAccess.Excel;

DashboardExcelDataSource excelDs = new DashboardExcelDataSource("Excel Data Source");
excelDs.ConnectionName = "excelDataConnection";
excelDs.SourceOptions = new ExcelSourceOptions(new ExcelWorksheetSettings("Sheet1"));

// Provide file path at runtime
configurator.ConfigureDataConnection += (object sender, ConfigureDataConnectionWebEventArgs e) => {
    if (e.ConnectionName == "excelDataConnection") {
        ExcelDataSourceConnectionParameters excelParams =
            (ExcelDataSourceConnectionParameters)e.ConnectionParameters;
        excelParams.FileName = fileProvider.GetFileInfo("Data/Sales.xlsx").PhysicalPath;
    }
};

dataSourceStorage.RegisterDataSource("excelDataSource", excelDs.SaveToXml());
```

---

## OLAP Data Source (`DashboardOlapDataSource`)

Supported providers: XMLA via MSMDPUMP, or ADOMD.NET (`Microsoft.AnalysisServices.AdomdClient` NuGet)

Pass the connection name directly to the constructor and define it in `appsettings.json`. Do not use `ConfigureDataConnection` for OLAP — it is not required and leads to unnecessary complexity.

On **.NET 8+**, set the static `OlapDataProvider` before creating any OLAP data source instance:

```csharp
using DevExpress.DashboardCommon;

// Required once at startup for .NET Core applications
DashboardOlapDataSource.OlapDataProvider = OlapDataProviderType.Xmla;

DashboardOlapDataSource olapDs = new DashboardOlapDataSource("OLAP Data Source", "olapConnection");
dataSourceStorage.RegisterDataSource("olapDataSource", olapDs.SaveToXml());
```

Define the connection string in `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "olapConnection": "Provider=MSOLAP;Data Source=https://demos.devexpress.com/Services/OLAP/msmdpump.dll;Initial catalog=Adventure Works DW Standard Edition;Cube name=Adventure Works"
  }
}
```

---

## Federated Data Source (`DashboardFederationDataSource`)

A federated data source combines multiple data sources (SQL, JSON, Excel, Object — not OLAP) using Join, Union/UnionAll, or Transformation queries.

### Join Query

```csharp
using DevExpress.DashboardCommon;
using DevExpress.DataAccess.DataFederation;

// Assumes sqlDataSource, excelDataSource, objDataSource are already configured.
// The first Source argument is the ALIAS used in join conditions below.
Source sqlSource = new Source("sqlite", sqlDataSource, "SQLite Orders");
Source excelSource = new Source("excel", excelDataSource);
Source objectSource = new Source("SalesPersonDS", objDataSource);

DashboardFederationDataSource federationDs =
    new DashboardFederationDataSource("Federated Data Source (JOIN)");

SelectNode joinQuery =
    sqlSource.From()
    .Select("OrderDate", "ShipCity", "ShipCountry")
    .Join(excelSource, "[excel.OrderID] = [sqlite.OrderID]")
        .Select("CategoryName", "ProductName", "Extended Price")
        .Join(objectSource, "[SalesPersonDS.SalesPerson] = [excel.Sales Person]")
            .Select("SalesPerson", "Weight")
            .Build("FDS");
federationDs.Queries.Add(joinQuery);

dataSourceStorage.RegisterDataSource("federationDs", federationDs.SaveToXml());
```

> Join condition operands use the **aliases** declared in the `Source` constructors (`[alias.Column]`), not variable names.

### Union / UnionAll Query

```csharp
DashboardFederationDataSource federationDs =
    new DashboardFederationDataSource("Federated Data Source (UNION)");

UnionNode queryUnionAll = sqlSource.From()
    .Select("OrderID", "OrderDate").Build("OrdersSqlite")
    .UnionAll(excelSource.From().Select("OrderID", "OrderDate").Build("OrdersExcel"))
    .Build("OrdersUnionAll");
federationDs.Queries.Add(queryUnionAll);
```

### Calculated Fields in Federated Data Source

```csharp
// "FDS" is the federated query name from Build("FDS") in the Join example above
federationDs.CalculatedFields.Add("FDS", "[Weight] * [Extended Price] / 100", "Score");
```

The first argument is the query name, the second is the expression, and the third is the field name.

For master-detail relationships between federation queries (`FederationMasterDetailInfo`) or parameterized `Fill()` calls, use MCP:
```
devexpress_docs_search(technologies=["Dashboard"], question="DashboardFederationDataSource master detail relations")
```

---

## XPO and Extract Data Sources

For `DashboardXpoDataSource` and `DashboardExtractDataSource`, fetch the topic for **your platform** via dxdocs MCP — the two sets are not interchangeable, and every code example in this file is ASP.NET Core:

**ASP.NET Core / Blazor**

```
devexpress_docs_get_content(url="https://docs.devexpress.com/Dashboard/401226/web-dashboard/integrate-dashboard-component/dashboard-backend/prepare-data-source-storage-for-the-aspnet-core-framework/xpo-data-source")
devexpress_docs_get_content(url="https://docs.devexpress.com/Dashboard/116657/web-dashboard/integrate-dashboard-component/dashboard-backend/prepare-data-source-storage-for-the-aspnet-core-framework/extract-data-source")
```

**WinForms / WPF** — see also [winforms-wpf-data-sources.md](winforms-wpf-data-sources.md) for the surrounding desktop registration pattern

```
devexpress_docs_get_content(url="https://docs.devexpress.com/Dashboard/401313/winforms-dashboard/winforms-designer/create-dashboards-in-the-winforms-designer/providing-data/xpo-data-source")
devexpress_docs_get_content(url="https://docs.devexpress.com/Dashboard/115900/winforms-dashboard/winforms-designer/create-dashboards-in-the-winforms-designer/providing-data/extract-data-source")
```


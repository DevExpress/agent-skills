# Data Sources — WinForms and WPF

## When to Use This Reference

Use this when you need to:
- Bind a WinForms `DashboardDesigner`/`DashboardViewer` (`DevExpress.DashboardWin`) or WPF `DashboardControl` (`DevExpress.DashboardWpf`) to data in code
- Add data sources to a `Dashboard` object programmatically
- Understand the difference from the web `DashboardConfigurator` pattern

## How It Differs from the Web Pattern

| | Web (ASP.NET Core) | WinForms / WPF |
|---|---|---|
| Registration | `DataSourceInMemoryStorage.RegisterDataSource()` | `Dashboard.DataSources.Add()` |
| Central config class | `DashboardConfigurator` (scoped service) | None — configure on the `Dashboard` object |
| Object data supply | `DataLoading` event | `DataSource = typeof(MyClass)` + `DataMember` (fills automatically) |
| NuGet package | `DevExpress.AspNetCore.Dashboard` | `DevExpress.Win.Dashboard` (WinForms) |

## NuGet Package

Install the required NuGet package for your platform:
- WinForms: `DevExpress.Win.Dashboard`
- WPF: `DevExpress.Wpf.Dashboard`

> **The package ID and the namespace are transposed — do not use one as the other.**
> | | WinForms | WPF |
> |---|---|---|
> | NuGet package | `DevExpress.Win.Dashboard` | `DevExpress.Wpf.Dashboard` |
> | `using` namespace | `DevExpress.DashboardWin` | `DevExpress.DashboardWpf` |
>
> `using DevExpress.Win.Dashboard;` does not compile — there is no `DevExpress.Win` namespace.

Target framework must be `net8.0-windows` or later.

```xml
<TargetFramework>net8.0-windows</TargetFramework>
```

### .NET Framework

The same package IDs (`DevExpress.Win.Dashboard` / `DevExpress.Wpf.Dashboard`) also support .NET Framework projects — install via the NuGet Package Manager (or `Install-Package` in the Package Manager Console) and target `net462` or later.

---

## Object Data Source (WinForms / WPF)

The WinForms pattern uses `DataSource = typeof(MyClass)` and `DataMember` to load data at bind time — the dashboard fills the data source automatically when it is displayed or exported.

```csharp
using DevExpress.DashboardCommon;
using DevExpress.DashboardWin;   // DashboardDesigner / DashboardViewer (WPF: DevExpress.DashboardWpf)
using DevExpress.DataAccess.ObjectBinding;

Dashboard dashboard = new Dashboard();

DashboardObjectDataSource objectDataSource = new DashboardObjectDataSource();
objectDataSource.DataSource = typeof(Invoices);  // the class that provides data
objectDataSource.DataMember = "CreateData";       // the static method or property name
objectDataSource.Constructor = ObjectConstructorInfo.Default;

dashboard.DataSources.Add(objectDataSource);

// Assign the dashboard to the designer or viewer
dashboardDesigner1.Dashboard = dashboard;         // WinForms DashboardDesigner
// dashboardViewer1.Dashboard = dashboard;        // WinForms DashboardViewer
// dashboardControl.Dashboard = dashboard;        // WPF DashboardControl
```

The class referenced by `DataSource` must implement the `IEnumerable` or `IListSource` or have a public method or property returning `IEnumerable` or `IListSource`. The `DataMember` is the method or property name.

> Don't call `.Fill()` manually when `DataSource`/`DataMember` (or the SQL/EF connection) are already set correctly — it just re-fetches the data an extra time. The data source fills itself automatically when the dashboard is shown or exported.

---

## Entity Framework Data Source (WinForms / WPF)

```csharp
using DevExpress.DashboardCommon;
using DevExpress.DashboardWin;   // DashboardDesigner / DashboardViewer
using DevExpress.DataAccess.EntityFramework;

Dashboard dashboard = new Dashboard();

DashboardEFDataSource efDataSource = new DashboardEFDataSource();
efDataSource.ConnectionParameters = new EFConnectionParameters(
    typeof(OrdersContext),      // your DbContext type
    "SalesPersonModel");        // connection string name — appsettings.json (.NET 8+)
                                //   or app.config <connectionStrings> (.NET Framework)

dashboard.DataSources.Add(efDataSource);
dashboardDesigner1.Dashboard = dashboard;
```

---

## SQL Data Source (WinForms / WPF)

**Prerequisite (.NET 8+)**: requires the ADO.NET driver for the database behind the connection string — `Microsoft.Data.SqlClient` for `XpoProvider=MSSqlServer`, `Microsoft.Data.Sqlite` for `XpoProvider=SQLite`. It is loaded by reflection on the first data request, so a missing package builds cleanly and fails at runtime. Read the `.csproj` and add it if missing before generating code (Rule 5).

```csharp
using DevExpress.DashboardCommon;
using DevExpress.DashboardWin;   // DashboardDesigner / DashboardViewer
using DevExpress.DataAccess.Sql;

Dashboard dashboard = new Dashboard();

// 2nd argument = a ConnectionStrings key in appsettings.json / app.config, not the value
DashboardSqlDataSource sqlDataSource = new DashboardSqlDataSource(
    "SQL Data Source", "NWindConnectionString");
SelectQuery query = SelectQueryFluentBuilder
    .AddTable("Orders")
    .SelectAllColumnsFromTable()
    .Build("Orders");
sqlDataSource.Queries.Add(query);

dashboard.DataSources.Add(sqlDataSource);
dashboardDesigner1.Dashboard = dashboard;
```

Where the connection string lives depends on the target framework — DevExpress reads both automatically, with no extra NuGet packages or manual configuration code.

#### .NET 8+ — `appsettings.json`

```json
{
  "ConnectionStrings": {
    "NWindConnectionString": "XpoProvider=MSSqlServer;data source=localhost;Initial Catalog=Northwind;Integrated Security=True;TrustServerCertificate=True;Encrypt=True;"
  }
}
```

> **Always write `XpoProvider=` when generating a new connection string** — it names the provider explicitly and matches the format used everywhere else in this skill (see the provider table in [connection-strings.md](connection-strings.md)). DevExpress does fall back to the MS SQL provider when the string sets `Initial Catalog` and contains no `Provider` keyword, so an existing prefix-less SQL Server string in a project already works — **leave it alone**; do not "fix" it. The fallback covers SQL Server only, so any other database must carry the prefix.

Set `appsettings.json` to copy to the output directory in the `.csproj`:

```xml
<ItemGroup>
  <None Update="appsettings.json">
    <CopyToOutputDirectory>Always</CopyToOutputDirectory>
  </None>
</ItemGroup>
```

#### .NET Framework 4.x — `app.config`

Same connection name, different file: put it in `app.config` under `<connectionStrings>`.

```xml
<configuration>
  <connectionStrings>
    <add name="NWindConnectionString"
         connectionString="XpoProvider=MSSqlServer;data source=localhost;Initial Catalog=Northwind;Integrated Security=True;TrustServerCertificate=True;Encrypt=True;" />
  </connectionStrings>
</configuration>
```

> `app.config` is automatically copied to the output directory as `{AssemblyName}.exe.config` by the .NET Framework build — no `.csproj` entry is needed.

#### Either way — pass the connection name to the constructor

DevExpress resolves the name at runtime from whichever file the project uses:

```csharp
DashboardSqlDataSource sqlDataSource = new DashboardSqlDataSource(
    "SQL Data Source", "NWindConnectionString"); // resolved from appsettings.json (.NET 8+)
                                                 // or app.config (.NET Framework)
```

`"NWindConnectionString"` is the connection string name/key from the project's config file (appsettings.json / app.config). Example: a connection whose entry is named `NWindConnectionStringSQL` gets `new DashboardSqlDataSource("SQL Data Source", "NWindConnectionStringSQL")` (Rule 7).

> **Alternative — pass `ConnectionParameters` directly (no config file needed):**
> ```csharp
> using DevExpress.DataAccess.ConnectionParameters;
>
> DashboardSqlDataSource sqlDataSource = new DashboardSqlDataSource();
> sqlDataSource.ConnectionParameters = new MsSqlConnectionParameters {
>     ServerName = "localhost",
>     DatabaseName = "Northwind",
>     AuthorizationType = MsSqlAuthorizationType.Windows
> };
> ```
> Use this when credentials must be supplied at runtime or you want to avoid a config file.

### Stored Procedure Query (`StoredProcQuery`)

The `Queries` API is identical on WinForms/WPF and ASP.NET Core — only registration differs (`Dashboard.DataSources.Add()` here vs. `storage.RegisterDataSource()` on the web). A `StoredProcQuery` works the same way as the `SelectQuery` above:

```csharp
using DevExpress.DashboardCommon;
using DevExpress.DataAccess.Sql;

// 2nd argument = the ConnectionStrings key/name, as above — from the project's config file
DashboardSqlDataSource sqlDataSource = new DashboardSqlDataSource(
    "SQL Data Source", "NWindConnectionString");

StoredProcQuery spQuery = new StoredProcQuery {
    Name = "CustOrdersDetail",
    StoredProcName = "CustOrdersDetail"
};
spQuery.Parameters.Add(new QueryParameter("@OrderID", typeof(int), 10248));
sqlDataSource.Queries.Add(spQuery);

dashboard.DataSources.Add(sqlDataSource);
dashboardDesigner1.Dashboard = dashboard;
```

`Name` is the query display name shown inside the dashboard; `StoredProcName` is the actual stored procedure name in the database. See [data-source-types.md](data-source-types.md#stored-procedure-query-storedprocquery) for `CustomSqlQuery` and other query-building details — the same reference applies to both platforms.

---

## Other Data Source Types (JSON, Excel, MongoDB, OLAP, XPO, Extract, Federation)

Desktop registration is uniform: construct the data source and call `dashboard.DataSources.Add(ds)` — only the registration step differs from the web (no storage/configurator on desktop). For JSON, Excel, MongoDB, OLAP, and Federation, the type-specific constructors and configuration are the same as in the ASP.NET Core examples in [data-source-types.md](data-source-types.md).

> **Those examples are written for the web — translate the last line before using them.** Every example in `data-source-types.md` ends by registering into storage. On desktop, drop that line and finish with the two desktop lines instead:
>
> | In the web example | Write instead |
> |---|---|
> | `dataSourceStorage.RegisterDataSource("id", ds.SaveToXml());` | `dashboard.DataSources.Add(ds);` then `dashboardDesigner1.Dashboard = dashboard;` |
>
> Everything above that line — constructors, connection names, queries, type-specific properties — carries over unchanged. Nothing named `DataSourceInMemoryStorage`, `DashboardConfigurator`, or `SaveToXml()` belongs in desktop code.

For XPO and Extract, use dxdocs MCP directly:

```
devexpress_docs_get_content(url="https://docs.devexpress.com/Dashboard/115900/winforms-dashboard/winforms-designer/create-dashboards-in-the-winforms-designer/providing-data/extract-data-source")
devexpress_docs_get_content(url="https://docs.devexpress.com/Dashboard/401313/winforms-dashboard/winforms-designer/create-dashboards-in-the-winforms-designer/providing-data/xpo-data-source")
```

---

## Loading a Dashboard from File with Existing Data Sources

If the dashboard XML already has data source definitions saved in it, they load automatically:

```csharp
dashboardDesigner1.LoadDashboard("Data/Dashboards/sales.xml");
```

Data sources embedded in the XML are restored on load. You only need to add data sources in code when registering new sources not yet saved to the XML.

---

## Security — Type Trust (WinForms / WPF)

When a dashboard XML is opened by an end user, DevExpress validates the data source types before loading them. Rules differ by source type.

### Object Data Source

Access is granted by **trusting the data source classes** (Safe Deserialization). Register each class the dashboard's object data sources use:

```csharp
// At application startup — trust the types used by object data sources:
DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(SampleOrder));
```

Untrusted classes are not loaded. For trusted classes, opening a dashboard that contains a `DashboardObjectDataSource` still shows a trust dialog by default — suppress it in code:

```csharp
// Load trusted object data sources without prompting
// (DocumentLoadingBehavior values: LoadAsIs, LoadSafely, Prompt)
dashboardDesigner1.DataSourceOptions.ObjectDataSourceLoadingBehavior =
    DevExpress.DataAccess.DocumentLoadingBehavior.LoadAsIs;
```

Full mechanism: [Safe Deserialization](https://docs.devexpress.com/content/GeneralInformation/404441/security/safe-deserialization?md=true).

> **Web (ASP.NET Core)**: with the `DataLoading` pattern nothing about the class is serialized into the XML, so there is no trust dialog and nothing to register. A web source built from a `Type` instead does store the type name and does need `RegisterTrustedClass` — see [aspnetcore-setup.md](aspnetcore-setup.md).

### Entity Framework Data Source

By default, loading a custom assembly referenced by a `DashboardEFDataSource` is blocked. **If the `DbContext` assembly is already referenced by the project and loaded at startup, no extra step is needed** — `CustomAssemblyLoading` is not raised for already-loaded assemblies.

To allow an assembly that is not already in the app:

```csharp
using DevExpress.DashboardCommon;

dashboardDesigner1.CustomAssemblyLoading += (object s, DashboardCustomAssemblyLoadingEventArgs e) => {
    // args expose: AssemblyPath, DataSourceName, DataSourceComponentName, ContextName, AllowLoading
    if (e.AssemblyPath.EndsWith("MyDomain.Models.dll"))
        e.AllowLoading = true;
    // Unauthorized attempts throw CustomAssemblyLoadingProhibitedException
};
```

### Custom Dashboard Items and XtraReports Controls

Same rule as EFDS: if the assembly containing the custom item or report control is already referenced by the application, `CustomAssemblyLoading` does not fire and no extra trust step is required.

### Custom Parameters

- **Enum type**: No trust concern — serialized as its underlying primitive value.
- **Complex type**: Requires a custom converter. Consult DevExpress support for the current API (`IOneTypeObjectConverter`).

---

## Troubleshooting

| Symptom | Cause | Solution |
|---------|-------|----------|
| `DashboardDesigner` not found | Wrong NuGet package | WinForms requires `DevExpress.Win.Dashboard`; WPF requires `DevExpress.Wpf.Dashboard` (same package IDs for .NET Framework, targeting `net462`+) |
| Trust dialog appears on every open | `ObjectDataSourceLoadingBehavior` not set | Register the classes via `DeserializationSettings.RegisterTrustedClass` and set `DataSourceOptions.ObjectDataSourceLoadingBehavior = DocumentLoadingBehavior.LoadAsIs` |
| Object data source class not loaded at all | Class not trusted (Safe Deserialization) | Call `DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(...))` at startup |
| SQL connection fails / "login failed" even though `appsettings.json` looks correct | Code manually re-read the connection string (`ConfigurationBuilder`) and rebuilt it with an extra `XpoProvider=` prefix, doubling the one already in the stored value | Delete the manual config-reading code; pass the plain connection **name** string to the `DashboardSqlDataSource` constructor — DevExpress resolves it from `appsettings.json`/`app.config` automatically |
| `CustomAssemblyLoadingProhibitedException` on EF source | Assembly not loaded and `CustomAssemblyLoading` not handled | Handle the event and set `e.AllowLoading = true`, or reference the assembly directly in the project |
| SQL Server connection fails at runtime on .NET 8+ | `Microsoft.Data.SqlClient` not referenced — it is required, not optional | Add the `Microsoft.Data.SqlClient` package; check the `.csproj` before generating connection code (Rule 5) |

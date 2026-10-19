# Getting Started — Dashboard Data Sources (ASP.NET Core)

This guide shows how to configure data source storage for an ASP.NET Core Web Dashboard backend.

## System Requirements

- .NET 8.0+
- DevExpress packages (available on nuget.org)

## Installation

```
dotnet add package DevExpress.AspNetCore.Dashboard
```

## Quick Start — Minimal Registration

Register a data source into storage inside the scoped `DashboardConfigurator` factory (full `Program.cs` below):

```csharp
var storage = new DataSourceInMemoryStorage();
// 2nd argument = a ConnectionStrings key in appsettings.json, not the value (Rule 7)
var sql = new DashboardSqlDataSource("SQL Data Source", "NWindConnectionString");
sql.Queries.Add(SelectQueryFluentBuilder
    .AddTable("Categories").Join("Products", "CategoryID")
    .SelectAllColumnsFromTable().Build("Products_Categories"));
storage.RegisterDataSource("sqlDataSource", sql.SaveToXml());

configurator.SetDataSourceStorage(storage);
configurator.SetConnectionStringsProvider(new DashboardConnectionStringsProvider(configuration));
```

> A data source saved inside a dashboard's `.xml` loads with it and stays bound to its items. A copy in `DataSourceInMemoryStorage` does not rebind them (Rule 8), so supply what the embedded source needs: `DataLoading` for object sources, `ConfigureDataConnection` or a connection strings provider for the rest.

## Full Program.cs Setup

The `DashboardConfigurator` must be registered as a scoped service. All data source registration happens inside the factory lambda.

```csharp
using DevExpress.AspNetCore;
using DevExpress.DashboardAspNetCore;
using DevExpress.DashboardCommon;
using DevExpress.DashboardWeb;
using DevExpress.DataAccess.Sql;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);

IFileProvider fileProvider = builder.Environment.ContentRootFileProvider;
IConfiguration configuration = builder.Configuration;

builder.Services.AddDevExpressControls();
builder.Services.AddRazorPages();
builder.Services.AddScoped<DashboardConfigurator>((IServiceProvider serviceProvider) => {
    DashboardConfigurator configurator = new DashboardConfigurator();

    // 1. Dashboard storage — where .xml dashboard definitions are saved/loaded
    configurator.SetDashboardStorage(
        new DashboardFileStorage(fileProvider.GetFileInfo("Data/Dashboards").PhysicalPath));

    // 2. Data source storage — which data sources are available to end users
    DataSourceInMemoryStorage dataSourceStorage = new DataSourceInMemoryStorage();

    // Register your data sources here (see data-source-types.md for each type)
    DashboardObjectDataSource objDataSource = new DashboardObjectDataSource("Object Data Source");
    // DataId identifies the source to the DataLoading handler below — match it exactly there.
    objDataSource.DataId = "objectDataSource";
    // The first RegisterDataSource argument is the STORAGE id (how the dashboard XML refers to
    // this source). It is a different string from DataId and is never seen by DataLoading.
    dataSourceStorage.RegisterDataSource("objDataSource", objDataSource.SaveToXml());

    configurator.SetDataSourceStorage(dataSourceStorage);

    // 3. Supply data for Object data sources via DataLoading event
    configurator.DataLoading += (object sender, DataLoadingWebEventArgs e) => {
        if (e.DataId == "objectDataSource") {
            e.Data = MyData.GetOrders(); // return IEnumerable or DataTable
        }
    };

    // 4. Connection strings provider — reads from appsettings.json ConnectionStrings section
    configurator.SetConnectionStringsProvider(
        new DashboardConnectionStringsProvider(configuration));

    // Credentials never reach the browser by default (DashboardConfigurator.PassCredentials is false) — nothing to set here.

    return configurator;
});

var app = builder.Build();
app.UseDevExpressControls();
app.MapRazorPages();
app.MapDashboardRoute("api/dashboard", "DefaultDashboard");
app.Run();
```

> The pattern above serializes nothing about your class — `DashboardObjectDataSource` saves only `DataId`, `DataSchema`, and `Filter` — so no trust call is needed and the desktop trust dialog never appears. Only a dashboard XML carrying `<DataSource Type="…">` needs its type trusted, once at startup before `app.Run()` (Rule 9): `DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(Order));`

## Dashboard Controller

Add a controller that inherits `DashboardController`. The `DashboardConfigurator` is injected automatically.

```csharp
using DevExpress.DashboardAspNetCore;
using DevExpress.DashboardWeb;
using Microsoft.AspNetCore.DataProtection;

public class DefaultDashboardController : DashboardController {
    public DefaultDashboardController(DashboardConfigurator configurator,
        IDataProtectionProvider? dataProtectionProvider = null)
        : base(configurator, dataProtectionProvider) { }
}
```

## Connection Strings in appsettings.json

Add a `ConnectionStrings` section. The key names match the connection names you use when creating data sources.

```json
{
  "ConnectionStrings": {
    "NWindConnectionString": "XpoProvider=SQLite;Data Source=Data/nwind.db",
    "msSqlConnection": "XpoProvider=MSSqlServer;data source=localhost;initial catalog=Northwind;Integrated Security=SSPI;",
    "jsonUrlConnection": "uri=https://raw.githubusercontent.com/DevExpress-Examples/DataSources/master/JSON/customers.json",
    "mongoDataSourceConnection": "mongodb://localhost:27017/"
  }
}
```

The `DashboardConnectionStringsProvider` exposes these to the Dashboard Data Source Wizard automatically.

## Key APIs

### `DashboardConfigurator`

| Member | Description |
|--------|-------------|
| `SetDataSourceStorage(IDataSourceStorage)` | Register the data source storage |
| `SetConnectionStringsProvider(IDataSourceWizardConnectionStringsProvider)` | Set the connection strings provider |
| `ConfigureDataConnection` (event) | Fires per data request; set `e.ConnectionParameters` to inject connection settings |
| `DataLoading` (event) | Fires for Object data sources; set `e.Data` to supply data (Rule 3). Identify the source by `e.DataId`, or — for one defined in dashboard XML, which often has no `DataId` — by `e.DataSourceComponentName` (the XML's `ComponentName`) or `e.DataSourceName` |
| `AllowExecutingCustomSql` | Set `true` to allow `CustomSqlQuery` execution (off by default — Rule 2) |
| `PassCredentials` (static) | `false` by default — credentials stay off the client; don't set it (Rule 4) |

### `DataSourceInMemoryStorage`

`RegisterDataSource(string id, XDocument xml)` — register a source by ID. Call `dataSource.SaveToXml()` on any `DashboardDataSourceBase` descendant to get the `XDocument`.

## Troubleshooting

| Symptom | Cause | Solution |
|---------|-------|----------|
| Data Source Wizard shows no connections | `SetConnectionStringsProvider` not called / provider empty | Call it with a configured provider; verify `appsettings.json` has a `ConnectionStrings` section |
| Data source not visible in dashboard | Wrong ID, or storage not set | Verify `RegisterDataSource` and `SetDataSourceStorage` are both called (correct storage instance) |
| Items go empty after the data source is moved out of the dashboard XML into storage | The binding goes with it; a copy in storage does not restore it | Put the data source back in the XML and supply its data or connection instead (Rule 8) |
| `DataLoading` not firing | `DashboardObjectDataSource.DataId` unset, or handler checks a different `e.DataId` | Set `DataId` and match it exactly in the handler (Rule 3). For a source that lives in the dashboard XML and has no `DataId`, match `e.DataSourceComponentName` instead of inventing one |
| Custom SQL query ignored / rejected | Custom SQL disabled, or validation rejected it | Set `AllowExecutingCustomSql = true`; only SELECT passes the built-in validator (Rule 2) |
| Build error: `DevExpress.Dashboard.Core not found` | Package not installed | `dotnet add package DevExpress.AspNetCore.Dashboard` — it brings `DevExpress.Dashboard.Core` in transitively; do not install Core directly in a web project |
| SQL Server connection fails at runtime on .NET 8+ | `Microsoft.Data.SqlClient` not referenced — it is required, not optional | `dotnet add package Microsoft.Data.SqlClient`; check the `.csproj` before generating connection code (Rule 5) |
| Dashboard shows another user's or tenant's rows after a per-request connection switch | The data cache served an existing record, so `ConfigureDataConnection` never ran | Add the tenant to the key in `DataSourceCacheKeyCreated` (`e.Key.CustomData`), or set `DataSourceCacheEnabled = false` — [connection-strings.md](connection-strings.md) |

## ASP.NET MVC / Web Forms

Not covered by this skill. The registration API is the same but uses the static `DashboardConfigurator.Default` singleton (configured in `Global.asax.cs`), reads connection strings via `ConfigFileConnectionStringsProvider` from `Web.config`, and stores connections in `<connectionStrings>` instead of `appsettings.json`. Query the docs MCP server for a full example: `devexpress_docs_search(technologies=["Dashboard"], question="register data sources ASP.NET MVC DashboardConfigurator.Default")`.

## What's Next

- [data-source-types.md](data-source-types.md) — per-type code examples (SQL, JSON, Object, EF, MongoDB, etc.)
- [connection-strings.md](connection-strings.md) — custom providers, runtime credentials, multi-tenancy
- [winforms-wpf-data-sources.md](winforms-wpf-data-sources.md) — WinForms and WPF (completely different API from the web pattern)
- In-memory data source caching is on by default (`DashboardConfigurator.DataSourceCacheEnabled`), and it matters as soon as a connection varies per request. `ConfigureDataConnection` runs only while a cache record is created, so without `DataSourceCacheKeyCreated` or `DataSourceCacheEnabled = false`, one user's rows are served to the next — see **Approach 4** in [connection-strings.md](connection-strings.md). Full topic: `devexpress_docs_get_content(url="https://docs.devexpress.com/Dashboard/400983/web-dashboard/integrate-dashboard-component/dashboard-backend/manage-an-in-memory-data-cache")`

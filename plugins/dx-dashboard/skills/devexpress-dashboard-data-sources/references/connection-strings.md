# Connection Strings & Security — DevExpress Dashboard (ASP.NET Core)

## When to Use This Reference

This reference covers the ASP.NET Core `DashboardConfigurator` pattern; the XPO connection string format table at the end applies to both platforms. For WinForms/WPF — `appsettings.json` on .NET 8+, `app.config` `<connectionStrings>` on .NET Framework 4.x — see [winforms-wpf-data-sources.md](winforms-wpf-data-sources.md).

Use this when you need to:
- Supply named database connections to the Dashboard Data Source Wizard
- Implement custom per-user or runtime connection logic
- Prevent credentials from being exposed in the browser

---

## Approach 1: appsettings.json (Recommended for Static Connections)

Store connection strings in `appsettings.json`. The built-in `DashboardConnectionStringsProvider` exposes these to the Dashboard Data Source Wizard automatically.

```json
{
  "ConnectionStrings": {
    "msSqlConnection": "XpoProvider=MSSqlServer;data source=localhost;initial catalog=Northwind;Integrated Security=SSPI;Persist Security Info=True;",
    "xmlConnection": "XpoProvider=InMemoryDataStore;Read Only=true;Data Source=Data/nwind.xml;",
    "jsonUrlConnection": "uri=https://raw.githubusercontent.com/DevExpress-Examples/DataSources/master/JSON/customers.json",
    "mongoDataSourceConnection": "mongodb://localhost:27017/"
  }
}
```

Register in the `DashboardConfigurator` factory (`Program.cs`):

```csharp
using DevExpress.DashboardAspNetCore;

configurator.SetConnectionStringsProvider(new DashboardConnectionStringsProvider(configuration));
```

`DashboardConnectionStringsProvider` is in namespace `DevExpress.DashboardAspNetCore`. Pass the `IConfiguration` instance from `builder.Configuration`.

This line is the only place `IConfiguration` is used for connections. A data source refers to its connection by the JSON key alone — `new DashboardSqlDataSource("SQL Data Source", "msSqlConnection")`, or `jsonDs.ConnectionName = "jsonUrlConnection"` — and the provider resolves the value. Never call `configuration.GetConnectionString(...)` yourself (Rule 7).

---

## Approach 2: Custom Connection Strings Provider

Implement `IDataSourceWizardConnectionStringsProvider` to add custom logic — reading from a database, per-user filtering, or secret management.

```csharp
using DevExpress.DataAccess.Web;
using DevExpress.DashboardCommon;
using DevExpress.DataAccess.Json;
using DevExpress.DataAccess.ConnectionParameters;

public class MyConnectionStringsProvider : IDataSourceWizardConnectionStringsProvider {

    public Dictionary<string, string> GetConnectionDescriptions() {
        // Key = connection NAME (what a data source's ConnectionName must match and what
        // GetDataConnectionParameters receives). Value = the label shown in the wizard.
        return new Dictionary<string, string> {
            { "jsonUrlConnection", "JSON URL Connection" },
            { "msSqlConnection", "MS SQL Connection" }
        };
    }

    public DataConnectionParametersBase GetDataConnectionParameters(string name) {
        if (name == "jsonUrlConnection") {
            return new JsonSourceConnectionParameters {
                JsonSource = new UriJsonSource(
                    new Uri("https://raw.githubusercontent.com/DevExpress-Examples/DataSources/master/JSON/customers.json"))
            };
        }
        if (name == "msSqlConnection") {
            return new MsSqlConnectionParameters {
                ServerName = "localhost",
                DatabaseName = "Northwind",
                AuthorizationType = MsSqlAuthorizationType.Windows
            };
        }
        throw new Exception($"Unknown connection: {name}");
    }
}
```

Register in the `DashboardConfigurator` factory:

```csharp
configurator.SetConnectionStringsProvider(new MyConnectionStringsProvider());
```

---

## Approach 3: ConfigureDataConnection Event (Runtime Injection)

Handle `DashboardConfigurator.ConfigureDataConnection` to set connection parameters per-request. Useful when credentials vary by user session or environment.

This isn't a standalone alternative to Approaches 1/2 — it layers on top of whichever provider already supplies the named connection, letting you override or fill in parameters for a specific connection name at request time regardless of how that connection was originally configured.

```csharp
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.Utils;

configurator.ConfigureDataConnection += (object sender, ConfigureDataConnectionWebEventArgs e) => {
    if (e.ConnectionName == "msSqlConnection") {
        // Windows (Integrated) authentication
        e.ConnectionParameters = new MsSqlConnectionParameters {
            ServerName = "localhost",
            DatabaseName = "Northwind",
            AuthorizationType = MsSqlAuthorizationType.Windows,
            Encrypt = DefaultBoolean.True,
            TrustServerCertificate = DefaultBoolean.True
        };
        // SQL Server authentication (supply credentials from config/secrets, not hardcoded):
        // e.ConnectionParameters = new MsSqlConnectionParameters {
        //     ServerName = "localhost",
        //     DatabaseName = "Northwind",
        //     AuthorizationType = MsSqlAuthorizationType.SqlServer,
        //     UserName = Environment.GetEnvironmentVariable("DB_USER"),
        //     Password = Environment.GetEnvironmentVariable("DB_PASSWORD"),
        //     Encrypt = DefaultBoolean.True,
        //     TrustServerCertificate = DefaultBoolean.True
        // };
    }

    if (e.ConnectionName == "sqliteConnection") {
        e.ConnectionParameters = new SQLiteConnectionParameters {
            FileName = "Data/nwind.db"
        };
    }

    if (e.ConnectionName == "mongoDataSourceConnection") {
        e.ConnectionParameters =
            new MongoDBCustomConnectionParameters("mongodb://localhost:27017");
    }
};
```

`ConfigureDataConnectionWebEventArgs.ConnectionName` matches the connection name you assigned to the data source (e.g., `sqlDataSource.ConnectionName = "sqliteConnection"`).

---

## Security: Preventing Credential Leakage

All three approaches above supply credentials server-side and keep them out of the dashboard XML sent to the browser. That is already the default — `DashboardConfigurator.PassCredentials` (static) is `false` — so no code is needed here; leave it alone (Rule 4).

---

### Restricting which rows a user sees

Two server-side hooks, both on `DashboardConfigurator`. Do not use a hidden dashboard parameter as the boundary — its value can surface in exports, and if you source it from a request header the client chooses it.

| Hook | What it does |
|---|---|
| `CustomFilterExpression` (event) | Appends a WHERE clause to `DashboardSqlDataSource` queries — SQL sources only |
| `SetDBConnectionInterceptor(IDBConnectionInterceptor)` | Intercepts the connection itself (`ConnectionOpening` / `ConnectionOpened`, `DevExpress.DataAccess.Sql`) (row-level security) |

## Approach 4: Switch the Database at Runtime (Request Headers)

A common requirement: the same dashboard reads from a different database per user/tenant/click. Pass a key from the client via request headers and resolve it in `ConfigureDataConnection`:

```javascript
// Client: set a header, then reload data
dashboardControl.option('fetchRemoteService.headers', { "database": "nwind2" });
dashboardControl.reloadData();
```

```csharp
// Server: Program.cs DashboardConfigurator factory
builder.Services.AddHttpContextAccessor();
// inside AddScoped<DashboardConfigurator>((serviceProvider) => { ... :

configurator.ConfigureDataConnection += (object s, ConfigureDataConnectionWebEventArgs e) => {
    if (e.ConnectionName == "nwind") {
        var httpContextAccessor = serviceProvider.GetService<IHttpContextAccessor>();
        IHeaderDictionary headers = httpContextAccessor.HttpContext.Request.Headers;
        string dbKey = headers.ContainsKey("database") ? headers["database"] : "nwind";
        string connectionString = dbKey switch {
            "nwind"  => "XpoProvider=InMemoryDataStore;Read Only=true;Data Source=Data\\nwind.xml",
            "nwind2" => "XpoProvider=InMemoryDataStore;Read Only=true;Data Source=Data\\nwind2.xml",
            _ => throw new ArgumentOutOfRangeException("Incorrect database name")
        };
        e.ConnectionParameters = new CustomStringConnectionParameters(connectionString);
    }
};
```

**Switching the connection is not enough — the data cache has to know about it.** `ConfigureDataConnection` runs only while a cache record is created, and the key is built from the dashboard, data source, query, parameters, and columns. Your header is not part of it, so the first tenant's rows are served to every tenant. Put the tenant in the key, or turn the cache off:

```csharp
// A — keep caching, one record per database (e.Key.CustomData is part of the cache key)
configurator.DataSourceCacheKeyCreated += (object s, DataSourceCacheKeyCreatedEventArgs e) => {
    var httpContextAccessor = serviceProvider.GetService<IHttpContextAccessor>();
    IHeaderDictionary headers = httpContextAccessor.HttpContext.Request.Headers;
    e.Key.CustomData["database"] = headers.ContainsKey("database") ? headers["database"] : "nwind";
};

// B — simplest: no caching, every request reaches the database
configurator.DataSourceCacheEnabled = false;
```

`e.InvalidateCacheRecord()` in the same handler drops a single record on demand; use it when the data has to be re-read but the connection has not changed. Cache details: `devexpress_docs_get_content(url="https://docs.devexpress.com/Dashboard/400983/web-dashboard/integrate-dashboard-component/dashboard-backend/manage-an-in-memory-data-cache")`

> Export requests cannot carry custom headers over the Ajax remote service (`form.submit`) — use the Fetch remote service (`fetchRemoteService`) when headers must apply to exports too.

---

## XPO Connection String Formats

XPO-based SQL providers use connection strings with `XpoProvider=` prefix:

| Database | Connection String Format |
|----------|--------------------------|
| MS SQL Server | `XpoProvider=MSSqlServer;Data Source=localhost;User ID=user;Password=pwd;Initial Catalog=Northwind;` |
| SQLite | `XpoProvider=SQLite;Data Source=Data/nwind.db` |
| PostgreSQL | `XpoProvider=Postgres;Server=127.0.0.1;User ID=user;Password=pwd;Database=db;` |
| MySQL | `XpoProvider=MySql;Server=address;User ID=user;Password=pwd;Database=db;` |
| Oracle (ODP.NET managed) | `XpoProvider=ODPManaged;Data Source=oracleHost;User ID=user;Password=pwd;` |
| XML/In-Memory | `XpoProvider=InMemoryDataStore;Read Only=true;Data Source=Data/nwind.xml;` |
| JSON (URI) | `uri=https://example.com/data.json` |
| MongoDB | `mongodb://localhost:27017/` |

For other providers (DB2, Firebird, Teradata, Redshift, BigQuery, etc.), fetch the supported-providers topic directly via dxdocs MCP server:
```
devexpress_docs_get_content(url="https://docs.devexpress.com/Dashboard/117284/common-features/custom-connection-strings-for-data-sources")
```

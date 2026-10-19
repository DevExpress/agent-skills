# Data Sources & the Data Source Wizard — ASP.NET Core

## When to Use This Reference

Use this when you need to:
- Register connection strings (SQL, JSON, MongoDB) so they appear in the Report Wizard / Data Source Wizard — either the built-in configuration-file providers or a custom provider
- Source connection strings from somewhere other than `appsettings.json` (a database, a secrets manager, a curated subset, a non-default `IConfigurationSection`)
- Shape the database schema the Data Source Wizard shows — hide tables/views/procedures, support synonyms
- Let end users bind reports to .NET business objects (Object Data Source): supply the type list, filter constructors, filter members
- Pre-configure a data source in the controller so the user sees it already set up in the Designer
- Resolve an Entity Framework Core `DbContext` or a JSON connection from the DI container
- Raise the record limit the wizard scans when inferring a JSON data source's schema

Related but **not** here:
- `IConnectionProviderFactory` (restoring a connection by *name* on report deserialization, the web-farm alternative to a shared machine key) — see [report-storage.md](report-storage.md#web-farm--multi-server-setup).
- Protecting connection strings sent to the client (`ISecureDataConverter`, `IDataSourceProtectionService`) and allowing custom SQL (`EnableCustomSql`, `ISqlDataSourceWizardCustomizationService`) — see [security.md](security.md).
- Row-level (multi-tenant) SELECT filtering (`ISelectQueryFilterService`) — see [security.md](security.md#multi-tenant-data-filtering).

## Registration Patterns

Data-source services register through one of two surfaces:

```csharp
using DevExpress.AspNetCore;
using DevExpress.AspNetCore.Reporting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureReportingServices(configurator => {
    // 1. Data Source Wizard services — via ConfigureReportDesigner
    configurator.ConfigureReportDesigner(designerConfigurator => {
        designerConfigurator.RegisterDataSourceWizardConfigFileConnectionStringsProvider();
        designerConfigurator.RegisterObjectDataSourceWizardTypeProvider<CustomTypeProvider>();
    });
    // 2. Viewer-scoped data services — via ConfigureWebDocumentViewer
    configurator.ConfigureWebDocumentViewer(viewerConfigurator => {
        viewerConfigurator.RegisterEFContextProviderFactory<CustomEFContextProviderFactory>();
    });
    // 3. Query-builder-service-scoped schema services — via ConfigureQueryBuilder
    configurator.ConfigureQueryBuilder(queryBuilderConfigurator => {
        queryBuilderConfigurator.RegisterDataSourceWizardDBSchemaProviderExFactory<CustomSchemaProviderFactory>();
    });
});

// 4. Plain DI registration — for services with no dedicated *ConfigurationBuilder method
builder.Services.AddSingleton<IDataSourceWizardDBSchemaProviderExFactory, DBSchemaProviderExFactory>();

var app = builder.Build();
```

## Connection String Providers

### Built-In (Configuration File) Providers

These serve connection strings straight from the app's configuration file (`appsettings.json`'s `ConnectionStrings` section) — no code to write. This is the must-have registration covered in [getting-started.md](getting-started.md) Step 10.

| Method (on `ReportDesignerConfigurationBuilder`) | Purpose |
|---|---|
| `RegisterDataSourceWizardConfigFileConnectionStringsProvider()` | Serves **SQL** connection strings from the configuration file. |
| `RegisterDataSourceWizardConfigFileJsonConnectionStringsProvider()` | Serves **JSON** connection strings (different value format — see below). |
| `RegisterDataSourceWizardConfigFileMongoDBConnectionStringsProvider()` | Serves **MongoDB** connection strings (different value format — see below). |
| `RegisterDataSourceWizardConfigurationConnectionStringsProvider(IConfigurationSection[, IConfigurationSection])` | Serves connection strings from a **custom `IConfigurationSection`** — use when connection info lives under a section other than the default `ConnectionStrings`. |

Value formats differ per provider and are **not** interchangeable — an incorrect connection string causes exceptions at runtime.

```json
{
  "ConnectionStrings": {
    "NWindConnectionString": "XpoProvider=SQLite;Data Source=|DataDirectory|/Data/nwind.db",
    "JsonConnection": "Uri=https://raw.githubusercontent.com/DevExpress-Examples/DataSources/master/JSON/customers.json",
    "MongoDBConnection": "mongodb://localhost:27017/"
  }
}
```

**Every connection needs its data-provider package referenced by the app.** DevExpress ships no drivers and loads them reflectively at first connection, so a missing one is invisible until runtime — the data call returns HTTP **200** with `{"error":"Unable to open a database."}` (real cause: `TypeLoadException: None of the following assemblies found: …`). Packages per database: [Supported Data Providers](https://docs.devexpress.com/CoreLibraries/403638/devexpress-data-library/data-sources/use-the-sql-data-source/supported-data-providers).

### Custom Providers

| Service / Method | Why / When |
|---|---|
| `IDataSourceWizardConnectionStringsProvider` | The Data Source Wizard's SQL connection list defaults to whatever connection strings exist in the app's configuration file. Implement this to serve a different or filtered set instead (from a database, a secrets manager, or a curated subset). Register with `RegisterDataSourceWizardConnectionStringsProvider<T>()`; the `<T>(bool)` overload additionally controls whether the same provider is used by the Designer's integrated Preview. |
| `RegisterDataSourceWizardJsonConnectionStorage<T>()` / `<T>(bool)` | Registers a custom service that both **stores and supplies** JSON data source connections (read/write) — unlike the read-only providers above, which only serve them. The `bool` overload controls whether the Designer's integrated Preview uses it too. |
| `RegisterDataSourceWizardMongoDBConnectionStringsProvider<T>()` | Registers a custom connection-string provider specifically for **MongoDB** data sources. |
| `RegisterJsonDataConnectionProviderFactory<T>()` | Registered on `WebDocumentViewerConfigurationBuilder`. Returns a JSON data source's connection string by name — the JSON-source equivalent of a SQL connection-string provider, used when the Viewer resolves a report bound to a named JSON connection. |
| `RegisterEFContextProviderFactory<T>()` | Registered on `WebDocumentViewerConfigurationBuilder`. Resolves an Entity Framework Core `DbContext` from the DI container for reports/data sources bound to EF Core — without it, an EF-bound report has no context to read from. |

Official articles for the connection-registration workflows: [Register SQL Data Connections](https://docs.devexpress.com/XtraReports/400279), [Register JSON Data Connections](https://docs.devexpress.com/XtraReports/401655), [Register MongoDB Data Connections](https://docs.devexpress.com/XtraReports/405584).

## Database Schema Providers — Three Independent Scopes

`IDataSourceWizardDBSchemaProviderExFactory` shapes the tables/views/stored procedures a component reads from the database — implement it to support database synonyms, or to hide specific objects from the schema tree entirely.

The **same interface** is registered at three independent scopes. Registering one does **not** affect the others — pick the scope where the customization is actually needed, or register all three if it should apply everywhere:

| Scope | Registration | Applies to |
|---|---|---|
| Document Viewer | `WebDocumentViewerConfigurationBuilder.RegisterDBSchemaProviderExFactory<T>()` | The standalone Viewer's own schema needs (e.g., drill-through scenarios). |
| Report Designer | `ReportDesignerConfigurationBuilder.RegisterDataSourceWizardDBSchemaProviderExFactory<T>(bool overrideReportPreviewProvider)` | The Designer's Data Source Wizard. Pass `true` to also use it for the Designer's integrated Preview, `false` to scope it to the wizard only. |
| Query-builder service | `QueryBuilderConfigurationBuilder.RegisterDataSourceWizardDBSchemaProviderExFactory<T>()` | The query-building service behind the Designer's Data Source Wizard (the `DXXQB` endpoint served by `QueryBuilderController`). |

There is also a plain-DI path — `builder.Services.AddSingleton<IDataSourceWizardDBSchemaProviderExFactory, MyFactory>()` — for the interface with no builder-method scoping.

> `QueryBuilderConfigurationBuilder.RegisterDataSourceWizardDBSchemaProviderFactory<T>()` is **deprecated**. Do not use it in new code — use the `...ExFactory<T>()` form above.

## Object Data Source Services

| Service / Method | Why / When |
|---|---|
| `IObjectDataSourceWizardTypeProvider` | By default, the Report/Data Source Wizard lists **no** .NET types for an Object Data Source. Implement this to supply the list of business-object types end users are allowed to bind a report to. Register with `RegisterObjectDataSourceWizardTypeProvider<T>()`. |
| `IObjectDataSourceConstructorFilterService` | Once a type is registered, the wizard lists every public constructor on it. Implement this to filter that list — both for usability (hide constructors that make no sense for reporting) and to avoid exposing constructors you don't want end users invoking through the wizard's reflection UI. Register with `RegisterObjectDataSourceConstructorFilterService<T>()`. |
| `IObjectDataSourceMemberFilterService` | Same idea, for the members (properties/fields) offered as report fields — filter out members that shouldn't be exposed to report authors. Register with `RegisterObjectDataSourceMemberFilterService<T>()`. |

### Worked Example — Register an Object Data Source Provider

```csharp
using DevExpress.DataAccess.Web;

public class ObjectDataSourceWizardCustomTypeProvider : IObjectDataSourceWizardTypeProvider {
    public IEnumerable<Type> GetAvailableTypes(string context) {
        return new[] { typeof(Employees.DataSource) };
    }
}
```

Register the provider in `Program.cs` and trust the custom type for deserialization:

```csharp
builder.Services.ConfigureReportingServices(configurator => {
    configurator.ConfigureReportDesigner(designerConfigurator => {
        designerConfigurator.RegisterObjectDataSourceWizardTypeProvider
            <ObjectDataSourceWizardCustomTypeProvider>();
    });
});

DevExpress.Utils.DeserializationSettings.RegisterTrustedClass(typeof(Employees.DataSource));
```

> The `RegisterTrustedClass` call is mandatory and must run **before** `WebApplication.CreateBuilder(args)`. See [report-storage.md](report-storage.md#data-source-trust-registration--custom-data-sources) for the full rules; omitting it throws `NonTrustedTypeDeserializationException`.

## JSON Schema Discovery

| Service | Why / When |
|---|---|
| `IWizardJsonDataSourceSchemaDiscoveryInterceptor` | The Data Source Wizard only scans the first `JsonDataSource.SchemaDiscoveryMaxItemCount` records (100 by default) of a JSON source to infer its schema — a field that first appears deeper in the file never shows up in the wizard. Implement this to raise that limit (or lower it, for very large files) specifically for wizard-driven schema discovery. No dedicated builder method — register via plain DI (`builder.Services.AddSingleton<...>()`). |

## Adding Data Sources in the Controller

Use this when the end user should see a data source **pre-configured** in the Data Source Wizard (e.g., a shared `Northwind` connection) instead of configuring one from scratch.

This builds on the base model-builder pattern (`IReportDesignerModelBuilder` →
`.Report(name).BuildModel()` → `return View(model)`, with a matching
`@model ReportDesignerModel` view) documented in
[controller-view-binding.md](controller-view-binding.md#report-designer) (Style 3).
The only addition here is a `.DataSources(...)` call that constructs a
`SqlDataSource` (a fluent SELECT query built with `SelectQueryFluentBuilder`) and
registers it under a name the Wizard will show:

```csharp
using DevExpress.DataAccess.Sql;

// ... inside the Designer action, on the builder chain from controller-view-binding.md:
var ds = new SqlDataSource("NWindConnectionString");
var query = SelectQueryFluentBuilder
    .AddTable("Products")
    .SelectAllColumnsFromTable()
    .Build("Products");
ds.Queries.Add(query);
ds.RebuildResultSchema();

var model = builder
    .Report("SalesReport")
    .DataSources(sources => sources.Add("Northwind", ds))  // ← the data-source delta
    .BuildModel();
return View(model);
```

In Razor Pages, the same `.DataSources(...)` call goes on the builder chain inside the page's `OnGet` handler — see [controller-view-binding.md](controller-view-binding.md#report-designer) for the `PageModel` version.

## See Also

- [report-storage.md](report-storage.md) — `IConnectionProviderFactory` and named connections for web farms; data source trust registration.
- [security.md](security.md) — protecting connection strings sent to the client; allowing custom SQL safely; multi-tenant SELECT filtering.
- [getting-started.md](getting-started.md) — Step 10's must-have connection-string provider registration in context of a full setup.
- [designer-customization.md](designer-customization.md) — Data Source Wizard client-side customization (`CustomizeWizard`, `WizardSettings`).

# Data Binding — Blazor Grid

When you need to: bind the Grid to different data sources; choose between in-memory, EF Core, server-mode, or custom binding; understand feature availability per binding mode.

## Data Binding Modes

| Mode | Best For | Large Datasets | WASM |
|---|---|---|---|
| **In-memory** (`List<T>`, `IEnumerable<T>`) | Small/medium collections | ❌ | ✅ |
| **IQueryable** (`DbSet<T>` via EF Core) | Medium datasets with deferred execution | ⚠️ | ✅ |
| **Server Mode** (`EntityInstantFeedbackSource`) | Large datasets (10,000+ rows) | ✅ | ❌ |
| **GridDevExtremeDataSource** | Web API / OData / custom REST | ✅ | ✅ |
| **GridCustomDataSource** | Full custom implementation | ✅ | ✅ |

## In-Memory Data Binding

Bind the `Data` property to any `IEnumerable<T>` or `IListSource`:

```razor
<DxGrid Data="@Items" KeyFieldName="Id">
    <Columns>
        <DxGridDataColumn FieldName="Name" />
        <DxGridDataColumn FieldName="Amount" />
    </Columns>
</DxGrid>

@code {
    List<Product> Items { get; set; }

    protected override void OnInitialized() {
        Items = ProductService.GetAll();
    }
}
```

## EF Core — IQueryable Binding

Bind directly to `IQueryable<T>` (e.g., a `DbSet<T>`). EF Core evaluates the query when the Grid requests data:

```razor
@inject IDbContextFactory<NorthwindContext> DbFactory
@implements IDisposable

<DxGrid Data="@Employees" KeyFieldName="EmployeeId">
    <Columns>
        <DxGridDataColumn FieldName="FirstName" />
        <DxGridDataColumn FieldName="LastName" />
    </Columns>
</DxGrid>

@code {
    NorthwindContext Db;
    IQueryable<Employee> Employees;

    protected override void OnInitialized() {
        Db = DbFactory.CreateDbContext();
        Employees = Db.Employees;
    }

    public void Dispose() => Db?.Dispose();
}
```

> **Note**: IQueryable binding does not support: interval grouping, custom sorting/grouping by display text, unbound columns.

## Server Mode (Large Datasets — Blazor Server only)

Use `EntityInstantFeedbackSource` (async, non-blocking) or `EntityServerModeSource` (synchronous) for 10,000+ rows. The Grid loads data on demand; all shaping is delegated to the database.

```razor
@using DevExpress.Data.Linq
@implements IDisposable

<DxGrid Data="@DataSource" KeyFieldName="EmployeeId">
    <Columns>
        <DxGridDataColumn FieldName="FirstName" />
        <DxGridDataColumn FieldName="LastName" />
    </Columns>
</DxGrid>

@code {
    EntityInstantFeedbackSource DataSource;
    NorthwindContext Db;

    protected override void OnInitialized() {
        Db = DbFactory.CreateDbContext();
        DataSource = new EntityInstantFeedbackSource(e => {
            e.KeyExpression = "EmployeeId";
            e.QueryableSource = Db.Employees;
        });
    }

    public void Dispose() {
        DataSource?.Dispose();
        Db?.Dispose();
    }
}
```

Required NuGet: `DevExpress.Data` (included with `DevExpress.Blazor`).
Required namespace: `DevExpress.Data.Linq`.

### Server Mode Limitations

The following features are **not supported** with `EntityServerModeSource` / `EntityInstantFeedbackSource`:

- Edit Cell mode
- Sorting/filtering by display text
- Custom sort/group algorithms
- Unbound columns (use `UnboundExpression` instead)
- Custom summaries (only `Finalize` stage fires)

## Feature Support Matrix

| Feature | In-Memory | IQueryable | Server Mode | Custom |
|---|---|---|---|---|
| Sorting | ✅ | ✅ | ✅ | ✅ |
| Grouping | ✅ | ✅ | ✅ | ✅ |
| Filtering | ✅ | ✅ | ✅ | ✅ |
| Interval grouping | ✅ | ❌ | ✅ | ❌ |
| Total/Group summaries | ✅ | ✅ | ✅ | ✅ |
| Unbound columns | ✅ | ❌ | ⚠️ | ❌ |
| Edit Row / Edit Form | ✅ | ✅ | ✅ | ✅ |
| Edit Cell | ✅ | ❌ | ❌ | ⚠️ |
| Custom summaries | ✅ | ❌ | ⚠️ | ❌ |
| WebAssembly | ✅ | ✅ | ❌ | ✅ |

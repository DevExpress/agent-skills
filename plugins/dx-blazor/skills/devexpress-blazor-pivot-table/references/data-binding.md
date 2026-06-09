# Data Binding — Blazor Pivot Table

When you need to: bind the Pivot Table to in-memory data or IQueryable; understand supported data types; design a pivot-ready data model.

## In-Memory Binding

Bind `Data` to any `IEnumerable<T>`:

```razor
<DxPivotTable Data="@Orders">
    <Fields>
        <DxPivotTableField Field="@nameof(Order.Category)" Area="PivotTableArea.Row" />
        <DxPivotTableField Field="@nameof(Order.OrderDate)" Area="PivotTableArea.Column"
                           GroupInterval="PivotTableGroupInterval.DateYear" />
        <DxPivotTableField Field="@nameof(Order.Revenue)" Area="PivotTableArea.Data"
                           SummaryType="PivotTableSummaryType.Sum" />
    </Fields>
</DxPivotTable>

@code {
    List<Order> Orders { get; set; }

    protected override async Task OnInitializedAsync() {
        Orders = await OrderService.GetAllAsync();
    }
}
```

## IQueryable Binding (EF Core)

Bind to `IQueryable<T>` for deferred evaluation — the pivot engine issues optimized queries:

```razor
@inject IDbContextFactory<SalesDbContext> DbFactory
@implements IDisposable

<DxPivotTable Data="@SalesQuery">
    <Fields>
        <DxPivotTableField Field="@nameof(Sale.Region)" Area="PivotTableArea.Row" />
        <DxPivotTableField Field="@nameof(Sale.Amount)" Area="PivotTableArea.Data"
                           SummaryType="PivotTableSummaryType.Sum" />
    </Fields>
</DxPivotTable>

@code {
    SalesDbContext Db;
    IQueryable<Sale> SalesQuery;

    protected override void OnInitialized() {
        Db = DbFactory.CreateDbContext();
        SalesQuery = Db.Sales;
    }

    public void Dispose() => Db?.Dispose();
}
```

## Data Model Design Tips

| Recommendation | Details |
|---|---|
| Use flat records | Each row should be one transaction/event; avoid nested objects |
| Include date fields as `DateTime` | Required for `PivotTableGroupInterval.DateYear/DateQuarter/DateMonth` |
| Use numeric value fields | Only numeric fields can go into the Data area with Sum/Average/Min/Max |
| Include dimension strings | Country, Region, Category — these are your Row/Column fields |
| Avoid nulls in key dimensions | Null values appear as a separate "(blank)" group |

## Supported Data Types

| .NET Type | Row / Column Area | Data Area |
|---|---|---|
| `string` | ✅ | ❌ (only Count) |
| `int`, `long`, `decimal`, `double` | ✅ | ✅ |
| `DateTime` | ✅ (with GroupInterval) | ❌ (only Count) |
| `bool` | ✅ | ❌ (only Count) |
| `Enum` | ✅ | ❌ (only Count) |
| `Guid` | ✅ | ❌ |

# Data Binding

## When to Use This Reference

Use when binding a report to any data source: plain .NET collections, DataTable/DataSet, DevExpress data sources (SqlDataSource, ObjectDataSource, JsonDataSource), or Entity Framework.

## Quick Reference

```csharp
// IList / List<T> / IEnumerable (simplest)
report.DataSource = products;      // List<Product>
// DataMember not required for single-level collections

// DataTable
report.DataSource = dataTable;

// DataSet (specify which table)
report.DataSource = dataSet;
report.DataMember = "Orders";

// DevExpress SqlDataSource (runtime)
var sql = new SqlDataSource("Northwind");
sql.Queries.Add(SelectQuery.Create("Products", ...));
report.DataSource = sql;
report.DataMember = "Products";
```

## Standard .NET Data Sources

### IList / List<T>

```csharp
var products = new List<Product> {
    new Product { Name = "Widget A", Price = 9.99m, Category = "Tools" },
    new Product { Name = "Widget B", Price = 14.99m, Category = "Parts" }
};
report.DataSource = products;
// Field binding: "[Name]", "[Price]", "[Category]"
```

### DataTable

```csharp
var table = new DataTable();
table.Columns.Add("Name", typeof(string));
table.Columns.Add("Price", typeof(decimal));
table.Rows.Add("Widget A", 9.99m);

report.DataSource = table;
// DataMember not required for DataTable directly
```

### DataSet

```csharp
var ds = new DataSet();
// ... populate ds
report.DataSource = ds;
report.DataMember = "Orders";    // table name within the DataSet
```

## DevExpress Data Sources

### ObjectDataSource

Bind to any class with a parameterless static/instance method:

```csharp
using DevExpress.DataAccess.ObjectBinding;

var ods = new ObjectDataSource {
    DataSource = typeof(ProductRepository),
    DataMember = "GetAll"        // static method name
};
report.DataSource = ods;
report.DataMember = "GetAll";
```

### SqlDataSource (runtime configuration)

```csharp
using DevExpress.DataAccess.Sql;

var sql = new SqlDataSource {
    ConnectionName = "Northwind",
    ConnectionParameters = new MsSqlConnectionParameters(
        "server=.;database=Northwind;trusted_connection=true")
};

var query = new CustomSqlQuery {
    Name = "Products",
    Sql = "SELECT ProductName, UnitPrice FROM Products"
};
sql.Queries.Add(query);
sql.Fill();

report.DataSource = sql;
report.DataMember = "Products";
```

### JsonDataSource

```csharp
using DevExpress.DataAccess.Json;

var json = new JsonDataSource {
    JsonSource = new UriJsonSource(new Uri("https://api.example.com/products"))
};
json.Fill();

report.DataSource = json;
report.DataMember = "products";   // JSON root key
```

## Master-Detail Binding

Use `DetailReportBand` with its own `DataSource` for nested data:

```csharp
// Master report binds to orders
report.DataSource = orders;      // List<Order>

var masterDetail = new DetailReportBand {
    DataSource = orders,
    DataMember = "Items"         // navigation property: Order.Items
};
report.Bands.Add(masterDetail);

var nestedDetail = new DetailBand();
masterDetail.Bands.Add(nestedDetail);
nestedDetail.HeightF = 20;

// Controls on nestedDetail bind to items fields: "[ItemName]", "[Qty]", etc.
```

## Filtering

Apply a filter string to limit records:

```csharp
report.FilterString = "[Category] = 'Tools'";
report.FilterString = "[Price] >= 10 And [Price] <= 100";
report.FilterString = "[OrderDate] >= ?StartDate";  // ? prefix = parameter reference
```

## Sorting

Sort via `DetailBand.SortFields`:

```csharp
detail.SortFields.Add(new GroupField("Price", XRColumnSortOrder.Descending));
```

## VB.NET

```vb
Dim products As New List(Of Product)()
products.Add(New Product() With { .Name = "Widget A", .Price = 9.99D })

report.DataSource = products
' Field binding: "[Name]", "[Price]"
```

## Notes

- `DataSource` is not serialized to `.repx`. Always re-assign it after `LoadLayoutFromXml`.
- `DataMember` is the path within the data source (table name for DataSet, collection name for complex objects).
- For Entity Framework: assign the `DbSet<T>` or a LINQ query result (`ToList()`) as `DataSource`.
- `ComponentStorage` can hold multiple data sources for reports with subreports.

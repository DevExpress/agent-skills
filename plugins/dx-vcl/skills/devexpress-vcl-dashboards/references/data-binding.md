# Data binding: connections and data sources

A dashboard gets data from **backend connection components** on the form (or a data module). The layout's data
source names the connection; the backend finds it by `DisplayName`. Verified on v26.1+.

| Data | Connection component (unit) | Layout data source |
|---|---|---|
| `TDataSet` (FireDAC, `TdxMemData`, ...) | `TdxBackendDataSetJSONConnection` (`dxBackend.ConnectionString.JSON.DataSet`) | `<JsonDataSource ConnectionName RootElement>` |
| JSON string or URL | `TdxBackendInMemoryJSONConnection` (`dxBackend.ConnectionString.JSON`) | `<JsonDataSource ConnectionName [RootElement]>` |
| SQLite (verified), other XPO databases (built into the backend) | `TdxBackendDatabaseSQLConnection` (`dxBackend.ConnectionString.SQL`) | `<SqlDataSource>` + `<Connection Name FromAppConfig="true"/>` |

Connection lookup by `DisplayName`: a connection owned by the dashboard's owner (the control's form, or the
`TdxDashboard`'s owner) first, then a connection owned by any data module, then any other one. Only active
connections count. `OnResolveConnection(ASender, ADisplayName, var AConnection)` can supply one. A name that is not
found raises `Connection "X" could not be resolved: OnResolveConnection handler returned nil`.

Set `DisplayName` explicitly. When it is empty, it follows the component `Name`, so renaming the component in the
IDE silently breaks the layout.

## `TDataSet` → `TdxBackendDataSetJSONConnection`

The connection serializes each dataset to JSON: `{"<DataSetAlias>":[{field: value, ...}, ...]}`.

```delphi
AConnection := TdxBackendDataSetJSONConnection.Create(Self);
AConnection.DisplayName := 'OrdersConnection';
AConnection.DataSets.Add('Orders', dsOrders);       // (DataSetAlias, TDataSource)
// or: AItem := AConnection.DataSets.Add as TdxBackendDataSetCollectionItem;
//     AItem.DataSource := dsOrders; AItem.DataSetAlias := 'Orders';
```
```cpp
TdxBackendDataSetJSONConnection *AConnection = new TdxBackendDataSetJSONConnection(this);
AConnection->DisplayName = "OrdersConnection";
AConnection->DataSets->Add("Orders", dsOrders);
```
```xml
<JsonDataSource Name="Orders" ConnectionName="OrdersConnection" RootElement="Orders" ComponentName="dsOrders" />
```

- **`DataSetAlias` must equal `RootElement`.** A mismatch is silent: the items show headers and no rows, and the
  measure caption loses its summary suffix ("Amount" instead of "Amount (Sum)").
- When `DataSetAlias` is empty it defaults to the DataSource/DataSet name.
- Field names are the `TField.FieldName`s; they become `DataMember`s in the layout (case-sensitive).
- The dataset must be open when the dashboard loads or exports. A `TdxMemData` needs persistent fields
  (`FieldDefs` alone are ignored); it has a hidden `RecId` field.
- The JSON data source needs no `<Schema>`: the backend reads the fields from the data.

## JSON → `TdxBackendInMemoryJSONConnection`

```delphi
AConnection := TdxBackendInMemoryJSONConnection.Create(Self);
AConnection.DisplayName := 'SalesJson';
AConnection.SetJSONValue('[{"Region":"Europe","Amount":120},{"Region":"America","Amount":80}]');
```
```cpp
AConnection->SetJSONValue("[{\"Region\":\"Europe\",\"Amount\":120}]");
```
```xml
<JsonDataSource Name="Sales" ConnectionName="SalesJson" ComponentName="dsSales" />            <!-- array at the root -->
<JsonDataSource Name="Sales" ConnectionName="SalesJson" RootElement="Sales" ComponentName="dsSales" />  <!-- {"Sales":[...]} -->
```

- An array at the root: no `RootElement`. An object: `RootElement` = the property that holds the array.
- A JSON file: read it with `TFile.ReadAllText(Path, TEncoding.UTF8)` and pass the text; or use a URI connection
  (MCP: `TdxBackendInMemoryJSONConnection`).
- Main-thread exports are fine.

## SQL → `TdxBackendDatabaseSQLConnection`

```delphi
AConnection := TdxBackendDatabaseSQLConnection.Create(Self);
AConnection.DisplayName := 'ShopConnection';
AConnection.ConnectionString := 'XpoProvider=SQLite;Data Source=shop.db';     // relative = exe folder
```
```xml
<SqlDataSource Name="Shop" ComponentName="sqlShop">
  <Connection Name="ShopConnection" FromAppConfig="true" />
  <Query Type="SelectQuery" Name="Sales">
    <Tables><Table Name="Sales" /></Tables>
    <Columns>
      <Column Table="Sales" Name="SaleDate" />
      <Column Table="Sales" Name="Amount" />
    </Columns>
  </Query>
  <ConnectionOptions CloseConnection="true" />
</SqlDataSource>
```
Items on a SQL data source name the query: `DataSource="sqlShop" DataMember="Sales"`.

- `<Connection Name="X">` links the data source to the connection component whose `DisplayName` is `X`. Write
  `FromAppConfig="true"`, as the Designer does.
- **Relative `Data Source` resolves against the exe folder**, not the current directory. SQLite ships inside the
  backend: no FireDAC, no `sqlite3.dll`.
- `<AllColumns Table="Sales" />` selects every column.
- A `DATETIME` column comes in as a date and supports `DateTimeGroupInterval`.
- Joins, query filters, query parameters and custom SQL follow the DevExpress Dashboard/DataAccess XML (MCP:
  `technologies: ["Dashboard"]`). A query filter bound to a dashboard parameter:
  ```xml
  <Query Type="SelectQuery" Name="Sales">
    <Parameter Name="pCountry" Type="DevExpress.DataAccess.Expression">(System.String)(?Country)</Parameter>
    <Tables><Table Name="Sales" /></Tables>
    <Columns><AllColumns Table="Sales" /></Columns>
    <Filter>[Sales.Country] = ?pCountry</Filter>
  </Query>
  ```
- Custom SQL (`<Query Type="CustomSqlQuery">`) requires `EnableCustomSql`; see MCP.

## Where to filter

| Need | Put it |
|---|---|
| User picks a value in your VCL UI (combo box) | A dashboard parameter + item `FilterString`, set `Parameters['P'].Value` from code ([dashboard-xml-cookbook.md](dashboard-xml-cookbook.md#parameters)) |
| User clicks in one item to filter others | `<InteractivityOptions MasterFilterMode="Multiple" />` on that item |
| Fixed restriction | Item `FilterString`, or the SQL query `<Filter>` |
| Only a subset should ever be loaded (large tables) | SQL query `<Filter>` with a query parameter |


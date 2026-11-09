# Data binding recipes (VCL v26.1+)

## Choosing a connection

The `.dfm` blocks, published fields and unit lists are in [design-time-dfm.md](design-time-dfm.md); this file adds what to choose and why.

| Data | Connection | Notes |
|---|---|---|
| SQLite, SQL Server, Azure SQL | `TdxBackendDatabaseSQLConnection` ([block](design-time-dfm.md#form-with-a-sql-database-connection-and-a-report)) | Built into the backend: no FireDAC and nothing to deploy. Queries live in the layout's SqlDataSource (select, custom SQL, stored procedures, relations). Custom SQL is off by default (`TdxReport.EnableCustomSql`); enable it only with database-level permissions |
| Data already in a `TDataSet` (FireDAC query, `TdxMemData`) | `TdxBackendDataSetJSONConnection` ([block](design-time-dfm.md#firedac-any-tdataset-through-a-dataset-connection)) | Each report preparation serializes every item to `{"<DataSetAlias>":[...]}` (nulls included); the dataset must be open |
| A JSON URL or string | `TdxBackendInMemoryJSONConnection` ([block](design-time-dfm.md#remote-or-inline-json)) | The backend downloads the URI at run time, so it needs network access |

- **SQL Server / LocalDB connection string:**
  `XpoProvider=MSSqlServer;Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=Northwind;Integrated Security=True;TrustServerCertificate=True;Persist Security Info=False;Encrypt=True;`
- **In-memory data defined in code.** Use a `TdxMemData` on the form with persistent fields, and fill it with `Append` +
  `FieldByName(...).Value` + `Post` in `FormCreate` (`troubleshooting.md` #17a, #17b). This works because serialization
  happens at preparation time, not at load time.
- **Typing pitfall:** SQLite `numeric` without precision becomes an integer in FireDAC. Add a MapRule to `dtDouble`
  (see `troubleshooting.md` #16).

## Choosing where to filter

| Scenario | Filter at | How |
|---|---|---|
| FireDAC source, per-tab data | FireDAC SQL parameter | `FDQuery1.ParamByName('CountrySqlParameter').Value := 'UK'; FDQuery1.Open; dxReportControl1.Active := True;` (query `ActiveStoredUsage` without `auRunTime`) |
| SQL connection, user-editable | Report parameter → query Expression parameter | `<Parameter ... Type="DevExpress.DataAccess.Expression">(System.String)(?CountryReportParameter)</Parameter>` plus `<Filter>`; from code: `Parameters['CountryReportParameter'].Value := 'UK'` |
| Small in-memory data | Report `FilterString` | e.g. `[Company] In (?pCompany)` |
| Detail rows per master row | Relation / Federation / subreport parameter | see `repx-dfm-layout-cookbook.md` |

## Recipe: show only the records selected in a DevExpress grid

Based on the public example `vcl-filter-report-preview-based-on-grid-selected-rows`. It stays correct after
sorting the grid and moving columns.

```pascal
procedure TMainForm.btnPreviewClick(Sender: TObject);
var
  i: Integer;
  ProductIDs: TStringList;
  RowIndex: Integer;
begin
  ProductIDs := TStringList.Create;
  try
    for i := 0 to gvProducts.Controller.SelectedRowCount - 1 do
    begin
      RowIndex := gvProducts.Controller.SelectedRows[i].RecordIndex;
      ProductIDs.Add(gvProducts.DataController.Values[RowIndex, gvProductsProductID.Index]);
    end;

    if ProductIDs.Count > 0 then
      dxReport1.FilterString := Format('[ProductID] In (%s)', [ProductIDs.CommaText]) // integer keys only; quote string keys, see troubleshooting #15a
    else
      dxReport1.FilterString := '';   // no selection = all records

    dxReport1.ShowViewer;
  finally
    ProductIDs.Free;
  end;
end;
```

- **`CommaText` is safe only for integer keys.** String keys must be single-quoted with `'` doubled. `CommaText`
  produces double quotes, which is a parser error (`troubleshooting.md` #15a).
- **The filter goes with the layout on every prepare,** so changing it between previews works.
- **`TdxReport.FilterString` replaces a filter stored in the layout** (`troubleshooting.md` #15c).
- **To see exactly what data is sent,** call `dxDataSetToJSONString(DataSet)` (unit `dxBackend.ConnectionString.JSON.DataSet`).
  A `TdxMemData` also sends its `RecId` field.

## Master-detail options

1. **SqlDataSource relation:** detail band `DataMember="Master.Relation"` (`repx-dfm-layout-cookbook.md`, "SqlDataSource: master-detail relation").
2. **FederationDataSource over JSON/FireDAC:** detail band `DataMember="MasterAlias.MasterAliasDetailAlias"` (`repx-dfm-layout-cookbook.md`, "FederationDataSource").
3. **Subreport:** a separate `TdxReport` + `ParameterBindings` + subreport `FilterString`. It renders the detail
   with its own layout.

## Setting report parameters from code

```pascal
repSub.Parameters.Count;                         // only if the layout has subreports (troubleshooting #12)
dxReport1.Parameters['OrderIDParameter'].Value := 11001;
dxReport1.ShowViewer;                            // or ExportToPDF / dxReportControl1.Active := True
```

- **Multi-value:** `Parameters['P'].Value := VarArrayOf(['A', 'B'])`. Don't combine it with `SelectAllValues="true"` (`troubleshooting.md` #13).
- **Collection lifecycle:** the `Parameters` collection is filled from the backend on first access, and
  `Layout` changes clear it.

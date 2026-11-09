# Design Time — VCL Reports in the `.dfm`

`.dfm` blocks for DevExpress VCL v26.1+. General `.dfm` syntax: [dfm-format-rules.md](dfm-format-rules.md).

## When You Need This

- Place report, viewer, and data-connection components on a form without the IDE
- Store a report layout (REPX) in the form file
- Wire FireDAC/TDataSet data, SQL databases, or JSON to a report at design time

## VCL Reports `.dfm` Rules

1. **The layout is a `Layout.Strings` list** on `TdxReport` and `TdxReportControl`: one string item per REPX line,
   written by `DefineProperties`, so it is not a published property you can set in the Object Inspector.
   - Generate the block with `scripts/Set-DfmLayout.ps1`; it applies the string rules from
     [dfm-format-rules.md](dfm-format-rules.md): `''`, `#NNNN` for non-ASCII, ` +` continuation.
   - Never hand-wrap long REPX lines.
2. **`ReportName` is required** on every `TdxReport` / `TdxReportControl`; an empty name never prepares.
3. **Connections are nested `object` blocks inside `TdxBackendDataConnectionManager`.** Dataset items are nested inside
   `TdxBackendDataSetJSONConnection`. Each nested component also needs a published field in the form class.
4. **`DisplayName` must equal the connection name used inside the layout.** When it equals the component `Name`, it is
   not written to the `.dfm`, and renaming the component renames it too. Write `DisplayName` explicitly with a value
   different from `Name`.
5. **`TdxReportControl` in an on-demand form:** set `Active = False` in the `.dfm` and switch it on in code after
   parameters or data are ready.

## Form with a SQL database connection and a report

```
object Form1: TForm1
  Caption = 'Orders'
  ClientHeight = 120
  ClientWidth = 320
  TextHeight = 15
  object dxBackendDataConnectionManager1: TdxBackendDataConnectionManager
    Left = 40
    Top = 8
    object NorthwindConnection: TdxBackendDatabaseSQLConnection
      DisplayName = 'Northwind'
      ConnectionString =
        'XpoProvider=MSSqlServer;Data Source=(localdb)\MSSQLLocalDB;Initia' +
        'l Catalog=Northwind;Integrated Security=True;TrustServerCertifica' +
        'te=True;Persist Security Info=False;Encrypt=True;'
    end
  end
  object dxReport1: TdxReport
    ReportName = 'OrderDetails'
    Layout.Strings = (
      '<?xml version="1.0" encoding="utf-8"?>'
      '<XtraReportsLayoutSerializer SerializerVersion="26.1.0.0" Ref="1' +
        '" ControlType="DevExpress.XtraReports.UI.XtraReport, DevExpress.' +
        'XtraReports.v26.1, Version=26.1.0.0, Culture=neutral, PublicKeyT' +
        'oken=b88d1754d700e49a" Name="OrderDetails" ...>'
      '  ...'
      '</XtraReportsLayoutSerializer>')
    Left = 240
    Top = 8
  end
end
```

```delphi
type
  TForm1 = class(TForm)
    dxBackendDataConnectionManager1: TdxBackendDataConnectionManager;
    NorthwindConnection: TdxBackendDatabaseSQLConnection;
    dxReport1: TdxReport;
  end;
// uses dxBackend, dxBackend.ConnectionString.SQL, dxReport
```

SQLite: `ConnectionString = 'XpoProvider=SQLite;Data Source=C:\...\nwind.db'`. A relative `Data Source` resolves against
the exe folder.

## FireDAC (any TDataSet) through a dataset connection

```
  object FDConnection1: TFDConnection
    Params.Strings = (
      'Database=C:\Users\Public\Documents\DevExpress VCL Demos\MegaDemos\Product Demos\ExpressReports\Data\devav.sqlite3'
      'DriverID=SQLite')
    LoginPrompt = False
    Left = 24
    Top = 8
  end
  object FDQuery1: TFDQuery
    Active = True
    Connection = FDConnection1
    SQL.Strings = (
      'SELECT * FROM CUSTOMERS')
    Left = 88
    Top = 8
  end
  object DataSource1: TDataSource
    DataSet = FDQuery1
    Left = 152
    Top = 8
  end
  object dxBackendDataConnectionManager1: TdxBackendDataConnectionManager
    Left = 240
    Top = 8
    object CustomersConnection: TdxBackendDataSetJSONConnection
      DisplayName = 'CustomersData'
      object CustomersItem: TdxBackendDataSetCollectionItem
        DataSource = DataSource1
        DataSetAlias = 'Customers'
      end
    end
  end
```

- **Fields:** `FDConnection1: TFDConnection; FDQuery1: TFDQuery; DataSource1: TDataSource;
  dxBackendDataConnectionManager1: TdxBackendDataConnectionManager; CustomersConnection: TdxBackendDataSetJSONConnection;
  CustomersItem: TdxBackendDataSetCollectionItem;`
- **Units:** `dxBackend, dxBackend.ConnectionString.JSON, dxBackend.ConnectionString.JSON.DataSet, cxClasses`, plus FireDAC
  and its SQLite driver units: `FireDAC.Phys.SQLite, FireDAC.Phys.SQLiteDef, FireDAC.Phys.SQLiteWrapper.Stat,
  FireDAC.Stan.Def, FireDAC.Stan.Async, FireDAC.DApt, FireDAC.Stan.ExprFuncs, FireDAC.VCLUI.Wait`.
  `FireDAC.Phys.SQLiteWrapper.Stat` links SQLite into the exe; without it FireDAC loads `sqlite3.dll`, which a
  customer machine without RAD Studio usually lacks. This is the one list; other files point here.
- **Built-in alternative:** for a SQLite, SQL Server or Azure SQL database, `TdxBackendDatabaseSQLConnection`
  (above) needs none of these units and no `sqlite3.dll`: the backend opens the database itself.
- **Layout data member:** the layout's `JsonDataSource` uses `ConnectionName="CustomersData"` and its data member is
  `Customers` (the `DataSetAlias`).
- **TdxMemData:** a `TdxMemData` source needs persistent fields: nested `TField` objects in its `.dfm` block. `FieldDefs`
  are ignored.

## Remote or inline JSON

```
    object CustomersJSON: TdxBackendInMemoryJSONConnection
      DisplayName = 'CustomersData'
      ConnectionString =
        'Uri=https://raw.githubusercontent.com/DevExpress-Examples/DataSo' +
        'urces/master/JSON/customers.json'
    end
```

Unit `dxBackend.ConnectionString.JSON`. The backend downloads the URI at run time.

## Embedded viewer (`TdxReportControl`) on an MDI child form

```
object ReportChildForm: TReportChildForm
  Caption = 'ReportChildForm'
  ClientHeight = 480
  ClientWidth = 800
  FormStyle = fsMDIChild
  Visible = True
  OnCreate = dxFormCreate
  TextHeight = 15
  object dxReportControl1: TdxReportControl
    Left = 0
    Top = 0
    Width = 800
    Height = 480
    Active = False
    Align = alClient
    ReportName = 'CustomersReport'
    TabOrder = 0
    Layout.Strings = (
      ...)
  end
end
```

```delphi
procedure TReportChildForm.dxFormCreate(Sender: TObject);
var
  G: TGUID;
begin
  CreateGUID(G);                                   // a separate layout/data scope per tab
  dxReportControl1.GroupName := GUIDToString(G);
end;

// main form button:
AForm := TReportChildForm.Create(Self);
AForm.dxReportControl1.Parameters['CountryReportParameter'].Value := 'UK';
AForm.dxReportControl1.Active := True;
```

Units: `dxReport.Control` (plus `dxForms` for `TdxForm`). The connection manager can live on the main form.

## Connections in a data module, layouts loaded at run time

This is the pattern of the DevExpress ExpressReports demo.
- **Connections live in a data module.** The data module holds a `TdxBackendDataConnectionManager` with nested
  `TdxBackendDatabaseSQLConnection` objects, for example `ReportsNWindConnectionString` with
  `DisplayName = 'NWindConnectionString'`. The `.dpr` creates the data module **before** the main form.
- **Connection lookup is not limited to one form.** A report on another form finds the connection by `DisplayName`,
  but a connection owned by the report's own form wins.
- **The form stores no layout.**
  - The form declares only `object dxReportControl: TdxReportControl` with `ReportName = 'Report'`, plus
    `object repSubReport: TdxReport` with `ReportName = 'SubReport'`.
  - Code switches layouts with `dxReportControl.Report.Layout.LoadFromFile(...)` and then
    `dxReportControl.Active := True`.
  - Subreport layouts are loaded or cleared the same way: `repSubReport.Layout.LoadFromFile` / `Layout.Clear`.

Use this pattern instead of `Layout.Strings` when one viewer shows many layouts, or when layouts are deployed as
`.repx` files.

## Subreport on the same form

```
  object repContacts: TdxReport
    ReportName = 'Contacts'
    Layout.Strings = (...)        // contains <XRSubreport ReportSourceUrl="ContactPhones">
  end
  object repContactPhones: TdxReport
    ReportName = 'ContactPhones'
    Layout.Strings = (...)
  end
```

The subreport is resolved by `ReportName` among the `TdxReport` components of the same owner with the same `GroupName`.

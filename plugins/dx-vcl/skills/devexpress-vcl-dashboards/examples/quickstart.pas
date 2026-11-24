unit quickstart;

{ VCL Dashboards quick start: a TdxMemData with orders, a DataSet connection, a dashboard control with a layout
  built in code, and an Export PDF button. }

interface

uses
  System.SysUtils, System.Classes, System.IOUtils, Vcl.Controls, Vcl.Forms, dxForms, Vcl.StdCtrls, Vcl.ExtCtrls, Data.DB,
  dxmdaset, dxBackend, dxBackend.ConnectionString.JSON, dxBackend.ConnectionString.JSON.DataSet, dxDashboard.Control;

type
  TQuickStartForm = class(TdxForm)          // DevExpress DPI-aware form (dxForms)
    pnlTop: TPanel;
    btnExport: TButton;
    procedure FormCreate(Sender: TObject);
    procedure btnExportClick(Sender: TObject);
  private
    FOrders: TdxMemData;
    FOrdersSource: TDataSource;
    FConnection: TdxBackendDataSetJSONConnection;
    FDashboard: TdxDashboardControl;
    procedure CreateData;
  end;

var
  QuickStartForm: TQuickStartForm;

implementation

{$R *.dfm}

const
  OrdersDashboardXml =
    '<?xml version="1.0" encoding="utf-8"?>' + sLineBreak +
    '<Dashboard>' + sLineBreak +
    '  <Title Text="Orders" />' + sLineBreak +
    '  <DataSources>' + sLineBreak +
    '    <JsonDataSource Name="Orders" ConnectionName="OrdersConnection" RootElement="Orders" ComponentName="dsOrders" />' + sLineBreak +
    '  </DataSources>' + sLineBreak +
    '  <Items>' + sLineBreak +
    '    <Grid ComponentName="gridCountries" Name="Orders by Country" DataSource="dsOrders">' + sLineBreak +
    '      <DataItems>' + sLineBreak +
    '        <Dimension DataMember="Country" DefaultId="DataItem0" />' + sLineBreak +
    '        <Measure DataMember="Amount" DefaultId="DataItem1">' + sLineBreak +
    '          <NumericFormat FormatType="Number" Precision="2" Unit="Ones" IncludeGroupSeparator="true" />' + sLineBreak +
    '        </Measure>' + sLineBreak +
    '      </DataItems>' + sLineBreak +
    '      <GridColumns>' + sLineBreak +
    '        <GridDimensionColumn><Dimension DefaultId="DataItem0" /></GridDimensionColumn>' + sLineBreak +
    '        <GridMeasureColumn><Measure DefaultId="DataItem1" /></GridMeasureColumn>' + sLineBreak +
    '      </GridColumns>' + sLineBreak +
    '      <GridOptions />' + sLineBreak +
    '    </Grid>' + sLineBreak +
    '  </Items>' + sLineBreak +
    '  <LayoutTree><LayoutGroup><LayoutItem DashboardItem="gridCountries" /></LayoutGroup></LayoutTree>' + sLineBreak +
    '</Dashboard>';

procedure TQuickStartForm.CreateData;
var
  ACountryField: TWideStringField;
  AAmountField: TFloatField;

  procedure AddOrder(const ACountry: string; AAmount: Double);
  begin
    FOrders.Append;
    FOrders.FieldByName('Country').AsString := ACountry;
    FOrders.FieldByName('Amount').AsFloat := AAmount;
    FOrders.Post;
  end;

begin
  FOrders := TdxMemData.Create(Self);
  ACountryField := TWideStringField.Create(FOrders);   // TdxMemData needs persistent fields; FieldDefs are ignored
  ACountryField.FieldName := 'Country';
  ACountryField.Size := 30;
  ACountryField.DataSet := FOrders;
  AAmountField := TFloatField.Create(FOrders);
  AAmountField.FieldName := 'Amount';
  AAmountField.DataSet := FOrders;
  FOrders.Open;
  AddOrder('Germany', 1855);
  AddOrder('France', 760);
  AddOrder('USA', 1640);
  FOrdersSource := TDataSource.Create(Self);
  FOrdersSource.DataSet := FOrders;
end;

procedure TQuickStartForm.FormCreate(Sender: TObject);
begin
  CreateData;
  FConnection := TdxBackendDataSetJSONConnection.Create(Self);   // owner = the dashboard's form
  FConnection.DisplayName := 'OrdersConnection';                 // = JsonDataSource ConnectionName
  FConnection.DataSets.Add('Orders', FOrdersSource);             // alias = JsonDataSource RootElement

  FDashboard := TdxDashboardControl.Create(Self);
  FDashboard.Parent := Self;
  FDashboard.Align := alClient;
  FDashboard.DashboardName := 'Orders';                          // required
  FDashboard.Layout.Text := OrdersDashboardXml;
end;

procedure TQuickStartForm.btnExportClick(Sender: TObject);
begin
  FDashboard.ExportToPDF(TPath.Combine(ExtractFilePath(ParamStr(0)), 'Orders.pdf'));
end;

end.

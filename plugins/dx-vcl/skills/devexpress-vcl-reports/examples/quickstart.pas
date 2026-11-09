unit quickstart;

{ DevExpress VCL Reports quick start: a TdxMemData dataset is exposed to the report through
  TdxBackendDataSetJSONConnection, a REPX layout is assigned in code, and the Report Viewer is shown.
  Everything is created in code; at design time the same components and the layout (Layout.Strings)
  are stored in the form's .dfm. Deploy WebView2Loader.dll next to the executable. }

interface

uses
  System.SysUtils, System.Classes, System.NetEncoding, Vcl.Forms, Vcl.StdCtrls, Data.DB,
  dxmdaset, dxBackend, dxBackend.ConnectionString.JSON.DataSet, dxReport;

type
  TForm1 = class(TForm)
    procedure FormCreate(Sender: TObject);
  private
    FData: TdxMemData;
    FDataSource: TDataSource;
    FReport: TdxReport;
    procedure CreateField(const AName: string);
    procedure AddCustomer(const AName, ACity: string);
    procedure CreateData;
    procedure CreateReport;
    procedure ShowReportClick(Sender: TObject);
  public
    property Report: TdxReport read FReport;
  end;

var
  Form1: TForm1;

implementation

{$R *.dfm}

const
  // JsonDataSource XML: ConnectionName = the connection's DisplayName; the array name = DataSetAlias
  JsonDataSourceXml =
    '<JsonDataSource Name="jsonDataSource1" ConnectionName="CustomersData"><Schema>' +
    '<Node Name="root" Selected="true" NodeType="Object"><Node Name="Customers" Selected="true" NodeType="Array">' +
    '<Node Name="Name" Type="System.String" Selected="true" NodeType="Property" />' +
    '<Node Name="City" Type="System.String" Selected="true" NodeType="Property" />' +
    '</Node></Node></Schema></JsonDataSource>';

function Base64Utf8(const S: string): string;
var
  ABytes: TBytes;
begin
  ABytes := TEncoding.UTF8.GetBytes(S);
  Result := TNetEncoding.Base64String.EncodeBytesToString(ABytes);
end;

procedure TForm1.FormCreate(Sender: TObject);
var
  AButton: TButton;
begin
  CreateData;
  CreateReport;
  AButton := TButton.Create(Self);
  AButton.Parent := Self;
  AButton.SetBounds(16, 16, 160, 32);
  AButton.Caption := 'Show Report';
  AButton.OnClick := ShowReportClick;
end;

procedure TForm1.AddCustomer(const AName, ACity: string);
begin
  // TdxMemData has a hidden first field (RecId): never use positional AppendRecord([...])
  FData.Append;
  FData.FieldByName('Name').AsString := AName;
  FData.FieldByName('City').AsString := ACity;
  FData.Post;
end;

procedure TForm1.CreateField(const AName: string);
var
  AField: TWideStringField;
begin
  AField := TWideStringField.Create(FData);
  AField.FieldName := AName;
  AField.Size := 50;
  AField.DataSet := FData;
end;

procedure TForm1.CreateData;
begin
  // TdxMemData ignores FieldDefs: create persistent fields (the .dfm equivalent is nested TField objects)
  FData := TdxMemData.Create(Self);
  CreateField('Name');
  CreateField('City');
  FData.Open;
  AddCustomer('Alfreds Futterkiste', 'Berlin');
  AddCustomer('Around the Horn', 'London');
  FDataSource := TDataSource.Create(Self);
  FDataSource.DataSet := FData;
end;

procedure TForm1.CreateReport;
var
  AConnection: TdxBackendDataSetJSONConnection;
begin
  AConnection := TdxBackendDataSetJSONConnection.Create(Self);
  AConnection.DisplayName := 'CustomersData';           // must match JsonDataSource ConnectionName
  AConnection.DataSets.Add('Customers', FDataSource);    // JSON: {"Customers":[...]}

  FReport := TdxReport.Create(Self);
  FReport.ReportName := 'CustomerList';                  // mandatory
  FReport.Layout.Text :=
    '<?xml version="1.0" encoding="utf-8"?>' + sLineBreak +
    '<XtraReportsLayoutSerializer SerializerVersion="26.1.0.0" Ref="1" ControlType="DevExpress.XtraReports.UI.XtraReport, DevExpress.XtraReports.v26.1, Version=26.1.0.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a" Name="CustomerList" Version="26.1" RequestParameters="false" DataMember="Customers" DataSource="#Ref-0">' + sLineBreak +
    '  <Bands>' + sLineBreak +
    '    <Item1 Ref="2" ControlType="TopMarginBand" Name="TopMargin" />' + sLineBreak +
    '    <Item2 Ref="3" ControlType="DetailBand" Name="Detail" HeightF="25"><Controls>' + sLineBreak +
    '      <Item1 Ref="4" ControlType="XRLabel" Name="lblName" SizeF="300,25" LocationFloat="0,0"><ExpressionBindings><Item1 Ref="5" EventName="BeforePrint" PropertyName="Text" Expression="[Name]" /></ExpressionBindings></Item1>' + sLineBreak +
    '      <Item2 Ref="6" ControlType="XRLabel" Name="lblCity" SizeF="300,25" LocationFloat="300,0"><ExpressionBindings><Item1 Ref="7" EventName="BeforePrint" PropertyName="Text" Expression="[City]" /></ExpressionBindings></Item2>' + sLineBreak +
    '    </Controls></Item2>' + sLineBreak +
    '    <Item3 Ref="8" ControlType="BottomMarginBand" Name="BottomMargin" />' + sLineBreak +
    '  </Bands>' + sLineBreak +
    '  <ComponentStorage>' + sLineBreak +
    '    <Item1 Ref="0" ObjectType="DevExpress.DataAccess.Json.JsonDataSource,DevExpress.DataAccess.v26.1" Name="jsonDataSource1" Base64="' + Base64Utf8(JsonDataSourceXml) + '" />' + sLineBreak +
    '  </ComponentStorage>' + sLineBreak +
    '</XtraReportsLayoutSerializer>';
end;

procedure TForm1.ShowReportClick(Sender: TObject);
begin
  FReport.ShowViewer;
end;

end.

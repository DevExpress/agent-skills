// DevExpress VCL Reports - Quick Start (C++Builder)
// A TdxMemData dataset is exposed to the report through TdxBackendDataSetJSONConnection, a REPX layout is
// assigned in code, and the Report Viewer is shown. At design time the same components and the layout
// (Layout.Strings) are stored in the form's .dfm. Deploy WebView2Loader.dll next to the executable.
//
// Files: ReportQuickStart.h (class declaration), ReportQuickStart.dfm (form, OnCreate = FormCreate).
// Add all three to a C++Builder VCL application and create the form with USEFORM / Application->CreateForm.

#include <vcl.h>
#pragma hdrstop

#include "ReportQuickStart.h"
#include <System.NetEncoding.hpp>
#include "dxmdaset.hpp"
#include "dxBackend.hpp"
#include "dxBackend.ConnectionString.JSON.DataSet.hpp"
#include "dxReport.hpp"

#pragma package(smart_init)
#pragma link "dxmdaset"
#pragma link "dxBackend"
#if defined(_WIN64)   // a dotted unit name needs the object-file extension
#pragma link "dxBackend.ConnectionString.JSON.DataSet.o"
#else
#pragma link "dxBackend.ConnectionString.JSON.DataSet.obj"
#endif
#pragma link "dxReport"
#pragma resource "*.dfm"
TfrmReportQuickStart *frmReportQuickStart;
__fastcall TfrmReportQuickStart::TfrmReportQuickStart(TComponent *Owner) : TForm(Owner) {}

// JsonDataSource XML: ConnectionName = the connection's DisplayName; the array name = DataSetAlias
static const String JsonDataSourceXml =
  "<JsonDataSource Name=\"jsonDataSource1\" ConnectionName=\"CustomersData\"><Schema>"
  "<Node Name=\"root\" Selected=\"true\" NodeType=\"Object\"><Node Name=\"Customers\" Selected=\"true\" NodeType=\"Array\">"
  "<Node Name=\"Name\" Type=\"System.String\" Selected=\"true\" NodeType=\"Property\" />"
  "<Node Name=\"City\" Type=\"System.String\" Selected=\"true\" NodeType=\"Property\" />"
  "</Node></Node></Schema></JsonDataSource>";

static String Base64Utf8(const String S)
{
  // Base64String (not Base64): no line breaks inside the attribute value
  // UTF8String holds exactly the encoded bytes (no terminating zero in Length())
  UTF8String AUtf8 = S;
  return TNetEncoding::Base64String->EncodeBytesToString(AUtf8.c_str(), AUtf8.Length());
}

void __fastcall TfrmReportQuickStart::FormCreate(TObject *Sender)
{
  CreateData();
  CreateReport();
  TButton *AButton = new TButton(this);
  AButton->Parent = this;
  AButton->SetBounds(16, 16, 160, 32);
  AButton->Caption = "Show Report";
  AButton->OnClick = ShowReportClick;
}

void __fastcall TfrmReportQuickStart::CreateField(const String AName)
{
  TWideStringField *AField = new TWideStringField(FData);
  AField->FieldName = AName;
  AField->Size = 50;
  AField->DataSet = FData;
}

void __fastcall TfrmReportQuickStart::AddCustomer(const String AName, const String ACity)
{
  // TdxMemData has a hidden first field (RecId): never use positional AppendRecord
  FData->Append();
  FData->FieldByName("Name")->AsString = AName;
  FData->FieldByName("City")->AsString = ACity;
  FData->Post();
}

void __fastcall TfrmReportQuickStart::CreateData()
{
  // TdxMemData ignores FieldDefs: create persistent fields
  FData = new TdxMemData(this);
  CreateField("Name");
  CreateField("City");
  FData->Open();
  AddCustomer("Alfreds Futterkiste", "Berlin");
  AddCustomer("Around the Horn", "London");
  FDataSource = new TDataSource(this);
  FDataSource->DataSet = FData;
}

void __fastcall TfrmReportQuickStart::CreateReport()
{
  TdxBackendDataSetJSONConnection *AConnection = new TdxBackendDataSetJSONConnection(this);
  AConnection->DisplayName = "CustomersData";           // must match JsonDataSource ConnectionName
  AConnection->DataSets->Add("Customers", FDataSource);  // JSON: {"Customers":[...]}

  FReport = new TdxReport(this);
  FReport->ReportName = "CustomerList";                  // mandatory
  FReport->Layout->Text =
    String("<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n") +
    "<XtraReportsLayoutSerializer SerializerVersion=\"26.1.0.0\" Ref=\"1\" ControlType=\"DevExpress.XtraReports.UI.XtraReport, DevExpress.XtraReports.v26.1, Version=26.1.0.0, Culture=neutral, PublicKeyToken=b88d1754d700e49a\" Name=\"CustomerList\" Version=\"26.1\" RequestParameters=\"false\" DataMember=\"Customers\" DataSource=\"#Ref-0\">\r\n"
    "  <Bands>\r\n"
    "    <Item1 Ref=\"2\" ControlType=\"TopMarginBand\" Name=\"TopMargin\" />\r\n"
    "    <Item2 Ref=\"3\" ControlType=\"DetailBand\" Name=\"Detail\" HeightF=\"25\"><Controls>\r\n"
    "      <Item1 Ref=\"4\" ControlType=\"XRLabel\" Name=\"lblName\" SizeF=\"300,25\" LocationFloat=\"0,0\"><ExpressionBindings><Item1 Ref=\"5\" EventName=\"BeforePrint\" PropertyName=\"Text\" Expression=\"[Name]\" /></ExpressionBindings></Item1>\r\n"
    "      <Item2 Ref=\"6\" ControlType=\"XRLabel\" Name=\"lblCity\" SizeF=\"300,25\" LocationFloat=\"300,0\"><ExpressionBindings><Item1 Ref=\"7\" EventName=\"BeforePrint\" PropertyName=\"Text\" Expression=\"[City]\" /></ExpressionBindings></Item2>\r\n"
    "    </Controls></Item2>\r\n"
    "    <Item3 Ref=\"8\" ControlType=\"BottomMarginBand\" Name=\"BottomMargin\" />\r\n"
    "  </Bands>\r\n"
    "  <ComponentStorage>\r\n"
    "    <Item1 Ref=\"0\" ObjectType=\"DevExpress.DataAccess.Json.JsonDataSource,DevExpress.DataAccess.v26.1\" Name=\"jsonDataSource1\" Base64=\"" +
    Base64Utf8(JsonDataSourceXml) + "\" />\r\n"
    "  </ComponentStorage>\r\n"
    "</XtraReportsLayoutSerializer>";
}

void __fastcall TfrmReportQuickStart::ShowReportClick(TObject *Sender)
{
  FReport->ShowViewer();
}

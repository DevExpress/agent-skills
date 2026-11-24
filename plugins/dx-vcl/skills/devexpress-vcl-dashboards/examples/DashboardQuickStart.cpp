// VCL Dashboards quick start: a TdxMemData with orders, a DataSet connection, a dashboard control with a layout
// built in code, and an Export PDF button.
#include <vcl.h>
#pragma hdrstop
#include <System.IOUtils.hpp>
#include "DashboardQuickStart.h"
#pragma package(smart_init)
#pragma link "dxForms"
#pragma link "dxmdaset"
#pragma link "dxBackend"
#pragma comment(lib, "dxDashboardsRS29")   // links the ExpressDashboards units (RS29 = RAD Studio 12)
#pragma resource "*.dfm"
TDashboardQuickStartForm *DashboardQuickStartForm;

static const String OrdersDashboardXml =
  "<?xml version=\"1.0\" encoding=\"utf-8\"?>\n"
  "<Dashboard>\n"
  "  <Title Text=\"Orders\" />\n"
  "  <DataSources>\n"
  "    <JsonDataSource Name=\"Orders\" ConnectionName=\"OrdersConnection\" RootElement=\"Orders\" ComponentName=\"dsOrders\" />\n"
  "  </DataSources>\n"
  "  <Items>\n"
  "    <Grid ComponentName=\"gridCountries\" Name=\"Orders by Country\" DataSource=\"dsOrders\">\n"
  "      <DataItems>\n"
  "        <Dimension DataMember=\"Country\" DefaultId=\"DataItem0\" />\n"
  "        <Measure DataMember=\"Amount\" DefaultId=\"DataItem1\">\n"
  "          <NumericFormat FormatType=\"Number\" Precision=\"2\" Unit=\"Ones\" IncludeGroupSeparator=\"true\" />\n"
  "        </Measure>\n"
  "      </DataItems>\n"
  "      <GridColumns>\n"
  "        <GridDimensionColumn><Dimension DefaultId=\"DataItem0\" /></GridDimensionColumn>\n"
  "        <GridMeasureColumn><Measure DefaultId=\"DataItem1\" /></GridMeasureColumn>\n"
  "      </GridColumns>\n"
  "      <GridOptions />\n"
  "    </Grid>\n"
  "  </Items>\n"
  "  <LayoutTree><LayoutGroup><LayoutItem DashboardItem=\"gridCountries\" /></LayoutGroup></LayoutTree>\n"
  "</Dashboard>";

__fastcall TDashboardQuickStartForm::TDashboardQuickStartForm(TComponent *Owner) : TdxForm(Owner)
{
}

void __fastcall TDashboardQuickStartForm::CreateData()
{
  FOrders = new TdxMemData(this);
  TWideStringField *ACountry = new TWideStringField(FOrders);   // TdxMemData needs persistent fields
  ACountry->FieldName = "Country";
  ACountry->Size = 30;
  ACountry->DataSet = FOrders;
  TFloatField *AAmount = new TFloatField(FOrders);
  AAmount->FieldName = "Amount";
  AAmount->DataSet = FOrders;
  FOrders->Open();
  const String ACountries[] = { "Germany", "France", "USA" };
  const double AAmounts[] = { 1855, 760, 1640 };
  for (int I = 0; I < 3; I++)
  {
    FOrders->Append();
    ACountry->AsString = ACountries[I];
    AAmount->AsFloat = AAmounts[I];
    FOrders->Post();
  }
  FOrdersSource = new TDataSource(this);
  FOrdersSource->DataSet = FOrders;
}

void __fastcall TDashboardQuickStartForm::FormCreate(TObject *Sender)
{
  CreateData();
  FConnection = new TdxBackendDataSetJSONConnection(this);   // owner = the dashboard's form
  FConnection->DisplayName = "OrdersConnection";             // = JsonDataSource ConnectionName
  FConnection->DataSets->Add("Orders", FOrdersSource);       // alias = JsonDataSource RootElement

  FDashboard = new TdxDashboardControl(this);
  FDashboard->Parent = this;
  FDashboard->Align = alClient;
  FDashboard->DashboardName = "Orders";                      // required
  FDashboard->Layout->Text = OrdersDashboardXml;
}

void __fastcall TDashboardQuickStartForm::btnExportClick(TObject *Sender)
{
  FDashboard->ExportToPDF(TPath::Combine(ExtractFilePath(ParamStr(0)), "Orders.pdf"));
}

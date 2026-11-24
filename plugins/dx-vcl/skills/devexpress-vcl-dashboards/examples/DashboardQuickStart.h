#ifndef DashboardQuickStartH
#define DashboardQuickStartH
#include <System.Classes.hpp>
#include <Vcl.Controls.hpp>
#include <Vcl.StdCtrls.hpp>
#include <Vcl.ExtCtrls.hpp>
#include <Vcl.Forms.hpp>
#include "dxForms.hpp"
#include <Data.DB.hpp>
#include "dxmdaset.hpp"
#include "dxBackend.hpp"
#include "dxBackend.ConnectionString.JSON.hpp"
#include "dxBackend.ConnectionString.JSON.DataSet.hpp"
#include "dxDashboard.Control.hpp"

class TDashboardQuickStartForm : public TdxForm   // DevExpress DPI-aware form (dxForms)
{
__published:
    TPanel *pnlTop;
    TButton *btnExport;
    void __fastcall FormCreate(TObject *Sender);
    void __fastcall btnExportClick(TObject *Sender);
private:
    TdxMemData *FOrders;
    TDataSource *FOrdersSource;
    TdxBackendDataSetJSONConnection *FConnection;
    TdxDashboardControl *FDashboard;
    void __fastcall CreateData();
public:
    __fastcall TDashboardQuickStartForm(TComponent *Owner);
};
extern PACKAGE TDashboardQuickStartForm *DashboardQuickStartForm;
#endif

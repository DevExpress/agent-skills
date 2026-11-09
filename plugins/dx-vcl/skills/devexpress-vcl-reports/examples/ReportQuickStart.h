#ifndef ReportQuickStartH
#define ReportQuickStartH
#include <System.Classes.hpp>
#include <Vcl.Controls.hpp>
#include <Vcl.StdCtrls.hpp>
#include <Vcl.Forms.hpp>
#include <Data.DB.hpp>
#include "dxmdaset.hpp"
#include "dxReport.hpp"
class TfrmReportQuickStart : public TForm
{
__published:
    void __fastcall FormCreate(TObject *Sender);
private:
    TdxMemData *FData;
    TDataSource *FDataSource;
    TdxReport *FReport;
    void __fastcall CreateField(const String AName);
    void __fastcall AddCustomer(const String AName, const String ACity);
    void __fastcall CreateData();
    void __fastcall CreateReport();
    void __fastcall ShowReportClick(TObject *Sender);
public:
    __fastcall TfrmReportQuickStart(TComponent *Owner);
};
extern PACKAGE TfrmReportQuickStart *frmReportQuickStart;
#endif

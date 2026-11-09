program ExportHarness;

{ Headless verification harness for DevExpress VCL Reports.
  Build it OUTSIDE the project under test (build-harness.cmd adds the project folder to the unit/resource paths).
  It creates the project's main form, waits until the message loop runs, executes RunChecks, writes
  result.txt (OK / EXCEPTION ...) into the folder passed as ParamStr(1), and terminates.
  Edit only the uses clause and RunChecks. }

uses
  Vcl.Forms, Vcl.ExtCtrls, System.SysUtils, System.Classes, System.Variants,
  uMainForm; // the project's form unit(s); add child-form units if needed

type
  TRunner = class
    Timer: TTimer;
    Log: TStringList;
    OutDir: string;
    Failed: Boolean;
    procedure Tick(Sender: TObject);
    procedure RunChecks;
    procedure AppException(Sender: TObject; E: Exception);
  end;

procedure TRunner.AppException(Sender: TObject; E: Exception);
begin
  // Never show a modal error dialog: it blocks the run and pops up on the user's desktop
  Failed := True;
  Log.Add('APP EXCEPTION ' + E.ClassName + ': ' + E.Message);
  Log.SaveToFile(OutDir + '\result.txt', TEncoding.UTF8);
end;

procedure TRunner.RunChecks;
begin
  // Examples (adapt):
  //   MainForm.dxReport1.ExportToHTML(OutDir + '\default.html');
  //   MainForm.dxReport1.ExportToImage(OutDir + '\default.png');
  //
  //   // Parameters; touch subreport components first if the layout has XRSubreport:
  //   // MainForm.repSub.Parameters.Count;
  //   MainForm.dxReport1.Parameters['OrderIDParameter'].Value := 11001;
  //   MainForm.dxReport1.ExportToHTML(OutDir + '\p11001.html');
  //
  //   // MDI: call the real button handlers, then export each child's TdxReportControl:
  //   MainForm.btnUKCustomersClick(nil); Application.ProcessMessages;
  //   (MainForm.ActiveMDIChild as TReportChildForm).dxReportControl1.ExportToHTML(OutDir + '\uk.html');
  MainForm.dxReport1.ExportToHTML(OutDir + '\report.html');
  MainForm.dxReport1.ExportToImage(OutDir + '\report.png');
  Log.Add('exported report.html, report.png');
end;

procedure TRunner.Tick(Sender: TObject);
begin
  Timer.Enabled := False;
  try
    RunChecks;
    if Failed then
      Log.Add('FAIL')
    else
      Log.Add('OK');
  except
    on E: Exception do
      Log.Add('EXCEPTION ' + E.ClassName + ': ' + E.Message);
  end;
  Log.SaveToFile(OutDir + '\result.txt', TEncoding.UTF8);
  Application.Terminate;
end;

var
  R: TRunner;
begin
  Application.Initialize;
  R := TRunner.Create;
  R.Log := TStringList.Create;
  R.OutDir := ExcludeTrailingPathDelimiter(ParamStr(1));
  if R.OutDir = '' then
    R.OutDir := ExtractFileDir(ParamStr(0));
  Application.OnException := R.AppException;
  // Create data modules first, in the same order as the project's .dpr
  Application.CreateForm(TMainForm, MainForm);
  R.Timer := TTimer.Create(nil);
  R.Timer.Interval := 500;
  R.Timer.OnTimer := R.Tick;
  Application.Run;
end.

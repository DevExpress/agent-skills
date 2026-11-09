# Non-interactive export (services, console apps, batch jobs)

## Minimal console export

```delphi
program ExportOrder;
{$APPTYPE CONSOLE}
uses
  System.SysUtils, System.IOUtils,
  dxBackend, dxBackend.Bundled, dxBackend.ConnectionString.SQL, dxReport, dxReport.Parameters;

var
  AConnection: TdxBackendDatabaseSQLConnection;
  AReport: TdxReport;
  AAppDir: string;
begin
  AAppDir := ExtractFilePath(ParamStr(0));
  AConnection := TdxBackendDatabaseSQLConnection.Create(nil);
  AReport := TdxReport.Create(nil);
  try
    AConnection.DisplayName := 'NWindConnectionString';   // = <Connection Name> in the layout
    AConnection.ConnectionString := 'XpoProvider=SQLite;Data Source=nwind.db';  // relative to the exe folder
    AReport.ReportName := 'OrderReport';
    AReport.Layout.LoadFromFile(TPath.Combine(AAppDir, 'Order.repx'));          // NOT relative to the current folder
    AReport.Parameters['OrderIdParameter'].Value := 11077;
    AReport.ExportToPDF(TPath.Combine(AAppDir, 'Order11077.pdf'));
  finally
    AReport.Free;
    AConnection.Free;
  end;
end.
```

- **No form, no `Application.Run`, no WebView2Loader.dll.** A main-thread `ExportTo*` works because the
  export wait loop calls `Application.ProcessMessages` itself.
- **Keep `dxBackend.Bundled` in `uses`.** Without it, the per-process backend folder in `%TEMP%` is left behind
  after every run. With it, the folder is removed when the process exits normally; a killed process (for example,
  one hung in a worker-thread export) leaves it, so purge stale folders in services and scheduled jobs.

C++Builder: the same steps, with `Report->Parameters->ParamByName["OrderIdParameter"]->Value = 11077;` and the
dotted units linked as object files ([troubleshooting.md](troubleshooting.md) #5e), and the file name built by
concatenation (`"Order_" + IntToStr(OrderId) + ".pdf"`): `Format("...", ...)` is ambiguous in C++ (#5g). **Link a C++ console app with
runtime packages**: statically linked, it crashes before `main` (#5f).

## Formats

`TdxReportExportFormat`: CSV, DOCX, HTML, Image, MHT, PDF, RTF, Text, XLS, XLSX.
- **CSV, HTML and Image** are continuous single-page output. Image is one tall PNG.
- **Suggested file name.** `ExportTo(Format, Stream, out FileName)` suggests `<ReportName>.<ext>`; for Text the extension is `.text`.
- **Format methods:** `ExportToPDF(FileName | Stream)`, `ExportToDOCX`, `ExportToXLSX`, ... and the generic `ExportTo(Format, ...)`.

## Batch runs

Backend start-up dominates a batch run. For batches, loop inside one process; creating a new
`TdxReport` per item costs nothing extra.

## Threads

- **Export from a worker `TThread` hangs forever** if the main thread doesn't process synchronization.
  The backend calls `TThread.Synchronize`/`Queue` and the export timeout is `INFINITE`.
- **In a service or console host,** keep the main thread calling `CheckSynchronize(100)` in a loop, or export on the main thread.

## Errors and edge cases

- A query that returns no rows produces an empty but valid document, with no error. Check the data yourself.
- A wrong connection name raises `EdxBackendException: Export failed: Connection "X" could not be resolved ...`.
- A failed export can leave a 0-byte output file. Check the exception, not only that the file exists.
- Each process unpacks the embedded backend to `%TEMP%\<exe name>\VCLBackend\<pid>` (~95 MB). Purge stale
  folders on servers.
- If one `.dproj` sets the same `DCC_ExeOutput` for Win32 and Win64, building one platform overwrites the other's exe.

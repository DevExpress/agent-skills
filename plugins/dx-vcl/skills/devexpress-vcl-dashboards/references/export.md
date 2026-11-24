# Exporting dashboards (button, console app, service)

Verified on DevExpress VCL v26.1+, Win32.

## Members

`TdxDashboardControl` and `TdxDashboard` have the same export methods:

| Method | Notes |
|---|---|
| `ExportToPDF(FileName)`, `ExportToXLSX`, `ExportToPNG`, `ExportToJPG`, `ExportToSVG` | File name or `TStream` overloads |
| `ExportTo(Format, FileName)` / `ExportTo(Format, Stream)` / `ExportTo(Format, Stream, out SuggestedFileName)` | `Format: TdxDashboardExportFormat` (unit `dxBackend`, scoped: `TdxDashboardExportFormat.PDF`) |

- The dashboard name is **required**: `DashboardName` (control) / `Name` (`TdxDashboard`). Without it the export
  raises "The value cannot be an empty string. (Parameter 'dashboardName')" and leaves a 0-byte file.
- The export applies the current `State` (master filter selections, parameter values) and `Parameters`.
- The first export starts the backend (about 1–2 s); later exports take a fraction of a second.
- File names: build them from `ExtractFilePath(ParamStr(0))` when the file should be "next to the program"; a bare
  name goes to the current directory, which differs when the app is started from a shortcut or another folder.
- The export of the whole dashboard has the viewer's layout. Numbers use the measure's `NumericFormat`; the default
  format abbreviates (`1.33K`). The export does not use `Language` or the layout's `CurrencyCulture`.

## Export from a button

Call the export directly (verified with a `TdxMemData` on a visible and on a hidden form, with `TdxDashboardControl`
and `TdxDashboard`):

```delphi
procedure TForm1.btnExportClick(Sender: TObject);
begin
  dxDashboardControl1.ExportToPDF(TPath.Combine(ExtractFilePath(ParamStr(0)), 'Orders.pdf'));
end;
```
```cpp
void __fastcall TForm1::btnExportClick(TObject *Sender)
{
  dxDashboardControl1->ExportToPDF(TPath::Combine(ExtractFilePath(ParamStr(0)), "Orders.pdf"));
}
```

## Export without UI (`TdxDashboard`)

```delphi
var
  ADashboard: TdxDashboard;
begin
  ADashboard := TdxDashboard.Create(nil);   // owner: the component that owns the connections, or nil
  try
    ADashboard.Layout.LoadFromFile(TPath.Combine(ExtractFilePath(ParamStr(0)), 'Sales.xml'));
    ADashboard.Name := 'Sales';                           // required
    ADashboard.Parameters['Country'].Value := 'France';   // after Layout: assigning Layout clears parameters
                                                          // C++: ADashboard->Parameters->ParamByName["Country"]->Value
    ADashboard.ExportToPDF(TPath.Combine(ExtractFilePath(ParamStr(0)), 'Sales_France.pdf'));
  finally
    ADashboard.Free;
  end;
end;
```

- `TdxDashboard` is a `TPersistent`, not a component: create and free it in code.
- Connections are looked up by `DisplayName`: those owned by the `TdxDashboard`'s owner first, then data modules, then
  any other connection. In a console app, create the connections in code (owner `nil` is fine).
- Keep `dxBackend.Bundled` in the `uses` of console and service apps.
- A console app needs no `WebView2Loader.dll` for export.

## Export format menu in the viewer

`ExportFormats` limits what the viewer's Export menu offers; it does not affect `ExportTo*` from code:

```delphi
dxDashboardControl1.ExportFormats := [TdxDashboardUIExportFormat.PDF];   // unit dxBackend
```
```cpp
dxDashboardControl1->ExportFormats = TdxDashboardUIExportFormats() << TdxDashboardUIExportFormat::PDF;
```
`.dfm`: `ExportFormats = [PDF]`. Values: `PDF`, `Excel`, `Image`.

`OnExport(ASender; var AFileName: string; var ACancel: Boolean)` fires when the user exports from the viewer: change
the file name or cancel. `ExportTo*` methods never raise `OnExport`.

Documentation: [Export functionality](https://docs.devexpress.com/VCL/405909/ExpressDashboards/export-functionality);
example with a SQL connection: [vcl-dashboards-non-interactive-export](https://github.com/DevExpress-Examples/vcl-dashboards-non-interactive-export).

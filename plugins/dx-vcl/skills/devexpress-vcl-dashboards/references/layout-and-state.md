# Layout and state: storage, Designer edits, user selections

A dashboard has two separate texts:

| | `Layout` | `State` |
|---|---|---|
| Content | Dashboard XML: data sources, items, parameters | JSON: the user's master filter selections, drill-down, parameter values |
| Changed by | Code, the Dashboard Designer (Save) | The viewer (clicks, filter elements, parameter dialog), `Parameters[...]` from code |
| Event (control) | `OnLayoutChanged` — after a Designer save | `OnStateChanged` — after a change in the viewer |
| `.dfm` | `Layout.Strings` | `State.Strings` |

Both are `TStringList`s (UTF-8), so `LoadFromFile`/`SaveToFile`/`Text`/`Assign` and BLOB streams work.

## Order matters

**Assigning `Layout` clears `Parameters` and `State`.** Always: layout first, then state and parameter values.

```delphi
dxDashboardControl1.Layout.LoadFromFile(ALayoutFile);
if FileExists(AStateFile) then
  dxDashboardControl1.State.LoadFromFile(AStateFile);       // after the layout
dxDashboardControl1.Parameters['Country'].Value := 'France';  // after the layout
```

## Layout in a file

```delphi
function TForm1.LayoutFileName: string;
begin
  Result := TPath.Combine(ExtractFilePath(ParamStr(0)), 'Sales.xml');   // "next to the program"
end;

procedure TForm1.btnSaveLayoutClick(Sender: TObject);
begin
  dxDashboardControl1.Layout.SaveToFile(LayoutFileName, TEncoding.UTF8);
end;

procedure TForm1.btnLoadLayoutClick(Sender: TObject);
begin
  dxDashboardControl1.Layout.LoadFromFile(LayoutFileName, TEncoding.UTF8);
end;
```
```cpp
String TForm1::LayoutFileName()
{
  return TPath::Combine(ExtractFilePath(ParamStr(0)), "Sales.xml");
}
void __fastcall TForm1::btnSaveLayoutClick(TObject *Sender)
{
  dxDashboardControl1->Layout->SaveToFile(LayoutFileName(), TEncoding::UTF8);
}
```
- `DashboardName` stays as it is; it does not come from the file.
- Loading a new layout reloads the dashboard on the control.
- Database: `TBlobField.AsString := Layout.Text` / `Layout.Text := TBlobField.AsString` (UTF-8 text field).

## Keep the user's Designer edits

The Designer opens from the viewer's menu or from code (`ShowDesigner`). Its **Save** sends the edited layout back
to the control (`Layout` changes) and fires the control's `OnLayoutChanged`. Nothing is written to disk by itself:
persist the layout in the handler and load it at start-up.

```delphi
procedure TForm1.FormCreate(Sender: TObject);
begin
  if FileExists(LayoutFileName) then
    dxDashboardControl1.Layout.LoadFromFile(LayoutFileName, TEncoding.UTF8);  // else: the layout from the .dfm
end;

procedure TForm1.dxDashboardControl1LayoutChanged(ASender: TdxCustomDashboardControl);
begin
  ASender.Layout.SaveToFile(LayoutFileName, TEncoding.UTF8);
end;
```
```cpp
// Form1.h, __published: void __fastcall dxDashboardControl1LayoutChanged(TdxCustomDashboardControl *ASender);
// The declaration and the definition both need __fastcall (else E4427).
void __fastcall TForm1::dxDashboardControl1LayoutChanged(TdxCustomDashboardControl *ASender)
{
  ASender->Layout->SaveToFile(LayoutFileName(), TEncoding::UTF8);
}
```
- Event signature: `procedure(ASender: TdxCustomDashboardControl) of object` (both `OnLayoutChanged` and
  `OnStateChanged`); `TdxCustomDashboardControl` is in `dxDashboard.Control`.
- `OnLayoutChanged` does **not** fire when code assigns `Layout`, so loading the file in `FormCreate` does not
  write it back.
- With `TdxDashboard.ShowDesigner` (modal), use `TdxDashboard.OnLayoutChanged` (`procedure(ASender: TdxDashboard)`).
  Note: on `TdxDashboard`, this event fires for **every** layout change, also from code.
- In a `.dfm`, assign the handler as `OnLayoutChanged = dxDashboardControl1LayoutChanged`.

## Remember the user's selections (state)

```delphi
function TForm1.StateFileName: string;
begin
  Result := TPath.Combine(ExtractFilePath(ParamStr(0)), 'Sales.state.json');
end;

procedure TForm1.FormCreate(Sender: TObject);
begin
  // the layout is already loaded (from the .dfm or a file)
  if FileExists(StateFileName) then
    dxDashboardControl1.State.LoadFromFile(StateFileName, TEncoding.UTF8);
end;

procedure TForm1.dxDashboardControl1StateChanged(ASender: TdxCustomDashboardControl);
begin
  ASender.State.SaveToFile(StateFileName, TEncoding.UTF8);
end;
```
- State JSON example: `{"Items":{"gridCountries":{"MasterFilterValues":[["France"]]}},"Parameters":{"Country":"France"}}`.
  Keys under `Items` are item `ComponentName`s.
- A state saved for one layout may not fit another: after the layout changes, discard the saved state.
- Exports apply the current state, so a restored selection also filters `ExportTo*` output.
- `OnStateChanged` fires for user changes in the viewer only, not for `State`/`Parameters` assigned from code.

# Troubleshooting: symptom → cause → fix


## Build and deployment

| # | Symptom | Cause | Fix |
|---|---|---|---|
| 1 | Viewer/Designer fails: `"WebView2Loader.dll" is not found in folder ...` | The DLL isn't next to the exe (design time uses `$(BDS)\Redist`, runtime uses the app folder). | Post-build copy from `$(BDS)\Redist\win32` or `\win64`. |
| 2 | Post-build "0 file(s) copied", or the DLL lands in the project root | `"$(OUTPUTDIR)"` ends with `\`, so `\"` escapes the closing quote. Some hand-made `.dproj` files also leave `$(OUTPUTDIR)` empty under command-line MSBuild, and an unquoted empty destination copies into the project folder. | `copy /Y "$(BDS)\Redist\win32\WebView2Loader.dll" ".\$(Platform)\$(Config)"` (match the project's `DCC_ExeOutput`; some examples use `.`). |
| 3 | Runtime `FireDAC ... driver not registered` / no wait cursor class | The SQLite driver units aren't linked when the connection is configured only in the DFM. | Add the FireDAC driver units listed in [design-time-dfm.md](design-time-dfm.md#firedac-any-tdataset-through-a-dataset-connection), or use `TdxBackendDatabaseSQLConnection` (built-in SQLite, no FireDAC). |
| 5a | `SQLite Error 14: 'unable to open database file'` at export | A relative `Data Source=nwind.db` in an XPO SQL connection resolves against `TdxBackend.SQLDataDirectory`, which defaults to the **exe folder**. The backend itself runs from `%TEMP%`. | Deploy the DB next to the exe, use an absolute path, or set `TdxBackend.SQLDataDirectory` before the backend starts. |
| 5d | `SQLite Error 14` with `Data Source=\|DataDirectory\|\Data\nwind.db` (as in the DevExpress demos) | The `\|DataDirectory\|` macro did not resolve in a custom host, even with `TdxBackend.SQLDataDirectory` set. | Use `Data Source=Data\nwind.db` (relative to the exe folder) or an absolute path. |
| 5b | App works from the IDE but "loses" files when started from a shortcut or another folder | Paths like `'data.dat'` / `'..\..\x.repx'` in `FormCreate` are relative to the **working folder**, while SQLite paths are relative to the exe folder: two different bases. | Build every path from `ExtractFilePath(ParamStr(0))`. |
| 5c | C++Builder: `Unable to open file 'VCL.THEMES.OBJ'` (Win32) / `'.O'` (Win64) | A generated `*.dxSettings.cpp` contains `#pragma link "Vcl.Themes"` / `"System.SysUtils"`; these units are in `vcl.lib`/`rtl.lib`, not separate object files. | Remove those `#pragma link` lines (or link with runtime packages). |
| 5e | C++Builder: `Fatal: Unable to open file 'DXBACKEND.CONNECTIONSTRING.SQL'` (or another dotted DevExpress unit) | The linker reads the last segment of a dotted `#pragma link` name as a file extension. | Link the object file explicitly: `#pragma link "dxBackend.ConnectionString.SQL.obj"` (Win32) / `".o"` (Win64), under `#if defined(_WIN64)`. |
| 5f | C++Builder console app (statically linked) exits with an access violation before `main` runs, even with no arguments | Unit initialization order: `cxControls` initializes before `dxUxTheme` has loaded the theme functions and calls a null `BufferedPaintInit`. VCL forms apps are not affected. | Link the console app with runtime packages (`UsePackages=true` and `LinkPackageImports` for the Debug and Release configurations) and deploy the DevExpress BPLs, or move the export into a VCL forms app. |
| 5g | C++Builder: `E5423: call to 'Format' is ambiguous` when building a file name or message | `Format("...", ARRAYOFCONST(...))` with a literal matches several `System::Sysutils::Format` overloads. | Concatenate strings: `"Order_" + IntToStr(OrderId) + ".pdf"`. |
## Report and component wiring

| # | Symptom | Cause | Fix |
|---|---|---|---|
| 6a | `Error when populating the datasource ... The connection string contains no XpoProvider key or the specified key is invalid` | The SqlDataSource `<Connection Name="X" />` has no `FromAppConfig="true"`, so the backend expects the connection string inside the layout instead of using the connection component named X. | Write `<Connection Name="X" FromAppConfig="true" />` with X = the connection's `DisplayName`. |
| 6 | `Connection "X" could not be resolved: OnResolveConnection handler returned nil` | The layout's `ConnectionName` / `<Connection Name>` doesn't equal the connection's `DisplayName`. | Make them equal. Or handle `OnResolveConnection` (the returned connection must have the same DisplayName and be Active). |
| 7 | A layout that used to work breaks after renaming a connection component | `DisplayName` defaults to `Name`, isn't stored in the DFM when it equals `Name`, and renaming the component renames it too. | Set `DisplayName` explicitly to a value different from `Name`. |
| 8 | `TdxReportControl` stays empty / report never prepares | Empty `ReportName`. The Designer sets it on Save. | Set `ReportName` in the DFM. |
| 9 | Report shows another tab's data, or one tab's layout changes another | Report components with the same `ReportName`+`GroupName` share one backend layout. | GUID `GroupName` per independent viewer instance (MDI child `OnCreate`). |
| 10 | DataMember / JSON array name unexpected (`DataSource1`) | `DataSetAlias` defaults to the assigned DataSource/DataSet name. | Set `DataSetAlias` explicitly; use it as the layout `DataMember`. |
| 11 | Subreport area empty or `Could not find report 'Sub'` | The `XRSubreport.ReportSourceUrl` doesn't match a `TdxReport.ReportName` on the same owner (and same `GroupName`). | Add the subreport `TdxReport` to the same form with that `ReportName`. |
## Parameters

| # | Symptom | Cause | Fix |
|---|---|---|---|
| 12 | `EdxBackgroundCommandServerException: Could not find report 'Sub' for session 'default'` when reading/writing `TdxReport.Parameters` from code | `Parameters` loads parameters by sending only the main layout. The backend then loads the full report and can't resolve the subreport, which isn't sent yet (`Prepare` sends subreports first, `Parameters` doesn't). | Touch the subreport first: `repSub.Parameters.Count;` then use `repMain.Parameters[...]`. Layouts without subreports are not affected. |
| 13 | Multi-value parameter set from code (`Parameters['P'].Value := VarArrayOf([...])`) is ignored at export: all values are used | `SelectAllValues="true"` wins over values passed from code during document creation. | Remove `SelectAllValues`. For "empty = all", use the optional pattern: `AllowNull="true"`, no default, filter `?P Is Null Or [Field] In (?P)`. |
| 13a | Need defaults for a multi-value parameter | The value list syntax is pipe-separated. | `ValueInfo="USA\|UK"`; it can be combined with `SelectAllValues="true"`. |
| 14 | After a lookup parameter comes from code, `Values.Count = 0` although the panel shows all selected | `SelectAllValues` is applied by the backend/web viewer, not reflected in the Delphi `Parameters` collection. | Don't infer "all selected" from `Values.Count` on the Delphi side. |
| 15 | Query parameter bound to a report parameter returns nothing or fails | The expression parameter needs the target type. | `<Parameter Name="@OrderID" Type="DevExpress.DataAccess.Expression">(System.Int32)(?OrderIDParameter)</Parameter>` |
## Filtering

| # | Symptom | Cause | Fix |
|---|---|---|---|
| 15a | `EdxBackendException: Parser error ... Invalid input character """` for a string-key filter | `TStringList.CommaText` wraps values that contain spaces in double quotes; criteria strings need single quotes. | Build literals yourself: `'` + `StringReplace(V, '''', '''''', [rfReplaceAll])` + `'`. Don't use `CommaText` for string keys. |
| 15b | `FilterString` with a misspelled field shows **all** rows, no error | An unknown field name in the criteria is silently ignored (a syntax error, by contrast, raises). | Check field names against the data source schema. |
| 15c | The filter stored in the layout stops working once code sets `TdxReport.FilterString` | `TdxReport.FilterString` **replaces** the layout's `FilterString` instead of combining with it. | Put all conditions into the code-side string. |
| 15d | Slow preview with a small selection over a big dataset | `TdxBackendDataSetJSONConnection` serializes the whole dataset on every prepare; `FilterString` filters in the backend. | Filter the dataset or query for large data; use `FilterString` for small in-memory data. |
## Non-interactive export

| # | Symptom | Cause | Fix |
|---|---|---|---|
| 16a | Export from a worker thread never completes | The backend synchronizes to the main thread, and the export wait is `INFINITE`. | Export on the main thread, or keep the main thread calling `CheckSynchronize`. |
| 16b | ~95 MB folders pile up in `%TEMP%\<exe>\VCLBackend\<pid>` | The embedded backend is unpacked per process; cleanup can fail silently. | Keep `dxBackend.Bundled` in `uses`; purge stale folders on servers. |
| 16c | Each run takes 6–8 s | Backend start-up (~4 s) per process. | Batch inside one process. |
| 16d | Empty PDF, exit code 0, for a non-existent key | An empty data set is a valid document. | Validate the input data yourself. |
## Data

| # | Symptom | Cause | Fix |
|---|---|---|---|
| 16 | Prices shown as integers (4.50 → 4) for SQLite (nwind.db) via FireDAC | A SQLite `numeric` column without precision maps to an integer field type in FireDAC. | `FormatOptions.OwnMapRules := True` + MapRule `NameMask='UnitPrice' SourceDataType=dtInt64 TargetDataType=dtDouble` (or `CAST(UnitPrice AS REAL)` in SQL). The XPO SQL connection isn't affected (Decimal). |
| 17 | JSON schema type mismatch / empty columns | Hand-written JsonDataSource schema types don't match the JSON. | Integers `System.Nullable`1[System.Int64]`, floats `System.Nullable`1[System.Double]`, text `System.String`. Unselected fields: `Selected="false"`. |
| 17a | `Field 'Name' not found` (or an AV in export) after `TdxMemData.FieldDefs.Add(...)` + `Open` | `TdxMemData` ignores `FieldDefs`; only its hidden `RecId` field exists. | Create persistent fields: `F := TWideStringField.Create(MemData); F.FieldName := 'Name'; F.DataSet := MemData;` (in the DFM: nested `TField` objects), then `Open`. |
| 17b | `'Alfreds' is not a valid integer value for field 'RecId'` | `TdxMemData` has a hidden first field `RecId`, so positional `AppendRecord([...])` is shifted by one. | Use `Append` + `FieldByName(...).Value := ...` + `Post`. |
| 18 | Remote JSON report is empty offline | `Uri=` connections are fetched by the backend at runtime. | Needs network access, or embed the JSON. |
## Layout storage (files and databases)

| # | Symptom | Cause | Fix |
|---|---|---|---|
| 18a | `OnLayoutChanged` never fires after loading or changing `Layout` from code | It fires only from `SetLayoutFromBackend`, i.e. a Designer Save or a `TdxReportControl` round trip, and only when the text changed. `Layout.LoadFromFile`/`Assign`/`Text :=` don't fire it (they clear `Parameters`). | Persist and refresh the UI yourself after loading from code. |
| 18b | Saved subreport layouts overwrite the main record or get lost | The Designer creates extra `TdxReport` objects for subreports and copies `OnLayoutChanged` to them. A handler that always saves `dxReport1` ignores which report changed. | Use `ASender` in `OnLayoutChanged` and store each layout by `ASender.ReportName`. |
| 18c | A REPX saved with `Layout.SaveToFile` gains a BOM, or LF becomes CRLF | `Layout` is a `TStringList` with `DefaultEncoding=UTF8`, `WriteBOM=True`, CRLF, `TrailingLineBreak=False`. A UTF-8+BOM+CRLF file round-trips byte-identically. | Expected. Set `Layout.WriteBOM := False` if needed; compare files after normalizing the BOM and line endings. |
| 18d | `EEncodingError: No mapping for the Unicode character...` on `Layout.LoadFromFile` | The file isn't UTF-8 (e.g. Windows-1252). The XML `encoding=` attribute is ignored. | Re-save as UTF-8, or `Layout.LoadFromFile(F, TEncoding.GetEncoding(1252))`. |
| 18e | Layout empty after changing the report name | `ReportName := ''` calls `Clear`, which empties `Layout`. | Set `ReportName` and `Layout` together; never blank the name temporarily. |
| 18f | Database storage: non-ASCII text garbled in the layout BLOB | `ftMemo` / ANSI fields. | Use a `TBlobField`; `Field.Assign(Report.Layout)` and `Report.Layout.Assign(Field)` round-trip UTF-8 (with BOM) losslessly. |
## Layout authoring and tooling

| # | Symptom | Cause | Fix |
|---|---|---|---|
| 19a | C++Builder: `Invalid data member 'X'` although the dataset and the connection are correct; the layout's Base64 data-source XML decodes with a trailing `\0` | `TEncoding::UTF8->GetBytes(String)` picks a character-array overload in C++ and encodes the terminating zero, so the backend cannot read the data-source XML and finds no connection name. | Encode a `UTF8String`: `UTF8String u = S; TNetEncoding::Base64String->EncodeBytesToString(u.c_str(), u.Length());` (as in `examples/ReportQuickStart.cpp`). |
| 19 | Garbled Cyrillic/accented text in the report after editing the DFM by hand | Non-ASCII must be `#NNNN` outside quotes in text DFMs. | Generate the block with `scripts/Set-DfmLayout.ps1`. |
| 21 | Copying data-source XML from demo layouts yields old assembly versions | Demo REPX files declare v24.2/v25.2. | They still load, but write `v26.1` in new layouts. |
| 22 | Detail band empty in a federation master-detail | Wrong `DataMember`. The federation relation is auto-named `<MasterAlias><DetailAlias>`. | `DataMember="CategoriesTable.CategoriesTableProductsTable"`, `DataSource="#Ref-<federation>"`. |
| 23 | Backend `Object reference not set to an instance of an object` after changing `Layout.Text` and then using `Parameters` in the same run | Re-sending a modified layout under the same `ReportName` after the backend already loaded it. | Set the final layout before the first backend access (or use a new `ReportName`). |
## Verification gotchas

| # | Symptom | Cause | Fix |
|---|---|---|---|
| 24 | `grep "John Smith"` finds nothing in an HTML export that visibly contains it | The HTML export separates words with `&nbsp;`, and writes Latin-1 accents as `&#NNN;` entities (Cyrillic stays raw UTF-8). | `sed 's/&nbsp;/ /g'` and decode entities before grepping, or use `ExportToText` for text checks. |
| 24a | A failed `ExportTo*` still leaves an output file | A 0-byte file is created before the export fails. | Check the size or the exception; don't check only that the file exists. |
| 24b | A test run hangs; an error dialog pops up on the desktop | An exception during form creation or `FormCreate`'s `ShowMessage` shows a modal dialog. | In harnesses, set `Application.OnException` to log, and check that required files exist before `CreateForm`. |
| 24d | PNG export shows "Page 1 of 1", no watermark, no page breaks | `ExportToImage` produces one continuous image. | Check paging, watermarks, and TOC page numbers in a PDF export (e.g. `pdftotext -layout`). |
| 24e | `ExportToImage` fails with `A generic error occurred in GDI+` and leaves a 0-byte PNG on long reports | Image size limits for one continuous page. | Export to PDF or HTML instead. |
| 24f | Non-ASCII characters garbled in `ExportToText` output | The Text export writes ANSI (code page 1252, no BOM), not UTF-8. | Read the file as ANSI, or check text via HTML/PDF. |
| 24g | A harness without a main form exits at once without exporting | `Application.Run` returns immediately when no main form exists. | Use `repeat Application.HandleMessage until Application.Terminated;` instead. |
| 24c | Garbled non-ASCII string literals in a harness `.dpr` | dcc32 reads a UTF-8 source without a BOM as ANSI. | Use `#$0418` character codes or save the source with a BOM. |
# Troubleshooting (symptom → cause → fix)

Verified on DevExpress VCL v26.1+ unless marked "from source".

## Data

| Symptom | Cause | Fix |
|---|---|---|
| Items show headers, no rows, no error; measure caption is "Amount" instead of "Amount (Sum)" | `DataSetAlias` ≠ the JSON data source's `RootElement` | Make them equal |
| Same, alias is right | Field name ≠ `DataMember` (case-sensitive); dataset closed or empty when the dashboard loads | Use the `FieldName`s; open the dataset first |
| `Connection "X" could not be resolved: OnResolveConnection handler returned nil` | No active connection with `DisplayName = 'X'` (renamed component, empty `DisplayName`, typo) | Set `DisplayName` to the layout's `ConnectionName` / `<Connection Name>`, or change the layout to the connection's name |
| SQLite: unable to open the database | Relative `Data Source` resolves against the exe folder | Deploy the file next to the exe, or use an absolute path |
| SQL dashboard exports without data on 26.1.3 | Fixed in 26.1.6 | Update |
| `Filter` attribute on `<JsonDataSource>` has no effect | Not supported | Filter in the items (`FilterString`) or in the SQL query |
| `TdxMemData` field not found | `FieldDefs` are ignored | Create persistent fields |

## Export

| Symptom | Cause | Fix |
|---|---|---|
| `The value cannot be an empty string. (Parameter 'dashboardName')`, 0-byte file | `DashboardName` / `Name` empty | Set it |
| The file is not "next to the program" | Bare file name → current directory | `TPath.Combine(ExtractFilePath(ParamStr(0)), ...)` |
| Exported numbers are abbreviated (`1.86K`) | Default measure format | `NumericFormat FormatType="Number" Unit="Ones" Precision="2" IncludeGroupSeparator="true"` |

## Layout, state, parameters

| Symptom | Cause | Fix |
|---|---|---|
| Parameter value or restored state is lost | Assigning `Layout` clears `Parameters` and `State` | Set them after the layout |
| `EdxBackendParameterException: Parameter "X" not found` | The parameter is not declared in the layout's `<Parameters>` | Add it to the layout; or use `FindParam` |
| Parameter set, nothing filtered | No item/data source uses `?X` | Add `FilterString="[DataItemN] = ?X"` to the items (data item IDs, not field names) |
| Designer changes lost after restart | Nothing persists the layout | Save `Layout` in `OnLayoutChanged`, load it at start-up |
| `OnLayoutChanged` / `OnStateChanged` never fire from code | They fire only for Designer saves / viewer interaction | Expected; persist directly where code changes the layout |
| User's master filter selection lost after restart | `State` is not persisted | Save `State` in `OnStateChanged`, load it after the layout |
| `MasterFilterMode="Single"` always selects a row | By design: single mode has one selected row | Use `Multiple` when "no selection" must be possible |
| Master filter does not affect an item | The item uses another data source, or `IgnoreMasterFilters="true"` | `IsMasterFilterCrossDataSource="true"` on the master, or the same data source |

## UI and deployment

| Symptom | Fix |
|---|---|
| `"WebView2Loader.dll" is not found in folder ...` | Deploy `WebView2Loader.dll` (matching bitness) next to the exe: post-build `copy /Y "$(BDS)\Redist\win32\WebView2Loader.dll" ".\$(Platform)\$(Config)"` |
| Control stays blank | `DashboardName` empty, `Layout` empty (often: the layout is assigned in `FormCreate`, but the `.dfm` has no `OnCreate = FormCreate`), or `Active = False` |
| The form fails to load after a `.dfm` edit | Run `scripts/check-form-load.ps1`; fix the property it names ([dfm-format-rules.md](dfm-format-rules.md)) |
| C++ linker: "Unable to open file 'DXDASHBOARD.CONTROL.OBJ'" (or `DXDASHBOARD.OBJ`, another `DX*.OBJ`) | `dxDashboardsRS<nn>.lib` is not linked: write `#pragma comment(lib, "dxDashboardsRS<nn>")` in the unit (`RS29` = RAD Studio 12), or add the library to the project. A `.dfm` written by hand does not add it |
| C++ linker: "Unable to open file 'DXBACKEND.CONNECTIONSTRING.SQL'" (another dotted unit name) | A `#pragma link` for a dotted unit: the linker reads the last segment as an extension. Remove it; `dxDashboardsRS<nn>.lib` and the backend library contain the unit |
| `EInvalidOperation: Cannot create form. No MDI forms are currently active` when the app closes | Create MDI children with `Application` as the owner, not the main form |
| C++: `reference to 'TdxBackendInMemoryJSONConnection' is ambiguous` (or another DevExpress class) | The header forward-declares the class in the global namespace (`class TdxDashboardControl;`), next to the real class from the unit's namespace. Remove the forward declaration and `#include` the unit's `.hpp` (`dxDashboard.Control.hpp`, `dxBackend.ConnectionString.JSON.hpp`) in the header |

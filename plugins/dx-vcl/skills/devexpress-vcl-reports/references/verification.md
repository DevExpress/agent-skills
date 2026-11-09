# Verify a Report Headlessly

A successful compile proves nothing about a report: the layout, the data binding and the connection names are checked
only when the backend prepares the document. Verify by exporting the report and inspecting the result. The Viewer and
Designer are WebView2 hosts of the **web** Document Viewer / Report Designer; clicking Win32 controls does not drive them.

## When You Need This

- Prove that a report shows the expected rows, totals, colors and grouping
- Check parameters, filters, master-detail and subreports without a person clicking through the viewer
- Check the live web UI (language, toolbar, page text) automatically

## Build from the command line

```bat
call "%ProgramFiles(x86)%\Embarcadero\Studio\23.0\bin\rsvars.bat"
msbuild MyReportApp.dproj /t:Build /p:Config=Release /p:Platform=Win32 /v:minimal
```

Adjust the `Studio\<version>` folder to the installed RAD Studio. In C++Builder, build the `.cbproj` the same way.
- **WebView2Loader.dll post-build step:** `copy /Y "$(BDS)\Redist\win32\WebView2Loader.dll" ".\$(Platform)\$(Config)"`.
  Match the project's exe output folder. Never quote a path that ends with `\`.
- **Running from Git Bash:** put the commands into a `.cmd` file and run `cmd //c "C:\full\path\build.cmd"`.

## Export harness

`scripts/harness/ExportHarness.dpr` + `scripts/harness/build-harness.cmd`.

1. Copy both files to a folder **outside** the project.
2. Edit the `uses` clause (the project's form and data-module units) and `RunChecks`:
   - call the real button handlers;
   - set parameters;
   - export.
3. Build: `cmd //c "<harness>\build-harness.cmd" "<project folder>"`. It runs `dcc32` with the DevExpress
   `Library\RS<nn>` paths and copies `WebView2Loader.dll`. Adjust the version folders inside it.
4. Copy `ExportHarness.exe` into the application's exe output folder and run `ExportHarness.exe "<output folder>"` from
   there: SQLite `Data Source` paths and the `localization\` folder resolve against the exe folder. Then read
   `result.txt`: it ends with `OK`, or with `FAIL` after the logged exceptions.

What the harness does for you:
- **Timer-driven exports.** It exports from a `TTimer` after `Application.Run` starts.
- **No modal dialogs.** It logs every exception through `Application.OnException`, so no error dialog blocks the run.
  An unhandled exception in `FormCreate` would otherwise hang the test.
- **Embedded viewers.** `TdxReportControl.ExportTo*` passes through to its internal report, so embedded viewers are verified without
  showing the WebView.
- **Handlers that call `ShowViewer`.** The viewer is modal. Assign `Report.OnViewerFormShow` to start a timer that sets
  `AForm.ModalResult := mrCancel`; the handler then returns and the filter it set stays in place.
- **Designer save flows.** `Report.ReportName := 'X'; Report.SetLayoutFromBackend(NewText);` does exactly what the
  Designer's Save does and fires the application's `OnLayoutChanged`.
- **Console and service apps.** Use `{$APPTYPE CONSOLE}` and call the export on the main thread
  ([non-interactive-export.md](non-interactive-export.md)).

### C++Builder

The harness is a Delphi program and does not build against C++ units.
- **Layout in a `.repx` file or in code:** verify with a small C++ console exporter that loads the same layout and
  connection and calls `ExportToPDF`/`ExportToText`, as in [non-interactive-export.md](non-interactive-export.md)
  (C++Builder paragraph: `ParamByName`, object-file links, runtime packages). Then check the output as below.
- **Layout only in a form's `.dfm`:** there is no tested C++ harness. Build, run `check-form-load.ps1`, and tell the
  developer that the report output is not verified.

## Check the output

- **HTML.**
  - Words are separated by `&nbsp;`, and Latin-1 accents are written as `&#NNN;`. Normalize before searching:
    `sed 's/&nbsp;/ /g' report.html | grep -oE "Name1|Name2"`.
  - `ExportToText` is simpler for text checks.
- **PNG / PDF.** Look at them to check layout, colors and grouping. Limits of `ExportToImage`:
  - it produces one continuous page: no page breaks, no watermark, "Page 1 of 1";
  - on long reports it can fail with a GDI+ error.

  Check paging, watermarks and TOC page numbers in a PDF; `pdftotext -layout` ships with Git for Windows
  (`/mingw64/bin`).
- **Text exports** are ANSI (code page 1252), not UTF-8.
- **Harness without a main form** (data-module-only apps): replace `Application.Run` with
  `repeat Application.HandleMessage until Application.Terminated;`.
- **Totals.** Compare them with a direct SQL query.
- **Failed exports.** A failed export can leave a 0-byte file, so check `result.txt`, not only that the file exists.
- **Tab isolation.** Export tab A, open tab B, export A again, and compare the two A exports byte for byte.
- **Culture.** `ExportTo*` uses `TdxReport.Language` as the document culture; export with `de-DE` / `fr-FR` to check
  formats.

## Live web UI checks (WebView2 DevTools protocol)

**Optional.** This needs Node.js 22 or later (it uses the built-in `WebSocket`; no npm packages).
Without Node.js, rely on the export checks above and ask the
developer to confirm the viewer UI by hand. All other scripts in this skill need only the Windows PowerShell that ships
with Windows and the RAD Studio command-line tools.

1. Make a harness that opens the viewer or designer from a timer. The window is modal, so the process keeps running.
2. Start it with remote debugging:
   `WEBVIEW2_ADDITIONAL_BROWSER_ARGUMENTS=--remote-debugging-port=9222 ./Harness.exe &`
3. Run `node scripts/inspect-viewer.mjs <outPrefix> [port]`. It:
   - waits for the `http://localhost:<port>/client/...` page target (`/client/viewer.html` for the viewer);
   - after 8 s, saves `<outPrefix>_info.json` and a screenshot `<outPrefix>.png`. The JSON holds:
     - the URL, `navigator.language`, and `document.cookie`, which carries `ReportName`/`GroupName`/`Language`;
     - the loaded localization JSON files;
     - `innerText`;
     - the toolbar `[title]` attributes.
4. Kill the process afterwards.

To drive the parameters panel, extend the script with `Runtime.evaluate` calls that set the editor values and click
Submit.

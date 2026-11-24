# Verifying a dashboard

A dashboard is verified by **exporting it and reading the result**, not by compiling it: wrong names, aliases or
filters compile fine and show an empty or wrong dashboard.

## 1. Build

```bat
call "%ProgramFiles(x86)%\Embarcadero\Studio\23.0\bin\rsvars.bat"
msbuild MyApp.dproj /t:Build /p:Config=Debug /p:Platform=Win32
```
C++Builder: `msbuild MyApp.cbproj ...`. Bare `dcc32` lacks the DevExpress library paths.

## 2. The form loads

For `.dfm` changes run the skill's form-load check against the built exe until it prints `PASS`:

```bat
powershell -ExecutionPolicy Bypass -File <this skill>\scripts\check-form-load.ps1 -Exe bin\MyApp.exe
```

## 3. Export and read the text

Export the dashboard to PDF and extract the text with `pdftotext` (ships with Git for Windows:
`%ProgramFiles%\Git\mingw64\bin\pdftotext.exe`):

```bat
pdftotext -layout Orders.pdf -
```

Check the rows and totals you expect (country names, sums). Remember:

- The default measure format abbreviates (`1.86K`); set `NumericFormat Unit="Ones"` when exact values matter.
- Chart values appear in the text only with `PointLabelOptions ShowPointLabels="true"`; axis labels always do.
- An empty grid ("Country | Amount" and nothing else) means no data: check `DataSetAlias`/`RootElement`, field names,
  and that the dataset is open.

How to export in a test:

- **Console or service code**: call the export directly ([export.md](export.md)).
- **A GUI app**: add a temporary command-line switch or a hidden test mode that exports and exits, or ask the
  developer to click the button. Do not leave windows of a test run on the developer's screen.

## 4. Things that cannot be checked by export

- The viewer UI itself (menus, the parameter dialog, Designer behavior) — check it by hand or over the WebView2
  DevTools protocol.
- Number formatting with `CurrencyCulture` (the export ignores it).

If you could not run the program, say exactly what is not verified.

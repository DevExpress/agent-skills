# Printing and Exporting

This reference covers the two export engines (data-aware and WYSIWYG), the one-call `ExportTo*` methods, the customization events on `XlsxExportOptionsEx`/`XlsExportOptionsEx`/`CsvExportOptionsEx`, master-detail printing, and the `PrintingSystem` for advanced reports.

## When to Use This Reference

- Exporting grid contents to XLSX, XLS, CSV, PDF, HTML, RTF, MHT, or image files.
- Choosing between data-aware export (formulas, group rows, format rules retained) and WYSIWYG (pixel-perfect snapshot).
- Customizing exported cells (substitute values, change formatting, merge cells, add header/footer rows).
- Including master-detail in print output.
- Building richer reports via `PrintableComponentLink` or XtraReports.

> **Scope:** this reference is about **printing and exporting** — a subtopic of the Data Grid skill, not general grid configuration. Most of what follows is **runtime (code) API**; see the table below for what can instead be set up front on the view.

## Declarative Options vs Runtime API

| Capability | Set up front on the view (`InitializeComponent` or startup code) | Runtime (code) | Notes |
|---|---|---|---|
| `view.OptionsPrint.*` (headers/footers, lines, `AutoWidth`, `PrintDetails`, `UsePrintStyles`) | ✅ yes | ✅ also settable later | Affects print **and** WYSIWYG export; not data-aware export |
| Print / Print Preview command in the UI | ✅ yes — a Ribbon/Bar **Print** item, or `PrintControl` | `ShowPrintPreview()` / `Print()` | |
| `ExportTo*` methods (XLSX/PDF/CSV/…) | ❌ no | ✅ code only | Always an explicit call |
| Export customization events (`CustomizeCell`, `AfterAddRow`, `CustomizeSheetHeader/Footer/Settings`, `BeforeExportTable`) | ❌ no | ✅ code only | Subscribe in code |
| `PrintingSystem` / `PrintableComponentLink` (composite reports) | ❌ no | ✅ code only | Advanced, code-based |
| Full reporting | (separate **XtraReports** product) | code or report designer | Out of this skill's scope |

**Rule of thumb:** if a request is "set print layout / show a print preview", much of it is covered by `OptionsPrint` + a Ribbon Print command. If it is "export to a file" or "customize the exported document", it is **runtime-only** code.

## Choosing a Printing/Export Approach

Pick the **simplest** option that meets the requirement — do not reach for event-based customization or `PrintingSystem` when a built-in setting or option does the job:

1. **One-call export** (`ExportTo*`) — a file with default layout. Start here.
2. **Export options object** (`XlsxExportOptionsEx`, `CsvExportOptionsEx`, …) — tweak sheet name, text vs raw values, encoding/separator, fixed header — **before** writing any custom event code.
3. **Data-aware export** — when Excel semantics matter (collapsible groups, native filter/summary formulas, retained format rules).
4. **WYSIWYG export / `OptionsPrint`** — when visual fidelity / print layout matters (exact colors, charts in cells, master-detail).
5. **Export customization events** — only when the built-in options above cannot produce the needed output.
6. **`PrintingSystem` / `PrintableComponentLink`** — composite documents (multiple controls, marginal headers, watermarks).
7. **XtraReports** — full, designed reports (out of scope here).

> **Applies per view type.** The data-aware vs WYSIWYG split below depends on the view: `GridView`/`BandedGridView`/`AdvBandedGridView` (and `TreeList`) support data-aware export; `CardView`/`LayoutView`/`TileView`/`WinExplorerView` always export WYSIWYG. Do not transfer data-aware advice to the always-WYSIWYG views.
>
> **Version note:** export/print APIs and option members evolve between DevExpress versions. This skill targets **v26.1**; confirm member availability for other versions via the DevExpress Docs MCP rather than assuming it is universal.

## Choose Export Mode

| Feature | Data-aware | WYSIWYG |
|---|---|---|
| Available views | `GridView`, `BandedGridView`, `AdvBandedGridView` (and `TreeList`) | All views (Card, Layout, Tile, WinExplorer too) |
| Grouping (collapsible) in XLS/XLSX | yes | no (rendered flat) |
| Native Excel filter + summary formulas | yes | no |
| Conditional formatting rules retained | yes | no (rendered as static colors) |
| Lookup column display values | yes | yes |
| Master-detail print | no | yes (set `OptionsPrint.PrintDetails`) |
| Charts/gauges/RTF in cells | dropped | rendered |
| Exact column widths/colors | approximate | exact |
| Performance | fast, lower memory | slower, higher memory |

```csharp
// Per call: override the default mode
gridControl1.ExportToXlsx("file.xlsx",
    new XlsxExportOptionsEx { ExportType = ExportType.DataAware });

// Or set globally:
DevExpress.Export.ExportSettings.DefaultExportType = ExportType.DataAware;
```

The default for `GridView`/`BandedGridView`/`AdvBandedGridView` exports to XLS/XLSX/CSV is **data-aware**. `Card`, `Layout`, `Tile`, and `WinExplorer` always use WYSIWYG. PDF/HTML/RTF/MHT always use WYSIWYG (via the printing engine).

## One-Call Export Methods

```csharp
gridControl1.ExportToXlsx("out.xlsx");
gridControl1.ExportToXls ("out.xls");
gridControl1.ExportToCsv ("out.csv");
gridControl1.ExportToPdf ("out.pdf");
gridControl1.ExportToHtml("out.html");
gridControl1.ExportToRtf ("out.rtf");
gridControl1.ExportToMht ("out.mht");
gridControl1.ExportToImage("out.png", ImageFormat.Png);
```

Each method has overloads that take a `Stream` and/or an options object.

`TreeList`, `VGridControl`, `PivotGridControl`, and most other DevExpress data-aware controls expose the same methods.

## Customize the Output (`XlsxExportOptionsEx`)

> **A last resort.** Reach for these customization events **only** when the plain options (`SheetName`, `TextExportMode`, `Encoding`, `Separator`, fixed header, data-aware mode) cannot produce the result. For most exports, the one-call method plus an options object is enough.

Pass an options instance with subscribed customization events:

```csharp
var options = new XlsxExportOptionsEx {
    SheetName = "Orders",
    ExportType = ExportType.DataAware,
    TextExportMode = TextExportMode.Text       // export display text, not raw values
};

options.CustomizeCell += (e) => {
    if (e.ColumnFieldName == "Status" && Equals(e.Value, "Overdue")) {
        e.Formatting = new XlCellFormatting {
            Font = new XlFont { Bold = true, Color = Color.White },
            Fill = XlFill.SolidFill(Color.Firebrick)
        };
    }
};

options.AfterAddRow += (e) => {
    // Merge cells of group rows
    if (e.DataSourceRowIndex < 0)
        e.ExportContext.MergeCells(new XlCellRange(
            new XlCellPosition(0, e.DocumentRow - 1),
            new XlCellPosition(5, e.DocumentRow - 1)));
};

options.CustomizeSheetHeader += (e) => {
    e.ExportContext.AddRow(new[] { new XlCellObject { Value = XlVariantValue.FromString("Company Confidential") } });
};

options.CustomizeSheetFooter += (e) => {
    e.ExportContext.AddRow(new[] { new XlCellObject { Value = XlVariantValue.FromString($"Generated {DateTime.Now:s}") } });
};

options.CustomizeSheetSettings += (e) => {
    e.ExportContext.SetFixedHeader(1, 4);   // freeze 1 row, 4 cols
};

gridControl1.ExportToXlsx("file.xlsx", options);
```

Events on `XlsxExportOptionsEx` / `XlsExportOptionsEx`:

- `CustomizeCell` — change value, formatting, hyperlink per cell.
- `AfterAddRow` — fires after each row (data or group) is appended; lets you merge cells, add notes, etc.
- `CustomizeDocumentColumn` — adjust column width, formatting, hide, or collapse the group containing the column.
- `CustomizeSheetHeader` / `CustomizeSheetFooter` — prepend/append rows.
- `CustomizeSheetSettings` — freeze panes, set zoom, print area, etc.
- `BeforeExportTable` — modify the native Excel table settings before write.

`CsvExportOptionsEx` exposes a subset; `TextExportMode`, `Encoding`, `Separator`, `QuoteStringsWithSeparators`, etc.

## Print

```csharp
gridControl1.ShowPrintPreview();          // requires DevExpress.Win.Printing
gridControl1.Print();
```

To customize the printed layout, configure `view.OptionsPrint`:

```csharp
view.OptionsPrint.PrintHeader        = true;
view.OptionsPrint.PrintFooter        = false;
view.OptionsPrint.PrintGroupFooter   = true;
view.OptionsPrint.PrintHorzLines     = true;
view.OptionsPrint.PrintVertLines     = true;
view.OptionsPrint.AutoWidth          = true;
view.OptionsPrint.PrintDetails       = true;     // print master-detail
view.OptionsPrint.ExpandAllDetails   = true;
view.OptionsPrint.UsePrintStyles     = true;
```

`OptionsPrint` settings affect both print and WYSIWYG export, not the data-aware export.

## Advanced: `PrintingSystem` / `PrintableComponentLink`

For composite reports that combine multiple controls or add headers, page numbers, watermarks, etc.:

```csharp
using var ps = new PrintingSystem();
var link = new PrintableComponentLink(ps) { Component = gridControl1 };
link.CreateMarginalHeaderArea += (s, e) => {
    e.Graph.DrawString("Confidential", new Font("Arial", 16),
        Brushes.Red, new RectangleF(0, 0, e.Bounds.Width, 30), StringFormat.GenericDefault);
};
link.CreateDocument();
link.ShowPreview();
link.ExportToPdf("composite.pdf");
```

For full reporting, use the XtraReports library and embed the grid via the printing component link.

## Document Post-Processing

DevExpress' `DocumentPostProcessing` lets you intercept generated documents to redact, watermark, sign, or rewrite content before final output. See the *Document Post-Processing* docs for details.

## Common Issues

- **Conditional formatting lost in export**: only data-aware export retains rules. Switch `ExportType`.
- **Currency column exports as `0.00\%`**: data-aware mode rewrites `"P"` display format as `"0.00\\%"`. Use a custom `DisplayFormat` if you need plain decimals.
- **Export hangs on very large grids**: turn off `AllowGrouping` and `ShowTotalSummaries` in `XlsxExportOptionsEx` to skip those passes; use `TextExportMode.Text` to skip lookup resolution.
- **Custom display text not exported**: set `column.ColumnEdit.ExportMode = ExportMode.DisplayText` (per column) or `options.TextExportMode = Text` (globally).
- **CSV missing columns**: hidden columns are skipped by default. Set `XlExportOptionsBase.ExportType` per requirements and enable `CsvExportOptionsEx.ExportHyperlinks` / column-by-column visibility.
- **Numbers/dates look wrong in CSV on another machine**: CSV is culture-sensitive (decimal separator, date format) and encoding-sensitive. Set `CsvExportOptionsEx.Encoding` (e.g. UTF-8) and `Separator` explicitly; for data-aware XLSX the cell types are preserved regardless of culture.
- **Print/Preview throws or is unavailable**: `ShowPrintPreview()`/`Print()` and PDF/HTML/RTF export require the **`DevExpress.Win.Printing`** package referenced.
- **Master-detail export/print is slow or huge**: `OptionsPrint.PrintDetails` + `ExpandAllDetails` expands every detail (WYSIWYG only). For large data, export only the focused view or disable `ExpandAllDetails`.
- **Print/WYSIWYG fonts or layout differ from the screen**: WYSIWYG rendering uses print metrics, so wrapping/widths can shift; tune `OptionsPrint.AutoWidth` and column widths, or use data-aware export when exact on-screen fidelity is not required.

## Source Material

- `articles/controls-and-libraries/data-grid/export-and-printing.md` (`xref:WindowsForms.115790`).
- `articles/controls-and-libraries/data-grid/export-and-printing/export-to-xls-and-xlsx-formats.md` (`xref:WindowsForms.17733`).
- `articles/controls-and-libraries/data-grid/export-and-printing/export-methods-and-settings.md` (`xref:WindowsForms.2536`).
- `articles/controls-and-libraries/data-grid/export-and-printing/advanced-grid-printing-and-exporting.md` (`xref:WindowsForms.114962`).
- `articles/controls-and-libraries/data-grid/examples/export-and-printing/how-to-customize-the-gridcontrols-data-aware-export-output.md` (`xref:WindowsForms.114095`).
- `articles/controls-and-libraries/data-grid/export-and-printing/document-post-processing.md` (`xref:WindowsForms.404683`).
- `api/DevExpress.XtraPrinting.XlsxExportOptionsEx.yml`.
- `api/DevExpress.XtraPrinting.XlsxExportOptionsEx.CustomizeCell.yml`.
- `api/DevExpress.XtraGrid.Views.Grid.GridOptionsPrint.yml`.

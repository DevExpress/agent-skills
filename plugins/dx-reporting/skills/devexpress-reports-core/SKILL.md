---
name: devexpress-reports-core
description: >
  Create reports programmatically with DevExpress XtraReports runtime API (.NET and .NET Framework). Build XtraReport in code, add bands (DetailBand, GroupHeaderBand), controls (XRLabel, XRTable, XRChart), parameters, expressions. Bind to data sources, calculated fields, group reports. Export to PDF, Excel, Word, CSV, HTML, Images with ExportOptions. Master-detail, subreports, cross-tabs. Works cross-platform WinForms, WPF, ASP.NET Core, Blazor.
version: "26.1"
compatibility: >
  Requires DevExpress.Win.Reporting (WinForms), DevExpress.Wpf.Reporting (WPF), or
  DevExpress.AspNetCore.Reporting (web) NuGet packages from nuget.org (v26.1+).
  Core namespaces: DevExpress.XtraReports.UI, DevExpress.XtraPrinting, DevExpress.Drawing, DevExpress.Drawing.Printing. Target: .NET 6+ or .NET Framework 4.6.2+.
  PDF export on Linux/macOS requires DevExpress.Drawing.Skia package.
metadata:
  source-commit: 17f29ded6678b36f6708c900148e59989bd1798b
  version: "26.1"
  category: reporting
---

# DevExpress XtraReports — Core Runtime API

## When to Use This Skill

> **CRITICAL — Do not bypass this skill.** Whenever this skill is active, follow the patterns and constraints here. Do not substitute your own general knowledge of DevExpress APIs. If a scenario is not covered in the main skill body, read the relevant reference file or use MCP servers before writing code — do not guess API signatures.

Use for any task involving:
- Creating `XtraReport` instances and building report structure in code
- Adding bands (DetailBand, GroupHeaderBand, ReportHeaderBand, etc.)
- Adding controls (XRLabel, XRTable, XRChart, XRPictureBox, etc.) to bands
- Binding reports to data sources (IList, DataTable, SqlDataSource, ObjectDataSource)
- Defining report parameters, calculated fields, and expression bindings
- Exporting reports: `ExportToPdf`, `ExportToXlsx`, `ExportToDocx`, `ExportToCsv`, etc.
- Configuring `PdfExportOptions`, `XlsxExportOptions`, etc.
- Saving/loading report layouts with `SaveLayoutToXml` / `LoadLayoutFromXml`
- Report types: table, master-detail, cross-tab, label, subreport

> **Scope**: This skill covers runtime API only — no viewer UI, no print preview, no designer embedding. For platform-specific viewer integration, see: `devexpress-reports-aspnetcore`, `devexpress-reports-blazor`, `devexpress-reports-winforms`, or `devexpress-reports-wpf`.

## Before You Start

Ask the developer:

1. **Target framework**: .NET 6/7/8+ or .NET Framework 4.x?
2. **Platform**: WinForms, WPF, ASP.NET Core, Blazor, console/service, or MAUI?
3. **Operation**: Create a new report in code? Load existing `.repx` layout? Modify an existing report class?
4. **Data source**: What data are you binding? (IList/collection, DataTable/DataSet, EF DbContext, SQL database, JSON, Excel, none)
5. **Output**: Export to file/stream? Which format(s)? Or display in a viewer?
6. **Existing NuGet setup**: Are DevExpress packages already installed, or starting fresh?

## Prerequisites & Installation

Add the platform NuGet package from `nuget.org`:

| Platform | NuGet Package |
|----------|---------------|
| WinForms | `DevExpress.Win.Reporting` |
| WPF | `DevExpress.Wpf.Reporting` |
| ASP.NET Core | `DevExpress.AspNetCore.Reporting` |
| ASP.NET Web Forms and MVC | `DevExpress.Web.Reporting` |
| Console / server | `DevExpress.Reporting.Core` (core only) |

All packages include the required namespaces (`DevExpress.XtraReports.UI`, `DevExpress.XtraPrinting`, `DevExpress.Drawing`) and export infrastructure.

> **Linux/macOS** (non-Windows): Add `DevExpress.Drawing.Skia` for PDF rendering support.

## Core Classes Overview

| Class | Namespace | Role |
|-------|-----------|------|
| `XtraReport` | `DevExpress.XtraReports.UI` | Root report object |
| `DetailBand` | `DevExpress.XtraReports.UI` | Repeating data rows (mandatory) |
| `ReportHeaderBand` | `DevExpress.XtraReports.UI` | Once at report start |
| `ReportFooterBand` | `DevExpress.XtraReports.UI` | Once at report end |
| `GroupHeaderBand` | `DevExpress.XtraReports.UI` | Group header |
| `GroupFooterBand` | `DevExpress.XtraReports.UI` | Group summary |
| `PageHeaderBand` | `DevExpress.XtraReports.UI` | Every page top |
| `PageFooterBand` | `DevExpress.XtraReports.UI` | Every page bottom |
| `DetailReportBand` | `DevExpress.XtraReports.UI` | Nested (master-detail) data |
| `XRLabel` | `DevExpress.XtraReports.UI` | Text / field display |
| `XRTable` / `XRTableRow` / `XRTableCell` | `DevExpress.XtraReports.UI` | Tabular layout |
| `XRPictureBox` | `DevExpress.XtraReports.UI` | Images |
| `XRChart` | `DevExpress.XtraReports.UI` | Charts |
| `XRCrossTab` | `DevExpress.XtraReports.UI` | Pivot tables |
| `XRSubreport` | `DevExpress.XtraReports.UI` | Embedded subreport |
| `XRPageInfo` | `DevExpress.XtraReports.UI` | Page number, date |
| `Parameter` | `DevExpress.XtraReports.Parameters` | Report parameter |
| `CalculatedField` | `DevExpress.XtraReports.UI` | Computed field |

## Quick Start — Report in Code

```csharp
using DevExpress.XtraReports.UI;

var report = new XtraReport {
    DataSource = GetProducts(), // IList, DataTable, etc.
    Name = "ProductReport"
};

var detail = new DetailBand();
report.Bands.Add(detail);
detail.HeightF = 30;

var nameLabel = new XRLabel();
detail.Controls.Add(nameLabel);
nameLabel.BoundsF = new RectangleF(0, 0, 200, 25);
nameLabel.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "[ProductName]"));

// Export
report.ExportToPdf("output.pdf");
```

See `examples/quickstart.cs` for a full working example with grouping, a page header, and PDF export.

> **Single-field display only**: The pattern above uses `XRLabel` for one field. For reports with multiple side-by-side columns, group subtotals, images, or charts — read `references/report-controls.md` (control types) and `references/report-bands.md` (band types) before writing code.

> For designer-backed reports (with `InitializeComponent()`): do not add a second `DetailBand`. See **Antipatterns** (AP7) and Constraint 9.

## Export API

All export methods have sync and async variants:

```csharp
// To file path
report.ExportToPdf("report.pdf");
report.ExportToXlsx("report.xlsx");
report.ExportToDocx("report.docx");
report.ExportToCsv("report.csv");
report.ExportToHtml("report.html");
report.ExportToImage("report.png");
report.ExportToRtf("report.rtf");
report.ExportToText("report.txt");
report.ExportToXls("report.xls");
report.ExportToMht("report.mht");

// To stream (preferred for web/API scenarios)
using var ms = new MemoryStream();
report.ExportToPdf(ms);

// Async (required in web apps)
await report.ExportToPdfAsync(ms);
```

**With options:**
```csharp
var pdfOptions = new DevExpress.XtraPrinting.PdfExportOptions {
    PageRange = "1-3",
    PdfACompatibility = DevExpress.XtraPrinting.PdfACompatibility.PdfA2b
};
report.ExportToPdf("report.pdf", pdfOptions);
```

📄 See `references/export.md` for all ExportOptions classes and their properties.

## Common Patterns

**Pattern 1 — Data-bound report:**
```csharp
report.DataSource = myList;       // IList, DataTable, or DevExpress data source
report.DataMember = "Orders";     // Table name (for DataSet) or empty for simple list
```

**Pattern 2 — Expression binding:**

> Do not use `DataBindings.Add` — this is the legacy binding mode.. See **Antipatterns** (AP1).

```csharp
// Specify an expression that calculates total value from UnitPrice and UnitsInStock fields.
label.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "[UnitPrice]*[UnitsInStock]"));
// Specify an expression that is rendered within the PrintOnPage event.
label.ExpressionBindings.Add(new ExpressionBinding("PrintOnPage", "Text", "'Printed'"));
```

> Expression function names are NOT .NET method names — see **Antipatterns** (AP2) and `references/expressions.md` for the complete catalogue.

**Pattern 3 — Report parameter:**
```csharp
// Create a date report parameter.
// Use an expression to specify the parameter's default value.
var dateParameter = new Parameter() {
    Name = "date",
    Description = "Date:",
    Type = typeof(System.DateTime),
    ExpressionBindings = { new BasicExpressionBinding("Value", "Now()") }
};
// Add the parameter to the report's Parameters collection.
report.Parameters.Add(dateParameter);
// Create a label and bind the label's Text property to the parameter value.
// Use the parameter's name to reference the parameter in the label's expression.
var dateLabel = new XRLabel();
report.Bands[BandKind.Detail].Controls.Add(dateLabel);
dateLabel.BoundsF = new RectangleF(0, 0, 200, 50);
dateLabel.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "?date"));
// Use the parameter in the report filter if needed: 
report.FilterString = "[OrderDate] >= ?date";
```

**Pattern 4 — Save/Load layout:**
```csharp
report.SaveLayoutToXml("layout.repx");  // .repx = XML serialization

var loaded = XtraReport.FromXmlFile("layout.repx");
```

**Pattern 5 — CreateDocument before export (optional, ensures data is prepared):**
```csharp
report.CreateDocument();           // sync — OK in desktop apps
// OR in web:
await report.CreateDocumentAsync();
report.ExportToPdf(outputStream);
```

**Pattern 6 — Tabular column layout with XRTable:**

> Use `XRTable` / `XRTableRow` / `XRTableCell` for every multi-column layout — data rows, header rows, and summary rows. See **Antipatterns** (AP3).

```csharp
var table = new XRTable();
detail.Controls.Add(table);
table.BeginInit();
var row = new XRTableRow();
table.Rows.Add(row);
var nameCell = new XRTableCell { WidthF = 450 };  // absolute column width in pixels
var priceCell = new XRTableCell { WidthF = 200 };  // 450 + 200 = 650 (equals table SizeF.Width)
row.Cells.Add(nameCell);
row.Cells.Add(priceCell);
nameCell.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "[ProductName]"));
priceCell.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "[UnitPrice]"));
table.SizeF = new SizeF(650, 25);  // total width = sum of cell WidthF values
table.EndInit();
// See references/report-controls.md for the full XRTable reference.
```

**Pattern 7 — Group or report summary using XRSummary:**

> Summary labels require `XRSummary`. See **Antipatterns** (AP5, AP6).

```csharp
// Pattern A — XRSummary.Func + FormatString (simplest for standard aggregates):
var countLabel = new XRLabel();
groupFooter.Controls.Add(countLabel);
countLabel.BoundsF = new RectangleF(0, 0, 200, 25);
countLabel.Summary = new XRSummary {
    Func = SummaryFunc.Count,        // .Sum, .Avg, .Max, .Min, .Count, etc.
    Running = SummaryRunning.Group,  // .Page / .Report
    FormatString = "Count: {0}"      // {0} is replaced by the calculated value
};

// Pattern B — XRSummary.Running + ExpressionBinding with sum*() for computed expressions:
var totalLabel = new XRLabel();
groupFooter.Controls.Add(totalLabel);
totalLabel.BoundsF = new RectangleF(0, 0, 200, 25);
totalLabel.Summary = new XRSummary { Running = SummaryRunning.Group };
totalLabel.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "sumSum([Price])"));
// Other sum*() functions: sumAvg([Field]), sumCount(), sumSum([Field]), sumMin([Field])
// See references/expressions.md for the full Summary Functions reference.
```

**Pattern 8 — Calculate available control width from page dimensions:**
```csharp
// Set Margins first, then compute availableWidth once and reuse it across all bands.
Margins = new DevExpress.Drawing.DXMargins(50, 50, 50, 50); // Left, Right, Top, Bottom
var availableWidth = PageWidthF - Margins.Left - Margins.Right;

// Single control filling the full band width:
label.BoundsF = new RectangleF(0, 0, availableWidth, 25);
table.SizeF   = new SizeF(availableWidth, 25);

// Multi-column table — proportional fractions of availableWidth:
cell1.WidthF = availableWidth * 0.67f;
cell2.WidthF = availableWidth * 0.33f;
// Rule: cell1.WidthF + cell2.WidthF must equal table.SizeF.Width exactly.
```

> Set full-band controls to `availableWidth`. See **Antipatterns** (AP9).

## Key Properties — XtraReport

| Property | Type | Description |
|----------|------|-------------|
| `DataSource` | `object` | Data source (IList, DataTable, DevExpress source) |
| `DataMember` | `string` | Table/collection path within the data source |
| `Bands` | `BandCollection` | All bands in the report |
| `Parameters` | `ParameterCollection` | Report parameters |
| `CalculatedFields` | `CalculatedFieldCollection` | Calculated fields |
| `FilterString` | `string` | Report-level data filter |
| `ExportOptions` | `ExportOptions` | Default export settings |
| `StyleSheet` | `XRControlStyleCollection` | Named styles |
| `ReportUnit` | `ReportUnit` | HundredthsOfAnInch or TenthsOfAMillimeter |
| `Margins` | `DXMargins` | Page margins (`DevExpress.Drawing`) |
| `PaperKind` | `DXPaperKind` | Paper size (`DevExpress.Drawing.Printing`) |
| `Landscape` | `bool` | Page orientation |

## Troubleshooting

| Symptom | Cause | Fix |
|---------|-------|-----|
| `DetailBand` required exception at render | No `DetailBand` added | Always add a `DetailBand` to `report.Bands` |
| Export file is empty / 0 bytes | `DataSource` null or no records | Verify data source returns data before export |
| `ko is not defined` at runtime (web) | Not relevant to core API | See `devexpress-reports-aspnetcore` skill |
| PDF export fails on Linux | Missing Skia package | Add `DevExpress.Drawing.Skia` NuGet package |
| `ExpressionBinding` has no effect | Binding added to wrong property name | Check property name is `"Text"`, `"Visible"`, etc. — case-sensitive |
| `LoadLayoutFromXml` loses data source | `.repx` does not store data | Re-assign `DataSource` after `LoadLayoutFromXml` |
| Build error: namespace not found | Missing `using` directive | Add `using DevExpress.XtraReports.UI;` and `using DevExpress.XtraReports.Parameters;` |
| `CreateDocument()` hangs in web app | Blocking call on async context | Use `await report.CreateDocumentAsync()` in web/API |
| Code uses `DataBindings.Add("Text", null, "Field")` | Legacy binding API | Replace with `ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "[Field]"))`. See Constraint 13. |
| Group footer label shows last value, or error XRE093 "no summary functions" | Expression uses `Sum([Field])` — not a valid DevExpress reporting function | Replace with `sumSum([Field])`; ensure `label.Summary.Running` is set. See Pattern 7 and `references/expressions.md` |
| Summary label displays expression literally or shows 0 / nothing | `XRSummary` not set on the label — `sumCount()`, `sumSum()` etc. were used in `ExpressionBinding` without `label.Summary` | Assign `label.Summary = new XRSummary { Running = SummaryRunning.Group }` (Pattern A or B). `XRSummary` is always required. See Pattern 7. |
| Expression binding silently ignored or runtime error with unrecognized function | Expression function name guessed from C# knowledge (e.g., `Format()`, `String.Format()`, `ToString()`) — no such function in DevExpress expression language | Use `FormatString(format, value)` for formatting; check `references/expressions.md` for the full function list. See Pattern 2. |
| `System.Exception: Incorrect band type` at report constructor | A second instance of a singleton band (`DetailBand`, `TopMarginBand`, `BottomMarginBand`) was added via `Bands.Add()` after the singleton band was already created previously | Remove the `Bands.Add()` call; get the existing band from the designer field (e.g., `detailBand1`) and configure it directly. See Constraint 9. |

## Constraints & Rules

1. **Always add DetailBand** for newly created reports: `XtraReport` requires at least one `DetailBand` in `Bands` to render.
2. **Never mix package versions**: All DevExpress NuGet packages in a project must be the same version (e.g., all `26.1.*`).
3. **Namespace imports**: Always include `using DevExpress.XtraReports.UI;`. Never assume it exists.
4. **ExpressionBinding property names are case-sensitive**: `"Text"` not `"text"`.
5. **DataSource is not serialized**: `.repx` files store layout only. When loading with `LoadLayoutFromXml`, re-assign `DataSource` explicitly. When using `XtraReport.FromXmlStream()`, the concrete subclass is restored so constructor-assigned data sources are preserved — but any runtime-assigned data must still be re-assigned.
6. **Async in web**: Use `await report.CreateDocumentAsync()` and `await report.ExportToPdfAsync()` in ASP.NET Core and Blazor.
7. **No destructive changes**: When modifying existing report classes, preserve existing band/control structure; only add or modify what is required.
8. **Verify build**: Always run `dotnet build` and confirm 0 errors before reporting task complete.
9. **Singleton bands in designer-backed reports**: `XtraReport` enforces exactly one `DetailBand`, one `TopMarginBand`, and one `BottomMarginBand`. In a designer-backed partial class (one with `InitializeComponent()`), these bands **already exist**. Calling `Bands.Add(new DetailBand())` will throw `System.Exception: Incorrect band type` at runtime. **Rule**: Reuse the existing singleton bands declared in the designer file (e.g., `detailBand1`). Only call `Bands.Add(...)` for band types that may appear multiple times (e.g., `GroupHeaderBand`, `PageFooterBand`) **and** that `InitializeComponent()` did not already add.
10. **Project structure — designer file class ordering**: When adding helper or model classes alongside a report class, **never place them before the `XtraReport` subclass** in the same `.cs` file. Visual Studio requires the designed class to be the first class in its file. Place model/helper classes in a dedicated separate file (e.g., `Model/Product.cs`).
11. **Project structure — respect existing folders**: Before creating a new folder for model or helper classes, inspect the existing project structure. If a folder already exists for models (e.g., `Model`, `Models`, `Data`) or reports (e.g., `Reports`, `PredefinedReports`), place new files inside it and match its name and namespace exactly. Never create a parallel folder with a similar name.
12. **Tabular layout must use XRTable**: **Any multi-column layout** — data rows in `DetailBand`, static column header rows in `GroupHeaderBand` or `PageHeaderBand`, summary rows in `GroupFooterBand` — must use `XRTable` / `XRTableRow` / `XRTableCell`. Never build a helper that positions `XRLabel` controls at calculated `x` offsets. Never simulate a table with multiple `XRLabel` controls at absolute X positions. This constraint applies to header rows and static text rows equally, not only to data-bound rows. See Pattern 6.
13. **Never use `DataBindings`**: `DataBindings` is the legacy binding mode. For new reports, always use `ExpressionBindings` with `new ExpressionBinding(eventName, propertyName, expression)`. Generating `DataBindings.Add(...)` is not recommended unless maintaining legacy reports.
14. **Set size/position properties AFTER adding to parent — never in object initializers**: Always call `Bands.Add(band)` before setting `band.HeightF`, and always call `Controls.Add(control)` before setting `control.BoundsF`, `control.LocationF`, or `control.SizeF`. Report objects inherit the parent's measure unit when added to a parent; sizes assigned before this point will be silently recalculated and produce incorrect layout. **Other properties** (Text, Font, TextAlignment, ForeColor, ExpressionBindings, GroupFields, etc.) may be set at any time, including in object initializers. ✅ Correct: `var label = new XRLabel { Text = "X", Font = … }; band.Controls.Add(label); label.BoundsF = …;` ❌ Wrong: `var label = new XRLabel { BoundsF = …, Text = "X" }; band.Controls.Add(label);`
15. **Content properties are mandatory — never omit them**: Some controls have one or more essential content properties that hold the actual data to be displayed. `XRLabel` requires `Text`, `XRPictureBox` requires `ImageSource` or `ImageUrl`, `XRBarCode` requires `Text` or `BinaryData`, `XRCheckBox` requires `CheckBoxState`, `XRGauge` requires `ActualValue`, `XRRichText` requires `Rtf` or `Html`, etc. **These are not optional styling tweaks** — without them the control will be invisible or non-functional. Always bind content properties via `ExpressionBindings` or assign values directly. See `references/report-controls.md` for each control's content property requirements.

## Antipatterns

The following patterns produce incorrect output, runtime errors, or layout bugs. Never generate code matching these shapes.

| # | Antipattern | Correct Pattern |
|---|-------------|----------------|
| AP1 | `label.DataBindings.Add("Text", null, "Field")` — legacy binding mode | `label.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "[Field]"))`. See Constraint 13. |
| AP2 | Expression function names guessed from C#/LINQ/SQL (e.g., `Format(...)`, `String.Format(...)`, `ToString()`)` in expression strings) | Look up every function name in `references/expressions.md` before writing it. DevExpress expression functions differ from C#/LINQ/SQL equivalents. |
| AP3 | Multiple `XRLabel` controls at calculated absolute X positions to simulate columns | Use `XRTable` / `XRTableRow` / `XRTableCell` for every multi-column layout — data rows, static header rows, and summary rows alike. See Pattern 6 and Constraint 12. |
| AP4 | `cell.Weight` used for column sizing (`XRTableCell.Weight`) | Use `cell.WidthF` (absolute units). The sum of all `WidthF` values in a row must equal `table.SizeF.Width`. |
| AP5 | `Sum([Price])` or `Count()` in an `ExpressionBinding` on a summary label | Use the `sum*()` family: `sumSum([Price])`, `sumCount()`, `sumAvg([Price])`, `sumMin([Price])`, `sumMax([Price])`. `Sum([Price])` is an aggregate function; it raises error XRE093 in summary labels. See Pattern 7. |
| AP6 | `sumSum([Price])` in `ExpressionBinding` without setting `label.Summary` | Always assign `label.Summary = new XRSummary { Running = SummaryRunning.Group }` (or `.Page`, `.Report`) along with the binding. Without `XRSummary`, the label displays the expression literally or shows the last row value. See Pattern 7. |
| AP7 | `Bands.Add(new DetailBand())` inside a designer-backed class (one with `InitializeComponent()`) | `InitializeComponent()` already added `DetailBand`, `TopMarginBand`, and `BottomMarginBand`. Reuse the existing designer fields (e.g., `detailBand1`). Call `Bands.Add(...)` only for band types not already present. See Constraint 9. |
| AP8 | `var label = new XRLabel { BoundsF = new RectangleF(0, 0, 200, 25) }; band.Controls.Add(label);` — size set before parent | Add to parent first, then set size: `band.Controls.Add(label); label.BoundsF = new RectangleF(0, 0, 200, 25);`. Controls inherit the parent's measure unit on insertion; sizes set before this are silently recalculated. See Constraint 14. |
| AP9 | Hardcoded pixel width spanning the full band (e.g., `new SizeF(650, 25)` as a magic constant) | `var availableWidth = PageWidthF - Margins.Left - Margins.Right;` then use `availableWidth` as the control width. See Pattern 8. |
| AP10 | Model or helper class placed before the `XtraReport` subclass in the same `.cs` file | Place model/helper classes in a separate file. Visual Studio requires the designed class to be the first class in its file. See Constraint 10. |
| AP11 | New folder created (e.g., `Data/`, `Models/`, `Reports/`) when a similar folder already exists in the project | Inspect the project tree first. Reuse the existing folder name and match its namespace exactly. See Constraint 11. |
| AP12 | Adding a control to a band without setting its content property: `var label = new XRLabel(); band.Controls.Add(label);` — no `Text` property set. Or `var pic = new XRPictureBox(); band.Controls.Add(pic);` — no `ImageSource`. | Always set the control's primary content property. For `XRLabel` → `Text` or `ExpressionBinding("BeforePrint", "Text", "[Field]")`. For `XRPictureBox` → `ImageSource` or `ImageUrl`. For `XRBarCode` → `Text` or `BinaryData`. For `XRCheckBox` → `CheckBoxState`. For `XRGauge` → `ActualValue`. See Constraint 15 and `references/report-controls.md`. |
| AP13 | Expression that references another control's value: `[ReportItems.Label1.Text]` — creates tight coupling between controls and may not work reliably (the referenced control's value might not be resolved at the moment the current expression is calculated) | Use `CalculatedField` or share the underlying data binding instead. If Label1 displays a data field, bind the second control to the same data field rather than to the first control's output. For derived or composite values, create a `CalculatedField` and bind both controls to it. See `references/expressions.md`. |

## Navigation Guide

| Need | Reference File |
|------|---------------|
| NuGet setup and first report | `references/getting-started.md` |
| Band types and when to use each | `references/report-bands.md` |
| **Control types & properties (index)** | **`references/report-controls.md`** |
| └─ Text controls | `references/report-controls-text.md` |
| └─ Table layout | `references/report-controls-table.md` |
| └─ Images | `references/report-controls-images.md` |
| └─ Barcodes | `references/report-controls-barcode.md` |
| └─ Checkboxes | `references/report-controls-checkbox.md` |
| └─ Page info | `references/report-controls-pageinfo.md` |
| └─ Charts | `references/report-controls-charts.md` |
| └─ Cross-tabs | `references/report-controls-crosstab.md` |
| └─ Sparklines | `references/report-controls-sparkline.md` |
| └─ Gauges | `references/report-controls-gauge.md` |
| └─ Layout & structure | `references/report-controls-layout.md` |
| └─ Subreports | `references/report-controls-subreport.md` |
| └─ Table of contents | `references/report-controls-toc.md` |
| └─ PDF & signatures | `references/report-controls-pdf.md` |
| └─ Common properties & expressions | `references/report-controls-common.md` |
| Data binding — all data source types | `references/data-binding.md` |
| Parameters, cascading, multi-value | `references/parameters.md` |
| Expressions, calculated fields, filtering | `references/expressions.md` |
| ExportTo*** methods + ExportOptions classes | `references/export.md` |
| Report types (table, master-detail, label) | `references/report-types.md` |

## Using DevExpress Documentation MCP

If the DxDocs MCP server is available (check for `devexpress_docs_search` tool), use it to verify exact API signatures or explore less common features:

```
devexpress_docs_search(technology="XtraReports", query="PdfExportOptions properties")
devexpress_docs_get_content(url="<article URL from search result>")
```

Use built-in references for: getting started, bands, controls, data binding, parameters, export.
Use MCP for: exact enum values, advanced scenarios, version-specific API changes.

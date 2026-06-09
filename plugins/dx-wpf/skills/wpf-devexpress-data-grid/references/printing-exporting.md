# Printing and Exporting — DevExpress WPF Data Grid

The Data Grid prints and exports through the **Printing-Exporting** library. The key choice is the **export mode**: **Data-Aware** (XLSX/XLS/CSV with preserved grouping, formulas, and conditional formatting) or **WYSIWYG** (any format with visual fidelity to the on-screen grid). Plus print preview, direct printing, and customization hooks at the cell, column, sheet, and document level.

## When to Use This Reference

Use this when you need to:

- Pick between **Data-Aware Export** and **WYSIWYG Export** (decision matrix below)
- Show a Print Preview window (with or without Ribbon, modal or modeless)
- Print directly without preview
- Embed a print preview into a window (`DocumentPreviewControl`)
- Export to XLSX/XLS/CSV/PDF/HTML/MHT/RTF/Image/XPS/TXT
- Customize **individual cells / rows** in the exported document (`CustomizeCell`)
- Customize **columns** in the exported document (`CustomizeDocumentColumn`)
- Add **sheet headers, footers, and document settings** (Data-Aware)
- Customize **printed appearance** via `PrintCellStyle` / `PrintColumnHeaderStyle` (WYSIWYG)
- Combine multiple controls into one document (`CompositeLink`)
- Add **report-wide page headers and footers** (`PrintableControlLink`)
- Generate a fully customizable grid-based report
- Tune print options per view (Table / Card / TreeList) and per master-detail layout
- Handle Data-Aware limitations (master-detail master-only, custom summary, ...)

## Decision Matrix — Data-Aware vs WYSIWYG vs Print

| Use case | Mode | API |
|---|---|---|
| Users export to Excel and continue working with summaries / sorting / grouping there | **Data-Aware** | `view.ExportToXlsx(path)`, `ExportToXls`, `ExportToCsv` |
| Visually faithful copy of the grid (cell colors, custom templates, paginated layout) → PDF / HTML | **WYSIWYG** | `view.ExportToPdf(path)`, `ExportToHtml`, etc. |
| User prints a hard copy directly | **WYSIWYG** print | `view.Print()` (dialog), `view.PrintDirect()` (default printer) |
| Preview before print / export | **WYSIWYG** preview | `view.ShowPrintPreview()`, `ShowPrintPreviewDialog`, `ShowRibbonPrintPreview` |
| Generate a customizable report (designer-editable, parameters, formulas) | **Report Generation** | https://docs.devexpress.com/content/WPF/117300?md=true (separate API) |

**Default behavior**:
- `ExportToXlsx` / `ExportToXls` / `ExportToCsv` → Data-Aware mode
- Everything else (`ExportToPdf`, `ExportToHtml`, `ExportToMht`, `ExportToRtf`, `ExportToImage`, `ExportToText`, `ExportToXps`) → WYSIWYG

Source: `articles/controls-and-libraries/data-grid/printing-and-exporting/data-aware-export.md` and `wysiwyg-export.md`.

## Data-Aware Export

Default mode for **XLS, XLSX, CSV**. Preserves:

- **Grouping** — exported as Excel collapsible groups
- **Sorting / filtering** — exported as Excel sort / filter dropdown
- **Total / Group Summaries** — exported as Excel formulas (users can resort in Excel and the formulas update)
- **Excel-style Conditional Formatting** — exported as native Excel rules
- **Data Validation** — lookup / combo box columns produce Excel validation dropdowns
- **Fixed Columns** (left-fixed only — right-fixed columns are NOT preserved)
- **Band Columns** — exported as merged header cells
- Data types (numbers stay numbers, dates stay dates)

```csharp
view.ExportToXlsx(@"C:\export\orders.xlsx");
view.ExportToXls (@"C:\export\orders.xls");
view.ExportToCsv (@"C:\export\orders.csv");
```

> `CardView` does NOT support Data-Aware export. Use WYSIWYG for cards.

Source: `articles/controls-and-libraries/data-grid/printing-and-exporting/data-aware-export.md`.

### Customize Cells in the Output (Data-Aware)

The `XlsxExportOptionsEx.CustomizeCell` event lets you change formatting per cell:

```csharp
using DevExpress.XtraPrinting;
using System.Drawing;   // ColorEnum reference

void Button_Click_Export(object sender, RoutedEventArgs e) {
    var options = new XlsxExportOptionsEx();
    options.CustomizeCell += Options_CustomizeCell;
    options.CustomizeDocumentColumn += Options_CustomizeDocumentColumn;
    view.ExportToXlsx(@"C:\export\orders.xlsx", options);
}

void Options_CustomizeCell(CustomizeCellEventArgs e) {
    // Stripe odd rows
    if (e.DocumentRow % 2 != 0)
        e.Formatting.BackColor = Color.PeachPuff;

    // Highlight overdue cells
    if (e.ColumnFieldName == "DueDate" &&
        e.Value is DateTime due && due < DateTime.Today) {
        e.Formatting.ForeColor = Color.Red;
        e.Formatting.Font = new Font(e.Formatting.Font, FontStyle.Bold);
    }

    e.Handled = true;   // Tell the engine we handled formatting for this cell
}

void Options_CustomizeDocumentColumn(CustomizeDocumentColumnEventArgs e) {
    if (e.ColumnFieldName == "Birthday")
        e.DocumentColumn.WidthInPixels = 300;

    if (e.ColumnFieldName.StartsWith("Internal"))
        e.DocumentColumn.Hidden = true;
}
```

### `CustomizeCellEventArgs` Members

| Member | Description |
|---|---|
| `Value` | The cell value being exported |
| `ColumnFieldName` | The column's `FieldName` |
| `DocumentRow` | Row index in the output document (0-based, includes headers) |
| `DocumentColumn` | The output document column |
| `Formatting` | The `Formatting` object to set (BackColor, ForeColor, Font, Alignment, etc.) |
| `Handled` | Set to `true` to signal you've handled the cell (otherwise default formatting applies) |

### Data-Aware Customization Events (Full Set)

| Event | Effect |
|---|---|
| `CustomizeCell` | Per-cell formatting |
| `CustomizeDocumentColumn` | Per-column width, hidden state, formatting |
| `CustomizeSheetHeader` | Add a header (logo, title) to the sheet |
| `CustomizeSheetFooter` | Add a footer (page number, timestamp) |
| `CustomizeSheetSettings` | Page orientation, margins, print area |

Same events exist on `XlsExportOptionsEx` and `CsvExportOptionsEx`.

### Customize Page Headers / Footers

```csharp
options.CustomizeSheetHeader += e => {
    e.SheetHeader.SetLeft("Confidential", 0);
    e.SheetHeader.SetCenter("Sales Report — " + DateTime.Now.ToShortDateString(), 0);
    e.SheetHeader.SetRight("Page &P of &N", 0);
};
```

See the GitHub example "Add Page Headers and Footers to Exported Data" linked from the source article.

### Force Data-Aware to Skip a Column

```xaml
<dxg:GridColumn FieldName="InternalId" AllowPrinting="False"/>
```

`AllowPrinting="False"` excludes the column from both export and print.

### Data-Aware Limitations

- **Master-detail**: only master rows are exported. Detail data is dropped.
- **Printing styles** (`PrintCellStyle`, `PrintColumnHeaderStyle`) and **print templates** are NOT used.
- **Top-positioned total summary** (`TotalSummaryPosition="Top"`) — summaries export as text strings, not formulas. Use `Bottom` to get formulas.
- **Blank cells** — `Count` summaries skip them by default. Set `XlsxExportOptionsEx.SummaryCountBlankCells = true` to include.
- **Custom Summary** — not supported.
- **Conditional formatting filter** applied — only `FormatCondition` rules survive; data-analysis filters are stripped.
- **Images** export to **.xlsx only** (via `XlsxExportOptionsEx.AllowCellImages = true`). Other formats omit images.
- **Custom masks** — not exported.
- **Unbound columns** with `UnboundDataType = String` or `DateTime` are NOT exported as formulas when `UnboundExpressionExportMode` is set.

## WYSIWYG Export

Visually faithful copy of the on-screen grid. Use when:

- Output is a paper-style document (PDF, RTF, MHT)
- Visual styling (custom cell colors, templates, fonts) must be preserved
- Pagination matters

```csharp
view.ExportToPdf  (@"C:\export\orders.pdf");
view.ExportToHtml (@"C:\export\orders.html");
view.ExportToMht  (@"C:\export\orders.mht");
view.ExportToRtf  (@"C:\export\orders.rtf");
view.ExportToImage(@"C:\export\orders.png");
view.ExportToXps  (@"C:\export\orders.xps");
view.ExportToText (@"C:\export\orders.txt");
```

> XLSX/XLS/CSV default to Data-Aware. Force WYSIWYG via `ExportType`.

### Force WYSIWYG for XLSX/XLS/CSV

**Per-call**:

```csharp
view.ExportToXlsx(@"C:\export\orders.xlsx",
    new XlsxExportOptionsEx { ExportType = ExportType.WYSIWYG });
```

**Application-wide** (in `App.xaml.cs`):

```csharp
using DevExpress.Export;

public partial class App : System.Windows.Application {
    static App() {
        ExportSettings.DefaultExportType = ExportType.WYSIWYG;
    }
}
```

### Customize WYSIWYG Cells via `PrintCellStyle`

WYSIWYG mode uses styles, not events. Set per-column:

```xaml
<dxg:GridColumn FieldName="ProductName">
    <dxg:GridColumn.PrintCellStyle>
        <Style TargetType="dxe:TextEdit"
               BasedOn="{StaticResource {dxgt:TableViewThemeKey ResourceKey=DefaultPrintCellStyle}}">
            <Setter Property="Background" Value="{Binding RowData.Row.Color}"/>
        </Style>
    </dxg:GridColumn.PrintCellStyle>
</dxg:GridColumn>
```

Note the `BasedOn` with the **theme key resource** — without it, the print style overrides default theme settings entirely.

Required XAML namespaces:
```xml
xmlns:dxgt="http://schemas.devexpress.com/winfx/2008/xaml/grid/themekeys"
xmlns:dxe="http://schemas.devexpress.com/winfx/2008/xaml/editors"
```

### Customize WYSIWYG Column Headers

```xaml
<Window.Resources>
    <Style x:Key="customPrintColumnHeaderStyle"
           TargetType="dxe:BaseEdit"
           BasedOn="{StaticResource {dxgt:TableViewThemeKey ResourceKey=DefaultPrintHeaderStyle}}">
        <Setter Property="Background" Value="White"/>
        <Setter Property="FontWeight" Value="Bold"/>
    </Style>
</Window.Resources>

<dxg:TableView PrintColumnHeaderStyle="{StaticResource customPrintColumnHeaderStyle}"/>
```

### Custom Render in WYSIWYG (Images, etc.)

```xaml
<Style x:Key="ImageColumnPrintingStyle"
       TargetType="dxe:PopupImageEdit"
       BasedOn="{StaticResource {dxgt:TableViewThemeKey ResourceKey=DefaultPrintCellStyle}}">
    <Setter Property="dxp:ExportSettings.TargetType" Value="Panel"/>
    <Setter Property="DisplayTemplate">
        <Setter.Value>
            <ControlTemplate TargetType="dxe:PopupImageEdit">
                <dxe:ImageEdit Source="{Binding Path=Value}" IsPrintingMode="True"/>
            </ControlTemplate>
        </Setter.Value>
    </Setter>
</Style>

<dxg:GridColumn FieldName="Image" PrintCellStyle="{StaticResource ImageColumnPrintingStyle}">
    <dxg:GridColumn.EditSettings>
        <dxe:PopupImageEditSettings/>
    </dxg:GridColumn.EditSettings>
</dxg:GridColumn>
```

`dxp:ExportSettings.TargetType="Panel"` instructs the export to treat the cell as a `Panel` (so the inner `ImageEdit` renders).

## Print Data

### Print Methods

```csharp
view.Print();          // Show Print dialog, then print
view.PrintDirect();    // Print to default printer immediately, no dialog
```

`Print()` is the modal-dialog variant. `PrintDirect()` is fire-and-forget.

### Print Preview

```csharp
view.ShowPrintPreview();              // Modeless window
view.ShowPrintPreviewDialog();        // Modal window
view.ShowRibbonPrintPreview();        // Modeless with Ribbon UI
view.ShowRibbonPrintPreviewDialog();  // Modal with Ribbon UI
```

The Print Preview window lets the user:
- Print
- Export to any format (XLSX, PDF, HTML, etc.) via toolbar buttons
- Configure page setup (orientation, margins, scaling)
- Navigate pages

Source: `articles/controls-and-libraries/data-grid/printing-and-exporting/print-data.md`.

## Embed Print Preview Into Your Window — `DocumentPreviewControl`

For workflows where the preview should be part of the application UI (not a popup):

```xaml
<Window xmlns:dxg="http://schemas.devexpress.com/winfx/2008/xaml/grid"
        xmlns:dxp="http://schemas.devexpress.com/winfx/2008/xaml/printing">
    <Grid>
        <Grid.ColumnDefinitions>
            <ColumnDefinition Width="300"/>
            <ColumnDefinition />
        </Grid.ColumnDefinitions>

        <dxg:GridControl Grid.Column="0" .../>
        <Button Grid.Column="0" Content="Print"
                VerticalAlignment="Bottom"
                Click="Button_Click_Print"/>

        <dxp:DocumentPreviewControl Grid.Column="1" x:Name="documentPreview1"/>
    </Grid>
</Window>
```

```csharp
using DevExpress.Xpf.Printing;

private void Button_Click_Print(object sender, RoutedEventArgs e) {
    var link = new PrintableControlLink(view);
    link.CreateDocument();
    documentPreview1.DocumentSource = link;
}
```

`PrintableControlLink` wraps the view as a printable document; assign the link to the preview control's `DocumentSource`.

Source: `articles/controls-and-libraries/data-grid/printing-and-exporting/print-data.md` § "Add Print Preview to a Project".

## Advanced Layout — `PrintableControlLink` and `CompositeLink`

### Vertical Content Splitting (Wide Tables)

By default, columns that don't fit are clipped. Use `VerticalContentSplitting` to fit all columns:

```csharp
var link = new PrintableControlLink(view) {
    VerticalContentSplitting = VerticalContentSplitting.Smart
};
link.ShowPrintPreview(this);
```

`Smart` splits columns intelligently across pages. `Exact` splits at fixed widths.

### Combine Multiple Controls in One Document

```csharp
var link1 = new PrintableControlLink(view1);
var link2 = new PrintableControlLink(view2);
var composite = new CompositeLink(new List<TemplatedLink> { link1, link2 });
composite.ShowPrintPreviewDialog(this);
```

### Custom Page / Report Headers and Footers

```csharp
var link = new PrintableControlLink(view) {
    PageHeaderTemplate = (DataTemplate)Resources["PageHeader"],
    PageFooterTemplate = (DataTemplate)Resources["PageFooter"],
    ReportHeaderTemplate = (DataTemplate)Resources["ReportHeader"],
    ReportFooterTemplate = (DataTemplate)Resources["ReportFooter"],
};
```

| Property | When it appears |
|---|---|
| `PageHeaderTemplate` / `PageHeaderData` | Top of every page |
| `PageFooterTemplate` / `PageFooterData` | Bottom of every page |
| `ReportHeaderTemplate` / `ReportHeaderData` | First page only |
| `ReportFooterTemplate` / `ReportFooterData` | Last page only |

Source: `templates/grid-print-export-customization.md` (template include).

## Print Options Per View

### Table View

| Property | Effect |
|---|---|
| `DataViewBase.PrintTotalSummary` | Print the Summary Panel |
| `DataViewBase.PrintFixedTotalSummary` | Print the Fixed Summary Panel |
| `GridViewBase.PrintAllGroups` | Print with all groups expanded |
| `TableView.PrintAutoWidth` | Fit columns to page width |
| `TableView.PrintColumnHeaders` | Include column headers |
| `TableView.PrintGroupFooters` | Include group footers |
| `BaseColumn.AllowPrinting` | Hide specific columns from print / export |

### Card View

| Property | Effect |
|---|---|
| `DataViewBase.PrintTotalSummary` | Print Summary Panel |
| `CardView.PrintAutoCardWidth` | Auto-resize cards horizontally |
| `CardView.PrintCardMargin` | Card frame thickness |
| `CardView.PrintMaximumCardColumns` | Max card columns per page |

### TreeList View

| Property | Effect |
|---|---|
| `TreeListView.PrintAllNodes` | Print with all nodes expanded |
| `TreeListView.PrintAutoWidth` | Fit columns to page width |
| `TreeListView.PrintColumnHeaders` | Include column headers |

### Master-Detail

| Property | Effect |
|---|---|
| `TableView.AllowPrintDetails` | Include details in printout |
| `TableView.AllowPrintEmptyDetails` | Include details that have no data |
| `TableView.PrintDetailTopIndent` / `PrintDetailBottomIndent` | Spacing between master rows and details |

## Grid-Based Report Generation

For richer reports (designer-editable, parameters, multi-band layouts), generate a Report from the grid:

```csharp
// TODO: Verify exact API. Typical pattern uses GridControl.CreateReport
// or a ReportBuilder helper from the Printing-Exporting library.
// devexpress_docs_search(technology="WPF Data Grid", query="generate grid based report")
```

See `articles/controls-and-libraries/data-grid/printing-and-exporting/grid-based-report-generation.md` and https://docs.devexpress.com/content/WPF/117300?md=true.

## Document Post-Processing

After generation but before display / save, you can modify the document:

See `articles/controls-and-libraries/data-grid/printing-and-exporting/document-post-processing.md`. Hook into the printing-exporting library's document tree (`DevExpress.XtraPrinting.PrintingSystem.Document.Pages`).

## Required NuGet Package

```bash
dotnet add package DevExpress.Wpf.Printing
```

The Print and Export methods require this package. Without it, calls compile but the runtime methods fail.

## Apply to TreeListControl

| GridControl method | TreeListControl equivalent |
|---|---|
| `TableView.ExportToXlsx` / `ExportToXls` / `ExportToCsv` | `TreeListView.ExportToXlsx` / `ExportToXls` / `ExportToCsv` |
| `DataViewBase.ExportToPdf` / `Html` / `Mht` / `Rtf` / `Image` / `Text` / `Xps` | same (inherited from `DataViewBase`) |
| `DataViewBase.Print` / `PrintDirect` / `ShowPrintPreview*` | same (inherited) |
| `TableView.PrintAllGroups` | `TreeListView.PrintAllNodes` (different name; same semantic) |
| `TableView.PrintAutoWidth` / `PrintColumnHeaders` | `TreeListView.PrintAutoWidth` / `PrintColumnHeaders` |

CardView supports only WYSIWYG export. Use `CardView.ExportToXlsx`/`Xls`/`Csv` only with `ExportType=WYSIWYG`.

## Common Issues

- **Data-Aware export drops custom cell styles** — by design. Use WYSIWYG mode if visual fidelity matters more than Excel-functional output.
- **`CustomizeCell` doesn't fire** — wired the event after calling `ExportToXlsx`. Wire it on the `options` object before passing to the export call.
- **Wide grid clips columns in print** — set `PrintAutoWidth="True"` to fit, or use `PrintableControlLink.VerticalContentSplitting = Smart` to span pages.
- **Master-detail master-only export** — known Data-Aware limitation. Export detail data separately or use WYSIWYG.
- **`AllowPrinting="False"` doesn't hide column in Data-Aware** — does work; check the column was actually configured before the export call (changes after `ExportToXlsx` is invoked don't apply to that call).
- **CustomizeSheetHeader text shows literally for `&P`** — the page-number placeholder needs the right `Set*` overload. See the Excel sheet-header reference for valid format strings.
- **Print preview empty for CardView** — `CardView` has different print properties (`PrintAutoCardWidth`, `PrintMaximumCardColumns`); make sure they're set to sensible values.

## Source Material

- `articles/controls-and-libraries/data-grid/printing-and-exporting.md` (root)
- `articles/controls-and-libraries/data-grid/printing-and-exporting/data-aware-export.md`
- `articles/controls-and-libraries/data-grid/printing-and-exporting/wysiwyg-export.md`
- `articles/controls-and-libraries/data-grid/printing-and-exporting/print-data.md`
- `articles/controls-and-libraries/data-grid/printing-and-exporting/grid-based-report-generation.md`
- `articles/controls-and-libraries/data-grid/printing-and-exporting/document-post-processing.md`
- `templates/grid-print-export-customization.md` (template include, resolved)
- `articles/controls-and-libraries/data-grid/appearance-customization/format-cell-values.md` (cross-reference: which display techniques survive export)

# Advanced Features — DevExpress WPF Pivot Grid

This reference collects features beyond the basics: conditional formatting, KPI displays, chart integration, printing/export, color customization, and MVVM integration.

## When to Use This Reference

Use this when you need to:

- Apply Excel-style conditional formatting to cells (color scales, data bars, icon sets)
- Display KPIs from an OLAP cube
- Integrate the Pivot Grid with `ChartControl` for visual analytics
- Print, preview, or export to PDF / XLSX / HTML / CSV / RTF / MHT / TXT
- Customize cell, value, and total colors
- Bind pivot state to a ViewModel

## Conditional Formatting

The Pivot Grid supports an Excel-inspired conditional formatting engine — color scales, data bars, icon sets, top/bottom rules, and value/expression conditions. Rules live in `PivotGridControl.FormatConditions` and reference fields by `Name` (set `Name` on every field). Condition classes: `ColorScaleFormatCondition`, `DataBarFormatCondition`, `IconSetFormatCondition`, `TopBottomRuleFormatCondition`, `FormatCondition` (all inherit `FormatConditionBase`). End users can manage rules via `AllowConditionalFormattingMenu` / `AllowConditionalFormattingManager`.

**For the full reference (condition types, custom `DataBarFormat` / `IconSetFormat`, scoping with `ApplyToSpecificLevel`, code-based rules, export limitations) see [conditional-formatting.md](conditional-formatting.md).**

## Key Performance Indicators (KPI)

The Pivot Grid renders KPI status/trend/value/goal/weight indicators from Analysis Services cubes or any data source supplying integer KPI values (`-1`, `0`, `1`). Key API surface: `PivotGridControl.GetOlapKpiList()`, `GetOlapKpiMeasures()`, `PivotGridField.KpiType`, `KpiGraphic`.

**For the full reference (cube discovery, per-component binding pattern, server vs client graphics, custom KPI cell templates, non-OLAP usage) see [kpi.md](kpi.md).**

## Chart Integration

`ChartControl` can pull visible Pivot Grid data via `PivotGridControl.ChartDataSource` — yielding a live chart that reshapes as the user reorganizes the pivot.

**For the full reference (Series/Arguments/Values mapping, `ChartProvideDataByColumns`, total handling, series/point caps, drill-into-chart via `ChartSelectionOnly`, type conversion) see [chart-integration.md](chart-integration.md).**

## Printing and Exporting

```csharp
pivotGridControl1.ShowPrintPreview(this, "Sales", "Q1 2026 Sales Report");
pivotGridControl1.Print();

pivotGridControl1.ExportToXlsx("pivot.xlsx");
pivotGridControl1.ExportToPdf("pivot.pdf");
pivotGridControl1.ExportToHtml("pivot.html");
pivotGridControl1.ExportToMht("pivot.mht");
pivotGridControl1.ExportToCsv("pivot.csv");
pivotGridControl1.ExportToRtf("pivot.rtf");
pivotGridControl1.ExportToText("pivot.txt");
```

XAML button using a Pivot Grid command:

```xaml
<dx:SimpleButton Content="Print Preview"
                 Command="{x:Static dxpg:PivotGridCommands.ShowPrintPreviewDialog}"
                 CommandTarget="{Binding ElementName=pivot}"/>
```

### XLSX-Specific Options

For an Excel-faithful export, use `PivotGridXlsxExportOptions` with `ExportType = WYSIWYG`:

```csharp
var options = new PivotGridXlsxExportOptions {
    ExportType = DevExpress.Export.ExportType.WYSIWYG
};
pivotGridControl1.ExportToXlsx("pivot.xlsx", options);
```

Required package: `DevExpress.Wpf.Printing`.

Source: `articles/controls-and-libraries/pivot-grid/printing-and-exporting/tutorial-printing-and-exporting-a-pivot-grid.md` and `printing-and-exporting/export-to-tabular-formats.md` (https://docs.devexpress.com/content/WPF/8446?md=true).

## Appearance — Colors, Styles, Templates

The Pivot Grid exposes dedicated color properties for cells, values, and totals (`CellBackground`, `CellTotalBackground`, `ValueBackground`, `ValueTotalBackground`, and their `*Selected*` / `*Foreground` variants), plus the standard `Control` brushes (`Background`, `Foreground`, `BorderBrush`). Element styles (`CellStyle`, `FieldHeaderContentStyle`, `FieldValueStyle`, ...), cell/field-value templates and selectors, and the `CustomCellAppearance` event give finer control.

**For the full reference (theme color overrides, element styles, cell/field-value templates and selectors, and the `CustomCellAppearance` event) see [appearance.md](appearance.md).**

## MVVM Integration

The Pivot Grid supports MVVM through `FieldsSource` / `GroupsSource` (ViewModel-defined fields rendered via templates), `dx:XamlHelper.Name` for layout-stable templated fields, and bindable state (`DataSource`, `FocusedCell`).

**For the full reference (field descriptor pattern, `FieldGeneratorTemplate` / `FieldGeneratorTemplateSelector`, `dxci:DependencyObjectExtensions.DataContext` for performance, ViewModel-driven groups, drill-down commands) see [mvvm.md](mvvm.md).**

## Source Material

- `articles/controls-and-libraries/pivot-grid.md` (root)
- `articles/controls-and-libraries/pivot-grid/appearance.md`
- `articles/controls-and-libraries/pivot-grid/appearance/customizing-pivot-grid-colors.md` (https://docs.devexpress.com/content/WPF/11213?md=true)
- `articles/controls-and-libraries/pivot-grid/data-analysis/conditional-formatting.md` (https://docs.devexpress.com/content/WPF/114038?md=true)
- `articles/controls-and-libraries/pivot-grid/data-analysis/key-performance-indicators-kpis.md` (https://docs.devexpress.com/content/WPF/11641?md=true)
- `articles/controls-and-libraries/pivot-grid/data-analysis/integration-with-the-chart-control.md` (https://docs.devexpress.com/content/WPF/8016?md=true)
- `articles/controls-and-libraries/pivot-grid/printing-and-exporting/tutorial-printing-and-exporting-a-pivot-grid.md`
- `articles/controls-and-libraries/pivot-grid/printing-and-exporting/export-to-tabular-formats.md`
- `articles/controls-and-libraries/pivot-grid/mvvm-enhancements/`

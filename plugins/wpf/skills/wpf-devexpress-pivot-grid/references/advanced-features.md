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

The Pivot Grid supports the same Excel-inspired conditional formatting engine as `GridControl` — color scales, data bars, icon sets, top/bottom rules, and value/expression conditions.

### Setup Requirements

1. **Every `PivotGridField` must have a `Name`** — format conditions reference fields by name, not by column index.
2. The format condition's `MeasureName` points at the data field whose values are being formatted.
3. Add the condition to `PivotGridControl.FormatConditions`.

### Available Condition Classes

| Class | Effect |
|---|---|
| `ColorScaleFormatCondition` | 2-color or 3-color gradient based on value distribution |
| `DataBarFormatCondition` | Horizontal bar inside the cell, proportional to value |
| `IconSetFormatCondition` | Icons (arrows, traffic lights, etc.) based on value range |
| `TopBottomRuleFormatCondition` | Format top-N / bottom-N values or above/below average |
| `FormatCondition` | Value comparison (Equal, Less, Greater, Between) or arbitrary criteria expression |

All inherit from `FormatConditionBase` (shared properties: `MeasureName`, `ApplyToSpecificLevel`, `ColumnName`, `RowName`).

### XAML — Data Bar Condition

Apply a data bar to the intersection of `fieldSalesPerson` (row) and `fieldQuarter` (column), measuring `fieldVariation` with separate gradients for positive vs negative:

```xaml
<dxpg:PivotGridControl.FormatConditions>
    <dxpg:DataBarFormatCondition ApplyToSpecificLevel="True"
                                 ColumnName="fieldQuarter"
                                 RowName="fieldSalesPerson"
                                 MeasureName="fieldVariation">
        <dx:DataBarFormat BorderBrush="#FF63C384"
                          BorderBrushNegative="#FFFF555A">
            <dx:DataBarFormat.Fill>
                <LinearGradientBrush EndPoint="1,0">
                    <GradientStop Color="#FF63C384" Offset="0"/>
                    <GradientStop Color="White" Offset="1"/>
                </LinearGradientBrush>
            </dx:DataBarFormat.Fill>
            <dx:DataBarFormat.FillNegative>
                <LinearGradientBrush EndPoint="1,0">
                    <GradientStop Color="White" Offset="0"/>
                    <GradientStop Color="#FFFF555A" Offset="1"/>
                </LinearGradientBrush>
            </dx:DataBarFormat.FillNegative>
        </dx:DataBarFormat>
    </dxpg:DataBarFormatCondition>
</dxpg:PivotGridControl.FormatConditions>
```

### Apply to All Cells vs Specific Intersection

- `ApplyToSpecificLevel="False"` (default): condition applies to all data cells of the measure. `ColumnName` / `RowName` are ignored.
- `ApplyToSpecificLevel="True"`: condition applies only at the row×column intersection identified by `ColumnName` and `RowName`. **Required for `TopBottomRuleFormatCondition`** — Top/Bottom needs a specific intersection.

### Enable End-User Conditional Formatting

```xaml
<dxpg:PivotGridControl AllowConditionalFormattingMenu="True"
                       AllowConditionalFormattingManager="True"/>
```

- `AllowConditionalFormattingMenu` — right-click a data cell → Conditional Formatting menu.
- `AllowConditionalFormattingManager` — Manage Rules dialog (create/edit/delete/reorder).

### Limitations

> When printing and exporting to PDF, HTML, MHT, RTF, XLS/XLSX, **icons and data bars are not exported**. Color-scale fills do export.

Source: `articles/controls-and-libraries/pivot-grid/data-analysis/conditional-formatting.md` (`xref:114038`).

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

Source: `articles/controls-and-libraries/pivot-grid/printing-and-exporting/tutorial-printing-and-exporting-a-pivot-grid.md` and `printing-and-exporting/export-to-tabular-formats.md` (`xref:8446`).

## Customize Pivot Grid Colors

The Pivot Grid exposes dedicated color properties for cells, values, and totals — separate from theme-level brushes.

### Standard `Control` Properties

`BorderBrush`, `Background`, `Foreground` work as on any WPF control.

### Pivot-Specific Color Properties

| Property | Affects |
|---|---|
| `CellBackground` / `CellForeground` | Regular data cells |
| `CellSelectedBackground` / `CellSelectedForeground` | Selected data cells |
| `CellTotalBackground` / `CellTotalForeground` | Total-row / total-column data cells |
| `CellTotalSelectedBackground` / `CellTotalSelectedForeground` | Selected total cells |
| `ValueBackground` / `ValueForeground` | Row/column header values |
| `ValueSelectedBackground` / `ValueSelectedForeground` | Selected header values |
| `ValueTotalBackground` / `ValueTotalForeground` | Total header values |
| `ValueTotalSelectedBackground` / `ValueTotalSelectedForeground` | Selected total header values |

```xaml
<dxpg:PivotGridControl
    CellBackground="WhiteSmoke"
    CellTotalBackground="LightYellow"
    ValueBackground="#FFEFF6FB"
    ValueTotalBackground="LightGoldenrodYellow"/>
```

Source: `articles/controls-and-libraries/pivot-grid/appearance/customizing-pivot-grid-colors.md` (`xref:11213`).

## Styles and Templates

The Pivot Grid exposes styles for every visual element:

- `PivotGridControl.CellStyle` — data-cell style
- Field-header styles
- Group-row styles
- Row/column header content styles

For the full inventory, see `articles/controls-and-libraries/pivot-grid/appearance.md` → "Pivot Grid Styles" (`xref:8401`) and "Pivot Grid Elements That Support Templates" (`xref:8400`).

## MVVM Integration

The Pivot Grid supports MVVM through `FieldsSource` / `GroupsSource` (ViewModel-defined fields rendered via templates), `dx:XamlHelper.Name` for layout-stable templated fields, and bindable state (`DataSource`, `FocusedCell`).

**For the full reference (field descriptor pattern, `FieldGeneratorTemplate` / `FieldGeneratorTemplateSelector`, `dxci:DependencyObjectExtensions.DataContext` for performance, ViewModel-driven groups, drill-down commands) see [mvvm.md](mvvm.md).**

## Source Material

- `articles/controls-and-libraries/pivot-grid.md` (root)
- `articles/controls-and-libraries/pivot-grid/appearance.md`
- `articles/controls-and-libraries/pivot-grid/appearance/customizing-pivot-grid-colors.md` (`xref:11213`)
- `articles/controls-and-libraries/pivot-grid/data-analysis/conditional-formatting.md` (`xref:114038`)
- `articles/controls-and-libraries/pivot-grid/data-analysis/key-performance-indicators-kpis.md` (`xref:11641`)
- `articles/controls-and-libraries/pivot-grid/data-analysis/integration-with-the-chart-control.md` (`xref:8016`)
- `articles/controls-and-libraries/pivot-grid/printing-and-exporting/tutorial-printing-and-exporting-a-pivot-grid.md`
- `articles/controls-and-libraries/pivot-grid/printing-and-exporting/export-to-tabular-formats.md`
- `articles/controls-and-libraries/pivot-grid/mvvm-enhancements/`

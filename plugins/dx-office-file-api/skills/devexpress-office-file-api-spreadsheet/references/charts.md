# Charts — DevExpress Spreadsheet Document API

Create and configure charts embedded in worksheets or on dedicated chart sheets.

## When to Use This Reference

Use this when you need to:
- Create a chart (column, line, pie, bar, scatter, area, bubble, stock, radar, Excel 2016 types)
- Specify chart data from a cell range or from explicit series definitions
- Configure chart axes, titles, legends, and data labels
- Change a chart type or create a combination (multi-type) chart
- Move a chart to a dedicated chart sheet
- Style chart elements (fill, outline, font, colors)
- Add trendlines or format series points

## Key Classes and Types

| Class/Interface | Purpose |
|----------------|---------|
| `ChartCollection` | Collection of charts on a worksheet (`Worksheet.Charts`) |
| `Chart` | The chart object (alias for `ChartObject`) |
| `ChartType` | Enum of all supported chart types |
| `Series` | A data series on a chart |
| `SeriesCollection` | Collection of series (`Chart.Series`) |
| `ChartData` | Data source for a series (range-based or literal values) |
| `Axis` | Represents a chart axis (X, Y, secondary) |
| `Legend` | Chart legend configuration |
| `ChartTitle` | Chart or axis title |
| `ChartView` | Groups series of the same type (used for combination charts) |
| `LegendPosition` | Enum for legend placement (Bottom, Top, Left, Right, TopRight) |
| `ChartDataDirection` | Enum: Column or Row — data direction for `SelectData` |

## Create a Chart

### From a Cell Range (Simplest Approach)

```csharp
using DevExpress.Spreadsheet;
using DevExpress.Spreadsheet.Charts;

Worksheet sheet = workbook.Worksheets[0];

// Populate some data
sheet["A1"].Value = "Month"; sheet["B1"].Value = "Sales"; sheet["C1"].Value = "Expenses";
sheet["A2"].Value = "Jan";   sheet["B2"].Value = 12000;   sheet["C2"].Value = 9000;
sheet["A3"].Value = "Feb";   sheet["B3"].Value = 15000;   sheet["C3"].Value = 11000;
sheet["A4"].Value = "Mar";   sheet["B4"].Value = 18000;   sheet["C4"].Value = 13000;

// Create a clustered column chart from the data range
Chart chart = sheet.Charts.Add(ChartType.ColumnClustered, sheet["A1:C4"]);

// Position by anchoring to cells
chart.TopLeftCell = sheet.Cells["E2"];
chart.BottomRightCell = sheet.Cells["L16"];
```

### From Explicit Series (Non-Contiguous Ranges)

```csharp
Chart chart = sheet.Charts.Add(ChartType.ColumnClustered);

// Position the chart
chart.TopLeftCell = sheet.Cells["E2"];
chart.BottomRightCell = sheet.Cells["L16"];

// Add series: (seriesNameCell, categoriesRange, valuesRange)
chart.Series.Add(sheet["B1"], sheet["A2:A4"], sheet["B2:B4"]); // Sales series
chart.Series.Add(sheet["C1"], sheet["A2:A4"], sheet["C2:C4"]); // Expenses series
```

### From Literal Values (No Cell References)

```csharp
Chart chart = sheet.Charts.Add(ChartType.ColumnClustered);
chart.TopLeftCell = sheet.Cells["A6"];
chart.BottomRightCell = sheet.Cells["G20"];

// Two-argument overload: (categories, values) — series gets default name "Series 1"
Series s = chart.Series.Add(
    new CellValue[] { "Jan", "Feb", "Mar", "Apr" },   // categories
    new CellValue[] { 12000, 15000, 18000, 22000 });  // values
```

### Specify Data Direction

```csharp
Chart chart = sheet.Charts.Add(ChartType.ColumnClustered);
// Data direction: Column = series in columns, Row = series in rows
chart.SelectData(sheet["A1:D5"], ChartDataDirection.Column);
```

## Configure Chart Elements

### Title

```csharp
chart.Title.Visible = true;
chart.Title.SetValue("Monthly Sales Report");

// Or link title to a cell
chart.Title.SetReference(sheet["A1"]);
```

### Legend

```csharp
chart.Legend.Visible = true;
chart.Legend.Position = LegendPosition.Bottom;
```

### Axes

```csharp
// Primary axes
Axis categoryAxis = chart.PrimaryAxes.CategoryAxis; // X-axis
Axis valueAxis = chart.PrimaryAxes.ValueAxis;        // Y-axis

// Axis title
valueAxis.Title.Visible = true;
valueAxis.Title.SetValue("Revenue ($)");

// Number format on the value axis
valueAxis.NumberFormat = "$#,##0";

// Axis scaling
valueAxis.Scaling.Min = 0;
valueAxis.Scaling.Max = 30000;
valueAxis.Scaling.LogScale = false;

// Hide category axis
categoryAxis.Visible = false;

// Reverse axis direction
valueAxis.Scaling.Orientation = AxisOrientation.MaxMin;

// Gridlines
valueAxis.MajorGridlines.Visible = true;
valueAxis.MinorGridlines.Visible = false;
```

### Data Labels

```csharp
// Show data labels for all series in the first chart view
chart.Views[0].DataLabels.ShowValue = true;
chart.Views[0].DataLabels.Position = DataLabelPosition.OutsideEnd;
chart.Views[0].DataLabels.NumberFormat = "$#,##0";
chart.Views[0].DataLabels.ShowCategoryName = false;
```

## Supported Chart Types

| Category | `ChartType` Values |
|----------|-------------------|
| Column | `ColumnClustered`, `ColumnStacked`, `ColumnFullStacked`, `Column3D` |
| Bar | `BarClustered`, `BarStacked`, `BarFullStacked` |
| Line | `Line`, `LineMarker`, `LineStacked`, `Line3D` |
| Pie | `Pie`, `PieExploded`, `Pie3D` |
| Doughnut | `Doughnut`, `DoughnutExploded` |
| Area | `Area`, `AreaStacked`, `AreaFullStacked`, `Area3D` |
| Scatter | `ScatterLine`, `ScatterLineMarker`, `ScatterMarker`, `ScatterSmooth` |
| Bubble | `Bubble`, `Bubble3D` |
| Stock | `StockHighLowClose`, `StockOpenHighLowClose`, `StockVolumeHighLowClose` |
| Radar | `Radar`, `RadarMarker`, `RadarFilled` |
| Excel 2016 | `BoxWhisker`, `Waterfall`, `Histogram`, `Pareto`, `Funnel`, `Sunburst`, `Treemap` |

## Combination (Multi-Type) Charts

```csharp
// Start with a column chart
Chart chart = sheet.Charts.Add(ChartType.ColumnClustered, sheet["A1:C4"]);
chart.TopLeftCell = sheet.Cells["E2"];
chart.BottomRightCell = sheet.Cells["L16"];

// Change the second series to a line
// This automatically creates a second ChartView for the line series
chart.Series[1].ChangeType(ChartType.Line);
```

> Not all combinations are valid. 2-D and 3-D types cannot be mixed.

## Change Chart Type

```csharp
// Change the entire chart type
chart.ChangeType(ChartType.BarClustered);
```

## Chart Sheets

```csharp
// Create a new chart sheet directly
ChartSheet chartSheet = workbook.ChartSheets.Add("SalesChart");
Chart chart = chartSheet.Chart;
chart.SelectData(sheet["A1:C4"], ChartDataDirection.Column);

// Or move an existing embedded chart to a new chart sheet
Chart embeddedChart = sheet.Charts[0];
embeddedChart.MoveToNewChartSheet("MyChartSheet");
```

## Styling Chart Elements

```csharp
using System.Drawing;

// Apply a built-in chart style (1–48)
chart.Style = 2;

// Fill the chart area with a solid color
chart.ChartArea.Fill.SetSolidFill(Color.WhiteSmoke);

// Format the plot area
chart.PlotArea.Fill.SetSolidFill(Color.FromArgb(240, 240, 255));
chart.PlotArea.Outline.SetSolidFill(Color.LightGray);

// Format a series
Series series = chart.Series[0];
series.Fill.SetSolidFill(Color.SteelBlue);
series.Outline.SetSolidFill(Color.DarkBlue);
series.Outline.Width = 1.5;

// Format chart font
chart.Font.Name = "Calibri";
chart.Font.Size = 10;
```

## Reorder and Remove Series

```csharp
// Move a series forward (closer to the front)
chart.Series[1].BringForward();

// Move to front
chart.Series[0].BringToFront();

// Remove a series
chart.Series.RemoveAt(1);

// Remove all series
chart.Series.Clear();
```

## Troubleshooting

- **Chart appears empty**: Verify the data range is correct and contains values. Call `workbook.Calculate()` if the range contains formulas.
- **Wrong data direction**: If series appear as categories, try `chart.SwitchRowColumn()` or specify `ChartDataDirection` in `SelectData`.
- **Combination chart clears all series**: The new type is incompatible with existing types. Check the list of compatible types in the reference docs.
- **Chart sheet not found**: Use `workbook.ChartSheets["name"]` not `workbook.Worksheets["name"]`.
- **Excel 2016 chart types not printing/exporting**: Box & Whisker, Waterfall, Histogram, Pareto, Funnel, Sunburst, and Treemap can be loaded/saved and created in code, but cannot be printed or exported to PDF.

# Charts and ChartEx — DevExpress Presentation API

**v26.1+** — Programmatic chart creation for PowerPoint presentations.

## When You Need to:
- Add a standard chart (bar, line, pie, scatter, area, etc.) to a slide
- Add an Office 2016+ chart type (waterfall, funnel, treemap, sunburst, etc.)
- Configure chart data series programmatically (inline arrays or embedded cell references)
- Customize axes: titles, scale, labels, gridlines, secondary axes
- Add data labels: values, category names, percentages, custom number format
- Create combo charts combining multiple series types (e.g., bar + line)
- Add error bars or trend lines to a series

## Chart vs. ChartEx

| | `Chart` | `ChartEx` |
|---|---|---|
| **Namespace** | `DevExpress.Docs.Presentation` | `DevExpress.Docs.Presentation` |
| **File format** | Standard OOXML chart | Office 2016+ extended chart |
| **Types** | Bar, Line, Pie, Scatter, Area, Bubble, Radar, Stock, Surface, Doughnut, etc. | Waterfall, Funnel, Histogram, Pareto, Box & Whisker, Treemap, Sunburst, Filled Map |
| **`ChartType` enum** | `DevExpress.Docs.Office.ChartType` | — |
| **`ChartExType` enum** | — | `DevExpress.Docs.Office.ChartExType` |
| **Series base class** | Series-specific (e.g., `BarSeries`, `LineSeries`) | Series-specific (e.g., `WaterfallSeries`, `FunnelSeries`) |

Use `Chart` for all standard chart types. Use `ChartEx` for Office 2016+ types.

## Add a Chart to a Slide

```csharp
using DevExpress.Docs.Office;
using DevExpress.Docs.Presentation;

Presentation presentation = new Presentation();
presentation.Slides.Clear();

Slide slide = new Slide(SlideLayoutType.Blank);
presentation.Slides.Add(slide);

// Standard chart
Chart chart = new Chart();
slide.Shapes.Add(chart);
chart.Width = 800;
chart.Height = 500;

// Add two bar series
BarSeries series1 = new BarSeries();
series1.Arguments = new ChartStringData(new[] { "Q1", "Q2", "Q3", "Q4", "Q5" });
series1.Values    = new ChartNumericData(new double[] { 10, 13, 4, 7, 5 });
chart.Series.Add(series1);

BarSeries series2 = new BarSeries();
series2.Arguments = new ChartStringData(new[] { "Q1", "Q2", "Q3", "Q4", "Q5" });
series2.Values    = new ChartNumericData(new double[] { 5, 6, 4, 9, 11 });
chart.Series.Add(series2);

// Customize the view (data labels)
BarChartView barView = (BarChartView)chart.Views[0];
barView.DataLabels = new DataLabels { ShowCategoryName = true, ShowValue = true };

using FileStream outStream = new FileStream("output.pptx", FileMode.Create);
presentation.SaveDocument(outStream);
```

```vb
Imports DevExpress.Docs.Office
Imports DevExpress.Docs.Presentation

Dim presentation As New Presentation()
presentation.Slides.Clear()

Dim slide As New Slide(SlideLayoutType.Blank)
presentation.Slides.Add(slide)

Dim chart As New Chart()
slide.Shapes.Add(chart)
chart.Width = 800
chart.Height = 500

Dim series1 As New BarSeries()
series1.Arguments = New ChartStringData(New String() {"Q1", "Q2", "Q3", "Q4", "Q5"})
series1.Values    = New ChartNumericData(New Double() {10, 13, 4, 7, 5})
chart.Series.Add(series1)

Dim series2 As New BarSeries()
series2.Arguments = New ChartStringData(New String() {"Q1", "Q2", "Q3", "Q4", "Q5"})
series2.Values    = New ChartNumericData(New Double() {5, 6, 4, 9, 11})
chart.Series.Add(series2)

Dim barView As BarChartView = DirectCast(chart.Views(0), BarChartView)
barView.DataLabels = New DataLabels() With {.ShowCategoryName = True, .ShowValue = True}

Using outStream As New FileStream("output.pptx", FileMode.Create)
    presentation.SaveDocument(outStream)
End Using
```

## Add a ChartEx (Office 2016+ Types)

```csharp
using DevExpress.Docs.Office;
using DevExpress.Docs.Presentation;

Presentation presentation = new Presentation();
presentation.Slides.Clear();

Slide slide = new Slide(SlideLayoutType.Blank);
presentation.Slides.Add(slide);

ChartEx chart = new ChartEx();
slide.Shapes.Add(chart);
chart.Width = 800;
chart.Height = 600;

// Waterfall series
WaterfallSeries series = new WaterfallSeries();
series.Text = "Cash Flow";
series.Arguments = new ChartStringData(new[] { "Start", "Jan", "Feb", "Mar", "End" });
series.Values    = new ChartNumericData(new[] { 0.0, 12.0, -5.0, 8.0, 15.0 });
chart.Series.Add(series);

using FileStream outStream = new FileStream("output.pptx", FileMode.Create);
presentation.SaveDocument(outStream);
```

```vb
Imports DevExpress.Docs.Office
Imports DevExpress.Docs.Presentation

Dim presentation As New Presentation()
presentation.Slides.Clear()

Dim slide As New Slide(SlideLayoutType.Blank)
presentation.Slides.Add(slide)

Dim chart As New ChartEx()
slide.Shapes.Add(chart)
chart.Width = 800
chart.Height = 600

Dim series As New WaterfallSeries()
series.Text = "Cash Flow"
series.Arguments = New ChartStringData(New String() {"Start", "Jan", "Feb", "Mar", "End"})
series.Values    = New ChartNumericData(New Double() {0.0, 12.0, -5.0, 8.0, 15.0})
chart.Series.Add(series)

Using outStream As New FileStream("output.pptx", FileMode.Create)
    presentation.SaveDocument(outStream)
End Using
```

## Standard Chart Types (`Chart` class)

| Chart Types | Series Class | View Class |
|-------------|-------------|------------|
| Area, Stacked Area, 100% Stacked Area | `AreaSeries` | `AreaChartView` |
| 3D Area variants | `Area3DSeries` | `Area3DChartView` |
| Bar, Clustered Bar, Stacked Bar, Column variants | `BarSeries` | `BarChartView` |
| 3D Bar/Column variants | `Bar3DSeries` | `Bar3DChartView` |
| Bubble, 3D Bubble | `BubbleSeries` | `BubbleChartView` |
| Doughnut | `DoughnutSeries` | `DoughnutChartView` |
| Line, Stacked Line, 100% Stacked Line | `LineSeries` | `LineChartView` |
| 3D Line | `Line3DSeries` | `Line3DChartView` |
| Pie of Pie, Bar of Pie | `ChartOfPieSeries` | `ChartOfPieView` |
| Pie | `PieSeries` | `PieChartView` |
| Pie 3D | `Pie3DSeries` | `Pie3DChartView` |
| Radar, Filled Radar | `RadarSeries` | `RadarChartView` |
| Scatter | `ScatterSeries` | `ScatterChartView` |
| Stock (High-Low-Close, OHLC, Volume variants) | `StockSeries` | `StockChartView` |
| Surface | `SurfaceSeries` | `SurfaceChartView` |
| Surface 3D | `Surface3DSeries` | `Surface3DChartView` |

## Office 2016+ Chart Types (`ChartEx` class)

| Chart Type | Series Class |
|------------|-------------|
| Box and Whisker | `BoxAndWhiskerSeries` |
| Funnel | `FunnelSeries` |
| Histogram | `HistogramSeries` |
| Pareto | `ParetoSeries` |
| Filled Map | `MapSeries` |
| Sunburst | `SunburstSeries` |
| Treemap | `TreemapSeries` |
| Waterfall | `WaterfallSeries` |

## Key API

| Member | Description |
|--------|-------------|
| `Chart` | Standard chart shape — add to `slide.Shapes` |
| `ChartEx` | Office 2016+ chart shape — add to `slide.Shapes` |
| `Chart.Series` | `SeriesCollection` — add `BarSeries`, `LineSeries`, etc. |
| `ChartEx.Series` | `DataSeriesExCollection` — add `WaterfallSeries`, etc. |
| `Chart.Views` | Associated chart views — auto-created when series are added |
| `ChartBase.Data` | Embedded spreadsheet (`ChartData`) — set cell values by reference |
| `ChartDataReference(from, to)` | References a range in the embedded spreadsheet |
| `ChartStringData(string[])` | Inline string category/argument data |
| `ChartNumericData(double[])` | Inline numeric value data |
| `BarChartView.DataLabels` | `BarDataLabels` with `ShowValue`, `ShowCategoryName` |
| `Chart.ArgumentAxis` | Primary X-axis (`CategoryAxis` or `DateAxis` or `ValueAxis`) |
| `Chart.ValueAxis` | Primary Y-axis (`ValueAxis`) |
| `Chart.SecondaryArgumentAxis` / `.SecondaryValueAxis` | Secondary axes |
| `ChartEx.ArgumentAxis` | `CategoryAxisEx` — ChartEx X-axis |
| `ChartEx.ValueAxis` | `ValueAxisEx` — ChartEx Y-axis |
| `AxisScaling` | Min/Max/LogBase/Orientation for axis scale |
| `ErrorBar` | Error bar on a series |
| `TrendLine` | Trend line on a series |
| `ShapeBase.Width` / `.Height` | Chart size in points |

## Load Data from Embedded Spreadsheet Cells

Use `ChartDataReference` to point series to cells in the chart's embedded spreadsheet (`Chart.Data`). This is the primary approach when data comes from the same spreadsheet cells. Use `ChartStringData` / `ChartNumericData` for purely inline data.

```csharp
Chart chart = new Chart();
slide.Shapes.Add(chart);

// Populate the embedded spreadsheet
chart.Data[0, "A1"].TextValue = "Q1";
chart.Data[0, "A2"].TextValue = "Q2";
chart.Data[0, "A3"].TextValue = "Q3";
chart.Data[0, "B1"].NumericValue = 120;
chart.Data[0, "B2"].NumericValue = 95;
chart.Data[0, "B3"].NumericValue = 140;

BarSeries series = new BarSeries();
series.Arguments = new ChartDataReference(sheetIndex: 0, fromCellReference: "A1", toCellReference: "A3");
series.Values    = new ChartDataReference(sheetIndex: 0, fromCellReference: "B1", toCellReference: "B3");
chart.Series.Add(series);
```

`ChartDataReference(string from, string to)` uses the default sheet (index 0). Add a `sheetIndex` parameter when you have multiple sheets.

## Axes

Chart axes are created automatically when the first series is added. Primary axes:

| Property | Type | Series types |
|----------|------|-------------|
| `Chart.ArgumentAxis` | `CategoryAxis` or `DateAxis` | Bar, Line, Area, Column |
| `Chart.ArgumentAxis` | `ValueAxis` | Scatter, Bubble |
| `Chart.ValueAxis` | `ValueAxis` | all of the above |
| `Chart.SeriesAxis` | `SeriesAxis` | Surface, Bar3D |

```csharp
// Customize primary argument axis
CategoryAxis xAxis = chart.ArgumentAxis as CategoryAxis;
if (xAxis != null) {
    xAxis.Scaling = new AxisScaling { Min = 1, Max = 3 };
    xAxis.MajorTickMark = AxisTickMarkType.Cross;
    xAxis.Title = new ChartTitleOptions(new TextArea("Quarter"));
}

// Customize primary value axis
ValueAxis yAxis = chart.ValueAxis;
if (yAxis != null) {
    yAxis.NumberFormat = new NumberFormatOptions { FormatCode = "$0" };
    yAxis.LabelTextProperties = new TextProperties { FontSize = 14 };
}
```

### Secondary Axes

```csharp
// Assign a series to secondary axes
series3.AxisGroup = ChartAxisGroupType.Secondary;
chart.SecondaryArgumentAxis.Visible = false;
chart.SecondaryValueAxis.Position = AxisPositionType.Right;
```

### ChartEx Axes

`ChartEx` uses `CategoryAxisEx` and `ValueAxisEx`:

```csharp
CategoryAxisEx xAxis = chartEx.ArgumentAxis;
if (xAxis != null)
    xAxis.TextProperties = new TextProperties { FontSize = 24 };

ValueAxisEx yAxis = chartEx.ValueAxis;
if (yAxis != null)
    yAxis.TextProperties = new TextProperties { FontSize = 24 };
```

## Data Labels

Initialize `DataLabels` on a view (affects all series in that view) or on an individual series (overrides view settings).

```csharp
// View-level: all bar series
BarChartView view = (BarChartView)chart.Views[0];
view.DataLabels = new BarDataLabels {
    ShowValue    = true,
    ShowCategoryName = true,
    LabelPosition = BarDataLabelPosition.OutsideEnd,
    NumberFormat = new NumberFormatOptions { FormatCode = "#,##0.00", IsSourceLinked = false }
};

// Series-level
barSeries.DataLabels = new BarDataLabels { ShowValue = true };

// Per-data-point override (zero-based index)
barSeries.DataLabels[1] = new BarDataLabel { ShowCategoryName = true, LabelPosition = BarDataLabelPosition.InsideEnd };

// Hide a specific point label
barSeries.DataLabels[2] = new BarDataLabel { Hidden = true };

// Pie / doughnut
pieSeries.DataLabels = new PieDataLabels { ShowValue = true, ShowPercent = true, Separator = "\n" };
```

Available `Show*` properties: `ShowValue`, `ShowCategoryName`, `ShowSeriesText`, `ShowLegendKey`, `ShowPercent` (pie), `ShowBubbleSize` (bubble), `ShowDataLabelRange`.

## Combo Charts

Add multiple series of different compatible types to the same `Chart`:

```csharp
// Line + Bar + Area
LineSeries lineSeries = new LineSeries();
lineSeries.Arguments = new ChartDataReference("A1", "A4");
lineSeries.Values    = new ChartDataReference("B1", "B4");
chart.Series.Add(lineSeries);

BarSeries barSeries = new BarSeries();
barSeries.Arguments = new ChartDataReference("A1", "A4");
barSeries.Values    = new ChartDataReference("C1", "C4");
chart.Series.Add(barSeries);

AreaSeries areaSeries = new AreaSeries();
areaSeries.Arguments = new ChartDataReference("A1", "A4");
areaSeries.Values    = new ChartDataReference("D1", "D4");
chart.Series.Add(areaSeries);
```

Series types can be combined only if they share compatible axes. 3D series cannot be combined with 2D series (throws `InvalidOperationException`).

## Customize Individual Series Points

Use the series `CustomDataPoints` property to customize individual data points. It is a dictionary mapping zero-based point indices to `DataPoint` objects:

```csharp
using DevExpress.Docs.Office;
using DevExpress.Docs.Presentation;
using System.Drawing;

// Customize markers on a line series
lineSeries.Marker.Symbol = MarkerStyle.Circle;
lineSeries.Marker.Size = 20;

// Override the first data point with a red fill
lineSeries.CustomDataPoints = new DataPointDictionary {
    { 0, new DataPoint { Marker = new Marker { Fill = new SolidFill(Color.Red) } } }
};
```

```vb
Imports DevExpress.Docs.Office
Imports DevExpress.Docs.Presentation
Imports System.Drawing

lineSeries.Marker.Symbol = MarkerStyle.Circle
lineSeries.Marker.Size = 20

lineSeries.CustomDataPoints = New DataPointDictionary() From {
    {0, New DataPoint() With {
        .Marker = New Marker() With {.Fill = New SolidFill(Color.Red)}
    }}
}
```

## Legend

`Chart.Legend` (or `ChartEx.Legend`) returns a `Legend` (`LegendEx` for ChartEx) object. The legend is visible only when the chart has at least one series. Set `chart.Legend = null` to remove it.

```csharp
using DevExpress.Docs.Office;
using DevExpress.Docs.Presentation;
using System.Drawing;

Legend legend = chart.Legend;

// Position and appearance
legend.Position = LegendPositionType.TopRight;
legend.AllowOverlapChart = true;
legend.TextProperties = new TextProperties { FontSize = 16, Bold = true };
legend.Fill = new SolidFill(Color.AliceBlue);
legend.OutlineStyle = new OutlineStyle {
    Fill = new SolidFill(Color.SlateGray),
    Width = 2
};

// Customize an individual entry (index = series index)
legend.CustomEntries.Add(0, new LegendEntry {
    TextProperties = new TextProperties { Fill = new SolidFill(Color.Red) }
});
```

```vb
Imports DevExpress.Docs.Office
Imports DevExpress.Docs.Presentation
Imports System.Drawing

Dim legend As Legend = chart.Legend

legend.Position = LegendPositionType.TopRight
legend.AllowOverlapChart = True
legend.TextProperties = New TextProperties() With {.FontSize = 16, .Bold = True}
legend.Fill = New SolidFill(Color.AliceBlue)

legend.CustomEntries.Add(0, New LegendEntry() With {
    .TextProperties = New TextProperties() With {.Fill = New SolidFill(Color.Red)}
})
```

`LegendPositionType` values: `Bottom`, `Top`, `Left`, `Right`, `TopRight`.  
`LegendEntry.Hidden = true` hides an individual legend entry.

## Error Bars and Trend Lines

Supported on `LineSeries`, `BarSeries`, `AreaSeries`, `BubbleSeries`, `ScatterSeries`.

```csharp
// Error bars
lineSeries.ErrorBars = new ErrorBarCollection {
    new ErrorBar {
        Axis      = ErrorBarAxis.Vertical,
        Direction = ErrorBarDirection.Both,
        Type      = ErrorValueType.FixedValue,
        Value     = 1.5,
        ShowCap   = true
    }
};

// Trend line
lineSeries.TrendLine = new TrendLine {
    Type                = TrendLineType.Linear,
    ShowEquationLabel   = true,
    ShowRSquaredValue   = true,
    ForecastForward     = 1
};
```

`TrendLine.Type` values: `Linear`, `Exponential`, `Logarithmic`, `Polynomial`, `Power`.

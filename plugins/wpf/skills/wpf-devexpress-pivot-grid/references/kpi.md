# KPI (Key Performance Indicators) — DevExpress WPF Pivot Grid

The Pivot Grid renders **KPI status / trend / value / goal / weight** indicators with server-defined or client-defined graphics. KPI support is most natural with **OLAP cubes** (Analysis Services), where KPIs are first-class cube elements, but it also works with **table data sources** that supply integer KPI values.

## When to Use This Reference

Use this when you need to:

- Display KPI values from an Analysis Services OLAP cube
- Show traffic-light / arrow / cylinder status icons for measures
- Configure KPI graphics for non-OLAP data sources
- Replace built-in KPI graphics with a custom DataTemplate

## KPI Concept

A KPI in Analysis Services is a bundle of:

| KPI Component | Meaning |
|---|---|
| **Value** | The actual measured value (e.g., current sales) |
| **Goal** | The target value (e.g., budgeted sales) |
| **Status** | Discrete state — typically `-1` (bad), `0` (neutral), `1` (good) |
| **Trend** | Discrete state — typically `-1` (worsening), `0` (flat), `1` (improving) |
| **Weight** | Importance multiplier for aggregating KPIs |

Each KPI component is internally just a measure. To display a KPI, you bind a `PivotGridField` to the measure and tell the field which **`KpiType`** it represents.

## Discover Cube KPIs

```csharp
// All KPI names defined on the cube
List<string> kpiNames = pivotGridControl1.GetOlapKpiList();

// Measure names that back a specific KPI
PivotOlapKpiMeasures measures = pivotGridControl1.GetOlapKpiMeasures("Revenue");
// measures.ValueMeasure, measures.GoalMeasure, measures.StatusMeasure,
// measures.TrendMeasure, measures.WeightMeasure

// Current KPI value (returns a structured object with Value/Goal/Status/Trend/Weight)
PivotOlapKpiValue actual = pivotGridControl1.GetOlapKpiValue("Revenue");
```

## Add KPI Fields to the Pivot

```csharp
PivotOlapKpiMeasures m = pivotGridControl1.GetOlapKpiMeasures("Revenue");

// Status field — shows good/neutral/bad icon based on cube-defined status measure
var statusField = pivotGridControl1.Fields.Add();
statusField.Caption = "Revenue Status";
statusField.Area = FieldArea.DataArea;
statusField.DataBinding = new DataSourceColumnBinding(m.StatusMeasure);
statusField.KpiGraphic = PivotKpiGraphic.ServerDefined;   // Use cube's chosen icons

// Trend field
var trendField = pivotGridControl1.Fields.Add();
trendField.Caption = "Revenue Trend";
trendField.Area = FieldArea.DataArea;
trendField.DataBinding = new DataSourceColumnBinding(m.TrendMeasure);
trendField.KpiGraphic = PivotKpiGraphic.ServerDefined;

// Value field (just a number, no icon)
var valueField = pivotGridControl1.Fields.Add();
valueField.Caption = "Revenue";
valueField.Area = FieldArea.DataArea;
valueField.DataBinding = new DataSourceColumnBinding(m.ValueMeasure);
```

> `PivotGridField.KpiType` is **read-only** — it is set automatically by the control based on the measure (cube metadata) or by the OLAP server. You configure `KpiGraphic` to choose the icon set, but you do not assign `KpiType` from user code.

## KPI Properties

| Property | Use |
|---|---|
| `PivotGridField.KpiType` | **Read-only.** Which KPI component this field represents — set by the control: `Value`, `Goal`, `Status`, `Trend`, `Weight`. |
| `PivotGridField.KpiGraphic` | Graphic set for `Status` / `Trend`. `ServerDefined`, `Cylinder`, `ReversedCylinder`, `RoadSigns`, `TrafficLights`, `Faces`, `Shapes`, `Gauge`, `ReversedGauge`, `Thermometer`, `ReversedThermometer`, `StatusArrow`, `ReversedStatusArrow`, `StandardArrow`, `VarianceArrow`, `None`. |

### Server vs Client Graphics

- **`PivotKpiGraphic.ServerDefined`** — uses the icon set defined by the cube author. Available only for OLAP data sources.
- **Any other value** (e.g., `Cylinder`, `TrafficLight`) — uses a built-in DevExpress icon set. Works for **table data sources too**, provided the value field supplies the integer values `-1`, `0`, or `1`.

```csharp
statusField.KpiGraphic = PivotKpiGraphic.TrafficLights;   // Works with any source
```

### Get Server's Chosen Graphic

```csharp
PivotKpiGraphic serverGraphic =
    pivotGridControl1.GetOlapKpiServerGraphic("Revenue", PivotKpiType.Status);
```

### Fetch Graphic Bitmaps Manually

```csharp
ImageSource good     = pivotGridControl1.GetKpiBitmap(PivotKpiGraphic.Cylinder, +1);
ImageSource neutral  = pivotGridControl1.GetKpiBitmap(PivotKpiGraphic.Cylinder,  0);
ImageSource bad      = pivotGridControl1.GetKpiBitmap(PivotKpiGraphic.Cylinder, -1);
```

Useful when you want to render KPI graphics outside the pivot (in a status panel, tooltip, etc.).

## Customize KPI Cell Appearance

Apply a global template:

```xaml
<dxpg:PivotGridControl FieldCellKpiTemplate="{StaticResource MyKpiCellTemplate}"/>
```

Per-field override:

```csharp
statusField.CellTemplate = (DataTemplate)FindResource("RevenueStatusTemplate");
statusField.CellTemplateSelector = new MyKpiTemplateSelector();
```

Or use a `DataTemplateSelector` for control-wide conditional templating:

```xaml
<dxpg:PivotGridControl FieldCellKpiTemplateSelector="{StaticResource MyKpiSelector}"/>
```

> **Note**: `FieldCellTemplate` and `FieldCellTemplateSelector` (the non-KPI variants) are ignored for fields that render KPI graphics. Use the **KPI-specific** template properties for KPI fields.

## KPI Without OLAP (Table Data Source)

If your data source is a regular `DataTable` / `List<T>` and you have a column whose values are `-1`, `0`, `1`, you can render KPI graphics directly:

```csharp
public class CountryStat {
    public string Country { get; set; } = "";
    public decimal Sales { get; set; }
    public int SalesStatus { get; set; }   // -1, 0, or 1
}

var statusField = pivotGridControl1.Fields.Add();
statusField.Area = FieldArea.DataArea;
statusField.DataBinding = new DataSourceColumnBinding("SalesStatus");
statusField.KpiType = PivotKpiType.Status;
statusField.KpiGraphic = PivotKpiGraphic.TrafficLights;   // NOT ServerDefined for table sources
```

## Common Issues

- **No icons appear, just numbers** — `KpiGraphic` is `None`, or for table data sources you used `ServerDefined` (which requires an OLAP cube).
- **Icons but wrong direction** — `KpiType` mismatch. `Status` and `Trend` are visually similar but semantically distinct; the cube defines which icon set goes with which.
- **Custom template ignored** — used `FieldCellTemplate` instead of `FieldCellKpiTemplate` (or per-field `CellTemplate` is fine, but `FieldCellTemplate` is bypassed for KPI fields).
- **Values outside `-1, 0, 1`** — KPI graphics expect exactly these three values. For continuous values, use Conditional Formatting (icon sets) instead.

## Source Material

- `articles/controls-and-libraries/pivot-grid/data-analysis/key-performance-indicators-kpis.md` (`xref:11641`)
- [Analysis Services KPI documentation](https://docs.microsoft.com/en-us/sql/analysis-services/multidimensional-models/key-performance-indicators-kpis-in-multidimensional-models)

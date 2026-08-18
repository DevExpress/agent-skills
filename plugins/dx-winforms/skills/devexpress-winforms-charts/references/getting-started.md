# Getting Started

This reference covers the first chart end-to-end: NuGet packages, namespaces, dropping a `ChartControl` on a form, the four-level element hierarchy, and a minimal first chart in both designer-driven and code-only flavors.

For data binding details see [data-binding.md](data-binding.md). For picking a series view see [series-and-diagrams.md](series-and-diagrams.md).

## When to Use This Reference

- First time adding a `ChartControl` to a WinForms project.
- Confirming which NuGet package(s) to reference.
- Understanding the `ChartControl` → `Diagram` → `Series` → `View` hierarchy before configuring anything else.
- Running the **Chart Designer** at runtime for end-user customization.

## NuGet Packages

| Package | Provides | When Needed |
|---|---|---|
| `DevExpress.Win.Charts` | `ChartControl`, all diagrams, all series views, Chart Designer. | Always — the core package. |
| `DevExpress.Win.Printing` | `ShowPrintPreview()`, `ExportToPdf`/`ExportToImage`/`ExportToXlsx`. | When you need print or file export. |
| `DevExpress.Win` | Umbrella metapackage that includes Charts. | When you already use many DX controls. |

```xml
<ItemGroup>
  <PackageReference Include="DevExpress.Win.Charts" Version="26.1.*" />
</ItemGroup>
```

Assemblies brought in by `DevExpress.Win.Charts`:

- `DevExpress.XtraCharts.v26.1.UI.dll` — WinForms-specific (the `ChartControl` itself).
- `DevExpress.XtraCharts.v26.1.dll` — cross-platform core (series, diagrams, scales).

(The `v26.1` segment in these assembly names matches your installed DevExpress version.)

## Namespaces

```csharp
using DevExpress.XtraCharts;          // ChartControl, Series, ViewType, all diagrams and views
using DevExpress.Utils;               // DefaultBoolean (used by Visibility properties)
using DevExpress.XtraEditors;         // XtraForm if you want skin-aware host form
```

For native designer assets (skin-aware controls, repository items) also `using DevExpress.XtraEditors.Repository;`.

## Add a `ChartControl` to a Form

```csharp
var chart = new ChartControl {
    Dock = DockStyle.Fill
};
Controls.Add(chart);
```

The control is now empty (no diagram, no series) and shows a placeholder image.

## The Chart Element Hierarchy

```text
ChartControl
└── Diagram                     ← exactly one (null until first series is added)
    ├── XYDiagram               (bar / line / area / point / bubble / financial / boxplot)
    ├── SimpleDiagram           (pie / doughnut / nested-doughnut / funnel)
    ├── GanttDiagram            (side-by-side / overlapped gantt)
    ├── RadarDiagram            (radar point / line / area / range-area)
    ├── PolarDiagram            (polar point / line / area / range-area)
    ├── SwiftPlotDiagram        (high-volume datasets)
    ├── SimpleDiagram3D         (3D pie / doughnut)
    └── XYDiagram3D             (3D bar / line / area / manhattan)
        └── Series[]            ← collection of Series owned by the chart
            ├── Series.Name     identifier
            ├── Series.View     a *SeriesView instance (BarSeriesView, LineSeriesView, PieSeriesView, …)
            └── Series.Points / DataSource / ArgumentDataMember / ValueDataMembers
```

Key consequences:

- **The diagram type is implicit.** It is determined by the first series' `ViewType`. Add a `ViewType.Pie` series first → diagram becomes `SimpleDiagram`. Add `ViewType.Bar` first → `XYDiagram`. You cannot set `Diagram` directly.
- **`chart.Diagram` is `null`** until at least one series is added. Null-check before casting.
- **Only series views from a compatible diagram can coexist.** Trying to add a `ViewType.Pie` series to a chart that already has a `ViewType.Bar` series throws.
- **`Series.View`** holds the actual visual settings — color, point markers, transparency. Cast to the concrete type (`(BarSeriesView)series.View`) for view-specific options.

## Minimal First Chart — Unbound

```csharp
var chart = new ChartControl { Dock = DockStyle.Fill };
Controls.Add(chart);

var series = new Series("Sales", ViewType.Bar);
series.Points.Add(new SeriesPoint("Q1", 120));
series.Points.Add(new SeriesPoint("Q2", 145));
series.Points.Add(new SeriesPoint("Q3", 168));
series.Points.Add(new SeriesPoint("Q4", 192));
chart.Series.Add(series);
```

This produces an `XYDiagram` with a single bar series and a `Qualitative` X-axis (since the argument is a `string`).

## Minimal First Chart — Bound

```csharp
public record MonthlySales(DateTime Month, decimal Total);

var data = new List<MonthlySales> {
    new(new DateTime(2026, 1, 1), 12_300m),
    new(new DateTime(2026, 2, 1), 14_100m),
    new(new DateTime(2026, 3, 1), 13_750m),
};

var series = new Series("Revenue", ViewType.Line);
series.DataSource          = data;
series.ArgumentDataMember  = nameof(MonthlySales.Month);
series.ValueDataMembers.AddRange(nameof(MonthlySales.Total));
series.ArgumentScaleType   = ScaleType.DateTime;

chart.Series.Add(series);
```

See [data-binding.md](data-binding.md) for every binding flavor.

## Chart Title

```csharp
chart.Titles.Add(new ChartTitle {
    Text       = "Monthly Revenue",
    Alignment  = StringAlignment.Center,
    Font       = new Font("Segoe UI", 14, FontStyle.Bold)
});
```

`ChartControl.Titles` is a collection — add as many as you need; subsequent titles stack vertically.

## Chart Designer (Runtime End-User Customization)

The **Chart Designer** is a full GUI editor for every chart property — series, diagrams, axes, legends, tooltips, palette. Show it to end users at runtime:

```csharp
using DevExpress.XtraCharts.Designer;

new ChartDesigner(chart).Show();
```

The runtime designer requires `DevExpress.XtraCharts.v26.1.Design.dll` (the `v26.1` segment matches your installed DevExpress version; added automatically when the design assembly is referenced).

## Common First-Chart Pitfalls

- **`chart.Diagram` is `null`** — add a series first. Casting to `XYDiagram` before adding any series throws `NullReferenceException`.
- **Wrong diagram type after adding the wrong series first** — clear `Series` and re-add in the intended order, or build the diagram via an initial dummy series of the right `ViewType`.
- **Dates show as numbers** — set `series.ArgumentScaleType = ScaleType.DateTime` explicitly.
- **Currency shows raw numbers** — set `Axis.Label.TextPattern = "{V:c0}"` (see [axis-titles-and-labels.md](axis-titles-and-labels.md)).

## Common Issues

- **`ViewType` enum value not found** — confirm the `using DevExpress.XtraCharts;` directive; some niche views (e.g., `ViewType.Bar3D`) require referencing the 3D-enabled package version.
- **Chart shows placeholder image** — no series added or all series are empty. Add data via `Points` or `DataSource`.
- **License dialog appears** — register a valid DevExpress license (`licenses.licx`) or use a trial activation.

## Source Material

- `articles/controls-and-libraries/chart-control/getting-started.md` (`xref:WindowsForms.117539`).
- `articles/controls-and-libraries/chart-control/fundamentals/chart-elements.md` (`xref:WindowsForms.5394`).
- `api/DevExpress.XtraCharts.ChartControl.yml`.
- `api/DevExpress.XtraCharts.Series.yml`.
- `api/DevExpress.XtraCharts.Designer.ChartDesigner.yml`.

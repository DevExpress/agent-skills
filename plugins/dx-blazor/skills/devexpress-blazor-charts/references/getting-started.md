# Getting Started with DevExpress Blazor Charts

## System Requirements

- .NET 8.0+ (NuGet package targets .NET 8+)
- Visual Studio 2022+ or JetBrains Rider
- DevExpress packages available on NuGet.org

## Installation

### Step 1: Install the Package

```bash
dotnet add package DevExpress.Blazor
```

Or in Package Manager Console:
```
Install-Package DevExpress.Blazor
```

### Step 2: Register Services

In `Program.cs`:
```csharp
using DevExpress.Blazor;

builder.Services.AddDevExpressBlazor();
```

### Step 3: Add Imports

In `_Imports.razor`:
```razor
@using DevExpress.Blazor
```

### Step 4: Apply Theme and Scripts

In `App.razor`, inside `<head>`:
```razor
@using DevExpress.Blazor
@DxResourceManager.RegisterTheme(Themes.Fluent)
@DxResourceManager.RegisterScripts()
```

### Step 5: Enable Interactivity

Blazor Charts support static render mode to display data as static images. To use zoom/pan, selection, tooltips, and other interactive features, enable interactivity on the Razor page:

```razor
@rendermode InteractiveServer
```

Or set it at component level in a .NET 8 Blazor Web App:
```razor
<DxChart @rendermode="InteractiveServer" Data="DataSource">
    ...
</DxChart>
```

## Your First Chart

### Step 1: Create a Data Model

In your `Services/` (or `Data/`) folder:

```csharp
// DataPoint.cs
public class DataPoint {
    public string Country { get; set; }
    public double AppleProduction { get; set; }
    public DataPoint(string country, double appleProduction) {
        Country = country;
        AppleProduction = appleProduction;
    }
}

public class DataPointProvider {
    public static List<DataPoint> ReturnPoints() {
        return new List<DataPoint>() {
            new DataPoint("USA", 4.21),
            new DataPoint("China", 3.33),
            new DataPoint("Turkey", 2.6),
            new DataPoint("Italy", 2.2),
            new DataPoint("India", 2.16),
        };
    }
}
```

### Step 2: Create the Chart Component

```razor
@rendermode InteractiveServer
@using MyBlazorApp.Services

<DxChart Data="DataSource">
    <DxChartLineSeries ArgumentField="@((DataPoint v) => v.Country)"
                       ValueField="@((DataPoint v) => v.AppleProduction)"
                       Name="Apples">
        <DxChartSeriesLabel Visible="true" />
    </DxChartLineSeries>
    <DxChartTitle Text="Fruit Production By Country">
        <DxChartSubTitle Text="(Millions of Tons)" />
    </DxChartTitle>
    <DxChartLegend Visible="false" />
</DxChart>

@code {
    IEnumerable<DataPoint> DataSource;

    protected override void OnInitialized() {
        DataSource = DataPointProvider.ReturnPoints();
    }
}
```

## Chart vs. Pie Chart vs. Polar Chart

| Component | When to use |
|---|---|
| `DxChart<T>` | Cartesian coordinate charts (X/Y axes): line, bar, area, scatter, bubble, financial |
| `DxPieChart<T>` | Circular proportion charts; use `InnerDiameter > 0` for donut |
| `DxPolarChart<T>` | Polar coordinate charts; set `UseSpiderWeb="true"` for radar/spider-web appearance |

All three use the same `Data` property and similar series child components. Descriptive element components (`DxChartTitle`, `DxChartLegend`, `DxChartSeriesLabel`) are shared across all three chart types.

## Async Data Binding

For async data sources (services, databases):

```razor
@rendermode InteractiveServer

<DxChart Data="@ChartData">
    <DxChartLineSeries ValueField="@((DataPoint i) => i.Value)"
                       ArgumentField="@(i => i.Date)"
                       Name="Temperature, °C" />
    <DxChartTitle Text="Weather Forecast" />
    <DxChartLegend Visible="false" />
</DxChart>

@code {
    IEnumerable<DataPoint> ChartData;

    protected override async Task OnInitializedAsync() {
        // Replace with your real source:
        //   @inject HttpClient Http
        //   ChartData = await Http.GetFromJsonAsync<List<DataPoint>>("api/forecast");
        ChartData = await Task.FromResult(new List<DataPoint> {
            new DataPoint(DateTime.Today.AddDays(-3), 12.3),
            new DataPoint(DateTime.Today.AddDays(-2), 15.1),
            new DataPoint(DateTime.Today.AddDays(-1), 11.8),
            new DataPoint(DateTime.Today,             14.0),
        });
    }

    record DataPoint(DateTime Date, double Value);
}
```

## Troubleshooting Setup

| Issue | Solution |
|---|---|
| `dx-blazor.css` not found | Add `@DxResourceManager.RegisterTheme(Themes.Fluent)` and `@DxResourceManager.RegisterScripts()` to `App.razor` inside `<head>` |
| Chart renders but is empty | Check that `Data` is not null or empty; call `StateHasChanged()` after async load |
| License error at startup | Register your DevExpress license key per the installation guide |
| "Could not find 'X' in window.DxBlazor" | JavaScript resources not loaded; check layout file for the required `<script>` tags |

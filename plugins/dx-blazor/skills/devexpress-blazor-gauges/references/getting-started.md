# Getting Started with DevExpress Blazor Visualization Components

This guide covers setup for `DxBarGauge`, `DxRangeSelector`, `DxSankey`, `DxSparkline`, and `DxMap`.

## 1. Install the NuGet Package

All five components ship in the `DevExpress.Blazor` package.

```bash
dotnet add package DevExpress.Blazor
```

Alternatively, add to your `.csproj`:

```xml
<PackageReference Include="DevExpress.Blazor" Version="26.1.*" />
```

The package is available on NuGet.org.

## 2. Register Services

In `Program.cs`:

```csharp
builder.Services.AddDevExpressBlazor();
```

## 3. Add to Imports

In `_Imports.razor`:

```razor
@using DevExpress.Blazor
```

## 4. Apply Theme and Scripts

In `App.razor`, inside `<head>`:

```razor
@using DevExpress.Blazor
@DxResourceManager.RegisterTheme(Themes.Fluent)
@DxResourceManager.RegisterScripts()
```

## 5. Configure Interactive Render Mode

Slider interaction (`DxRangeSelector`), hover/click events (`DxSankey`, `DxMap`), and user interaction (`DxBarGauge` tooltip) require an interactive render mode. Add this to your page or set it globally:

```razor
@rendermode InteractiveServer
```

Or, set `InteractiveWebAssembly`/`InteractiveAuto` depending on your project's hosting model.

See the [interactive render mode guide](https://docs.devexpress.com/Blazor/403727) for details on configuring render mode at the page, component, or app level.

## 6. DxMap: GIS Provider API Key

`DxMap` requires an API key from a GIS provider:

- **Azure Maps**: Create a resource in the Azure portal. Use `MapProvider.Azure`.
- **Google Maps**: Obtain a key from the Google Cloud Console. Use `MapProvider.Google`.
- **GoogleStatic**: Obtain a key from the Google Cloud Console. Use `MapProvider.GoogleStatic`.

> **Note:** Bing Maps is deprecated and no longer supported.

Pass the key when you declare the map:

```razor
<DxMap Provider="MapProvider.Azure"
       Zoom="10">
    <DxMapApiKeys Azure="YOUR-AZURE-MAPS-KEY" />
</DxMap>
```

## First Component: DxSparkline

The simplest component to start with — no interactivity or API key required:

```razor
@using DevExpress.Blazor

<DxSparkline Data="@DataSource"
             Type="SparklineType.Bar"
             ArgumentFieldName="Month"
             ValueFieldName="VisitorCount"
             Height="50px"
             Width="200px" />

@code {
    IEnumerable<SparklineDataPoint> DataSource = Enumerable.Empty<SparklineDataPoint>();
    protected override void OnInitialized() {
        DataSource = new List<SparklineDataPoint> {
            new SparklineDataPoint(1, 2210),
            new SparklineDataPoint(2, 2103),
            new SparklineDataPoint(3, 2132),
            new SparklineDataPoint(4, 2000),
        };
    }
    record SparklineDataPoint(int Month, int VisitorCount);
}
```

## Demos

- Bar Gauge: `https://demos.devexpress.com/blazor/BarGaugeGeometry`
- Sparkline: `https://demos.devexpress.com/blazor/ChartSparkline`
- Range Selector: `https://demos.devexpress.com/blazor/RangeSelectorOverview`
- Map: `https://demos.devexpress.com/blazor/Map`

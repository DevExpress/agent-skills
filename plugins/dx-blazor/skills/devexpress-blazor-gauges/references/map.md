# DxMap Reference

`DxMap` renders an interactive geo map with support for markers and routes. It integrates with Azure Maps, Google Maps, or Google Static Maps as the tile provider.

API Reference: [DxMap members](xref:DevExpress.Blazor.DxMap._members)

Demo: `https://demos.devexpress.com/blazor/Map`

> **Important:** Bing Maps is deprecated and is no longer supported. Use Azure Maps, Google Maps, or GoogleStatic instead.

## API Key Setup

`DxMap` requires a valid API key from your chosen GIS provider:

| Provider | Where to Get Key | Free Tier |
|---|---|---|
| Azure Maps | Azure Portal → Create a **Maps** resource | 1,000 map tiles/month free |
| Google Maps | Google Cloud Console → **Maps JavaScript API** | $200/month credit for new accounts |
| GoogleStatic | Google Cloud Console → **Maps Static API** | Included in the $200/month credit |

> **Quick start tip:** `MapProvider.GoogleStatic` renders a non-interactive static image and works with the same Google API key. It's the easiest option to test locally without setting up a full interactive tile provider.

Pass the key using `DxMapApiKeys`:

```razor
<DxMap Provider="MapProvider.Azure" Zoom="12" Width="950px" Height="500px">
    <DxMapApiKeys Azure="YOUR-AZURE-MAPS-KEY" />
</DxMap>
```

For Google:
```razor
<DxMap Provider="MapProvider.Google" Zoom="12" Width="950px" Height="500px">
    <DxMapApiKeys Google="YOUR-GOOGLE-MAPS-KEY" />
</DxMap>
```

## Basic Map with Markers

```razor
@rendermode InteractiveServer
@using DevExpress.Blazor

<DxMap Provider="MapProvider.Azure"
       Zoom="12"
       Width="950px"
       Height="500px">
    <DxMapApiKeys Azure="YOUR-AZURE-MAPS-KEY" />
    <DxMapMarkers>
        <DxMapMarker>
            <DxMapMarkerLocation Latitude="51.5074" Longitude="-0.1278" />
            <DxMapMarkerTooltip Text="London" />
        </DxMapMarker>
        <DxMapMarker>
            <DxMapMarkerLocation GeoPosition="48.8566,2.3522" />
            <DxMapMarkerTooltip Text="Paris" />
        </DxMapMarker>
    </DxMapMarkers>
</DxMap>
```

### Marker Location Formats

You can specify a marker's location in two ways:

```razor
@* Separate Latitude/Longitude properties *@
<DxMapMarkerLocation Latitude="51.5074" Longitude="-0.1278" />

@* Combined GeoPosition string ("lat,lon") *@
<DxMapMarkerLocation GeoPosition="51.5074,-0.1278" />
```

### Custom Marker Icons

```razor
<DxMapMarker MarkerId="BusStation" IconUrl="images/bus-station-mark.png">
    <DxMapMarkerLocation GeoPosition="51.481633,-0.008281" />
</DxMapMarker>
```

## Map with Routes

Use `DxMapRoutes` and `DxMapRoute` to draw routes between waypoints:

```razor
<DxMap Zoom="15" Provider="MapProvider.Azure" Width="950px" Height="500px">
    <DxMapApiKeys Azure="YOUR-AZURE-MAPS-KEY" />
    <DxMapRoutes>
        <DxMapRoute Color="coral" Weight="9" Mode="MapRouteMode.Walking">
            <DxMapRouteLocations>
                <DxMapRouteLocation GeoPosition="51.519852,-0.077593" />
                <DxMapRouteLocation GeoPosition="51.514763,-0.080787" />
                <DxMapRouteLocation Latitude="51.512471" Longitude="-0.082368" />
            </DxMapRouteLocations>
        </DxMapRoute>
    </DxMapRoutes>
    <DxMapMarkers>
        <DxMapMarker>
            <DxMapMarkerLocation GeoPosition="51.519852,-0.077593" />
            <DxMapMarkerTooltip Text="Start" />
        </DxMapMarker>
    </DxMapMarkers>
</DxMap>
```

`MapRouteMode` values:
- `MapRouteMode.Driving` — Car route
- `MapRouteMode.Walking` — Walking route

## Click Events

### Marker Click

```razor
<DxMap ...
       MarkerClick="OnMapMarkerClick">
    <DxMapMarkers>
        <DxMapMarker MarkerId="HQ">
            <DxMapMarkerLocation GeoPosition="51.5074,-0.1278" />
        </DxMapMarker>
    </DxMapMarkers>
</DxMap>

@code {
    void OnMapMarkerClick(MapMarkerClickEventArgs e) {
        Console.WriteLine($"Marker clicked: {e.Marker.MarkerId}");
    }
}
```

### Map Click (any point on the map)

```razor
<DxMap ...
       MapClick="OnMapClick">
</DxMap>

@code {
    void OnMapClick(MapClickEventArgs e) {
        Console.WriteLine($"Map clicked at {e.Location.Latitude}, {e.Location.Longitude}");
    }
}
```

## Navigation Controls

| Property | Type | Description |
|---|---|---|
| `Zoom` | `int` | Initial zoom level (1–20) |
| `Width` / `Height` | `string` | Component dimensions |

Users can navigate the map using mouse drag (pan) and scroll wheel (zoom) in interactive render modes. Use `MapProvider.GoogleStatic` for a non-interactive static image.

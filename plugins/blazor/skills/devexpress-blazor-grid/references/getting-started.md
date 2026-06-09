# Getting Started with Blazor Grid

When you need to: set up the Grid from scratch, create your first grid with columns and data, enable interactive render mode.

## Prerequisites

- .NET 8, 9, or 10
- `DevExpress.Blazor` NuGet package installed from the DevExpress feed
- A valid DevExpress license

## Step 1 — Install the Package

```bash
dotnet add package DevExpress.Blazor
```

DevExpress package source: `https://nuget.devexpress.com/free/api`

## Step 2 — Register Resources in `Program.cs`

```csharp
using DevExpress.Blazor;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Register DevExpress Blazor services
builder.Services.AddDevExpressBlazor();

var app = builder.Build();
// ...
```

## Step 3 — Apply Theme and Scripts

In `App.razor` (Blazor Web App), inside `<head>`:

```razor
@using DevExpress.Blazor
@DxResourceManager.RegisterTheme(Themes.Fluent)
@DxResourceManager.RegisterScripts()
```

## Step 4 — Add the Namespace

In `_Imports.razor`:

```razor
@using DevExpress.Blazor
```

## Step 5 — Add the Grid to a Page

Create or edit a Razor page. The Grid requires an **interactive render mode** — add `@rendermode InteractiveServer` (or `InteractiveWebAssembly` / `InteractiveAuto`) at the top.

```razor
@page "/weather"
@rendermode InteractiveServer
@inject WeatherForecastService ForecastService

<DxGrid Data="@Forecasts" KeyFieldName="Id">
    <Columns>
        <DxGridDataColumn FieldName="Date" DisplayFormat="d" />
        <DxGridDataColumn FieldName="TemperatureC" Caption="Temp (°C)" />
        <DxGridDataColumn FieldName="Forecast" />
    </Columns>
</DxGrid>

@code {
    List<WeatherForecast> Forecasts { get; set; }

    protected override void OnInitialized() {
        Forecasts = ForecastService.GetForecast();
    }
}
```

## Render Mode Notes

| Render Mode | Sorting | Filtering | Editing | Export |
|---|---|---|---|---|
| Static SSR (no rendermode) | ❌ | ❌ | ❌ | ❌ |
| `InteractiveServer` | ✅ | ✅ | ✅ | ✅ |
| `InteractiveWebAssembly` | ✅ | ✅ | ✅ | ✅ |
| `InteractiveAuto` | ✅ | ✅ | ✅ | ✅ |

> **Note**: The Grid can display static read-only data in static SSR mode, but interactive features (sorting, paging, editing) require an interactive render mode.

## Using the DevExpress Template Kit (Fastest Setup)

The DevExpress Template Kit (available for Visual Studio, VS Code, and JetBrains Rider) creates a pre-configured Blazor project with the Grid already set up. See https://docs.devexpress.com/Blazor/405308 for installation instructions.

## See Also

- [Data Binding](data-binding.md)
- [Columns & Templates](columns-and-templates.md)
- [Editing & Validation](editing-and-validation.md)

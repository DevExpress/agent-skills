# Getting Started with Blazor ComboBox

When you need to: set up the ComboBox from scratch, create your first drop-down selector with data binding, enable interactive render mode.

## Prerequisites

- .NET 8, 9, or 10
- `DevExpress.Blazor` NuGet package from NuGet.org
- A valid DevExpress license
- Interactive render mode (InteractiveServer, InteractiveWebAssembly, or InteractiveAuto) — `DxComboBox` does not work in Static SSR

## Step 1 — Install the Package

```bash
dotnet add package DevExpress.Blazor
```

DevExpress packages are available on NuGet.org.

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

## Step 3 — Apply Theme and Client Scripts in `App.razor`

Add inside the `<head>` section of `Components/App.razor`:

```razor
@using DevExpress.Blazor
@DxResourceManager.RegisterTheme(Themes.Fluent)
@DxResourceManager.RegisterScripts()
```

> Without `RegisterScripts()`, drop-down open/close and keyboard navigation will not work.

## Step 4 — Add the Namespace

In `_Imports.razor`:

```razor
@using DevExpress.Blazor
```

## Step 5 — Add a ComboBox to a Page

The ComboBox requires an **interactive render mode** — add `@rendermode InteractiveServer` (or `InteractiveWebAssembly` / `InteractiveAuto`) to the page.

### Simple String List

```razor
@page "/combobox-demo"
@rendermode InteractiveServer

<DxComboBox Data="@Cities"
            @bind-Value="@SelectedCity"
            NullText="Select a city…"
            ClearButtonDisplayMode="DataEditorClearButtonDisplayMode.Auto" />

@code {
    IEnumerable<string> Cities = new List<string> { "London", "Berlin", "Paris" };
    string SelectedCity { get; set; }
}
```

### Cascading ComboBoxes (Complete Example)

```razor
@* CascadingComboBox.razor — requires InteractiveServer or InteractiveWebAssembly *@
<div style="display:flex; gap:1rem;">
    <DxComboBox Data="@Countries"
                TextFieldName="@nameof(Country.Name)"
                Value="@SelectedCountry"
                ValueChanged="@((Country c) => OnCountryChanged(c))"
                NullText="Select country…" />

    <DxComboBox Data="@FilteredCities"
                TextFieldName="@nameof(City.Name)"
                @bind-Value="@SelectedCity"
                NullText="Select city…" />
</div>

@code {
    record Country(int Id, string Name);
    record City(int Id, int CountryId, string Name);

    static readonly List<Country> Countries = new()
    {
        new(1, "Germany"), new(2, "France"), new(3, "USA")
    };

    static readonly List<City> AllCities = new()
    {
        new(1, 1, "Berlin"), new(2, 1, "Munich"),
        new(3, 2, "Paris"), new(4, 2, "Lyon"),
        new(5, 3, "New York"), new(6, 3, "Chicago")
    };

    Country SelectedCountry { get; set; } = Countries[0];
    City SelectedCity { get; set; }
    List<City> FilteredCities { get; set; } = new();

    protected override void OnInitialized() => OnCountryChanged(SelectedCountry);

    void OnCountryChanged(Country country)
    {
        SelectedCountry = country;
        FilteredCities = AllCities.Where(c => c.CountryId == country.Id).ToList();
        SelectedCity = FilteredCities.FirstOrDefault();
    }
}
```

## Render Mode Notes

| Render Mode | Drop-down | Search | Events |
|---|---|---|---|
| Static SSR (no rendermode) | ❌ | ❌ | ❌ |
| `InteractiveServer` | ✅ | ✅ | ✅ |
| `InteractiveWebAssembly` | ✅ | ✅ | ✅ |
| `InteractiveAuto` | ✅ | ✅ | ✅ |

> `DxComboBox` does not function in Static SSR mode. Interactive render mode is required.

## See Also

- [Data Binding](data-binding.md)
- [Buttons & Cascading](buttons-and-cascading.md)
- [Validation](validation.md)

# Getting Started — Blazor Pivot Table

When you need to: install the required packages; configure namespaces; display your first pivot table.

## Prerequisites

- .NET 8, 9, or 10
- Two NuGet packages: `DevExpress.Blazor.PivotTable` and `DevExpress.PivotGrid.Core` from NuGet.org
- A valid DevExpress license
- Interactive render mode (`InteractiveServer`, `InteractiveWebAssembly`, or `InteractiveAuto`)

> **Note**: DevExpress Blazor components require .NET 8 or later. .NET Framework is not supported — Blazor itself is a .NET Core/.NET 5+ technology.

## Step 1 — Install NuGet Packages

The Pivot Table requires **two** NuGet packages:

```bash
dotnet add package DevExpress.Blazor.PivotTable
dotnet add package DevExpress.PivotGrid.Core
```

> **Warning**: Installing only `DevExpress.Blazor.PivotTable` without `DevExpress.PivotGrid.Core` will result in runtime errors.

## Step 2 — Register Services

In `Program.cs`:

```csharp
builder.Services.AddDevExpressBlazor();
```

## Step 3 — Apply Theme and Scripts

In `App.razor`, inside `<head>`:

```razor
@using DevExpress.Blazor
@DxResourceManager.RegisterTheme(Themes.Fluent)
@DxResourceManager.RegisterScripts()
```

## Step 4 — Add Namespaces

In `_Imports.razor`, add **both** namespaces:

```razor
@using DevExpress.Blazor
@using DevExpress.Blazor.PivotTable
```

> The `DevExpress.Blazor.PivotTable` namespace is separate from `DevExpress.Blazor` and contains `DxPivotTable`, `DxPivotTableField`, and the `PivotTableArea`, `PivotTableSummaryType`, and `PivotTableGroupInterval` enums.

## Step 5 — Add the Pivot Table to a Page

```razor
@page "/pivot"
@rendermode InteractiveServer

<DxPivotTable Data="@Sales">
    <Fields>
        <DxPivotTableField Field="@nameof(Sale.Country)"
                           Area="PivotTableArea.Row"
                           Caption="Country" />
        <DxPivotTableField Field="@nameof(Sale.OrderDate)"
                           Area="PivotTableArea.Column"
                           GroupInterval="PivotTableGroupInterval.DateYear"
                           Caption="Year" />
        <DxPivotTableField Field="@nameof(Sale.Amount)"
                           Area="PivotTableArea.Data"
                           SummaryType="PivotTableSummaryType.Sum"
                           Caption="Total ($)" />
    </Fields>
</DxPivotTable>

@code {
    List<Sale> Sales { get; set; }

    protected override void OnInitialized() {
        Sales = new List<Sale> {
            new Sale { Country = "USA", OrderDate = new DateTime(2024, 1, 15), Amount = 1500 },
            new Sale { Country = "UK", OrderDate = new DateTime(2024, 3, 22), Amount = 800 },
            new Sale { Country = "Germany", OrderDate = new DateTime(2024, 6, 10), Amount = 1200 },
        };
    }

    class Sale {
        public string Country { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal Amount { get; set; }
    }
}
```

## Render Mode Requirements

`DxPivotTable` requires an interactive render mode for:
- Expanding/collapsing row and column headers
- Dragging fields in the field list
- Filtering data

```razor
@rendermode InteractiveServer
```

Or for WebAssembly:

```razor
@rendermode InteractiveWebAssembly
```

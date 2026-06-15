# Getting Started — DevExpress Blazor Toolbar

## When to Use This Reference

- Setting up `DxToolbar` for the first time
- Understanding unbound vs. bound item modes
- Creating a minimal working toolbar

## Prerequisites

- .NET 8.0, 9.0, or 10.0
- Interactive render mode for click handling (InteractiveServer, InteractiveWebAssembly, or InteractiveAuto)
- Access to NuGet.org and a valid DevExpress license

## Step 1: Install NuGet Package

Install the package from NuGet.org:

```bash
dotnet add package DevExpress.Blazor
```

## Step 2: Register DevExpress Resources

### Program.cs

```csharp
builder.Services.AddDevExpressBlazor();
```

### App.razor

Add theme and script registration inside the `<head>` section of `Components/App.razor`:

```razor
@using DevExpress.Blazor
@DxResourceManager.RegisterTheme(Themes.Fluent)
@DxResourceManager.RegisterScripts()
```

> Without these calls, DevExpress components will appear unstyled and client-side interactivity will not work.

## Step 3: Add Global Using (optional)

Add to `_Imports.razor` to avoid repeating `@using` in every file:

```razor
@using DevExpress.Blazor
```

## Step 4: Add DxToolbar to a Page

### Option A: Unbound Mode (Inline Items)

Declare items directly as child components:

```razor
@page "/toolbar-demo"
@rendermode InteractiveServer
@using DevExpress.Blazor

<DxToolbar>
    <DxToolbarItem Text="New" IconCssClass="oi oi-file" Click="OnNew" />
    <DxToolbarItem Text="Open" IconCssClass="oi oi-folder" Click="OnOpen" />
    <DxToolbarItem Text="Save" IconCssClass="oi oi-cloud-upload"
                   BeginGroup="true" Click="OnSave" />
    <DxToolbarItem IconCssClass="oi oi-cog"
                   Alignment="ToolbarItemAlignment.Right" />
</DxToolbar>

@code {
    void OnNew()  { }
    void OnOpen() { }
    void OnSave() { }
}
```

### Option B: Bound Mode (Data Collection)

Bind to a hierarchical data collection using `Data` and `DxToolbarDataMapping`:

```razor
@page "/toolbar-bound"
@rendermode InteractiveServer
@using DevExpress.Blazor

<DxToolbar Data="@toolbarItems">
    <DataMappings>
        <DxToolbarDataMapping Text="Name" Key="Id" ParentKey="ParentId" />
    </DataMappings>
</DxToolbar>

@code {
    record ToolbarMenuItem(int Id, int? ParentId, string Name);

    List<ToolbarMenuItem> toolbarItems = new() {
        new(1, null, "File"),
        new(2, 1,    "New"),
        new(3, 1,    "Open"),
        new(4, 1,    "Save"),
        new(5, null, "Edit"),
        new(6, 5,    "Cut"),
        new(7, 5,    "Copy"),
        new(8, 5,    "Paste"),
    };
}
```

## Step 5: Verify the Build

```bash
dotnet build
```

Common issues and fixes:

| Error | Fix |
|---|---|
| `DxToolbar` not recognized | Add `@using DevExpress.Blazor` or update `_Imports.razor` |
| Click handlers not firing | Add `@rendermode InteractiveServer` to the page |
| License error | Verify DevExpress license registration |

## Placing the Toolbar in a Layout

To add a Toolbar to the application shell, edit `MainLayout.razor`:

```razor
@inherits LayoutComponentBase
@using DevExpress.Blazor

<DxToolbar>
    <DxToolbarItem Text="Home" NavigateUrl="/" />
    <DxToolbarItem Text="Counter" NavigateUrl="/counter" />
    <DxToolbarItem Text="Weather" NavigateUrl="/weather" />
</DxToolbar>

<main>
    @Body
</main>
```

> Items with `NavigateUrl` are rendered as links and work in static SSR without interactive mode.

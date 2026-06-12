# Getting Started — DevExpress Blazor Ribbon

## When to Use This Reference

- Setting up `DxRibbon` for the first time
- Creating a complete minimal ribbon with tabs, groups, and items
- Adding an Application tab (File menu)

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

## Step 4: Add DxRibbon to a Page

### Minimal Ribbon (No Application Tab)

```razor
@page "/ribbon"
@rendermode InteractiveServer
@using DevExpress.Blazor

<DxRibbon>
    <DxRibbonTab Text="Home">
        <DxRibbonGroup>
            <DxRibbonItem Text="Cut" />
            <DxRibbonItem Text="Copy" />
            <DxRibbonItem Text="Paste" />
        </DxRibbonGroup>
        <DxRibbonGroup>
            <DxRibbonItem Text="Bold" />
            <DxRibbonItem Text="Italic" />
            <DxRibbonItem Text="Underline" />
        </DxRibbonGroup>
    </DxRibbonTab>
    <DxRibbonTab Text="Insert">
        <DxRibbonGroup>
            <DxRibbonItem Text="Table" />
            <DxRibbonItem Text="Image" />
        </DxRibbonGroup>
    </DxRibbonTab>
</DxRibbon>
```

### Ribbon with Application Tab

`DxRibbonApplicationTab` renders a special "File" button at the left edge of the tab strip. Child `DxRibbonApplicationTabItem` elements placed between the open and close tags become the drop-down menu.

> **Always use open/close tags.** A self-closing `<DxRibbonApplicationTab />` renders the button with an empty menu.

```razor
<DxRibbon>
    <DxRibbonApplicationTab Text="File" Click="@(() => IsFileMenuOpen = true)">
        <DxRibbonApplicationTabItem Text="New" />
        <DxRibbonApplicationTabItem Text="Open" />
        <DxRibbonApplicationTabItem Text="Save" />
        <DxRibbonApplicationTabItem Text="Save As" />
        <DxRibbonApplicationTabItem Text="Print" />
    </DxRibbonApplicationTab>
    <DxRibbonTab Text="Home">
        <DxRibbonGroup>
            <DxRibbonItem Text="Command 1" />
        </DxRibbonGroup>
    </DxRibbonTab>
</DxRibbon>

@code {
    bool IsFileMenuOpen { get; set; } = false;
}
```

## Ribbon Structure Reference

```
DxRibbon
├── DxRibbonApplicationTab          ← Optional "File" button
│   └── DxRibbonApplicationTabItem  ← Menu items (nestable for sub-menus)
├── DxRibbonTab (Text="Home")       ← Regular tab
│   ├── DxRibbonGroup               ← Group container
│   │   ├── DxRibbonItem            ← Button
│   │   ├── DxRibbonToggleGroup     ← Container for radio-group toggles
│   │   │   └── DxRibbonToggleItem  ← Toggle button
│   │   ├── DxRibbonComboBoxItem    ← Inline combobox
│   │   ├── DxRibbonSpinEditItem    ← Inline spin editor
│   │   ├── DxRibbonColorGroup      ← Color palette container
│   │   │   └── DxRibbonColorPaletteItem
│   │   └── DxRibbonItem (SplitDropDownButton="true")  ← Split button
│   └── DxRibbonGroup               ← Additional group in same tab
└── DxRibbonTab (Contextual="true") ← Contextual tab (conditional visibility)
```

## Step 5: Verify the Build

```bash
dotnet build
```

Common issues and fixes:

| Error | Fix |
|---|---|
| `DxRibbon` not recognized | Add `@using DevExpress.Blazor` or update `_Imports.razor` |
| Item clicks not firing | Add `@rendermode InteractiveServer` to the page |
| License error | Verify DevExpress license registration |
| Contextual tab always visible | Ensure `Visible="@boolProperty"` is set on the tab |

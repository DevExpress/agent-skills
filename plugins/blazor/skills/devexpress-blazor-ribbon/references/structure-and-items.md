# Structure and Items — DevExpress Blazor Ribbon

## When to Use This Reference

- Understanding available ribbon item types
- Using toggle items and radio-group toggles
- Adding combo boxes or spin editors to ribbon groups
- Adding color palette pickers
- Adding icons and click handlers to items

## Ribbon Structure Overview

The ribbon uses a hierarchical structure:

```
DxRibbon → DxRibbonTab → DxRibbonGroup → [item components]
```

Items are always placed inside a `DxRibbonGroup`. Groups are always placed inside a `DxRibbonTab`.

## Standard Item: DxRibbonItem

A button-style command item:

```razor
<DxRibbonGroup>
    <DxRibbonItem Text="Cut"   Click="OnCut" />
    <DxRibbonItem Text="Copy"  Click="OnCopy" />
    <DxRibbonItem Text="Paste" Click="OnPaste" />
</DxRibbonGroup>

@code {
    void OnCut()   { }
    void OnCopy()  { }
    void OnPaste() { }
}
```

### Split Drop-Down Button

Use `SplitDropDownButton="true"` to render a button with a separate drop-down arrow:

```razor
<DxRibbonItem Text="Paste" SplitDropDownButton="true" Click="OnPaste">
    <Items>
        <DxRibbonItem Text="Paste Special" />
        <DxRibbonItem Text="Paste as Text" />
    </Items>
</DxRibbonItem>
```

## Toggle Items: DxRibbonToggleItem

A ribbon toggle button that switches between checked and unchecked states. Use `DxRibbonToggleGroup` to create radio-group behavior (only one checked at a time):

### Independent Toggle (On/Off)

```razor
<DxRibbonGroup>
    <DxRibbonToggleItem Text="Bold"   @bind-Checked="IsBold" />
    <DxRibbonToggleItem Text="Italic" @bind-Checked="IsItalic" />
</DxRibbonGroup>

@code {
    bool IsBold   { get; set; } = false;
    bool IsItalic { get; set; } = false;
}
```

### Radio Group Toggles

Wrap items in `DxRibbonToggleGroup` for radio-group behavior:

```razor
<DxRibbonGroup>
    <DxRibbonToggleGroup>
        <DxRibbonToggleItem Text="Align Left"   @bind-Checked="AlignLeft" />
        <DxRibbonToggleItem Text="Align Center" @bind-Checked="AlignCenter" />
        <DxRibbonToggleItem Text="Align Right"  @bind-Checked="AlignRight" />
    </DxRibbonToggleGroup>
</DxRibbonGroup>

@code {
    bool AlignLeft   { get; set; } = true;
    bool AlignCenter { get; set; } = false;
    bool AlignRight  { get; set; } = false;
}
```

> Items in a `DxRibbonToggleGroup` behave as radio buttons — selecting one automatically unchecks the others.

## Combo Box Item: DxRibbonComboBoxItem

An inline combo box embedded directly in a ribbon group. Use generic type parameters `<TData, TValue>`:

```razor
<DxRibbonGroup>
    <DxRibbonComboBoxItem Data="@FontNames"
                          @bind-Value="CurrentFont"
                          NullText="Font"
                          Width="140px" />
    <DxRibbonComboBoxItem Data="@FontSizes"
                          @bind-Value="CurrentFontSize"
                          AllowUserInput="true"
                          NullText="Size"
                          Width="80px" />
</DxRibbonGroup>

@code {
    string? CurrentFont { get; set; }
    int? CurrentFontSize { get; set; }

    List<string> FontNames = new() { "Arial", "Calibri", "Georgia", "Times New Roman" };
    List<int>    FontSizes = new() { 8, 10, 12, 14, 16, 18, 24, 36, 48 };
}
```

For complex data types, specify `TextFieldName`:

```razor
<DxRibbonComboBoxItem Data="@FontFamilies"
                      @bind-Value="SelectedFamily"
                      TextFieldName="@nameof(FontFamilyInfo.Name)"
                      Width="160px" />

@code {
    record FontFamilyInfo(string Name, bool IsMonospace);

    FontFamilyInfo? SelectedFamily;
    List<FontFamilyInfo> FontFamilies = new() {
        new("Arial", false),
        new("Consolas", true),
    };
}
```

### DxRibbonComboBoxItem Key Properties

| Property | Type | Description |
|---|---|---|
| `Data` | `IEnumerable<TData>` | Data collection |
| `@bind-Value` | `TValue` | Selected value (two-way) |
| `TextFieldName` | `string` | Property name to display for complex types |
| `AllowUserInput` | `bool` | Enables free-text editing |
| `NullText` | `string` | Placeholder when no value is selected |
| `Width` | `string` | CSS width (e.g., `"120px"`) |

## Spin Editor Item: DxRibbonSpinEditItem

An inline numeric spin editor embedded in a ribbon group:

```razor
<DxRibbonGroup>
    <DxRibbonSpinEditItem Value="@FontSize"
                          ValueChanged="@((int v) => FontSize = v)"
                          MinValue="6"
                          MaxValue="72"
                          Increment="2"
                          Width="80px" />
</DxRibbonGroup>

@code {
    int FontSize { get; set; } = 12;
}
```

### DxRibbonSpinEditItem Key Properties

| Property | Type | Description |
|---|---|---|
| `Value` | `T` | Current numeric value |
| `ValueChanged` | `EventCallback<T>` | Fires when the value changes |
| `MinValue` | `T` | Minimum allowed value |
| `MaxValue` | `T` | Maximum allowed value |
| `Increment` | `T` | Step size for each spin |
| `Width` | `string` | CSS width |

## Color Palette Item: DxRibbonColorPaletteItem

An inline color picker that displays a color swatch grid:

```razor
<DxRibbonGroup>
    <DxRibbonColorGroup>
        <DxRibbonColorPaletteItem @bind-Value="HighlightColor"
                                   Colors="@StandardColors"
                                   ShowNoColorTile="true">
            <IconTemplate>
                <div style="display:flex;flex-direction:column;align-items:center;gap:2px">
                    <span class="oi oi-pencil" style="font-size:14px"></span>
                    <span style="height:4px;width:16px;background:@HighlightColor;display:block"></span>
                </div>
            </IconTemplate>
        </DxRibbonColorPaletteItem>
    </DxRibbonColorGroup>
</DxRibbonGroup>

@code {
    string HighlightColor = "#FFFF00";

    List<string> StandardColors = new() {
        "#FF0000", "#FF6600", "#FFFF00",
        "#00FF00", "#00FFFF", "#0000FF",
        "#8B00FF", "#FF00FF", "#FFFFFF",
        "#CCCCCC", "#888888", "#000000"
    };
}
```

### DxRibbonColorPaletteItem Key Properties

| Property | Type | Description |
|---|---|---|
| `@bind-Value` | `string` | Selected color (CSS color or hex, two-way) |
| `Colors` | `List<string>` | Color list displayed in the palette |
| `PaletteCssClass` | `string` | CSS class applied to the palette container |
| `ShowNoColorTile` | `bool` | Shows a "no color" swatch at the start |
| `IconTemplate` | `RenderFragment` | Custom button icon (typically shows current color) |

`DxRibbonColorGroup` is a container for one or more `DxRibbonColorPaletteItem` components.

## Enabled / Visible State

All ribbon item components support `Enabled` and `Visible`:

```razor
<DxRibbonItem Text="Delete" Enabled="@canDelete" />
<DxRibbonItem Text="Admin Only" Visible="@isAdmin" />
```

## DxRibbonGroup Labels and Adaptive Collapse

Groups can collapse into labeled drop-down buttons when the ribbon narrows. Use `Text` and `IconCssClass` to define the collapsed button appearance — without these, collapsed groups show no label.

```razor
<DxRibbonTab Text="Home">
    <DxRibbonGroup Text="Clipboard" IconCssClass="dx-icon-copy" AdaptivePriority="2">
        <DxRibbonItem Text="Cut" />
        <DxRibbonItem Text="Copy" />
        <DxRibbonItem Text="Paste" />
    </DxRibbonGroup>
    <DxRibbonGroup Text="Font" IconCssClass="dx-icon-bold" AdaptivePriority="1">
        <DxRibbonItem Text="Bold" />
        <DxRibbonItem Text="Italic" />
    </DxRibbonGroup>
    <DxRibbonGroup Text="Style">
        <!-- AdaptivePriority defaults to 0 — collapses last -->
    </DxRibbonGroup>
</DxRibbonTab>
```

`AdaptivePriority` on a group controls group-level collapse order (higher value collapses first). Item-level `AdaptivePriority` controls which individual items collapse within a group — both levels work independently.

## Item Tooltips, Primary Highlight, and Navigation Links

### Tooltip and IsPrimary

```razor
<DxRibbonGroup Text="Actions">
    <DxRibbonItem Text="Save"
                  IconCssClass="dx-icon-save"
                  Tooltip="Save the document"
                  IsPrimary="true" />
    <DxRibbonItem Text="Print"
                  IconCssClass="dx-icon-print"
                  Tooltip="Print the document" />
</DxRibbonGroup>
```

`IsPrimary="true"` applies an accent color highlight. It is visual-only and does not affect behavior or keyboard navigation.

### NavigateUrl

Use `NavigateUrl` to make a ribbon item a navigation link — no click handler needed:

```razor
<DxRibbonGroup Text="Help">
    <DxRibbonItem Text="Documentation" NavigateUrl="https://docs.devexpress.com" />
    <DxRibbonItem Text="Support" NavigateUrl="https://supportcenter.devexpress.com" />
</DxRibbonGroup>
```

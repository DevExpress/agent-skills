# Appearance Customization

The Pivot Grid's look is layered: override individual **theme colors** for grid-specific tweaks, apply **styles** to particular element types (cells, field headers, field values), or replace the entire visual tree of an element with a **template**. Most projects start with a couple of color overrides; templates come in when you need rich content inside cells.

## When to Use This Reference

Use this when you need to:

- Tweak the grid's colors (alternate row, selection, totals, ...) without changing the whole theme
- Apply a `Style` to a category of element (every data cell, every field header)
- Replace a cell's or field-value's visual tree with a `DataTemplate` (icons, sparklines, two-line text)
- Customize the cell template based on the current data (`CellTemplateSelector`)

## Override Theme Colors for the Pivot Grid

Use `PivotGridControl` color properties to override the theme's color choices for specific elements without touching the theme itself. See the [full list](https://docs.devexpress.com/WPF/11213). Common ones:

| Property | Use |
|---|---|
| `CellBackground` | Data cell background |
| `CellSelectedBackground` / `CellSelectedForeground` | Selected cell |
| `CellTotalBackground` | Totals row/column background |
| `ValueBackground` | Row/column field-value headers |
| `ValueTotalBackground` | Grand-total headers |
| `ValueSelectedBackground` | Selected field value header |

```xaml
<dxpg:PivotGridControl CellBackground="WhiteSmoke"
                       CellTotalBackground="LightYellow"
                       ValueTotalBackground="Khaki"
                       CellSelectedBackground="LightSkyBlue"/>
```

For broader theme color overrides (via `AppearanceThemeKey`), see [How to: Override Theme Colors](https://docs.devexpress.com/WPF/11220).

## Pivot Grid Styles

Each element type has a `*Style` property that takes a WPF `Style`. The target type is the underlying control (see [Pivot Grid Styles](https://docs.devexpress.com/WPF/8401) for the full list); the common ones:

| `PivotGridControl` member | Targets |
|---|---|
| `CellStyle` | Data cells |
| `FieldHeaderContentStyle` | Field headers (row/column/data/filter area) |
| `FieldValueStyle` | Field value headers (e.g., individual "Country: USA" header) |
| `FilteringPanelStyle` | The panel that shows currently applied filters |
| `FieldListStyle` / `FieldGeneratorStyle` / `GroupGeneratorStyle` | Customization-form pieces (field list popup, etc.) |

```xaml
<dxpg:PivotGridControl.CellStyle>
    <Style TargetType="{x:Type dxpg:CellElement}">
        <Setter Property="Background"  Value="WhiteSmoke"/>
        <Setter Property="Foreground"  Value="DarkSlateGray"/>
        <Setter Property="FontFamily"  Value="Consolas"/>
        <Setter Property="HorizontalContentAlignment" Value="Right"/>
        <Setter Property="Padding"     Value="6,2"/>
    </Style>
</dxpg:PivotGridControl.CellStyle>
```

The `CellStyle` setters apply to every data cell uniformly. For selected-cell or totals colors, use the dedicated color properties above (`CellSelectedBackground`, `CellTotalBackground`, ...) — `CellElement` does not expose `IsSelected`/`IsGrandTotal` style triggers.

To color cells based on their role or value, handle the `CustomCellAppearance` event:

```csharp
pivotGrid.CustomCellAppearance += (s, e) => {
    if (e.RowValueType == FieldValueType.GrandTotal) {
        e.Background = Brushes.Khaki;
    }
};
```

## Templates — Replace the Visual Tree

Templates replace the entire content of an element with a `DataTemplate`. Bound data comes through the `DataContext` — for data cells it is `CellElementData`; see [Pivot Grid Elements That Support Templates](https://docs.devexpress.com/WPF/8400) for the full list.

### `FieldCellTemplate` — Custom Data Cell

```xaml
<dxpg:PivotGridControl.FieldCellTemplate>
    <DataTemplate>
        <Grid>
            <Grid.ColumnDefinitions>
                <ColumnDefinition Width="*"/>
                <ColumnDefinition Width="Auto"/>
            </Grid.ColumnDefinitions>
            <TextBlock Grid.Column="0"
                       Text="{Binding DisplayText}"
                       VerticalAlignment="Center"/>
            <Image    Grid.Column="1" Width="16" Height="16" Margin="4,0,0,0"
                       Source="{Binding Value,
                                Converter={StaticResource ValueToTrendIconConverter}}"/>
        </Grid>
    </DataTemplate>
</dxpg:PivotGridControl.FieldCellTemplate>
```

`DataContext` for `FieldCellTemplate` is a `CellElementData`, which exposes:
- `Value` — raw cell value
- `DisplayText` — formatted display string
- `Field` — the data field this cell belongs to
- `RowValue` / `ColumnValue` — the row/column values at this cell (and `RowValueDisplayText` / `ColumnValueDisplayText`)
- `RowIndex` / `ColumnIndex` — visual indexes
- `RowTotalValue` / `ColumnTotalValue` — totals for the cell's row/column

### `FieldValueTemplate` — Custom Row/Column Header Cell

```xaml
<dxpg:PivotGridControl.FieldValueTemplate>
    <DataTemplate>
        <StackPanel Orientation="Horizontal" Margin="4,2">
            <Image Width="14" Height="14"
                   Source="{Binding Value, Converter={StaticResource FlagConverter}}"/>
            <TextBlock Margin="6,0,0,0" Text="{Binding DisplayText}"/>
        </StackPanel>
    </DataTemplate>
</dxpg:PivotGridControl.FieldValueTemplate>
```

Useful for showing flags next to country names, glyphs next to product categories, etc.

### Per-Field Templates

Each `PivotGridField` has its own template hooks:

| `PivotGridField` member | Use |
|---|---|
| `CellTemplate` | Per-field data cell template (overrides the grid-level one for cells where this is the data field) |
| `ValueTemplate` | Per-field value-header template (overrides the grid-level one for this field's values) |

```xaml
<dxpg:PivotGridField Name="fieldCountry" Area="RowArea" FieldName="Country">
    <dxpg:PivotGridField.ValueTemplate>
        <DataTemplate>
            <StackPanel Orientation="Horizontal">
                <Image Width="14" Source="{Binding Value, Converter={StaticResource Flag}}"/>
                <TextBlock Margin="6,0,0,0" Text="{Binding DisplayText}"/>
            </StackPanel>
        </DataTemplate>
    </dxpg:PivotGridField.ValueTemplate>
</dxpg:PivotGridField>
```

### Template Selectors — Different Templates by Data

When you need different visuals for different cells in the same area, use a `DataTemplateSelector`:

```csharp
public class CellTemplateSelector : DataTemplateSelector {
    public DataTemplate Negative { get; set; }
    public DataTemplate Default  { get; set; }

    public override DataTemplate SelectTemplate(object item, DependencyObject container) {
        if (item is CellElementData info &&
            info.Value is decimal v && v < 0) return Negative;
        return Default;
    }
}
```

```xaml
<Window.Resources>
    <local:CellTemplateSelector x:Key="cellTplSelector">
        <local:CellTemplateSelector.Negative>
            <DataTemplate>
                <TextBlock Foreground="Red" FontWeight="Bold" Text="{Binding DisplayText}"/>
            </DataTemplate>
        </local:CellTemplateSelector.Negative>
        <local:CellTemplateSelector.Default>
            <DataTemplate>
                <TextBlock Text="{Binding DisplayText}"/>
            </DataTemplate>
        </local:CellTemplateSelector.Default>
    </local:CellTemplateSelector>
</Window.Resources>

<dxpg:PivotGridControl FieldCellTemplateSelector="{StaticResource cellTplSelector}" .../>
```

## Common Patterns

### Pattern 1: Color Tweaks Only

```xaml
<dxpg:PivotGridControl CellBackground="#FAFAFA"
                       CellSelectedBackground="#CFE2F3"
                       ValueBackground="#EAEAEA"
                       ValueTotalBackground="#FFE4B5"/>
```

### Pattern 2: Numeric Cell with Trend Icon

```xaml
<dxpg:PivotGridControl.FieldCellTemplate>
    <DataTemplate>
        <Grid>
            <Grid.ColumnDefinitions>
                <ColumnDefinition Width="*"/>
                <ColumnDefinition Width="16"/>
            </Grid.ColumnDefinitions>
            <TextBlock Grid.Column="0" Text="{Binding DisplayText}"
                       TextAlignment="Right" VerticalAlignment="Center"/>
            <Path     Grid.Column="1" Width="12" Height="12"
                       Data="{Binding Value, Converter={StaticResource TrendArrow}}"
                       Fill="{Binding Value, Converter={StaticResource TrendFill}}"/>
        </Grid>
    </DataTemplate>
</dxpg:PivotGridControl.FieldCellTemplate>
```

### Pattern 3: Per-Field Cell Template

Apply a sparkline visualization to one specific data field only:

```xaml
<dxpg:PivotGridField Name="fieldSales" Area="DataArea">
    <dxpg:PivotGridField.CellTemplate>
        <DataTemplate>
            <local:Sparkline Values="{Binding Value}" Stroke="SteelBlue"/>
        </DataTemplate>
    </dxpg:PivotGridField.CellTemplate>
</dxpg:PivotGridField>
```

Other data fields keep the default template.

### Pattern 4: Highlight Grand Totals

Grand-total cells are not selectable via a `CellElement` style trigger. Use the dedicated total color properties for a uniform tint:

```xaml
<dxpg:PivotGridControl CellTotalBackground="LightGoldenrodYellow"
                       ValueTotalBackground="LightGoldenrodYellow"/>
```

…or handle `CustomCellAppearance` to target the grand total specifically:

```csharp
pivotGrid.CustomCellAppearance += (s, e) => {
    if (e.RowValueType == FieldValueType.GrandTotal ||
        e.ColumnValueType == FieldValueType.GrandTotal) {
        e.Background = Brushes.LightGoldenrodYellow;
    }
};
```

### Pattern 5: Per-Cell Conditional Color via Event

For appearance that depends on data (rather than just role), the `CustomCellAppearance` event is simpler than template selectors:

```csharp
pivotGrid.CustomCellAppearance += (s, e) => {
    if (e.Value is decimal v) {
        if (v < 0)        e.Foreground = Brushes.Crimson;
        else if (v > 1e6) e.Background = Brushes.LightGreen;
    }
};
```

But prefer `FormatConditions` ([conditional-formatting.md](conditional-formatting.md)) when the rule fits a built-in type — they persist with the layout and integrate with the user-facing rule manager.

## Common Issues

- **`CellStyle` doesn't apply** — wrong `TargetType` for the style. Use `dxpg:CellElement` for data cells.
- **`FieldCellTemplate` shows blank** — bound to the wrong property. The data context exposes `DisplayText` (formatted) and `Value` (raw); confirm which one is needed.
- **Per-field template applies to only some cells** — `PivotGridField.CellTemplate` overrides the grid-level `FieldCellTemplate` only for cells where that field is the data field; cells of other data fields keep the grid-level template. Set the template on the field whose cells you want to affect, or set `FieldCellTemplate` at the grid level for all cells.
- **Custom colors don't appear in printed/exported output** — `CustomCellAppearance` event-set values may not export. Use `FormatConditions` for export-safe styling, or hook the printing-specific events.
- **`FieldCellTemplateSelector` fires only for some cells** — the grid virtualizes; you'll see the selector run for visible cells only.

## Source Material

- `articles/controls-and-libraries/pivot-grid/appearance.md` (https://docs.devexpress.com/content/WPF/8399?md=true)
- `articles/controls-and-libraries/pivot-grid/appearance/pivot-grid-styles.md` (https://docs.devexpress.com/content/WPF/8401?md=true)
- `articles/controls-and-libraries/pivot-grid/appearance/pivot-grid-elements-that-support-templates.md` (https://docs.devexpress.com/content/WPF/8400?md=true)
- `articles/controls-and-libraries/pivot-grid/appearance/customizing-pivot-grid-colors.md` (https://docs.devexpress.com/content/WPF/11213?md=true)
- `articles/controls-and-libraries/pivot-grid/examples/appearance/how-to-customize-the-cell-template.md` (https://docs.devexpress.com/content/WPF/8038?md=true)
- `articles/controls-and-libraries/pivot-grid/examples/appearance/how-to-create-the-field-value-template.md` (https://docs.devexpress.com/content/WPF/8143?md=true)
- `articles/controls-and-libraries/pivot-grid/examples/appearance/how-to-specify-custom-styles-for-pivot-grid-elements.md` (https://docs.devexpress.com/content/WPF/11221?md=true)
- `articles/controls-and-libraries/pivot-grid/examples/appearance/how-to-override-theme-colors.md` (https://docs.devexpress.com/content/WPF/11220?md=true)

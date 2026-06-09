# Legend

A legend tells users which series is which — and can optionally include indicators, strips, constant lines, and series points. It supports position controls, custom titles, check-box toggles for visibility, formatted item text, custom items, and full template overrides.

## When to Use This Reference

Use this when you need to:

- Add one or more `Legend` objects to the chart
- Position the legend (top, bottom, left, right, inside/outside the diagram)
- Add a title to the legend
- Show check boxes that toggle series visibility
- Format legend item text via `LegendTextPattern`
- Add custom (non-series) items to the legend
- Customize the visual look of legend items

## Add a Legend

```xaml
<dxc:ChartControl>
    <dxc:ChartControl.Legends>
        <dxc:Legend/>
    </dxc:ChartControl.Legends>
    ...
</dxc:ChartControl>
```

A `Legend` with no position settings appears in the top-right corner.

### What Appears in the Legend

The legend automatically lists:

- **Series** whose `Visible` and `ShowInLegend` are `true` (uses `Series.DisplayName`)
- **Indicators** with their `LegendText` (`Indicator.LegendText`)
- **Constant lines** (`ConstantLine.LegendText`)
- **Strips** (`Strip.LegendText`)
- Series points — for **Simple 2D diagrams** automatically; for **Cartesian and Circular diagrams** only if `XYSeries.ColorEach="True"`

Setting `DisplayName` / `LegendText` to `String.Empty` hides the element from the legend.

## Position the Legend

```xaml
<dxc:Legend HorizontalPosition="Center"
            VerticalPosition="BottomOutside"
            Orientation="Horizontal"
            ReverseItems="False"
            IndentFromDiagram="0,16,0,0"/>
```

| Property | Use |
|---|---|
| `HorizontalPosition` | `Left`, `LeftOutside`, `Center`, `Right`, `RightOutside` |
| `VerticalPosition` | `Top`, `TopOutside`, `Center`, `Bottom`, `BottomOutside` |
| `Orientation` | `Horizontal` (items in a row) or `Vertical` (items in a column) |
| `ReverseItems` | Reverse the display order |
| `IndentFromDiagram` | `Thickness` margin between legend and diagram |

`*Outside` values place the legend outside the diagram area (recommended for crowded charts); inside values overlay it on the plot area.

## Multiple Legends

A chart can have any number of legends — useful for multi-pane charts where each pane has its own legend.

```xaml
<dxc:ChartControl>
    <dxc:ChartControl.Legends>
        <dxc:Legend x:Name="topLegend"
                    DockTarget="{Binding ElementName=topPane}"/>
        <dxc:Legend x:Name="bottomLegend"
                    DockTarget="{Binding ElementName=bottomPane}"/>
    </dxc:ChartControl.Legends>

    <dxc:XYDiagram2D>
        <dxc:XYDiagram2D.DefaultPane>
            <dxc:Pane x:Name="topPane"/>
        </dxc:XYDiagram2D.DefaultPane>
        <dxc:XYDiagram2D.Panes>
            <dxc:Pane x:Name="bottomPane"/>
        </dxc:XYDiagram2D.Panes>

        <dxc:LineSeries2D DisplayName="Temperature"/>
        <dxc:AreaSeries2D DisplayName="Pressure"
                          Pane="{Binding ElementName=bottomPane}"
                          Legend="{Binding ElementName=bottomLegend}"/>
    </dxc:XYDiagram2D>
</dxc:ChartControl>
```

| Property | Use |
|---|---|
| `Legend.DockTarget` | The pane (or pane ViewModel) that contains this legend |
| `Series.Legend` | The legend that displays this series's info |

If `Series.Legend` is not set, the series uses the first legend in the collection.

## Add a Title

```xaml
<dxc:Legend>
    <dxc:Legend.Title>
        <dxc:LegendTitle Content="Year" HorizontalAlignment="Center"/>
    </dxc:Legend.Title>
</dxc:Legend>
```

A legend has exactly one title (`LegendTitle`).

| Property | Use |
|---|---|
| `TitleBase.Content` | The title text or `UIElement` |
| `TitleBase.HorizontalAlignment` | `Near`, `Center`, `Far` |
| `TitleBase.ContentTemplate` | Custom rendering template |

## Check Boxes for Series Visibility

Let users toggle series visibility by adding check boxes:

```xaml
<dxc:Legend MarkerMode="CheckBoxAndMarker"/>
```

| `LegendMarkerMode` | Effect |
|---|---|
| `Marker` (default) | Color swatch only |
| `CheckBox` | Check box only |
| `CheckBoxAndMarker` | Both |

Per-element opt-out:

```xaml
<dxc:LineSeries2D CheckableInLegend="True" CheckedInLegend="True"/>
```

| Property | Use |
|---|---|
| `Series.CheckableInLegend` / `Indicator.CheckableInLegend` / `ConstantLine.CheckableInLegend` / `Strip.CheckableInLegend` | Show the check box for this element |
| `Series.CheckedInLegend` / etc. | Initial check state |

> Check boxes for **series points** are not available. Use `Series.FilterCriteria` to hide individual points.

## Format Legend Item Text

`Series.LegendTextPattern` controls how each item is rendered. Combine plain text with placeholders.

```xaml
<dxc:PieSeries2D LegendTextPattern="{}{A}: ${V}M"/>
```

### Placeholders

| Placeholder | Use |
|---|---|
| `{S}` | Series name (`Series.Name`) |
| `{A}` | Argument |
| `{V}` | Value |
| `{VP}` | Value as percentage (Pie, Full-Stacked) |
| `{G}` | Stacked-group name |
| `{TV}` | Total group value (Pie, Stacked) |
| `{W}` | Weight (Bubble series) |
| `{V1}`, `{V2}`, `{VD}` | Range series (first / second / duration) |
| `{VDTD}`, `{VDTH}`, `{VDTM}`, `{VDTS}`, `{VDTMS}` | Range duration in days / hours / minutes / seconds / ms |
| `{OV}`, `{HV}`, `{LV}`, `{CV}` | Financial series — open / high / low / close |

Combine with .NET format specifiers: `{V:F2}` (2 decimals), `{VP:p1}` (percent with 1 decimal).

For data-bound charts, you can also reference data fields directly: `{S}: {V:F2} (Discount: {Discount:P0})`.

> **Always start XAML patterns with `{}`** to escape the leading `{`: `LegendTextPattern="{}{A}: ${V}M"`.

`{S}` requires the series to have a `Name` set.

## Custom Legend Items

Add items that aren't tied to series — totals, headers, footnotes:

```xaml
<dxc:Legend ItemVisibilityMode="AutoGeneratedAndCustom">
    <dxc:Legend.CustomItems>
        <dxc:CustomLegendItem Text="Total: $4390M"
                              MarkerBrush="#FFC3C3C3">
            <dxc:CustomLegendItem.MarkerTemplate>
                <DataTemplate>
                    <Image Source="/Images/sum.png"/>
                </DataTemplate>
            </dxc:CustomLegendItem.MarkerTemplate>
        </dxc:CustomLegendItem>
    </dxc:Legend.CustomItems>
</dxc:Legend>
```

| Property | Use |
|---|---|
| `LegendBase.CustomItems` | Collection of `CustomLegendItem` |
| `LegendBase.ItemVisibilityMode` | `Default` (custom replaces auto) or `AutoGeneratedAndCustom` (both) |
| `CustomLegendItem.Text` | Item caption |
| `CustomLegendItem.MarkerBrush` | Color of the marker swatch |
| `CustomLegendItem.MarkerTemplate` | Custom marker layout |

### MVVM — Generate Custom Items

```xaml
<dxc:Legend CustomItemsSource="{Binding LegendItems}">
    <dxc:Legend.CustomItemTemplate>
        <DataTemplate>
            <dxc:CustomLegendItem Text="{Binding Text}"
                                  MarkerBrush="{Binding Brush}"/>
        </DataTemplate>
    </dxc:Legend.CustomItemTemplate>
</dxc:Legend>
```

## Customize Legend Item Appearance

Override the item template for full visual control:

```xaml
<dxc:Legend HorizontalPosition="Left"
            VerticalPosition="Top"
            Orientation="Vertical">
    <dxc:Legend.ItemTemplate>
        <DataTemplate>
            <dxc:LegendItemContainer>
                <Grid Width="12" Height="12">
                    <Rectangle Stretch="Uniform" Fill="Transparent"/>
                    <dxc:ChartContentPresenter Content="{Binding}"
                                               ContentTemplate="{Binding Path=MarkerTemplate}"/>
                </Grid>
                <TextBlock Text="{Binding Path=Text}"
                           VerticalAlignment="Center"
                           Margin="4"
                           Foreground="{Binding Path=MarkerBrush}"/>
            </dxc:LegendItemContainer>
        </DataTemplate>
    </dxc:Legend.ItemTemplate>
</dxc:Legend>
```

The `DataContext` of the template includes:

- `Text` — the item's caption
- `MarkerBrush` — its color
- `MarkerTemplate` — its default marker layout

Use `dxc:LegendItemContainer` as the template root to keep DevExpress layout behavior.

## Font and Appearance

`Legend` inherits standard `Control` properties: `Background`, `BorderBrush`, `BorderThickness`, `Foreground`, `FontFamily`, `FontSize`, etc.

```xaml
<dxc:Legend Background="#FFFAFAFA"
            BorderBrush="LightGray"
            BorderThickness="1"
            Padding="8"
            FontFamily="Segoe UI"
            FontSize="12"
            Foreground="#FF333333"/>
```

For per-item text styling, use `Legend.ItemTemplate` (above).

## Common Issues

- **Legend doesn't show series** — `Series.ShowInLegend="False"` or `Series.DisplayName` is empty.
- **Legend item text shows literal `{V}`** — pattern is missing the `{}` escape. Use `LegendTextPattern="{}{A}: {V}"`.
- **`{S}` shows empty** — `Series.Name` is not set. Set `Name`, not just `DisplayName`.
- **Check box doesn't appear** — `Legend.MarkerMode` is the default `Marker`; switch to `CheckBox` or `CheckBoxAndMarker`. Also ensure the series has `CheckableInLegend="True"`.
- **Series points not in the legend (Cartesian chart)** — set `XYSeries.ColorEach="True"`. Without this, only the series itself appears (not its points).
- **Custom items replace auto items** — `ItemVisibilityMode` defaults to `Default`. Set to `AutoGeneratedAndCustom` to keep both.
- **Legend overlaps the diagram** — `*Outside` not used. Switch position to `BottomOutside` / `RightOutside` etc.

## Source Material

- `articles/controls-and-libraries/charts-suite/chart-control/legends.md` (https://docs.devexpress.com/content/WPF/6343?md=true)

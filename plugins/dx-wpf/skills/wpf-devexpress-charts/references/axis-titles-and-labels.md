# Axis Titles and Labels

Axis titles are the descriptive captions next to an axis ("Year", "Sales (USD)"). Axis labels are the values rendered along the axis ("Jan 2024", "1,000", "5h"). Both are property-customizable for color, font, alignment, and rotation, and both support patterns (`TextPattern`) and custom formatters (`IAxisLabelFormatter`) for fine-grained content control.

## When to Use This Reference

Use this when you need to:

- Add an `AxisTitle` to an x- or y-axis
- Style the axis title (font, color, alignment)
- Format axis label text via `TextPattern` (`{V} min`, `{A:yyyy}`, etc.)
- Apply a custom `IAxisLabelFormatter`
- Rotate / stagger / hide labels when they overlap
- Define custom labels at specific axis positions (`CustomAxisLabel`)
- Customize axis-label appearance (background, font, angle)

## Axis Titles

### Add a Title

```xaml
<dxc:XYDiagram2D>
    <dxc:XYDiagram2D.AxisX>
        <dxc:AxisX2D>
            <dxc:AxisX2D.Title>
                <dxc:AxisTitle Content="Year"/>
            </dxc:AxisX2D.Title>
        </dxc:AxisX2D>
    </dxc:XYDiagram2D.AxisX>

    <dxc:XYDiagram2D.AxisY>
        <dxc:AxisY2D>
            <dxc:AxisY2D.Title>
                <dxc:AxisTitle Content="Oil Price (USD)"/>
            </dxc:AxisY2D.Title>
        </dxc:AxisY2D>
    </dxc:XYDiagram2D.AxisY>
</dxc:XYDiagram2D>
```

Secondary axes work identically — set their `Title` property to an `AxisTitle`.

### Style the Title

`AxisTitle` is a `ContentControl`, so the standard WPF appearance properties (`Foreground`, `FontFamily`, `FontSize`, etc.) work via `ContentTemplate` or directly on the element where the API allows.

```xaml
<dxc:AxisTitle Content="Oil Price (USD)" Alignment="Center"/>
```

For richer styling, use `TitleBase.ContentTemplate`:

```xaml
<Window.Resources>
    <DataTemplate x:Key="axisTitleTemplate">
        <TextBlock Text="{Binding}"
                   Foreground="Black"
                   FontFamily="SegoeUI"
                   FontStyle="Italic"
                   FontSize="16"
                   FontWeight="Medium"/>
    </DataTemplate>
</Window.Resources>

<dxc:AxisTitle Content="Oil Price (USD)"
               ContentTemplate="{StaticResource axisTitleTemplate}"
               Alignment="Center"/>
```

### Key Properties

| Property | Use |
|---|---|
| `TitleBase.Content` | The title text or `UIElement` |
| `TitleBase.ContentTemplate` | Full layout / styling template |
| `AxisTitle.Alignment` | `Near`, `Center`, `Far` along the axis |

## Axis Labels

The chart automatically generates labels from series data. You control them through:

1. **`AxisLabel` settings** — assigned to `AxisBase.Label`. Controls text formatting and appearance.
2. **`CustomAxisLabel` entries** — assigned to `Axis2D.CustomLabels`. Define labels at specific positions.
3. **`ResolveOverlappingOptions`** — assigned to `Axis2D.ResolveOverlappingOptions`. Controls overlap behavior.

### Format Label Text via Pattern

Use `AxisLabel.TextPattern` with placeholders to control label content:

```xaml
<dxc:XYDiagram2D.AxisY>
    <dxc:AxisY2D>
        <dxc:AxisY2D.Label>
            <dxc:AxisLabel TextPattern="{}{V} min"/>
        </dxc:AxisY2D.Label>
    </dxc:AxisY2D>
</dxc:XYDiagram2D.AxisY>
```

> **Always start XAML patterns with `{}`** to escape the leading `{` (it'd otherwise be a markup extension).

### Available Placeholders

| Placeholder | Where | Meaning |
|---|---|---|
| `{A}` | Argument (X) axes | Series point argument |
| `{V}` | Value (Y) axes | Series point value |
| `{VP}` | Value (Y) axes | Value as percentage |

Combine with .NET format specifiers — `{V:F2}` (2 decimals), `{A:yyyy-MM-dd}` (date), `{V:p1}` (percent).

### Custom Label Formatter (`IAxisLabelFormatter`)

When `TextPattern` isn't enough — conditional logic, magnitude scaling, custom strings:

```csharp
public class ThousandsFormatter : IAxisLabelFormatter {
    public string GetAxisLabelText(object axisValue) {
        return Convert.ToString((double)axisValue / 1000) + "K";
    }
}
```

```xaml
<dxc:AxisY2D.Label>
    <dxc:AxisLabel>
        <dxc:AxisLabel.Formatter>
            <local:ThousandsFormatter/>
        </dxc:AxisLabel.Formatter>
    </dxc:AxisLabel>
</dxc:AxisY2D.Label>
```

> If `Formatter` is set, `TextPattern` is ignored.
>
> For **qualitative** scale labels, `TextPattern` doesn't work — use `Formatter` instead.

### Label Appearance Properties

Set directly on `AxisLabel`:

```xaml
<dxc:AxisLabel Foreground="DarkSlateBlue"
               Background="LightBlue"
               FontSize="14"
               FontStyle="Italic"
               FontWeight="Bold"
               Angle="45"
               TextPattern="{}{V} year"/>
```

| Property | Effect |
|---|---|
| `Angle` | Rotation in degrees |
| `Background` / `Foreground` | Standard brushes |
| `BorderBrush` / `BorderThickness` | Label border |
| `FontFamily`, `FontSize`, `FontStyle`, `FontWeight`, `FontStretch` | Standard font |
| `Padding` | Internal padding |
| `ElementTemplate` | Full layout template (overrides default rendering) |

### Custom Label Layout via `ElementTemplate`

For complete control over how a label is drawn:

```xaml
<Window.Resources>
    <DataTemplate x:Key="AxisXLabelTemplate">
        <Border BorderThickness="1" CornerRadius="9">
            <Border.Background>
                <SolidColorBrush Color="Orange"/>
            </Border.Background>
            <Label Content="{Binding Path=Content}"
                   Padding="5,1,5,1.5"
                   Foreground="DarkSlateBlue" FontSize="12"/>
        </Border>
    </DataTemplate>
</Window.Resources>

<dxc:AxisX2D.Label>
    <dxc:AxisLabel ElementTemplate="{StaticResource AxisXLabelTemplate}"/>
</dxc:AxisX2D.Label>
```

`{Binding Path=Content}` accesses the rendered text. `Angle` still applies (it rotates the template too).

## Resolve Label Overlap

When labels don't fit (long category names, dense date ticks), the chart automatically hides some. You can configure the resolution strategy:

```xaml
<dxc:AxisX2D.Label>
    <dxc:AxisLabel TextPattern="{}{A:dd/MM HH:mm}">
        <dxc:Axis2D.ResolveOverlappingOptions>
            <dxc:AxisLabelResolveOverlappingOptions AllowHide="True"
                                                    AllowRotate="True"
                                                    AllowStagger="True"
                                                    MinIndent="5"/>
        </dxc:Axis2D.ResolveOverlappingOptions>
    </dxc:AxisLabel>
</dxc:AxisX2D.Label>
```

| Option | Effect |
|---|---|
| `AllowHide` | Skip labels that don't fit |
| `AllowRotate` | Rotate labels (horizontal axes only) |
| `AllowStagger` | Alternate label rows (horizontal axes only) |
| `MinIndent` | Minimum pixels between adjacent labels |

`AllowRotate` and `AllowStagger` only affect labels on a horizontal axis (the argument x-axis, or the value y-axis when `XYDiagram2D.Rotated="True"`).

## Custom Labels at Specific Positions

To show extra labels at specific values (e.g., 2h / 6h / 10h on a duration axis):

```xaml
<dxc:AxisY2D LabelVisibilityMode="AutoGeneratedAndCustom">
    <dxc:AxisY2D.CustomLabels>
        <dxc:CustomAxisLabel Value="120" Content="2h"/>
        <dxc:CustomAxisLabel Value="360" Content="6h"/>
        <dxc:CustomAxisLabel Value="600" Content="10h"/>
        <dxc:CustomAxisLabel Value="840" Content="14h"/>
    </dxc:AxisY2D.CustomLabels>
</dxc:AxisY2D>
```

| Property | Use |
|---|---|
| `Axis2D.CustomLabels` | Collection of `CustomAxisLabel` entries |
| `CustomAxisLabel.Value` | Position on the axis |
| `CustomAxisLabel.Content` | Label content |
| `Axis2D.LabelVisibilityMode` | `Default` (custom replaces auto) or `AutoGeneratedAndCustom` (both shown) |

### MVVM — Custom Labels from ViewModel

```xaml
<dxc:AxisY2D CustomLabelItemsSource="{Binding CustomLabels}">
    <dxc:AxisY2D.CustomLabelItemTemplate>
        <DataTemplate>
            <dxc:CustomAxisLabel Value="{Binding Item1}" Content="{Binding Item2}"/>
        </DataTemplate>
    </dxc:AxisY2D.CustomLabelItemTemplate>
</dxc:AxisY2D>
```

```csharp
public class MainViewModel {
    public IEnumerable<Tuple<double, string>> CustomLabels { get; } = new[] {
        Tuple.Create(120.0, "2h"),
        Tuple.Create(360.0, "6h"),
        Tuple.Create(600.0, "10h"),
        Tuple.Create(840.0, "14h"),
    };
}
```

## Common Issues

- **Pattern shows literal `{V}` instead of value** — missing leading `{}` escape: write `TextPattern="{}{V} min"`, not `TextPattern="{V} min"`.
- **Pattern ignored on qualitative axis** — `TextPattern` doesn't apply to qualitative scale labels. Use `AxisLabel.Formatter` with an `IAxisLabelFormatter` implementation.
- **Custom labels replace auto labels** — `LabelVisibilityMode` defaults to `Default` (custom-only when custom is present). Switch to `AutoGeneratedAndCustom` to keep both.
- **`AllowRotate` / `AllowStagger` do nothing** — these only apply to horizontal axes. For a vertical y-axis without `Rotated="True"`, only `AllowHide` and `MinIndent` apply.
- **Title doesn't appear** — `Title` property not set on the axis, or `AxisTitle.Content` empty. Set both.
- **Pattern shows excess decimals** — add a format specifier: `TextPattern="{}{V:F0}"` (no decimals), `{V:F2}` (two), `{V:N0}` (with thousand separators).

## Source Material

- `articles/controls-and-libraries/charts-suite/chart-control/axes/axis-titles.md` (https://docs.devexpress.com/content/WPF/7096?md=true)
- `articles/controls-and-libraries/charts-suite/chart-control/axes/axis-labels.md` (https://docs.devexpress.com/content/WPF/6336?md=true)

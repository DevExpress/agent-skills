# Selection

Selection lets end users click (or tap on touch devices) to highlight chart elements — points, arguments, or whole series. The bound `SelectedItem` / `SelectedItems` properties make selection MVVM-friendly: bind them to a ViewModel and react to user picks without code-behind.

## When to Use This Reference

Use this when you need to:

- Enable end-user selection in the chart (`SelectionMode`)
- Choose between Single, Multiple, or Extended selection
- Choose what gets selected on click (Point, Argument, or Series)
- Bind selected elements to a ViewModel
- Customize the selection rectangle appearance
- Visually highlight selected points (template override)

## Enable Selection

Selection is **off** by default (`SelectionMode="None"`). Turn it on:

```xaml
<dxc:ChartControl SelectionMode="Single">
    ...
</dxc:ChartControl>
```

## Selection Modes

`ElementSelectionMode` (assigned to `ChartControl.SelectionMode`):

| Mode | Behavior |
|---|---|
| `None` (default) | No selection. Clicks don't select anything. |
| `Single` | Click selects one element. Clicking another deselects the previous. |
| `Multiple` | Click adds to / removes from the selection. Drag a **selection rectangle** while holding `Ctrl` + left mouse button to select multiple elements at once. |
| `Extended` | Single + Multiple combined. Plain click acts as `Single`; `Ctrl`+click acts as `Multiple`. |

```xaml
<dxc:ChartControl SelectionMode="Extended"/>
```

## Series Selection Mode — What Gets Selected on a Click

`SeriesSelectionMode` (assigned to `ChartControl.SeriesSelectionMode`) determines the *target* of each click:

| Mode | What's selected |
|---|---|
| `Point` (default) | A single series point (the one clicked) |
| `Argument` | All points across all series that share the clicked point's argument (vertical "slice") |
| `Series` | The entire series the clicked point belongs to |

```xaml
<dxc:ChartControl SelectionMode="Single"
                  SeriesSelectionMode="Series"/>
```

Combining modes:

- `SelectionMode="Single"` + `SeriesSelectionMode="Series"` → click any point on a series to select that whole series.
- `SelectionMode="Multiple"` + `SeriesSelectionMode="Argument"` → drag to select all points for several arguments (vertical slices).

## Access Selected Elements

```xaml
<dxc:ChartControl SelectionMode="Single"
                  SelectedItem="{Binding SelectedCountry, Mode=TwoWay}">
    ...
</dxc:ChartControl>
```

| Property | Use |
|---|---|
| `ChartControl.SelectedItem` | The single selected element (`Single` / `Extended` modes; `null` if nothing selected) |
| `ChartControl.SelectedItems` | Collection of selected elements (`Multiple` / `Extended` modes) |

The bound element type depends on `SeriesSelectionMode`:

| `SeriesSelectionMode` | Bound element type |
|---|---|
| `Point` | The underlying data object (e.g., your `SalesPoint` instance) |
| `Argument` | The argument value (e.g., `string`, `DateTime`) |
| `Series` | The `Series` instance |

```csharp
public class MainViewModel : ViewModelBase {
    public SalesPoint? SelectedCountry {
        get => GetValue<SalesPoint?>();
        set => SetValue(value);
    }
}
```

## Selection Rectangle (Multiple Mode)

In `Multiple` (and `Extended`) modes, `Ctrl` + drag draws a selection rectangle. Customize its appearance:

```xaml
<dxc:XYDiagram2D EnableAxisXNavigation="True"
                 EnableAxisYNavigation="True">
    <dxc:XYDiagram2D.SelectionTemplate>
        <DataTemplate>
            <Border BorderThickness="1" BorderBrush="Gray">
                <Rectangle Opacity="0.3" Fill="Gray"/>
            </Border>
        </DataTemplate>
    </dxc:XYDiagram2D.SelectionTemplate>
</dxc:XYDiagram2D>
```

`XYDiagram2D.SelectionTemplate` accepts a `DataTemplate` that wraps the visual rectangle. The template applies during drag.

## Customize Selected Point Appearance

To change the look of a selected point, swap in a **custom series model** that exposes the `IsSelected` flag to a converter:

```csharp
public class IsSelectedToBrushConverter : IValueConverter {
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
        if (value is bool isSelected && targetType == typeof(Brush))
            return isSelected ? Brushes.Black : Brushes.Red;
        return null!;
    }
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        => throw new NotImplementedException();
}
```

```xaml
<Window.Resources>
    <local:IsSelectedToBrushConverter x:Key="isSelectedConverter"/>
</Window.Resources>

<dxc:ChartControl SelectionMode="Multiple" CrosshairEnabled="False">
    <dxc:XYDiagram2D>
        <dxc:BarSideBySideSeries2D>
            <dxc:BarSideBySideSeries2D.Model>
                <dxc:CustomBar2DModel>
                    <dxc:CustomBar2DModel.PointTemplate>
                        <ControlTemplate>
                            <Border Background="{Binding IsSelected, Converter={StaticResource isSelectedConverter}}"/>
                        </ControlTemplate>
                    </dxc:CustomBar2DModel.PointTemplate>
                </dxc:CustomBar2DModel>
            </dxc:BarSideBySideSeries2D.Model>
        </dxc:BarSideBySideSeries2D>
    </dxc:XYDiagram2D>
</dxc:ChartControl>
```

DevExpress ships a `Custom*2DModel` class for each templatable point shape: `CustomBar2DModel`, `CustomMarker2DModel` (used by point/line/area series via their marker), `CustomPie2DModel`, `CustomCandleStick2DModel`, `CustomStock2DModel`, `CustomRangeBar2DModel`. The model's `PointTemplate` controls how each point is rendered; the template's `DataContext` exposes `IsSelected`.

## MVVM Pattern — React to Selection

Bind `SelectedItem` and respond in the setter:

```csharp
public class MainViewModel : ViewModelBase {
    public SalesPoint? SelectedSale {
        get => GetValue<SalesPoint?>();
        set {
            SetValue(value);
            OnSaleSelected(value);
        }
    }

    void OnSaleSelected(SalesPoint? sale) {
        if (sale == null) return;
        // Drill into sale, open a details panel, etc.
    }
}
```

For multi-selection, bind `SelectedItems`:

```xaml
<dxc:ChartControl SelectionMode="Multiple"
                  SelectedItems="{Binding SelectedSales, Mode=TwoWay}"/>
```

`SelectedItems` is a `ChartElementsCollection`. For simpler MVVM, expose a derived collection and subscribe to its `CollectionChanged`.

## Selection Combined with Crosshair / Tooltip

Selection is independent of the crosshair and tooltip — all three can be active. Common pattern:

- **Crosshair** for tracking values during exploration
- **Tooltip** for inspecting one point
- **Selection** for picking points to act on (drill-down, comparison, deletion)

Disable the crosshair on charts with rich selection UI to avoid visual noise:

```xaml
<dxc:ChartControl SelectionMode="Multiple" CrosshairEnabled="False"/>
```

## Picking a Selection Setup

| Goal | `SelectionMode` | `SeriesSelectionMode` |
|---|---|---|
| Click a country to drill into details | `Single` | `Point` |
| Pick a year column across all series for comparison | `Single` or `Multiple` | `Argument` |
| Toggle a whole series's visibility / focus by clicking it | `Single` | `Series` |
| Lasso-select bars for batch operations | `Multiple` | `Point` |
| Power-user UX with both click and Ctrl-click | `Extended` | `Point` |

## Common Issues

- **Clicks do nothing** — `SelectionMode` is `None` (default). Set to `Single`, `Multiple`, or `Extended`.
- **`SelectedItem` always `null`** — `SeriesSelectionMode="Series"` returns a `Series`, not your data object. Match the binding type to the selection mode.
- **`SelectedItems` doesn't update** — used the imperative collection but bound to `SelectedItem` (singular). Either bind both, or use `SelectedItems` for multi-selection modes.
- **Selection rectangle not appearing** — needed `XYDiagram2D.EnableAxisXNavigation` / `EnableAxisYNavigation` to host the gesture. Set to `True` (they enable zoom/scroll too).
- **Visual highlight doesn't change on selection** — default theme applies a subtle highlight; for strong feedback, use a custom series model with an `IsSelected`→brush converter (see above).
- **Argument selection picks one point instead of a column** — `SeriesSelectionMode` is `Point`. Switch to `Argument`.

## Source Material

- `articles/controls-and-libraries/charts-suite/chart-control/selection.md` (https://docs.devexpress.com/content/WPF/117345?md=true)

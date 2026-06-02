# MVVM Integration — DevExpress WPF Pivot Grid

The Pivot Grid supports the **Model-View-ViewModel** pattern through:

1. **`FieldsSource`** — bind a collection of view-model objects, render each as a `PivotGridField` via a `DataTemplate`.
2. **`GroupsSource`** — same idea for field groups.
3. **`XamlHelper.Name`** — stable identifier on a generated field (lets layout serialization match templated fields by name).
4. **DataAnnotations + `[Command]` attributes** — for declarative ViewModel commands wired to pivot events.

This decouples field definitions from XAML, enabling dynamic schemas, runtime field changes, and unit-testable pivot configuration.

## When to Use This Reference

Use this when you need to:

- Define pivot fields in a ViewModel collection rather than XAML
- Generate fields dynamically based on user choice / data schema / report metadata
- Support multiple field "looks" via a `DataTemplateSelector`
- Persist layout from / restore layout to a ViewModel
- Keep code-behind empty (pure MVVM)

## ViewModel-Driven Fields (`FieldsSource`)

### 1. Field Descriptor Class

```csharp
public class PivotFieldDescriptor {
    public string FieldName { get; set; } = "";
    public string UniqueName { get; set; } = "";
    public string FieldCaption { get; set; } = "";
    public FieldArea FieldArea { get; set; }
    public int AreaIndex { get; set; }
    public FieldGroupInterval Interval { get; set; }
    public string GroupName { get; set; } = "";
    public int GroupIndex { get; set; }
}
```

### 2. ViewModel Exposes the Collection

```csharp
using System.Collections.ObjectModel;
using DevExpress.Mvvm;
using DevExpress.Xpf.PivotGrid;

public class MainViewModel : ViewModelBase {
    public ObservableCollection<PivotFieldDescriptor> Fields { get; }
    public DataTable Sales { get; }

    public MainViewModel() {
        Sales = SalesData.Build();
        Fields = new ObservableCollection<PivotFieldDescriptor> {
            new() { FieldName = "Country",       UniqueName = "fieldCountry",
                    FieldArea = FieldArea.RowArea,    AreaIndex = 0 },
            new() { FieldName = "OrderDate",     UniqueName = "fieldYear",
                    FieldArea = FieldArea.ColumnArea, AreaIndex = 0,
                    Interval = FieldGroupInterval.DateYear, FieldCaption = "Year" },
            new() { FieldName = "ExtendedPrice", UniqueName = "fieldPrice",
                    FieldArea = FieldArea.DataArea,   AreaIndex = 0,
                    FieldCaption = "Sales" },
        };
    }
}
```

> Use `ObservableCollection<T>` (or any `INotifyCollectionChanged` source) if you want runtime field additions to propagate to the pivot.

### 3. Field Template (Bind Each Property)

Use the `dxci:DependencyObjectExtensions.DataContext` attached path — it avoids repeated `DataContext` lookups and keeps performance acceptable for large field counts:

```xaml
<DataTemplate x:Key="DefaultFieldTemplate">
    <ContentControl>
        <dxpg:PivotGridField
            FieldName="{Binding Path=(dxci:DependencyObjectExtensions.DataContext).FieldName, RelativeSource={RelativeSource Self}}"
            Area="{Binding Path=(dxci:DependencyObjectExtensions.DataContext).FieldArea, RelativeSource={RelativeSource Self}}"
            AreaIndex="{Binding Path=(dxci:DependencyObjectExtensions.DataContext).AreaIndex, RelativeSource={RelativeSource Self}}"
            Caption="{Binding Path=(dxci:DependencyObjectExtensions.DataContext).FieldCaption, RelativeSource={RelativeSource Self}}"
            dx:XamlHelper.Name="{Binding Path=(dxci:DependencyObjectExtensions.DataContext).UniqueName, RelativeSource={RelativeSource Self}}"/>
    </ContentControl>
</DataTemplate>
```

`dx:XamlHelper.Name` sets the **identifier used by layout serialization**. Without it, layouts saved from a templated field can't be restored back to the same field.

### 4. Wire Up the Pivot

```xaml
<Window xmlns:dxpg="http://schemas.devexpress.com/winfx/2008/xaml/pivotgrid"
        xmlns:dx="http://schemas.devexpress.com/winfx/2008/xaml/core"
        xmlns:dxci="http://schemas.devexpress.com/winfx/2008/xaml/core/internal">
    <Window.DataContext>
        <local:MainViewModel/>
    </Window.DataContext>
    <Window.Resources>
        <DataTemplate x:Key="DefaultFieldTemplate">
            <!-- as above -->
        </DataTemplate>
    </Window.Resources>

    <dxpg:PivotGridControl DataSource="{Binding Sales}"
                           FieldsSource="{Binding Fields}"
                           FieldGeneratorTemplate="{StaticResource DefaultFieldTemplate}"/>
</Window>
```

If a single template fits all fields, use `FieldGeneratorTemplate`. For conditional templates, use a selector (next section).

## Conditional Templates with `DataTemplateSelector`

For different field shapes (e.g., date-grouped fields with a calendar icon):

```csharp
public class PivotFieldTemplateSelector : DataTemplateSelector {
    public override DataTemplate? SelectTemplate(object item, DependencyObject container) {
        var field = (PivotFieldDescriptor)item;
        var control = (Control)container;
        return field.Interval == FieldGroupInterval.DateYear
            ? (DataTemplate)control.FindResource("IntervalFieldTemplate")
            : (DataTemplate)control.FindResource("DefaultFieldTemplate");
    }
}
```

```xaml
<dxpg:PivotGridControl DataSource="{Binding Sales}"
                       FieldsSource="{Binding Fields}"
                       FieldGeneratorTemplateSelector="{StaticResource FieldSelector}"/>
```

## Style for All Generated Fields

To set properties shared across all template variants, use `FieldGeneratorStyle`:

```xaml
<dxpg:PivotGridControl FieldGeneratorStyle="{StaticResource PivotFieldBaseStyle}"
                       FieldsSource="{Binding Fields}"/>
```

## Field Groups in ViewModel (`GroupsSource`)

For dynamic groups, supply a parallel collection — see `articles/controls-and-libraries/pivot-grid/mvvm-enhancements/binding-to-a-collection-of-groups.md`.

```csharp
public ObservableCollection<PivotGroupDescriptor> Groups { get; }
```

```xaml
<dxpg:PivotGridControl FieldsSource="{Binding Fields}"
                       GroupsSource="{Binding Groups}"/>
```

## Bindable State Properties

Beyond `DataSource` (TwoWay) and `FieldsSource`, the Pivot Grid exposes bindable members for end-user state:

- **`FocusedCell`** (`System.Drawing.Point`, TwoWay) — coordinates of the focused cell. `Point.X` is the column index, `Point.Y` is the row index. Use `GetFocusedCellInfo()` to obtain a `CellInfo` for the focused cell, or handle `FocusedCellChanged` to react to changes.
- **Cell selection** — there is no bindable `SelectedCells` property. Use:
  - `SelectedCellInfo` (`CellInfo`, OneWay-bindable) for the currently selected data cell — suitable for direct MVVM binding.
  - `Selection` (`System.Drawing.Rectangle`, TwoWay) when `SelectMode = SolidSelection` — `Left`/`Top` are the top-left cell's column/row indices, `Width`/`Height` are the spans.
  - `MultiSelection` (`IMultipleSelection`, read-only) when `SelectMode = MultiSelection` (default) — provides the coordinates of all selected blocks.
  - `CellSelectionChanged` event to react to selection changes from a ViewModel.
- **Drill-down arguments** via the `DrillDownDataSource` / `DrillDownCommand` pattern

For drill-down from a ViewModel:

```csharp
[Command]
public void OnCellClick(object args) {
    // Cast args to the appropriate event arg type; use pivot.GetDrillDownDataSource(cellInfo)
    // to retrieve raw rows behind the clicked cell.
}
```

> Verify exact event-args type and bindable property names against your version via DxDocs MCP. The pattern is consistent with `GridControl` MVVM enhancements (`xref:115335`).

## Persist Layout from the ViewModel

See [save-restore-layout.md](save-restore-layout.md) § MVVM Persistence — store the layout as `byte[]` on the ViewModel and expose Save/Restore commands.

## Common Issues

- **Layout doesn't restore properly with FieldsSource** — fields generated from a template lack a `Name`. Set `dx:XamlHelper.Name` on each templated `PivotGridField`.
- **Adding to `Fields` collection doesn't update the pivot** — collection must implement `INotifyCollectionChanged`. Use `ObservableCollection<T>`.
- **Templates work but performance is poor with many fields** — direct `DataContext` bindings re-resolve repeatedly. Switch to `dxci:DependencyObjectExtensions.DataContext` (shown above).
- **Conditional template never picks the right branch** — `SelectTemplate` is called once per item but result is cached; if your `DataContext` mutates, replace the item in the collection rather than mutating in place.

## Apply to GridControl

For shared MVVM concepts (ViewModelBase, `[Command]`, DelegateCommand, DXSerializer for layout, services from DevExpress.Mvvm), see [the data-grid skill's MVVM patterns](../../wpf-devexpress-data-grid/SKILL.md).

The Pivot Grid extends those patterns with `FieldsSource` / `GroupsSource` and the template/selector mechanism documented above.

## Source Material

- `articles/controls-and-libraries/pivot-grid/mvvm-enhancements/binding-to-a-collection-of-fields.md` (`xref:115438`)
- `articles/controls-and-libraries/pivot-grid/mvvm-enhancements/binding-to-a-collection-of-groups.md`
- `articles/controls-and-libraries/pivot-grid/examples/binding-to-data/how-to-bind-the-pivot-grid-to-fields-and-groups-specified-in-viewmodel.md` (`xref:118813`)

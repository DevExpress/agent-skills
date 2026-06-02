# End-User Features — DevExpress WPF Pivot Grid

End users interact with the Pivot Grid through three main mechanisms: **dragging fields** between areas (Customization Form / runtime drag-drop), **drill-down** on data cells (see the source rows behind a summary), and **filtering** via the Excel-style filter dropdown.

## When to Use This Reference

Use this when you need to:

- Allow / restrict drag-and-drop of fields between areas at runtime
- Enable drill-down to inspect the rows behind a data cell
- Configure the Excel-style filter dropdown
- Show / hide the Field List and Filter Header Area
- Customize the navigation buttons and Result Info Panel

## Drag-and-Drop of Fields

By default, users can drag a field header from one area to another. To disable runtime customization at the control level:

```xaml
<dxpg:PivotGridControl AllowDrag="False"/>
```

Restrict on specific fields:

```xaml
<dxpg:PivotGridField Caption="Country" Area="RowArea" AllowDrag="False"/>
```

Both `PivotGridControl.AllowDrag` and `PivotGridField.AllowDrag` are real properties (type `bool`).

Source: `articles/controls-and-libraries/pivot-grid/end-user-interaction.md` and `pivot-grid/end-user-capabilities.md`.

## Drill-Down (Inspect Source Records)

When a user double-clicks a Data cell, the Pivot Grid can show the underlying rows that contributed to that aggregated value. Handle `PivotGridControl.CellDoubleClick` (event args: `PivotCellEventArgs`) and call `CreateDrillDownDataSource`:

```csharp
private void pivotGridControl1_CellDoubleClick(object sender, PivotCellEventArgs e) {
    // Get the records that contributed to this cell
    var details = pivotGridControl1.CreateDrillDownDataSource(e.ColumnIndex, e.RowIndex);
    // Show `details` in a popup or secondary grid
}
```

Source: `articles/controls-and-libraries/pivot-grid.md` § "drill-down" mentions; `pivot-grid/end-user-interaction.md`.

## Excel-Style Filter Dropdown

Each field's header has a filter glyph. Clicking it opens a checkbox list of unique values. This is on by default in modern themes:

```xaml
<dxpg:PivotGridControl AllowFilter="True"
                       AllowDragOnRunTime="True"/>
```

The Excel-style demo is referenced from the root pivot-grid article:

> `[!demo[](dxdemo://Wpf/DXPivotGrid/MainDemo/ExcelStyleFiltering)]`

## Field List

A separate panel showing all defined fields; users drag fields from the list into the pivot.

```csharp
pivotGridControl1.ShowFieldList();    // method — opens the Field List
pivotGridControl1.HideFieldList();
```

For XAML / data binding, use `IsFieldListVisible` (this is the bindable property; `ShowFieldList` is a method, not a property):

```xaml
<dxpg:PivotGridControl IsFieldListVisible="True"/>
```

Use the Field List when the data source has many fields and users only want to bring in a subset.

## Customization Form

The Field List (a.k.a. "Customization Form") is enabled by default through `PivotGridControl.AllowCustomizationForm`:

```xaml
<dxpg:PivotGridControl AllowCustomizationForm="True"/>
```

See [layout-and-fields.md](layout-and-fields.md) § Field List (Customization Form) for the full set of properties.

## Resize Columns

End users can drag column borders to resize. Programmatic best-fit on the control (no `BestFit()` method on `PivotGridField`):

```csharp
pivotGridControl1.BestFit();
```

## End-User Interaction Settings (Summary Table)

| Property | Type | Default | Effect |
|---|---|---|---|
| `PivotGridControl.AllowDrag` | `bool` | `True` | Users can drag fields between areas |
| `PivotGridControl.AllowFilter` | `bool` | `True` | Filter dropdowns are available |
| `PivotGridControl.IsFieldListVisible` | `bool` | `False` | Field List panel visibility (bindable) |
| `PivotGridControl.AllowCustomizationForm` | `bool` | `True` | End user can invoke the Field List |
| `PivotGridControl.AllowHideFields` | `AllowHideFieldsType` | `WhenFieldListVisible` | Can fields be dropped into the Field List to hide them |

## Source Material

- `articles/controls-and-libraries/pivot-grid.md` (root, § End-User Interaction)
- `articles/controls-and-libraries/pivot-grid/end-user-capabilities.md`
- `articles/controls-and-libraries/pivot-grid/end-user-interaction.md`
- `articles/controls-and-libraries/pivot-grid/ui-elements.md`

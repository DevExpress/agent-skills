# Layout and Fields — DevExpress WPF Pivot Grid

The Pivot Grid layout is built around four **header areas** — Row, Column, Data, Filter — plus optional **field groups** for organizing related fields. Layout state can be saved and restored across sessions.

## When to Use This Reference

Use this when you need to:

- Understand the four areas (Row, Column, Data, Filter) and what each does
- Organize multiple fields into Field Groups
- Show the Customization Form (lets end users drag fields between areas)
- Use Best Fit to auto-size column widths
- Save / restore pivot layout

## The Four Header Areas

| `FieldArea` | Position | What appears | Use for |
|---|---|---|---|
| `RowArea` | Left column headers | Field values, vertically stacked | Categories you read down: countries, products, employees |
| `ColumnArea` | Top row headers | Field values, horizontally arranged | Time periods, scenarios, categorical breakdowns you read across |
| `DataArea` | Cells (intersection) | Aggregated summaries of the bound column | Metrics: Sum, Count, Average, Min/Max |
| `FilterArea` | Top filter selectors | Field values shown as filter dropdowns | Fields you want to slice the whole pivot by without showing on rows/columns |

Source: `articles/controls-and-libraries/pivot-grid/fundamentals/fields.md` and `articles/controls-and-libraries/pivot-grid/layout.md`.

### Multiple Fields in One Area

Stacking multiple fields in Row or Column creates a hierarchy. For example:

```csharp
AddField("Country",  FieldArea.RowArea, "Country",  0);
AddField("Category", FieldArea.RowArea, "Category", 1);
// → Row headers: USA > Beverages, USA > Produce, Canada > Beverages, ...
```

`AreaIndex` controls the position within the area (0 is outermost).

### Multiple Data Fields

Place several fields in `DataArea` to compute multiple metrics side-by-side:

```csharp
AddField("Sales",     FieldArea.DataArea, "Amount").SummaryType = FieldSummaryType.Sum;
AddField("Orders",    FieldArea.DataArea, "OrderId").SummaryType = FieldSummaryType.Count;
AddField("Avg Order", FieldArea.DataArea, "Amount").SummaryType = FieldSummaryType.Average;
```

Each Row × Column cell shows three values: Sales, Orders, Avg Order.

## Field Groups

Combine related fields into a single **group** so the user moves them as a unit. End users cannot separate grouped fields by drag-drop or hide one in the field list — the group travels together. Each group also has a single Expand/Collapse button that hides or shows all child fields' columns/rows.

API: `PivotGridControl.Groups` (collection of `PivotGridGroup`), plus per-field properties `Group`, `GroupName`, `GroupIndex`, `ExpandedInFieldsGroup`.

### Two Ways to Assign Fields to a Group

**By `Group` reference (XAML, with `ElementName` binding):**

```xaml
<dxpg:PivotGridControl Name="pivot">
    <dxpg:PivotGridControl.Fields>
        <dxpg:PivotGridField Name="fieldCountry"  FieldName="Country"     Area="RowArea"
                             Group="{Binding ElementName=groupCountryCustomer}"/>
        <dxpg:PivotGridField Name="fieldCustomer" FieldName="SalesPerson" Area="RowArea"
                             Group="{Binding ElementName=groupCountryCustomer}"
                             Caption="Customer"/>
    </dxpg:PivotGridControl.Fields>
    <dxpg:PivotGridControl.Groups>
        <dxpg:PivotGridGroup Name="groupCountryCustomer"/>
    </dxpg:PivotGridControl.Groups>
</dxpg:PivotGridControl>
```

**By `GroupName` (string identifier):**

```csharp
field1.GroupName = "Geography";
field2.GroupName = "Geography";
// Both fields now belong to the same group.
```

### Create / Modify Groups in Code

```csharp
// Add() takes the fields that should join the new group:
PivotGridGroup group = pivotGridControl1.Groups.Add(fieldCategoryName, fieldProductName);
```

### Group Behavior

- **Area / AreaIndex of the group** is the `Area`/`AreaIndex` of the **first field** in the group. Setting it on subsequent fields has no effect — they follow the first.
- **`PivotGridField.GroupIndex`** controls a field's position *within* the group.
- **`PivotGridField.ExpandedInFieldsGroup`** — `true` = child columns/rows visible; `false` = collapsed (only the parent shown).

Source: `articles/controls-and-libraries/pivot-grid/layout/field-groups.md` (`xref:8020`) and `examples/how-to-group-fields2152.md`.

## Field List (Customization Form)

A built-in window that lets end users **add / remove / rearrange fields** at runtime. End users invoke it from the header-area context menu; you can show/hide it programmatically.

```xaml
<dxpg:PivotGridControl AllowCustomizationForm="True"
                       AllowHideFields="Always"
                       FieldListIncludeVisibleFields="True"/>
```

### Show / Hide in Code

```csharp
pivotGridControl1.ShowFieldList();      // Open the Field List
pivotGridControl1.HideFieldList();      // Close the Field List
```

Lifecycle events: `ShownFieldList`, `HiddenFieldList`.

### Key Properties

| Property | Purpose |
|---|---|
| `PivotGridControl.AllowCustomizationForm` | Master switch: can the end user invoke the Field List at all. |
| `PivotGridControl.AllowHideFields` | Type `AllowHideFieldsType` enum (`Always` / `Never` / `WhenFieldListVisible`). Can fields be dropped *into* the Field List (i.e., hidden from the pivot). |
| `PivotGridControl.AllowDragInCustomizationForm` | Can fields be dragged out of or within the Field List. |
| `PivotGridControl.FieldListIncludeVisibleFields` | When `true`, visible fields also appear in the list (with checkboxes for show/hide). |
| `PivotGridField.ShowInCustomizationForm` | Per-field opt-out: prevent a hidden field from appearing in the list at all. |
| `PivotGridField.AllowDragInCustomizationForm` | Per-field drag override. |

For an embedded variant (inside your window instead of a popup), see `xref:11753` (Standalone Customization Control). For arbitrary grouping of fields shown in the list, see `xref:11754` (User Folders).

Source: `articles/controls-and-libraries/pivot-grid/layout/customization-form/customization-form-overview.md` (`xref:11751`) and `customization-form.md` (`xref:8018`).

## Best Fit

Auto-size column widths to fit content:

```csharp
pivotGridControl1.BestFit();           // all columns
pivotGridControl1.BestFitColumn(...);  // a specific column
pivotGridControl1.BestFitRow(...);     // a specific row
```

`BestFit()` is a method on `PivotGridControl` only — `PivotGridField` does **not** have a `BestFit()` method. To influence how a field participates in best-fit, set `PivotGridField.BestFitArea`, `BestFitMode`, and `BestFitMaxRowCount`.

Source: `articles/controls-and-libraries/pivot-grid/layout.md` § Best Fit (`xref:8331`).

## Save and Restore Layout

`PivotGridControl.SaveLayoutToXml/Stream` and `RestoreLayoutFromXml/Stream/StreamAsync` persist the full pivot state — field placement, sort, filter, grouping, summaries, format conditions, and column sizes.

**For the full reference (separate collapsed-state API, `PivotSerializationOptions`, `AddNewFields` / `RemoveOldFields` matching, `LayoutUpgrade` event, MVVM persistence, restore ordering rules) see [save-restore-layout.md](save-restore-layout.md).**

## Layout via XAML (Declarative)

For simple cases, fields can be declared in XAML:

```xaml
<dxpg:PivotGridControl DataSource="{Binding Sales}">
    <dxpg:PivotGridControl.Fields>
        <dxpg:PivotGridField Caption="Country" Area="RowArea"    AreaIndex="0">
            <dxpg:PivotGridField.DataBinding>
                <dxpg:DataSourceColumnBinding ColumnName="Country"/>
            </dxpg:PivotGridField.DataBinding>
        </dxpg:PivotGridField>
        <dxpg:PivotGridField Caption="Year"    Area="ColumnArea" AreaIndex="0">
            <dxpg:PivotGridField.DataBinding>
                <dxpg:DataSourceColumnBinding ColumnName="OrderDate" GroupInterval="DateYear"/>
            </dxpg:PivotGridField.DataBinding>
        </dxpg:PivotGridField>
        <dxpg:PivotGridField Caption="Sales"   Area="DataArea"   AreaIndex="0">
            <dxpg:PivotGridField.DataBinding>
                <dxpg:DataSourceColumnBinding ColumnName="Amount"/>
            </dxpg:PivotGridField.DataBinding>
        </dxpg:PivotGridField>
    </dxpg:PivotGridControl.Fields>
</dxpg:PivotGridControl>
```

This XAML produces the same pivot as the code-behind quickstart. Code-behind is more common for the Pivot Grid because field configuration is often dynamic (responding to user actions, varying schemas, calculated fields), but XAML works for static reports.

## Field Headers and Captions

- `PivotGridField.Caption` — text shown in the header area
- `PivotGridField.Name` — programmatic identifier. To look up a field by `Name`, use `pivotGridControl1.Fields.GetFieldByName("name")`. The `Fields[string]` indexer searches by **data-source column name** (`FieldName`), not by `Name`.
- `PivotGridField.HeaderTemplate` — custom DataTemplate for the header

For multi-line headers or icons, use a `HeaderTemplate`.

## MVVM Integration

The Pivot Grid supports MVVM patterns. See `articles/controls-and-libraries/pivot-grid/mvvm-enhancements/` and `xref:115335`. Bindable members include `DataSource` (TwoWay supported), focused row, drill-down arguments, etc.

## Source Material

- `articles/controls-and-libraries/pivot-grid.md` (root § Layout)
- `articles/controls-and-libraries/pivot-grid/layout.md`
- `articles/controls-and-libraries/pivot-grid/fundamentals/fields.md`
- `articles/controls-and-libraries/pivot-grid/mvvm-enhancements/`

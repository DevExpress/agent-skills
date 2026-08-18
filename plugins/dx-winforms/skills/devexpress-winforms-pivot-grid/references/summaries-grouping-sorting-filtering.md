# Summaries, Grouping, Sorting, and Filtering — WinForms PivotGridControl (DevExpress v26.1)

## Summaries

### Automatic SummaryType

Set on data fields (Area = `DataArea`):

```csharp
fieldSales.SummaryType = DevExpress.Data.PivotGrid.PivotSummaryType.Sum;
```

| `PivotSummaryType` value | Calculation |
|---|---|
| `Sum` | Total of all values (default for numeric) |
| `Count` | Count of records |
| `Average` | Arithmetic mean |
| `Min` | Minimum value |
| `Max` | Maximum value |
| `StdDev` | Sample standard deviation |
| `StdDevp` | Population standard deviation |
| `Var` | Sample variance |
| `Varp` | Population variance |
| `Custom` | Handled via the `CustomSummary` event |

### Summary Display Types

Shows correlations rather than raw totals:

```csharp
fieldSales.SummaryDisplayType =
    DevExpress.Data.PivotGrid.PivotSummaryDisplayType.PercentOfColumnGrandTotal;
```

| `PivotSummaryDisplayType` value | Shows |
|---|---|
| `Default` | Raw summary value |
| `PercentOfColumn` | % of the column subtotal |
| `PercentOfColumnGrandTotal` | % of the column grand total |
| `PercentOfRow` | % of the row subtotal |
| `PercentOfRowGrandTotal` | % of the row grand total |
| `PercentOfGrandTotal` | % of the overall grand total |
| `AbsoluteVariation` | Absolute diff from the previous cell |
| `PercentVariation` | % diff from the previous cell |
| `RankInColumnSmallestToLargest` | Rank within column (1 = smallest) |
| `RankInRowSmallestToLargest` | Rank within row |

### Custom Summary

Handle the `CustomSummary` event when `SummaryType = PivotSummaryType.Custom`:

```csharp
fieldSales.SummaryType = PivotSummaryType.Custom;
pivotGridControl1.CustomSummary += PivotGrid_CustomSummary;

private void PivotGrid_CustomSummary(object sender, PivotGridCustomSummaryEventArgs e)
{
    if (e.DataField != fieldSales) return;

    // Distinct count example
    var values = new HashSet<object>();
    for (int i = 0; i < e.SummaryProcess.RecordCount; i++)
        values.Add(e.SummaryProcess.GetFieldValue(fieldSales, i));

    e.CustomValue = values.Count;
}
```

Key `PivotGridCustomSummaryEventArgs` members:
- `e.DataField` — which field is being calculated
- `e.SummaryProcess.RecordCount` — number of underlying records
- `e.SummaryProcess.GetFieldValue(field, index)` — read a raw value
- `e.CustomValue` — assign the result here

---

## Grouping

> **Group by interval with the built-in `GroupInterval` — do NOT precompute group columns in LINQ or `ExpressionDataBinding` for simple ranges.** To bucket a numeric field ("group by 5") or build a date hierarchy, set `GroupInterval` (and `GroupIntervalNumericRange` for numbers) on the field's **`DataSourceColumnBinding`**. Computing a `SizeGroup` string in your query, or an `ExpressionDataBinding` like `Floor([Size]/5)*5`, is unnecessary and loses correct numeric sorting.
>
> **Engine note:** Set interval grouping on the field's `DataSourceColumnBinding` — this is required for the `Optimized` engine (this skill's recommended/default engine) and for Server mode. The field-level `PivotGridField.GroupInterval` / `GroupIntervalNumericRange` properties only work in the `Legacy` / `LegacyOptimized` engines and are **ignored** under `Optimized`/Server.

### Numeric Range Grouping ("group by N")

Set `GroupInterval = PivotGroupInterval.Numeric` and `GroupIntervalNumericRange` (the bucket width; default `10`) on the binding:

```csharp
// Group the Size field into ranges of 5: 0–4, 5–9, 10–14, …
fieldSize.DataBinding = new DataSourceColumnBinding("Size") {
    GroupInterval             = PivotGroupInterval.Numeric,
    GroupIntervalNumericRange = 5
};

// If the field was created via AddDataSourceColumn, cast its existing binding instead:
// var b = (DataSourceColumnBinding)fieldSize.DataBinding;
// b.GroupInterval = PivotGroupInterval.Numeric;
// b.GroupIntervalNumericRange = 5;
```

`GroupIntervalNumericRange` also applies to the age intervals `YearAge` / `MonthAge` / `WeekAge` / `DayAge`.

### Date / Alphabetical Interval Grouping

`GroupInterval` works the same way for date hierarchies and text — set it on the field's `DataSourceColumnBinding`:

```csharp
// Date hierarchy — Year then Quarter (two fields over the same column)
fieldYear.DataBinding    = new DataSourceColumnBinding("OrderDate") { GroupInterval = PivotGroupInterval.DateYear };
fieldQuarter.DataBinding = new DataSourceColumnBinding("OrderDate") { GroupInterval = PivotGroupInterval.DateQuarter };

// Alphabetical grouping by first letter
fieldProduct.DataBinding = new DataSourceColumnBinding("ProductName") { GroupInterval = PivotGroupInterval.Alphabetical };
```

### Custom Group Intervals

Only when a built-in `PivotGroupInterval` value cannot express the buckets, handle the `CustomGroupInterval` event (Legacy / LegacyOptimized engine) or use an `ExpressionDataBinding` (Optimized engine):

```csharp
// Optimized mode: a custom, non-uniform bucket expression
fieldBucket.DataBinding = new ExpressionDataBinding("Floor([Sales] / 1000) * 1000");
```

### PivotGridGroup (field hierarchy)

Groups multiple related fields that always appear/move together:

```csharp
var yearQtrGroup = new PivotGridGroup();
yearQtrGroup.AddRange(new[] { fieldYear, fieldQuarter, fieldMonth });
pivotGridControl1.Groups.Add(yearQtrGroup);
```

---

## Sorting

### Default Sort Order

```csharp
field.SortOrder = DevExpress.XtraPivotGrid.PivotSortOrder.Ascending;  // default
field.SortOrder = DevExpress.XtraPivotGrid.PivotSortOrder.Descending;
```

### Sort by Display Text vs. Value

```csharp
// Sort alphabetically by display text instead of underlying value
field.SortMode = PivotSortMode.DisplayText;

// Sort by underlying value (default)
field.SortMode = PivotSortMode.Value;
```

### Sort by Summary (Sort a Row/Column by a Data Field's Values)

```csharp
// Sort the "Category" row field by the "Sales" data field summary
fieldCategory.SortBySummaryInfo.Field = fieldSales;
// Direction is set on the field itself — SortBySummaryInfo has no SortOrder:
fieldCategory.SortOrder = PivotSortOrder.Descending;
// Optionally restrict the sort to a specific column/row crossing by adding
// entries to fieldCategory.SortBySummaryInfo.Conditions.
```

### Top N (limit displayed values)

```csharp
// Show only the top 5 categories by Sales (largest first)
fieldCategory.SortBySummaryInfo.Field = fieldSales;   // rank by the Sales summary
fieldCategory.SortOrder = PivotSortOrder.Descending;   // largest first
fieldCategory.TopValueCount    = 5;
fieldCategory.TopValueType     = PivotTopValueType.Absolute; // Absolute = top N by count (Percent / Sum also available)
fieldCategory.TopValueShowOthers = true; // group the rest as "Others"
```

---

## Filtering

### Programmatic Filter via FilterValues

```csharp
// Include only "USA" in the Country filter field
fieldCountry.FilterValues.Clear();
fieldCountry.FilterValues.FilterType = PivotFilterType.Included;
fieldCountry.FilterValues.Add("USA");
fieldCountry.FilterValues.Add("Germany");

// Exclude specific values
fieldCountry.FilterValues.FilterType = PivotFilterType.Excluded;
fieldCountry.FilterValues.Add("N/A");

// Remove all filters
fieldCountry.FilterValues.Clear();
```

### Excel-Style Filter Dropdowns

Enabled by default. Users can open a filter dropdown from any column/row header.

```csharp
// Disable the filter dropdown for a field
fieldCategory.Options.ShowFilterPopup = false;

// Disable the Filter Panel strip (shows active filter criteria)
pivotGridControl1.OptionsView.ShowFilterPanel = false;
```

### Filter Criteria (DevExpress Criteria Language)

```csharp
// Apply a criteria-based filter to the whole control
pivotGridControl1.ActiveFilterCriteria = new BinaryOperator("Sales", 1000, BinaryOperatorType.GreaterOrEqual);

// Equivalent string-based form:
// pivotGridControl1.ActiveFilterString = "[Sales] >= 1000";

// Clear criteria filter
pivotGridControl1.ActiveFilterCriteria = null;
```

### Group Filters

Filtering for grouped date/numeric fields: filter the parent group field — it hides all child values automatically.

---

## Totals Show/Hide per Field

```csharp
// Show totals for a specific field
fieldCategory.Options.ShowTotals = true;

// Never show a total for this field
fieldCategory.Options.ShowTotals = false;
```

---

## Key API Reference

| Member | Description |
|---|---|
| `PivotGridField.SummaryType` | `PivotSummaryType` — aggregation function |
| `PivotGridField.SummaryDisplayType` | `PivotSummaryDisplayType` — relative display mode |
| `PivotGridControl.CustomSummary` | Event for custom aggregation logic |
| `DataSourceColumnBinding.GroupInterval` | `PivotGroupInterval` — date/numeric/alphabetical split (use this in Optimized/Server mode) |
| `DataSourceColumnBinding.GroupIntervalNumericRange` | Bucket width when `GroupInterval = Numeric` (or an age interval); default `10` |
| `PivotGridControl.Groups` | `PivotGridGroupCollection` |
| `PivotGridGroup.AddRange(fields)` | Add fields to a group |
| `PivotGridField.SortOrder` | `PivotSortOrder.Ascending` / `Descending` |
| `PivotGridField.SortMode` | `PivotSortMode.Value` / `DisplayText` |
| `PivotGridField.SortBySummaryInfo` | Sort row/col by a data field |
| `PivotGridField.TopValueCount` | Limit displayed values |
| `PivotGridField.TopValueType` | `PivotTopValueType.Absolute` / `Percent` / `Sum` |
| `PivotGridField.TopValueShowOthers` | Group remaining values as "Others" |
| `PivotGridField.FilterValues` | `PivotGridFieldFilterValues` — programmatic filter |
| `FilterValues.FilterType` | `PivotFilterType.Included` / `Excluded` |
| `FilterValues.Add(value)` | Add a filter value |
| `FilterValues.Clear()` | Remove all filter values |
| `PivotGridControl.ActiveFilterCriteria` | DevExpress criteria filter on the whole control (`CriteriaOperator`) |
| `PivotGridControl.ActiveFilterString` | Same filter expressed as a criteria string |
| `PivotGridField.Options.ShowTotals` | Per-field total visibility |
| `PivotGridField.Options.ShowFilterPopup` | Show/hide per-field filter dropdown |

---

## Source

- Summaries: https://docs.devexpress.com/content/WindowsForms/9384?md=true
- Custom Summaries: https://docs.devexpress.com/content/WindowsForms/9391?md=true
- Summary Display Types: https://docs.devexpress.com/content/WindowsForms/1935?md=true
- Grouping: https://docs.devexpress.com/content/WindowsForms/1846?md=true
- Sorting: https://docs.devexpress.com/content/WindowsForms/1809?md=true
- Filtering: https://docs.devexpress.com/content/WindowsForms/1811?md=true

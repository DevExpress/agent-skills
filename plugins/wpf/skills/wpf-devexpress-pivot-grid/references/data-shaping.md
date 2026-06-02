# Data Shaping — DevExpress WPF Pivot Grid

"Data Shaping" covers everything between data binding and visual rendering: **aggregation** (how cell values are computed), **grouping** (how dates / numbers / strings roll up), **sorting** (by value, by summary, by custom rule), and **filtering** (per-field and whole-pivot).

## When to Use This Reference

Use this when you need to:

- Change the aggregation function per field (Sum, Count, Average, Min, Max, Custom)
- Group date values by year / quarter / month / day / week
- Group numeric values into ranges
- Group strings alphabetically (A–E, F–J, etc.)
- Sort field values by their summary instead of their text
- Apply filters at the field or pivot level
- Compute calculated fields or window calculations

## Key Properties and Enums

| Type / Member | Purpose |
|---|---|
| `PivotGridField.SummaryType` | Aggregation function. `Sum`, `Count`, `Average`, `Min`, `Max`, `Custom`, `StdDev`, `Var`, etc. |
| `FieldSummaryType` | Enum of standard summary functions. |
| `DataSourceColumnBinding.GroupInterval` | Grouping resolution: `Default`, `Alphabetical`, `DateYear`, `DateMonth`, `DateDay`, `DateQuarter`, `DateWeekOfYear`, `DateWeekOfMonth`, `DateDayOfYear`, `DateDayOfWeek`, `DateHour`, `DateMinute`, `DateSecond`, `NumericRange`. |
| `DataSourceColumnBinding.GroupIntervalNumericRange` | When `GroupInterval = NumericRange`, the bucket width (e.g., `100` → 0–99, 100–199, ...). |
| `PivotGridField.SortOrder` | `Ascending` or `Descending`. |
| `PivotGridField.SortByField` / `SortByFieldName` | The data field whose summary defines the sort order. |
| `PivotGridField.TopValueCount` / `TopValueType` / `TopValueShowOthers` | Top-N display limit (absolute count or percentage). |
| `PivotGridControl.FilterCriteria` / `FilterString` | Pivot-wide filter (uses Criteria Language Syntax). |

## Aggregation (`SummaryType`)

Numeric fields placed in the Data area default to `Sum`. Change explicitly:

```csharp
var field = pivotGridControl1.Fields.Add();
field.Caption = "Avg Sales";
field.Area = FieldArea.DataArea;
field.DataBinding = new DataSourceColumnBinding("Amount");
field.SummaryType = FieldSummaryType.Average;
```

Available functions (from the docs):

- `Sum`
- `Count` / `CountDistinct`
- `Average`
- `Min` / `Max`
- `StdDev` / `StdDevp` / `Var` / `Varp`
- `Median` / `Mode`
- `Custom` (handle `PivotGridControl.CustomSummary` event for arbitrary aggregation)

Source: `articles/controls-and-libraries/pivot-grid/data-shaping.md` § Aggregation (`xref:11732`) and `pivot-grid/data-shaping/aggregation/summaries/`.

## Date Grouping

Group daily `OrderDate` values into year, quarter, month, etc.:

```csharp
var yearField = pivotGridControl1.Fields.Add();
yearField.Caption = "Year";
yearField.Area = FieldArea.ColumnArea;
yearField.DataBinding = new DataSourceColumnBinding("OrderDate") {
    GroupInterval = FieldGroupInterval.DateYear
};
```

Stack multiple group intervals on the same column to build a date hierarchy:

```csharp
AddDate("Year",    FieldArea.ColumnArea, "OrderDate", FieldGroupInterval.DateYear,    0);
AddDate("Quarter", FieldArea.ColumnArea, "OrderDate", FieldGroupInterval.DateQuarter, 1);
AddDate("Month",   FieldArea.ColumnArea, "OrderDate", FieldGroupInterval.DateMonth,   2);
```

User can expand Year → Quarter → Month inline.

Source: `articles/controls-and-libraries/pivot-grid/data-shaping.md` § Grouping (`xref:8061`).

## Numeric Range Grouping

Bucket numeric values into ranges of a fixed width:

```csharp
var priceField = pivotGridControl1.Fields.Add();
priceField.Caption = "Price Range";
priceField.Area = FieldArea.RowArea;
priceField.DataBinding = new DataSourceColumnBinding("UnitPrice") {
    GroupInterval = FieldGroupInterval.NumericRange,
    GroupIntervalNumericRange = 50    // 0–49, 50–99, 100–149, ...
};
```

## Alphabetical Grouping

Bucket string values by first letter range:

```csharp
field.DataBinding = new DataSourceColumnBinding("CustomerName") {
    GroupInterval = FieldGroupInterval.Alphabetical
};
```

## Sorting

End users click a column or row header to sort. The Pivot Grid sorts within the level — siblings under a common parent.

### Sort Order

```csharp
field.SortOrder = FieldSortOrder.Ascending;   // or Descending
```

### Sort by Summary Value

To sort a row/column field by the summary of a *data* field, assign the data field (or its `Name`) to `SortByField` or `SortByFieldName`:

```csharp
// Sort products by their Extended Price summary, descending
fieldProductName.SortByField = fieldExtendedPrice;
fieldProductName.SortOrder = FieldSortOrder.Descending;
```

XAML:

```xaml
<dxpg:PivotGridField Name="fieldProductName"
                     SortByField="{Binding ElementName=fieldExtendedPrice}"
                     SortOrder="Descending"/>
```

By default the Pivot sorts by the **Grand Total** column/row of the chosen data field.

#### Sort by a Specific Column/Row (Not Grand Total)

Add `SortByCondition` entries to pin the sort target to a specific cell column:

```csharp
fieldProductName.SortByField = fieldExtendedPrice;
fieldProductName.SortByConditions.Add(new SortByCondition(fieldYear, 2016));
fieldProductName.SortByConditions.Add(new SortByCondition(fieldQuarter, 2));
// Now products are sorted by Extended Price values in the Q2 2016 column.
```

End users can right-click any innermost column or row header to invoke **Sort By Summary** from the context menu. Enable/disable globally with `PivotGridControl.AllowSortBySummary` (and per-field with `PivotGridField.AllowSortBySummary`).

### Sort by Specific Summary Function

`SortByFieldName` (string-name form) lets you sort by a summary function different from the one being displayed:

```csharp
fieldProductName.SortBySummaryType = FieldSummaryType.Average;
fieldProductName.SortByFieldName = "Extended Price";
```

### Top-N Limit (Display Only Top Values)

Limit displayed field values to the top N (by sort order). Other values are **hidden and excluded from totals**.

```csharp
salesPersonField.SortByField = extendedPriceField;
salesPersonField.SortOrder = FieldSortOrder.Descending;
salesPersonField.TopValueCount = 5;                     // Show top 5 only
salesPersonField.TopValueShowOthers = true;             // Combine the rest into "Others"
```

| Property | Meaning |
|---|---|
| `PivotGridField.TopValueCount` | Number of top values to display. Default = 0 (show all). |
| `PivotGridField.TopValueType` | `Absolute` (default) — `TopValueCount` is a count. `Percent` — interpret it as a percentage. |
| `PivotGridField.TopValueShowOthers` | When `true`, hidden values are combined into a single "Others" row below the visible ones. |

In **OLAP mode** with `TopValueType.Percent`, the Pivot Grid shows top values whose **cumulative total** is ≥ the specified percentage of the Grand Total. In regular mode, percent means a percentage of the count of unique values.

> In OLAP mode, totals are always calculated against *all* values, even when Top N is active.

### Sorting Limitations

Sort by Summary ignores: running totals, custom `SummaryDisplayType`, and values produced by `CustomCellValue` / `CustomCellDisplayText` events.

Source:
- `articles/controls-and-libraries/pivot-grid/data-shaping/sorting/sorting-by-summary.md` (`xref:8072`)
- `articles/controls-and-libraries/pivot-grid/data-shaping/sorting/display-top-n-values.md` (`xref:8063`)

## Filtering

### Per-Field Filter

End users click the field's filter glyph to open a checkbox list of values:

```xaml
<dxpg:PivotGridControl AllowFilter="True"/>
```

For programmatic filter setup:

```csharp
field.FilterValues.FilterType = FieldFilterType.Excluded;
field.FilterValues.Values.Add("Beverages");
```

`FieldFilterValues.FilterType` is of type `FieldFilterType` (values: `Included`, `Excluded`).

### Pivot-Wide Filter (Criteria Expression)

```csharp
pivotGridControl1.FilterCriteria = new BinaryOperator("Amount", 500, BinaryOperatorType.Greater);
// Or string form:
pivotGridControl1.FilterString = "[Amount] > 500";
```

The filter criterion uses the same Criteria Language Syntax as `GridControl.FilterString`. Available via `DevExpress.Data.Filtering`.

Source: `articles/controls-and-libraries/pivot-grid/data-shaping.md` § Filtering (`xref:10927`).

## Calculated Fields

A field can compute its value from an expression:

```csharp
var profit = pivotGridControl1.Fields.Add();
profit.Caption = "Profit";
profit.Area = FieldArea.DataArea;
profit.DataBinding = new ExpressionDataBinding { Expression = "[Revenue] - [Cost]" };
profit.SummaryType = FieldSummaryType.Sum;
```

The expression uses Criteria Language Syntax — field names in square brackets, operators `+`, `-`, `*`, `/`, parentheses, and functions (`Iif`, `Substring`, etc.).

Source: `articles/controls-and-libraries/pivot-grid/fundamentals/fields.md` references `xref:8025`.

## Window Calculations

Running totals, differences, ranks, moving averages, percent-of-total — produced by binding a data field to a **window calculation binding** instead of a raw column. Available only with `DataProcessingEngine.Optimized`.

| Binding class | Calculation |
|---|---|
| `RunningTotalBinding` | Cumulative total across the window |
| `DifferenceBinding` | Difference between consecutive values |
| `RankBinding` | Rank of each value within the window |
| `PercentOfTotalBinding` | Percentage of total within the window |
| `MovingCalculationBinding` | Moving average / moving sum across a sliding window |
| `WindowExpressionBinding` | Arbitrary expression over the window |

All live in `DevExpress.Xpf.PivotGrid` and inherit from `DataBinding`. Pattern: wrap the base `DataSourceColumnBinding` in the window binding via its `Source` property.

### Running Total (XAML)

```xaml
<dxpg:PivotGridControl DataProcessingEngine="Optimized" DataSource="{Binding Sales}">
    <dxpg:PivotGridControl.Fields>
        <dxpg:PivotGridField Area="DataArea" Caption="Running Total" CellFormat="C">
            <dxpg:PivotGridField.DataBinding>
                <dxpg:RunningTotalBinding PartitioningCriteria="ColumnValue"
                                          SummaryType="Sum">
                    <dxpg:RunningTotalBinding.Source>
                        <dxpg:DataSourceColumnBinding ColumnName="Extended Price"/>
                    </dxpg:RunningTotalBinding.Source>
                </dxpg:RunningTotalBinding>
            </dxpg:PivotGridField.DataBinding>
        </dxpg:PivotGridField>
    </dxpg:PivotGridControl.Fields>
</dxpg:PivotGridControl>
```

### Running Total (C#)

```csharp
var runningTotal = pivotGridControl1.Fields.Add();
runningTotal.Caption = "Running Total";
runningTotal.Area = FieldArea.DataArea;
runningTotal.CellFormat = "C";
runningTotal.DataBinding = new RunningTotalBinding {
    Source = new DataSourceColumnBinding("Extended Price"),
    PartitioningCriteria = CalculationPartitioningCriteria.ColumnValue,
    SummaryType = FieldSummaryType.Sum,
};
```

**Recipe**:

1. Pick the window binding class for the calculation you want.
2. Set its `Source` to a `DataSourceColumnBinding` (the raw column to aggregate).
3. Set `PartitioningCriteria` to control whether the window runs across columns, rows, or a sub-partition.
4. Assign to `PivotGridField.DataBinding`.

> Window calculations require `DataProcessingEngine.Optimized` (the default for non-legacy data sources). The Legacy engine ignores window-calculation bindings.

Source: `articles/controls-and-libraries/pivot-grid/binding-to-data/in-memory-mode/Optimized-Mode/bind-pivot-grid-fields-to-window-calculations.md` (`xref:403913`).

## Custom Summary

For aggregation that doesn't fit the built-in `FieldSummaryType` values, handle `PivotGridControl.CustomSummary`:

```csharp
pivotGridControl1.CustomSummary += (s, e) => {
    // e.SummaryValue.SummaryValue = ...   (verify exact arg shape via MCP)
};
```

Set the field to `SummaryType.Custom`:

```csharp
field.SummaryType = FieldSummaryType.Custom;
```

> Verify exact `CustomSummary` event-arg member names via DxDocs MCP:
> `devexpress_docs_search(technology="WPF Pivot Grid", query="CustomSummary event handler")`

## Source Material

- `articles/controls-and-libraries/pivot-grid.md` (root, § Data Shaping)
- `articles/controls-and-libraries/pivot-grid/data-shaping.md`
- `articles/controls-and-libraries/pivot-grid/data-shaping/aggregation/summaries/`
- `articles/controls-and-libraries/pivot-grid/fundamentals/fields.md`

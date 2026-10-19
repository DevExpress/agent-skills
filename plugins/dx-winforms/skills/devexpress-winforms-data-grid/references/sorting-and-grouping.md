# Sorting and Grouping

This reference covers sorting and grouping in `GridView`, `BandedGridView`, `AdvBandedGridView`, and `TreeList`. Topics: simple and multi-column sort, custom sort comparisons, sorting by display text, group panel UX, custom grouping logic, grouping by date intervals, and customizing the text shown in group rows.

## When to Use This Reference

- Defining a default sort or group order in code.
- Implementing a custom comparison (e.g., season ordering, range buckets).
- Sorting a lookup column by its display text instead of its raw key.
- Grouping `DateTime` columns by year/month/day/range.
- Customizing group-row text or summary alignment.

## Sort in Code

```csharp
gridView1.Columns["OrderDate"].SortOrder = ColumnSortOrder.Descending;
gridView1.Columns["OrderDate"].SortIndex = 0;  // primary sort key
gridView1.Columns["Customer"].SortIndex  = 1;  // secondary
```

`SortIndex >= 0` makes a column participate in sorting in the given priority order. `SortOrder` controls direction. To clear all sort, call `gridView1.ClearSorting()`.

### Replace the entire sort with `SortInfo`

```csharp
gridView1.SortInfo.ClearAndAddRange(new[] {
    new GridColumnSortInfo(gridView1.Columns["Country"], ColumnSortOrder.Ascending),
    new GridColumnSortInfo(gridView1.Columns["City"],    ColumnSortOrder.Ascending)
});
```

### Merged sort (sort multiple columns as a single logical key)

```csharp
gridView1.SortInfo.ClearAndAddRange(new[] {
    new GridMergedColumnSortInfo(
        new[] { colShipCountry, colShipCity, colShipRegion },
        new[] { ColumnSortOrder.Ascending, ColumnSortOrder.Descending, ColumnSortOrder.Ascending }),
    new GridColumnSortInfo(colCustomerID, ColumnSortOrder.Descending)
});
```

## Sort Modes

```csharp
gridView1.Columns["Status"].SortMode = ColumnSortMode.DisplayText;
```

| Value | Meaning |
|---|---|
| `Default` | Numeric/date columns sort by value; string columns sort by display text. |
| `Value` | Always sort by the raw value (edit value). |
| `DisplayText` | Sort by the formatted display text — useful for columns with custom `DisplayFormat` or `CustomColumnDisplayText`. |
| `Custom` | Raise `CustomColumnSort` for every comparison; you implement the logic. |

In ServerMode, `DisplayText` is unsupported — use `FieldNameSortGroup` to point the column at a sortable field:

```csharp
// Sort a lookup column by the related display field even in ServerMode
gridView1.Columns["CategoryId"].FieldNameSortGroup = "CategoryName";
```

## Custom Sort

```csharp
gridView1.Columns["OrderDate"].SortMode = ColumnSortMode.Custom;
gridView1.CustomColumnSort += (s, e) => {
    if (e.Column.FieldName != "OrderDate") return;
    var seasonA = GetSeason((DateTime)e.Value1);
    var seasonB = GetSeason((DateTime)e.Value2);
    e.Result  = Comparer<string>.Default.Compare(seasonA, seasonB);
    e.Handled = true;
};

static string GetSeason(DateTime d) => (d.Month / 3) switch {
    1 => "Spring", 2 => "Summer", 3 => "Autumn", _ => "Winter"
};
```

`CustomColumnSort` also fires for grouping (rows in the same custom bucket are grouped together).

## Group Panel and Grouping

```csharp
gridView1.OptionsView.ShowGroupPanel = true;       // show drag-target area
gridView1.OptionsCustomization.AllowGroup = true;  // users may group/ungroup
gridView1.Columns["Country"].GroupIndex = 0;       // group by Country
gridView1.Columns["City"].GroupIndex    = 1;       // sub-group by City
gridView1.ExpandAllGroups();
```

To ungroup a column set `GroupIndex = -1`.

### Show grouped columns inside data rows

By default, columns used for grouping disappear from the data area. To keep them visible:

```csharp
gridView1.OptionsView.ShowGroupedColumns = true;
```

### Sort group rows by summary or text

```csharp
gridView1.GroupSummarySortInfo.Add(
    new GridGroupSummarySortInfo(
        gridView1.GroupSummary[0],
        gridView1.Columns["Country"],
        ColumnSortOrder.Descending));
```

## Custom Group Intervals (Date)

For `DateTime` columns, use the built-in `GroupInterval`:

| Value | Buckets |
|---|---|
| `DateYear` | One group per year. |
| `DateMonth` | Per month. |
| `DateDay` | Per day. |
| `DateRange` | Today, Yesterday, Last Week, Last Month, Older… (Outlook-style). |
| `Value` *(default)* | Distinct values. |
| `DisplayText` | Distinct display-text values. |
| `Alphabetical` | First character of the value (string columns). |

```csharp
gridView1.Columns["OrderDate"].GroupInterval = ColumnGroupInterval.DateRange;
gridView1.Columns["OrderDate"].Group();
```

End users can change the interval via the column header's *Group Interval* submenu (`view.OptionsMenu.ShowDateTimeGroupIntervalMenu`).

## Custom Grouping

```csharp
gridView1.Columns["OrderSum"].SortMode = ColumnSortMode.Custom;
gridView1.Columns["OrderSum"].Group();

gridView1.CustomColumnGroup += (s, e) => {
    if (e.Column.FieldName != "OrderSum") return;

    double a = Math.Floor(Convert.ToDouble(e.Value1) / 100);
    double b = Math.Floor(Convert.ToDouble(e.Value2) / 100);
    e.Result  = (a >= 14 && b >= 14) ? 0 : Comparer<double>.Default.Compare(a, b);
    e.Handled = true;
};

// Custom group-row text
gridView1.CustomColumnDisplayText += (s, e) => {
    if (e.Column.FieldName != "OrderSum" || !e.IsForGroupRow) return;

    double row = Convert.ToDouble(gridView1.GetGroupRowValue(e.GroupRowHandle, e.Column));
    double bucket = Math.Floor(row / 100);
    e.DisplayText = bucket >= 14
        ? $"Order sum: ≥ {bucket * 100:c0}"
        : $"Order sum: {bucket * 100:c0} – {(bucket + 1) * 100 - 0.01:c0}";
};
```

## Group Row Text and Format

Three layers, in order of precedence:

1. `GridView.GroupFormat` — view-wide template (`"{0} (Count={1})"`).
2. `GridColumn.GroupFormat` — per-column template.
3. `CustomColumnDisplayText` with `e.IsForGroupRow == true` — full control.

```csharp
gridView1.GroupFormat = "[#image]{1} {2}";   // 1 = column caption, 2 = group value
gridView1.Columns["Country"].GroupFormat = "{0}: {1}";
```

## Group Summary Alignment

```csharp
gridView1.OptionsBehavior.AlignGroupSummaryInGroupRow = DefaultBoolean.True;
```

## Persist Sort and Group

Sort/group settings are part of the layout — they roundtrip via `SaveLayoutToXml` etc. See [saving-and-restoring-layout.md](saving-and-restoring-layout.md).

## TreeList Specifics

`TreeList` does **not** support data grouping (the tree is the grouping). Sorting is the same shape:

```csharp
treeList1.Columns["LastName"].SortOrder = SortOrder.Ascending;
treeList1.Columns["LastName"].SortIndex = 0;
treeList1.CustomColumnSort += /* same args */;
```

Use `TreeList.SortMode` enum (`Tree`, `Default`, `Flat`) to control whether sorting respects the hierarchy.

## Common Issues

- **Sorting clears my filter**: it should not — verify the data source does not refire `RefreshDataSource`. Filter and sort are independent.
- **`CustomColumnSort` not fired**: `column.SortMode` is still `Default`. Set it to `Custom` explicitly.
- **Custom group buckets fall apart after a refresh**: `CustomColumnGroup` is called only during the sort pass; ensure `e.Handled = true` and that `SortMode = Custom`.
- **Lookup column sorts by ID even with `SortMode = DisplayText`**: ServerMode does not support `DisplayText`. Use `FieldNameSortGroup` instead.
- **`Group()` removes column from view**: that is the default. Set `OptionsView.ShowGroupedColumns = true` to keep it visible.

# Filtering and Search

This reference covers every way to filter and search data in `GridControl`, `TreeList`, and similar data-aware controls — the UI surfaces (Excel-style filter, Auto Filter Row, Filter Editor, Find Panel, incremental search) and the code APIs (`ActiveFilterString`, `ActiveFilterCriteria`, `ActiveFilter.NonColumnFilter`, `CustomRowFilter`, `SubstituteFilter`, `ApplyFindFilter`, `LocateByValue`).

## When to Use This Reference

- Enabling end-user filter UI: dropdowns, Auto Filter Row, Filter Editor, Find Panel.
- Applying or modifying filters in code, including filters that are not bound to any column.
- Implementing custom row visibility logic (`CustomRowFilter`).
- Intercepting and modifying filter criteria before they take effect (`SubstituteFilter`).
- Customizing the items shown in the Excel-style filter menu (`FilterPopupExcelData`).
- Performing an incremental search or programmatic value lookup.

## UI Filter Surfaces

| Surface | Toggle | Notes |
|---|---|---|
| **Excel-style filter dropdown** | Built-in; click the funnel in a column header. | Two tabs — "Values" + "Filters". Per-column. |
| **Classic filter dropdown** | `OptionsFilter.UseNewCustomFilterDialog = false`. | Legacy two-condition dialog. |
| **Auto Filter Row** | `view.OptionsView.ShowAutoFilterRow = true`. | Topmost row; type to filter. |
| **Filter Panel** | Appears automatically once a filter is active. | "Clear", "Toggle", recent filters, "Edit Filter". |
| **Advanced Filter Editor** | Click *Edit Filter* in the Filter Panel or right-click a header → *Filter Editor*. | Visual + text views, autocomplete. |
| **Find Panel** | `view.OptionsFind.AlwaysVisible = true` or `Ctrl+F` after enabling `view.OptionsFind.FindMode`. | Two modes: `Always` (filter rows) and `FindByEnter` (highlight). |
| **Filtering UI Context** | External `FilteringUIContext` component bound to the grid. | Generates editors on a side panel for users to filter. |
| **Incremental search** | `view.OptionsBehavior.AllowIncrementalSearch = true`. | Type while a column is focused — matches are highlighted. |
| **Conditional Formatting Filters** | `view.OptionsMenu.ShowConditionalFormattingItem = true` and at least one rule. | Filter rows by applied formatting rule. |
| **AI Semantic Search** | `view.OptionsFind.SearchType = SemanticSearch`. | Embedding-based search via DevExpress AI Integration. |

### Feature matrix

| Control | Excel filter | Classic | Auto Filter Row | Filter Editor | Find Panel | Filter in code | Conditional-format filter |
|---|---|---|---|---|---|---|---|
| `GridControl` | yes | yes | yes | yes | yes | yes | yes |
| `TreeList`, `GanttControl` | yes | yes | yes | yes | yes | yes | yes |
| `PivotGridControl` | yes | yes | yes | yes | yes | yes | no |
| `VGridControl` / `PropertyGridControl` | yes | no | no | yes | yes | yes | n/a |

## Apply a Filter in Code

Two interchangeable properties on `ColumnView`:

```csharp
// Text expression
gridView1.ActiveFilterString = "[Country] = 'USA' AND [Freight] > 50";

// Strongly typed criteria
gridView1.ActiveFilterCriteria = CriteriaOperator.Parse(
    "[Country] = ? AND [Freight] > ?", "USA", 50);
```

`ActiveFilterString` parses the string into a `CriteriaOperator` and assigns it to `ActiveFilterCriteria`. Use `CriteriaOperator.Parse` (`?` placeholders) to avoid string concatenation and quoting issues.

### Per-column filter

```csharp
gridView1.Columns["Country"].FilterInfo = new ColumnFilterInfo("[Country] = 'USA'");
gridView1.Columns["Country"].ClearFilter();
```

### Filter not tied to a column

Use `NonColumnFilter` for arbitrary expressions that should not appear in any column header dropdown:

```csharp
gridView1.ActiveFilter.NonColumnFilter = "UnitPrice * Quantity >= 50";
```

End users cannot clear or modify a NonColumnFilter from a column dropdown.

### Toggle and clear

```csharp
gridView1.ActiveFilterEnabled = false;     // temporarily disable
gridView1.ActiveFilterString  = string.Empty;  // clear all column filters
gridView1.ClearColumnsFilter();            // clear column filters only
```

## Excel-Style Filter Menu

Enabled by default. Customize via per-column or per-view options:

```csharp
gridView1.OptionsFilter.DefaultFilterEditorView = FilterEditorViewMode.VisualAndText;
gridView1.Columns["Country"].OptionsFilter.FilterPopupMode = FilterPopupMode.CheckedList;
gridView1.Columns["Country"].OptionsFilter.AllowFilter = true;
```

### Add custom predefined items to the Filters tab

```csharp
gridView1.FilterPopupExcelData += (s, e) => {
    if (e.Column.FieldName != "MPGCity") return;

    e.AddFilter("Fuel Economy (<color=green>High</color>)",   $"[{e.Column.FieldName}] <= 15", isPreset: true);
    e.AddFilter("Fuel Economy (<color=orange>Medium</color>)",$"[{e.Column.FieldName}] > 15 AND [{e.Column.FieldName}] < 25", isPreset: true);
    e.AddFilter("Fuel Economy (<color=red>Low</color>)",     $"[{e.Column.FieldName}] >= 25", isPreset: true);
};
```

Other Excel-filter customization events:
- `FilterPopupExcelCustomizeTemplate` — modify templates.
- `FilterPopupExcelPrepareTemplate` — replace the Values tab UI.
- `FilterPopupExcelParseFilterCriteria` — read back parsed criteria.
- `FilterPopupExcelQueryFilterCriteria` — change generated criteria.

## Auto Filter Row

```csharp
gridView1.OptionsView.ShowAutoFilterRow = true;
gridView1.Columns["Quantity"].OptionsFilter.AutoFilterCondition = AutoFilterCondition.Between;
gridView1.Columns["Name"].OptionsFilter.ImmediateUpdateAutoFilter = false;  // commit on Enter
```

Auto Filter Row supports `*` and `%` wildcards in string columns and rich comparison operators per column.

## Find Panel

```csharp
gridView1.OptionsFind.AlwaysVisible    = true;
gridView1.OptionsFind.SearchInPreview  = true;
gridView1.OptionsFind.HighlightFindResults = true;
gridView1.OptionsFind.FindFilterColumns = "Customer;OrderDate";  // or "*"
gridView1.OptionsFind.FindMode         = FindMode.Always;        // filter as you type
```

Programmatic search:

```csharp
gridView1.ApplyFindFilter("Berlin");
```

Find Panel syntax supports phrases, exclusion (`-foo`), and per-column qualifiers (`Customer:Acme`). See *Find Panel Syntax* in the DevExpress docs for the full grammar.

## CustomRowFilter — Override Visibility

Force individual rows visible or hidden regardless of the active filter. Use for "pinned" rows or virtual data-shaping logic.

```csharp
gridView1.CustomRowFilter += (s, e) => {
    var country = gridView1.GetListSourceRowCellValue(e.ListSourceRow, "Country");
    if (Equals(country, "USA")) {
        e.Visible = true;
        e.Handled = true;     // prevent default filter from also evaluating
    }
};
```

> **`e.ListSourceRow`** is the data-source index (`int`), not the grid row handle. It is stable across grouping and sorting.

`CustomRowFilter` is unsupported in ServerMode.

## SubstituteFilter — Intercept the Final Criteria

`SubstituteFilter` fires after the user changes the active filter, just before it is applied. You receive `e.Filter` (the current criteria) and can replace or compose it.

```csharp
gridView1.SubstituteFilter += (s, e) => {
    // Always restrict to the currently selected year, ANDed with whatever the user picked.
    var year = Convert.ToInt32(showByYearEdit.EditValue);
    e.Filter &= CriteriaOperator.Parse("GetYear(OrderDate) = ?", year);
};
```

> Do not mutate the existing `e.Filter` object — assign a new criteria via `e.Filter = …` or use `&=` / `|=` which produce a new combined operator.

To trigger a refresh after dependent UI changes, rebind the filter:

```csharp
var current = gridView1.ActiveFilterCriteria;
gridView1.BeginDataUpdate();
gridView1.ActiveFilterCriteria = null;
gridView1.ActiveFilterCriteria = current;
gridView1.EndDataUpdate();
```

## MRU Filters

Recent filters appear in the Filter Panel dropdown. Customize:

```csharp
gridView1.OptionsFilter.AllowMRUFilterList = true;
gridView1.OptionsFilter.MRUFilterListCount  = 10;

// Pre-populate at startup:
gridView1.MRUFiltersInfo.Add(new ViewFilterInfo {
    ColumnsFilter = {
        new ViewFilterColumnInfo {
            ColumnName   = "colShipCountry",
            FilterString = "[ShipCountry] = 'USA'",
            Type         = ColumnFilterType.Custom
        }
    }
});
```

## Locate / Find a Row Programmatically

```csharp
int handle = gridView1.LocateByValue(0, gridView1.Columns["OrderID"], 10248);
if (handle != GridControl.InvalidRowHandle)
    gridView1.FocusedRowHandle = handle;

// By display text:
gridView1.LocateByDisplayText(0, column, "Acme Corporation");
```

`LocateByValue` searches synchronously by edit value. For long-running searches use the asynchronous overload that takes a callback.

## TreeList Specifics

`TreeList` has the same filter surfaces plus:

- `TreeList.FilterMode = TreeListFilterMode.Smart` — auto-expand matching nodes; `Normal` — show only matches with their ancestors.
- `TreeList.OptionsFilter.NodesFiltrationMode` — `HideOldestParents`, `ShowAll`, `OnlyMatches`.
- `TreeList.FindFilterText` / `TreeList.ApplyFindFilter(text)` — Find Panel.
- `TreeList.CustomRowFilter` and `TreeList.SubstituteFilter` work the same as on the grid.

## Common Issues

- **Filter ignores accents**: see the case-/accent-insensitive filter KB article; for ServerMode use a `_CI_AI` collation on the SQL column.
- **`ActiveFilterString` throws on `=`**: text literals require single quotes (`[X] = 'foo'`), not double quotes. Use `CriteriaOperator.Parse` with `?` placeholders to avoid quoting.
- **Filter dropdown is empty**: the column's `OptionsFilter.FilterPopupMode = NoFilter` or the column was hidden. Set `FilterPopupMode = CheckedList` (or `List`, `Date`, etc.).
- **`Find Panel` filters but does not highlight**: enable `OptionsFind.HighlightFindResults`.
- **ServerMode filter throws "expression not supported"**: complex functions (custom criteria, regex) are not translatable to SQL. Move them client-side or pre-filter at the data source.

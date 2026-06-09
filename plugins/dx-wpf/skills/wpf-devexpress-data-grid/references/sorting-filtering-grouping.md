# Sorting, Filtering, and Grouping — DevExpress WPF Data Grid

The Data Grid supports unlimited-level sorting, multi-column filtering with several UI flavors, and drag-to-group via the Group Panel. All three operations can be driven from XAML or code, and each has a hook for custom logic (`CustomColumnSort`, `CustomRowFilter`, `SubstituteFilter`, `CustomColumnGroup`). The same APIs apply to `TreeListControl` (different event names — see § "Apply to TreeListControl").

## When to Use This Reference

Use this when you need to:

- Sort by one or many columns from XAML or code
- Sort by **display text** instead of edit values
- Implement **custom sort rules** via `CustomColumnSort`
- Apply filters via Drop-down Filter, Auto Filter Row, Filter Editor, Filter Panel, Filter Elements, predefined filters
- Filter in code via `FilterString` or `FilterCriteria`
- Apply **custom row filters** that don't fit Criteria Language (`CustomRowFilter` / `CustomNodeFilterCommand`)
- **Substitute the user's filter** with a different one before it's applied (`SubstituteFilter`)
- Configure search (search panel, incremental, AI-powered semantic search)
- Group data with the Group Panel or in code
- Implement **custom group logic** (`CustomColumnGroup`, `CustomGroupDisplayText`)
- Group **by date intervals** (year, month, quarter, day) or alphabetical buckets
- Use **merged grouping** (multiple columns into one group level)
- Customize group row appearance

## Sorting

End users sort by clicking column headers (hold <kbd>Shift</kbd> to sort by multiple columns; <kbd>Ctrl</kbd>-click to clear a column's sort).

### Enable / Disable Sorting

| Property | Where | Effect |
|---|---|---|
| `DataViewBase.AllowSorting` | View | Master switch for end-user sorting |
| `ColumnBase.AllowSorting` | Per column | Override view-level setting. `null` inherits |
| `DataViewBase.ShowSortIndicator` | View | Show sort glyph on all headers |
| `ColumnBase.ShowSortIndicator` | Per column | Override per column |

```xaml
<dxg:TableView AllowSorting="True" ShowSortIndicator="True"/>
<dxg:GridColumn FieldName="OrderId" AllowSorting="False"/>
<dxg:GridColumn FieldName="OrderDate" SortOrder="Descending" SortIndex="0"/>
<dxg:GridColumn FieldName="ShipCity"  SortOrder="Ascending"  SortIndex="1"/>
```

`SortOrder` + `SortIndex` define a sort sequence at startup. **Sorting in code is always allowed** regardless of `AllowSorting`.

### Sort Modes (`ColumnBase.SortMode`)

| Value | Behavior |
|---|---|
| `Value` (default) | Sort by raw cell value (uses `IComparable.CompareTo`) |
| `DisplayText` | Sort by formatted display text (e.g., currency-masked, enum-name) |
| `Custom` | Use `CustomColumnSort` event / `CustomColumnSortCommand` |

```xaml
<dxg:GridColumn FieldName="Status" SortMode="DisplayText"/>
```

> **`DisplayText` and `Custom` sort modes are NOT supported in Server Mode.** Server-mode columns must sort by raw value (the server does the work).

### Sort by Another Column (`SortFieldName`)

Sort by a different field than the column displays — e.g., display `OrderId` but sort by `CreateDate`:

```xaml
<dxg:GridColumn FieldName="OrderId"
                SortFieldName="CreateDate"
                SortOrder="Ascending"/>
```

When the user clicks the `OrderId` header, the grid sorts rows by `CreateDate` instead. Same applies to grouping: if `SortFieldName` is set, grouping by this column groups by the alternative field.

### Sorting in Code

```csharp
column.SortOrder = ColumnSortOrder.Ascending;
column.SortOrder = ColumnSortOrder.Descending;
column.SortOrder = ColumnSortOrder.None;   // Clear
```

Multi-column sort:

```csharp
columnDate.SortOrder = ColumnSortOrder.None;
columnCity.SortOrder = ColumnSortOrder.None;

columnDate.SortOrder = ColumnSortOrder.Descending; columnDate.SortIndex = 0;
columnCity.SortOrder = ColumnSortOrder.Ascending;  columnCity.SortIndex = 1;
```

### Custom Sorting

```xaml
<dxg:GridControl CustomColumnSortCommand="{Binding CustomColumnSortCommand}">
    <dxg:GridColumn FieldName="Status" SortMode="Custom" SortOrder="Ascending"/>
</dxg:GridControl>
```

```csharp
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Xpf;

[Command]
public void CustomColumnSort(SortArgs args) {
    if (args.FieldName != nameof(Order.Status)) return;

    // Compare two cell values according to your business rules.
    var weight1 = StatusWeight(args.FirstValue);
    var weight2 = StatusWeight(args.SecondValue);

    args.Result = weight1.CompareTo(weight2);    // -1, 0, +1
}

int StatusWeight(object value) => value as string switch {
    "Critical" => 0,
    "High" => 1,
    "Normal" => 2,
    "Low" => 3,
    _ => 4
};
```

`args.Result` semantics:
- **-1** → first row above second when sorted ascending (below when descending)
- **+1** → first row below second when sorted ascending (above when descending)
- **0** → equal; grid arranges by source-collection index as tiebreaker

Event-based alternative (non-MVVM): handle `GridControl.CustomColumnSort` directly.

Source: `articles/controls-and-libraries/data-grid/sorting/sorting-modes-and-custom-sorting.md`.

### Sort Group Rows by Summary Values

Sort groups by aggregated value (e.g., highest-total group first) rather than by the group key:

```csharp
// Sort group rows by the Sum(Amount) summary of each group
column.GroupInterval = ColumnGroupInterval.Value;
column.SortFieldName = "Amount";          // (verify; pattern from "sorting-group-rows-by-summary-values.md")
// + use SortBySummary API per the article
```

> Verify exact `SortBySummary` API name via DxDocs MCP if needed:
> `devexpress_docs_search(technology="WPF Data Grid", query="sort group rows by summary")`

Source: `articles/controls-and-libraries/data-grid/sorting/sorting-group-rows-by-summary-values.md`.

### Immediate Update of Row Position

When a user edits a cell, the grid moves the row to its new sorted/grouped/filtered position immediately. To preserve position until manual refresh:

```xaml
<dxg:TableView ImmediateUpdateRowPosition="False"/>
```

## Filtering

The Data Grid supports several filter UIs that all combine into a single filter expression (AND-joined):

| UI | How users invoke it | Setup property |
|---|---|---|
| **Drop-down Filter** | Click filter glyph in header | On by default |
| **Auto Filter Row** | Row of input boxes at top | `TableView.ShowAutoFilterRow="True"` |
| **Filter Editor** | Right-click → "Filter Editor" | `TableView.AllowFilterEditor="True"` |
| **Filter Panel** | Strip at bottom showing current filter | On by default when filter applied |
| **Filter Elements** | Inline UI bound to grid (e.g., search box outside the grid) | See `filter-elements.md` |
| **Search Panel** | Search box, optional toolbar button | `TableView.ShowSearchPanelMode="Always"` |

```xaml
<dxg:TableView ShowAutoFilterRow="True"
               AllowFilterEditor="True"
               ShowSearchPanelMode="Always"/>
```

### Drop-Down Filter Variants

DevExpress provides several drop-down filter styles per column (`ColumnBase.AllowedDropDownFilters`):

- **Regular** — checkbox list of unique values
- **Checked** — same as Regular with built-in "(Select All)"
- **Excel-style** — Excel-mimicking filter dropdown with search, sort, and value list
- **Date-Time** — calendar + relative date ranges ("Last 7 days", "This Year")
- **Custom** — your own drop-down content

Source: `articles/controls-and-libraries/data-grid/filtering-and-searching/filter-dropdown/`.

### Filter by Display Text (`ColumnFilterMode`)

```xaml
<dxg:GridColumn FieldName="Status" ColumnFilterMode="DisplayText"/>
```

| Value | Behavior |
|---|---|
| `Value` (default) | Filter by raw cell value (numeric / date / enum) |
| `DisplayText` | Filter by formatted display text (string match against masked / formatted output) |

> **`DisplayText` filter mode is NOT supported in Server Mode.**

### Filtering in Code (`FilterString` / `FilterCriteria`)

#### `FilterString` — Criteria Language Syntax

```csharp
grid.FilterString = "[OrderDate] >= #1/1/2024# AND [Freight] > 50";
grid.FilterString = "[ShipCountry] In ('USA', 'Canada')";
grid.FilterString = "Contains([Customer], 'Inc')";
grid.FilterString = "";   // Clear
```

Field names in square brackets. Operators: `=`, `<>`, `<`, `>`, `<=`, `>=`, `Like`, `In`, `Between`, `IsNull`, `Contains`, `StartsWith`, `EndsWith`, `Not`, `And`, `Or`. Date literals in `#...#`. See https://docs.devexpress.com/content/CoreLibraries/4928?md=true (Criteria Language Syntax) and https://docs.devexpress.com/content/WPF/6499?md=true (Filter Expressions).

#### `FilterCriteria` — Strongly-Typed CriteriaOperator

```csharp
using DevExpress.Data.Filtering;

grid.FilterCriteria =
    new BinaryOperator("OrderDate", new DateTime(2024, 1, 1), BinaryOperatorType.GreaterOrEqual) &
    new BinaryOperator("Freight", 50, BinaryOperatorType.Greater);
```

`&` (AND) and `|` (OR) operators combine criteria. Use `BinaryOperator`, `InOperator`, `FunctionOperator`, `GroupOperator`, etc. — all in `DevExpress.Data.Filtering`.

#### Apply Without Resetting Other Column Filters

```csharp
grid.MergeColumnFilters("[Freight] > 100");
```

#### Fixed Filter (Cannot Be Modified by User)

```xaml
<dxg:GridControl FixedFilter="[Status] != 'Archived'"/>
```

Users can layer additional filters on top, but cannot remove the fixed criterion.

Source: `articles/controls-and-libraries/data-grid/filtering-and-searching/filtering-in-code.md`.

### Data Analysis Filters (Top/Bottom, Above/Below Average, Unique, Duplicate)

```csharp
grid.FilterString = "[#TopItems]([Profit], 10)";        // Top 10
grid.FilterString = "[#TopPercent]([Profit], 10)";      // Top 10%
grid.FilterString = "[#BottomItems]([Profit], 10)";     // Bottom 10
grid.FilterString = "[#BottomPercent]([Profit], 10)";   // Bottom 10%
grid.FilterString = "[#AboveAverage]([Profit])";
grid.FilterString = "[#BelowAverage]([Profit])";
grid.FilterString = "[#Unique]([Profit])";
grid.FilterString = "[#Duplicate]([Profit])";
```

Or via `FunctionOperator`:

```csharp
grid.FilterCriteria = new FunctionOperator(
    "#TopItems",
    new OperandProperty("Profit"),
    new OperandValue(10));
```

### Conditional Formatting Filters

When a conditional format is applied, filter rows that match (or don't match) the format rule:

```csharp
grid.FilterString = "[@TopItems]([Profit], 5)";   // Match the "Top 5" conditional format
```

(Note: `@` prefix for conditional formatting filters vs `#` for data-analysis filters.)

### `CustomRowFilter` — Per-Row Custom Filter

When the standard filter syntax can't express your rule, use a custom row filter. The grid invokes your handler for every row.

```xaml
<dxg:GridControl CustomRowFilterCommand="{Binding CustomRowFilterCommand}">
    ...
</dxg:GridControl>
```

```csharp
[Command]
public void CustomRowFilter(RowFilterArgs args) {
    var order = (Order)args.Item;

    // args.DefaultVisibility = current visibility after standard filters
    // Set args.Visible to override
    args.Visible = order.IsActive && order.Amount > GetThresholdFor(order.Customer);
}
```

| `RowFilterArgs` Property | Description |
|---|---|
| `Item` | The data row being processed |
| `SourceIndex` | Row's index in the data source |
| `DefaultVisibility` | Visibility after standard filters applied |
| `Visible` | Final visibility (your decision) |

The custom row filter **takes priority over** filter criteria from Drop-down Filter or Auto Filter Row. Use it for business rules that depend on multiple fields or external state.

Event-based alternative: handle `GridControl.CustomRowFilter` directly (non-MVVM).

> **Performance**: this runs for every row on every refresh. Keep the predicate fast.

Source: `articles/controls-and-libraries/data-grid/filtering-and-searching/filter-modes-and-custom-filtering.md`.

### `SubstituteFilter` — Replace the User's Filter Before Application

Modify the filter criterion the user produced (e.g., expand "USA" to include territories, or rewrite a regex match into a `Like` clause) before the grid applies it:

```xaml
<dxg:GridControl SubstituteFilter="grid_SubstituteFilter"/>
```

```csharp
void grid_SubstituteFilter(object sender, SubstituteFilterEventArgs e) {
    // e.Filter is the original criterion
    // Set e.Filter to a different CriteriaOperator to substitute

    if (e.Filter is BinaryOperator bo &&
        bo.OperatorType == BinaryOperatorType.Equal &&
        Equals(bo.RightOperand, new OperandValue("USA"))) {

        e.Filter = new InOperator(bo.LeftOperand,
            new[] { new OperandValue("USA"), new OperandValue("Canada"), new OperandValue("Mexico") });
    }
}
```

Use cases:
- Translate a user's selection in a filter UI into a richer underlying query
- Add a tenancy filter on top of every user filter ("WHERE TenantId = X AND (user filter)")
- Map a display-text filter to a different field

`SubstituteFilter` fires only when the user changes the filter via UI. To apply a global / always-on constraint, use `FixedFilter` instead.

Source: `articles/controls-and-libraries/data-grid/filtering-and-searching/filter-modes-and-custom-filtering.md`.

### Search

```xaml
<dxg:TableView ShowSearchPanelMode="Always"
               SearchString="{Binding SearchText, Mode=TwoWay}"/>
```

`ShowSearchPanelMode`: `Always`, `Default`, `Never`, `ShowOnFocus`, `OnHotKey` (default Ctrl+F).

`SearchString` filters across all visible columns. For AI-powered semantic search, attach a `SemanticSearchBehavior` (namespace `xmlns:dxai="http://schemas.devexpress.com/winfx/2008/xaml/ai"`) to the `GridControl`. This requires the `DevExpress.AIIntegration.Wpf` and `DevExpress.Wpf.Grid` packages:

```xaml
<dxg:GridControl ItemsSource="{Binding Cars}">
    <dxmvvm:Interaction.Behaviors>
        <dxai:SemanticSearchBehavior VectorCollectionName="Cars"
                                     DataSourceKeyField="Id"
                                     SearchMode="Semantic"
                                     SearchResultCount="10"/>
    </dxmvvm:Interaction.Behaviors>
    <dxg:GridControl.View>
        <dxg:TableView ShowSearchPanelMode="Always"/>
    </dxg:GridControl.View>
</dxg:GridControl>
```

At startup, register an embedding generator and a vector collection on the AI container — semantic search vectorizes the data, so it needs an embedding generator and vector store, not a chat client:

```csharp
var container = AIExtensionsContainerDesktop.Default;
// embeddingGenerator: an IEmbeddingGenerator (e.g., an Azure OpenAI embedding model)
var vectorCollection = new InMemoryCollection<string, VectorStoreRecord>(
    "Cars", new InMemoryCollectionOptions { EmbeddingGenerator = embeddingGenerator });
container.RegisterVectorCollection(vectorCollection);
```

> The Data Grid supports AI-powered Semantic Search via the `DevExpress.AIIntegration.Wpf` package. Unlike substring search, semantic search analyzes the query as natural language. Source: `articles/ai-powered-extensions/semantic-search.md`.

### Customize Search Behavior

The `DataViewBase.SearchStringToFilterCriteria` event lets you customize how the search string is converted to a filter criterion:

```csharp
void View_SearchStringToFilterCriteria(object sender, SearchStringToFilterCriteriaEventArgs e) {
    // e.SearchString is the user's input
    // e.Filter is the criteria the grid would apply by default
    // Replace e.Filter with your custom version

    if (e.SearchString.StartsWith("@")) {
        // Treat as exact match on Customer field
        e.Filter = new BinaryOperator("Customer",
            e.SearchString.Substring(1), BinaryOperatorType.Equal);
    }
}
```

### Clear Filters

```csharp
grid.FilterString = "";              // Clear grid-level filter
grid.FilterCriteria = null;          // Same effect
grid.ClearColumnFilter("OrderDate"); // Clear one column
```

### Filter Events and State

| API | Description |
|---|---|
| `DataControlBase.FilterChanged` | Fires after filter changes |
| `ColumnBase.IsFiltered` | `true` if this column has an active filter |
| `DataControlBase.GetColumnFilterCriteria(columnName)` | Get one column's criterion |
| `DataControlBase.GetColumnFilterString(columnName)` | String form |
| `DataControlBase.VisibleItems` | Items currently visible after all filters |

## Grouping

Users group two ways:

1. Drag a column header to the **Group Panel** (`ShowGroupPanel="True"` required).
2. Right-click a header → **Group By This Column**.

Both require `GridViewBase.AllowGrouping="True"`.

### Group from XAML at Startup

```xaml
<dxg:GridColumn FieldName="ShipCountry" GroupIndex="0"/>
<dxg:GridColumn FieldName="ShipCity"    GroupIndex="1"/>
```

`GroupIndex` `-1` = not grouped; `0` = outermost group; `1` = nested below 0; etc.

### Group Panel and Visibility

```xaml
<dxg:TableView ShowGroupPanel="True" AllowGrouping="True"/>
```

By default, columns used for grouping are hidden from the table area (only headers visible in the Group Panel). To keep visible:

```xaml
<dxg:TableView ShowGroupedColumns="True"/>
```

### Per-Column Grouping Permission

```xaml
<dxg:GridColumn FieldName="OrderId" AllowGrouping="False"/>
```

### Partial Grouping (Hide Single-Row Groups)

```xaml
<dxg:TableView AllowPartialGrouping="True"/>
```

Groups containing only one row are flattened — the row appears at the parent level without a group header. Not compatible with Instant Feedback UI mode.

### Programmatic Grouping

```csharp
column.GroupIndex = 0;        // Group by this column first
column.GroupIndex = -1;       // Un-group

foreach (var col in view.GroupedColumns)
    Debug.WriteLine($"Grouped by {col.FieldName}");
```

### Group Intervals (Date, Numeric, Alphabetical)

Group rows by **interval** instead of by raw value — e.g., group dates by month, prices by range, names by first letter:

```xaml
<dxg:GridColumn FieldName="OrderDate" GroupIndex="0" GroupInterval="DateMonth"/>
<dxg:GridColumn FieldName="UnitPrice" GroupIndex="1" GroupInterval="Value"/>
<dxg:GridColumn FieldName="Customer"  GroupIndex="2" GroupInterval="Alphabetical"/>
```

`ColumnGroupInterval` values:

| Value | Effect |
|---|---|
| `Value` (default) | Group by raw value |
| `DateYear` | Group dates by year |
| `DateMonth` | Group by month (e.g., "January 2024") |
| `DateQuarter` | Group by quarter |
| `DateDay` | Group by day |
| `DateRange` | Group by named range (Today, Yesterday, Last Week, Last Month, etc.) |
| `Alphabetical` | Group strings by first letter |

> Verify exact enum names against `apidoc/DevExpress.Xpf.Grid/ColumnGroupInterval/` if your version differs — the values listed are standard.

Multi-level date grouping (Year → Month):

```xaml
<dxg:GridColumn FieldName="OrderDate" GroupIndex="0" GroupInterval="DateYear"  SortFieldName="OrderDate"/>
<dxg:GridColumn FieldName="OrderDate" GroupIndex="1" GroupInterval="DateMonth" SortFieldName="OrderDate"/>
```

Source: `articles/controls-and-libraries/data-grid/grouping/group-modes-and-custom-grouping.md`.

### Merged Grouping

Combine multiple columns into one group row (e.g., group by City + Country in a single combined group):

```xaml
<dxg:TableView AllowMergedGrouping="True"
               MergedGroupingMode="CtrlKeyPressed"/>
<dxg:GridColumn FieldName="City"    GroupIndex="0"/>
<dxg:GridColumn FieldName="Visits"  GroupIndex="1" MergeWithPreviousGroup="True"/>
```

`MergedGroupingMode`:
- `CtrlKeyPressed` (default) — user holds Ctrl while dragging to merge
- `Always` — every drag merges with the previous group

### Custom Grouping (`CustomColumnGroup`)

When the standard grouping by value doesn't fit (e.g., group dates into custom buckets like "Recent / Older / Archived"):

```xaml
<dxg:GridColumn FieldName="OrderDate"
                GroupIndex="0"
                SortMode="Custom"/>
```

```csharp
[Command]
public void CustomColumnGroup(GroupArgs args) {
    if (args.FieldName != nameof(Order.OrderDate)) return;

    var d1 = (DateTime)args.FirstValue;
    var d2 = (DateTime)args.SecondValue;
    args.Result = Bucket(d1).CompareTo(Bucket(d2));
}

int Bucket(DateTime d) {
    if (d > DateTime.Today.AddDays(-30)) return 0;     // Recent
    if (d > DateTime.Today.AddYears(-1))  return 1;     // Older
    return 2;                                            // Archived
}
```

The grid first sorts using `CustomColumnSort`, then walks adjacent rows and uses `CustomColumnGroup` to decide whether two rows belong to the same group (`args.Result = 0` means same group). Both events / commands must be set, and the column's `SortMode` must be `Custom`.

### Custom Group Display Text (`CustomGroupDisplayText`)

Replace the default group caption ("OrderDate: 2024-01-15") with a custom string:

```xaml
<dxg:GridControl CustomGroupDisplayTextCommand="{Binding CustomGroupDisplayTextCommand}"/>
```

```csharp
[Command]
public void CustomGroupDisplayText(CustomGroupDisplayTextArgs args) {
    if (args.FieldName != nameof(Order.OrderDate)) return;
    var bucket = Bucket((DateTime)args.Value);
    args.DisplayText = bucket switch {
        0 => "Recent orders (last 30 days)",
        1 => "Older orders (last year)",
        2 => "Archived",
        _ => args.DisplayText
    };
}
```

Source: `articles/controls-and-libraries/data-grid/grouping/group-modes-and-custom-grouping.md`.

### Customize Group Row Appearance

| Property | Effect |
|---|---|
| `GridViewBase.GroupRowStyle` | Style applied to all group rows |
| `GridViewBase.GroupRowTemplate` | Full `DataTemplate` for the group row |
| `GridViewBase.GroupValueTemplate` / `GridColumn.GroupValueTemplate` | Template for the group value portion |

```xaml
<dxg:TableView.GroupRowStyle>
    <Style TargetType="dxg:GroupRowControl">
        <Setter Property="Background" Value="#FFE7F4FB"/>
        <Setter Property="FontWeight" Value="Bold"/>
    </Style>
</dxg:TableView.GroupRowStyle>
```

### Programmatic Expand / Collapse

| Method | Effect |
|---|---|
| `GridViewBase.ExpandGroup(int rowHandle)` | Expand a group |
| `GridViewBase.CollapseGroup(int rowHandle)` | Collapse a group |
| `GridViewBase.ExpandAllGroups()` | Expand all |
| `GridViewBase.CollapseAllGroups()` | Collapse all |

> Verify exact method signatures against `apidoc/DevExpress.Xpf.Grid/GridViewBase/` if needed.

### Grouping Requires Sorting

> The grid sorts data against group column values. Users can only group data if data sorting is allowed (`DataViewBase.AllowSorting`). If a grouping column isn't sorted, the grid auto-applies ascending sort.

## Combining Sort + Filter + Group

The order of operations:
1. **Custom row filter** (if any) — fires per row
2. **Standard filter** — applies `FilterString` / `FilterCriteria`
3. **Sort** — `SortOrder` / `SortIndex` / `CustomColumnSort`
4. **Group** — `GroupIndex` / `CustomColumnGroup`
5. **Summaries** — `TotalSummary` / `GroupSummary` aggregations

The user-facing layout reflects all five steps. `DataControlBase.VisibleItems` returns the final result.

## Server Mode and Virtual Source Caveats

The following do NOT work in server mode / virtual sources (the server does sort and filter):

| Feature | Server Mode | Virtual Sources |
|---|---|---|
| `ColumnFilterMode = DisplayText` | ❌ | ❌ |
| `SortMode = DisplayText` | ❌ | Depends on source |
| `SortMode = Custom` | ❌ | Depends on source |
| `ColumnInputFilter` (display-text filter) | ❌ | Depends on source |
| `CustomRowFilter` | ❌ (filter happens on the server) | ❌ |
| `SubstituteFilter` | ✅ | ✅ |
| Standard `FilterString` / `FilterCriteria` | ✅ | ✅ |
| `Custom Sort` of group rows by summary | ❌ | ❌ |

Source: `articles/controls-and-libraries/data-grid/bind-to-data/server-mode-and-instant-feedback/server-mode-limitations.md`.

## Apply to TreeListControl

| GridControl member | TreeListControl equivalent |
|---|---|
| `GridControl.CustomColumnSort` event / `CustomColumnSortCommand` | `TreeListView.CustomColumnSort` / `CustomColumnSortCommand` |
| `GridControl.CustomColumnGroup` event / `CustomColumnGroupCommand` | `TreeListView.CustomColumnGroup` / `CustomColumnGroupCommand` |
| `GridControl.CustomGroupDisplayText` / `Command` | `TreeListView.CustomGroupDisplayText` / `Command` |
| `GridControl.CustomRowFilter` / `CustomRowFilterCommand` | **`TreeListView.CustomNodeFilter` / `CustomNodeFilterCommand`** |
| `GridControl.SubstituteFilter` event | `TreeListView.SubstituteFilter` event |
| `DataControlBase.FilterString` / `FilterCriteria` | same |
| `DataViewBase.SearchStringToFilterCriteria` event | same |

TreeListControl supports filtering with the same Criteria Language Syntax, plus tree-aware modes (see `articles/controls-and-libraries/data-grid/grid-view-data-layout/nodes/filter-nodes.md`).

## Common Issues

- **Custom sort doesn't fire** — `ColumnBase.SortMode` is not set to `Custom`. Setting just the command isn't enough.
- **Custom row filter doesn't run** — bound the command but didn't apply any standard filter that would refresh the view. Set `FilterCriteria` to anything (even null after assigning) to force refresh. Or call `RefreshData()`.
- **DisplayText sort returns alphabetic order on numbers** — that's the intended behavior (display text is a string). Switch to `Value` if you want numeric ordering.
- **Custom group caption shows raw value** — the `CustomGroupDisplayText` handler isn't matching the right `FieldName`, or fires before `CustomColumnGroup` has decided the bucket. Check both events have the column's `SortMode = Custom`.
- **`SubstituteFilter` runs in a tight loop** — your handler reassigns `e.Filter` to a new criteria that the grid then passes back through `SubstituteFilter`. Guard with a flag or compare structurally.

## Source Material

- `articles/controls-and-libraries/data-grid/sorting.md`
- `articles/controls-and-libraries/data-grid/sorting/sorting-modes-and-custom-sorting.md`
- `articles/controls-and-libraries/data-grid/sorting/sorting-in-code.md`
- `articles/controls-and-libraries/data-grid/sorting/sorting-group-rows-by-summary-values.md`
- `articles/controls-and-libraries/data-grid/filtering-and-searching.md`
- `articles/controls-and-libraries/data-grid/filtering-and-searching/filter-modes-and-custom-filtering.md`
- `articles/controls-and-libraries/data-grid/filtering-and-searching/filtering-in-code.md`
- `articles/controls-and-libraries/data-grid/filtering-and-searching/filter-dropdown/` (Regular, Checked, Excel-style, Date-Time, Custom)
- `articles/controls-and-libraries/data-grid/filtering-and-searching/automatic-filter-row.md`
- `articles/controls-and-libraries/data-grid/filtering-and-searching/filter-editor.md`
- `articles/controls-and-libraries/data-grid/filtering-and-searching/data-analysis-filters.md`
- `articles/controls-and-libraries/data-grid/filtering-and-searching/conditional-formatting-filters.md`
- `articles/controls-and-libraries/data-grid/filtering-and-searching/predefined-filters.md`
- `articles/controls-and-libraries/data-grid/grouping.md`
- `articles/controls-and-libraries/data-grid/grouping/group-modes-and-custom-grouping.md`
- `articles/controls-and-libraries/data-grid/grouping/grouping-in-code.md`
- `articles/controls-and-libraries/data-grid/grouping/processing-group-rows.md`
- `articles/controls-and-libraries/data-grid/grouping/expanding-and-collapsing-group-rows.md`
- `templates/ai-semantic-search-tip.md` (embedded include)

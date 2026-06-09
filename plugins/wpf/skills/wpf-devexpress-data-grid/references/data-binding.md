# Data Binding — DevExpress WPF Data Grid

The Data Grid binds via the `ItemsSource` property (inherited from `DataControlBase`). Choosing the **right source type** is the most important decision: it determines what features work, how the grid handles 1K vs 1M rows, and where data shaping happens (client vs server). This reference is a decision matrix plus details and limits for each source type. Same APIs apply to `TreeListControl` — but with notable exceptions (see § "TreeListControl Specifics").

## When to Use This Reference

Use this when you need to:

- Pick the right source type for your data size and use case
- Understand each source type's limits (what features don't work)
- Use `ChunkList<T>` for large in-memory data with frequent batch updates
- Bind to Entity Framework Core, XPO, OData, LINQ to SQL, or WCF
- Use Server Mode for very large data
- Use Virtual Sources for REST/NoSQL/custom paging APIs
- Use Instant Feedback Mode for async loading
- Use `RealTimeSource` for high-frequency data updates
- Tune frequent-update performance (`BeginDataUpdate`/`EndDataUpdate`, `OptimizeSummaryCalculation`)
- Use `ICollectionView`, `DataTable`, or dynamic data
- Use the Items Source Wizard at design time

## Decision Matrix — Pick the Right Source

| Data shape & size | Recommended source | Why |
|---|---|---|
| Up to ~10K rows, in-memory POCOs | `List<T>` or `ObservableCollection<T>` | Simplest. All features work client-side. |
| 10K–100K rows with frequent **batch** updates | **`ChunkList<T>`** | Faster batch insertions than `List<T>` / `ObservableCollection<T>`. |
| 100K–1M rows in-memory | `List<T>` + `OptimizeSummaryCalculation` + `BeginDataUpdate` patterns | Standard collection with performance tuning. |
| 1M–100M rows in EF / SQL backend, sort / filter / group on server | **Server Mode** (`EntityServerModeSource`, `LinqServerModeSource`) | Server does aggregation. Grid only fetches visible window. |
| Same as above + want UI responsiveness during loads | **Instant Feedback Mode** (`EntityInstantFeedbackSource`) | Background-thread fetch. UI stays interactive. |
| Cursor / page API (REST, NoSQL, custom WCF, OData), unknown total count | **Virtual Sources** (`InfiniteAsyncSource`, `PagedAsyncSource`) | Doesn't require total count; explicit per-feature opt-in. |
| Data already modeled as Microsoft Analysis Services cube | **OLAP source** | Pre-aggregated dimensions / measures. |
| Real-time updates (every ms / sec) | **`RealTimeSource`** | Buffers updates, dispatches on UI thread. |
| Standard WPF `ICollectionView` semantics needed | `ICollectionView` | Limited (treats columns as unbound; restrictions apply). |
| Dynamic schema (no compile-time type) | `DataTable` / `ExpandoObject` / `UnboundDataSource` | Use `ColumnBase.Binding` instead of `FieldName`. |

> **TreeListControl: Server Mode and Virtual Sources are NOT supported.** TreeListControl works with in-memory data only (`List<T>`, `ObservableCollection<T>`, `ChunkList<T>`, hierarchical or self-referential).

Source: `articles/controls-and-libraries/data-grid/bind-to-data.md` (root) and subarticles.

## Local Collection (`List<T>`, `ObservableCollection<T>`)

Simplest binding. All features work — sort, filter, group, summaries, CRUD, conditional formatting, virtualization. Best up to ~100K rows; above that, consider Server Mode or `ChunkList<T>`.

```xaml
<dxg:GridControl ItemsSource="{Binding Orders}"
                 AutoGenerateColumns="AddNew"/>
```

```csharp
public ObservableCollection<Order> Orders { get; } = new();
```

`ObservableCollection<T>` triggers live updates when items added / removed. `List<T>` doesn't — call `grid.RefreshData()` after changes, or set `AllowLiveDataShaping="True"` for sort / filter / summary refresh on item-property changes.

## `ChunkList<T>` — Large Data With Frequent Batch Updates

`DevExpress.Xpf.ChunkList.ChunkList<T>` is a specialized collection for cases where ordinary `List<T>` / `ObservableCollection<T>` become slow on batch operations.

### When to Use

- Data source contains **at least 10,000 elements**.
- Each update operation affects a **batch of elements** at once (insert / remove many).

### When NOT to Use

- Many operations iterate over items (summary calculation, sort, filter) — `ChunkList` may be slower than `List<T>` for these.
- Small collections (< 10K).

```csharp
using DevExpress.Xpf.ChunkList;

public ChunkList<Order> Orders { get; } = new();

void LoadBatch(IEnumerable<Order> newOrders) {
    Orders.AddRange(newOrders);   // Batch-friendly
}
```

`ChunkList<T>` implements `IList<T>` and `INotifyCollectionChanged` — assignable to `ItemsSource` directly.

Source: `articles/controls-and-libraries/data-grid/performance-improvement/frequent-data-updates.md` § ChunkList.

## Entity Framework Core

```csharp
using DevExpress.Mvvm;
using Microsoft.EntityFrameworkCore;

public class MainViewModel : ViewModelBase {
    NorthwindContext _db;

    public ICollection<Order> Orders {
        get => GetValue<ICollection<Order>>();
        private set => SetValue(value);
    }

    public MainViewModel() {
        _db = new NorthwindContext();
        _db.Orders.Load();
        Orders = _db.Orders.Local;   // Observable change-tracker view
    }
}
```

`DbSet<T>.Local` exposes the local change-tracker as an observable collection — edits in the grid show immediately, and the DbContext tracks changes for `SaveChanges()`.

For larger sets, prefer `EntityServerModeSource` (Server Mode) over loading all rows into memory.

Source: `articles/controls-and-libraries/data-grid/bind-to-data/bind-to-entity-framework-core-sources.md`.

## CRUD with the MVVM Items Source Wizard Pattern

When you use the Items Source Wizard (Quick Actions → Bind to a Data Source), it generates this MVVM scaffolding:

```xaml
<dxg:GridControl ItemsSource="{Binding ItemsSource}"
                 RestoreStateKeyFieldName="OrderId"
                 RestoreStateOnSourceChange="True">
    <dxg:GridControl.View>
        <dxg:TableView NewItemRowPosition="Top"
                       ShowUpdateRowButtons="OnCellEditorOpen"
                       ValidateRowCommand="{Binding ValidateRowCommand}"
                       ValidateRowDeletionCommand="{Binding ValidateRowDeletionCommand}"
                       DataSourceRefreshCommand="{Binding DataSourceRefreshCommand}"/>
    </dxg:GridControl.View>
</dxg:GridControl>
```

```csharp
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.Mvvm.Xpf;

public class MainViewModel : ViewModelBase {
    [Command]
    public void ValidateRow(RowValidationArgs args) {
        var order = (Order)args.Item;
        if (args.IsNewItem) _context.Orders.Add(order);
        _context.SaveChanges();
    }

    [Command]
    public void ValidateRowDeletion(ValidateRowDeletionArgs args) {
        _context.Orders.Remove((Order)args.Items.Single());
        _context.SaveChanges();
    }

    [Command]
    public void DataSourceRefresh(DataSourceRefreshArgs args) {
        // Re-fetch from DB; rebind ItemsSource
    }
}
```

See [validation.md](validation.md) for richer validation flows.

## Server Mode (Very Large Data)

When the data source has 100K+ rows and you want **server-side aggregation** (sort / filter / group / summaries run on the database), use a server-mode source. The grid pulls only the rows currently visible in its viewport.

| Data Access Technology | Server Mode Source | Instant Feedback (background-thread) |
|---|---|---|
| Entity Framework 4+ / Core | `DevExpress.Data.Linq.EntityServerModeSource` | `DevExpress.Data.Linq.EntityInstantFeedbackSource` |
| LINQ to SQL | `DevExpress.Data.Linq.LinqServerModeSource` | `DevExpress.Data.Linq.LinqInstantFeedbackSource` |
| eXpress Persistent Objects | `DevExpress.Xpo.XPServerCollectionSource` | `DevExpress.Xpo.XPInstantFeedbackSource` |
| OData v4 | `DevExpress.Data.ODataLinq.ODataServerModeSource` | `DevExpress.Data.ODataLinq.ODataInstantFeedbackSource` |
| WCF Data Services | `DevExpress.Data.WcfLinq.WcfServerModeSource` | `DevExpress.Data.WcfLinq.WcfInstantFeedbackSource` |
| Parallel LINQ to Objects | `DevExpress.Data.PLinq.PLinqServerModeSource` | `DevExpress.Data.PLinq.PLinqInstantFeedbackSource` |

```csharp
using DevExpress.Data.Linq;

var ctx = new NorthwindContext();
pivotGridControl1.DataSource = new EntityServerModeSource {
    ElementType = typeof(Order),
    QueryableSource = ctx.Orders,
    KeyExpression = "OrderId",
};
```

> **Instant Feedback Mode** loads data asynchronously on a background thread — UI stays responsive while data fetches. Use it when the underlying query is slow.

### Server Mode Limitations

The following do NOT work in Server Mode / Instant Feedback (because the server does the work):

**Not supported in `TreeListView`** — Server Mode sources do NOT work with `TreeListView`. Use Table View.

**Data Editing**:
- Add / remove records is disabled when data is grouped.

**Sorting** (NOT supported):
- Sort by `DisplayText` (`ColumnFilterMode.DisplayText`).
- Custom sort via `CustomColumnSort` event.
- Sort for unbound columns populated by `CustomUnboundColumnData` event (use `ColumnBase.UnboundExpression` instead — that does work).
- Sort for columns bound via `ColumnBase.Binding`.

**Grouping** (NOT supported):
- `ColumnGroupInterval.Alphabetical` and `DisplayText`.
- Custom grouping via `CustomColumnGroup` event.
- Merged grouping (except `XPServerCollectionSource`).
- Group on unbound columns populated by events.
- Group on `Binding`-property columns.

**Filtering and Searching** (NOT supported):
- Filter by display text.
- Custom row filter via `CustomRowFilter`.
- Search Panel filtering on non-string columns.
- Incremental Search.
- `WcfServerModeDataSource` / `WcfInstantFeedbackDataSource` / `RiaInstantFeedbackDataSource` don't support `Like`, `Not Like`, or special DateTime conditions (Yesterday, Tomorrow, Last Week).
- Search Panel converts the query to lowercase — case-sensitive backends miss upper-case matches. Use case-insensitive collations.
- Filter for unbound columns populated by events.
- Filter for `Binding`-property columns.

**Selection**:
- Row selection is NOT preserved when data is sorted, grouped, or filtered.

**Summary Calculation** (NOT supported):
- Custom summaries via `CustomSummary` event (also `CustomSummaryExists` is ignored).
- Summary for unbound columns populated by events.
- Total summaries for unbound columns populated by events (unbound expression columns DO work).

**Conditional Formatting**:
- Top/Bottom rules (Top10Items, Top10Percent, BottomItems, BelowAverage, etc.) NOT supported.
- Data values rounding NOT supported.

**Other**:
- Master-Detail NOT supported in master OR detail grids.
- Fixed Rows NOT supported.
- `GetRowHandleByListIndex` returns incorrect values.
- Unbound column printing NOT supported.
- Maximum 10K visible data groups.
- Compound keys only via Linq / Entity / XPO providers.

### Performance Pitfalls (Server Mode)

- `GridControl.AutoExpandAllGroups` and `IsRecursiveExpand` issue a separate SQL query **per group**. With many groups, "Expand All" causes a query storm.
- Each expand operation = one server round trip. Tune indexes accordingly.

Source: `articles/controls-and-libraries/data-grid/bind-to-data/server-mode-and-instant-feedback/server-mode-limitations.md`.

## Virtual Sources (REST / NoSQL / Custom APIs)

Use Virtual Sources when:

- The data source exposes a **page-based** or **skip/take** API
- The total record count is **unknown** or expensive to compute
- You want explicit control over which operations (sort / filter / summary) are sent to the server

| Source Type | Behavior |
|---|---|
| `DevExpress.Xpf.Data.InfiniteAsyncSource` | Infinite scrolling (no pages); fetches portions on demand |
| `DevExpress.Xpf.Data.InfiniteSource` | Same as above but synchronous |
| `DevExpress.Xpf.Data.PagedAsyncSource` | Page-based (page navigation buttons) |
| `DevExpress.Xpf.Data.PagedSource` | Page-based, synchronous |
| `DevExpress.Xpf.Data.FetchRowsAsyncSource` | Custom fetch with skip tokens — for cursor-based APIs |

### Minimum Requirement

Your backend must expose:
- A way to **fetch a portion** of data (skip + take, page index, or cursor token)

Optional:
- Sort, filter, summary — each is independently opt-in.

### Example (InfiniteAsyncSource)

```csharp
using DevExpress.Xpf.Data;

var source = new InfiniteAsyncSource {
    ElementType = typeof(Question),
};

source.FetchRows += (sender, e) => {
    e.Result = Task.Run(() => FetchFromApi(e.Skip, 20, e.SortOrder, e.Filter));
};

grid.ItemsSource = source;
```

```csharp
async Task<FetchRowsResult> FetchFromApi(int skip, int take, IList<SortOrder> sort, CriteriaOperator filter) {
    var items = await stackExchangeApi.GetQuestions(skip, take, sort, filter);
    bool hasMore = items.Count == take;
    return new FetchRowsResult(items.ToArray(), hasMore);
}
```

### Async Lifecycle Events

`InfiniteAsyncSource` / `PagedAsyncSource` raise events on the UI thread; you assign `Task<T>` to `e.Result` to compute on a background thread:

| Event | Purpose |
|---|---|
| `GetTotalSummaries` | (Optional) compute aggregate summaries |
| `FetchRows` (Infinite) / `FetchPage` (Paged) | Fetch a data portion |
| `GetUniqueValues` | (Optional) populate drop-down filter values |

### Virtual Source Limitations

**`TreeListView` NOT supported** with any virtual source.

**By default, the following are DISABLED** — you must opt in explicitly via `FetchEventArgsBase` properties:
- Sorting (use `FetchEventArgsBase.SortOrder` in the fetch callback)
- Filtering (use `FetchEventArgsBase.Filter`)
- Total Summaries (handle `GetTotalSummaries` event)
- Searching (use `SearchString` + `Filter`)

**Per-column control** of allowed operations:
- `ColumnBase.AllowSorting` (per column, not view-level)
- `ColumnBase.AllowedUnaryFilters`, `AllowedBinaryFilters`, `AllowedAnyOfFilters`, `AllowedDateTimeFilters`, `AllowedBetweenFilters`
- `DataViewBase.AllowedGroupFilters`
- `ColumnBase.AllowedTotalSummaries`

**Single-column-only restrictions** (set `AllowGroupingSortingBySingleColumnOnly="False"` to override):
- Sort by single column only
- Group by single column only

**Disabled features**:
- **Grouping** is not supported (unsupported, period)
- Master-Detail
- Scrollbar Annotations
- Incremental Search
- Fixed Rows
- Search text highlighting (would force loading many rows)

**Editing**: only **Edit Entire Row** mode is supported (set `ShowUpdateRowButtons`). Validate via `GridViewBase.ValidateRow` with `e.UpdateRowResult = Task.Run(...)` for async save.

**Unbound columns**:
- Only via `ColumnBase.UnboundExpression` (expression-based) work for sort / filter / summary.
- Event-driven unbound columns do NOT participate in shaping.

**Conditional Formatting**: only `FormatCondition` rules supported.

**Printing / Exporting**:
- `PagedAsyncSource` / `PagedSource`: print / export NOT supported.
- `InfiniteAsyncSource` / `InfiniteSource`: prints / exports only **loaded** rows.

**Selection**:
- `InfiniteAsyncSource` / `InfiniteSource`: `SelectAll` is ignored; Web-style row selection (checkbox) doesn't work.
- `PagedAsyncSource` / `PagedSource`: `SelectAll` selects only the current page.

**Drag-and-Drop**: disabled by default; use custom drag-drop events.

**Best Fit**:
- `InfiniteAsyncSource` / `InfiniteSource`: `BestFitMode.AllRows` works only for loaded rows.
- `PagedAsyncSource` / `PagedSource`: works only for current page rows.

**Data Navigator**: Add / Edit / Remove buttons disabled (read-only navigation only).

Source: `articles/controls-and-libraries/data-grid/bind-to-data/bind-to-any-data-source-with-virtual-sources/virtual-source-limitations.md`.

## OLAP (Microsoft Analysis Services)

Used by `PivotGridControl` more commonly than `GridControl`. For `GridControl`, prefer flattening OLAP into rows or use one of the relational server-mode sources.

## `RealTimeSource` — High-Frequency Updates

For data that updates at high frequency (every millisecond / second), use `DevExpress.Xpf.Core.DataSources.RealTimeSource`. It buffers updates and dispatches them to the UI on the dispatcher thread.

```csharp
using DevExpress.Xpf.Core.DataSources;

public RealTimeSource Source { get; } = new RealTimeSource {
    Source = streamingObservableCollection,
};
```

```xaml
<dxg:GridControl ItemsSource="{Binding Source}"/>
```

Pattern for ticker / quote streams, IoT telemetry, log aggregations.

Source: `articles/controls-and-libraries/data-grid/bind-to-data.md` § "Frequently Updated Sources".

## Frequent Update Performance

Three patterns for taming high-frequency updates:

### 1. `BeginDataUpdate` / `EndDataUpdate` (Batch Lock)

```csharp
grid.BeginDataUpdate();
try {
    foreach (var item in batch)
        underlyingList.Add(item);
} finally {
    grid.EndDataUpdate();
}
```

Suppresses UI updates between calls. Best for data sources < 100K rows with multiple updates per batch. For `TreeListView`:

```csharp
view.BeginDataUpdate(recreateNodesOnEndDataUpdateOnly: true);
// ... bulk changes
view.EndDataUpdate();
```

The `recreateNodesOnEndDataUpdateOnly` parameter (TreeListView only) batches node hierarchy recalculation.

### 2. `OptimizeSummaryCalculation`

```xaml
<dxg:GridControl OptimizeSummaryCalculation="True"/>
```

Requirements:
- Source is `ObservableCollection<T>` or `ChunkList<T>` whose items implement `INotifyPropertyChanged` AND `INotifyPropertyChanging`.
- No unbound columns in summaries.
- No custom summaries.
- No `BeginDataUpdate` lock for batched updates.

Performance gain: summary calculation cost becomes O(changed records) instead of O(all records).

### 3. Manual Refresh (`AllowLiveDataShaping=False` + `RefreshData()`)

```xaml
<dxg:GridControl AllowLiveDataShaping="False"/>
```

```csharp
grid.RefreshData();   // Refresh on demand
```

Use when data changes but you only want UI updates on user trigger. Best up to 100K rows.

### 4. Automatic Refresh on Timer (Custom Pattern)

For data updating every millisecond: implement a custom collection that blocks notifications, copies data on a timer, then notifies. See GitHub example linked from `frequent-data-updates.md`.

Source: `articles/controls-and-libraries/data-grid/performance-improvement/frequent-data-updates.md`.

## `ICollectionView`

Standard WPF `ICollectionView` works as an `ItemsSource`, but with restrictions:

- Columns behave like **unbound columns** internally (limitations apply).
- The Items Source Wizard does NOT generate code for `ICollectionView`.
- Cannot be used in master and detail grids simultaneously.

```csharp
var view = CollectionViewSource.GetDefaultView(collection);
view.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Ascending));
grid.ItemsSource = view;
```

The grid honors `ICollectionView.Filter`, `SortDescriptions`, `GroupDescriptions`.

Source: `articles/controls-and-libraries/data-grid/bind-to-data/bind-to-icollectionview.md`.

## `DataTable`

```csharp
DataTable table = LoadFromDb();
grid.ItemsSource = table.DefaultView;
```

Works for dynamic schemas. `ColumnBase.FieldName` = column name in the `DataTable`. Sort / filter / group work; some advanced features have caveats — see the article.

Source: `articles/controls-and-libraries/data-grid/bind-to-data.md` § "Custom (Unstructured) Data".

## Dynamic Data (No Compile-Time Schema)

For data structures that change at runtime (e.g., `ExpandoObject` collections), use `ColumnBase.Binding` instead of `FieldName`:

```xaml
<dxg:GridColumn Binding="{Binding ProductName, Mode=TwoWay}"/>
```

`Binding`-bound columns work with `ExpandoObject` and other dynamic types. See [columns.md](columns.md) § "`FieldName` vs `Binding` vs Unbound" for the decision matrix.

The `UnboundDataSource` component is designed for cases where no strongly-typed data set is available at compile time.

Source: `articles/controls-and-libraries/data-grid/bind-to-data.md` § "Dynamic Data".

## TreeListControl Specifics

`TreeListControl` shares most binding patterns with `GridControl` BUT:

- **Server Mode sources NOT supported** — `EntityServerModeSource`, `LinqServerModeSource`, etc. do not work with `TreeListView`. Use in-memory data.
- **Virtual Sources NOT supported** — `InfiniteAsyncSource`, `PagedAsyncSource`, etc. do not work.
- **Hierarchy modes specific to TreeList**: `KeyFieldName`/`ParentFieldName` (self-referential), `ChildNodesPath` (nested), `ChildNodesSelector` (type-aware), `HierarchicalDataTemplate`, **Unbound Mode** (TreeListNode tree).

For TreeListControl-specific binding details, see the `wpf-devexpress-tree-list` skill's `references/data-binding.md` and `nodes.md`.

## Source Type Decision Tree

```text
                    Start: how big is your data?
                              |
            ┌─────────────────┼─────────────────┐
            v                                   v
       < 10K rows                           > 1M rows
            |                                   |
            v                          ┌────────┼────────┐
   ┌────────┴────────┐                 v        v        v
   v                 v               EF/SQL  REST/  OLAP cube
List<T>     ObservableCollection<T>          NoSQL
   |                 |                    |     |      |
 fastest          live-update         Server   Virtual  OLAP
                                       Mode    Source   src
                                          |     |
                                          v     v
                                  EntityServerModeSource
                                  InfiniteAsyncSource etc.
            ───────────────────────────────────────
                       Special cases:
            • 10K–100K with frequent batches → ChunkList<T>
            • Streaming / real-time           → RealTimeSource
            • Standard WPF ICollectionView    → ICollectionView
            • Dynamic schema                  → DataTable / ColumnBase.Binding
```

## Common Issues

- **Grid is empty** — `ItemsSource` is null or the bound property doesn't fire `PropertyChanged`. Check ViewModel constructor wiring.
- **Edits don't persist** — wire `ValidateRowCommand` (MVVM) or call `SaveChanges()` from `CellValueChanged`.
- **Sort / filter doesn't refresh after data update** — set `AllowLiveDataShaping="True"`.
- **Performance bad on 100K+ rows** — switch to Server Mode, or use `ChunkList<T>` + `OptimizeSummaryCalculation`.
- **Server Mode + custom sort doesn't work** — known limitation; use `ColumnBase.UnboundExpression` instead.
- **Virtual Source: filter UI is invisible** — by default disabled. Set `ColumnBase.AllowedBinaryFilters` etc. to opt in.
- **Server Mode + master-detail** — not supported. Use in-memory data for master-detail layouts.
- **TreeListView + Server Mode** — not supported. Use Table View or in-memory data.

## Source Material

- `articles/controls-and-libraries/data-grid/bind-to-data.md` (root)
- `articles/controls-and-libraries/data-grid/bind-to-data/bind-to-a-local-collection.md`
- `articles/controls-and-libraries/data-grid/bind-to-data/bind-to-entity-framework-core-sources.md`
- `articles/controls-and-libraries/data-grid/bind-to-data/bind-to-entity-framework-sources.md`
- `articles/controls-and-libraries/data-grid/bind-to-data/bind-to-icollectionview.md`
- `articles/controls-and-libraries/data-grid/bind-to-data/bind-to-virtual-source.md`
- `articles/controls-and-libraries/data-grid/bind-to-data/server-data-and-large-data-sources.md`
- `articles/controls-and-libraries/data-grid/bind-to-data/server-mode-and-instant-feedback-mode.md`
- `articles/controls-and-libraries/data-grid/bind-to-data/server-mode-and-instant-feedback/server-mode-limitations.md`
- `articles/controls-and-libraries/data-grid/bind-to-data/server-mode-and-instant-feedback/bind-to-in-memory-data-using-plinq.md`
- `articles/controls-and-libraries/data-grid/bind-to-data/server-mode-and-instant-feedback/bind-to-xpo-data-sources.md`
- `articles/controls-and-libraries/data-grid/bind-to-data/bind-to-any-data-source-with-virtual-sources.md`
- `articles/controls-and-libraries/data-grid/bind-to-data/bind-to-any-data-source-with-virtual-sources/virtual-source-limitations.md`
- `articles/controls-and-libraries/data-grid/bind-to-data/bind-to-any-data-source-with-virtual-sources/bind-to-iqueryable.md`
- `articles/controls-and-libraries/data-grid/bind-to-data/bind-to-any-data-source-with-virtual-sources/use-virtual-sources.md`
- `articles/controls-and-libraries/data-grid/performance-improvement/frequent-data-updates.md`

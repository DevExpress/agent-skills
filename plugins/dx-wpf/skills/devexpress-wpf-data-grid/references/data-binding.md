# Data Binding — DevExpress WPF Data Grid

The Data Grid binds via the `ItemsSource` property (inherited from `DataControlBase`). Choosing the **right source type** is the most important decision: it determines what features work, how the grid handles 1K vs 1M rows, and where data shaping happens (client vs server).

This reference holds the *decision* — the matrix and tree that pick a source type for a given data shape/size — as authored content, since that judgment call is exactly what an agent using this reference needs synthesized. It does **not** hold a copy of each source type's code samples, exact class members, or full limitations tables, and it does **not** link directly to the DevExpress documentation pages that hold them: a hardcoded URL is exactly what breaks when DevExpress reorganizes a doc page. Instead, every reference below is a **search query** — verified (2026-08-27) to surface the intended page as its top or a clearly-correct hit via `devexpress_docs_search(technologies=["WPF"], question="<query>")` — paired with the page's title so a human or agent can recognize whether the fetch found the right thing. Fetch the page with `devexpress_docs_get_content` on the top hit's URL, not a URL typed from memory: that tool's own contract requires the URL come from a prior search.

Same APIs apply to `TreeListControl` — but with notable exceptions (see § "TreeListControl Specifics").

## When to Use This Reference

Use this when you need to:

- Pick the right source type for your data size and use case
- Find the search query that surfaces the canonical DevExpress documentation page for whichever source type you've picked, to pull exact code, namespaces, and NuGet package names from
- Understand which grid features are unsupported for a given source type (via that source's Limitations page)
- Get the authored technology → class mapping for Server Mode / Instant Feedback (EF Core, EF6, XPO, OData, LINQ to SQL, PLINQ)

> For an agent-driven advisor that inspects your ViewModel/DbContext/service code and picks a source automatically instead of you working through the matrix below by hand — and that fetches each source's exact implementation details via a documentation sub-agent rather than a bundled copy — see the [`devexpress-wpf-grid-datasource`](../../devexpress-wpf-grid-datasource/SKILL.md) skill.

## Decision Matrix — Pick the Right Source

| Data shape & size | Recommended source | Why | Doc Search Query → Expected Page |
|---|---|---|---|
| Up to ~10K rows, in-memory POCOs | `List<T>` or `ObservableCollection<T>` | Simplest. All features work client-side. | `"bind grid to a local collection ObservableCollection List"` → *Bind to a Local Collection* |
| 10K–100K rows with frequent **batch** updates | **`ChunkList<T>`** | Faster batch insertions than `List<T>` / `ObservableCollection<T>`. | `"ChunkList large data frequent updates performance"` → *ChunkList\<T\> Class* |
| 100K–1M rows in-memory | `List<T>` + `OptimizeSummaryCalculation` + `BeginDataUpdate` patterns | Standard collection with performance tuning. | `"frequent data updates performance BeginDataUpdate OptimizeSummaryCalculation ChunkList"` → *Frequent Data Updates* |
| 1M–100M rows in EF / SQL backend, sort / filter / group on server | **Server Mode** (`EntityServerModeSource`, `LinqServerModeSource`, …) | Server does aggregation. Grid only fetches visible window. | `"Server Mode and Instant Feedback Mode overview for GridControl large data"` → *Server Mode and Instant Feedback Mode* |
| Same as above + want UI responsiveness during loads | **Instant Feedback Mode** (`EntityInstantFeedbackSource`, …) | Background-thread fetch. UI stays interactive. | Same query as above — Instant Feedback is a section (§ Instant Feedback Mode) on the same page |
| Cursor / page API (REST, NoSQL, custom WCF, OData), unknown total count | **Virtual Sources** (`InfiniteAsyncSource`, `PagedAsyncSource`) | Doesn't require total count; explicit per-feature opt-in. | `"bind grid to any data source with virtual sources REST API paging"` → *Bind to any Data Source with Virtual Sources* |
| Data already modeled as Microsoft Analysis Services cube | No direct `GridControl` OLAP source | Flatten into rows, or use one of the relational Server Mode sources instead — `PivotGridControl` is the OLAP-native control. | `"server data and large data sources GridControl choosing binding mode"` → *Server Data and Large Data Sources* |
| Real-time updates (every ms / sec) | **`RealTimeSource`** | Buffers updates, dispatches on UI thread. | `"RealTimeSource frequently updated data source"` → *RealTimeSource* |
| Standard WPF `ICollectionView` semantics needed | `ICollectionView` | Limited (treats columns as unbound; runs the grid in Server Mode-equivalent restrictions unless `AllowCollectionView="False"`). | `"bind grid to ICollectionView"` → *Bind to ICollectionView* |
| Dynamic schema (no compile-time type) | `DataTable` / `ExpandoObject` / `UnboundDataSource` | Use `ColumnBase.Binding` instead of `FieldName`. | `"UnboundDataSource dynamic data no strongly typed data set WPF"` → *UnboundDataSource* |

> **TreeListControl: Server Mode and Virtual Sources are NOT supported.** TreeListControl works with in-memory data only (`List<T>`, `ObservableCollection<T>`, `ChunkList<T>`, hierarchical or self-referential). See § "TreeListControl Specifics".

Root topic query: `"WPF Data Grid bind to data overview all source types"` → *WPF Data Grid: Bind to Data* (links every source-specific article; a good starting point if none of the more specific queries above fit).

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

## Local Collection (`List<T>`, `ObservableCollection<T>`)

Simplest binding — assign to `ItemsSource` directly, all features work client-side. Best up to ~100K rows; above that, consider Server Mode or `ChunkList<T>`. `ObservableCollection<T>` gives live updates on add/remove; `List<T>` needs an explicit `grid.RefreshData()` or `AllowLiveDataShaping="True"` after changes.

Doc search queries:
- `"bind grid to a local collection ObservableCollection List"` → *Bind to a Local Collection* (exact XAML/C# and MVVM pattern)
- EF Core in-memory (`DbSet<T>.Local`): `"bind GridControl to Entity Framework Core sources"` → *Bind to Entity Framework Core Sources*
- EF6 / XPO in-memory (`.ToList()`, `XPCollection`): `"bind GridControl to classic Entity Framework EF6 XPO data source"` → *Bind to XPO Data Sources* (top hit) / *Bind to Entity Framework Sources* (close second — both are the correct topic family)

## `ChunkList<T>` — Large Data With Frequent Batch Updates

`DevExpress.Xpf.ChunkList.ChunkList<T>` is a specialized collection for cases where ordinary `List<T>` / `ObservableCollection<T>` become slow on **batch** insert/remove — not for iteration-heavy workloads (sort/filter/summary), where it can be slower than `List<T>`. Use it when the data source holds 10,000+ elements and updates arrive as batches.

Doc search queries:
- `"ChunkList large data frequent updates performance"` → *ChunkList\<T\> Class* (constructors, `IChunkListObject` vs. chunks-cache tradeoff, benchmark table)
- `"frequent data updates performance BeginDataUpdate OptimizeSummaryCalculation ChunkList"` → *Frequent Data Updates*

## CRUD with the MVVM Items Source Wizard Pattern

The Items Source Wizard (Quick Actions → Bind to a Data Source) generates MVVM CRUD scaffolding: `ValidateRowCommand`, `ValidateRowDeletionCommand`, `DataSourceRefreshCommand` on the view, backed by `[Command]`-attributed methods (`DevExpress.Mvvm.DataAnnotations`) on the view model.

> **Prerequisite**: `[Command]`-generated commands only produce working bindings when the view model is instantiated via `DevExpress.Mvvm.POCO.ViewModelSource.Create(...)` (or an equivalent POCO/Fody interceptor) — a plain `new MyViewModel()` leaves them `null` at bind time with no obvious error.

Doc search queries:
- `"Implement CRUD Operations in a Data-Bound WPF Grid"` → usually surfaces *Edit and Validate Data* or the *CRUD Operations in a Data-Bound Grid* topic directly (both link to each other; not a single guaranteed top hit — if the fetched page isn't the CRUD-specific one, follow its own "CRUD Operations" link)
- `"Items Source Wizard WPF GridControl"` → *Items Source Wizard*

See also [validation.md](validation.md) for richer validation flows.

## Server Mode (Very Large Data)

When the data source has 100K+ rows and you want **server-side aggregation** (sort / filter / group / summaries run on the database), use a Server Mode or Instant Feedback source. The grid pulls only the rows currently visible in its viewport; Instant Feedback additionally fetches on a background thread so the UI stays responsive.

| Data Access Technology | Server Mode Source | Instant Feedback (background-thread) |
|---|---|---|
| Entity Framework 4+ / Core | `EntityServerModeSource` | `EntityInstantFeedbackSource` |
| LINQ to SQL | `LinqServerModeSource` | `LinqInstantFeedbackSource` |
| eXpress Persistent Objects | `XPServerCollectionSource` | `XPInstantFeedbackSource` |
| OData v4 | `ODataServerModeSource` | `ODataInstantFeedbackSource` |
| WCF Data Services | `WcfServerModeSource` | `WcfInstantFeedbackSource` |
| Parallel LINQ to Objects | `PLinqServerModeSource` | `PLinqInstantFeedbackSource` |

All under `DevExpress.Data.*` (or `DevExpress.Xpo.*` for XPO) — confirm the exact member signatures from the fetched page below, not from this table alone.

> Prefer **Instant Feedback** over plain Server Mode whenever UI responsiveness during fetches matters. Note: WPF also ships a parallel wizard-generated family under `DevExpress.Xpf.Core.ServerMode` with a `...DataSource` suffix (e.g. `EntityInstantFeedbackDataSource`) — this is what the Items Source Wizard emits; verify against actual wizard output if the developer used that path rather than assuming the `DevExpress.Data.*` family above.

Server Mode / Instant Feedback disable a specific, non-obvious set of features (custom sort/group/filter via events, `Binding`-property columns, master-detail, `TreeListView`, add/remove while grouped, and more) — don't guess at these; fetch the current list.

Doc search queries:
- `"Server Mode and Instant Feedback Mode overview for GridControl large data"` → *Server Mode and Instant Feedback Mode* (concepts, exact instantiation code per technology)
- `"Server Mode limitations grid"` → *Server Mode and Instant Feedback Limitations* (the full unsupported-features list)

## Virtual Sources (REST / NoSQL / Custom APIs)

Use Virtual Sources when the backend exposes a **page-based** or **skip/take** API, the total record count is **unknown** or expensive to compute, and you want explicit control over which operations (sort / filter / summary) are sent to the server.

| Source Type | Behavior |
|---|---|
| `InfiniteAsyncSource` | Infinite scrolling (no pages); fetches portions on demand |
| `PagedAsyncSource` | Page-based (page navigation buttons) |
| `FetchRowsAsyncSource` | Custom fetch with skip tokens — for cursor-based APIs |

Unlike Server Mode, sorting, filtering, total summaries, searching, and grouping (not supported at all) are **disabled by default** and must be opted in per column/view via `ColumnBase.AllowSorting`, `Allowed*Filters`, etc. CRUD only supports Edit Entire Row (`ShowUpdateRowButtons`), validated via `GridViewBase.ValidateRow` with an async `UpdateRowResult`.

Doc search queries:
- `"bind grid to any data source with virtual sources REST API paging"` → *Bind the WPF Data Grid to any Data Source with Virtual Sources* (exact `FetchRows`/`FetchPage` event wiring, minimum requirements)
- `"virtual source limitations WPF grid unsupported features"` → *Virtual Source Limitations* (the full opt-in/unsupported list — `TreeListView` unsupported with any virtual source, grouping unsupported at all)

## `RealTimeSource` — High-Frequency Updates

For data that updates at high frequency (up to ~20,000 updates/second) on a **small** dataset (docs cite "not more than several hundred data records"), use `DevExpress.Data.RealTimeSource`. It buffers updates from an already-notifying collection (`INotifyCollectionChanged`/`IBindingList.ListChanged`/`INotifyPropertyChanged`) and dispatches them to the UI thread — it does not generate change notifications itself, and does not help with static data. In-place cell editing and `TreeListView` are not supported when bound via `RealTimeSource`.

`RealTimeSource` doesn't care what pushes the updates — a SignalR hub, a timer-driven simulation, or any other push source all work the same way: feed the underlying collection, let `RealTimeSource` marshal the notifications. SignalR-specific client wiring isn't covered by DevExpress's own docs (no query finds it); see the [`devexpress-wpf-grid-datasource`](../../devexpress-wpf-grid-datasource/SKILL.md) skill's [`source-selection.md`](../../devexpress-wpf-grid-datasource/references/source-selection.md) § "RealTimeSource & SignalR" for that piece specifically.

Doc search query: `"RealTimeSource frequently updated data source"` → *RealTimeSource* (exact `DataSource`/`DisplayableProperties`/`IgnoreItemEvents` usage and the full limitations list).

## `ICollectionView`

Standard WPF `ICollectionView` works as an `ItemsSource` — the grid honors `ICollectionView.Filter`, `SortDescriptions`, `GroupDescriptions` — but delegates data management to it, which runs the grid under Server Mode-equivalent restrictions unless `GridControl.AllowCollectionView="False"`. The Items Source Wizard does not generate code for `ICollectionView`, and it can't be used in master and detail grids simultaneously.

Doc search query: `"bind grid to ICollectionView"` → *Bind the WPF Data Grid to ICollectionView*.

## Dynamic / Unstructured Data (`DataTable`, `ExpandoObject`, `UnboundDataSource`)

For a `DataTable`, `ColumnBase.FieldName` maps to the column name directly. For structures with no compile-time schema (e.g. `ExpandoObject` collections), use `ColumnBase.Binding` instead of `FieldName` — see [columns.md](columns.md) § "`FieldName` vs `Binding` vs Unbound" for that decision. `UnboundDataSource` is designed specifically for binding scenarios with no strongly-typed data set available at compile time.

Doc search query: `"UnboundDataSource dynamic data no strongly typed data set WPF"` → *UnboundDataSource*.

## Frequent Update Performance

Three orthogonal techniques for taming high-frequency updates on an otherwise in-memory source, in increasing order of intrusiveness: lock updates with `BeginDataUpdate`/`EndDataUpdate` around a batch (works for `TreeListView` too, via an extra `recreateNodesOnEndDataUpdateOnly` parameter); set `OptimizeSummaryCalculation="True"` so summary cost becomes O(changed records) instead of O(all records) — requires `ObservableCollection<T>`/`ChunkList<T>` items implementing both `INotifyPropertyChanged` and `INotifyPropertyChanging`, no unbound-column or custom summaries, and no `BeginDataUpdate` lock at the same time; or turn off live shaping (`AllowLiveDataShaping="False"`) and call `grid.RefreshData()` on demand. For sub-millisecond update rates where none of these keep up, `RealTimeSource` (above) is the intended tool, not a bigger batch window.

Doc search query: `"frequent data updates performance BeginDataUpdate OptimizeSummaryCalculation ChunkList"` → *Frequent Data Updates* (exact property requirements and the automatic-refresh-on-timer pattern).

## TreeListControl Specifics

`TreeListControl` shares most binding patterns with `GridControl` **except**: Server Mode sources are **not supported** (`EntityServerModeSource`, `LinqServerModeSource`, etc.), Virtual Sources are **not supported** (`InfiniteAsyncSource`, `PagedAsyncSource`, etc.) — in-memory data only. TreeList-specific hierarchy modes (`KeyFieldName`/`ParentFieldName`, `ChildNodesPath`, `ChildNodesSelector`, `HierarchicalDataTemplate`, unbound `TreeListNode` mode) live in the `devexpress-wpf-tree-list` skill's `references/data-binding.md` and `nodes.md`.

## Common Issues

- **Grid is empty** — `ItemsSource` is null or the bound property doesn't fire `PropertyChanged`.
- **Edits don't persist / cells don't refresh** — `"GridControl cells do not update troubleshoot data grid updates"` → *Troubleshooting: Update Data* (wire `ValidateRowCommand`, confirm `INotifyPropertyChanged`/POCO instantiation, `AllowLiveDataShaping`).
- **Performance bad on 100K+ rows** — switch to Server Mode, or use `ChunkList<T>` + `OptimizeSummaryCalculation`.
- **Server Mode / Virtual Source + custom sort, master-detail, or TreeListView** — not supported by either — see each family's Limitations topic above rather than assuming.
- **Virtual Source: filter/sort UI is invisible** — disabled by default; set the `Allowed*` properties per column.

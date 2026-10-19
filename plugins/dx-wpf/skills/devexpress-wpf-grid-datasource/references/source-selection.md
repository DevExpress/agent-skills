# Source Selection — GridControl Data Source Advisor

This file is a self-contained decision tree from [code-inspection.md](code-inspection.md)'s classified signals (data origin, update frequency, data volume, plus target-control type) to one destination and its pinned documentation URL(s) — it does not depend on `devexpress-wpf-data-grid`'s `data-binding.md` to resolve a source. Things still genuinely out of this skill's scope even after covering every branch below: OLAP-cube data sources, the "Frequent Update Performance" tuning techniques (`BeginDataUpdate`/`OptimizeSummaryCalculation`) for an already-chosen in-memory source, and — for a `TreeListControl` target specifically — the hierarchy-wiring decision (self-referential `KeyFieldName`/`ParentFieldName`, `ChildNodesPath`, `ChildNodesSelector`/`HierarchicalDataTemplate`, or Unbound `TreeListNode` mode), which is a second, separate decision this tree doesn't make; see `devexpress-wpf-tree-list`'s `references/data-binding.md` for that.

## Decision Tree

**Every input below must already be a resolved, known value before you evaluate this tree — never plug in "unresolved."** In particular, `rows >= 100_000` and `rows >= 10_000` are numeric comparisons: if Data Volume is unresolved, that comparison has no answer, not a false one. Treating "unresolved" as if it failed the check silently walks the tree into the small-volume default branches below — which is exactly the bug this note exists to block. If any signal feeding this tree is still unresolved, stop and go back to `SKILL.md` Step 3 to ask; do not run this tree provisionally and disclose the assumption afterward.

Evaluate top to bottom — stop at the first matching branch:

```
source =
  if targetIsTreeListControl:
      InMemory                              # This branch only picks the ItemsSource collection type — re-apply the
                                             # batchUpdates/rows>=10_000 check below to choose List<T>/ObservableCollection<T>
                                             # vs ChunkList<T>, exactly as the non-TreeList branches do.
                                             # Server Mode, Virtual Sources, and RealTimeSource are never available for
                                             # TreeListControl (confirmed: WPF/6281, WPF/120191, WPF/118169).
                                             # This does NOT cover hierarchy wiring (KeyFieldName/ParentFieldName,
                                             # ChildNodesPath, ChildNodesSelector, or Unbound TreeListNode mode) — that's
                                             # a separate decision; see devexpress-wpf-tree-list's data-binding.md.
  elif dynamicSchema:                       # DataTable / ExpandoObject / Dictionary<string,object> rows /
                                             # no compile-time entity type reachable for the bound data
      UnboundDataSource                     # pinned: https://docs.devexpress.com/WPF/118089
  elif realTimeHighFrequency:               # SignalR push, Rx stream, or any other high-frequency push/event source
      RealTimeSource                        # pinned: https://docs.devexpress.com/WPF/118169
                                             # SignalR client wiring: see "RealTimeSource & SignalR" below (not on this page)
  elif requiresICollectionView:             # code already implements/exposes ICollectionView, or the developer
                                             # explicitly asked for ICollectionView semantics
      ICollectionView                       # pinned: https://docs.devexpress.com/WPF/11124
  elif externalPagedOrCursorAPI:            # REST/HTTP, page- or skip/take-shaped, unknown/expensive total count
      VirtualSource                         # InfiniteAsyncSource (infinite scroll, or cursor/skip-token via its
                                             # SkipToken property) / PagedAsyncSource (page-index navigation) —
                                             # only these two classes exist, see notes below
                                             # pinned: overview https://docs.devexpress.com/WPF/10803
                                             #         limits   https://docs.devexpress.com/WPF/120191
  elif efOrXpoOrigin and rows >= 100_000:
      InstantFeedbackMode if uiResponsivenessMatters else ServerMode
                                             # EntityServerModeSource/EntityInstantFeedbackSource etc. — exact class
                                             # depends on detected technology, see notes below
                                             # pinned: overview https://docs.devexpress.com/WPF/6279
                                             #         limits   https://docs.devexpress.com/WPF/6281
  elif efOrXpoOrigin:                       # small–medium volume — plain EF/XPO binding, not server mode
      EfOrXpoLocalBinding                   # e.g. grid.ItemsSource = context.Entities.ToList() (EF) / XPCollection (XPO)
                                             # pinned: EF (Core or 6) https://docs.devexpress.com/WPF/403740 — its "Local Data"
                                             #         section; do NOT use https://docs.devexpress.com/WPF/8601 here, that
                                             #         EF-Core-specific page only walks through Instant Feedback Mode
                                             #         XPO             https://docs.devexpress.com/WPF/6091
  elif batchUpdates and rows >= 10_000:      # plain in-memory origin, large volume, frequent batch updates
      ChunkList<T>                          # pinned: https://docs.devexpress.com/WPF/DevExpress.Xpf.ChunkList.ChunkList-1
  else:                                      # plain in-memory origin, default
      List<T> / ObservableCollection<T>     # pinned: https://docs.devexpress.com/WPF/404127
```

**Notes not carried by the tree** (exact class names always come from the `doc-fetch` result, never from memory):
- `EfOrXpoLocalBinding` branch: fetch WPF/403740 for either EF flavor, WPF/6091 for XPO — never WPF/8601, which despite its "Entity Framework Core" title has no local-binding walkthrough at all.
- Server Mode branch: prefer **Instant Feedback** over plain Server Mode whenever UI responsiveness during fetches matters — default to it unless the developer specifically wants the simpler synchronous behavior.
- Virtual Source branch: the exact class depends on backend shape — infinite scroll or cursor/skip-token (via `FetchRowsAsyncEventArgs.SkipToken`) → `InfiniteAsyncSource`; page-index navigation → `PagedAsyncSource` (prefer these `Async` variants; the non-`Async` `InfiniteSource`/`PagedSource` classes are obsolete). Sort/filter/summaries/search are off by default here (opt in per column); grouping isn't supported at all.
- Three names never to emit: `FetchRowsAsyncSource` isn't a real class — cursor/skip-token fetching is a *property* on `InfiniteAsyncSource`'s event args, not a separate source class. Bare `InstantFeedbackSource` isn't a real class either — use the technology-specific one (`EntityInstantFeedbackSource`, `LinqInstantFeedbackSource`, etc.). `AsyncInstantFeedbackSource` doesn't exist at all — it's just Instant Feedback Mode's existing async behavior; substitute the matching `*InstantFeedbackSource` class instead.
- `requiresICollectionView`/`ICollectionView` branch: this runs `GridControl` under Server-Mode-equivalent restrictions unless `AllowCollectionView="False"` — mention this tradeoff to the developer rather than assuming it's acceptable.

## RealTimeSource & SignalR

DevExpress's docs don't cover the SignalR *client wiring pattern* specifically — no page walks through connecting a `HubConnection` to `RealTimeSource` — so the wiring below is this skill's own content for that one piece, not something a fetch will find. This does not mean Step 6 (MCP search) should be skipped for this family: `doc-fetch` will likely report a `Gaps` item naming the missing SignalR integration — dispatch `doc-search` for it anyway, exactly as Step 6 requires unconditionally. A search can still surface genuinely useful, real facts adjacent to the gap that aren't in the snippet below (e.g. thread-marshaling requirements when a background callback mutates a bound collection) even though the core wiring *pattern* stays this file's own content. Treat "not documented" as "this exact pattern isn't on a page," not as "nothing more is findable" — those are different claims, and only the first one is true here.

**Recognizing SignalR code during inspection:**

| You see | Means |
|---|---|
| `using Microsoft.AspNetCore.SignalR.Client;`, `new HubConnectionBuilder().WithUrl(...).Build()` | A SignalR client is being constructed |
| `connection.On<T>("MethodName", item => ...)` | Subscribing to server-pushed messages of type `T` — the payload type the grid binds to |
| `await connection.StartAsync();` | The connection is actually started |
| `IObservable<T>`, `Subject<T>`, `.Subscribe(` without a `HubConnection` nearby | A non-SignalR push stream — still maps to `RealTimeSource` |

**The skill's own SignalR → RealTimeSource pattern:** wire the hub's `On<T>` callback to add/update items in the collection `RealTimeSource` wraps; `RealTimeSource` buffers and dispatches those changes to the UI thread — never update the bound collection directly from the SignalR callback thread.

```csharp
using DevExpress.Data;
using Microsoft.AspNetCore.SignalR.Client;

public class MarketDataViewModel {
    readonly ObservableCollection<Quote> _quotes = new();
    public RealTimeSource Source { get; }

    public MarketDataViewModel() {
        Source = new RealTimeSource { DataSource = _quotes };
        var connection = new HubConnectionBuilder().WithUrl("https://.../marketdatahub").Build();
        connection.On<Quote>("QuoteUpdated", quote => {
            var existing = _quotes.FirstOrDefault(q => q.Symbol == quote.Symbol);
            if (existing != null) _quotes[_quotes.IndexOf(existing)] = quote;
            else _quotes.Add(quote);
        });
        _ = connection.StartAsync();
    }
}
```
```xaml
<dxg:GridControl ItemsSource="{Binding Source}" AutoGenerateColumns="AddNew"/>
```

`_quotes` must raise `INotifyCollectionChanged` (e.g. `ObservableCollection<T>`) for the grid to see changes. Confirm the exact `RealTimeSource` member names (constructor, `DataSource` property) against the `doc-fetch` result rather than this snippet if they ever disagree — its self-check verifies property-vs-event before reporting.

Anti-patterns: don't mutate the wrapped collection from a non-UI thread expecting `RealTimeSource` to fix races on its own — it marshals notifications, it doesn't serialize writes. In-place cell editing and `TreeListView` are not supported when bound via `RealTimeSource` — a documented limitation, not a bug to work around.

## When Data Volume Is the Only Unresolved Signal

This is the common case (see [code-inspection.md](code-inspection.md) — volume is rarely inferable). Ask one question, framed against the two thresholds the tree above actually branches on (10K for `ChunkList<T>`, 100K for Server Mode/Instant Feedback):

> "Roughly how many `{Entity}` rows will this hold in production — a few thousand, tens of thousands with frequent batch updates, or hundreds of thousands+?"

Map the answer directly to the matching branch in the tree above for the already-known origin/frequency.

## TreeListControl Exception

`TreeListControl` shares the same tree above, but its first branch always wins: Server Mode, Virtual Sources, and `RealTimeSource` are never available for it, regardless of what any other signal says. If inspection's other signals would otherwise point toward one of those for a TreeList target, tell the developer the constraint and fall back to in-memory data (`List<T>`/`ObservableCollection<T>`/`ChunkList<T>`, picked by the same volume/batch-update check as the non-TreeList branches) instead.

Picking the in-memory collection type is only half the job for a `TreeListControl` — it still needs its hierarchy wired (self-referential `KeyFieldName`/`ParentFieldName`, `ChildNodesPath`, `ChildNodesSelector`/`HierarchicalDataTemplate`, or Unbound `TreeListNode` mode). That decision, and its own pinned facts, live in `devexpress-wpf-tree-list`'s `references/data-binding.md` — this skill's own pinned URLs (e.g. `WPF/404127`) only cover flat `GridControl` binding and say nothing about hierarchy properties.

## Common Issues

- **Two source families both look plausible** — e.g. code shows both an EF `DbContext` and a REST client for the same entity. Trace the specific property bound to the grid, not the whole class; prefer whichever one directly feeds it.
- **Developer names a source that contradicts inspection** — see "Resolving Contradictions" in [code-inspection.md](code-inspection.md); implement what they asked and mention the mismatch once.
- **Target is `TreeListControl`** — see the exception above.

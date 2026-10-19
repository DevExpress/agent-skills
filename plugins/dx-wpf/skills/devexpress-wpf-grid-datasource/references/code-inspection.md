# Code Inspection Heuristics — GridControl Data Source Advisor

This is the core logic that makes this skill an *advisor* instead of a questionnaire: before asking anything, read the project's existing code and classify what it reveals against three signals. This reference is a lookup table for a host agent's own grep/read tools — it is deliberately plain instructions, not an executable script, so it works identically in Claude Code, GitHub Copilot, Cursor, and any other host that reads `SKILL.md`.

**Delegating this step to the `inspect` sub-agent is mandatory whenever the host can dispatch one.** `SKILL.md`'s Step 1 requires spawning `inspect` (target-finding plus inspection together) whenever any sub-agent/subtask dispatch tool is available (e.g. Claude Code's `Agent`/`Task` tool) — check your tool list before inspecting anything yourself. Inspecting inline is only acceptable when no such tool exists at all, or the target and signals are already known from earlier in the conversation. `inspect` applies the tables below and returns only the "Output of This Step" result — whether a target was found, the three classified signals with one-line evidence each, plus the files-of-interest list (see below) — not the raw contents of the files it read along the way.

## When to Use This Reference

Use this when you need to:

- Decide which files to read and how far to follow the dependency graph before giving up
- Map a code pattern you found to one of the three signal values
- Decide whether a signal is "known from code" or "unresolved" (and therefore worth asking about)

## How Far to Look

1. **Start at the target.** If a `GridControl` or `TreeListControl` already exists in XAML with `ItemsSource="{Binding X}"`, find `X` on its `DataContext` type (usually a ViewModel). Note the View file's path, which of the two controls it is (this gates `source-selection.md`'s first tree branch), and how `DataContext` is currently set (a XAML object element vs. code-behind) — you'll report this as a file of interest. If no grid/tree exists yet, report that clearly rather than guessing — the caller will ask the developer which ViewModel/feature it's for; this is a "target," not a "signal," question, and is always fine to ask.
2. **Read that ViewModel file in full.**
3. **Follow exactly one hop** into any type it takes through constructor injection or instantiates directly (a repository, a service, a `DbContext` subclass, an API client wrapper). Do not recursively chase the whole call graph — one hop is enough to see the shape of the data access.
4. **Check DI registration** (`App.xaml.cs`, `Program.cs`, `Startup.cs`, or a `ConfigureServices`/`AddDbContext`/`AddHttpClient`/`AddSignalR` call) when the ViewModel takes an interface — the registration reveals the concrete type.
5. **Pre-filter with the `.csproj`** before reading source: a `Microsoft.EntityFrameworkCore*` reference makes EF a candidate; `*.SignalR.Client` makes SignalR a candidate; `System.Reactive` makes Rx a candidate. Absence of a package doesn't rule anything out on its own (e.g. `HttpClient` ships in the BCL, no package needed) — treat this step as a hint, not a verdict.

**Before inspecting code, check the developer's own request and the conversation so far.** If they already stated a signal directly ("this will hold about 500,000 rows," "it needs to update live," "just use EF") treat that as known — from the request, not from code — and do not ask about it again. Code inspection exists to avoid asking about what the developer *hasn't* already told you; it is not a reason to re-ask what they have.

**If there's no `.csproj` to grep** (a snippet-review context, loose files handed to you directly, or a task that predates project scaffolding), treat step 5 above as skipped, not blocking — proceed straight to reading the ViewModel and its dependencies. The `.csproj` pre-filter is an optional cheap accelerator, never a prerequisite for the rest of the workflow.

If the code behind the target genuinely doesn't exist yet (a brand-new ViewModel, an empty class), stop inspecting and ask the developer directly — this is not "unresolved," it's "nothing to inspect." If the entity/model type itself doesn't exist yet either (no `Report`/`Order`-equivalent class anywhere reachable), say so and ask about its shape (or propose a minimal one) before writing binding code — every reference file's worked examples assume the entity type already exists, and generated code won't compile without one.

## Signal 1: Data Origin

| Pattern found | Origin | Confidence |
|---|---|---|
| A type deriving from `DbContext`; `IQueryable<T>`; `.Include(`; `using Microsoft.EntityFrameworkCore` | EF Core / LINQ | High |
| `EntityFramework.dll` reference, `System.Data.Entity`, `ObjectContext` | EF6 (classic) | High |
| `XPCollection`, `Session`, `using DevExpress.Xpo` | XPO | High |
| `HttpClient`, `HttpRequestMessage`, `IHttpClientFactory`, `.GetFromJsonAsync(`, a typed REST client wrapper | REST / HTTP API | High |
| `HubConnection`, `using Microsoft.AspNetCore.SignalR.Client`, `HubConnectionBuilder`, `.On<T>(` | SignalR push stream | High |
| `IObservable<T>`, `Subject<T>`, `using System.Reactive` (and no `HubConnection` alongside it) | Rx observable stream | High |
| `ObservableCollection<T>`/`List<T>` populated directly in the constructor or an in-memory seed method, no fetch/service call anywhere in the class | Plain in-memory collection | High |
| The bound property is already typed `ICollectionView`, or built via `CollectionViewSource`/`CollectionViewSource.GetDefaultView(...)`; or the developer explicitly asked for `ICollectionView`/shared-across-controls filter-sort-group semantics | ICollectionView requested | High |
| `DataTable`, `ExpandoObject`, `Dictionary<string, object>` rows, or no compile-time entity/POCO type reachable at all for the bound data | Dynamic/unstructured schema | Medium — weaker than the others since it's argued from *absence* of a type, not a positive pattern; don't guess this one on a thin one-hop read |
| A repository/service interface with no visible implementation in the one-hop walk, and DI registration doesn't resolve it either | Unresolved | — ask |

If more than one pattern matches (e.g. an EF `DbContext` wrapped behind a service that's also called over HTTP by a different layer), prefer the pattern closest to what actually feeds the ViewModel property bound to the grid — trace the specific property, not the whole class.

## Signal 2: Update Frequency

| Pattern found | Frequency | Confidence |
|---|---|---|
| `DispatcherTimer`, `System.Timers.Timer`, `PeriodicTimer`, an explicit polling loop (`while`/`Task.Delay` re-fetch) | Polling | High |
| `HubConnection.On<T>(...)` handler plus a `StartAsync()` call | Push / subscription | High |
| `IObservable<T>.Subscribe(` | Event stream | High |
| None of the above; data is loaded once in a constructor, `OnLoaded`, or an explicit one-shot `LoadAsync()` with no re-invocation elsewhere | Static (no live updates) | High — **not ambiguous, nothing to ask** |
| A `DbContext` with `.Local` exposed as the bound collection, no timer/hub/observable anywhere | Static, but edits made in the grid still reflect live via change tracking — note this distinction to the developer, it is not the same as a live *external* update feed | High |

## Signal 3: Data Volume

Data volume is almost never visible in source code — treat it as **ask-only by default**. The exceptions, where existing code already proves scale:

| Pattern found | Volume signal |
|---|---|
| `DevExpress.Xpf.ChunkList.ChunkList<T>` already in use | Large, batch-updated (10K+) |
| `EntityServerModeSource`, `LinqServerModeSource`, or any `*ServerModeSource`/`*InstantFeedbackSource` already wired up elsewhere in the app for the same or a similar entity | Very large (100K–100M) |
| An existing paging implementation with an explicit large page-size constant, or a comment/config value naming an expected row count | Use the stated number directly |

If none of these are present, ask the developer directly — do not infer volume from origin alone (an EF `DbContext` is just as often backing a 500-row lookup table as a 10-million-row fact table), and do not assume a small/default volume and proceed to implement, planning to disclose that assumption afterward. Volume is the signal most often left with zero code evidence (a brand-new or empty ViewModel gives none at all) — that makes it "unresolved," not "small." Report it as unresolved and let `SKILL.md` Step 3 ask before source selection runs.

## Resolving Contradictions

If the developer states a source type explicitly ("just use `ObservableCollection`") but inspection shows evidence pointing elsewhere (e.g. a `DbContext` with millions of rows expected), don't silently override their instruction — implement what they asked, but mention the mismatch once: "Note: your `NorthwindContext` here has [reason to expect large volume] — `ObservableCollection<T>` will work but may be slow past ~100K rows; let me know if you'd like Server Mode instead." Code inspection informs the recommendation; it does not override an explicit developer decision.

## When Every Signal Is Unresolved (Nothing to Inspect)

A brand-new, empty ViewModel gives you zero signal, which means the usual "inspect first, ask only the gap" premise doesn't pay off — you have to ask about all three at once. Don't reconstruct three separate questions from the signal tables above (that becomes exactly the generic questionnaire the skill exists to avoid); ask one merged question instead:

> "Where will this data come from — an existing database/EF `DbContext`, a REST/HTTP API, a real-time push feed (e.g. SignalR), or an in-memory list you'll populate directly? If you already have a model/entity or a repository/service for it elsewhere in the solution, point me to it and I'll inspect that instead. Also, roughly how many rows will this hold in production, and does it need to reflect live updates or is a one-time load enough?"

This is still one targeted ask, not a questionnaire — it's scoped tightly to the three signals this skill needs, nothing more (no framework/version questions bundled in unless the project itself is also ambiguous about those).

## Files of Interest to Report

Alongside the three signals, track and report the file paths you actually touched during inspection — path plus a one-line role note, not file contents. This is what lets the main agent go straight to editing instead of re-discovering these locations itself:

| File | Role note to include |
|---|---|
| The View XAML (e.g. `MainWindow.xaml`) | Whether `GridControl` already exists there or needs to be added; how `DataContext` is currently set — this determines whether `SKILL.md` Step 7's instantiation-path check applies |
| The View's code-behind (`.xaml.cs`), only if `DataContext` is wired there rather than in XAML | One line on what it currently does (e.g. "constructs `OrdersViewModel` with no arguments") |
| The target ViewModel file | One line identifying it as the inspection target |
| Each dependency file followed one hop (repository/service/`DbContext`) | One line naming the concrete type and what it exposes (e.g. "`NorthwindContext : DbContext`, exposes `DbSet<Order> Orders`") |
| The DI registration file, only if it was actually consulted (interface-typed dependency) | One line naming the concrete type it resolves to |

Example report:

- `Views/MainWindow.xaml` — View bound to `OrdersViewModel`; no `GridControl` yet, add it here
- `Views/MainWindow.xaml.cs` — code-behind; `DataContext` currently unset (bound via a `<local:OrdersViewModel/>` XAML object element instead)
- `ViewModels/OrdersViewModel.cs` — inspection target
- `Data/NorthwindContext.cs` — `NorthwindContext : DbContext`, exposes `DbSet<Order> Orders`

## Output of This Step

By the end of inspection you should have:
- For each of the three signals, either **a classified value** with a one-line reason ("Found `NorthwindContext : DbContext`" / "Found `HubConnection.On<Quote>` + `StartAsync()`"), stated to the developer before proceeding, or **unresolved**, to be asked about per `SKILL.md` Step 2.
- The **files-of-interest list** above.

Pass the result to [source-selection.md](source-selection.md) to pick the actual source class.

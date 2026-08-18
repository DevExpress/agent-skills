# Sorting, Filtering, Summaries

The TreeList includes Data Grid-style data shaping: sort, filter, search, and summarize a hierarchy. Always use the TreeList's own API — not the data source's — so the tree structure is preserved.

## When to Use This Reference

- Sorting in code or enabling end-user sorting
- Filtering (programmatic, Excel-style, Find Panel, auto-filter row)
- Calculating total / group / custom summaries

## Sorting

Sort against a column with `SortIndex` (sort priority) and `SortOrder`:

```csharp
// Simplest: assign a sort priority. SortIndex = 0 makes this the primary sort column.
treeList.Columns["Region"].SortIndex = 0;

// Explicit direction (SortOrder enum: None / Ascending / Descending):
treeList.Columns["Sales"].SortOrder = System.Windows.Forms.SortOrder.Descending;
```

> TreeList sorting has specifics: by default each node level is sorted independently within its parent. See [TreeList Sorting Specifics](https://docs.devexpress.com/content/WindowsForms/5604?md=true). For custom comparison logic, handle the custom-sort event (`CompareNodeValues` — confirm via MCP).

End-user sorting is on by default (clicking a header). Control it through `OptionsBehavior` / column options.

## Filtering

### Programmatic Filter

```csharp
treeList.ActiveFilterString = "[MarchSales] > 10000";
// or build criteria objects:
// treeList.ActiveFilterCriteria = new BinaryOperator("MarchSales", 10000, BinaryOperatorType.Greater);
```

### Auto Filter Row

A row under the headers where users type per-column filter values:

```csharp
treeList.OptionsView.ShowAutoFilterRow = true;
```

### Excel-Style / Classic Pop-up Filter

Column header filter buttons open a pop-up filter menu. Choose the style via the column's filter options (`OptionsFilter`) — Excel-style is the modern default. See [Excel Style](https://docs.devexpress.com/content/WindowsForms/120620?md=true) / [Classic Style](https://docs.devexpress.com/content/WindowsForms/120621?md=true).

### Find Panel

A search box that filters/highlights matching nodes:

```csharp
treeList.OptionsFind.AllowFindPanel = true;   // enable
treeList.ShowFindPanel();                      // show it programmatically
// treeList.HideFindPanel();
```

## Summaries

Summaries aggregate column values in the column footer (total) or group footers. Use `SummaryItemType` (`Sum`, `Min`, `Max`, `Count`, `Average`, `Custom`, `None`).

```csharp
treeList.Columns["Sales"].SummaryFooter          = DevExpress.XtraTreeList.SummaryItemType.Sum;
treeList.Columns["Sales"].SummaryFooterStrFormat = "Total={0:c0}";
treeList.Columns["Sales"].AllNodesSummary        = false;   // root nodes only (false) vs all nodes (true)
treeList.OptionsView.ShowSummaryFooter           = true;    // show the footer
```

For custom aggregation, set `SummaryFooter = SummaryItemType.Custom` and handle the custom-summary event (confirm the exact event name via MCP — typically the summary-calculation event).

## Source Material

- `articles/controls-and-libraries/tree-list/feature-center/sorting/...` (`xref:307`, specifics `xref:5604`)
- `articles/controls-and-libraries/tree-list/feature-center/filtering/...` (`xref:5551`; in code `xref:5608`; Excel `xref:120620`)
- `articles/controls-and-libraries/tree-list/feature-center/summaries/...` (`xref:313`)
- [SummaryItemType](https://docs.devexpress.com/WindowsForms/DevExpress.XtraTreeList.SummaryItemType) — summary type enum (the TreeList-specific `DevExpress.XtraTreeList.SummaryItemType`, distinct from `DevExpress.Data.SummaryItemType`)
- [ActiveFilterString](https://docs.devexpress.com/content/WindowsForms/DevExpress.XtraTreeList.TreeList.ActiveFilterString?md=true), [ShowFindPanel](https://docs.devexpress.com/content/WindowsForms/DevExpress.XtraTreeList.TreeList.ShowFindPanel?md=true), [OptionsFind](https://docs.devexpress.com/content/WindowsForms/DevExpress.XtraTreeList.TreeList.OptionsFind?md=true)

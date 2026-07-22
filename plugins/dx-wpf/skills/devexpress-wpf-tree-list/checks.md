# Skill Generation Report — `devexpress-wpf-tree-list`

Generated against WPF docs repo HEAD `b16066c633b85ee391d1e6188ffc4cd815ee5e8f` on 2026-05-15.

## Files Created

```
skills/wpf/devexpress-wpf-tree-list/
├── SKILL.md                            (417 lines)
├── checks.md                           (this file)
├── references/
│   ├── getting-started.md              (145 lines, .NET 6/7/8+)
│   ├── getting-started-dotnet-fw.md    (74 lines)
│   ├── data-binding.md                 (238 lines)
│   ├── nodes.md                        (454 lines)
│   ├── editing.md                      (186 lines)
│   ├── validation.md                   (188 lines)
│   ├── focus-and-selection.md          (187 lines)
│   ├── end-user-features.md            (159 lines)
│   ├── drag-and-drop.md                (267 lines)
│   ├── save-restore-layout.md          (208 lines)
│   └── advanced-features.md            (141 lines)
└── examples/
    ├── quickstart.cs                   (100 lines, multi-section reference)
    ├── quickstart.xaml                 (48 lines)
    ├── quickstart-mainview.xaml        (40 lines)
    └── quickstart-mainviewmodel.cs     (59 lines)
```

Total: **15 files, ~2911 lines** of generated content.

## Validation Results

| Check | Result |
|---|---|
| YAML frontmatter valid (`---` delimiters, kebab-case name) | PASS |
| `description` ≤ 1024 chars | PASS (883 chars — initially 1064, trimmed) |
| `description` has no XML angle brackets | PASS |
| `compatibility` field present, ≤ 500 chars | PASS (395 chars) |
| `metadata.author` set to `DevExpress` | PASS |
| `metadata.version` quoted as YAML string (`"26.1"`) | PASS |
| `metadata.source-commit` set to repo HEAD | PASS (`b16066c633...`) |
| SKILL.md ≤ 500 lines | PASS (371 lines) |
| All `references/*.md` referenced in Navigation Guide exist | PASS (all 5 referenced files present) |
| No `// TODO: Verify API` placeholders in `getting-started.md` | PASS |
| No `// TODO: Verify API` placeholders in `quickstart-*.cs/.xaml` | PASS |
| No `// TODO: Verify API` placeholders in `SKILL.md` | PASS |
| `.NET` and `.NET Framework` getting-started both present | PASS |
| WPF-specific elicitation (designer vs code, data shape, async loading) | PASS — extends data-grid pilot with TreeList-specific Qs (data shape, auto-expand, async loading) |
| WPF theming snippet (`ApplicationThemeHelper`) | N/A — covered by separate theme skill |
| `net{X}-windows` TFM noted | PASS |
| Inherited `Application` ambiguity gotcha documented (lesson from data-grid pilot) | N/A — theme/App.OnStartup boilerplate removed; covered by separate theme skill |

## Placeholders Requiring Verification (2 total — both in deep features)

Per the meta-skill protocol, placeholders in deep feature references are acceptable if documented with risk level. Critical files (getting-started, quickstart, SKILL.md) have **zero** placeholders.

| File | Line | Topic | Risk | Resolution |
|---|---|---|---|---|
| `references/data-binding.md` | 128 | Exact name of the async child-nodes selector interface (`IAsyncChildNodesSelector` vs `IChildNodesSelectorAsync`) | Low | `devexpress_docs_search(query="asynchronous child nodes selector fetch on demand")` |
| `references/advanced-features.md` | 77 | XAML schema for `FormatCondition` inside `TreeListControl.ConditionalFormattings` | Medium | `devexpress_docs_search(technology="WPF TreeList", query="conditional formatting XAML FormatCondition")` |

## WPF-Specific Adaptations (per `generating-new-skills.md` § "WinForms and WPF")

All five points from the doc are covered, plus lessons learned from the data-grid pilot:

| Requirement | Where Addressed |
|---|---|
| Designer-first patterns (both designer + code-only) | SKILL.md elicitation Q4; `getting-started-dotnet-fw.md` describes Template Gallery + toolbox path |
| Platform scope: `net{X}-windows` explicit | SKILL.md compatibility + Constraints rule 2; `getting-started.md` Step 2 |
| Theming snippet (`ApplicationThemeHelper.ApplicationThemeName`) | N/A — covered by separate theme skill |
| Data shape elicitation (self-ref / hierarchical / unbound / async) | SKILL.md elicitation Q6 + Q7 — TreeList-specific (more variants than GridControl) |
| Events coverage (signatures, not just properties) | `editing.md` covers `CellValueChanged`; `advanced-features.md` enumerates drag-drop events |
| **Application ambiguity gotcha** (carried over from data-grid pilot) | SKILL.md Troubleshooting row; `getting-started.md` Step 4 note; `quickstart-app.xaml.cs` comment |

## Source Articles Read (Verified Against)

- `articles/controls-and-libraries/tree-list.md` (root)
- `articles/controls-and-libraries/tree-list/getting-started/lesson-1-add-a-treelistcontrol-to-a-project.md`
- `articles/controls-and-libraries/tree-list/getting-started/lesson-2-build-a-tree-in-unbound-mode.md`
- `articles/controls-and-libraries/tree-list/concepts.md` (referenced)
- `articles/controls-and-libraries/tree-list/end-user-capabilities.md` and subtopics
- `articles/controls-and-libraries/tree-list/design-time-features.md`
- `articles/controls-and-libraries/data-grid/display-hierarchical-data.md` (shared hierarchical binding article)

API names verified against `apidoc/`:
- `apidoc/DevExpress.Xpf.Grid/TreeListControl/` (confirmed: `#ctor`, `Background`, `Bands`, etc.)
- `apidoc/DevExpress.Xpf.Grid/TreeListView/` (confirmed: `KeyFieldName`, `Nodes`, `KeyFieldNameProperty`)
- `apidoc/DevExpress.Xpf.Grid/TreeListColumn/` (confirmed)
- `apidoc/DevExpress.Xpf.Grid/TreeListNode/` (confirmed: `Content`, `Nodes`, `ExpandStateBinding`)

## Template Includes Resolved

No template includes encountered in the sampled TreeList articles (`tree-list/getting-started/lesson-1` and `lesson-2`). The `data-grid/display-hierarchical-data.md` article that TreeList shares with GridControl also contained no resolvable includes for the sections referenced here.

No unresolved includes.

## What Differs From the data-grid Pilot

1. **Smaller scope**: 5 reference files vs 9 (TreeList has 18 source articles vs 287 for Data Grid).
2. **TreeList-specific elicitation**: SKILL.md asks about **data shape** (self-ref vs hierarchical vs unbound) and **async loading** — these don't apply to GridControl.
3. **Unbound mode coverage**: `data-binding.md` includes a full unbound-mode pattern (XAML + code) — a TreeList feature absent in GridControl.
4. **`TreeListNode` API**: documented in SKILL.md API surface and `data-binding.md` (no equivalent in the data-grid pilot).
5. **`Application` ambiguity gotcha**: pre-emptively documented (lesson learned from `TestGridApp` build failure).

## Testing — Next Steps

1. **Resolve the 2 placeholders** using the `devexpress_docs_search` queries above.
2. **Try the quickstart in a real WPF project**:
   ```bash
   dotnet new wpf -n DevExpressTreeListQuickstart
   cd DevExpressTreeListQuickstart
   dotnet add package DevExpress.Wpf.Grid.Core
   # Replace MainWindow.xaml and add MainViewModel.cs / App.xaml.cs from examples/
   dotnet build
   dotnet run
   ```
3. **Run the 5-prompt manual test** from the meta-skill.
4. **Compare with `devexpress-wpf-data-grid`**: many references (binding, theming, validation, conditional formatting) follow the same pattern. Discrepancies are intentional (e.g., master-detail is GridControl-only; unbound mode is TreeList-only).

## Ready to Apply Pattern to PivotGrid?

This skill validates that the data-grid pilot pattern scales to a smaller member of the family. PivotGrid (`devexpress-wpf-pivot-grid`) is next — different feature set (data shaping, OLAP binding, field areas) but the same shape: SKILL.md + references + examples + checks.md.

## Post-Generation Updates

- Fixed the "Using DevExpress Documentation MCP" section in SKILL.md: corrected the tool signature to `devexpress_docs_search(technologies=["WPF"], question=...)` / `devexpress_docs_get_content(url=...)`, removed the "if available" availability hedge, added the cross-agent tool-naming note (host-specific prefixes), and added/verified the untrusted-content security note.
- Bumped the minimum supported modern-.NET floor in SKILL.md's `compatibility:` frontmatter from `.NET 6+` to `.NET 8+` (v26.1 policy; `.NET Framework 4.6.2+` alternative unchanged).

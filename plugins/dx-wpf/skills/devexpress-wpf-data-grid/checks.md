# Pilot Skill Generation Report — `devexpress-wpf-data-grid`

Generated against WPF docs repo HEAD `b16066c633b85ee391d1e6188ffc4cd815ee5e8f` on 2026-05-15.

## Files Created

```
skills/wpf/devexpress-wpf-data-grid/
├── SKILL.md                            (477 lines)
├── checks.md                           (this file)
├── references/
│   ├── getting-started.md              (222 lines, .NET 6/7/8+)
│   ├── getting-started-dotnet-fw.md    (87 lines)
│   ├── data-binding.md                 (545 lines)
│   ├── views.md                        (146 lines)
│   ├── columns.md                      (507 lines)
│   ├── cell-display-and-editing.md     (377 lines)
│   ├── editing.md                      (170 lines)
│   ├── validation.md                   (425 lines)
│   ├── sorting-filtering-grouping.md   (649 lines)
│   ├── data-summaries.md               (123 lines)
│   ├── focus-and-selection.md          (447 lines)
│   ├── master-detail.md                (582 lines)
│   ├── conditional-formatting.md       (436 lines)
│   ├── drag-and-drop.md                (395 lines)
│   ├── save-restore-layout.md          (370 lines)
│   ├── printing-exporting.md           (466 lines)
│   ├── card-view.md                    (331 lines)
│   └── advanced-features.md           (112 lines)
└── examples/
    ├── quickstart.cs                   (multi-section quickstart reference)
    ├── quickstart.xaml                 (XAML quickstart)
    ├── quickstart-mainview.xaml        (XAML for MainWindow)
    ├── quickstart-mainviewmodel.cs     (ViewModel + Order model)
    └── quickstart-app.xaml.cs          (App startup with theme)
```

Total: **21 reference files + 5 examples, ~7500 lines** of generated content.

## Validation Results

| Check | Result |
|---|---|
| YAML frontmatter valid (`---` delimiters, kebab-case name) | PASS |
| `description` ≤ 1024 chars | PASS (883 chars) |
| `description` has no XML angle brackets | PASS |
| `compatibility` field present, ≤ 500 chars | PASS (321 chars) |
| `metadata.author` set to `DevExpress` | PASS |
| `metadata.version` quoted as YAML string (`"26.1"`) | PASS |
| `metadata.source-commit` set to repo HEAD | PASS (`b16066c633...`) |
| SKILL.md ≤ 500 lines | PASS (421 lines) |
| All `references/*.md` referenced in Navigation Guide exist | PASS (all 11 referenced) |
| No `// TODO: Verify API` placeholders in `getting-started.md` | PASS |
| No `// TODO: Verify API` placeholders in `quickstart-*.cs/.xaml` | PASS |
| No `// TODO: Verify API` placeholders in `SKILL.md` | PASS |
| `.NET` and `.NET Framework` getting-started both present | PASS |
| WPF-specific elicitation (designer vs code, data source) | PASS — covered in SKILL.md "Before You Start" |
| WPF theming snippet (`ApplicationThemeHelper`) | N/A — theme setup removed; covered by separate theme skill |
| `net8.0-windows` TFM noted | PASS |

## Placeholders Requiring Verification (7 total — all in deep feature references)

Per the meta-skill protocol, placeholders in deep feature references are acceptable if documented with risk level. Critical files (getting-started, quickstart, SKILL.md) have **zero** placeholders.

| File | Line | Topic | Risk | Resolution |
|---|---|---|---|---|
| `references/data-binding.md` | 187 | Virtual Source constructor / member names | Low | `devexpress_docs_search(technology="WPF Data Grid", query="virtual source InfiniteAsyncSource")` |
| `references/data-summaries.md` | 111 | `CustomSummaryEventArgs` member names | Low | `devexpress_docs_search(query="CustomSummary event")` |
| `references/printing-exporting.md` | 80 | `CreateReportDocument` / `ReportBuilder` API | Medium | `devexpress_docs_search(query="generate grid based report")` |
| `references/master-detail.md` | 103 | `TabViewDetailDescriptor` or equivalent | Medium | `devexpress_docs_search(query="tabbed master detail descriptor")` |
| `references/conditional-formatting.md` | 59 | XAML schema for `ColorScaleFormat` inside `ConditionalFormattings` | High | `devexpress_docs_search(query="ColorScaleFormat ConditionalFormattings XAML")` |
| `references/conditional-formatting.md` | 72 | `FormatCondition` root element / attached properties | High | Same as above |
| `references/conditional-formatting.md` | 116 | Documentation note about three placeholders above | — | Informational |

## WPF-Specific Adaptations (per `generating-new-skills.md` § "WinForms and WPF")

| Requirement | Where Addressed |
|---|---|
| Designer-first patterns (both designer + code-only) | SKILL.md elicitation Q4; `references/data-binding.md` includes both Items Source Wizard path and code-first path |
| Platform scope: `net8.0-windows` explicit | SKILL.md compatibility field + Constraints rule 2; `references/getting-started.md` step 2 |
| Theming snippet (`ApplicationThemeHelper.ApplicationThemeName`) | N/A — theme setup removed; covered by separate theme skill |
| Data binding elicitation (DataTable / `List<T>` / EF Core / XPO / virtual) | SKILL.md elicitation Q6 |
| Events coverage (signatures, not just properties) | `references/master-detail.md` enumerates `MasterRowExpanding/Expanded/Collapsing/Collapsed`; `editing.md` covers `CellValueChanged`; `advanced-features.md` enumerates drag-drop events |

## Source Articles Read (Verified Against)

- `articles/controls-and-libraries/data-grid.md` (root)
- `articles/controls-and-libraries/data-grid/getting-started/code/lesson-1-add-a-gridcontrol-to-a-project.md`
- `articles/controls-and-libraries/data-grid/getting-started/code/lesson-2-display-and-edit-data.md`
- `articles/controls-and-libraries/data-grid/getting-started/designer/lesson-1-add-a-gridcontrol-to-a-project.md`
- `articles/controls-and-libraries/data-grid/bind-to-data.md`
- `articles/controls-and-libraries/data-grid/views.md`
- `articles/controls-and-libraries/data-grid/sorting.md`
- `articles/controls-and-libraries/data-grid/grouping.md`
- `articles/controls-and-libraries/data-grid/filtering-and-searching.md`
- `articles/controls-and-libraries/data-grid/data-summaries.md`
- `articles/controls-and-libraries/data-grid/master-detail-data-representation.md`
- `articles/controls-and-libraries/data-grid/printing-and-exporting.md`
- `articles/controls-and-libraries/data-grid/conditional-formatting.md`
- `articles/controls-and-libraries/data-grid/display-hierarchical-data.md`
- `articles/common-concepts/themes.md`

API names verified against `apidoc/`:
- `apidoc/DevExpress.Xpf.Grid/` (288 entries — GridControl, TableView, CardView, GridColumn, ColumnBase, DataControlBase, DataViewBase, GridViewBase, GridSummaryItem, etc.)
- `apidoc/DevExpress.Xpf.Grid.ConditionalFormatting/`

## Template Includes Resolved

Only one template include was encountered in sampled articles:
- `templates/ai-semantic-search-tip.md` → embedded in `filtering-and-searching.md`. Resolved and content reflected in `references/sorting-filtering-grouping.md` § Search.

No unresolved includes.

## IDE Linter Noise (Not Skill Defects)

The VS Code DevExpress DocFX linter flags the following warnings on files in `skills/` because it scans every `.md` in the workspace, regardless of `docfx.json` exclude paths. The `skills/` directory is **not** included in `docfx.json` `content` paths, so DocFX itself does not process these files.

- `[doc.ctor] Empty URL scheme` on relative markdown links like `[data-binding.md](references/data-binding.md)` — these are valid relative links in the Agent Skills standard (portable across Copilot, Claude, Cursor, Gemini).
- `[doc.ctor] Banned term 'docs.devexpress.com/'` — one occurrence in `conditional-formatting.md` was reworded to avoid the external URL.
- `MD060/table-column-style` on `|---|---|` separators — repo style uses this compact form; the IDE linter prefers `| --- | --- |`.
- `MD033/no-inline-html` on `<kbd>` tags in `master-detail.md` — `<kbd>` is used in the source article (DocFX supports it).
- Spell-check warnings on technical terms (`args`, `enum`, `apidoc`, `refetched`) — these appear inside code identifiers / file paths in prose. Add to a `cspell` dictionary or surround with backticks in a later pass.

**Recommendation**: add `skills/**` to whatever ignore configuration the DevExpress DocFX linter uses (the warnings do not affect the DocFX build or the Agent Skills functionality). If the warnings are blocking, the alternative is to disable the markdown linter for `skills/**` via `.vscode/settings.json`.

## Testing — Next Steps (per meta-skill `references/testing-and-evaluation.md`)

1. **Resolve the 7 placeholders** using `devexpress_docs_search` / `devexpress_docs_get_content` queries listed above.
2. **Try the quickstart in a real WPF project**:
   ```bash
   dotnet new wpf -n DevExpressGridQuickstart
   cd DevExpressGridQuickstart
   dotnet add package DevExpress.Wpf.Grid.Core
   # Replace MainWindow.xaml and add MainViewModel.cs / App.xaml.cs from examples/
   dotnet build
   dotnet run
   ```
3. **Run the 5-prompt manual test** from the meta-skill (see `~/.claude/skills/devexpress-doc-skill-creator/references/testing-and-evaluation.md`).
4. **Optionally run the automated benchmark** with `evaluation/evals-data-grid.json` and target ≥ 90% WITH pass rate.

## Ready to Apply Pattern to TreeList and PivotGrid?

This pilot validated the structure, elicitation, theming integration, and reference-file shape for the data-grid family. Once you review and approve, the same shape can be cloned for:

- `skills/data-grid/devexpress-wpf-tree-list/` — narrower scope (18 source articles, 5 feature areas)
- `skills/data-grid/devexpress-wpf-pivot-grid/` — different feature set (data shaping, OLAP binding, 203 source articles)

Each will take less time because the WPF infrastructure (theming, MVVM patterns, `.NET` vs `.NET Framework` split, designer-vs-code paths) is already documented and reusable.

## Post-Generation Updates

- Fixed the "Using DevExpress Documentation MCP" section in SKILL.md: corrected the tool signature to `devexpress_docs_search(technologies=["WPF"], question=...)` / `devexpress_docs_get_content(url=...)`, removed the "if available" availability hedge, added the cross-agent tool-naming note (host-specific prefixes), and added/verified the untrusted-content security note.
- Bumped the minimum supported modern-.NET floor in SKILL.md's `compatibility:` frontmatter from `.NET 6+` to `.NET 8+` (v26.1 policy; `.NET Framework 4.6.2+` alternative unchanged).

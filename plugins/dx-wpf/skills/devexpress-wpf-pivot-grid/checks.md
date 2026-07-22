# Skill Generation Report — `devexpress-wpf-pivot-grid`

Generated against WPF docs repo HEAD `b16066c633b85ee391d1e6188ffc4cd815ee5e8f` on 2026-05-15.

## Files Created

```
skills/data-grid/devexpress-wpf-pivot-grid/
├── SKILL.md                            (404 lines)
├── checks.md                           (this file)
├── references/
│   ├── getting-started.md              (184 lines, .NET 6/7/8+)
│   ├── getting-started-dotnet-fw.md    (110 lines)
│   ├── data-binding.md                 (189 lines — DataTable, EF, OLAP, server mode, async)
│   ├── data-shaping.md                 (220 lines — aggregation, grouping, sorting, filtering)
│   ├── layout-and-fields.md            (163 lines — areas, groups, customization form)
│   ├── end-user-features.md            (143 lines — drag-drop, drill-down, filter dropdown)
│   └── advanced-features.md            (184 lines — themes, KPI, chart, print/export)
└── examples/
    ├── quickstart-mainview.xaml        (Minimal XAML — just the empty control)
    ├── quickstart-mainwindow.xaml.cs   (Field setup in Window_Loaded)
    ├── quickstart-salesdata.cs         (Sale POCO + 19-record sample data)
    └── quickstart-app.xaml.cs          (Theme setup + Application ambiguity fix)
```

Total: **12 files, ~1600 lines** of generated content. Sits between data-grid (2200 lines) and tree-list (1400 lines) — matches the middle-of-pack scope (203 source articles vs 287 / 18).

## Validation Results

| Check | Result |
|---|---|
| YAML frontmatter valid (`---` delimiters, kebab-case name) | PASS |
| `description` ≤ 1024 chars | PASS (957 chars) |
| `description` has no XML angle brackets | PASS |
| `compatibility` field present, ≤ 500 chars | PASS (468 chars) |
| `metadata.author` set to `DevExpress` | PASS |
| `metadata.version` quoted as YAML string (`"26.1"`) | PASS |
| `metadata.source-commit` set to repo HEAD | PASS (`b16066c633...`) |
| SKILL.md ≤ 500 lines | PASS (404 lines) |
| All `references/*.md` referenced in Navigation Guide exist | PASS (all 6 referenced files present) |
| No `// TODO: Verify API` placeholders in `getting-started.md` | PASS |
| No `// TODO: Verify API` placeholders in `quickstart-*.cs/.xaml` | PASS |
| No `// TODO: Verify API` placeholders in `SKILL.md` | PASS |
| `.NET` and `.NET Framework` getting-started both present | PASS |
| WPF-specific elicitation (designer vs code, theme, data binding mode, areas, aggregation) | PASS — most thorough elicitation of the three skills (11 questions) |
| WPF theming snippet (`ApplicationThemeHelper`) | N/A (handled by separate theme skill) |
| `net{X}-windows` TFM noted | PASS |
| `Application` ambiguity gotcha documented (lesson from data-grid pilot) | PASS |
| **`dxpg:` namespace (vs `dxg:`) called out explicitly** | PASS — flagged in SKILL.md Constraints, Troubleshooting, and Component Overview |

## Placeholders Requiring Verification (9 total — all in deep features)

Per the meta-skill protocol, placeholders in deep feature references are acceptable if documented with risk level. Critical files (getting-started, quickstart, SKILL.md) have **zero** placeholders.

PivotGrid has **more placeholders than data-grid (7) and tree-list (2)** because:
1. Its docs use more `xref:`-only references (Customization Form, KPI, OLAP)
2. Many specialized APIs (window calculations, KPI binding, async mode) are documented but not shown inline in the lessons I read
3. I deliberately flagged borderline calls instead of guessing class names

| File | Line | Topic | Risk | Resolution |
|---|---|---|---|---|
| `references/data-binding.md` | 106 | Exact OLAP setup API (`OlapConnectionString` vs dedicated source class) | Medium | `devexpress_docs_search(query="OLAP cube connection setup")` |
| `references/data-binding.md` | 121 | Exact API for Asynchronous Mode | Medium | `devexpress_docs_search(query="asynchronous mode background thread")` |
| `references/data-shaping.md` | 128 | Top-N limit property name | Low | `devexpress_docs_search(query="limit number of displayed values top N")` |
| `references/data-shaping.md` | 187 | `WindowCalculationBinding` constructor signature | Medium | `devexpress_docs_search(query="WindowCalculationBinding")` |
| `references/layout-and-fields.md` | 55 | Field-group API (`PivotGridGroup` vs `FieldGroup`) | Low | `devexpress_docs_search(query="field groups create programmatically")` |
| `references/advanced-features.md` | 52 | `FormatConditions` collection XAML schema | High | `devexpress_docs_search(query="conditional formatting XAML FormatCondition")` |
| `references/advanced-features.md` | 81 | KPI binding API | High | `devexpress_docs_search(query="key performance indicators KPI OLAP cube")` |
| `references/advanced-features.md` | 95 | Chart integration API | Medium | `devexpress_docs_search(query="integration with chart control DataSource")` |
| `references/advanced-features.md` | 136 | Pivot grid color property names | Low | `devexpress_docs_search(query="customize pivot grid colors")` |

## WPF-Specific Adaptations (per `generating-new-skills.md` § "WinForms and WPF")

All five points from the doc are covered, plus lessons from the data-grid and tree-list pilots:

| Requirement | Where Addressed |
|---|---|
| Designer-first patterns (both designer + code-only) | SKILL.md elicitation Q4; `getting-started-dotnet-fw.md` describes toolbox + Items Source Configuration Wizard path |
| Platform scope: `net{X}-windows` explicit | SKILL.md compatibility + Constraints rule 2 |
| Theming snippet (`ApplicationThemeHelper.ApplicationThemeName`) | N/A (handled by separate theme skill) |
| Data binding **mode** elicitation (In-Memory / Server Mode / OLAP / Asynchronous) | SKILL.md elicitation Q6 — PivotGrid-specific (more options than GridControl) |
| Events coverage | `end-user-features.md` documents drill-down events; `data-shaping.md` notes `CustomSummary` event |
| **Application ambiguity gotcha** | SKILL.md Troubleshooting; `getting-started.md` Step 4; `quickstart-app.xaml.cs` comment |
| **`dxpg:` vs `dxg:` namespace distinction** | Called out 3× — Component Overview, Troubleshooting, Constraints rule 5 (PivotGrid-only concern; previous skills didn't need this) |

## Source Articles Read (Verified Against)

- `articles/controls-and-libraries/pivot-grid.md` (root)
- `articles/controls-and-libraries/pivot-grid/get-started.md`
- `articles/controls-and-libraries/pivot-grid/fundamentals.md`
- `articles/controls-and-libraries/pivot-grid/fundamentals/fields.md`
- `articles/controls-and-libraries/pivot-grid/getting-started/NET-Core/lesson-1-bind-a-pivot-grid-to-an-mdb-database-net.md`
- `articles/controls-and-libraries/pivot-grid/getting-started/` (subdirectory structure)
- `templates/add-pivot-to-the-net-core-wpf-project.md` (template include resolved)

Section-overview articles referenced but not read in full (covered by their root descriptions in `pivot-grid.md`):
- `binding-to-data.md`, `data-shaping.md`, `data-analysis.md`, `layout.md`, `end-user-capabilities.md`, `end-user-interaction.md`, `appearance.md`, `printing-and-exporting.md`

API names verified against `apidoc/`:
- `apidoc/DevExpress.Xpf.PivotGrid/PivotGridControl/` — confirmed: `DataSource`, `DataProcessingEngine`, `Fields`, `BeginUpdate`, `EndUpdate`, `EndUpdateAsync`, `DataSourceChanged`
- `apidoc/DevExpress.Xpf.PivotGrid/PivotGridField/` — confirmed: `Area`, `AreaIndex`, `Caption`, `DataBinding`
- `apidoc/DevExpress.Xpf.PivotGrid/FieldArea/` — confirmed: `ColumnArea`, `DataArea`, `FilterArea`, `RowArea`
- `apidoc/DevExpress.Xpf.PivotGrid/DataSourceColumnBinding/` — confirmed: `ColumnName`, `GroupInterval`, `GroupIntervalNumericRange`

## Template Includes Resolved

- `templates/add-pivot-to-the-net-core-wpf-project.md` — referenced by Lesson 1 (`[!include[<HowToBindToMDB>](~/templates/add-pivot-to-the-net-core-wpf-project.md)]`). Resolved and content reflected in `references/getting-started.md` Step 5 (XAML namespace declaration with `dxpg:`).
- `templates/pivot-design-time-net-5-note.md` — referenced by Lesson 1 (design-time .NET 5 note). Not reproduced; informational only.

No unresolved includes.

## What Differs From the data-grid and tree-list Skills

1. **`DataSource` (not `ItemsSource`)** — fundamental binding API difference, called out 3× in SKILL.md.
2. **`dxpg:` (not `dxg:`)** — different XAML namespace, called out 3×.
3. **Code-behind first** — field configuration (`Area`, `AreaIndex`, `DataBinding`) is typically imperative, not XAML. This is the opposite of `GridControl`, where columns are usually XAML-declared.
4. **4 data binding modes** — In-Memory / Server / OLAP / Asynchronous. The other two skills have only In-Memory + Server.
5. **Aggregation-first model** — `data-shaping.md` is the deepest reference (220 lines), bigger than for the other two skills. Pivot tables ARE aggregation.
6. **No master-detail, no unbound mode, no TreeListNode** — different problem domain.
7. **Larger Troubleshooting section** — includes `dxpg:` mismatch, MDB Engine, dataset-empty (forgot to add fields).
8. **More TODO placeholders** (9 vs 7 / 2) — Pivot Grid's specialized APIs (KPI, chart integration, OLAP) are xref-heavy in the docs and need MCP follow-up to ship precise code.

## Testing — Next Steps

1. **Resolve the 9 placeholders** using the `devexpress_docs_search` queries above. Priority order:
   - High: `FormatConditions` XAML schema, KPI binding (impact most users)
   - Medium: OLAP setup, Async Mode, WindowCalculationBinding, Chart integration
   - Low: Top-N limit, field-group API, color properties
2. **Try the quickstart in a real WPF project**:
   ```bash
   dotnet new wpf -n DevExpressPivotGridQuickstart
   cd DevExpressPivotGridQuickstart
   dotnet add package DevExpress.Wpf.PivotGrid
   # Replace MainWindow.xaml and add MainWindow.xaml.cs / App.xaml.cs / SalesData.cs from examples/
   dotnet build
   dotnet run
   ```
3. **Run the 5-prompt manual test** from the meta-skill.

## Family-Level Status

Current repository status for the WPF data-grid family from the perspective of this report:

| Product | Skill | Status |
|---|---|---|
| **PivotGrid** | **`devexpress-wpf-pivot-grid`** | **✅ Generated, not yet tested in real project** |

Family completion should be evaluated only after the other data-grid family skills are added to this repository. Next family from the original 11-family plan: **editors** (data-editors + property-grid), **charts-and-gauges**, or **scheduling**. Or pause to test PivotGrid in a real project first.

## Post-Generation Updates

- Fixed the "Using DevExpress Documentation MCP" section in SKILL.md: corrected the tool signature to `devexpress_docs_search(technologies=["WPF"], question=...)` / `devexpress_docs_get_content(url=...)`, removed the "if available" availability hedge, added the cross-agent tool-naming note (host-specific prefixes), and added/verified the untrusted-content security note.
- Bumped the minimum supported modern-.NET floor in SKILL.md's `compatibility:` frontmatter from `.NET 6+` to `.NET 8+` (v26.1 policy; `.NET Framework 4.6.2+` alternative unchanged).

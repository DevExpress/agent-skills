# Skill Validation Report — `devexpress-winforms-data-grid`

Validated against the reviewed WPF skill standard. WinForms docs repo HEAD `12b6ef2be47e805fd5fb7c9ce6f27cf705151d1c`.

## Files

```
skills/winforms/devexpress-winforms-data-grid/
├── SKILL.md                            (308 lines)
├── checks.md                           (this file)
├── references/
│   ├── getting-started.md              (156 lines, .NET 6/7/8+)
│   ├── getting-started-dotnet-fw.md    (.NET Framework variant)
│   ├── data-binding.md                 (260 lines)
│   ├── columns.md                      (285 lines)
│   ├── cell-display-and-editing.md     (241 lines)
│   ├── conditional-formatting.md       (214 lines)
│   ├── validation.md                   (199 lines)
│   ├── filtering-and-search.md         (236 lines)
│   ├── sorting-and-grouping.md         (213 lines)
│   ├── master-detail.md                (218 lines)
│   ├── focus-and-selection.md          (192 lines)
│   ├── drag-and-drop.md                (228 lines)
│   ├── printing-and-exporting.md       (172 lines)
│   ├── saving-and-restoring-layout.md  (177 lines)
│   ├── cardview-layout.md              (184 lines)
│   └── treelist-nodes.md               (244 lines)
└── examples/
    └── quickstart.cs                   (104 lines, multi-section reference)
```

Total: **19 files, ~3711 lines**.

## Validation Results

| Check | Result |
|---|---|
| YAML frontmatter valid (`---` delimiters, kebab-case name) | PASS |
| `description` ≤ 1024 chars | PASS (929 chars — trimmed from 2826) |
| `description` has no XML angle brackets | PASS |
| `compatibility` field present, ≤ 500 chars | PASS (422 chars; corrupted em-dash fixed) |
| `metadata.author` set to `DevExpress` | PASS |
| `metadata.version` quoted as YAML string (`"26.1"`) | PASS |
| `metadata.source-commit` set to WinForms docs HEAD | PASS (`12b6ef2…`) |
| SKILL.md ≤ 500 lines | PASS (308 lines) |
| All `references/*.md` referenced in the Navigation Guide exist | PASS (all 15 present) |
| `.NET` and `.NET Framework` getting-started both present | PASS |
| No `// TODO: Verify API` placeholders in any file | PASS (0 found) |
| Examples present (`examples/quickstart.cs`) | PASS |
| Human-in-the-loop elicitation section present | PASS |
| Constraints & Rules section present | PASS |
| MCP integration section present | PASS |

## Notes

- This report records a **compliance check against the reviewed WPF standard**, not an original generation trace — the skill predates this audit.
- Normalization applied during the audit: description trimmed 2826 → 929 chars; corrupted em-dash (`â€"`) in `compatibility` repaired; `source-commit` corrected from the WPF docs commit (`b16066c6`) to the WinForms docs HEAD; the skill moved into `skills/`; an `examples/quickstart.cs` added.
- Coverage spans `GridControl` views (GridView, BandedGridView, AdvBandedGridView, CardView, LayoutView, TileView, WinExplorerView) plus the `TreeList` (see also the dedicated `devexpress-winforms-tree-list` skill).

## Placeholders Requiring Verification

None found (0 `// TODO: Verify API` markers across SKILL.md, references, and examples).

## Source Material

- `articles/controls-and-libraries/data-grid/` (Data Grid feature tree) and `articles/controls-and-libraries/tree-list/`
- [DevExpress.XtraGrid](https://docs.devexpress.com/content/WindowsForms/DevExpress.XtraGrid?md=true), [DevExpress.XtraGrid.Views.Grid](https://docs.devexpress.com/content/WindowsForms/DevExpress.XtraGrid.Views.Grid?md=true), [DevExpress.XtraGrid.Columns](https://docs.devexpress.com/content/WindowsForms/DevExpress.XtraGrid.Columns?md=true), [DevExpress.XtraTreeList](https://docs.devexpress.com/content/WindowsForms/DevExpress.XtraTreeList?md=true)

## Post-Generation Updates

- Fixed the "Using DevExpress Documentation MCP" section in SKILL.md as part of a repo-wide MCP signature/hedge fix: corrected the tool signature to `devexpress_docs_search(technologies=["WindowsForms"], question="...")` (was the broken singular `technology=`/`query=` form), added the cross-agent tool-naming note (host-specific prefixes), and added the untrusted-content security note (none of these three were previously present).

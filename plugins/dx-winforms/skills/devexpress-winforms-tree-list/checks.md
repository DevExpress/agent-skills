# Skill Validation Report — `devexpress-winforms-tree-list`

Generated against the DevExpress WinForms docs repo HEAD `12b6ef2be47e805fd5fb7c9ce6f27cf705151d1c`.
API verified against the local `apidoc/` tree and the DevExpress Docs MCP.

## Files Created

```
plugins/dx-winforms/skills/devexpress-winforms-tree-list/
├── SKILL.md                            (328 lines)
├── checks.md                           (this file)
├── references/
│   ├── getting-started.md              (105 lines, .NET 6/7/8+)
│   ├── getting-started-dotnet-fw.md    (.NET Framework variant)
│   ├── data-binding.md                 (130 lines)
│   ├── nodes.md                        (115 lines)
│   ├── columns-and-editing.md          (113 lines)
│   ├── sorting-filtering-summaries.md  (78 lines)
│   ├── appearance-and-formatting.md    (99 lines)
│   └── printing-and-export.md          (72 lines)
└── examples/
    └── quickstart.cs                   (100 lines, multi-section reference)
```

Total: **11 files, ~1210 lines**.

## Validation Results

| Check | Result |
|---|---|
| YAML frontmatter valid (`---` delimiters, kebab-case name) | PASS |
| `description` ≤ 1024 chars | PASS (1022 chars) |
| `description` has no XML angle brackets | PASS |
| `compatibility` field present, ≤ 500 chars | PASS (354 chars) |
| `metadata.author` set to `DevExpress` | PASS |
| `metadata.version` quoted as YAML string (`"26.1"`) | PASS |
| `metadata.source-commit` set to WinForms docs HEAD | PASS (`12b6ef2…`) |
| SKILL.md ≤ 500 lines | PASS (328 lines) |
| All `references/*.md` referenced in the Navigation Guide exist | PASS (all 7 present) |
| `.NET` and `.NET Framework` getting-started both present | PASS |
| No `// TODO: Verify API` placeholders in any file | PASS (0 found) |
| Examples present (`examples/quickstart.cs`) | PASS |
| Human-in-the-loop elicitation section present | PASS ("Before You Start") |
| Constraints & Rules section present | PASS |
| MCP integration section present | PASS |

## Placeholders Requiring Verification

None. All API names used in `SKILL.md`, `references/`, and `examples/` were verified against the local `apidoc/` tree or confirmed via the DevExpress Docs MCP.

## Key API Verified

- Package `DevExpress.Win.TreeList`; assembly `DevExpress.XtraTreeList.v26.1.dll`; namespace `DevExpress.XtraTreeList`.
- Bound mode: `DataSource`, `KeyFieldName`, `ParentFieldName`, `RootValue`, `OptionsBehavior.PopulateServiceColumns`/`AutoPopulateColumns`, `PopulateColumns()`.
- Unbound/dynamic: `AppendNode(object[], TreeListNode)`, `BeginUnboundLoad`/`EndUnboundLoad`, `TreeListNode.HasChildren`, `BeforeExpand`, `ExportToXml`/`ImportFromXml`.
- Nodes: `Nodes`, `ParentNode`, `Expand`/`Collapse`/`Expanded`, `FocusedNode`, `FindNodeByKeyID`/`FindNodeByFieldValue`/`FindNodeByID`, `GetRowCellValue`/`SetRowCellValue`, `DeleteNode`, `AfterCheckNode`, `NodesIterator`.
- Columns/editing: `TreeListColumn`, `Columns.AddField`, `UnboundExpression`/`UnboundDataType`, `ColumnEdit`, `RepositoryItems`, `OptionsColumn`.
- Data shaping: `SortIndex`/`SortOrder` (type `System.Windows.Forms.SortOrder`), `ActiveFilterString`, `OptionsView.ShowAutoFilterRow`, `ShowFindPanel`/`OptionsFind`, `SummaryFooter`/`SummaryItemType`/`OptionsView.ShowSummaryFooter`.
- Formatting/export: `FormatRules`/`TreeListFormatRule`, `CustomDrawNodeCell`, `ExportToXlsx`/`ExportToPdf`/`ExportToCsv`/`ExportToHtml`, `ShowPrintPreview`, `SaveLayoutToXml`/`RestoreLayoutFromXml`.

## Source Material (Verified Against)

- `articles/controls-and-libraries/tree-list.md` (`xref:2434`) and the Feature Center subtree
- `articles/controls-and-libraries/tree-list/feature-center/data-binding/bound-mode.md` (`xref:116708`)
- `articles/controls-and-libraries/tree-list/feature-center/data-binding/unbound-mode.md` (`xref:5557`)
- `articles/controls-and-libraries/tree-list/feature-center/nodes.md` (`xref:5593`)
- `articles/controls-and-libraries/tree-list/examples/data-binding/how-to-create-treelist-at-runtime.md` (`xref:403837`)
- [DevExpress.XtraTreeList](https://docs.devexpress.com/content/WindowsForms/DevExpress.XtraTreeList?md=true), [DevExpress.XtraTreeList.Columns](https://docs.devexpress.com/content/WindowsForms/DevExpress.XtraTreeList.Columns?md=true), [DevExpress.XtraTreeList.Nodes](https://docs.devexpress.com/content/WindowsForms/DevExpress.XtraTreeList.Nodes?md=true)

## Template Includes Resolved

`unbound-mode.md` references `~/examples/unbound-mode-beforeexpand-event13299.md` (BeforeExpand dynamic loading) and `how-to-create-treelist-at-runtime.md` includes `~/examples/treelist-create-at-runtime.md` — both resolved and read. No unresolved includes.

## Post-Generation Updates

- Fixed the "Using DevExpress Documentation MCP" section in SKILL.md as part of a repo-wide MCP signature/hedge fix: verified the tool signature already used the correct `devexpress_docs_search(technologies=["WindowsForms"], question="...")` form, removed the conditional "If the DevExpress Docs MCP server is available" hedge, and added the cross-agent tool-naming note (host-specific prefixes). The untrusted-content security note was already present and left untouched.
- Bumped the general .NET targeting floor in `SKILL.md` from `.NET 6/7/8+` to `.NET 8+` (compatibility line and the "Target ..." constraint) to match the v26.1 template used across other WinForms skills. The `getting-started.md (105 lines, .NET 6/7/8+)` line in the directory tree above is a frozen point-in-time generation snapshot and was intentionally left unchanged.

# Advanced Features — DevExpress WPF TreeList

This reference collects features beyond the basics: drag-and-drop, conditional formatting, printing/export, design-time tooling, MVVM integration, and the relationship with the wider DevExpress WPF infrastructure.

## When to Use This Reference

Use this when you need to:

- Configure drag-and-drop between trees or external WPF controls
- Apply conditional formatting (color scales, data bars, icon sets, custom criteria)
- Print, preview, or export to XLSX / PDF / HTML
- Use design-time configuration in Visual Studio (Quick Actions / smart tag panel)
- Bind tree state (focused node, selected items) to a ViewModel

## Drag-and-Drop

TreeList supports native drag-and-drop:

- Drag and drop nodes within the same `TreeListControl` (reparent in the tree)
- Drag and drop nodes between two `TreeListControl` instances
- Drag from / to external WPF controls and external applications

```xaml
<dxg:TreeListView AllowDragDrop="True" ShowDragDropHint="True"/>
```

For programmatic control over drag-drop pipeline, handle:

| Event | Trigger |
|---|---|
| `StartRecordDrag` | Drag operation starts |
| `DragRecordOver` | Drag hovers over a target |
| `DropRecord` | User drops on a target |
| `CompleteRecordDragDrop` | Drag-drop operation completes |

> Exact event signatures inherit from `DataViewBase`. Verify member names via DxDocs MCP if needed:
> `devexpress_docs_search(technology="WPF TreeList", query="drag and drop events")`

Source: `articles/controls-and-libraries/tree-list.md` § Drag-and-Drop (https://docs.devexpress.com/content/WPF/11346?md=true).

## Conditional Formatting

TreeList supports the same conditional formatting engine as GridControl: color scales, data bars, icon sets, comparison rules, top/bottom values, above/below average, unique/duplicate, custom criteria.

```xaml
<dxg:TreeListControl>
    <!-- TODO: Verify exact XAML schema for FormatCondition inside TreeListControl.
         The pattern matches GridControl (DevExpress.Xpf.Grid.ConditionalFormatting namespace).
         devexpress_docs_search(technology="WPF TreeList",
                                query="conditional formatting XAML FormatCondition") -->
</dxg:TreeListControl>
```

**Limitations** (per `articles/controls-and-libraries/data-grid/conditional-formatting.md`):

- Only `TreeListView` in **Optimized Mode** supports conditional formatting.
- Conditional formatting does NOT work with **Hierarchical Data Templates** (mode `HierarchicalDataTemplate`).

Source: `articles/controls-and-libraries/tree-list.md` § Conditional Formatting; `articles/controls-and-libraries/data-grid/conditional-formatting.md` (shared engine).

## Printing and Exporting

TreeListControl integrates with the Printing-Exporting library:

```csharp
treeListView1.ShowPrintPreview();
treeListView1.ShowPrintPreviewDialog();

treeListView1.ExportToXlsx("tree.xlsx");
treeListView1.ExportToCsv("tree.csv");
treeListView1.ExportToPdf("tree.pdf");
treeListView1.ExportToHtml("tree.html");
```

| Mode | Formats | What's Preserved |
|---|---|---|
| Data-Aware | XLSX, CSV | Sort / filter / expand state; summaries as Excel formulas |
| WYSIWYG | XLSX, PDF, HTML | Visual appearance, column widths, conditional formatting |

Required package: `DevExpress.Wpf.Printing`.

> Verify exact method overloads (file-path vs. stream, options object types) via DxDocs MCP if needed.

Source: `articles/controls-and-libraries/tree-list.md` § Print and Export (https://docs.devexpress.com/content/WPF/117296?md=true).

## Design-Time Features

In Visual Studio with the Unified Component Installer:

- **Quick Actions / Smart Tag**: Click the smart tag arrow on a TreeListControl to bind `ItemsSource`, add columns, configure the View.
- **Items Source Wizard**: Generates binding + CRUD code for Entity Framework, XPO, OData, and other sources.
- **Toolbox**: Drag `TreeListControl` from the DevExpress toolbox section.

Source: `articles/controls-and-libraries/tree-list/design-time-features.md` and `articles/controls-and-libraries/tree-list/design-time-features/customizing-unbound-treelist.md`.

## MVVM: Bind Tree State

| Bindable Property | What It Holds |
|---|---|
| `DataViewBase.FocusedRow` | The focused data item |
| `DataControlBase.SelectedItem` | Single-select item |
| `DataControlBase.SelectedItems` | Multi-select collection |
| `DataControlBase.CurrentItem` | Active record |

```xaml
<dxg:TreeListControl ItemsSource="{Binding Employees}"
                     CurrentItem="{Binding CurrentEmployee, Mode=TwoWay}"
                     SelectedItem="{Binding SelectedEmployee, Mode=TwoWay}"/>
```

> Verify exact property names against `apidoc/DevExpress.Xpf.Grid/DataControlBase/` if you need to bind less common state.

## Save / Restore Layout

```csharp
treeListControl1.SaveLayoutToXml("treelist-layout.xml");
treeListControl1.RestoreLayoutFromXml("treelist-layout.xml");
```

The serialized state includes column order, widths, sort, filter, and expand state. See `articles/common-concepts/save-and-restore-layouts.md`.

## Compare With Standard WPF TreeView

| Standard `TreeView` | DevExpress `TreeListControl` |
|---|---|
| `TreeView.ItemsSource` (`HierarchicalDataTemplate`) | `TreeListControl.ItemsSource` + `KeyFieldName`/`ParentFieldName` or `ChildNodesPath` |
| Single-column item view | Multi-column tabular view |
| No built-in sorting / filtering | Full sorting, filtering, grouping, summaries |
| No data binding to columns | `TreeListColumn.FieldName` per column |
| Manual TreeViewItem tracking | `TreeListNode` (unbound) or auto-derived from data source |
| `TreeViewItem.IsExpanded` | `TreeListNode.ExpandStateBinding` |
| No virtualization for very large trees | Full row virtualization in Optimized Mode |

## Source Material

- `articles/controls-and-libraries/tree-list.md` (root article — feature overview)
- `articles/controls-and-libraries/tree-list/design-time-features.md`
- `articles/controls-and-libraries/tree-list/concepts.md`
- `articles/controls-and-libraries/data-grid/drag-and-drop.md` (shared events)
- `articles/controls-and-libraries/data-grid/conditional-formatting.md` (shared engine)
- `articles/controls-and-libraries/data-grid/printing-and-exporting.md` (shared library)

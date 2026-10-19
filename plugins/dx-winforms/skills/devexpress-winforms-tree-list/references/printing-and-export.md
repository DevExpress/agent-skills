# Printing and Export

Print the tree, show a print preview, or export it to common formats. Unbound trees can also round-trip through XML.

## When to Use This Reference

- Printing or showing a print preview
- Exporting to XLSX / XLS / CSV / PDF / HTML
- Saving/loading unbound node data as XML

## Prerequisite

Print preview and document export require the `DevExpress.Win.Printing` NuGet package:

```powershell
Install-Package DevExpress.Win.Printing
```

The TreeList is printable directly (it implements `IPrintable`) — no separate `PrintableComponentLink` is required for basic printing.

## Print and Preview

```csharp
// Show the print preview (Ribbon-style preview if DevExpress.Win.Printing is referenced)
treeList.ShowPrintPreview();

// Print directly (with the system print dialog)
treeList.PrintDialog();
```

Check `treeList.IsPrintingAvailable` before printing to verify the printing assemblies are present.

## Export

The control exports its **current view** (visible columns, current sort/filter, expanded nodes) to a file or stream:

```csharp
treeList.ExportToXlsx("tree.xlsx");
treeList.ExportToXls("tree.xls");
treeList.ExportToCsv("tree.csv");
treeList.ExportToPdf("tree.pdf");
treeList.ExportToHtml("tree.html");
```

Each method also has a `Stream` overload and an overload that takes export-options objects (e.g., `XlsxExportOptions`, `PdfExportOptions`) for fine control over the output. Use the MCP tool for the exact options surface:

```
devexpress_docs_search(technologies=["WindowsForms"], question="TreeList ExportToXlsx XlsxExportOptions customize")
```

## Unbound Data as XML

In unbound mode you can persist the node structure and cell data to XML and reload it later:

```csharp
treeList.ExportToXml("data.xml");     // save nodes + data
treeList.ImportFromXml("data.xml");   // reload
```

This is different from layout serialization — `ExportToXml`/`ImportFromXml` round-trips the **data** (unbound nodes), while `SaveLayoutToXml`/`RestoreLayoutFromXml` persist the **layout** (columns, sort, expand state). For layout persistence, use:

```csharp
treeList.SaveLayoutToXml("layout.xml");
treeList.RestoreLayoutFromXml("layout.xml");
```

## Source Material

- [ExportToXlsx](https://docs.devexpress.com/content/WindowsForms/DevExpress.XtraTreeList.TreeList.ExportToXlsx.overloads?md=true), [ExportToPdf](https://docs.devexpress.com/content/WindowsForms/DevExpress.XtraTreeList.TreeList.ExportToPdf.overloads?md=true), [ExportToCsv](https://docs.devexpress.com/content/WindowsForms/DevExpress.XtraTreeList.TreeList.ExportToCsv.overloads?md=true), [ExportToHtml](https://docs.devexpress.com/content/WindowsForms/DevExpress.XtraTreeList.TreeList.ExportToHtml.overloads?md=true), [ShowPrintPreview](https://docs.devexpress.com/content/WindowsForms/DevExpress.XtraTreeList.TreeList.ShowPrintPreview?md=true), [PrintDialog](https://docs.devexpress.com/content/WindowsForms/DevExpress.XtraTreeList.TreeList.PrintDialog?md=true)

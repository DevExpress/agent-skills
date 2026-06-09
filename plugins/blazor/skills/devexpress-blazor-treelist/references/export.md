# Export — Blazor TreeList

When you need to: export tree data to CSV, XLS/XLSX, or PDF; control which rows are exported.

## Export Methods

| Method | Format | Options Class |
|---|---|---|
| `ExportToCsvAsync(fileName, options)` | CSV | `TreeListCsvExportOptions` |
| `ExportToXlsxAsync(fileName, options)` | XLS/XLSX | `TreeListXlExportOptions` |
| `ExportToPdfAsync(fileName, options)` | PDF | `TreeListPdfExportOptions` |

## Basic Export

```razor
<DxTreeList @ref="TreeList" Data="@Tasks" KeyFieldName="Id" ParentKeyFieldName="ParentId">
    <Columns>
        <DxTreeListDataColumn FieldName="Name" />
        <DxTreeListDataColumn FieldName="Status" />
    </Columns>
    <ToolbarTemplate>
        <DxToolbar>
            <DxToolbarItem Text="Export to CSV"  Click="ExportCsv" />
            <DxToolbarItem Text="Export to XLSX" Click="ExportXlsx" />
            <DxToolbarItem Text="Export to PDF"  Click="ExportPdf" />
        </DxToolbar>
    </ToolbarTemplate>
</DxTreeList>

@code {
    ITreeList TreeList;

    async Task ExportCsv()  => await TreeList.ExportToCsvAsync("tasks.csv");
    async Task ExportXlsx() => await TreeList.ExportToXlsxAsync("tasks.xlsx");
    async Task ExportPdf()  => await TreeList.ExportToPdfAsync("tasks.pdf");
}
```

## Control Which Rows Are Exported

### Row Expansion Mode (XLSX)

```csharp
await TreeList.ExportToXlsxAsync("tasks.xlsx", new TreeListXlExportOptions {
    RowExpandMode = TreeListExportRowExpandMode.All       // Export all rows (even collapsed)
    // or
    // RowExpandMode = TreeListExportRowExpandMode.Expanded  // Only expanded rows
});
```

### Selected Rows Only (PDF)

```csharp
await TreeList.ExportToPdfAsync("selected.pdf", new TreeListPdfExportOptions {
    ExportSelectedRowsOnly = true
});
```

## PDF Export with Custom Styles

```csharp
async Task ExportPdfStyled() {
    await TreeList.ExportToPdfAsync("report.pdf", new TreeListPdfExportOptions {
        Landscape = true,
        CustomizeCell = args => {
            if (args.AreaType == DocumentExportAreaType.Header) {
                args.ElementStyle.BackColor = System.Drawing.Color.LightSteelBlue;
            }
            args.Handled = true;
        }
    });
}
```

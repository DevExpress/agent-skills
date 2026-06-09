# Export — Blazor Grid

When you need to: export grid data to CSV, XLS/XLSX, or PDF; customize exported styles; export selected rows only.

## Export Methods

| Method | Format | Options Class |
|---|---|---|
| `ExportToCsvAsync(fileName, options)` | CSV | `GridCsvExportOptions` |
| `ExportToXlsxAsync(fileName, options)` | XLS/XLSX | `GridXlExportOptions` |
| `ExportToPdfAsync(fileName, options)` | PDF | `GridPdfExportOptions` |

## Basic Export

```razor
<DxGrid @ref="Grid" Data="@Items">
    <Columns>
        <DxGridDataColumn FieldName="Name" />
        <DxGridDataColumn FieldName="Price" />
    </Columns>
    <ToolbarTemplate>
        <DxToolbar>
            <DxToolbarItem Text="Export to CSV" Click="ExportCsv" />
            <DxToolbarItem Text="Export to XLSX" Click="ExportXlsx" />
            <DxToolbarItem Text="Export to PDF" Click="ExportPdf" />
        </DxToolbar>
    </ToolbarTemplate>
</DxGrid>

@code {
    IGrid Grid;

    async Task ExportCsv()  => await Grid.ExportToCsvAsync("data.csv");
    async Task ExportXlsx() => await Grid.ExportToXlsxAsync("data.xlsx");
    async Task ExportPdf()  => await Grid.ExportToPdfAsync("data.pdf");
}
```

## PDF Export with Customization

Add `@using DevExpress.Drawing` to the Razor file or `_Imports.razor` to access `DXFont` and `DXFontStyle`.

```razor
@using DevExpress.Drawing

async Task ExportPdfCustomized() {
    await Grid.ExportToPdfAsync("report.pdf", new GridPdfExportOptions {
        // CustomizeDocument: page-level settings (Landscape, Margins, PaperKind, PageSize)
        CustomizeDocument = e => {
            e.Landscape = true;
            e.Margins = new DXMargins(50, 50, 50, 50);
        },
        // CustomizeCell: cell-level style overrides
        CustomizeCell = e => {
            if (e.AreaType == DocumentExportAreaType.Header) {
                e.ElementStyle.BackColor = System.Drawing.Color.LightSteelBlue;
                e.ElementStyle.Font = new DXFont(e.ElementStyle.Font, DXFontStyle.Bold);
            }
            e.Handled = true;
        },
        CustomizeDocumentHeader = e => {
            e.Text = "Sales Report";
            e.ElementStyle.TextAlignment = DevExpress.XtraPrinting.TextAlignment.MiddleCenter;
        }
    });
}
```

### GridPdfExportOptions Key Members

| Member | Type | Description |
|---|---|---|
| `ExportSelectedRowsOnly` | `bool` | Export only selected rows |
| `Landscape` | `bool` | Page orientation |
| `CustomizeDocument` | delegate | Customize overall document settings (font, orientation) |
| `CustomizeCell` | delegate | Customize individual cell styles |
| `CustomizeDocumentHeader` | delegate | Add and style a document header |
| `CustomizeDocumentFooter` | delegate | Add and style a document footer |

### DocumentExportAreaType Values

| Value | Description |
|---|---|
| `Header` | Column header row |
| `DataArea` | Data row |

> `DocumentExportAreaType` is unqualified when used inside a `@code` block of a Razor file (no additional `@using` needed — it comes from `DevExpress.Blazor`).

## XLSX Export Options

`DevExpress.Export.SheetAreaType` and `DevExpress.Export.Xl.XlCellFont` are used for XLSX cell customization. Reference them fully-qualified (no extra `@using` needed).

```csharp
async Task ExportXlsxCustomized() {
    await Grid.ExportToXlsxAsync("data.xlsx", new GridXlExportOptions {
        CustomizeCell = e => {
            // Header row: bold
            if (e.AreaType == DevExpress.Export.SheetAreaType.Header)
                e.Formatting.Font = new DevExpress.Export.Xl.XlCellFont { Bold = true };
            // Data rows: color by condition
            if (e.AreaType == DevExpress.Export.SheetAreaType.DataArea)
                e.Formatting.BackColor = System.Drawing.Color.LightBlue;
            e.Handled = true;
        }
    });
}
```

## CSV Export with Encoding

```razor
async Task ExportCsvUtf8() {
    await Grid.ExportToCsvAsync("data.csv", new GridCsvExportOptions {
        Encoding = System.Text.Encoding.UTF8
    });
}
```

## Export Selected Rows Only

```razor
<DxGrid @ref="Grid"
        Data="@Items"
        KeyFieldName="Id"
        SelectionMode="GridSelectionMode.Multiple"
        @bind-SelectedDataItems="@SelectedItems">
    <Columns>
        <DxGridSelectionColumn />
        <DxGridDataColumn FieldName="Name" />
    </Columns>
    <ToolbarTemplate>
        <DxToolbar>
            <DxToolbarItem Text="Export Selected to PDF" Click="ExportSelected" />
        </DxToolbar>
    </ToolbarTemplate>
</DxGrid>

@code {
    IGrid Grid;
    IReadOnlyList<object> SelectedItems { get; set; } = new List<object>();

    async Task ExportSelected() {
        await Grid.ExportToPdfAsync("selected.pdf", new GridPdfExportOptions {
            ExportSelectedRowsOnly = true
        });
    }
}
```

## Export to Other Formats (via DevExpress Reporting)

You can use DevExpress Reporting tools to export to DOCX, HTML, MHT, and other formats. See the GitHub examples:
- Server: https://github.com/DevExpress-Examples/blazor-server-grid-export
- WASM: https://github.com/DevExpress-Examples/blazor-webassembly-grid-export

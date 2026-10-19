# Export Options — DevExpress Dashboard (all platforms)

Option classes live in `DevExpress.DashboardCommon` and are used in two different places — pick the one that matches the task:

- **Default options for the UI "Export To" button** (end user clicks it, dialog pre-fills with these values) — set once at startup/initialization, per platform: see **Set Default UI Export Options** below.
- **Explicit options for a single programmatic export call** (`AspNetCoreDashboardExporter`, WinForms/WPF `ExportToPdf(...)`, non-visual `DashboardExporter`) — the options object is always the **last** argument to the export method (after `state`); see the exporter-specific references ([aspnetcore-export.md](aspnetcore-export.md), [winforms-wpf-export.md](winforms-wpf-export.md)).

A task like "enable parameter visibility in the exported PDF" at the *application/view level* (no export controller involved) is the first case — do not add an export controller/exporter call for it.

## `DashboardPdfExportOptions`

| Property | Type | Description |
|----------|------|-------------|
| `ExportFilters` | `bool` | Include active master-filter values in the document |
| `ExportParameters` | `bool` | Include parameter values in the document |
| `IncludeHiddenParameters` | `bool` | Also include parameters hidden from the dialog. Requires `ExportParameters`; both default `false` |
| `DashboardStatePosition` | `DashboardStateExportPosition` | Where filter/parameter info goes: `Below` or `SeparatePage` |
| `ShowTitle` | `bool` | Include the dashboard title / item caption |
| `ScaleFactor` | `double` | Scale factor (fraction of 1) |

> **`ExportParameters` + `IncludeHiddenParameters` publishes hidden parameter values into the document.** Same pair on the Image and Excel options. The switch also exists on the **client-side** export extension options, so treat the `false` defaults as defaults, not as a control you own — the durable mitigation is not to put anything sensitive in a parameter.


```csharp
var pdf = new DashboardPdfExportOptions {
    ExportFilters = true,
    DashboardStatePosition = DashboardStateExportPosition.Below
};
```

Page size/orientation (`PageLayout`, `PaperKind`) and per-item size modes exist too — fetch the class page via MCP for the full list.

## `DashboardImageExportOptions`

| Property | Type | Description |
|----------|------|-------------|
| `Format` | `DashboardExportImageFormat` | `Png`, `Jpeg`, `Svg`, `Gif` (default `Png`) |
| `Resolution` | `int` | DPI (default 96) |
| `ShowTitle` | `bool` | Include the dashboard title / item caption |
| `ExportFilters` / `ExportParameters` | `bool` | Include filter / parameter values |

## `DashboardExcelExportOptions`

| Property | Type | Description |
|----------|------|-------------|
| `Format` | `ExcelFormat` | `Xlsx`, `Xls`, or `Csv` |
| `DashboardStatePosition` | `DashboardStateExcelExportPosition` | `Below` or `SeparateSheet` |
| `ExportFilters` / `ExportParameters` | `bool` | Include filter / parameter values |
| `CsvValueSeparator` | `char` | Value separator when `Format = ExcelFormat.Csv` |

> **Two different state-position enums.** PDF and Image use `DashboardStateExportPosition` (`Below` / `SeparatePage`). Excel uses `DashboardStateExcelExportPosition` (`Below` / `SeparateSheet`). They are not interchangeable — using the wrong one will not compile.

For the complete property list of any options class, use dxdocs MCP server:

```
devexpress_docs_search(technologies=["Dashboard"], question="DashboardPdfExportOptions properties")
devexpress_docs_search(technologies=["Dashboard"], question="DashboardExcelExportOptions properties")
devexpress_docs_search(technologies=["Dashboard"], question="DashboardImageExportOptions properties")
```

---

## Set Default UI Export Options (per platform)

These configure what pre-fills when the end user clicks the built-in **Export To** button — no controller or exporter class involved.

### ASP.NET Core

Set via the `DashboardExport` extension's `PdfExportOptions`/`ImageExportOptions`/`ExcelExportOptions` builders — the same extension that toggles `AllowExportDashboard`/`AllowExportDashboardItems` ([aspnetcore-export.md](aspnetcore-export.md) → Enable/Disable the Export UI):

```cshtml
@(Html.DevExpress().Dashboard("dashboardControl1")
    .ControllerName("DefaultDashboard")
    .Width("100%")
    .Height("100%")
    .Extensions(ext => {
        ext.DashboardExport(opt => {
            opt.PdfExportOptions(pdf => { pdf.ExportParameters = true; });
        });
    })
)
```

### Blazor

Set via the `DxDashboardExport` options' `DxPdfExportOptions`/`DxImageExportOptions`/`DxExcelExportOptions`:

```razor
<DxDashboard Endpoint="api/dashboard" style="width: 100%; height: 100%;">
    <DxExtensions>
        <DxDashboardExport>
            <DxExcelExportOptions ExportParameters="true" />
        </DxDashboardExport>
    </DxExtensions>
</DxDashboard>
```

### JavaScript / Angular / React / Vue

Supply the options under the `dashboardExport` key of the control's `extensions` object — `allowExportDashboard` / `allowExportDashboardItems` toggle the export UI, the `*ExportOptions` members set the dialog defaults:

```javascript
var dashboardControl = new DashboardControl(document.querySelector("#dashboard-container"), {
    endpoint: "https://demos.devexpress.com/services/dashboard/api",
    extensions: {
        dashboardExport: {
            allowExportDashboard: true,
            excelExportOptions: { Format: "Xlsx", DashboardStatePosition: "SeparateSheet" }
        }
    }
});
```

> **Extension keys are camelCase; the option members inside them are PascalCase.** `pdfExportOptions` / `imageExportOptions` / `excelExportOptions` follow the usual client-option casing, but their *contents* mirror the .NET class properties — `Format`, `DashboardStatePosition`, `ExportFilters`, `FontInfo`. Match that casing exactly; do not camelCase the inner members by analogy with the outer keys.

Full member list (including the `onExportDialog*` handlers and the runtime `get`/`setPdfExportOptions` accessors):
`devexpress_docs_search(technologies=["Dashboard"], question="DashboardExportExtensionOptions members")`

The options object is the same across jQuery/JS, Angular, React, and Vue — only the mechanism for handing it to the control differs (constructor options vs. wrapper-component bindings). Per-framework extension registration:
- React: https://docs.devexpress.com/content/Dashboard/402280/web-dashboard/integrate-dashboard-component/dashboard-component-for-react/manage-extensions?md=true
- Vue: https://docs.devexpress.com/content/Dashboard/402557/web-dashboard/integrate-dashboard-component/dashboard-component-for-vue/manage-extensions?md=true
- Angular: https://docs.devexpress.com/content/Dashboard/402279/web-dashboard/integrate-dashboard-component/dashboard-component-for-angular/manage-extensions?md=true
- jQuery / Knockout / plain JS: https://docs.devexpress.com/content/Dashboard/403350/web-dashboard/integrate-dashboard-component/dashboard-control-for-javascript-applications-jquery-knockout-etc/manage-extensions?md=true

### WinForms / WPF

Set directly on the designer/viewer/control instance — see [winforms-wpf-export.md](winforms-wpf-export.md) → **Default Export Options** (`dashboardDesigner1.PdfExportOptions.ExportFilters = true`, etc.).
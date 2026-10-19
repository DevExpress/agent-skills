# Export with Parameter State

The parameter values in effect at export time decide what the exported document contains. This reference covers only the **parameter side** of exporting — for formats, options, scheduling, and email, use the **`devexpress-dashboard-export`** skill.

---

## The Rule: Who Holds the State

What matters is not the platform but **what you export from**:

| Export path | Where the state comes from | Do you pass a `DashboardState`? |
|---|---|---|
| A **control** — `DashboardViewer`, `DashboardDesigner`, WPF `DashboardControl` | The control itself: the user's current parameters, master filters, and drill-down | **No.** `ExportTo…` already uses them |
| The Web Dashboard's own **Export To** button | The client sends its current state with the request | **No.** Nothing to write |
| An **exporter class** — `AspNetCoreDashboardExporter`, or the non-visual `DashboardExporter` | The dashboard loads fresh with no control or user session context | **Yes.** `new DashboardState()` is empty, meaning "filters cleared, parameters at their defaults" |

Set state on a control only to export values **different** from what is on screen — it applies to the live control, so the user sees the change too.

`CustomParameters` fires for export data requests on every path, so a hidden parameter set from the authenticated identity cannot be bypassed by an export.

### Printing the values into the document is a separate switch

Applying parameter values decides **which data** is exported. Whether the values are *printed* in the document is controlled separately, and the two options are not interchangeable:

| Option | Prints |
|--------|--------|
| `DashboardPdfExportOptions.ExportParameters` | the **parameter** values |
| `DashboardPdfExportOptions.ExportFilters` | the **filter** state (master filters, item filters) |

Both default to `false`, so a PDF exported with parameters applied shows the filtered data but no parameter summary until you set `ExportParameters = true`. `DashboardExcelExportOptions` and `DashboardImageExportOptions` expose the same pair.

---

## Namespaces

```csharp
using DevExpress.DashboardCommon;       // DashboardState, DashboardParameterState,
                                        // DashboardPdfExportOptions, DashboardExcelExportOptions
using DevExpress.DashboardWin;          // DashboardViewer, DashboardDesigner (WinForms)
using DevExpress.DashboardWpf;          // DashboardControl (WPF)
using DevExpress.DashboardAspNetCore;   // AspNetCoreDashboardExporter
```

No `DevExpress.Export.ExportOptions` type or `ExportDashboard(options)` method exists on the dashboard controls.

---

## Desktop: Export What the User Sees

No parameter code. The control exports its current state; the only decision is whether to print the values in the document:

```csharp
using System;
using DevExpress.DashboardCommon;
using DevExpress.DashboardWin;

private void ExportButton_Click(object sender, EventArgs e) {
    var options = new DashboardPdfExportOptions();
    options.ExportParameters = true;   // print the parameter values in the document
    options.ExportFilters = true;      // print the filter state as well (separate switch)

    // Path overload; a Stream overload exists as well.
    dashboardViewer1.ExportToPdf(@"C:\Reports\Dashboard.pdf", options);
}
```

`DashboardStatePosition` moves that summary; it already defaults to `Below`.

The same `ExportToPdf` / `ExportToImage` / `ExportToExcel` methods exist on `DashboardDesigner` (WinForms) and `DashboardControl` (WPF).

## Desktop: Export Values Other Than the Displayed Ones

Only when the export must differ from what is on screen. Change the values on the control, then export:

```csharp
using System;
using DevExpress.DashboardCommon;
using DevExpress.DashboardWin;

private void ExportFiltered_Click(object sender, EventArgs e) {
    dashboardViewer1.BeginUpdateParameters();
    dashboardViewer1.Parameters["yearParameter"].SelectedValue = "2023";
    dashboardViewer1.EndUpdateParameters();

    dashboardViewer1.ExportToPdf(@"C:\Reports\Dashboard_2023.pdf", new DashboardPdfExportOptions());
}
```

`SetDashboardState` works too and is the WPF route (WPF has no parameter descriptors). Use a `List<T>` in `DashboardParameterState.Value` for a multi-value parameter:

```csharp
using System.Collections.Generic;
using DevExpress.DashboardCommon;

DashboardState exportState = dashboardControl.GetDashboardState();   // start from the current state
foreach (DashboardParameterState parameterState in exportState.Parameters) {
    if (parameterState.Name == "categoryParameter") {
        parameterState.Value = new List<string> { "Beverages", "Produce" };
    }
}
dashboardControl.SetDashboardState(exportState);
dashboardControl.ExportToPdf(@"C:\Reports\Dashboard.pdf");
```

Both apply to the live control, so capture the previous state first if you need to restore it.

---

## ASP.NET Core: Export From Your Own Endpoint

This is the path that **does** need a state, because `AspNetCoreDashboardExporter` loads the dashboard fresh:

```csharp
using System.IO;
using DevExpress.DashboardAspNetCore;
using DevExpress.DashboardCommon;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// Inside a controller whose constructor injects DashboardConfigurator _configurator
// and IWebHostEnvironment _hostEnvironment.
// [Authorize] only checks sign-in. The storage check below decides which dashboards this user may export.
[Authorize]
[HttpPost]
public IActionResult ExportToPdf(string dashboardId) {
    DashboardState state = new DashboardState();
    state.Parameters.Add(new DashboardParameterState {
        Name = "yearParameter", Type = typeof(string), Value = "2023"
    });

    var exporter = new AspNetCoreDashboardExporter(_configurator, _hostEnvironment);
    using (var stream = new MemoryStream()) {
        exporter.ExportToPdf(dashboardId, stream,
            new System.Drawing.Size(1920, 1080), state, new DashboardPdfExportOptions());
        return File(stream.ToArray(), "application/pdf", $"{dashboardId}.pdf");
    }
}
```

> **Restrict which dashboards the endpoint may export.** `dashboardId` arrives from the caller, and `DashboardFileStorage` loads any id whose file exists — it applies no access control. Put the check in the dashboard storage: filter `GetAvailableDashboardsInfo()` by the current user and throw from `LoadDashboard` when the requested id is not in that list — fetch `devexpress_docs_get_content(url="https://docs.devexpress.com/Dashboard/402924/web-dashboard/integrate-dashboard-component/dashboard-backend/manage-multi-tenancy")`. The exporter loads the dashboard through that same storage, so one check covers the viewer and any export that loads by `dashboardId`. If you export with the non-visual `DashboardExporter`, check access yourself before you load the `Dashboard`.

To export what a specific user is looking at, capture `getDashboardState()` on the client and post it in — an empty state means defaults, not "the current view". (This is the same point the export skill makes as its Rule 3.)

> A parameter injected via a request header reaches data requests but not exports on the Ajax remote service, which exports through `form.submit`. Use the Fetch remote service.

---

## Troubleshooting

| Issue | Cause | Solution |
|-------|-------|----------|
| `ExportOptions` / `ExportDashboard` not found | Those APIs do not exist | Use `ExportToPdf`/`ExportToImage`/`ExportToExcel` with `Dashboard*ExportOptions` |
| Exported document shows unfiltered data | Exported through an exporter class with an empty `DashboardState` | Pass the user's state, or rely on `CustomParameters` |
| Values changed after export on a control | State applied to the live control, which the user keeps looking at | Expected; restore the previous state after exporting |
| Multi-value parameter exports one value | Scalar in `DashboardParameterState.Value` | Use a `List<T>` |
| Parameter values missing from the exported document | `ExportParameters` is off (it defaults to off, and `ExportFilters` does not cover parameters) | Set `DashboardPdfExportOptions.ExportParameters = true`; add `ExportFilters = true` for the filter state |

---

## Related

- **`devexpress-dashboard-export` skill** — formats, options, server-side and non-visual export (`DashboardExporter`), scheduling, email
- [winforms-wpf-runtime.md](winforms-wpf-runtime.md) — applying parameter values on the desktop
- [web-parameters.md](web-parameters.md) — applying parameter values in ASP.NET Core

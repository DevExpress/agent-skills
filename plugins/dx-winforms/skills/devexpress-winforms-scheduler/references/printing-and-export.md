# Printing and Exporting the Schedule

The WinForms Scheduler prints and previews through the DevExpress **XtraPrinting Library**. `SchedulerControl` exposes direct `Print()` / `ShowPrintPreview()` methods for quick output, a set of print styles for layout, and a `SchedulerPrintAdapter` for full report-based printing and export.

## When to Use This Reference

- Print the current schedule or show a Print Preview from code or a toolbar button.
- Let end users adjust page setup (margins, orientation, printed time range).
- Select the print style (Day / Week / Month / Details / Memo / Trifold layout).
- Export the schedule to PDF, image, HTML, or XLSX.

## Prerequisites

Printing requires the XtraPrinting library (`DevExpress.XtraPrinting.v26.1.dll`, provided by the `DevExpress.Win.Printing` package, which most DevExpress Win packages reference transitively). Always guard printing calls with `IsPrintingAvailable`:

```csharp
if (!schedulerControl1.IsPrintingAvailable) {
    MessageBox.Show("The DevExpress.XtraPrinting library is not available.");
    return;
}
```

## Print and Print Preview

```csharp
using DevExpress.XtraScheduler;

// Send directly to the default printer
schedulerControl1.Print();

// Show the Print Preview window
schedulerControl1.ShowPrintPreview();

// Show the Page Setup dialog (margins, orientation, printed time range, …)
schedulerControl1.ShowPrintOptionsForm();
```

`SchedulerControl.OptionsPrint` (a `SchedulerOptionsPrint`) holds the print settings in code, including the print-style kind via `OptionsPrint.PrintStyle`.

## Print Styles

A print style defines the printed layout. The built-in styles (Day, Week, Month, Details, Memo, Trifold) are held in `SchedulerControl.PrintStyles`; the one used for output is `SchedulerControl.ActivePrintStyle`. There is also a `ShowPrintPreview(SchedulerPrintStyle)` overload (`DevExpress.XtraScheduler.Printing.SchedulerPrintStyle`) to preview with a specific style without changing `ActivePrintStyle`.

```csharp
// Preview with the currently active print style
schedulerControl1.ShowPrintPreview();
```

## Report-Based Printing and Export

For full layout control and for export, bind a scheduler report to the control's `SchedulerPrintAdapter`:

```csharp
// myReport is an XtraReport-based ISchedulerReport (DevExpress.XtraScheduler.Reporting)
schedulerControl1.SchedulerPrintAdapter.AssignToReport(myReport);
```

The adapter composes the data set the report renders and lets you filter appointments/resources before they reach the report. From the **Print Preview** window (or the report's document preview), end users can **export to PDF, image, HTML, or XLSX** via the built-in export commands — the scheduler itself has no separate `ExportToPdf` method; export flows through the preview/printing system.

## Common Issues

| Symptom | Likely Cause | Fix |
|---|---|---|
| `Print()` / `ShowPrintPreview()` do nothing or throw | XtraPrinting library missing | Reference the `DevExpress.Win.Printing` package; guard with `IsPrintingAvailable` |
| Printed time range is wrong | Print interval not configured | Set it via `OptionsPrint`, or let the user pick it in `ShowPrintOptionsForm()` |
| Wrong layout on paper | Unintended print style active | Set `ActivePrintStyle` from `PrintStyles` (or pass a `SchedulerPrintStyle` to `ShowPrintPreview`) before printing |
| Need PDF/XLSX output | Looking for a control-level export method | Export from the Print Preview window, or render a scheduler report via `SchedulerPrintAdapter` |

## Source Material

- Print a Scheduler and show its Print Preview — `https://docs.devexpress.com/WindowsForms/2270`
- `DevExpress.XtraScheduler.SchedulerControl.Print` (`xref:DevExpress.XtraScheduler.SchedulerControl.Print`)
- `DevExpress.XtraScheduler.SchedulerControl.ShowPrintPreview` (`xref:DevExpress.XtraScheduler.SchedulerControl.ShowPrintPreview`)
- `DevExpress.XtraScheduler.SchedulerControl.SchedulerPrintAdapter` (`xref:DevExpress.XtraScheduler.SchedulerControl.SchedulerPrintAdapter`)
- `DevExpress.XtraScheduler.Printing.SchedulerPrintStyle` (`xref:DevExpress.XtraScheduler.Printing.SchedulerPrintStyle`)

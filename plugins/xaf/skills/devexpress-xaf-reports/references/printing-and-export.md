# Printing Without Preview & Data Export — Code Snippets

## Printing Without Preview

Create an abstract controller in the module project, then implement platform-specific descendants:

```csharp
// Platform-agnostic (Module project)
public abstract class PrintContactsController : ObjectViewController<ListView, Contact> {
    public PrintContactsController() {
        var action = new SimpleAction(this, "PrintContacts", PredefinedCategory.Reports);
        action.ImageName = "Action_Printing_Print";
        action.Execute += (s, e) => PrintReport("Contacts Report", e.SelectedObjects);
    }

    protected abstract void PrintReport(string reportName, IList selectedObjects);
}

// WinForms implementation (Win project)
public class WinPrintContactsController : PrintContactsController {
    readonly IReportExportService reportExportService;

    [ActivatorUtilitiesConstructor]
    public WinPrintContactsController(IServiceProvider serviceProvider) : base() {
        reportExportService = serviceProvider.GetRequiredService<IReportExportService>();
    }

    protected override void PrintReport(string reportName, IList selectedObjects) {
        using XtraReport report = reportExportService.LoadReport<ReportDataV2>(
            r => r.DisplayName == reportName);

        CriteriaOperator criteria = ((BaseObjectSpace)ObjectSpace)
            .GetObjectsCriteria(((ObjectView)View).ObjectTypeInfo, selectedObjects);

        SortProperty[] sortProperties = { };
        reportExportService.SetupReport(report, criteria.ToString(), sortProperties);
        report.PrintDialog();
    }
}
```

## ExportController and Export Formats

XAF includes built-in `ExportController` with an **Export to** action.

```csharp
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.SystemModule;

public class TriggerExportController : ViewController<ListView> {
    protected override void OnActivated() {
        base.OnActivated();

        var exportController = Frame.GetController<ExportController>();
        if (exportController == null) {
            return;
        }

        // ExportController is active only for supported List Views.
        // Execute the built-in action programmatically when needed.
        exportController.ExportAction.DoExecute();
    }
}
```

Supported formats (depend on platform and installed DevExpress assemblies):

| Format | Extension |
|---|---|
| XLSX | `.xlsx` |
| XLS | `.xls` |
| CSV | `.csv` |
| PDF | `.pdf` |
| HTML | `.html` |
| MHT | `.mht` |
| RTF | `.rtf` |
| DOCX | `.docx` |
| Text | `.txt` |
| Image | `.png` / `.jpg` |

Typical platform coverage:
- Blazor: CSV, XLS, XLSX, PDF
- WinForms: CSV, XLS, XLSX, PDF, HTML, MHT, RTF, DOCX, Text, Image

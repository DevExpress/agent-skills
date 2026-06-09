# Report Preview from Code — Code Snippets

## Invoking Report Preview

```csharp
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.ReportsV2;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.EF;
using Microsoft.Extensions.DependencyInjection;

public class ShowReportController : ObjectViewController<ListView, Contact> {
    public ShowReportController() {
        var action = new SimpleAction(this, "ShowContactReport", PredefinedCategory.Reports);
        action.Execute += Action_Execute;
    }

    private void Action_Execute(object sender, SimpleActionExecuteEventArgs e) {
        var reportController = Frame.GetController<ReportServiceController>();
        if (reportController == null) {
            return;
        }

        var reportStorage = Application.ServiceProvider.GetRequiredService<IReportStorage>();

        using var os = Application.CreateObjectSpace(typeof(ReportDataV2));
        var reportData = os.FirstOrDefault<ReportDataV2>(d => d.DisplayName == "Contacts Report");
        if (reportData == null) {
            return;
        }

        // Handle is a string identifier for ShowPreview
        string handle = reportStorage.GetReportContainerHandle(reportData);

        // Show preview
        reportController.ShowPreview(handle);
    }
}
```

## Filter Report Data with CriteriaOperator

```csharp
// Filter by selected objects
CriteriaOperator selectedCriteria = ((BaseObjectSpace)ObjectSpace)
    .GetObjectsCriteria(View.ObjectTypeInfo, e.SelectedObjects);
reportController.ShowPreview(handle, selectedCriteria);

// Or use custom criteria (parameterized)
CriteriaOperator userCriteria = CriteriaOperator.Parse("[Department.Name] = ?", "Sales");
reportController.ShowPreview(handle, userCriteria);
```

When using `CollectionDataSource`, assign criteria on the data source before preview (for example in `OnBeforeShowPreview`) so filtering happens at the source.

Never build criteria from raw user input by string concatenation; use parameterized `CriteriaOperator.Parse("Prop = ?", value)` and convert to string only when required by the API.

## Load XtraReport from ReportDataV2

```csharp
using DevExpress.XtraReports.UI;

var reportStorage = application.ServiceProvider.GetRequiredService<IReportStorage>();
using var os = application.CreateObjectSpace(typeof(ReportDataV2));
var reportData = os.FirstOrDefault<ReportDataV2>(d => d.DisplayName == "Sales Report");
if (reportData != null) {
    XtraReport report = reportStorage.LoadReport(reportData);

    // Programmatic customization before preview/export
    report.ExportOptions.Pdf.NeverEmbedFonts = true;
}
```

## Filter CollectionDataSource in OnBeforeShowPreview

```csharp
builder.Modules.AddReports(options => {
    options.ReportDataType = typeof(ReportDataV2);
    options.Events.OnBeforeShowPreview = context => {
        var report = context.Report;

        var dataSource = report.ComponentStorage
            .OfType<CollectionDataSource>()
            .FirstOrDefault();

        if (dataSource != null) {
            Guid userId = /* resolve current user id */ Guid.Empty;
            dataSource.Criteria = CriteriaOperator
                .Parse("[AssignedTo.Oid] = ?", userId)
                .ToString();
        }
    };
});
```

Setting report parameters and setting `CollectionDataSource.Criteria` are independent operations.

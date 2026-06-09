# Predefined & In-Place Reports — Code Snippets

## Register Predefined Reports in Module

Add an XtraReport via the **DevExpress Report** template. Use `CollectionDataSource` or `ViewDataSource` and set `ObjectTypeName` to the target business class.

`PredefinedReportsUpdater` is created with `Application`, `objectSpace`, and `versionFromDB` in `GetModuleUpdaters`. `AddPredefinedReport<TReport>(displayName, dataType, isInplaceReport)` then registers the compiled report type for a business object type.

```csharp
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.ReportsV2;
using DevExpress.Persistent.BaseImpl.EF;

public sealed class MyModule : ModuleBase {
    public override IEnumerable<ModuleUpdater> GetModuleUpdaters(
        IObjectSpace objectSpace, Version versionFromDB) {

        var updater = new DatabaseUpdate.Updater(objectSpace, versionFromDB);
        var reportsUpdater = new PredefinedReportsUpdater(
            Application, objectSpace, versionFromDB);

        // Guard against duplicates (updaters run on each startup)
        bool invoiceExists = objectSpace.FirstOrDefault<ReportDataV2>(
            r => r.DisplayName == "Invoice") != null;

        if (!invoiceExists) {
            reportsUpdater.AddPredefinedReport<InvoiceReport>(
                "Invoice", typeof(Invoice), isInplaceReport: true);
        }

        bool contactsExists = objectSpace.FirstOrDefault<ReportDataV2>(
            r => r.DisplayName == "Contacts Report") != null;

        if (!contactsExists) {
            reportsUpdater.AddPredefinedReport<ContactReport>(
                "Contacts Report", typeof(Contact));
        }

        return new ModuleUpdater[] { updater, reportsUpdater };
    }
}
```

Predefined reports are read-only for end users. Users can use **Copy Predefined Report** Action to create an editable copy.

## In-Place Reports and ShowInReport

In-place reports are linked to a specific business type. When a user selects objects in a List View or opens a Detail View, the **ShowInReport** action renders the report for the current context.

- Set `IReportDataV2.IsInplaceReport = true` on the report data object
- Or register with `isInplaceReport: true` in `AddPredefinedReport`
- Ensure `options.EnableInplaceReports = true` in `AddReports`

```csharp
reportsUpdater.AddPredefinedReport<OrderReport>(
    "Order Details", typeof(Order), isInplaceReport: true);
```

The `PrintSelectionBaseController` manages the ShowInReport action and its items.

```csharp
// Trigger built-in ShowInReport action programmatically when controller/action are active
var reportController = Frame.GetController<ReportServiceController>();
if (reportController?.ShowInReportAction != null && reportController.ShowInReportAction.Active) {
    reportController.ShowInReportAction.DoExecute();
}

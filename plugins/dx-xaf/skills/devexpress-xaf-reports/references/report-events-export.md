# Report Events & Export Options — Code Snippets

## Report Events in AddReports

```csharp
using DevExpress.Persistent.BaseImpl.EF;

builder.Modules
    .AddReports(options => {
        options.ReportDataType = typeof(ReportDataV2);

        // Fired when the report is loaded from storage (before preview)
        options.Events.OnReportLoaded = context => {
            var report = context.Report;
            // Post-load setup
        };

        // Fired after load, immediately before preview is shown
        options.Events.OnBeforeShowPreview = context => {
            var report = context.Report;

            // Access scoped services
            var previewContext = context.ServiceProvider.GetService<ReportPreviewContext>();
            if (previewContext != null && report.Parameters["MyParam"] != null) {
                report.Parameters["MyParam"].Value = previewContext.ParameterValue;
            }

            // Export options are customized per report instance
            report.ExportOptions.Pdf.NeverEmbedFonts = true;
            report.ExportOptions.Xlsx.RawDataMode = false;
        };
    });
```

## Customizing Export Options

```csharp
// Reusable helper
public static class ExportConfigurator {
    public static void Setup(ExportOptions exportOptions) {
        exportOptions.Pdf.DocumentOptions.Title = "My Report";
        exportOptions.Pdf.NeverEmbedFonts = true;
        exportOptions.Xlsx.SheetName = "Report Data";
        exportOptions.Xls.ShowGridLines = true;
        exportOptions.Html.EmbedImagesInHTML = true;
    }
}

// Apply in OnBeforeShowPreview
options.Events.OnBeforeShowPreview = context => {
    ExportConfigurator.Setup(context.Report.ExportOptions);
};
```

`OnReportLoaded` is for post-load initialization. `OnBeforeShowPreview` is for per-invocation runtime customization such as user-specific parameters and criteria.

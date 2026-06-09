# Report Parameters — Code Snippets

## Passing Parameters via Scoped Service

```csharp
// 1. Define a scoped parameter context
public class ReportPreviewContext {
    public object ParameterValue { get; set; }
}

// 2. Register as scoped service
services.AddScoped<ReportPreviewContext>();
```

```csharp
// 3. Populate the scoped context in a controller before preview
using DevExpress.ExpressApp.ReportsV2;

private readonly ReportPreviewContext previewContext;

public ShowReportController(ReportPreviewContext previewContext) {
    this.previewContext = previewContext;
}

private void Action_Execute(object sender, SimpleActionExecuteEventArgs e) {
    previewContext.ParameterValue = "CustomValue";

    var reportController = Frame.GetController<ReportServiceController>();
    // ... resolve handle from IReportStorage
    reportController?.ShowPreview(handle);
}
```

```csharp
// 4. Read the scoped value in OnBeforeShowPreview
builder.Modules
    .AddReports(options => {
        options.ReportDataType = typeof(ReportDataV2);
        options.Events.OnBeforeShowPreview = context => {
            var scopedContext = context.ServiceProvider.GetService<ReportPreviewContext>();
            if (scopedContext != null) {
                context.Report.Parameters["MyParam"].Value = scopedContext.ParameterValue;
            }
        };
    });
```

Use scoped lifetime for per-request or per-preview parameter state. Avoid static fields or singleton state for user-specific values.

# Parameters — DevExpress Reporting for Angular

## When to Use This Reference

Use when the developer needs to pass parameter values to reports from the Angular UI, handle the ParametersInitialized event, or build a custom parameter submission UI.

## Parameters Panel (Built-In)

The Document Viewer shows the Parameters panel automatically when a report has visible parameters. Users fill in values and click Submit.

No code is needed to enable this — it works by default when the report defines parameters with `Visible = true`.

## Submitting Parameters Programmatically via URL

The simplest approach: encode parameters in the `reportUrl` string. The backend parses them in a custom `IReportProvider`.

### Angular component

```typescript
// report-viewer.ts
import { Component, Inject, ViewChild, ViewEncapsulation } from '@angular/core';
import { DxReportViewerComponent, DxReportViewerModule } from 'devexpress-reporting-angular';

@Component({
  selector: 'report-viewer',
  encapsulation: ViewEncapsulation.None,
  imports: [DxReportViewerModule],
  templateUrl: './report-viewer.html',
  styleUrls: [ /* ... */ ]
})
export class ReportViewer {
  @ViewChild(DxReportViewerComponent, { static: false })
  protected viewer!: DxReportViewerComponent;

  reportUrl    = 'XtraReport1';
  invokeAction = '/DXXRDV';

  submitParameter(paramValue: string) {
    // Reload the report with parameter encoded in the URL
    this.viewer.bindingSender.OpenReport(this.reportUrl + '?strParam=' + paramValue);
  }

  constructor(@Inject('BASE_URL') protected readonly hostUrl: string) {}
}
```

```html
<!-- report-viewer.html -->
<input #paramValue type="text"/>
<button (click)="submitParameter(paramValue.value)">Submit</button>

<dx-report-viewer [reportUrl]="reportUrl" height="800px">
  <dxrv-request-options [invokeAction]="invokeAction" [host]="hostUrl"></dxrv-request-options>
</dx-report-viewer>
```

### Backend — IReportProvider parsing URL parameters

```csharp
using DevExpress.XtraReports.Services;
using DevExpress.XtraReports.UI;
using System;
using Microsoft.AspNetCore.WebUtilities;

public class CustomReportProvider : IReportProvider {
    public XtraReport GetReport(string id, ReportProviderContext context) {
        string[] parts = id.Split('?');
        string reportName = parts[0];
        string queryString = parts.Length > 1 ? "?" + parts[1] : string.Empty;

        XtraReport report = reportName switch {
            "XtraReport1" => new XtraReport1(),
            _ => throw new DevExpress.XtraReports.Web.ClientControls.FaultException(
                     $"Could not find report '{reportName}'.")
        };

        var parameters = QueryHelpers.ParseQuery(queryString);
        foreach (var kvp in parameters) {
            if (report.Parameters[kvp.Key] != null)
                report.Parameters[kvp.Key].Value = Convert.ChangeType(
                    kvp.Value.ToString(), report.Parameters[kvp.Key].Type);
        }

        // Hide the parameters panel (values already applied)
        foreach (var parameter in report.Parameters)
            parameter.Visible = false;

        return report;
    }
}
```

Register in `Program.cs`:
```csharp
builder.Services.AddScoped<IReportProvider, CustomReportProvider>();
```

## Handle ParametersInitialized Event

Use when you need to set initial parameter values or react to parameter changes programmatically:

```typescript
import { Component, Inject, ViewEncapsulation } from '@angular/core';
import { DxReportViewerModule } from 'devexpress-reporting-angular';

@Component({
  encapsulation: ViewEncapsulation.None,
  imports: [DxReportViewerModule],
  // ...
})
export class ReportViewer {
  reportUrl    = 'XtraReport1';
  invokeAction = '/DXXRDV';

  OnParametersInitialized(event: any) {
    const parametersModel = event.args.ParametersModel;

    // Helper to find a parameter by its path
    const findParam = (path: string) =>
      parametersModel.parameters.find((p: any) => p.path === path);

    const intParam     = findParam('intParam');
    const boolParam    = findParam('booleanParam');
    const strParam     = findParam('strParam');

    if (intParam && boolParam && strParam) {
      // Set an invisible parameter's initial value
      parametersModel.setParameterValue('intParam', 42);

      // Set a visible boolean parameter's initial value
      parametersModel.setParameterValue('booleanParam', true);

      // React to changes in one parameter and update another
      boolParam.events.on('propertyChanged', (args: any) => {
        if (args.propertyName === 'value') {
          parametersModel.setParameterValue('strParam', args.newValue?.toString());
        }
      });

      // Submit parameter values to load the report
      event.args.Submit();
    }
  }

  constructor(@Inject('BASE_URL') protected readonly hostUrl: string) {}
}
```

```html
<dx-report-viewer [reportUrl]="reportUrl" height="800px">
  <dxrv-request-options [invokeAction]="invokeAction" [host]="hostUrl"></dxrv-request-options>
  <dxrv-callbacks (ParametersInitialized)="OnParametersInitialized($event)"></dxrv-callbacks>
</dx-report-viewer>
```

## Key Parameter API

| Method / property | Description |
|-------------------|-------------|
| `event.args.ParametersModel` | The viewer's parameter model object |
| `parametersModel.parameters` | Array of parameter descriptors |
| `parametersModel.setParameterValue(path, value)` | Set a parameter value programmatically |
| `parameter.path` | The parameter's path identifier |
| `parameter.events.on('propertyChanged', fn)` | Subscribe to value changes |
| `event.args.Submit()` | Apply parameter values and reload the report |
| `viewer.bindingSender.OpenReport(url)` | Reload viewer with a new report URL |

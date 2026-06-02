# Report Designer — DevExpress Reporting for Angular

## When to Use This Reference

Use when integrating the end-user Web Report Designer (`dx-report-designer`) into an Angular app, configuring designer options, or customizing the designer's toolbar and data sources.

## npm Package

`devexpress-reporting-angular` (same package as the viewer).

Optional: `devexpress-richedit` — adds the Rich Text inline editor for `XRRichText` controls. Adds significant bundle size; omit if not needed.

## Minimal Designer Setup

```typescript
// report-designer.ts
import { Component, ViewEncapsulation } from '@angular/core';
import { DxReportDesignerModule } from 'devexpress-reporting-angular';

@Component({
  selector: 'report-designer',
  encapsulation: ViewEncapsulation.None,   // REQUIRED
  imports: [DxReportDesignerModule],
  templateUrl: './report-designer.html',
  styleUrls: [
    '../../../node_modules/devextreme/dist/css/dx.light.css',
    '../../../node_modules/@devexpress/analytics-core/dist/css/dx-analytics.common.css',
    '../../../node_modules/@devexpress/analytics-core/dist/css/dx-analytics.light.css',
    '../../../node_modules/@devexpress/analytics-core/dist/css/dx-querybuilder.css',
    '../../../node_modules/devexpress-reporting/dist/css/dx-webdocumentviewer.css',
    '../../../node_modules/devexpress-reporting/dist/css/dx-reportdesigner.css'
  ]
})
export class ReportDesignerComponent {
  reportUrl              = 'TestReport';
  hostUrl                = 'http://localhost:5000/';
  getDesignerModelAction = '/DXXRD/GetDesignerModel';
}
```

```html
<!-- report-designer.html -->
<dx-report-designer [reportUrl]="reportUrl" height="700px">
  <dxrd-request-options
    [getDesignerModelAction]="getDesignerModelAction"
    [host]="hostUrl">
  </dxrd-request-options>
</dx-report-designer>
```

## Backend — Custom Controller (when you need to provide data sources)

```csharp
// Controllers/ReportingControllers.cs
using DevExpress.AspNetCore.Reporting.ReportDesigner;
using DevExpress.AspNetCore.Reporting.ReportDesigner.Native.Services;
using DevExpress.XtraReports.Web.ReportDesigner;
using Microsoft.AspNetCore.Mvc;

public class CustomReportDesignerController : ReportDesignerController {
    public CustomReportDesignerController(IReportDesignerMvcControllerService controllerService)
        : base(controllerService) { }

    [HttpPost("[action]")]
    public IActionResult GetDesignerModel(
        [FromForm] string reportUrl,
        [FromServices] IReportDesignerModelBuilder designerModelBuilder,
        [FromForm] ReportDesignerSettingsBase designerModelSettings)
    {
        var designerModel = designerModelBuilder
            .Report(reportUrl)
            .BuildModel();
        designerModel.Assign(designerModelSettings);
        return DesignerModel(designerModel);
    }
}
```

## Designer Callbacks (dxrd-callbacks)

```html
<dx-report-designer [reportUrl]="reportUrl" height="700px">
  <dxrd-request-options [getDesignerModelAction]="getDesignerModelAction" [host]="hostUrl">
  </dxrd-request-options>
  <dxrd-callbacks (CustomizeMenuActions)="onCustomizeMenuActions($event)">
  </dxrd-callbacks>
</dx-report-designer>
```

```typescript
import { ActionId } from 'devexpress-reporting/dx-reportdesigner';

onCustomizeMenuActions(event: any) {
  // Hide New Report button
  var newReportAction = event.args.GetById(ActionId.NewReport);
  if (newReportAction) newReportAction.visible = false;
}
```

> **Note**: Import `ActionId` from `devexpress-reporting/dx-reportdesigner` for the designer (not `dx-webdocumentviewer`). For preview callbacks inside the designer, prefix the event name with `Preview`: e.g., `PreviewCustomizeExportOptions`.

## Designer Preview Callbacks

```html
<dxrd-callbacks (PreviewCustomizeExportOptions)="onCustomizeExportOptions($event)">
</dxrd-callbacks>
```

```typescript
import { ExportFormatID } from 'devexpress-reporting/dx-webdocumentviewer';

onCustomizeExportOptions(event: any) {
  event.args.HideFormat(ExportFormatID.XLS);
}
```

## Designer Model Settings

Control designer behavior via nested components:

```html
<dx-report-designer [reportUrl]="reportUrl" height="700px">
  <dxrd-request-options [getDesignerModelAction]="getDesignerModelAction" [host]="hostUrl">
  </dxrd-request-options>
  <dxrd-designer-model-settings [allowMDI]="true">
    <!-- Restrict data source management -->
    <dxrd-datasource-settings
      [allowAddDataSource]="false"
      [allowRemoveDataSource]="false">
    </dxrd-datasource-settings>
    <!-- Wizard configuration -->
    <dxrd-wizard-settings
      [useFullscreenWizard]="false"
      [enableSqlDataSource]="false">
    </dxrd-wizard-settings>
    <!-- Preview panel settings -->
    <dxrd-preview-settings>
      <dxrv-export-settings [useSameTab]="false"></dxrv-export-settings>
      <dxrv-search-settings [useAsyncSearch]="false"></dxrv-search-settings>
    </dxrd-preview-settings>
  </dxrd-designer-model-settings>
</dx-report-designer>
```

## Access the Designer API (bindingSender)

```typescript
import { ViewChild } from '@angular/core';
import { DxReportDesignerComponent } from 'devexpress-reporting-angular';

@ViewChild(DxReportDesignerComponent, { static: false })
designer: DxReportDesignerComponent;

openReport() {
  this.designer.bindingSender.OpenReport('AnotherReport');
}
```

## Designer CSS Files

The designer requires these CSS files (in addition to the shared ones):

```typescript
styleUrls: [
  // Shared with viewer:
  '...devextreme/dist/css/dx.light.css',
  '...@devexpress/analytics-core/dist/css/dx-analytics.common.css',
  '...@devexpress/analytics-core/dist/css/dx-analytics.light.css',
  // Designer-specific:
  '...@devexpress/analytics-core/dist/css/dx-querybuilder.css',
  '...devexpress-reporting/dist/css/dx-webdocumentviewer.css',
  '...devexpress-reporting/dist/css/dx-reportdesigner.css',
  // Optional (rich text):
  '...devexpress-richedit/dist/dx.richedit.css',
  '...ace-builds/css/ace.css',
  '...ace-builds/css/theme/dreamweaver.css',
  '...ace-builds/css/theme/ambiance.css',
]
```

## Backend Routes

| Route | Purpose |
|-------|---------|
| `/DXXRDV` | Document Viewer requests |
| `/DXXRD/GetDesignerModel` | Report Designer model initialization |
| `/DXXRQB` | Query Builder requests (auto-registered) |

All routes are registered automatically by `AddDevExpressControls` + `UseDevExpressControls`. Custom controllers inherit from the base controller classes.

# Viewer Customization — DevExpress Reporting for Angular

## When to Use This Reference

Use when the developer needs to customize the Document Viewer toolbar, filter export formats, handle viewer events, or access the viewer's client-side API programmatically.

## Callbacks — How They Work

Callbacks are registered via the `dxrv-callbacks` nested component. Each Angular event binding maps to a viewer client-side event.

```html
<dx-report-viewer [reportUrl]="reportUrl" height="800px">
  <dxrv-request-options [invokeAction]="invokeAction" [host]="hostUrl"></dxrv-request-options>
  <dxrv-callbacks
    (CustomizeMenuActions)="onCustomizeMenuActions($event)"
    (CustomizeExportOptions)="onCustomizeExportOptions($event)"
    (ParametersInitialized)="onParametersInitialized($event)">
  </dxrv-callbacks>
</dx-report-viewer>
```

The `$event` object always has `sender` (the viewer JS object) and `args` (event-specific arguments).

## Hide / Show Toolbar Buttons (CustomizeMenuActions)

Import `ActionId` from `devexpress-reporting/dx-webdocumentviewer`:

```typescript
import { ActionId } from 'devexpress-reporting/dx-webdocumentviewer';
import { Component, ViewEncapsulation } from '@angular/core';
import { DxReportViewerModule } from 'devexpress-reporting-angular';

@Component({
  encapsulation: ViewEncapsulation.None,
  imports: [DxReportViewerModule],
  // ...
})
export class ReportViewerComponent {
  onCustomizeMenuActions(event: any) {
    // Hide the Print button
    var printAction = event.args.GetById(ActionId.Print);
    if (printAction) printAction.visible = false;

    // Hide Print Page
    var printPageAction = event.args.GetById(ActionId.PrintPage);
    if (printPageAction) printPageAction.visible = false;

    // Hide search
    var searchAction = event.args.GetById(ActionId.Search);
    if (searchAction) searchAction.hotKey = null;  // also removes hotkey
  }
}
```

Common `ActionId` values: `Print`, `PrintPage`, `Search`, `PrevPage`, `NextPage`, `FirstPage`, `LastPage`, `Zoom`, `HighlightEditingFields`.

## Filter Export Formats (CustomizeExportOptions)

Import `ExportFormatID` from `devexpress-reporting/dx-webdocumentviewer`:

```typescript
import { ExportFormatID } from 'devexpress-reporting/dx-webdocumentviewer';

onCustomizeExportOptions(event: any) {
  // Hide XLS and XLSX — keep PDF only
  event.args.HideFormat(ExportFormatID.XLS);
  event.args.HideFormat(ExportFormatID.XLSX);
  event.args.HideFormat(ExportFormatID.CSV);
  event.args.HideFormat(ExportFormatID.RTF);
  event.args.HideFormat(ExportFormatID.MHT);
  event.args.HideFormat(ExportFormatID.HTML);
  event.args.HideFormat(ExportFormatID.TEXT);
  event.args.HideFormat(ExportFormatID.IMAGE);
  event.args.HideFormat(ExportFormatID.DOCX);
  // ExportFormatID.PDF is not hidden — remains visible
}
```

> **Important**: Use `CustomizeExportOptions` with `HideFormat` + `ExportFormatID` to filter formats. Do NOT use `CustomizeMenuActions` for this purpose — it controls toolbar button visibility, not the export format list.

## Remove the Entire Toolbar (CustomizeElements)

```typescript
import { PreviewElements } from 'devexpress-reporting/dx-webdocumentviewer';

onCustomizeElements(event: any) {
  var toolbarPart = event.args.GetById(PreviewElements.Toolbar);
  var index = event.args.Elements.indexOf(toolbarPart);
  event.args.Elements.splice(index, 1);
}
```

In the template:
```html
<dxrv-callbacks (CustomizeElements)="onCustomizeElements($event)"></dxrv-callbacks>
```

## Add a Custom Toolbar Button

```typescript
import { ActionId } from 'devexpress-reporting/dx-webdocumentviewer';

onCustomizeMenuActions(event: any) {
  event.args.Actions.push({
    text: 'My Action',
    visible: true,
    disabled: false,
    imageTemplateName: 'myIconTemplate',  // registered via TemplateEngine
    clickAction: () => {
      this.viewer.bindingSender.Print();
    }
  });
}
```

## Access the Viewer API (bindingSender)

Get a reference to the viewer and call its JS API:

```typescript
import { ViewChild } from '@angular/core';
import { DxReportViewerComponent } from 'devexpress-reporting-angular';

@ViewChild(DxReportViewerComponent, { static: false })
viewer: DxReportViewerComponent;

ngAfterViewInit() {
  // Safe to use bindingSender after view initializes
  this.viewer.bindingSender.OpenReport('AnotherReport');
}

print() {
  this.viewer.bindingSender.Print();
}
```

## Export Settings Options

Control how print/export behave:

```html
<dxrv-export-settings
  [useSameTab]="false"
  [useAsynchronousExport]="true"
  [showPrintNotificationDialog]="false">
</dxrv-export-settings>
```

## Mobile Mode

```html
<dx-report-viewer [reportUrl]="reportUrl" [isMobile]="true" height="100vh">
  <dxrv-request-options [invokeAction]="invokeAction" [host]="hostUrl"></dxrv-request-options>
  <dxrv-mobile-mode-settings [readerMode]="true" [animationEnabled]="true">
  </dxrv-mobile-mode-settings>
</dx-report-viewer>
```

## Full Example — Viewer with Customizations

```typescript
import { Component, Inject, ViewEncapsulation, ViewChild } from '@angular/core';
import { DxReportViewerComponent, DxReportViewerModule } from 'devexpress-reporting-angular';
import { ActionId, ExportFormatID } from 'devexpress-reporting/dx-webdocumentviewer';

@Component({
  selector: 'report-viewer',
  encapsulation: ViewEncapsulation.None,
  imports: [DxReportViewerModule],
  templateUrl: './report-viewer.html',
  styleUrls: [
    '../../../node_modules/devextreme/dist/css/dx.light.css',
    '../../../node_modules/@devexpress/analytics-core/dist/css/dx-analytics.common.css',
    '../../../node_modules/@devexpress/analytics-core/dist/css/dx-analytics.light.css',
    '../../../node_modules/devexpress-reporting/dist/css/dx-webdocumentviewer.css'
  ]
})
export class ReportViewerComponent {
  @ViewChild(DxReportViewerComponent, { static: false }) viewer: DxReportViewerComponent;
  reportUrl    = 'TestReport';
  invokeAction = '/DXXRDV';

  onCustomizeMenuActions(event: any) {
    var printAction = event.args.GetById(ActionId.Print);
    if (printAction) printAction.visible = false;
  }

  onCustomizeExportOptions(event: any) {
    event.args.HideFormat(ExportFormatID.XLS);
    event.args.HideFormat(ExportFormatID.XLSX);
  }

  print() {
    this.viewer.bindingSender.Print();
  }

  constructor(@Inject('BASE_URL') public hostUrl: string) {}
}
```

```html
<!-- report-viewer.html -->
<button (click)="print()">Print</button>
<dx-report-viewer [reportUrl]="reportUrl" height="800px">
  <dxrv-request-options [invokeAction]="invokeAction" [host]="hostUrl"></dxrv-request-options>
  <dxrv-export-settings [useSameTab]="false"></dxrv-export-settings>
  <dxrv-search-settings [useAsyncSearch]="false"></dxrv-search-settings>
  <dxrv-callbacks
    (CustomizeMenuActions)="onCustomizeMenuActions($event)"
    (CustomizeExportOptions)="onCustomizeExportOptions($event)">
  </dxrv-callbacks>
</dx-report-viewer>
```

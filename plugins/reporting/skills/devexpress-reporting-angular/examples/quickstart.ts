// DevExpress Reporting for Angular — Quickstart
// Demonstrates: Document Viewer + toolbar customization + export filter + parameters
// npm packages: devextreme, devextreme-angular, @devexpress/analytics-core, devexpress-reporting-angular

// =================== app.ts (Document Viewer) ===================
import { Component, Inject, ViewChild, ViewEncapsulation } from '@angular/core';
import { DxReportViewerComponent, DxReportViewerModule } from 'devexpress-reporting-angular';
import { ActionId, ExportFormatID } from 'devexpress-reporting/dx-webdocumentviewer';

@Component({
  selector: 'app-root',
  encapsulation: ViewEncapsulation.None,   // REQUIRED — prevents CSS scoping issues
  imports: [DxReportViewerModule],
  templateUrl: './app.html',
  styleUrls: [
    '../../node_modules/devextreme/dist/css/dx.light.css',
    '../../node_modules/@devexpress/analytics-core/dist/css/dx-analytics.common.css',
    '../../node_modules/@devexpress/analytics-core/dist/css/dx-analytics.light.css',
    '../../node_modules/devexpress-reporting/dist/css/dx-webdocumentviewer.css'
  ]
})
export class App {
  @ViewChild(DxReportViewerComponent, { static: false })
  viewer!: DxReportViewerComponent;

  reportUrl    = 'TestReport';
  hostUrl      = 'http://localhost:5000/';
  invokeAction = '/DXXRDV';    // built-in ASP.NET Core backend route

  // Toolbar customization — hide Print button
  onCustomizeMenuActions(event: any) {
    var printAction = event.args.GetById(ActionId.Print);
    if (printAction) printAction.visible = false;
  }

  // Export format filter — show PDF only
  onCustomizeExportOptions(event: any) {
    event.args.HideFormat(ExportFormatID.XLS);
    event.args.HideFormat(ExportFormatID.XLSX);
    event.args.HideFormat(ExportFormatID.CSV);
    event.args.HideFormat(ExportFormatID.RTF);
    event.args.HideFormat(ExportFormatID.MHT);
    event.args.HideFormat(ExportFormatID.HTML);
    event.args.HideFormat(ExportFormatID.TEXT);
    event.args.HideFormat(ExportFormatID.IMAGE);
    event.args.HideFormat(ExportFormatID.DOCX);
    // PDF remains visible — not hidden
  }

  // Programmatic print via bindingSender
  print() {
    this.viewer.bindingSender.Print();
  }
}

// =================== app.html (Document Viewer) ===================
/*
<button (click)="print()">Print</button>

<dx-report-viewer [reportUrl]="reportUrl" height="800px">
  <dxrv-request-options [invokeAction]="invokeAction" [host]="hostUrl"></dxrv-request-options>
  <dxrv-export-settings [useSameTab]="false" [useAsynchronousExport]="true"></dxrv-export-settings>
  <dxrv-search-settings [useAsyncSearch]="false"></dxrv-search-settings>
  <dxrv-callbacks
    (CustomizeMenuActions)="onCustomizeMenuActions($event)"
    (CustomizeExportOptions)="onCustomizeExportOptions($event)">
  </dxrv-callbacks>
</dx-report-viewer>
*/

// =================== report-designer.ts (Designer) ===================
import { DxReportDesignerComponent, DxReportDesignerModule } from 'devexpress-reporting-angular';
import { ActionId as DesignerActionId } from 'devexpress-reporting/dx-reportdesigner';

@Component({
  selector: 'report-designer',
  encapsulation: ViewEncapsulation.None,
  imports: [DxReportDesignerModule],
  templateUrl: './report-designer.html',
  styleUrls: [
    '../../node_modules/devextreme/dist/css/dx.light.css',
    '../../node_modules/@devexpress/analytics-core/dist/css/dx-analytics.common.css',
    '../../node_modules/@devexpress/analytics-core/dist/css/dx-analytics.light.css',
    '../../node_modules/@devexpress/analytics-core/dist/css/dx-querybuilder.css',
    '../../node_modules/devexpress-reporting/dist/css/dx-webdocumentviewer.css',
    '../../node_modules/devexpress-reporting/dist/css/dx-reportdesigner.css'
  ]
})
export class ReportDesignerApp {
  @ViewChild(DxReportDesignerComponent, { static: false })
  designer!: DxReportDesignerComponent;

  reportName             = 'TestReport';
  hostUrl                = 'http://localhost:5000/';
  getDesignerModelAction = '/DXXRD/GetDesignerModel';

  onCustomizeMenuActions(event: any) {
    // Hide the "New Report" button
    var newReportAction = event.args.GetById(DesignerActionId.NewReport);
    if (newReportAction) newReportAction.visible = false;
  }

  open() {
    this.designer.bindingSender.OpenReport('AnotherReport');
  }
}

// =================== report-designer.html (Designer) ===================
/*
<dx-report-designer [reportUrl]="reportName" height="700px">
  <dxrd-request-options
    [getDesignerModelAction]="getDesignerModelAction"
    [host]="hostUrl">
  </dxrd-request-options>
  <dxrd-callbacks (CustomizeMenuActions)="onCustomizeMenuActions($event)">
  </dxrd-callbacks>
  <dxrd-designer-model-settings [allowMDI]="true">
    <dxrd-wizard-settings [useFullscreenWizard]="false" [enableSqlDataSource]="false">
    </dxrd-wizard-settings>
  </dxrd-designer-model-settings>
</dx-report-designer>
*/

// =================== Program.cs (ASP.NET Core backend) ===================
/*
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options => {
    options.AddPolicy("AllowCorsPolicy", b => {
        b.SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost");
        b.AllowAnyHeader();
        b.AllowAnyMethod();
    });
});

builder.Services.AddDevExpressControls();
builder.Services.AddMvc();

var app = builder.Build();

app.UseRouting();
app.UseCors("AllowCorsPolicy");   // AFTER UseRouting, BEFORE endpoints
app.UseDevExpressControls();
app.UseEndpoints(endpoints => {
    endpoints.MapControllerRoute("default", "{controller=Home}/{action=Index}/{id?}");
});

app.Run();
*/

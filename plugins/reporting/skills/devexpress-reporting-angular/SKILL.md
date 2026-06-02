---
name: devexpress-reporting-angular
description: >
  Use when integrating DevExpress Reporting into an Angular application. Covers the
  dx-report-viewer and dx-report-designer Angular components, ASP.NET Core backend setup
  with CORS, npm package installation, component options, callbacks (CustomizeMenuActions,
  CustomizeExportOptions, ParametersInitialized), toolbar and export customization, parameter
  passing, mobile mode, and best practices. Trigger phrases: "add report viewer to Angular",
  "Angular document viewer DevExpress", "dx-report-viewer", "dx-report-designer Angular",
  "devexpress-reporting-angular npm", "Angular reporting app", "integrate DevExpress report
  in Angular", "DxReportViewerModule", "DxReportDesignerModule".
version: "26.1"
compatibility: >
  Requires Angular 19+ and Node.js 18.13+. Backend: ASP.NET Core (.NET 8+) with
  DevExpress.AspNetCore.Reporting NuGet package. Frontend npm packages: devextreme,
  devextreme-angular, @devexpress/analytics-core, devexpress-reporting-angular.
  Frontend and backend package versions must match exactly. DevExpress license required.
metadata:
  source-commit: 838d10f9d77835a140d1e15ab06a315039c0023e
  version: "26.1"
---

# DevExpress Reporting for Angular

Integrate the DevExpress Web Document Viewer and Web Report Designer into an Angular SPA backed by an ASP.NET Core API.

## When to Use

- Adding a **document viewer** (`dx-report-viewer`) to an Angular app
- Adding an **end-user report designer** (`dx-report-designer`) to an Angular app
- Customizing the viewer/designer toolbar, export options, or callbacks
- Passing parameter values to reports from the Angular UI
- Configuring CORS between an Angular SPA and ASP.NET Core backend

## Before You Start

Before generating code, ask the developer:

1. **Which component?** Document Viewer only, Report Designer only, or both?
2. **New or existing project?** If existing, is the Angular app already created and does it have the npm packages installed?
3. **Backend already set up?** Does an ASP.NET Core backend with `AddDevExpressControls` / `UseDevExpressControls` already exist, or does it need to be created?
4. **DevExpress version?** (e.g., 26.1). Frontend npm packages must exactly match the backend NuGet version.
5. **Angular version?** Angular 19+ with standalone components (`imports` array in `@Component`) is the current pattern.
6. **Do you need the Rich Text inline editor?** (requires optional `devexpress-richedit` package — adds significant bundle size)

## Prerequisites & Installation

### Backend (ASP.NET Core)

NuGet package: `DevExpress.AspNetCore.Reporting`

```csharp
// Program.cs
builder.Services.AddCors(options => {
    options.AddPolicy("AllowCorsPolicy", builder => {
        builder.SetIsOriginAllowed(origin => new Uri(origin).Host == "localhost");
        builder.AllowAnyHeader();
        builder.AllowAnyMethod();
    });
});
builder.Services.AddDevExpressControls();
// ...
app.UseRouting();
app.UseCors("AllowCorsPolicy");  // after UseRouting, before UseEndpoints
app.UseDevExpressControls();
```

Or scaffold a ready backend with the DevExpress CLI template:
```console
dotnet new install DevExpress.AspNetCore.ProjectTemplates
dotnet new dx.aspnetcore.reporting.backend -n ServerApp
```

### Frontend (Angular)

npm packages:
```console
npm install devextreme@26.1-stable devextreme-angular@26.1-stable \
  @devexpress/analytics-core@26.1-stable devexpress-reporting-angular@26.1-stable
```

Replace `26.1` with your actual DevExpress version. Versions must match the backend exactly.

## Component Overview

| Angular component | Angular module | Purpose |
|------------------|---------------|---------|
| `dx-report-viewer` | `DxReportViewerModule` | Display and print reports (read-only) |
| `dx-report-designer` | `DxReportDesignerModule` | Full end-user report designer |

## Quick Start — Document Viewer

```typescript
// app.ts
import { Component, ViewEncapsulation } from '@angular/core';
import { DxReportViewerModule } from 'devexpress-reporting-angular';

@Component({
  selector: 'app-root',
  encapsulation: ViewEncapsulation.None,   // REQUIRED — prevents Angular style scoping
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
  reportUrl = 'TestReport';
  hostUrl   = 'http://localhost:5000/';
  invokeAction = '/DXXRDV';              // built-in ASP.NET Core backend route
}
```

```html
<!-- app.html -->
<dx-report-viewer [reportUrl]="reportUrl" height="800px">
  <dxrv-request-options [invokeAction]="invokeAction" [host]="hostUrl">
  </dxrv-request-options>
</dx-report-viewer>
```

## Quick Start — Report Designer

```typescript
// app.ts
import { Component, ViewEncapsulation } from '@angular/core';
import { DxReportDesignerModule } from 'devexpress-reporting-angular';
import 'devexpress-reporting/dx-richedit';  // optional — rich text inline editor

@Component({
  selector: 'app-root',
  encapsulation: ViewEncapsulation.None,
  imports: [DxReportDesignerModule],
  templateUrl: './app.html',
  styleUrls: [
    '../../node_modules/devextreme/dist/css/dx.light.css',
    '../../node_modules/@devexpress/analytics-core/dist/css/dx-analytics.common.css',
    '../../node_modules/@devexpress/analytics-core/dist/css/dx-analytics.light.css',
    '../../node_modules/devexpress-reporting/dist/css/dx-webdocumentviewer.css',
    '../../node_modules/devexpress-reporting/dist/css/dx-reportdesigner.css'
  ]
})
export class App {
  reportName = 'TestReport';
  hostUrl    = 'http://localhost:5000/';
  getDesignerModelAction = '/DXXRD/GetDesignerModel';
}
```

```html
<!-- app.html -->
<dx-report-designer [reportUrl]="reportName" height="700px">
  <dxrd-request-options [getDesignerModelAction]="getDesignerModelAction" [host]="hostUrl">
  </dxrd-request-options>
</dx-report-designer>
```

## Documentation & Navigation Guide

| Reference file | Load when the developer asks about... |
|---------------|---------------------------------------|
| [`references/getting-started.md`](references/getting-started.md) | Step-by-step setup, CORS, backend CLI template, CSS imports, running both apps |
| [`references/viewer-customization.md`](references/viewer-customization.md) | Hiding toolbar buttons, filtering export formats, CustomizeMenuActions, CustomizeExportOptions, CustomizeElements, bindingSender |
| [`references/report-designer.md`](references/report-designer.md) | Designer component options, dxrd-callbacks, dxrd-designer-model-settings, wizard settings, backend controller |
| [`references/parameters.md`](references/parameters.md) | ParametersInitialized event, setParameterValue, custom UI parameter submission, IReportProvider |

## Key Angular Component Options

### dx-report-viewer root options

| Option | Type | Description |
|--------|------|-------------|
| `reportUrl` | string (required) | Report identifier resolved by the backend |
| `height` | string | Control height (default `'700px'`) |
| `width` | string | Control width (default `'100%'`) |
| `isMobile` | boolean | Enable mobile/reader mode |
| `developmentMode` | boolean | Extended diagnostics output |

### dxrv-* nested components

| Tag | Key options | Purpose |
|-----|------------|---------|
| `dxrv-request-options` | `host`, `invokeAction` (both required) | Backend URL routing |
| `dxrv-callbacks` | event bindings | Client-side event handlers |
| `dxrv-export-settings` | `useSameTab`, `useAsynchronousExport` | Export behavior |
| `dxrv-search-settings` | `useAsyncSearch` | Search behavior |
| `dxrv-tabpanel-settings` | `position`, `width` | Side panel layout |

### dx-report-designer root options

| Option | Type | Description |
|--------|------|-------------|
| `reportUrl` | string (required) | Initial report to open |
| `height` | string | Control height (default `'700px'`) |

### dxrd-* nested components

| Tag | Key options | Purpose |
|-----|------------|---------|
| `dxrd-request-options` | `host`, `getDesignerModelAction` (both required) | Backend URL routing |
| `dxrd-callbacks` | event bindings | Designer-level event handlers |
| `dxrd-designer-model-settings` | `allowMDI`, `rightToLeft` | Designer behavior |
| `dxrd-wizard-settings` | `useFullscreenWizard`, `enableSqlDataSource` | Report wizard settings |
| `dxrd-datasource-settings` | `allowAddDataSource`, `allowRemoveDataSource` | Field List restrictions |

> **Tip**: Use `dxr**v**-callbacks` for the Viewer and `dxr**d**-callbacks` for the Designer. They are different tags. Designer preview callbacks use the `Preview` prefix (e.g., `PreviewCustomizeExportOptions`).

## Common Patterns

### Access the viewer/designer from TypeScript

```typescript
import { ViewChild } from '@angular/core';
import { DxReportViewerComponent } from 'devexpress-reporting-angular';

@ViewChild(DxReportViewerComponent, { static: false }) viewer: DxReportViewerComponent;

// After view init — call client-side API via bindingSender:
this.viewer.bindingSender.OpenReport('AnotherReport');
this.viewer.bindingSender.Print();
```

### Hide a toolbar button (CustomizeMenuActions)

```typescript
import { ActionId } from 'devexpress-reporting/dx-webdocumentviewer';

CustomizeMenuActions(event: any) {
  var printAction = event.args.GetById(ActionId.Print);
  if (printAction) printAction.visible = false;
}
```

### Filter export formats (CustomizeExportOptions)

```typescript
import { ExportFormatID } from 'devexpress-reporting/dx-webdocumentviewer';

OnCustomizeExportOptions(event: any) {
  event.args.HideFormat(ExportFormatID.XLS);
  event.args.HideFormat(ExportFormatID.XLSX);
}
```

## Troubleshooting

| Symptom | Likely cause | Fix |
|---------|-------------|-----|
| Blank page / no viewer | Missing CSS imports or wrong `ViewEncapsulation` | Add all 4 CSS files; ensure `ViewEncapsulation.None` |
| `ko is not defined` | Knockout loaded after DevExtreme | Not applicable in Angular — check npm package versions match |
| CORS errors in browser | Backend missing `UseCors` | Add CORS policy; call `UseCors` after `UseRouting` |
| Version mismatch error | npm ≠ NuGet version | Ensure all DevExpress packages share the same `xx.x` version |
| `bindingSender` is null | Accessing before `ngAfterViewInit` | Move to `ngAfterViewInit` lifecycle hook |

## Constraints & Rules

1. **Version match**: Frontend npm package versions must exactly match backend NuGet package versions. Mixing versions causes runtime failures.
2. **ViewEncapsulation.None**: Always set on components that host `dx-report-viewer` or `dx-report-designer`. Without it, DevExpress CSS scoping breaks.
3. **CORS required**: The Angular SPA runs on a different port than the ASP.NET Core backend. CORS must be configured on the backend.
4. **Callback tags**: `dxrv-callbacks` is for the Viewer; `dxrd-callbacks` is for the Designer. Never swap them.
5. **CSS load order**: All four CSS files must be listed in `styleUrls`. Missing any one causes display issues.
6. **NuGet package**: Backend requires `DevExpress.AspNetCore.Reporting`. Never guess alternative package names.
7. **License**: DevExpress requires a valid license. Refer the user to their DevExpress account if license errors appear.
8. **Build verification**: After setup, run `dotnet run` (backend) and `npm start` (frontend) to verify end-to-end before reporting success.

## Using DevExpress Documentation MCP

If the DevExpress Docs MCP server is available, use it to supplement this skill:

1. **Search**: `devexpress_docs_search(technology="Reporting for Angular", query="your question")`
2. **Fetch**: `devexpress_docs_get_content(url="<article URL>")`

Use MCP for: advanced customization scenarios (CSP configuration, localization, custom parameter editors, standalone parameters panel), version-specific API changes, and anything not covered in the references above.

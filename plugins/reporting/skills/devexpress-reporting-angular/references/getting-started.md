# Getting Started — DevExpress Reporting for Angular

## When to Use This Reference

Use when setting up a new Angular + ASP.NET Core reporting application from scratch, or diagnosing a blank page / version mismatch issue.

## Architecture

Angular reporting has two separate applications:

- **Backend (ASP.NET Core)**: handles data sources, report storage, PDF generation, and report export. Runs on port 5000/5001.
- **Frontend (Angular SPA)**: hosts `dx-report-viewer` or `dx-report-designer` components. Runs on port 4200.

The two communicate over HTTP. CORS must be enabled on the backend.

## Backend Setup

### Option A — DevExpress CLI Template (fastest)

```console
dotnet new install DevExpress.AspNetCore.ProjectTemplates

# For viewer only:
dotnet new dx.aspnetcore.reporting.backend -n ServerApp --add-designer false

# For viewer + designer:
dotnet new dx.aspnetcore.reporting.backend -n ServerApp

cd ServerApp
dotnet run
```

### Option B — Manual Setup

1. Add NuGet package: `DevExpress.AspNetCore.Reporting`

2. Configure `Program.cs`:

```csharp
var builder = WebApplication.CreateBuilder(args);

// CORS — allow Angular SPA on localhost
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
    endpoints.MapControllerRoute(
        name: "default",
        pattern: "{controller=Home}/{action=Index}/{id?}");
});

app.Run();
```

3. The built-in `/DXXRDV` (viewer) and `/DXXRD/GetDesignerModel` (designer) routes are registered automatically by `AddDevExpressControls` + `UseDevExpressControls`. No custom controller is needed for basic scenarios.

## Frontend Setup

### Prerequisites

- Node.js 18.13 or later
- Angular CLI matching your Angular version: use Angular/CLI 19+ for DevExpress Reporting v25.2, or Angular/CLI 20+ for v26.1

### Create Angular Project

```console
ng new my-reporting-app
cd my-reporting-app
```

### Install npm Packages

Replace `26.1` with your DevExpress version (must match backend NuGet version exactly):

```console
npm install devextreme@26.1-stable \
            devextreme-angular@26.1-stable \
            @devexpress/analytics-core@26.1-stable \
            devexpress-reporting-angular@26.1-stable
```

Optional (for Rich Text inline editor in designer):
```console
npm install devexpress-richedit@26.1-stable
```

### Document Viewer — Full Component

```typescript
// src/app/app.ts
import { Component, ViewEncapsulation } from '@angular/core';
import { DxReportViewerModule } from 'devexpress-reporting-angular';

@Component({
  selector: 'app-root',
  encapsulation: ViewEncapsulation.None,   // REQUIRED
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
  reportUrl    = 'TestReport';
  hostUrl      = 'http://localhost:5000/';
  invokeAction = '/DXXRDV';
}
```

```html
<!-- src/app/app.html -->
<dx-report-viewer [reportUrl]="reportUrl" height="800px">
  <dxrv-request-options [invokeAction]="invokeAction" [host]="hostUrl">
  </dxrv-request-options>
</dx-report-viewer>
```

### Report Designer — Full Component

```typescript
// src/app/app.ts
import { Component, ViewEncapsulation } from '@angular/core';
import { DxReportDesignerModule } from 'devexpress-reporting-angular';
import 'devexpress-reporting/dx-richedit';  // omit if not using rich text

@Component({
  selector: 'app-root',
  encapsulation: ViewEncapsulation.None,
  imports: [DxReportDesignerModule],
  templateUrl: './app.html',
  styleUrls: [
    '../../node_modules/ace-builds/css/ace.css',
    '../../node_modules/ace-builds/css/theme/dreamweaver.css',
    '../../node_modules/ace-builds/css/theme/ambiance.css',
    '../../node_modules/devextreme/dist/css/dx.light.css',
    '../../node_modules/devexpress-richedit/dist/dx.richedit.css',
    '../../node_modules/@devexpress/analytics-core/dist/css/dx-analytics.common.css',
    '../../node_modules/@devexpress/analytics-core/dist/css/dx-analytics.light.css',
    '../../node_modules/@devexpress/analytics-core/dist/css/dx-querybuilder.css',
    '../../node_modules/devexpress-reporting/dist/css/dx-webdocumentviewer.css',
    '../../node_modules/devexpress-reporting/dist/css/dx-reportdesigner.css'
  ]
})
export class App {
  reportName             = 'TestReport';
  hostUrl                = 'http://localhost:5000/';
  getDesignerModelAction = '/DXXRD/GetDesignerModel';
}
```

```html
<!-- src/app/app.html -->
<dx-report-designer [reportUrl]="reportName" height="700px">
  <dxrd-request-options [getDesignerModelAction]="getDesignerModelAction" [host]="hostUrl">
  </dxrd-request-options>
</dx-report-designer>
```

## Run the Applications

1. Start the backend: `cd ServerApp && dotnet run`
2. Start the Angular app: `cd my-reporting-app && npm start`
3. Open `http://localhost:4200` in a browser

Make sure `hostUrl` in the Angular component points to the actual backend port.

## Common Setup Errors

| Error | Cause | Fix |
|-------|-------|-----|
| Blank page | Missing CSS or wrong `ViewEncapsulation` | Add all CSS files; set `ViewEncapsulation.None` |
| CORS error | Backend missing CORS config | Add `AddCors` + `UseCors` to `Program.cs` |
| Version mismatch | npm ≠ NuGet versions | All DevExpress packages must use the same version |
| `skipLibCheck` warning | TypeScript strict checks | Add `"skipLibCheck": true` to `tsconfig.json` |

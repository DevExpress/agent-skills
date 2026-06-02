---
name: devexpress-reporting-winforms
description: >
  AI skill for DevExpress WinForms Reporting (.NET and .NET Framework). Use when embedding a report
  print preview in a WinForms app, showing the End-User Report Designer, integrating reporting into
  a WinForms project, customizing the print preview toolbar, customizing the designer toolbar or
  toolbox, using ReportDesignTool, using XRDesignMethods, using ReportPrintTool, using
  DocumentViewer, using PrintPreviewFormEx, or deploying a WinForms reporting application.
  Trigger phrases: "WinForms report viewer", "print preview WinForms", "end-user designer WinForms",
  "ReportDesignTool", "ReportPrintTool", "XRDesignMethods", "embed report designer",
  "DocumentViewer WinForms", "WinForms reporting license", "DevExpress.Win.Reporting".
version: "26.1"
compatibility: >
  Requires DevExpress.Win.Reporting NuGet from nuget.devexpress.com (v26.1+). End-User Designer
  embedding requires DevExpress.Win.Design (WinForms subscription). Target: .NET 6+ or .NET
  Framework 4.6.2+. License: Reporting subscription for print preview and ReportDesignTool;
  WinForms subscription for embedded designer customization (XRDesignForm, XRDesignRibbonForm).
metadata:
  source-commit: 17f29ded6678b36f6708c900148e59989bd1798b
  version: "26.1"
  category: reporting
---

# DevExpress WinForms Reporting

## When to Use This Skill

Use for WinForms applications that need to:
- Show a Print Preview window for a report
- Embed a `DocumentViewer` control in a form
- Launch the End-User Report Designer (both `ReportDesignTool` approach and embedded approach)
- Customize the print preview toolbar or ribbon
- Customize the report designer toolbox or toolbar
- Deploy a WinForms reporting application

> **Runtime API** (creating reports in code, data binding, export): see `devexpress-reporting-core`.

## Before You Start

Ask the developer:

1. **Target framework**: .NET 6/7/8+ or .NET Framework 4.x?
2. **Feature needed**: Print preview only? End-User Designer? Both?
3. **Designer approach**: Default designer form (simpler, `ReportDesignTool`) or embedded designer component in your own form (`XRDesignRibbonForm` / `RibbonReportDesigner`)?
4. **Toolbar style**: Standard toolbar or Ribbon UI?
5. **License**: Do you have a Reporting subscription or a WinForms subscription? (This determines which designer embedding options are available — see Licensing section.)
6. **New project or existing**: Is DevExpress already installed?

## Licensing — Important

| Subscription | What You Can Do |
|--------------|-----------------|
| **Reporting** | Use `ReportDesignTool` to launch the default designer form. Full print preview. Export. |
| **WinForms** (includes Reporting) | Everything in Reporting, PLUS: embed designer in custom forms (`XRDesignForm`, `XRDesignRibbonForm`, `RibbonReportDesigner`, `StandardReportDesigner`), customize designer components. |

> If you only have a **Reporting subscription**, use `ReportDesignTool` — it does not require the WinForms package. The embedded designer API (`XRDesignForm`, `XRDesignMdiController`) requires the **WinForms subscription**.

## NuGet Setup

```bash
dotnet nuget add source https://nuget.devexpress.com/api --name DevExpressNuGet

# For print preview and ReportDesignTool (Reporting license)
dotnet add package DevExpress.Win.Reporting

# For embedded designer components (WinForms license required)
dotnet add package DevExpress.Win.Design
```

## Print Preview — Quick Start

### Using ReportPrintTool (recommended)

```csharp
using DevExpress.XtraReports.UI;
using DevExpress.LookAndFeel;

// Ribbon-based preview (modern UI)
var tool = new ReportPrintTool(new SalesReport());
tool.ShowRibbonPreview();                              // non-modal
tool.ShowRibbonPreviewDialog(UserLookAndFeel.Default); // modal

// Standard toolbar preview
tool.ShowPreview();
tool.ShowPreviewDialog(UserLookAndFeel.Default);
```

### VB.NET

```vb
Imports DevExpress.XtraReports.UI
Imports DevExpress.LookAndFeel

Dim tool As New ReportPrintTool(New SalesReport())
tool.ShowRibbonPreview()
' or modal:
tool.ShowRibbonPreviewDialog(UserLookAndFeel.Default)
```

## End-User Report Designer — ReportDesignTool Approach

`ReportDesignTool` works with a **Reporting subscription** (does not require WinForms bundle).

```csharp
using DevExpress.XtraReports.UI;
using DevExpress.LookAndFeel;

var tool = new ReportDesignTool(new SalesReport());

// Non-modal ribbon designer
tool.ShowRibbonDesigner();

// Modal ribbon designer
tool.ShowRibbonDesignerDialog(UserLookAndFeel.Default);

// Standard toolbar (non-ribbon)
tool.ShowDesigner();
tool.ShowDesignerDialog(UserLookAndFeel.Default);
```

### VB.NET

```vb
Imports DevExpress.XtraReports.UI
Imports DevExpress.LookAndFeel

Dim tool As New ReportDesignTool(New SalesReport())
tool.ShowRibbonDesignerDialog(UserLookAndFeel.Default)
```

## End-User Report Designer — Embedded Approach

**Requires WinForms subscription** (`DevExpress.Win.Design` package).

### Option A: Drop component onto your form (Visual Designer)

1. Add `RibbonReportDesigner` or `StandardReportDesigner` from the Toolbox to your form
2. Open a report on it in the `Load` event:

```csharp
using DevExpress.XtraReports.UserDesigner;

private void Form_Load(object sender, EventArgs e) {
    ribbonReportDesigner1.OpenReport(new SalesReport());
}
```

### Option B: Create a custom designer form in code

```csharp
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.UserDesigner;

// Access the ReportDesignTool's pre-built designer form
var tool = new ReportDesignTool(new SalesReport());
XRDesignRibbonForm designForm = tool.DesignRibbonForm;

// Customize via MDI controller before showing
designForm.DesignMdiController.DefaultReportSettings.ShowGrid = false;
designForm.Show();
```

### Option C: Subclass XRDesignForm for full control

```csharp
using DevExpress.XtraReports.UserDesigner;

public class MyDesignerForm : XRDesignRibbonForm {
    public MyDesignerForm(XtraReport report) {
        OpenReport(report);
        // Customize ribbon, panels, etc. here
    }
}
```

## Embedded DocumentViewer Control

Place `DocumentViewer` directly inside a WinForms form:

```csharp
using DevExpress.XtraReports.UI;
using DevExpress.XtraPrinting.Caching;

public class PreviewForm : Form {
    DocumentViewer documentViewer1;

    public PreviewForm() {
        documentViewer1 = new DocumentViewer { Dock = DockStyle.Fill };
        Controls.Add(documentViewer1);
    }

    public void LoadReport(XtraReport report) {
        documentViewer1.DocumentSource = report;
        report.CreateDocument();
    }

    // For large reports — use CachedReportSource
    public void LoadLargeReport(XtraReport report) {
        var storage = new MemoryDocumentStorage();
        var cached = new CachedReportSource(report, storage);
        documentViewer1.DocumentSource = cached;
        cached.CreateDocumentAsync();
    }
}
```

## Common Patterns

**Pattern 1 — Show preview and handle close:**
```csharp
var tool = new ReportPrintTool(report);
var previewForm = tool.PreviewRibbonForm;
previewForm.FormClosed += (s, e) => report.Dispose();
tool.ShowRibbonPreview();
```

**Pattern 2 — Export directly (no preview):**
```csharp
var report = new SalesReport { DataSource = data };
report.ExportToPdf("report.pdf");
// See devexpress-reporting-core for export options
```

**Pattern 3 — Designer saves to storage:**
```csharp
var tool = new ReportDesignTool(new XtraReport());
var form = tool.DesignRibbonForm;
form.DesignMdiController.ReportStorageGetData += (s, e) => {
    e.Stream = File.OpenRead(e.Url);
};
form.DesignMdiController.ReportStorageSetData += (s, e) => {
    using var fs = File.Create(e.Url);
    e.Stream.CopyTo(fs);
};
tool.ShowRibbonDesigner();
```

## Key API Reference

| Class | Namespace | Purpose |
|-------|-----------|---------|
| `ReportPrintTool` | `DevExpress.XtraReports.UI` | Show print preview forms |
| `ReportDesignTool` | `DevExpress.XtraReports.UI` | Launch designer forms |
| `DocumentViewer` | `DevExpress.XtraReports.UI` (WinForms) | Embeddable viewer control |
| `XRDesignForm` | `DevExpress.XtraReports.UserDesigner` | Standard designer form (WinForms license) |
| `XRDesignRibbonForm` | `DevExpress.XtraReports.UserDesigner` | Ribbon designer form (WinForms license) |
| `XRDesignMdiController` | `DevExpress.XtraReports.UserDesigner` | MDI controller for designer |
| `RibbonReportDesigner` | `DevExpress.XtraReports.UserDesigner` | Drop-in ribbon designer component |
| `StandardReportDesigner` | `DevExpress.XtraReports.UserDesigner` | Drop-in standard designer component |
| `PrintPreviewFormEx` | `DevExpress.XtraPrinting` | Standard preview form (low-level) |
| `PrintPreviewRibbonFormEx` | `DevExpress.XtraPrinting` | Ribbon preview form (low-level) |
| `CachedReportSource` | `DevExpress.XtraPrinting.Caching` | Large report memory management |

## Troubleshooting

| Symptom | Cause | Fix |
|---------|-------|-----|
| `ReportDesignTool` compiles but designer doesn't show customizations | Missing `DevExpress.Win.Design` package | Install `DevExpress.Win.Design` (WinForms license required) |
| `XRDesignForm` type not found | Wrong package — needs WinForms license | Use `ReportDesignTool` (Reporting license) or install `DevExpress.Win.Design` |
| Preview opens blank, no error | Report has no data or no `DetailBand` | Check `DataSource` is set and `DetailBand` exists |
| `DocumentViewer` shows spinner then goes blank | `CreateDocument()` not called | Call `report.CreateDocument()` after assigning `DocumentSource` |
| Designer does not save changes | No storage handlers wired | Wire `ReportStorageGetData` / `ReportStorageSetData` on `DesignMdiController` |
| Preview form appears then immediately closes | Modal call from wrong thread | Ensure `ShowRibbonPreviewDialog` is called on the UI thread |
| Fonts/icons appear blank in deployed app | Missing icon font files | Include `wwwroot/css/icons/` font assets or DevExpress skin files in deployment |

## Constraints & Rules

1. **Licensing check first**: Before writing embedded designer code, confirm the user has a WinForms subscription. `ReportDesignTool` works with Reporting subscription only.
2. **Never mix package versions**: All DevExpress NuGet packages must be the same version.
3. **`CreateDocument()` must be called**: When using `DocumentViewer.DocumentSource`, call `report.CreateDocument()` (or `CreateDocumentAsync()`) to populate the viewer.
4. **UI thread**: All preview/designer `Show` calls must happen on the WinForms UI thread.
5. **Verify build**: Run `dotnet build` and check for 0 errors before reporting success.
6. **No destructive changes**: Preserve existing form layout and controls; only add what is required.

## Navigation Guide

| Need | Reference File |
|------|---------------|
| NuGet setup, first preview | `references/getting-started.md` |
| .NET Framework setup | `references/getting-started-dotnet-fw.md` |
| Print preview — embedding, customization | `references/print-preview.md` |
| Designer — ReportDesignTool + embedded | `references/designer-integration.md` |
| Print API (direct print without preview) | `references/print-api.md` |

## Using DevExpress Documentation MCP

```
devexpress_docs_search(technology="WinForms Reporting", query="customize print preview toolbar")
devexpress_docs_get_content(url="<article URL>")
```

Use built-in references for: NuGet setup, preview, designer integration.
Use MCP for: advanced designer customization (toolbar items, toolbox categories, custom controls), theming, localization.

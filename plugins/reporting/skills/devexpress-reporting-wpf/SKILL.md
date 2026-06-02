---
name: devexpress-reporting-wpf
description: >
  AI skill for DevExpress WPF Reporting (.NET and .NET Framework). Use when embedding a report
  document preview in a WPF application, using DocumentPreviewControl, using PrintHelper, using
  the WPF ReportDesigner control, binding reports in MVVM, integrating DevExpress WPF reporting,
  customizing the WPF document preview toolbar, or deploying a WPF reporting application.
  Trigger phrases: "WPF report viewer", "WPF document preview", "DocumentPreviewControl",
  "PrintHelper WPF", "WPF ReportDesigner", "MVVM report WPF", "DevExpress.Wpf.Reporting",
  "embed report WPF", "WPF reporting license".
version: "26.1"
compatibility: >
  Requires DevExpress.Wpf.Reporting NuGet from nuget.devexpress.com (v26.1+). Target: .NET 6+ or
  .NET Framework 4.6.2+. Namespaces: DevExpress.Xpf.Printing (DocumentPreviewControl, PrintHelper),
  DevExpress.Xpf.Reports.UserDesigner (ReportDesigner). MVVM: DevExpress.Mvvm (optional, for
  POCO ViewModels and services). License: Reporting subscription for DocumentPreviewControl and
  PrintHelper; WPF subscription for ReportDesigner embedding and full WPF control suite.
metadata:
  source-commit: 17f29ded6678b36f6708c900148e59989bd1798b
  version: "26.1"
  category: reporting
---

# DevExpress WPF Reporting

## When to Use This Skill

Use for WPF applications that need to:
- Show a print preview window for a report (`PrintHelper`)
- Embed a `DocumentPreviewControl` in a WPF Window or UserControl
- Embed the `ReportDesigner` WPF control for end-user report editing
- Bind report display to a ViewModel (MVVM pattern)
- Pass parameters from a ViewModel to a report

> **Runtime API** (creating reports in code, data binding, export): see `devexpress-reporting-core`.

## Before You Start

Ask the developer:

1. **Target framework**: .NET 6/7/8+ or .NET Framework 4.x?
2. **Feature needed**: Preview only? End-User Designer? Both?
3. **MVVM**: Is the project using MVVM? Which framework (DevExpress MVVM, CommunityToolkit.Mvvm, plain INotifyPropertyChanged)?
4. **Toolbar style**: Ribbon (modern) or Bars (classic)?
5. **License**: Reporting subscription or WPF subscription? (WPF subscription required for `ReportDesigner` embedding and advanced customization — see Licensing.)
6. **New project or existing**: Are DevExpress packages already installed?

## Licensing — Important

| Subscription | What You Can Do |
|--------------|-----------------|
| **Reporting** | `DocumentPreviewControl`, `PrintHelper`. Full preview and export. |
| **WPF** (includes Reporting) | Everything in Reporting, PLUS: embed `ReportDesigner` control, use all 130+ DevExpress WPF controls, customize designer UI. |

> `ReportDesigner` WPF control requires the **WPF subscription**.

## NuGet Setup

```bash
dotnet nuget add source https://nuget.devexpress.com/api --name DevExpressNuGet

dotnet add package DevExpress.Wpf.Reporting
# For MVVM helpers (optional)
dotnet add package DevExpress.Mvvm
```

## XAML Namespace Declarations

```xaml
<!-- Document preview / PrintHelper -->
xmlns:dxp="http://schemas.devexpress.com/winfx/2008/xaml/printing"

<!-- Report designer -->
xmlns:dxrud="http://schemas.devexpress.com/winfx/2008/xaml/reports/userdesigner"

<!-- DevExpress core (ThemedWindow, etc.) -->
xmlns:dx="http://schemas.devexpress.com/winfx/2008/xaml/core"

<!-- DevExpress MVVM behaviors -->
xmlns:dxmvvm="http://schemas.devexpress.com/winfx/2008/xaml/mvvm"
```

## Print Preview — Quick Start

### Option A: PrintHelper (simplest — no XAML)

```csharp
using DevExpress.Xpf.Printing;
using DevExpress.XtraReports.UI;

private void Window_Loaded(object sender, RoutedEventArgs e) {
    var report = new SalesReport { DataSource = GetData() };

    // Ribbon preview (non-modal)
    PrintHelper.ShowRibbonPrintPreview(this, report);

    // Ribbon preview (modal)
    PrintHelper.ShowRibbonPrintPreviewDialog(this, report);

    // Standard toolbar preview
    PrintHelper.ShowPrintPreview(this, report);
}
```

### Option B: DocumentPreviewControl in XAML

```xaml
<dxp:DocumentPreviewControl x:Name="documentPreview"
                            RequestDocumentCreation="True"
                            DocumentSource="{Binding Report}" />
```

```csharp
// Code-behind: assign report
documentPreview.DocumentSource = new SalesReport { DataSource = GetData() };
// RequestDocumentCreation="True" means the control builds the document automatically
```

### VB.NET

```vb
Imports DevExpress.Xpf.Printing
Imports DevExpress.XtraReports.UI

PrintHelper.ShowRibbonPrintPreviewDialog(Me, New SalesReport())
```

## End-User Report Designer — Quick Start

Requires **WPF subscription** (`DevExpress.Wpf.Reporting` includes designer assemblies).

```xaml
<Window xmlns:dxrud="http://schemas.devexpress.com/winfx/2008/xaml/reports/userdesigner"
        x:Class="MyApp.DesignerWindow">
    <dxrud:ReportDesigner x:Name="reportDesigner" />
</Window>
```

```csharp
private void Window_Loaded(object sender, RoutedEventArgs e) {
    reportDesigner.OpenDocument(new SalesReport());
}
```

### VB.NET

```vb
Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
    reportDesigner.OpenDocument(New SalesReport())
End Sub
```

## MVVM — Bind Report to ViewModel

### With DevExpress MVVM (POCO ViewModel)

```csharp
using DevExpress.Mvvm;
using DevExpress.Mvvm.DataAnnotations;
using DevExpress.XtraReports.UI;

[POCOViewModel]
public class ReportViewModel : ViewModelBase {
    public virtual XtraReport Report { get; protected set; }

    public void LoadReport() {
        var r = new SalesReport { DataSource = GetData() };
        Report = r;
    }
}
```

```xaml
<Window DataContext="{dxmvvm:ViewModelSource Type=local:ReportViewModel}">
    <dxp:DocumentPreviewControl RequestDocumentCreation="True"
                               DocumentSource="{Binding Report}" />
</Window>
```

### With Plain INotifyPropertyChanged

```csharp
public class ReportViewModel : INotifyPropertyChanged {
    private XtraReport _report;
    public XtraReport Report {
        get => _report;
        set { _report = value; OnPropertyChanged(); }
    }

    public void LoadReport(object data) {
        Report = new SalesReport { DataSource = data };
    }

    public event PropertyChangedEventHandler PropertyChanged;
    protected void OnPropertyChanged([System.Runtime.CompilerServices.CallerMemberName] string name = "") =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
```

## MVVM — Pass Parameters to Report

```csharp
[POCOViewModel]
public class OrderPreviewViewModel : ViewModelBase {
    public virtual XtraReport Report { get; protected set; }

    public void ShowOrderReport(int orderId) {
        var report = new OrderReport();
        report.Parameters["OrderId"].Value = orderId;
        report.Parameters["OrderId"].Visible = false;  // hide from parameter panel
        Report = report;
    }
}
```

## Large Reports — CachedReportSource

```csharp
using DevExpress.Xpf.Printing;
using DevExpress.XtraPrinting.Caching;

private void Window_Loaded(object sender, RoutedEventArgs e) {
    var storage = new MemoryDocumentStorage();
    var report = new LargeReport { DataSource = GetData() };
    var cached = new CachedReportSource(report, storage);

    documentPreview.DocumentSource = cached;
    cached.CreateDocumentAsync();   // builds asynchronously; preview shows progress
}
```

## Common Patterns

**Pattern 1 — CommandBarStyle (toolbar type):**
```xaml
<dxp:DocumentPreviewControl CommandBarStyle="Ribbon"  <!-- or "Bars" or "None" -->
                            DocumentSource="{Binding Report}"
                            RequestDocumentCreation="True" />
```

**Pattern 2 — ReportDesigner with multiple documents:**
```csharp
// Open a new blank document
reportDesigner.NewDocument();

// Open from stream
reportDesigner.OpenDocument(stream);

// Access currently open documents
foreach (var doc in reportDesigner.Documents) {
    Console.WriteLine(doc.Caption);
}
```

**Pattern 3 — ReportDesigner MVVM service** (for controlling designer from ViewModel):
For this pattern, see `references/designer-integration.md`.

## Key API Reference

| Class | Namespace | Purpose |
|-------|-----------|---------|
| `DocumentPreviewControl` | `DevExpress.Xpf.Printing` | XAML preview control |
| `PrintHelper` | `DevExpress.Xpf.Printing` | Shortcut preview/print methods |
| `ReportDesigner` | `DevExpress.Xpf.Reports.UserDesigner` | XAML designer control |
| `ReportDesignerDocument` | `DevExpress.Xpf.Reports.UserDesigner` | Open document in designer |
| `CachedReportSource` | `DevExpress.XtraPrinting.Caching` | Large report memory management |
| `MemoryDocumentStorage` | `DevExpress.XtraPrinting.Caching` | In-memory storage for cached source |
| `ViewModelBase` | `DevExpress.Mvvm` | Base MVVM class (optional) |

## Troubleshooting

| Symptom | Cause | Fix |
|---------|-------|-----|
| `DocumentPreviewControl` stays blank (no spinner) | `RequestDocumentCreation` not set | Add `RequestDocumentCreation="True"` to XAML |
| Preview shows spinner indefinitely | `DocumentSource` assigned but `CreateDocumentAsync` failed silently | Check Output window for exceptions; verify DataSource is set |
| `ReportDesigner` type not found | Missing XMLNS declaration | Add `xmlns:dxrud="http://schemas.devexpress.com/winfx/2008/xaml/reports/userdesigner"` |
| `PrintHelper.ShowRibbonPrintPreview` shows blank | Report DataSource null or no DetailBand | Set DataSource before calling PrintHelper; ensure DetailBand exists |
| MVVM binding: report not updating | ViewModel property doesn't raise `PropertyChanged` | Use `ViewModelBase` from DevExpress.Mvvm or implement `INotifyPropertyChanged` |
| `ReportDesigner` needs WPF license | Reporting-only subscription | Confirm WPF subscription or use `PrintHelper` / `DocumentPreviewControl` only |
| `OpenDocument` does nothing visible | Window not yet loaded | Call `OpenDocument` in `Window_Loaded`, not constructor |

## Constraints & Rules

1. **Licensing check first**: `ReportDesigner` WPF control requires WPF subscription. Use `PrintHelper` and `DocumentPreviewControl` for Reporting subscription.
2. **`RequestDocumentCreation="True"` in XAML**: Always set this when binding `DocumentSource` to an `XtraReport` via data binding — it triggers automatic document build.
3. **Async in WPF**: Use `CachedReportSource.CreateDocumentAsync()` for large reports — it keeps the UI responsive.
4. **XAML namespace**: The `dxrud:` prefix is required for `ReportDesigner`. Missing this causes designer/compiler errors.
5. **Never mix package versions**: All DevExpress NuGet packages must be the same version.
6. **Verify build**: Run `dotnet build` and check for 0 errors before reporting success.

## Navigation Guide

| Need | Reference File |
|------|---------------|
| NuGet setup, first preview | `references/getting-started.md` |
| .NET Framework setup | `references/getting-started-dotnet-fw.md` |
| DocumentPreviewControl — embedding and properties | `references/document-preview.md` |
| ReportDesigner — integration, MVVM service | `references/designer-integration.md` |
| Print API (direct print without preview) | `references/print-api.md` |

## Using DevExpress Documentation MCP

```
devexpress_docs_search(technology="WPF Reporting", query="customize document preview toolbar")
devexpress_docs_search(technology="WPF Reporting", query="report designer wizard customization")
devexpress_docs_get_content(url="<article URL>")
```

Use built-in references for: NuGet setup, DocumentPreviewControl, PrintHelper, ReportDesigner basics, MVVM binding.

Use MCP for: deep designer customization (wizard steps, toolbox categories, custom controls), appearance theming, localization, advanced MVVM service patterns.

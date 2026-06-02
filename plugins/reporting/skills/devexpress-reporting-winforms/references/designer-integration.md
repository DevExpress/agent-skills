# Designer Integration — WinForms

## When to Use This Reference

Use when launching or embedding the End-User Report Designer in a WinForms application. Two approaches are covered:

1. **ReportDesignTool** (Reporting subscription — simpler)
2. **Embedded designer components** (WinForms subscription — more control)

## Licensing Reminder

- `ReportDesignTool` → works with **Reporting subscription** only, no WinForms package needed.
- `XRDesignForm`, `XRDesignRibbonForm`, `RibbonReportDesigner` → require **WinForms subscription** (`DevExpress.Win.Design` package).

---

## Approach 1: ReportDesignTool (Reporting License)

Simplest way to show the designer. No setup beyond the Reporting NuGet package.

```csharp
using DevExpress.XtraReports.UI;
using DevExpress.LookAndFeel;

// Ribbon designer (modern UI) — non-modal
var tool = new ReportDesignTool(new SalesReport());
tool.ShowRibbonDesigner();

// Ribbon designer — modal
tool.ShowRibbonDesignerDialog(UserLookAndFeel.Default);

// Standard toolbar — non-modal
tool.ShowDesigner();

// Standard toolbar — modal
tool.ShowDesignerDialog(UserLookAndFeel.Default);
```

### With an existing .repx file

```csharp
var report = new XtraReport();
report.LoadLayoutFromXml("layout.repx");
report.DataSource = GetData();     // re-assign data after load

var tool = new ReportDesignTool(report);
tool.ShowRibbonDesignerDialog(UserLookAndFeel.Default);

// After the dialog closes, save the modified report
report.SaveLayoutToXml("layout.repx");
```

### VB.NET

```vb
Imports DevExpress.XtraReports.UI
Imports DevExpress.LookAndFeel

Dim tool As New ReportDesignTool(New SalesReport())
tool.ShowRibbonDesignerDialog(UserLookAndFeel.Default)
```

---

## Approach 2: Embedded Designer (WinForms License)

Requires `DevExpress.Win.Design` package and **WinForms subscription**.

### Option A: Drag-and-Drop Component (Visual Designer)

1. In Visual Studio Toolbox, find `RibbonReportDesigner` or `StandardReportDesigner`.
2. Drag it onto your form — it fills the form area.
3. In code-behind:

```csharp
using DevExpress.XtraReports.UserDesigner;

private void Form_Load(object sender, EventArgs e) {
    // Open an existing report for editing
    ribbonReportDesigner1.OpenReport(new SalesReport());

    // Or open a blank new report
    ribbonReportDesigner1.NewDocument();
}
```

### Option B: XRDesignRibbonForm Subclass

```csharp
using DevExpress.XtraReports.UserDesigner;

public class MyDesignerForm : XRDesignRibbonForm {
    public MyDesignerForm(XtraReport report) : base() {
        OpenReport(report);
    }
}

// Usage:
var form = new MyDesignerForm(new SalesReport());
form.ShowDialog();
```

### Option C: Access designer form via ReportDesignTool

```csharp
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.UserDesigner;

var tool = new ReportDesignTool(new SalesReport());

// Get access to the pre-built designer form
XRDesignRibbonForm designForm = tool.DesignRibbonForm;

// Customize before showing
designForm.DesignMdiController.DefaultReportSettings.ShowGrid = false;
designForm.RibbonControl.Toolbar.ShowCustomizationButton = false;

designForm.ShowDialog();
```

---

## Customizing the Designer

### Hide a Toolbar Button

```csharp
XRDesignRibbonForm form = tool.DesignRibbonForm;
foreach (var item in form.RibbonControl.Items) {
    if (item.Name == "Save")
        item.Visibility = BarItemVisibility.Never;
}
```

### Restrict Toolbox Items

```csharp
// Access the designer panel's toolbox
var panel = form.DesignMdiController.ActiveDesignPanel;
// Toolbox customization requires WinForms license and is complex.
// Use DxDocs MCP for detailed toolbox customization:
// devexpress_docs_search(technology="WinForms Reporting", query="customize designer toolbox")
```

### Wire Save/Load Storage

```csharp
form.DesignMdiController.ReportStorageGetData += (sender, e) => {
    e.Stream = File.OpenRead(e.Url);
};
form.DesignMdiController.ReportStorageSetData += (sender, e) => {
    using var fs = File.Create(e.Url);
    e.Stream.CopyTo(fs);
};
```

---

## Deep Customization

For deep designer customization (custom toolbox categories, wizard steps, custom controls registration, custom data source wizards), use the DevExpress Docs MCP:

```
devexpress_docs_search(technology="WinForms Reporting", query="end-user report designer customization")
devexpress_docs_search(technology="WinForms Reporting", query="report designer toolbox categories")
devexpress_docs_get_content(url="<article URL from search>")
```

---

## VB.NET — Embedded Component

```vb
Imports DevExpress.XtraReports.UserDesigner

Public Class MyDesignerForm
    Inherits XRDesignRibbonForm

    Public Sub New(report As XtraReport)
        MyBase.New()
        OpenReport(report)
    End Sub
End Class
```

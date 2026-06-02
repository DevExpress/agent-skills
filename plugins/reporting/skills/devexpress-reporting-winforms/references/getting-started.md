# Getting Started — WinForms Reporting (.NET 6+)

## When to Use This Reference

Use when setting up a new .NET 6+ WinForms project with DevExpress reporting.

## NuGet Setup

```bash
dotnet nuget add source https://nuget.devexpress.com/api --name DevExpressNuGet

# Print preview, ReportDesignTool (Reporting license)
dotnet add package DevExpress.Win.Reporting

# Embedded designer components (WinForms license required)
dotnet add package DevExpress.Win.Design
```

## Required Usings

```csharp
using DevExpress.XtraReports.UI;             // XtraReport, ReportPrintTool, ReportDesignTool
using DevExpress.XtraReports.UserDesigner;   // XRDesignForm, XRDesignRibbonForm (WinForms license)
using DevExpress.LookAndFeel;               // UserLookAndFeel.Default (for modal dialogs)
using DevExpress.XtraPrinting.Caching;     // CachedReportSource (for large reports)
```

## Minimal Print Preview

```csharp
// Form.cs (or button click handler)
private void btnPreview_Click(object sender, EventArgs e) {
    var report = new SalesReport { DataSource = GetSalesData() };
    var tool = new ReportPrintTool(report);
    tool.ShowRibbonPreviewDialog(UserLookAndFeel.Default);
}
```

## Minimal Designer Launch

```csharp
private void btnDesign_Click(object sender, EventArgs e) {
    var tool = new ReportDesignTool(new SalesReport());
    tool.ShowRibbonDesignerDialog(UserLookAndFeel.Default);
}
```

## Minimal Embedded Viewer

1. Add a `Panel` with `Dock = DockStyle.Fill` to your form.
2. In code:

```csharp
using DevExpress.XtraReports.UI;

public partial class ReportViewerForm : Form {
    DocumentViewer viewer;

    public ReportViewerForm() {
        InitializeComponent();

        viewer = new DocumentViewer { Dock = DockStyle.Fill };
        panel1.Controls.Add(viewer);
    }

    private void ReportViewerForm_Load(object sender, EventArgs e) {
        var report = new SalesReport { DataSource = GetData() };
        viewer.DocumentSource = report;
        report.CreateDocument();
    }
}
```

## Project File (.csproj)

```xml
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>WinExe</OutputType>
    <TargetFramework>net8.0-windows</TargetFramework>
    <UseWindowsForms>true</UseWindowsForms>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="DevExpress.Win.Reporting" Version="26.1.*" />
    <!-- Add DevExpress.Win.Design if WinForms license -->
  </ItemGroup>
</Project>
```

> Always set `<UseWindowsForms>true</UseWindowsForms>` in the project file.

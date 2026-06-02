# Getting Started — XtraReports Core (.NET 6+)

## When to Use This Reference

Use when setting up a new project for XtraReports runtime use: installing NuGet packages, creating a first report, and running a basic export.

## NuGet Installation

Install the platform package from nuget.org: 

```bash
# WinForms
dotnet add package DevExpress.Win.Reporting

# WPF
dotnet add package DevExpress.Wpf.Reporting

# ASP.NET Core
dotnet add package DevExpress.AspNetCore.Reporting

# Console / background service / test
dotnet add package DevExpress.Reporting.Core
```

**Linux/macOS** — add Skia for PDF rendering:
```bash
dotnet add package DevExpress.Drawing.Skia
```

All packages bring in `DevExpress.XtraReports.UI` and `DevExpress.XtraPrinting` infrastructure transitively.

## Required Usings

```csharp
using DevExpress.Drawing;              // DXMargins
using DevExpress.Drawing.Printing;     // DXPaperKind
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.Parameters;
using DevExpress.XtraPrinting;         // export options (PdfExportOptions, etc.)
```

## First Report — Step by Step

### 1. Create the report and assign data

```csharp
var report = new XtraReport();
report.DataSource = new List<Product> {
    new Product { Name = "Widget A", Price = 9.99m },
    new Product { Name = "Widget B", Price = 14.99m }
};
```

### 2. Add mandatory DetailBand

```csharp
var detail = new DetailBand();
report.Bands.Add(detail);
detail.HeightF = 30;
```

### 3. Add controls to the band

```csharp
var nameLabel = new XRLabel();
detail.Controls.Add(nameLabel);
nameLabel.LocationF = new System.Drawing.PointF(0, 0);
nameLabel.SizeF = new System.Drawing.SizeF(150, 30);
nameLabel.Font = new DevExpress.Drawing.DXFont("Arial", 10f, DevExpress.Drawing.DXFontStyle.Bold);
nameLabel.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "[Name]"));
```

### 4. Export

```csharp
report.ExportToPdf("products.pdf");
```

### Complete example

See `examples/quickstart.cs` for a full report with a report header, grouping, and PDF export.

## Report as a Class (Recommended)

For reusable reports, subclass `XtraReport`:

```csharp
public class ProductReport : XtraReport {
    public ProductReport() {
        Name = "ProductReport";
        PaperKind = DevExpress.Drawing.Printing.DXPaperKind.A4;
        Margins = new DevExpress.Drawing.DXMargins(40, 40, 40, 40);

        var detail = new DetailBand();
        Bands.Add(detail);
        detail.HeightF = 30;

        var label = new XRLabel();
        detail.Controls.Add(label);
        label.LocationF = new System.Drawing.PointF(0, 0);
        label.SizeF = new System.Drawing.SizeF(report.PageWidthF - report.Margins.Left - report.Margins.Right, 30);
        label.Font = new DevExpress.Drawing.DXFont("Arial", 10f, DevExpress.Drawing.DXFontStyle.Bold);
        label.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "[Name]"));
    }
}

// Usage
var report = new ProductReport { DataSource = products };
report.ExportToPdf("output.pdf");
```

## Load/Save Layout (.repx)

```csharp
// Save layout (structure only — no data)
report.SaveLayoutToXml("layout.repx");

// Load layout and re-assign data
var report2 = XtraReport.FromXmlFile("layout.repx");
report2.DataSource = GetData();   // data is not stored in .repx
report2.ExportToPdf("output.pdf");
```

## .NET Framework 4.x Differences

For .NET Framework projects, see `references/getting-started-dotnet-fw.md` for NuGet vs. direct assembly reference setup.

Key differences:
- `app.config` may need binding redirects for DevExpress assemblies
- DevExpress NuGet feed same URL; packages are the same names
- `async/await` patterns work in .NET Framework 4.5+ but `ExportToPdfAsync` requires a proper synchronization context

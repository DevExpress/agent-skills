# Getting Started — XtraReports Core (.NET Framework 4.x)

## When to Use This Reference

Use when targeting .NET Framework 4.6.2 or later with XtraReports runtime API.

## NuGet Installation

```powershell
# Package Manager Console
Install-Package DevExpress.Win.Reporting          # WinForms
Install-Package DevExpress.Wpf.Reporting          # WPF
Install-Package DevExpress.Web.Reporting          # ASP.NET Web Forms and MVC
Install-Package DevExpress.Reporting.Core         # console/service
```

## Assembly References (without NuGet)

Minimum assemblies for export-only (no viewer):
- `DevExpress.Data.v<XX.X>.dll`
- `DevExpress.Drawing.v<XX.X>.dll`
- `DevExpress.Printing.v<XX.X>.Core.dll`
- `DevExpress.XtraReports.v<XX.X>.dll`
- `DevExpress.Pdf.v<XX.X>.Core.dll` (PDF export)
- `DevExpress.Pdf.v<XX.X>.Drawing.dll` (PDF rendering)

## Differences from .NET 8+

| Aspect | .NET Framework | .NET 8+ |
|--------|---------------|---------|
| Async export | Available (`ExportToPdfAsync`) | Available |
| `ExpressionBinding` | Same API | Same API |
| `LoadLayoutFromXml` | Same | Same |
| `app.config` binding redirects | May be required | Not applicable |
| SkiaSharp | Not required | Linux/macOS only |

## First Report

```csharp
using DevExpress.XtraReports.UI;

var report = new XtraReport();
report.Margings = new DevExpress.Drawing.DXMargins(50, 50, 50, 50);
report.PaperKind = DevExpress.Drawing.Printing.DXPaperKind.Letter;
report.DataSource = GetData();

var detail = new DetailBand();
report.Bands.Add(detail);
detail.HeightF = 30;

var label = new XRLabel();
detail.Controls.Add(label);
label.LocationF = new System.Drawing.PointF(0, 0);
label.SizeF = new System.Drawing.SizeF(report.PageWidthF - report.Margins.Left - report.Margins.Right, 30);
label.Font = new DevExpress.Drawing.DXFont("Arial", 10f, DevExpress.Drawing.DXFontStyle.Bold);
label.ExpressionBindings.Add(new ExpressionBinding("BeforePrint", "Text", "[Name]"));

report.ExportToPdf("output.pdf");
```

```vb
Imports DevExpress.XtraReports.UI

Dim report As New XtraReport()
report.DataSource = GetData()

Dim detail As New DetailBand()
detail.HeightF = 30
report.Bands.Add(detail)

Dim label As New XRLabel()
detail.Controls.Add(label)
label.BoundsF = New System.Drawing.RectangleF(0, 0, 200, 25)
label.ExpressionBindings.Add(New ExpressionBinding("BeforePrint", "Text", "[Name]"))

report.ExportToPdf("output.pdf")
```

## app.config Binding Redirects

If you encounter assembly version conflicts, add to `app.config`:

```xml
<configuration>
  <runtime>
    <assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">
      <dependentAssembly>
        <assemblyIdentity name="DevExpress.Data.v26.1" publicKeyToken="b88d1754d700e49a" />
        <bindingRedirect oldVersion="0.0.0.0-65535.65535.65535.65535" newVersion="26.1.X.0" />
      </dependentAssembly>
    </assemblyBinding>
  </runtime>
</configuration>
```

Replace `X` with the build number from your installed package.

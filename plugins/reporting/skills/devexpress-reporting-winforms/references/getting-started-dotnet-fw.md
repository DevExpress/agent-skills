# Getting Started — WinForms Reporting (.NET Framework 4.x)

## When to Use This Reference

Use when targeting .NET Framework 4.6.2 or later with DevExpress WinForms Reporting.

## NuGet Setup

```powershell
Install-Package DevExpress.Win.Reporting
Install-Package DevExpress.Win.Design    # if WinForms license — embedded designer
```

Or add via NuGet.Config:
```xml
<packageSources>
  <add key="DevExpress" value="https://nuget.devexpress.com/api" />
</packageSources>
```

## Assembly References (without NuGet)

Minimum for print preview:
- `DevExpress.Data.v<XX.X>.dll`
- `DevExpress.Drawing.v<XX.X>.dll`
- `DevExpress.Utils.v<XX.X>.dll`
- `DevExpress.XtraEditors.v<XX.X>.dll`
- `DevExpress.XtraBars.v<XX.X>.dll`
- `DevExpress.XtraPrinting.v<XX.X>.dll`
- `DevExpress.XtraReports.v<XX.X>.dll`

For embedded designer (WinForms license):
- Add `DevExpress.XtraReports.v<XX.X>.Extensions.dll`
- Add `DevExpress.XtraTreeList.v<XX.X>.dll`
- Add `DevExpress.CodeParser.v<XX.X>.dll`

## First Preview (VB.NET)

```vb
Imports DevExpress.XtraReports.UI
Imports DevExpress.LookAndFeel

Private Sub btnPreview_Click(sender As Object, e As EventArgs) Handles btnPreview.Click
    Dim report As New SalesReport()
    report.DataSource = GetSalesData()

    Dim tool As New ReportPrintTool(report)
    tool.ShowRibbonPreviewDialog(UserLookAndFeel.Default)
End Sub
```

## First Designer (VB.NET)

```vb
Imports DevExpress.XtraReports.UI
Imports DevExpress.LookAndFeel

Private Sub btnDesign_Click(sender As Object, e As EventArgs) Handles btnDesign.Click
    Dim tool As New ReportDesignTool(New SalesReport())
    tool.ShowRibbonDesignerDialog(UserLookAndFeel.Default)
End Sub
```

## Differences from .NET 6+

| Aspect | .NET Framework | .NET 6+ |
|--------|---------------|---------|
| Project file | `packages.config` or `<PackageReference>` | `<PackageReference>` in SDK-style .csproj |
| `UseWindowsForms` | Not required (implicit) | Required in .csproj |
| `app.config` binding redirects | May be required | Not applicable |
| Visual designer | Supported | Supported (.NET 8 WinForms designer in VS 2022+) |

## app.config Binding Redirects

```xml
<runtime>
  <assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">
    <dependentAssembly>
      <assemblyIdentity name="DevExpress.XtraPrinting.v26.1" publicKeyToken="b88d1754d700e49a" />
      <bindingRedirect oldVersion="0.0.0.0-65535.65535.65535.65535" newVersion="26.1.X.0" />
    </dependentAssembly>
  </assemblyBinding>
</runtime>
```

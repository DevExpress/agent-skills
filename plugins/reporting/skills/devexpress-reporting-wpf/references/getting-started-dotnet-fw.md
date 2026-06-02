# Getting Started — WPF Reporting (.NET Framework 4.x)

## When to Use This Reference

Use when targeting .NET Framework 4.6.2 or later with DevExpress WPF Reporting.

## NuGet Setup

```powershell
Install-Package DevExpress.Wpf.Reporting
Install-Package DevExpress.Mvvm          # optional
```

## Assembly References (without NuGet)

Minimum for `DocumentPreviewControl` and `PrintHelper`:
- `DevExpress.Data.v<XX.X>.dll`
- `DevExpress.Drawing.v<XX.X>.dll`
- `DevExpress.Mvvm.v<XX.X>.dll`
- `DevExpress.Printing.v<XX.X>.Core.dll`
- `DevExpress.Xpf.Core.v<XX.X>.dll`
- `DevExpress.Xpf.DocumentViewer.v<XX.X>.Core.dll`
- `DevExpress.Xpf.Printing.v<XX.X>.dll`
- `DevExpress.Xpf.Ribbon.v<XX.X>.dll`
- `DevExpress.XtraReports.v<XX.X>.dll`

For `ReportDesigner` (WPF license):
- Add `DevExpress.Xpf.ReportDesigner.v<XX.X>.dll`
- Add `DevExpress.Xpf.DataAccess.v<XX.X>.dll`
- Add `DevExpress.Xpf.ExpressionEditor.v<XX.X>.dll`
- Add `DevExpress.CodeParser.v<XX.X>.dll`

## First Preview (VB.NET)

```vb
Imports DevExpress.Xpf.Printing
Imports DevExpress.XtraReports.UI

Private Sub btnPreview_Click(sender As Object, e As RoutedEventArgs)
    Dim report As New SalesReport()
    report.DataSource = GetData()
    PrintHelper.ShowRibbonPrintPreviewDialog(Me, report)
End Sub
```

## First Preview — XAML + Code-Behind (VB.NET)

```xaml
<dxp:DocumentPreviewControl x:Name="preview" RequestDocumentCreation="True" />
```

```vb
Private Sub Window_Loaded(sender As Object, e As RoutedEventArgs) Handles Me.Loaded
    preview.DocumentSource = New SalesReport() With {.DataSource = GetData()}
End Sub
```

## Differences from .NET 6+

| Aspect | .NET Framework | .NET 6+ |
|--------|---------------|---------|
| Project file | Standard XML .csproj | SDK-style .csproj |
| `UseWPF` | Not required (implicit) | Required in .csproj |
| `app.config` redirects | May be required | Not applicable |
| MVVM pattern | Same POCO/ViewModelBase API | Same |

## app.config Binding Redirects

```xml
<runtime>
  <assemblyBinding xmlns="urn:schemas-microsoft-com:asm.v1">
    <dependentAssembly>
      <assemblyIdentity name="DevExpress.Xpf.Printing.v26.1" publicKeyToken="b88d1754d700e49a" />
      <bindingRedirect oldVersion="0.0.0.0-65535.65535.65535.65535" newVersion="26.1.X.0" />
    </dependentAssembly>
  </assemblyBinding>
</runtime>
```

# Viewer Customization - WPF Reporting

## When to Use This Reference

Use this reference to customize `DevExpress.Xpf.Printing.DocumentPreviewControl`: its toolbar,
panels, parameter editors, and commands. **WPF subscription required** for advanced
customization. Basic embedding and `CommandBarStyle` changes work with a Reporting subscription.

For `ReportDesigner` dock panels, toolbar, commands, and other Designer-specific UI, see
[designer-customization.md](designer-customization.md).

## Customize the Document Preview Toolbar

Customize the toolbar's UI: remove, update, or insert toolbar items. Use
`DevExpress.Xpf.Printing.DocumentCommandProvider.RibbonActions` for the Ribbon UI, or
`DevExpress.Xpf.DocumentViewer.CommandProvider.Actions` for a classic Bar toolbar.

### Toolbar Style (Ribbon vs. Bars vs. None)

Set in XAML:

```xaml
<dxp:DocumentPreviewControl x:Name="preview"
                           CommandBarStyle="Ribbon"
                           RequestDocumentCreation="True"
                           DocumentSource="{Binding Report}" />
```

Options: `Ribbon` (modern), `Bars` (classic), `None` (no toolbar):

```xaml
<!-- Modern Ribbon toolbar (default) -->
<dxp:DocumentPreviewControl CommandBarStyle="Ribbon" ... />

<!-- Classic toolbar with drop-down menus -->
<dxp:DocumentPreviewControl CommandBarStyle="Bars" ... />

<!-- No toolbar at all -->
<dxp:DocumentPreviewControl CommandBarStyle="None" ... />
```

### Hide Specific Toolbar Commands

Use `RemoveAction` with `DefaultPreviewBarItemNames` in the `DocumentCommandProvider`:

```xaml
<dxp:DocumentPreviewControl RequestDocumentCreation="True">
    <dxp:DocumentPreviewControl.CommandProvider>
        <dxp:DocumentCommandProvider>
            <dxp:DocumentCommandProvider.RibbonActions>
                <dxb:RemoveAction ElementName="{x:Static dxpbars:DefaultPreviewBarItemNames.ExportPdf}"/>
            </dxp:DocumentCommandProvider.RibbonActions>
        </dxp:DocumentCommandProvider>
    </dxp:DocumentPreviewControl.CommandProvider>
</dxp:DocumentPreviewControl>
```

Required namespaces:

```xaml
xmlns:dxb="http://schemas.devexpress.com/winfx/2008/xaml/bars"
xmlns:dxpbars="http://schemas.devexpress.com/winfx/2008/xaml/printing/bars"
```

### Update Toolbar Items

Use `UpdateAction` with `DefaultPreviewBarItemNames` in the `DocumentCommandProvider`:

```xaml
<dxp:DocumentPreviewControl RequestDocumentCreation="True">
    <dxp:DocumentPreviewControl.CommandProvider>
        <dxp:DocumentCommandProvider>
            <dxp:DocumentCommandProvider.RibbonActions>
                <dxb:UpdateAction ElementName="{x:Static dxpbars:DefaultPreviewBarItemNames.Print}"
                    Property="{x:Static dxb:BarItem.LargeGlyphProperty}"
                    Value="{dxp:PrintingResourceImage ResourceName='Images/BarItems/Print.svg'}" />
            </dxp:DocumentCommandProvider.RibbonActions>
        </dxp:DocumentCommandProvider>
    </dxp:DocumentPreviewControl.CommandProvider>
</dxp:DocumentPreviewControl>
```

### Add Toolbar Items

Use `InsertAction` with `DefaultPreviewBarItemNames` in the `DocumentCommandProvider`:

```xaml
<dxp:DocumentPreviewControl RequestDocumentCreation="True">
    <dxp:DocumentPreviewControl.CommandProvider>
        <dxp:DocumentCommandProvider>
            <dxp:DocumentCommandProvider.RibbonActions>
                 <dxb:InsertAction ContainerName="{x:Static dxpbars:DefaultPreviewBarItemNames.ExportGroup}">
                    <dxb:InsertAction.Element>
                        <dxb:BarButtonItem Content="Clear Preview" ItemClick="ClearPreview_ItemClick" />
                    </dxb:InsertAction.Element>
                </dxb:InsertAction>
            </dxp:DocumentCommandProvider.RibbonActions>
        </dxp:DocumentCommandProvider>
    </dxp:DocumentPreviewControl.CommandProvider>
</dxp:DocumentPreviewControl>
```

## Manage Panel Visibility

- `DevExpress.Xpf.Printing.DocumentPreviewControl.AutoShowDocumentMap` automatically displays
    the Document Map when the report contains bookmarks.
- `DevExpress.Xpf.Printing.DocumentPreviewControl.AutoShowParametersPanel` automatically displays
    the Parameters panel when the report contains visible parameters.
- `DevExpress.Xpf.Printing.DocumentPreviewPropertyProvider.ShowDocumentMap` controls Document Map
    visibility.
- `DevExpress.Xpf.Printing.DocumentPreviewPropertyProvider.ShowParametersPanel` controls
    Parameters panel visibility.
- `DevExpress.Xpf.Printing.DocumentPreviewControl.ShowThumbnails` displays or hides the
    Thumbnails panel.

## Custom Parameter Editor

For custom parameter editors, use DevExpress Documentation MCP to review
[Provide Custom Editors for Report Parameters](https://docs.devexpress.com/XtraReports/17763/wpf-reporting/wpf-reporting-document-preview/api-and-customization/provide-custom-editors-for-report-parameters).

## Appearance & Theming

`DevExpress.Xpf.Printing.DocumentPreviewControl` and
`DevExpress.Xpf.Reports.UserDesigner.ReportDesigner` use the `Office2019Colorful` theme by default.
Set `DevExpress.Xpf.Core.ApplicationThemeHelper.ApplicationThemeName` during application startup to apply another
theme to the entire application. This also affects `ReportDesigner`; see
[designer-customization.md](designer-customization.md) for its component-specific customization.

`DevExpress.Wpf.Themes.All` NuGet package contains all DevExpress themes.
Or, each separate theme requires the corresponding NuGet package or assembly. For example,
`DevExpress.Xpf.Core.Theme.Win11Dark` requires `DevExpress.Xpf.Themes.Win11Dark.v26.1` (.NET Framework) or `DevExpress.Wpf.Themes.Win11Dark` (.NET).

### Apply a Theme at Startup

```bash
dotnet add package DevExpress.Wpf.Themes.Office2016Black
```

# [C#](#tab/csharp-theme-startup)
```csharp
using DevExpress.Xpf.Core;
using System.Windows;

public partial class App : Application {
    protected override void OnStartup(StartupEventArgs e) {
        ApplicationThemeHelper.ApplicationThemeName = Theme.Office2016BlackName;
        base.OnStartup(e);
    }
}
```

# [VB.NET](#tab/vbnet-theme-startup)
```vb
Imports DevExpress.Xpf.Core

Class App
    Inherits Application

    Protected Overrides Sub OnStartup(e As StartupEventArgs)
        ApplicationThemeHelper.ApplicationThemeName = Theme.Office2016BlackName
        MyBase.OnStartup(e)
    End Sub
End Class
```
***

### Change the Theme at Runtime

# [C#](#tab/csharp-theme-runtime)
```csharp
ApplicationThemeHelper.UpdateApplicationThemeName(Theme.Office2016BlackName);
```

# [VB.NET](#tab/vbnet-theme-runtime)
```vb
ApplicationThemeHelper.UpdateApplicationThemeName(Theme.Office2016BlackName)
```
***

## Override Document Preview Commands

Override command behavior without changing the toolbar. Derive from
`DevExpress.Xpf.Printing.DocumentPreviewControl` and override the method behind the required command. The following
example keeps the Export UI but disables PDF export.

# [C#](#tab/csharp-override-commands)
```csharp
using DevExpress.Xpf.Printing;
using DevExpress.XtraPrinting;

public class CustomDocumentPreviewControl : DocumentPreviewControl {
    public override bool CanExport(ExportFormat? format) {
        return format != ExportFormat.Pdf && base.CanExport(format);
    }

    public override void Export(ExportFormat? format) {
        // Add custom export logic here.
        base.Export(format);
    }
}
```

# [VB.NET](#tab/vbnet-override-commands)
```vb
Imports DevExpress.Xpf.Printing
Imports DevExpress.XtraPrinting

Public Class CustomDocumentPreviewControl
    Inherits DocumentPreviewControl

    Public Overrides Function CanExport(format As ExportFormat?) As Boolean
        Return format <> ExportFormat.Pdf AndAlso MyBase.CanExport(format)
    End Function

    Public Overrides Sub Export(format As ExportFormat?)
        ' Add custom export logic here.
        MyBase.Export(format)
    End Sub
End Class
```
***

## Execute Document Preview Commands from Application UI

Use the preview command objects to invoke standard actions from application controls.

# [C#](#tab/csharp-preview-commands)
```csharp
if (documentPreview.SetZoomModeCommand.CanExecute(ZoomMode.FitToWidth))
    documentPreview.SetZoomModeCommand.Execute(ZoomMode.FitToWidth);

if (documentPreview.PrintCommand.CanExecute(null))
    documentPreview.PrintCommand.Execute(null);
```

# [VB.NET](#tab/vbnet-preview-commands)
```vb
If documentPreview.SetZoomModeCommand.CanExecute(ZoomMode.FitToWidth) Then
    documentPreview.SetZoomModeCommand.Execute(ZoomMode.FitToWidth)
End If

If documentPreview.PrintCommand.CanExecute(Nothing) Then
    documentPreview.PrintCommand.Execute(Nothing)
End If
```
***
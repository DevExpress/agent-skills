# Designer Customization - WPF Reporting

## When to Use This Reference

Use this reference to customize `DevExpress.Xpf.Reports.UserDesigner.ReportDesigner`: dock panels,
toolbar and context-menu actions, data source wizard options, commands, Report Gallery storage, and
the toolbox. **WPF subscription required** for advanced customization.

For stand-alone `DocumentPreviewControl` toolbar, panels, and commands, see
[viewer-customization.md](viewer-customization.md). Application themes affect both controls and
are documented in that reference.

## Hide and Customize Report Designer Dock Panels

Use Report Designer option objects to configure dock panel visibility and behavior.

```xaml
<dxrud:ReportDesigner Name="designer">
    <dxrud:ReportDesigner.GroupAndSortPanelOptions>
        <dxrud:GroupAndSortPanelOptions Visibility="Closed" />
    </dxrud:ReportDesigner.GroupAndSortPanelOptions>
    <dxrud:ReportDesigner.ReportExplorerOptions>
        <dxrud:ReportExplorerOptions Visibility="Hidden" />
    </dxrud:ReportDesigner.ReportExplorerOptions>
    <dxrud:ReportDesigner.FieldListOptions>
        <dxrud:FieldListOptions SortOrder="Descending" />
    </dxrud:ReportDesigner.FieldListOptions>
    <dxrud:ReportDesigner.PropertyGridOptions>
        <dxrud:PropertyGridOptions UseTabbedView="False" ShowFavoriteProperties="True" />
    </dxrud:ReportDesigner.PropertyGridOptions>
    <dxrud:ReportDesigner.ErrorPanelOptions>
        <dxrud:ErrorPanelOptions Visibility="Closed" />
    </dxrud:ReportDesigner.ErrorPanelOptions>
</dxrud:ReportDesigner>
```

## Customize the Report Designer Toolbar

Use `DevExpress.Xpf.Reports.UserDesigner.ReportDesigner.RibbonCustomizationActions` to remove built-in items or insert application commands into
the Designer and Preview tabs.

```xaml
<dxrud:ReportDesigner x:Name="reportDesigner">
    <dxrud:ReportDesigner.RibbonCustomizationActions>
        <dxb:RemoveAction ElementName="{x:Static dxrud:DefaultBarItemNames.SaveAs}" />
        <dxb:RemoveAction ContainerName="{x:Static dxrud:DefaultBarItemNames.DesignerHomePage}"
                          ElementName="{x:Static dxrud:DefaultBarItemNames.FontGroup}" />
        <dxb:InsertAction ContainerName="{x:Static dxrud:DefaultBarItemNames.ReportGroup}">
            <dxb:InsertAction.Element>
                <dxb:BarButtonItem Content="Close Document"
                                   ItemClick="CloseDocumentButton_ItemClick" />
            </dxb:InsertAction.Element>
        </dxb:InsertAction>
    </dxrud:ReportDesigner.RibbonCustomizationActions>
</dxrud:ReportDesigner>
```

# [C#](#tab/csharp-designer-toolbar)
```csharp
private void CloseDocumentButton_ItemClick(
    object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e) {
    if (reportDesigner.ActiveDocument != null)
        reportDesigner.ActiveDocument.Close();
}
```

# [VB.NET](#tab/vbnet-designer-toolbar)
```vb
Private Sub CloseDocumentButton_ItemClick(
    sender As Object, e As DevExpress.Xpf.Bars.ItemClickEventArgs)
    If reportDesigner.ActiveDocument IsNot Nothing Then
        reportDesigner.ActiveDocument.Close()
    End If
End Sub
```
***

## Configure Report Gallery Storage

Assign a `DevExpress.XtraReports.Extensions.ReportGalleryDirectoryExtension` to use a different file for gallery templates.

# [C#](#tab/csharp-gallery-storage)
```csharp
string path = Path.Combine(
    Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory), "ReportGallery.xml");
reportDesigner.ReportGalleryOptions.Storage = new ReportGalleryDirectoryExtension(path);
```

# [VB.NET](#tab/vbnet-gallery-storage)
```vb
Dim path = Path.Combine(
    Path.GetDirectoryName(AppDomain.CurrentDomain.BaseDirectory), "ReportGallery.xml")
reportDesigner.ReportGalleryOptions.Storage = New ReportGalleryDirectoryExtension(path)
```
***

## Add a Custom Control to the Report Designer Toolbox

Register an existing custom report control with the Report Designer toolbox.

# [C#](#tab/csharp-toolbox-control)
```csharp
private void reportDesigner_Loaded(object sender, RoutedEventArgs e) {
    ReportDesigner.RegisterControl<NumericLabel>();
    reportDesigner.OpenDocument(new XtraReport1());
}
```

# [VB.NET](#tab/vbnet-toolbox-control)
```vb
Private Sub reportDesigner_Loaded(sender As Object, e As RoutedEventArgs)
    ReportDesigner.RegisterControl(Of NumericLabel)()
    reportDesigner.OpenDocument(New XtraReport1())
End Sub
```
***

## Restrict Data Providers in the Data Source Wizard

Restrict the SQL providers displayed in the Data Source Wizard through
`DevExpress.Xpf.Reports.UserDesigner.ReportDesignerBase.DataSourceWizardSettings`.

```xaml
<dxrud:ReportDesigner DataSourceWizardSettings="{dxda:DataSourceWizardSettings
    SqlWizardSettings={dxda:SqlWizardSettings AvailableDataProviders=MSSqlServer}}" />
```

For different restrictions at runtime, use an `IWizardCustomizationService` registered through
`ReportDesigner.ServicesRegistry`.

# [C#](#tab/csharp-wizard-service)
```csharp
reportDesigner.ServicesRegistry.Add(new TypeEntry {
    ServiceType = typeof(IWizardCustomizationService),
    ConcreteType = typeof(MyWizardCustomizationService)
});
```

# [VB.NET](#tab/vbnet-wizard-service)
```vb
reportDesigner.ServicesRegistry.Add(New TypeEntry With {
    .ServiceType = GetType(IWizardCustomizationService),
    .ConcreteType = GetType(MyWizardCustomizationService)
})
```
***

## Hide Properties in the Report Designer

Handle `DevExpress.XtraReports.UI.XtraReport.FilterComponentProperties` to remove properties from the Properties panel.

# [C#](#tab/csharp-hide-properties)
```csharp
XtraReport.FilterComponentProperties += XtraReport_FilterComponentProperties;

static void XtraReport_FilterComponentProperties(
    object sender, FilterComponentPropertiesEventArgs e) {
    e.Properties.Remove("Scripts");
}
```

# [VB.NET](#tab/vbnet-hide-properties)
```vb
AddHandler XtraReport.FilterComponentProperties, AddressOf XtraReport_FilterComponentProperties

Private Shared Sub XtraReport_FilterComponentProperties(
    sender As Object, e As FilterComponentPropertiesEventArgs)
    e.Properties.Remove("Scripts")
End Sub
```
***

## Override Report Designer Commands and Hot Keys

Assign a `DevExpress.Xpf.Reports.UserDesigner.ReportDesignerCommands` descendant to
`DevExpress.Xpf.Reports.UserDesigner.ReportDesigner.Commands` to replace built-in
command behavior. Register, unregister, or replace shortcuts with the Report Designer hot-key API.

# [C#](#tab/csharp-designer-commands)
```csharp
public class CustomDesignerCommands : ReportDesignerCommands {
    protected override void SaveDocumentAs() {
        Debug.WriteLine("CustomDesignerCommands.SaveDocumentAs");
        base.SaveDocumentAs();
    }
}

designer.Commands = new CustomDesignerCommands();

// Register a custom shortcut for a ViewModel command.
designer.RegisterHotKey(Key.OemTilde, ModifierKeys.Control, () => viewModel.TestCommand, null);

// Remove all default shortcuts when required.
designer.UnregisterHotKeys();
```

# [VB.NET](#tab/vbnet-designer-commands)
```vb
Public Class CustomDesignerCommands
    Inherits ReportDesignerCommands

    Protected Overrides Sub SaveDocumentAs()
        Debug.WriteLine("CustomDesignerCommands.SaveDocumentAs")
        MyBase.SaveDocumentAs()
    End Sub
End Class

designer.Commands = New CustomDesignerCommands()

' Register a custom shortcut for a ViewModel command.
designer.RegisterHotKey(Key.OemTilde, ModifierKeys.Control, Function() viewModel.TestCommand, Nothing)

' Remove all default shortcuts when required.
designer.UnregisterHotKeys()
```
***

## Customize Report Designer Context Menus

Use `DevExpress.Xpf.Reports.UserDesigner.ReportDesigner.ContextMenuCustomizationActions` to remove standard menu items or add commands for
specific report elements.

```xaml
<dxrud:ReportDesigner>
    <dxrud:ReportDesigner.ContextMenuCustomizationActions>
        <dxb:RemoveAction ContainerName="{x:Static dxrud:DefaultContextMenuItemNames.LabelContextMenu}"
                          ElementName="{x:Static dxrud:DefaultContextMenuItemNames.Cut}" />
        <dxb:RemoveAction ElementName="{x:Static dxrud:DefaultContextMenuItemNames.Properties}" />
        <dxb:InsertAction ContainerName="{x:Static dxrud:DefaultContextMenuItemNames.LabelContextMenu}"
                          Index="0">
            <dxb:InsertAction.Element>
                <dxb:BarButtonItem Content="About" ItemClick="About_ItemClick" />
            </dxb:InsertAction.Element>
        </dxb:InsertAction>
    </dxrud:ReportDesigner.ContextMenuCustomizationActions>
</dxrud:ReportDesigner>
```

# [C#](#tab/csharp-context-menu)
```csharp
private void About_ItemClick(object sender, DevExpress.Xpf.Bars.ItemClickEventArgs e) {
    MessageBox.Show("About Window", "About");
}
```

# [VB.NET](#tab/vbnet-context-menu)
```vb
Private Sub About_ItemClick(sender As Object, e As DevExpress.Xpf.Bars.ItemClickEventArgs)
    MessageBox.Show("About Window", "About")
End Sub
```
***
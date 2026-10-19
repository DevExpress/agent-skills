# Designer Customization - WinForms Reporting

## When to Use This Reference

Use when customizing commands, toolbars, wizard data source types, or parameter types in the WinForms End-User Report Designer. **Requires WinForms/Universal/DXPerience subscription**.

For print preview toolbar, parameter editor, printing command, and dock panel customization, see [viewer-customization.md](viewer-customization.md).

## Register a Custom Parameter Type in the Report Designer

To use a custom parameter type in the WinForms End-User Report Designer and save it in a REPX
layout:

1. Derive a custom extension from `DevExpress.XtraReports.Extensions.ReportDesignExtension`.
2. Override `DevExpress.XtraReports.Extensions.ReportDesignExtension.AddParameterTypes` to add the
   type and its display name.
3. Register the extension with
   `DevExpress.XtraReports.Extensions.ReportDesignExtension.RegisterExtension`.
4. Associate the report with the extension through
   `DevExpress.XtraReports.Extensions.ReportDesignExtension.AssociateReportWithExtension`.

## Hide Designer Commands

Call `DevExpress.XtraReports.UserDesigner.XRDesignMdiController.SetCommandVisibility` before opening the Ribbon Designer. This example hides both entry points for the Report Wizard. For a standard designer, use `DevExpress.XtraReports.UI.ReportDesignTool.DesignForm` and `DevExpress.XtraReports.UI.ReportDesignTool.ShowDesignerDialog()` instead.

# [C#](#tab/csharp-hide-commands)

```csharp
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.UserDesigner;

var tool = new ReportDesignTool(new XtraReport());
IDesignForm form = tool.DesignRibbonForm;
XRDesignMdiController controller = form.DesignMdiController;
controller.SetCommandVisibility(ReportCommand.NewReportWizard, CommandVisibility.None);
controller.SetCommandVisibility(ReportCommand.VerbReportWizard, CommandVisibility.None);
tool.ShowRibbonDesignerDialog();
```

# [VB.NET](#tab/vbnet-hide-commands)

```vb
Imports DevExpress.XtraReports.UI
Imports DevExpress.XtraReports.UserDesigner

Dim tool As New ReportDesignTool(New XtraReport())
Dim form As IDesignForm = tool.DesignRibbonForm
Dim controller As XRDesignMdiController = form.DesignMdiController
controller.SetCommandVisibility(ReportCommand.NewReportWizard, CommandVisibility.None)
controller.SetCommandVisibility(ReportCommand.VerbReportWizard, CommandVisibility.None)
tool.ShowRibbonDesignerDialog()
```

***

Source: [Hide Commands from the End-User Report Designer](https://docs.devexpress.com/XtraReports/2747/winforms-reporting/end-user-report-designer-for-winforms/api-and-customization/hide-commands-from-end-user-report-designer).

## Override Open and Save Commands

Register the Open handler with `DevExpress.XtraReports.UserDesigner.XRDesignMdiController.AddCommandHandler`. Register the Save handler with `DevExpress.XtraReports.UserDesigner.XRDesignPanel.AddCommandHandler` for each panel in `DevExpress.XtraReports.UserDesigner.XRDesignMdiController.DesignPanelLoaded`. Replace the sample file path and Open action with your application logic. This command-based approach does not require `DevExpress.XtraReports.Extensions.ReportStorageExtension`.

# [C#](#tab/csharp-override-commands)

```csharp
using DevExpress.XtraReports.UI;
using DevExpress.XtraReports.UserDesigner;
using System.Windows.Forms;

var form = new XRDesignForm();
XRDesignMdiController controller = form.DesignMdiController;
controller.AddCommandHandler(new OpenCommandHandler());
controller.DesignPanelLoaded += (sender, e) =>
    ((XRDesignPanel)sender).AddCommandHandler(new SaveCommandHandler((XRDesignPanel)sender));
controller.OpenReport(new XtraReport());
form.ShowDialog();
if (controller.ActiveDesignPanel != null)
    controller.ActiveDesignPanel.CloseReport();

public class SaveCommandHandler : ICommandHandler {
    readonly XRDesignPanel panel;
    public SaveCommandHandler(XRDesignPanel panel) => this.panel = panel;

    public bool CanHandleCommand(ReportCommand command, ref bool useNextHandler) {
        useNextHandler = command != ReportCommand.SaveFile && command != ReportCommand.SaveFileAs;
        return !useNextHandler;
    }

    public void HandleCommand(ReportCommand command, object[] args) {
        panel.Report.SaveLayout("report1.repx");
        panel.ReportState = ReportState.Saved;
    }
}

public class OpenCommandHandler : ICommandHandler {
    public bool CanHandleCommand(ReportCommand command, ref bool useNextHandler) {
        useNextHandler = command != ReportCommand.OpenFile;
        return !useNextHandler;
    }

    public void HandleCommand(ReportCommand command, object[] args) {
        MessageBox.Show("Open a report using your application logic.");
    }
}
```

# [VB.NET](#tab/vbnet-override-commands)

```vb
Imports DevExpress.XtraReports.UI
Imports DevExpress.XtraReports.UserDesigner
Imports System.Windows.Forms

' In a Form method:
Dim designForm As New XRDesignForm()
Dim controller As XRDesignMdiController = designForm.DesignMdiController
controller.AddCommandHandler(New OpenCommandHandler())
AddHandler controller.DesignPanelLoaded, Sub(sender, e)
    Dim panel As XRDesignPanel = DirectCast(sender, XRDesignPanel)
    panel.AddCommandHandler(New SaveCommandHandler(panel))
End Sub
controller.OpenReport(New XtraReport())
designForm.ShowDialog()
If controller.ActiveDesignPanel IsNot Nothing Then
    controller.ActiveDesignPanel.CloseReport()
End If

Public Class SaveCommandHandler
    Implements ICommandHandler

    Private ReadOnly panel As XRDesignPanel
    Public Sub New(panel As XRDesignPanel)
        Me.panel = panel
    End Sub

    Public Function CanHandleCommand(command As ReportCommand, ByRef useNextHandler As Boolean) As Boolean Implements ICommandHandler.CanHandleCommand
        useNextHandler = command <> ReportCommand.SaveFile AndAlso command <> ReportCommand.SaveFileAs
        Return Not useNextHandler
    End Function

    Public Sub HandleCommand(command As ReportCommand, args As Object()) Implements ICommandHandler.HandleCommand
        panel.Report.SaveLayout("report1.repx")
        panel.ReportState = ReportState.Saved
    End Sub
End Class

Public Class OpenCommandHandler
    Implements ICommandHandler

    Public Function CanHandleCommand(command As ReportCommand, ByRef useNextHandler As Boolean) As Boolean Implements ICommandHandler.CanHandleCommand
        useNextHandler = command <> ReportCommand.OpenFile
        Return Not useNextHandler
    End Function

    Public Sub HandleCommand(command As ReportCommand, args As Object()) Implements ICommandHandler.HandleCommand
        MessageBox.Show("Open a report using your application logic.")
    End Sub
End Class
```

***

Source: [Override Commands in the End-User Report Designer](https://docs.devexpress.com/XtraReports/2211/winforms-reporting/end-user-report-designer-for-winforms/api-and-customization/override-commands-in-end-user-report-designer-implement-custom-saving).

## Add a Toolbar Button

In a custom standard designer form, add a `DevExpress.XtraBars.BarButtonItem` through the Design Bar Manager. In a custom Ribbon designer form, add a `DevExpress.XtraBars.Ribbon.RibbonPageGroup`, then add a button to that group. Set `DevExpress.XtraBars.BarItem.Caption` and attach `DevExpress.XtraBars.BarItem.ItemClick` in the form designer. The event handler is the same for both variants:

# [C#](#tab/csharp-add-button)

```csharp
using DevExpress.XtraBars;
using System.Windows.Forms;

private void barButtonItem1_ItemClick(object sender, ItemClickEventArgs e) {
    MessageBox.Show("About this report designer", "About");
}
```

# [VB.NET](#tab/vbnet-add-button)

```vb
Imports DevExpress.XtraBars
Imports System.Windows.Forms

Private Sub barButtonItem1_ItemClick(sender As Object, e As ItemClickEventArgs) Handles barButtonItem1.ItemClick
    MessageBox.Show("About this report designer", "About")
End Sub
```

***

Sources: [Standard Toolbar](https://docs.devexpress.com/XtraReports/2557/winforms-reporting/end-user-report-designer-for-winforms/api-and-customization/add-items-to-a-report-designers-standard-toolbar), [Ribbon Toolbar](https://docs.devexpress.com/XtraReports/3301/winforms-reporting/end-user-report-designer-for-winforms/api-and-customization/add-items-to-a-report-designers-ribbon-toolbar).

## Restrict Data Source Types in the Wizards

Implement `DevExpress.XtraReports.Wizards.IWizardCustomizationService` and register it with `DevExpress.XtraReports.UserDesigner.XRDesignMdiController.AddService` before invoking a wizard. This example removes Excel and Object binding from both the Report Wizard and the Data Source Wizard.

# [C#](#tab/csharp-restrict-data-sources)

```csharp
using DevExpress.DataAccess.UI.Wizard;
using DevExpress.DataAccess.Wizard.Model;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.XtraReports.Wizards;
using System.ComponentModel.Design;

class WizardCustomizationService : IWizardCustomizationService {
    void IWizardCustomizationService.CustomizeReportWizard(IWizardCustomization<XtraReportModel> tool) {
        RemoveTypes(tool);
    }

    void IWizardCustomizationService.CustomizeDataSourceWizard(IWizardCustomization<XtraReportModel> tool) {
        RemoveTypes(tool);
    }

    static void RemoveTypes(IWizardCustomization<XtraReportModel> tool) {
        var types = (DataSourceTypes)tool.Resolve(typeof(DataSourceTypes));
        types.Remove(DataSourceType.Excel);
        types.Remove(DataSourceType.Object);
    }

    bool IWizardCustomizationService.TryCreateDataSource(IDataSourceModel model, out object dataSource, out string dataMember) {
        dataSource = null;
        dataMember = null;
        return false;
    }

    bool IWizardCustomizationService.TryCreateReport(IDesignerHost host, XtraReportModel model, object dataSource, string dataMember) {
        return false;
    }
}

// In the form constructor, after InitializeComponent():
reportDesigner1.AddService(typeof(IWizardCustomizationService), new WizardCustomizationService());
// Optional: invoke the wizard after registration.
reportDesigner1.CreateNewReportWizard();
```

# [VB.NET](#tab/vbnet-restrict-data-sources)

```vb
Imports DevExpress.DataAccess.UI.Wizard
Imports DevExpress.DataAccess.Wizard.Model
Imports DevExpress.DataAccess.Wizard.Services
Imports DevExpress.XtraReports.Wizards
Imports System.ComponentModel.Design

Public Class WizardCustomizationService
    Implements IWizardCustomizationService

    Private Sub CustomizeReportWizard(tool As IWizardCustomization(Of XtraReportModel)) Implements IWizardCustomizationService.CustomizeReportWizard
        RemoveTypes(tool)
    End Sub

    Private Sub CustomizeDataSourceWizard(tool As IWizardCustomization(Of XtraReportModel)) Implements IWizardCustomizationService.CustomizeDataSourceWizard
        RemoveTypes(tool)
    End Sub

    Private Shared Sub RemoveTypes(tool As IWizardCustomization(Of XtraReportModel))
        Dim types As DataSourceTypes = DirectCast(tool.Resolve(GetType(DataSourceTypes)), DataSourceTypes)
        types.Remove(DataSourceType.Excel)
        types.Remove(DataSourceType.Object)
    End Sub

    Private Function TryCreateDataSource(model As IDataSourceModel, ByRef dataSource As Object, ByRef dataMember As String) As Boolean Implements IWizardCustomizationService.TryCreateDataSource
        dataSource = Nothing
        dataMember = Nothing
        Return False
    End Function

    Private Function TryCreateReport(host As IDesignerHost, model As XtraReportModel, dataSource As Object, dataMember As String) As Boolean Implements IWizardCustomizationService.TryCreateReport
        Return False
    End Function
End Class

' In the form constructor, after InitializeComponent():
ReportDesigner1.AddService(GetType(IWizardCustomizationService), New WizardCustomizationService())
' Optional: invoke the wizard after registration.
ReportDesigner1.CreateNewReportWizard()
```

***

Source: [Remove Data Source Types from the Report Wizard](https://docs.devexpress.com/XtraReports/115724/winforms-reporting/end-user-report-designer-for-winforms/api-and-customization/remove-data-source-types-from-the-report-wizard).

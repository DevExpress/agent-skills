# Viewer Customization - WinForms Reporting

## When to Use This Reference

Use when customizing the print preview toolbar, ribbon, parameter editors, printing commands, or dock panels. **Requires WinForms/Universal/DXPerience subscription**.

For custom parameter types in the End-User Report Designer, see [designer-customization.md](designer-customization.md).

## Print Preview Toolbar Customization

### Add a Custom Button

Custom toolbar and ribbon buttons are added at design time. When the
`DevExpress.XtraPrinting.Preview.DocumentViewer` is already on the form, ask the user to add the
button through the control's smart tag (**Create Standard/Ribbon Toolbar**, then **Add Button**).
Once the button exists, handle its `DevExpress.XtraBars.BarItem.ItemClick` event and add the
following code:

# [C#](#tab/csharp-add-button)
```csharp
using DevExpress.XtraReports.UI;

private void EditBarButtonItem_ItemClick(object sender, DevExpress.XtraBars.ItemClickEventArgs e) {
    var report = new SalesReport();
    var designTool = new ReportDesignTool(report);
    designTool.ShowRibbonDesignerDialog();
}
```

# [VB.NET](#tab/vbnet-add-button)
```vb
Imports DevExpress.XtraReports.UI

Private Sub EditBarButtonItem_ItemClick(sender As Object, e As DevExpress.XtraBars.ItemClickEventArgs)
    Dim report = New SalesReport()
    Dim designTool = New ReportDesignTool(report)
    designTool.ShowRibbonDesignerDialog()
End Sub
```
***

See [Add Items to a Print Preview's Standard Toolbar](https://docs.devexpress.com/XtraReports/2568/winforms-reporting/winforms-reporting-print-preview/api-and-customization/add-items-to-a-print-previews-standard-toolbar)
and [Add Items to a Print Preview's Ribbon Toolbar](https://docs.devexpress.com/XtraReports/7070/winforms-reporting/winforms-reporting-print-preview/api-and-customization/add-items-to-a-print-previews-ribbon-toolbar).

### Show/Hide Built-in Commands

Use `DevExpress.XtraPrinting.PrintingSystemBase.SetCommandVisibility` to hide or display built-in
toolbar buttons and menu items. Access the printing system through
`DevExpress.XtraPrinting.PrintToolBase.PrintingSystem`.

# [C#](#tab/csharp-hide-commands)
```csharp
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;

var tool = new ReportPrintTool(report);

// Hide the Watermark and Excel export commands
tool.PrintingSystem.SetCommandVisibility(
    new[] { PrintingSystemCommand.Watermark, PrintingSystemCommand.ExportXlsx },
    DevExpress.XtraPrinting.CommandVisibility.None);

tool.ShowRibbonPreviewDialog();
```

# [VB.NET](#tab/vbnet-hide-commands)
```vb
Imports DevExpress.XtraPrinting
Imports DevExpress.XtraReports.UI

Dim tool = New ReportPrintTool(report)

' Hide the Watermark and Excel export commands
tool.PrintingSystem.SetCommandVisibility(
    New PrintingSystemCommand() {PrintingSystemCommand.Watermark, PrintingSystemCommand.ExportXlsx},
    DevExpress.XtraPrinting.CommandVisibility.None)

tool.ShowRibbonPreviewDialog()
```
***

## Custom Parameter Editors

### Replace a Parameter Editor in Print Preview

# [C#](#tab/csharp-parameter-editor)
```csharp
using DevExpress.XtraEditors;
using DevExpress.XtraEditors.Controls;
using DevExpress.XtraReports.Parameters;

private void XtraReport_ParametersRequestBeforeShow(
    object sender, ParametersRequestEventArgs e) {
    var radioGroup = CreateRadioGroup();

    foreach (var info in e.ParametersInformation) {
        if (info.Parameter.Name == "shape") {
            info.Editor = radioGroup;
            break;
        }
    }
}

private RadioGroup CreateRadioGroup() {
    var radioGroup = new RadioGroup();
    string[] items = { "Circle", "Rectangle", "Ellipse", "Triangle", "Square" };

    foreach (var item in items) {
        radioGroup.Properties.Items.Add(new RadioGroupItem(item, item));
    }

    radioGroup.Properties.ItemVertAlignment = RadioItemVertAlignment.Top;
    radioGroup.MinimumSize = new System.Drawing.Size(0, 100);
    return radioGroup;
}
```

# [VB.NET](#tab/vbnet-parameter-editor)
```vb
Imports DevExpress.XtraEditors
Imports DevExpress.XtraEditors.Controls
Imports DevExpress.XtraReports.Parameters

Private Sub XtraReport_ParametersRequestBeforeShow(
        sender As Object, e As ParametersRequestEventArgs)
    Dim radioGroup = CreateRadioGroup()

    For Each info In e.ParametersInformation
        If info.Parameter.Name = "shape" Then
            info.Editor = radioGroup
            Exit For
        End If
    Next
End Sub

Private Function CreateRadioGroup() As RadioGroup
    Dim radioGroup = New RadioGroup()
    Dim items As String() = {"Circle", "Rectangle", "Ellipse", "Triangle", "Square"}

    For Each item In items
        radioGroup.Properties.Items.Add(New RadioGroupItem(item, item))
    Next

    radioGroup.Properties.ItemVertAlignment = RadioItemVertAlignment.Top
    radioGroup.MinimumSize = New System.Drawing.Size(0, 100)
    Return radioGroup
End Function
```
***

Handle `DevExpress.XtraReports.UI.XtraReport.ParametersRequestBeforeShow` and assign a
`DevExpress.XtraEditors.BaseEdit` descendant to
`DevExpress.XtraReports.Parameters.ParameterInfo.Editor`. Use `System.Windows.Forms.Control.MinimumSize`
and `System.Windows.Forms.Control.MaximumSize` to size the editor.

## Printing System Commands

### Override a Command

Override a built-in command's behavior with a custom
`DevExpress.XtraPrinting.ICommandHandler`, registered through
`DevExpress.XtraPrinting.PrintingSystemBase.AddCommandHandler`.

# [C#](#tab/csharp-override-command)
```csharp
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;

var tool = new ReportPrintTool(report);
tool.Report.CreateDocument(false);  // build the document to access its PrintingSystem
tool.PrintingSystem.AddCommandHandler(new ExportToImageCommandHandler());
tool.ShowRibbonPreviewDialog();

// ...

public class ExportToImageCommandHandler : DevExpress.XtraPrinting.ICommandHandler {
    public void HandleCommand(
        PrintingSystemCommand command,
        object[] args,
        IPrintControl printControl,
        ref bool handled) {

        if (command != PrintingSystemCommand.ExportGraphic)
            return;

        printControl.PrintingSystem.ExportToImage(
            "Report.png",
            System.Drawing.Imaging.ImageFormat.Png);

        handled = true;  // prevent the default export
    }

    public bool CanHandleCommand(
        PrintingSystemCommand command,
        IPrintControl printControl) {

        return command == PrintingSystemCommand.ExportGraphic;
    }
}
```

# [VB.NET](#tab/vbnet-override-command)
```vb
Imports DevExpress.XtraPrinting
Imports DevExpress.XtraReports.UI

Dim tool = New ReportPrintTool(report)
tool.Report.CreateDocument(False)  ' build the document to access its PrintingSystem
tool.PrintingSystem.AddCommandHandler(New ExportToImageCommandHandler())
tool.ShowRibbonPreviewDialog()

' ...

Public Class ExportToImageCommandHandler
    Implements DevExpress.XtraPrinting.ICommandHandler

    Public Sub HandleCommand(
            command As PrintingSystemCommand,
            args As Object(),
            printControl As IPrintControl,
            ByRef handled As Boolean) _
            Implements DevExpress.XtraPrinting.ICommandHandler.HandleCommand

        If command <> PrintingSystemCommand.ExportGraphic Then
            Return
        End If

        printControl.PrintingSystem.ExportToImage(
            "Report.png",
            System.Drawing.Imaging.ImageFormat.Png)

        handled = True  ' prevent the default export
    End Sub

    Public Function CanHandleCommand(
            command As PrintingSystemCommand,
            printControl As IPrintControl) As Boolean _
            Implements DevExpress.XtraPrinting.ICommandHandler.CanHandleCommand

        Return command = PrintingSystemCommand.ExportGraphic
    End Function
End Class
```
***

### Available Commands

Common `DevExpress.XtraPrinting.PrintingSystemCommand` enumeration values:

- `Print`, `PrintDirect` - Print to printer
- `ShowFirstPage`, `ShowPrevPage`, `ShowNextPage`, `ShowLastPage`
- `ExportPdf`, `ExportDocx`, `ExportXlsx`, `ExportXls`, `ExportCsv`, `ExportTxt`, `ExportRtf`, `ExportMht`, `ExportHtm`, `ExportGraphic`
- `ExportFile` - Export the document using the export command
- `SendFile` - Export and e-mail the document
- `Find` - Find text in document
- `ZoomIn`, `ZoomOut`, `ZoomToPageWidth`, `ZoomToWholePage`
- `PageSetup` - Paper size, orientation, margins
- `Thumbnails`, `DocumentMap`, `Parameters` - Dock panels

## Dock Panel Customization

Print Preview panels (Parameters, Document Map, Thumbnails) are dock panels maintained by a
`DevExpress.XtraBars.Docking.DockManager`. Access it through
`DevExpress.XtraPrinting.Control.PrintControl.DockManager`.

### Hide/Show Panels

Access the `PrintControl` after the preview form is initialized, then toggle dock panels through
`DevExpress.XtraPrinting.Control.PrintControl.ExecCommand` with the corresponding command:

# [C#](#tab/csharp-dock-toggle)
```csharp
using DevExpress.XtraPrinting;
using DevExpress.XtraPrinting.Control;
using DevExpress.XtraReports.UI;

var tool = new ReportPrintTool(report);

tool.PreviewRibbonForm.Shown += (s, e) => {
    PrintControl printControl = tool.PreviewRibbonForm.PrintControl;

    // Hide the Thumbnails and Document Map panels
    if (printControl.CanExecCommand(PrintingSystemCommand.Thumbnails))
        printControl.ExecCommand(
            PrintingSystemCommand.Thumbnails,
            new object[] { false });

    if (printControl.CanExecCommand(PrintingSystemCommand.DocumentMap))
        printControl.ExecCommand(
            PrintingSystemCommand.DocumentMap,
            new object[] { false });
};

// Remove their toolbar buttons
tool.PrintingSystem.SetCommandVisibility(
    new[] {
        PrintingSystemCommand.Thumbnails,
        PrintingSystemCommand.DocumentMap
    },
    DevExpress.XtraPrinting.CommandVisibility.None);

tool.ShowRibbonPreview();
```


# [VB.NET](#tab/vbnet-dock-toggle)
```vb
Imports DevExpress.XtraPrinting
Imports DevExpress.XtraPrinting.Control
Imports DevExpress.XtraReports.UI

Dim tool = New ReportPrintTool(report)

AddHandler tool.PreviewRibbonForm.Shown, Sub(s, e)
    Dim printControl As PrintControl = tool.PreviewRibbonForm.PrintControl

    ' Hide the Thumbnails and Document Map panels
    If printControl.CanExecCommand(PrintingSystemCommand.Thumbnails) Then
        printControl.ExecCommand(
            PrintingSystemCommand.Thumbnails,
            New Object() {False})
    End If

    If printControl.CanExecCommand(PrintingSystemCommand.DocumentMap) Then
        printControl.ExecCommand(
            PrintingSystemCommand.DocumentMap,
            New Object() {False})
    End If
End Sub

' Remove their toolbar buttons
tool.PrintingSystem.SetCommandVisibility(
    New PrintingSystemCommand() {
        PrintingSystemCommand.Thumbnails,
        PrintingSystemCommand.DocumentMap
    },
    DevExpress.XtraPrinting.CommandVisibility.None)

tool.ShowRibbonPreview()
```
***

### Save/Restore Panel Layout

`DevExpress.XtraBars.Docking.DockManager.SaveLayoutToXml` and
`DevExpress.XtraBars.Docking.DockManager.RestoreLayoutFromXml` (plus registry and stream
overloads) persist the user's panel layout.

The `DockManager` is not initialized when `ReportPrintTool` is constructed. Retrieve it after the
preview form is shown, then reuse that instance while the form is still active:

# [C#](#tab/csharp-dock-layout)
```csharp
using System.IO;
using DevExpress.XtraBars.Docking;
using DevExpress.XtraReports.UI;

var tool = new ReportPrintTool(report);
DockManager dockManager = null;

tool.PreviewRibbonForm.Shown += (s, e) => {
    dockManager = tool.PreviewRibbonForm.PrintControl.DockManager;

    if (File.Exists("PreviewLayout.xml"))
        dockManager.RestoreLayoutFromXml("PreviewLayout.xml");
};

tool.PreviewRibbonForm.FormClosing += (s, e) => {
    if (dockManager != null && !dockManager.IsDisposed)
        dockManager.SaveLayoutToXml("PreviewLayout.xml");
};

tool.ShowRibbonPreview();
```

# [VB.NET](#tab/vbnet-dock-layout)
```vb
Imports System.IO
Imports DevExpress.XtraBars.Docking
Imports DevExpress.XtraReports.UI

Dim tool = New ReportPrintTool(report)
Dim dockManager As DockManager = Nothing

AddHandler tool.PreviewRibbonForm.Shown, Sub(s, e)
    dockManager = tool.PreviewRibbonForm.PrintControl.DockManager

    If File.Exists("PreviewLayout.xml") Then
        dockManager.RestoreLayoutFromXml("PreviewLayout.xml")
    End If
End Sub

AddHandler tool.PreviewRibbonForm.FormClosing, Sub(s, e)
    If dockManager IsNot Nothing AndAlso Not dockManager.IsDisposed Then
        dockManager.SaveLayoutToXml("PreviewLayout.xml")
    End If
End Sub

tool.ShowRibbonPreview()
```
***

## Common Customization Patterns

### Pattern 1 - Minimal toolbar (only export)

# [C#](#tab/csharp-pattern-minimal)
```csharp
var tool = new ReportPrintTool(report);
var preview = tool.PreviewRibbonForm;

// Hide all pages except first
var ribbon = preview.RibbonControl;
foreach (var item in ribbon.Items.OfType<BarItem>()) {
    if (item.Caption?.Contains("Page") == true) {
        item.Enabled = false;
    }
}

tool.ShowRibbonPreview();
```

# [VB.NET](#tab/vbnet-pattern-minimal)
```vb
Dim tool = New ReportPrintTool(report)
Dim preview = tool.PreviewRibbonForm

' Hide all pages except first
Dim ribbon = preview.RibbonControl
For Each item In ribbon.Items.OfType(Of BarItem)()
    If item.Caption IsNot Nothing AndAlso item.Caption.Contains("Page") Then
        item.Enabled = False
    End If
Next

tool.ShowRibbonPreview()
```
***

### Pattern 2 - Read-only preview (no export, no print)

Use the top-level export and send commands instead of listing individual export formats. This also
covers formats such as TXT without requiring the list to be updated when additional export formats
are available.

# [C#](#tab/csharp-pattern-readonly)
```csharp
using DevExpress.XtraPrinting;
using DevExpress.XtraReports.UI;

var tool = new ReportPrintTool(report);

// Hide all export, send, and print commands
tool.PrintingSystem.SetCommandVisibility(
    new[] {
        PrintingSystemCommand.ExportFile,
        PrintingSystemCommand.SendFile,
        PrintingSystemCommand.Print,
        PrintingSystemCommand.PrintDirect
    },
    DevExpress.XtraPrinting.CommandVisibility.None);

tool.ShowRibbonPreviewDialog();
```

# [VB.NET](#tab/vbnet-pattern-readonly)
```vb
Imports DevExpress.XtraPrinting
Imports DevExpress.XtraReports.UI

Dim tool = New ReportPrintTool(report)

' Hide all export, send, and print commands
tool.PrintingSystem.SetCommandVisibility(
    New PrintingSystemCommand() {
        PrintingSystemCommand.ExportFile,
        PrintingSystemCommand.SendFile,
        PrintingSystemCommand.Print,
        PrintingSystemCommand.PrintDirect
    },
    DevExpress.XtraPrinting.CommandVisibility.None)

tool.ShowRibbonPreviewDialog()
```
***
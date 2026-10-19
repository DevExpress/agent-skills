# Storage & Security — WPF Reporting

## When to Use This Reference

Use when implementing custom report storage (database, file system), securing SQL query access, protecting connection strings, implementing multi-user report sharing.

## Custom Report Storage

Use a custom report storage when the application needs to load and save report layouts from a database, service, file storage, or another custom location instead of working directly with REPX files.

For the WPF Report Designer, implement the `DevExpress.Xpf.Reports.UserDesigner.IReportStorage` interface.

### Register Custom Report Storage

Assign the custom storage instance to the
`DevExpress.Xpf.Reports.UserDesigner.ReportDesignerBase.ReportStorage` property.

# [C#](#tab/csharp-register-storage)

```csharp
reportDesigner.ReportStorage = new CustomReportStorage();
```

# [VB.NET](#tab/vbnet-register-storage)
```vb
reportDesigner.ReportStorage = New CustomReportStorage()
```
***

For example, this can be done when the window is loaded:

# [C#](#tab/csharp-register-storage-loaded)
```csharp
public partial class MainWindow : Window {
    public MainWindow() {
        InitializeComponent();
        Loaded += MainWindow_Loaded;
    }


    void MainWindow_Loaded(object sender, RoutedEventArgs e) {
        reportDesigner.ReportStorage = new CustomReportStorage();
    }
}
```

# [VB.NET](#tab/vbnet-register-storage-loaded)
```vb
Partial Public Class MainWindow
    Inherits Window

    Public Sub New()
        InitializeComponent()
        AddHandler Loaded, AddressOf MainWindow_Loaded
    End Sub

    Private Sub MainWindow_Loaded(sender As Object, e As RoutedEventArgs)
        reportDesigner.ReportStorage = New CustomReportStorage()
    End Sub
End Class
```
***

### Implement IReportStorage

Create a class that implements `DevExpress.Xpf.Reports.UserDesigner.IReportStorage`.

# [C#](#tab/csharp-report-storage)
```csharp
using DevExpress.Xpf.Reports.UserDesigner;
using DevExpress.XtraReports.UI;


public class CustomReportStorage : IReportStorage {
    public bool CanCreateNew() {
        return true;
    }


    public bool CanOpen() {
        return true;
    }


    public XtraReport CreateNew() {
        return new XtraReport();
    }


    public XtraReport CreateNewSubreport() {
        return new XtraReport();
    }


    public XtraReport Load(
        string reportId,
        IReportSerializer designerReportSerializer) {


        byte[] layout = LoadReportLayout(reportId);


        using var stream = new MemoryStream(layout);
        return XtraReport.FromStream(stream, true);
    }


    public string Save(
        string reportId,
        IReportProvider reportProvider,
        bool saveAs,
        string reportTitle,
        IReportDesignerUI designer) {


        XtraReport report = reportProvider.GetReport();


        if (reportId == null) {
            reportId = Guid.NewGuid().ToString();
            saveAs = true;
        }


        if (saveAs && !ShowSaveAsDialog(ref reportTitle, designer))
            return null;


        using var stream = new MemoryStream();
        report.SaveLayoutToXml(stream);


        SaveReportLayout(
            reportId,
            reportTitle,
            stream.ToArray());


        return reportId;
    }


    public string Open(IReportDesignerUI designer) {
        // Display your report selection UI and return
        // the ID of the selected report.
        return SelectReportId();
    }


    public string GetErrorMessage(Exception exception) {
        return exception.Message;
    }


    // Application helper called from Save(); not an IReportStorage member.
    bool ShowSaveAsDialog(
        ref string reportTitle,
        IReportDesignerUI designer) {


        // Display custom Save As UI and update reportTitle.
        return true;
    }


    byte[] LoadReportLayout(string reportId) {
        // Load the serialized report layout from your storage.
        throw new NotImplementedException();
    }


    void SaveReportLayout(
        string reportId,
        string reportTitle,
        byte[] layout) {


        // Persist the serialized report layout in your storage.
        throw new NotImplementedException();
    }


    string SelectReportId() {
        // Return an ID understood by Load().
        throw new NotImplementedException();
    }
}
```

# [VB.NET](#tab/vbnet-report-storage)
```vb
Imports DevExpress.Xpf.Reports.UserDesigner
Imports DevExpress.XtraReports.UI

Public Class CustomReportStorage
    Implements IReportStorage

    Public Function CanCreateNew() As Boolean Implements IReportStorage.CanCreateNew
        Return True
    End Function

    Public Function CanOpen() As Boolean Implements IReportStorage.CanOpen
        Return True
    End Function

    Public Function CreateNew() As XtraReport Implements IReportStorage.CreateNew
        Return New XtraReport()
    End Function

    Public Function CreateNewSubreport() As XtraReport Implements IReportStorage.CreateNewSubreport
        Return New XtraReport()
    End Function

    Public Function Load(reportId As String, serializer As IReportSerializer) As XtraReport _
        Implements IReportStorage.Load
        Dim layout = LoadReportLayout(reportId)
        Using stream = New MemoryStream(layout)
            Return XtraReport.FromStream(stream, True)
        End Using
    End Function

    Public Function Save(reportId As String, reportProvider As IReportProvider,
                         saveAs As Boolean, reportTitle As String,
                         designer As IReportDesignerUI) As String _
        Implements IReportStorage.Save
        Dim report = reportProvider.GetReport()
        If reportId Is Nothing Then
            reportId = Guid.NewGuid().ToString()
            saveAs = True
        End If

        If saveAs AndAlso Not ShowSaveAsDialog(reportTitle, designer) Then
            Return Nothing
        End If

        Using stream = New MemoryStream()
            report.SaveLayoutToXml(stream)
            SaveReportLayout(reportId, reportTitle, stream.ToArray())
        End Using
        Return reportId
    End Function

    Public Function Open(designer As IReportDesignerUI) As String Implements IReportStorage.Open
        Return SelectReportId()
    End Function

    Public Function GetErrorMessage(exception As Exception) As String _
        Implements IReportStorage.GetErrorMessage
        Return exception.Message
    End Function

    ' Application helper called from Save(); not an IReportStorage member.
    Private Function ShowSaveAsDialog(ByRef reportTitle As String,
                                      designer As IReportDesignerUI) As Boolean
        ' Display custom Save As UI and update reportTitle.
        Return True
    End Function

    Private Function SelectReportId() As String
        Throw New NotImplementedException()
    End Function
End Class
```
***

### Automatic File-Based Report Storage

Use a dialog-free `DevExpress.Xpf.Reports.UserDesigner.IReportStorage` implementation when the application determines where reports are stored and users do not need to select files or report names.

The following example stores report layouts as `.repx` files in a predefined local folder. It does not display file-picker or custom report-selection dialogs.


# [C#](#tab/csharp-automatic-file-storage)

```csharp
using DevExpress.Xpf.Reports.UserDesigner;
using DevExpress.XtraReports.UI;
using System;
using System.IO;

public class FileReportStorage : IReportStorage {
    readonly string folder;

    public FileReportStorage() {
        folder = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "Reports");
        Directory.CreateDirectory(folder);
    }

    public bool CanCreateNew() {
        return true;
    }

    public bool CanOpen() {
        return true;
    }

    public XtraReport CreateNew() {
        return new XtraReport();
    }

    public XtraReport CreateNewSubreport() {
        return new XtraReport();
    }

    public XtraReport Load(
        string reportId,
        IReportSerializer designerReportSerializer) {

        using var stream = File.OpenRead(GetPath(reportId));
        return XtraReport.FromStream(stream, true);
    }

    public string Save(
        string reportId,
        IReportProvider reportProvider,
        bool saveAs,
        string reportTitle,
        IReportDesignerUI designer) {

        XtraReport report = reportProvider.GetReport();

        if (string.IsNullOrEmpty(reportId))
            reportId = Guid.NewGuid().ToString();

        using var stream = new MemoryStream();
        report.SaveLayoutToXml(stream);
        File.WriteAllBytes(GetPath(reportId), stream.ToArray());

        return reportId;
    }

    public string Open(IReportDesignerUI designer) {
        var files = Directory.GetFiles(folder, "*.repx");

        return files.Length > 0
            ? Path.GetFileNameWithoutExtension(files[0])
            : string.Empty;
    }

    public string GetErrorMessage(Exception exception) {
        return exception.Message;
    }

    string GetPath(string reportId) {
        return Path.Combine(folder, reportId + ".repx");
    }
}
```

Assign the storage to the Report Designer:

```csharp
reportDesigner.ReportStorage = new FileReportStorage();
```

Use this pattern when report persistence should be automatic. The storage maps report IDs to `.repx` files in a predefined location and saves and restores reports through their XML layouts.

Assign the storage implementation to the `DevExpress.Xpf.Reports.UserDesigner.ReportDesignerBase.ReportStorage` property.

Do not introduce `OpenFileDialog`, `SaveFileDialog`, or custom report-selection dialogs when the application is expected to determine the storage location and report ID automatically.

Use the dialog-based `DevExpress.Xpf.Reports.UserDesigner.IReportStorage` pattern below only when users need to select a report or provide a report name.

### Implement the Open Dialog

`Open()` is responsible for displaying custom report selection UI and returning the selected report ID.

# [C#](#tab/csharp-open-dialog)
```csharp
public string Open(IReportDesignerUI designer) {
    var dialog = new ReportSelectionWindow();


    if (dialog.ShowDialog() == true)
        return dialog.SelectedReportId;


    return string.Empty;
}
```

# [VB.NET](#tab/vbnet-open-dialog)
```vb
Public Function Open(designer As IReportDesignerUI) As String
    Dim dialog = New ReportSelectionWindow()

    If dialog.ShowDialog() = True Then
        Return dialog.SelectedReportId
    End If

    Return String.Empty
End Function
```
***

Return an empty string when the operation is canceled.

### Implement Save As

`IReportStorage` has no Save As member — handle Save As inside `Save` when `saveAs` is `true`. The `ShowSaveAsDialog` method below is an application helper that `Save` calls when the application needs custom UI for entering or selecting a report name.

# [C#](#tab/csharp-save-as)
```csharp
bool ShowSaveAsDialog(
    ref string reportTitle,
    IReportDesignerUI designer) {


    var dialog = new ReportNameWindow {
        ReportTitle = reportTitle
    };


    if (dialog.ShowDialog() != true)
        return false;


    reportTitle = dialog.ReportTitle;
    return true;
}
```

# [VB.NET](#tab/vbnet-save-as)
```vb
Private Function ShowSaveAsDialog(ByRef reportTitle As String,
                                  designer As IReportDesignerUI) As Boolean
    Dim dialog = New ReportNameWindow With {.ReportTitle = reportTitle}

    If dialog.ShowDialog() <> True Then
        Return False
    End If

    reportTitle = dialog.ReportTitle
    Return True
End Function
```
***

The Save implementation can generate a new report ID for a new report:

# [C#](#tab/csharp-new-report-id)
```csharp
if (reportId == null) {
    reportId = Guid.NewGuid().ToString();
    saveAs = true;
}
```

# [VB.NET](#tab/vbnet-new-report-id)
```vb
If reportId Is Nothing Then
    reportId = Guid.NewGuid().ToString()
    saveAs = True
End If
```
***

If saveAs is false, update the existing report. If saveAs is true, obtain the new report name from ShowSaveAsDialog before saving.

## Store and Restore Data Connections

Use the following documented WPF Reporting services for connection management:

- `DevExpress.DataAccess.Wizard.Services.IConnectionStorageService` initializes, saves, and
    restores connection strings in the End-User Report Designer's Data Source Wizard.
- `DevExpress.DataAccess.Wizard.Services.IConnectionProviderService` saves and restores data
    source connections when users load reports from custom storage.

# [C#](#tab/csharp-connection-services)
```csharp
reportDesigner.ServicesRegistry.Add(new TypeEntry {
    ServiceType = typeof(IConnectionStorageService),
    ConcreteType = typeof(MyConnectionStorageService)
});

reportDesigner.ServicesRegistry.Add(new TypeEntry {
    ServiceType = typeof(IConnectionProviderService),
    ConcreteType = typeof(MyConnectionProviderService)
});
```

# [VB.NET](#tab/vbnet-connection-services)
```vb
reportDesigner.ServicesRegistry.Add(New TypeEntry With {
    .ServiceType = GetType(IConnectionStorageService),
    .ConcreteType = GetType(MyConnectionStorageService)
})

reportDesigner.ServicesRegistry.Add(New TypeEntry With {
    .ServiceType = GetType(IConnectionProviderService),
    .ConcreteType = GetType(MyConnectionProviderService)
})
```
***

## Security: Protect SQL Query Access

### Block Dangerous SQL Operations

The WPF Report Designer allows only visually constructed `SELECT` queries by default. Keep this
default restriction unless custom SQL is required, then validate custom queries before execution.

Source: [Data Access Security in WPF Applications](https://docs.devexpress.com/XtraReports/117318/wpf-reporting/wpf-reporting-application-security/data-access-security) and
[Custom SQL Query Validation (WPF)](https://docs.devexpress.com/XtraReports/403335/wpf-reporting/wpf-reporting-application-security/custom-sql-query-validation).

### Validate All Custom SQL Queries

# [C#](#tab/csharp-sql-validation)
```csharp
using DevExpress.DataAccess;
using DevExpress.DataAccess.Sql;

SqlDataSource.ValidateCustomSqlQueryGlobal += ValidateCustomSqlQuery;

void ValidateCustomSqlQuery(object sender, ValidateCustomSqlQueryEventArgs e) {
    CustomSqlQuery customQuery = e.CustomSqlQuery;
    // Reject by default; replace this with application-specific validation of customQuery.Sql.

    e.Valid = false;

}
```

# [VB.NET](#tab/vbnet-sql-validation)
```vb
Imports DevExpress.DataAccess
Imports DevExpress.DataAccess.Sql

AddHandler SqlDataSource.ValidateCustomSqlQueryGlobal, AddressOf ValidateCustomSqlQuery

Private Sub ValidateCustomSqlQuery(sender As Object, e As ValidateCustomSqlQueryEventArgs)
    Dim customQuery As CustomSqlQuery = e.CustomSqlQuery
    Dim validationResult As Boolean
    ' Insert custom validation logic here.
    e.Valid = validationResult
End Sub
```
***

### Validate a Specific SqlDataSource

# [C#](#tab/csharp-instance-validation)
```csharp
var sqlSource = new SqlDataSource("Sales", connectionString);
sqlSource.ValidateCustomSqlQuery += ValidateCustomSqlQuery;
```

# [VB.NET](#tab/vbnet-instance-validation)
```vb
Dim sqlSource = New SqlDataSource("Sales", connectionString)
AddHandler sqlSource.ValidateCustomSqlQuery, AddressOf ValidateCustomSqlQuery
```
***

## Security Checklist

Before deploying a WPF Reporting application, verify the following:

- [ ] Keep custom SQL disabled unless the application requires it.
- [ ] Validate custom SQL before execution when custom SQL is enabled.
- [ ] Use database access-control features to restrict report data access.
- [ ] Keep custom assembly loading for Entity Framework data sources disabled unless required.
- [ ] Protect report storage and limit access to reports according to application permissions.

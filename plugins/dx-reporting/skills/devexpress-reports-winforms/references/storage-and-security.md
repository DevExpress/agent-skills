# Storage & Security - WinForms Reporting

## When to Use This Reference

Use when:

- Implementing custom report storage (database, file system, network share)
- Securing data access in multi-user or untrusted environments
- Validating user-entered SQL queries
- Protecting connection strings and credentials
- Controlling user access to reports and data
- Resolving safe-deserialization errors for trusted custom types such as `ObjectDataSource` types

**NuGet package**: `DevExpress.Win.Reporting` (v26.1+)

## Custom Report Storage

To implement custom report storage, create a class derived from `ReportStorageExtension`. Treat the report URL as a storage-specific report identifier. `GetData` loads a serialized report layout, `SetData` updates an existing report, and `SetNewData` stores a new report and returns its identifier.

### Implement Report Storage

# [C#](#tab/csharp-custom-storage)
```csharp
using System;
using System.IO;
using DevExpress.XtraReports.Extensions;
using DevExpress.XtraReports.UI;

public class CustomReportStorage : ReportStorageExtension {
    public override bool IsValidUrl(string url) {
        // Validate the report identifier.
        return !string.IsNullOrWhiteSpace(url);
    }

    public override bool CanSetData(string url) {
        // Return false here if an existing report is read-only.
        return true;
    }

    public override byte[] GetData(string url) {
        // Load the serialized report layout from your storage.
        // Examples: database, file system, cloud storage, etc.
        byte[] reportLayout = LoadReportLayout(url);
        if (reportLayout == null)
            throw new InvalidOperationException($"Report '{url}' was not found.");

        return reportLayout;
    }

    public override void SetData(XtraReport report, string url) {
        // Save an existing report.
        // Use a classic using statement so the snippet also compiles in projects
        // that use C# 7.3, which is common for .NET Framework projects in VS 2022.
        using (var stream = new MemoryStream()) {
            report.SaveLayoutToXml(stream);
            SaveReportLayout(url, stream.ToArray());
        }
    }

    public override string SetNewData(XtraReport report, string defaultUrl) {
        // Generate or obtain a unique identifier for a new report.
        string url = GetUniqueReportUrl(defaultUrl);
        SetData(report, url);
        return url;
    }

    private byte[] LoadReportLayout(string url) {
        // Implement storage-specific loading.
        throw new NotImplementedException();
    }

    private void SaveReportLayout(string url, byte[] layout) {
        // Implement storage-specific saving.
        throw new NotImplementedException();
    }

    private string GetUniqueReportUrl(string defaultUrl) {
        // Implement storage-specific naming/ID generation.
        return defaultUrl;
    }
}

// Register storage in Program.cs.
ReportStorageExtension.RegisterExtensionGlobal(new CustomReportStorage());
```

# [VB.NET](#tab/vbnet-custom-storage)
```vb
Imports System
Imports System.IO
Imports DevExpress.XtraReports.Extensions
Imports DevExpress.XtraReports.UI

Public Class CustomReportStorage
    Inherits ReportStorageExtension

    Public Overrides Function IsValidUrl(url As String) As Boolean
        ' Validate the report identifier.
        Return Not String.IsNullOrWhiteSpace(url)
    End Function

    Public Overrides Function CanSetData(url As String) As Boolean
        ' Return false here if an existing report is read-only.
        Return True
    End Function

    Public Overrides Function GetData(url As String) As Byte()
        ' Load the serialized report layout from your storage.
        ' Examples: database, file system, cloud storage, etc.
        Dim reportLayout As Byte() = LoadReportLayout(url)
        If reportLayout Is Nothing Then
            Throw New InvalidOperationException($"Report '{url}' was not found.")
        End If

        Return reportLayout
    End Function

    Public Overrides Sub SetData(report As XtraReport, url As String)
        ' Save an existing report.
        Using stream = New MemoryStream()
            report.SaveLayoutToXml(stream)
            SaveReportLayout(url, stream.ToArray())
        End Using
    End Sub

    Public Overrides Function SetNewData(report As XtraReport, defaultUrl As String) As String
        ' Generate or obtain a unique identifier for a new report.
        Dim url As String = GetUniqueReportUrl(defaultUrl)
        SetData(report, url)
        Return url
    End Function

    Private Function LoadReportLayout(url As String) As Byte()
        ' Implement storage-specific loading.
        Throw New NotImplementedException()
    End Function

    Private Sub SaveReportLayout(url As String, layout As Byte())
        ' Implement storage-specific saving.
        Throw New NotImplementedException()
    End Sub

    Private Function GetUniqueReportUrl(defaultUrl As String) As String
        ' Implement storage-specific naming/ID generation.
        Return defaultUrl
    End Function
End Class

' Register storage in Program.cs.
ReportStorageExtension.RegisterExtensionGlobal(New CustomReportStorage())
```
***

### Storage by Location

| Location | Use Case | Pros | Cons |
|----------|----------|------|------|
| **File System** | Single machine, development | Simple, no DB needed | Not multi-user, lock conflicts |
| **SQL Database** | Multi-user, enterprise | Shared, versioned, backed up | Requires DB connection, complexity |
| **Network Share** | Team reporting | Easy file sync | UNC path limitations, lock conflicts |
| **Cloud Storage** | Distributed teams | Accessible anywhere | Network dependency, cost |
| **XPO/EF DbContext** | ORM-based apps | Integrated with app model | Serialization complexity |

### File System Storage Example

# [C#](#tab/csharp-filesystem-storage)
```csharp
using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using DevExpress.XtraReports.Extensions;
using DevExpress.XtraReports.UI;

public class FileSystemReportStorage : ReportStorageExtension {
    private readonly string storagePath;

    public FileSystemReportStorage(string path) {
        storagePath = path;
        Directory.CreateDirectory(storagePath);
    }

    public override bool IsValidUrl(string url) {
        return !string.IsNullOrWhiteSpace(url) &&
            !Path.GetInvalidFileNameChars().Any(url.Contains);
    }

    public override bool CanSetData(string url) {
        return true;
    }

    public override void SetData(XtraReport report, string url) {
        string filePath = Path.Combine(storagePath, url + ".repx");
        report.SaveLayoutToXml(filePath);
    }

    public override string SetNewData(XtraReport report, string defaultUrl) {
        SetData(report, defaultUrl);
        return defaultUrl;
    }

    public override byte[] GetData(string url) {
        string filePath = Path.Combine(storagePath, url + ".repx");
        if (!File.Exists(filePath))
            throw new InvalidOperationException($"Report '{url}' was not found.");

        return File.ReadAllBytes(filePath);
    }

    public override string[] GetStandardUrls(ITypeDescriptorContext context) {
        return Directory.GetFiles(storagePath, "*.repx")
            .Select(Path.GetFileNameWithoutExtension)
            .ToArray();
    }
}

// Usage
string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reports");
var reportStorage = new FileSystemReportStorage(path);
ReportStorageExtension.RegisterExtensionGlobal(reportStorage);
```

# [VB.NET](#tab/vbnet-filesystem-storage)
```vb
Imports System
Imports System.ComponentModel
Imports System.IO
Imports System.Linq
Imports DevExpress.XtraReports.Extensions
Imports DevExpress.XtraReports.UI

Public Class FileSystemReportStorage
    Inherits ReportStorageExtension

    Private ReadOnly storagePath As String

    Public Sub New(path As String)
        storagePath = path
        Directory.CreateDirectory(storagePath)
    End Sub

    Public Overrides Function IsValidUrl(url As String) As Boolean
        Return Not String.IsNullOrWhiteSpace(url) AndAlso
            Not Path.GetInvalidFileNameChars().Any(AddressOf url.Contains)
    End Function

    Public Overrides Function CanSetData(url As String) As Boolean
        Return True
    End Function

    Public Overrides Sub SetData(report As XtraReport, url As String)
        Dim filePath As String = Path.Combine(storagePath, url & ".repx")
        report.SaveLayoutToXml(filePath)
    End Sub

    Public Overrides Function SetNewData(report As XtraReport, defaultUrl As String) As String
        SetData(report, defaultUrl)
        Return defaultUrl
    End Function

    Public Overrides Function GetData(url As String) As Byte()
        Dim filePath As String = Path.Combine(storagePath, url & ".repx")
        If Not File.Exists(filePath) Then
            Throw New InvalidOperationException($"Report '{url}' was not found.")
        End If

        Return File.ReadAllBytes(filePath)
    End Function

    Public Overrides Function GetStandardUrls(context As ITypeDescriptorContext) As String()
        Return Directory.GetFiles(storagePath, "*.repx").
            Select(AddressOf Path.GetFileNameWithoutExtension).
            ToArray()
    End Function
End Class

' Usage
Dim path As String = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Reports")
Dim reportStorage = New FileSystemReportStorage(path)
ReportStorageExtension.RegisterExtensionGlobal(reportStorage)
```
***

## Security Overview

Default security configuration:

- SQL queries are created visually in the Query Builder; manual SQL editing is disabled by default.
- Connection passwords are obscured in the UI.
- Custom assemblies and potentially unsafe serialized types are restricted.
- Default custom SQL validation allows `SELECT` queries, but should not be treated as a complete security boundary.

For untrusted environments, implement additional validation and enforce database permissions.

## Secure Data Access

### 1. Disable Custom SQL Queries (Most Restrictive)

Manual SQL editing is disabled by default. Do not enable it unless your application requires end users to enter SQL text.

### 2. Enable Custom SQL in the WinForms Report Designer

In the WinForms End-User Report Designer, configure this option on the `XRDesignMdiController`:

# [C#](#tab/csharp-enable-custom-sql)
```csharp
using DevExpress.DataAccess.Sql;

var designTool = new ReportDesignTool(report);
designTool.DesignRibbonForm.DesignMdiController.DataSourceWizardSettings.SqlWizardSettings.EnableCustomSql = true;
```

# [VB.NET](#tab/vbnet-enable-custom-sql)
```vb
Imports DevExpress.DataAccess.Sql

Dim designTool = New ReportDesignTool(report)
designTool.DesignRibbonForm.DesignMdiController.DataSourceWizardSettings.SqlWizardSettings.EnableCustomSql = True
```
***

### 3. Validate Custom SQL Queries

Custom SQL queries are validated before execution. DevExpress provides both global and per-data-source validation events.

# [C#](#tab/csharp-validate-sql-global)
```csharp
using DevExpress.DataAccess;
using DevExpress.DataAccess.Sql;

private static void ValidateSqlQuery(object sender, ValidateCustomSqlQueryEventArgs e) {
    string query = e.CustomSqlQuery.Sql;

    // Add application-specific validation here.
    // Do not rely on simple keyword matching as the only security boundary.
}

// Register globally for all SqlDataSource instances.
SqlDataSource.ValidateCustomSqlQueryGlobal += ValidateSqlQuery;
```

# [VB.NET](#tab/vbnet-validate-sql-global)
```vb
Imports DevExpress.DataAccess
Imports DevExpress.DataAccess.Sql

Private Shared Sub ValidateSqlQuery(sender As Object, e As ValidateCustomSqlQueryEventArgs)
    Dim query As String = e.CustomSqlQuery.Sql

    ' Add application-specific validation here.
    ' Do not rely on simple keyword matching as the only security boundary.
End Sub

' Register globally for all SqlDataSource instances.
AddHandler SqlDataSource.ValidateCustomSqlQueryGlobal, AddressOf ValidateSqlQuery
```
***

For validation scoped to a specific data source:

# [C#](#tab/csharp-validate-sql-instance)
```csharp
var sqlDataSource = new SqlDataSource();

sqlDataSource.ValidateCustomSqlQuery += (s, e) => {
    // Validate e.CustomSqlQuery.Sql for this data source.
};
```

# [VB.NET](#tab/vbnet-validate-sql-instance)
```vb
Dim sqlDataSource = New SqlDataSource()

AddHandler sqlDataSource.ValidateCustomSqlQuery, Sub(s, e)
    ' Validate e.CustomSqlQuery.Sql for this data source.
End Sub
```
***

The default validation mechanism is not a replacement for database permissions. Use a database account that has only the permissions required for reporting.

### 4. Manage Database Access Control

Use database-level access control rather than relying only on application-side SQL validation.

```sql
CREATE LOGIN ReportUser WITH PASSWORD = '<PASSWORD_FROM_SECURE_PROVISIONING>';

CREATE USER ReportUser FOR LOGIN ReportUser;
GRANT SELECT ON Sales TO ReportUser;
GRANT SELECT ON Products TO ReportUser;
DENY SELECT ON Employees TO ReportUser;
```

The reporting application should connect with a restricted account instead of an administrative database account.

### 5. Protect Connection Strings

`SqlDataSource` does not expose `ConnectionString` or `StoreConnectionNameOnly` properties directly. Connection parameters belong to the data connection used by the data source.

Prefer a named connection and resolve the actual credentials from application configuration or a custom connection provider at runtime:

# [C#](#tab/csharp-connection-name)
```csharp
using DevExpress.DataAccess.Sql;

var sqlDataSource = new SqlDataSource {
    ConnectionName = "ReportDb"
};
```

# [VB.NET](#tab/vbnet-connection-name)
```vb
Imports DevExpress.DataAccess.Sql

Dim sqlDataSource = New SqlDataSource With {
    .ConnectionName = "ReportDb"
}
```
***

If you create connection parameters in code, assign them through `ConnectionParameters`:

# [C#](#tab/csharp-connection-parameters)
```csharp
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Sql;

var connectionParameters = new MsSqlConnectionParameters {
    ServerName = "server",
    DatabaseName = "mydb",
    UserName = "ReportUser",
    Password = GetPasswordFromSecureStorage(),
    AuthorizationType = MsSqlAuthorizationType.SqlServer
};

var sqlDataSource = new SqlDataSource {
    ConnectionName = "ReportDb",
    ConnectionParameters = connectionParameters
};
```

# [VB.NET](#tab/vbnet-connection-parameters)
```vb
Imports DevExpress.DataAccess.ConnectionParameters
Imports DevExpress.DataAccess.Sql

Dim connectionParameters = New MsSqlConnectionParameters With {
    .ServerName = "server",
    .DatabaseName = "mydb",
    .UserName = "ReportUser",
    .Password = GetPasswordFromSecureStorage(),
    .AuthorizationType = MsSqlAuthorizationType.SqlServer
}

Dim sqlDataSource = New SqlDataSource With {
    .ConnectionName = "ReportDb",
    .ConnectionParameters = connectionParameters
}
```
***

To serialize only the connection name, set `StoreConnectionNameOnly` on the underlying data connection after it is created:

# [C#](#tab/csharp-store-connection-name)
```csharp
using DevExpress.DataAccess;

var connection = sqlDataSource.Connection as DataConnectionBase;
if (connection != null)
    connection.StoreConnectionNameOnly = true;
```

# [VB.NET](#tab/vbnet-store-connection-name)
```vb
Imports DevExpress.DataAccess

Dim connection = TryCast(sqlDataSource.Connection, DataConnectionBase)
If connection IsNot Nothing Then
    connection.StoreConnectionNameOnly = True
End If
```
***

For designer scenarios where connection parameters must be supplied at runtime, use the following services from `DevExpress.DataAccess.Wizard.Services`:

- [IConnectionProviderService](https://docs.devexpress.com/CoreLibraries/DevExpress.DataAccess.Wizard.Services.IConnectionProviderService) resolves a named connection through `LoadConnection(string connectionName)`, returning a `SqlDataConnection` with parameters obtained from application configuration or secure storage.
- [IConnectionStorageService](https://docs.devexpress.com/CoreLibraries/DevExpress.DataAccess.Wizard.Services.IConnectionStorageService) manages the connections available in the Data Source Wizard, including saving and restoring connection settings.

Register both services with `XRDesignMdiController` before opening the designer and with `DesignerLoadedEventArgs.DesignerHost` during `DesignPanelLoaded`, as shown below. Registration alone does not suppress connection parameter serialization: set [IDataConnection.StoreConnectionNameOnly](https://docs.devexpress.com/CoreLibraries/DevExpress.DataAccess.Wizard.Model.IDataConnection.StoreConnectionNameOnly) when saving and restoring connections.

#### Name-Only Connections in the Designer

The following classes implement both services and open a modal designer. They share an application-supplied catalog of connection names and connection strings. The catalog is in memory only: retain or repopulate it before reopening saved reports, including after an application restart. No additional sample classes are required.

# [C#](#tab/csharp-designer-connections)
```csharp
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using DevExpress.DataAccess.ConnectionParameters;
using DevExpress.DataAccess.Sql;
using DevExpress.DataAccess.Wizard.Model;
using DevExpress.DataAccess.Wizard.Services;
using DevExpress.XtraReports.UI;

public class ConnectionProviderService : IConnectionProviderService {
    private readonly IDictionary<string, string> catalog;

    public ConnectionProviderService(IDictionary<string, string> catalog) {
        this.catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));
    }

    public SqlDataConnection LoadConnection(string connectionName) {
        if (string.IsNullOrWhiteSpace(connectionName))
            throw new ArgumentException("A connection name is required.", nameof(connectionName));
        string connectionString;
        if (!catalog.TryGetValue(connectionName, out connectionString))
            throw new InvalidOperationException("The requested connection is not available.");
        return new SqlDataConnection(connectionName,
            new CustomStringConnectionParameters(connectionString)) {
            StoreConnectionNameOnly = true
        };
    }
}

public class ConnectionStorageService : IConnectionStorageService {
    private readonly IDictionary<string, string> catalog;
    private readonly ConnectionProviderService provider;

    public ConnectionStorageService(IDictionary<string, string> catalog) {
        this.catalog = catalog ?? throw new ArgumentNullException(nameof(catalog));
        provider = new ConnectionProviderService(catalog);
    }

    public bool CanSaveConnection { get { return true; } }

    public bool Contains(string connectionName) {
        return !string.IsNullOrWhiteSpace(connectionName) && catalog.ContainsKey(connectionName);
    }

    public IEnumerable<SqlDataConnection> GetConnections() {
        return catalog.Keys.Select(provider.LoadConnection).ToArray();
    }

    public void SaveConnection(string connectionName, IDataConnection connection, bool saveCredentials) {
        if (string.IsNullOrWhiteSpace(connectionName))
            throw new ArgumentException("A connection name is required.", nameof(connectionName));
        if (connection == null)
            throw new ArgumentNullException(nameof(connection));
        connection.StoreConnectionNameOnly = true;
        catalog[connectionName] = connection.CreateConnectionString(!saveCredentials);
    }
}

public static class ConnectionDesigner {
    public static void Show(XtraReport report, IDictionary<string, string> catalog) {
        var provider = new ConnectionProviderService(catalog);
        var storage = new ConnectionStorageService(catalog);
        var tool = new ReportDesignTool(report);
        var controller = tool.DesignRibbonForm.DesignMdiController;
        ReplaceService(controller, typeof(IConnectionProviderService), provider);
        ReplaceService(controller, typeof(IConnectionStorageService), storage);
        controller.DesignPanelLoaded += (sender, args) => {
            ReplaceService(args.DesignerHost, typeof(IConnectionProviderService), provider);
            ReplaceService(args.DesignerHost, typeof(IConnectionStorageService), storage);
        };
        tool.ShowRibbonDesignerDialog();
    }

    private static void ReplaceService(IServiceContainer container, Type serviceType, object service) {
        if (container.GetService(serviceType) != null)
            container.RemoveService(serviceType);
        container.AddService(serviceType, service);
    }
}
```

# [VB.NET](#tab/vbnet-designer-connections)
```vb
Imports System
Imports System.Collections.Generic
Imports System.ComponentModel.Design
Imports System.Linq
Imports DevExpress.DataAccess.ConnectionParameters
Imports DevExpress.DataAccess.Sql
Imports DevExpress.DataAccess.Wizard.Model
Imports DevExpress.DataAccess.Wizard.Services
Imports DevExpress.XtraReports.UI

Public Class ConnectionProviderService
    Implements IConnectionProviderService

    Private ReadOnly catalog As IDictionary(Of String, String)

    Public Sub New(catalog As IDictionary(Of String, String))
        If catalog Is Nothing Then Throw New ArgumentNullException(NameOf(catalog))
        Me.catalog = catalog
    End Sub

    Public Function LoadConnection(connectionName As String) As SqlDataConnection Implements IConnectionProviderService.LoadConnection
        If String.IsNullOrWhiteSpace(connectionName) Then
            Throw New ArgumentException("A connection name is required.", NameOf(connectionName))
        End If
        Dim connectionString As String = Nothing
        If Not catalog.TryGetValue(connectionName, connectionString) Then
            Throw New InvalidOperationException("The requested connection is not available.")
        End If
        Return New SqlDataConnection(connectionName,
            New CustomStringConnectionParameters(connectionString)) With {
            .StoreConnectionNameOnly = True
        }
    End Function
End Class

Public Class ConnectionStorageService
    Implements IConnectionStorageService

    Private ReadOnly catalog As IDictionary(Of String, String)
    Private ReadOnly provider As ConnectionProviderService

    Public Sub New(catalog As IDictionary(Of String, String))
        If catalog Is Nothing Then Throw New ArgumentNullException(NameOf(catalog))
        Me.catalog = catalog
        provider = New ConnectionProviderService(catalog)
    End Sub

    Public ReadOnly Property CanSaveConnection As Boolean Implements IConnectionStorageService.CanSaveConnection
        Get
            Return True
        End Get
    End Property

    Public Function Contains(connectionName As String) As Boolean Implements IConnectionStorageService.Contains
        Return Not String.IsNullOrWhiteSpace(connectionName) AndAlso catalog.ContainsKey(connectionName)
    End Function

    Public Function GetConnections() As IEnumerable(Of SqlDataConnection) Implements IConnectionStorageService.GetConnections
        Return catalog.Keys.Select(AddressOf provider.LoadConnection).ToArray()
    End Function

    Public Sub SaveConnection(connectionName As String, connection As IDataConnection, saveCredentials As Boolean) Implements IConnectionStorageService.SaveConnection
        If String.IsNullOrWhiteSpace(connectionName) Then
            Throw New ArgumentException("A connection name is required.", NameOf(connectionName))
        End If
        If connection Is Nothing Then Throw New ArgumentNullException(NameOf(connection))
        connection.StoreConnectionNameOnly = True
        catalog(connectionName) = connection.CreateConnectionString(Not saveCredentials)
    End Sub
End Class

Public Module ConnectionDesigner
    Public Sub Show(report As XtraReport, catalog As IDictionary(Of String, String))
        Dim provider = New ConnectionProviderService(catalog)
        Dim storage = New ConnectionStorageService(catalog)
        Dim tool = New ReportDesignTool(report)
        Dim controller = tool.DesignRibbonForm.DesignMdiController
        ReplaceService(controller, GetType(IConnectionProviderService), provider)
        ReplaceService(controller, GetType(IConnectionStorageService), storage)
        AddHandler controller.DesignPanelLoaded,
            Sub(sender, args)
                ReplaceService(args.DesignerHost, GetType(IConnectionProviderService), provider)
                ReplaceService(args.DesignerHost, GetType(IConnectionStorageService), storage)
            End Sub
        tool.ShowRibbonDesignerDialog()
    End Sub

    Private Sub ReplaceService(container As IServiceContainer, serviceType As Type, service As Object)
        If container.GetService(serviceType) IsNot Nothing Then
            container.RemoveService(serviceType)
        End If
        container.AddService(serviceType, service)
    End Sub
End Module
```
***

With the imports above, add the following method to your WinForms form and call it from a button handler. Replace `localhost` and `Northwind` with your SQL Server and database. Windows authentication uses the current user's identity; no password is embedded in the code.

# [C#](#tab/csharp-designer-connections-usage)
```csharp
private void OpenConnectionDesigner() {
    var catalog = new Dictionary<string, string>(StringComparer.Ordinal);
    var storage = new ConnectionStorageService(catalog);
    var connection = new SqlDataConnection("ReportDb", new MsSqlConnectionParameters {
        ServerName = "localhost",
        DatabaseName = "Northwind",
        AuthorizationType = MsSqlAuthorizationType.Windows
    });
    storage.SaveConnection("ReportDb", connection, false);
    ConnectionDesigner.Show(new XtraReport(), catalog);
}
```

# [VB.NET](#tab/vbnet-designer-connections-usage)
```vb
Private Sub OpenConnectionDesigner()
    Dim catalog = New Dictionary(Of String, String)(StringComparer.Ordinal)
    Dim storage = New ConnectionStorageService(catalog)
    Dim connection = New SqlDataConnection("ReportDb", New MsSqlConnectionParameters With {
        .ServerName = "localhost",
        .DatabaseName = "Northwind",
        .AuthorizationType = MsSqlAuthorizationType.Windows
    })
    storage.SaveConnection("ReportDb", connection, False)
    ConnectionDesigner.Show(New XtraReport(), catalog)
End Sub
```
***

`saveCredentials` controls whether credentials are retained in the application-side catalog, not in the REPX. If it is `false`, `CreateConnectionString(true)` removes credentials; SQL-authenticated connections then need credentials supplied by your application at runtime. For production, replace the session-only catalog with protected application-side connection storage, enforce connection access permissions, and do not log connection strings. Setting the flag does not retroactively sanitize existing REPX files; load and resave them with name-only connections.

Source: [Save Only Connection Names with Reports](https://github.com/DevExpress-Examples/reporting-winforms-designer-connection-save-safe). The official example includes file-based persistence in `connections.xml`; this is demonstration storage, not a protected credential store. The complete example above does not require fetching those source files.

## Entity Framework Security

### Restrict Access to Unauthorized Assemblies

By default, the reporting application cannot load custom assemblies referenced by Entity Framework data sources. To allow the application to load a specific assembly, handle the `EFDataSource.BeforeLoadCustomAssemblyGlobal` event and set the `AllowLoading` property based on the requested assembly.

# [C#](#tab/csharp-ef-assembly)

```csharp
EFDataSource.BeforeLoadCustomAssemblyGlobal += (s, e) => {
    e.AllowLoading = IsTrustedAssembly(e.AssemblyPath, e.ContextName);
};

# [VB.NET](#tab/vbnet-ef-assembly)
```vb
AddHandler EFDataSource.BeforeLoadCustomAssemblyGlobal, Sub(s, e)
    e.AllowLoading = IsTrustedAssembly(e.AssemblyPath, e.ContextName)
End Sub
```
***
The `DevExpress.DataAccess.EntityFramework.BeforeLoadCustomAssemblyEventArgs.AssemblyPath` property specifies the path to the requested assembly. The `DevExpress.DataAccess.EntityFramework.BeforeLoadCustomAssemblyEventArgs.ContextName` property specifies the type to load from that assembly. Implement `IsTrustedAssembly` according to your application's security requirements and set `DevExpress.DataAccess.EntityFramework.BeforeLoadCustomAssemblyEventArgs.AllowLoading` to true only for assemblies that your application explicitly trusts.

### Entity Framework Connection String Security

An `EFDataSource` references a connection by name (`ConnectionName`). Resolve the actual connection at runtime instead of serializing credentials into the report layout.

# [C#](#tab/csharp-ef-connection)
```csharp
var efDataSource = new EFDataSource {
    ConnectionName = "MyDbContext"
};
```

# [VB.NET](#tab/vbnet-ef-connection)
```vb
Dim efDataSource = New EFDataSource With {
    .ConnectionName = "MyDbContext"
}
```
***

## Safe Deserialization

DevExpress Reporting blocks potentially unsafe custom types during deserialization. This commonly affects reports that contain an `ObjectDataSource` whose data source type is an application-defined class.

If the type is known and trusted, register it at application startup before a report that references the type is loaded:

# [C#](#tab/csharp-trusted-type)
```csharp
using DevExpress.Utils;

DeserializationSettings.RegisterTrustedClass(typeof(EmployeeDataSource));
```

# [VB.NET](#tab/vbnet-trusted-type)
```vb
Imports DevExpress.Utils

DeserializationSettings.RegisterTrustedClass(GetType(EmployeeDataSource))
```
***

Register only types that are controlled by your application. Do not register arbitrary types from untrusted report layouts.

A blocked type can otherwise cause a `NonTrustedTypeDeserializationException` when the report is loaded.

See [Reporting - Safe Deserialization](https://docs.devexpress.com/XtraReports/404485/common-information/reporting-security/safe-deserialization?p=netframework).

## Safe Report Loading

Source: [Ensure Safe Loading of Reports (WinForms)](https://docs.devexpress.com/XtraReports/119159/winforms-reporting/winforms-reporting-application-security/ensure-safe-loading-of-reports).

Disable loading potentially unsafe reports in the End-User Report Designer:

# [C#](#tab/csharp-safe-loading)
```csharp
DevExpress.XtraReports.Configuration.Settings.Default.UserDesignerOptions
    .ReportLoadingRestrictionLevel = DevExpress.XtraReports.UI.RestrictionLevel.Disable;
```

# [VB.NET](#tab/vbnet-safe-loading)
```vb
DevExpress.XtraReports.Configuration.Settings.Default.UserDesignerOptions.
    ReportLoadingRestrictionLevel = DevExpress.XtraReports.UI.RestrictionLevel.Disable
```
***

## Troubleshooting

| Symptom | Cause | Fix |
|---------|-------|-----|
| "Access Denied" when saving report | No write permission in file system or database | Verify directory permissions or database user privileges; check that `CanSetData()` returns `true` |
| Connection parameters appear in a REPX file | Connection details are serialized with the data source | Use a named connection and resolve credentials at runtime; when appropriate, set `DataConnectionBase.StoreConnectionNameOnly = true` |
| Custom SQL query rejected but looks valid | Default or custom validation rejects it | Review the validation handler and the query type |
| Query Editor is grayed out | `XRDesignMdiController.SqlWizardSettings.EnableCustomSql` is `false` | Enable it only when required and add validation |
| Custom assembly not loading (EF) | Assembly is not trusted | Review `BeforeLoadCustomAssemblyGlobal` handling |
| `NonTrustedTypeDeserializationException` when loading a report with ObjectDataSource | The custom data source type is not trusted for deserialization | Register the known application type with `DeserializationSettings.RegisterTrustedClass` before loading the report |
| Report loads without authorization | No application-specific report access check | Validate report identifiers and user permissions before loading |

